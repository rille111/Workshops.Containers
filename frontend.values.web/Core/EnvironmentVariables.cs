/// <summary>
/// Singleton
/// </summary>
public class EnvironmentVariables
{
    private static EnvironmentVariables _current;
    /// Singleton Instance
    public static EnvironmentVariables Current => _current ?? (_current = new EnvironmentVariables());

    public bool LOGS_ELASTICFORMATTING => nameof(LOGS_ELASTICFORMATTING).GetEnvValue().ToBoolean();
}

public static class EnvironmentVariableHelper
{
    public static string GetEnvValue(this string source)
    {
        return System.Environment.GetEnvironmentVariable(source);
    }
}

public static class StringHelper
{
    public static bool ToBoolean(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return false;
        if (source.Trim() == "1")
            return true;
        if (source.ToLower() == "true")
            return true;
        if (source.ToLower() == "yes")
            return true;
        return false;
    }
}