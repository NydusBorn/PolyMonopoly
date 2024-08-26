using asp_backend.Contexts;
using Microsoft.AspNetCore.Identity;

namespace asp_backend;

public static class Statics
{
    public static readonly UserContext _userContext = new ();
    public static readonly PasswordHasher<User> _hasher = new ();
}