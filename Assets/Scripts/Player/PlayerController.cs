using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public TeamID teamID;
    public TMPro.TextMeshPro healthText;
    public StateManager playerState;
    public PlayerCharacter character;
    public GameObject model;
    public Light playerGlow;
    [Space]

    public Stats currentStats;
    public SkillPoints skillPoints;


    void Awake() {
        currentStats = new Stats();
        currentStats.Init();

        if (playerState == null)
            playerState = this.gameObject.AddComponent<StateManager>();

        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager.sessionData.isGameLoaded)
            this.OnGameLoaded();
        else
            gameManager.OnGameLoaded.AddListener(OnGameLoaded);
    }

    public void SetState<T>() where T : State {
        playerState.AddState<T>();
    }

    void OnGameLoaded() {
        playerState.AddState<CharacterSelectState>();
    }
}

public class PlayerActiveState : State
{
    PlayerController playerController;
    GameManager gameManager;
    List<GunComponent> equippedGuns;
    Vector2 currentMovement;
    Rigidbody rigidBody;

    float deadZoneRange = 0.17f;
    bool isShooting = false;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = GameObject.FindObjectOfType<GameManager>();

        playerController.model.SetActive(true);
        playerController.healthText.gameObject.SetActive(true);

        playerController.model.GetComponent<MeshRenderer>().material = gameManager.teamManager.GetTeam(playerController.teamID).playerMat;
        playerController.playerGlow.color = gameManager.teamManager.GetTeam(playerController.teamID).color;

        playerController.currentStats.Respawn();
        playerController.currentStats.OnDeath.AddListener(OnDeath);
        playerController.currentStats.OnTakeDamage.AddListener(OnDamaged);

        this.GetGuns();
        
        currentMovement = new Vector2(0,0);
        rigidBody = this.GetComponentInChildren<Rigidbody>(true);

        if (playerController.healthText == null) playerController.healthText = playerController.GetComponentInChildren<TMPro.TextMeshPro>();
        playerController.healthText.text = playerController.currentStats.health.ToString();

        
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
    }

    void GetGuns() {
        if (equippedGuns == null)
            equippedGuns = new List<GunComponent>();

        foreach (GunComponent gun in this.transform.GetComponentsInChildren<GunComponent>()) {
            equippedGuns.Add(gun);
            gun.gameObject.SetActive(true);
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
            playerController.model.transform.rotation = Quaternion.Euler(0,inputRotation,0);
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

    // Handles movement of the player
    void doMovement() {
        float dampSpeed = 3f;
        
        if (currentMovement.magnitude > deadZoneRange) {
            float x = currentMovement.x * PlayerStatsBase.acceleration;
            float z = currentMovement.y * PlayerStatsBase.acceleration;
            Vector3 newVelocity = rigidBody.velocity + new Vector3(x,0,z) * Time.deltaTime;

            // Get highest normalized value to cap movespeed for smooth acceleration/deceleration
            float dampingAlpha = Mathf.Max(currentMovement.magnitude,rigidBody.velocity.magnitude/PlayerStatsBase.moveSpeed);

            rigidBody.velocity = Vector3.ClampMagnitude(newVelocity,PlayerStatsBase.moveSpeed * dampingAlpha);
        }
        else {
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * dampSpeed);
        }
    }

    void OnDamaged() {
        playerController.healthText.text = playerController.currentStats.health.ToString();
        gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health);
    }

    void OnDeath() {
        playerController.SetState<PlayerDeathState>();
    }
}

public class CharacterSelectState : State
{
    PlayerController playerController;
    GameManager gameManager;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();

        foreach (GunComponent gun in this.transform.GetComponentsInChildren<GunComponent>()) {
            gun.gameObject.SetActive(false);
        }

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

    void SetCharacter(PlayerCharacter character) {
        
    }
}

public class BuffSelectState : State
{
    PlayerController playerController;
    GameManager gameManager;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();

        playerController.model.SetActive(false);
        playerController.currentStats.isAlive = false;
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

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update() {
        float dampSpeed = 3.0f;
        Rigidbody rigidBody = this.GetComponentInChildren<Rigidbody>(true);
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * dampSpeed);
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
        gameManager = FindObjectOfType<GameManager>();

        // TODO: Do explosion stuff then ->E
        playerController.model.SetActive(false);
        playerController.currentStats.isAlive = false;
        gameManager.OnNewCameraTarget.Invoke();
    }

    void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= respawnTime) {
            playerController.SetState<PlayerActiveState>();
            gameManager.SpawnPlayer(playerController);
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
        gameManager = FindObjectOfType<GameManager>();

        playerController.model.SetActive(false);
        playerController.currentStats.isAlive = false;
        gameManager.OnNewCameraTarget.Invoke();
    }
}