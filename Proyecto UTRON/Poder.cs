using System;
using System.Windows.Forms;

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
            switch (Tipo)
            {
                case TipoPoder.Turbo:
                    moto.Velocidad = Math.Min(moto.Velocidad + 3, 10);
                    break;
                case TipoPoder.Invencibilidad:
                    moto.EsInvencible = true;
                    break;
            }

            // Crear un timer para deshacer el efecto después de 10 segundos
            Timer timer = new Timer();
            timer.Interval = 10000; // 10 segundos
            timer.Tick += (sender, e) =>
            {
                RemoverEfecto(moto); // Llamar a un nuevo método que remueve el efecto
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void RemoverEfecto(Moto moto)
        {
            switch (Tipo)
            {
                case TipoPoder.Turbo:
                    moto.Velocidad = Math.Max(moto.Velocidad - 3, 1); // Reducir la velocidad después del turbo
                    break;
                case TipoPoder.Invencibilidad:
                    moto.EsInvencible = false; // Desactivar la invencibilidad
                    break;
            }
        }
    }
}
