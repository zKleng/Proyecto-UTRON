using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_UTRON
{
    public abstract class ElementoJuego
    {
        public string Nombre { get; protected set; }
        public abstract void Aplicar(Moto moto);
    }
}
