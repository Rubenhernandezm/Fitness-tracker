using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace EjercicioFísico
{
    /// <summary>
    /// Lógica de interacción para VentanaEjercicioNuevo.xaml
    /// </summary>
    public partial class VentanaEjercicioNuevo : Window
    {
        public Ejercicio NuevoEjercicio { get; set; }
        public VentanaEjercicioNuevo()
        {
            InitializeComponent();
        }

        private void guardar_click(object sender,RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text)
                ||string.IsNullOrWhiteSpace(txtGruposMusculares.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!esCadenaCaracteres(txtNombre.Text)||!esCadenaCaracteres(txtDescripcion.Text)
                ||!esCadenaCaracteres(txtGruposMusculares.Text))
            {
                MessageBox.Show("Los campos solo pueden contener letras,espacios,puntos y comas.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NuevoEjercicio = new Ejercicio( 

                txtNombre.Text,
                txtDescripcion.Text,
                txtGruposMusculares.Text
            );
   
            DialogResult = true;
            Close();

        }

        private void cancelar_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool esCadenaCaracteres(string cadena)
        {
            return  cadena.All(c=>char.IsLetter(c)||char.IsWhiteSpace(c)||c=='.'||c==',');
        }
    }
}
