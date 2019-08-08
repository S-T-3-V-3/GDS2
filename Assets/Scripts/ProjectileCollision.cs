using UnityEngine.Events;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public Projectile owner;
    void OnTriggerEnter(Collider other) {
        if (other.isTrigger) return;
        owner.OnProjectileOverlap.Invoke(other.gameObject);
    }
}
