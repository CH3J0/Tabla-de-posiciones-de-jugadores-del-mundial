using System;                                                      // Necesario para tipos base

// Implementacion propia de un Arbol B+ (no se usan colecciones nativas de .NET)
// Es la estructura principal del sistema: indexa a los jugadores por su Id
// Las hojas guardan los datos reales y estan enlazadas entre si, lo que permite
// recorrer todo el catalogo en orden con un solo recorrido secuencial (sin recursion)
class ArbolBMas
{
    private NodoBMas raiz;                                         // Nodo raiz del arbol
    private int orden;                                             // Orden del arbol (maximo de hijos por nodo interno)
    private int cantidad;                                          // Cantidad de jugadores almacenados

    // Constructor: se define el orden del arbol (orden 4 -> maximo 3 claves por nodo)
    public ArbolBMas(int orden)
    {
        this.orden = orden;                                        // Guardamos el orden configurado
        raiz = new NodoBMas(orden, true);                          // La raiz inicia como una hoja vacia
        cantidad = 0;                                               // Aun no hay jugadores insertados
    }

    public int Cantidad => cantidad;                                // Propiedad de solo lectura para el total

    // INSERTAR: agrega un jugador usando su Id como clave de indexacion
    // Devuelve false si ya existia un jugador con ese Id (no se permite duplicar la clave)
    public bool Insertar(Jugador jugador)
    {
        if (Buscar(jugador.Id) != null) return false;               // Rechazamos Id duplicado antes de insertar

        int claveSubida;                                            // Clave que sube al padre si el nodo se divide
        NodoBMas nodoDividido;                                      // Nueva mitad derecha si hay division

        InsertarRecursivo(raiz, jugador, out claveSubida, out nodoDividido);  // Insertamos desde la raiz

        if (nodoDividido != null)                                  // Si la raiz se dividio, el arbol crece en altura
        {
            NodoBMas nuevaRaiz = new NodoBMas(orden, false);       // Creamos una nueva raiz interna
            nuevaRaiz.Claves[0] = claveSubida;                     // Guardamos la clave separadora
            nuevaRaiz.Hijos[0] = raiz;                             // La mitad izquierda es la raiz anterior
            nuevaRaiz.Hijos[1] = nodoDividido;                     // La mitad derecha es el nodo nuevo
            nuevaRaiz.NumClaves = 1;                                // La nueva raiz solo tiene una clave
            raiz = nuevaRaiz;                                       // Actualizamos la raiz del arbol
        }

        cantidad++;                                                 // Aumentamos el contador de jugadores
        return true;                                                 // Insercion exitosa
    }

    // Insercion recursiva: baja hasta la hoja correcta y propaga la division hacia arriba si es necesario
    private void InsertarRecursivo(NodoBMas nodo, Jugador jugador, out int claveSubida, out NodoBMas nodoDividido)
    {
        claveSubida = -1;                                           // Por defecto no hay division
        nodoDividido = null;                                        // Por defecto no hay nodo nuevo

        if (nodo.EsHoja)                                            // Caso base: llegamos a una hoja
        {
            int pos = 0;                                             // Buscamos la posicion de insercion ordenada
            while (pos < nodo.NumClaves && nodo.Claves[pos] < jugador.Id) pos++;

            for (int i = nodo.NumClaves; i > pos; i--)               // Desplazamos elementos para abrir espacio
            {
                nodo.Claves[i] = nodo.Claves[i - 1];
                nodo.Datos[i] = nodo.Datos[i - 1];
            }

            nodo.Claves[pos] = jugador.Id;                           // Insertamos la clave (Id)
            nodo.Datos[pos] = jugador;                               // Insertamos el jugador
            nodo.NumClaves++;                                        // Aumentamos el contador local

            if (nodo.NumClaves == orden)                             // La hoja quedo llena, hay que dividirla
            {
                DividirHoja(nodo, out claveSubida, out nodoDividido);
            }
        }
        else                                                         // Caso recursivo: nodo interno
        {
            int pos = 0;                                             // Buscamos por que hijo debemos bajar
            while (pos < nodo.NumClaves && jugador.Id >= nodo.Claves[pos]) pos++;

            int claveSubidaHijo;                                     // Clave que pudiera subir desde el hijo
            NodoBMas nodoDivididoHijo;                                // Nodo nuevo que pudiera venir del hijo
            InsertarRecursivo(nodo.Hijos[pos], jugador, out claveSubidaHijo, out nodoDivididoHijo);

            if (nodoDivididoHijo != null)                            // El hijo se dividio, hay que acomodarlo aqui
            {
                for (int i = nodo.NumClaves; i > pos; i--)            // Desplazamos claves e hijos
                {
                    nodo.Claves[i] = nodo.Claves[i - 1];
                    nodo.Hijos[i + 1] = nodo.Hijos[i];
                }

                nodo.Claves[pos] = claveSubidaHijo;                  // Insertamos la clave separadora
                nodo.Hijos[pos + 1] = nodoDivididoHijo;              // Insertamos el nuevo hijo
                nodo.NumClaves++;                                     // Aumentamos el contador local

                if (nodo.NumClaves == orden)                          // Este nodo tambien quedo lleno
                {
                    DividirInterno(nodo, out claveSubida, out nodoDividido);
                }
            }
        }
    }

