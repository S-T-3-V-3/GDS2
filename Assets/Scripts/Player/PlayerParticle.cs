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
        try
        {

        
        foreach(LineRenderer line in lineRenderers)
        {
            line.startColor = color;
            line.endColor = color;
        }
        }
        catch
        {
            print("no line renderers");
        }
        foreach (ParticleSystem ps in particleSystems)
        {

            var main = ps.main;
            main.startColor = color;
            //ps.Emit(1);
        }
    }

    public void SetVector(Vector3 dir) {
        foreach(ParticleSystem ps in particleSystems)
        {
            ps.transform.rotation = Quaternion.FromToRotation(Vector3.forward,dir);

            if (ps.name != "Buzz") {
                ParticleSystem.MainModule main = ps.main;
                main.startSpeed = dir.magnitude;
            }
        }
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
            GameObject.Destroy(this.gameObject);
    }
}
