﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Proyecto_UTRON
{
    public class Moto
    {
        public Nodo PosicionActual { get; private set; }
        private Grid grid;
        private Random random = new Random();

        public int Velocidad { get; set; }
        public int TamanoEstela { get; set; }
        public int Combustible { get; set; }
        private int celdasRecorridas;
        private Direccion direccion;

        public bool EsInvencible { get; set; }

        public NodoEstela EstelaInicio { get; private set; }

        public Queue<Item> ColaItems { get; private set; }
        public Stack<Poder> PilaPoderes { get; private set; } // Cambiado de List<Poder> a Stack<Poder>


        private Timer itemTimer;

        public Juego Juego { get; private set; }
        public List<Poder> PoderesRecogidos { get; private set; }

        public Moto(Nodo posicionInicial, Grid grid)
        {
            PosicionActual = posicionInicial;
            this.grid = grid;
            Velocidad = 1;
            TamanoEstela = 3;
            Combustible = 100;
            celdasRecorridas = 0;
            EstelaInicio = null;
            PosicionActual.Ocupado = true;

            EsInvencible = false;

            ColaItems = new Queue<Item>();
            PilaPoderes = new Stack<Poder>(); // Inicializar la pila de poderes

            itemTimer = new Timer();
            itemTimer.Interval = 1000;
            itemTimer.Tick += (sender, e) => AplicarItems();
            itemTimer.Start();

            Juego = new Juego(grid);
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
                EstelaInicio = new NodoEstela(PosicionActual, DateTime.Now, EstelaInicio);

                PosicionActual.Ocupado = false;
                PosicionActual = siguientePosicion;
                PosicionActual.Ocupado = true;

                ActualizarEstela();

                celdasRecorridas++;
                VerificarCombustible();

                VerificarRecoleccion();
            }
        }

        private void ActualizarEstela()
        {
            DateTime tiempoActual = DateTime.Now;
            NodoEstela actual = EstelaInicio;
            NodoEstela previo = null;

            while (actual != null)
            {
                if ((tiempoActual - actual.TiempoCreacion).TotalSeconds > 4)
                {
                    if (previo == null)
                    {
                        EstelaInicio = actual.Siguiente;
                    }
                    else
                    {
                        previo.Siguiente = actual.Siguiente;
                    }
                }
                else
                {
                    previo = actual;
                }

                actual = actual.Siguiente;
            }
        }

        private Nodo ObtenerNodoSiguiente(Direccion direccion)
        {
            Nodo siguienteNodo = null;

            switch (direccion)
            {
                case Direccion.Arriba:
                    siguienteNodo = PosicionActual.Arriba;
                    break;
                case Direccion.Abajo:
                    siguienteNodo = PosicionActual.Abajo;
                    break;
                case Direccion.Izquierda:
                    siguienteNodo = PosicionActual.Izquierda;
                    break;
                case Direccion.Derecha:
                    siguienteNodo = PosicionActual.Derecha;
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

        public void VerificarRecoleccion()
        {
            var item = Juego.Items.FirstOrDefault(i => i.Nodo == PosicionActual);
            if (item != null)
            {
                RecolectarItem(item);
                Juego.Items.Remove(item);
            }

            var poder = Juego.Poderes.FirstOrDefault(p => p.Nodo == PosicionActual);
            if (poder != null)
            {
                RecolectarPoder(poder);
                Juego.Poderes.Remove(poder);
            }
        }

        private void RecolectarItem(Item item)
        {
            ColaItems.Enqueue(item);
        }

        private void RecolectarPoder(Poder poder)
        {
            PilaPoderes.Push(poder); // Añadir poder a la pila
        }

        public void UsarPoder()
        {
            if (PilaPoderes.Count > 0)
            {
                var poder = PilaPoderes.Pop(); // Usar el último poder añadido
                poder.Aplicar(this);
            }
        }

        private void AplicarItems()
        {
            if (ColaItems.Count > 0)
            {
                var item = ColaItems.Dequeue();
                item.Aplicar(this);
            }
        }
    }

    public class NodoEstela
    {
        public Nodo Nodo { get; }
        public DateTime TiempoCreacion { get; }
        public NodoEstela Siguiente { get; set; }

        public NodoEstela(Nodo nodo, DateTime tiempoCreacion, NodoEstela siguiente)
        {
            Nodo = nodo;
            TiempoCreacion = tiempoCreacion;
            Siguiente = siguiente;
        }
    }
}