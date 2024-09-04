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
            Nodo nodoItem;
            do
            {
                nodoItem = grid.ObtenerNodoAleatorio();
            } while (nodoItem.Ocupado || Poderes.Exists(p => p.Nodo == nodoItem));
            // Verifica que no haya un poder en el mismo nodo

            TipoItem tipo = (TipoItem)new Random().Next(0, Enum.GetValues(typeof(TipoItem)).Length);
            Items.Add(new Item(nodoItem, tipo));
        }

        public void GenerarPoder()
        {
            Nodo nodoPoder;
            do
            {
                nodoPoder = grid.ObtenerNodoAleatorio();
            } while (nodoPoder.Ocupado || Items.Exists(i => i.Nodo == nodoPoder));
            // Verifica que no haya un ítem en el mismo nodo

            TipoPoder tipo = (TipoPoder)new Random().Next(0, Enum.GetValues(typeof(TipoPoder)).Length);

            // Crear un nombre para el poder basado en su tipo
            string nombre = tipo.ToString();

            Poderes.Add(new Poder(nodoPoder, tipo, nombre));
        }
    }
}
