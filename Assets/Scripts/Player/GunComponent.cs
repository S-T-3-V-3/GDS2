using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunComponent : MonoBehaviour
{
    public GunType gun;
    public PlayerController owner;
    GameObject gunModel;
    float reloadCooldown = 0f;

    void Start() {
        gunModel = GameObject.Instantiate(gun.gunPrefab,this.gameObject.transform);
        gunModel.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
        gunModel.GetComponent<Renderer>().material = FindObjectOfType<GameManager>().teamManager.GetTeam(owner.teamID).playerMat;
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
        Projectile newProjectile = GameObject.Instantiate(gun.projectilePrefab, this.transform.position, this.transform.rotation).GetComponent<Projectile>();
        newProjectile.Init(owner, this.gameObject.transform.forward, gun);
    }
}