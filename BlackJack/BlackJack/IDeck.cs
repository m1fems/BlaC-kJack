using System.Collections.Generic;

namespace BlackJack
{
    interface IDeck
    {
        //Methods that will be used in the Deck class:

        void CreateCards();
        Stack<Card> Shuffle();
        Card Deal();
    }
}