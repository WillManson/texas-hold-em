using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexasHoldemPoker
{
    class Program
    {
        static GameEngine gameEngine;
        static DatabaseEngine databaseEngine;

        /*
         * This method handles the initialisation of the application
         * and the subsequent processes.
         */
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Texas Hold 'em Poker";

            databaseEngine = new DatabaseEngine();
            handleLogin();

            char[] inputArray = { 'N', 'L', 'S', 'Q' };
            char input = ' ';

            while (input != 'Q')
            {
                input = ' ';

                Console.Clear();
                displayMenu();

                while (!inputArray.Contains(input))
                {
                    input = (char)Console.Read();
                }

                switch (input)
                {
                    case 'N':
                        createNewGame();
                        break;
                    case 'L':
                        int gameToLoad = databaseEngine.chooseGameToLoad();
                        if (gameToLoad != -1)
                            loadSavedGame(gameToLoad);
                        else
                        {
                            Console.WriteLine("No game to load");
                            Console.WriteLine("Press enter to continue");
                            Console.ReadLine();
                        }
                        break;
                    case 'S':
                        databaseEngine.loadStatistics();
                        break;
                    case 'Q':
                        Console.WriteLine("Quitting application...");
                        databaseEngine.close();
                        break;
                    default:
                        Console.WriteLine("Invalid input");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                        break;
                }
            }
        }

        /*
         * This method asks for the user to enter the desired difficulty
         * of the opponents and the number of hands he or she wishes to
         * play. The method validates the input and, after valid input
         * has been made, initiates a game with the entered parameters.
         */
        static void createNewGame()
        {
            int opponentDifficulty = -1;
            Console.WriteLine("Please select a difficulty for the opponents (0 - easy, 1 - medium, 2 - hard)");
            Console.Write("Difficulty: ");

            while (opponentDifficulty < 0 || opponentDifficulty > 2)
            {
                bool inputSuccess = Int32.TryParse(Console.ReadLine(), out opponentDifficulty);
                if (!inputSuccess)
                    opponentDifficulty = -1;
            }

            int numberOfHands = -1;
            int[] availableHandNumbers = { 5, 10, 15, 20 };
            Console.WriteLine("Please select the number of hands you wish to play (5, 10, 15 or 20)");
            Console.Write("Number of hands: ");

            while (!availableHandNumbers.Contains(numberOfHands))
            {
                bool inputSuccess = Int32.TryParse(Console.ReadLine(), out numberOfHands);
                if (!inputSuccess)
                    numberOfHands = -1;
            }

            runGameplay(opponentDifficulty, numberOfHands);
        }

        /*
         * This method handles the login process of the application.
         * If a password does not exist, it asks for the user to create
         * one, during the process of which it validates the password to
         * ensure security. If a password does exist, it asks the user 
         * to enter a valid input. Access to the application is only 
         * provided when the correct password is entered.
         */
        static void handleLogin()
        {
            if (LoginSystem.hasCreatedPassword())
            {
                string passwordAttempt = "";

                while (true)
                {
                    Console.Write("Please enter your password: ");
                    passwordAttempt = Console.ReadLine();

                    bool isPasswordValid = LoginSystem.checkEnteredPassword(passwordAttempt);

                    if (isPasswordValid)
                        break;
                    else
                    {
                        Console.WriteLine("Password not correct");
                        Console.WriteLine("Press enter to try again");
                        Console.ReadLine();
                        Console.Clear();
                    }

                }

                Console.WriteLine("Correct password entered");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }
            else
            {
                // Password creation process

                string newPassword = "";

                while (!LoginSystem.isPasswordValid(newPassword))
                {
                    Console.Write("Please define your password: ");
                    newPassword = Console.ReadLine();

                    if (LoginSystem.isPasswordValid(newPassword))
                    {
                        LoginSystem.saveNewPassword(newPassword);
                    }
                    else
                    {
                        Console.WriteLine("Password invalid");
                        Console.WriteLine("A password must adhere to the following rules: ");
                        Console.WriteLine(" - It must have at least 8 characters");
                        Console.WriteLine(" - It must have at most 20 characters");
                        Console.WriteLine(" - It must contain at least one number");
                        Console.WriteLine(" - It must only contain uppercase letters, lowercase letters and number");
                    }
                }

                Console.WriteLine("Password valid");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }
        }

        /*
         * This method displays the main menu, which presents the
         * available options to the user.
         */
        static void displayMenu()
        {
            Console.WriteLine("Please select a menu option");
            Console.WriteLine("N: Start a new game");
            Console.WriteLine("L: Load a previous game");
            Console.WriteLine("S: View game statistics");
            Console.WriteLine("Q: Quit application");
        }

        /*
         * This method is called when the user requests to start a new
         * game from the main menu (option 'L'). It ends up doing
         * similar things as the runGameplay() method. The different
         * aspects of this class are as follows:
         *  - It continues the hand in progress from the exact point at
         *    which the game was left.
         *  - It performs "UPDATE" SQL queries when saving the game as
         *    opposed to "INSERT" SQL queries to avoid duplicate save
         *    games.
         */
        static void loadSavedGame(int gameID)
        {
            databaseEngine = new DatabaseEngine();
            gameEngine = databaseEngine.loadGameEngine(gameID);

            if (gameEngine != null)
            {
                bool shouldQuitAndSave = false;

                // Special treatment for first hand

                // Begin hand from pre-flop if appropriate
                if (gameEngine.getNumberOfCommunityCards() == 0)
                {
                    shouldQuitAndSave = gameEngine.performBets();

                    if (!shouldQuitAndSave)
                    {
                        gameEngine.collectBets();
                        gameEngine.revealFlop();
                    }
                }

                // Begin/continue hand from flop if appropriate
                if (gameEngine.getNumberOfCommunityCards() == 3 && !shouldQuitAndSave)
                {
                    if (gameEngine.getTotalActivePlayers() > 1)
                    {
                        shouldQuitAndSave = gameEngine.performBets();

                        if (!shouldQuitAndSave)
                        {
                            gameEngine.collectBets();
                            gameEngine.revealTurn();
                        }
                    }
                }

                // Begin/continue hand from turn if appropriate
                if (gameEngine.getNumberOfCommunityCards() == 4 && !shouldQuitAndSave)
                {
                    if (gameEngine.getTotalActivePlayers() > 1)
                    {
                        shouldQuitAndSave = gameEngine.performBets();

                        if (!shouldQuitAndSave)
                        {
                            gameEngine.collectBets();
                            gameEngine.revealRiver();
                        }
                    }
                }

                // Begin/continue hand from river if appropriate
                if (gameEngine.getNumberOfCommunityCards() == 5 && !shouldQuitAndSave)
                {
                    if (gameEngine.getTotalActivePlayers() > 1)
                    {
                        shouldQuitAndSave = gameEngine.performBets();

                        if (!shouldQuitAndSave)
                        {
                            gameEngine.collectBets();

                            if (gameEngine.getTotalActivePlayers() > 1)
                            {
                                gameEngine.finishHand();
                            }
                            else
                            {
                                gameEngine.endHandEarly();
                            }
                        }
                    }
                }

                // Continue hands in the usual manner
                if (!shouldQuitAndSave)
                {
                    while (gameEngine.isGameRunning())
                    {
                        gameEngine.beginHand();

                        if (!gameEngine.isGameRunning())
                            break;

                        shouldQuitAndSave = gameEngine.performBets();

                        if (shouldQuitAndSave)
                        {
                            break;
                        }

                        gameEngine.collectBets();

                        if (gameEngine.getTotalActivePlayers() > 1)
                        {
                            gameEngine.revealFlop();

                            shouldQuitAndSave = gameEngine.performBets();

                            if (shouldQuitAndSave)
                            {
                                break;
                            }

                            gameEngine.collectBets();
                        }

                        if (gameEngine.getTotalActivePlayers() > 1)
                        {
                            gameEngine.revealTurn();

                            shouldQuitAndSave = gameEngine.performBets();

                            if (shouldQuitAndSave)
                            {
                                break;
                            }

                            gameEngine.collectBets();
                        }

                        if (gameEngine.getTotalActivePlayers() > 1)
                        {
                            gameEngine.revealRiver();

                            shouldQuitAndSave = gameEngine.performBets();

                            if (shouldQuitAndSave)
                            {
                                break;
                            }

                            gameEngine.collectBets();
                        }

                        if (gameEngine.getTotalActivePlayers() > 1)
                        {
                            gameEngine.finishHand();
                        }
                        else
                        {
                            gameEngine.endHandEarly();
                        }
                    }
                }

                if (shouldQuitAndSave)
                {
                    // Update the database
                    databaseEngine.updateSavedGame(gameID,
                        gameEngine.CurrentPlayerTurn, gameEngine.CurrentHandNumber,
                        gameEngine.HandsToPlay, gameEngine.CurrentDealerPlayer, gameEngine.Difficulty,
                        databaseEngine.findCardNumber(gameEngine.getCard(0)),
                        databaseEngine.findCardNumber(gameEngine.getCard(1)),
                        databaseEngine.findCardNumber(gameEngine.getCard(2)),
                        databaseEngine.findCardNumber(gameEngine.getCard(3)),
                        databaseEngine.findCardNumber(gameEngine.getCard(4)),
                        gameEngine.CurrentTotalPot);

                    TablePlayer[] tablePlayers = gameEngine.TablePlayers;

                    for (int i = 0; i < 4; i++)
                    {
                        databaseEngine.updateTablePlayer(gameID, i, tablePlayers[i].Chips,
                            tablePlayers[i].TotalBetThisBettingRound, tablePlayers[i].TotalBetThisHand,
                            databaseEngine.findCardNumber(tablePlayers[i].Card1),
                            databaseEngine.findCardNumber(tablePlayers[i].Card2),
                            tablePlayers[i].HasFolded);
                    }
                }
                else
                {
                    databaseEngine.deleteSavedGameAndPlayers(gameID);

                    databaseEngine.addFinishedGame
                        (gameEngine.HandsToPlay, gameEngine.Difficulty, gameEngine.TablePlayers[0].Chips);
                }
            }
            else
            {
                Console.WriteLine("There has been an error loading this game save");
                Console.ReadLine();
            }
        }

        /*
         * This method instantiates the GameEngine object and controls
         * it. This is called when the user requests to start a new
         * game from the main menu (option 'N').
         */
        static void runGameplay(int opponentDifficulty, int numberOfHands)
        {
            gameEngine = new GameEngine(opponentDifficulty, numberOfHands);
            gameEngine.initiatePlayers();
            databaseEngine = new DatabaseEngine();
            bool shouldQuitAndSave = false;

            while (gameEngine.isGameRunning())
            {
                gameEngine.beginHand();

                if (!gameEngine.isGameRunning())
                    break;

                shouldQuitAndSave = gameEngine.performBets();

                if (shouldQuitAndSave)
                {
                    break;
                }

                gameEngine.collectBets();

                if (gameEngine.getTotalActivePlayers() > 1)
                {
                    gameEngine.revealFlop();

                    shouldQuitAndSave = gameEngine.performBets();

                    if (shouldQuitAndSave)
                    {
                        break;
                    }

                    gameEngine.collectBets();
                }

                if (gameEngine.getTotalActivePlayers() > 1)
                {
                    gameEngine.revealTurn();

                    shouldQuitAndSave = gameEngine.performBets();

                    if (shouldQuitAndSave)
                    {
                        break;
                    }

                    gameEngine.collectBets();
                }

                if (gameEngine.getTotalActivePlayers() > 1)
                {
                    gameEngine.revealRiver();

                    shouldQuitAndSave = gameEngine.performBets();

                    if (shouldQuitAndSave)
                    {
                        break;
                    }

                    gameEngine.collectBets();
                }

                if (gameEngine.getTotalActivePlayers() > 1)
                {
                    gameEngine.finishHand();
                }
                else
                {
                    gameEngine.endHandEarly();
                }
            }

            if (shouldQuitAndSave)
            {
                databaseEngine.insertSavedGame(gameEngine.CurrentPlayerTurn, gameEngine.CurrentHandNumber,
                    gameEngine.HandsToPlay, gameEngine.CurrentDealerPlayer, gameEngine.Difficulty,
                    databaseEngine.findCardNumber(gameEngine.getCard(0)),
                    databaseEngine.findCardNumber(gameEngine.getCard(1)),
                    databaseEngine.findCardNumber(gameEngine.getCard(2)),
                    databaseEngine.findCardNumber(gameEngine.getCard(3)),
                    databaseEngine.findCardNumber(gameEngine.getCard(4)),
                    gameEngine.CurrentTotalPot);

                int gameSaveID = databaseEngine.getMostRecentID();
                TablePlayer[] tablePlayers = gameEngine.TablePlayers;

                for (int i = 0; i < 4; i++)
                {
                    databaseEngine.insertTablePlayer(gameSaveID, i, tablePlayers[i].Chips,
                        tablePlayers[i].TotalBetThisBettingRound, tablePlayers[i].TotalBetThisHand,
                        databaseEngine.findCardNumber(tablePlayers[i].Card1),
                        databaseEngine.findCardNumber(tablePlayers[i].Card2),
                        tablePlayers[i].HasFolded);
                }
            }
            else
            {
                databaseEngine.addFinishedGame
                        (gameEngine.HandsToPlay, gameEngine.Difficulty, gameEngine.TablePlayers[0].Chips);
            }
        }
    }
}