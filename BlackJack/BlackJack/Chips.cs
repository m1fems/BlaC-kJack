namespace BlackJack
{
    public class Chips
    {
        private int total;
        private int bet;

        public Chips()
        {
            Total = 100;
            Bet = 0;
        }

        public int Total
        {
            get { return total; }
            set { total = value; }
        }

        public int Bet
        {
            get { return bet; }
            set { bet = value; }
        }

        public virtual void WinBet() // Adds the bet to the current chip amount.
        {
            Total += Bet;
        }

        public virtual void LoseBet() // Removes the bet from the current chip amount.
        {
            Total -= Bet;
        }
    }
}