using System;                                                      // Necesario para tipos base y Console

class Program
{
    static void Main()
    {
        GestorMundial gestor = new GestorMundial();                 // Instancia principal del sistema
        bool salir = false;                                          // Bandera para controlar el ciclo del menu

        while (!salir)                                                // Ciclo principal del menu
        {
            MostrarMenu();                                            // Imprimimos las opciones disponibles
            string opcion = Console.ReadLine();                        // Leemos la opcion elegida

            switch (opcion)                                            // Ejecutamos la accion correspondiente
            {
                case "1": RegistrarManual(gestor); break;               // Registrar jugador por teclado
                case "2": CargarArchivo(gestor); break;                 // Cargar jugadores desde CSV
                case "3": BuscarJugador(gestor); break;                 // Buscar por Id
                case "4": ActualizarEstadisticas(gestor); break;        // Actualizar estadisticas
                case "5": EliminarJugador(gestor); break;               // Eliminar por Id
                case "6": gestor.MostrarTodosPorId(); break;             // Listado general por Id
                case "7": ListadoPorCategoria(gestor); break;            // Listado ordenado (Max Heap)
                case "8": Top5PorCategoria(gestor); break;               // Top 5 (Min Heap acotado)
                case "9": DemoMonticulo(gestor); break;                  // Demo: insertar/buscar/eliminar/imprimir/recorrer
                case "0": salir = true; break;                          // Salir del programa
                default: Console.WriteLine("Opcion invalida."); break;  // Opcion no reconocida
            }
        }
    }

    // Imprime las opciones del menu principal con un esquema de colores "verde cancha" (verde + blanco)
    static void MostrarMenu()
    {
        ConsoleColor colorOriginal = Console.ForegroundColor;      // Guardamos el color original de la consola

        Console.ForegroundColor = ConsoleColor.Green;               // Titulo en verde
        Console.WriteLine();
        Console.WriteLine("===================================");
        Console.WriteLine("    TABLA DE POSICIONES MUNDIAL");
        Console.WriteLine("===================================");
        Console.ForegroundColor = colorOriginal;                    // Volvemos al color original para el texto

        string[] opciones =                                          // Arreglo con las opciones del menu
        {
            "1. Registrar jugador manualmente",
            "2. Cargar jugadores desde archivo CSV",
            "3. Buscar jugador por Id",
            "4. Actualizar estadisticas de un jugador",
            "5. Eliminar jugador",
            "6. Mostrar listado general (por Id)",
            "7. Mostrar listado ordenado por categoria",
            "8. Mostrar Top 5 por categoria",
            "9. Demostracion de Monticulo (buscar/eliminar/imprimir)",
            "0. Salir"
        };

        foreach (string opcion in opciones)                          // Imprimimos cada opcion con el numero en verde
        {
            int puntoFinal = opcion.IndexOf('.');                    // Posicion del punto que separa numero y texto
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(opcion.Substring(0, puntoFinal + 1));       // Numero y punto en verde
            Console.ForegroundColor = colorOriginal;
            Console.WriteLine(opcion.Substring(puntoFinal + 1));      // Resto del texto en el color original (blanco)
        }

        Console.ForegroundColor = ConsoleColor.Green;                 // Prompt final tambien en verde
        Console.Write("\nSeleccione una opcion: ");
        Console.ForegroundColor = colorOriginal;                      // Restauramos el color antes de leer la entrada
    }

    // Pide los datos de un jugador por teclado y lo registra en el sistema
    static void RegistrarManual(GestorMundial gestor)
    {
        Console.Write("Id (numero): "); int id = int.Parse(Console.ReadLine());
        Console.Write("Nombre: "); string nombre = Console.ReadLine();
        Console.Write("Seleccion: "); string seleccion = Console.ReadLine();
        Console.Write("Posicion: "); string posicion = Console.ReadLine();
        Console.Write("Minutos jugados: "); int minutos = int.Parse(Console.ReadLine());
        Console.Write("Goles: "); int goles = int.Parse(Console.ReadLine());
        Console.Write("Asistencias: "); int asistencias = int.Parse(Console.ReadLine());
        Console.Write("Tarjetas: "); int tarjetas = int.Parse(Console.ReadLine());
        Console.Write("Partidos disputados: "); int partidos = int.Parse(Console.ReadLine());

        Jugador jugador = new Jugador(id, nombre, seleccion, posicion, minutos, goles, asistencias, tarjetas, partidos);
        gestor.RegistrarJugador(jugador);                             // Delegamos el registro al gestor
    }

    // Pide la ruta del archivo CSV y delega la carga al gestor
    static void CargarArchivo(GestorMundial gestor)
    {
        Console.Write("Ruta del archivo CSV: ");
        string ruta = Console.ReadLine();
        gestor.CargarDesdeCsv(ruta);                                   // Delegamos la carga al gestor
    }

    // Busca un jugador por Id y lo muestra en pantalla
    static void BuscarJugador(GestorMundial gestor)
    {
        Console.Write("Id a buscar: ");
        int id = int.Parse(Console.ReadLine());
        Jugador j = gestor.BuscarJugador(id);                          // Buscamos usando el Arbol B+

        if (j != null) Console.WriteLine("Encontrado: " + j);          // Se encontro el jugador
        else Console.WriteLine("No se encontro un jugador con ese Id.");
    }

    // Actualiza las estadisticas de un jugador tras un nuevo partido
    static void ActualizarEstadisticas(GestorMundial gestor)
    {
        Console.Write("Id del jugador: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Minutos jugados en el partido: "); int minutos = int.Parse(Console.ReadLine());
        Console.Write("Goles anotados: "); int goles = int.Parse(Console.ReadLine());
        Console.Write("Asistencias: "); int asistencias = int.Parse(Console.ReadLine());
        Console.Write("Tarjetas recibidas: "); int tarjetas = int.Parse(Console.ReadLine());

        bool ok = gestor.ActualizarEstadisticas(id, minutos, goles, asistencias, tarjetas);
        Console.WriteLine(ok ? "Estadisticas actualizadas." : "Jugador no encontrado.");
    }

    // Elimina un jugador del sistema por su Id
    static void EliminarJugador(GestorMundial gestor)
    {
        Console.Write("Id a eliminar: ");
        int id = int.Parse(Console.ReadLine());
        bool ok = gestor.EliminarJugador(id);
        Console.WriteLine(ok ? "Jugador eliminado." : "Jugador no encontrado.");
    }

    // Pide la categoria y muestra el listado completo ordenado (usa el Max Heap)
    static void ListadoPorCategoria(GestorMundial gestor)
    {
        string categoria = PedirCategoria();
        gestor.MostrarListadoOrdenado(categoria);
    }

    // Pide la categoria y muestra el Top 5 (usa el Min Heap acotado)
    static void Top5PorCategoria(GestorMundial gestor)
    {
        string categoria = PedirCategoria();
        gestor.MostrarTop5(categoria);
    }

    // Pide categoria e Id de prueba y ejecuta la demostracion completa sobre el monticulo
    static void DemoMonticulo(GestorMundial gestor)
    {
        string categoria = PedirCategoria();
        Console.Write("Id de un jugador para probar Buscar/Eliminar en el monticulo: ");
        int id = int.Parse(Console.ReadLine());
        gestor.DemoMonticulo(categoria, id);
    }

    // Solicita al usuario que elija una categoria valida
    static string PedirCategoria()
    {
        Console.WriteLine("Categorias: goles, asistencias, minutos, tarjetas, partidos");
        Console.Write("Categoria: ");
        return Console.ReadLine().Trim().ToLower();
    }
}