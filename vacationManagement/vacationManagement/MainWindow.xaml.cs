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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vacationManagement
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable employees;
        string conn;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            user.Content = ConfigurationManager.AppSettings["userName"];
            conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ConfigurationManager.AppSettings["excelPath"] + ";Extended Properties='Excel 12.0;HDR=YES;';";
            fillDataGrid();
            EmpBox1.Items.Clear();
            EmpBox1.SelectedIndex = EmpBox1.Items.Add("-- Seleccione al empleado --");
            foreach (DataRow employee in employees.Rows)
            {
                EmpBox1.Items.Add(employee["Trabajador"]);
            }
        }

        private void fillDataGrid()
        {
            employees = excelToTable(conn);
            gridEmployees.DataContext = new DataView(employees).ToTable(false, new string[] { "Trabajador", "Vacaciones" }).DefaultView;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<DataRow> boss = employees.Select().Where(x => string.Equals(x.Field<string>("Trabajador"), ConfigurationManager.AppSettings["userName"], StringComparison.OrdinalIgnoreCase));
                IEnumerable<DataRow> employee = employees.Select().Where(x => string.Equals(x.Field<string>("Trabajador"), EmpBox1.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase));

                if (Cipher.Decrypt(employee.First().Field<string>("Contraseña"), "pass") == EmpPassText.Password && Cipher.Decrypt(boss.First().Field<string>("Contraseña"), "pass") == JefePassText.Password)
                    updateExcel(EmpBox1.SelectedItem.ToString(), DaysText.Text);
                else
                {
                    MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }
                MessageBox.Show("El fichero ha sido actualizado correctamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el fichero. Compruebe el log para obtener mas información", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                using (StreamWriter writer = new StreamWriter($@"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{DateTime.Now.ToString("yyyyMMdd")}.log", true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString() + Environment.NewLine);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }
            finally
            {
                JefePassText.Password = "";
                EmpPassText.Password = "";
                EmpBox1.SelectedValue = "-- Seleccione al empleado --";
                DaysText.Text = "";
            }  
        }

        public static DataTable excelToTable(string conn)
        {
            DataTable dtResult = null;
            DataTable result = new DataTable();

            try
            {
                using (OleDbConnection objConn = new OleDbConnection(conn))
                {
                    objConn.Open();
                    DataSet ds = new DataSet();
                    OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" +
                        ConfigurationManager.AppSettings["excelSheet"] + "]", objConn);
                    OleDbDataAdapter oleda = new OleDbDataAdapter(cmd);
                    oleda.Fill(ds, "excelData");
                    dtResult = ds.Tables["excelData"];
                    objConn.Close();

                    IEnumerable<DataRow> employee = dtResult.Select();

                    if (employee.Count() > 0)
                    {
                        result = employee.CopyToDataTable();
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
        
        public void updateExcel(string employee, string days)
        {
            using (OleDbConnection objConn = new OleDbConnection(conn))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE [" +
                    ConfigurationManager.AppSettings["excelSheet"] + $"] SET Vacaciones = (Vacaciones - {days}), FechaActualizacion = '{DateTime.Now}' WHERE Trabajador = '{employee}'", objConn);
                cmd.ExecuteNonQuery();
            }
            fillDataGrid();
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            NewUser nu = new NewUser();
            if(nu.ShowDialog() == true)
            {
                fillDataGrid();
            }           
        }
    }
}
