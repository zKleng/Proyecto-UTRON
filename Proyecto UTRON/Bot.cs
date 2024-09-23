using Proyecto_UTRON;
using System.Collections.Generic;
using System;
using System.Linq;

public class Bot
{
    public Nodo PosicionActual { get; private set; }
    private int combustible; // Combustible del bot
    private int velocidad; // Velocidad del bot
    private List<Nodo> estela; // Estela del bot
    private Random random = new Random();
    private string direccionActual; // Dirección en la que se mueve el bot

    public Bot(Grid grid, int combustibleInicial)
    {
        PosicionActual = grid.ObtenerPosicionAleatoria();
        combustible = combustibleInicial;
        velocidad = random.Next(1, 11);
        estela = new List<Nodo>();
        direccionActual = ElegirDireccionAleatoria();
    }

    // Mover al bot de forma lineal
    public void Mover(Grid grid)
    {
        if (combustible > 0)
        {
            // Marcar la posición actual como parte de la estela
            estela.Add(PosicionActual);
            PosicionActual.Ocupado = true;

            // Mover en la dirección actual
            Nodo siguienteNodo = ObtenerSiguienteNodo(PosicionActual, direccionActual, grid);

            if (siguienteNodo == null || siguienteNodo.Ocupado)
            {
                // Cambiar de dirección si no puede moverse en la actual
                direccionActual = ElegirDireccionAleatoria();
                siguienteNodo = ObtenerSiguienteNodo(PosicionActual, direccionActual, grid);
            }

            // Actualiza la posición si encuentra un nodo válido
            if (siguienteNodo != null && !siguienteNodo.Ocupado)
            {
                PosicionActual.Ocupado = false;
                PosicionActual = siguienteNodo;
                combustible--;
            }
        }
    }

    // Propiedad para obtener el inicio de la estela (el primer nodo)
    public Nodo EstelaInicio
    {
        get
        {
            if (estela.Count > 0)
            {
                return estela.First(); // Retorna el primer nodo de la estela
            }
            return null; // Si no hay nodos en la estela, retorna null
        }
    }

    // Elegir una dirección aleatoria
    private string ElegirDireccionAleatoria()
    {
        string[] direcciones = { "Arriba", "Abajo", "Izquierda", "Derecha" };
        return direcciones[random.Next(direcciones.Length)];
    }

    // Obtener el siguiente nodo en la dirección elegida
    private Nodo ObtenerSiguienteNodo(Nodo actual, string direccion, Grid grid)
    {
        switch (direccion)
        {
            case "Arriba":
                return actual.Arriba;
            case "Abajo":
                return actual.Abajo;
            case "Izquierda":
                return actual.Izquierda;
            case "Derecha":
                return actual.Derecha;
            default:
                return null;
        }
    }

    // Verificar si el bot ha colisionado o se quedó sin combustible
    public bool VerificarColisionOCombustible(Grid grid, List<Bot> bots, Nodo posicionJugador, List<Nodo> estelaJugador)
    {
        // Verificar si el bot colisiona con la estela del jugador
        if (VerificarColisionConEstela(estelaJugador))
        {
            // El bot toca la estela del jugador; eliminar el bot pero no activar game over
            return false;
        }

        // Verificar colisión con la posición actual del jugador
        if (PosicionActual == posicionJugador)
        {
            // Colisión detectada con la posición actual del jugador
            return true; // Activar game over
        }

        // Verificar si el bot colisiona con las paredes o con otro bot
        if (PosicionActual.Ocupado || combustible <= 0)
        {
            // El bot desaparece si colisiona con una pared o con otro bot
            return false; // No activar game over, solo eliminar el bot
        }

        return false; // No hay colisión y el bot tiene combustible
    }

    // Método para verificar la colisión con la estela del jugador
    public bool VerificarColisionConEstela(List<Nodo> estelaJugador)
    {
        // Verificar si el bot colisiona con la estela del jugador
        return estelaJugador.Any(nodo => nodo == PosicionActual);
    }
}

