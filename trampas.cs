public static class TrampaManager
{
    private static Dictionary<(int, int), int> jugadoresInvalidados = new Dictionary<(int, int), int>();
    private static Dictionary<(int, int), (int, int)> conexionesTrampa3 = new Dictionary<(int, int), (int, int)>();

    public static void ManejarTrampa(Ficha ficha, Casilla casilla, Tablero tablero)
    {
        switch (casilla.Tipo)
        {
            case TipoCasilla.Trampa1:
                InvalidarJugador(ficha);
                Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 1. Pierde el siguiente turno!");
                casilla.Tipo = TipoCasilla.Vacia; // La trampa se destruye al activarse
                break;

            case TipoCasilla.Trampa2:
                RegresarAlInicio(ficha, tablero);
                Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 2. ¡Regresa al inicio!");
                casilla.Tipo = TipoCasilla.Vacia; // La trampa se destruye al activarse
                break;

            case TipoCasilla.Trampa3:
                TeletransportarAOtraTrampa3(ficha, casilla, tablero);
                break;
        }
    }

    private static void InvalidarJugador(Ficha ficha)
    {
        jugadoresInvalidados[(ficha.PosX, ficha.PosY)] = 1; // Durará 1 turno
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
    }

    private static void TeletransportarAOtraTrampa3(Ficha ficha, Casilla casillaActual, Tablero tablero)
    {
        // Buscar otra trampa nivel 3 en el tablero
        List<(int, int)> trampasTipo3 = new List<(int, int)>();
        
        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                if (tablero.ObtenerCasilla(i, j).Tipo == TipoCasilla.Trampa3 &&
                    (i != casillaActual.PosX || j != casillaActual.PosY))
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
        return jugadoresInvalidados.ContainsKey((ficha.PosX, ficha.PosY));
    }

    public static void ActualizarEstadoInvalidaciones()
    {
        var invalidacionesActualizadas = new Dictionary<(int, int), int>();
        
        foreach (var kvp in jugadoresInvalidados)
        {
            if (kvp.Value > 1)
                invalidacionesActualizadas[kvp.Key] = kvp.Value - 1;
        }
        
        jugadoresInvalidados = invalidacionesActualizadas;
    }
}
