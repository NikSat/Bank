using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Bank
{

    internal abstract class AppMenu
    {
        // Attributes go here
        protected List<string> MenuChoices = new List<string>();
        protected List<string> Users = new List<string>();
        protected int index = 0;
        protected string Message;
        protected CultureInfo Culture;
        protected dynamic Inter;
        // Inter is dynamic can change to Admin or SimpleUser at runtime

        // Functions go here

        // A general function to build menus can be used by both child classes
        // This function actually makes a new menu everytime you press the arrow or enter

        public string BuildMenu(List<string> items, string message)
        {
            Console.Clear();            
            Console.WriteLine($"{message}\n\n");
            for (int i = 0; i < items.Count; i++)
            {
                if (i == index)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.WriteLine(items[i]);
                }
                else
                {
                    Console.WriteLine(items[i]);
                }
                Console.ResetColor();
            }

            ConsoleKeyInfo ckey = Console.ReadKey();

            if (ckey.Key == ConsoleKey.DownArrow)
            {
                if (index == items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
            }
            else if (ckey.Key == ConsoleKey.UpArrow)
            {
                if (index <= 0)
                {
                    index = items.Count - 1;
                }
                else
                {
                    index--;
                }
            }
            else if (ckey.Key == ConsoleKey.Enter)
            {
                string a= items[index];
                index = 0;
                return a;
            }



            return "";
        }

        // An abstract class used to make the menu which is unique to every child
        // Must be implemented by individual child 

        public abstract void ShowMenu();

        // Functions common to both users

        internal virtual void DepictBalance()
        {
            Console.Clear();
            Tuple<DateTime, decimal> retrieve = new Tuple<DateTime, decimal>(DateTime.Now, -1);
            retrieve = Inter.GetBalance(true);
            if (retrieve.Item2 == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n\t\tUnable to retrieve account balance. Transaction Failed.\n");
                Console.ResetColor();
                Console.Write("\nPress any key to continue...");
            }
            else
            {
                Console.Write("\nYour ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("account balance");
                Console.ResetColor();
                Console.Write($" on {DateTime.Now.ToString(Culture)} is {retrieve.Item2.ToString("c", Culture)}\n\nPress any key to continue...");

            }
            Console.ReadKey();

        }


        internal virtual void Deposit()
        {
            string Type = "Deposit to";
            index = 0;
            string dep;
            decimal deposit;
            decimal amount = Inter.GetBalance(false).Item2;
            if (amount == -1)
            {
                Inter.Logger(false,Type,"-",0,DateTime.Now,0);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t\tUnable to procceed. Transaction failed.\n");
                Console.ResetColor();
                Console.Write("\\nPlease try again later, press any key to continue...");
                Console.ReadKey();
                return;
            }
            string Selection = "";
            Console.Clear();
            Console.CursorVisible = false;
            string a = Inter.UserName;
            if (Users.Count == 1 && Users[0] == "Error")
            {
                Inter.Logger(false, Type, "-", 0, DateTime.Now, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t\tUnable to retrieve balance. Transaction failed.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            };
            Users.RemoveAll(x => x == Inter.UserName);
            string messa = "Please select the name of the user you would like to make a deposit to:";
            while (Selection.Length == 0)
            {
                Selection = BuildMenu(Users, messa);
            }
            Console.TreatControlCAsInput = false;
            Console.WriteLine($"\nYou have selected: {Selection}");
            Console.Write($"\nThe available founds at your disposal are {amount.ToString("c", Culture)}\n");
            Console.CursorVisible = true;
            Console.Write("\nNow enter the amount you would like to deposit: ");
            dep = Console.ReadLine();
            bool chec = Decimal.TryParse(dep, out deposit);
            int n = 0;
            while (!chec || (deposit > amount) || (deposit <= 0))
            {
                if (!chec)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You did not enter a number.\n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write("\nYou chose to try again.\nPlease enter the amount you would like to deposit: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit > amount)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You do not have sufficient founds to complete this transaction.\n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"\nYou chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit <= 0)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You may not use zero or a negative number.\n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"\nYou chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }

            }

            Tuple<bool, decimal, decimal, DateTime> success = Inter.Deposit(Selection, deposit);
            if (success.Item1)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n\t\tTransaction completed successfuly");
                Console.ResetColor();
                Console.Write(" on {0}.\n\t\t   Your current balance is {1} ", success.Item4.ToString(Culture),success.Item2.ToString(Culture));
                Console.Write("\n\nPress any key to continue...");
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n\t\tTransaction Failed");
                Console.ResetColor();
                Console.Write(" on {0}. \n\nPress any key to continue...", DateTime.Now.ToString(Culture));
            }
            Console.CursorVisible = false;
            Console.ReadKey();
        }

        internal virtual void SaveStatement()
        {
            Console.Clear();
            Console.Write("Creating logfile. ");
            Inter.Print();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;


        }

        internal virtual void Exit()
        {
            Console.Clear();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;
            //Console.TreatControlCAsInput = false;

        }


    }



    /// User menu child class, a small class: only individual menu and constructor



    internal class UserMenu : AppMenu
    {
        
        // User specific menu function

        public override void ShowMenu()
        {
            Console.CursorVisible = false;
            while (true)
            {
                string Selection = BuildMenu(MenuChoices, Message);
                switch (Selection)
                {
                    case "[View Personal Account]":
                        DepictBalance();
                        break;
                    case "[Deposit to Account]":
                        Deposit();
                        break;
                    case "[Save Statement to File and Exit]":
                        SaveStatement();
                        return;
                    case "[Exit Application]":
                        Exit();
                        return;
                }

            }

        }



        // Constructor goes here
        internal UserMenu(SimpleUser inter)
        {
            Inter = inter;
            MenuChoices = inter.Actions;
            Users = inter.Users;
            Message = inter.Message;
            Culture = new CultureInfo("el-GR");
            Console.OutputEncoding = Encoding.UTF8;
        }

    }







    /// Administrator child class: menu plus two extra functions

    internal class AdminMenu : AppMenu
    {
                
        // Function that creates the unique menu for admin

        public override void ShowMenu()
        {
            index = 0;
            Console.CursorVisible = false;
            while (true)
            {
                string Selection = BuildMenu(MenuChoices, Message);
                switch (Selection)
                {
                    case "[View Personal Account]":
                        DepictBalance();
                        break;
                    case "[View Member Accounts]":
                        ShowMember();
                        break;
                    case "[Deposit to Account]":
                        Deposit();
                        break;
                    case "[Withdraw from Account]":
                        Withdraw();
                        break;
                    case "[Save Statement to File and Exit]":
                        SaveStatement();
                        return;
                    case "[Exit Application]":
                        Exit();
                        return;
                }

            }

        }
        

        //Functions only applicable to the admin class


        // View other members accounts
        internal void ShowMember()
        {
            string Type = "View Balance of";
            Console.CursorVisible = false;
            string Selection = "";
            Console.Clear();
            if (Users.Count == 1 && Users[0] == "Error")
            {
                Inter.Logger(false, Type, "-", 0, DateTime.Now, 0);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t\tUnable to procceed. Transaction failed.\n");
                Console.ResetColor();
                Console.Write("\\nPlease try again later, press any key to continue...");
                Console.ReadKey();
                return;
            };
            Users.RemoveAll(x => x == Inter.UserName);
            string messa = "Please select the name of the user you would like to check";
            while (Selection.Length == 0)
            {
                Selection = BuildMenu(Users, messa);
            }
            Tuple<DateTime, decimal> result = new Tuple<DateTime, decimal>(DateTime.Now,-1);
            result = Inter.GetBalance(Selection,true);
            Console.Clear();
            if (result.Item2 == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n\t\tUnable to retrieve account balance. Transaction Failed.\n");
                Console.ResetColor();
                Console.Write("\nPress any key to continue...");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nAccount balance ");
                Console.ResetColor();
                Console.Write($"for user: {Selection} on {DateTime.Now.ToString(Culture)} is {result.Item2.ToString("c", Culture)}\n\nPress any key to continue...");

            }
            Console.ReadKey();

        }

        // Withdraw from accounts

        internal void Withdraw()
        {
            string Type = "Withdraw from";
            index = 0;
            string with;
            decimal withdraw;
            decimal amount;
            string Selection = "";
            Console.Clear();
            Console.CursorVisible = false;
            Users.RemoveAll(x => x == Inter.UserName);
            if (Users.Count == 1 && Users[0] == "Error")
            {
                Inter.Logger(false, Type, "-", 0, DateTime.Now, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n\t\tUnable to perform this request. Transaction failed.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            };
            string messa = "Please select the name of the user you would like to withdraw from:";
            while (Selection.Length == 0)
            {
                Selection = BuildMenu(Users, messa);
            }
            amount = Inter.GetBalance(Selection,false).Item2;
            if (amount==-1)
            {
                Inter.Logger(false, Type, "-", 0, DateTime.Now, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n\t\tUnable to perform this request. Transaction failed.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            Console.TreatControlCAsInput = false;
            Console.WriteLine($"\nYou have selected: {Selection}");
            Console.Write($"\nThe available founds at your disposal are {amount}\n");
            Console.CursorVisible = true;
            Console.Write("\nNow enter the amount you would like to withdraw: ");
            with = Console.ReadLine();
            bool chec = Decimal.TryParse(with, out withdraw);
            int n = 0;
            while (!chec || (withdraw > amount)|| (withdraw<=0))
            {
                if (!chec)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You did not enter a number.\n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write("\nYou chose to try again.\nPlease enter the amount you would like to withdraw: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }
                if (withdraw > amount)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You do not have sufficient founds to complete this transaction.\n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"\nYou chose to try again.\nPlease try again using a different amount which must be less than {amount}: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }
                if (withdraw <= 0)
                {
                    n++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\nWarning: ");
                    Console.ResetColor();
                    Console.Write("You may not use zero or a negative number. \n");
                    if (n == 3)
                    {
                        Console.Write("Three erroneous imputs. Returning to main menu.\nPress any key to continue...");
                        Console.ReadKey();
                        Console.CursorVisible = false;
                        return;
                    }
                    else
                    {
                        Console.Write("You can stop this transaction by pressing "); 
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("q");
                        Console.ResetColor();
                        Console.Write(" or press any key to try again.\n");
                    }
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"\nYou chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }




            }
            Tuple<bool, decimal, decimal, DateTime> success = Inter.Withdraw(Selection, withdraw);
            if (success.Item1)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n\t\tTransaction completed successfuly");
                Console.ResetColor();
                Console.Write(" on {0}.\n\t\t   Your current balance is {1} ", success.Item4.ToString(Culture), success.Item3.ToString(Culture));
                Console.Write("\n\nPress any key to continue...");
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n\t\tTransaction Failed");
                Console.ResetColor();
                Console.Write(" on {0}. \n\nPress any key to continue...", success.Item4.ToString(Culture));
            }
            Console.CursorVisible = false;
            Console.ReadKey();

        }




        // Constructor goes here
        internal AdminMenu(Admin inter)
        {
            Inter = inter;
            MenuChoices = inter.Actions;
            Users = inter.Users;
            Message = inter.Message;
            Culture = new CultureInfo("el-GR");
            Console.OutputEncoding = Encoding.UTF8;
        }




    }


}
