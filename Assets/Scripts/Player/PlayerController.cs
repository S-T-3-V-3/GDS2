using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public TeamID teamID = TeamID.NONE;
    public int playerID;
    public int playerWeaponSelection = 0;
    public int playerModelSelection = 0;
    public PlayerModelConfig playerModelConfig;
    public TMPro.TextMeshPro healthText;
    public StateManager playerState;
    public GameObject playerModel;
    public CurrentStats currentStats;
    public SkillPoints skillPoints;
    [Space]
    public UnityEvent OnPlayerSpawn;
    public UnityEvent<Vector3> OnPlayerDeath;
    public UnityEvent OnPlayerLevelUp;
    public Vector3 pawnPosition = new Vector3(0,0,0);
    public Vector3 deathForce;
    [Space]
    public GameManager gameManager;
    public bool hasPawn = false;
    public bool isPlaying = false;
    public bool ready = false;
    [Space]
    Vector3 pauseVeclocity;


    void Awake() {
        currentStats = new CurrentStats();
        currentStats.Init();

        skillPoints = new SkillPoints();

        OnPlayerDeath = new V3Event();
        OnPlayerDeath.AddListener(OnDeath);
        OnPlayerSpawn = new UnityEvent();
        OnPlayerSpawn.AddListener(OnSpawn);

        if (playerState == null)
            playerState = this.gameObject.AddComponent<StateManager>();

        if (FindObjectOfType<MainMenuController>() != null)
            SetState<PlayerMenuState>();

        gameManager = GameManager.Instance;

        if (gameManager.sessionData.isGameLoaded)
            this.OnGameLoaded();
        else
            gameManager.OnGameLoaded.AddListener(OnGameLoaded);
    }

    public void Pause() {
        if (hasPawn) {
            Rigidbody rb = playerModel.GetComponent<Rigidbody>();

            if (gameManager.sessionData.isPaused) {
                pauseVeclocity = rb.velocity;
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
            else {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.velocity = pauseVeclocity;
            }
        }
    }

    public void CreateNewPawn() {
        playerModelConfig = gameManager.gameSettings.characterModels[playerModelSelection];
        playerModel = GameObject.Instantiate(playerModelConfig.model,gameManager.playerParent);

        hasPawn = true;

        playerModel.GetComponent<PlayerModelController>().owner = this;
        playerModel.GetComponent<PlayerModelController>().SetPlayerColor(TeamManager.Instance.GetTeamColor(teamID));
        foreach (GunComponent gc in playerModel.GetComponent<PlayerModelController>().guns) {
            gc.gun = gameManager.gameSettings.guns[playerWeaponSelection];
        }

        SoundManager.Instance.Play("spawn");
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
        SetState<CharacterSelectState>();
    }

    void OnSpawn() {
        PlayerParticle spawnFX = GameObject.Instantiate(gameManager.SpawnFXPrefab).GetComponent<PlayerParticle>();
        spawnFX.transform.position = playerModel.transform.position;
        spawnFX.SetColor(TeamManager.Instance.GetTeamColor(teamID));
    }

    void OnDeath(Vector3 force) {
        PlayerParticle deathFX = GameObject.Instantiate(gameManager.DeathFXPrefab).GetComponent<PlayerParticle>();
        deathFX.transform.position = playerModel.transform.position;
        deathFX.SetColor(TeamManager.Instance.GetTeamColor(teamID));
        deathFX.SetVector(force);
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
        gameManager = playerController.gameManager;

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
        SoundManager.Instance.Play("weapon deploy");

        //if (playerController.healthText == null) playerController.healthText = playerController.GetComponentInChildren<TMPro.TextMeshPro>();
        //playerController.healthText.text = playerController.currentStats.health.ToString();
    }

    void Update()
    {
        if (gameManager.sessionData.isPaused) return;
        if (!gameManager.sessionData.roundManager.isStarted) return;

        if (isShooting) {
            foreach (GunComponent gun in equippedGuns) {
                gun.RequestShoot();
            }
        }
    }

    void FixedUpdate()
    {
        if (gameManager.sessionData.isPaused) return;
        if (!gameManager.sessionData.roundManager.isStarted) return;

        doMovement();
        playerController.pawnPosition = playerController.playerModel.transform.position;
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
        if (GameManager.Instance.sessionData.isPaused) {
            if (CameraController.Instance.focusPlayer == playerController)
                CameraController.Instance.PauseOffset(value.Get<Vector2>());
        }

        currentMovement = value.Get<Vector2>();
    }

    // Fired upon change in rotation input
    // Note: if adding rotation speed, look at trying mathf.min of
    // current-input and current-360-input in positive rotational space
    public void OnRightStick(InputValue value) {
        if (GameManager.Instance.sessionData.isPaused) {
            if (CameraController.Instance.focusPlayer == playerController)
                CameraController.Instance.PauseAngle(value.Get<Vector2>());
            
            return;
        }

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

    // Handles movement of the player
    void doMovement() {
        if (GameManager.Instance.sessionData.isPaused) return;

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
        if (GameManager.Instance.sessionData.isPaused) return;
        //playerController.healthText.text = playerController.currentStats.health.ToString();
        gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health, playerController.currentStats.maxHealth);
    }

    void OnDeath(Vector3 force) {
        playerController.OnPlayerDeath.Invoke(force);
        SoundManager.Instance.Play("death");
        playerController.SetState<PlayerDeathState>();
    }

    void OnStart() {
        gameManager.PauseGame(playerController);
    }
}

