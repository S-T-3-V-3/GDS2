using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData : MonoBehaviour{
    public bool isStarted = false;
    public bool isGameLoaded = false;
    public RoundManager roundManager;
    public ScoreHandler score;
    public UnityEvent OnRoundPrepare;
    public UnityEvent OnRoundBegin;
    public UnityEvent OnRoundComplete;
    public UnityEvent OnCarouselBegin;
    public UnityEvent OnCarouselEnd;

    GameManager gameManager;

    void Awake() {
        OnRoundPrepare = new UnityEvent();
        OnRoundBegin = new UnityEvent();
        OnRoundComplete = new UnityEvent();
        OnCarouselBegin = new UnityEvent();
        OnCarouselEnd = new UnityEvent();

        gameManager = this.GetComponent<GameManager>();
        roundManager = this.gameObject.AddComponent<RoundManager>();
        score = new ScoreHandler(gameManager);   

    }

    public void Reset() {
        score.Reset();
        this.StopSession();
        isGameLoaded = false;
    }

    public void StartSession() {
        if (isStarted) return;
        
        score.Reset();
        isStarted = true;
        gameManager.OnSessionStart.Invoke();
    }

    public void StartRound() {
        roundManager.SetState<RoundCountdownState>();
    }

    public void StopRound() {
        foreach (PlayerController player in gameManager.currentPlayers) {
            if (player.teamID != TeamID.NONE)
                player.SetState<PlayerInactiveState>();
        }
    }

    public void StopSession() {
        isStarted = false;
    }
}