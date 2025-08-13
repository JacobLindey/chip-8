namespace JEL.Chip8.CPU;

public partial class Chip8
{
    private void Table0()
    {
        _opTable0[GetOpcodeNibble3()]();
    }

    private void Table8()
    {
        _opTable8[GetOpcodeNibble3()]();
    }

    private void TableE()
    {
        _opTableE[GetOpcodeNibble3()]();
    }

    private void TableF()
    {
        _opTableF[GetOpcodeByte1()]();
    }

    private void Op_NULL()
    {
        // intentionally blank
    }
    
    /// <summary>
    /// Clear the display.
    /// </summary>
    private void OP_00E0()
    {
        for (var i = 0; i < VideoBufferLength; i++)
        {
            Video[i] = 0;
        }
    }

    /// <summary>
    /// Return from a subroutine
    /// </summary>
    private void OP_00EE()
    {
        StackPointer--;
        ProgramCounter = Stack[StackPointer];
    }

    /// <summary>
    /// Jump to location nnn.
    /// </summary>
    /// <remarks>
    /// A jump doesn't remember its origin, so no stack interaction required.
    /// </remarks>
    private void OP_1nnn()
    {
        var addr = GetOpcodeAddress();
        ProgramCounter = addr;
    }

    /// <summary>
    /// Call subroutine at nnn.
    /// </summary>
    /// <remarks>
    /// When we call a subroutine, we want to return eventually, so we put the current PC on the top of the stack.
    /// </remarks>
    private void OP_2nnn()
    {
        var addr = GetOpcodeAddress();
        Stack[StackPointer] = ProgramCounter;
        StackPointer++;
        ProgramCounter = addr;
    }

    /// <summary>
    /// Skip next instruction if Vx = kk.
    /// </summary>
    /// <remarks>
    /// Compare register x to the value kk. If equal, skip the next instruction.
    /// </remarks>
    private void OP_3xkk()
    {
        var Vx = GetOpcodeNibble1();
        var kk = GetOpcodeByte1();

        if (Registers[Vx] == kk)
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Skip next instruction if Vx != kk.
    /// </summary>
    /// <remarks>
    /// Compares register x to the value kk. If not equal, skip the next instruction.
    /// </remarks>
    private void OP_4xkk()
    {
        var Vx = GetOpcodeNibble1();
        var kk = GetOpcodeByte1();

        if (Registers[Vx] != kk)
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Skip next instruction if Vx == Vy.
    /// </summary>
    private void Op_5xy0()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        if (Registers[Vx] == Registers[Vy])
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Set Vx = kk.
    /// </summary>
    private void Op_6xkk()
    {
        var Vx = GetOpcodeNibble1();
        var kk = GetOpcodeByte1();

        Registers[Vx] = kk;
    }

    /// <summary>
    /// Set Vx = Vx + kk.
    /// </summary>
    private void Op_7xkk()
    {
        var Vx = GetOpcodeNibble1();
        var kk = GetOpcodeByte1();

        Registers[Vx] += kk;
    }
    
    /// <summary>
    /// Set Vx = Vy;
    /// </summary>
    private void Op_8xy0()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        Registers[Vx] = Registers[Vy];
    }

    /// <summary>
    /// Set Vx = Vx OR Vy;
    /// </summary>
    private void Op_8xy1()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        Registers[Vx] |= Registers[Vy];
    }
    
