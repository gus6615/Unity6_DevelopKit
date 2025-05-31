using UnityEngine;

namespace DevelopKit.Editor
{
    public abstract class PackageDependency
    {
        public abstract string Name { get; }
        public abstract string URL { get; }
    }

    public class UnitaskPD : PackageDependency
    {
        public override string Name => "com.cysharp.unitask";

        public override string URL => "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask";
    }

    public class AddressablePD : PackageDependency
    {
        public override string Name => "com.unity.addressables";

        public override string URL => "2.2.2";
    }
}
