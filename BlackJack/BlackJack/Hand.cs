using System;
using System.Collections.Generic;

namespace BlackJack
{
    public class Hand : Deck
    {
        private byte handValue;
        private byte aces; // count aces

        public Hand()
        {
        }

        public Hand(byte handValue, byte aces)
        {
            HandValue = handValue;
            Aces = aces;
        }

        public Hand(Stack<Card> cards) : base(cards)
        {
            Cards = new Stack<Card>();
            HandValue = 0;
            Aces = 0;
        }

        public byte HandValue
        {
            get { return handValue; }
            set { handValue = value; }
        }

        public byte Aces
        {
            get { return aces; }
            set { aces = value; }
        }

        public virtual void AddCard(Card card) // Converts the string rank to the corresponding number and adds it to the hand value.
        {
            Cards.Push(card);
            switch (card.Rank)
            {
                case "Two":
                    HandValue += 2;
                    break;

                case "Three":
                    HandValue += 3;
                    break;

                case "Four":
                    HandValue += 4;
                    break;

                case "Five":
                    HandValue += 5;
                    break;

                case "Six":
                    HandValue += 6;
                    break;

                case "Seven":
                    HandValue += 7;
                    break;

                case "Eight":
                    HandValue += 8;
                    break;

                case "Nine":
                    HandValue += 9;
                    break;

                case "Ten":
                    HandValue += 10;
                    break;

                case "Jack":
                    HandValue += 10;
                    break;

                case "Queen":
                    HandValue += 10;
                    break;

                case "King":
                    HandValue += 10;
                    break;

                case "Ace":
                    HandValue += 11;
                    break;

                default:
                    throw new ArgumentException("There is no such rank");
            }

            if (card.Rank == "Ace")
            {
                Aces += 1;
            }
        }

        public virtual void AdjustAces() // Checks whether the points are > 21 and we have and ace
        {
            while (HandValue > 21 && Aces >= 1)
            {
                HandValue -= 10;
                Aces -= 1;
            }
        }
    }
}