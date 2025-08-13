namespace JEL.Chip8.CPU;

public static class Program
{
    public static void Main()
    {
        var platform = new ConsolePlatform("CHIP-8 Emulator");
        Emulator.Run(platform, 4, "tetris.ch8");
    }
}