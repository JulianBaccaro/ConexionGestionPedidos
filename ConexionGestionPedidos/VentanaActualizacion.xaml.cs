using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Data;
using System.Configuration;

namespace ConexionGestionPedidos
{
    /// <summary>
    /// Lógica de interacción para VentanaActualizacion.xaml
    /// </summary>
    public partial class VentanaActualizacion : Window
    {
       private int z;
        SqlConnection miConexionSql = new SqlConnection();
        public VentanaActualizacion(int Id)
        {
            InitializeComponent();
            z = Id;
            string miConexion = ConfigurationManager.ConnectionStrings["ConexionGestionPedidos.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 string consulta = "UPDATE CLIENTE SET NOMBRE =@nombre WHERE Id =" + z;
                SqlCommand comando = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                comando.Parameters.AddWithValue("@nombre", txtCuadroActualizar.Text);
                comando.ExecuteNonQuery(); 
                miConexionSql.Close();
                MessageBox.Show("El cliente se ha modificado satisfactoriamente");
               
               this.Close();
               
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
