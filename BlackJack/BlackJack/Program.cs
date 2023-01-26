using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    class Program
    {
        private static JsonFileOperationService<List<User>> userService;
        private static List<User> users;
        private static User user;
        static void Main(string[] args)
        {
            // initialize json user service and get deserialized data
            userService = new JsonFileOperationService<List<User>>(
                   "../../../users.json",
                   () => new List<User>());

            users = userService.GetObject();

            Console.Write("Name: ");
            string username = Console.ReadLine().Trim();

            user = users.Find(u => u.Username == username);

            if (user != null)
            {
                Console.WriteLine($"Welcome back, {user.Username}. Please enter your password.");
                Console.Write("Password: ");
                while (true)
                {
                    string userPassword = Console.ReadLine().Trim();
                    if (userPassword == user.Password)
                    {
                        Console.WriteLine($"Successful login.");

                        if (user.Chips.Total == 0)
                        {
                            user.Chips.Total = 10;
                        }

                        break;
                    }
                    Console.WriteLine("Incorrect password. Please try again.");
                    Console.Write("Password: ");
                }
            }
            else
            {
                Console.WriteLine($"Welcome new player {username}");
                Console.Write("Please create your password: ");
                string newPassword = Console.ReadLine().Trim();
                user = new User(username, newPassword, new Chips());
                users.Add(user);
            }

            Console.WriteLine();

            Console.WriteLine($"Welcome to blackjack {user.Username}");

            Console.WriteLine($"You have {user.Chips.Total} chips");

            Deck deck = new Deck();

            for (int i = 0; i < 8; i++) // Creates 8 decks and combines them.
            {
                deck.CreateCards();
            }

            Stack<Card> shuffledStack = deck.Shuffle(); // Shuffles the combined deck
            Deck shuffledDeck = new Deck(shuffledStack);

            while (user.Chips.Total > 0)
            {
                Hand playerHand = new Hand();
                Hand dealerHand = new Hand();

                string insurance = "";
                string splitInput = "";
                string inp = "";

                for (int i = 0; i < 4; i++) // Deals both the dealer and the player 2 cards.
                {
                    if (i < 2) // First 2 are for the player, third and fourth - for the dealer.
                    {
                        Card playerCard = shuffledDeck.Deal();
                        playerHand.AddCard(playerCard);
                    }

                    else
                    {
                        Card dealerCard = shuffledDeck.Deal();
                        dealerHand.AddCard(dealerCard);
                    }
                }

                dealerHand.AdjustAces();
                Chips bet = new Chips();
                bet.Total = TakeBet(user.Chips);

                ShowSomeCards(playerHand, dealerHand, user.Username); // Dealer's first card is still hidden.

                while (inp.ToLower() != "s")
                {
                    if (user.Chips.Total - bet.Total >= 2 && bet.Total >= 2) // Checks whether we have enough chips left to insure.
                    {
                        insurance = Insurance(playerHand, dealerHand, user.Chips, user.Username);

                        if (insurance == "y")
                            break;
                    }

                    if ((user.Chips.Total - bet.Total >= bet.Total) && inp.ToLower() != "h") // Checks whether we have enough chips to split.
                    {
                        Card card1 = playerHand.Cards.ElementAt(0);
                        Card card2 = playerHand.Cards.ElementAt(1);
                        if (card1.Rank == card2.Rank) // Split logic.
                        {
                            while (splitInput.ToLower() != "n")
                            {
                                Console.WriteLine("Would you like to split? (y/n): ");
                                splitInput = Console.ReadLine();

                                if (splitInput.ToLower() == "y")
                                {
                                    Split(playerHand, dealerHand, shuffledDeck, user.Chips, bet, user.Username);
                                    break;
                                }
                                else if (splitInput.ToLower() == "n") // The game continues normally.
                                {
                                    playerHand.AdjustAces();
                                    while (inp.ToLower() != "s")
                                    {
                                        inp = HitOrStand(shuffledDeck, playerHand, user.Username);
                                        ShowSomeCards(playerHand, dealerHand, user.Username);
                                        if (playerHand.HandValue > 21)
                                        {
                                            PlayerBusts(playerHand, dealerHand, user.Chips, user.Username);
                                            break;
                                        }
                                    }
                                }
                                else
                                    continue;
                            }
                            if (splitInput.ToLower() == "y" || splitInput.ToLower() == "n") // If the input does not match the expected answer, the program lets the player type again.
                                break;
                        }
                    }

                    inp = HitOrStand(shuffledDeck, playerHand, user.Username);
                    ShowSomeCards(playerHand, dealerHand, user.Username);

                    if (playerHand.HandValue > 21) // Player loses.
                    {
                        PlayerBusts(playerHand, dealerHand, user.Chips, user.Username);
                        break;
                    }
                }

                if (playerHand.HandValue <= 21 && insurance != "y" && splitInput.ToLower() != "y") // Dealer is playing (if we have not insured or split)
                {
                    while (dealerHand.HandValue <= 17)
                    {
                        Hit(shuffledDeck, dealerHand);
                    }

                    ShowAllCards(playerHand, dealerHand, user.Username);

                    // Checks who won - player or dealer.
                    if (dealerHand.HandValue > 21)
                        DealerBusts(playerHand, dealerHand, user.Chips);

                    else if (dealerHand.HandValue > playerHand.HandValue)
                        DealerWins(playerHand, dealerHand, user.Chips);

                    else if (dealerHand.HandValue < playerHand.HandValue)
                        PlayerWins(playerHand, dealerHand, user.Chips, user.Username);

                    else
                        Push(playerHand, dealerHand, user.Username);
                }
                Console.WriteLine();
                Console.WriteLine($"{user.Username}'s chips: " + user.Chips.Total);
                Console.WriteLine();

                if (user.Chips.Total <= 0) // If you don't have any more chips, the game ends.
                {
                    Console.WriteLine("You lost all your chips");
                    UpdateUsers();
                    break;
                }

                while (true) // If you still have chips, you can play again.
                {
                    string newGame = " ";
                    Console.Write("Would you like to play again? (y/n): ");
                    newGame = Console.ReadLine();

                    if (newGame.ToLower() == "y")
                    {
                        break;
                    }

                    else if (newGame.ToLower() == "n")
                    {
                        Console.WriteLine("Thanks for playing!");
                        UpdateUsers();
                        return;
                    }

                    else
                    {
                        Console.WriteLine("Wrong input");
                        continue;
                    }
                }
            }
        }
        private static int TakeBet(Chips chips) // Takes the amount of chips that the player wants to bet.
        {
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("How many chips would you like to bet? ");
                    chips.Bet = int.Parse(Console.ReadLine());

                    if (chips.Bet > chips.Total || chips.Bet <= 0)
                    {
                        Console.WriteLine("You can't bet that many chips");
                        continue;
                    }

                    else
                        break;
                }

                catch (FormatException) // If the input is not an integer, throw a format exception.
                {
                    Console.WriteLine("Bet must be an integer!");
                    continue;
                }
            }
            return chips.Bet;
        }

        public static string Insurance(Hand playerHand, Hand dealerHand, Chips chips, string name)
        {
            Chips insuranceChips = new Chips();
            string inp = "";
            if (dealerHand.Cards.ElementAt(1).Rank == "Ace") // If the dealer's shown card is an ace, you can insure.
            {
                while (true)
                {
                    Console.WriteLine("You can insure, do you want to? (y/n): ");
                    inp = Console.ReadLine();
                    if (inp.ToLower() == "y")
                    {
                        while (insuranceChips.Total > chips.Bet / 2)
                        {
                            try
                            {
                                Console.WriteLine();
                                Console.Write("How many chips would you like to insure: ");
                                insuranceChips.Total = int.Parse(Console.ReadLine());
                                if (insuranceChips.Total > chips.Bet / 2) // you can insure maximum half of your chips. 
                                {
                                    Console.WriteLine($"You can't insure that many chips (can be up to {chips.Bet / 2} chips)");
                                    continue;
                                }
                                else
                                    break;
                            }

                            catch (FormatException) // If the input is not an integer, throw a format excpetion.
                            {
                                Console.WriteLine("Insurance must be an integer!");
                                continue;
                            }
                        }

                        ShowAllCards(playerHand, dealerHand, name);

                        if (dealerHand.Cards.ElementAt(0).Rank == "Ten" || dealerHand.Cards.ElementAt(0).Rank == "Jack" || dealerHand.Cards.ElementAt(0).Rank == "Queen" || dealerHand.Cards.ElementAt(0).Rank == "King")
                        {
                            if (playerHand.HandValue < dealerHand.HandValue) // The player loses the chips that he has bet, but wins the insurance.
                            {
                                chips.LoseBet();
                                insuranceChips.WinBet();
                                chips.Total += 2 * insuranceChips.Total;
                                break;
                            }

                            else // The player wins the insurance. 
                            {
                                insuranceChips.WinBet();
                                chips.Total += 2 * insuranceChips.Total;
                                break;
                            }
                        }

                        else // Dealer's second card's value is not 10
                        {
                            if (playerHand.HandValue > dealerHand.HandValue) // The player wins the bet but loses the insurance.
                            {
                                chips.WinBet();

                                chips.Total -= insuranceChips.Total;
                                break;
                            }

                            else if (playerHand.HandValue < dealerHand.HandValue) // The player loses both the bet and the insurance.
                            {
                                chips.LoseBet();

                                chips.Total -= insuranceChips.Total;
                                break;
                            }

                            else // The player loses the insurance, but not the bet.
                            {
                                insuranceChips.LoseBet();
                                chips.Total -= insuranceChips.Total;
                                break;
                            }
                        }
                    }

                    else if (inp.ToLower() == "n")
                        break;

                    else // If the input does not match the expected answer, the program lets the player type again.
                        Console.WriteLine("Wrong input");
                }
            }
            return inp.ToLower();
        }

        public static void Split(Hand playerHand1, Hand dealerHand, Deck deck, Chips chips, Chips bet, string name) // Split logic.
        {

            // Creates 2 separate hands
            Hand playerHand2 = new Hand();
            byte points = (byte)(playerHand1.HandValue / 2);
            playerHand2.Cards.Push(playerHand1.Cards.ElementAt(1));
            playerHand1.HandValue = points;
            playerHand2.HandValue = points;
            playerHand1.Cards.Pop();
            string inp1 = " ";
            string inp2 = " ";

            // Game logic for hand 1
            Console.WriteLine("Hand 1: ");
            ShowSomeCards(playerHand1, dealerHand, name);

            while (inp1.ToLower() != "s")
            {
                inp1 = HitOrStand(deck, playerHand1, name);
                ShowSomeCards(playerHand1, dealerHand, name);

                if (playerHand1.HandValue > 21)
                {
                    PlayerBusts(playerHand1, dealerHand, bet, name);
                    chips.Total -= bet.Total;
                    break;
                }
            }

            // Game logic for hand 2
            Console.WriteLine("Hand 2: ");
            ShowSomeCards(playerHand2, dealerHand, name);

            while (inp2.ToLower() != "s")
            {
                inp2 = HitOrStand(deck, playerHand2, name);
                ShowSomeCards(playerHand2, dealerHand, name);
                if (playerHand2.HandValue > 21)
                {
                    PlayerBusts(playerHand2, dealerHand, bet, name);
                    chips.Total -= bet.Total;
                    break;
                }

            }

            if (playerHand2.HandValue <= 21 || playerHand1.HandValue <= 21)
            {
                while (dealerHand.HandValue <= 17)
                    Hit(deck, dealerHand);

                ShowAllCards(playerHand2, dealerHand, name);

                Console.WriteLine("Hand 1: ");
                if (playerHand1.HandValue > 21)
                    PlayerBusts(playerHand1, dealerHand, bet, name);

                else if (dealerHand.HandValue > 21)
                    DealerBusts(playerHand1, dealerHand, chips);

                else if (dealerHand.HandValue > playerHand1.HandValue)
                    DealerWins(playerHand1, dealerHand, chips);

                else if (dealerHand.HandValue < playerHand1.HandValue)
                    PlayerWins(playerHand1, dealerHand, chips, name);

                else
                    Push(playerHand1, dealerHand, name);

                Console.WriteLine();
                Console.WriteLine("Hand 2 : ");

                if (playerHand2.HandValue > 21)
                    PlayerBusts(playerHand2, dealerHand, bet, name);
                else if (dealerHand.HandValue > 21)
                {
                    DealerBusts(playerHand2, dealerHand, bet);
                    chips.Total += bet.Total;
                }

                else if (dealerHand.HandValue > playerHand2.HandValue)
                {
                    DealerWins(playerHand2, dealerHand, bet);
                    chips.Total -= bet.Total;
                }

                else if (dealerHand.HandValue < playerHand2.HandValue)
                {
                    PlayerWins(playerHand2, dealerHand, bet, name);
                    chips.Total += bet.Total;
                }

                else
                    Push(playerHand2, dealerHand, name);
            }

            if (chips.Total <= 0) // If you don't have any more chips, the game ends.
            {
                Console.WriteLine("You lost all your chips");
                UpdateUsers();
            }
        }

        public static void Hit(Deck deck, Hand hand) // Hit logic - player or dealer draws a card and adjusts aces if needed.
        {
            hand.AddCard(deck.Deal());
            hand.AdjustAces();
        }

        public static string HitOrStand(Deck deck, Hand hand, string name) // Player draws a card or stays. When stood, dealer starts playing.
        {
            string inp = " ";

            while (true)
            {
                Console.Write("Would you like to Hit or Stand? Enter 'h' or 's': ");
                inp = Console.ReadLine();

                if (inp.ToLower() == "h")
                    Hit(deck, hand);

                else if (inp.ToLower() == "s")
                {
                    Console.WriteLine($"{name} stands. Dealer is playing.");
                    break;
                }

                else
                {
                    Console.WriteLine("Sorry, please try again.");
                    continue;
                }

                break;
            }
            return inp;
        }

        public static void ShowSomeCards(Hand playerHand, Hand dealerHand, string name) // Displays only one of the dealer's cards and all of the player's cards.
        {
            Console.WriteLine("\nDealer's Hand:");
            Console.WriteLine("<card hidden>");
            Console.WriteLine(dealerHand.Cards.ElementAt(1));
            Console.WriteLine();
            Console.WriteLine($"{name}'s hand:");

            foreach (var card in playerHand.Cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.WriteLine();
            Console.WriteLine($"{name}'s points = {playerHand.HandValue}");
            Console.WriteLine();
        }

        public static void ShowAllCards(Hand playerHand, Hand dealerHand, string name) // Displays all of player and dealer's cards.
        {
            Console.WriteLine("Dealer's hand:");

            foreach (var card in dealerHand.Cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.WriteLine();
            Console.WriteLine($"Dealer's points = {dealerHand.HandValue}");
            Console.WriteLine();
            Console.WriteLine($"{name}'s hand:");

            foreach (var card in playerHand.Cards)
            {
                Console.WriteLine(card.ToString());
            }

            Console.WriteLine();
            Console.WriteLine($"{name}'s points = {playerHand.HandValue}");
            Console.WriteLine();
        }

        public static void PlayerBusts(Hand playerHand, Hand dealerHand, Chips chips, string name) // Player has more than 21 pts.
        {
            Console.WriteLine($"{name} busts!");
            chips.LoseBet();
        }
        public static void PlayerWins(Hand playerHand, Hand dealerHand, Chips chips, string name) // player's pts > dealer's pts
        {
            Console.WriteLine($"{name} wins!");
            chips.WinBet();
        }
        public static void DealerBusts(Hand playerHand, Hand dealerHand, Chips chips) // dealer's pts > 21
        {
            Console.WriteLine("Dealer busts!");
            chips.WinBet();
        }
        public static void DealerWins(Hand playerHand, Hand dealerHand, Chips chips) // dealer's pts > player's pts
        {
            Console.WriteLine("Dealer wins!");
            chips.LoseBet();
        }
        public static void Push(Hand playerHand, Hand dealerHand, string name) // Draw.
        {
            Console.WriteLine($"Dealer and {name} tie! It's a push.");
        }

        private static void UpdateUsers()
        {
            userService.UpdateObject(users);
        }
    }
}