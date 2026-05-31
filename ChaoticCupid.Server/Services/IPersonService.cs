using ChaoticCupid.Server.Models;

namespace ChaoticCupid.Server.Services
{
    public interface IPersonService
    {
        bool RegisterPerson(SinglePerson person);

        bool ConfirmLetterReceived(string username);

        bool BlockUser(string username, string blockedUsername);

        List<SinglePerson> GetAllPersons();

        SinglePerson? GetByUsername(string username);

        void RemoveByConnectionId(string connectionId);
    }
}