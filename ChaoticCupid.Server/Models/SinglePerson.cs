namespace ChaoticCupid.Server.Models
{
    public class SinglePerson
    {
        private string _username;
        private string _city;
        private int _age;
        private string _phoneNumber;
        private string _connectionId;
        private bool _isWaitingConfirmation;
        private HashSet<string> _blockedUsers;

        public SinglePerson()
        {
            _username = string.Empty;
            _city = string.Empty;
            _age = 0;
            _phoneNumber = string.Empty;
            _connectionId = string.Empty;
            _isWaitingConfirmation = false;
            _blockedUsers = new HashSet<string>();
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string ConnectionId
        {
            get { return _connectionId; }
            set { _connectionId = value; }
        }

        public bool IsWaitingConfirmation
        {
            get { return _isWaitingConfirmation; }
            set { _isWaitingConfirmation = value; }
        }

        public HashSet<string> BlockedUsers
        {
            get { return _blockedUsers; }
            set { _blockedUsers = value; }
        }
    }
}