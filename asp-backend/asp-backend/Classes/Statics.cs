using System.Text.Json;
using asp_backend.Contexts;
using asp_backend.Controllers;
using Microsoft.AspNetCore.Identity;

namespace asp_backend;

public static class Statics
{
    public static readonly UserContext _userContext = new ();
    public static readonly PasswordHasher<User> _hasher = new ();
    public static List<Lobby> _lobbies = new ();
    public static Dictionary<int, string> _usersInLobbies = new ();

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions{IncludeFields = true});
    }
}