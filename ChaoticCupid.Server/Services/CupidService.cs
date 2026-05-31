using System.Security.Cryptography;
using ChaoticCupid.Server.Models;
using Microsoft.AspNetCore.SignalR;
using ChaoticCupid.Server.Hubs;

namespace ChaoticCupid.Server.Services
{
    public class CupidService : ICupidService
    {
        private readonly IPersonService _personService;
        private readonly IHubContext<CupidHub> _hubContext;

        private readonly string[] _messages;

        public CupidService(IPersonService personService, IHubContext<CupidHub> hubContext)
        {
            _personService = personService;
            _hubContext = hubContext;

            _messages = new string[]
            {
                "Radujem se našem susretu!",
                "Želim da se upoznamo.",
                "Nisam zainteresovan/a za upoznavanje."
            };
        }

        public async Task SendLettersToAllAsync()
        {
            List<SinglePerson> persons = _personService.GetAllPersons();

            foreach (SinglePerson receiver in persons)
            {
                if (receiver.IsWaitingConfirmation)
                {
                    continue;
                }

                SinglePerson? bestSender = FindBestSender(receiver, persons);

                if (bestSender == null)
                {
                    continue;
                }

                int score = CalculateScore(receiver, bestSender);
                string message = GetRandomMessage();

                LoveLetter letter = new LoveLetter();

                letter.FromUsername = bestSender.Username;
                letter.FromCity = bestSender.City;
                letter.FromAge = bestSender.Age;
                letter.Message = message;
                letter.Score = score;

                if (message == "Nisam zainteresovan/a za upoznavanje.")
                {
                    letter.FromPhoneNumber = null;
                }
                else
                {
                    letter.FromPhoneNumber = bestSender.PhoneNumber;
                }

                SinglePerson? realReceiver = _personService.GetByUsername(receiver.Username);

                if (realReceiver == null)
                {
                    continue;
                }

                realReceiver.IsWaitingConfirmation = true;

                await _hubContext.Clients
                    .Client(realReceiver.ConnectionId)
                    .SendAsync("ReceiveLoveLetter", letter);
            }
        }

        private SinglePerson? FindBestSender(SinglePerson receiver, List<SinglePerson> persons)
        {
            SinglePerson? bestSender = null;
            int bestScore = -1;

            foreach (SinglePerson candidate in persons)
            {
                if (candidate.Username == receiver.Username)
                {
                    continue;
                }

                if (receiver.BlockedUsers.Contains(candidate.Username))
                {
                    continue;
                }

                int score = CalculateScore(receiver, candidate);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestSender = candidate;
                }
            }

            return bestSender;
        }

        private int CalculateScore(SinglePerson receiver, SinglePerson candidate)
        {
            int score = 0;

            if (receiver.City.Equals(candidate.City, StringComparison.OrdinalIgnoreCase))
            {
                score += 30;
            }

            int ageDifference = Math.Abs(receiver.Age - candidate.Age);

            if (ageDifference <= 2)
            {
                score += 20;
            }

            score += GetSecureRandomNumber(0, 100);

            return score;
        }

        private string GetRandomMessage()
        {
            int index = GetSecureRandomNumber(0, _messages.Length - 1);
            return _messages[index];
        }

        private int GetSecureRandomNumber(int minValue, int maxValue)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);

                uint randomNumber = BitConverter.ToUInt32(randomBytes, 0);

                int range = maxValue - minValue + 1;

                return (int)(randomNumber % range) + minValue;
            }
        }
    }
}