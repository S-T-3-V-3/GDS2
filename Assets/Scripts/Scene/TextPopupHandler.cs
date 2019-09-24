using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopupHandler : MonoBehaviour
{
    public TextMeshPro textMesh;
    public Vector3 positionOffset;
    public int outlineSize = 5;
    public float lifetime = 1.5f;
    float timeElapsed = 0f;

    public void Init(Vector3 position, string value, Color color, float scaleRatio = 1) {
        this.transform.localScale *= scaleRatio;
        this.transform.position = position + positionOffset;
        textMesh.text = value;
        textMesh.color = color;

        int sizeOffset = Random.Range(-2,2);
        textMesh.fontSize += sizeOffset;

        float rotationOffset = Random.Range(-5f,5f);
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y + rotationOffset, this.transform.eulerAngles.z);
    }

    // TODO: Fade out then destroy
    void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= lifetime)
            GameObject.Destroy(this.gameObject);
    }
}
