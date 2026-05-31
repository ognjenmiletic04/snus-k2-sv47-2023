using Microsoft.AspNetCore.SignalR.Client;

namespace ChaoticCupid.Client
{
    public class Program
    {
        private static string _username = string.Empty;
        private static bool _waitingForConfirmation = false;
        private static HubConnection? _connection;

        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Haotični Kupidon - Klijent ===");
            Console.WriteLine();

            _username = ReadRequiredText("Unesite username: ");
            string city = ReadRequiredText("Unesite grad: ");
            int age = ReadPositiveNumber("Unesite godine: ");
            string phoneNumber = ReadPhoneNumber("Unesite broj telefona: ");

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7044/cupidHub")
                .WithAutomaticReconnect()
                .Build();

            RegisterServerEvents();

            try
            {
                await _connection.StartAsync();

                await _connection.InvokeAsync(
                    "InitSinglePerson",
                    _username,
                    city,
                    age,
                    phoneNumber
                );

                Console.WriteLine();
                Console.WriteLine("Komande:");
                Console.WriteLine("ENTER              - potvrda prijema pisma");
                Console.WriteLine("/block username    - blokiranje korisnika");
                Console.WriteLine("/exit              - izlaz iz aplikacije");
                Console.WriteLine();

                await ReadCommands();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška pri povezivanju sa serverom:");
                Console.WriteLine(ex.Message);
            }
        }

        private static void RegisterServerEvents()
        {
            if (_connection == null)
            {
                return;
            }

            _connection.On<string>("RegistrationSuccess", message =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(message);
                Console.ResetColor();
            });

            _connection.On<string>("RegistrationFailed", message =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(message);
                Console.WriteLine("Aplikacija se zatvara.");
                Console.ResetColor();

                Environment.Exit(0);
            });

            _connection.On<string>("ServerMessage", message =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine();
                Console.WriteLine(message);
                Console.ResetColor();
            });

            _connection.On<LoveLetter>("ReceiveLoveLetter", letter =>
            {
                _waitingForConfirmation = true;

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("Stiglo je ljubavno pismo!");
                Console.WriteLine("========================================");
                Console.ResetColor();

                Console.WriteLine("Od: " + letter.FromUsername);
                Console.WriteLine("Grad: " + letter.FromCity);
                Console.WriteLine("Godine: " + letter.FromAge);

                if (!string.IsNullOrWhiteSpace(letter.FromPhoneNumber))
                {
                    Console.WriteLine("Telefon: " + letter.FromPhoneNumber);
                }

                Console.WriteLine("Poruka: " + letter.Message);
                Console.WriteLine("Score: " + letter.Score);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("Pritisnite ENTER da potvrdite prijem pisma.");
                Console.WriteLine("Ili unesite /block " + letter.FromUsername + " da blokirate korisnika.");
                Console.ResetColor();
            });
        }

        private static async Task ReadCommands()
        {
            if (_connection == null)
            {
                return;
            }

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == null)
                {
                    continue;
                }

                input = input.Trim();

                if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                {
                    await _connection.StopAsync();
                    break;
                }

                if (input.StartsWith("/block ", StringComparison.OrdinalIgnoreCase))
                {
                    string blockedUsername = input.Substring("/block ".Length).Trim();

                    if (string.IsNullOrWhiteSpace(blockedUsername))
                    {
                        Console.WriteLine("Morate uneti username za blokiranje.");
                        continue;
                    }

                    await _connection.InvokeAsync("BlockUser", _username, blockedUsername);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    if (_waitingForConfirmation)
                    {
                        await _connection.InvokeAsync("ConfirmLetterReceived", _username);
                        _waitingForConfirmation = false;
                    }
                    else
                    {
                        Console.WriteLine("Trenutno nemate pismo za potvrdu.");
                    }

                    continue;
                }

                Console.WriteLine("Nepoznata komanda.");
            }
        }

        private static string ReadRequiredText(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Polje ne sme biti prazno.");
                    continue;
                }

                return input.Trim();
            }
        }

        private static int ReadPositiveNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Godine ne smeju biti prazne.");
                    continue;
                }

                int number;

                bool success = int.TryParse(input, out number);

                if (!success)
                {
                    Console.WriteLine("Morate uneti broj, ne karaktere.");
                    continue;
                }

                if (number < 0)
                {
                    Console.WriteLine("Godine ne smeju biti negativne.");
                    continue;
                }

                return number;
            }
        }

        private static string ReadPhoneNumber(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Broj telefona ne sme biti prazan.");
                    continue;
                }

                input = input.Trim();

                bool valid = true;

                foreach (char c in input)
                {
                    if (!char.IsDigit(c) && c != '+')
                    {
                        valid = false;
                        break;
                    }
                }

                if (!valid)
                {
                    Console.WriteLine("Broj telefona sme da sadrži samo cifre i eventualno znak +.");
                    continue;
                }

                return input;
            }
        }
    }

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