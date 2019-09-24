using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunComponent : MonoBehaviour
{
    public GunType gun;
    public PlayerController owner;
    float reloadCooldown = 0f;

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
        Projectile newProjectile = GameObject.Instantiate(gun.projectilePrefab, this.transform.position, this.transform.rotation).GetComponent<Projectile>();
        newProjectile.Init(owner, this.gameObject.transform.forward, gun);
    }
}