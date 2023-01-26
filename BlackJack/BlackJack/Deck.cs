using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack
{
    public class Deck : IDeck
    {
        private Stack<Card> cards = new Stack<Card>();

        public Deck()
        {
        }

        public Deck(Stack<Card> cards)
        {
            Cards = cards;
        }

        public Stack<Card> Cards
        {
            get { return cards; }
            set { cards = value; }
        }
        public void CreateCards() // Combines the rank and suit into a result card, which is put in the Stack.
        {
            List<string> suits = new List<string> { "Hearts", "Diamonds", "Spades", "Clubs" };

            List<Tuple<string, byte>> ranks = new List<Tuple<string, byte>>
            {
                new Tuple<string, byte>("Two", 2),
                new Tuple<string, byte>("Three", 3),
                new Tuple<string, byte>("Four", 4),
                new Tuple<string, byte>("Five", 5),
                new Tuple<string, byte>("Six", 6),
                new Tuple<string, byte>("Seven", 7),
                new Tuple<string, byte>("Eight", 8),
                new Tuple<string, byte>("Nine", 9),
                new Tuple<string, byte>("Ten", 10),
                new Tuple<string, byte>("Jack", 10),
                new Tuple<string, byte>("Queen", 10),
                new Tuple<string, byte>("King", 10),
                new Tuple<string, byte>("Ace", 11),
            };

            foreach (string suit1 in suits)
            {
                foreach (var rank1 in ranks)
                {
                    Cards.Push(new Card(suit1, rank1.Item1));
                }
            }
        }
        public virtual Stack<Card> Shuffle() // Shuffles the deck and returns a new Stack.
        {
            var random = new Random();
            Stack<Card> newShuffledStack = new Stack<Card>();
            List<Card> copyList = Cards.ToList();
            int listcCount = copyList.Count;

            for (int i = 0; i < listcCount; i++)
            {
                var randomElementInList = random.Next(0, copyList.Count);

                newShuffledStack.Push(copyList.ElementAt(randomElementInList));

                copyList.RemoveAt(randomElementInList);
            }
            return newShuffledStack;
        }
        public virtual Card Deal() // Player/Dealer gets a new card (from the top of the deck).
        {
            Card single_card = Cards.Pop();
            return single_card;
        }
    }
}