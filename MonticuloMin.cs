using System;                                                      // Necesario para tipos base y Console

// Monticulo (heap) Min implementado con arreglo propio, usado como "monticulo acotado"
// para obtener el Top-K (por ejemplo Top 5) de una categoria sin ordenar a todos los
// jugadores: solo se conservan los K mejores, y la raiz es siempre el peor de esos K
// (el primero en salir si llega alguien mejor). Costo: O(n log k) en vez de O(n log n)
class MonticuloMin
{
    private Jugador[] datos;                                       // Arreglo que almacena el monticulo
    private int tamanioActual;                                     // Cantidad de elementos en el monticulo
    private int capacidadMaxima;                                   // Tope K de elementos conservados 
    private string categoria;                                      // Categoria numerica usada para comparar

    // Constructor: k = cuantos elementos como maximo se conservan (Top-K)
    public MonticuloMin(int k, string categoria)
    {
        capacidadMaxima = k;                                        // Guardamos el tope 
        datos = new Jugador[k];                                      // Arreglo de tamanio fijo k
        tamanioActual = 0;                                           // Inicialmente vacio
        this.categoria = categoria;                                  // Guardamos la categoria de comparacion
    }

    public int Cantidad => tamanioActual;                            // Propiedad de solo lectura

    private int Valor(int i) => datos[i].ObtenerValorCategoria(categoria); // Valor numerico del elemento i

    // INSERTAR (con tope): si hay espacio libre se agrega normal; si esta lleno,
    // solo se agrega si el nuevo jugador supera al peor del Top-K actual (la raiz)
    public void Insertar(Jugador jugador)
    {
        if (tamanioActual < capacidadMaxima)                          // Todavia hay espacio libre en el Top-K
        {
            datos[tamanioActual] = jugador;                            // Colocamos al final
            int i = tamanioActual;
            tamanioActual++;

            while (i > 0)                                              // Sift-up: reordenamos hacia arriba
            {
                int padre = (i - 1) / 2;
                if (Valor(i) < Valor(padre))                           // Si el hijo es menor, se intercambian
                {
                    Jugador temp = datos[i];
                    datos[i] = datos[padre];
                    datos[padre] = temp;
                    i = padre;
                }
                else break;
            }
        }
        else if (jugador.ObtenerValorCategoria(categoria) > Valor(0))  // El Top-K esta lleno, pero este es mejor
        {
            datos[0] = jugador;                                        // Reemplazamos al peor del Top-K (la raiz)

            int i = 0;
            while (true)                                                // Sift-down: reordenamos hacia abajo
            {
                int izq = 2 * i + 1;
                int der = 2 * i + 2;
                int menor = i;

                if (izq < tamanioActual && Valor(izq) < Valor(menor)) menor = izq;
                if (der < tamanioActual && Valor(der) < Valor(menor)) menor = der;

                if (menor == i) break;                                  // Ya quedo ordenado

                Jugador temp = datos[i];
                datos[i] = datos[menor];
                datos[menor] = temp;
                i = menor;
            }
        }
        // Si esta lleno y el nuevo jugador no supera al peor del Top-K, simplemente se descarta
    }

    // BUSCAR: recorre el arreglo interno buscando un jugador por su Id
    public Jugador Buscar(int id)
    {
        for (int i = 0; i < tamanioActual; i++)                      // Recorrido secuencial del arreglo interno
        {
            if (datos[i].Id == id) return datos[i];                   // Encontrado
        }
        return null;                                                   // No existe ese Id en el Top-K actual
    }

    // ELIMINAR: quita un jugador especifico del monticulo (no solo la raiz) y reordena
    public bool Eliminar(int id)
    {
        int pos = -1;
        for (int i = 0; i < tamanioActual; i++)                       // Ubicamos la posicion del jugador a eliminar
        {
            if (datos[i].Id == id) { pos = i; break; }
        }
        if (pos == -1) return false;                                   // No se encontro el jugador

        tamanioActual--;                                                // Reducimos el tamanio
        datos[pos] = datos[tamanioActual];                              // Sustituimos con el ultimo elemento

        // Reacomodamos hacia abajo desde la posicion afectada (sift-down)
        int i2 = pos;
        while (true)
        {
            int izq = 2 * i2 + 1;
            int der = 2 * i2 + 2;
            int menor = i2;

            if (izq < tamanioActual && Valor(izq) < Valor(menor)) menor = izq;
            if (der < tamanioActual && Valor(der) < Valor(menor)) menor = der;

            if (menor == i2) break;

            Jugador temp = datos[i2];
            datos[i2] = datos[menor];
            datos[menor] = temp;
            i2 = menor;
        }

        // Tambien reacomodamos hacia arriba, por si el elemento sustituto es menor que su nuevo padre
        int i3 = pos;
        while (i3 > 0)
        {
            int padre = (i3 - 1) / 2;
            if (Valor(i3) < Valor(padre))
            {
                Jugador temp = datos[i3];
                datos[i3] = datos[padre];
                datos[padre] = temp;
                i3 = padre;
            }
            else break;
        }

        return true;                                                    // Eliminacion exitosa
    }

    // IMPRIMIR: muestra el arreglo interno tal como esta almacenado (orden de heap, NO orden ascendente)
    public void Imprimir()
    {
        Console.WriteLine($"\nMonticulo Min interno (categoria: {categoria}), {tamanioActual} elementos:");
        for (int i = 0; i < tamanioActual; i++)
        {
            Console.WriteLine($"  [{i}] {datos[i]}");                   // Mostramos la posicion real dentro del arreglo
        }
    }

    // MOSTRAR: imprime el Top-K de mayor a menor (se apoya en un Monticulo Max para ordenar la salida)
    public void MostrarTop()
    {
        MonticuloMax ordenador = new MonticuloMax(capacidadMaxima, categoria); // Reutilizamos el Max Heap
        for (int i = 0; i < tamanioActual; i++) ordenador.Insertar(datos[i]);

        Console.WriteLine($"\nTop {capacidadMaxima} jugadores por {categoria}:");
        int puesto = 1;
        while (ordenador.Cantidad > 0)                                 // Extraemos del mejor al peor
        {
            Jugador j = ordenador.ExtraerMaximo();
            Console.WriteLine($"{puesto}. {j}");
            puesto++;
        }
    }
}
