using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public TeamID teamID;
    public PlayerModelConfig playerModelConfig;
    public TMPro.TextMeshPro healthText;
    public StateManager playerState;
    public GameObject playerModel;
    public Stats currentStats;
    public SkillPoints skillPoints;
    [Space]
    public UnityEvent OnPlayerSpawn;
    public UnityEvent<Vector3> OnPlayerDeath;
    public UnityEvent OnPlayerGainExp;
    public UnityEvent OnPlayerLevelUp;
    public Vector3 pawnPosition = new Vector3(0,0,0);
    public Vector3 deathForce;
    [Space]
    public GameManager gameManager;
    public bool hasPawn = false;

    void Awake() {
        currentStats = new Stats();
        currentStats.Init();

        skillPoints = new SkillPoints();

        OnPlayerDeath = new V3Event();
        OnPlayerDeath.AddListener(OnDeath);
        OnPlayerSpawn = new UnityEvent();
        OnPlayerSpawn.AddListener(OnSpawn);
        OnPlayerGainExp = new UnityEvent();
        OnPlayerGainExp.AddListener(OnGainExp);
        OnPlayerLevelUp = new UnityEvent();
        OnPlayerLevelUp.AddListener(OnLevelUp);

        if (playerState == null)
            playerState = this.gameObject.AddComponent<StateManager>();

        gameManager = FindObjectOfType<GameManager>();

        int modelNum = UnityEngine.Random.Range(0,3);
        playerModelConfig = gameManager.gameSettings.characterModels[modelNum];

        if (gameManager.sessionData.isGameLoaded)
            this.OnGameLoaded();
        else
            gameManager.OnGameLoaded.AddListener(OnGameLoaded);
    }

    public void CreateNewPawn() {
        playerModel = GameObject.Instantiate(playerModelConfig.model,gameManager.playerParent);

        hasPawn = true;

        playerModel.GetComponent<PlayerModelController>().owner = this;
        playerModel.GetComponent<PlayerModelController>().SetPlayerColor(gameManager.teamManager.GetTeam(teamID).color);
    }

    public void DestroyPawn()
    {
        if (playerModel != null) {
            GameObject.Destroy(playerModel);
            hasPawn = false;
            currentStats.isAlive = false;
        }
    }

    public void SetState<T>() where T : State {
        playerState.AddState<T>();
    }

    void OnGameLoaded() {
        playerState.AddState<CharacterSelectState>();
    }

    void OnSpawn() {
        PlayerParticle spawnFX = GameObject.Instantiate(gameManager.SpawnFXPrefab).GetComponent<PlayerParticle>();
        spawnFX.transform.position = playerModel.transform.position;
        spawnFX.SetColor(gameManager.teamManager.GetTeam(teamID).color);
    }

    void OnDeath(Vector3 force) {
        PlayerParticle deathFX = GameObject.Instantiate(gameManager.DeathFXPrefab).GetComponent<PlayerParticle>();
        deathFX.transform.position = playerModel.transform.position;
        deathFX.SetColor(gameManager.teamManager.GetTeam(teamID).color);
        deathFX.SetVector(force);
    }

    void OnGainExp() {
        //UI stuff
        Debug.Log($"Exp: {currentStats.exp}. Exp required for next level: {gameManager.gameSettings.expRequired[currentStats.level - 1]}");
    }

    void OnLevelUp() {
        //UI stuff
        Debug.Log($"Level: {currentStats.level}");
    }
}

public class PlayerActiveState : State
{
    PlayerController playerController;
    GameManager gameManager;
    List<GunComponent> equippedGuns;
    Vector2 currentMovement;
    Rigidbody rigidBody;
    PlayerLevelUp levelUpScript;

    float deadZoneRange = 0.17f;
    bool isShooting = false;
    float elapsedExpTime = 0f;
    float elapsedFaceTime = 0f;
    float faceCooldown = 1f;
    List<float> expRequired;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
        expRequired = gameManager.gameSettings.expRequired;

        if (playerController.playerModel == null)
            playerController.CreateNewPawn();

        gameManager.SpawnPlayer(playerController);

        // playerController.healthText.gameObject.SetActive(true);

