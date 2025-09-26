using Game.Anim;
using Game.FSM;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
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
        public float maxHealth;

        public float stamina;
        public float maxStamina;
        public float staminaRegnPerSec = 8f;

        public float walkSpeed;
        public float runSpeed;
        public float currentSpeed;

        public float maxMana;
        public float manaRegnPerSec = 15f;

        int gravity = 35;

        [Header("Magic Stats")]
        public SpellSO currentSpell;
        public Transform firePoint;
        [SerializeField] private SpellSO[] availableSpells;
        [Header("Vector & Rotational variables")]
        public Vector3 targetVel;
        private Vector3 deltaTargetPos;
        private Quaternion deltaTargetRot = Quaternion.identity;

        [Header("Dodge")]
        //[SerializeField] AnimationCurve dodgeCurve;
        //float dodgeTimer;
        //public bool canControl = true;
        //public bool isDodging;
        //public float dodgeDistance = 20f; // tune with your curve

        // tracks how fast player moved up or down. Gravity decreases it over time
        // clamping prevents unrelaistic fall speeds . ensures smooth falls and jumps
        float velocityY;

        //[Header("Player Equip")]
        //public GameObject mainSword;
        //public GameObject swordOnShoulder;

        //public bool isEquipped;
        //public bool isEquipping;

        //[Header("Attack Combo 1")]
        //private float attackWindowReset = 1.5f;
        //private float lastInputTime = 0f;
        //private int comboIndex = 0;
        //private bool isAttacking;

        
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
        //public InputActionReference rollActionReference;
        //public InputActionReference equipActionReference;
        //public InputActionReference slashActionReferecne;
        public InputActionReference spellActionRefercne;


        [Header("Player States")]
        public LocomotionState locomotionState { get; private set; }
        public RunningState runningState { get; private set; }
        public WalkingRollState walkRollState { get; private set; }



        private void OnRightClick(InputAction.CallbackContext ctx)
        {
            Debug.Log("Made it to combo start");
            animator.SetTrigger("NoStaffAttackLeftHand");

            //CastSpell();

        }
        //private void OnLeftClick(InputAction.CallbackContext ctx)
        //{
        //    Debug.Log("Made it to combo start");

        //    TryCombo(1);
        //}
        //public void TryCombo(int attack)
        //{
        //    isAttacking = true;

        //                Debug.Log("Made it to combo start");

        //    if (Time.time - lastInputTime > attackWindowReset)
        //    {
        //        ResetAttacks();
        //    }

        //    lastInputTime = Time.time;
        //    comboIndex++;

        //    if (attack == 1 && comboIndex == 1)
        //    {

        //        animator.SetInteger("ComboIndex", comboIndex);
        //        animator.SetTrigger("Attack1");

        //    }
        //    else if (attack == 1 && comboIndex == 2)
        //    {

        //        animator.SetInteger("ComboIndex", comboIndex);
        //        animator.SetTrigger("Attack2");
        //    }
        //    else if (attack == 2 && comboIndex == 3)
        //    {

        //        animator.SetInteger("ComboIndex", comboIndex);
        //        animator.SetTrigger("Attack3");
        //    }

        //    else
        //    {
        //        ResetAttacks();
        //    }
        //}


        //// Placed via anaimtion event in all attacks 
        //public void ResetAttacks()
        //{
        //    isAttacking = false;
        //    comboIndex = 0;
        //    animator.SetInteger("ComboIndex", comboIndex);
        //}

        //public bool SwordEquippedAndAttacking()
        //{
        //    //rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

        //    if (isEquipped && isAttacking == true)
        //    {

        //        return true;
        //    }

        //    return false;
        //}
        //private void Equip()
        //{
        //    if (equipActionReference.action.IsPressed())
        //    {
        //        isEquipping = true;
        //        animator.SetTrigger("Equip");
        //        canControl = false;

        //    }
        //}

        //public void ActiveSword()
        //{
        //    if (!isEquipped)
        //    {
        //        mainSword.SetActive(true);
        //        swordOnShoulder.SetActive(false);
        //        animator.SetBool("SwordEquipped", true);
        //        isEquipped = !isEquipped;

        //    }

        //    else
        //    {
        //        mainSword.SetActive(false);
        //        swordOnShoulder.SetActive(true);
        //        animator.SetBool("SwordEquipped", false);
        //        isEquipped = !isEquipped;
        //    }

        //}
        //public void Equipped()
        //{
        //    isEquipping= false;
        //    canControl = true;
        //}





        /// <summary>
        /// Obtains the total area under the curve
        /// </summary>
        /// <param name="curve"> Actual aniamtion curve object we read / feed the function to be used</param>
        /// <param name="duration"> Total Duration of the AniamtionCurve , clip</param>
        /// <param name="steps">Number of slices / chunks the ACurve is cut in to </param>
        /// <returns></returns>
        //float SampleCurveArea(AnimationCurve curve, float duration, int steps = 60)
        //{
        //    float area = 0f;
        //    // splits curve in to chunks / holds current chunk to a certain degree
        //    float dt = duration / steps;
        //    // index of slice we are on
        //    float t = 0f;
        //    for (int i = 0; i < steps; i++)
        //    {
        //        // obtains value of curve at point in time specified
        //        float a = curve.Evaluate(t);// left edge
        //        float b = curve.Evaluate(t + dt); // right edge 
        //        area += (a + b) * 0.5f * dt;
        //        t += dt; // slide to the next slice
        //    }
        //    return Mathf.Max(0.0001f, area);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //IEnumerator Dodge()
        //{
        //    canControl = false;
        //    isDodging = true;
        //    float timer = 0f;
        //    //float prevAlpha = 0f;
        //    float duration = dodgeCurve.keys[dodgeCurve.length - 1].time;

        //    Vector3 f = cameraTransform.forward; f.y = 0; f.Normalize();
        //    Vector3 r = cameraTransform.right; r.y = 0; r.Normalize();
        //    Vector3 m = (r * movementInput.x + f * movementInput.y);
        //    Vector3 dir = (m.sqrMagnitude > 0.001f) ? m.normalized : transform.forward;

        //    // Face roll direction & play anim
        //    transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        //    animator.CrossFade(PlayerAnimIds.walkingRoll, 0.05f, 0);

        //    //  Normalize curve so total distance == dodgeDistance
        //    float area = SampleCurveArea(dodgeCurve, duration); // seconds * multiplier
        //    float speedScale = dodgeDistance / area;            // meters / (multiplied seconds)

        //    while (timer < duration)
        //    {
        //        // speed(t) = curve(t) * speedScale
        //        float horizSpeed = dodgeCurve.Evaluate(timer) * speedScale; // m/s
        //        Vector3 v = dir * horizSpeed;
        //        v.y = rb.linearVelocity.y; // preserve gravity
        //        rb.linearVelocity = v;


        //        timer += Time.fixedDeltaTime;
        //        yield return new WaitForFixedUpdate();
        //    }
        //    rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        //    canControl = true;
        //    isDodging = false;
        //    stateMachine.SetState(isSprinting ? runningState : locomotionState); 

        //}
        public void CastSpell()
        {
            if (currentSpell == null || currentSpell.spellPrefab == null)
            {
                Debug.LogWarning("No spell or prefab assigned!");
                return;
            }

            GameObject spellInstance = Instantiate(
                currentSpell.spellPrefab,
                firePoint.position,
                firePoint.rotation * Quaternion.Euler(-90f, 0f, 0f)
            );

            // 2. Pass SpellSO data to the Spell script
            Spell spell = spellInstance.GetComponent<Spell>();
            if (spell != null)
            {
                spell.Initialize(currentSpell);

                ProjectileSpell projectile = spell as ProjectileSpell;
                if (projectile != null)
                {
                    projectile.Launch(firePoint.forward);
                }
            }
        }
        public void Awake()
        {
            health = baseStats.maxHealth;
            maxHealth = baseStats.maxHealth;
            stamina = baseStats.maxStamina;
            maxStamina = baseStats.maxStamina;
            walkSpeed = baseStats.walkSpeed;
            runSpeed = baseStats.runSpeed;
            currentSpeed = walkSpeed;
            maxMana = baseStats.maxMana;



            animator = this.GetComponent<Animator>();
            rb = this.GetComponent<Rigidbody>();
            locomotionState = new LocomotionState(this, animator);
            runningState = new RunningState(this, animator);
            //walkRollState = new WalkingRollState(this, animator);
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
            //rollActionReference.action.Enable();
            //equipActionReference.action.Enable();
            //slashActionReferecne.action.Enable();
            spellActionRefercne.action.Enable();
            //slashActionReferecne.action.performed += OnLeftClick;
            spellActionRefercne.action.performed += OnRightClick;

        }
        public void OnDisable()
        {
            movementActionReference.action.Disable();
            sprintActionReference.action.Disable();
            //rollActionReference.action.Disable();
            //equipActionReference.action.Disable();
            //slashActionReferecne.action.Disable();
            spellActionRefercne.action.Disable();
            //slashActionReferecne.action.performed -= OnLeftClick;
            spellActionRefercne.action.performed -= OnRightClick;



        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created



        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentSpell = availableSpells[0];
            }
            //if(comboIndex > 0 && Time.time - lastInputTime > attackWindowReset)
            //{
            //    ResetAttacks();
            //}
            velocityY -= Time.deltaTime * gravity;
            velocityY = Mathf.Clamp(velocityY, -10, 10);
            movementInput = movementActionReference.action.ReadValue<Vector2>();
            bool isSpritningHeld = sprintActionReference.action.IsPressed();
            //bool isWalkRoll = rollActionReference.action.IsPressed();

            //Equip();
            
            //if (rollActionReference.action.WasPressedThisFrame() && !isSprinting && canControl)
            //{
            //    StartCoroutine(Dodge());
            //}
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
            //if (!canControl || isDodging) { 
            //var v = rb.linearVelocity;           // (use rb.velocity on older Unity)
            //v.x = 0f;
            //v.z = 0f;
            //rb.linearVelocity = v;
            //return;
            //    }   
            //if (isSprinting)
            //{
            //    currentSpeed = runSpeed;
            //}
            //if (isStrafeWalk)
            //{
            //    currentSpeed = walkSpeed;
            //}


            Vector3 forward = cameraTransform.forward;
            //Vector3 right = cameraTransform.right;

 

            forward.y = 0;
            //right.y = 0;

            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, forward);

            //right.Normalize();
            Vector3 moveDirection = (right * movementInput.x + forward * movementInput.y).normalized;

            
            targetVel = moveDirection * currentSpeed;
            rb.linearVelocity = new Vector3 (targetVel.x, rb.linearVelocity.y, targetVel.z);
            //rb.AddForce(targetVel, ForceMode.Acceleration);
            //rb.AddForce(moveDirection * walkSpeed, ForceMode.Acceleration);

            if ( isStrafeWalk && forward.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(forward, Vector3.up);
                 transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);

            }

            // should be used for sprinting
             if (isSprinting && moveDirection.sqrMagnitude > 0.001f)
            {
                //Debug.Log("Hello");

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

