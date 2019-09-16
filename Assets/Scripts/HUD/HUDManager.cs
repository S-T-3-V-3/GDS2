using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public GameObject PlayerScorecardPrefab;
    public GameManager gameManager;
    public TextMeshProUGUI RoundTimer;
    public GameObject gameplay;
    public GameObject joinMessage;
    public GameObject carouselMessage;
    public GameObject playerLobby;
    public ConnectedPlayers connectedPlayers;

    public GameObject upperUI;
    public GameObject playersLayout;

    List<PlayerScorecard> playerScorecards;
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
        if (gameManager.sessionData.roundManager.roundIsStarted) {
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
        
        UpdatePlayers();

        mainMenu = GameObject.Instantiate(gameManager.MainMenuPrefab,this.transform);
    }

    public void Announcement(string message, float time, Color ? color = null) {
        Announcement newAnnouncement = GameObject.Instantiate(gameManager.AnnouncementPrefab, this.transform).GetComponent<Announcement>();
        newAnnouncement.Init(message, time, color ?? Color.white);
    }

    void UpdateScore() {
        foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams) {
            if (teamScore.score > 0)
                foreach(PlayerScorecard currentScorecard in playerScorecards)
                {
                    if(currentScorecard.teamText.text == ("" + teamScore.teamID))
                    {
                        currentScorecard.pointsText.text = "" + (int)teamScore.score + " pts";
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
                currentScorecard.playerHPText.text = "" + health.ToString();
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

        if (playerScorecards == null)
            playerScorecards = new List<PlayerScorecard>();
        
        foreach (PlayerScorecard ps in playerScorecards)
            GameObject.Destroy(ps.gameObject);

        foreach (PlayerController player in gameManager.currentPlayers)
           playerScorecards.Add(CreatePlayerScorecard((gameManager.currentPlayers.IndexOf(player) + 1), player.teamID));

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
        PlayerScorecard newScorecard = GameObject.Instantiate(PlayerScorecardPrefab, playersLayout.transform).GetComponent<PlayerScorecard>();
        newScorecard.playerText.text = "Player " + playerNumber;
        newScorecard.teamText.text = "" + playerTeam;
        newScorecard.teamText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        newScorecard.pointsText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        newScorecard.playerHP.uvRect = new Rect(0, 0, 1, 1);

        return newScorecard;
    }
}