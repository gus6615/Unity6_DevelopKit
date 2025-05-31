using System.Linq;

namespace DevelopKit.BasicTemplate
{
    public partial class GameFlowManager
    {
        public void AddUpdateHandler(UpdateCallback callback, UpdatePriority priority = UpdatePriority.Normal)
        {
            _updateHandlers.Add(new PrioritizedUpdateHandler(callback, priority));
            isDirtyFlagOfUpdateHandler = true;
        }

        public void RemoveUpdateHandler(UpdateCallback callback)
        {
            foreach (var handler in _updateHandlers.ToList())
            {
                if (handler.UpdateCallback.Equals(callback))
                {
                    _updateHandlers.Remove(handler);
                }
            }
        }

        public T PushState<T>(object data = null) where T : StateBase, new ()
        {
            T state = new T();
            state.Initialize(data);
            _stateQueue.Enqueue(state);
            return state;
        }

        public void Pause()
        {
            _isPause = true;
        }

        public void Resume()
        {
            _isPause = false;
        }
        
        public void SetUpdateSpeed(float speed)
        {
            _updateSpeed = speed;
        }
    }
}