using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
        }

        private void Createbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (OleDbConnection objConn = new OleDbConnection(conn))
                {
                    objConn.Open();
                    OleDbCommand cmd = new OleDbCommand("INSERT INTO [" +
                        ConfigurationManager.AppSettings["excelSheet"] + $"](Trabajador, Contraseña, Vacaciones, Administrador, FechaAlta) VALUES ('{user.Text}', '{Cipher.Encrypt(PassText.Password, "pass")}', '0', '0', '{DateTime.Now}')", objConn);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Empleado creado correctamente", "OK", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error al crear el usuario", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                using (StreamWriter writer = new StreamWriter($@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{DateTime.Now.ToString("yyyyMMdd")}.log", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString() + Environment.NewLine);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }

            this.DialogResult = true;
            this.Close();
            
        }
    }
}
