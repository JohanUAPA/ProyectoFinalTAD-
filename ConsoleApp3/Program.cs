using System;
using System.Collections.Generic;
using System.Linq;

// Clase que representa una tarea
public class Tarea
{
    // Propiedades de la tarea
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int Prioridad { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Categoria { get; set; }
    public string Subcategoria { get; set; }

    // Constructor de la tarea
    public Tarea(string titulo, string descripcion, int prioridad, DateTime fechaVencimiento, string categoria, string subcategoria)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        FechaVencimiento = fechaVencimiento;
        Categoria = categoria;
        Subcategoria = subcategoria;
    }

    // Sobrescribir ToString para mostrar la información de la tarea
    public override string ToString()
    {
        return $"Titulo: {Titulo}, Descripcion: {Descripcion} , Prioridad: {Prioridad}, Vencimiento: {FechaVencimiento.ToShortDateString()}, " +
            $"Categoria: {Categoria}/{Subcategoria}";
    
    }
}

// Clase que registra acciones en el historial (para deshacer y rehacer)
public class AccionHistorial
{
    public Tarea Tarea { get; set; }
    public string TipoAccion { get; set; } // "Agregar" o "Eliminar"
    public int? PosicionOriginal { get; set; } // Para restaurar tareas en su posición original

    // Constructor para crear un registro de acción
    public AccionHistorial(Tarea tarea, string tipoAccion, int? posicionOriginal = null)
    {
        Tarea = tarea;
        TipoAccion = tipoAccion;
        PosicionOriginal = posicionOriginal;
    }
}

// Clase Historial para manejar las pilas de deshacer y rehacer
public class Historial
{
    private Stack<AccionHistorial> accionesRealizadas = new Stack<AccionHistorial>();
    private Stack<AccionHistorial> accionesDeshechas = new Stack<AccionHistorial>();

    public void RegistrarAccion(AccionHistorial accion)
    {
        accionesRealizadas.Push(accion);
        accionesDeshechas.Clear();
    }

    // Deshacer la última acción realizada
    public AccionHistorial? DeshacerAccion(List<Tarea> listaDeTareas)
    {
        if (accionesRealizadas.Count > 0)
        {
            var accion = accionesRealizadas.Pop();
            accionesDeshechas.Push(accion);

            if (accion.TipoAccion == "Agregar")
            {
                listaDeTareas.Remove(accion.Tarea);
            }
            else if (accion.TipoAccion == "Eliminar")
            {
                if (accion.PosicionOriginal.HasValue)
                    listaDeTareas.Insert(accion.PosicionOriginal.Value, accion.Tarea);
                else
                    listaDeTareas.Add(accion.Tarea);
            }
            return accion;
        }
        return null;
    }

    // Rehacer la última acción deshecha
    public AccionHistorial? RehacerAccion(List<Tarea> listaDeTareas)
    {
        if (accionesDeshechas.Count > 0)
        {
            var accion = accionesDeshechas.Pop();
            accionesRealizadas.Push(accion);

            if (accion.TipoAccion == "Agregar")
            {
                listaDeTareas.Add(accion.Tarea);
            }
            else if (accion.TipoAccion == "Eliminar")
            {
                listaDeTareas.Remove(accion.Tarea);
            }
            return accion;
        }
        return null;
    }
}

// Clase para manejar la cola de tareas urgentes
public class ColaUrgente
{
    private Queue<Tarea> tareasUrgentes = new Queue<Tarea>();

    public void AgregarTareaUrgente(Tarea tarea)
    {
        tareasUrgentes.Enqueue(tarea);
    }

    // Procesar (remover) la primera tarea urgente en la cola
    public Tarea? ProcesarTareaUrgente()
    {
        if (tareasUrgentes.Count > 0)
        {
            return tareasUrgentes.Dequeue();
        }
        return null;
    }
}

// Clase para representar un nodo en el árbol de categorías
public class NodoCategoria
{
    public string Nombre { get; set; }
    public List<Tarea> Tareas { get; set; } = new List<Tarea>();
    public List<NodoCategoria> Subcategorias { get; set; } = new List<NodoCategoria>();

    // Constructor del nodo categoría
    public NodoCategoria(string nombre)
    {
        Nombre = nombre;
    }

    // Agregar una subcategoría al nodo actual
    public void AgregarSubcategoria(NodoCategoria subcategoria)
    {
        Subcategorias.Add(subcategoria);
    }

    // Agregar una tarea a la categoría actual
    public void AgregarTarea(Tarea tarea)
    {
        Tareas.Add(tarea);
    }

