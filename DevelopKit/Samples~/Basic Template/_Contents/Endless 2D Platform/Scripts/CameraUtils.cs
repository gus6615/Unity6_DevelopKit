using UnityEngine;

namespace DevelopKit.EndlessPlatform
{
    public static class CameraUtils
    {
        private static Camera _main;
        public static Camera Main
        {
            get
            {
                if (_main == null)
                {
                    _main = Camera.main;
                }

                return _main;
            }
        }
        
        public static float GetCamWidth() => Main.orthographicSize * Screen.width / Screen.height;
        
        public static float GetCamHeight() => Main.orthographicSize;
        
        public static bool OutOfView(EndlessPlatformNode node, float threshold = 0.5f) 
            => node.transform.position.x - node.Width - threshold > Main.transform.position.x + GetCamWidth()
               || node.transform.position.x + node.Width + threshold <= Main.transform.position.x - GetCamWidth();
        
        public static bool InLeftEdge(EndlessPlatformNode node, float threshold = 0.5f) 
            => node.transform.position.x - node.Width - threshold > Main.transform.position.x - GetCamWidth();
        
        public static bool InRightEdge(EndlessPlatformNode node, float threshold = 0.5f) 
            => node.transform.position.x + node.Width - threshold <= Main.transform.position.x + GetCamWidth();
    }
}