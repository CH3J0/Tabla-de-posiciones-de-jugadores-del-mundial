using System;                                                      // Necesario para tipos base
using System.IO;                                                   // Necesario para leer archivos 

// Clase que administra el catalogo de jugadores. Usa el Arbol B+ como estructura
// principal (indexado por Id) y genera reportes apoyandose en los monticulos Min y Max
class GestorMundial
{
    private ArbolBMas arbol;                                       // Estructura principal: Arbol B+ indexado por Id

    // Constructor: inicializa el Arbol B+ de orden 4 (maximo 3 claves por nodo)
    public GestorMundial()
    {
        arbol = new ArbolBMas(4);                               
    }

    // REGISTRAR: agrega un nuevo jugador al arbol 
    public void RegistrarJugador(Jugador jugador)
    {
        bool insertado = arbol.Insertar(jugador);                   // Insertamos en el Arbol B+ (clave = Id)

        if (insertado)
            Console.WriteLine($"Jugador {jugador.Nombre} registrado con Id {jugador.Id}.");
        else
            Console.WriteLine($"Ya existe un jugador con Id {jugador.Id}. No se registro.");
    }

    // BUSCAR: encuentra un jugador por su Id usando el Arbol B+
    public Jugador BuscarJugador(int id)
    {
        return arbol.Buscar(id);                                    // Busqueda O(log n) apoyada en el arbol
    }

    // ELIMINAR: quita un jugador del arbol por su Id
    public bool EliminarJugador(int id)
    {
        return arbol.Eliminar(id);                                  // Delegamos la eliminacion al arbol
    }

    // ACTUALIZAR ESTADISTICAS: busca al jugador y suma los datos del nuevo partido
    public bool ActualizarEstadisticas(int id, int minutos, int goles, int asistencias, int tarjetas)
    {
        Jugador jugador = arbol.Buscar(id);                         // Ubicamos al jugador por su Id
        if (jugador == null) return false;                          // No existe ese jugador

        jugador.MinutosJugados += minutos;                          // Sumamos minutos del nuevo partido
        jugador.Goles += goles;                                     // Sumamos goles
        jugador.Asistencias += asistencias;                         // Sumamos asistencias
        jugador.Tarjetas += tarjetas;                                // Sumamos tarjetas
        jugador.PartidosDisputados += 1;                            // Un partido mas disputado
        return true;                                                  // Actualizacion exitosa
    }

    // TOP 5 por categoria: recorre el Arbol B+ y usa un Monticulo Min acotado a 5 elementos
    public void MostrarTop5(string categoria)
    {
        Jugador[] todos = arbol.ObtenerTodosOrdenadosPorId();       // Obtenemos todos los jugadores del arbol
        MonticuloMin topK = new MonticuloMin(5, categoria);          // Monticulo Min acotado a 5

        for (int i = 0; i < todos.Length; i++)
            topK.Insertar(todos[i]);                                 // Cada insercion decide si entra al Top 5

        topK.MostrarTop();                                            // Imprimimos el resultado final
    }

    // LISTADO GENERAL por categoria: usa un Monticulo Max para ordenar de mayor a menor
    public void MostrarListadoOrdenado(string categoria)
    {
        Jugador[] todos = arbol.ObtenerTodosOrdenadosPorId();        // Obtenemos todos los jugadores del arbol
        MonticuloMax monticulo = new MonticuloMax(todos.Length, categoria); // Capacidad ajustada al total actual

        for (int i = 0; i < todos.Length; i++)
            monticulo.Insertar(todos[i]);                             // Insertamos a todos en el monticulo

        monticulo.MostrarOrdenadoDescendente();                       // Imprimimos ordenado de mayor a menor
    }

