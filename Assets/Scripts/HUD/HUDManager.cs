using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public GameObject PlayerScorecardPrefab;
    public GameObject LobbyCardPrefab;
    public GameObject LoadoutCardPrefab;
    public GameObject StatCardPrefab;
    public GameObject TeamScorecardPrefab;
    public GameObject PauseScreenPrefab;
    public GameObject PlayerLobbyPrefab;
    public GameObject EndGamePrefab;
    public GameObject IndividualStatPrefab;
    [Space]
    public GameManager gameManager;
    public TextMeshProUGUI RoundTimer;
    public GameObject lobby;
    public GameObject gameplay;
    public GameObject joinMessage;
    public GameObject playerLobby;
    public PlayerList playerList;

    public GameObject upperUI;
    public GameObject playersLayout;
    public GameObject scoresLayout;

    public List<PlayerScorecard> playerScorecards;
    public List<TeamScoreCard> teamScorecards;
    
    List<PlayerLoadoutCard> playerLoadoutCards;

    GameObject mainMenu;
    GameObject pauseScreen;
    Announcement currentAnnouncement;

    void Start() {
        gameManager.OnScoreUpdated.AddListener(UpdateScore);
        gameManager.OnGameLoaded.AddListener(OnGameLoaded);
        gameManager.sessionData.OnEndGame.AddListener(OnEndGame);
        gameManager.sessionData.OnRoundPrepare.AddListener(OnRoundPrepare);
        gameManager.sessionData.OnLoadoutBegin.AddListener(OnLoadoutBegin);
        gameManager.sessionData.OnLoadoutEnd.AddListener(OnLoadoutEnd);

        Reset();
    }

    void Update() {
        if (GameManager.Instance.sessionData.isPaused) return;
        
        if (gameManager.sessionData.roundManager.isStarted) {
            float maxTime = gameManager.gameSettings.roundTime;
            float elapsedTime = gameManager.sessionData.roundManager.elapsedTime;
            float remainingTime = maxTime - elapsedTime;

            int seconds = (int)remainingTime%60;
            int minutes = (int)remainingTime/60;

            string minuteString = minutes.ToString();
            string secondsString = seconds.ToString();

            if (minutes < 10)
                minuteString = "0" + minutes.ToString();
            if (seconds < 10)
                secondsString = "0" + secondsString.ToString();

            RoundTimer.text = minuteString + ":" + secondsString;
        }
    }

    public void Reset() {
        gameplay.SetActive(false);
        scoresLayout.SetActive(false);

        mainMenu = GameObject.Instantiate(gameManager.MainMenuPrefab,this.transform);
        SoundManager.Instance.PlayMusic("menu music");
    }

    public void Pause(Color color, int playerID) {
        if (gameManager.sessionData.isPaused) {
            if (pauseScreen != null)
                GameObject.Destroy(pauseScreen);
            pauseScreen = GameObject.Instantiate(PauseScreenPrefab,this.transform);
            pauseScreen.GetComponent<PauseScreen>().targetTextColor = color;

            try {
                pauseScreen.GetComponent<PauseScreen>().ShowStats(gameManager.sessionData.gameStats.GetPlayerStats(playerID));
            }
            catch {
                
            }
        }
    }

    public void Announcement(string message, float time, Color ? color = null) {
        if (currentAnnouncement != null)
            currentAnnouncement.ForceStop();

        currentAnnouncement = GameObject.Instantiate(gameManager.AnnouncementPrefab, this.transform).GetComponent<Announcement>();
        currentAnnouncement.Init(message, time, color ?? Color.white);
    }

    void UpdateScore() {
        scoresLayout.SetActive(true);
        foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams) {
            foreach (PlayerScorecard currentScorecard in playerScorecards)
            {
                if (currentScorecard.teamText.text == ("" + teamScore.teamID))
                {
                    currentScorecard.pointsText.text = "" + (int)teamScore.score + " pts";
                }
            }
            //Re-Order teamScorecards List
            teamScorecards.Sort((x, y) => x.score.CompareTo(y.score));
            teamScorecards.Reverse();       
            foreach (TeamScoreCard currentTeamCard in teamScorecards)
            {
                if (currentTeamCard.team.ID == (teamScore.teamID))
                {
                    currentTeamCard.score = teamScore.score;
                    currentTeamCard.pointsText.text = "" + (int)currentTeamCard.score/* + " pts" */;
                    currentTeamCard.scoreBar.uvRect = new Rect((1f - teamScore.score / gameManager.sessionData.score.GetTotalScore()), 0, 1, 1);
                    currentTeamCard.transform.SetSiblingIndex(teamScorecards.IndexOf(currentTeamCard));

                    //Debug.Log("Index of " + currentTeamCard.team.ID + " card= " + teamScorecards.IndexOf(currentTeamCard));
                    //Debug.Log("New Index of " + currentTeamCard.team.ID + " card= " + teamScorecards.IndexOf(currentTeamCard));
                }
            }

        }
    }

    public void UpdateHealth(PlayerController pc, int health, int maxHealth)
    {
        //TO:DO Possibly add HUD animation code here? 
        foreach (PlayerScorecard currentScorecard in playerScorecards)
        {
            if (currentScorecard.playerText.text == "Player " + pc.playerID)
            {
                currentScorecard.playerHPText.text = health.ToString();
                currentScorecard.playerHP.uvRect = new Rect((1f-(float)health/maxHealth), 0, 1, 1);
            }
        }
    }

    void OnGameLoaded() {
        GameObject.Destroy(mainMenu);
        playerLobby = GameObject.Instantiate(PlayerLobbyPrefab,lobby.transform);
        joinMessage.SetActive(true);
        playerList = playerLobby.GetComponentInChildren<PlayerList>();
    }

    void OnRoundPrepare() {
        gameplay.SetActive(true);
        joinMessage.SetActive(false);
        scoresLayout.SetActive(false);

        GameObject.Destroy(playerLobby);

        //Initialise PlayerScorecards
        if (playerScorecards == null)
            playerScorecards = new List<PlayerScorecard>();        
        foreach (PlayerScorecard ps in playerScorecards)
            GameObject.Destroy(ps.gameObject);

        playerScorecards.Clear();

        foreach (PlayerController player in gameManager.currentPlayers)
           playerScorecards.Add(CreatePlayerScorecard(player));

        //Initialise TeamScorecards
        if (teamScorecards == null)
            teamScorecards = new List<TeamScoreCard>();

        foreach (TeamScoreCard ts in teamScorecards)
            GameObject.Destroy(ts.gameObject);

        teamScorecards.Clear();

        foreach (PlayerController player in gameManager.currentPlayers) {
            if (teamScorecards.Where(x => x.team.ID == player.teamID).Count() == 0) 
                teamScorecards.Add(CreateTeamScorecard(player.playerID, player.teamID));
        }

        upperUI.SetActive(true);
    }

    void OnLoadoutBegin()
    {
        playerLobby = GameObject.Instantiate(PlayerLobbyPrefab,lobby.transform);
        playerList = playerLobby.GetComponentInChildren<PlayerList>();
    }

    void OnLoadoutEnd()
    {
        GameObject.Destroy(playerLobby);
    }

    void OnEndGame() {
        playerLobby = GameObject.Instantiate(EndGamePrefab,this.transform);
        playerList = playerLobby.GetComponentInChildren<PlayerList>();
        gameplay.SetActive(false);
    }

    public PlayerScorecard CreatePlayerScorecard(PlayerController pc)
    {
        PlayerScorecard newScorecard = GameObject.Instantiate(PlayerScorecardPrefab, playersLayout.transform).GetComponent<PlayerScorecard>();
        newScorecard.team = pc.teamID;
        newScorecard.model = pc.playerModelSelection;
        newScorecard.playerNumber = pc.playerID;
        newScorecard.Init();
        return newScorecard;
    }

    public TeamScoreCard CreateTeamScorecard(int playerNumber, TeamID playerTeam)
    {
        TeamScoreCard newScorecard = Instantiate(TeamScorecardPrefab, scoresLayout.transform).GetComponent<TeamScoreCard>();        
        newScorecard.team.ID = playerTeam;
        newScorecard.scoreBar.uvRect = new Rect(0, 0, 1, 1);
        newScorecard.scoreBar.color = TeamManager.Instance.GetTeamColor(playerTeam);
        newScorecard.transform.localScale = Vector3.one;
        //newScorecard.score = gameManager.sessionData.score.currentTeams.Find(x => x.teamID == playerTeam).score;
        newScorecard.score = 0;
        newScorecard.pointsText.text = newScorecard.score.ToString();

        return newScorecard;
    }
}

