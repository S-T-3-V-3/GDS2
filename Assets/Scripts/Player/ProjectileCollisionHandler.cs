using UnityEngine.Events;
using UnityEngine;

public class ProjectileCollisionHandler : MonoBehaviour
{
    public Projectile owner;
    void OnTriggerEnter(Collider other) {
        if (other.isTrigger) return;
        owner.OnProjectileOverlap.Invoke(other.gameObject);
    }
}
