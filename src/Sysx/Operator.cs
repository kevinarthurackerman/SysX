namespace Sysx;

/// <inheritdoc cref="Operator{TLeft, TRight, TResult}" />
public static class Operator
{
    /// <inheritdoc cref="Operator{TValue}.Zero" />
    public static TValue Zero<TValue>() => Operator<TValue>.Zero;

    /// <inheritdoc cref="Operator{TValue}.HasValue(TValue)" />
    public static bool HasValue<TValue>(TValue value) => Operator<TValue>.HasValue(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Not(TValue)" />
    public static TValue Not<TValue>(TValue value) => Operator<TValue, TValue>.Not(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Not(TValue)" />
    public static TResult Not<TValue, TResult>(TValue value) => Operator<TValue, TResult>.Not(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Increment(TValue)" />
    public static TValue Increment<TValue>(TValue value) => Operator<TValue, TValue>.Increment(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Increment(TValue)" />
    public static TResult Increment<TValue, TResult>(TValue value) => Operator<TValue, TResult>.Increment(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Decrement(TValue)" />
    public static TValue Decrement<TValue>(TValue value) => Operator<TValue, TValue>.Decrement(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Decrement(TValue)" />
    public static TResult Decrement<TValue, TResult>(TValue value) => Operator<TValue, TResult>.Decrement(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.UnaryPlus(TValue)" />
    public static TValue UnaryPlus<TValue>(TValue value) => Operator<TValue, TValue>.UnaryPlus(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.UnaryPlus(TValue)" />
    public static TResult UnaryPlus<TValue, TResult>(TValue value) => Operator<TValue, TResult>.UnaryPlus(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Negate(TValue)" />
    public static TValue Negate<TValue>(TValue value) => Operator<TValue, TValue>.Negate(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Negate(TValue)" />
    public static TResult Negate<TValue, TResult>(TValue value) => Operator<TValue, TResult>.Negate(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.NegateChecked(TValue)" />
    public static TValue NegateChecked<TValue>(TValue value) => Operator<TValue, TValue>.NegateChecked(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.NegateChecked(TValue)" />
    public static TResult NegateChecked<TValue, TResult>(TValue value) => Operator<TValue, TResult>.NegateChecked(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.OnesComplement(TValue)" />
    public static TValue OnesComplement<TValue>(TValue value) => Operator<TValue, TValue>.OnesComplement(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.OnesComplement(TValue)" />
    public static TResult OnesComplement<TValue, TResult>(TValue value) => Operator<TValue, TResult>.OnesComplement(value);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.And(TLeft, TRight)" />
    public static TValue And<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.And(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.And(TLeft, TRight)" />
    public static TResult And<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.And(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AndAlso(TLeft, TRight)" />
    public static TValue AndAlso<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.AndAlso(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AndAlso(TLeft, TRight)" />
    public static TResult AndAlso<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.AndAlso(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Or(TLeft, TRight)" />
    public static TValue Or<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Or(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Or(TLeft, TRight)" />
    public static TResult Or<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Or(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.OrElse(TLeft, TRight)" />
    public static TValue OrElse<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.OrElse(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.OrElse(TLeft, TRight)" />
    public static TResult OrElse<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.OrElse(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ExclusiveOr(TLeft, TRight)" />
    public static TValue ExclusiveOr<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ExclusiveOr(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ExclusiveOr(TLeft, TRight)" />
    public static TResult ExclusiveOr<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.ExclusiveOr(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Equal(TLeft, TRight)" />
    public static TValue Equal<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Equal(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Equal(TLeft, TRight)" />
    public static TResult Equal<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Equal(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.NotEqual(TLeft, TRight)" />
    public static TValue NotEqual<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.NotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.NotEqual(TLeft, TRight)" />
    public static TResult NotEqual<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.NotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceEqual(TLeft, TRight)" />
    public static TValue ReferenceEqual<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ReferenceEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceEqual(TLeft, TRight)" />
    public static TResult ReferenceEqual<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.ReferenceEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceNotEqual(TLeft, TRight)" />
    public static TValue ReferenceNotEqual<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ReferenceNotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceNotEqual(TLeft, TRight)" />
    public static TResult ReferenceNotEqual<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.ReferenceNotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThan(TLeft, TRight)" />
    public static TValue GreaterThan<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.GreaterThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThan(TLeft, TRight)" />
    public static TResult GreaterThan<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.GreaterThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThanOrEqual(TLeft, TRight)" />
    public static TValue GreaterThanOrEqual<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.GreaterThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThanOrEqual(TLeft, TRight)" />
    public static TResult GreaterThanOrEqual<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.GreaterThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThan(TLeft, TRight)" />
    public static TValue LessThan<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LessThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThan(TLeft, TRight)" />
    public static TResult LessThan<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.LessThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThanOrEqual(TLeft, TRight)" />
    public static TValue LessThanOrEqual<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LessThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThanOrEqual(TLeft, TRight)" />
    public static TResult LessThanOrEqual<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.LessThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Multiply(TLeft, TRight)" />
    public static TValue Multiply<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Multiply(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Multiply(TLeft, TRight)" />
    public static TResult Multiply<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Multiply(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.MultiplyChecked(TLeft, TRight)" />
    public static TValue MultiplyChecked<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.MultiplyChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.MultiplyChecked(TLeft, TRight)" />
    public static TResult MultiplyChecked<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.MultiplyChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Divide(TLeft, TRight)" />
    public static TValue Divide<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Divide(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Divide(TLeft, TRight)" />
    public static TResult Divide<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Divide(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Modulo(TLeft, TRight)" />
    public static TValue Modulo<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Modulo(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Modulo(TLeft, TRight)" />
    public static TResult Modulo<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Modulo(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Power(TLeft, TRight)" />
    public static TValue Power<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Power(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Power(TLeft, TRight)" />
    public static TResult Power<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Power(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Add(TLeft, TRight)" />
    public static TValue Add<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Add(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Add(TLeft, TRight)" />
    public static TResult Add<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Add(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AddChecked(TLeft, TRight)" />
    public static TValue AddChecked<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.AddChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AddChecked(TLeft, TRight)" />
    public static TResult AddChecked<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.AddChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Subtract(TLeft, TRight)" />
    public static TValue Subtract<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Subtract(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Subtract(TLeft, TRight)" />
    public static TResult Subtract<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Subtract(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.SubtractChecked(TLeft, TRight)" />
    public static TValue SubtractChecked<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.SubtractChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.SubtractChecked(TLeft, TRight)" />
    public static TResult SubtractChecked<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.SubtractChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LeftShift(TLeft, TRight)" />
    public static TValue LeftShift<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LeftShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LeftShift(TLeft, TRight)" />
    public static TResult LeftShift<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.LeftShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.RightShift(TLeft, TRight)" />
    public static TValue RightShift<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.RightShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.RightShift(TLeft, TRight)" />
    public static TResult RightShift<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.RightShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Coalesce(TLeft, TRight)" />
    public static TValue Coalesce<TValue>(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Coalesce(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Coalesce(TLeft, TRight)" />
    public static TResult Coalesce<TLeft, TRight, TResult>(TLeft left, TRight right) => Operator<TLeft, TRight, TResult>.Coalesce(left, right);
}

/// <inheritdoc cref="Operator{TLeft, TRight, TResult}" />
public static class Operator<TValue>
{
    private static INullOp<TValue> NullOp { get; }

    /// <summary>
    /// Returns the zero value for value-types (even for <see cref="Nullable{T}"/>) - or <see langword="null"/> for reference types.
    /// </summary>
    public static TValue Zero { get; }

    /// <summary>
    /// Indicates if the supplied value is non-<see langword="null"/>, for reference-types or <see cref="Nullable{T}"/>
    /// </summary>
    public static bool HasValue(TValue value) => NullOp.HasValue(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Not(TValue)" />
    public static TValue Not(TValue value) => Operator<TValue, TValue>.Not(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Increment(TValue)" />
    public static TValue Increment(TValue value) => Operator<TValue, TValue>.Increment(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Decrement(TValue)" />
    public static TValue Decrement(TValue value) => Operator<TValue, TValue>.Decrement(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.UnaryPlus(TValue)" />
    public static TValue UnaryPlus(TValue value) => Operator<TValue, TValue>.UnaryPlus(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.Negate(TValue)" />
    public static TValue Negate(TValue value) => Operator<TValue, TValue>.Negate(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.NegateChecked(TValue)" />
    public static TValue NegateChecked(TValue value) => Operator<TValue, TValue>.NegateChecked(value);

    /// <inheritdoc cref="Operator{TValue, TResult}.OnesComplement(TValue)" />
    public static TValue OnesComplement(TValue value) => Operator<TValue, TValue>.OnesComplement(value);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.And(TLeft, TRight)" />
    public static TValue And(TValue left, TValue right) => Operator<TValue, TValue, TValue>.And(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AndAlso(TLeft, TRight)" />
    public static TValue AndAlso(TValue left, TValue right) => Operator<TValue, TValue, TValue>.AndAlso(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Or(TLeft, TRight)" />
    public static TValue Or(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Or(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.OrElse(TLeft, TRight)" />
    public static TValue OrElse(TValue left, TValue right) => Operator<TValue, TValue, TValue>.OrElse(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ExclusiveOr(TLeft, TRight)" />
    public static TValue ExclusiveOr(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ExclusiveOr(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Equal(TLeft, TRight)" />
    public static TValue Equal(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Equal(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.NotEqual(TLeft, TRight)" />
    public static TValue NotEqual(TValue left, TValue right) => Operator<TValue, TValue, TValue>.NotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceEqual(TLeft, TRight)" />
    public static TValue ReferenceEqual(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ReferenceEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.ReferenceNotEqual(TLeft, TRight)" />
    public static TValue ReferenceNotEqual(TValue left, TValue right) => Operator<TValue, TValue, TValue>.ReferenceNotEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThan(TLeft, TRight)" />
    public static TValue GreaterThan(TValue left, TValue right) => Operator<TValue, TValue, TValue>.GreaterThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.GreaterThanOrEqual(TLeft, TRight)" />
    public static TValue GreaterThanOrEqual(TValue left, TValue right) => Operator<TValue, TValue, TValue>.GreaterThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThan(TLeft, TRight)" />
    public static TValue LessThan(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LessThan(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LessThanOrEqual(TLeft, TRight)" />
    public static TValue LessThanOrEqual(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LessThanOrEqual(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Multiply(TLeft, TRight)" />
    public static TValue Multiply(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Multiply(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.MultiplyChecked(TLeft, TRight)" />
    public static TValue MultiplyChecked(TValue left, TValue right) => Operator<TValue, TValue, TValue>.MultiplyChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Divide(TLeft, TRight)" />
    public static TValue Divide(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Divide(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Modulo(TLeft, TRight)" />
    public static TValue Modulo(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Modulo(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Power(TLeft, TRight)" />
    public static TValue Power(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Power(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Add(TLeft, TRight)" />
    public static TValue Add(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Add(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.AddChecked(TLeft, TRight)" />
    public static TValue AddChecked(TValue left, TValue right) => Operator<TValue, TValue, TValue>.AddChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Subtract(TLeft, TRight)" />
    public static TValue Subtract(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Subtract(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.SubtractChecked(TLeft, TRight)" />
    public static TValue SubtractChecked(TValue left, TValue right) => Operator<TValue, TValue, TValue>.SubtractChecked(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.LeftShift(TLeft, TRight)" />
    public static TValue LeftShift(TValue left, TValue right) => Operator<TValue, TValue, TValue>.LeftShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.RightShift(TLeft, TRight)" />
    public static TValue RightShift(TValue left, TValue right) => Operator<TValue, TValue, TValue>.RightShift(left, right);

    /// <inheritdoc cref="Operator{TLeft, TRight, TResult}.Coalesce(TLeft, TRight)" />
    public static TValue Coalesce(TValue left, TValue right) => Operator<TValue, TValue, TValue>.Coalesce(left, right);

    static Operator()
    {
        if (typeof(TValue).IsValueType && typeof(TValue).IsGenericType && (typeof(TValue).GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
            // get the *inner* zero (not a null Nullable<TValue>, but default(TValue))
            Type nullType = (typeof(TValue).GetGenericArguments()[0] ?? throw new NullReferenceException());
            Zero = (TValue)Activator.CreateInstance(nullType)!;
            NullOp = (INullOp<TValue>)Activator.CreateInstance(typeof(StructNullOp<>).MakeGenericType(nullType))!;
        }
        else
        {
            Zero = default!;
            if (typeof(TValue).IsValueType)
            {
                NullOp = (INullOp<TValue>)Activator.CreateInstance(typeof(StructNullOp<>).MakeGenericType(typeof(TValue)))!;
            }
            else
            {
                NullOp = (INullOp<TValue>)Activator.CreateInstance(typeof(ClassNullOp<>).MakeGenericType(typeof(TValue)))!;
            }
        }
    }
}

/// <inheritdoc cref="Operator{TLeft, TRight, TResult}" />
public static class Operator<TValue, TResult>
{
    private static readonly Func<TValue, TResult> not = ExpressionX.Function<TValue, TResult>(Expression.Not, "Not");
    private static readonly Func<TValue, TResult> increment = ExpressionX.Function<TValue, TResult>(Expression.Increment, "Increment");
    private static readonly Func<TValue, TResult> decrement = ExpressionX.Function<TValue, TResult>(Expression.Decrement, "Decrement");
    private static readonly Func<TValue, TResult> unaryPlus = ExpressionX.Function<TValue, TResult>(Expression.UnaryPlus, "UnaryPlus");
    private static readonly Func<TValue, TResult> negate = ExpressionX.Function<TValue, TResult>(Expression.Negate, "Negate");
    private static readonly Func<TValue, TResult> negateChecked = ExpressionX.Function<TValue, TResult>(Expression.NegateChecked, "NegateChecked");
    private static readonly Func<TValue, TResult> onesComplement = ExpressionX.Function<TValue, TResult>(Expression.OnesComplement, "OnesComplement");

    /// <summary>
    /// Executes the unary Not <see langword="operator" /> (<c>!x</c>).
    /// </summary>
    public static TResult Not(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return not(value);
    }

    /// <summary>
    /// Executes the unary Increment <see langword="operator" /> (<c>x+1</c>).
    /// </summary>
    public static TResult Increment(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return increment(value);
    }

    /// <summary>
    /// Executes the unary Decrement <see langword="operator" /> (<c>x-1</c>).
    /// </summary>
    public static TResult Decrement(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return decrement(value);
    }

    /// <summary>
    /// Executes the unary UnaryPlus <see langword="operator" /> (<c>+x</c>).
    /// </summary>
    public static TResult UnaryPlus(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));
        
        return unaryPlus(value);
    }

    /// <summary>
    /// Executes the unary Negate <see langword="operator" /> (<c>-x</c>).
    /// </summary>
    public static TResult Negate(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return negate(value);
    }

    /// <summary>
    /// Executes the unary NegateChecked <see langword="operator" /> (<c>checked(-x)</c>).
    /// </summary>
    public static TResult NegateChecked(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return negateChecked(value);
    }

    /// <summary>
    /// Executes the unary OnesComplement <see langword="operator" /> (<c>~x</c>).
    /// </summary>
    public static TResult OnesComplement(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return onesComplement(value);
    }
}

/// <summary>
/// Provides methods for executing operations on any valid <see cref="Type" />.
/// </summary>
public static class Operator<TLeft, TRight, TResult>
{
    private static readonly Func<TLeft, TRight, TResult> and = ExpressionX.Function<TLeft, TRight, TResult>(Expression.And, "And");
    private static readonly Func<TLeft, TRight, TResult> andAlso = ExpressionX.Function<TLeft, TRight, TResult>(Expression.AndAlso, "AndAlso");
    private static readonly Func<TLeft, TRight, TResult> or = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Or, "Or");
    private static readonly Func<TLeft, TRight, TResult> orElse = ExpressionX.Function<TLeft, TRight, TResult>(Expression.OrElse, "OrElse");
    private static readonly Func<TLeft, TRight, TResult> exclusiveOr = ExpressionX.Function<TLeft, TRight, TResult>(Expression.ExclusiveOr, "ExclusiveOr");
    private static readonly Func<TLeft, TRight, TResult> equal = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Equal, "Equal");
    private static readonly Func<TLeft, TRight, TResult> notEqual = ExpressionX.Function<TLeft, TRight, TResult>(Expression.NotEqual, "NotEqual");
    private static readonly Func<TLeft, TRight, TResult> referenceEqual = ExpressionX.Function<TLeft, TRight, TResult>(Expression.ReferenceEqual, "ReferenceEqual");
    private static readonly Func<TLeft, TRight, TResult> referenceNotEqual = ExpressionX.Function<TLeft, TRight, TResult>(Expression.ReferenceNotEqual, "ReferenceNotEqual");
    private static readonly Func<TLeft, TRight, TResult> greaterThan = ExpressionX.Function<TLeft, TRight, TResult>(Expression.GreaterThan, "GreaterThan");
    private static readonly Func<TLeft, TRight, TResult> greaterThanOrEqual = ExpressionX.Function<TLeft, TRight, TResult>(Expression.GreaterThanOrEqual, "GreaterThanOrEqual");
    private static readonly Func<TLeft, TRight, TResult> lessThan = ExpressionX.Function<TLeft, TRight, TResult>(Expression.LessThan, "LessThan");
    private static readonly Func<TLeft, TRight, TResult> lessThanOrEqual = ExpressionX.Function<TLeft, TRight, TResult>(Expression.LessThanOrEqual, "LessThanOrEqual");
    private static readonly Func<TLeft, TRight, TResult> multiply = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Multiply, "Multiply");
    private static readonly Func<TLeft, TRight, TResult> multiplyChecked = ExpressionX.Function<TLeft, TRight, TResult>(Expression.MultiplyChecked, "MultiplyChecked");
    private static readonly Func<TLeft, TRight, TResult> divide = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Divide, "Divide");
    private static readonly Func<TLeft, TRight, TResult> modulo = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Modulo, "Modulo");
    private static readonly Func<TLeft, TRight, TResult> power = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Power, "Power");
    private static readonly Func<TLeft, TRight, TResult> add = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Add, "Add");
    private static readonly Func<TLeft, TRight, TResult> addChecked = ExpressionX.Function<TLeft, TRight, TResult>(Expression.AddChecked, "AddChecked");
    private static readonly Func<TLeft, TRight, TResult> subtract = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Subtract, "Subtract");
    private static readonly Func<TLeft, TRight, TResult> subtractChecked = ExpressionX.Function<TLeft, TRight, TResult>(Expression.SubtractChecked, "SubtractChecked");
    private static readonly Func<TLeft, TRight, TResult> leftShift = ExpressionX.Function<TLeft, TRight, TResult>(Expression.LeftShift, "LeftShift");
    private static readonly Func<TLeft, TRight, TResult> rightShift = ExpressionX.Function<TLeft, TRight, TResult>(Expression.RightShift, "RightShift");
    private static readonly Func<TLeft, TRight, TResult> coalesce = ExpressionX.Function<TLeft, TRight, TResult>(Expression.Coalesce, "Coalesce");

    /// <summary>
    /// Executes the binary And <see langword="operator" /> (<c>x&y</c>).
    /// </summary>
    public static TResult And(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return and(left, right);
    }

    /// <summary>
    /// Executes the binary AndAlso <see langword="operator" /> (<c>x&&y</c>).
    /// </summary>
    public static TResult AndAlso(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return andAlso(left, right);
    }

    /// <summary>
    /// Executes the binary Or <see langword="operator" /> (<c>x|y</c>).
    /// </summary>
    public static TResult Or(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return or(left, right);
    }

    /// <summary>
    /// Executes the binary OrElse <see langword="operator" /> (<c>x||y</c>).
    /// </summary>
    public static TResult OrElse(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return orElse(left, right);
    }

    /// <summary>
    /// Executes the binary ExclusiveOr <see langword="operator" /> (<c>x^y</c>).
    /// </summary>
    public static TResult ExclusiveOr(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return exclusiveOr(left, right);
    }

    /// <summary>
    /// Executes the binary Equal <see langword="operator" /> (<c>x==y</c>).
    /// </summary>
    public static TResult Equal(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return equal(left, right);
    }

    /// <summary>
    /// Executes the binary NotEqual <see langword="operator" /> (<c>x!=y</c>).
    /// </summary>
    public static TResult NotEqual(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return notEqual(left, right);
    }

    /// <summary>
    /// Executes the binary ReferenceEqual <see langword="operator" /> (<c>System.Object.ReferenceEquals(x,y)</c>).
    /// </summary>
    public static TResult ReferenceEqual(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return referenceEqual(left, right);
    }

    /// <summary>
    /// Executes the binary ReferenceNotEqual <see langword="operator" /> (<c>!System.Object.ReferenceEquals(x,y)</c>).
    /// </summary>
    public static TResult ReferenceNotEqual(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return referenceNotEqual(left, right);
    }

    /// <summary>
    /// Executes the binary GreaterThan <see langword="operator" /> (<c>x>y</c>).
    /// </summary>
    public static TResult GreaterThan(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return greaterThan(left, right);
    }

    /// <summary>
    /// Executes the binary GreaterThanOrEqual <see langword="operator" /> (<c>x>=y</c>).
    /// </summary>
    public static TResult GreaterThanOrEqual(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return greaterThanOrEqual(left, right);
    }

    /// <summary>
    /// Executes the binary LessThan <see langword="operator" /> (<c>x<y</c>).
    /// </summary>
    public static TResult LessThan(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return lessThan(left, right);
    }

    /// <summary>
    /// Executes the binary LessThanOrEqual <see langword="operator" /> (<c>x<=y</c>).
    /// </summary>
    public static TResult LessThanOrEqual(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return lessThanOrEqual(left, right);
    }

    /// <summary>
    /// Executes the binary Multiply <see langword="operator" /> (<c>x*y</c>).
    /// </summary>
    public static TResult Multiply(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return multiply(left, right);
    }

    /// <summary>
    /// Executes the binary MultiplyChecked <see langword="operator" /> (<c>checked(x*y)</c>).
    /// </summary>
    public static TResult MultiplyChecked(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return multiplyChecked(left, right);
    }

    /// <summary>
    /// Executes the binary Divide <see langword="operator" /> (<c>x/y</c>).
    /// </summary>
    public static TResult Divide(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return divide(left, right);
    }

    /// <summary>
    /// Executes the binary Modulo <see langword="operator" /> (<c>x%y</c>).
    /// </summary>
    public static TResult Modulo(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return modulo(left, right);
    }

    /// <summary>
    /// Executes the binary Power <see langword="operator" /> (<c>x^y</c>).
    /// </summary>
    public static TResult Power(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return power(left, right);
    }

    /// <summary>
    /// Executes the binary Add <see langword="operator" /> (<c>x+y</c>).
    /// </summary>
    public static TResult Add(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return add(left, right);
    }

    /// <summary>
    /// Executes the binary AddChecked <see langword="operator" /> (<c>checked(x+y)</c>).
    /// </summary>
    public static TResult AddChecked(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return addChecked(left, right);
    }

    /// <summary>
    /// Executes the binary Subtract <see langword="operator" /> (<c>x-y</c>).
    /// </summary>
    public static TResult Subtract(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return subtract(left, right);
    }

    /// <summary>
    /// Executes the binary SubtractChecked <see langword="operator" /> (<c>x-y</c>).
    /// </summary>
    public static TResult SubtractChecked(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return subtractChecked(left, right);
    }

    /// <summary>
    /// Executes the binary LeftShift <see langword="operator" /> (<c>x<<y</c>).
    /// </summary>
    public static TResult LeftShift(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return leftShift(left, right);
    }

    /// <summary>
    /// Executes the binary RightShift <see langword="operator" /> (<c>x>>y</c>).
    /// </summary>
    public static TResult RightShift(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return rightShift(left, right);
    }

    /// <summary>
    /// Executes the binary Coalesce <see langword="operator" /> (<c>x??y</c>).
    /// </summary>
    public static TResult Coalesce(TLeft left, TRight right)
    {
        EnsureArg.HasValue(left, nameof(left));
        EnsureArg.HasValue(right, nameof(right));

        return coalesce(left, right);
    }
}
