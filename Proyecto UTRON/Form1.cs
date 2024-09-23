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
        private List<Bot> bots;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            grid = new Grid(54, 23);
            moto = new Moto(grid.Inicio, grid, bots);
            bots = new List<Bot>();


            movimientoTimer = new Timer();
            movimientoTimer.Interval = 100;
            movimientoTimer.Tick += MovimientoTimer_Tick;
            movimientoTimer.Start();

            generacionItemsPoderesTimer = new Timer();
            generacionItemsPoderesTimer.Interval = 5000;
            generacionItemsPoderesTimer.Tick += GeneracionItemsPoderesTimer_Tick;
            generacionItemsPoderesTimer.Start();
            // Crear 4 bots en posiciones aleatorias
            for (int i = 0; i < 4; i++)
            {
                Nodo posicionInicialBot = grid.ObtenerPosicionAleatoria(); // Asume que tienes un método para esto
                Bot bot = new Bot(grid, 100); // Puedes ajustar el valor de combustible inicial según lo que desees.
                bots.Add(bot);
            }

            // Agrega un timer para mover los bots de manera aleatoria
            Timer botMovimientoTimer = new Timer();
            botMovimientoTimer.Interval = 200; // Intervalo para mover los bots
            botMovimientoTimer.Tick += BotMovimientoTimer_Tick;
            botMovimientoTimer.Start();
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
                Location = new Point(1090, 350),
                Size = new Size(200, 300),
                AutoScroll = true,
                BackColor = Color.Black
            };
            Controls.Add(panelItems);
        }
        private void BotMovimientoTimer_Tick(object sender, EventArgs e)
        {
            var botsParaEliminar = new List<Bot>();

            foreach (var bot in bots)
            {
                bot.Mover(grid);

                // Verificar si el bot colisiona con la estela del jugador
                NodoEstela estela = moto.EstelaInicio;
                bool colisionaConEstela = false;
                while (estela != null)
                {
                    if (estela.Nodo == bot.PosicionActual)
                    {
                        colisionaConEstela = true;
                        botsParaEliminar.Add(bot);
                        break; // Salir del bucle si el bot ha tocado la estela
                    }
                    estela = estela.Siguiente;
                }

                // Si el bot no colisionó con la estela, verifica la colisión con la moto
                if (!colisionaConEstela && bot.PosicionActual == moto.PosicionActual)
                {
                    if (!moto.EsInvencible)
                    {
                        GameOver();
                        return; // Termina el ciclo si ocurre game over
                    }
                }
            }

            // Eliminar los bots que colisionaron con la estela
            foreach (var bot in botsParaEliminar)
            {
                bots.Remove(bot);
                // Si necesitas hacer algo más con el bot eliminado, agrégalo aquí
            }

            Invalidate(); // Redibujar la pantalla para actualizar las posiciones de los bots
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
                // Navegación de poderes
                case Keys.A:
                    if (moto.PilaPoderes.Count > 0)
                    {
                        // Mueve la selección hacia la izquierda
                        indicePoderSeleccionado = (indicePoderSeleccionado - 1 + moto.PilaPoderes.Count) % moto.PilaPoderes.Count;
                        ActualizarPanelPoderes(); // Actualiza la visualización de poderes
                    }
                    break;
                case Keys.D:
                    if (moto.PilaPoderes.Count > 0)
                    {
                        // Mueve la selección hacia la derecha
                        indicePoderSeleccionado = (indicePoderSeleccionado + 1) % moto.PilaPoderes.Count;
                        ActualizarPanelPoderes(); // Actualiza la visualización de poderes
                    }
                    break;

                // Selección del poder con Enter
                case Keys.Enter:
                    if (indicePoderSeleccionado >= 0 && moto.PilaPoderes.Count > 0)
                    {
                        // Convertimos la pila en un array temporal para acceder al poder seleccionado
                        var poderesArray = moto.PilaPoderes.ToArray();
                        var poderSeleccionado = poderesArray[indicePoderSeleccionado];

                        // Aplica el poder a la moto
                        poderSeleccionado.Aplicar(moto);

                        // Reconstruye la pila excluyendo el poder seleccionado
                        var nuevaPila = new Stack<Poder>(poderesArray.Where((p, i) => i != indicePoderSeleccionado).Reverse());

                        // Vacía la pila original y empuja los elementos de la nueva pila
                        moto.PilaPoderes.Clear();
                        foreach (var poder in nuevaPila)
                        {
                            moto.PilaPoderes.Push(poder);
                        }

                        // Reinicia la selección
                        indicePoderSeleccionado = -1;
                        ActualizarPanelPoderes(); // Actualiza la visualización de poderes
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

                    // Verificar si ocurre game over
                    if (moto.VerificarColisionOCombustible())
                    {
                        GameOver();
                        return; // Termina el ciclo si ocurre game over
                    }

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


        private void VerificarColisionesConBots()
        {
            var botsParaEliminar = new List<Bot>();

            foreach (var bot in moto.Bots)
            {
                if (bot.PosicionActual == moto.PosicionActual)
                {
                    // El bot está colisionando con la moto
                    if (!moto.EsInvencible)
                    {
                        GameOver();
                        return;
                    }
                }

                // Verificar colisión del bot con la estela del jugador
                if (moto.EstelaInicio != null)
                {
                    NodoEstela estela = moto.EstelaInicio;
                    while (estela != null)
                    {
                        if (estela.Nodo == bot.PosicionActual)
                        {
                            botsParaEliminar.Add(bot);
                            break;
                        }
                        estela = estela.Siguiente;
                    }
                }
            }

            // Eliminar los bots que colisionaron con la estela
            foreach (var bot in botsParaEliminar)
            {
                moto.Bots.Remove(bot);
                // Si necesitas hacer algo más con el bot eliminado, agrégalo aquí
            }
        }




        private void GeneracionItemsPoderesTimer_Tick(object sender, EventArgs e)
        {
            moto.Juego.GenerarItemsYPoderes();
        }

        public void ActualizarPanelPoderes()
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
        public void ActualizarPanelItems()
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
        private void GameOver()
        {
            // Detener todos los timers del juego
            movimientoTimer.Stop();
            generacionItemsPoderesTimer.Stop();

            // Mostrar mensaje de game over
            MessageBox.Show("Game Over! La moto colisionó o se quedó sin combustible.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Opcional: Puedes reiniciar el juego o cerrar la ventana
            Application.Exit(); // Cerrar el juego
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

            foreach (var bot in bots)
            {
                e.Graphics.FillRectangle(Brushes.Blue, bot.PosicionActual.PosX * 20, bot.PosicionActual.PosY * 20, 20, 20);
            }

            e.Graphics.DrawString($"Velocidad: {moto.Velocidad}", this.Font, Brushes.Black, new PointF(10, 500));
            e.Graphics.DrawString($"Tamaño Estela: {moto.TamanoEstela}", this.Font, Brushes.Black, new PointF(10, 520));
            e.Graphics.DrawString($"Combustible: {moto.Combustible}", this.Font, Brushes.Black, new PointF(10, 540));
        }

    }
}
