using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_UTRON
{
    public class Grid
    {
        public Nodo Inicio { get; private set; }
        public int Ancho { get; }
        public int Alto { get; }

        public Grid(int ancho, int alto)
        {
            Ancho = ancho;
            Alto = alto;
            InicializarGrid();
        }

        private void InicializarGrid()
        {
            Nodo nodoArriba = null;
            Nodo nodoIzquierda = null;

            for (int y = 0; y < Alto; y++)
            {
                Nodo filaAnterior = nodoArriba;
                nodoArriba = null;

                for (int x = 0; x < Ancho; x++)
                {
                    Nodo nuevoNodo = new Nodo(x, y);

                    if (x == 0 && y == 0)
                    {
                        Inicio = nuevoNodo;
                    }

                    if (nodoIzquierda != null)
                    {
                        nuevoNodo.Izquierda = nodoIzquierda;
                        nodoIzquierda.Derecha = nuevoNodo;
                    }

                    if (filaAnterior != null)
                    {
                        nuevoNodo.Arriba = filaAnterior;
                        filaAnterior.Abajo = nuevoNodo;
                        filaAnterior = filaAnterior.Derecha;
                    }

                    nodoIzquierda = nuevoNodo;

                    if (nodoArriba == null)
                    {
                        nodoArriba = nuevoNodo;
                    }
                }
            }
        }
    }
}
