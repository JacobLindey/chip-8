namespace JEL.Chip8.CPU;

public partial class Chip8
{
    private readonly Random _random;
    
    public byte RandByte
    {
        get
        {
            var buffer = new byte[1];
            _random.NextBytes(buffer);
            return buffer[0];
        }
    }
}