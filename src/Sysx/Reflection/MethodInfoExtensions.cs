namespace Sysx.Reflection;

public static class MethodInfoExtensions
{
    /// <summary>
    /// Checks if the other method matches the signature of this one.
    /// Signatures match when they have the same name, the same return type, and the same parameter types in the same order.
    /// </summary>
    public static bool MatchesSignature(this MethodInfo? methodInfo, MethodInfo? otherMethodInfo)
    {
        if (methodInfo == null) return false;
        if (otherMethodInfo == null) return false;
        if (methodInfo.Name != otherMethodInfo.Name) return false;
        if (methodInfo.ReturnType != otherMethodInfo.ReturnType) return false;

        var parameters = methodInfo.GetParameters();
        var otherParameters = otherMethodInfo.GetParameters();

        if (parameters.Length != otherParameters.Length) return false;

        for (var i = 0; i < parameters.Length; i++)
            if (parameters[i].ParameterType != otherParameters[i].ParameterType) return false;

        return true;
    }
}