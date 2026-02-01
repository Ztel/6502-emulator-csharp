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
    }
}
