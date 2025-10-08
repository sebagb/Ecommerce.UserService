using UserService.Application.Database;
using UserService.Application.Models;

namespace UserService.Application.Repositories;

public class UserRepository(RepositoryDbContext context) : IUserRepository
{
    private readonly RepositoryDbContext context = context;

    public bool Create(User user)
    {
        context.User.Add(user);
        return context.SaveChanges() > 0;
    }

    public User? GetByCredentials(string username, string password)
    {
        return context.User.SingleOrDefault(x =>
            x.Username.Equals(username) && x.Password.Equals(password));
    }

    public User? GetById(Guid id)
    {
        return context.User.SingleOrDefault(x => x.Id == id);
    }

    public bool Update(User user)
    {
        var old = context.User.Single(x => x.Id == user.Id);
        old = user;
        return context.SaveChanges() > 0;
    }
}