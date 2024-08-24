using Proyecto_UTRON;

public class Grid
{
    public Nodo Inicio { get; private set; }
    public int Ancho { get; private set; }
    public int Alto { get; private set; }

    public Grid(int ancho, int alto)
    {
        Ancho = ancho;
        Alto = alto;
        CrearGrid();
    }

    private void CrearGrid()
    {
        Nodo[,] nodos = new Nodo[Ancho, Alto];

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
        // Asegúrate de que las coordenadas están dentro del rango válido
        if (x < 0 || x >= Ancho || y < 0 || y >= Alto)
        {
            return null; // O puedes lanzar una excepción si lo prefieres
        }

        return Inicio;
    }
}

