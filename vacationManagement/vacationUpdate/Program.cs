using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vacationUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
            updateExcel(conn);
        }

        public static void updateExcel(string conn)
        {
            using (OleDbConnection objConn = new OleDbConnection(conn))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE [" +
                    ConfigurationManager.AppSettings["excelSheet"] + $"] SET Vacaciones = (Vacaciones + 2), FechaActualizacion = '{DateTime.Now}'", objConn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
