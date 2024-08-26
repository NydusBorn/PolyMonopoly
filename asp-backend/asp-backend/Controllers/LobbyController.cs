using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using asp_backend.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace asp_backend.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class LobbyController : ControllerBase
{
    List<Lobby> _lobbies = new ();
    Dictionary<int, string> _usersInLobbies = new ();

    [HttpGet]
    public List<Tuple<string, string, int>> GetLobbies()
    {
        List<Tuple<string, string, int>> found_lobbies = new();
        foreach (var lobby in _lobbies)
        {
            found_lobbies.Add(new (lobby.Id, lobby.GameName, lobby.Participants.Count));
        }

        return found_lobbies;
    }

    [HttpGet]
    public Dictionary<string, string> GetOwnState()
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Dictionary<string,string> state = new();
        state["lobbyid"] = _usersInLobbies.TryGetValue(userUid, out var lobbyid) ? lobbyid : "";
        if (state["lobbyid"] == "") return state;
        if (_lobbies.Exists(x => x.Id == state["lobbyid"]))
        {
            var lobby = _lobbies.Find(x => x.Id == state["lobbyid"]);
            var user = lobby.Participants.FirstOrDefault(x => x.Primitive.Id == userUid);
            if (user != null)
            {
                state["role"] = user.Role.ToString();
                state["ready"] = user.Ready.ToString();
            }
        }
        return state;
    }

    [HttpPost]
    public string CreateLobby(Lobby.GameTypes gameType, string gameName)
    {
        int creatorUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Lobby newLobby = new(creatorUid, gameType, gameName);
        while (_lobbies.Any(x=>x.Id == newLobby.Id))
        {
            newLobby = new (creatorUid, gameType, gameName);
        }
        _lobbies.Add(newLobby);
        return newLobby.Id;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult JoinLobby(string lobbyId)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var possibleLobby = _lobbies.Find(x => x.Id == lobbyId);
        if (possibleLobby == null)
        {
            return NotFound("Lobby not found");
        }
        else
        {
            _usersInLobbies[userUid] = possibleLobby.Id;
            possibleLobby.AddParticipant(userUid);
            return Ok();
        }
    }

    [HttpPost]
    public void LeaveLobby()
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (_usersInLobbies.ContainsKey(userUid))
        {
            var lobbyId = _usersInLobbies[userUid];
            var possibleLobby = _lobbies.Find(x => x.Id == lobbyId);
            possibleLobby?.RemoveParticipant(userUid);
            _usersInLobbies.Remove(userUid);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult SetReady(bool ready)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (_usersInLobbies.TryGetValue(userUid, out var lobbyId))
        {
            var possibleLobby = _lobbies.Find(x => x.Id == lobbyId);
            possibleLobby?.SetReady(userUid, ready);
            return Ok();
        }
        else
        {
            return NotFound("Lobby not found");
        }
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult SetRole(Lobby.TUser.Roles role)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (_usersInLobbies.TryGetValue(userUid, out var lobbyId))
        {
            var possibleLobby = _lobbies.Find(x => x.Id == lobbyId);
            possibleLobby?.SetRole(userUid, role);
            return Ok();
        }
        else
        {
            return NotFound("Lobby not found");
        }
    }
}

public class Lobby
{
    public enum GameTypes
    {
        Demo,
        Default
    }
    public class TUser(User user)
    {
        public User Primitive = user;

        public enum Roles
        {
            Player,
            Spectator
        }

        public Roles Role = Roles.Player;
        public bool Ready = false;
    }


    public readonly string Id;
    public string GameName;
    public GameTypes GameType;
    public User Creator;
    public HashSet<TUser> Participants;

    public Lobby(int creatorUid, GameTypes gameType, string gameName)
    {
        User? possibleCreator = Statics._userContext.Users.FirstOrDefault(x => x.Id == creatorUid);
        Creator = possibleCreator ?? throw new AuthenticationException("User not found");
        StringBuilder idBuilder = new StringBuilder();
        for (int i = 0; i < 16; i++)
        {
            idBuilder.Append((char)(Random.Shared.Next('a', 'z' + 1)));
        }
        Id = idBuilder.ToString();
        GameType = gameType;
        Participants =
        [
            new TUser(Creator)
        ];
        GameName = gameName;
    }

    public void AddParticipant(int userUid)
    {
        User? possibleUser = Statics._userContext.Users.FirstOrDefault(x => x.Id == userUid);
        if (possibleUser == null)
        {
            throw new AuthenticationException("User not found");
        }
        else
        {
            if (Participants.All(x => x.Primitive.Id != possibleUser.Id))
            {
                Participants.Add(new(possibleUser));
            }
        }
    }

    public void RemoveParticipant(int userUid)
    {
        TUser? possibleUser = Participants.FirstOrDefault(x => x.Primitive.Id == userUid);
        if (possibleUser != null)
        {
            Participants.Remove(possibleUser);
        }
    }

    public void SetReady(int userUid, bool ready)
    {
        TUser? possibleUser = Participants.FirstOrDefault(x => x.Primitive.Id == userUid);
        if (possibleUser == null)
        {
            throw new AuthenticationException("User not found");
        }
        else
        {
            possibleUser.Ready = ready;
        }
    }

    public void SetRole(int userUid, TUser.Roles role)
    {
        TUser? possibleUser = Participants.FirstOrDefault(x => x.Primitive.Id == userUid);
        if (possibleUser == null)
        {
            throw new AuthenticationException("User not found");
        }
        else
        {
            possibleUser.Role = role;
        }
    }
}