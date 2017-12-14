using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Bank
{

    public abstract class AppMenu
    {
        // Attributes go here
        protected List<string> MenuChoices = new List<string>();
        protected List<string> Users = new List<string>();
        protected int index = 0;
        protected string Message;
        protected CultureInfo Culture;


        // Functions go here

        // A general functions to build menus can be used by both child classes
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

        // An abstract class used to make the menu 
        // Must be implemented by every child 

        public abstract void ShowMenu();

        // Functions common to both users


        internal abstract void DepictBalance();
        internal abstract void Deposit();
        internal abstract void SaveStatement();
        internal abstract void Exit();

    }



    /// User menu child class



    public class UserMenu : AppMenu
    {
        internal SimpleUser Inter;

        // User available functions

        public override void ShowMenu()
        {
            Console.CursorVisible = false;
            //Console.TreatControlCAsInput = true;
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

        // Functions to be implemented
        internal override void DepictBalance()
        {
            Console.Clear();
            Tuple<DateTime, decimal> retrieve = new Tuple<DateTime, decimal>(DateTime.Now, -1);
            retrieve = Inter.GetBalance(true);
            if (retrieve.Item2 == -1)
            {
                Console.Write("\n\n\t\tSomething went wrong, unable to retrieve account balance.\n\nPress any key to continue...");
            }
            else
            {
                Console.Write($"\nYour account balance on {DateTime.Now.ToString(Culture)} is {retrieve.Item2.ToString("c", Culture)}\n\nPress any key to continue...");

            }
            Console.ReadKey();

        }


        internal override void Deposit()
        {
            string Type = "Deposit to";
            index = 0;
            string dep;
            decimal deposit;
            decimal amount = Inter.GetBalance(false).Item2;
            if (amount == -1)
            {
                Inter.Logger("*FAILED " + Type, "-", 0, false);
                Console.Clear();
                Console.WriteLine("\n\n\t\tSomething went wrong....\n\nPlease try again later, press any key to continue");
                Console.ReadKey();
                return;
            }
            string Selection = "";
            Console.Clear();
            Console.CursorVisible = false;
            string a = Inter.UserName;
            if (Users.Count == 1 && Users[0]=="Error")
            {
                Inter.Logger("*FAILED " + Type, "-", 0, false);
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
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
            Console.Write($"\nThe available founds at your disposal are {amount.ToString("c",Culture)}\n");
            Console.CursorVisible = true;
            Console.Write("\nNow enter the amount you would like to deposit: ");
            dep = Console.ReadLine();
            bool chec = Decimal.TryParse(dep, out deposit);
            while (!chec || (deposit > amount) || (deposit <=0))
            {
                if (!chec)
                {
                    Console.Write("\nYou did not enter a number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write("You chose to try again.\nPlease enter the amount you would like to deposit: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit > amount)
                {
                    Console.Write("\nYou do not have sufficient founds to complete this transaction. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c",Culture)}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit <= 0)
                {
                    Console.Write("\nYou may not use zero or a negative number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                


            }            
            bool success=Inter.Deposit(Selection, deposit);
            if (success)
            {
                Console.Write("Transaction completed successfuly on {0}. \nPress any key to continue...",DateTime.Now.ToString(Culture));
            }
            else
            {
                Console.Write("Transaction failed on {0}. \nPress any key to continue...", DateTime.Now.ToString(Culture));

            }
            Console.CursorVisible = false;
            Console.ReadKey();
        }

        internal override void SaveStatement()
        {
            Console.Clear();
            Console.Write("Creating logfile. ");
            Inter.Print();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;


        }

        internal override void Exit()
        {
            Console.Clear();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;
            //Console.TreatControlCAsInput = false;

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

    /// Administrator child class

    public class AdminMenu : AppMenu
    {

        internal Admin Inter;

        // Administrator extra functions

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

        // Common functions

        internal override void DepictBalance()
        {
            Console.Clear();
            Tuple<DateTime, decimal> retrieve = new Tuple<DateTime, decimal>(DateTime.Now,-1);
            retrieve = Inter.GetBalance(true);
            if (retrieve.Item2 == -1)
            {
                Console.Write("\n\n\t\tSomething went wrong, unable to retrieve account balance.\n\nPress any key to continue...");
            }
            else
            {
                Console.Write($"\nYour account balance on {DateTime.Now.ToString(Culture)} is {retrieve.Item2.ToString("c", Culture)}\n\nPress any key to continue...");

            }
            Console.ReadKey();
        }

        internal override void Deposit()
        {
            index = 0;
            string dep;
            decimal deposit;
            string Type = "Deposit to";
            decimal amount = Inter.GetBalance(false).Item2;
            if (amount == -1)
            {
                Inter.Logger("*FAILED " + Type, "-", 0, false);
                Console.Clear();
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            string Selection = "";
            Console.Clear();
            Console.CursorVisible = false;
            string a = Inter.UserName;
            if (Users.Count == 1 && Users[0] == "Error")
            {
                Inter.Logger("*FAILED " + Type,"-",0,false);
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
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
            Console.Write($"\nThe available founds at your disposal are {amount}\n");
            Console.CursorVisible = true;
            Console.Write("\nNow enter the amount you would like to deposit: ");
            dep = Console.ReadLine();
            bool chec = Decimal.TryParse(dep, out deposit);
            while (!chec || (deposit > amount) || (deposit<=0))
            {
                if (!chec)
                {
                    Console.Write("\nYou did not enter a number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write("You chose to try again.\nPlease enter the amount you would like to deposit: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit > amount)
                {
                    Console.Write("\nYou do not have sufficient founds to complete this transaction. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount which must be less than {amount}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }
                if (deposit <= 0)
                {
                    Console.Write("\nYou may not use zero or a negative number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    dep = Console.ReadLine();
                    chec = Decimal.TryParse(dep, out deposit);
                }


            }
            bool success=Inter.Deposit(Selection, deposit);
            if (success)
            {
                Console.Write("Transaction completed successfuly on {0}. \nPress any key to continue...", DateTime.Now.ToString(Culture));
            }
            else
            {
                Console.Write("Transaction failed on {0}. \nPress any key to continue...", DateTime.Now.ToString(Culture));

            }
            Console.CursorVisible = false;
            Console.ReadKey();

        }

        internal override void SaveStatement()
        {
            Console.Clear();
            Console.Write("Creating logfile. ");
            Inter.Print();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;
        }

        internal override void Exit()
        {
            Console.Clear();
            Console.Write("Thank you for using Co-op Bank services.\nPress any key to exit.");
            Console.ReadKey();
            Console.CursorVisible = true;

        }








        // View other members accounts
        internal void ShowMember()
        {
            string Type = "View Balance of";
            Console.CursorVisible = false;
            string Selection = "";
            Console.Clear();
            if (Users.Count == 1 && Users[0] == "Error")
            {
                Inter.Logger("*FAILED " + Type, "-", 0, false);
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
                Console.WriteLine("\nPress any key to continue...");
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
            if (result.Item2 == -1)
            {
                Console.Write("\n\n\t\tSomething went wrong, unable to retrieve account balance.\n\nPress any key to continue...");
            }
            else
            {
                Console.Write($"\n\nAccount balance for user: {Selection} on {DateTime.Now.ToString(Culture)} is {result.Item2.ToString("c", Culture)}\n\nPress any key to continue...");

            }
            Console.ReadKey();

        }


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
                Inter.Logger("*FAILED " + Type, "-", 0, false);
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
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
                Inter.Logger("*FAILED " + Type, Selection, 0, false);
                Console.WriteLine("\n\n\t\tSomething went wrong, unable to perform this request.");
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
            while (!chec || (withdraw > amount))
            {
                if (!chec)
                {
                    Console.Write("\nYou did not enter a number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write("You chose to try again.\nPlease enter the amount you would like to withdraw: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }
                if (withdraw > amount)
                {
                    Console.Write("\nYou do not have sufficient founds to complete this transaction. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount which must be less than {amount}: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }
                if (withdraw <= 0)
                {
                    Console.Write("\nYou may not use zero or a negative number. \nYou can stop this transaction by pressing q or press any key to try again.\n");
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    key = Console.ReadKey(true);
                    if (key.KeyChar == 'Q' || key.KeyChar == 'q')
                    {
                        return;
                    }
                    Console.CursorVisible = true;
                    Console.Write($"You chose to try again.\nPlease try again using a different amount \nwhich must be less than {amount.ToString("c", Culture)}: ");
                    with = Console.ReadLine();
                    chec = Decimal.TryParse(with, out withdraw);
                }




            }
            bool success=Inter.Withdraw(Selection, withdraw);
            if (success)
            {
                Console.Write("Transaction completed successfuly on {0}. \nPress any key to continue...", DateTime.Now.ToString(Culture));
            }
            else
            {
                Console.Write("Transaction failed on {0}. \nPress any key to continue...", DateTime.Now.ToString(Culture));

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
