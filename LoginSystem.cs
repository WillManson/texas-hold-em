using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;

namespace TexasHoldemPoker
{
    /*
     * This class exists to handle the login functionality of the
     * application.
     */
    class LoginSystem
    {
        /*
         * fileName gives the location of the file containing the
         * hashed password on the user's drive.
         *  
         * folderName gives the folder which contains the above file.
         * 
         * folderName should change if the folder in which the hashed
         * password is to be saved changes.
         */
        static readonly string folderName = @"N:\pokerdata";
        static readonly string fileName = folderName + @"\hashedpassword.txt";

        /*
         * This method checks whether or not a given password is valid
         * (according to the objectives of the project) and returns a
         * boolean value to denote this.
         */
        public static bool isPasswordValid(string newPassword)
        {
            if (newPassword.Length < 8)
                return false;
            else if (newPassword.Length > 20)
                return false;
            // Check whether the password contains only letters
            else if (Regex.Match(newPassword, @"^[A-Za-z]+$").Success)
                return false;
            // Check whether the password contains only letters and
            // numbers (i.e. is valid, given that it must thus contain
            // at least one number since it failed the previous test)
            else if (Regex.Match(newPassword, @"^[A-Za-z0-9]+$").Success)
                return true;
            else
                return false;
        }

        /*
         * Save a new password to the position on the drive referenced
         * by fileName. This method creates the folder referenced by
         * folderName if it does not already exist
         */
        public static void saveNewPassword(string newPassword)
        {
            byte[] password = createHashedBytes(newPassword);

            // Create the folder if it does not yet exist
            if (!File.Exists(folderName))
                Directory.CreateDirectory(folderName);

            System.IO.File.WriteAllBytes(fileName, password);
        }

        /*
         * Check whether an entered password matches the password that
         * has been saved
         */
        public static bool checkEnteredPassword(string enteredPassword)
        {
            byte[] passwordToTest = createHashedBytes(enteredPassword);

            byte[] fileContents = System.IO.File.ReadAllBytes(fileName);

            string enteredString = BitConverter.ToString(passwordToTest);
            string actualPassword = BitConverter.ToString(fileContents);

            return enteredString == actualPassword;
        }

        /*
         * Create an array of bytes representing a hashed version of
         * the string.
         */
        public static byte[] createHashedBytes(string input)
        {
            HashAlgorithm shaAlgorithm = new SHA256CryptoServiceProvider();

            Byte[] inputByteArray = Encoding.UTF8.GetBytes(input);
            Byte[] hashedByteArray = shaAlgorithm.ComputeHash(inputByteArray);

            return hashedByteArray;
        }

        /*
         * This method checks to see whether a file already exists with
         * the path denoted by fileName. This determines whether or not
         * a password has already been created by the user.
         */
        public static bool hasCreatedPassword()
        {
            return File.Exists(fileName);
        }
    }
}