using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevelopKit.Editor
{
    public class DevelopKitEditor : EditorWindow
    {
        private static readonly Vector2 WindowSize = new Vector2(400, 600);
        private static readonly float HubButtonWidth = 150;
        private static readonly float HubButtonHeight = 30;
        
        private static Dictionary<string, HubButton> _hubButtons;
        internal static Dictionary<string, HubButton> HubButtons
        {
            get
            {
                if (_hubButtons == null)
                {
                    var hubButtonTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => typeof(HubButton).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                        .ToList();
                
                    _hubButtons = new();
                    foreach (var hubButtonType in hubButtonTypes)
                    {
                        var hubButton = Activator.CreateInstance(hubButtonType) as HubButton;
                        if (hubButton == null)
                            continue;
                    
                        _hubButtons[hubButton.Name] = hubButton;
                    }
                }

                return _hubButtons;
            }
        }
        
        private Texture2D _hubTexture;
        private Texture2D _hubButtonPanelTexture;
        private Texture2D _hubButtonInstallHoverTexture, _hubButtonInstallNormalTexture, _hubButtonInstallActionTexture;
        private Texture2D _hubButtonDeleteHoverTexture, _hubButtonDeleteNormalTexture, _hubButtonDeleteActionTexture;

        private GUIStyle _hubTitleStyle;
        private GUIStyle _hubButtonPanelStyle;
        private GUIStyle _hubButtonInstallStyle;
        private GUIStyle _hubButtonSolveDependenciesStyle;
        private GUIStyle _hubButtonDeleteStyle;
        private GUIStyle _hubLabelStyle;

        [MenuItem("DevelopKit/Hub")]
        public static void ShowWindow()
        {
            //var window = GetWindow<DevelopKitEditor>("DevelopKit Hub");
            var window = CreateInstance<DevelopKitEditor>();
            window.titleContent = new GUIContent("DevelopKit Hub");
            
            window.minSize = WindowSize;
            window.maxSize = WindowSize;
            window.ShowUtility();
        }
        
        private void OnGUI()
        {
            SetGUIData();
            
            Repaint();
            
            var imageRect = new Rect(0, 0, 400, 100);
            GUI.DrawTexture(imageRect, _hubTexture, ScaleMode.ScaleToFit);
            
            GUILayout.BeginVertical();
            
            GUILayout.Space(110);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            GUILayout.Label("DevelopKit은 게임 개발에 필요한 유용한 기능 혹은 유틸리티를\n제공하는 도구입니다.", _hubTitleStyle);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            
            var hubButtonList = HubButtons.Values.ToList();
            hubButtonList.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            
            foreach (var hubButton in hubButtonList)
            {
                GUILayout.BeginHorizontal(_hubButtonPanelStyle);
                GUILayout.FlexibleSpace();
                    
                bool isInstalled = hubButton.IsInstalled();

                if (isInstalled)
                {
                    if (GUILayout.Button($"Delete {hubButton.Name}", _hubButtonDeleteStyle
                            , GUILayout.Width(HubButtonWidth), GUILayout.Height(HubButtonHeight)))
                    {
                        hubButton.OnClickedDelete();
                    }
                }
                else
                {
                    if (GUILayout.Button($"Install {hubButton.Name}", _hubButtonInstallStyle
                            , GUILayout.Width(HubButtonWidth), GUILayout.Height(HubButtonHeight)))
                    {
                        hubButton.OnClickedInstall();
                    }
                }
                
                GUILayout.Label(hubButton.Description, _hubLabelStyle);
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(10);
            }
            
            GUILayout.BeginHorizontal(_hubButtonPanelStyle);
            GUILayout.FlexibleSpace();
            
            // 의존성 검사
            if (GUILayout.Button("Solve Package Dependencies", _hubButtonSolveDependenciesStyle,
                    GUILayout.Width(200), GUILayout.Height(HubButtonHeight)))
            {
                PackageInstaller.SolvePackageDependencies();
            }
            
            GUILayout.Label("만약 Package 의존성 문제가 발생하면 왼쪽 버튼을 눌러주세요.", _hubLabelStyle);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
        
        private void SetGUIData()
        {
            if (_hubTitleStyle == null)
            {
                _hubTitleStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 12,
                    normal = { textColor = Color.white }
                };
            }
            
            if (_hubTexture == null)
            {
                string path = "Packages/com.cheonnyang.developkit/HubTexture.png";
                _hubTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (_hubTexture == null)
                {
                    path = "Assets/_DevelopKit/HubTexture.png";
                    _hubTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }
            
            if (_hubButtonPanelStyle == null)
            {
                _hubButtonPanelTexture = CreateColorTexture(new Color(0.2f, 0.2f, 0.2f), false);
                _hubButtonPanelStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleLeft,
                    normal = { background = _hubButtonPanelTexture },
                    padding = new RectOffset(10, 10, 10, 10)
                };
            }

            if (_hubLabelStyle == null)
            {
                _hubLabelStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 10,
                    normal = { textColor = Color.white },
                    padding = new RectOffset(10, 10, 0, 10)
                };
            }

            if (_hubButtonInstallStyle == null)
            {
                _hubButtonInstallNormalTexture = CreateColorTexture(new Color(0.1f, 0.3f, 0.1f), true);
                _hubButtonInstallHoverTexture = CreateColorTexture(new Color(0.1f, 0.4f, 0.1f), true);
                _hubButtonInstallActionTexture = CreateColorTexture(new Color(0.1f, 0.35f, 0.1f), true);
                _hubButtonInstallStyle = new GUIStyle(GUI.skin.button)
                {
                    normal =
                    {
                        background = _hubButtonInstallNormalTexture,
                        textColor = new Color(0.5f, 1.0f, 0.5f)
                    },
                    hover =
                    {
                        background = _hubButtonInstallHoverTexture,
                        textColor = new Color(0.4f, 1.0f, 0.45f)
                    },
                    active =
                    {
                        background = _hubButtonInstallActionTexture,
                        textColor = new Color(0.45f, 1.0f, 0.45f)
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            
            if (_hubButtonDeleteStyle == null)
            {
                _hubButtonDeleteNormalTexture = CreateColorTexture(new Color(0.4f, 0.1f, 0.1f), true);
                _hubButtonDeleteHoverTexture = CreateColorTexture(new Color(0.3f, 0.1f, 0.1f), true);
                _hubButtonDeleteActionTexture = CreateColorTexture(new Color(0.35f, 0.1f, 0.1f), true);
                _hubButtonDeleteStyle = new GUIStyle(GUI.skin.button)
                {
                    normal =
                    {
                        background = _hubButtonDeleteNormalTexture,
                        textColor = new Color(1.0f, 0.2f, 0.2f)
                    },
                    hover =
                    {
                        background = _hubButtonDeleteHoverTexture,
                        textColor = new Color(1.0f, 0.1f, 0.1f)
                    },
                    active = 
                    {
                        background = _hubButtonDeleteActionTexture,
                        textColor = new Color(1.0f, 0.15f, 0.15f)
                    },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (_hubButtonSolveDependenciesStyle == null)
            {
                _hubButtonSolveDependenciesStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                };
            }
        }
        
        private Texture2D CreateColorTexture(Color color, bool hasBorder)
        {
            Texture2D texture = new Texture2D((int)HubButtonWidth, (int)HubButtonHeight);
            Color borderColor = Color.black;
            int border = 1;

            for (int x = 0; x < HubButtonWidth; x++)
            {
                for (int y = 0; y < HubButtonHeight; y++)
                {
                    if (hasBorder && (x < border || x >= HubButtonWidth - border || y < border || y >= HubButtonHeight - border)) // 테두리 부분
                    {
                        texture.SetPixel(x, y, borderColor);
                    }
                    else // 내부 배경
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
                
            texture.Apply();
            return texture;
        }
    }
}