using UnityEngine;

public class TileSprite : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float transitionTime = 2f;
    public float delay = 1f;

    float elapsedTime = 0f;
    bool isChanging = false;
    Color currentColor, newColor;

    void Update()
    {
        if (GameManager.Instance.sessionData.isPaused) return;
        
        if (!isChanging) return;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= transitionTime+delay) {
            isChanging = false;
        }
        else  if (elapsedTime >= delay) {
            float alpha = (elapsedTime - delay) / transitionTime;
            sprite.color = Color.Lerp(currentColor,newColor,alpha);
        }               
    }

    public void DoColorChange(Color newColor) {
        isChanging = true;
        elapsedTime = 0f;
        this.newColor = newColor;
        currentColor = sprite.color;
    }
}
