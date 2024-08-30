using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    [HttpGet]
    public List<Dictionary<string,string>> GetLobbies()
    {
        List<Dictionary<string,string>> found_lobbies = new();
        foreach (var lobby in Statics._lobbies)
        {
            found_lobbies.Add(new ()
            {
                {"lobbyid", lobby.Id},
                {"lobbyname", lobby.GameName},
                {"gametype", lobby.GameType.ToString()},
                {"playercount", lobby.Participants.Count(x=> x.Role == Lobby.TUser.Roles.Player).ToString()},
            });
        }
        return found_lobbies;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((string lobbyid, List<Dictionary<string, string>> participants)))]
    public string GetOwnState()
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        (string lobbyid, List<Dictionary<string, string>> participants) state = new()
        {
            lobbyid = Statics._usersInLobbies.TryGetValue(userUid, out var lobbyid) ? lobbyid : "",
            participants = []
        };
        if (state.lobbyid == "") return Statics.Serialize(state);
        if (!Statics._lobbies.Exists(x => x.Id == state.lobbyid))
        {
            Statics._usersInLobbies.Remove(userUid);
            state.lobbyid = "";
            return Statics.Serialize(state);
        }
        if (Statics._lobbies.Exists(x => x.Id == state.lobbyid))
        {
            var lobby = Statics._lobbies.Find(x => x.Id == state.lobbyid);
            foreach (var participant in lobby.Participants)
            {
                Dictionary<string, string> usr = new()
                {
                    ["uid"] = participant.Primitive.Id.ToString(),
                    ["username"] = participant.Primitive.Name,
                    ["iscreator"] = participant.Primitive.Id == lobby.Creator.Id ? "True" : "False",
                    ["role"] = participant.Role.ToString(),
                    ["ready"] = participant.Ready.ToString()
                };
                state.participants.Add(usr);
            }
        }
        return Statics.Serialize(state);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
    public ActionResult CreateLobby([FromQuery, Required] Lobby.GameTypes gameType, [FromQuery, Optional] string? gameName)
    {
        int creatorUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (Statics._usersInLobbies.TryGetValue(creatorUid, out var lobby))
        {
            var exists = Statics._lobbies.Find(x => x.Id == lobby);
            if (exists != null) return Conflict("Can't create lobby when already in one");
        }
        Lobby newLobby = new(creatorUid, gameType, gameName??"");
        while (Statics._lobbies.Exists(x=>x.Id == newLobby.Id))
        {
            newLobby = new (creatorUid, gameType, gameName??"");
        }
        Statics._lobbies.Add(newLobby);
        Statics._usersInLobbies[creatorUid] = newLobby.Id;
        return Ok(newLobby.Id);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult JoinLobby([FromQuery, Required] string lobbyId)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var possibleLobby = Statics._lobbies.Find(x => x.Id == lobbyId);
        if (possibleLobby == null)
        {
            return NotFound("Lobby not found");
        }
        else
        {
            Statics._usersInLobbies[userUid] = possibleLobby.Id;
            possibleLobby.AddParticipant(userUid);
            return Ok();
        }
    }

    [HttpPost]
    public void LeaveLobby()
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (Statics._usersInLobbies.ContainsKey(userUid))
        {
            var lobbyId = Statics._usersInLobbies[userUid];
            var possibleLobby = Statics._lobbies.Find(x => x.Id == lobbyId);
            possibleLobby?.RemoveParticipant(userUid);
            Statics._usersInLobbies.Remove(userUid);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult SetReady([FromQuery, Required]bool ready)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (Statics._usersInLobbies.TryGetValue(userUid, out var lobbyId))
        {
            var possibleLobby = Statics._lobbies.Find(x => x.Id == lobbyId);
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
    public ActionResult SetRole([FromQuery, Required] Lobby.TUser.Roles role)
    {
        int userUid = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (Statics._usersInLobbies.TryGetValue(userUid, out var lobbyId))
        {
            var possibleLobby = Statics._lobbies.Find(x => x.Id == lobbyId);
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
        Monopoly
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