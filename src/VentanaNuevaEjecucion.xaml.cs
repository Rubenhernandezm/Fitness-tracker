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
using System.Windows.Shapes;

namespace EjercicioFísico
{
    /// <summary>
    /// Lógica de interacción para NuevaEjecucion.xaml
    /// </summary>
    public partial class VentanaNuevaEjecucion : Window
    {
        public Ejecucion NuevaEjecucion { get;   set; }
        public VentanaNuevaEjecucion()
        {
            InitializeComponent();
        }

        private void AceptarEjecucion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Intentar convertir los valores ingresados
                if (!int.TryParse(Repeticionestxbx.Text, out int repeticiones) || repeticiones < 0)
                {
                    MessageBox.Show("El número de repeticiones debe ser un valor positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Repeticionestxbx.Focus();
                    return;
                }

                if (!double.TryParse(Pesotxbx.Text, out double peso) || peso < 0)
                {
                    MessageBox.Show("El peso debe ser un valor positivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Pesotxbx.Focus();
                    return;
                }

                // Verificar la fecha
                DateTime fecha = FechaPicker.SelectedDate ?? DateTime.Now;

                // Crear la nueva ejecución
                NuevaEjecucion = new Ejecucion
                {
                    Repeticiones = repeticiones,
                    Peso = peso,
                    Fecha = fecha
                };

                DialogResult = true; // Cierra la ventana con éxito
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la ejecución: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelarEjecucion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
