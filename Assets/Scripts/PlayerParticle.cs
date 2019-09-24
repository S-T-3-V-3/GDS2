using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public ParticleSystem[] particleSystems;
    public float lifetime = 2f;

    float elapsedTime = 0f;

    public void SetColor(Color color)
    {
        foreach(LineRenderer line in lineRenderers)
        {
            line.startColor = color;
            line.endColor = color;
        }
        foreach(ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = color;
            ps.Emit(1);
        }
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
            GameObject.Destroy(this.gameObject);
    }
}
