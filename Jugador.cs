using System;                                                      // Necesario para tipos base

// Clase que representa un jugador del mundial con sus datos y estadisticas
class Jugador
{
    public int Id;                                                 // Codigo unico del jugador, es la clave del Arbol B+
    public string Nombre;                                          // Nombre completo del jugador
    public string Seleccion;                                       // Seleccion (pais) a la que pertenece
    public string Posicion;                                        // Posicion en la que juega (DEL, MED, DEF, POR)
    public int MinutosJugados;                                     // Minutos acumulados en el torneo
    public int Goles;                                              // Goles acumulados
    public int Asistencias;                                        // Asistencias acumuladas
    public int Tarjetas;                                           // Tarjetas recibidas (amarillas+rojas)
    public int PartidosDisputados;                                 // Cantidad de partidos jugados

    // Constructor que recibe todos los datos del jugador
    public Jugador(int id, string nombre, string seleccion, string posicion,
                    int minutos, int goles, int asistencias, int tarjetas, int partidos)
    {
        Id = id;                                                   // Asignamos el codigo unico
        Nombre = nombre;                                           // Asignamos el nombre
        Seleccion = seleccion;                                     // Asignamos la seleccion
        Posicion = posicion;                                       // Asignamos la posicion
        MinutosJugados = minutos;                                  // Asignamos minutos jugados
        Goles = goles;                                             // Asignamos goles
        Asistencias = asistencias;                                 // Asignamos asistencias
        Tarjetas = tarjetas;                                       // Asignamos tarjetas
        PartidosDisputados = partidos;                             // Asignamos partidos disputados
    }

    // Devuelve el valor numerico de la categoria solicitada (usado por los monticulos)
    public int ObtenerValorCategoria(string categoria)
    {
        switch (categoria)                                         // Evaluamos que categoria se pidio
        {
            case "goles": return Goles;                            // Categoria goles
            case "asistencias": return Asistencias;                // Categoria asistencias
            case "minutos": return MinutosJugados;                 // Categoria minutos jugados
            case "tarjetas": return Tarjetas;                      // Categoria tarjetas
            case "partidos": return PartidosDisputados;            // Categoria partidos disputados
            default: return 0;                                     // Categoria no reconocida
        }
    }

    // Representacion en texto de un jugador, usada para imprimir en consola
    public override string ToString()
    {
        return $"ID:{Id,-5} {Nombre,-20} {Seleccion,-15} {Posicion,-5} " +           // Id, nombre, seleccion, posicion
               $"Min:{MinutosJugados,-5} Gol:{Goles,-3} Asis:{Asistencias,-3} " +     // Minutos, goles, asistencias
               $"Tarj:{Tarjetas,-3} PJ:{PartidosDisputados,-3}";                      // Tarjetas y partidos jugados
    }
}
