using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 5.5f;
    public float runSpeed = 8.5f;

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaRegnPerSec = 8f;

    [Header("Combat")]
    public float lightDamage = 12f;
    public float HeavyDamage = 22f;



}
