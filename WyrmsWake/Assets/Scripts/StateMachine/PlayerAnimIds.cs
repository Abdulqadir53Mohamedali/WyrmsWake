using UnityEngine;
namespace Game.Anim
{
    public static class PlayerAnimIds
    {
        public static int X = Animator.StringToHash("X");
        public static int Y = Animator.StringToHash("Y");
        public static int Speed = Animator.StringToHash("Speed");


        public static readonly int locomotion = Animator.StringToHash("Base Layer/LoocmotionWalk");
    }
}

