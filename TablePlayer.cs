using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * The TablePlayer class accepts user input to decide on how to react
     * to the happenings of the hand.
     * 
     * In the future, this class will be a superclass, from which several
     * AI opponent classes and a (live) player class will inherit.
     */
    abstract class TablePlayer
    {
        protected Card[] cardPair;
        protected int totalBetThisBettingRound;
        protected int totalBetThisHand;
        protected int chips;
        protected int playerID;
        protected bool hasFolded;
        protected bool stillPlaying;

        // Simple getters and setters
        public bool HasFolded
        {
            set { this.hasFolded = value; }
            get { return hasFolded; }
        }

        public int TotalBetThisBettingRound
        {
            set { this.totalBetThisBettingRound = value; }
            get { return totalBetThisBettingRound; }
        }

        public int TotalBetThisHand
        {
            set { this.totalBetThisHand = value; }
            get { return totalBetThisHand; }
        }

        public int PlayerId
        {
            set { this.playerID = value; }
            get { return this.playerID; }
        }

        public int Chips
        {
            set { this.chips = value; }
            get { return this.chips; }
        }

        public bool StillPlaying
        {
            set { this.stillPlaying = value; }
            get { return this.stillPlaying; }
        }

        public Card Card1
        {
            set { this.cardPair[0] = value; }
            get { return this.cardPair[0]; }
        }

        public Card Card2
        {
            set { this.cardPair[1] = value; }
            get { return this.cardPair[1]; }
        }

        /*
         * The constructor of this class instantiates the necessary objects
         * (to avoid later null pointer exceptions) and gives the player
         * 250 chips. This number is subject to change.
         */
        public TablePlayer(int playerID)
        {
            cardPair = new Card[2];
            totalBetThisBettingRound = 0;
            totalBetThisHand = 0;
            chips = 250;
            this.playerID = playerID;
            hasFolded = false;
            stillPlaying = true;
        }

        /*
         * This method accepts two cards as parameters and saves them locally
         * in the object. This method is called as the cards are being dealt
         * out at the start of a given hand.
         */
        public void giveDealtCards(Card firstCard, Card secondCard)
        {
            cardPair[0] = firstCard;
            cardPair[1] = secondCard;
        }

        /*
         * This method clears the cards in the player's hand. This method is called
         * at the start of a new hand.
         */
        public void clearHand()
        {
            if (chips == 0)
            {
                stillPlaying = false;
            }
            else
            {
                totalBetThisHand = 0;
                totalBetThisBettingRound = 0;
                cardPair = new Card[2];
            }
        }

        /*
         * This method asks for input from the user in order to select the
         * desired action.
         */
        abstract public bool performGameplayDecision(Card[] communityCards, int currentPot, int currentMaxTableBet, int numberOfRemainingPlayers, int maxPossibleBet);

        /*
         * This methods is used to fold the player's hand (i.e. resign
         * them from the hand).
         */
        public void fold()
        {
            hasFolded = true;
        }

        /*
         * This method is used to raise the current maximum table bet by
         * a given amount. It is important that input to this function 
         * has been validated already in order to produce valid output.
         */
        public void raise(int amount, int currentMaxTableBet)
        {
            totalBetThisBettingRound = currentMaxTableBet + amount;
        }

        /*
         * This method is used to bet the given amount. It is important
         * that the input to this function has been validated already in
         * order to produce valid output.
         */
        public void bet(int amount)
        {
            totalBetThisBettingRound += amount;
        }

        /*
         * This method is used to call the current table maximum amount.
         * It is important taht the input to this function has already
         * been validated in order to produce valid output.
         */
        public void call(int currentMaxTableBet)
        {
            totalBetThisBettingRound = currentMaxTableBet;
        }

        /*
         * This method is used when the player wishes to check. Checking
         * means that the users wishes to do nothing. As such, this
         * method does nothing.
         * 
         * There are two reasons for the existence of this empty method:
         *  - Funcionality may need to be added in the future if, say, 
         *    a flag needs to be set, etc.
         *  - This method makes checking in various parts of the
         *    application more obvious and readable.
         */
        public void check()
        {

        }

        /*
         * This method calculates how many chips the player has 
         * remaining to bet.
         */
        public int getRemainingChips()
        {
            return (chips - totalBetThisBettingRound);
        }

        /*
         * This method subtracts the bet of the player from his/her chip 
         * count, before wiping the bet. This method is called after each
         * betting round.
         */
        public void wipeBet()
        {
            chips -= totalBetThisBettingRound;
            totalBetThisHand += totalBetThisBettingRound;
            totalBetThisBettingRound = 0;
        }

        /* 
         * This method instantiates a HandEvaluator object and passes it
         * the seven cards available to the player (the card pair, private
         * to the player, and the community cards on the table).
         * 
         * The HandEvaluator object is used to quantify the value of a 
         * hand, before returning the quantification for the purposes of 
         * comparison against the optimum hands of the opposing players.
         */
        public int computeBestHand(Card[] communityCards)
        {
            List<Card> allAvailableCards = new List<Card>();
            allAvailableCards.AddRange(cardPair);
            allAvailableCards.AddRange(communityCards);

            HandEvaluator handEvaluator = new HandEvaluator();

            return handEvaluator.evaluateHand(allAvailableCards);
        }

        /*
         * This method gives the player (this player object) a given number
         * of chips. This method is called after the player has won a given
         * hand.
         */
        public void giveChips(int numberToAdd)
        {
            chips += numberToAdd;
        }

        /*
         * This method returns a string to represent the cards that the
         * player has in his/her pair. This is useful for the purposes of 
         * debugging while the application is in its command line state.
         */
        public string getHandString()
        {
            return cardPair[0].getOutputString() + " " + cardPair[1].getOutputString();
        }
    }
}