using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * This class is used to store all of the cards remaining in the deck.
     * This is useful in order to be able to keep track of which cards have
     * already been dealt, thereby avoiding repetition of cards.
     */
    class Deck
    {
        List<Card> cards;
        Random randomNumberGenerator;

        /*
         * Simple constructor which instantiates the objects required for the
         * running of this class.
         */
        public Deck()
        {
            cards = new List<Card>();
            randomNumberGenerator = new Random();
        }

        /*
         * This method fills the deck with precisely one of each possible 
         * card.
         */
        public void fillDeck()
        {
            cards.Clear();

            foreach (int suit in Enum.GetValues(typeof(Card.Suits)))
            {
                for (int value = 1; value <= 13; value++)
                {
                    cards.Add(new Card(suit, value));
                }
            }
        }

        /*
         * This method fills the deck with precisely one of each possible 
         * card, except those cards appearing in the list given as an
         * argument.
         */
        public void fillDeck(List<Card> doNotAdd)
        {
            cards.Clear();

            foreach (int suit in Enum.GetValues(typeof(Card.Suits)))
            {
                for (int value = 1; value <= 13; value++)
                {
                    foreach (Card card in doNotAdd)
                    {
                        if (card.getSuit() == suit && card.getValue() == value)
                            continue;
                    }

                    cards.Add(new Card(suit, value));
                }
            }
        }

        /*
         * This card picks a random card out of the deck (removing it from
         * the deck itself), before returning it.
         * 
         * This is useful when dealing out cards to the players and dealing
         * out the community cards.
         */
        public Card extractRandomCard()
        {
            int listLength = cards.Count;

            if (listLength > 1)
            {
                int randomValue = randomNumberGenerator.Next(listLength);

                Card selectedCard = cards[randomValue];
                cards.RemoveAt(randomValue);

                return selectedCard;
            }
            else
            {
                return null;
            }
        }
    }
}