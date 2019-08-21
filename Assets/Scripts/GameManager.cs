using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.PlayerInput;

public class GameManager : MonoBehaviour
{
    public List<GameObject> spawnpoints;
    public List<PlayerController> currentPlayers;
    public List<GunType> availableGuns;

    public SessionData sessionData;
    public GameObject playerPrefab;

    public UnityEvent OnPlayersChanged;
    public UnityEvent OnScoreUpdated;
    public HUDManager hud;

    void Awake() {
        sessionData = new SessionData(this);
        sessionData.isStarted = false;
        hud = FindObjectOfType<HUDManager>();
    }

    void StartGame() {
        sessionData.StartSession();
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
        List<TeamID> teamList = new List<TeamID>();
        teamList.Add(TeamID.BLUE);
        teamList.Add(TeamID.RED);
        teamList.Add(TeamID.GREEN);
        teamList.Add(TeamID.ORANGE);
        teamList.Add(TeamID.YELLOW);
        TeamID randomTeam = teamList[Random.Range(0,teamList.Count-1)];
        GetTeamManager().currentTeams.Where(x => x.ID == randomTeam).First().AddPlayer(newController);
        newController.teamID = randomTeam;

        OnPlayersChanged.Invoke();
    }

    // Remove player from current players, TODO: remove player object or handle reconnection
    public void OnPlayerLeft(PlayerInput exitingPlayer) {
        currentPlayers.Remove(exitingPlayer.GetComponent<PlayerController>());

        //exitingPlayer.GetComponent<PlayerController>().

        OnPlayersChanged.Invoke();
    }

    public TeamManager GetTeamManager() {
        return this.GetComponent<TeamManager>();
    }

    public GunType GetDefaultGun() {
        return availableGuns[0];
    }

    // Set player spawn point, currently random spawn location
    void SetPlayerSpawn(PlayerController player) {
        player.gameObject.transform.position = spawnpoints[Random.Range(0,spawnpoints.Count)].transform.position;
    }
}