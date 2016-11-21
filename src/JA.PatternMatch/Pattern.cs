using System;

namespace JA.PatternMatch
{
    /// <summary>
    /// Poors man pattern matching. 
    /// </summary>
    public static class Pattern
    {

        /// <summary>
        /// Match pattern of <see cref="T"/> to value of <see cref="R"/>
        /// </summary>
        /// <typeparam name="T">Pattern type</typeparam>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="value">Input value</param>
        /// <returns></returns>
        public static PatternMatch<T, R> Match<T, R>(T value)
        {
            return new PatternMatch<T, R>(value);
        }

        /// <summary>
        /// Match pattern of <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">Pattern type</typeparam>
        /// <param name="value">Value</param>
        /// <returns>Nothing</returns>
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

            /// <summary>
            /// Match condition 
            /// </summary>
            /// <param name="condition">Condition to evaluate</param>
            /// <param name="action">Action to call if pattern match</param>
            public PatternMatch<T> When(Func<T, bool> condition, Action action)
            {
                _inner.When(condition, () =>
                {
                    action();
                    return Nothing.Instance;
                });

                return this;
            }

            /// <summary>
            /// Match with type <see cref="C"/>
            /// </summary>
            /// <typeparam name="C">Type <see cref="C"/> to compare with <see cref="T"/></typeparam>
            /// <param name="action">Action to call if type match</param>
            public PatternMatch<T> When<C>(Action<C> action)
            {
                _inner.When<C>(c =>
                {
                    action(c);
                    return Nothing.Instance;
                });

                return this;
            }

            /// <summary>
            /// Match condition
            /// </summary>
            /// <param name="condition">Condition to evaluate</param>
            /// <param name="action">Action to call if pattern match with value of <see cref="C"/></param>
            public PatternMatch<T> When<C>(Func<C, bool> condition, Action<C> action)
            {
                _inner.When<C>(condition, c =>
                {
                    action(c);
                    return Nothing.Instance;
                });
                return this;
            }

            /// <summary>
            /// Match with value
            /// </summary>
            /// <param name="value">Value to match</param>
            /// <param name="action">Action to call if value match</param>
            public PatternMatch<T> When<C>(C value, Action action)
            {
                _inner.When<C>(value,
                () =>
                {
                    action();
                    return Nothing.Instance;
                });

                return this;
            }

            /// <summary>
            /// If other patterns didn't match
            /// </summary>
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

            /// <summary>
            /// Pattern match condition
            /// </summary>
            /// <param name="condition">Condition to evaluate</param>
            /// <param name="action">Action to call if pattern match</param>
            public PatternMatch<T, R> When(Func<T, bool> condition, Func<R> action)
            {
                if (!IsMatched && condition(_value))
                {
                    _result = action();
                    IsMatched = true;
                }

                return this;
            }

            /// <summary>
            /// Match type <see cref="T"/>
            /// </summary>
            /// <typeparam name="C">Type to compare with <see cref="T"/></typeparam>
            /// <param name="action">Action to call if type match</param>
            public PatternMatch<T, R> When<C>(Func<C, R> action)
            {
                if (!IsMatched && _value is C)
                {
                    _result = action((C)(object)_value);
                    IsMatched = true;
                }
                return this;
            }

            /// <summary>
            /// Match condition
            /// </summary>
            /// <param name="condition">Condition to evaluate</param>
            /// <param name="action">Action to call if condition is true</param>
            public PatternMatch<T, R> When<C>(Func<C, bool> condition, Func<C, R> action)
            {
                if (!IsMatched && _value is C && condition((C)(object)_value))
                {
                    _result = action((C)(object)_value);
                    IsMatched = true;
                }
                return this;
            }

            /// <summary>
            /// Match value
            /// </summary>
            /// <param name="value">Match value</param>
            /// <param name="action">Action to call if equals to pattern value</param>
            public PatternMatch<T, R> When<C>(C value, Func<R> action)
            {
                if (!IsMatched && value.Equals(_value))
                {
                    _result = action();
                    IsMatched = true;
                }
                return this;
            }

            /// <summary>
            /// Result value of pattern matching
            /// </summary>
            public R Result => _result;

            internal bool IsMatched { get; private set; }

            internal T Value => _value;

            /// <summary>
            /// If other patterns didn't match
            /// </summary>
            public OtherwiseMatch<T,R> Otherwise => new OtherwiseMatch<T,R>(this);
        }

        public struct OtherwiseMatch<T, R>
        {
            private readonly PatternMatch<T, R> _patternMatch;

            public OtherwiseMatch(PatternMatch<T,R> patternMatch)
            {
                _patternMatch = patternMatch;
            }

            /// <summary>
            /// Default action if other patterns didn't match
            /// </summary>
            public R Default(Func<R> action)
            {
                return !_patternMatch.IsMatched ? action() : _patternMatch.Result;
            }

            /// <summary>
            /// Throws exception if pattern is not matched
            /// </summary>
            /// <param name="message">Exception message</param>
            /// <returns>Value if pattern is matched</returns>
            public R Throw(string message = null)
            {
                if(!_patternMatch.IsMatched) throw new ArgumentException(string.IsNullOrEmpty(message) ? $"Match for [{_patternMatch.Value}] not found." : message);

                return _patternMatch.Result;
            }
        }
    }
}
