public static class TrampaManager
{
    private static Dictionary<Ficha, int> jugadoresInvalidados = new Dictionary<Ficha, int>();
    private static Dictionary<(int, int), (int, int)> conexionesTrampa3 = new Dictionary<(int, int), (int, int)>();

    public static int ManejarTrampa(Ficha ficha, Casilla casilla, Tablero tablero, GestorTurnos gestor)
    {
            if(ficha is CaballeroDeLaMuerte)
            {
                Console.WriteLine($"EL {ficha.Nombre} es inmune a las trampas");
                return 1;
            }

            switch (casilla.Tipo)
            {
                case TipoCasilla.Trampa1:
                    InvalidarJugador(ficha);
                    Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 1. ¡Pierde el siguiente turno!");
                    casilla.Tipo = TipoCasilla.Vacia;
                    gestor.FinalizarTurnoInmediato();
                    return 0;

                case TipoCasilla.Trampa2:
                    // Volver al inicio
                    tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;
                    var posInicial = gestor.posicionesIniciales[ficha];
                    ficha.PosX = posInicial.Item1;
                    ficha.PosY = posInicial.Item2;
                    tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = true;
                    Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 2. ¡Regresa al inicio!");
                    casilla.Tipo = TipoCasilla.Vacia;
                    return -1;

                case TipoCasilla.Trampa3:
                    // Buscar otra trampa nivel 3
                    List<(int x, int y)> trampasTipo3 = new List<(int x, int y)>();
                    for (int i = 0; i < tablero.Tamaño; i++)
                    {
                        for (int j = 0; j < tablero.Tamaño; j++)
                        {
                            if (tablero.ObtenerCasilla(i, j).Tipo == TipoCasilla.Trampa3 &&
                                (i != ficha.PosX || j != ficha.PosY))
                            {
                                trampasTipo3.Add((i, j));
                            }
                        }
                    }

                    if (trampasTipo3.Count > 0)
                    {
                        Random random = new Random();
                        var destino = trampasTipo3[random.Next(trampasTipo3.Count)];
                        tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;
                        ficha.PosX = destino.x;
                        ficha.PosY = destino.y;
                        tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = true;
                        gestor.victoryManager.IncrementarContadorTrampas3(ficha);
                        Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 3. ¡Se teletransporta a otra trampa!");
                    }
                    return -1;

                default:
                    System.Console.WriteLine("eros");
                    return -1;
            }
        }
    

    private static void InvalidarJugador(Ficha ficha)
    {
        jugadoresInvalidados[ficha] = 1; // Durará 1 turno
    }

    private static void RegresarAlInicio(Ficha ficha, Tablero tablero)
    {
        // Desocupar la casilla actual
        tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;

        // Determinar punto inicial basado en la posición actual
        if (ficha.PosX > tablero.Tamaño / 2)
        {
            ficha.PosX = tablero.Tamaño - 1;
            ficha.PosY = tablero.Tamaño - 1;
        }
        else
        {
            ficha.PosX = 0;
            ficha.PosY = 0;
        }

        // Ocupar la nueva casilla
        tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = true;

        System.Console.WriteLine("karla");
    }

    private static void TeletransportarAOtraTrampa3(Ficha ficha, Casilla casilla, Tablero tablero)
    {
        // Buscar otra trampa nivel 3 en el tablero
        List<(int, int)> trampasTipo3 = new List<(int, int)>();
        
        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                if (tablero.ObtenerCasilla(i, j).Tipo == TipoCasilla.Trampa3 &&
                    (i != casilla.PosX || j != casilla.PosY))
                {
                    trampasTipo3.Add((i, j));
                }
            }
        }

        if (trampasTipo3.Count > 0)
        {
            // Elegir una trampa aleatoria
            Random random = new Random();
            var destinoTrampa = trampasTipo3[random.Next(trampasTipo3.Count)];

            // Desocupar la casilla actual
            tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;

            // Mover a la nueva posición
            ficha.PosX = destinoTrampa.Item1;
            ficha.PosY = destinoTrampa.Item2;

            // Ocupar la nueva casilla
            tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = true;

            Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 3. ¡Se teletransporta a otra trampa!");
        }
    }

    public static bool EstaJugadorInvalidado(Ficha ficha)
    {
        if(jugadoresInvalidados.ContainsKey(ficha))
        {
            return jugadoresInvalidados[ficha]>0;
        }
        return false;
    }

    public static void ActualizarEstadoInvalidaciones()
{
    var jugadoresParaRemover = new List<Ficha>();
    
    foreach (var kvp in jugadoresInvalidados)
    {
        jugadoresInvalidados[kvp.Key] -= 1;
        if (jugadoresInvalidados[kvp.Key] <= 0)
        {
            jugadoresParaRemover.Add(kvp.Key);
        }
    }
    
    foreach (var ficha in jugadoresParaRemover)
    {
        jugadoresInvalidados.Remove(ficha);
    }
}
}
