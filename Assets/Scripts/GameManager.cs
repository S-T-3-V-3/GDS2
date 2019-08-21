using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.PlayerInput;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    [Space]
    public GameSettings gameSettings;
    public HUDManager hud;
    public List<PlayerController> currentPlayers;
    public List<GameObject> spawnpoints;
    public List<GunType> availableGuns;
    [Space]
    public SessionData sessionData;
    [Space]
    public UnityEvent OnPlayersChanged;
    public UnityEvent OnScoreUpdated;

    MapManager mapManager;

    void Awake() {
        sessionData = new SessionData(this);
        sessionData.isStarted = false;
        hud = FindObjectOfType<HUDManager>();

        StartGame();
    }

    void StartGame() {
        if (sessionData.isStarted) return;

        if (gameSettings.roundMapSettings.Count == 0) {
            gameSettings.roundMapSettings.Add(new MapSettings());
        }

        sessionData.StartSession();
    }

    public void LoadMap() {       
        if (mapManager != null)
            UnloadMap();

        mapManager = this.gameObject.AddComponent<MapManager>();
    }

    void UnloadMap() {
        if (mapManager == null) return;

        sessionData.score.Reset();
        mapManager.UnloadMap();
    }

    void Update() {
        if (!sessionData.isStarted) return;
            
        sessionData.score.Calculate();
    }

    // Runs every time a player joins the game, will trigger session start if first player connected.
    public void OnPlayerJoined(PlayerInput newPlayer) {
        PlayerController newController = newPlayer.GetComponent<PlayerController>();

        if (!sessionData.isStarted) {
            StartGame();
        }

        currentPlayers.Add(newController);
        newController.SetGameManager(this);
        SetPlayerSpawn(newController);

        // Add player to random team
        List<TeamID> teamList = GetTeamManager().currentTeams.Select(x => x.ID).Where(x => x != TeamID.NONE).ToList();

        TeamID randomTeam = teamList[Random.Range(0,teamList.Count-1)];
        GetTeamManager().JoinTeam(newController, randomTeam);
        newController.teamID = randomTeam;

        OnPlayersChanged.Invoke();
    }

    public void OnPlayerLeft(PlayerInput exitingPlayer) {
        currentPlayers.Remove(exitingPlayer.GetComponent<PlayerController>());
        GetTeamManager().LeaveTeam(exitingPlayer.GetComponent<PlayerController>(), exitingPlayer.GetComponent<PlayerController>().teamID);
        OnPlayersChanged.Invoke();
    }

    public TeamManager GetTeamManager() {
        return this.GetComponent<TeamManager>();
    }

    public GunType GetDefaultGun() {
        return availableGuns[0];
    }

    public MapSettings GetMapSettings() {
        return (gameSettings.roundMapSettings[sessionData.currentRound]);
    }

    // Set player spawn point, currently random spawn location
    void SetPlayerSpawn(PlayerController player) {
        player.gameObject.transform.position = spawnpoints[Random.Range(0,spawnpoints.Count)].transform.position;
    }
}

[System.Serializable]
public class GameSettings {
    public int numRounds = 4;
    public float roundTime = 90f;
    [Space]
    public int baseTileValue = 1;
    public int specialTileValue = 10;
    [Space]
    public List<MapSettings> roundMapSettings;
}