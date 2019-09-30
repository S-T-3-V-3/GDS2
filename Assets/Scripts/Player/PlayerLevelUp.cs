using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{

    SkillPoints skillPoints;
    UpgradeType upgradeType = UpgradeType.NEITHER;
    int upgradesRemaining;

    void Awake() {
        skillPoints = gameObject.GetComponent<SkillPoints>();
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
            upgradeType = UpgradeType.NEITHER;
        }
    }

    public void ChooseUpgradeType(string button) {
        switch (button) {
            case "east":
                upgradeType = UpgradeType.PLAYER;
                break;
            case "west":
                upgradeType = UpgradeType.GUN;
                break;
            default:
                break;
        }
    }

    public void ChoosePlayerUpgrade(string button) {
        switch (button) {
            case "north":
                skillPoints.moveSpeed += 1;
                StatCalculation.GetMoveSpeed(skillPoints.moveSpeed);
                break;
            case "east":
                skillPoints.acceleration += 1;
                StatCalculation.GetAcceleration(skillPoints.acceleration);
                break;
            case "south":
                skillPoints.health += 1;
                StatCalculation.GetMaxHealth(skillPoints.health);
                break;
            case "west":
                skillPoints.trailLength += 1;
                StatCalculation.GetTrailLength(skillPoints.trailLength);
                break;
            default:
                break;
        }
    }

    public void ChooseGunUpgrade(string button) {

    }

    public void AddUpgrade() {
        upgradesRemaining += 1;
    }

    public enum UpgradeType {
        PLAYER,
        GUN,
        NEITHER
    }
}
