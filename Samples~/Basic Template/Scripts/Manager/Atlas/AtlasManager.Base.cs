using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    public partial class AtlasManager : Manager
    {
        private AtlasDataBase _atlasDataBase;
        private readonly Dictionary<AssetReferenceT<SpriteAtlas>, string> _assetRefToAtlasNameDict = new();
        private readonly Dictionary<string, SpriteAtlas> _loadedAtlasNameDict = new();
        private readonly Dictionary<AtlasType, SpriteAtlas> _loadedAtlasTypeDict = new();
        public override ManagerPriority Priority => ManagerPriority.Atlas;
        public override async UniTask StartUp()
        {
            var atlasDataBaseKey = ManagerHub.AtlasDataBaseKey;
            _atlasDataBase = await Addressables.LoadAssetAsync<AtlasDataBase>(atlasDataBaseKey);
            SceneLoader.OnStartChangeScene += OnStartChangeScene;
        }
        
        private Sprite GetSprite(AtlasType atlasType, int id)
        {
            if (_loadedAtlasTypeDict.TryGetValue(atlasType, out SpriteAtlas atlas))
            {
                return atlas.GetSprite($"{atlasType}_{id}");
            }
            else
            {
                Debug.LogError($"아틀라스 타입 '{atlasType}'이 로드되지 않았습니다.");
            }
            return null;
        }
        
        // * 씬 전환 시 호출 필요 (씬 아틀라스 로드/해제)
        private async UniTask OnStartChangeScene(string prevSceneName, string nextSceneName)
        {
            foreach (var data in _atlasDataBase.atlasRefs)
            {
                AssetReferenceT<SpriteAtlas> assetRef = data.assetRef;
                AtlasType atlasType = data.atlasType;
                List<string> sceneNames = data.sceneNames;
                
                if (atlasType == AtlasType.None)
                {
                    Debug.LogError("아틀라스 타입이 None입니다. 아틀라스 타입을 설정해주세요.");
                    continue;
                }

                // 이전 씬에 있었는데 다음 씬에 없는 아틀라스를 해제
                if (sceneNames.Contains(prevSceneName))
                {
                    if (sceneNames.Contains(nextSceneName))
                    {
                        continue;
                    }
                    
                    if (_assetRefToAtlasNameDict.TryGetValue(assetRef, out string atlasName) &&
                        _loadedAtlasNameDict.TryGetValue(atlasName, out SpriteAtlas atlas))
                    {
                        Addressables.Release(atlas);
                        _loadedAtlasNameDict.Remove(atlasName);
                        _loadedAtlasTypeDict.Remove(atlasType);
                    }
                }

                // 다음 씬에 존재하는 아틀라스 로드
                if (sceneNames.Contains(nextSceneName))
                {
                    var atlas = await Addressables.LoadAssetAsync<SpriteAtlas>(assetRef);
                    _assetRefToAtlasNameDict.TryAdd(assetRef, atlas.name);
                    _loadedAtlasNameDict.TryAdd(atlas.name, atlas);
                    _loadedAtlasTypeDict.TryAdd(atlasType, atlas);
                }
            }
        }
    }
}