using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    GameManager gameManager;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnScoreUpdated.AddListener(UpdateScore);
    }

    public void UpdateScore() {
        ScoreText.text = gameManager.sessionData.score.scoreString;
    }
}
