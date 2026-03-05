using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
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
using System.Xml.Serialization;

namespace EjercicioFísico
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Ejercicio> Ejercicios { get; set; }
        //usamos observable collection para que la lista de ejercicios se actualice automaticamente en la vista
        //si se realiza alguna modificacion en la lista
        private VentanaDetallesEjercicio ventanaDetallesEjercicio;
        DateTime fechaSeleccionada = DateTime.Now;


        public MainWindow()
        {
            InitializeComponent();
           

            Ejercicios = new ObservableCollection<Ejercicio>
            {
                new Ejercicio ("Plancha", "Un ejercicio isométrico para trabajar el core, especialmente los abdominales.", "Core") ,
                new Ejercicio ( "Curl de Bíceps", "Un ejercicio simple pero efectivo para desarrollar los brazos, especialmente los bíceps.", "Brazos" ),
                new Ejercicio ( "Press de banca",  "Este ejercicio se realiza en una máquina guiada y permite trabajar los músculos del pecho con mayor control.", "Pecho" ),
                new Ejercicio (  "Jalón al pecho",  "Un ejercicio en máquina para trabajar la espalda, especialmente el dorsal ancho.","Espalda" ),
                new Ejercicio (  "Prensa de pierna",  "Una máquina guiada para trabajar los músculos de las piernas, especialmente los cuádriceps.", "Piernas" ),
                new Ejercicio (  "Extensión de pierna", "Este ejercicio se enfoca en el desarrollo de los cuádriceps mediante una máquina guiada.",  "Piernas" ),
                new Ejercicio ( "Press de hombros",  "Un ejercicio para trabajar los hombros utilizando una máquina guiada.",  "Brazos" )
            };

            EjerciciosDataGrid.ItemsSource = Ejercicios;
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void SalirAplicacion_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Desea Salir?",
                                "Salir",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }



        private void AbrirVentanaSecundaria(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (EjerciciosDataGrid.SelectedItem is Ejercicio ejercicioSeleccionado)
                {
                    // Verifica si la ventana secundaria está abierta
                    if (ventanaDetallesEjercicio == null || !ventanaDetallesEjercicio.IsVisible)
                    {
                        // Crear nueva ventana secundaria
                        ventanaDetallesEjercicio = new VentanaDetallesEjercicio(ejercicioSeleccionado)
                        {
                            Owner = this // Establecer la ventana principal como propietaria para que se cierre al cerrar la principal
                        };
                        ventanaDetallesEjercicio.Show();
                        ventanaDetallesEjercicio.Closed += (s, args) => ventanaDetallesEjercicio = null;

                        //suscribimos al evento de la ventana secundaria para que se actualice el grafico radial
                        ventanaDetallesEjercicio.EjecucionSeleccionada += VentanaDetalles_EjecucionSeleccionada;
                        // Suscribir al evento de adición de ejecución
                        ventanaDetallesEjercicio.EjecucionAñadida += VentanaDetalles_EjecucionAñadida;
                        // Suscribir al evento de eliminación de ejecución
                        ventanaDetallesEjercicio.EjecucionEliminada += VentanaDetalles_EjecucionEliminada;
                    }
                    else
                    {
                        // Reutilizar la ventana secundaria abierta y actualizar los datos
                        ventanaDetallesEjercicio.ActualizarDatosEjercicio(ejercicioSeleccionado);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Declaramos el controlador del evento de la ventana secundaria
        private void VentanaDetalles_EjecucionSeleccionada(object sender,VentanaDetallesEjercicio.EjecucionSeleccionadaEventArgs e)
        {
            //actualizamos el grafico radial
            CrearGraficoDaylyInsight(e.Fecha);
        }
        private void VentanaDetalles_EjecucionAñadida()
        {
            // Actualizar el gráfico radial con la fecha actualmente seleccionada
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void VentanaDetalles_EjecucionEliminada()
        {
            // Actualizar el gráfico radial con la fecha actualmente seleccionada
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void CrearGraficoDaylyInsight(DateTime fechaSeleccionada)
        {
            // Limpiar el canvas
            GraficoRadialCanvas.Children.Clear();

            if (FechaSeleccionadatxt == null)
            {
                FechaSeleccionadatxt = new TextBlock
                {
                    Foreground = Brushes.Black,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Text = $"Fecha seleccionada: {fechaSeleccionada:dd/MM/yyyy}" // Texto inicial
                };

                // Añadir el TextBlock al Canvas
                //GraficoRadialCanvas.Children.Add(FechaSeleccionadatxt);
            }
            else
            {
                // Actualizar el texto del TextBlock en llamadas posteriores
                FechaSeleccionadatxt.Text = $"Fecha seleccionada: {fechaSeleccionada:dd/MM/yyyy}";
            }
           

            // Grupos musculares en el orden deseado
            var gruposMusculares = new[] { "Core", "Espalda", "Pecho", "Brazos", "Piernas" };

            // Calcular las repeticiones acumuladas por grupo muscular en la fecha seleccionada
            var repeticionesPorGrupo = gruposMusculares.ToDictionary(
                grupo => grupo, //genera las claves del diccionario
                grupo => ObtenerRepeticionesPorGrupoYFecha(grupo, fechaSeleccionada)//genera los valores del diccionario
            );

            // Si no hay datos, mostrar mensaje y salir
            if (repeticionesPorGrupo.All(r => r.Value == 0))
            {
                var texto = new TextBlock
                {
                    Text = "No hay datos para la fecha seleccionada",
                    Foreground = Brushes.Black,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(texto, GraficoRadialCanvas.Width / 4);
                Canvas.SetTop(texto, GraficoRadialCanvas.Height / 2);
                GraficoRadialCanvas.Children.Add(texto);
                return;
            }

            // Configuración del gráfico
            double canvasCenterX = GraficoRadialCanvas.Width / 2;
            double canvasCenterY = GraficoRadialCanvas.Height / 2;
            double radioMaximo = Math.Min(GraficoRadialCanvas.Width, GraficoRadialCanvas.Height) / 2 - 70;
            double maxRepeticiones = 100;

            // Dibujar los ejes
            for (int i = 0; i < gruposMusculares.Length; i++)
            {
                // Calcular el ángulo en sentido horario, comenzando desde arriba
                double angle = i * (2 * Math.PI / gruposMusculares.Length) - Math.PI / 2;

                // Coordenadas del extremo del eje
                double x = canvasCenterX + radioMaximo * Math.Cos(angle);
                double y = canvasCenterY - radioMaximo * Math.Sin(angle);

                // Línea del eje desde el centro hasta el extremo
                var linea = new Line
                {
                    X1 = canvasCenterX,
                    Y1 = canvasCenterY,
                    X2 = x,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GraficoRadialCanvas.Children.Add(linea);

                // Etiqueta del eje
                var etiqueta = new TextBlock
                {
                    Text = gruposMusculares[i],
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold
                };

                // Ajustar la posición de las etiquetas
                if (Math.Abs(x - canvasCenterX) < 10) // Eje vertical (arriba o abajo)
                {
                    Canvas.SetLeft(etiqueta, x - 20); // Centrar etiqueta
                    Canvas.SetTop(etiqueta, y > canvasCenterY ? y + 10 : y - 25);
                }
                else if (x > canvasCenterX) // Ejes derechos
                {
                    Canvas.SetLeft(etiqueta, x + 10);
                    Canvas.SetTop(etiqueta, y - 10);
                }
                else // Ejes izquierdos
                {
                    Canvas.SetLeft(etiqueta, x - 50); // Más espacio a la izquierda
                    Canvas.SetTop(etiqueta, y - 10);
                }

                GraficoRadialCanvas.Children.Add(etiqueta);
            }

            // Dibujar el polígono
            var puntos = new PointCollection();
            foreach (var grupo in gruposMusculares)
            {
                double porcentaje = Math.Min(repeticionesPorGrupo[grupo] / maxRepeticiones, 1.0);
                double angle = Array.IndexOf(gruposMusculares, grupo) * (2 * Math.PI / gruposMusculares.Length) -Math.PI/2;

                double x = canvasCenterX + Math.Cos(angle) * radioMaximo * porcentaje;
                double y = canvasCenterY - Math.Sin(angle) * radioMaximo * porcentaje;

                puntos.Add(new Point(x, y));

                // Marcador para poner los puntos en el gráfico
                var marcador = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = Brushes.Red,
                    ToolTip = new ToolTip
                    {
                        Content = $"Repeticiones: {repeticionesPorGrupo[grupo]}"
                    }
                };
                Canvas.SetLeft(marcador, x - 6);
                Canvas.SetTop(marcador, y - 6);
                GraficoRadialCanvas.Children.Add(marcador);
            }

            // Dibujar el área
            var poligono = new Polygon
            {
                Points = puntos,
                Stroke = Brushes.Blue,
                Fill = Brushes.LightBlue,
                StrokeThickness = 2,
                Opacity = 0.6
            };
            GraficoRadialCanvas.Children.Add(poligono);
        }


        private int ObtenerRepeticionesPorGrupoYFecha(string grupoMuscular, DateTime fechaSeleccionada)
        {
            return Ejercicios
                  .Where(e => e.GruposMusculares.Contains(grupoMuscular))
                  .SelectMany(e => e.Ejecuciones)
                  .Where(e => e.Fecha.Date == fechaSeleccionada.Date)
                  .Sum(e => e.Repeticiones);
        }



        private void DiaAnterior_Click(object sender, RoutedEventArgs e)
        {
            fechaSeleccionada = fechaSeleccionada.AddDays(-1);
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void Hoy_Click(object sender, RoutedEventArgs e)
        {
            fechaSeleccionada = DateTime.Now;
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void DiaSiguiente_Click(object sender, RoutedEventArgs e)
        {
            fechaSeleccionada = fechaSeleccionada.AddDays(1);
            CrearGraficoDaylyInsight(fechaSeleccionada);
        }

        private void AñadirEjercicio_Click(object sender, RoutedEventArgs e)
        {
            var ventanaEjercicioNuevo= new VentanaEjercicioNuevo();
            if(ventanaEjercicioNuevo.ShowDialog() == true)
            {
                Ejercicios.Add(ventanaEjercicioNuevo.NuevoEjercicio);
            }
        }

        private void ModificarEjercicio_Click(object sender, RoutedEventArgs e)
        {
            
        }
        

        private void EliminarEjercicio_Click(object sender, RoutedEventArgs e)
        {
            if(EjerciciosDataGrid.SelectedItem is Ejercicio ejercicioSeleccionado)
            {
                if (MessageBox.Show($"¿Está seguro de eliminar el ejercicio {ejercicioSeleccionado.Nombre}?",
                    "Eliminar Ejercicio",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Ejercicios.Remove(ejercicioSeleccionado);
                }
                CrearGraficoDaylyInsight(fechaSeleccionada);
            }
            else
            {
                MessageBox.Show("Seleccione un ejercicio para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
