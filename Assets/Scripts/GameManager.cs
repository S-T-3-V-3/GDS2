using System.Collections.Generic;
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
        sessionData = new SessionData();
        sessionData.isStarted = false;
        hud = FindObjectOfType<HUDManager>();
    }

    void StartGame() {
        sessionData.Init();
        sessionData.StartSession();
        foreach (HexTileManager currentTile in FindObjectsOfType<HexTileManager>()) {
            currentTile.Init(this);
            sessionData.score.AddTile();
        }
    }

    // Runs every time a player joins the game, will trigger session start if first player connected.
    public void OnPlayerJoined(PlayerInput newPlayer) {
        PlayerController newController = newPlayer.GetComponent<PlayerController>();

        if (!sessionData.isStarted) {
            StartGame();
        }

        // Remove on team implementation
        newController.teamID = currentPlayers.Count;
        sessionData.score.currentScores.Add(0);
        //

        currentPlayers.Add(newController);
        newController.SetGameManager(this);
        setPlayerSpawn(newController);

        OnPlayersChanged.Invoke();
    }

    // Remove player from current players, TODO: remove player object or handle reconnection
    public void OnPlayerLeft(PlayerInput exitingPlayer) {
        currentPlayers.Remove(exitingPlayer.GetComponent<PlayerController>());

        //exitingPlayer.GetComponent<PlayerController>().

        OnPlayersChanged.Invoke();
    }

    public GunType GetDefaultGun() {
        return availableGuns[0];
    }

    // Set player spawn point, currently random spawn location
    void setPlayerSpawn(PlayerController player) {
        player.gameObject.transform.position = spawnpoints[Random.Range(0,spawnpoints.Count)].transform.position;
    }
}