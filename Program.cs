class Program
{
    static void Main(string[] args)
    {
        Tablero tablero = new Tablero(8);
        GestorTurnos gestor = new GestorTurnos(tablero);
        gestor.IniciarJuego();
    }
}
