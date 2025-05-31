using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class StartUpState : StateBase
    {
        public override void OnEnter()
        {
            Debug.Log("StartUpState OnEnter");
        }

        public override void OnUpdate(float dt)
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("StartUpState OnExit");
        }
    }
}