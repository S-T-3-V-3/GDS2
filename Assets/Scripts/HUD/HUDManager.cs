using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public GameManager gameManager;
    //public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI RoundTimer;
    public GameObject gameplay;
    public GameObject mainMenu;
    public GameObject debugMenu;
    public GameObject joinMessage;
    public GameObject carouselMessage;
    public GameObject playerLobby;
    public DebugHUD debugHUD;
    public ConnectedPlayers connectedPlayers;

    public GameObject upperUI;
    public GameObject playersLayout;
    public GameObject playerHUDPrefab;
    List<PlayerHUDPrefab> currentPlayerHUDs;

    void Start() {
        gameManager.OnScoreUpdated.AddListener(UpdateScore);
        gameManager.OnPlayersChanged.AddListener(UpdatePlayers);
        gameManager.OnGameLoaded.AddListener(OnGameLoaded);
        gameManager.sessionData.OnRoundPrepare.AddListener(OnRoundPrepare);
        gameManager.sessionData.OnCarouselBegin.AddListener(OnCarouselBegin);
        gameManager.sessionData.OnCarouselEnd.AddListener(OnCarouselEnd);

        Reset();
        
        debugMenu.SetActive(false);
    }

    void Update() {
        DebugUpdate();

        UpdateTimer();
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

    void UpdateTimer() {
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

    void UpdateScore() {
        //ScoreText.text = "";

        foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams) {
            if (teamScore.score > 0)
                foreach(PlayerHUDPrefab php in currentPlayerHUDs)
                {
                    if(php.teamText.text == ("" + teamScore.teamID))
                    {
                        php.pointsText.text = "" + (int)teamScore.score + " pts";
                    }
                    
                }
                //ScoreText.text += teamScore.teamID + ": " + (int)teamScore.score + "\n";
        }
    }

    public void UpdateHealth(PlayerController pc, int health)
    {
        //TO:DO Possibly add HUD animation code here? 
        foreach (PlayerHUDPrefab php in currentPlayerHUDs)
        {
            if (php.playerText.text == "Player " + (gameManager.currentPlayers.IndexOf(pc) + 1))
            {
                php.playerHPText.text = "" + health.ToString();
                php.playerHP.uvRect = new Rect((1f-health/100f), 0, 1, 1); //100 for now, might need to change w/HP buffs
            }
        }
    }

    void UpdatePlayers() {
        connectedPlayers.UpdatePlayerList(gameManager.currentPlayers); 
    }

    void DebugUpdate() {
        if (Input.GetKeyDown(KeyCode.A)) {
            debugMenu.SetActive(!debugMenu.activeSelf);
        }

        if (debugMenu.activeSelf) {
            debugHUD.sessionStatusText.text = "Session Started: " + gameManager.sessionData.isStarted;

            if (gameManager.sessionData.roundManager != null)
                debugHUD.roundStatusText.text = "Round Number: " + gameManager.sessionData.roundManager.roundNumber + "\nRound Started: " + gameManager.sessionData.roundManager.roundIsStarted + "\nRound Time: " + gameManager.sessionData.roundManager.elapsedTime.ToString().Substring(0,4);
        }
    }

    void OnGameLoaded() {
        gameplay.SetActive(true);
        GameObject.Destroy(mainMenu);
    }

    void OnRoundPrepare() {
        joinMessage.SetActive(false);
        playerLobby.SetActive(false);
        connectedPlayers.gameObject.SetActive(false);

        if (currentPlayerHUDs == null)
            currentPlayerHUDs = new List<PlayerHUDPrefab>();

        foreach (PlayerController player in gameManager.currentPlayers)
        {
           currentPlayerHUDs.Add(CreatePlayerHUD((gameManager.currentPlayers.IndexOf(player) + 1), player.teamID));
            
        }
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

    public PlayerHUDPrefab CreatePlayerHUD(int playerNumber, TeamID playerTeam)
    {
        GameObject newHUDInstance = Instantiate(playerHUDPrefab, playersLayout.transform) as GameObject;
        PlayerHUDPrefab php = newHUDInstance.GetComponent<PlayerHUDPrefab>();
        php.playerText.text = "Player " + playerNumber;
        php.teamText.text = "" + playerTeam;
        php.teamText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        php.pointsText.color = gameManager.teamManager.GetTeam(playerTeam).color;
        php.playerHP.uvRect = new Rect(0, 0, 1, 1);

        //When/if we need to change player portraits -
        //php.playerPortrait.sprite = ;
        return php;
    }
}



[System.Serializable]
public class DebugHUD {
    public Text sessionStatusText;
    public Text roundStatusText;
}

