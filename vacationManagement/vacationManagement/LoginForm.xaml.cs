using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Lógica de interacción para LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
            ConfigurationManager.AppSettings["userName"] = user.Text;
            if(excelToTable(conn, user.Text, pass.Password))
            {
                this.DialogResult = true;
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Usuario incorrecto", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
        }

        public static Boolean excelToTable(string conn, string user, string pass)
        {
            DataTable dtResult = null;
            Boolean result = false;
            
            try
            {
                using (OleDbConnection objConn = new OleDbConnection(conn))
                {
                    objConn.Open();
                    DataSet ds = new DataSet();
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" +
                    ConfigurationManager.AppSettings["excelSheet"] +"]", objConn);
                    OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                    oleda.Fill(ds, "excelData");
                    dtResult = ds.Tables["excelData"];
                    objConn.Close();

                    IEnumerable<DataRow> employee = dtResult.Select().Where(x => string.Equals(x.Field<string>("Trabajador"), user, StringComparison.OrdinalIgnoreCase) && Cipher.Decrypt(x.Field<string>("Contraseña"), "pass") == pass
                                                                                    && x.Field<Double>("Administrador") == -1);

                    if (employee.Count() > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter($@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{DateTime.Now.ToString("yyyyMMdd")}.log", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString() + Environment.NewLine);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }

            return result;
        }
    }
}
