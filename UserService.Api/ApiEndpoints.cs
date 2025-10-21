using System.Text.Json;
using UserService.Api.Auth;
using UserService.Application.Repositories;
using UserService.Contract.Requests;
using UserService.Contract.Responses;

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
        HttpContext context, IUserRepository repo)
    {
        CreateUserRequest? createUserRequest;
        try
        {
            createUserRequest =
                await context.Request.ReadFromJsonAsync<CreateUserRequest>();
        }
        catch (Exception ex)
        {
            if (ex is JsonException || ex is InvalidOperationException)
            {
                return Results.BadRequest(ex.Message);
            }
            throw;
        }

        var user = createUserRequest!.MapToUser();

        repo.Create(user);

        var userResponse = user.MapToResponse();
        return Results.Ok(userResponse);
    }

    private static IResult GetUserByCredentials(
        HttpRequest request,
        IUserRepository repo)
    {
        var credentials = request.Query.MapToGetUserByCredentialsRequest();
        var username = credentials.Username;
        var password = credentials.Password;

        var user = repo.GetByCredentials(username, password);

        if (user == null)
        {
            return Results.NotFound();
        }

        var jwt = UserToken.CreateUserToken(user);

        var response = new JwtResponse { Access_token = jwt };

        return Results.Ok(response);
    }

    private static IResult GetUserById(Guid id, IUserRepository repo)
    {
        var user = repo.GetById(id);
        if (user == null)
        {
            return Results.NoContent();
        }
        var response = user.MapToResponse();
        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateUser(
        HttpContext context,
        IUserRepository repo)
    {
        var userIdClaim = context.User.FindFirst(AuthConstants.UserIdClaimName);
        if (userIdClaim == null)
        {
            return Results.BadRequest("Invalid JWT");
        }

        UpdateUserRequest? updateUserRequest;
        try
        {
            updateUserRequest =
                await context.Request.ReadFromJsonAsync<UpdateUserRequest>();
        }
        catch (Exception ex)
        {
            if (ex is JsonException || ex is InvalidOperationException)
            {
                return Results.BadRequest(ex.Message);
            }
            throw;
        }

        var userId = new Guid(userIdClaim.Value);
        var user = updateUserRequest.MapToUser(userId);

        var isUpdated = repo.Update(user);
        if (!isUpdated)
        {
            return Results.InternalServerError("User is not updated");
        }

        var response = user.MapToResponse();
        return Results.Ok(response);
    }
}