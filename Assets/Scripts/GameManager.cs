using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject CameraPrefab;
    public GameObject MainMenuPrefab;
    public GameObject AnnouncementPrefab;
    public GameObject SpawnFXPrefab;
    public GameObject DeathFXPrefab;
    public GameObject DeathColliderPrefab;
    public GameObject TextPopupPrefab;

    public Material WallMaterial;
    public Transform playerParent;

    //Sam
    public GameObject KothPrefab;

    [Space]
    public GameSettings gameSettings;
    public TeamSettings teamSettings;
    public SessionData sessionData;
    public HUDManager hud;
    public TeamManager teamManager;

    [Space]
    public GameObject KingOfTheHill;
    public List<PlayerController> currentPlayers;
    public List<Vector3> tilePositions;

    [Space]
    public UnityEvent OnGameLoaded;
    public UnityEvent OnSessionStart;
    public UnityEvent OnPlayersChanged;
    public UnityEvent OnNewCameraTarget;
    public UnityEvent OnScoreUpdated;
    public UnityEvent OnTilesChanged;
    public UnityEvent OnMapLoaded;
    [Space]
    
    MapManager mapManager;
    Camera mainCamera;

    void Awake() {
        sessionData = this.gameObject.AddComponent<SessionData>();
        teamManager = this.gameObject.AddComponent<TeamManager>();
        
        mainCamera = GameObject.Instantiate(CameraPrefab).GetComponent<CameraController>().mainCamera;

        sessionData.isStarted = false;

        OnMapLoaded.AddListener(SpawnKoth); //Sam
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!sessionData.isStarted) return;
        
        if (!sessionData.roundManager.isStarted) return;
        sessionData.score.ScoreUpdate();
    }

    public void Reset() {
        tilePositions.Clear();

        ResetPlayers();
        StopGame();
        UnloadMap();

        sessionData.roundManager.Reset();
        sessionData.Reset();
        hud.Reset();
    }

    public void LoadGame() {
        sessionData.isGameLoaded = true;
        OnGameLoaded.Invoke();
    }

    public void StartGame() {
        if (sessionData.isStarted) return;
        
        StartNextRound();
    }

    public void StartNextRound() {
        OnMapLoaded.AddListener(SpawnPlayers);

        this.LoadMap();
        sessionData.StartRound();
    }

    public void ResetPlayers() {
        foreach (PlayerController player in currentPlayers) {
            if (player.teamID != TeamID.NONE) {
                player.SetState<PlayerMenuState>();
                player.teamID = TeamID.NONE;
            }       
        }
    }

    public void SpawnPlayers() {
        OnMapLoaded.RemoveListener(SpawnPlayers);

        foreach (PlayerController player in currentPlayers) {
            if (player.teamID != TeamID.NONE) {
                player.SetState<PlayerActiveState>();
            }       
        }

        OnNewCameraTarget.Invoke();
        OnPlayersChanged.Invoke();
    }

    public void StopGame() {
        if (!sessionData.isStarted) return;

        sessionData.StopSession();
    }

    public void LoadMap() {       
        if (mapManager != null)
            UnloadMap();

        mapManager = this.gameObject.AddComponent<MapManager>();
    }

    public void UnloadMap() {
        if (mapManager == null) return;
        mapManager.UnloadMap();
        DespawnKoth(); //Sam
    }

    public void OnPlayerKilled(PlayerController killer, PlayerController killed) {
        sessionData.score.PlayerKilled(killer.teamID);
        hud.Announcement(killer.teamID + " player killed " + killed.teamID + " player!",3, teamManager.GetTeam(killer.teamID).color);

        PlayerPostDeathHandler deathHandler = GameObject.Instantiate(DeathColliderPrefab).GetComponent<PlayerPostDeathHandler>();
        deathHandler.transform.position = killed.pawnPosition;
        deathHandler.targetTeam = killer.teamID;

        TextPopupHandler textPopup = GameObject.Instantiate(TextPopupPrefab).GetComponent<TextPopupHandler>();
        string textValue = "+" + gameSettings.pointsPerKill.ToString();
        textPopup.Init(killer.pawnPosition, textValue, teamManager.GetTeam(killer.teamID).color);
    }
 
    // Runs every time a player joins the game, will trigger session start if first player connected.
    public void OnPlayerJoined(PlayerInput newPlayer) {
        PlayerController newController = newPlayer.GetComponent<PlayerController>();
        newController.transform.parent = playerParent;
        currentPlayers.Add(newController);

        OnPlayersChanged.Invoke();
    }

    public void OnPlayerLeft(PlayerInput exitingPlayer) {
        currentPlayers.Remove(exitingPlayer.GetComponent<PlayerController>());
        teamManager.LeaveTeam(exitingPlayer.GetComponent<PlayerController>(), exitingPlayer.GetComponent<PlayerController>().teamID);

        OnNewCameraTarget.Invoke();
    }

    public MapSettings GetMapSettings() {
        return gameSettings.roundMapSettings[sessionData.roundManager.roundNumber];
    }

    // Set player spawn point, currently random spawn location
    public void SpawnPlayer(PlayerController player) {
        int index = Random.Range(0,tilePositions.Count-1);
        player.playerModel.transform.position = tilePositions[index] + new Vector3(0,1f,0);
        player.OnPlayerSpawn.Invoke();

        tilePositions.RemoveAt(index);
        OnPlayersChanged.Invoke();
        OnNewCameraTarget.Invoke();
    }

    public Vector3 GetRandomPosition() {
        int index = Random.Range(0,tilePositions.Count-1);
        return tilePositions[index];
    }

    IEnumerator ILoadScene(string scene) {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    //Sam
    public void SpawnKoth() {
        Vector3 targetPos = GetRandomPosition();
        targetPos.y = 0.7f;
        KingOfTheHill = GameObject.Instantiate(KothPrefab, targetPos, transform.rotation);
    }
    
    //Sam
    void DespawnKoth() {
        Destroy(KingOfTheHill);
    }
}