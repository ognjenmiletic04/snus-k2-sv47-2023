using ChaoticCupid.Server.Models;
using ChaoticCupid.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Hubs
{
    public class CupidHub : Hub
    {
        private readonly IPersonService _personService;

        public CupidHub(IPersonService personService)
        {
            _personService = personService;
        }

        public Task InitSinglePerson(string username, string city, int age, string phoneNumber)
        {
            SinglePerson person = new SinglePerson();

            person.Username = username;
            person.City = city;
            person.Age = age;
            person.PhoneNumber = phoneNumber;
            person.ConnectionId = Context.ConnectionId;
            person.IsWaitingConfirmation = false;

            bool success = _personService.RegisterPerson(person);

            if (!success)
            {
                return Clients.Caller.SendAsync("RegistrationFailed", "Username je već zauzet.");
            }

            return Clients.Caller.SendAsync("RegistrationSuccess", "Uspešno ste prijavljeni kod Kupidona.");
        }

        public Task ConfirmLetterReceived(string username)
        {
            bool success = _personService.ConfirmLetterReceived(username);

            if (!success)
            {
                return Clients.Caller.SendAsync("ServerMessage", "Greška: osoba nije pronađena.");
            }

            return Clients.Caller.SendAsync("ServerMessage", "Potvrdili ste prijem pisma.");
        }

        public Task BlockUser(string username, string blockedUsername)
        {
            bool success = _personService.BlockUser(username, blockedUsername);

            if (!success)
            {
                return Clients.Caller.SendAsync("ServerMessage", "Blokiranje nije uspelo.");
            }

            return Clients.Caller.SendAsync("ServerMessage", "Blokirali ste korisnika: " + blockedUsername);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _personService.RemoveByConnectionId(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}