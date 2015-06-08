using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace TexasHoldemPoker
{
    /*
     * The DatabaseEngine class handles all of the interaction that the
     * application has with the MySQL database.
     */
    class DatabaseEngine
    {
        MySqlConnection connection;

        /*
         * The constructor of this class opens up a connection with the
         * database with given parameters. These parameters should
         * change if the name of the database change and/or a password
         * is added to the database in the future.
         */
        public DatabaseEngine()
        {
            connection = null;
            string server = "127.0.0.1";
            string database = "poker";

            try
            {
                string connectionString = @"Server=" + server + ";Uid=root";
                string connectionString2 = @"Server=" + server + ";Database=" + database + ";Uid=root";

                connection = new MySqlConnection(connectionString);
                connection.Open();

                // Check whether or not the database yet exists
                string sqlCheckDatabaseExistence =
                    "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'poker'";

                MySqlCommand existenceCheckCommand = new MySqlCommand(sqlCheckDatabaseExistence, connection);
                MySqlDataReader dataReader = existenceCheckCommand.ExecuteReader();

                // If the database does not exist
                if (!dataReader.HasRows)
                {
                    dataReader.Close();

                    MySqlCommand databaseCreateCommand = new MySqlCommand();
                    databaseCreateCommand.Connection = connection;

                    // Create the database
                    string createDatabase =
                        "CREATE DATABASE poker";

                    databaseCreateCommand.CommandText = createDatabase;
                    databaseCreateCommand.ExecuteNonQuery();

                    connection.Close();

                    connection = new MySqlConnection(connectionString2);
                    connection.Open();

                    string createTblsavedgame =
                        "CREATE TABLE tblsavedgame "
                            + "(id int(4) AUTO_INCREMENT,"
                            + " playerturn int(1),"
                            + " handnumber int(2),"
                            + " totalhands int(2),"
                            + " dealerplayer int(1),"
                            + " difficulty int(1),"
                            + " card1 int(2),"
                            + " card2 int(2),"
                            + " card3 int(2),"
                            + " card4 int(2),"
                            + " card5 int(2),"
                            + " pot int(4),"
                            + " PRIMARY KEY (id))";

                    string createTbltableplayer =
                        "CREATE TABLE tbltableplayer"
                            + "(gameid int(4) NOT NULL,"
                            + " playerid int(1) NOT NULL,"
                            + " totalchips int(4),"
                            + " betthisround int(4),"
                            + " betthishand int(4),"
                            + " card1 int(2),"
                            + " card2 int(2),"
                            + " folded int(1),"
                            + " PRIMARY KEY (gameid, playerid))";

                    string createTblcard =
                        "CREATE TABLE tblcard"
                            + "(id int(4) AUTO_INCREMENT,"
                            + " rank int(2),"
                            + " suit int(1),"
                            + " PRIMARY KEY (id))";

                    string createTblfinishedgame =
                        "CREATE TABLE tblfinishedgame"
                            + "(id int(4) AUTO_INCREMENT,"
                            + " hands int(2),"
                            + " difficulty int(1),"
                            + " finalamount int(4),"
                            + " day int(2),"
                            + " month int(2),"
                            + " year int(4),"
                            + " PRIMARY KEY (id))";

                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;

                    // Create the tblcard table
                    command.CommandText = createTblcard;
                    command.ExecuteNonQuery();

                    // Create the tblsavedgame table
                    command.CommandText = createTblsavedgame;
                    command.ExecuteNonQuery();

                    // Create the tbltableplayer table
                    command.CommandText = createTbltableplayer;
                    command.ExecuteNonQuery();

                    // Create the tblfinishedgame table
                    command.CommandText = createTblfinishedgame;
                    command.ExecuteNonQuery();

                    // Populate the tblcard table
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 1; j <= 13; j++)
                        {
                            string sqlInsertcard =
                                "INSERT INTO tblcard"
                                    + "(rank, suit)"
                                    + " VALUES "
                                    + "(" + j + ", " + i + ")";

                            command.CommandText = sqlInsertcard;
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    connection = new MySqlConnection(connectionString2);
                    connection.Open();
                }

                dataReader.Close();
            }
            catch (MySqlException exception)
            {
                Console.WriteLine("Error: " + exception.ToString());
                Console.ReadLine();
            }
        }

        /*
         * This method inserts a saved game into the database with the
         * entered parameters.
         */
        public void insertSavedGame(int playerTurn, int handNumber, int totalHands, int dealerPlayer, int difficulty, int card1, int card2, int card3, int card4, int card5, int pot)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "INSERT INTO tblsavedgame "
                + "(playerturn, handnumber, totalhands, dealerplayer, difficulty, card1, card2, card3, card4, card5, pot)"
                + " VALUES "
                + "(" + playerTurn + ", " + handNumber + ", " + totalHands + ", " + dealerPlayer + ", " + difficulty + ", " + card1 + ", " + card2 + ", " + card3 + ", " + card4 + ", " + card5 + ", " + pot + ")";

            command.ExecuteNonQuery();
        }

        /*
         * This method updates a saved game in the database with the
         * entered parameters.
         */
        public void updateSavedGame(int gameID, int playerTurn, int handNumber, int totalHands, int dealerPlayer, int difficulty, int card1, int card2, int card3, int card4, int card5, int pot)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE tblsavedgame"
                + " SET playerturn=" + playerTurn + ", handnumber=" + handNumber + ", totalhands=" + totalHands + ", dealerplayer=" + dealerPlayer + ","
                + " pot=" + pot + ", difficulty=" + difficulty + ","
                + " card1=" + card1 + ", card2=" + card2 + ", card3=" + card3 + ", card4=" + card4 + ", card5=" + card5
                + " WHERE id=" + gameID;

            command.ExecuteNonQuery();
        }

        /*
         * This method inserts details about a table player into the
         * database with the entered parameters.
         */
        public void insertTablePlayer(int gameID, int playerID, int totalChips, int betThisRound, int betThisHand, int card1, int card2, bool folded)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "INSERT INTO tbltableplayer "
                + "(gameID, playerID, totalchips, betthisround, betthishand, card1, card2, folded)"
                + " VALUES "
                + "(" + gameID + ", " + playerID + ", " + totalChips + ", " + betThisRound + ", " + betThisHand + ", " + card1 + ", " + card2 + ", " + boolToInt(folded) + ")";

            command.ExecuteNonQuery();
        }

        /*
         * This method updates details about a table player in the
         * database with the entered parameters.
         */
        public void updateTablePlayer(int gameID, int playerID, int totalChips, int betThisRound, int betThisHand, int card1, int card2, bool folded)
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "UPDATE tbltableplayer "
                + " SET totalchips=" + totalChips + ", betthisround=" + betThisRound + ", betthishand=" + betThisHand + ","
                + " card1=" + card1 + ", card2=" + card2 + ", folded=" + boolToInt(folded)
                + " WHERE gameid=" + gameID + " AND playerid=" + playerID;

            command.ExecuteNonQuery();
        }

        /*
         * This method finds and returns the ID of the card in the 
         * database with the entered parameters.
         */
        public int findCardNumber(int rank, int suit)
        {
            MySqlDataReader dataReader = null;
            int returnValue = 0;

            try
            {
                string query = "SELECT * FROM tblcard WHERE rank=" + rank + " AND suit=" + suit;

                MySqlCommand command = new MySqlCommand(query, connection);
                dataReader = command.ExecuteReader();

                dataReader.Read();
                returnValue = (int)dataReader["id"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }

            return returnValue;
        }

        /*
         * This method takes a Card object as a parameter and finds the
         * ID in the database which represents that card (by using the
         * method of the same name with integer parameters).
         * 
         * This exists for the convenience of finding card IDs when 
         * saving gameplay data.
         */
        public int findCardNumber(Card card)
        {
            if (card == null)
                return 0;
            else
                return findCardNumber(card.getValue(), card.getSuit());
        }

        /*
         * This method finds the ID of the saved game in the table with
         * the highest ID of the table. This is used to find out the ID
         * of the game most recently added, a piece of information that
         * is used as a foreign key for the rows of table player
         * details.
         */
        public int getMostRecentID()
        {
            MySqlDataReader dataReader = null;
            int returnValue = 0;

            try
            {
                string query = "SELECT MAX(id) FROM tblsavedgame";

                MySqlCommand command = new MySqlCommand(query, connection);
                dataReader = command.ExecuteReader();

                dataReader.Read();
                returnValue = (int)dataReader["MAX(id)"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }

            return returnValue;
        }

        /*
         * This method closes the database connection.
         */
        public void close()
        {
            if (connection != null)
                connection.Close();
        }

        /*
         * This method converts an integer to a boolean value
         *  0: false
         *  1: true
         */
        public bool intToBool(int i)
        {
            if (i == 0)
                return false;
            return true;
        }

        /*
         * This method converts a boolean value to an integer
         *  false: 0
         *  true: 1
         */
        public int boolToInt(bool b)
        {
            if (b)
                return 1;
            return 0;
        }

        /*
         * This method creates a GameEngine object with the details 
         * obtained from the row in the saved game database with the
         * ID of the integer parameter.
         */
        public GameEngine loadGameEngine(int gameID)
        {
            MySqlDataReader dataReader = null;
            MySqlDataReader playerDataReader = null;
            GameEngine loadedGameEngine = null;

            try
            {
                string query = "SELECT * FROM tblsavedgame WHERE id=" + gameID;

                MySqlCommand command = new MySqlCommand(query, connection);
                dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    dataReader.Read();

                    int difficulty = (int)dataReader["difficulty"];
                    int numberOfHands = (int)dataReader["totalhands"];

                    loadedGameEngine = new GameEngine(difficulty, numberOfHands);

                    loadedGameEngine.CurrentDealerPlayer = (int)dataReader["dealerplayer"];
                    loadedGameEngine.CurrentHandNumber = (int)dataReader["handnumber"];
                    loadedGameEngine.CurrentTotalPot = (int)dataReader["pot"];

                    Card[] loadedCommunityCards = new Card[5];
                    List<Card> activeCards = new List<Card>();

                    int[] cardIDs = new int[] { (int)dataReader["card1"],
                                        (int)dataReader["card2"],
                                        (int)dataReader["card3"],
                                        (int)dataReader["card4"],
                                        (int)dataReader["card5"] };

                    dataReader.Close();

                    for (int i = 0; i < 5; i++)
                    {
                        Card cardToAdd = getCardFromID(cardIDs[i]);
                        loadedCommunityCards[i] = cardToAdd;

                        if (cardToAdd != null)
                            activeCards.Add(cardToAdd);
                    }

                    TablePlayer[] tablePlayers = new TablePlayer[4];

                    loadedGameEngine.CommunityCards = loadedCommunityCards;

                    for (int i = 0; i < 4; i++)
                    {
                        string playerQuery = "SELECT * FROM tbltableplayer WHERE gameid=" + gameID + " AND playerid=" + i;

                        MySqlCommand playerCommand = new MySqlCommand(playerQuery, connection);
                        playerDataReader = playerCommand.ExecuteReader();

                        if (playerDataReader.HasRows)
                        {
                            TablePlayer tablePlayer;
                            playerDataReader.Read();

                            if (i == 0)
                            {
                                tablePlayer = new UserPlayer(i);
                            }
                            else
                            {
                                tablePlayer = new AIPlayer(i, difficulty);
                            }

                            tablePlayer.Chips = (int)playerDataReader["totalchips"];
                            tablePlayer.TotalBetThisHand = (int)playerDataReader["betthishand"];
                            tablePlayer.TotalBetThisBettingRound = (int)playerDataReader["betthisround"];
                            tablePlayer.HasFolded = intToBool((int)playerDataReader["folded"]);

                            int card1ID = (int)playerDataReader["card1"];
                            int card2ID = (int)playerDataReader["card2"];

                            playerDataReader.Close();

                            Card card1 = getCardFromID(card1ID); ;
                            Card card2 = getCardFromID(card2ID);

                            tablePlayer.giveDealtCards(card1, card2);

                            activeCards.Add(card1);
                            activeCards.Add(card2);

                            tablePlayers[i] = tablePlayer;
                        }
                    }

                    loadedGameEngine.TablePlayers = tablePlayers;

                    Deck loadedDeck = new Deck();

                    foreach (Card card in activeCards)
                    {
                        if (card == null)
                            Console.WriteLine("Null");
                        else
                            Console.WriteLine(card.getOutputString());
                    }

                    // Fills the deck except for those cards in "activeCards"
                    loadedDeck.fillDeck(activeCards);

                    loadedGameEngine.Deck = loadedDeck;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
                if (playerDataReader != null)
                    playerDataReader.Close();
            }

            return loadedGameEngine;
        }

        /*
         * This method returns a Card object containing the data from
         * the database from the row with the entered card ID.
         */
        public Card getCardFromID(int cardID)
        {
            Card cardToReturn = null;
            MySqlDataReader dataReader = null;

            if (cardID != 0)
            {
                try
                {
                    string query = "SELECT * FROM tblcard WHERE id=" + cardID;

                    MySqlCommand command = new MySqlCommand(query, connection);
                    dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        dataReader.Read();
                        cardToReturn = new Card((int)dataReader["suit"], (int)dataReader["rank"]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }
            }

            return cardToReturn;
        }

        /*
         * This method deletes a saved game with the entered ID.
         */
        public void deleteSavedGameAndPlayers(int gameID)
        {
            string sqlDeleteGame = "DELETE FROM tblsavedgame WHERE id=" + gameID;
            string sqlDeleteTablePlayers = "DELETE FROM tbltableplayer WHERE gameid=" + gameID;

            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;

            command.CommandText = sqlDeleteGame;
            command.ExecuteNonQuery();

            command.CommandText = sqlDeleteTablePlayers;
            command.ExecuteNonQuery();
        }

        public void addFinishedGame(int numberOfHands, int difficulty, int finalAmount)
        {
            DateTime now = DateTime.Now;

            string sqlAddFinishedGame =
                "INSERT INTO tblfinishedgame "
                    + " (hands, difficulty, finalamount, day, month, year)"
                    + " VALUES "
                    + " (" + numberOfHands + ", " + difficulty + ", " + finalAmount + ", "
                    + now.Day + ", " + now.Month + ", " + now.Year + ")";

            MySqlCommand command = new MySqlCommand();
            command.Connection = connection;

            command.CommandText = sqlAddFinishedGame;
            command.ExecuteNonQuery();

            double averageProfitPerHand = ((double)(finalAmount - 250)) / ((double)numberOfHands);
            averageProfitPerHand = Math.Round(averageProfitPerHand * 100.0) / 100.0;

            if (averageProfitPerHand >= 0)
                Console.Write("You made an average of " + averageProfitPerHand);
            else
                Console.Write("You lost an average of " + (0.0 - averageProfitPerHand));

            string[] difficultyStrings = new string[] { "easy", "medium", "hard" };
            Console.WriteLine(" chips per hand on difficulty '" + difficultyStrings[difficulty] + "' over the course of " + numberOfHands + " hands");
            Console.WriteLine("Press enter to return to main menu");
            Console.ReadLine();
        }

        /*
         * This method searches the database for possible games for the
         * user to load. It then outputs a list of these games and asks
         * the user for a valid selection.
         */
        public int chooseGameToLoad()
        {
            int gameToLoad = -1;

            MySqlDataReader dataReader = null;

            try
            {
                string query = "SELECT * FROM tblsavedgame";

                MySqlCommand command = new MySqlCommand(query, connection);
                dataReader = command.ExecuteReader();

                List<int> availableGames = new List<int>();

                if (dataReader.HasRows)
                {
                    Console.WriteLine("Available games to load: ");
                    string[] difficultyStrings = new string[] { "easy", "medium", "hard" };

                    while (dataReader.Read())
                    {
                        availableGames.Add((int)dataReader["id"]);

                        Console.WriteLine(dataReader["id"] + " - "
                            + "difficulty: " + difficultyStrings[(int)dataReader["difficulty"]]
                            + "; hand: " + dataReader["handnumber"] + " of "
                            + dataReader["totalhands"]);
                    }

                    Console.Write("Please pick a game to load: ");

                    while (!availableGames.Contains(gameToLoad))
                    {
                        Int32.TryParse(Console.ReadLine(), out gameToLoad);
                    }
                }
                else
                {
                    Console.WriteLine("No games to load.");
                    return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (dataReader != null)
                    dataReader.Close();
            }

            return gameToLoad;
        }

        /*
         * This method asks the user to select parameters for the games
         * about which they would like to view data.
         */
        public void loadStatistics()
        {
            List<int> difficultiesToCheck = new List<int>();
            List<int> handNumbersToCheck = new List<int>();

            string[] difficultiesStrings = new string[] { "Easy", "Medium", "Hard" };
            int[] handNumbers = new int[] { 5, 10, 15, 20 };

            for (int i = 0; i < difficultiesStrings.Length; i++)
            {
                Console.WriteLine("Would you like to see data about games of difficulty '" + difficultiesStrings[i] + "'?");

                if (getYesNoInput())
                    difficultiesToCheck.Add(i);
            }

            for (int i = 0; i < handNumbers.Length; i++)
            {
                Console.WriteLine("Would you like to see data about games of total hand number '" + handNumbers[i] + "'?");

                if (getYesNoInput())
                    handNumbersToCheck.Add(handNumbers[i]);
            }

            if (difficultiesToCheck.Count == 0)
            {
                Console.WriteLine("You must choose to view data about at least one difficulty.");
                loadStatistics();
            }
            else if (handNumbersToCheck.Count == 0)
            {
                Console.WriteLine("You must choose to view data about at least one total hand number.");
                loadStatistics();
            }
            else
            {
                MySqlDataReader dataReader = null;

                string difficultyString = "";

                for (int i = 0; i < difficultiesToCheck.Count; i++)
                {
                    if (i != 0)
                        difficultyString += " OR ";

                    difficultyString += "difficulty = " + difficultiesToCheck[i];
                }

                string handNumberString = "";

                for (int i = 0; i < handNumbersToCheck.Count; i++)
                {
                    if (i != 0)
                        handNumberString += " OR ";

                    handNumberString += "hands = " + handNumbersToCheck[i];
                }

                string queryString = "SELECT * FROM tblfinishedgame "
                    + " WHERE (" + difficultyString + ") AND (" + handNumberString + ")"
                    + " ORDER BY id ASC";

                try
                {
                    MySqlCommand command = new MySqlCommand(queryString, connection);
                    dataReader = command.ExecuteReader();

                    DateTime currentDate = DateTime.Now;

                    List<Tuple<double, double>> data = new List<Tuple<double, double>>();

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int numberOfHands = (int)dataReader["hands"];
                            int finalAmount = (int)dataReader["finalamount"];

                            int day = (int)dataReader["day"];
                            int month = (int)dataReader["month"];
                            int year = (int)dataReader["year"];
                            DateTime dateOfCompletion = new DateTime(year, month, day);

                            double dayDifference = (dateOfCompletion - currentDate).TotalDays;
                            double averageProfitPerHand = ((double)(finalAmount - 250)) / ((double)numberOfHands);

                            // Round the data to 2 decimal places
                            dayDifference = Math.Round(dayDifference * 100.0) / 100.0;
                            averageProfitPerHand = Math.Round(averageProfitPerHand * 100.0) / 100.0;

                            data.Add(new Tuple<double, double>(dayDifference, averageProfitPerHand));
                        }

                        string wolframString = "linear fit ";

                        for (int i = 0; i < data.Count; i++)
                        {
                            Tuple<double, double> datum = data[i];

                            if (i != 0)
                                wolframString += ", ";

                            wolframString += "{";
                            wolframString += datum.Item1;
                            wolframString += ", ";
                            wolframString += datum.Item2;
                            wolframString += "}";
                        }

                        Console.WriteLine("The following string may be entered into WolframAlpha to show the desired graph");
                        Console.WriteLine(wolframString);

                        Clipboard.Clear();
                        Clipboard.SetText(wolframString);

                        Console.WriteLine("The above string has been copied to your clipboard");
                        Console.WriteLine("Press enter to return to main menu");

                        Console.ReadLine();
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("No entries meet your criteria");
                        Console.WriteLine("Press enter to continue");

                        Console.ReadLine();
                        Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
                }
            }
        }

        public bool getYesNoInput()
        {
            char[] validInputs = new char[] { 'Y', 'N' };
            char input = ' ';

            Console.Write("Please select an option Y/N: ");

            while (!validInputs.Contains(input))
            {
                input = (char)Console.Read();
            }

            if (input == 'Y')
                return true;
            else
                return false;
        }
    }
}