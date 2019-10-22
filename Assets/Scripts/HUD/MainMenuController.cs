using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public List<GameObject> menuOptions;
    public Color selectedColor;
    public Color hiddenColor;
    public float fadeTime = 0.2f;

    int currentSelection = 0;

    void Awake() {
        Show();
    }

    public void Next() {
        Hide();

        if (currentSelection < menuOptions.Count - 1) {
            currentSelection++;
        }
        else {
            currentSelection = 0;
        }

        Show();
    }

    public void Prev() {
        Hide();

        if (currentSelection > 0) {
            currentSelection--;
        }
        else {
            currentSelection = menuOptions.Count - 1;
        }

        Show();
    }

    public void Select() {
        switch (currentSelection) {
            case 0:
                GameManager.Instance.LoadGame();
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                Exit();
                break;
            default:
                break;
        }
    }

    void Exit() {
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.Exit(0);
        #endif
    }

    void Show() {
        ButtonOutlineLerp show = this.gameObject.AddComponent<ButtonOutlineLerp>();
        show.image = menuOptions[currentSelection].GetComponent<Image>();
        show.startColor = hiddenColor;
        show.targetColor = selectedColor;
        show.fadeTime = fadeTime;
        show.elapsedTime = 0f;
    }

    void Hide() {
        ButtonOutlineLerp hide = this.gameObject.AddComponent<ButtonOutlineLerp>();
        hide.image = menuOptions[currentSelection].GetComponent<Image>();
        hide.startColor = selectedColor;
        hide.targetColor = hiddenColor;
        hide.fadeTime = fadeTime;
        hide.elapsedTime = 0f;
    }
}

public class ButtonOutlineLerp : MonoBehaviour {
    public Image image;
    public Color startColor;
    public Color targetColor;
    public float elapsedTime = 0;
    public float fadeTime = 0.2f;

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < fadeTime)
            image.color = Color.Lerp(startColor,targetColor,elapsedTime / fadeTime);
        else {
            image.color = targetColor;
            Destroy(this);
        }
    }
}