    /// <summary>
    /// Set Vx = Vx AND Vy;
    /// </summary>
    private void Op_8xy2()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        Registers[Vx] &= Registers[Vy];
    }
    
    /// <summary>
    /// Set Vx = Vx XOR Vy;
    /// </summary>
    private void Op_8xy3()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        Registers[Vx] ^= Registers[Vy];
    }
    
    /// <summary>
    /// Set Vx = Vx + Vy, set VF = carry.
    /// </summary>
    private void Op_8xy4()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        var sum = Registers[Vx] + Registers[Vy];

        if (sum > 255)
        {
            Registers[0xF] = 1;
        }
        else
        {
            Registers[0xF] = 0;
        }
        
        Registers[Vx] = (byte) (sum & 0xFF);
    }
    
    /// <summary>
    /// Set Vx = Vx - Vy, set VF = NOT borrow.
    /// </summary>
    private void Op_8xy5()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();
        
        if (Registers[Vx] > Registers[Vy])
        {
            Registers[0xF] = 1;
        }
        else
        {
            Registers[0xF] = 0;
        }
        
        Registers[Vx] -= Registers[Vy];
    }

    /// <summary>
    /// Set Vx = Vx SHR 1
    /// </summary>
    /// <remarks>
    /// A right shift is performed (division by 2), and the least significant bit is saved in Register VF.
    /// </remarks>
    private void Op_8xy6()
    {
        var Vx = GetOpcodeNibble1();

        Registers[0xF] = (byte)(Registers[Vx] & 0x1);
        Registers[Vx] >>= 1;
    }
    
    /// <summary>
    /// Set Vx = Vx - Vy, set VF = NOT borrow.
    /// </summary>
    private void Op_8xy7()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();
        
        if (Registers[Vy] > Registers[Vx])
        {
            Registers[0xF] = 1;
        }
        else
        {
            Registers[0xF] = 0;
        }
        
        Registers[Vx] = (byte)(Registers[Vy] - Registers[Vx]);
    }

    /// <summary>
    /// Set Vx = Vx SHL 1
    /// </summary>
    /// <remarks>
    /// A left shift is performed (multiplication by 2), and the most significant bit is saved in Register VF.
    /// </remarks>
    private void Op_8xyE()
    {
        var Vx = GetOpcodeNibble1();

        Registers[0xF] = (byte)((Registers[Vx] & 0x80) >> 7);
        Registers[Vx] <<= 1;
    }

    /// <summary>
    /// Skip the next instruction if Vx != Vy.
    /// </summary>
    private void Op_9xy0()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();

        if (Registers[Vx] != Registers[Vy])
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Set Index = nnn
    /// </summary>
    private void Op_Annn()
    {
        var addr = GetOpcodeAddress();
        MemoryIndex = addr;
    }

    /// <summary>
    /// Jump to location nnn + V0
    /// </summary>
    private void Op_Bnnn()
    {
        var addr = GetOpcodeAddress();
        ProgramCounter = (ushort) (Registers[0] + addr);
    }

    /// <summary>
    /// Set Vx = random byte AND kk.
    /// </summary>
    private void Op_Cxkk()
    {
        var Vx = GetOpcodeNibble1();
        var kk = GetOpcodeByte1();

        Registers[Vx] = (byte)(RandByte & kk);
    }

    /// <summary>
    /// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    private void Op_Dxyn()
    {
        var Vx = GetOpcodeNibble1();
        var Vy = GetOpcodeNibble2();
        var height = GetOpcodeNibble3();

        var xPos = Registers[Vx] % VideoWidth;
        var yPos = Registers[Vy] % VideoHeight;

        Registers[0xF] = 0;

        for (var row = 0; row < height; row++)
        {
            var spriteByte = Memory[MemoryIndex + row];

            for (var col = 0; col < 8; col++)
            {
                var spritePixel = spriteByte & (0x80u >> col);
                var videoIndex = (yPos + row) * VideoWidth + xPos + col;
                var screenPixel = Video[videoIndex];

                if (spritePixel > 0)
                {
                    if (screenPixel == 0xFFFFFFFF)
                    {
                        Registers[0xF] = 1;
                    }

                    Video[videoIndex] ^= 0xFFFFFFFF;
                }
            }
        }
    }

    /// <summary>
    /// Skip next instruction if key with the value of Vx is pressed.
    /// </summary>
    private void Op_Ex9E()
    {
        var Vx = GetOpcodeNibble1();
        var key = Registers[Vx];

        if (Keypad[key])
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Skip next instruction if key with the value of Vx is pressed.
    /// </summary>
    private void Op_ExA1()
    {
        var Vx = GetOpcodeNibble1();
        var key = Registers[Vx];
        
        if (!Keypad[key])
        {
            ProgramCounter += 2;
        }
    }

    /// <summary>
    /// Set Vx = DelayTimer;
    /// </summary>
    private void Op_Fx07()
    {
        var Vx = GetOpcodeNibble1();
        Registers[Vx] = DelayTimer;
    }
    
    /// <summary>
    /// Wait for key press, store the value of the key in Vx.
    /// </summary>
    /// <remarks>
    /// The easiest way to "wait" is to decrement the PC by 2 whenever a keypad value is not detected.
    /// </remarks>
    private void Op_Fx0A()
    {
        var Vx = GetOpcodeNibble1();

        var found = false;
        for (byte i = 0; i < KeypadSize; i++)
        {
            if (Keypad[0])
            {
                found = true;
                Registers[Vx] = i;
            }
        }

        if (!found)
        {
            ProgramCounter -= 2;
        }
    }

    /// <summary>
    /// Set DelayTimer = Vx.
    /// </summary>
    private void Op_Fx15()
    {
        var Vx = GetOpcodeNibble1();
        DelayTimer = Registers[Vx];
    }

    /// <summary>
    /// Set SoundTimer = Vx.
    /// </summary>
    private void Op_Fx18()
    {
        var Vx = GetOpcodeNibble1();
        SoundTimer = Registers[Vx];
    }

    /// <summary>
    /// Set I = I + Vx
    /// </summary>
    private void Op_Fx1E()
    {
        var Vx = GetOpcodeNibble1();
        MemoryIndex += Registers[Vx];
    }

    /// <summary>
    /// Set I = location of sprite for digit Vx
    /// </summary>
    private void Op_Fx29()
    {
        var Vx = GetOpcodeNibble1();
        var digit = Registers[Vx];
        MemoryIndex = (ushort) (FontSetStartAddress + 5 * digit);
    }

    /// <summary>
    /// Store binary coded decimal representation of Vx in memory locations I, I+1, adn I+2.
    /// </summary>
    private void Op_Fx33()
    {
        var Vx = GetOpcodeNibble1();
        var value = Registers[Vx];

        // Ones-place
        Memory[MemoryIndex + 2] = (byte)(value % 10);
        value /= 10;

        // Tens-place
        Memory[MemoryIndex + 1] = (byte)(value % 10);
        value /= 10;

        // Hundreds-place
        Memory[MemoryIndex] = (byte)(value % 10);
    }

    /// <summary>
    /// Store registers V0 through Vx in memory starting at location I.
    /// </summary>
    private void Op_Fx55()
    {
        var Vx = GetOpcodeNibble1();
        for (var i = 0; i <= Vx; i++)
        {
            Memory[MemoryIndex + i] = Registers[i];
        }
    }

    /// <summary>
    /// Read registers V0 through Vx from memory starting at location I.
    /// </summary>
    private void Op_Fx65()
    {
        var Vx = GetOpcodeNibble1();
        for (var i = 0; i <= Vx; i++)
        {
            Registers[i] = Memory[MemoryIndex + i];
        }
    }

    private byte GetOpcodeNibble0() => (byte) ((OpCode & 0xF000u) >> 12);
    private byte GetOpcodeNibble1() => (byte) ((OpCode & 0x0F00u) >> 8);
    private byte GetOpcodeNibble2() => (byte) ((OpCode & 0x00F0u) >> 4);
    private byte GetOpcodeNibble3() => (byte) (OpCode & 0x000Fu);
    private byte GetOpcodeByte1() => (byte) (OpCode & 0x00FFu);

    private ushort GetOpcodeAddress() => (ushort) (OpCode & 0x0FFFu);
}