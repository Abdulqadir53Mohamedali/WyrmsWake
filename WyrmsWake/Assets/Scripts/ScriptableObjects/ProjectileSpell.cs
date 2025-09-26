using UnityEngine;

public class ProjectileSpell : Spell
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] LayerMask destroyOnLayers;


    protected override void Update()
    {
        base.Update(); // keep lifetime ticking

        //if (spellRigidBody != null)
        //{
        //    spellRigidBody.linearVelocity = transform.forward * speed;
        //}
    }

    public void Launch(Vector3 direction)
    {
        if (spellRigidBody != null)
        {
            spellRigidBody.linearVelocity = direction.normalized * speed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is in destroyOnLayers
        if (((1 << other.gameObject.layer) & destroyOnLayers) != 0)
        {
            //// Optionally apply damage
            //Enemy enemy = other.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damageAmount);
            //}
            Debug.Log("Enemy hit");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }


}
