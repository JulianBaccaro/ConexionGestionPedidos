using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ConexionGestionPedidos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection miConexionSql;

        public MainWindow()
        {
            InitializeComponent();
            //almaceno en un string la conexión a base de datos.
            string miConexion = ConfigurationManager.ConnectionStrings["ConexionGestionPedidos.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;
            //Especifico que mi conexion es del tipo SQL.
            miConexionSql = new SqlConnection(miConexion);
            RetornaClientes();
            RetornaTodosLosPedidos();
        }

        private void RetornaClientes()
        {
            try
            {
                string consulta = "select * from Cliente"; //Esto es lo que va a devolver mi lista. Todos los clientes.
                SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
                using (adaptadorSql)
                {
                    DataTable clientesTabla = new DataTable();
                    adaptadorSql.Fill(clientesTabla); //Metodo Fill(dataTable) Agrega filas a un Dataset
                    ListaClientes.DisplayMemberPath = "nombre";  //De la tabla, esta info voy a ver en ListBox
                    ListaClientes.SelectedValuePath = "Id"; //en el listbox seleccionamos un elemento y hacemos algo con el
                    ListaClientes.ItemsSource = clientesTabla.DefaultView; //Especificamos de donde viene la info del listbox
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            

        }

        private void RetornaPedidos()
        {
            try
            {
                string consulta = "SELECT * FROM PEDIDO P INNER JOIN CLIENTE C ON C.ID = P.codCliente " +
                    "WHERE C.ID=@ClienteId "; //Esto es lo que va a devolver mi lista. Todos los Pedidos.

                //esto nos permite ejecutar una instruccion SQL con parametro (En este caso el cliente seleccionado)
                SqlCommand comando = new SqlCommand(consulta, miConexionSql);


                SqlDataAdapter adaptadorSql = new SqlDataAdapter(comando);
                using (adaptadorSql)
                {
                    comando.Parameters.AddWithValue("@ClienteId", ListaClientes.SelectedValue);
                    DataTable pedidosTabla = new DataTable();
                    adaptadorSql.Fill(pedidosTabla); //Metodo Fill(dataTable) Agrega filas a un Dataset
                    pedidosCliente.DisplayMemberPath = "fechaPedido";  //De la tabla, esta info voy a ver en ListBox
                    pedidosCliente.SelectedValuePath = "Id"; //en el listbox seleccionamos un elemento y hacemos algo con el
                    pedidosCliente.ItemsSource = pedidosTabla.DefaultView; //Especificamos de donde viene la info del listbox
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RetornaTodosLosPedidos()
        {
            try
            {
                string consulta = "SELECT *, CONCAT(codCliente,' ', fechaPedido,' ', formaPago) AS INFOCOMPLETA FROM PEDIDO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, miConexionSql);
                using (adaptador)
                {
                    DataTable todosLosPedidos = new DataTable();
                    adaptador.Fill(todosLosPedidos);
                    todos_los_pedidos.DisplayMemberPath = "INFOCOMPLETA";
                    todos_los_pedidos.SelectedValuePath = "Id";
                    todos_los_pedidos.ItemsSource = todosLosPedidos.DefaultView;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            

        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(todos_los_pedidos.SelectedValue.ToString());
            string consulta = "DELETE FROM PEDIDO WHERE ID= @PEDIDOID";
            SqlCommand comando = new SqlCommand(consulta, miConexionSql);
            miConexionSql.Open();
            comando.Parameters.AddWithValue("@PEDIDOID", todos_los_pedidos.SelectedValue);
            comando.ExecuteNonQuery();
            miConexionSql.Close();
            RetornaTodosLosPedidos(); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtInsertaCliente.Text.Length == 0) throw new Exception("Debe ingresar un nombre");
                string consulta = "INSERT INTO CLIENTE (nombre) VALUES (@nombre)";
                SqlCommand comando = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                comando.Parameters.AddWithValue("@nombre", txtInsertaCliente.Text);
                comando.ExecuteNonQuery();
                miConexionSql.Close();
                RetornaClientes();
                txtInsertaCliente.Text = "";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //Boton borrar
        {
            try
            {
                string consulta = "DELETE FROM CLIENTE WHERE ID= @CLIENTEID";
                SqlCommand comando = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                comando.Parameters.AddWithValue("@CLIENTEID", ListaClientes.SelectedValue);
                comando.ExecuteNonQuery();
                miConexionSql.Close();
                RetornaClientes();
            }
            catch (Exception ex )
            {

                MessageBox.Show(ex.Message);
            }
           
          
        }

        private void ListaClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RetornaPedidos();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            VentanaActualizacion ventanaActualizar = new VentanaActualizacion((int)ListaClientes.SelectedValue);
            

            try
            {
                if (ListaClientes.SelectedValue == null) throw new Exception("Debe seleccionar un cliente para actualizar");
                string consulta = "select nombre from Cliente where Id = @ClienteId";  
                SqlCommand comando = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter adaptadorSql = new SqlDataAdapter(comando);
                using (adaptadorSql)
                {
                    comando.Parameters.AddWithValue("@ClienteId", ListaClientes.SelectedValue);
                    DataTable clientesTabla = new DataTable();
                    adaptadorSql.Fill(clientesTabla);
                    ventanaActualizar.txtCuadroActualizar.Text = clientesTabla.Rows[0]["nombre"].ToString();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            ventanaActualizar.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            RetornaClientes();
        }
    }
}
