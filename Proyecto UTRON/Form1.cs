using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Proyecto_UTRON
{
    public partial class Form1 : Form
    {
        private Grid grid;
        private Moto moto;
        private Timer movimientoTimer;
        private Timer generacionItemsPoderesTimer;
        private Direccion direccionActual = Direccion.Ninguna; // Inicializar con un valor predeterminado
        private FlowLayoutPanel panelPoderes; // Panel para mostrar los poderes como botones
        private FlowLayoutPanel panelItems; // Panel para mostrar los ítems como botones

        private int indiceItemSeleccionado = -1;
        private int indicePoderSeleccionado = -1;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            grid = new Grid(54, 23);
            moto = new Moto(grid.Inicio, grid);

            movimientoTimer = new Timer();
            movimientoTimer.Interval = 100;
            movimientoTimer.Tick += MovimientoTimer_Tick;
            movimientoTimer.Start();

            generacionItemsPoderesTimer = new Timer();
            generacionItemsPoderesTimer.Interval = 5000;
            generacionItemsPoderesTimer.Tick += GeneracionItemsPoderesTimer_Tick;
            generacionItemsPoderesTimer.Start();

            panelPoderes = new FlowLayoutPanel
            {
                Location = new Point(1090, 20),
                Size = new Size(200, 300),
                AutoScroll = true,
                BackColor = Color.White
            };
            Controls.Add(panelPoderes);

            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true; // Asegúrate de que el formulario reciba eventos de teclado

            panelItems = new FlowLayoutPanel
            {
                Location = new Point(1090, 300),
                Size = new Size(200, 300),
                AutoScroll = true,
                BackColor = Color.Black
            };
            Controls.Add(panelItems);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Control de dirección
                case Keys.Up:
                    direccionActual = Direccion.Arriba;
                    break;
                case Keys.Down:
                    direccionActual = Direccion.Abajo;
                    break;
                case Keys.Left:
                    direccionActual = Direccion.Izquierda;
                    break;
                case Keys.Right:
                    direccionActual = Direccion.Derecha;
                    break;

                // Navegación y uso de ítems
                case Keys.D1:
                    if (moto.ColaItems.Count > 0)
                    {
                        indiceItemSeleccionado = (indiceItemSeleccionado + 1) % moto.ColaItems.Count;
                        ActualizarPanelItems(); // Actualizar la visualización de ítems
                    }
                    break;
                case Keys.D2:
                    moto.UsarItem(indiceItemSeleccionado); // Usar el ítem seleccionado
                    indiceItemSeleccionado = -1; // Reiniciar selección
                    ActualizarPanelItems(); // Actualizar la visualización de ítems
                    break;

                // Navegación y uso de poderes
                case Keys.A:
                    if (moto.PilaPoderes.Count > 0)
                    {
                        indicePoderSeleccionado = (indicePoderSeleccionado + 1) % moto.PilaPoderes.Count;
                        ActualizarPanelPoderes();
                    }
                    break;
                case Keys.D:
                    if (indicePoderSeleccionado >= 0 && moto.PilaPoderes.Count > 0)
                    {
                        var poder = moto.PilaPoderes.Pop(); // Usar el poder seleccionado
                        poder.Aplicar(moto);
                        indicePoderSeleccionado = -1; // Reiniciar selección
                        ActualizarPanelPoderes();
                    }
                    break;
            }
        }


        private void UsarItem()
        {
            if (indiceItemSeleccionado >= 0 && moto.ColaItems.Count > 0)
            {
                var item = moto.ColaItems.ElementAt(indiceItemSeleccionado);
                item.Aplicar(moto); // Aplica el ítem y lo elimina de la cola
                indiceItemSeleccionado = -1; // Reiniciar selección
                ActualizarPanelItems(); // Actualizar después de usar el ítem
            }
        }

        private void MovimientoTimer_Tick(object sender, EventArgs e)
        {
            if (direccionActual != Direccion.Ninguna)
            {
                for (int i = 0; i < moto.Velocidad; i++)
                {
                    moto.Moverse(direccionActual);
                    moto.VerificarRecoleccion();
                    if (moto.Combustible <= 0)
                    {
                        movimientoTimer.Stop();
                        break;
                    }
                }
                ActualizarPanelPoderes(); // Actualizar la visualización de los poderes
                Invalidate(); // Redibujar el formulario para reflejar el movimiento
            }
        }

        private void GeneracionItemsPoderesTimer_Tick(object sender, EventArgs e)
        {
            moto.Juego.GenerarItemsYPoderes();
        }

        private void ActualizarPanelPoderes()
        {
            panelPoderes.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panelPoderes, true, null);

            int currentButtonCount = panelPoderes.Controls.Count;

            if (currentButtonCount != moto.PilaPoderes.Count)
            {
                panelPoderes.Controls.Clear();

                foreach (var poder in moto.PilaPoderes)
                {
                    var button = new Button
                    {
                        Text = poder.Tipo.ToString(),
                        Size = new Size(180, 40),
                        BackColor = Color.LightBlue
                    };
                    panelPoderes.Controls.Add(button);
                }
            }
        }
        private void ActualizarPanelItems()
        {
            // Evita parpadeos al habilitar el doble búfer en el panel de ítems
            panelItems.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panelItems, true, null);

            // Obtener la cantidad actual de botones en el panel
            int currentButtonCount = panelItems.Controls.Count;

            // Si la cantidad de botones no coincide con la cantidad de ítems en la cola, actualizamos
            if (currentButtonCount != moto.ColaItems.Count)
            {
                panelItems.Controls.Clear(); // Solo limpiar si es necesario

                foreach (var item in moto.ColaItems)
                {
                    var button = new Button
                    {
                        Text = item.Tipo.ToString(),
                        Size = new Size(180, 40),
                        BackColor = Color.LightGreen
                    };
                    panelItems.Controls.Add(button);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Nodo actual = grid.Inicio;
            while (actual != null)
            {
                Nodo fila = actual;
                while (fila != null)
                {
                    Color color = fila.Ocupado ? Color.Red : Color.Black;
                    e.Graphics.FillRectangle(new SolidBrush(color), fila.PosX * 20, fila.PosY * 20, 20, 20);
                    e.Graphics.DrawRectangle(Pens.Black, fila.PosX * 20, fila.PosY * 20, 20, 20);

                    fila = fila.Derecha;
                }
                actual = actual.Abajo;
            }

            NodoEstela estelaActual = moto.EstelaInicio;
            while (estelaActual != null)
            {
                e.Graphics.FillRectangle(Brushes.Red, estelaActual.Nodo.PosX * 20, estelaActual.Nodo.PosY * 20, 20, 20);
                estelaActual = estelaActual.Siguiente;
            }

            foreach (var item in moto.Juego.Items)
            {
                e.Graphics.FillRectangle(Brushes.Green, item.Nodo.PosX * 20, item.Nodo.PosY * 20, 20, 20);
            }

            foreach (var poder in moto.Juego.Poderes)
            {
                e.Graphics.FillRectangle(Brushes.Purple, poder.Nodo.PosX * 20, poder.Nodo.PosY * 20, 20, 20);
            }

            e.Graphics.DrawString($"Velocidad: {moto.Velocidad}", this.Font, Brushes.Black, new PointF(10, 500));
            e.Graphics.DrawString($"Tamaño Estela: {moto.TamanoEstela}", this.Font, Brushes.Black, new PointF(10, 520));
            e.Graphics.DrawString($"Combustible: {moto.Combustible}", this.Font, Brushes.Black, new PointF(10, 540));
        }
    }
}
