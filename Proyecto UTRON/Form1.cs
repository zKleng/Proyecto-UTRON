using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_UTRON
{
    public partial class Form1 : Form
    {
        private Grid grid;
        private Moto moto;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            grid = new Grid(54, 23);
            moto = new Moto(grid.Inicio, grid); // Asegúrate de pasar el grid al constructor de Moto
        }

        // Método que se ejecuta cuando el formulario se carga
        private void Form1_Load(object sender, EventArgs e)
        {
            // Puedes agregar cualquier código que necesites ejecutar al cargar el formulario
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    moto.Moverse(Direccion.Arriba);
                    break;
                case Keys.Down:
                    moto.Moverse(Direccion.Abajo);
                    break;
                case Keys.Left:
                    moto.Moverse(Direccion.Izquierda);
                    break;
                case Keys.Right:
                    moto.Moverse(Direccion.Derecha);
                    break;
            }

            Invalidate(); // Redibujar el formulario para reflejar el movimiento
            return base.ProcessCmdKey(ref msg, keyData);
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

            // Mostrar estadísticas
            e.Graphics.DrawString($"Velocidad: {moto.Velocidad}", this.Font, Brushes.Black, new PointF(10, 500));
            e.Graphics.DrawString($"Tamaño Estela: {moto.TamanoEstela}", this.Font, Brushes.Black, new PointF(10, 520));
            e.Graphics.DrawString($"Combustible: {moto.Combustible}", this.Font, Brushes.Black, new PointF(10, 550));
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Elimina este método si no usas un Label en tu formulario
        }
    }
}
