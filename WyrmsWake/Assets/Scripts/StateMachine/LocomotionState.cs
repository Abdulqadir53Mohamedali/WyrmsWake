using UnityEngine;
using Game.Player;
using Game.Anim;

namespace Game.FSM
{
    public class LocomotionState : BaseState
    {


        public LocomotionState( PlayerController player, Animator animator) : base(player,animator)
        {

        }

        public override void  OnEnter()
        {
            animator.CrossFade(PlayerAnimIds.locomotion,crossFade,0);
        }



        public override void Update()
        {

        }
        public override void FixedUpdate()
        {
            player.Walking();
            Vector3 local = player.transform.InverseTransformDirection(player.targetVel);
            float nx = Mathf.Clamp(local.x / player.walkSpeed, -1f, 1f);
            float ny = Mathf.Clamp(local.z / player.walkSpeed, -1f, 1f);
            
            animator.SetFloat(PlayerAnimIds.X, nx, 0.12f, Time.deltaTime);
            animator.SetFloat(PlayerAnimIds.Y, ny, 0.12f, Time.deltaTime);
        }


        public override void OnExit()
        {
            player.isStrafeWalk = false;
        }



    }
}

