using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AlphaImageFader : MonoBehaviour
{
    public UnityEvent OnFadeComplete;

    Image image;
    bool isActive = false;
    bool isFadingOut;
    float fadeTime;
    float elapsedTime;
    float targetAlpha;
    float startAlpha;

    void Awake() {
        OnFadeComplete = new UnityEvent();
    }

    public void Init(Image image, float targetAlpha, float fadeTime = 0.5f) {
        this.image = image;
        this.fadeTime = fadeTime;
        this.targetAlpha = targetAlpha;
        this.startAlpha = image.color.a;

        image.type = Image.Type.Sliced;

        this.transform.SetAsLastSibling();

        elapsedTime = 0f;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;
        if (image == null) Destroy(this);

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= fadeTime) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
            OnComplete();
        }
        else {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(startAlpha,targetAlpha,elapsedTime/fadeTime));
        }
    }

    void OnComplete() {
        OnFadeComplete.Invoke();
        Destroy(this);
    }
}