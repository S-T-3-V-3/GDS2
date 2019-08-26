using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject announcementPrefab;
    public Transform playerParent;
    [Space]
    public GameSettings gameSettings;
    public HUDManager hud;
    public List<PlayerController> currentPlayers;
    public List<GameObject> spawnpoints;
    public List<GunType> availableGuns;
    public List<Vector3> tilePositions;
    [Space]
    public SessionData sessionData;
    [Space]
    public UnityEvent OnSessionStart;
    public UnityEvent OnPlayersChanged;
    public UnityEvent OnNewCameraTarget;
    public UnityEvent OnScoreUpdated;
    public UnityEvent OnTilesChanged;
    public UnityEvent OnMapLoaded;

    MapManager mapManager;

    void Awake() {
        sessionData = this.gameObject.AddComponent<SessionData>();

        sessionData.isStarted = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!sessionData.isStarted) return;
        
        if (!sessionData.roundManager.roundIsStarted) return;
        sessionData.score.ScoreUpdate();
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

    public void SpawnPlayers() {
        OnMapLoaded.RemoveListener(SpawnPlayers);

        foreach (PlayerController player in currentPlayers) {
            if (player.teamID != TeamID.NONE) {
                this.SpawnPlayer(player);

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
    }

    public void OnPlayerKilled(PlayerController killer, PlayerController killed) {
        sessionData.score.PlayerKilled(killer.teamID);
        hud.Announcement(killer.teamID + " player killed " + killed.teamID + " player!",3, Color.red);
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
        GetTeamManager().LeaveTeam(exitingPlayer.GetComponent<PlayerController>(), exitingPlayer.GetComponent<PlayerController>().teamID);

        OnNewCameraTarget.Invoke();
    }

    public TeamManager GetTeamManager() {
        return this.GetComponent<TeamManager>();
    }

    public MapSettings GetMapSettings() {
        return gameSettings.roundMapSettings[sessionData.roundManager.roundNumber];
    }

    // Set player spawn point, currently random spawn location
    public void SpawnPlayer(PlayerController player) {
        int index = Random.Range(0,tilePositions.Count-1);

        player.gameObject.transform.position = tilePositions[index] + new Vector3(0,1.5f,0);
        tilePositions.RemoveAt(index);
        
        OnPlayersChanged.Invoke();
    }
}