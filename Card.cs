using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * This class is used to store data about each individual card on the table.
     */
    class Card
    {
        public enum Suits { Spades, Hearts, Clubs, Diamonds };
        int cardSuit;
        int cardValue;

        /*
         * Simple constructor.
         */
        public Card(int cardSuit, int cardValue)
        {
            this.cardSuit = cardSuit;
            this.cardValue = cardValue;
        }

        /*
         * This method is useful for debugging purposes. It takes the value
         * of the card and the suit of the card, and creates a string that
         * is easily recognisable as the card. Unicode symbols are used to
         * enhance this process.
         * 
         * In the future, this output will be done through the user 
         * application using images of some kind, so strings like the ones
         * that this method returns will no longer be useful.
         */
        public string getOutputString()
        {
            string returnString = "";

            if (cardValue >= 2 && cardValue <= 10)
            {
                returnString += cardValue;
            }
            else
            {
                switch (cardValue)
                {
                    case 1:
                        returnString += "A";
                        break;
                    case 11:
                        returnString += "J";
                        break;
                    case 12:
                        returnString += "Q";
                        break;
                    case 13:
                        returnString += "K";
                        break;
                }
            }

            switch (cardSuit)
            {
                case (int)Suits.Spades:
                    returnString += "$\spadesuit$";
                    break;
                case (int)Suits.Hearts:
                    returnString += "$\heartsuit$";
                    break;
                case (int)Suits.Clubs:
                    returnString += "$\clubsuit$";
                    break;
                case (int)Suits.Diamonds:
                    returnString += "$\diamondsuit$";
                    break;
            }

            return returnString;
        }

        /*
         * Simple getter method.
         */
        public int getSuit()
        {
            return cardSuit;
        }

        /*
         * Simple getter method.
         */
        public int getValue()
        {
            return cardValue;
        }
    }
}