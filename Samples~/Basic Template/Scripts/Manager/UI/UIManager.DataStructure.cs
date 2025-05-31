using System;

namespace DevelopKit.BasicTemplate
{
    public class UIPropertyAttribute : Attribute
    {
        public readonly UIType UIType;
        public readonly string AddressableKey;

        public UIPropertyAttribute(UIType uiType, string addressableKey)
        {
            this.UIType = uiType;
            this.AddressableKey = addressableKey;
        }
    }

    public class UIProperty
    {
        public UIType UIType { get; private set; }
        public string AddressableKey { get; private set; }

        public UIProperty(UIType uiType, string addressableKey)
        {
            this.UIType = uiType;
            this.AddressableKey = addressableKey;
        }
    }
        
    public enum UIType
    {
        None,
        HUD,        // 화면 일부 + 정보          (예: HP바)
        Alert,      // 화면 일부 + 단순 조작     (예: 확인 팝업)
        Popup,      // 화면 일부 + 콘텐츠       (예: 우편함)
        Cover,      // 화면 전체 + 단순 조작    (예: 로딩 화면)
        Overlay,    // 화면 전체 + 콘텐츠      (예: 이벤트)      
    }

    public class CallbackTuple
    {
        public Action YesAction { get; private set; }
        public Action NoAction { get; private set; }

        public CallbackTuple(Action yesAction, Action noAction)
        {
            YesAction = yesAction;
            NoAction = noAction;
        }
    }
}