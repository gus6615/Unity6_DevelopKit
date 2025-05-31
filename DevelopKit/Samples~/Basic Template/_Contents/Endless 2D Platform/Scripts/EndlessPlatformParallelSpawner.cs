using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.EndlessPlatform
{
    public class EndlessPlatformParallelSpawner : MonoBehaviour
    {
        [SerializeField] private float minMoveSpeed;
        [SerializeField] private float maxMoveSpeed;
        private List<EndlessPlatformSerialSpawner> spawners;
        
        private void Start()
        {
            spawners = new List<EndlessPlatformSerialSpawner>(GetComponentsInChildren<EndlessPlatformSerialSpawner>());
            if (spawners.Count == 0)
            {
                Debug.LogError("No spawner found.");
                return;
            }
            
            spawners[0].Initialize(maxMoveSpeed);
            for (int i = 1; i < spawners.Count; i++)
            {
                var moveSpeed = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, (float)i / (spawners.Count - 1));
                spawners[i].Initialize(moveSpeed);
            }
        }
        
        private void Update()
        {
            foreach (var spawner in spawners)
            {
                spawner.SpawnRightPlatformNode();
                spawner.SpawnLeftPlatformNode();
                spawner.ClearPlatformNode();
                spawner.MovePlatformNode();
            }
        }
    }
}
