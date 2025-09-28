using Unity.VisualScripting;
using UnityEngine;

//[RequireComponent (typeof(Rigidbody))]
//[RequireComponent (typeof(Collider))]


public class Spell : MonoBehaviour
{


    public float damageAmount;
    public float manaCost;
    public float lifeTime;
    public float speed;
    public string element;
    public GameObject spellprefab;

    public float timer;

    public SpellSO spellSO;


    
    public Rigidbody spellRigidBody;
    public Collider spellCollider;


    protected virtual void Awake()
    {
        spellRigidBody = GetComponent<Rigidbody>();
        spellCollider = GetComponent<Collider>();


        if (spellSO != null)
        {
            ApplySOData();
        }
    }
    public virtual void Initialize(SpellSO so)
    {
        spellSO = so;
        ApplySOData();
    }

    private void ApplySOData()
    {
        damageAmount = spellSO.damageAmount;
        manaCost = spellSO.manaCost;
        lifeTime = spellSO.lifetime;
        speed = spellSO.speed;
        element = spellSO.element;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if(timer >= lifeTime)
        {
            Destroy(gameObject);
        }

    }




}
