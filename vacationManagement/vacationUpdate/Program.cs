using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace vacationUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
                updateExcel(conn);
            }
            catch(Exception ex)
            {
                using (StreamWriter writer = new StreamWriter($@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{DateTime.Now.ToString("yyyyMMdd")}.log", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString() + Environment.NewLine);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            
        }

        public static void updateExcel(string conn)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            int day = 0;
            int month = 0;
            int year = 0;
            int missingDays = 0;
            Boolean cond = true;
            int daysToAdd = 0;
            int check = 0;

            if (DateTime.Now.Day > 14)
                day = 15;
            else
                day = 1;
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;

            

            while (cond)
            {
                if(day == 1)
                {
                    if (day != Int32.Parse(config.AppSettings.Settings["lastUpdateDay"].Value) ||
                        month != Int32.Parse(config.AppSettings.Settings["lastUpdateMonth"].Value) ||
                        year != Int32.Parse(config.AppSettings.Settings["lastUpdateYear"].Value))
                    {
                        missingDays++;
                        day = 15;
                        if (month == 1)
                        {
                            month = 12;
                            year--;
                        }
                        else
                            month--;
                    }
                    else
                        cond = false;
                }
                else if (day == 15)
                {
                    if (day != Int32.Parse(config.AppSettings.Settings["lastUpdateDay"].Value) ||
                        month != Int32.Parse(config.AppSettings.Settings["lastUpdateMonth"].Value) ||
                        year != Int32.Parse(config.AppSettings.Settings["lastUpdateYear"].Value))
                    {
                        missingDays++;
                        day = 1;
                    }
                    else
                        cond = false;
                }
            }

            daysToAdd = Int32.Parse(config.AppSettings.Settings["addDays"].Value) * missingDays;
            
            using (OleDbConnection objConn = new OleDbConnection(conn))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE [" +
                    ConfigurationManager.AppSettings["excelSheet"] + $"] SET Vacaciones = (Vacaciones + {daysToAdd}), FechaActualizacion = '{DateTime.Now}'", objConn);
                check = cmd.ExecuteNonQuery();
            }

            if(check > 0)
            {
                if (DateTime.Now.Day > 14)
                    config.AppSettings.Settings["lastUpdateDay"].Value = "15";
                else
                    config.AppSettings.Settings["lastUpdateDay"].Value = "1";

                config.AppSettings.Settings["lastUpdateMonth"].Value = DateTime.Now.Month.ToString();
                config.AppSettings.Settings["lastUpdateYear"].Value = DateTime.Now.Year.ToString();
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }
    }
}
