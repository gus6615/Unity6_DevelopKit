
namespace DevelopKit.BasicTemplate
{
    public abstract class StateBase
    {
        public virtual void Initialize(object data = null) { }
        public abstract void OnEnter();
        public abstract void OnUpdate(float dt);
        public abstract void OnExit();
    }
}