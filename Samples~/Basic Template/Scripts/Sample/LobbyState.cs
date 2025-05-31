using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class LobbyState : StateBase
    {
        List<ExampleGameObject> _list = new();
        
        public override void OnEnter()
        {
            Debug.Log("LobbyState OnEnter");
        }

        public override void OnUpdate(float dt)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Q");
                var go = ExamplePool.Get();
                _list.Add(go);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("W");
                if (_list.Count == 0)
                {
                    return;
                }
                var go = _list[0];
                ExamplePool.Release(go);
                _list.Remove(go);
            }
        }
        
        public override void OnExit()
        {
            Debug.Log("LobbyState OnExit");
        }
    }
}
