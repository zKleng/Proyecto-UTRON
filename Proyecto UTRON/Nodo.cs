using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_UTRON
{
    public class Nodo
    {
        public Nodo Arriba { get; set; }
        public Nodo Abajo { get; set; }
        public Nodo Izquierda { get; set; }
        public Nodo Derecha { get; set; }

        public int PosX { get; }
        public int PosY { get; }

        public bool Ocupado { get; set; }

        public Nodo(int x, int y)
        {
            PosX = x;
            PosY = y;
            Ocupado = false;
        }
    }

}
