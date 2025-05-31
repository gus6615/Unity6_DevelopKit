using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public enum StartUpPriority
    {
        Default = 0,
    }
    
    public interface IStartUpTask
    {
        bool IsDone { get; set; }
        StartUpPriority Priority { get; }
        UniTask StartUp();
    }
}