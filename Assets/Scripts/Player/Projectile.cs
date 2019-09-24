using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public Light projectileLight;
    public GameObject projectileBody;
    public ProjectileEvent OnProjectileOverlap;
    public ParticleSystem[] particleSystems;

    GameManager gameManager;
    PlayerController owningPlayer;
    GameObject owner;
    Vector3 movementDirection;
    public bool initialized = false;
    public float lifeTime;
    float moveSpeed;
    int damage;

    public void Init(PlayerController owningPlayer, Vector3 forwardVector, GunType gun)
    {
        gameManager = FindObjectOfType<GameManager>();

        this.owningPlayer = owningPlayer;
        this.owner = owningPlayer.playerModel;
        this.lifeTime = gun.projectileLifetime;

        movementDirection = forwardVector;
        moveSpeed = gun.projectileSpeed;
        damage = gun.projectileDamage;

        projectileBody.transform.localScale = new Vector3(gun.projectileSize / 2, gun.projectileSize/2, gun.projectileSize / 2);

        projectileBody.GetComponent<MeshRenderer>().material.color = Color.white;

        projectileLight.color = gameManager.teamManager.GetTeam(owningPlayer.teamID).color;

        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = gameManager.teamManager.GetTeam(owningPlayer.teamID).color;
        }



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

        if (other.tag == "Player") {
            PlayerController hitPlayer = other.GetComponent<PlayerModelController>().owner;

            hitPlayer.currentStats.TakeDamage(damage, this.movementDirection * moveSpeed);
            
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