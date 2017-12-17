using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;


namespace Bank
{
    /// <summary>
    /// This class handles the creation of the report files
    /// </summary>
    
    internal class FileAccess
    {
        // Properties go here
        private string DepictedUser { get; set; }
        protected List<Tuple<bool, string, string, DateTime, decimal, decimal>> PrintList { get; set; } = new List<Tuple<bool, string, string, DateTime, decimal, decimal>>();
        private string Statement { get; set; }
        private string Filename { get; set; }
        private CultureInfo Gr { get; set; } = new CultureInfo("el-Gr");
        // A pattern to name users        
        string patt = @"(^user)(\w)";

        // Functions go here

        internal string GiveName(string user)
        {
            DateTime today = DateTime.Now;
            string name ="_"+ today.ToString("dd") + "_" + today.Month.ToString(Gr) + "_" + today.Year.ToString(Gr) + ".txt";
            if (Regex.IsMatch(user, patt))
            {
                string[] one = Regex.Split(user, patt);
                name = "statement_" + one[1] + "_" + one[2] + name; 

            }
            else
            {
                name ="statement_"+ DepictedUser + name;
            }

            bool flag = false;
            while (File.Exists(name))
            {
                flag = true;
                name = Rename(name);
            }

            if (flag)
            {
                Console.WriteLine($" File allready exists, renaming to {name}.");
            }

            return name;
        }

        internal void CreateFile(string filename)
        {
            
            try
            {

                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine("\t\t\tCo-op Investment Bank\n\n");
                    sw.WriteLine($"Statement for user: {DepictedUser}\n");
                    sw.WriteLine($"Statement summary:");
                    sw.WriteLine(Statement);
                    sw.WriteLine();
                    if (PrintList.Count==0)
                    {
                        sw.WriteLine("No transactions took place today.");
                        
                    }
                    else
                    {
                        sw.WriteLine("No\t\tStatus\t\tTransaction type\t\tParticipant Name\t\tDate and Time\t\tAmount\t\tBalance");
                        for (int i=0;i<PrintList.Count;i++)
                        {
                            string amount;
                            if (PrintList[i].Item5==0)
                            {
                                amount = "-";
                            }
                            else
                            {
                                amount = PrintList[i].Item5.ToString("c", Gr);
                            }
                            string total;
                            if (PrintList[i].Item6 == -1|| PrintList[i].Item1==false)
                            {
                                total = "-"; 
                            }
                            else
                            {
                                total = PrintList[i].Item6.ToString("c", Gr);
                            }

                            if (PrintList[i].Item1==true)
                            {
                                sw.WriteLine((i+1).ToString()+"\t\tCOMPLETE\t\t"+ PrintList[i].Item2 + "\t\t" + PrintList[i].Item3 + "\t\t" + PrintList[i].Item4.ToString(Gr) + "\t\t" + amount + "\t\t" + total);

                            }
                            else
                            {
                                sw.WriteLine((i + 1).ToString() + "\t\t*FAILED*\t\t" + PrintList[i].Item2 + "\t\t" + PrintList[i].Item3 + "\t\t" + PrintList[i].Item4.ToString(Gr) + "\t\t" + amount + "\t\t" + total);

                            }

                        }
                        sw.WriteLine();
                        sw.WriteLine("Note: n.a. stands for \"not applicable\".");
                    }


                }

            }
            catch
            {
                Console.WriteLine("Something went wrong, unable to write file");
            }



        }



        internal string Rename(string nam)
        {
            string patt1 = @"\((\d*)\)";
            string name;
            if (Regex.IsMatch(nam, patt1))
            {
                string[] one = Regex.Split(nam, patt1); 
                name = one[0] + "(" + (int.Parse(one[1]) + 1).ToString() + ")" + one[2];

            }
            else
            {
                string[] two = nam.Split(".");
                name = two[0] + "(1)." + two[1];

            }
            return name;
        }

        // Constructors go here
        internal FileAccess(string user, List<Tuple<bool,string, string, DateTime, decimal, decimal>> list, string state)
        {
            DepictedUser = user;
            PrintList = list;
            Statement = state;
            Filename = GiveName(user);
            CreateFile(Filename);

        }

    }



}

