using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunComponent : MonoBehaviour
{
    public GunType gun;

    GameObject gunModel;
    GameManager gameManager;
    float reloadCooldown = 0f;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        gun = gameManager.GetDefaultGun();

        gunModel = GameObject.Instantiate(gun.gunPrefab,this.gameObject.transform);
        gunModel.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
    }

    void Update() {
        if (reloadCooldown > 0)
            reloadCooldown -= Time.deltaTime;
    }

    public void RequestShoot() {
        if (reloadCooldown > 0) return;

        DoShoot();
        reloadCooldown = gun.reloadTime;
    }

    void DoShoot() {
        Projectile newProjectile = GameObject.Instantiate(gun.projectilePrefab, this.transform.position, this.transform.rotation, gameManager.gameObject.transform).GetComponent<Projectile>();
        newProjectile.Init(this.transform.parent.gameObject, this.gameObject.transform.forward, gun);
    }
}

// Note: Look at creating class for projectiles
[System.Serializable]
public class GunType {
    public string gunName;
    public GameObject gunPrefab;
    public GameObject projectilePrefab;
    public float reloadTime;
    public float projectileSize;
    public float projectileSpeed;
    public int projectileDamage;
    public float projectileLifetime;
}