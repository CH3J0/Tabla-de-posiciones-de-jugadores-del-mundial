using System;                                                      // Necesario para tipos base

// Nodo del Arbol B+. Puede ser interno (guia la busqueda) u hoja (guarda los datos reales)
class NodoBMas
{
    public int[] Claves;                                           // Claves almacenadas en el nodo (los Id)
    public int NumClaves;                                          // Cantidad de claves actualmente en uso
    public bool EsHoja;                                            // Indica si el nodo es hoja o interno
    public NodoBMas[] Hijos;                                       // Punteros a hijos (solo se usan en nodos internos)
    public Jugador[] Datos;                                        // Jugadores asociados a cada clave (solo en hojas)
    public NodoBMas Siguiente;                                     // Enlace a la siguiente hoja (recorrido secuencial)

    // Constructor: orden define cuantas claves como maximo puede tener un nodo antes de dividirse
    public NodoBMas(int orden, bool esHoja)
    {
        Claves = new int[orden];                                   // Reservamos espacio (con un extra temporal para el split)
        Hijos = new NodoBMas[orden + 1];                            // Un hijo mas que claves, para nodos internos
        Datos = new Jugador[orden];                                 // Solo se usa si el nodo es hoja
        NumClaves = 0;                                              // Al crear el nodo no tiene claves
        EsHoja = esHoja;                                            // Guardamos el tipo de nodo
        Siguiente = null;                                           // Aun no esta enlazado a otra hoja
    }
}
