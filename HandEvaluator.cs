using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    /*
     * This class is fairly complex and is used to quantify the value
     * of given hands for the purposes of evaluating the relative merits
     * of the hands of opposing players.
     */
    class HandEvaluator
    {
        public HandEvaluator()
        {
        }

        /*
         * This is a recursive method which analyses each one of the 
         * possible twenty-one hands from the seven-card hand (the 
         * player's private pair of cards and the five community cards) 
         * and returns the highest score.
         */
        public int evaluateHand(List<Card> cards)
        {
            if (cards.Count == 5)
            {
                return quantifyCardsValue(cards);
            }
            else
            {
                int highestEvaluation = 0;

                List<Card> reducedCardCollection;

                for (int i = 0; i < cards.Count; i++)
                {
                    reducedCardCollection = new List<Card>(cards);
                    reducedCardCollection.RemoveAt(i);

                    int cardCollectionValue = evaluateHand(reducedCardCollection);

                    if (cardCollectionValue > highestEvaluation)
                    {
                        highestEvaluation = cardCollectionValue;
                    }
                }

                return highestEvaluation;
            }
        }

        /*
         * This method is responsible for quantifying a given set of five 
         * cards.
         */
        public int quantifyCardsValue(List<Card> cards)
        {
            bool containsFlush = hasFlush(cards);
            int straightType = findStraightType(cards);

            int[] valueCounter = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            foreach (Card card in cards)
            {
                valueCounter[card.getValue() - 1]++;
            }

            int pairCount = 0;
            bool hasThreeOfAKind = false;
            bool hasFourOfAKind = false;

            foreach (int i in valueCounter)
            {
                if (i == 2)
                {
                    pairCount++;
                }
                else if (i == 3)
                {
                    hasThreeOfAKind = true;
                }
                else if (i == 4)
                {
                    hasFourOfAKind = true;
                }
            }

            if (containsFlush)
            {
                if (straightType == 14)
                {
                    // Royal flush
                    return 9;
                }
                else if (straightType >= 1)
                {
                    // Straight flush
                    return 8;
                }
                else
                {
                    // Normal flush
                    return 5;
                }
            }
            else if (straightType >= 1)
            {
                // Normal straight
                return 4;
            }
            else if (hasFourOfAKind)
            {
                // Four of a kind
                return 7;
            }
            else if (hasThreeOfAKind)
            {
                if (pairCount == 1)
                {
                    // Full house
                    return 6;
                }
                else
                {
                    // Regular three of a kind
                    return 3;
                }
            }
            else if (pairCount == 2)
            {
                // Two pair
                return 2;
            }
            else if (pairCount == 1)
            {
                // One pair;
                return 1;
            }

            // High card
            return 0;
        }

        /*
         * This method checks for the existence of a flush within a set
         * of five given cards.
         */
        public bool hasFlush(List<Card> cards)
        {
            return (cards[0].getSuit() == cards[1].getSuit()
                 && cards[1].getSuit() == cards[2].getSuit()
                 && cards[2].getSuit() == cards[3].getSuit()
                 && cards[3].getSuit() == cards[4].getSuit());
        }

        /*
         * This method checks for the existence of a straight within a set
         * of five given cards, and returns the value of the highest card,
         * except in the case of a 10 to Ace straight (the strongest kind)
         * wherein the method returns 14.
         * 
         * Given that the list of cards is sorted (this occurs at the start
         * of the method), the checking process is very simple: we know
         * precisely which card is going to be consecutive from another.
         */
        public int findStraightType(List<Card> cards)
        {
            // Sort the cards into value order using a lambda expression
            cards.Sort((x, y) => (x.getValue() - y.getValue()));

            if (cards[0].getValue() + 1 == cards[1].getValue() &&
                cards[1].getValue() + 1 == cards[2].getValue() &&
                cards[2].getValue() + 1 == cards[3].getValue() &&
                cards[3].getValue() + 1 == cards[4].getValue())
            {
                return cards[4].getValue();
            }
            else if (cards[1].getValue() == 10 &&
                     cards[2].getValue() == 11 &&
                     cards[3].getValue() == 12 &&
                     cards[4].getValue() == 13 &&
                     cards[0].getValue() == 1)
            {
                return 14;
            }

            return 0;
        }

        /*
         * This method uses a Monte Carlo simulation to approximate
         * (to an accuracy specified by the accuracy parameter) the
         * probability of getting different hands at the end of the
         * hand according to the currently visible cards.
         */
        public float[] findProbabilities(List<Card> cards, int accuracy)
        {
            int cardsToAdd = 7 - cards.Count;
            int attemptsToRun = accuracy;
            Deck duplicateDeck = new Deck(); ;
            List<Card> testingCards = new List<Card>();
            float[] probabilities = new float[10];
            int[] occurrences = new int[10];

            for (int i = 0; i < attemptsToRun; i++)
            {
                duplicateDeck.fillDeck(cards);

                testingCards.Clear();
                testingCards.AddRange(cards);

                for (int j = 0; j < cardsToAdd; j++)
                {
                    testingCards.Add(duplicateDeck.extractRandomCard());
                }

                occurrences[evaluateHand(testingCards)]++;
            }

            for (int i = 0; i < 10; i++)
            {
                probabilities[i] = ((float)occurrences[i]) / ((float)attemptsToRun);
            }

            return probabilities;
        }

        /*
         * This method returns a string with the name of a given hand
         * type.
         */
        public static string getHandType(int value)
        {
            switch (value)
            {
                case 0:
                    return "high card";
                case 1:
                    return "one pair";
                case 2:
                    return "two pair";
                case 3:
                    return "three of a kind";
                case 4:
                    return "straight";
                case 5:
                    return "flush";
                case 6:
                    return "full house";
                case 7:
                    return "four of a kind";
                case 8:
                    return "straight flush";
                case 9:
                    return "royal flush";
            }

            return "unknown hand";
        }
    }
}