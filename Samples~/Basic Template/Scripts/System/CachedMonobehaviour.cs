using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public class CachedMonobehaviour : MonoBehaviour
    {
        private Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();

        protected T GetCachedComponent<T>() where T : Component
        {
            Type type = typeof(T);

            if (_cachedComponents.ContainsKey(type))
            {
                return (T)_cachedComponents[type];
            }

            T component = GetComponent<T>();
            if (component != null)
            {
                _cachedComponents[type] = component;
            }
            return component;
        }

        private Transform _cachedTransform;

        public new Transform transform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = base.transform;
                }
                return _cachedTransform;
            }
        }
    }
}