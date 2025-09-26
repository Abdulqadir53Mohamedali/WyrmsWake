using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.5f;


    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    

    [Header("Combat")]
    public float maxMana = 200f;




}
