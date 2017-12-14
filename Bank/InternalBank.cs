using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Bank
{

    /// <summary>
    /// 
    /// The base class that handles the internal bank and calculations
    /// 
    /// Has two children : the Admin and SimpleUser 
    /// 
    /// </summary>



    internal abstract class InternalBank
    {
        // Properties go here

        internal string UserName { get; set; }
        internal List<string> Users { get; set; } = new List<string>();
        protected List<Tuple<string, string, DateTime,decimal,decimal>> SessionArchive = new List<Tuple<string, string, DateTime, decimal,decimal>>();
        internal List<string> Actions { get; set; }
        internal string Message { get; set; }
        protected AppMenu BankMenu;
        protected DateTime LastDate=DateTime.Now;
        protected Decimal LastBalance=-1;



        // Functions go here

        // Returns a list of users from the database

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();

            users = Database.GetUsers();


            return users;

        }

        // Gets the account info from the database

        public Tuple<DateTime, decimal> GetBalance(bool log)
        {
            Tuple<DateTime, decimal> result = new Tuple<DateTime, decimal>(DateTime.Now,-1);
            result = Database.CheckBalance(UserName);
            // If the log parameter is false do not log anything
            if (!log)
            {
                return result;
            }
            // This part handles the logging to the archive (-1 means there was an error and the balance cannot be retrieved)
            if (result.Item2==-1)
            {
                SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>("FAILED-View Personal Account", "n.a.", DateTime.Now, 0,LastBalance));
            }
            else
            {
                SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>("View Personal Account", "n.a.", DateTime.Now, 0, result.Item2));
                LastDate = result.Item1;
                LastBalance = result.Item2;
            }
            
            return result;

        }


        // Deposit to a user
        public bool Deposit(string user, decimal amount)
        {
            string TransactType = "Deposit to";
            bool success= Database.DepositTo(UserName, user, amount);
            Logger(TransactType,user,amount,success);
            return success;

        }

        // Print the session
        public void Print()
        {
            if (SessionArchive.Count==0)
            {
                Tuple<DateTime, decimal> previous = new Tuple<DateTime, decimal>(DateTime.Now,-1);
                previous = GetBalance(false);
                LastDate = previous.Item1;
                LastBalance = previous.Item2;
            }

            FileAccess PrintFile = new FileAccess(UserName,SessionArchive,this.ToString());

        }

        // This file logs the transactions to the sessions list - it also retrieves the current balance of the account
        internal void Logger(string TransactionType,string touser,decimal amoun,bool suc)
        {
            Tuple<DateTime, decimal> result = new Tuple<DateTime, decimal>(DateTime.Now, -1);
            result = Database.CheckBalance(UserName);

            // If the retrieval of the current balance fails for some reason just give the time it is now and the previous total ammount and warn with an asterisk

            if (result.Item2 == -1)
            {
                if (suc == false)
                {

                    string Report = "*FAILED-" + TransactionType;
                    SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>(Report, touser, DateTime.Now, amoun,LastBalance ));
                }
                else
                {
                    string Report = "*" + TransactionType;
                    SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>(Report, touser, DateTime.Now, amoun, LastBalance));

                }
            }
            else
            {
                if (suc == false)
                {
                    string Report = "*FAILED-" + TransactionType;
                    SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>(Report, touser, result.Item1, amoun, result.Item2));

                }
                else
                {
                    SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>(TransactionType, touser, result.Item1, amoun, result.Item2));
                    LastDate = result.Item1;
                    LastBalance = result.Item2;
                }
            }

        }

        // Override ToString()
        public override string ToString()
        {
            CultureInfo gr = new CultureInfo("el-GR");
            if (LastBalance == -1)
            {
                return "Unable to retrieve summary for " + UserName +  ". Possible connection Error.";
            }
            return "User name: " +UserName +" / Last transaction date: " +LastDate.ToString("d",gr) +" / Account Balance: " + LastBalance.ToString(gr);
        }




    }


    internal class SimpleUser : InternalBank
    {

        // Constructor for user
        internal SimpleUser(string name)
        {
            UserName = name;
            Actions = new List<string> { "[View Personal Account]", "[Deposit to Account]", "[Save Statement to File and Exit]", "[Exit Application]" };
            Message = $"Welcome user {name} \n\nBelow there is a selection of actions you can perform:";
            Users = GetUsers();
            BankMenu = new UserMenu(this);
            BankMenu.ShowMenu();


            // For testing
            //Console.WriteLine("I am a simple user");
            //Console.ReadKey();
        }

    }




    internal class Admin : InternalBank
    {

        // Extra Functions only the admin can do 

        // Withdraw from users
        public bool Withdraw(string user, decimal amount)
        {
            string TransactType = "Withdraw from";
            bool success=Database.DepositTo(user,UserName,amount);
            Logger(TransactType, user, amount, success);
            return success;
        }

        // See the balance of other users

        public Tuple<DateTime, decimal> GetBalance(string user,bool log)
        {
            Tuple<DateTime, decimal> result = new Tuple<DateTime, decimal>(DateTime.Now, -1);
            result = Database.CheckBalance(user);
            if (!log)
            {
                return result;
            }

            if (result.Item2 == -1)
            {
                SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>("FAILED-View Balance of", user, DateTime.Now, 0, 0));
            }
            else
            {
                SessionArchive.Add(new Tuple<string, string, DateTime, decimal, decimal>("View Balance of", user, DateTime.Now, 0, result.Item2));

            }

            return result;
        }


        // Constructor for Admin
        internal Admin(string name)
        {
            UserName = name;
            Actions = new List<string> { "[View Personal Account]", "[View Member Accounts]", "[Deposit to Account]", "[Withdraw from Account]", "[Save Statement to File and Exit]", "[Exit Application]" };
            Message = "Welcome Administrator \n\nBelow there is a selection of actions you can perform:";
            Users = GetUsers();
            BankMenu = new AdminMenu(this);
            BankMenu.ShowMenu();

            // For testing
            //Console.WriteLine("I am an Administrator");
            //Console.ReadKey();

        }


    }

}
