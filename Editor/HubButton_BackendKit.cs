
using System.Collections.Generic;

namespace DevelopKit.Editor
{
    public class HubButton_BackendKit : HubButton
    {
        public override string Name => "Backend Kit";
        public override string Description => "뒤끝 서버 SDK을 쉽게 사용하기 위한 템플릿입니다. " +
                                              "SNS 및 커스텀 로그인, 랭킹 및 채팅, 서버 시간 등 다양한 기능을 제공합니다.";
        public override HubButtonPriority Priority => HubButtonPriority.Backend;
        public override List<PackageDependency> Dependencies => null;
    }
}