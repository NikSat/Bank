﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Bank
{
    internal static class Database
    {
        // Properties go here (the connection string) 

        private const string ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_1; Integrated Security = true";


        // Testing strings to simulate loss of connection  with the database 
        //private static string ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_1; Integrated Security = true";
        //ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_2; Integrated Security = true";




        internal static bool CheckConnection()
        {
            // Try catch structure to check the connection to the database
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    return false;
                }

                return true;

            }

        }


        internal static User CheckCredentials(string username, string password)
        {
            // Loss of connection here
            //ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_2; Integrated Security = true";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Connect to the database
                    conn.Open();
                    // First check if the user name exists

                    // Old query the passwords where unprotected
                    //string selectQuery = "SELECT [username],[password] FROM  [afdemp_csharp_1].[dbo].[users] WHERE [username] = @id";

                    //New query
                    string selectQuery = "OPEN SYMMETRIC KEY Bootkey DECRYPTION BY CERTIFICATE Bootcert; SELECT [username],CONVERT(varchar, DecryptByKey([password])) FROM  [afdemp_csharp_1].[dbo].[users] WHERE [username] = @id; CLOSE SYMMETRIC KEY Bootkey";

                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", SqlDbType.VarChar);
                        cmd.Parameters["@id"].Value = username;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            // First count them to see if there are people with the same name abd store them in the process
                            int o = 0;
                            List<Tuple<string, string>> Credentials = new List<Tuple<string, string>>();
                            while (reader.Read())
                            {
                                o++;
                                Tuple<string, string> temp = new Tuple<string, string>((string)reader[0], (string)reader[1]);
                                Credentials.Add(temp);

                            }

                            // If the user can't be found return null type

                            if (o == 0)
                            {
                                return User.Null;
                            }
                            else if (o == 1)
                            {
                                //Console.WriteLine($"Found ya");
                                string RetrievedPassword = Credentials[0].Item2;
                                if (RetrievedPassword == password)
                                {
                                    // We got a winner!
                                    // Now what kind of winner
                                    if (Credentials[0].Item1 == "admin" || Credentials[0].Item1 == "Admin")
                                    {
                                        return User.Admin;
                                    }
                                    else
                                    {
                                        return User.Simple;
                                    }


                                }
                                else
                                {
                                    return User.Null;
                                }



                            }
                            else
                            {
                                // If there are more users with the same name return error

                                return User.Error;
                            }
                        }
                    }

                }
                catch
                {
                    return User.Error;
                }


            }
        }



        internal static List<string> GetUsers()
        {
            // Loss of connection here
            //ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_2; Integrated Security = true";

            List<string> Users = new List<string>();
            try
            {


                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    // Connect to the database
                    conn.Open();
                    string selectQuery = "SELECT [username] FROM  [afdemp_csharp_1].[dbo].[users]";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Users.Add((string)reader[0]);
                            }
                        }
                    }
                }
            }
            catch
            {
                Users.Add("Error");
            }

            return Users;
        }




        internal static Tuple<DateTime,decimal> CheckBalance(string username)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Connect to the database
                    conn.Open();

                    string selectQuery = "SELECT [transaction_date],[amount] FROM [afdemp_csharp_1].[dbo].[accounts] INNER JOIN [afdemp_csharp_1].[dbo].[users] ON [afdemp_csharp_1].[dbo].[users].id=[afdemp_csharp_1].[dbo].[accounts].user_id WHERE [afdemp_csharp_1].[dbo].[users].[username]=@id";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", SqlDbType.VarChar);
                        cmd.Parameters["@id"].Value = username;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Tuple<DateTime,decimal> result=new Tuple<DateTime, decimal>((DateTime)reader[0] ,(decimal)reader[1]);
                            return result;
                        }

                    }
                }
                catch
                {
                    Tuple<DateTime, decimal> result = new Tuple<DateTime, decimal>(DateTime.Now, -1);
                    return result;
                }

            }

        }


        internal static Tuple<bool, decimal, decimal, DateTime> DepositTo(string sender, string receiver, decimal amount)
        {
            // Loss of connection here
            //ConnectionString = "Data Source=LAPTOP\\SQLEXPRESS;Initial Catalog =afdemp_csharp_2; Integrated Security = true";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    // Connect to the database
                    conn.Open();
                    // Do both updates in one query
                    string selectQuery = "UPDATE [accounts] SET [amount] =[amount] - @mn, [transaction_date]=CURRENT_TIMESTAMP FROM [accounts] INNER JOIN [users] ON [users].[id] =[accounts].[user_id] WHERE [afdemp_csharp_1].[dbo].[users].[username] = @id1;UPDATE [accounts] SET [amount] =[amount] + @mn, [transaction_date]=CURRENT_TIMESTAMP FROM [accounts] INNER JOIN [users] ON [users].[id] =[accounts].[user_id] WHERE [afdemp_csharp_1].[dbo].[users].[username] = @id2";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mn", SqlDbType.Money);
                        cmd.Parameters.AddWithValue("@id1", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@id2", SqlDbType.VarChar);
                        cmd.Parameters["@id1"].Value = sender;
                        cmd.Parameters["@id2"].Value = receiver;
                        cmd.Parameters["@mn"].Value = amount;
                        cmd.ExecuteNonQuery();

                    }
                    // Now get the account details for both
                    selectQuery = "SELECT [transaction_date],[amount] FROM [afdemp_csharp_1].[dbo].[accounts] INNER JOIN [afdemp_csharp_1].[dbo].[users] ON [afdemp_csharp_1].[dbo].[users].id=[afdemp_csharp_1].[dbo].[accounts].user_id WHERE [afdemp_csharp_1].[dbo].[users].[username]=@id1;SELECT [transaction_date],[amount] FROM [afdemp_csharp_1].[dbo].[accounts] INNER JOIN [afdemp_csharp_1].[dbo].[users] ON [afdemp_csharp_1].[dbo].[users].id=[afdemp_csharp_1].[dbo].[accounts].user_id WHERE [afdemp_csharp_1].[dbo].[users].[username]=@id2";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@mn", SqlDbType.Money);
                        cmd.Parameters.AddWithValue("@id1", SqlDbType.VarChar);
                        cmd.Parameters.AddWithValue("@id2", SqlDbType.VarChar);
                        cmd.Parameters["@id1"].Value = sender;
                        cmd.Parameters["@id2"].Value = receiver;
                        cmd.Parameters["@mn"].Value = amount;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // For sender
                            reader.Read();
                            Tuple<DateTime, decimal> result1 = new Tuple<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1]);
                            reader.NextResult();
                            // For receiver
                            reader.Read();
                            Tuple<DateTime, decimal> result2 = new Tuple<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1]);

                            //The results combined  successful - sender -receiver - time of action
                            Tuple<bool, decimal, decimal, DateTime> results = new Tuple<bool, decimal, decimal, DateTime>(true,result1.Item2,result2.Item2,result2.Item1);
                            return results;

                        }
                    }



                }
                catch
                {
                    Tuple<bool, decimal, decimal, DateTime> results = new Tuple<bool, decimal, decimal, DateTime>(false, 0, 0, DateTime.Now);

                    return results;
                }


            }
        }

        
    }
}