using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleEvents : MonoBehaviour
{
    public UnityEvent OnParticleBegin;
    public UnityEvent OnParticleComplete;
    ParticleSystem ps;

    void Start() {
        ps = this.GetComponent<ParticleSystem>();
    }

    // TODO: OnParticleBegin

    void OnParticleSystemStopped() {
        OnParticleComplete.Invoke();
        GameObject.Destroy(this.gameObject);
    }
}
