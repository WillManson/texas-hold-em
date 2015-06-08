using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * This class controls the deck of cards, the players at the table
     * and the bets. It also establishes which player(s) has/have won
     * at the end of a given hand.
     * 
     * In a sense, this class can be thought of as the croupier of the
     * poker game.
     */
    class GameEngine
    {
        TablePlayer[] tablePlayers;
        Deck deck;
        Card[] communityCards;
        int currentTotalPot = 0;
        int handsToPlay;
        int currentHandNumber;
        int currentDealerPlayer = 0;
        int currentPlayerTurn = 0;
        int difficulty = 0;

        // Simple getter and setter methods
        public int CurrentTotalPot
        {
            get { return currentTotalPot; }
            set { this.currentTotalPot = value; }
        }

        public int HandsToPlay
        {
            get { return handsToPlay; }
            set { this.handsToPlay = value; }
        }

        public int CurrentHandNumber
        {
            get { return currentHandNumber; }
            set { this.currentHandNumber = value; }
        }

        public int CurrentDealerPlayer
        {
            get { return currentDealerPlayer; }
            set { this.currentDealerPlayer = value; }
        }

        public int CurrentPlayerTurn
        {
            get { return currentPlayerTurn; }
            set { this.currentPlayerTurn = value; }
        }

        public int Difficulty
        {
            get { return difficulty; }
            set { this.difficulty = value; }
        }

        public TablePlayer[] TablePlayers
        {
            get { return tablePlayers; }
            set { this.tablePlayers = value; }
        }

        public Card[] CommunityCards
        {
            get { return communityCards; }
            set { this.communityCards = value; }
        }

        public Deck Deck
        {
            get { return deck; }
            set { this.deck = value; }
        }

        /*
         * The constructor instantiates the table players and the deck,
         * objects which will be used on each and every hand.
         * 
         * At this stage, all TablePlayers are user-controlled. In the
         * future, TablePlayer will be a superclass, from which both
         * AI opponents and the player will inherit.
         */
        public GameEngine(int difficulty, int handsToPlay)
        {
            this.difficulty = difficulty;

            tablePlayers = new TablePlayer[4];

            currentDealerPlayer = 0;
            this.handsToPlay = handsToPlay;
            currentHandNumber = 1;

            deck = new Deck();

            communityCards = new Card[5];
        }

        /*
         * This method loads the tablePlayers array with instantiated
         * UserPlayer and AIPlayer objects according to difficulty,
         * etc.
         */
        public void initiatePlayers()
        {
            tablePlayers[0] = new UserPlayer(0);
            tablePlayers[1] = new AIPlayer(1, difficulty);
            tablePlayers[2] = new AIPlayer(2, difficulty);
            tablePlayers[3] = new AIPlayer(3, difficulty);
        }

        /*
         * This method gets the number of commuity cards that have 
         * already been dealt to the table. This is used when loading
         * a game part-way through in order to determined at which
         * part of the hand the application should pick up.
         */
        public int getNumberOfCommunityCards()
        {
            int count = 0;

            foreach (Card card in communityCards)
            {
                if (card != null)
                    count++;
            }

            return count;
        }

        /*
         * This method is used to determine whether or not gameplay 
         * should terminate on a given iteration.
         */
        public bool isGameRunning()
        {
            return (currentHandNumber <= handsToPlay && getTotalPlayingPlayers() > 1 && tablePlayers[0].Chips > 0);
        }

        /*
         * This method sets up the hand in preparation of gameplay. To
         * do this, it "flushes out" all of the data from the previous
         * hand, and starts anew. This is important so as to not have
         * any data accidentally reused from one game in the next.
         */
        public void beginHand()
        {
            deck.fillDeck();
            clearCommunityCards();
            currentTotalPot = 0;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                tablePlayer.clearHand();
                tablePlayer.HasFolded = false;
            }

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                dealTo(tablePlayer);
            }

            currentDealerPlayer = getPlayingPlayerAfterGivenPlayer(currentDealerPlayer);

            // Force small blind
            int smallBlindPlayer = getSmallBlindPlayer(currentDealerPlayer);
            tablePlayers[smallBlindPlayer].bet(1);

            // Force big blind
            int bigBlindPlayer = getBigBlindPlayer(currentDealerPlayer);
            tablePlayers[bigBlindPlayer].bet(2);
        }

        /*
         * This method is in charge of clearing the community
         * cards. This is called by beginHand()
         */
        public void clearCommunityCards()
        {
            communityCards = new Card[5];
        }

        /*
         * This method chooses two random cards from the deck and gives
         * them to the player in question. This is called by beginHand()
         * in order to give each player his/her initial two cards.
         */
        public void dealTo(TablePlayer tablePlayer)
        {
            tablePlayer.giveDealtCards(deck.extractRandomCard(), deck.extractRandomCard());
        }

        /*
         * This method takes three random cards from the deck and adds them
         * to the community cards. This is to simulate the "flop" in poker.
         */
        public void revealFlop()
        {
            communityCards[0] = deck.extractRandomCard();
            communityCards[1] = deck.extractRandomCard();
            communityCards[2] = deck.extractRandomCard();
        }

        /*
         * This method takes a random card from the deck and adds it to
         * the community cards. This is to simulate the "turn" in poker.
         */
        public void revealTurn()
        {
            communityCards[3] = deck.extractRandomCard();
        }


        /*
         * This method takes a random card from the deck and adds it to
         * the community cards. This is to simulate the "river" in poker.
         */
        public void revealRiver()
        {
            communityCards[4] = deck.extractRandomCard();
        }

        /*
         * This method gets the first playing (i.e. non-bankrupt) after
         * the entered player ID. This is used to determine which player
         * is the dealer, which player is the big blind, etc.
         */
        public int getPlayingPlayerAfterGivenPlayer(int givenPlayer)
        {
            int playerToTest = givenPlayer + 1;
            playerToTest %= 4;

            while (!tablePlayers[playerToTest].StillPlaying)
            {
                playerToTest++;
                playerToTest %= 4;
            }

            return playerToTest;
        }

        /*
         * This method gets the ID of the player who is the small blind
         * of the current hand (i.e. the active player after the dealer
         * of the current hand).
         */
        public int getSmallBlindPlayer(int dealerPlayer)
        {
            return getPlayingPlayerAfterGivenPlayer(currentDealerPlayer);
        }

        /*
         * This method gets teh ID fo teh player who is the big blind
         * of the current hand (i.e. the active player after the small
         * blind of the current hand).
         */
        public int getBigBlindPlayer(int dealerPlayer)
        {
            return getPlayingPlayerAfterGivenPlayer(getSmallBlindPlayer(dealerPlayer));
        }

        /*
         * This method gets the card of the community cards with the
         * given position.
         */
        public Card getCard(int cardNumber)
        {
            return communityCards[cardNumber];
        }

        /*
         * This method handles all of the betting between the players 
         * between community card reveals.
         */
        public bool performBets()
        {
            int totalPlayersAsked = 0;
            currentPlayerTurn = getPlayingPlayerAfterGivenPlayer(getBigBlindPlayer(currentDealerPlayer)); ;

            /*
             * This while loop asks each player in turn to perform an action.
             * The totalPlayersAsked variable helps to ensure that the table
             * is sweeped at least once to give each player the opportunity
             * to bet.
             * 
             * If one player bets during the round, this while loop does not
             * terminate until all players have bet precisely the same amount.
             */
            while ((!allActivePlayersAgree() || totalPlayersAsked < 4) && getTotalActivePlayers() > 1)
            {
                /*
                 * The first section of this code focusses on providing all
                 * of the relevant information to the user (the community 
                 * cards, etc.) to allow him/her to decide on an action.
                 */
                Console.Clear();
                Console.WriteLine("Current player: " + currentPlayerTurn);
                Console.WriteLine("Hand " + currentHandNumber + "/" + handsToPlay);

                Console.WriteLine("");

                for (int i = 0; i < tablePlayers.Length; i++)
                {
                    if (!tablePlayers[i].StillPlaying)
                    {
                        Console.WriteLine("Player " + i + " is out");
                    }
                    else
                    {
                        string additionalInformationString = "";

                        if (tablePlayers[i].HasFolded)
                            additionalInformationString += " (FOLDED)";

                        if (i == currentDealerPlayer)
                            additionalInformationString += " (DEALER)";

                        if (i == getSmallBlindPlayer(currentDealerPlayer))
                            additionalInformationString += " (SMALL BLIND)";

                        if (i == getBigBlindPlayer(currentDealerPlayer))
                            additionalInformationString += " (BIG BLIND)";

                        if (i == 0)
                            additionalInformationString += " (YOU)";

                        Console.WriteLine("Player " + i + ", chips: " + tablePlayers[i].Chips + "; bet: " + tablePlayers[i].TotalBetThisBettingRound + additionalInformationString);
                    }
                }

                Console.WriteLine("Pot: " + currentTotalPot);

                Console.WriteLine("");

                string communityCardString = "";
                foreach (Card card in communityCards)
                {
                    if (card != null)
                    {
                        communityCardString += card.getOutputString() + " ";
                    }
                }

                if (communityCardString.Length > 0)
                {
                    Console.WriteLine("Current community cards: " + communityCardString);
                }

                Console.WriteLine("Player cards: " + tablePlayers[0].Card1.getOutputString() + " " + tablePlayers[0].Card2.getOutputString());

                TablePlayer currentlyActivePlayer = tablePlayers[currentPlayerTurn];

                if (!currentlyActivePlayer.HasFolded)
                {
                    bool shouldQuitAndSave = currentlyActivePlayer.performGameplayDecision(communityCards, currentTotalPot, getCurrentMaxBet(), getTotalActivePlayers(), getMaxPossibleBet());

                    if (shouldQuitAndSave)
                        return true;
                }

                totalPlayersAsked++;

                currentPlayerTurn++;
                currentPlayerTurn %= 4;
            }

            return false;
        }

        /*
         * This method collects up all of the bets of the given betting
         * round from the various players. In doing so, it wipes the bets
         * of the players and adds it to a running total, known as the pot.
         */
        public void collectBets()
        {
            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                currentTotalPot += tablePlayer.TotalBetThisBettingRound;
                tablePlayer.wipeBet();
            }
        }

        /*
         * This method checks to see whether all of the players have bet 
         * precisely the same amount. This method is used to check whether 
         * the betting round should terminate.
         */
        public bool allActivePlayersAgree()
        {
            int theBetAmount = -1;
            int playersAsked = 0;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded)
                {
                    if (playersAsked == 0)
                    {
                        theBetAmount = tablePlayer.TotalBetThisBettingRound;
                    }
                    else if (tablePlayer.TotalBetThisBettingRound != theBetAmount)
                    {
                        return false;
                    }

                    playersAsked++;
                }
            }

            return true;
        }

        /*
         * This method searches through the list of table players to find
         * the current largest bet on the table.
         */
        public int getCurrentMaxBet()
        {
            int currentMaxBet = 0;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded && tablePlayer.TotalBetThisBettingRound > currentMaxBet)
                {
                    currentMaxBet = tablePlayer.TotalBetThisBettingRound;
                }
            }

            return currentMaxBet;
        }

        /* 
         * This method counts the number of players still active (i.e.
         * the total number of players who have not yet folded and are 
         * not yet bankrupt).
         */
        public int getTotalActivePlayers()
        {
            int count = 0;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded && tablePlayer.StillPlaying)
                {
                    count++;
                }
            }

            return count;
        }

        /*
         * This method counts the number of playing players (i.e. 
         * non-bankrupt players).
         */
        public int getTotalPlayingPlayers()
        {
            int count = 0;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (tablePlayer.StillPlaying)
                {
                    count++;
                }
            }

            return count;
        }

        /*
         * This method obtains the maximum possible bet/raise that any
         * player can make: for the purposes of this game, no player
         * can wager more than the number of remaining chips of the
         * poorest player on the table.
         */
        public int getMaxPossibleBet()
        {
            int smallestValue = -1;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded)
                    if (tablePlayer.StillPlaying)
                        if (smallestValue < 0 || tablePlayer.Chips < smallestValue)
                            smallestValue = tablePlayer.Chips;
            }

            return smallestValue;
        }

        /*
         * This method is called after the final round of betting within a 
         * given hand. It calculates the total amount bet, before checking 
         * whose hand(s) is/are the strongest, and then distributes the pot 
         * accordingly.
         */
        public void finishHand()
        {
            int bestHandValue = 0;
            int totalPlayerCount = getTotalActivePlayers();
            int playerCount = 0;

            Console.Clear();
            Console.WriteLine("Pot: " + currentTotalPot);

            string communityCardString = "";
            foreach (Card card in communityCards)
            {
                if (card != null)
                {
                    communityCardString += card.getOutputString() + " ";
                }
            }

            Console.WriteLine("Community cards: " + communityCardString);

            Console.WriteLine();

            /*
             * This List keeps track of the players with the highest hand values.
             */
            List<TablePlayer> winningPlayers = new List<TablePlayer>();

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded)
                {
                    int handValue = tablePlayer.computeBestHand(communityCards);

                    if (handValue > bestHandValue)
                    {
                        /*
                         * Since there is a new strongest hand, the List
                         * of winning players is cleared.
                         */
                        winningPlayers.Clear();
                        bestHandValue = handValue;
                        winningPlayers.Add(tablePlayer);
                    }
                    else if (handValue == bestHandValue)
                    {
                        winningPlayers.Add(tablePlayer);
                    }

                    Console.WriteLine("Player " + playerCount + " scores " + HandEvaluator.getHandType(handValue) + " with " + tablePlayer.getHandString());
                }

                playerCount++;
            }

            Console.WriteLine();

            /*
             * The share per winner is calculated in order to give an equal split
             * of the pot to each winning player.
             */
            int sharePerWinner = currentTotalPot / winningPlayers.Count;

            foreach (TablePlayer winner in winningPlayers)
            {
                winner.giveChips(sharePerWinner);
                Console.WriteLine("Player " + winner.PlayerId + " wins " + sharePerWinner + " chips");
            }

            currentHandNumber++;

            System.Threading.Thread.Sleep(4000);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            Console.ReadLine();
        }

        /*
         * This method is called if all but one player has folded and the hand
         * must terminate. The method gives all of the money in the pot to the
         * remaining player (as per the rules of poker).
         */
        public void endHandEarly()
        {
            int totalPlayerCount = getTotalActivePlayers();
            int playerCount = 0;
            int remainingPlayer = -1;

            foreach (TablePlayer tablePlayer in tablePlayers)
            {
                if (!tablePlayer.HasFolded)
                {
                    remainingPlayer = playerCount;
                }

                playerCount++;
            }

            TablePlayer winner = tablePlayers[remainingPlayer];
            winner.giveChips(currentTotalPot);
            System.Console.WriteLine("Player " + remainingPlayer + " wins " + currentTotalPot + " since all other players have folded");

            currentHandNumber++;

            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }
    }
}