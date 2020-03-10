using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace vacationManagement
{
    /// <summary>
    /// Lógica de interacción para NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {
        string conn;

        public NewUser()
        {
            InitializeComponent();
            conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
        }

        private void Createbtn_Click(object sender, RoutedEventArgs e)
        {
            using (OleDbConnection objConn = new OleDbConnection(conn))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand("INSERT INTO [" +
                    ConfigurationManager.AppSettings["excelSheet"] + $"](ID, Trabajador, Contraseña, Vacaciones, Administrador, Baja, FechaAlta) VALUES ('3', '{user.Text}', '{PassText.Password}', '0', '0', '0', '{DateTime.Now}')", objConn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
