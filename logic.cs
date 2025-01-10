using System;
using System.Collections.Generic;



    public enum TipoCasilla
    {
        Vacia,
        Obstaculo,
        Trampa1,
        Trampa2,
        Trampa3
    }

    public class Casilla
    {
        public TipoCasilla Tipo { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public bool EstaOcupada { get; set; }

        public Casilla(int x, int y)
        {
            PosX = x;
            PosY = y;
            Tipo = TipoCasilla.Vacia;
            EstaOcupada = false;
        }
    }

    public class Tablero
    {
        private Casilla[,] casillas;
        public int Tamaño { get; private set; }
        private Random random;

        // Puntos de inicio para los jugadores
        private readonly (int x, int y)[] puntosIniciales;

        public Tablero(int tamaño)
        {
            Tamaño = tamaño;
            casillas = new Casilla[tamaño, tamaño];
            random = new Random();

            // Definir puntos iniciales (esquinas opuestas)
            puntosIniciales = new[] {
                (0, 0),                          // Esquina superior izquierda
                (tamaño - 1, tamaño - 1)         // Esquina inferior derecha
            };

            while (!InicializarTableroSeguro())
            {
                // Seguir intentando hasta conseguir un tablero válido
                Console.WriteLine("Reintentando generar tablero válido...");
            }
        }

        private bool InicializarTableroSeguro()
        {
            // Crear casillas vacías
            for (int i = 0; i < Tamaño; i++)
            {
                for (int j = 0; j < Tamaño; j++)
                {
                    casillas[i, j] = new Casilla(i, j);
                }
            }

            // Mantener los puntos iniciales vacíos
            foreach (var punto in puntosIniciales)
            {
                casillas[punto.x, punto.y].Tipo = TipoCasilla.Vacia;
            }

            // Generar obstáculos (15% del tablero para dar más espacio)
            int numObstaculos = (int)(Tamaño * Tamaño * 0.15);
            if (!GenerarElementosSeguro(TipoCasilla.Obstaculo, numObstaculos))
                return false;

            // Generar trampas (10% del tablero)
            int numTrampas = (int)(Tamaño * Tamaño * 0.10);
            if (!GenerarElementosSeguro(TipoCasilla.Trampa1, numTrampas / 3) ||
                !GenerarElementosSeguro(TipoCasilla.Trampa2, numTrampas / 3) ||
                !GenerarElementosSeguro(TipoCasilla.Trampa3, numTrampas / 3))
                return false;

            // Verificar accesibilidad desde todos los puntos iniciales
            return VerificarAccesibilidadTotal();
        }

        private bool GenerarElementosSeguro(TipoCasilla tipo, int cantidad)
        {
            int intentos = 0;
            int elementosColocados = 0;
            int maxIntentos = Tamaño * Tamaño * 2; // Límite de intentos para evitar bucles infinitos

            while (elementosColocados < cantidad && intentos < maxIntentos)
            {
                int x = random.Next(Tamaño);
                int y = random.Next(Tamaño);

                // Verificar que no sea un punto inicial
                if (puntosIniciales.Any(p => p.x == x && p.y == y))
                {
                    intentos++;
                    continue;
                }

                // Verificar que la casilla esté vacía
                if (casillas[x, y].Tipo == TipoCasilla.Vacia)
                {
                    casillas[x, y].Tipo = tipo;
                    elementosColocados++;
                }
                intentos++;
            }

            return elementosColocados == cantidad;
        }

        private bool VerificarAccesibilidadTotal()
        {
            // Verificar accesibilidad desde cada punto inicial
            foreach (var puntoInicial in puntosIniciales)
            {
                bool[,] visitado = new bool[Tamaño, Tamaño];
                DFS(puntoInicial.x, puntoInicial.y, visitado);

                // Verificar que todos los puntos iniciales son accesibles entre sí
                foreach (var otroPunto in puntosIniciales)
                {
                    if (!visitado[otroPunto.x, otroPunto.y])
                        return false;
                }

                // Verificar que hay suficientes casillas accesibles
                int casillasAccesibles = 0;
                for (int i = 0; i < Tamaño; i++)
                {
                    for (int j = 0; j < Tamaño; j++)
                    {
                        if (visitado[i, j]) casillasAccesibles++;
                    }
                }

                // Asegurar que al menos el 60% del tablero es accesible
                if (casillasAccesibles < (Tamaño * Tamaño * 0.6))
                    return false;
            }

            return true;
        }

        private void DFS(int x, int y, bool[,] visitado)
        {
            if (x < 0 || x >= Tamaño || y < 0 || y >= Tamaño || 
                visitado[x, y] || casillas[x, y].Tipo == TipoCasilla.Obstaculo)
                return;

            visitado[x, y] = true;

            // Verificar las 4 direcciones
            DFS(x + 1, y, visitado);
            DFS(x - 1, y, visitado);
            DFS(x, y + 1, visitado);
            DFS(x, y - 1, visitado);
        }

        public void ImprimirTablero()
        {
            for (int i = 0; i < Tamaño; i++)
            {
                for (int j = 0; j < Tamaño; j++)
                {
                    switch (casillas[i, j].Tipo)
                    {
                        case TipoCasilla.Vacia:
                            Console.Write("· ");
                            break;
                        case TipoCasilla.Obstaculo:
                            Console.Write("█ ");
                            break;
                        case TipoCasilla.Trampa1:
                            Console.Write("1 ");
                            break;
                        case TipoCasilla.Trampa2:
                            Console.Write("2 ");
                            break;
                        case TipoCasilla.Trampa3:
                            Console.Write("3 ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        public bool EsMovimientoValido(int x, int y)
        {
            return x >= 0 && x < Tamaño && y >= 0 && y < Tamaño && 
                   casillas[x, y].Tipo != TipoCasilla.Obstaculo;
        }

        public Casilla ObtenerCasilla(int x, int y)
        {
            return casillas[x, y];
        }
    }


   
    public class GestorTurnos
{
    private Tablero tablero;
    private List<Ficha> fichas;
    private int jugadorActual;
    private bool juegoActivo;
    private Dictionary<Ficha, (int, int)> posicionesIniciales;
    private int[] contadorTrampas3;
    private int turnoInvalidado;

    public GestorTurnos(Tablero tablero)
    {
        this.tablero = tablero;
        fichas = new List<Ficha>();
        jugadorActual = 0;
        juegoActivo = true;
        posicionesIniciales = new Dictionary<Ficha, (int, int)>();
        contadorTrampas3 = new int[2]; // Contador de trampas nivel 3 para cada jugador
        turnoInvalidado = -1; 
        InicializarFichas();
    }

    private void InicializarFichas()
    {
        // Crear fichas con diferentes características
        fichas.Add(new Guerrero());
        fichas.Add(new Arquero());
        // fichas.Add(new Picaro());
        // fichas.Add(new Sacerdote());
        // fichas.Add(new CaballeroDeLaMuerte());

        // Asignar posiciones iniciales
        for (int i = 0; i < fichas.Count; i++)
        {
            if (i % 2 == 0)
            {
                fichas[i].PosX = 0;
                fichas[i].PosY = 0;
                posicionesIniciales[fichas[i]] = (0, 0);
            }
            else
            {
                fichas[i].PosX = tablero.Tamaño - 1;
                fichas[i].PosY = tablero.Tamaño - 1;
                posicionesIniciales[fichas[i]] = (tablero.Tamaño - 1, tablero.Tamaño - 1);
            }
            tablero.ObtenerCasilla(fichas[i].PosX, fichas[i].PosY).EstaOcupada = true;
        }
    }

    

    public void IniciarJuego()
    {
        while (juegoActivo)
        {
            EjecutarTurno();
            VerificarVictoria();
        }
    }

    private void EjecutarTurno()
    {
        Console.Clear();
        ImprimirEstadoJuego();

        Ficha fichaActual = fichas[jugadorActual];

        // Verificar si el jugador está invalidado
        if (TrampaManager.EstaJugadorInvalidado(fichaActual))
        {
            Console.WriteLine($"\n{fichaActual.Nombre} está invalidado este turno debido a una trampa!");
            Console.ReadKey(true);
            FinalizarTurno();
            return;
        }

        Console.WriteLine($"\nTurno del {fichaActual.Nombre}");
        Console.WriteLine("Usa las flechas para mover");
        Console.WriteLine("H: Usar Habilidad(consume todos los movimientos)");
        Console.WriteLine("F: Finalizar Turno");
        Console.WriteLine("Q: Salir del Juego");

        int movimientosRestantes = fichaActual.Velocidad;
        bool turnoFinalizado = false;

        while (!turnoFinalizado && movimientosRestantes > 0)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    if (RealizarMovimiento(fichaActual, tecla.Key))
                    {
                        movimientosRestantes--;
                        // Verificar trampa después del movimiento
                        VerificarYActivarTrampa(fichaActual);
                    }
                    break;

                case ConsoleKey.H:
                    if (fichaActual.UsarHabilidad(tablero))
                    {
                        movimientosRestantes = 0;
                        turnoFinalizado = true;
                        Console.WriteLine("\nHabilidad usada. El turno termina automáticamente.");
                        Console.ReadKey(true);
                    }
                    break;

                case ConsoleKey.F:
                    turnoFinalizado = true;
                    break;

                case ConsoleKey.Q:
                    juegoActivo = false;
                    turnoFinalizado = true;
                    break;
            }

            Console.Clear();
            ImprimirEstadoJuego();
            Console.WriteLine($"\nMovimientos restantes: {movimientosRestantes}");
        }

        if (movimientosRestantes == 0)
        {
            Console.WriteLine("\nSe agotaron los movimientos del turno.");
            Console.ReadKey(true);
        }

        FinalizarTurno();
    }

    private void VerificarYActivarTrampa(Ficha ficha)
    {
        Casilla casillaActual = tablero.ObtenerCasilla(ficha.PosX, ficha.PosY);
        int jugadorIndex = jugadorActual % 2; // Convertir a índice 0 o 1

        switch (casillaActual.Tipo)
        {
            case TipoCasilla.Trampa1:
                turnoInvalidado = jugadorIndex;
                Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 1. ¡Pierde el siguiente turno!");
                casillaActual.Tipo = TipoCasilla.Vacia;
                break;

            case TipoCasilla.Trampa2:
                // Volver al inicio
                tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;
                var posInicial = posicionesIniciales[ficha];
                ficha.PosX = posInicial.Item1;
                ficha.PosY = posInicial.Item2;
                tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = true;
                Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 2. ¡Regresa al inicio!");
                casillaActual.Tipo = TipoCasilla.Vacia;
                break;

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
                    contadorTrampas3[jugadorIndex]++;
                    Console.WriteLine($"{ficha.Nombre} ha caído en una trampa de nivel 3. ¡Se teletransporta a otra trampa!");
                }
                break;
        }
    }

    private bool RealizarMovimiento(Ficha ficha, ConsoleKey direccion)
    {
        int newX = ficha.PosX;
        int newY = ficha.PosY;

        switch (direccion)
        {
            case ConsoleKey.UpArrow:
                newX--;
                break;
            case ConsoleKey.DownArrow:
                newX++;
                break;
            case ConsoleKey.LeftArrow:
                newY--;
                break;
            case ConsoleKey.RightArrow:
                newY++;
                break;
        }

        if (tablero.EsMovimientoValido(newX, newY) && !tablero.ObtenerCasilla(newX, newY).EstaOcupada)
        {
            tablero.ObtenerCasilla(ficha.PosX, ficha.PosY).EstaOcupada = false;
            ficha.PosX = newX;
            ficha.PosY = newY;
            tablero.ObtenerCasilla(newX, newY).EstaOcupada = true;
            return true;
        }

        return false;
    }

    private void VerificarVictoria()
    {
        Ficha fichaActual = fichas[jugadorActual];
        
        // Verificar victoria por llegar a la base enemiga
        var posicionInicial = posicionesIniciales[fichaActual];
        var posicionObjetivo = posicionInicial.Item1 == 0 ? 
            (tablero.Tamaño - 1, tablero.Tamaño - 1) : (0, 0);

        if (fichaActual.PosX == posicionObjetivo.Item1 && 
            fichaActual.PosY == posicionObjetivo.Item2)
        {
            Console.Clear();
            Console.WriteLine($"¡{fichaActual.Nombre} ha ganado por capturar la base enemiga!");
            juegoActivo = false;
            return;
        }

        // Victoria por caer en 3 trampas nivel 3
        if (contadorTrampas3[jugadorActual] >= 3)
        {
            Console.Clear();
            Console.WriteLine($"¡{fichaActual.Nombre} ha perdido por caer en demasiadas trampas nivel 3!");
            Console.WriteLine($"¡El jugador {(jugadorActual + 1) % 2} es el ganador!");
            juegoActivo = false;
            return;
        }

        // Victoria por dominación de territorio
        int casillasControladas = ContarCasillasControladas(fichaActual);
        int casillasLibresTotales = ContarCasillasLibres();
        if (casillasControladas >= casillasLibresTotales * 0.7)
        {
            Console.Clear();
            Console.WriteLine($"¡{fichaActual.Nombre} ha ganado por dominación de territorio!");
            juegoActivo = false;
            return;
        }
    }

    private int ContarCasillasControladas(Ficha ficha)
    {
        int contador = 0;
        bool[,] controladas = new bool[tablero.Tamaño, tablero.Tamaño];

        // Contar casilla actual
        controladas[ficha.PosX, ficha.PosY] = true;
        contador++;

        // Contar casillas adyacentes
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                // Contar trampas como territorio controlado
                var casilla = tablero.ObtenerCasilla(i, j);
                if ((casilla.Tipo == TipoCasilla.Trampa1 || 
                     casilla.Tipo == TipoCasilla.Trampa2 || 
                     casilla.Tipo == TipoCasilla.Trampa3) && 
                    !controladas[i, j])
                {
                    controladas[i, j] = true;
                    contador++;
                }

                // Contar casillas a un movimiento de distancia
                if (Math.Abs(i - ficha.PosX) + Math.Abs(j - ficha.PosY) == 1 && 
                    !controladas[i, j] && 
                    tablero.EsMovimientoValido(i, j))
                {
                    controladas[i, j] = true;
                    contador++;
                }
            }
        }

        return contador;
    }

    private int ContarCasillasLibres()
    {
        int contador = 0;
        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                if (tablero.ObtenerCasilla(i, j).Tipo != TipoCasilla.Obstaculo)
                {
                    contador++;
                }
            }
        }
        return contador;
    }

    private void FinalizarTurno()
    {
        if (fichas[jugadorActual].EnfriamientoRestante > 0)
            fichas[jugadorActual].EnfriamientoRestante--;

        TrampaManager.ActualizarEstadoInvalidaciones();
        jugadorActual = (jugadorActual + 1) % fichas.Count;
    }

    private void ImprimirEstadoJuego()
    {
        char[,] visualizacion = new char[tablero.Tamaño, tablero.Tamaño];

        // Llenar con el estado base del tablero
        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                Casilla casilla = tablero.ObtenerCasilla(i, j);
                switch (casilla.Tipo)
                {
                    case TipoCasilla.Vacia:
                        visualizacion[i, j] = '·';
                        break;
                    case TipoCasilla.Obstaculo:
                        visualizacion[i, j] = '█';
                        break;
                    case TipoCasilla.Trampa1:
                        visualizacion[i, j] = '1';
                        break;
                    case TipoCasilla.Trampa2:
                        visualizacion[i, j] = '2';
                        break;
                    case TipoCasilla.Trampa3:
                        visualizacion[i, j] = '3';
                        break;
                }
            }
        }

        // Agregar las fichas
        foreach (var ficha in fichas)
        {
            visualizacion[ficha.PosX, ficha.PosY] = ficha.Simbolo;
        }

        // Imprimir el tablero
        for (int i = 0; i < tablero.Tamaño; i++)
        {
            for (int j = 0; j < tablero.Tamaño; j++)
            {
                Console.Write(visualizacion[i, j] + " ");
            }
            Console.WriteLine();
        }

        // Imprimir información de las fichas
        Console.WriteLine("\nEstado de las fichas:");
        foreach (var ficha in fichas)
        {
            string estadoInvalidacion = TrampaManager.EstaJugadorInvalidado(ficha) ? " [INVALIDADO]" : "";
            Console.WriteLine($"{ficha.Nombre} ({ficha.Simbolo}): Pos({ficha.PosX},{ficha.PosY}) " +
                            $"- Velocidad: {ficha.Velocidad} " +
                            $"- Enfriamiento: {ficha.EnfriamientoRestante}" +
                            estadoInvalidacion);
        }

        // Mostrar contadores de trampas nivel 3
        Console.WriteLine("\nTrampas nivel 3:");
        Console.WriteLine($"Jugador 1: {contadorTrampas3[0]}/3");
        Console.WriteLine($"Jugador 2: {contadorTrampas3[1]}/3");
    }
}