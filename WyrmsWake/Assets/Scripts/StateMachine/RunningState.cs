using UnityEngine;
using Game.Player;
using Game.Anim;

namespace Game.FSM
{
    public class RunningState : BaseState
    {
        public RunningState(PlayerController player, Animator animator) : base(player, animator) 
        {
        
        }
        public override void OnEnter()
        {
            //Debug.Log("runnign entered");

            animator.CrossFade(PlayerAnimIds.running, crossFade, 0);
        }

        public override void Update()
        {

        }
        public override void FixedUpdate()
        {
            player.Walking();
            Debug.Log("Running");

        }



        public override void OnExit()
        {
            player.isSprinting = false;   
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
    }
}



