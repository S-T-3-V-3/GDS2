using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int teamID;
    public Stats currentStats;
    public SkillPoints skillPoints;
    public TextMesh healthText; 

    List<GunComponent> equippedGuns;
    Vector2 currentMovement;
    GameManager gameManager;
    Rigidbody rigidBody;

    float deadZoneRange = 0.17f;
    bool isShooting = false;

    void Start() {
        if (equippedGuns == null)
            equippedGuns = new List<GunComponent>();

        foreach (GunComponent gun in this.transform.GetComponentsInChildren<GunComponent>()) {
            equippedGuns.Add(gun);
        }

        currentStats = new Stats();
        currentStats.Init();
        currentStats.OnDeath.AddListener(OnDeath);
        currentStats.OnTakeDamage.AddListener(OnDamaged);
            
        currentMovement = new Vector2(0,0);

        rigidBody = this.GetComponent<Rigidbody>();

        if (healthText == null) healthText = GetComponentInChildren<TextMesh>();
        healthText.text = currentStats.health.ToString();
    }

    void FixedUpdate() {
        doMovement();
    }

    void Update() {
        if (isShooting) {
            foreach (GunComponent gun in equippedGuns) {
                gun.RequestShoot();
            }
        }
    }

    // Called from game manager upon creation to avoid null references
    public void SetGameManager(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    // Fired upon *change* in movement input
    public void OnMovementReceived(InputAction.CallbackContext movementInput) {
        currentMovement = movementInput.ReadValue<Vector2>();
    }

    // Fired upon change in rotation input
    // Note: if adding rotation speed, look at trying mathf.min of
    // current-input and current-360-input in positive rotational space
    public void OnRotationReceived(InputAction.CallbackContext rotationInput) {
        Vector2 stickPosition = rotationInput.ReadValue<Vector2>();

        if (stickPosition.magnitude > deadZoneRange) {
            float inputRotation = Mathf.Atan2(stickPosition.x,stickPosition.y) * Mathf.Rad2Deg; 
            this.gameObject.transform.rotation = Quaternion.Euler(0,inputRotation,0);
        }
    }

    // Fired upon change in shoot input
    public void OnShootReceived(InputAction.CallbackContext shootInput) {
        float shootValue = shootInput.ReadValue<float>();

        if(shootValue < deadZoneRange)
            isShooting = false;
        else
            isShooting = true;
    }

    // Handles movement of the player
    void doMovement() {
        float dampSpeed = 3f;
        
        if (currentMovement.magnitude > deadZoneRange) {
            float x = currentMovement.x * PlayerStatsBase.acceleration;
            float z = currentMovement.y * PlayerStatsBase.acceleration;
            Vector3 newVelocity = rigidBody.velocity + new Vector3(x,0,z) * Time.deltaTime;

            // Get highest normalized value to cap movespeed for smooth acceleration/deceleration
            float dampingAlpha = Mathf.Max(currentMovement.magnitude,rigidBody.velocity.magnitude/PlayerStatsBase.moveSpeed);

            rigidBody.velocity = Vector3.ClampMagnitude(newVelocity,PlayerStatsBase.moveSpeed * dampingAlpha);
        }
        else {
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime * dampSpeed);
        }
    }

    void OnDamaged() {
        healthText.text = currentStats.health.ToString();
    }

    void OnDeath() {
        GameObject.Destroy(this.gameObject);
    }
}