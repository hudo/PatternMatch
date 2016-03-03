using System;

namespace PatternMatch
{
    /// <summary>
    /// Poor mans pattern matching. 
    /// Use it instead of switch/case, for more functional approach
    /// </summary>
    public static class Pattern
    {
        public static PatternMatch<T, R> Match<T, R>(T value)
        {
            return new PatternMatch<T, R>(value);
        }

        public static PatternMatch<T> Match<T>(T value)
        {
            return new PatternMatch<T>(value);
        }

        public struct PatternMatch<T>
        {
            private PatternMatch<T, Nothing> _inner;

            public PatternMatch(T value)
            {
                _inner = new PatternMatch<T, Nothing>(value);
            }

            public PatternMatch<T> When(Func<T, bool> condition, Action action)
            {
                _inner.When(condition, () => { action(); return Nothing.Instance; });

                return this;
            }

            public PatternMatch<T> When<C>(Action<C> action)
            {
                _inner.When<C>(c => { action(c); return Nothing.Instance; });
                return this;
            }

            public PatternMatch<T> When<C>(Func<C, bool> condition, Action<C> action)
            {
                _inner.When<C>(condition, c => { action(c); return Nothing.Instance; });
                return this;
            }

            public PatternMatch<T> When<C>(C value, Action action)
            {
                _inner.When<C>(value, () => { action(); return Nothing.Instance; });
                return this;
            }

            public void Default(Action action)
            {
                _inner.Default(() => { action(); return Nothing.Instance; });
            }

            public void ElseThrow(string message = null)
            {
                _inner.ElseThrow(message);
            }
        }

        public struct PatternMatch<T, R>
        {
            private readonly T _value;
            private R _result;

            private bool _matched;

            public PatternMatch(T value)
            {
                _value = value;
                _matched = false;
                _result = default(R);
            }

            public PatternMatch<T, R> When(Func<T, bool> condition, Func<R> action)
            {
                if (!_matched && condition(_value))
                {
                    _result = action();
                    _matched = true;
                }

                return this;
            }

            public PatternMatch<T, R> When<C>(Func<C, R> action)
            {
                if (!_matched && _value is C)
                {
                    _result = action((C)(object)_value);
                    _matched = true;
                }
                return this;
            }

            public PatternMatch<T, R> When<C>(Func<C, bool> condition, Func<C, R> action)
            {
                if (!_matched && _value is C && condition((C)(object)_value))
                {
                    _result = action((C)(object)_value);
                    _matched = true;
                }
                return this;
            }


            public PatternMatch<T, R> When<C>(C value, Func<R> action)
            {
                if (!_matched && value.Equals(_value))
                {
                    _result = action();
                    _matched = true;
                }
                return this;
            }


            public R Result => _result;

            public R Default(Func<R> action)
            {
                return !_matched ? action() : _result;
            }

            public R ElseThrow(string message = null)
            {
                if(!_matched) throw new ArgumentException(string.IsNullOrEmpty(message) ? $"Match for [{_value}] not found." : message);

                return _result;
            }
        }

        /// <summary>
        /// This class can be used instead of void for return types.
        /// </summary>
        private class Nothing
        {
            public static readonly Nothing Instance = new Nothing();

            private Nothing() { }

            public override string ToString()
            {
                return "Nothing";
            }
        }
    }
}
