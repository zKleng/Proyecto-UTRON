using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_UTRON
{
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

        // La estela es una lista de tuplas que contiene el nodo y su tiempo de creación
        public List<(Nodo nodo, DateTime tiempoCreacion)> Estela { get; private set; }

        public Moto(Nodo posicionInicial, Grid grid)
        {
            PosicionActual = posicionInicial;
            this.grid = grid;
            Velocidad = 2; // Velocidad entre 1 y 10
            TamanoEstela = 3; // Tamaño inicial de la estela
            Combustible = 100; // Valor inicial del combustible
            celdasRecorridas = 0;
            Estela = new List<(Nodo nodo, DateTime tiempoCreacion)>();
            PosicionActual.Ocupado = true;
        }

        public void Moverse(Direccion direccion)
        {
            if (Combustible <= 0)
            {
                return;
            }

            this.direccion = direccion;
            Nodo siguientePosicion = ObtenerNodoSiguiente(direccion);

            if (siguientePosicion != null && !siguientePosicion.Ocupado)
            {
                // Actualizar la estela
                Estela.Add((PosicionActual, DateTime.Now));

                // Liberar la posición anterior y mover la moto
                PosicionActual.Ocupado = false;
                PosicionActual = siguientePosicion;
                PosicionActual.Ocupado = true;

                // Verificar el tiempo de vida de la estela y eliminar nodos después de 4 segundos
                ActualizarEstela();

                celdasRecorridas++;
                VerificarCombustible();
            }
        }

        private void ActualizarEstela()
        {
            DateTime tiempoActual = DateTime.Now;

            // Filtrar los nodos cuya estela ha expirado (más de 4 segundos)
            foreach (var (nodo, tiempoCreacion) in Estela.ToList())
            {
                if ((tiempoActual - tiempoCreacion).TotalSeconds > 4)
                {
                    nodo.Ocupado = false;
                    Estela.Remove((nodo, tiempoCreacion));
                }
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
                        siguienteNodo = grid.ObtenerNodoEnPos(PosicionActual.PosX, 0);
                    }
                    break;

                case Direccion.Abajo:
                    siguienteNodo = PosicionActual.Abajo;
                    if (siguienteNodo == null)
                    {
                        siguienteNodo = grid.ObtenerNodoEnPos(0, PosicionActual.PosY);
                    }
                    break;

                case Direccion.Izquierda:
                    siguienteNodo = PosicionActual.Izquierda;
                    if (siguienteNodo == null)
                    {
                        siguienteNodo = grid.ObtenerNodoEnPos(255, PosicionActual.PosY);
                    }
                    break;

                case Direccion.Derecha:
                    siguienteNodo = PosicionActual.Derecha;
                    if (siguienteNodo == null)
                    {
                        siguienteNodo = grid.ObtenerNodoEnPos(0, PosicionActual.PosY);
                    }
                    break;
            }

            return siguienteNodo;
        }

        private void VerificarCombustible()
        {
            if (celdasRecorridas >= 5)
            {
                Combustible -= 1;
                celdasRecorridas = 0;
            }

            if (Combustible < 0)
            {
                Combustible = 0;
            }
        }
    }
}
