using System;

    public class NoneValueException : Exception
    {
        public NoneValueException() {}
        public NoneValueException(string message) : base(message) {}
    }
    
    /// <summary>
    /// Like Monad but is not monad
    /// </summary>
    public readonly struct Maybe<T>
    {
        private readonly T _value;
        
        /// <remarks>
        /// Check <see cref="HasValue"/> before call or use one of 'GetValueOrSomething' methods.
        /// </remarks>
        /// <exception cref="NoneValueException">Throws if monad was created via <see cref="Maybe.None(string)"/></exception>
        public T Value 
            => HasValue ? _value : throw new NoneValueException(Reason);

        public bool HasValue { get; }
        
        public string Reason { get; }
        
        private Maybe(string reason)
        {
            HasValue = false;
            _value = default;
            Reason = reason;
        }
        
        private Maybe(T value)
        {
            HasValue = true;
            Reason = default;
            _value = value;
        }

        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (HasValue)
            {
                if (some == null) throw new ArgumentNullException(nameof(some));
                return some.Invoke(Value);
            }

            if (none == null) throw new ArgumentNullException(nameof(none));
            return none.Invoke();
        }

        public void Match(Action<T> some, Action none)
        {
            if (HasValue)
            {
                if (some == null) throw new ArgumentNullException(nameof(some));
                some.Invoke(Value);
            }
            else
            {
                if (none == null) throw new ArgumentNullException(nameof(none));
                none.Invoke();
            }
        }

        public static implicit operator Maybe<T>(T someValue) 
            => new Maybe<T>(someValue);

        public static implicit operator Maybe<T>(in Maybe.MaybeNone value) 
            => new Maybe<T>(value.Reason);

        public static implicit operator bool(in Maybe<T> value) 
            => value.HasValue;
        
        public bool TryGetValue(out T value)
        {
            if (HasValue)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        public T GetValueOrDefault() 
            => HasValue ? _value : default;

        public T GetValueOr(T defaultValue) 
            => HasValue ? _value : defaultValue;

        public T GetValueOr(Func<T> defaultValueFactory) 
            => HasValue ? _value : defaultValueFactory != null 
                    ? defaultValueFactory.Invoke() 
                    : throw new ArgumentNullException(nameof(defaultValueFactory));

        public Maybe<T> GetValueOrMaybe(in Maybe<T> alternativeValue) 
            => HasValue ? this : alternativeValue;

        public Maybe<T> GetValueOrMaybe(Func<Maybe<T>> alternativeValueFactory) 
            => HasValue ? this : alternativeValueFactory?.Invoke() ?? throw new ArgumentNullException(nameof(alternativeValueFactory));

        public T GetValueOrThrow(string errorMessage) 
            => HasValue ? _value : throw new NoneValueException(errorMessage);
    }

    public static class Maybe
    {
        public readonly struct MaybeNone
        {
            public string Reason { get; }
            public MaybeNone(string reason) { Reason = reason; }
        }

        public static MaybeNone None(string reason = null) 
            => new MaybeNone(reason);

        /// <preconditions>
        /// Preconditions: value must be not null.
        /// </preconditions>
        /// <remarks>
        /// Check <see cref="value"/> is null before call or use <see cref="None"/> method instead.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Throws if <see cref="value"/> is null.</exception>
        public static Maybe<T> Some<T>(T value) 
            => value;
    }

    public readonly struct Maybe<TValue, TReason>
    {
        public bool HasValue { get; }
        public TValue Value { get; }
        public TReason Reason { get; }

        private Maybe(bool hasValue, TValue value, TReason reason)
        {
            HasValue = hasValue;
            Value = value;
            Reason = reason;
        }

        public static Maybe<TValue, TReason> Ok(TValue value)
        {
            var result = new Maybe<TValue, TReason>(true, value, default);
            return result;
        }

        public static Maybe<TValue, TReason> Error(TReason reason)
        {
            var result = new Maybe<TValue, TReason>(false, default, reason);
            return result;
        }
    }
