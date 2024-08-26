using System.Collections.Generic;
using System;

namespace Proyecto_UTRON
{
    public class Juego
    {
        public List<Item> Items { get; private set; }
        public List<Poder> Poderes { get; private set; }
        private Grid grid;

        public Juego(Grid grid)
        {
            this.grid = grid;
            Items = new List<Item>();
            Poderes = new List<Poder>();
        }

        public void GenerarItemsYPoderes()
        {
            GenerarItem();
            GenerarPoder();
        }

        public void GenerarItem()
        {
            Nodo nodo = grid.ObtenerNodoAleatorio();
            TipoItem tipo = (TipoItem)new Random().Next(0, Enum.GetValues(typeof(TipoItem)).Length);
            Items.Add(new Item(nodo, tipo));
        }

        public void GenerarPoder()
        {
            Nodo nodo = grid.ObtenerNodoAleatorio();
            TipoPoder tipo = (TipoPoder)new Random().Next(0, Enum.GetValues(typeof(TipoPoder)).Length);

            // Crear un nombre para el poder basado en su tipo
            string nombre = tipo.ToString(); // O puedes definir nombres específicos para cada tipo de poder

            Poderes.Add(new Poder(nodo, tipo, nombre));
        }
    }
}
