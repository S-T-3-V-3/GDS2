using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI RoundTimer;
    public GameObject debugMenu;
    public GameObject joinMessage;
    public GameObject carouselMessage;
    public DebugHUD debugHUD;
    public ConnectedPlayers connectedPlayers;

    GameManager gameManager;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnScoreUpdated.AddListener(UpdateScore);
        gameManager.OnPlayersChanged.AddListener(UpdatePlayers);
        gameManager.sessionData.OnRoundPrepare.AddListener(OnRoundPrepare);
        gameManager.sessionData.OnCarouselBegin.AddListener(OnCarouselBegin);
        gameManager.sessionData.OnCarouselEnd.AddListener(OnCarouselEnd);
        
        debugMenu.SetActive(false);
    }

    void Update() {
        DebugUpdate();

        UpdateTimer();
    }

    public void Announcement(string message, float time, Color ? color = null) {
        Announcement newAnnouncement = GameObject.Instantiate(gameManager.announcementPrefab, this.transform).GetComponent<Announcement>();
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
        ScoreText.text = "";

        foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams) {
            if (teamScore.score > 0)
                ScoreText.text += teamScore.teamID + ": " + (int)teamScore.score + "\n";
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

    void OnRoundPrepare() {
        joinMessage.SetActive(false);
    }
    
    void OnCarouselEnd()
    {
        carouselMessage.SetActive(false);
    }

    void OnCarouselBegin()
    {
        carouselMessage.SetActive(true);
    }
}

[System.Serializable]
public class DebugHUD {
    public Text sessionStatusText;
    public Text roundStatusText;
}