public class NarradorHistoria
{
    private readonly string[] historia = {
        "En las tierras místicas de Ethoria, donde la magia y el honor se entrelazan,",
        "surgió una antigua tradición para resolver los conflictos entre los reinos:",
        "La Batalla por la Bandera Sagrada.",
        "",
        "Se dice que esta bandera fue tejida con hilos del destino por los primeros",
        "hechiceros del reino, dotándola de un poder capaz de otorgar la victoria",
        "definitiva a quien logre conquistarla y llevarla a su territorio.",
        "",
        "Dos facciones se enfrentan en este ritual de combate:",
        "Los Guardianes del Alba, protectores de las tierras del este,",
        "y los Vigilantes del Ocaso, defensores de los territorios occidentales.",
        "",
        "Cada bando envía a su campeón más valiente para recuperar la bandera:",
        "- El poderoso Guerrero, maestro del combate cercano",
        "- El ágil Arquero, experto en movimientos tácticos",
        "- El místico Sacerdote, bendecido con poderes antiguos",
        "- El astuto Pícaro, maestro de las trampas y el engaño",
        "- El temible Caballero de la Muerte, controlador de las sombras",
        "",
        "El campo de batalla está sembrado de trampas ancestrales y obstáculos mágicos,",
        "¡que la batalla comience y que el mejor estratega triunfe!"
    };

    public void MostrarHistoria(bool soloHistoria = false)
    {
        Console.Clear();
        Console.WriteLine("\n\n");
        
        foreach (string linea in historia)
        {
            Console.WriteLine($"    {linea}");
            Thread.Sleep(150); // Pausa dramática entre líneas
        }

        Console.WriteLine("\n\n    Presiona cualquier tecla para continuar...");
        Console.ReadKey(true);

        if (!soloHistoria)
        {
            var selector = new SelectorPersonajes();
            int tamañoTablero = selector.SeleccionarTamañoTablero();
            Tablero tablero = new Tablero(tamañoTablero);
            GestorTurnos gestor = new GestorTurnos(tablero);
            gestor.IniciarJuego();
        }
    }
}