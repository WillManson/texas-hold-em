using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * This class is a subclass/derived class from TablePlayer. It
     * implements the functionality necessary for an object to be able
     * to make its own decisions during gameplay (using probability
     * etc.)
     */
    class AIPlayer : TablePlayer
    {
        // 0: Easy
        // 1: Medium
        // 2: Hard
        public int difficulty;
        Random randomNumberGenerator;

        /*
         * This constructor calls the constructor of the base class, and
         * also instantiates the Random object, which is necessary for
         * various probability-related computations in the
         * performGameplayDecision method.
         */
        public AIPlayer(int playerID, int difficulty)
            : base(playerID)
        {
            this.difficulty = difficulty;
            randomNumberGenerator = new Random();
        }

        /*
         * This method overrides the abstract declaration of the method
         * from the base class. An AI player reacts according to
         * probabilities.
         */
        public override bool performGameplayDecision(Card[] communityCards, int currentPot, int currentMaxTableBet, int numberOfRemainingPlayers, int maxPossibleBet)
        {
            bool canCall = (currentMaxTableBet > 0) && (TotalBetThisBettingRound < currentMaxTableBet) && (Chips >= currentMaxTableBet);
            bool canFold = (currentMaxTableBet > 0) && (TotalBetThisBettingRound < currentMaxTableBet);
            bool canRaise = (currentMaxTableBet > 0) && (chips > currentMaxTableBet);
            bool canBet = (currentMaxTableBet == 0);
            bool canCheck = (TotalBetThisBettingRound >= currentMaxTableBet);

            bool shouldCall = false;
            bool shouldFold = false;
            bool shouldRaise = false;
            bool shouldBet = false;
            bool shouldCheck = false;

            int potentialRaiseAmount = 0;
            int potentialBetAmount = 0;

            HandEvaluator hE = new HandEvaluator();

            List<Card> myCurrentCards = new List<Card>();
            List<Card> theirCurrentCards = new List<Card>();

            myCurrentCards.AddRange(cardPair);

            foreach (Card card in communityCards)
            {
                if (card != null)
                {
                    myCurrentCards.Add(card);
                    theirCurrentCards.Add(card);
                }
                else
                    break;
            }

            float[] myProbabilities = hE.findProbabilities(myCurrentCards, 300 * (int)Math.Pow(difficulty + 1, 5.0));
            float[] theirProbabilities = hE.findProbabilities(theirCurrentCards, 300 * (int)Math.Pow(difficulty + 1, 5.0));

            double probabilityOfWinningAgainstGivenPlayer = probabilityOfDRVGreaterThanOtherDRV(myProbabilities, theirProbabilities);
            double probabilityOfWinningHand = Math.Pow((double)probabilityOfWinningAgainstGivenPlayer, (double)(numberOfRemainingPlayers - 1));
            double probabilityOfLosingHand = 1 - probabilityOfWinningHand;

            int randomMistakeValue = randomNumberGenerator.Next(100);
            bool shouldMakeAMistake = (randomMistakeValue >= 65 + (difficulty * 15));
            double randomRiskValue = 8 + randomNumberGenerator.NextDouble() * 8;

            int amountNeededToCall = currentMaxTableBet - TotalBetThisBettingRound;
            double expectedWinOnCallOrCheck = probabilityOfWinningHand * ((double)(amountNeededToCall + currentPot)) - probabilityOfLosingHand * (TotalBetThisHand + amountNeededToCall);

            int amountAvailableToBet = getRemainingChips();
            if (maxPossibleBet < amountAvailableToBet)
                amountAvailableToBet = maxPossibleBet;

            potentialBetAmount = (int)((1 - Math.Pow(randomNumberGenerator.NextDouble(), (1.0 / 3.0))) * ((double)amountAvailableToBet));
            double expectedWinOnBet = probabilityOfWinningHand * ((double)(currentPot + potentialBetAmount)) - probabilityOfLosingHand * (TotalBetThisHand + potentialBetAmount);

            int amountAvailableToRaise = findMaximum(getRemainingChips() - currentMaxTableBet, 0);
            if (maxPossibleBet - currentMaxTableBet < amountAvailableToRaise)
                amountAvailableToRaise = maxPossibleBet - currentMaxTableBet;

            potentialRaiseAmount = (int)((1 - Math.Pow(randomNumberGenerator.NextDouble(), (1.0 / 3.0))) * ((double)amountAvailableToRaise));
            double expectedWinOnRaise = probabilityOfWinningHand * ((double)(currentPot + amountNeededToCall + potentialBetAmount)) - probabilityOfLosingHand * (TotalBetThisHand + amountNeededToCall + potentialRaiseAmount);

            if (canRaise && expectedWinOnRaise > expectedWinOnCallOrCheck)
            {
                shouldRaise = true;
            }
            else if (canBet && expectedWinOnBet > expectedWinOnCallOrCheck)
            {
                shouldBet = true;
            }
            else if (expectedWinOnCallOrCheck <= randomRiskValue && expectedWinOnCallOrCheck >= (0 - randomRiskValue))
            {
                shouldCall = true;
                shouldCheck = true;
            }
            else
            {
                shouldCheck = true;
                shouldFold = true;
            }

            if (shouldMakeAMistake)
            {
                if (canCheck && (shouldRaise || shouldBet)) check();
                else if (canFold && shouldCall) fold();
                else if (canCall && shouldFold) call(currentMaxTableBet);
                else if (canFold) fold();
                else if (canCheck) check();
            }
            else
            {
                if (canCheck && shouldCheck) check();
                else if (canFold && shouldFold) fold();
                else if (canCall && shouldCall) call(currentMaxTableBet);
                else if (canRaise && shouldRaise) raise(potentialRaiseAmount, currentMaxTableBet);
                else if (canBet && shouldBet) bet(potentialBetAmount);
                else if (canCheck) check();
                else if (canFold) fold();
            }

            return false;
        }

        /*
         * This method calculates the probability that one discrete
         * random variable has a greater value than another discrete
         * random variable, making the assumption that the variables
         * are independent.
         */
        public double probabilityOfDRVGreaterThanOtherDRV(float[] X, float[] Y)
        {
            double runningTotal = 0.0;

            for (int x = 1; x < X.Length; x++)
            {
                for (int y = 0; y < x; y++)
                {
                    runningTotal += X[x] * Y[y];
                }
            }

            return runningTotal;
        }

        /*
         * This method finds the maximum value of two given values.
         */
        public int findMaximum(int firstValue, int secondValue)
        {
            if (firstValue > secondValue) return firstValue;
            return secondValue;
        }
    }
}