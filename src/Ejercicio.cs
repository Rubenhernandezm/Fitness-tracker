using System.Collections.ObjectModel;
using System;

public class Ejercicio
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string GruposMusculares { get; set; }
    public ObservableCollection<Ejecucion> Ejecuciones { get; set; }

    public Ejercicio(string nombre, string descripcion, string gruposMusculares)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        GruposMusculares = gruposMusculares;

        // Inicializa las ejecuciones con datos específicos para cada ejercicio
        Ejecuciones = ObtenerEjecucionesPredeterminadas(nombre);
    }
    
    private ObservableCollection<Ejecucion> ObtenerEjecucionesPredeterminadas(string nombreEjercicio)
    {
        var ejecuciones = new ObservableCollection<Ejecucion>();

        // Definir ejecuciones predeterminadas para cada ejercicio
        switch (nombreEjercicio)
        {
            case "Plancha":
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now, Repeticiones = 60, Peso = 0 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now, Repeticiones = 70, Peso = 0 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now, Repeticiones = 80, Peso = 0 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(1), Repeticiones = 60, Peso = 0 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(1), Repeticiones = 80, Peso = 0 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(3), Repeticiones = 80, Peso = 0 });
                break;
            case "Prensa de pierna":
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now, Repeticiones = 12, Peso = 100 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now, Repeticiones = 15, Peso = 110 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(2), Repeticiones = 14, Peso = 115 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(2), Repeticiones = 12, Peso = 120 });
                ejecuciones.Add(new Ejecucion { Fecha = DateTime.Now.AddDays(5), Repeticiones = 15, Peso = 125 });
                break;
        }

        return ejecuciones;
    }
}

