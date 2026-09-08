using System;                                                      // Necesario para tipos base y Console

// Monticulo (heap) Max implementado con arreglo propio 
// Se usa para: 1) obtener rapido al mejor jugador de una categoria (raiz = maximo)
//              2) generar el listado completo ordenado de mayor a menor (heap sort)
class MonticuloMax
{
    private Jugador[] datos;                                       // Arreglo que almacena el monticulo
    private int tamanioActual;                                     // Cantidad de elementos en el monticulo
    private int capacidad;                                         // Capacidad actual del arreglo
    private string categoria;                                      // Categoria numerica usada para comparar

    // Constructor: recibe la capacidad inicial y la categoria de comparacion
    public MonticuloMax(int capacidadInicial, string categoria)
    {
        capacidad = capacidadInicial < 1 ? 1 : capacidadInicial;    // Evitamos capacidad cero o negativa
        datos = new Jugador[capacidad];                             // Reservamos el arreglo
        tamanioActual = 0;                                          // Inicialmente vacio
        this.categoria = categoria;                                 // Guardamos la categoria de comparacion
    }

    public int Cantidad => tamanioActual;                           // Propiedad de solo lectura

    // Duplica la capacidad del arreglo cuando se llena (arreglo dinamico propio)
    private void Redimensionar()
    {
        capacidad *= 2;                                              // Duplicamos la capacidad
        Jugador[] nuevo = new Jugador[capacidad];                    // Creamos un arreglo mas grande
        for (int i = 0; i < tamanioActual; i++) nuevo[i] = datos[i]; // Copiamos los datos existentes
        datos = nuevo;                                                // Reemplazamos el arreglo original
    }

    private int Valor(int i) => datos[i].ObtenerValorCategoria(categoria); // Valor de comparacion del elemento i

    // INSERTAR: agrega un jugador y reordena hacia arriba (sift-up)
    public void Insertar(Jugador jugador)
    {
        if (tamanioActual == capacidad) Redimensionar();            // Aseguramos espacio disponible

        datos[tamanioActual] = jugador;                              // Colocamos el nuevo elemento al final
        int i = tamanioActual;                                       // Posicion del nuevo elemento
        tamanioActual++;                                              // Aumentamos el tamanio

        while (i > 0)                                                 // Mientras no sea la raiz
        {
            int padre = (i - 1) / 2;                                  // Indice del nodo padre
            if (Valor(i) > Valor(padre))                              // Si el hijo es mayor, se intercambian
            {
                Jugador temp = datos[i];
                datos[i] = datos[padre];
                datos[padre] = temp;
                i = padre;                                             // Seguimos subiendo
            }
            else break;                                                // Ya quedo en su lugar correcto
        }
    }

    // EXTRAER MAXIMO: quita y devuelve la raiz (el valor mas alto) y reordena hacia abajo (sift-down)
    public Jugador ExtraerMaximo()
    {
        if (tamanioActual == 0) return null;                         // Monticulo vacio, no hay nada que extraer

        Jugador max = datos[0];                                       // La raiz siempre es el maximo
        tamanioActual--;                                               // Reducimos el tamanio
        datos[0] = datos[tamanioActual];                               // Movemos el ultimo elemento al inicio

        int i = 0;
        while (true)
        {
            int izq = 2 * i + 1;                                       // Indice del hijo izquierdo
            int der = 2 * i + 2;                                       // Indice del hijo derecho
            int mayor = i;                                              // Suponemos que el actual es el mayor

            if (izq < tamanioActual && Valor(izq) > Valor(mayor)) mayor = izq;
            if (der < tamanioActual && Valor(der) > Valor(mayor)) mayor = der;

            if (mayor == i) break;                                     // Ya quedo ordenado, terminamos

            Jugador temp = datos[i];                                   // Intercambiamos con el hijo mayor
            datos[i] = datos[mayor];
            datos[mayor] = temp;
            i = mayor;                                                  // Continuamos bajando
        }

        return max;                                                     // Devolvemos el jugador extraido
    }

    // BUSCAR: recorre el arreglo interno buscando un jugador por su Id (no aprovecha el orden del heap,
    // porque un heap no esta ordenado linealmente, solo garantiza que el padre es mayor que sus hijos)
    public Jugador Buscar(int id)
    {
        for (int i = 0; i < tamanioActual; i++)                      // Recorrido secuencial del arreglo interno
        {
            if (datos[i].Id == id) return datos[i];                   // Encontrado
        }
        return null;                                                   // No existe ese Id en el monticulo
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
            int mayor = i2;

            if (izq < tamanioActual && Valor(izq) > Valor(mayor)) mayor = izq;
            if (der < tamanioActual && Valor(der) > Valor(mayor)) mayor = der;

            if (mayor == i2) break;

            Jugador temp = datos[i2];
            datos[i2] = datos[mayor];
            datos[mayor] = temp;
            i2 = mayor;
        }

        // Tambien reacomodamos hacia arriba, por si el elemento sustituto es mayor que su nuevo padre
        int i3 = pos;
        while (i3 > 0)
        {
            int padre = (i3 - 1) / 2;
            if (Valor(i3) > Valor(padre))
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

    // IMPRIMIR: muestra el arreglo interno tal como esta almacenado (orden de heap, NO orden descendente)
    // Sirve para poder explicar en la defensa como luce la estructura por dentro
    public void Imprimir()
    {
        Console.WriteLine($"\nMonticulo Max interno (categoria: {categoria}), {tamanioActual} elementos:");
        for (int i = 0; i < tamanioActual; i++)
        {
            Console.WriteLine($"  [{i}] {datos[i]}");                   // Mostramos la posicion real dentro del arreglo
        }
    }

    // MOSTRAR: imprime el listado completo ordenado de mayor a menor sin destruir el monticulo original
    public void MostrarOrdenadoDescendente()
    {
        MonticuloMax copia = new MonticuloMax(capacidad, categoria);    // Copia de trabajo (heap sort no destructivo)
        for (int i = 0; i < tamanioActual; i++) copia.Insertar(datos[i]);

        Console.WriteLine($"\nListado ordenado por {categoria} (mayor a menor):");
        while (copia.Cantidad > 0)                                       // Extraemos en orden descendente
        {
            Jugador j = copia.ExtraerMaximo();
            Console.WriteLine(j);
        }
    }
}