public class CharacterSelectState : State
{
    PlayerController playerController;
    GameManager gameManager;

    PlayerLobbyCard card;

    bool hasResetX = true;
    bool hasResetY = true;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = GameManager.Instance;
        playerController.DestroyPawn();

        playerController.ready = false;

        gameManager.teamManager.JoinTeam(playerController,playerController.teamID);
    }

    public void OnFaceButtonSouth() {
        if (card == null) {
            card = gameManager.hud.playerList.AddPlayer(playerController, CardType.LOBBY).GetComponent<PlayerLobbyCard>();
            playerController.isPlaying = true;
        }
        else
        {
            card.Confirm();
        }
    }

    public void OnFaceButtonEast() {
        if (!playerController.ready && card != null) {
            gameManager.hud.playerList.RemovePlayer(playerController);
            card = null;
            playerController.isPlaying = false;
        }
        /*
        else
        {
            card.Confirm();
        }
        */
    }

    void Up() {
        if (!playerController.ready && card != null)
        {
            card.Up();
        }
    }

    void Down() {
        if (!playerController.ready && card != null)
        {
            card.Down();
        }
    }

    void Left() {
        if (!playerController.ready && card != null)
        {
            card.Left();
        }
    }

    void Right() {
        if (!playerController.ready && card != null)
        {
            card.Right();
        }
    }

    public void OnLeftStick(InputValue value)
    {
        float forwardDeadZone = 0.6f;
        float resetDeadZone = 0.3f;

        Vector2 currentValue = value.Get<Vector2>();

        if (currentValue.x > forwardDeadZone) {
            if (hasResetX) {
                Right();
                hasResetX = false;
            }
        }
        else if (currentValue.x < -forwardDeadZone) {
            if (hasResetX) {
                Left();
                hasResetX = false;
            }
        }
        else if (currentValue.x < resetDeadZone && currentValue.x > -resetDeadZone) {
            hasResetX = true;
        }

        if (currentValue.y > forwardDeadZone) {
            if (hasResetY) {
                Up();
                hasResetY = false;
            }
        }
        else if (currentValue.y < -forwardDeadZone) {
            if (hasResetY) {
                Down();
                hasResetY = false;
            }
        }
        else if (currentValue.y < resetDeadZone && currentValue.y > -resetDeadZone) {
            hasResetY = true;
        }
    }

    public void OnUpArrow()
    {
        Up();
    }

    public void OnDownArrow()
    {
        Down();
    }
    
    public void OnLeftArrow() {
        Left();
    }

    public void OnRightArrow() {
        Right();
    }

    public void OnStart(InputValue value)
    {
        card.Ready();
    }
}

public class LoadoutState : State
{
    PlayerController playerController;
    GameManager gameManager;
    PlayerLoadoutCard card;

    bool hasResetX = true;
    bool hasResetY = true;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
        playerController.DestroyPawn();
        playerController.ready = false;
        
        if (playerController.isPlaying)
            card = gameManager.hud.playerList.AddPlayer(playerController, CardType.LOADOUT).GetComponent<PlayerLoadoutCard>();

        gameManager.OnNewCameraTarget.Invoke();
    }

    public new void EndState() {
        GameObject.Destroy(card.gameObject);
    }
        
    public void OnFaceButtonEast()
    {
        card.Confirm();
    }

    public void OnLeftStick(InputValue value)
    {
        float forwardDeadZone = 0.6f;
        float resetDeadZone = 0.3f;

        Vector2 currentValue = value.Get<Vector2>();

        if (currentValue.x > forwardDeadZone) {
            if (hasResetX) {
                card.Right();
                hasResetX = false;
            }
        }
        else if (currentValue.x < -forwardDeadZone) {
            if (hasResetX) {
                card.Left();
                hasResetX = false;
            }
        }
        else if (currentValue.x < resetDeadZone && currentValue.x > -resetDeadZone) {
            hasResetX = true;
        }

        if (currentValue.y > forwardDeadZone) {
            if (hasResetY) {
                card.Up();
                hasResetY = false;
            }
        }
        else if (currentValue.y < -forwardDeadZone) {
            if (hasResetY) {
                card.Down();
                hasResetY = false;
            }
        }
        else if (currentValue.y < resetDeadZone && currentValue.y > -resetDeadZone) {
            hasResetY = true;
        }
    }

    void Up() {
        if (!playerController.ready)
        {
            card.Up();
        }
    }

    void Down() {
        if (!playerController.ready)
        {
            card.Down();
        }
    }

    void Left() {
        if (!playerController.ready)
        {
            card.Left();
        }
    }

    void Right() {
        if (!playerController.ready)
        {
            card.Right();
        }
    }

    public void OnUpArrow()
    {
        Up();
    }

    public void OnDownArrow()
    {
        Down();
    }
    
    public void OnLeftArrow() {
        Left();
    }

    public void OnRightArrow() {
        Right();
    }

    public void OnStart(InputValue value)
    {
        card.Ready();
    }
}

