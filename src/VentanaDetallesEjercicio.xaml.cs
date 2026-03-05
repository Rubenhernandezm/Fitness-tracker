using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EjercicioFísico
{
    /// <summary>
    /// Lógica de interacción para VentanaDetallesEjercicio.xaml
    /// </summary>
    public partial class VentanaDetallesEjercicio : Window
    {
        private Ejercicio _ejercicioSeleccionado;

        public delegate void EjecucionSeleccionadaEventHandler(object sender, EjecucionSeleccionadaEventArgs e);
        public event EjecucionSeleccionadaEventHandler EjecucionSeleccionada;
        public event Action EjecucionEliminada;
        public event Action EjecucionAñadida;

        public VentanaDetallesEjercicio(Ejercicio ejercicioSeleccionado)
        {
            InitializeComponent();
            _ejercicioSeleccionado = ejercicioSeleccionado;
            this.Title = $"Detalles del ejercicio: {ejercicioSeleccionado.Nombre}";
            EjecucionesDataGrid.ItemsSource = _ejercicioSeleccionado.Ejecuciones;
            GraficoEjecuciones.SizeChanged += (s, e) => DibujarGrafico();//redibujar el grafico si cambia el tamaño de la ventana
        }

        public void ActualizarDatosEjercicio(Ejercicio ejercicioSeleccionado)
        {
            _ejercicioSeleccionado = ejercicioSeleccionado; // Actualizar la referencia al ejercicio seleccionado
            this.Title = $"Detalles del ejercicio: {ejercicioSeleccionado.Nombre}";
            EjecucionesDataGrid.ItemsSource = null;
            EjecucionesDataGrid.ItemsSource = _ejercicioSeleccionado.Ejecuciones;

            // Redibujar gráfico con los datos actualizados
            DibujarGrafico();
        }


        private void CerrarPestaña_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void AñadirEjecucion_Click(object sender, RoutedEventArgs e)
        {
            var nuevaEjecucionWindow = new VentanaNuevaEjecucion();
            if (nuevaEjecucionWindow.ShowDialog() == true) // Hacemos una ventana modal para que se espere a que se cierre
            {
                // Añadir la nueva ejecución al ejercicio mostrado
                _ejercicioSeleccionado.Ejecuciones.Add(nuevaEjecucionWindow.NuevaEjecucion);
                EjecucionesDataGrid.Items.Refresh();
                // Redibujar gráfico y actualizar tabla
                DibujarGrafico();
                // Disparar el evento para notificar a la ventana principal
                EjecucionAñadida?.Invoke();
            }
        }


        private void EliminarEjecucion_Click(object sender, RoutedEventArgs e)
        {
            if(EjecucionesDataGrid.SelectedItem is Ejecucion ejecucionSeleccionada)
            {
                if (MessageBox.Show($"¿Deseas eliminar la ejecucion seleccionada?\nRepeticiones: {ejecucionSeleccionada.Repeticiones}" +
                    $"\nPeso: {ejecucionSeleccionada.Peso}kg\nFecha: {ejecucionSeleccionada.Fecha}", "Eliminar Ejecución",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _ejercicioSeleccionado.Ejecuciones.Remove(ejecucionSeleccionada);
                    EjecucionesDataGrid.Items.Refresh();
                    DibujarGrafico();
                    // Disparar el evento para notificar a la ventana principal
                    EjecucionEliminada?.Invoke();
                }
                else
                    MessageBox.Show("Por favor, selecciona una ejecución para eliminar.", "Información", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DibujarGrafico()
        {
            // Limpiar cualquier contenido previo del Canvas
            GraficoEjecuciones.Children.Clear();

            // Tamaño del Canvas
            double anchoCanvas = GraficoEjecuciones.ActualWidth;
            double altoCanvas = GraficoEjecuciones.ActualHeight;

            // Verificar si hay ejecuciones disponibles
            if (_ejercicioSeleccionado.Ejecuciones == null || !_ejercicioSeleccionado.Ejecuciones.Any())
                return;

            // Encontrar los valores máximos para escalas
            double maxRepeticiones = _ejercicioSeleccionado.Ejecuciones.Max(e => e.Repeticiones);
            double maxPeso = _ejercicioSeleccionado.Ejecuciones.Max(e => e.Peso);

            if (maxRepeticiones == 0)
            {
                MessageBox.Show("No se pueden dibujar gráficos con valores máximos de repeticiones iguales a 0.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ajustar la altura de las barras para que coincida con las escalas
            double alturaEscala = altoCanvas * 0.8; // Usar solo el 80% del canvas para el gráfico
            double espacioInferior = altoCanvas * 0.1; // Reservar un 10% para etiquetas

            // Agrupar por fecha y ordenar por fecha
            var ejecucionesAgrupadas = _ejercicioSeleccionado.Ejecuciones
                .GroupBy(e => e.Fecha.Date)
                .OrderBy(grupo => grupo.Key) // Ordenar por fecha ascendente
                .ToList();

            // Ajuste dinámico del espacio entre grupos basado en el ancho del Canvas
            double espacioEntreGrupos = anchoCanvas / (ejecucionesAgrupadas.Count + 1);
            double anchoBarra = espacioEntreGrupos * 0.1; // Reducir el ancho de las barras para que sean más estrechas

            var polyline = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            for (int i = 0; i < ejecucionesAgrupadas.Count; i++)
            {
                var grupo = ejecucionesAgrupadas[i];
                double xPosicionInicial = (i + 1) * espacioEntreGrupos;

                int indiceBarra = 0;
                foreach (var ejecucion in grupo)
                {
                    // Dibujar la barra de repeticiones
                    double alturaBarra = (ejecucion.Repeticiones / maxRepeticiones) * alturaEscala;
                    Rectangle bar = new Rectangle
                    {
                        Width = anchoBarra,
                        Height = alturaBarra,
                        Fill = Brushes.Red,
                        ToolTip = new ToolTip
                        {
                            Content = $"Reps: {ejecucion.Repeticiones}, Peso: {ejecucion.Peso}kg, Fecha: {ejecucion.Fecha}"
                        }
                    };
                    double xPosicionBarra = xPosicionInicial + indiceBarra * (anchoBarra + 5); // Espacio entre barras
                    Canvas.SetLeft(bar, xPosicionBarra);
                    Canvas.SetBottom(bar, espacioInferior); // Reservar espacio inferior para etiquetas
                    GraficoEjecuciones.Children.Add(bar);

                    // Calcular el punto del peso para la Polyline
                    double pesoY = maxPeso > 0
                        ? altoCanvas - (ejecucion.Peso / maxPeso) * alturaEscala - espacioInferior
                        : altoCanvas - espacioInferior;

                    double xPeso = xPosicionBarra + anchoBarra / 2; // Centrar el punto en la barra

                    polyline.Points.Add(new System.Windows.Point(xPeso, pesoY));

                    // Dibujar marcador para el peso
                    Ellipse puntoPeso = new Ellipse
                    {
                        Width = 5,
                        Height = 5,
                        Fill = Brushes.Blue
                    };
                    Canvas.SetLeft(puntoPeso, xPeso - puntoPeso.Width / 2);
                    Canvas.SetTop(puntoPeso, pesoY - puntoPeso.Height / 2);
                    GraficoEjecuciones.Children.Add(puntoPeso);

                    // Incrementar el índice de la barra
                    indiceBarra++;
                }

                // Dibujar la etiqueta de la fecha debajo del grupo
                TextBlock fechaLabel = new TextBlock
                {
                    Text = grupo.Key.ToString("dd/MM/yyyy"),
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(fechaLabel, xPosicionInicial - 20);
                Canvas.SetTop(fechaLabel, altoCanvas - espacioInferior + 5); // Ajustar posición para que quede visible
                GraficoEjecuciones.Children.Add(fechaLabel);
            }

            // Añadir la Polyline al Canvas
            GraficoEjecuciones.Children.Add(polyline);

        // Indicadores laterales para repeticiones y peso
        DibujarIndicadores(alturaEscala, espacioInferior, maxRepeticiones, maxPeso);
        }

        private void DibujarIndicadores(double alturaEscala, double espacioInferior, double maxRepeticiones, double maxPeso)
        {
            // Ajustar indicadores laterales para que coincidan con la altura de las barras
            for (int i = 0; i <= 8; i++)
            {
                // Escala de repeticiones
                double valorRep = maxRepeticiones / 8 * i;
                TextBlock labelRep = new TextBlock
                {
                    Text = valorRep.ToString("0.0"),
                    Foreground = Brushes.Red,
                    FontWeight = FontWeights.Bold
                };
                double yPos = alturaEscala - (valorRep / maxRepeticiones) * alturaEscala + espacioInferior;
                Canvas.SetLeft(labelRep, 5);
                Canvas.SetTop(labelRep, yPos - 10); // Ajustar para evitar que se corte
                GraficoEjecuciones.Children.Add(labelRep);

                // Escala de peso
                double valorPeso = maxPeso / 8 * i;
                TextBlock labelPeso = new TextBlock
                {
                    Text = valorPeso.ToString("0.0"),
                    Foreground = Brushes.Blue,
                    FontWeight = FontWeights.Bold
                };
                yPos = alturaEscala - (valorPeso / maxPeso) * alturaEscala + espacioInferior;
                Canvas.SetRight(labelPeso, 5);
                Canvas.SetTop(labelPeso, yPos - 10); // Ajustar para evitar que se corte
                GraficoEjecuciones.Children.Add(labelPeso);
            }

            // Etiquetas adicionales
            TextBlock repsLabel = new TextBlock
            {
                Text = "reps",
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(repsLabel, 5);
            Canvas.SetBottom(repsLabel, 5);
            GraficoEjecuciones.Children.Add(repsLabel);

            TextBlock pesoLabel = new TextBlock
            {
                Text = "peso (kg)",
                Foreground = Brushes.Blue,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetRight(pesoLabel, 5);
            Canvas.SetBottom(pesoLabel, 5);
            GraficoEjecuciones.Children.Add(pesoLabel);
        }


        private void SeleccionEjecuciones(object sender,SelectionChangedEventArgs e)
        {
            if(EjecucionesDataGrid.SelectedItem is Ejecucion ejecucionSeleccionada)
            {
                var args = new EjecucionSeleccionadaEventArgs(ejecucionSeleccionada.Fecha);
                //disparar el evento
                DisparadorEvento(args);
            }
        }

        protected virtual void DisparadorEvento(EjecucionSeleccionadaEventArgs e)
        {
            if(EjecucionSeleccionada != null)
                EjecucionSeleccionada(this, e);
        }

        public class EjecucionSeleccionadaEventArgs : EventArgs
        {
            public DateTime Fecha { get; }

            public EjecucionSeleccionadaEventArgs(DateTime fecha)
            {
                Fecha = fecha;
            }
        }

    }
}

