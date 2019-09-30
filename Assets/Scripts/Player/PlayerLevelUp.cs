using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{

    PlayerController playerController;
    UpgradeType upgradeType;
    int upgradesRemaining;

    void Awake() {
        playerController = GetComponent<PlayerController>();
        upgradeType = UpgradeType.NEITHER;
        upgradesRemaining = 1;
    }

    public void ChooseUpgrade(string button) {
        if (upgradeType == UpgradeType.NEITHER) {
            ChooseUpgradeType(button);
        }
        else {
            if (upgradeType == UpgradeType.PLAYER) {
                ChoosePlayerUpgrade(button);
            }
            else if (upgradeType == UpgradeType.GUN) {
                ChooseGunUpgrade(button);
            }
            upgradesRemaining -= 1;
            Debug.Log($"Upgrades remaining: {upgradesRemaining}");
            if (upgradesRemaining == 0) {
                Destroy(this);
            }
            else {
                upgradeType = UpgradeType.NEITHER;
            }
        }
    }

    public void ChooseUpgradeType(string button) {
        switch (button) {
            case "west":
                upgradeType = UpgradeType.PLAYER;
                Debug.Log("You chose player upgrades.");
                break;
            case "east":
                upgradeType = UpgradeType.GUN;
                Debug.Log("You chose gun upgrades.");
                break;
            default:
                break;
        }
    }

    public void ChoosePlayerUpgrade(string button) {
        switch (button) {
            case "west":
                playerController.skillPoints.moveSpeed += 1;
                playerController.currentStats.moveSpeed = StatCalculation.GetMoveSpeed(playerController.skillPoints.moveSpeed);
                Debug.Log($"Move speed: {playerController.skillPoints.moveSpeed}");
                break;
            case "east":
                playerController.skillPoints.acceleration += 1;
                playerController.currentStats.acceleration = StatCalculation.GetAcceleration(playerController.skillPoints.acceleration);
                Debug.Log($"Acceleration: {playerController.skillPoints.acceleration}");
                break;
            case "south":
                playerController.skillPoints.health += 1;
                playerController.currentStats.health = StatCalculation.GetMaxHealth(playerController.skillPoints.health);
                Debug.Log($"Health: {playerController.skillPoints.health}");
                break;
            case "north":
                playerController.skillPoints.trailLength += 1;
                playerController.currentStats.trailLength = StatCalculation.GetTrailLength(playerController.skillPoints.trailLength);
                Debug.Log($"Trail length: {playerController.skillPoints.trailLength}");
                break;
            default:
                break;
        }
    }

    public void ChooseGunUpgrade(string button) {

    }

    public int GetUpgradesRemaining() {
        return upgradesRemaining;
    }

    public void IncreaseUpgradesRemaining(int amount) {
        upgradesRemaining += amount;
    }

    public enum UpgradeType {
        PLAYER,
        GUN,
        NEITHER
    }
}
