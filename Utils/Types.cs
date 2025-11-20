using System;

namespace Replay.Utils
{
    public class Lazy<T>
    {
        private volatile bool _isInitialized;
        private T _value;
        private readonly Func<T> _initializer;
        private readonly object _lock = new object();

        public T Value
        {
            get
            {
                if (!_isInitialized)
                {
                    lock (_lock)
                    {
                        if (!_isInitialized)
                        {
                            try
                            {
                                _value = _initializer();
                            }
                            finally
                            {
                                _isInitialized = true;
                            }
                        }
                    }
                }
                return _value;
            }
        }

        public bool IsValueCreated => _isInitialized;

        public void Reset()
        {
            lock (_lock)
            {
                _isInitialized = false;
                _value = default;
            }
        }

        public static implicit operator T(Lazy<T> l) => l.Value;

        public Lazy(Func<T> initializer)
        {
            _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
            _isInitialized = false;
        }
    }
}