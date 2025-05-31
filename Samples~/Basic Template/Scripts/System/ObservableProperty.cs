using System;

namespace DevelopKit.BasicTemplate
{
    public class ObservableProperty<T>
    {
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }

                _value = value;
                _onValueChanged?.Invoke(_value);
            }
        }

        private T _value;
        private event Action<T> _onValueChanged;
        public event Action<T> OnValueChanged
        {
            add => _onValueChanged += value;
            remove => _onValueChanged -= value;
        }

        public ObservableProperty(T value)
        {
            _value = value;
        }
    }
}