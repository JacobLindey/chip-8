using System.Text;

namespace JEL.Chip8.CPU;

public class ConsolePlatform : IPlatform
{
    public ConsolePlatform(string title)
    {
        Console.Title = title;
        Console.WindowWidth = Chip8.VideoWidth;
        Console.WindowHeight = Chip8.VideoHeight;
    }
    
    public void Update(uint[] buffer, uint pitch)
    {
        Console.SetCursorPosition(0, 0);
        var sb = new StringBuilder();
        for (var y = 0; y < Chip8.VideoHeight; y++)
        {
            for (var x = 0; x < Chip8.VideoWidth; x++)
            {
                var index = y * Chip8.VideoWidth + x;
                sb.Append(buffer[index] > 0 ? '█' : ' ');
            }

            sb.Append('\n');
        }
        Console.WriteLine(sb.ToString());
    }

    public bool ProcessInput(bool[] keys)
    {
        // TODO: add input handling
        return false;
    }
}

public class InputHandler
{
    public InputHandler()
    {
        
    }
}