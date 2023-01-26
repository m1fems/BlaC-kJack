namespace BlackJack
{
    public class Card
    {
        private string suit;
        private string rank;

        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public string Suit
        {
            get { return suit; }
            set { suit = value; }
        }
        public string Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        public override string ToString() // Prints the card.
        {
            return $"{Rank} of {Suit}"; // Two of Hearts
        }
    }
}