public class EndGamePlayerState : State
{
    PlayerController playerController;
    GameManager gameManager;
    PlayerStatCard card;

    bool hasResetY = true;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        gameManager = playerController.gameManager;
        
        playerController.DestroyPawn();
        playerController.ready = false;

        card = gameManager.hud.playerList.AddPlayer(playerController, CardType.STAT).GetComponent<PlayerStatCard>();

        gameManager.OnNewCameraTarget.Invoke();
    }

    public new void EndState() {
        GameObject.Destroy(card.gameObject);
    }
        
    
    public void OnLeftStick(InputValue value)
    {
        float forwardDeadZone = 0.6f;
        float resetDeadZone = 0.3f;

        Vector2 currentValue = value.Get<Vector2>();

        if (currentValue.y > forwardDeadZone) {
            if (hasResetY) {
                Up();
                hasResetY = false;
            }
        }
        else if (currentValue.y < -forwardDeadZone) {
            if (hasResetY) {
                Down();
                hasResetY = false;
            }
        }
        else if (currentValue.y < resetDeadZone && currentValue.y > -resetDeadZone) {
            hasResetY = true;
        }
    }

    void Up() {
        if (!playerController.ready)
        {
            card.Up();
        }
    }

    void Down() {
        if (!playerController.ready)
        {
            card.Down();
        }
    }

    public void OnUpArrow()
    {
        Up();
    }

    public void OnDownArrow()
    {
        Down();
    }

    public void OnStart(InputValue value)
    {
        card.Ready();
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
        if (gameManager.sessionData.isPaused) return;

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
        if (gameManager.sessionData.isPaused) return;

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= respawnTime) {
            playerController.SetState<PlayerActiveState>();
            gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health, playerController.currentStats.maxHealth);
        }
    }
}

public class PlayerMenuState : State
{
    PlayerController playerController;
    GameManager gameManager;
    MainMenuController mainMenu;
    bool hasResetY = true;

    public override void BeginState()
    {
        playerController = this.GetComponent<PlayerController>();
        mainMenu = FindObjectOfType<MainMenuController>();
        gameManager = GameManager.Instance;
    }

    void OnDownArrow() {
        mainMenu.Next();
    }

    void OnUpArrow() {
        mainMenu.Prev();
    }

    void OnFaceButtonSouth() {
        mainMenu.Select();
    }

    public void OnLeftStick(InputValue value)
    {
        float forwardDeadZone = 0.6f;
        float resetDeadZone = 0.3f;

        Vector2 currentValue = value.Get<Vector2>();

        if (currentValue.y > forwardDeadZone) {
            if (hasResetY) {
                mainMenu.Prev();
                hasResetY = false;
            }
        }
        else if (currentValue.y < -forwardDeadZone) {
            if (hasResetY) {
                mainMenu.Next();
                hasResetY = false;
            }
        }
        else if (currentValue.y < resetDeadZone && currentValue.y > -resetDeadZone) {
            hasResetY = true;
        }
    }
}

public class ControllerMenu {
    public int primaryElement = 0;
    public int subElement {
        get {
            return subMenu[primaryElement].currentElement;
        }
    }
    List<ControllerList> subMenu = new List<ControllerList>();

    public void Init(int[] elements) {
        foreach (int x in elements) {
            ControllerList cl = new ControllerList();
            cl.maxElements = x;
            cl.currentElement = 0;

            subMenu.Add(cl);
        }
    }

    public void Clear() {
        primaryElement = 0;
        subMenu.Clear();
    }

    public void SetCurrent(int[] currentValues) {
        for (int i = 0; i < currentValues.Length; i++)
        {
            subMenu[i].currentElement = currentValues[i];
        }
    }

    public int Next() {
        if (this.primaryElement < this.subMenu.Count - 1) {
            this.primaryElement += 1;
        }
        else {
            this.primaryElement = 0;
        }

        return this.primaryElement;
    }

    public int Prev() {
        if (this.primaryElement > 0) {
            this.primaryElement -= 1;
        }
        else {
            this.primaryElement = this.subMenu.Count - 1;
        }

        return this.primaryElement;
    }

    public int SubNext() {
        return subMenu[primaryElement].Next();
    }

    public int SubPrev() {
        return subMenu[primaryElement].Prev();
    }
}

public class ControllerList {
    public int maxElements;
    public int currentElement;

    public int Next() {
        if (this.currentElement < this.maxElements - 1) {
            this.currentElement += 1;
        }
        else {
            this.currentElement = 0;
        }

        return this.currentElement;
    }

    public int Prev() {
        if (this.currentElement > 0) {
            this.currentElement -= 1;
        }
        else {
            this.currentElement = this.maxElements - 1;
        }

        return this.currentElement;
    }
}