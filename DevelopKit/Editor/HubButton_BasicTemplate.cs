
using System.Collections.Generic;

namespace DevelopKit.Editor
{
    public class HubButton_BasicTemplate : HubButton
    {
        public override string Name => "Basic Template";
        public override string Description => "새로운 프로젝트를 위한 기본 탬플릿입니다.\n" +
                                              "주로 씬 전환, 사운드, UI, 유틸리티, 게임 기본 시스템을 제공합니다.";
        public override HubButtonPriority Priority => HubButtonPriority.BasicTemplate;
        
        public override List<PackageDependency> Dependencies
        {
            get
            {
                if (_dependencies == null)
                {
                    _dependencies = new()
                    {
                        new UnitaskPD(),
                        new AddressablePD()
                    };
                }

                return _dependencies;
            }
        }
    }
}