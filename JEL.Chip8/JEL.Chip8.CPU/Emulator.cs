using JetBrains.Annotations;

namespace JEL.Chip8.CPU;

[PublicAPI]
public static class Emulator
{
    public static int Run(IPlatform platform, int cycleDelay, string romFilename)
    {
        var chip8 = new Chip8();
        chip8.LoadROM(romFilename);

        const int videoPitch = sizeof(uint) * Chip8.VideoWidth;
        
        var lastCycleTime = DateTime.Now;
        var quit = false;

        while (!quit)
        {
            quit = platform.ProcessInput(chip8.Keypad);
            var currentTime = DateTime.Now;
            var dt = (currentTime - lastCycleTime).Milliseconds;

            if (dt > cycleDelay)
            {
                lastCycleTime = currentTime;

                chip8.Cycle();

                platform.Update(chip8.Video, videoPitch);
            }
        }

        return 0;
    }
}