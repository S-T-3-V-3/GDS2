using UnityEngine;
using TMPro;

public class Announcement : MonoBehaviour
{
    TextMeshProUGUI announcement;

    float maxLifeTime;
    float elapsedTime = 0f;

    public void Init(string displayMessage, float displayTime, Color ? color = null) {
        announcement = this.GetComponent<TextMeshProUGUI>();
        announcement.text = displayMessage;
        announcement.color = color ?? Color.white;
        maxLifeTime = displayTime;
    }

    public void ForceStop() {
        elapsedTime = maxLifeTime;
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > maxLifeTime)
            this.End();
    }

    void End() {
        this.GetComponent<Animator>().SetBool("OnComplete",true);
    }

    public void OnFadeComplete() {
        GameObject.Destroy(this.gameObject);
    }
}
