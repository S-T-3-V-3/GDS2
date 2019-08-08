using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SessionData {
    public bool isStarted = false;
    public Score score;
    public UnityEvent OnSessionStart;

    public void Init() {
        score = new Score();
        score.Tiles = new List<int>();
        score.currentScores = new List<int>();
    }

    public void StartSession() {
        isStarted = true;
    }
}
