using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public GameObject TilePrefab;
    public GameObject CameraPrefab;
    public GameObject MainMenuPrefab;
    public GameObject AnnouncementPrefab;
    public GameObject SpawnFXPrefab;
    public GameObject DeathFXPrefab;
    public GameObject DeathColliderPrefab;
    public GameObject TextPopupPrefab;
    public GameObject UpgradePopupPrefab;

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
    public SoundManager soundManager;

    [Space]
    public GameObject KingOfTheHill;
    public List<PlayerController> currentPlayers;
    public List<Vector3> tilePositions;

    [Space]
    public UnityEvent OnGameLoaded;
    public UnityEvent OnSessionStart;
    public UnityEvent OnPlayersChanged;
    public UnityEvent OnNewWinningTeam;
    public UnityEvent OnNewCameraTarget;
    public UnityEvent OnScoreUpdated;
    public UnityEvent OnTilesChanged;
    public UnityEvent OnMapLoaded;
    [Space]
    
    MapManager mapManager;
    Camera mainCamera;

    void Awake() {
        if (GameManager.Instance != null)
            GameObject.Destroy(this.gameObject);
        else
            Instance = this;
        
        sessionData = this.gameObject.AddComponent<SessionData>();
        teamManager = this.gameObject.AddComponent<TeamManager>();
        
        mainCamera = GameObject.Instantiate(CameraPrefab).GetComponent<CameraController>().mainCamera;

        sessionData.isStarted = false;

        OnMapLoaded.AddListener(SpawnKoth); //Sam
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
        hud.FadeToBlack(1).AddListener(StartNextRound);
    }

    public void PauseGame(PlayerController caller) {
        CameraController cameraController = CameraController.Instance;
        sessionData.isPaused = !sessionData.isPaused;

        if (sessionData.isPaused) {
            cameraController.focusPlayer = caller;
            cameraController.SetState<CameraFocusState>();
            soundManager.Pause();
        }
        else {
            if (sessionData.roundManager.isStarted)
                cameraController.SetState<CameraActiveState>();
            else
                cameraController.SetState<CameraResetState>();
            
            soundManager.Resume();
        }

        foreach (PlayerController pc in currentPlayers) {
            pc.Pause();
        }

        hud.Pause(teamManager.GetTeamColor(caller.teamID), caller.playerID);

        List<Animator> animators = FindObjectsOfType<Animator>().ToList();
        if (sessionData.isPaused) {
            foreach (Animator a in animators) {
                a.speed = 0;
            }
        }
        else {
            foreach (Animator a in animators) {
                a.speed = 1;
            }
        }

        List<ParticleSystem> emitters = FindObjectsOfType<ParticleSystem>().ToList();
        if (sessionData.isPaused) {
            foreach (ParticleSystem ps in emitters) {
                ps.Pause();
            }
        }
        else {
            foreach (ParticleSystem ps in emitters) {
                ps.Play();
            }
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene("Master");
    }
    
    public void StartNextRound() {
        hud.FadeToBlack(1).AddListener(() => {
            GameObject.Destroy(hud.playerLobby);
            LoadMap();
        });

        OnMapLoaded.AddListener(() => {
            SpawnPlayers();

            sessionData.OnRoundPrepare.Invoke();

            hud.FadeFromBlack(1).AddListener(sessionData.StartRound);
        });
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
            if (player.isPlaying) {
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
        hud.Announcement($"<color=#{teamManager.GetTeamHexColor(killer.teamID)}>Player {killer.playerID}</color> killed <color=#{teamManager.GetTeamHexColor(killed.teamID)}>Player {killed.playerID}</color>!",3);

        PlayerPostDeathHandler deathHandler = GameObject.Instantiate(DeathColliderPrefab).GetComponent<PlayerPostDeathHandler>();
        deathHandler.transform.position = killed.pawnPosition;
        deathHandler.targetTeam = killer.teamID;
        deathHandler.owningPlayerID = killer.playerID;

        TextPopupHandler textPopup = GameObject.Instantiate(TextPopupPrefab).GetComponent<TextPopupHandler>();
        string textValue = "+" + gameSettings.pointsPerKill.ToString();
        textPopup.Init(killer.pawnPosition, textValue, teamManager.GetTeamColor(killer.teamID));
    }
 
    // Runs every time a player joins the game, will trigger session start if first player connected.
    public void OnPlayerJoined(PlayerInput newPlayer) {
        PlayerController newController = newPlayer.GetComponent<PlayerController>();

        newController.teamID = TeamID.BLUE;
        newController.ready = false;

        newController.transform.parent = playerParent;
        currentPlayers.Add(newController);

        foreach (PlayerController player in currentPlayers)
            player.playerID = currentPlayers.IndexOf(player) + 1;

        OnPlayersChanged.Invoke();
    }

    public void OnPlayerLeft(PlayerInput exitingPlayer) {
        currentPlayers.Remove(exitingPlayer.GetComponent<PlayerController>());
        teamManager.LeaveTeam(exitingPlayer.GetComponent<PlayerController>(), exitingPlayer.GetComponent<PlayerController>().teamID);

        OnNewCameraTarget.Invoke();

        foreach (PlayerController player in currentPlayers)
            player.playerID = currentPlayers.IndexOf(player) + 1;
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

    public void ReadyCheck()
    {
        if (sessionData.isComplete) {
            foreach (PlayerController pc in currentPlayers.Where(x => x.isPlaying == true))
                if (!pc.ready) return;

            RestartGame();
        }
        else {
            foreach (PlayerController pc in currentPlayers.Where(x => x.isPlaying == true))
                if (!pc.ready) return;

            StartNextRound();
        }
    }
}