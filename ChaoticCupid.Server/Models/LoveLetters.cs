namespace ChaoticCupid.Server.Models
{
    public class LoveLetter
    {
        private string _fromUsername;
        private string _fromCity;
        private int _fromAge;
        private string? _fromPhoneNumber;
        private string _message;
        private int _score;

        public LoveLetter()
        {
            _fromUsername = string.Empty;
            _fromCity = string.Empty;
            _fromAge = 0;
            _fromPhoneNumber = null;
            _message = string.Empty;
            _score = 0;
        }

        public string FromUsername
        {
            get { return _fromUsername; }
            set { _fromUsername = value; }
        }

        public string FromCity
        {
            get { return _fromCity; }
            set { _fromCity = value; }
        }

        public int FromAge
        {
            get { return _fromAge; }
            set { _fromAge = value; }
        }

        public string? FromPhoneNumber
        {
            get { return _fromPhoneNumber; }
            set { _fromPhoneNumber = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }
    }
}