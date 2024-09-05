using System;

namespace Proyecto_UTRON
{
    public class Item
    {
        public Nodo Nodo { get; private set; }
        public TipoItem Tipo { get; private set; }

        public Item(Nodo nodo, TipoItem tipo)
        {
            Nodo = nodo;
            Tipo = tipo;
        }

        public void Aplicar(Moto moto)
        {
            // Lógica para aplicar el ítem a la moto según su tipo
            switch (Tipo)
            {
                case TipoItem.CeldaCombustible:
                    moto.Combustible = Math.Min(moto.Combustible + 20, 100);
                    break;
                case TipoItem.MejoraVelocidad:
                    moto.Velocidad = Math.Min(moto.Velocidad + 1, 10);
                    break;
            }
        }
    }
}
