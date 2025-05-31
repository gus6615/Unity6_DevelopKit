using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DevelopKit.BasicTemplate
{
    public static class AddressableUtil
    {
        public static bool IsValidKey(object key)
        {
            return Addressables.LoadResourceLocationsAsync(key).WaitForCompletion().Count > 0 ? true : false;
        }
        
        public static T Instantiate<T>(object key, Transform parent, Vector3 position, Quaternion rotation) where T : Object
        {
            if (IsValidKey(key) == false)
            {
                Debug.LogError($"Addressable key is null : {key}");
                return null;
            }
            
            var newGo = Addressables.InstantiateAsync(key, position, rotation, parent).WaitForCompletion();
            newGo.name = $"{key} (Addressable Clone)";
            return newGo.GetComponent<T>();
        }
        
        public static T Instantiate<T>(object key, Transform parent = null) where T : Object
        {
            if (IsValidKey(key) == false)
            {
                Debug.LogError($"Addressable key is null : {key}");
                return null;
            }
            
            var newGo = Addressables.InstantiateAsync(key, parent).WaitForCompletion();
            newGo.name = $"{key} (Addressable Clone)";
            return newGo.GetComponent<T>();
        }
        
        public static T LoadAsset<T>(object key) where T : Object
        {
            if (IsValidKey(key) == false)
            {
                Debug.LogError($"Addressable key is null : {key}");
                return null;
            }
            
            return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
        }
        
        public static void Release(object key)
        {
            Addressables.Release(key);
        }
    }
}
