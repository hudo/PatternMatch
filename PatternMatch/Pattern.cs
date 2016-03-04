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

            public OtherwiseMatch<T, Nothing> Otherwise => new OtherwiseMatch<T, Nothing>(_inner);
        }

        public struct PatternMatch<T, R>
        {
            private readonly T _value;
            private R _result;


            public PatternMatch(T value)
            {
                _value = value;
                IsMatched = false;
                _result = default(R);
            }

            public PatternMatch<T, R> When(Func<T, bool> condition, Func<R> action)
            {
                if (!IsMatched && condition(_value))
                {
                    _result = action();
                    IsMatched = true;
                }

                return this;
            }

            public PatternMatch<T, R> When<C>(Func<C, R> action)
            {
                if (!IsMatched && _value is C)
                {
                    _result = action((C)(object)_value);
                    IsMatched = true;
                }
                return this;
            }

            public PatternMatch<T, R> When<C>(Func<C, bool> condition, Func<C, R> action)
            {
                if (!IsMatched && _value is C && condition((C)(object)_value))
                {
                    _result = action((C)(object)_value);
                    IsMatched = true;
                }
                return this;
            }


            public PatternMatch<T, R> When<C>(C value, Func<R> action)
            {
                if (!IsMatched && value.Equals(_value))
                {
                    _result = action();
                    IsMatched = true;
                }
                return this;
            }


            public R Result => _result;

            internal bool IsMatched { get; private set; }

            internal T Value => _value;
            
            public OtherwiseMatch<T,R> Otherwise => new OtherwiseMatch<T,R>(this);
        }

        public struct OtherwiseMatch<T, R>
        {
            private readonly PatternMatch<T, R> _patternMatch;

            public OtherwiseMatch(PatternMatch<T,R> patternMatch)
            {
                _patternMatch = patternMatch;
            }

            public R Default(Func<R> action)
            {
                return !_patternMatch.IsMatched ? action() : _patternMatch.Result;
            }

            public R Throw(string message = null)
            {
                if(!_patternMatch.IsMatched) throw new ArgumentException(string.IsNullOrEmpty(message) ? $"Match for [{_patternMatch.Value}] not found." : message);

                return _patternMatch.Result;
            }
        }
    }
}
