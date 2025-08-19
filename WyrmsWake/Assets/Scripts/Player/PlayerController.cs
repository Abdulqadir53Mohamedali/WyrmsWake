using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;


public class PlayerController : MonoBehaviour
{
    public PlayerStats baseStats;

    public float Health;
    public float Stamina;

    Animator animator;
    Rigidbody rb;

    public bool isGrounded;

    [SerializeField] private Transform cameraTransform;

    public Vector2 movementInput;

    public InputActionReference movementActionReference;
    public InputActionReference JumpActionReference;
    public InputActionReference CrouchActionReference;
    public InputActionReference SprintActionReference;


    public void Awake()
    {
        Health = baseStats.maxHealth;
        Stamina = baseStats.maxStamina;

        

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * movementInput.x + right * movementInput.y).normalized;

        
    }
}
