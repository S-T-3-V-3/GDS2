using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setColor : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer[] lineRenderers;
    public ParticleSystem[] particleSystems;
    float count;
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateColors(Color color)
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
}
