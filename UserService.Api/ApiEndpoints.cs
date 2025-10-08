namespace UserService.Api;

public static class ApiEndpoints
{
    private const string Base = "users";
    public static string Create { get; } = $"{Base}";
    public static string GetByCredentials { get; } = $"{Base}";
    public static string GetById { get; } = $"{Base}/{{id}}";
    public static string Update { get; } = $"{Base}";
}