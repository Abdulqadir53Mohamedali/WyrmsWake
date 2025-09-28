using UnityEngine;
public enum SpellType
{
    Projectile,
    InstantVFX
}
[CreateAssetMenu(fileName = "New Spell", menuName = "Scriptable Objects/SpellSO")]



public class SpellSO : ScriptableObject
{


    public float damageAmount = 10f;
    public float manaCost = 5f;
    public float lifetime = 2f;
    public float speed = 15f;
    public string element;
    public GameObject spellPrefab;
    public SpellType spellType;
    public string spellName;






    // more can be added 
    // Status effetcs 
    // Thumbnail
    // other magic elements 
}


