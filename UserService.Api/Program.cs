using System.Text.Json;
using UserService.Api;
using UserService.Contract.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost(ApiEndpoints.Create, async (HttpContext context) =>
{
    var createUserRequest = (CreateUserRequest?)null;
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

    // var isCreated = repo.Create(user);

    var userResponse = user.MapToResponse();
    return Results.Ok(userResponse);
});

app.MapGet(ApiEndpoints.GetByCredentials, (HttpRequest request) =>
{
    var credentials = request.Query.MapToGetUserByCredentialsRequest();
    var username = credentials.Username;
    var password = credentials.Password;

    //var user = repo.GetUserByCredentials(username, password)
    //var response = user.MapToResponse();
    var response = "user.MapToResponse()";
    return Results.Ok(response);
});

app.MapGet(ApiEndpoints.GetById, (int id) =>
{
    // var user = repo.GetUserById(id);
    // var response = user.MapToResponse();

    return Results.Ok($"Get by Id: {id}");
});

app.MapPut(ApiEndpoints.Update, async (HttpRequest request) =>
{
    var updateUserRequest = (UpdateUserRequest?)null;
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
});

app.Run();