        playerController.currentStats.Respawn();
        playerController.currentStats.OnDeath.AddListener(OnDeath);
        playerController.currentStats.OnTakeDamage.AddListener(OnDamaged);

        currentMovement = new Vector2(0,0);
        rigidBody = playerController.playerModel.GetComponent<Rigidbody>();

        this.GetGuns();

        //if (playerController.healthText == null) playerController.healthText = playerController.GetComponentInChildren<TMPro.TextMeshPro>();
        //playerController.healthText.text = playerController.currentStats.health.ToString();
    }

    void Update()
    {
        if (!gameManager.sessionData.roundManager.isStarted) return;

        if (isShooting) {
            foreach (GunComponent gun in equippedGuns) {
                gun.RequestShoot();
            }
        }
    }

    void FixedUpdate()
    {
        if (!gameManager.sessionData.roundManager.isStarted) return;

        doMovement();
        playerController.pawnPosition = playerController.playerModel.transform.position;

        ExpUpdate();
        LevelUpdate();

        if (elapsedFaceTime < faceCooldown) {
            elapsedFaceTime += Time.deltaTime;
        }
    }

    public void ExpUpdate() {
        elapsedExpTime += Time.deltaTime;

        if (elapsedExpTime >= gameManager.gameSettings.expGainInterval) {
            elapsedExpTime = 0f;
            if (playerController.currentStats.exp < expRequired[expRequired.Count-1]) {
                playerController.currentStats.exp += gameManager.gameSettings.expPerInterval;

                if  (playerController.currentStats.exp > expRequired[expRequired.Count - 1]) {
                    playerController.currentStats.exp = expRequired[expRequired.Count - 1];
                }
            }
            playerController.OnPlayerGainExp.Invoke();
        }
    }

    public void LevelUpdate() {
        if (playerController.currentStats.exp >= expRequired[playerController.currentStats.level-1] && playerController.currentStats.level < expRequired.Count) {
            playerController.currentStats.level += 1;

            if (levelUpScript == null) {
                levelUpScript = gameObject.AddComponent<PlayerLevelUp>();
            }
            else {
                levelUpScript.IncreaseUpgradesRemaining(1);
            }

            Debug.Log($"You levelled up. You have {levelUpScript.GetUpgradesRemaining()} upgrades remaining.");

            playerController.OnPlayerLevelUp.Invoke();
        }
    }

    void GetGuns() {
        if (equippedGuns == null)
            equippedGuns = new List<GunComponent>();

        foreach (GunComponent gun in playerController.playerModel.GetComponentsInChildren<GunComponent>()) {
            equippedGuns.Add(gun);
            gun.owner = playerController;
        }
    }

    // Fired upon *change* in movement input
    public void OnLeftStick(InputValue value) {
        currentMovement = value.Get<Vector2>();
    }

    // Fired upon change in rotation input
    // Note: if adding rotation speed, look at trying mathf.min of
    // current-input and current-360-input in positive rotational space
    public void OnRightStick(InputValue value) {
        Vector2 stickPosition = value.Get<Vector2>();

        if (stickPosition.magnitude > deadZoneRange) {
            float inputRotation = Mathf.Atan2(stickPosition.x,stickPosition.y) * Mathf.Rad2Deg;
            playerController.playerModel.transform.rotation = Quaternion.Euler(0,inputRotation,0);
        }
    }

    // Fired upon change in shoot input
    public void OnRightTrigger(InputValue value) {
        float shootValue = value.Get<float>();

        if(shootValue < deadZoneRange)
            isShooting = false;
        else
            isShooting = true;
    }

    public void OnRightBumper(InputValue value) {
        // Debug Inflict Damage
        // playerController.currentStats.TakeDamage(10);
    }

    public void OnFaceButtonWest() {
        if (levelUpScript != null && elapsedFaceTime > faceCooldown) {
            elapsedFaceTime = 0;
            levelUpScript.ChooseUpgrade("west");
            Debug.Log("West button pressed.");
        }
    }

    public void OnFaceButtonEast() {
        if (levelUpScript != null && elapsedFaceTime > faceCooldown) {
            elapsedFaceTime = 0;
            levelUpScript.ChooseUpgrade("east");
            Debug.Log("East button pressed.");
        }
    }

    public void OnFaceButtonSouth() {
        if (levelUpScript != null && elapsedFaceTime > faceCooldown) {
            elapsedFaceTime = 0;
            levelUpScript.ChooseUpgrade("south");
            Debug.Log("South button pressed.");
        }
    }

    public void OnFaceButtonNorth() {
        if (levelUpScript != null && elapsedFaceTime > faceCooldown) {
            elapsedFaceTime = 0;
            levelUpScript.ChooseUpgrade("north");
            Debug.Log("North button pressed.");
        }
    }

    // Handles movement of the player
    void doMovement() {
        float dampSpeed = 3f;

        if (currentMovement.magnitude > deadZoneRange) {
            float x = currentMovement.x * playerController.currentStats.acceleration;
            float z = currentMovement.y * playerController.currentStats.acceleration;
            Vector3 newVelocity = rigidBody.velocity + new Vector3(x,0,z) * Time.deltaTime;

            // Get highest normalized value to cap movespeed for smooth acceleration/deceleration
            float dampingAlpha = Mathf.Max(currentMovement.magnitude,rigidBody.velocity.magnitude/ playerController.currentStats.moveSpeed);

            rigidBody.velocity = Vector3.ClampMagnitude(newVelocity, playerController.currentStats.moveSpeed * dampingAlpha);
        }
        else {
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * dampSpeed);
        }
    }

    void OnDamaged() {
        //playerController.healthText.text = playerController.currentStats.health.ToString();
        gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health);
    }

    void OnDeath(Vector3 force) {
        playerController.OnPlayerDeath.Invoke(force);
        playerController.SetState<PlayerDeathState>();
    }

    void OnGainExp() {

    }

    void OnLevelUp() {

    }
}

