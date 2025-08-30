using Game.Anim;
using Game.FSM;
using System.Collections;
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

        [Header("Basic Stats")]
        public float health;
        public float stamina;
        public float walkSpeed;
        public float runSpeed;
        public float currentSpeed;
        int gravity = 35;

        [Header("Vector & Rotational variables")]
        public Vector3 targetVel;
        private Vector3 deltaTargetPos;
        private Quaternion deltaTargetRot = Quaternion.identity;

        [Header("Dodge")]
        [SerializeField] AnimationCurve dodgeCurve;
        float dodgeTimer;
        public bool canControl = true;
        public bool isDodging;
        public float dodgeDistance = 10f; // tune with your curve



        // tracks how fast player moved up or down. Gravity decreases it over time
        // clamping prevents unrelaistic fall speeds . ensures smooth falls and jumps
        float velocityY;


        [SerializeField] protected bool shouldFacemoveDirection;

        public Animator animator;
        public Rigidbody rb;
        public StateMachine stateMachine;

        [Header("Boolean Conditions")]
        public bool isGrounded;
        public bool isStrafeWalk = true;
        public bool isSprinting;
        public bool useRootMotion;
        public bool rollRequested;

        [SerializeField] private Transform cameraTransform;

        public Vector2 movementInput;

        [Header("Input Actions")]
        public InputActionReference movementActionReference;
        //public InputActionReference JumpActionReference;
        //public InputActionReference CrouchActionReference;
        public InputActionReference sprintActionReference;
        public InputActionReference rollActionReference;


        [Header("Player States")]
        public LocomotionState locomotionState {get; private set;}
        public RunningState runningState {get; private set;}
        public WalkingRollState walkRollState {get; private set;}

        // Put this helper in your PlayerController class
        float SampleCurveArea(AnimationCurve curve, float duration, int steps = 60)
        {
            float area = 0f;
            float dt = duration / steps;
            float t = 0f;
            for (int i = 0; i < steps; i++)
            {
                // trapezoid integration for smoothness
                float a = curve.Evaluate(t);
                float b = curve.Evaluate(t + dt);
                area += (a + b) * 0.5f * dt;
                t += dt;
            }
            return Mathf.Max(0.0001f, area);
        }

        IEnumerator Dodge()
        {
            canControl = false;
            isDodging = true;
            float timer = 0f;
            //float prevAlpha = 0f;
            float duration = dodgeCurve.keys[dodgeCurve.length - 1].time;

            Vector3 f = cameraTransform.forward; f.y = 0; f.Normalize();
            Vector3 r = cameraTransform.right; r.y = 0; r.Normalize();
            Vector3 m = (r * movementInput.x + f * movementInput.y);
            Vector3 dir = (m.sqrMagnitude > 0.001f) ? m.normalized : transform.forward;

            // Face roll direction & play anim
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            animator.CrossFade(PlayerAnimIds.walkingRoll, 0.05f, 0);

            //  Normalize curve so total distance == dodgeDistance
            float area = SampleCurveArea(dodgeCurve, duration); // seconds * multiplier
            float speedScale = dodgeDistance / area;            // meters / (multiplied seconds)

            while (timer < duration)
            {
                // speed(t) = curve(t) * speedScale
                float horizSpeed = dodgeCurve.Evaluate(timer) * speedScale; // m/s
                Vector3 v = dir * horizSpeed;
                v.y = rb.linearVelocity.y; // preserve gravity
                rb.linearVelocity = v;

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            canControl = true;
            isDodging = false;
            stateMachine.SetState(isSprinting ? runningState : locomotionState); 

        }
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
            walkRollState = new WalkingRollState(this, animator);
            stateMachine = new StateMachine();

            //Any(locomotionState, new FuncPredicate(() => isStrafeWalk && !isSprinting));
            //At(walkRollState, locomotionState, new FuncPredicate(() => !rollRequested && isStrafeWalk));
            //At(walkRollState, runningState, new FuncPredicate(() => !rollRequested && !isStrafeWalk && isSprinting));
            //At(locomotionState, walkRollState, new FuncPredicate(() => useRootMotion && !isSprinting));
            //Any(walkRollState, new FuncPredicate(() => rollRequested && !isSprinting));
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
            rollActionReference.action.Enable();
        }
        public void OnDisable()
        {
            movementActionReference.action.Disable();
            sprintActionReference.action.Disable();
            rollActionReference.action.Disable();



        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created



        // Update is called once per frame
        void Update()
        {
            velocityY -= Time.deltaTime * gravity;
            velocityY = Mathf.Clamp(velocityY, -10, 10);
            movementInput = movementActionReference.action.ReadValue<Vector2>();
            bool isSpritningHeld = sprintActionReference.action.IsPressed();
            bool isWalkRoll = rollActionReference.action.IsPressed();

            if(rollActionReference.action.WasPressedThisFrame() && !isSprinting && canControl)
            {
                StartCoroutine(Dodge());
            }
            // Sprinting = Shift held AND moving forward
            isSprinting = sprintActionReference.action.IsPressed() && movementInput.y > 0.1f;
            // If not sprinting, strafe-walk is true
            isStrafeWalk = !isSprinting;

            currentSpeed = isSprinting ? runSpeed : walkSpeed;

            stateMachine.Update();
        }
        public void Walking()
        {
            // BLOCK normal movement while dodging
            if (!canControl || isDodging)
                return;
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
        private void RootMotionActive()
        {
            if (useRootMotion)
            {

                rb.MovePosition(rb.position + deltaTargetPos);
                rb.MoveRotation(deltaTargetRot * rb.rotation);

                deltaTargetPos = Vector3.zero;
                deltaTargetRot = Quaternion.identity;

                return;
            }
        }
        void FixedUpdate()
        {
            stateMachine.FixedUpdate(); 


        }
    }
}

