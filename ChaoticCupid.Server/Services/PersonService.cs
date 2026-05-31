using ChaoticCupid.Server.Models;

namespace ChaoticCupid.Server.Services
{
    public class PersonService : IPersonService
    {
        private readonly object _lock;
        private readonly Dictionary<string, SinglePerson> _personsByUsername;

        public PersonService()
        {
            _lock = new object();
            _personsByUsername = new Dictionary<string, SinglePerson>();
        }

        public bool RegisterPerson(SinglePerson person)
        {
            lock (_lock)
            {
                if (_personsByUsername.ContainsKey(person.Username))
                {
                    return false;
                }

                _personsByUsername[person.Username] = person;
                return true;
            }
        }

        public bool ConfirmLetterReceived(string username)
        {
            lock (_lock)
            {
                if (!_personsByUsername.ContainsKey(username))
                {
                    return false;
                }

                _personsByUsername[username].IsWaitingConfirmation = false;
                return true;
            }
        }

        public bool BlockUser(string username, string blockedUsername)
        {
            lock (_lock)
            {
                if (!_personsByUsername.ContainsKey(username))
                {
                    return false;
                }

                if (username.Equals(blockedUsername, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                _personsByUsername[username].BlockedUsers.Add(blockedUsername);
                return true;
            }
        }

        public List<SinglePerson> GetAllPersons()
        {
            lock (_lock)
            {
                List<SinglePerson> persons = new List<SinglePerson>();

                foreach (SinglePerson person in _personsByUsername.Values)
                {
                    SinglePerson copy = new SinglePerson();

                    copy.Username = person.Username;
                    copy.City = person.City;
                    copy.Age = person.Age;
                    copy.PhoneNumber = person.PhoneNumber;
                    copy.ConnectionId = person.ConnectionId;
                    copy.IsWaitingConfirmation = person.IsWaitingConfirmation;
                    copy.BlockedUsers = new HashSet<string>(person.BlockedUsers);

                    persons.Add(copy);
                }

                return persons;
            }
        }

        public SinglePerson? GetByUsername(string username)
        {
            lock (_lock)
            {
                if (!_personsByUsername.ContainsKey(username))
                {
                    return null;
                }

                return _personsByUsername[username];
            }
        }

        public void RemoveByConnectionId(string connectionId)
        {
            lock (_lock)
            {
                SinglePerson? personToRemove = null;

                foreach (SinglePerson person in _personsByUsername.Values)
                {
                    if (person.ConnectionId == connectionId)
                    {
                        personToRemove = person;
                        break;
                    }
                }

                if (personToRemove != null)
                {
                    _personsByUsername.Remove(personToRemove.Username);
                }
            }
        }
    }
}