using UnityEngine;

[CreateAssetMenu(menuName="Gun")]
public class GunType : ScriptableObject {
    public string gunName;
    public string soundEffect;
    public GameObject projectilePrefab;
    public float reloadTime;
    public float projectileSize;
    public float projectileSpeed;
    public int projectileDamage;
    public float projectileLifetime;
}