    // Divide una hoja llena en dos mitades y las mantiene enlazadas entre si
    private void DividirHoja(NodoBMas hoja, out int claveSubida, out NodoBMas nuevaHoja)
    {
        int mitad = hoja.NumClaves / 2;                              // Punto medio de division
        nuevaHoja = new NodoBMas(orden, true);                      // La mitad derecha es una hoja nueva

        int contador = 0;
        for (int i = mitad; i < hoja.NumClaves; i++)                 // Copiamos la mitad derecha al nodo nuevo
        {
            nuevaHoja.Claves[contador] = hoja.Claves[i];
            nuevaHoja.Datos[contador] = hoja.Datos[i];
            contador++;
        }

        nuevaHoja.NumClaves = contador;                              // Cantidad de claves en la nueva hoja
        hoja.NumClaves = mitad;                                      // La hoja original se queda con la mitad izquierda

        nuevaHoja.Siguiente = hoja.Siguiente;                        // La hoja nueva enlaza a lo que seguia
        hoja.Siguiente = nuevaHoja;                                   // La hoja original enlaza a la nueva

        claveSubida = nuevaHoja.Claves[0];                            // En B+, la primera clave de la nueva hoja sube (se duplica)
    }

    // Divide un nodo interno lleno en dos mitades; la clave media sube y NO se duplica
    private void DividirInterno(NodoBMas nodo, out int claveSubida, out NodoBMas nuevoNodo)
    {
        int mitad = nodo.NumClaves / 2;                               // Punto medio de division
        claveSubida = nodo.Claves[mitad];                             // Esta clave sube al padre

        nuevoNodo = new NodoBMas(orden, false);                      // La mitad derecha es un nodo interno nuevo
        int contador = 0;
        for (int i = mitad + 1; i < nodo.NumClaves; i++)              // Copiamos claves posteriores a la media
        {
            nuevoNodo.Claves[contador] = nodo.Claves[i];
            contador++;
        }

        contador = 0;
        for (int i = mitad + 1; i <= nodo.NumClaves; i++)             // Copiamos los hijos correspondientes
        {
            nuevoNodo.Hijos[contador] = nodo.Hijos[i];
            contador++;
        }

        nuevoNodo.NumClaves = nodo.NumClaves - mitad - 1;             // Claves que quedaron en el nodo nuevo
        nodo.NumClaves = mitad;                                        // El nodo original se queda con la mitad izquierda
    }

    // BUSCAR: encuentra un jugador por su Id, bajando por los nodos guia hasta la hoja
    public Jugador Buscar(int id)
    {
        NodoBMas actual = raiz;                                       // Comenzamos desde la raiz

        while (!actual.EsHoja)                                        // Bajamos hasta llegar a una hoja
        {
            int pos = 0;
            while (pos < actual.NumClaves && id >= actual.Claves[pos]) pos++;
            actual = actual.Hijos[pos];                                // Avanzamos al hijo correspondiente
        }

        for (int i = 0; i < actual.NumClaves; i++)                    // Buscamos la clave dentro de la hoja
        {
            if (actual.Claves[i] == id) return actual.Datos[i];       // Encontrado
        }

        return null;                                                   // No existe un jugador con ese Id
    }

    // ELIMINAR: quita un jugador de la hoja correspondiente segun su Id
    // (simplificado: no fusiona nodos con pocas claves, suficiente para el alcance del proyecto)
    public bool Eliminar(int id)
    {
        NodoBMas actual = raiz;                                       // Comenzamos desde la raiz

        while (!actual.EsHoja)                                        // Bajamos hasta la hoja correspondiente
        {
            int pos = 0;
            while (pos < actual.NumClaves && id >= actual.Claves[pos]) pos++;
            actual = actual.Hijos[pos];
        }

        for (int i = 0; i < actual.NumClaves; i++)                    // Buscamos la clave dentro de la hoja
        {
            if (actual.Claves[i] == id)                                // La encontramos
            {
                for (int j = i; j < actual.NumClaves - 1; j++)         // Desplazamos para cerrar el espacio
                {
                    actual.Claves[j] = actual.Claves[j + 1];
                    actual.Datos[j] = actual.Datos[j + 1];
                }
                actual.NumClaves--;                                    // Reducimos el contador de la hoja
                cantidad--;                                             // Reducimos el contador global
                return true;                                            // Eliminacion exitosa
            }
        }

        return false;                                                   // No se encontro el Id a eliminar
    }

    // RECORRER: devuelve todos los jugadores ordenados por Id, aprovechando el enlace entre hojas
    public Jugador[] ObtenerTodosOrdenadosPorId()
    {
        Jugador[] resultado = new Jugador[cantidad];                  // Arreglo del tamanio exacto necesario

        NodoBMas actual = raiz;
        while (!actual.EsHoja) actual = actual.Hijos[0];              // Bajamos hasta la hoja mas a la izquierda

        int idx = 0;
        while (actual != null)                                        // Recorremos las hojas enlazadas de izq a der
        {
            for (int i = 0; i < actual.NumClaves; i++)
            {
                resultado[idx] = actual.Datos[i];
                idx++;
            }
            actual = actual.Siguiente;                                  // Avanzamos a la siguiente hoja
        }

        return resultado;
    }
}