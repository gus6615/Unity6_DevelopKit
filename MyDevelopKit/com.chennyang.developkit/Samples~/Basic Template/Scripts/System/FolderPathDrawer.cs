using UnityEngine;
using UnityEditor;

namespace DevelopKit.BasicTemplate
{
    public class FolderPathAttribute : PropertyAttribute
    {
        // 폴더 경로 속성을 나타내는 빈 속성 클래스
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private string _tempPath;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            Rect textFieldRect = new Rect(position.x, position.y, position.width - 30, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 25, position.y, 25, position.height);
            
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(textFieldRect, "Use [FolderPath] with a string.");
                return;
            }

            string path = property.stringValue;
            
            if (!path.Contains(Application.dataPath))
            {
                EditorGUI.LabelField(textFieldRect, "Select a correct path in Assets.");
                property.stringValue = string.Empty;
            }
            else
            {
                EditorGUI.TextField(textFieldRect, GetRelativePath(path));
            }
            
            // '폴더 아이콘' 버튼 생성
            GUIContent folderIcon = EditorGUIUtility.IconContent("Folder Icon");
            if (GUI.Button(buttonRect, folderIcon))
            {
                _tempPath = path;
                
                // 딜레이 콜을 하는 이유: GUI 시스템에서 레이아웃 그룹이 제대로 정리되지 않아 발생하는 오류 방지 => GUI 업데이트 이후에 실행
                EditorApplication.delayCall += () =>
                {
                    string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", _tempPath, "");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        property.stringValue = selectedPath;
                        
                        // 수동으로 inspector 업데이트
                        property.serializedObject.ApplyModifiedProperties();
                    }
                };
            }
            
            EditorGUI.EndProperty();
        }
        
        private string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return string.Empty;
            
            // 현재 프로젝트 폴더 경로 가져오기
            string projectPath = Application.dataPath.Replace("/Assets", "");

            // 프로젝트 내부 경로인지 확인
            if (fullPath.StartsWith(projectPath))
            {
                return fullPath.Substring(projectPath.Length + 1);
            }
            
            // 프로젝트 내부 경로가 아닌 경우 빈 문자열 반환
            return string.Empty;
        }
    }
#endif
}