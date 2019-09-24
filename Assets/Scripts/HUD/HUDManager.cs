using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public GameObject PlayerScorecardPrefab;
    public GameObject TeamScorecardPrefab;
    public GameManager gameManager;
    public TextMeshProUGUI RoundTimer;
    public GameObject gameplay;
    public GameObject joinMessage;
    public GameObject carouselMessage;
    public GameObject playerLobby;
    public ConnectedPlayers connectedPlayers;

    public GameObject upperUI;
    public GameObject playersLayout;
    public GameObject scoresLayout;

    public List<PlayerScorecard> playerScorecards;
    public List<TeamScoreCard> teamScorecards;

    GameObject mainMenu;

    void Start() {
        gameManager.OnScoreUpdated.AddListener(UpdateScore);
        gameManager.OnPlayersChanged.AddListener(UpdatePlayers);
        gameManager.OnGameLoaded.AddListener(OnGameLoaded);
        gameManager.sessionData.OnRoundPrepare.AddListener(OnRoundPrepare);
        gameManager.sessionData.OnCarouselBegin.AddListener(OnCarouselBegin);
        gameManager.sessionData.OnCarouselEnd.AddListener(OnCarouselEnd);

        Reset();
    }

    void Update() {
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
        joinMessage.SetActive(true);
        playerLobby.SetActive(true);
        connectedPlayers.gameObject.SetActive(true);
        scoresLayout.SetActive(false);
        
        UpdatePlayers();

        mainMenu = GameObject.Instantiate(gameManager.MainMenuPrefab,this.transform);
    }

    public void Announcement(string message, float time, Color ? color = null) {
        Announcement newAnnouncement = GameObject.Instantiate(gameManager.AnnouncementPrefab, this.transform).GetComponent<Announcement>();
        newAnnouncement.Init(message, time, color ?? Color.white);
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

    public void UpdateHealth(PlayerController pc, int health)
    {
        //TO:DO Possibly add HUD animation code here? 
        foreach (PlayerScorecard currentScorecard in playerScorecards)
        {
            if (currentScorecard.playerText.text == "Player " + (gameManager.currentPlayers.IndexOf(pc) + 1))
            {
                currentScorecard.playerHPText.text = health.ToString();
                currentScorecard.playerHP.uvRect = new Rect((1f-health/100f), 0, 1, 1); //100 for now, might need to change w/HP buffs
            }
        }
    }


    void UpdatePlayers() {
        connectedPlayers.UpdatePlayerList(gameManager.currentPlayers); 
    }

    void OnGameLoaded() {
        gameplay.SetActive(true);
        GameObject.Destroy(mainMenu);
    }

    void OnRoundPrepare() {
        joinMessage.SetActive(false);
        playerLobby.SetActive(false);
        connectedPlayers.gameObject.SetActive(false);
        scoresLayout.SetActive(false);

        //Initialise PlayerScorecards
        if (playerScorecards == null)
            playerScorecards = new List<PlayerScorecard>();        
        foreach (PlayerScorecard ps in playerScorecards)
            GameObject.Destroy(ps.gameObject);
        playerScorecards.Clear();
        foreach (PlayerController player in gameManager.currentPlayers)
           playerScorecards.Add(CreatePlayerScorecard((gameManager.currentPlayers.IndexOf(player) + 1), player.teamID));

        //Initialise TeamScorecards
        if (teamScorecards == null)
            teamScorecards = new List<TeamScoreCard>();
        foreach (TeamScoreCard ts in teamScorecards)
            GameObject.Destroy(ts.gameObject);
        teamScorecards.Clear();
        foreach (PlayerController player in gameManager.currentPlayers)
            teamScorecards.Add(CreateTeamScorecard((gameManager.currentPlayers.IndexOf(player) + 1), player.teamID));

        upperUI.SetActive(true);
    }
    
    void OnCarouselEnd()
    {
        carouselMessage.SetActive(false);
    }

    void OnCarouselBegin()
    {
        carouselMessage.SetActive(true);
    }

    public PlayerScorecard CreatePlayerScorecard(int playerNumber, TeamID playerTeam)
    {
        PlayerScorecard newScorecard = Instantiate(PlayerScorecardPrefab, playersLayout.transform).GetComponent<PlayerScorecard>();
        newScorecard.playerText.text = "Player " + playerNumber;
        newScorecard.teamText.text = "" + playerTeam;
        newScorecard.teamText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        newScorecard.pointsText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        newScorecard.playerHP.uvRect = new Rect(0, 0, 1, 1);
        newScorecard.transform.localScale = Vector3.one;

        return newScorecard;
    }

    public TeamScoreCard CreateTeamScorecard(int playerNumber, TeamID playerTeam)
    {
        TeamScoreCard newScorecard = Instantiate(TeamScorecardPrefab, scoresLayout.transform).GetComponent<TeamScoreCard>();        
        newScorecard.team.ID = playerTeam;
        newScorecard.scoreBar.uvRect = new Rect(0, 0, 1, 1);
        newScorecard.scoreBar.color = gameManager.teamManager.GetTeam(playerTeam).color;
        newScorecard.transform.localScale = Vector3.one;
        //newScorecard.score = gameManager.sessionData.score.currentTeams.Find(x => x.teamID == playerTeam).score;
        newScorecard.score = 0;
        newScorecard.pointsText.text = newScorecard.score.ToString();

        return newScorecard;
    }
}

