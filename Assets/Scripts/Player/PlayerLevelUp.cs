using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLevelUp : MonoBehaviour
{
    GameManager gameManager;
    PlayerController playerController;
    GameObject upgradePopup;
    TextMeshPro upgradeTextMesh;
    UpgradeType upgradeType;
    int upgradesRemaining;

    void Awake() {
        gameManager = FindObjectOfType<GameManager>();
        playerController = GetComponent<PlayerController>();
        upgradeType = UpgradeType.NEITHER;
        upgradesRemaining = 1;

        upgradePopup = Instantiate(gameManager.UpgradePopupPrefab);
        upgradeTextMesh = upgradePopup.GetComponent<TextMeshPro>();
    }

    void Update() {
        upgradePopup.transform.position = playerController.playerModel.transform.position + new Vector3(0,1,0);

        if (upgradeType == UpgradeType.NEITHER) {
            upgradeTextMesh.text = "Player ← Upgrade → Gun";
        }
        else if (upgradeType == UpgradeType.PLAYER) {
            upgradeTextMesh.text = "Trail Length\n↑\nMove Speed ← Upgrade → Acceleration\n↓\nHealth";
        }
        else if (upgradeType == UpgradeType.GUN) {
            upgradeTextMesh.text = "No gun upgrades yet.";
        }
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
                Destroy(upgradePopup.gameObject);
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
                int newMaxHealth = StatCalculation.GetMaxHealth(playerController.skillPoints.health);
                playerController.currentStats.health += StatCalculation.GetMaxHealth(playerController.skillPoints.health) - playerController.currentStats.maxHealth;
                playerController.currentStats.maxHealth = newMaxHealth;
                gameManager.hud.UpdateHealth(playerController, playerController.currentStats.health, playerController.currentStats.maxHealth);
                break;
            case "north":
                playerController.skillPoints.trailLength += 1;
                playerController.currentStats.trailLength = StatCalculation.GetTrailLength(playerController.skillPoints.trailLength);
                Debug.Log($"Trail length: {playerController.skillPoints.trailLength}");
                break;
            default:
                break;
        }
        upgradeType = UpgradeType.NEITHER;
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
