using Proyecto_UTRON;

public class Moto
{
    public Nodo PosicionActual { get; private set; }

    public Moto(Nodo posicionInicial)
    {
        PosicionActual = posicionInicial;
        PosicionActual.Ocupado = true;
    }

    public void Moverse(Direccion direccion)
    {
        Nodo siguientePosicion = null;

        switch (direccion)
        {
            case Direccion.Arriba:
                siguientePosicion = PosicionActual.Arriba;
                break;
            case Direccion.Abajo:
                siguientePosicion = PosicionActual.Abajo;
                break;
            case Direccion.Izquierda:
                siguientePosicion = PosicionActual.Izquierda;
                break;
            case Direccion.Derecha:
                siguientePosicion = PosicionActual.Derecha;
                break;
        }

        if (siguientePosicion != null && !siguientePosicion.Ocupado)
        {
            PosicionActual.Ocupado = false;
            PosicionActual = siguientePosicion;
            PosicionActual.Ocupado = true;
        }
    }
}
