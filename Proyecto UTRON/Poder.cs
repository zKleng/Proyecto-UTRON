using System;

namespace Proyecto_UTRON
{
    public class Poder
    {
        public Nodo Nodo { get; private set; }
        public TipoPoder Tipo { get; private set; }
        public string Nombre { get; private set; } // Nueva propiedad Nombre

        public Poder(Nodo nodo, TipoPoder tipo, string nombre)
        {
            Nodo = nodo;
            Tipo = tipo;
            Nombre = nombre;
        }
        public void Aplicar(Moto moto)
        {
            // Lógica para aplicar el poder a la moto según su tipo
            switch (Tipo)
            {
                case TipoPoder.Turbo:
                    moto.Velocidad = Math.Min(moto.Velocidad + 3, 10);
                    break;
                case TipoPoder.Invencibilidad:
                    moto.EsInvencible = true;
                    break;
                case TipoPoder.DoblePuntaje:
                    // Implementar lógica de doble puntaje
                    break;
            }
        }
    }
}
