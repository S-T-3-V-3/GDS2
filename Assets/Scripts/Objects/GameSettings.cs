using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game Settings")]
public class GameSettings : ScriptableObject {
    public int numRounds {
        get {
            return roundMapSettings.Count;
        }
    }
    public float roundTime = 90f;
    [Space]
    public float respawnTime = 3f;
    public int maxLives = 10;
    [Space]
    public int baseTileValue = 1;
    public int specialTileValue = 10;
    public int pointsPerKill = 10;
    public float pointsPerInterval = 10;
    public float pointDistributionInterval = 2f;
    [Space]
    public float KOTHPoints = 1;
    public float KOTHInterval = 0.1f;
    [Space]
    public List<MapSettings> roundMapSettings;

    // Can explore options with alternate game modes including base stat modifiers
    //public List<StatModifiers> baseStatModifiers;
}
