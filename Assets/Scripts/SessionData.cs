using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData {
    public bool isStarted = false;
    public ScoreHandler score;
    public UnityEvent OnSessionStart;

    public SessionData(GameManager gameManager) {
        score = new ScoreHandler(gameManager);
    }

    public void StartSession() {
        isStarted = true;
        score.Reset();
    }
}
