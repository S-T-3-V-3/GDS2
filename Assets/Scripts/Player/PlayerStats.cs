using UnityEngine.Events;
using UnityEngine;

public class Stats {
    // Player
    public float moveSpeed;
    public float acceleration;
    public int health;
    public bool isAlive;

    // Gun
    public float reloadSpeed;
    public float damage;
    public float projectileLifetime;

    // Events
    public V3Event OnDeath;
    public UnityEvent OnTakeDamage;

    // Functions
    public void Init() {
        moveSpeed = PlayerStatsBase.moveSpeed;
        acceleration = PlayerStatsBase.acceleration;
        health = PlayerStatsBase.health;
        isAlive = false;

        reloadSpeed = PlayerStatsBase.reloadSpeed;
        damage = PlayerStatsBase.damage;
        projectileLifetime = PlayerStatsBase.projectileLifetime;

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
    
    // Gun
    public int reloadSpeed = 1;
    public int damageModifier = 1;
    public int projectileLifetime = 1;
}

public class PlayerStatsBase {
    // Player
    public static float moveSpeed = 5f;
    public static float acceleration = 20f;
    public static int health = 100;

    // Gun
    public static float reloadSpeed = 0.5f;
    public static float damage = 10f;
    public static float projectileLifetime = 2f;
}

public static class StatCalculation {
    // TODO: Fill out stat formulas/multipliers
}

public class V3Event : UnityEvent<Vector3>
{
}