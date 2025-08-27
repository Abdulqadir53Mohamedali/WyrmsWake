using Game.FSM;
using System.Runtime.CompilerServices;
using UnityEditor;
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
        public float runSpeed;
        public float currentSpeed;

        public Vector3 targetVel;

        [SerializeField] protected bool shouldFacemoveDirection;

        public Animator animator;
        public Rigidbody rb;
        public StateMachine stateMachine;

        public bool isGrounded;
        public bool isStrafeWalk = true;
        public bool isSprinting;

        [SerializeField] private Transform cameraTransform;

        public Vector2 movementInput;

        public InputActionReference movementActionReference;
        //public InputActionReference JumpActionReference;
        //public InputActionReference CrouchActionReference;
        public InputActionReference sprintActionReference;


        public LocomotionState locomotionState {get; private set;}
        public RunningState runningState {get; private set;}

        public void Awake()
        {
            health = baseStats.maxHealth;
            stamina = baseStats.maxStamina;
            walkSpeed = baseStats.walkSpeed;
            runSpeed = baseStats.runSpeed;
            currentSpeed = walkSpeed;



            animator = this.GetComponent<Animator>();
            rb = this.GetComponent<Rigidbody>();
            locomotionState = new LocomotionState(this, animator);
            runningState = new RunningState(this, animator);
            stateMachine = new StateMachine();

            //Any(locomotionState, new FuncPredicate(() => isStrafeWalk && !isSprinting));
            At(locomotionState,runningState,new FuncPredicate(() => isSprinting));
            At(runningState, locomotionState, new FuncPredicate(() =>  !isSprinting));

            stateMachine.SetState(locomotionState);

        }

        void At(IState from,IState to,IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState from,IPredicate condition) => stateMachine.AddAnyTransition(from,condition);
        public void OnEnable()
        {
            movementActionReference.action.Enable();
            sprintActionReference.action.Enable();
        }
        public void OnDisable()
        {
            movementActionReference.action.Disable();
            sprintActionReference.action.Disable();


        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            movementInput = movementActionReference.action.ReadValue<Vector2>();
            bool isSpritningHeld = sprintActionReference.action.IsPressed();
            // Sprinting = Shift held AND moving forward
            isSprinting = sprintActionReference.action.IsPressed() && movementInput.y > 0.1f;
            // If not sprinting, strafe-walk is true
            isStrafeWalk = !isSprinting;

            currentSpeed = isSprinting ? runSpeed : walkSpeed;

            stateMachine.Update();
        }
        public void Walking()
        {
            //if (isSprinting)
            //{
            //    currentSpeed = runSpeed;
            //}
            //if (isStrafeWalk)
            //{
            //    currentSpeed = walkSpeed;
            //}
     
    
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.Normalize();
            right.Normalize();

            forward.y = 0;
            right.y = 0;

            Vector3 moveDirection = (right * movementInput.x + forward * movementInput.y).normalized;

            
            targetVel = moveDirection * currentSpeed;
            rb.linearVelocity = new Vector3 (targetVel.x, rb.linearVelocity.y, targetVel.z);
            //rb.AddForce(moveDirection * walkSpeed, ForceMode.Acceleration);

            if ( isStrafeWalk && forward.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);

            }

            // should be used for sprinting
             if (isSprinting && moveDirection.sqrMagnitude > 0.001f)
            {
                Debug.Log("Hello");

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
            stateMachine.FixedUpdate(); 


        }
    }
}

