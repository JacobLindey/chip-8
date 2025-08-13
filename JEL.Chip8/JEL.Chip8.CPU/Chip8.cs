namespace JEL.Chip8.CPU;

public delegate void ChipOp();

public partial class Chip8
{
    private const int RegisterBytes = 16;
    private const int MemoryBytes = 4096;
    private const int StackSize = 16;
    private const int KeypadSize = 16;
    public const int VideoHeight = 32;
    public const int VideoWidth = 64;
    private const int VideoBufferLength = VideoWidth * VideoHeight;

    private readonly ChipOp[] _opTable = new ChipOp[0xF + 1];
    private readonly ChipOp[] _opTable0 = new ChipOp[0xE + 1];
    private readonly ChipOp[] _opTable8 = new ChipOp[0xE + 1];
    private readonly ChipOp[] _opTableE = new ChipOp[0xE + 1];
    private readonly ChipOp[] _opTableF = new ChipOp[0x65 + 1];
    
    public Chip8()
    {
        ProgramCounter = ProgramStartAddress;

        // load font set
        for(var i = 0; i < FontSetSize; i++)
        {
            Memory[FontSetStartAddress + i] = FontSet[i];
        }

        _random = new Random();

        _opTable[0x0] = Table0;
        _opTable[0x1] = OP_1nnn;
        _opTable[0x2] = OP_2nnn;
        _opTable[0x3] = OP_3xkk;
        _opTable[0x4] = OP_4xkk;
        _opTable[0x5] = Op_5xy0;
        _opTable[0x6] = Op_6xkk;
        _opTable[0x7] = Op_7xkk;
        _opTable[0x8] = Table8;
        _opTable[0x9] = Op_9xy0;
        _opTable[0xA] = Op_Annn;
        _opTable[0xB] = Op_Bnnn;
        _opTable[0xC] = Op_Cxkk;
        _opTable[0xD] = Op_Dxyn;
        _opTable[0xE] = TableE;
        _opTable[0xF] = TableF;

        for (var i = 0; i <= 0xE; i++)
        {
            _opTable0[i] = Op_NULL;
            _opTable8[i] = Op_NULL;
            _opTableE[i] = Op_NULL;
        }

        _opTable0[0x0] = OP_00E0;
        _opTable0[0xE] = OP_00EE;

        _opTable8[0x0] = Op_8xy0;
        _opTable8[0x1] = Op_8xy1;
        _opTable8[0x2] = Op_8xy2;
        _opTable8[0x3] = Op_8xy3;
        _opTable8[0x4] = Op_8xy4;
        _opTable8[0x5] = Op_8xy5;
        _opTable8[0x6] = Op_8xy6;
        _opTable8[0x7] = Op_8xy7;
        _opTable8[0xE] = Op_8xyE;

        _opTableE[0x1] = Op_ExA1;
        _opTableE[0xE] = Op_Ex9E;

        for (var i = 0; i <= 0x65; i++)
        {
            _opTableF[i] = Op_NULL;
        }

        _opTableF[0x07] = Op_Fx07;
        _opTableF[0x0A] = Op_Fx0A;
        _opTableF[0x15] = Op_Fx15;
        _opTableF[0x18] = Op_Fx18;
        _opTableF[0x1E] = Op_Fx1E;
        _opTableF[0x29] = Op_Fx29;
        _opTableF[0x33] = Op_Fx33;
        _opTableF[0x55] = Op_Fx55;
        _opTableF[0x65] = Op_Fx65;
    }
    
    public byte[] Registers { get; } = new byte[RegisterBytes];
    public byte[] Memory { get; } = new byte[MemoryBytes];
    public ushort MemoryIndex { get; private set; }
    public ushort ProgramCounter { get; private set; }
    public ushort[] Stack { get; } = new ushort[StackSize];
    public byte StackPointer { get; private set; }
    public byte DelayTimer { get; private set; }
    public byte SoundTimer { get; private set; }
    public bool[] Keypad { get; } = new bool[KeypadSize];
    public uint[] Video { get; } = new uint[VideoBufferLength];
    public ushort OpCode { get; private set; }
    
    
    public void Cycle()
    {
        OpCode = (ushort) ((Memory[ProgramCounter] << 8) | Memory[ProgramCounter + 1]);
        ProgramCounter += 2;

        // decode
        var op = _opTable[GetOpcodeNibble0()];

        // execute
        op();

        if (DelayTimer > 0)
        {
            DelayTimer--;
        }

        if (SoundTimer > 0)
        {
            SoundTimer--;
        }
    }
}