public class CharacterSelectState : State
{
    PlayerController playerController;
    GameManager gameManager;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
        playerController.DestroyPawn();

        gameManager.teamManager.JoinTeam(playerController,playerController.teamID);
    }

    public void OnLeftStick(InputValue value) {
        Vector2 test = (Vector2)value.Get();
    }

    public void OnBumpers(InputValue value) {
        gameManager.teamManager.LeaveTeam(playerController,playerController.teamID);
        playerController.teamID += (int)value.Get<float>();

        if (playerController.teamID > TeamID.NONE)
            playerController.teamID = TeamID.BLUE;

        if (playerController.teamID < TeamID.BLUE)
            playerController.teamID = TeamID.NONE;

        gameManager.teamManager.JoinTeam(playerController,playerController.teamID);

        gameManager.OnPlayersChanged.Invoke();
    }

    public void OnStart(InputValue value) {
        if (playerController.teamID != TeamID.NONE)
            gameManager.StartNextRound();
    }
}

public class BuffSelectState : State
{
    PlayerController playerController;
    GameManager gameManager;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
        playerController.DestroyPawn();

        gameManager.OnNewCameraTarget.Invoke();
    }

    public void OnLeftStick(InputValue value) {
        //Vector2 test = (Vector2)value.Get();
    }

    public void OnBumpers(InputValue value) {
        //int test = value.Get<int>();
    }

    public void OnStart(InputValue value) {
        if (playerController.teamID != TeamID.NONE) {
            gameManager.StartNextRound();
        }
    }
}

public class PlayerInactiveState : State
{
    PlayerController playerController;
    GameManager gameManager;
    float dampSpeed = 3.0f;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
    }

    void Update() {
        if (playerController.hasPawn) {
            Rigidbody rigidBody = playerController.playerModel.GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * dampSpeed);
        }
    }
}

public class PlayerDeathState : State
{
    PlayerController playerController;
    GameManager gameManager;
    float respawnTime = 3f;
    float timeElapsed = 0f;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;

        playerController.DestroyPawn();

        gameManager.OnNewCameraTarget.Invoke();
    }

    void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= respawnTime) {
            playerController.SetState<PlayerActiveState>();
            gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health);
        }
    }
}

public class PlayerMenuState : State
{
    PlayerController playerController;
    GameManager gameManager;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;

        gameManager.OnNewCameraTarget.Invoke();
    }
}
