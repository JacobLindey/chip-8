namespace JEL.Chip8.CPU;

public interface IPlatform
{
    public void Update(uint[] buffer, uint pitch);

    public bool ProcessInput(bool[] keys);
}