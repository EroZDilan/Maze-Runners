public abstract class Ficha
{
        public string Nombre { get; protected set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PosicionInicial { get; protected set; }
        public int Velocidad { get; protected set; }
        public int TiempoEnfriamiento { get; protected set; }
        public int EnfriamientoRestante { get; set; }
        public char Simbolo { get; protected set; }
        public string SimboloEmoji{get; protected set;}

        public Ficha(string nombre, int velocidad, int tiempoEnfriamiento, char simbolo, string simboloEmoji)
        {
            Nombre = nombre;
            Velocidad = velocidad;
            TiempoEnfriamiento = tiempoEnfriamiento;
            EnfriamientoRestante = 0;
            Simbolo = simbolo;
            SimboloEmoji = simboloEmoji;
        }

        public abstract bool UsarHabilidad(Tablero tablero);
    }

     public class Guerrero : Ficha
    {
        public Guerrero() : base("Guerrero", 3, 2, 'G',"⚔️")
        {
        }

        public override bool UsarHabilidad(Tablero tablero)
        {
            if (EnfriamientoRestante > 0)
            {
                Console.WriteLine($"Habilidad en enfriamiento. Turnos restantes: {EnfriamientoRestante}");
                return false;
            }

            // Buscar trampas tipo 1 y 2 en casillas adyacentes
            bool trampaEncontrada = false;
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int i = 0; i < 4; i++)
            {
                int newX = PosX + dx[i];
                int newY = PosY + dy[i];

                if (newX >= 0 && newX < tablero.Tamaño && newY >= 0 && newY < tablero.Tamaño)
                {
                    Casilla casilla = tablero.ObtenerCasilla(newX, newY);
                    if (casilla.Tipo == TipoCasilla.Trampa1 || casilla.Tipo == TipoCasilla.Trampa2)
                    {
                        casilla.Tipo = TipoCasilla.Vacia;
                        trampaEncontrada = true;
                    }
                }
            }

            if (trampaEncontrada)
            {
                Console.WriteLine($"{Nombre} rompe las trampas adyacentes!");
                EnfriamientoRestante = TiempoEnfriamiento;
                return true;
            }
            else
            {
                Console.WriteLine("No hay trampas adyacentes para romper.");
                return false;
            }
        }
    }

        public class Arquero : Ficha
    {
        public Arquero() : base("Arquero", 4, 3, 'A',"🏹")
        {
        }

        public override bool UsarHabilidad(Tablero tablero)
        {
            if (EnfriamientoRestante > 0)
            {
                Console.WriteLine($"Habilidad en enfriamiento. Turnos restantes: {EnfriamientoRestante}");
                return false;
            }

            Console.WriteLine("Ingrese las coordenadas para teletransportarse:");
            Console.Write("X: ");
            if (!int.TryParse(Console.ReadLine(), out int newX))
                return false;

            Console.Write("Y: ");
            if (!int.TryParse(Console.ReadLine(), out int newY))
                return false;

            // Verificar si la posición es válida
            if (tablero.EsMovimientoValido(newX, newY) && !tablero.ObtenerCasilla(newX, newY).EstaOcupada)
            {
                // Desocupar la casilla actual
                tablero.ObtenerCasilla(PosX, PosY).EstaOcupada = false;

                // Actualizar posición
                PosX = newX;
                PosY = newY;

                // Ocupar nueva casilla
                tablero.ObtenerCasilla(newX, newY).EstaOcupada = true;

                Console.WriteLine($"{Nombre} se teletransporta a la posición ({newX}, {newY})!");
                EnfriamientoRestante = TiempoEnfriamiento;
                return true;
            }
            else
            {
                Console.WriteLine("Posición no válida para teletransporte.");
                return false;
            }
        }
    }

      public class Sacerdote : Ficha
    {
        public Sacerdote() : base("Sacerdote", 3, 3, 'S',"✨") { }

        public override bool UsarHabilidad(Tablero tablero)
        {
            // Mismo comportamiento que el Arquero
            if (EnfriamientoRestante > 0)
            {
                Console.WriteLine($"Habilidad en enfriamiento. Turnos restantes: {EnfriamientoRestante}");
                return false;
            }

            // (int, int) coordenada = Utiles.ValidarCoordenada();
            // int newX = coordenada.Item1;
            // int newY = coordenada.Item2;
            Console.WriteLine("Ingrese las coordenadas para teletransportarse:");
            Console.Write("X: ");
            if (!int.TryParse(Console.ReadLine(), out int newX))
              return false;
            
            
            Console.Write("Y: ");
            if (!int.TryParse(Console.ReadLine(), out int newY))
                return false;

            if (tablero.EsMovimientoValido(newX, newY) && !tablero.ObtenerCasilla(newX, newY).EstaOcupada)
            {
                tablero.ObtenerCasilla(PosX, PosY).EstaOcupada = false;
                PosX = newX;
                PosY = newY;
                tablero.ObtenerCasilla(newX, newY).EstaOcupada = true;

                Console.WriteLine($"{Nombre} se teletransporta a la posición ({newX}, {newY})!");
                EnfriamientoRestante = TiempoEnfriamiento;
                return true;
            }

            Console.WriteLine("Posición no válida para teletransporte.");
            return false;
        }
    }
    public class Picaro : Ficha
    {
        private bool trampaOcultaActiva = false;
        private (int x, int y) posicionTrampaOculta;

        public Picaro() : base("Pícaro", 3, 2, 'P',"🗡️") { }

        public override bool UsarHabilidad(Tablero tablero)
        {
            if (EnfriamientoRestante > 0)
            {
                Console.WriteLine($"Habilidad en enfriamiento. Turnos restantes: {EnfriamientoRestante}");
                return false;
            }

        
            Console.WriteLine("Selecciona la habilidad a usar");
            Console.WriteLine ("1. Mejorar tramapa");
            Console.WriteLine("2. Colocar trampa oculta");

            if(!int.TryParse(Console.ReadLine(), out int opcion) ||(opcion != 1 && opcion != 2))
            {
                Console.WriteLine("Opcion no valida");
                return false;
            }

            if(opcion == 1) return MejorarTrampa(tablero);
            else  return ColocarTrampaOculta(tablero); 
        }

        public bool MejorarTrampa(Tablero tablero)
        {
            
            Console.WriteLine("Ingrese las coordenadas de la trampa a mejorar:");
            Console.Write("X: ");
            if (!int.TryParse(Console.ReadLine(), out int x))
                return false;

            Console.Write("Y: ");
            if (!int.TryParse(Console.ReadLine(), out int y))
                return false;

            if (x >= 0 && x < tablero.Tamaño && y >= 0 && y < tablero.Tamaño)
            {
                Casilla casilla = tablero.ObtenerCasilla(x, y);
                if (casilla.Tipo == TipoCasilla.Trampa1)
                {
                    casilla.Tipo = TipoCasilla.Trampa2;
                    Console.WriteLine("¡Trampa mejorada de nivel 1 a nivel 2!");
                    EnfriamientoRestante = TiempoEnfriamiento;
                    return true;
                }
                else if (casilla.Tipo == TipoCasilla.Trampa2)
                {
                    casilla.Tipo = TipoCasilla.Trampa3;
                    Console.WriteLine("¡Trampa mejorada de nivel 2 a nivel 3!");
                    EnfriamientoRestante = TiempoEnfriamiento;
                    return true;
                }
            }
            
            Console.WriteLine("No hay una trampa válida para mejorar en esa posición.");
            return false;
        }

        private bool ColocarTrampaOculta(Tablero tablero)
        {
            if(trampaOcultaActiva)
            {
                tablero.ObtenerCasilla(posicionTrampaOculta.x,posicionTrampaOculta.y).Tipo = TipoCasilla.Vacia;
            }

            Console.WriteLine("Ingrese las coordenadas para colocar la trampa oculta");
            Console.Write("X: ");
            if(!int.TryParse(Console.ReadLine(), out int x))return false;

            if(!int.TryParse(Console.ReadLine(), out int y)) return false;

            if(x >= 0 && x< tablero.Tamaño && y >= 0 && y< tablero.Tamaño)
            {
                Casilla casilla = tablero.ObtenerCasilla(x,y);
                if(casilla.Tipo == TipoCasilla.Vacia && !casilla.EstaOcupada)
                {
                casilla.Tipo = TipoCasilla.Trampa1;
                trampaOcultaActiva = true;
                posicionTrampaOculta = (x, y);
                Console.WriteLine("¡Trampa oculta colocada con éxito!");
                EnfriamientoRestante = TiempoEnfriamiento;
                return true;
                }
            }

            Console.WriteLine("No se puede colocar la trampa en esa posición");
            return false;
        }
    }

        public class CaballeroDeLaMuerte : Ficha
    {
        public CaballeroDeLaMuerte() : base("Caballero de la Muerte", 2, 4, 'C',"💀") { }

        public override bool UsarHabilidad(Tablero tablero)
        {
            if (EnfriamientoRestante > 0)
            {
                Console.WriteLine($"Habilidad en enfriamiento. Turnos restantes: {EnfriamientoRestante}");
                return false;
            }

            Console.WriteLine("Ingrese las coordenadas para atravesar obstáculos:");
            Console.Write("X: ");
            if (!int.TryParse(Console.ReadLine(), out int newX))
                return false;

            Console.Write("Y: ");
            if (!int.TryParse(Console.ReadLine(), out int newY))
                return false;

            // Verificar que está dentro del tablero y la casilla no está ocupada
            if (newX >= 0 && newX < tablero.Tamaño && newY >= 0 && newY < tablero.Tamaño && 
                !tablero.ObtenerCasilla(newX, newY).EstaOcupada)
            {
                tablero.ObtenerCasilla(PosX, PosY).EstaOcupada = false;
                PosX = newX;
                PosY = newY;
                tablero.ObtenerCasilla(newX, newY).EstaOcupada = true;

                Console.WriteLine($"{Nombre} atraviesa los obstáculos hasta la posición ({newX}, {newY})!");
                EnfriamientoRestante = TiempoEnfriamiento;
                return true;
            }

            Console.WriteLine("Posición no válida o casilla ocupada.");
            return false;
        }
    }