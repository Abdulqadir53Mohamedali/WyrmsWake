using UnityEngine;
namespace Game.Anim
{
    public static class PlayerAnimIds
    {
        public static int X = Animator.StringToHash("X");
        public static int Y = Animator.StringToHash("Y");
        public static int Speed = Animator.StringToHash("Speed");
        //public static int IsSprinting = Animator.StringToHash("IsSprinting");


        public static readonly int locomotion = Animator.StringToHash("Locomotion state");
        //public static readonly int running = Animator.StringToHash("SwordAndShieldSprint");
        public static readonly int running = Animator.StringToHash("HumanMSprint01Forward");
        public static readonly int walkingRoll = Animator.StringToHash("StandToRoll");
    }
}

