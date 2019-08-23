using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public GameObject debugMenu;
    public DebugHUD debugHUD;

    GameManager gameManager;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnScoreUpdated.AddListener(UpdateScore);
        
        debugMenu.SetActive(false);
    }

    public void UpdateScore() {
        ScoreText.text = gameManager.sessionData.score.scoreString;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            debugMenu.SetActive(!debugMenu.activeSelf);
        }

        if (debugMenu.activeSelf) {
            debugHUD.sessionStatusText.text = "Session: " + gameManager.sessionData.isStarted;

            if (gameManager.sessionData.currentRound != null)
                debugHUD.roundStatusText.text = "Round: " + gameManager.sessionData.currentRound.roundNumber + " " + gameManager.sessionData.currentRound.isStarted;
            else
                debugHUD.roundStatusText.text = "Round: null";
        }
    }
}

[System.Serializable]
public class DebugHUD {
    public Text sessionStatusText;
    public Text roundStatusText;
}