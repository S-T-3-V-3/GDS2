using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData : MonoBehaviour{
    public bool isStarted = false;
    public bool isPaused = false;
    public bool isComplete = false;
    public bool isGameLoaded = false;
    public RoundManager roundManager;
    public ScoreHandler score;
    public UnityEvent OnRoundPrepare;
    public UnityEvent OnRoundBegin;
    public UnityEvent OnRoundComplete;
    public UnityEvent OnLoadoutBegin;
    public UnityEvent OnLoadoutEnd;
    public UnityEvent OnEndGame;

    GameManager gameManager;

    void Awake() {
        OnRoundPrepare = new UnityEvent();
        OnRoundBegin = new UnityEvent();
        OnRoundComplete = new UnityEvent();
        OnLoadoutBegin = new UnityEvent();
        OnLoadoutEnd = new UnityEvent();
        OnEndGame = new UnityEvent();

        gameManager = GameManager.Instance;
        roundManager = this.gameObject.AddComponent<RoundManager>();
        score = new ScoreHandler(gameManager); 
    }

    void Update() {
        if (isPaused) return;
        
        if (roundManager.isStarted)
            score.ScoreUpdate();
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
        score.ResetTileCount();

        SoundManager.Instance.PlayMusic($"round {roundManager.roundNumber+1}");
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