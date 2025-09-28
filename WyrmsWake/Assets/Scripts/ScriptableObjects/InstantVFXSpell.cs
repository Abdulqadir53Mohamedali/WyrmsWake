using UnityEngine.VFX;

using System.Collections;
using UnityEngine;


public class InstantVFXSpell : Spell
{
    [SerializeField] LayerMask damageLayers;
    private VisualEffect vfx;

    public override void Initialize(SpellSO data)
    {
        base.Initialize(data);
        // get the VFX component on this prefab
        vfx = GetComponent<VisualEffect>();
        if (vfx != null)
        {
            Debug.Log("Made it to VFX Play");

            vfx.Play();   
        }
        Debug.Log("Spawning VFX");
        StartCoroutine(DamageDelay());

        // Runs damage check immediately

    }

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(0.4f);
        DoDamage();
    }
    void OnDrawGizmosSelected()
    {
        Vector3 halfExtents = new Vector3(1.5f, 2f, 3.0f);
        Quaternion orientation = transform.rotation;

        // Push the center forward by half the Z size so it starts "in front" of the player
        Vector3 center = transform.position + orientation * new Vector3(0, 0, halfExtents.z);
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
    }

    public void DoDamage()
    {
        Debug.Log("Applying Damage to enemy");
        Vector3 halfExtents = new Vector3(1.5f, 2f, 3.0f);
        Quaternion orientation = transform.rotation;

        // Push the center forward by half the Z size so it starts "in front" of the player
        Vector3 center = transform.position + orientation * new Vector3(0, 0, halfExtents.z);
        StartCoroutine(DamageDelay());
        Debug.Log("Waited 0.4s");
        Collider[] hits = Physics.OverlapBox(center, halfExtents, orientation, damageLayers);
        foreach (Collider hit in hits)
        {
            //if (hit.TryGetComponent<Enemy>(out var enemy))
            //{
            //    enemy.TakeDamage(data.damageAmount);
            //}
            Debug.Log("enemy Hit");

        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the collided object is in destroyOnLayers
    //    if (((1 << other.gameObject.layer) & destroyOnLayers) != 0)
    //    {
    //        //// Optionally apply damage
    //        //Enemy enemy = other.GetComponent<Enemy>();
    //        //if (enemy != null)
    //        //{
    //        //    enemy.TakeDamage(damageAmount);
    //        //}
    //    }
    //}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
