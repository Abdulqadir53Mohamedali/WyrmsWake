using Game.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.FSM
{
    public abstract  class BaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;
        protected const float crossFade = 0.10f;

        protected BaseState(PlayerController p, Animator a) { player = p; animator = a;}


        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }

}
