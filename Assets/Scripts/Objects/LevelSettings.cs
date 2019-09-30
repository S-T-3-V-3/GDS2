using UnityEngine;

[CreateAssetMenu(menuName="Level Settings")]
public class LevelSettings : ScriptableObject {
    public int level;
    public UpgradeType upgradeType;

    public enum UpgradeType {
        player,
        gun,
        additional
    }

}