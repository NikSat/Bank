using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bank
{
    public class CoopBank
    {
        private static void Main(string[] args)
        {
            CoopBank MainApp = new CoopBank();

            MainApp.CheckConnection();
            MainApp.SecureInput();
            MainApp.InitiateInternal();

        }


        ///
        // The variables of the main function here
        ///

        private string UserName { get; set; }
        private bool Connect { get; set; }
        private int Count { get; set; } = 3;
        private User CurrentUser { get; set; }
        private InternalBank Current { get; set; }


        // Functions go here


        /// This part of the Main handles the login functions
        /// 
        /// Gets login name and password 
        /// and sets the CurrentUser variable
        /// 
        /// 
        /// Makes sure there is a user or exits the application 



        #region Login


        /// <summary>
        /// 
        /// This function checks the credentials and returns the user type
        /// 
        /// </summary>


        internal User CheckCredentials(string username, string password)
        {            
            return Database.CheckCredentials(username, password);
        }


        // Initialy check if connection with the database is possible
        public void CheckConnection()
        {
            Login.Greet();
            Login.CheckingConnection();
            Connect = Database.CheckConnection();
            if (Connect)
            {
                Login.ConnectionSuccess();
            }
            else
            {
                Login.ConnectionFailure();
                Environment.Exit(0);
            }


        }
            // Only have three chances of entering the credentials else the application terminates

            public void SecureInput()
        {
            bool Continue = true;
            UserName = Login.GetName();
            string Password = Login.GetPassword();
            CurrentUser = CheckCredentials(UserName, Password);


            while (Count > 1)
            {
                if (CurrentUser == User.Error)
                {
                    Login.ErrorMessage();
                    Login.Terminate();
                    Environment.Exit(0);
                    
                }
                
                if (CurrentUser == User.Null)
                {
                    Count--;
                    Login.WarningMessage(Count);
                    Continue = Login.Ask();
                    if (!Continue) { break; }
                    //Console.WriteLine($"Continue {Continue}");
                    UserName = Login.GetName();
                    Password = Login.GetPassword();
                    CurrentUser = CheckCredentials(UserName, Password);
                }
                else
                {
                    return;
                }

            }
            if (CurrentUser != User.Null)
            {
                return;
            }

            Login.Terminate();
            Environment.Exit(0);
        }

        #endregion


        /// This part starts the internal bank classes 
        /// from the user type and user name creates the
        /// corresponding internal data bank type
        /// Afterwards it calls the menu function which builds the main menu 



        #region Internal Bank

        /// <summary>
        /// 
        /// This function fires the corresponding internal bank type depending on the current user type
        /// 
        /// </summary>

        public void InitiateInternal()
        {
            switch (CurrentUser)
            {
                case User.Simple:
                    Current = new SimpleUser(UserName);
                    break;
                case User.Admin:
                    Current = new Admin(UserName);
                    break;

            }


        }



        #endregion

        // Constructors




    }


    internal enum User
    {
        Null,
        Simple,
        Admin,
        Error
    }

}
