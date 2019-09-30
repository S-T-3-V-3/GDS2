using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPostDeathHandler : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float targetSize = 4f;
    public float expansionTime = 1f;
    public TeamID targetTeam;

    float elapsedTime = 0f;
    float startSize = 0f;

    void Start() {
        sphereCollider.radius = startSize;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime <= expansionTime) {
            float alpha = elapsedTime/expansionTime;
            sphereCollider.radius = alpha*targetSize;
        }
        else
            GameObject.Destroy(this.gameObject);
    }
}
