using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleAutoDestruct : MonoBehaviour
{
    public ParticleSystem ps;

    [HideInInspector]
    public UnityEvent OnParticleComplete;
    
    void Start() {
        if (ps == null)
            ps = this.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
    public void OnParticleSystemStopped() {
        OnParticleComplete.Invoke();
        GameObject.Destroy(this.gameObject);
    }
}
