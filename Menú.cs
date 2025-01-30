public class MenuManager
{
    private const string TituloJuego = @"
    ╔═══════════════════════════════════════════╗
    ║             BATALLA POR LA BANDERA        ║
    ╚═══════════════════════════════════════════╝";

    private readonly string[] OpcionesMenu = {
        "Iniciar Partida",
        "Historia del Juego",
        "Salir"
    };

    private int opcionSeleccionada = 0;

    public void MostrarMenu()
    {
        bool menuActivo = true;
        while (menuActivo)
        {
            Console.Clear();
            Console.WriteLine(TituloJuego);
            Console.WriteLine("\n\n");

            for (int i = 0; i < OpcionesMenu.Length; i++)
            {
                if (i == opcionSeleccionada)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"    > {OpcionesMenu[i]} <    ");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"      {OpcionesMenu[i]}      ");
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    opcionSeleccionada = (opcionSeleccionada - 1 + OpcionesMenu.Length) % OpcionesMenu.Length;
                    break;
                case ConsoleKey.DownArrow:
                    opcionSeleccionada = (opcionSeleccionada + 1) % OpcionesMenu.Length;
                    break;
                case ConsoleKey.Enter:
                    switch (opcionSeleccionada)
                    {
                        case 0: // Iniciar Partida
                            new NarradorHistoria().MostrarHistoria();
                            return;
                        case 1: // Historia del Juego
                            new NarradorHistoria().MostrarHistoria(soloHistoria: true);
                            break;
                        case 2: // Salir
                            Environment.Exit(0);
                            break;
                    }
                    break;
            }
        }
    }
}