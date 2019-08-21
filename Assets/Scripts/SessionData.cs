using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData {
    public bool isStarted = false;
    public int currentRound = 0;
    public ScoreHandler score;
    public UnityEvent OnSessionStart;

    public SessionData(GameManager gameManager) {
        score = new ScoreHandler(gameManager);
    }

    public void StartSession() {
        isStarted = true;
        currentRound = 0;
        score.Reset();
    }
}

public class Round {

}