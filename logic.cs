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

            // Generar obstáculos (20% del tablero para dar más espacio)
            int numObstaculos = (int)(Tamaño * Tamaño * 0.20);
            if (!GenerarElementosSeguro(TipoCasilla.Obstaculo, numObstaculos))
                return false;

            // Generar trampas (10% del tablero)
            int numTrampas = (int)(Tamaño * Tamaño * 0.10);
            if (!GenerarElementosSeguro(TipoCasilla.Trampa1, numTrampas / 3) ||
                !GenerarElementosSeguro(TipoCasilla.Trampa2, numTrampas / 3) ||
                !GenerarElementosSeguro(TipoCasilla.Trampa3, numTrampas / 2))
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
    public int jugadorActual;
    private bool juegoActivo;
    public readonly Dictionary<Ficha, (int, int)> posicionesIniciales;
    public VictoryManager victoryManager;
    private int turnoInvalidado;

    public GestorTurnos(Tablero tablero)
    {
        this.tablero = tablero;
        fichas = new List<Ficha>();
        jugadorActual = 0;
        juegoActivo = true;
        posicionesIniciales = new Dictionary<Ficha, (int, int)>();
        victoryManager = new VictoryManager(tablero);
        turnoInvalidado = -1; 
        var selector = new SelectorPersonajes();
        fichas = selector.SeleccionarPersonajes();

        InicializarPosiciones();
    }

    private void InicializarPosiciones()
    {
        for (int i = 0; i < fichas.Count; i++)
        {
            if(i % 2 == 0)
            {
                fichas[i].PosX = 0;
                fichas[i].PosY = 0;
                posicionesIniciales[fichas[i]] = (0,0);

            }

            else
            {
                fichas[i].PosX = tablero.Tamaño -1;
                fichas[i].PosY = tablero.Tamaño -1;
                posicionesIniciales[fichas[i]]=(tablero.Tamaño -1, tablero.Tamaño-1);
            }
            tablero.ObtenerCasilla(fichas[i].PosX, fichas[i].PosY).EstaOcupada = true;
        }
    }

    public void IniciarJuego()
    {
        while (juegoActivo)
        {
            EjecutarTurno();
            //VerificarVictoria();
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
            System.Console.WriteLine("aveadaddadadada");
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
                        //VerificarYActivarTrampa(fichaActual);
                        Casilla casillaActual = tablero.ObtenerCasilla(fichaActual.PosX, fichaActual.PosY);
                        if(!(casillaActual.Tipo is TipoCasilla.Vacia))
                        {
                            TrampaManager.ManejarTrampa(fichaActual,casillaActual,tablero, this);
                            if(TrampaManager.EstaJugadorInvalidado(fichaActual))FinalizarTurno();
                        }
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

        VerificarVictoria();

        FinalizarTurno();
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

            if(victoryManager.IntentarCapturarBandera(ficha,fichas))
            {
                Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey(true);
            }
            
            Casilla casillaActual = tablero.ObtenerCasilla(ficha.PosX, ficha.PosY);
            if(!(casillaActual.Tipo is TipoCasilla.Vacia))
            {
                int resultado = TrampaManager.ManejarTrampa(ficha,casillaActual,tablero,this);
                if(resultado == 0) return false;
            }

            return true;
        }

        return false;
    }

    public void FinalizarTurnoInmediato()
    {
        if(fichas[jugadorActual].EnfriamientoRestante>0)fichas[jugadorActual].EnfriamientoRestante--;

        TrampaManager.ActualizarEstadoInvalidaciones();
        jugadorActual = (jugadorActual + 1)%fichas.Count;
    }

    private void VerificarVictoria()
    {
        if(victoryManager.VerificarVictoria(fichas[jugadorActual],posicionesIniciales))
        {
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
        
        victoryManager.DibujarBandera(visualizacion);

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
    }
}