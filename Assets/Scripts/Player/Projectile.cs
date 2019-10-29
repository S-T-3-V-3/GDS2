using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public Light projectileLight;
    public GameObject projectileBody;
    public ProjectileEvent OnProjectileOverlap;
    public ParticleSystem[] particleSystems;
    public GameObject HitObjectFX;
    public GameObject TimeoutFX;

    GameManager gameManager;
    PlayerController owningPlayer;
    GameObject owner;
    Vector3 movementDirection;
    GunType firedFromGun;
    public bool initialized = false;
    public float lifeTime;
    float moveSpeed;
    int damage;

    public void Init(PlayerController owningPlayer, Vector3 forwardVector, GunType gun)
    {
        gameManager = GameManager.Instance;
        firedFromGun = gun;

        Shot shot = new Shot();
        shot.fromPlayerID = owningPlayer.playerID;
        shot.fromGun = gun;
        shot.round = gameManager.sessionData.roundManager.roundNumber;
        gameManager.sessionData.gameStats.ShotFired(shot);

        this.owningPlayer = owningPlayer;
        this.owner = owningPlayer.playerModel;
        this.lifeTime = gun.projectileLifetime;

        movementDirection = forwardVector;
        moveSpeed = gun.projectileSpeed;
        damage = gun.projectileDamage;

        projectileBody.transform.localScale = new Vector3(gun.projectileSize / 2, gun.projectileSize/2, gun.projectileSize / 2);

        projectileBody.GetComponent<MeshRenderer>().material.color = Color.white;

        projectileLight.color = TeamManager.Instance.GetTeamColor(owningPlayer.teamID);

        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = TeamManager.Instance.GetTeamColor(owningPlayer.teamID);
        }



        if (OnProjectileOverlap == null)
            OnProjectileOverlap = new ProjectileEvent();
        OnProjectileOverlap.AddListener(OnCollision);

        initialized = true;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.sessionData.isPaused) return;
        
        if (!initialized) return;
                
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0) {
            SpawnFX(false);
            GameObject.Destroy(this.gameObject);
            return;
        }

        this.gameObject.transform.position += movementDirection * moveSpeed * Time.deltaTime;
    }

    void SpawnFX(bool wasHit)
    {
        ParticleSystem fx;
        ParticleSystem.MainModule fxmain;

        if (wasHit) {
            fx = GameObject.Instantiate(HitObjectFX, transform.position, transform.rotation,gameManager.transform).GetComponent<ParticleSystem>();
            
            fxmain = fx.main;
            fxmain.startColor = TeamManager.Instance.GetTeamColor(owningPlayer.teamID);
        }
        else {
            fx = GameObject.Instantiate(TimeoutFX, transform.position, transform.rotation,gameManager.transform).GetComponent<ParticleSystem>();
            fx.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,movementDirection);

            fxmain = fx.main;
            fxmain.startSpeed = moveSpeed;
            fxmain.startColor = TeamManager.Instance.GetTeamColor(owningPlayer.teamID);
        }

        ParticleSystemRenderer fxr = fx.GetComponent<ParticleSystemRenderer>();
        fxr.material.SetColor("_TintColor",Color.white);
    }

    void OnCollision(GameObject other) {
        if (other == owner) return;

        if (other.tag == "Player") {
            PlayerController hitPlayer = other.GetComponent<PlayerModelController>().owner;

            hitPlayer.currentStats.TakeDamage(damage, this.movementDirection * moveSpeed);
            hitPlayer.playerModel.GetComponent<Rigidbody>().AddForce(firedFromGun.projectileSize * this.movementDirection.normalized * gameManager.gameSettings.baseKnockbackValue, ForceMode.Impulse);
            
            TextPopupHandler textPopup = GameObject.Instantiate(gameManager.TextPopupPrefab).GetComponent<TextPopupHandler>();
                string textValue = "-" + damage.ToString();
                textPopup.Init(this.transform.position, textValue, TeamManager.Instance.GetTeamColor(owningPlayer.teamID), 0.5f);
                textPopup.lifetime = 0.5f;
            
            Damage d = new Damage();
            d.fromPlayerID = owningPlayer.playerID;
            d.toPlayerID = hitPlayer.playerID;
            d.fromGun = firedFromGun;
            d.amount = damage;
            d.toGun = gameManager.gameSettings.guns[hitPlayer.playerWeaponSelection];
            d.wasKill = false;

            if (hitPlayer.currentStats.health <= 0) {
                FindObjectOfType<GameManager>().OnPlayerKilled(owningPlayer, hitPlayer);
                d.wasKill = true;
            }

            gameManager.sessionData.gameStats.TookDamage(d);
            gameManager.sessionData.gameStats.EarnedPoints(owningPlayer.playerID, gameManager.gameSettings.pointsPerKill);
        }

        SpawnFX(true);
        GameObject.Destroy(this.gameObject);
    }
}

public class ProjectileEvent : UnityEvent<GameObject>
{

}