    public void DemoMonticulo(string categoria, int idParaProbar)
    {
        Jugador[] todos = arbol.ObtenerTodosOrdenadosPorId();          // Tomamos el catalogo actual
        MonticuloMax monticulo = new MonticuloMax(todos.Length, categoria);

        for (int i = 0; i < todos.Length; i++)
            monticulo.Insertar(todos[i]);                               // INSERTAR: uno por uno

        monticulo.Imprimir();                                            // IMPRIMIR: arreglo interno tal cual quedo

        Jugador encontrado = monticulo.Buscar(idParaProbar);             // BUSCAR: por Id dentro del monticulo
        Console.WriteLine(encontrado != null
            ? $"\nBuscar Id {idParaProbar} en el monticulo: encontrado -> {encontrado}"
            : $"\nBuscar Id {idParaProbar} en el monticulo: no se encontro.");

        if (encontrado != null)
        {
            monticulo.Eliminar(idParaProbar);                            // ELIMINAR: se quita del monticulo
            Console.WriteLine($"Jugador con Id {idParaProbar} eliminado del monticulo (no del catalogo).");
            monticulo.Imprimir();                                         // Mostramos como quedo tras eliminar
        }

        monticulo.MostrarOrdenadoDescendente();                          // RECORRER: listado ordenado final
    }

    // LISTADO GENERAL por Id: recorrido secuencial de las hojas enlazadas del Arbol B+
    public void MostrarTodosPorId()
    {
        Jugador[] todos = arbol.ObtenerTodosOrdenadosPorId();        // Recorrido natural del Arbol B+
        Console.WriteLine("\nListado general (ordenado por Id):");
        for (int i = 0; i < todos.Length; i++)
            Console.WriteLine(todos[i]);                              // Imprimimos cada jugador
    }

    // CARGAR DESDE CSV: formato -> Id,Nombre,Seleccion,Posicion,Minutos,Goles,Asistencias,Tarjetas,Partidos
    public void CargarDesdeCsv(string ruta)
    {
        if (!File.Exists(ruta))                                       // Verificamos que el archivo exista
        {
            Console.WriteLine("El archivo no existe: " + ruta);
            return;
        }

        string[] lineas = File.ReadAllLines(ruta);                    // Lectura de archivo (soporte auxiliar permitido)
        int cargados = 0;                                              // Contador de jugadores cargados correctamente
        int rechazados = 0;                                            // Contador de lineas con errores o duplicados

        for (int i = 0; i < lineas.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lineas[i])) continue;        // Saltamos lineas vacias
            if (i == 0 && lineas[i].ToLower().StartsWith("id")) continue; // Saltamos encabezado si existe

            string[] campos = lineas[i].Split(',');                    // Separamos la linea por comas
            if (campos.Length < 9)                                      // Linea incompleta
            {
                Console.WriteLine($"Linea {i + 1} incompleta, se omite: \"{lineas[i]}\"");
                rechazados++;
                continue;
            }

            try                                                          // Protegemos el parseo de cada campo numerico
            {
                Jugador j = new Jugador(
                    int.Parse(campos[0].Trim()),                          // Id
                    campos[1].Trim(),                                     // Nombre
                    campos[2].Trim(),                                     // Seleccion
                    campos[3].Trim(),                                     // Posicion
                    int.Parse(campos[4].Trim()),                          // Minutos
                    int.Parse(campos[5].Trim()),                          // Goles
                    int.Parse(campos[6].Trim()),                          // Asistencias
                    int.Parse(campos[7].Trim()),                          // Tarjetas
                    int.Parse(campos[8].Trim())                           // Partidos disputados
                );

                if (arbol.Insertar(j))                                    // Insertamos, verificando duplicados
                {
                    cargados++;                                           // Aumentamos el contador de exito
                }
                else
                {
                    Console.WriteLine($"Linea {i + 1}: Id {j.Id} duplicado, se omite.");
                    rechazados++;
                }
            }
            catch (FormatException)                                       // Algun campo numerico venia mal escrito
            {
                Console.WriteLine($"Linea {i + 1} con formato invalido, se omite: \"{lineas[i]}\"");
                rechazados++;
            }
        }

        Console.WriteLine($"Se cargaron {cargados} jugadores desde {ruta}. ({rechazados} lineas omitidas)");
    }
}
