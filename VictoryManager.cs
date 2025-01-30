public class VictoryManager
{
    private Tablero tablero;
    private Dictionary<Ficha, bool> tieneBandera;
    private (int x, int y) posicionBandera;
    private bool banderaRecogida;
    private Dictionary<Ficha, int> contadorTrampas3;
    private Random random;

    public VictoryManager(Tablero tablero)
    {
        this.tablero = tablero;
        tieneBandera = new Dictionary<Ficha, bool>();
        contadorTrampas3 = new Dictionary<Ficha, int>();
        banderaRecogida = false;
        random = new Random();
        GenerarBandera();
    }

    private void GenerarBandera()
    {
        Random random = new Random();
        int x, y;
        bool posicionValida = false;

        // Intentar encontrar una posición válida para la bandera
        do
        {
            x = random.Next(tablero.Tamaño);
            y = random.Next(tablero.Tamaño);

            // Verificar que no está en las esquinas (bases)
            bool noEstaEnBases = !((x == 0 && y == 0) || (x == tablero.Tamaño - 1 && y == tablero.Tamaño - 1));
            
            // Verificar que la casilla está vacía
            bool casillaVacia = tablero.ObtenerCasilla(x, y).Tipo == TipoCasilla.Vacia;
            
            posicionValida = noEstaEnBases && casillaVacia;
        } while (!posicionValida);

        posicionBandera = (x, y);
    }

    public void IncrementarContadorTrampas3(Ficha ficha)
    {
        if (!contadorTrampas3.ContainsKey(ficha))
        {
            contadorTrampas3[ficha] = 0;
        }
        contadorTrampas3[ficha]++;
    }

    public bool VerificarVictoria(Ficha fichaActual, Dictionary<Ficha, (int, int)> posicionesIniciales)
    {
        // Verificar victoria por trampas nivel 3
        if (contadorTrampas3.ContainsKey(fichaActual) && contadorTrampas3[fichaActual] >= 3)
        {
            Console.Clear();
            Console.WriteLine($"¡{fichaActual.Nombre} ha perdido por caer en demasiadas trampas nivel 3!");
            return true;
        }

        // Verificar si el jugador tiene la bandera y ha llegado a su base
        if (tieneBandera.ContainsKey(fichaActual) && tieneBandera[fichaActual])
        {
            var posInicial = posicionesIniciales[fichaActual];
            if (fichaActual.PosX == posInicial.Item1 && fichaActual.PosY == posInicial.Item2)
            {
                Console.Clear();
                Console.WriteLine($"¡{fichaActual.Nombre} ha ganado capturando la bandera!");
                return true;
            }
        }

        // Verificar si el jugador ha recogido la bandera
        if (!banderaRecogida && fichaActual.PosX == posicionBandera.x && fichaActual.PosY == posicionBandera.y)
        {
            banderaRecogida = true;
            tieneBandera[fichaActual] = true;
            Console.WriteLine($"¡{fichaActual.Nombre} ha recogido la bandera!");
        }

        return false;
    }

public bool IntentarCapturarBandera(Ficha fichaActual, List<Ficha> todasLasFichas)
{
    var jugadorAdyacente = EncontrarJugadorAdyacente(fichaActual, todasLasFichas);
    if(jugadorAdyacente == null) return false;

    bool fichaActualTieneBandera = tieneBandera.ContainsKey(fichaActual) && tieneBandera[fichaActual];
    bool adyacenteTieneBandera = tieneBandera.ContainsKey(jugadorAdyacente) &&tieneBandera[jugadorAdyacente];

    if(!fichaActualTieneBandera && !adyacenteTieneBandera) return false;

    var defensor = fichaActualTieneBandera ? fichaActual:jugadorAdyacente;
    var atacante = fichaActualTieneBandera ? jugadorAdyacente : fichaActual;

    Console.WriteLine($"\n¡Duelo por la bandera entre {defensor.Nombre} y {atacante.Nombre}!");
    Console.WriteLine("Presiona cualquier tecla para tirar los dados...");
    Console.ReadKey(true);

    int valorDefensor = random.Next(1,7);
    int valorAtacante = random.Next(1,7);

    Console.WriteLine($"{defensor.Nombre} sacó un {valorDefensor}");
    Console.WriteLine($"{atacante.Nombre} sacó un {valorAtacante}");
    
    if(valorAtacante > valorDefensor)
    {
        tieneBandera[defensor]= false;
        tieneBandera[atacante]= true;
        Console.WriteLine($"\n¡{atacante.Nombre} ha capturado la bandera!");
        return true;
    }
    else
    {
        Console.WriteLine($"\n¡{defensor.Nombre} ha mantenido la bandera!");
        return false;
    }
}

private Ficha EncontrarJugadorAdyacente(Ficha fichaActual, List<Ficha> todasLasFichas)
{
    int[] dx = {-1,0,1,0};
    int[] dy = {0,1,0,-1};

    for (int i = 0; i < 4; i++)
    {
        int newX = fichaActual.PosX + dx[i];
        int newY = fichaActual.PosY + dy[i];

        if (newX >= 0 && newX < tablero.Tamaño && newY >= 0 && newY < tablero.Tamaño)
        {
            foreach (var otraFicha in todasLasFichas)
            {
                if(otraFicha != fichaActual && otraFicha.PosX == newX && otraFicha.PosY == newY) 
                    return otraFicha;
            }
        }
    }
    return null;
}




    public void DibujarBandera(char[,] visualizacion)
    {
        if (!banderaRecogida)
        {
            visualizacion[posicionBandera.x, posicionBandera.y] = 'F';
        }
    }
}