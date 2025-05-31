using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    [Serializable]
    public class AtlasData
    {
        public AssetReferenceT<SpriteAtlas> assetRef;
        public AtlasType atlasType;
        public List<string> sceneNames;
    }
    
    [CreateAssetMenu(fileName = "AtlasDataBase", menuName = "DevelopKit/SO/AtlasDataBase", order = 1)]
    public class AtlasDataBase : ScriptableObject
    {
        [SerializeField] [Header("아틀라스가 쓰이는 씬들의 이름을 적어주세요.")]
        [Space(20)]
        public List<AtlasData> atlasRefs;
    }
}