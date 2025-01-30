public class SelectorPersonajes
{
    private List<Type> personajesDisponibles;

    public SelectorPersonajes()
    {
        personajesDisponibles = new List<Type>
        {
            typeof(Guerrero),
            typeof(Arquero),
            typeof(Sacerdote),
            typeof(Picaro),
            typeof(CaballeroDeLaMuerte)
        };
    }

    public int SeleccionarTamañoTablero()
    {
        Console.Clear();
        Console.WriteLine("Configuracion del tablero de juego");
        Console.WriteLine("--------------------------------------");
        Console.WriteLine("El tamaño mínimo recomendado es 6x6 y el maximo es de 36x36");
        int tamaño;

        do
        {
            Console.Write("Ingrese el tamaño del tablero :");
            while(!int.TryParse(Console.ReadLine(), out tamaño))
            {
                Console.Write("Ingresa un número válido");
            }
            if( tamaño < 6||tamaño>12)Console.WriteLine("El tamaño debe estar entre 6 y 12");
        }while (tamaño<6|| tamaño>36);

        Console.WriteLine($"Tablero configurado con dimensiones {tamaño}x{tamaño}");
        Console.WriteLine("Presione cualquier tecla para continuar ...");
        Console.ReadKey();
        return tamaño;
    }

    public List<Ficha> SeleccionarPersonajes()
    {
        List<Ficha> personajesSeleccionados = new List<Ficha>();

        for (int jugador = 1; jugador <= 2; jugador++)
        {
            Console.Clear();
            Console.WriteLine($"Jugador {jugador}, selecciona tu personaje:");
            Console.WriteLine("\nPersonajes disponibles:");
            
            for (int i = 0; i < personajesDisponibles.Count; i++)
            {
                var tipo = personajesDisponibles[i];
                // Crear una instancia temporal para obtener información
                var tempFicha = (Ficha)Activator.CreateInstance(tipo);
                Console.WriteLine($"{i + 1}. {tempFicha.Nombre} (Velocidad: {tempFicha.Velocidad}, Enfriamiento: {tempFicha.TiempoEnfriamiento})");
            }

            int seleccion;
            do
            {
                Console.Write("\nIngresa el número de tu elección (1-5): ");
            } while (!int.TryParse(Console.ReadLine(), out seleccion) || 
                    seleccion < 1 || 
                    seleccion > personajesDisponibles.Count);

            // Crear la instancia del personaje seleccionado
            var personajeSeleccionado = (Ficha)Activator.CreateInstance(personajesDisponibles[seleccion - 1]);
            personajesSeleccionados.Add(personajeSeleccionado);

            Console.WriteLine($"\n¡Has seleccionado a {personajeSeleccionado.Nombre}!");
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }

        return personajesSeleccionados;
    }
}