using UnityEngine.Events;
using UnityEngine;

public class Stats {
    // Player
    public float moveSpeed;
    public float acceleration;
    public int health;
    public int trailLength = 8;
    public int level;
    public float exp;
    public bool isAlive;

    // Events
    public V3Event OnDeath;
    public UnityEvent OnTakeDamage;

    // Functions
    public void Init() {
        moveSpeed = PlayerStatsBase.moveSpeed;
        acceleration = PlayerStatsBase.acceleration;
        health = PlayerStatsBase.health;
        trailLength = PlayerStatsBase.trailLength;
        level = PlayerStatsBase.level;
        exp = PlayerStatsBase.exp;
        isAlive = false;

        OnDeath = new V3Event();
        OnTakeDamage = new UnityEvent();
    }

    public void Respawn() {
        moveSpeed = PlayerStatsBase.moveSpeed;
        acceleration = PlayerStatsBase.acceleration;
        health = PlayerStatsBase.health;
        isAlive = true;
    }

    public void TakeDamage(int damage, Vector3 force) {
        health -= damage;

        OnTakeDamage.Invoke();

        if (health <= 0) {
            health = 0;
            OnDeath.Invoke(force);
        }
    }
}

public class SkillPoints
{
    // Player
    public int moveSpeed = 1;
    public int acceleration = 1;
    public int health = 1;
    public int trailLength = 1;
    
    // Gun
    public int reloadSpeed = 1;
    public int damageModifier = 1;
    public int projectileLifetime = 1;
    public int projectileSpeed = 1;
}

public class PlayerStatsBase {
    // Player
    public static float moveSpeed = 5f;
    public static float acceleration = 20f;
    public static int health = 100;
    public static int trailLength = 8;
    public static int level = 1;
    public static float exp = 0;
}

public static class StatCalculation {
    public static float GetMoveSpeed(int level) {
        float speedMultiplier = 0.2f;
        return (1 + speedMultiplier * (level - 1 )) * PlayerStatsBase.moveSpeed;
    }

    public static float GetAcceleration(int level) {
        float accelMultiplier = 0.2f;
        return (1 + accelMultiplier * (level - 1)) * PlayerStatsBase.acceleration;
    }

    public static int GetMaxHealth(int level) {
        int hpPerLevel = 20;
        return (hpPerLevel * (level - 1)) + PlayerStatsBase.health;
    }

    public static int GetTrailLength(int level) {
        int bonusTilesPerLevel = 2;
        return (bonusTilesPerLevel * level) + PlayerStatsBase.trailLength;
    }

    public static float GetReloadSpeed(float gunReloadSpeed, int level) {
        float reloadMultiplier = 0.2f;
        return 1/(1 + reloadMultiplier * (level - 1));
    }

    public static int GetDamageAmount(int gunDamage, int level) {
        float damageMultiplier = 0.2f;
        return Mathf.RoundToInt((1 + damageMultiplier * (level - 1)) * gunDamage);
    }

    public static float GetProjectileLifetime(float gunLifetime, int level) {
        float lifetimeMultiplier = 0.2f;
        return (1 + lifetimeMultiplier * (level - 1)) * gunLifetime;
    }

    public static float GetProjectileSpeed(float gunSpeed, int level) {
        float speedMultiplier = 0.2f;
        return (1 + speedMultiplier * (level - 1)) * gunSpeed;
    }
}

public class V3Event : UnityEvent<Vector3>
{
}