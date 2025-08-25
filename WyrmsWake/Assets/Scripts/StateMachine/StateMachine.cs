using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.FSM
{
    public class StateMachine 
    {
        StateNode current;

        // Holds all current transitions and states
        Dictionary<Type , StateNode> nodes = new();

        // Holds transitions which cna be accessed from any other transition 
        HashSet<ITransition> anyTransition = new();
        bool IsSameState(IState a, IState b) => a?.GetType() == b?.GetType();


        public void Update()
        {
            var transition = GetTransition();

            // As long as there is soemthing to transition , thne we are going to change 
            // the state to whatever the To state was in thta particular transition
            if (transition != null)
            {
                ChangeState(transition.To);
            }

            //  run update of current state 
            current.State?.Update();

        }
        public void FixedUpdate()
        {
            current.State?.FixedUpdate();
        }

        private void ChangeState(IState state)
        {
            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;

            if (IsSameState(nextState, previousState)) return;
 
            previousState?.OnExit();
            nextState?.OnEnter();

            // ensures current state is set to actual StateNode which is stored in the dicitonary
            current = nodes[state.GetType()];
        }
        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current.State.OnEnter();
        }


        ITransition GetTransition()
        {
            foreach (var transition in anyTransition)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
                
            }
            foreach(var transition in current.Transitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            return null;
        }
        public void AddTransition(IState from, IState to, IPredicate condition)
        {

            // create a from node , adding transtion which goes to next state
            // based ont the condition passed in

            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransition.Add(item: new Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            // takes in state , attmepts to find in dictionary 
            var node = nodes.GetValueOrDefault(key: state.GetType());

            // if returns default which is null , it will create a new stateNode using that state 

            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }





        class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }


            // accepot that  state and initilise a empty hashset
            public StateNode(IState state)
            {

                State = state;
                Transitions = new HashSet<ITransition>();
            }

            // To add as many tranisitions as necessary 
            // state we are going to and the condition upon whihc we will movwe 
            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(item: new Transition(to, condition));
            }

        }
    }
}

