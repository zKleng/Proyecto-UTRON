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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Compiler", "CS0649:Field is never assigned to, and will always have its default value")]
        private Direccion direccionActual = Direccion.Ninguna; // Inicializar con un valor predeterminado
        private FlowLayoutPanel panelPoderes; // Panel para mostrar los poderes como botones

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

            // Timer para generar ítems y poderes cada 5 segundos
            generacionItemsPoderesTimer = new Timer();
            generacionItemsPoderesTimer.Interval = 5000;
            generacionItemsPoderesTimer.Tick += GeneracionItemsPoderesTimer_Tick;
            generacionItemsPoderesTimer.Start();

            // Panel para mostrar los poderes
            panelPoderes = new FlowLayoutPanel
            {
                Location = new Point(1120, 20),
                Size = new Size(200, 300),
                AutoScroll = true
            };
            Controls.Add(panelPoderes);

            // Agregar manejador de eventos para las teclas
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true; // Asegúrate de que el formulario reciba eventos de teclado
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
            panelPoderes.Controls.Clear(); // Limpiar los botones previos

            foreach (var poder in moto.PoderesRecogidos)
            {
                var button = new Button
                {
                    Text = poder.Tipo.ToString(), // Cambiado de Nombre a Tipo.ToString()
                    Size = new Size(180, 40),
                    BackColor = Color.LightBlue
                };
                button.Click += (sender, e) => UsarPoderSeleccionado(poder);
                panelPoderes.Controls.Add(button);
            }
        }

        private void UsarPoderSeleccionado(Poder poder)
        {
            poder.Aplicar(moto); // Aplicar el poder seleccionado
            moto.PoderesRecogidos.Remove(poder); // Remover el poder usado de la lista
            ActualizarPanelPoderes(); // Actualizar el panel después de usar el poder
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
                    Color color = fila.Ocupado ? Color.Red : Color.White;
                    e.Graphics.FillRectangle(new SolidBrush(color), fila.PosX * 20, fila.PosY * 20, 20, 20);
                    e.Graphics.DrawRectangle(Pens.Black, fila.PosX * 20, fila.PosY * 20, 20, 20);

                    fila = fila.Derecha;
                }
                actual = actual.Abajo;
            }

            // Dibujar la estela
            NodoEstela estelaActual = moto.EstelaInicio;
            while (estelaActual != null)
            {
                e.Graphics.FillRectangle(Brushes.Red, estelaActual.Nodo.PosX * 20, estelaActual.Nodo.PosY * 20, 20, 20);
                estelaActual = estelaActual.Siguiente;
            }

            // Dibujar ítems y poderes en el mapa
            foreach (var item in moto.Juego.Items)
            {
                e.Graphics.FillRectangle(Brushes.Green, item.Nodo.PosX * 20, item.Nodo.PosY * 20, 20, 20);
            }

            foreach (var poder in moto.Juego.Poderes)
            {
                e.Graphics.FillRectangle(Brushes.Purple, poder.Nodo.PosX * 20, poder.Nodo.PosY * 20, 20, 20);
            }

            // Mostrar estadísticas
            e.Graphics.DrawString($"Velocidad: {moto.Velocidad}", this.Font, Brushes.Black, new PointF(10, 500));
            e.Graphics.DrawString($"Tamaño Estela: {moto.TamanoEstela}", this.Font, Brushes.Black, new PointF(10, 520));
            e.Graphics.DrawString($"Combustible: {moto.Combustible}", this.Font, Brushes.Black, new PointF(10, 540));
        }
    }
}