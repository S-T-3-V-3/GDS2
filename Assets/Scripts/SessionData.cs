using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData : MonoBehaviour{
    public bool isStarted = false;
    public Round currentRound;
    public ScoreHandler score;
    public UnityEvent OnSessionStart;

    GameManager gameManager;

    void Awake() {
        gameManager = this.gameObject.GetComponent<GameManager>();
        score = new ScoreHandler(gameManager);
        currentRound = new Round();
    }

    public void StartSession() {
        if (currentRound == null)
            currentRound = new Round();

        currentRound.Reset();
        score.Reset();

        isStarted = true;
    }

    void Update() {
        if (!isStarted) return;

        currentRound.Update();
    }
}

public class Round {
    public int roundNumber = 0;
    public float elapsedTime = 0f;
    public bool isStarted = false;

    public void Reset() {
        elapsedTime = 0f;
    }

    public void Start() {
        isStarted = true;
    }

    public void Stop() {
        isStarted = false;
    }

    public void Update() {
        if (!isStarted) return;

        elapsedTime += Time.deltaTime;
    }

    public void OnRoundComplete() {
        Stop();
        roundNumber++;
    }
}