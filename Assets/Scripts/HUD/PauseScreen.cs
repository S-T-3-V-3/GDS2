using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    public Color targetBackgroundColor;
    public Color targetTextColor;
    public Color fadeColor;
    public Image image;
    public TextMeshProUGUI text;
    public float transitionTime;

    float enterTimeElapsed = 0f;
    float exitTimeElapsed = 0f;
    GameManager gameManager;

    void Awake(){
        gameManager = GameManager.Instance;
    }
    
    void Update() {
        if (gameManager.sessionData.isPaused) {               
            if (enterTimeElapsed < transitionTime) {
                image.color = Color.Lerp(fadeColor,targetBackgroundColor, enterTimeElapsed/transitionTime);
                text.color = Color.Lerp(fadeColor,targetTextColor, enterTimeElapsed/transitionTime);
                enterTimeElapsed += Time.deltaTime;
            }
            else
                image.color = targetBackgroundColor;
                text.color = targetTextColor;
        }
        else {
            if (exitTimeElapsed < transitionTime) {
                image.color = Color.Lerp(targetBackgroundColor,fadeColor, exitTimeElapsed/transitionTime);
                text.color = Color.Lerp(targetTextColor,fadeColor, exitTimeElapsed/transitionTime);
                exitTimeElapsed += Time.deltaTime;
            }
            else {
                image.color = fadeColor;
                text.color = fadeColor;
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
