using Proyecto_UTRON;
using System;

public class Grid
{
    public Nodo Inicio { get; private set; }
    public int Ancho { get; private set; }
    public int Alto { get; private set; }
    private Nodo[,] nodos;
    private Random random = new Random();

    public Grid(int ancho, int alto)
    {
        Ancho = ancho;
        Alto = alto;
        CrearGrid();
    }

    private void CrearGrid()
    {
        nodos = new Nodo[Ancho, Alto];

        for (int x = 0; x < Ancho; x++)
        {
            for (int y = 0; y < Alto; y++)
            {
                nodos[x, y] = new Nodo(x, y);
            }
        }

        for (int x = 0; x < Ancho; x++)
        {
            for (int y = 0; y < Alto; y++)
            {
                Nodo nodo = nodos[x, y];
                nodo.Arriba = y > 0 ? nodos[x, y - 1] : null;
                nodo.Abajo = y < Alto - 1 ? nodos[x, y + 1] : null;
                nodo.Izquierda = x > 0 ? nodos[x - 1, y] : null;
                nodo.Derecha = x < Ancho - 1 ? nodos[x + 1, y] : null;
            }
        }

        Inicio = nodos[0, 0]; // O el nodo inicial que prefieras
    }

    public Nodo ObtenerNodoEnPos(int x, int y)
    {
        if (x < 0 || x >= Ancho || y < 0 || y >= Alto)
        {
            return null;
        }

        return nodos[x, y];
    }

    public Nodo ObtenerNodoAleatorio()
    {
        int x = random.Next(0, Ancho);
        int y = random.Next(0, Alto);
        return nodos[x, y];
    }

    // Método para obtener una posición aleatoria que no esté ocupada
    public Nodo ObtenerPosicionAleatoria()
    {
        int x = random.Next(0, Ancho); // Ancho del grid
        int y = random.Next(0, Alto);  // Alto del grid
        Nodo nodo = ObtenerNodoEnPos(x, y);

        // Asegurarse de que el nodo no esté ocupado
        // Asegurarse de que el nodo no esté ocupado
        while (nodo.Ocupado)
        {
            x = random.Next(0, Ancho);
            y = random.Next(0, Alto);
            nodo = ObtenerNodoEnPos(x, y);
        }

        return nodo;
    }
}
