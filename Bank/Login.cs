﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bank
{

    public static class Login
    {

        /// <summary>
        /// 
        /// This class handles only  the communication with the user
        /// and prints and receives data from the terminal
        /// 
        /// </summary>

        public static void Greet()
        {
            Console.Clear();
            Console.Write(@" 
 Welcome to


        /$$$$$$                                       
       /$$__  $$                                      
      | $$  \__/  /$$$$$$           /$$$$$$   /$$$$$$ 
      | $$       /$$__  $$ /$$$$$$ /$$__  $$ /$$__  $$
      | $$      | $$  \ $$|______/| $$  \ $$| $$  \ $$
      | $$    $$| $$  | $$        | $$  | $$| $$  | $$
      |  $$$$$$/|  $$$$$$/        |  $$$$$$/| $$$$$$$/
       \______/  \______/          \______/ | $$____/ 
                                            | $$      
                                            | $$      
                                            |__/ 

                                    the Cooperative Investment Bank.");
            Console.WriteLine("\n\n\n\n");

        }


        public static void ConnectionSuccess()
        {
            Console.Write("Connection to database established succesfully\n\n");
        }

        public static void ConnectionFailure()
        {
            Console.Write("Unable to establish connection to database. Exiting now...");
            Console.ReadKey();
        }

        public static void CheckingConnection()
        {
            Console.Write("Checking connection to database...\n");
        }

        public static void ErrorMessage()
        {
            Console.Write("It appears there is a problem with verifying your credentials. Please try again later.\nIf the problem persists, contact a bank representative...\n");
        }




        public static string GetName()
        {
            string Name = "";
            Console.Write("Please enter your user name: ");
            Name=Console.ReadLine();
            

            return Name;
            //Console.TreatControlCAsInput = true;
        }



        public static string GetPassword()
        {

            ConsoleKeyInfo key;

            string Password = "";
            Console.Write("Enter your password: ");
            do
            {
                // This part replaces the password with stars
                // Read each key (ReadKey(true) hides the key from the console)
                key = Console.ReadKey(true);

                // Prevent the app from ending if CTL+C is pressed. *Overkill, decided not to use it after all
                //Console.TreatControlCAsInput = false;
                // This loop gets the password and allows pressing backspace to delete part of the password
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    Password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && Password.Length > 0)
                    {
                        Password = Password.Substring(0, (Password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            //Console.Write($"\nYou have typed: {Password}");
            Console.WriteLine();
            return Password;

        }


        // Warns the user of failure to authenticate display how many attempts are left
        public static void WarningMessage(int num)
        {
            string tr;
            tr = (num == 1) ? "try" : "tries";
            Console.Write("\nThe user name and password you have entered ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("do not match");
            Console.ResetColor();            
            Console.Write(" our user database.\n");
            Console.Write("You will be allowed ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{num}");
            Console.ResetColor();
            Console.Write($" more {tr} before the application terminates.\n\n");


        }


        // Asks the user whether he wants to continue 
        public static bool Ask()
        {
            Console.Write("If you would like to terminate now press ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("q");
            Console.ResetColor();
            Console.Write(" or press any key to try again.\n\n");
            ConsoleKeyInfo key;
            key = Console.ReadKey(true);
            if (key.KeyChar == 'q' || key.KeyChar == 'Q')
            {
                return false;
            }
            return true;

        }

        public static void Terminate()
        {
            Console.Write("The application will now be terminated...\n");
            Console.ReadKey();

        }




    }

}