    // Mostrar todas las tareas de la categoría y subcategorías
    public void MostrarTareas(int nivel = 0)
    {
        Console.WriteLine(new string(' ', nivel * 2) + Nombre);
        foreach (var tarea in Tareas)
        {
            Console.WriteLine(new string(' ', (nivel + 1) * 2) + tarea);
        }
        foreach (var subcategoria in Subcategorias)
        {
            subcategoria.MostrarTareas(nivel + 1);
        }
    }
}

// Clase principal que contiene el programa
public class Programa
{
    private static List<Tarea> listaDeTareas = new List<Tarea>();
    private static Historial historial = new Historial();
    private static ColaUrgente colaUrgente = new ColaUrgente();
    private static NodoCategoria raizCategoria = new NodoCategoria("Raíz");

    // Método principal (punto de entrada)
    public static void Main(string[] args)
    {
        bool salir = false;
        while (!salir)
        {
            try
            {
                // Menú de opciones
                Console.WriteLine("\nGestión de Tareas:");
                Console.WriteLine("1. Agregar Tarea");
                Console.WriteLine("2. Eliminar Tarea");
                Console.WriteLine("3. Modificar Tarea");
                Console.WriteLine("4. Mostrar Tareas Ordenadas");
                Console.WriteLine("5. Deshacer Acción");
                Console.WriteLine("6. Rehacer Acción");
                Console.WriteLine("7. Agregar Tarea Urgente");
                Console.WriteLine("8. Procesar Tarea Urgente");
                Console.WriteLine("9. Mostrar Tareas por Categoría");
                Console.WriteLine("10. Salir");
                Console.Write("Seleccione una opción: ");
                int opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1:
                        AgregarTarea();
                        break;
                    case 2:
                        EliminarTarea();
                        break;
                    case 3:
                        ModificarTarea();
                        break;
                    case 4:
                        MostrarTareasOrdenadas();
                        break;
                    case 5:
                        DeshacerAccion();
                        break;
                    case 6:
                        RehacerAccion();
                        break;
                    case 7:
                        AgregarTareaUrgente();
                        break;
                    case 8:
                        ProcesarTareaUrgente();
                        break;
                    case 9:
                        MostrarTareasPorCategoria();
                        break;
                    case 10:
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Método para agregar una nueva tarea
    private static void AgregarTarea()
    {
        Console.Write("Título: ");
        string titulo = Console.ReadLine();
        if (listaDeTareas.Any(t => t.Titulo == titulo))
        {
            Console.WriteLine("Ya existe una tarea con este título.");
            return;
        }

        Console.Write("Descripción: ");
        string descripcion = Console.ReadLine();
        Console.Write("Prioridad (1-5): ");
        int prioridad;
        while (!int.TryParse(Console.ReadLine(), out prioridad) || prioridad < 1 || prioridad > 5)
        {
            Console.WriteLine("Prioridad no válida. Ingrese un número entre 1 y 5.");
        }

        Console.Write("Fecha de Vencimiento (yyyy-mm-dd): ");
        DateTime fechaVencimiento;
        while (!DateTime.TryParse(Console.ReadLine(), out fechaVencimiento) || fechaVencimiento < DateTime.Now)
        {
            Console.WriteLine("Fecha no válida o ya pasada. Inténtelo de nuevo.");
        }

        Console.Write("Categoría: ");
        string categoria = Console.ReadLine();
        Console.Write("Subcategoría: ");
        string subcategoria = Console.ReadLine();

        var tarea = new Tarea(titulo, descripcion, prioridad, fechaVencimiento, categoria, subcategoria);
        listaDeTareas.Add(tarea);
        historial.RegistrarAccion(new AccionHistorial(tarea, "Agregar", listaDeTareas.Count - 1));

        AgregarTareaACategoria(tarea);
        Console.WriteLine("Tarea agregada exitosamente.");
    }

    // Método para agregar la tarea a la estructura del árbol de categorías
    private static void AgregarTareaACategoria(Tarea tarea)
    {
        NodoCategoria categoriaNodo = raizCategoria.Subcategorias.Find(c => c.Nombre == tarea.Categoria);
        if (categoriaNodo == null)
        {
            categoriaNodo = new NodoCategoria(tarea.Categoria);
            raizCategoria.AgregarSubcategoria(categoriaNodo);
        }

        NodoCategoria subcategoriaNodo = categoriaNodo.Subcategorias.Find(sc => sc.Nombre == tarea.Subcategoria);
        if (subcategoriaNodo == null)
        {
            subcategoriaNodo = new NodoCategoria(tarea.Subcategoria);
            categoriaNodo.AgregarSubcategoria(subcategoriaNodo);
        }

        subcategoriaNodo.AgregarTarea(tarea);
    }

    // Método para eliminar una tarea
    private static void EliminarTarea()
    {
        Console.Write("Título de la tarea a eliminar: ");
        string titulo = Console.ReadLine();
        var tarea = listaDeTareas.FirstOrDefault(t => t.Titulo == titulo);
        if (tarea != null)
        {
            int posicionOriginal = listaDeTareas.IndexOf(tarea);
            listaDeTareas.Remove(tarea);
            historial.RegistrarAccion(new AccionHistorial(tarea, "Eliminar", posicionOriginal));
            Console.WriteLine("Tarea eliminada exitosamente.");
        }
        else
        {
            Console.WriteLine("Tarea no encontrada.");
        }
    }

    // Método para modificar una tarea
    private static void ModificarTarea()
    {
        Console.Write("Título de la tarea a modificar: ");
        string titulo = Console.ReadLine();
        var tarea = listaDeTareas.FirstOrDefault(t => t.Titulo == titulo);
        if (tarea != null)
        {
            Console.WriteLine("Deje en blanco si no desea modificar el campo.");
            Console.Write("Nuevo título: ");
            string nuevoTitulo = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoTitulo))
                tarea.Titulo = nuevoTitulo;

            Console.Write("Nueva descripción: ");
            string nuevaDescripcion = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaDescripcion))
                tarea.Descripcion = nuevaDescripcion;

            Console.Write("Nueva prioridad (1-5): ");
            string nuevaPrioridad = Console.ReadLine();
            if (int.TryParse(nuevaPrioridad, out int prioridad) && prioridad >= 1 && prioridad <= 5)
                tarea.Prioridad = prioridad;

            Console.Write("Nueva fecha de vencimiento (yyyy-mm-dd): ");
            string nuevaFecha = Console.ReadLine();
            if (DateTime.TryParse(nuevaFecha, out DateTime fechaVencimiento) && fechaVencimiento >= DateTime.Now)
                tarea.FechaVencimiento = fechaVencimiento;

            Console.Write("Nueva categoría: ");
            string nuevaCategoria = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaCategoria))
                tarea.Categoria = nuevaCategoria;

            Console.Write("Nueva subcategoría: ");
            string nuevaSubcategoria = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaSubcategoria))
                tarea.Subcategoria = nuevaSubcategoria;

            historial.RegistrarAccion(new AccionHistorial(tarea, "Modificar"));
            Console.WriteLine("Tarea modificada exitosamente.");
        }
        else
        {
            Console.WriteLine("Tarea no encontrada.");
        }
    }

    // Método para mostrar las tareas ordenadas por prioridad
    private static void MostrarTareasOrdenadas()
    {
        if (listaDeTareas.Count > 0)
        {
            var tareasOrdenadas = listaDeTareas.OrderBy(t => t.Prioridad).ThenBy(t => t.FechaVencimiento).ToList();
            foreach (var tarea in tareasOrdenadas)
            {
                Console.WriteLine(tarea);
            }
        }
        else
        {
            Console.WriteLine("No hay tareas para mostrar.");
        }
    }

    // Método para deshacer la última acción realizada
    private static void DeshacerAccion()
    {
        var accion = historial.DeshacerAccion(listaDeTareas);
        if (accion != null)
        {
            Console.WriteLine($"Acción deshecha: {accion.TipoAccion} - {accion.Tarea.Titulo}");
        }
        else
        {
            Console.WriteLine("No hay acciones para deshacer.");
        }
    }
    // Método para rehacer la última acción deshecha
    private static void RehacerAccion()
    {
        var accion = historial.RehacerAccion(listaDeTareas);
        if (accion != null)
        {
            Console.WriteLine($"Acción rehecha: {accion.TipoAccion} - {accion.Tarea.Titulo}");
        }
        else
        {
            Console.WriteLine("No hay acciones para rehacer.");
        }
    }

    // Método para agregar una tarea urgente a la cola
    private static void AgregarTareaUrgente()
    {
        Console.Write("Título de la tarea urgente: ");
        string titulo = Console.ReadLine();
        var tarea = listaDeTareas.FirstOrDefault(t => t.Titulo == titulo);
        if (tarea != null)
        {
            colaUrgente.AgregarTareaUrgente(tarea);
            Console.WriteLine("Tarea agregada a la cola de urgentes.");
        }
        else
        {
            Console.WriteLine("Tarea no encontrada.");
        }
    }

    // Método para procesar (remover) la tarea más urgente en la cola
    private static void ProcesarTareaUrgente()
    {
        var tarea = colaUrgente.ProcesarTareaUrgente();
        if (tarea != null)
        {
            Console.WriteLine("Tarea urgente procesada: " + tarea);
        }
        else
        {
            Console.WriteLine("No hay tareas urgentes para procesar.");
        }
    }
    // Método para mostrar las tareas organizadas por categoría y subcategoría
    private static void MostrarTareasPorCategoria()
    {
        raizCategoria.MostrarTareas();
    }
}

