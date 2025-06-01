using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    [CreateAssetMenu(fileName = "ProjectDataBase", menuName = "DevelopKit/SO/ProjectDataBase", order = 0)]

    public class ProjectDataBase : ScriptableObject
    {
        [Header("프로젝트 세팅에 필요한 데이터를 적어주세요.\n※. 각 파라미터를 호버하면 설명 툴팁이 등장합니다.")] [Space(20)] [Tooltip("가로 해상도")]
        public int ResolutionWidth;
        
        [Tooltip("세로 해상도")]
        public int ResolutionHeight;

        [Tooltip("수직 동기화\n\n0 = 비활성화\n1 = 활성화(매 프레임)\n2 = 활성화(매 두번째 프레임)")]
        public int VSyncCount;

        [Tooltip("최대 프레임 속도\n\n-1 = 제한 없음\n그 외 = 제한 프레임 속도")]
        public int TargetFrameRate;

        [Tooltip("화면이 꺼지지 않도록 설정\n\n0 = 화면이 꺼지지 않음\n1 = 시스템 설정에 따라 꺼짐\n그 외 = 값에 따라 꺼짐")]
        public int SleepTimeout;
    }
}