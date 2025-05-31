using System;

namespace DevelopKit.BasicTemplate
{
    public class GameLifeManager : SingletonMonoBehaviour<GameLifeManager>
    {
        public static event Action OnPausedCallback;
        public static event Action OnFocusedCallback;
        public static event Action OnQuitedCallback;
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                OnPausedCallback?.Invoke();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                OnFocusedCallback?.Invoke();
            }
        }
        
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            OnQuitedCallback?.Invoke();
        }
        
        public static void InvokeQuitCallback() => OnQuitedCallback?.Invoke();
    }
}