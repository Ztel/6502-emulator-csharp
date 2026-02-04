using Emulator6502;

namespace Emulator6502UnitTests
{
    [TestClass]
    public sealed class CPU_Tests
    {
        [TestMethod]
        public void GetStatusRegisterFlag_InvalidFlag_ExceptionThrown()
        {
            //Arrange
            CPU cpu = new CPU();
            char flag = 'Q';

            //Act and Assert
            Assert.ThrowsException<ArgumentException>(() => cpu.GetStatusRegisterFlag(flag));
        }

        [TestMethod]
        public void GetStatusRegisterFlag_ValidFlags_DefaultValueReturned()
        {
            //Arrange
            CPU cpu = new CPU();
            char[] flags = ['N', 'V', 'B', 'D', 'I', 'Z', 'C'];

            //Act and Assert
            foreach (char flag in flags)
            {
                Assert.AreEqual(0, cpu.GetStatusRegisterFlag(flag));
            }
        }

        [TestMethod]
        public void SetStatusRegisterFlag_InvalidFlag_ExceptionThrown()
        {
            //Arrange
            CPU cpu = new CPU();
            char flag = 'Q';

            //Act and Assert
            Assert.ThrowsException<ArgumentException>(() => cpu.SetStatusRegisterFlag(flag, true));
        }

        [TestMethod]
        public void SetStatusRegisterFlag_ValidFlags_TrueValueSet()
        {
            //Arrange
            CPU cpu = new CPU();
            char[] flags = ['N', 'V', 'B', 'D', 'I', 'Z', 'C'];

            //Act
            foreach (char flag in flags)
            {
                cpu.SetStatusRegisterFlag(flag, true);
            }

            //Assert
            foreach (char flag in flags)
            {
                Assert.AreEqual(1, cpu.GetStatusRegisterFlag(flag));
            }
        }

        [TestMethod]
        public void LDA_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA9;
            memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_ZeroPage_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA5;
            memory[0x8001] = 0x9B;
            memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_ZeroPageX_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xB5;
            memory[0x8001] = 0xFF;
            memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_Absolute_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xAD;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_AbsoluteX_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xBD;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_AbsoluteY_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xB9;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_IndexedIndirect_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA1;
            memory[0x8001] = 0x00;
            memory[0x01] = 0xCD;
            memory[0x02] = 0xAB;
            memory[0xABCD] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDA_IndirectIndexed_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xB1;
            memory[0x8001] = 0x00;
            memory[0x00] = 0xCD;
            memory[0x01] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void STA_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x85;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AB]);
        }

        [TestMethod]
        public void STA_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x95;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AC]);
        }

        [TestMethod]
        public void STA_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x8D;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCD]);
        }

        [TestMethod]
        public void STA_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x9D;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCE]);
        }

        [TestMethod]
        public void STA_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x99;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCE]);
        }

        [TestMethod]
        public void STA_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x81;
            memory[0x8001] = 0xAB;
            memory[0x00AC] = 0x34;
            memory[0x00AD] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x1234]);
        }

        [TestMethod]
        public void STA_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x91;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0x34;
            memory[0x00AC] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x1235]);
        }

        [TestMethod]
        public void LDX_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA2;
            memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDX_ZeroPage_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA6;
            memory[0x8001] = 0x9B;
            memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDX_ZeroPageY_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xB6;
            memory[0x8001] = 0xFF;
            memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDX_Absolute_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xAE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDX_AbsoluteY_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xBE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void STX_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x86;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AB]);
        }

        [TestMethod]
        public void STX_ZeroPageY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x96;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AC]);
        }

        [TestMethod]
        public void STX_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x8E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCD]);
        }

        [TestMethod]
        public void LDY_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA0;
            memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDY_ZeroPage_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA4;
            memory[0x8001] = 0x9B;
            memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDY_ZeroPageX_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xB4;
            memory[0x8001] = 0xFF;
            memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDY_Absolute_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xAC;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void LDY_AbsoluteX_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xBC;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
        }

        [TestMethod]
        public void STY_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x84;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AB]);
        }

        [TestMethod]
        public void STY_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x94;
            memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AC]);
        }

        [TestMethod]
        public void STY_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x8C;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCD]);
        }

        [TestMethod]
        public void TAX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xAA;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
        }

        [TestMethod]
        public void TXA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x8A;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
        }

        [TestMethod]
        public void TAY_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA8;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
        }

        [TestMethod]
        public void TYA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x98;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
        }

        [TestMethod]
        public void ADC_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x69;
            memory[0x8001] = 0b01111111;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b10000000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x65;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x08;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x10, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x75;
            memory[0x8001] = 0xAB;
            memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFE, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x6D;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0x1A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x2B;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x45, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x7D;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x79;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x61;
            memory[0x8001] = 0xCC;
            memory[0x00CD] = 0x34;
            memory[0x00CE] = 0x12;
            memory[0x1234] = 0x02;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x02;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x04, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ADC_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x71;
            memory[0x8001] = 0xCC;
            memory[0x00CC] = 0x34;
            memory[0x00CD] = 0x12;
            memory[0x1235] = 0x02;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x02;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x04, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xE9;
            memory[0x8001] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFE, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xE5;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x08;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xF5;
            memory[0x8001] = 0xAB;
            memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xED;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0x1A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x2B;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x11, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xFD;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0x03;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x10;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x0D, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xF9;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x02, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xE1;
            memory[0x8001] = 0xCC;
            memory[0x00CD] = 0x34;
            memory[0x00CE] = 0x12;
            memory[0x1234] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x05;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFD, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void SBC_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xF1;
            memory[0x8001] = 0xCC;
            memory[0x00CC] = 0x34;
            memory[0x00CD] = 0x12;
            memory[0x1235] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x80;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x7F, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xE6;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x09, memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xF6;
            memory[0x8001] = 0xAB;
            memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, memory[0x00AC]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xEE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0x7F;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x80, memory[0xABCD]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xFE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCF] = 0xFE;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x02;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0xABCF]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xC6;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0x09;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x08, memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xD6;
            memory[0x8001] = 0xAB;
            memory[0x00AC] = 0x00;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, memory[0x00AC]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xCE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0x81;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x80, memory[0xABCD]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xDE;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCF] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x02;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFE, memory[0xABCF]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xE8;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, cpu.XRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xCA;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x0;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INY_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xC8;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, cpu.YRegister);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEY_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x88;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x0;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x0A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10101010;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01010100, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x06;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0b01000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b10000000, memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x16;
            memory[0x8001] = 0xAB;
            memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00100000, memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x0E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, memory[0xABCD]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x1E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0b11111111;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b11111110, memory[0xABCE]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x4A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10101010;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01010101, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x46;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0x00, memory[0x00AB]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x56;
            memory[0x8001] = 0xAB;
            memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00001000, memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x4E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01000000, memory[0xABCD]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x5E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0b11111111;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01111111, memory[0xABCE]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x2A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10110001;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01100010, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x26;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00000010, memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x36;
            memory[0x8001] = 0xAB;
            memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00100000, memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x2E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00000000, memory[0xABCD]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x3E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0b00000000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00000001, memory[0xABCE]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x6A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10110001;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01011000, cpu.Accumulator);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x66;
            memory[0x8001] = 0xAB;
            memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00000000, memory[0x00AB]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x76;
            memory[0x8001] = 0xAB;
            memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b00001000, memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x6E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCD] = 0b11111111;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b01111111, memory[0xABCD]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0x7E;
            memory[0x8001] = 0xCD;
            memory[0x8002] = 0xAB;
            memory[0xABCE] = 0b00000000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0b10000000, memory[0xABCE]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }
    }
}
