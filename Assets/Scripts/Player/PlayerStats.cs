using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CurrentStats {
    // Player
    public float moveSpeed;
    public float acceleration;
    public int health;
    public int maxHealth;
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
        maxHealth = PlayerStatsBase.maxHealth;
        trailLength = PlayerStatsBase.trailLength;
        level = PlayerStatsBase.level;
        exp = PlayerStatsBase.exp;
        isAlive = false;

        OnDeath = new V3Event();
        OnTakeDamage = new UnityEvent();
    }

    public void Respawn() {
        health = maxHealth;
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
    public static int maxHealth = 100;
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
        return (hpPerLevel * (level - 1)) + PlayerStatsBase.maxHealth;
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

public class GameStats {
    List<Damage> damageDealt = new List<Damage>();
    List<Shot> shotsFired = new List<Shot>();
    List<PlayerPoints> pointsEarned = new List<PlayerPoints>();
    List<TileCapture> tilesCaptured = new List<TileCapture>();

    public PlayerStatistics GetPlayerStats(int playerID) {
        PlayerStatistics stats = new PlayerStatistics();

        stats.kills = damageDealt.Where(x => x.fromPlayerID == playerID && x.wasKill).Select(x => x.toPlayerID).ToList();
        stats.deaths = damageDealt.Where(x => x.toPlayerID == playerID && x.wasKill).Select(x => x.fromPlayerID).ToList();
        stats.numShots = shotsFired.Where(x => x.fromPlayerID == playerID).Count();
        stats.numHits = damageDealt.Where(x => x.fromPlayerID == playerID).Count();
        stats.damageDealt = damageDealt.Where(x => x.fromPlayerID == playerID).Select(x => x.amount).Sum();
        stats.accuracy = stats.numHits / stats.numShots;
        stats.pointsEarned = pointsEarned.Where(x => x.playerID == playerID).Select(x => x.points).Sum();
        stats.numTilesCaptured = tilesCaptured.Where(x => x.playerID == playerID).Count();

        return stats;
    }

    public void Init(List<int> playerIDs) {
        foreach (int i in playerIDs)
            AddPlayer(i);
    }

    void AddPlayer(int playerID) {
        PlayerPoints points = new PlayerPoints();
        points.playerID = playerID;
        pointsEarned.Add(points);
    }

    public void ShotFired(Shot shot) {
        shotsFired.Add(shot);
    }

    public void TookDamage(Damage damage) {
        damageDealt.Add(damage);
    }

    public void EarnedPoints(int playerID, int numPoints) {
        PlayerPoints player = pointsEarned.Where(x => x.playerID == playerID).First();
        player.points += numPoints;
    }

    public void TileCaptured(int playerID) {
        TileCapture tc = new TileCapture();
        tc.playerID = playerID;
        tilesCaptured.Add(tc);
    }
}

public struct Damage {
    public int fromPlayerID;
    public int toPlayerID;
    public GunType fromGun;
    public GunType toGun;
    public int round;
    public int amount;
    public bool wasKill;
}

public struct Shot {
    public int fromPlayerID;
    public GunType fromGun;
    public int round;
}

public struct PlayerPoints {
    public int playerID;
    public int points;
}

public struct TileCapture {
    public int playerID;
}

public struct PlayerStatistics {
    public List<int> kills;
    public List<int> deaths;
    public int numShots;
    public int numHits;
    public float accuracy;
    public int damageDealt;
    public int pointsEarned;
    public int numTilesCaptured;
}

public class V3Event : UnityEvent<Vector3>
{
}