using System;
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

        public bool EsInvencible { get; set; } = false;

        public NodoEstela EstelaInicio { get; private set; }

        public Queue<Item> ColaItems { get; private set; }
        public Stack<Poder> PilaPoderes { get; private set; }
        public List<Bot> Bots { get; private set; }  // Cambiado a private set

        private Timer itemTimer;

        public Juego Juego { get; private set; }

        public Moto(Nodo posicionInicial, Grid grid, List<Bot> bots)
        {
            PosicionActual = posicionInicial;
            this.grid = grid;
            Bots = bots ?? new List<Bot>();  // Inicializa Bots si es null
            Velocidad = 1;
            TamanoEstela = 3;
            Combustible = 100;
            celdasRecorridas = 0;
            EstelaInicio = null;
            PosicionActual.Ocupado = true;

            EsInvencible = false;

            ColaItems = new Queue<Item>();
            PilaPoderes = new Stack<Poder>();

            itemTimer = new Timer();
            itemTimer.Interval = 1000;
            itemTimer.Tick += (sender, e) => AplicarItems();
            itemTimer.Start();

            Juego = new Juego(grid);  // Puedes inicializar Juego aquí si es necesario
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

                VerificarRecoleccion(); // Solo recolectar, no aplicar
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

        public bool VerificarColisionOCombustible()
        {
            // Si la moto es invencible, no verificamos colisión con la estela
            if (!EsInvencible)
            {
                // Colisión con la estela
                NodoEstela actualEstela = EstelaInicio;
                while (actualEstela != null)
                {
                    if (actualEstela.Nodo == PosicionActual)
                    {
                        return false; // No hay colisión con la estela
                    }
                    actualEstela = actualEstela.Siguiente;
                }
            }

            // Verificar colisión con los bots
            foreach (var bot in Bots)
            {
                if (bot.PosicionActual == PosicionActual)
                {
                    return true; // Colisión detectada con un bot
                }
            }

            // Verificar si el combustible se ha agotado
            if (Combustible <= 0)
            {
                return true; // Combustible agotado
            }

            return false; // No hay colisión y hay combustible
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

        public void RecolectarPoder(Poder poder)
        {
            PilaPoderes.Push(poder);
        }

        public void RecolectarItem(Item item)
        {
            ColaItems.Enqueue(item);
            // Actualizar el panel de ítems cuando se recoja uno
            Form1 form = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form != null)
            {
                form.ActualizarPanelItems();
            }
        }

        public void UsarPoder()
        {
            if (PilaPoderes.Count > 0)
            {
                var poder = PilaPoderes.Pop();
                poder.Aplicar(this);
            }
        }

        public void AplicarItems()
        {
            if (ColaItems.Count > 0)
            {
                var item = ColaItems.Dequeue();
                if (item.Tipo == TipoItem.CeldaCombustible && this.Combustible >= 100)
                {
                    ColaItems.Enqueue(item); // No aplicar si el combustible es suficiente
                }
                else
                {
                    item.Aplicar(this); // Aplicar el ítem
                }
            }
        }

        public void UsarItem(int indiceItemSeleccionado)
        {
            if (ColaItems.Count > 0 && indiceItemSeleccionado >= 0)
            {
                var item = ColaItems.ElementAt(indiceItemSeleccionado);
                item.Aplicar(this); // Aplica el ítem
                ColaItems = new Queue<Item>(ColaItems.Where(i => i != item)); // Elimina el ítem de la cola
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
