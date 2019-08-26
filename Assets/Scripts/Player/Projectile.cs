using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public GameObject projectileBody;
    public ProjectileEvent OnProjectileOverlap;

    PlayerController owningPlayer;
    GameObject owner;
    Vector3 movementDirection;
    bool initialized = false;
    float lifeTime;
    float moveSpeed;
    int damage;

    public void Init(PlayerController owningPlayer, Vector3 forwardVector, GunType gun)
    {
        this.owningPlayer = owningPlayer;
        this.owner = owningPlayer.model;
        this.lifeTime = gun.projectileLifetime;

        movementDirection = forwardVector;
        moveSpeed = gun.projectileSpeed;
        damage = gun.projectileDamage;

        projectileBody.transform.localScale = new Vector3(gun.projectileSize, gun.projectileSize, gun.projectileSize);

        if (OnProjectileOverlap == null)
            OnProjectileOverlap = new ProjectileEvent();
        OnProjectileOverlap.AddListener(OnCollision);

        initialized = true;
    }

    void FixedUpdate()
    {
        if (!initialized) return;
                
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0) {
            OnDestroy();
            return;
        }

        this.gameObject.transform.position += movementDirection * moveSpeed * Time.deltaTime;
    }

    void OnDestroy() {
        GameObject.Destroy(this.gameObject);
    }

    void OnCollision(GameObject other) {
        if (other == owner) return;

        if (other.transform.parent.GetComponent<PlayerController>() != null) {
            PlayerController hitPlayer = other.transform.parent.GetComponent<PlayerController>();

            hitPlayer.currentStats.TakeDamage(damage);
            
            if (hitPlayer.currentStats.health <= 0) {
                FindObjectOfType<GameManager>().OnPlayerKilled(owningPlayer, hitPlayer);
            }   
        }

        

        OnDestroy();
    }
}

public class ProjectileEvent : UnityEvent<GameObject>
{
}