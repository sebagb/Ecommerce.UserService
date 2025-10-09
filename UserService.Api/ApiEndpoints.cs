using System.Text.Json;
using UserService.Contract.Requests;

namespace UserService.Api;

public static class ApiEndpoints
{
    private const string Base = "users";
    private static string Create { get; } = $"{Base}";
    private static string GetByCredentials { get; } = $"{Base}";
    private static string GetById { get; } = $"{Base}/{{id}}";
    private static string Update { get; } = $"{Base}";

    public static void RegisterUserEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(Create, (Delegate)CreateUser);
        builder.MapGet(GetByCredentials, GetUserByCredentials);
        builder.MapGet(GetById, GetUserById);
        builder.MapPut(Update, UpdateUser);
    }

    private static async Task<IResult> CreateUser(
        HttpContext context)
    {
        CreateUserRequest? createUserRequest;
        try
        {
            createUserRequest =
                await context.Request.ReadFromJsonAsync<CreateUserRequest>();
        }
        catch (JsonException)
        {
            return Results.BadRequest("Failed to deserialize request body");
        }

        var user = createUserRequest!.MapToUser();

        //var isCreated = repo.Create(user);

        var userResponse = user.MapToResponse();
        return Results.Ok(userResponse);
    }

    private static IResult GetUserByCredentials(HttpRequest request)
    {
        var credentials = request.Query.MapToGetUserByCredentialsRequest();
        var username = credentials.Username;
        var password = credentials.Password;

        //var user = repo.GetUserByCredentials(username, password)
        //var response = user.MapToResponse();
        var response = "user.MapToResponse()";
        return Results.Ok(response);
    }

    private static IResult GetUserById(int id)
    {
        // var user = repo.GetUserById(id);
        // var response = user.MapToResponse();

        return Results.Ok($"Get by Id: {id}");
    }

    private static async Task<IResult> UpdateUser(HttpRequest request)
    {
        UpdateUserRequest? updateUserRequest;
        try
        {
            updateUserRequest =
                await request.ReadFromJsonAsync<UpdateUserRequest>();
        }
        catch (JsonException)
        {
            return Results.BadRequest("Failed to deserialize request body");
        }

        var id = Guid.NewGuid(); // take from jwt
        var user = updateUserRequest.MapToUser(id);

        // var isUpdated = repo.Update(user)

        var response = user.MapToResponse();
        return Results.Ok(response);
    }
}