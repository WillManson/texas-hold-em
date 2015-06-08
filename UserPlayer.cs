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
      * to make decisions according to user input.
      */
    class UserPlayer : TablePlayer
    {
        /*
         * This constructor simply calls the constructor of the base
         * class.
         */
        public UserPlayer(int playerID)
            : base(playerID)
        {

        }

        /*
         * This method asks the user to make a decision for a gameplay
         * decision, validating input throughout.
         */
        public override bool performGameplayDecision(Card[] communityCards, int currentPot, int currentMaxTableBet, int numberOfRemainingPlayers, int maxPossibleBet)
        {
            Console.WriteLine("Please choose an action");
            List<char> validInputs = new List<char>();

            if (currentMaxTableBet > 0)
            {
                if (TotalBetThisBettingRound < currentMaxTableBet)
                {
                    Console.WriteLine("C. Call");
                    validInputs.Add('C');

                    Console.WriteLine("F. Fold");
                    validInputs.Add('F');
                }

                if (chips > currentMaxTableBet)
                {
                    Console.WriteLine("R. Raise");
                    validInputs.Add('R');
                }
            }
            else
            {
                Console.WriteLine("B. Bet");
                validInputs.Add('B');
            }

            if (TotalBetThisBettingRound >= currentMaxTableBet)
            {
                Console.WriteLine("Z. Check");
                validInputs.Add('Z');
            }

            Console.WriteLine("P. Probabilities");
            validInputs.Add('P');

            Console.WriteLine("Q. Quit and save");
            validInputs.Add('Q');

            char input = ' ';

            while (!validInputs.Contains(input))
            {
                input = (char)Console.Read();
            }

            if (input == 'B')
            {
                int betAmount = -1;

                int betLimit = getRemainingChips();
                if (maxPossibleBet < betLimit)
                    betLimit = maxPossibleBet;

                Console.WriteLine("Please specify the amount that you would like to bet (0 to " + betLimit + ")");

                while (betAmount < 0 || betAmount > betLimit)
                {
                    bool inputSuccess = Int32.TryParse(Console.ReadLine(), out betAmount);
                    if (!inputSuccess)
                        betAmount = -1;
                }

                bet(betAmount);
            }
            else if (input == 'C')
            {
                call(currentMaxTableBet);
            }
            else if (input == 'R')
            {
                int raiseAmount = -1;

                int raiseLimit = getRemainingChips() - currentMaxTableBet;
                if (maxPossibleBet - currentMaxTableBet < raiseLimit)
                    raiseLimit = maxPossibleBet - currentMaxTableBet;

                Console.WriteLine("Please specify the amount that you would like to raise (0 to " + raiseLimit + ")");

                while (raiseAmount < 0 || raiseAmount > raiseLimit)
                {
                    bool inputSuccess = Int32.TryParse(Console.ReadLine(), out raiseAmount);
                    if (!inputSuccess)
                        raiseAmount = -1;
                }

                raise(raiseAmount, currentMaxTableBet);
            }
            else if (input == 'F')
            {
                fold();
            }
            else if (input == 'P')
            {
                Console.WriteLine("Loading probabilities... please wait");

                HandEvaluator hE = new HandEvaluator();
                List<Card> currentCards = new List<Card>();
                currentCards.AddRange(cardPair);

                foreach (Card card in communityCards)
                {
                    if (card != null)
                    {
                        currentCards.Add(card);
                    }
                    else
                        break;
                }

                float[] probabilities = hE.findProbabilities(currentCards, 100000);

                for (int i = 0; i <= 9; i++)
                {
                    Console.WriteLine(HandEvaluator.getHandType(i) + ": " + probabilities[i]);
                }

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                Console.ReadLine();

                return performGameplayDecision(communityCards, currentPot, currentMaxTableBet, numberOfRemainingPlayers, maxPossibleBet);
            }
            else if (input == 'Q')
            {
                return true;
            }
            else if (input == 'Z') 
            {
                check();
            }

            return false;
        }
    }
}