using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.EndlessPlatform
{
    public class EndlessPlatformSerialSpawner : MonoBehaviour
    {
        [Header("[ If true, this spawner will manage itself ]")]
        [Space(5)]
        [SerializeField] private bool selfManaged = true;
        
        [Space(10)]
        [SerializeField] private Transform platform;
        [SerializeField] private float moveSpeed = 1.0f;
        
        [Space(10)]
        [Header("[ If true, this node will be spawned irregularly (Randomly) ]")]
        [Space(5)]
        [SerializeField] private bool irregularlySpawn = false;
        [SerializeField] private float minSpawnDistance = 2.0f;
        [SerializeField] private float spawnProbability = 0.5f;

        private EndlessPlatformNodeContainer _container;
        private List<EndlessPlatformNode> _nodePrefabs;
        private EndlessPlatformPool _pool;
        private float _cameraCurrentPosX;
        private float _cameraDeltaPosX;
        private float _lastPosX;
        
        private void Start()
        {
            if (!selfManaged)
                return; // EndlessPlatformParallelSpawner will manage this spawner
                
            Initialize(moveSpeed);
        }

        private void Update()
        {
            _cameraDeltaPosX = CameraUtils.Main.transform.position.x - _cameraCurrentPosX;
            _cameraCurrentPosX = CameraUtils.Main.transform.position.x;

            if (selfManaged)
            {
                SpawnRightPlatformNode();
                SpawnLeftPlatformNode();
                ClearPlatformNode();
                MovePlatformNode();   
            }
        }

        public void Initialize(float moveSpeedArg)
        {
            _container = new EndlessPlatformNodeContainer(moveSpeedArg);
            _nodePrefabs = new List<EndlessPlatformNode>();
            _pool = new EndlessPlatformPool();
            _cameraCurrentPosX = CameraUtils.Main.transform.position.x;
            
            var platformNodes = platform.GetComponentsInChildren<EndlessPlatformNode>();
            
            for (int i = 0; i < platformNodes.Length; i++)
            {
                var node = platformNodes[i];
                node.NodeIndex = i;
                node.gameObject.SetActive(false);
                _nodePrefabs.Add(node);
            }
            
            _pool.Bind(_nodePrefabs, transform);
            
            // initialize the screen with platform nodes
            foreach (var initNode in _nodePrefabs)
            {
                var newNode = _pool.Get(initNode.NodeIndex);
                _lastPosX = newNode.transform.position.x;
                _container.AddLast(newNode);
            }
        }
        
        public void SpawnRightPlatformNode()
        {
            if (irregularlySpawn && _cameraDeltaPosX < 0)
                return; // only spawn when camera moves right in irregularlySpawn mode
            
            var node = _container.Nodes[^1];
            if (CameraUtils.InRightEdge(node))
            {
                var newNode = SpawnRandomNode(node, 1);
                _container.AddLast(newNode);
                if (IsActivate(newNode))
                {
                    _lastPosX = newNode.transform.position.x;
                }
                else
                {
                    newNode.gameObject.SetActive(false);
                }
            }
        }
        
        public void SpawnLeftPlatformNode()
        {
            if (irregularlySpawn && _cameraDeltaPosX > 0)
                return; // only spawn when camera moves left in irregularlySpawn mode
            
            var node = _container.Nodes[0];
            if (CameraUtils.InLeftEdge(node))
            {
                var newNode = SpawnRandomNode(node, -1);
                _container.AddFirst(newNode);
                if (IsActivate(newNode))
                {
                    _lastPosX = newNode.transform.position.x;
                }
                else
                {
                    newNode.gameObject.SetActive(false);
                }
            }
        }
        
        public void ClearPlatformNode()
        {
            for (int i = _container.Nodes.Count - 1; i >= 0; i--)
            {
                var node = _container.Nodes[i];
                if (CameraUtils.OutOfView(node))
                {
                    _container.Remove(node);
                    _pool.Release(node);
                }
            }
        }
        
        public void MovePlatformNode()
        {
            _container.Move(_cameraDeltaPosX);
        }
        
        private bool IsActivate(EndlessPlatformNode current)
        {
            if (!irregularlySpawn)
                return true;
            
            var distance = Mathf.Abs(current.transform.position.x - _lastPosX);
            if (distance < minSpawnDistance)
                return false;
            
            var random = Random.Range(0.0f, 1.0f);
            return random < spawnProbability;
        }
        
        private EndlessPlatformNode SpawnRandomNode(EndlessPlatformNode current, float shift)
        {
            var randomNode = _pool.Get();
            var x = current.transform.position.x + shift * (current.Width + randomNode.Width);
            var y = randomNode.transform.position.y;
            var z = randomNode.transform.position.z;
            randomNode.transform.position = new Vector3(x, y, z);
            return randomNode;
        }
    }
}