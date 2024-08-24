using Proyecto_UTRON;
using System;

public class Moto
{
    public Nodo PosicionActual { get; private set; }
    private Grid grid;
    private Random random = new Random();

    public int Velocidad { get; private set; }
    public int TamanoEstela { get; private set; }
    public int Combustible { get; private set; }
    private int celdasRecorridas;
    private Direccion direccion;

    public Moto(Nodo posicionInicial, Grid grid)
    {
        PosicionActual = posicionInicial;
        this.grid = grid;
        Velocidad = random.Next(1, 11); // Velocidad entre 1 y 10
        TamanoEstela = 3; // Tamaño inicial de la estela
        Combustible = 100; // Valor inicial del combustible
        celdasRecorridas = 0;
        PosicionActual.Ocupado = true;
    }

    public void Moverse(Direccion direccion)
    {
        if (Combustible <= 0)
        {
            // Si no hay combustible, no mover la moto
            return;
        }

        this.direccion = direccion;
        Nodo siguientePosicion = ObtenerNodoSiguiente(direccion);

        if (siguientePosicion != null && !siguientePosicion.Ocupado)
        {
            PosicionActual.Ocupado = false;
            PosicionActual = siguientePosicion;
            PosicionActual.Ocupado = true;

            celdasRecorridas++;
            VerificarCombustible();
        }
    }

    private Nodo ObtenerNodoSiguiente(Direccion direccion)
    {
        Nodo siguienteNodo = null;

        switch (direccion)
        {
            case Direccion.Arriba:
                siguienteNodo = PosicionActual.Arriba;
                if (siguienteNodo == null)
                {
                    // Si llega al borde superior, ir al borde inferior
                    siguienteNodo = grid.ObtenerNodoEnPos(PosicionActual.PosX, grid.Alto - 1);
                }
                break;
            case Direccion.Abajo:
                siguienteNodo = PosicionActual.Abajo;
                if (siguienteNodo == null)
                {
                    // Si llega al borde inferior, ir al borde superior
                    siguienteNodo = grid.ObtenerNodoEnPos(PosicionActual.PosX, 0);
                }
                break;
            case Direccion.Izquierda:
                siguienteNodo = PosicionActual.Izquierda;
                if (siguienteNodo == null)
                {
                    // Si llega al borde izquierdo, ir al borde derecho
                    siguienteNodo = grid.ObtenerNodoEnPos(grid.Ancho - 1, PosicionActual.PosY);
                }
                break;
            case Direccion.Derecha:
                siguienteNodo = PosicionActual.Derecha;
                if (siguienteNodo == null)
                {
                    // Si llega al borde derecho, ir al borde izquierdo
                    siguienteNodo = grid.ObtenerNodoEnPos(0, PosicionActual.PosY);
                }
                break;
        }

        return siguienteNodo;
    }

    private void VerificarCombustible()
    {
        // Se consume 1 celda de combustible por cada 5 celdas recorridas
        if (celdasRecorridas >= 5)
        {
            Combustible -= 1;
            celdasRecorridas = 0; // Resetear el contador de celdas
        }

        // Asegurar que el combustible no sea menor a 0
        if (Combustible < 0)
        {
            Combustible = 0;
        }
    }
}
