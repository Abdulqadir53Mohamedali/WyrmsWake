using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerStats baseStats;

        public float health;
        public float stamina;
        public float walkSpeed;

        [SerializeField] protected bool shouldFacemoveDirection;

        Animator animator;
        Rigidbody rb;

        public bool isGrounded;

        [SerializeField] private Transform cameraTransform;

        public Vector2 movementInput;

        public InputActionReference movementActionReference;
        //public InputActionReference JumpActionReference;
        //public InputActionReference CrouchActionReference;
        //public InputActionReference SprintActionReference;


        public void Awake()
        {
            health = baseStats.maxHealth;
            stamina = baseStats.maxStamina;
            walkSpeed = baseStats.walkSpeed;



            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }

        public void OnEnable()
        {
            movementActionReference.action.Enable();
        }
        public void OnDisable()
        {
            movementActionReference.action.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            movementInput = movementActionReference.action.ReadValue<Vector2>();   
        }
        public void Walking()
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.Normalize();
            right.Normalize();

            forward.y = 0;
            right.y = 0;

            Vector3 moveDirection = (right * movementInput.x + forward * movementInput.y).normalized;


            Vector3 targetVel = moveDirection * walkSpeed;
            rb.linearVelocity = new Vector3 (targetVel.x, rb.linearVelocity.y, targetVel.z);
            //rb.AddForce(moveDirection * walkSpeed, ForceMode.Acceleration);

            if (shouldFacemoveDirection && moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);

            }
            else if (moveDirection.sqrMagnitude == 0)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
        void FixedUpdate()
        {
            Walking();


        }
    }
}

