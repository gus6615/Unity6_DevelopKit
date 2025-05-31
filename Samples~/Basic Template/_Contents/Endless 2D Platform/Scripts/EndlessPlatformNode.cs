using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DevelopKit.EndlessPlatform
{
    public class EndlessPlatformNode : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        public float Width => spriteRenderer ? spriteRenderer.bounds.size.x / 2 : 0.0f;
        public float Height => spriteRenderer ? spriteRenderer.bounds.size.y / 2 : 0.0f;
        public int NodeIndex { get; set; }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
    
    public class EndlessPlatformNodeContainer
    {
        private readonly List<EndlessPlatformNode> _nodes;
        public List<EndlessPlatformNode> Nodes => _nodes;
        private float MoveSpeed { get; set; }

        public EndlessPlatformNodeContainer(float moveSpeed)
        {
            _nodes = new List<EndlessPlatformNode>();
            MoveSpeed = moveSpeed;
        }

        public void AddFirst(EndlessPlatformNode node) => _nodes.Insert(0, node);
        public void AddLast(EndlessPlatformNode node) => _nodes.Add(node);
        public void Remove(EndlessPlatformNode node) => _nodes.Remove(node);
        
        public void Move(float delta)
        {
            foreach (var node in _nodes)
            {
                node.transform.position += delta * MoveSpeed * Vector3.right;
            }
        }
    }

    public class EndlessPlatformPool
    {
        private List<IObjectPool<EndlessPlatformNode>> _pools = new();
        private List<EndlessPlatformNode> _nodePrefabs = new();
        private Transform _parent;
        private int _currentCapacity;
        
        public void Bind(List<EndlessPlatformNode> prefabs, Transform parent = null)
        {
            _nodePrefabs = prefabs;
            _parent = parent;
            _currentCapacity = 0;

            for (int i = 0; i < prefabs.Count; i++)
            {
                var nodeIndex = i;
                _pools.Add(new ObjectPool<EndlessPlatformNode>(() => CreatePooledItem(nodeIndex), GetPooledItem, ReleasePooledItem));
            }
        }
        
        public EndlessPlatformNode Get()
        {
            var randomNodeIndex = GetRandomNodeIndex();
            return Get(randomNodeIndex);
        }
        
        public EndlessPlatformNode Get(int nodeIndex)
        {
            if (_currentCapacity <= 0)
            {
                return CreatePooledItem(nodeIndex);
            }
            
            _currentCapacity--;
            return _pools[nodeIndex].Get();
        }
        
        public void Release(EndlessPlatformNode item)
        {
            _currentCapacity++;
            var randomNodeIndex = item.NodeIndex;
            _pools[randomNodeIndex].Release(item);
        }
        
        private EndlessPlatformNode CreatePooledItem(int nodeIndex)
        {
            var prefab = _nodePrefabs[nodeIndex];
            var newNode = Object.Instantiate(prefab, prefab.transform.position, prefab.transform.rotation, _parent)
                .GetComponent<EndlessPlatformNode>();
            newNode.NodeIndex = nodeIndex;
            newNode.gameObject.SetActive(true);
            return newNode;
        }
        
        private void GetPooledItem(EndlessPlatformNode item)
        {
            item.gameObject.SetActive(true);
        }
        
        private void ReleasePooledItem(EndlessPlatformNode item)
        {
            item.gameObject.SetActive(false);
        }
        
        private int GetRandomNodeIndex() => Random.Range(0, _nodePrefabs.Count);
    }
}
