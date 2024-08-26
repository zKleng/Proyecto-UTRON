using Proyecto_UTRON;
using System;

public class Grid
{
    public Nodo Inicio { get; private set; }
    public int Ancho { get; private set; }
    public int Alto { get; private set; }
    private Nodo[,] nodos;

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
        Random rand = new Random();
        int x = rand.Next(0, Ancho);
        int y = rand.Next(0, Alto);
        return nodos[x, y];
    }
}