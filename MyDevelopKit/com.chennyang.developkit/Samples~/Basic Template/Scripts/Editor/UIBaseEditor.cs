using UnityEditor;

namespace DevelopKit.BasicTemplate
{
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            UIBase uiBase = target as UIBase;
            if (uiBase == null) return;

            EditorGUILayout.Space();

            if (uiBase.BaseAnimator == null)
            {
                EditorGUILayout.HelpBox("Enter & Exit 애니메이션을 추가하고 싶다면 오브젝트에 Animator 컴포넌트를 추가하여 Base Animator에 할당하세요.", MessageType.Info);
                return;
            }
            
            if (uiBase.BaseAnimator.runtimeAnimatorController == null)
            {
                EditorGUILayout.HelpBox("현재 Base Animator에 Runtime Animator Controller가 없습니다. Controller를 추가하세요.", MessageType.Warning);
            }
            else
            {
                bool hasEnterClip = false;
                bool hasExitClip = false;
                
                foreach (var clip in uiBase.BaseAnimator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name.Contains("Enter"))
                    {
                        hasEnterClip = true;
                    }
                    else if (clip.name.Contains("Exit"))
                    {
                        hasExitClip = true;
                    }
                }
                
                if (hasEnterClip == false)
                {
                    EditorGUILayout.HelpBox($"Enter 이름이 포함된 Clip을 추가하세요.", MessageType.Warning);
                }
                
                if (hasExitClip == false)
                {
                    EditorGUILayout.HelpBox($"Exit 이름이 포함된 Clip을 추가하세요.", MessageType.Warning);
                }
            }
        }
    }
}