using UnityEngine;
using Game.Anim;
using Game.Player;
namespace Game.FSM
{
    public class WalkingRollState : BaseState
    {
        public WalkingRollState(PlayerController player,Animator animator): base(player, animator)
        {

        }
        public override void OnEnter()
        {
            animator.CrossFade(PlayerAnimIds.walkingRoll, crossFade, 0);
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {
            
        }

        public override void OnExit()
        {
        }
    }

}
