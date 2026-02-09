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
            
            cpu.Memory[0x8000] = 0xA9;
            cpu.Memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xA5;
            cpu.Memory[0x8001] = 0x9B;
            cpu.Memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xB5;
            cpu.Memory[0x8001] = 0xFF;
            cpu.Memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xAD;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xBD;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xB9;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xA1;
            cpu.Memory[0x8001] = 0x00;
            cpu.Memory[0x01] = 0x34;
            cpu.Memory[0x02] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xB1;
            cpu.Memory[0x8001] = 0x00;
            cpu.Memory[0x00] = 0x34;
            cpu.Memory[0x01] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x85;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AB]);
        }

        [TestMethod]
        public void STA_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x95;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AC]);
        }

        [TestMethod]
        public void STA_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x8D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1234]);
        }

        [TestMethod]
        public void STA_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x9D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1235]);
        }

        [TestMethod]
        public void STA_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x99;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1235]);
        }

        [TestMethod]
        public void STA_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x81;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0x34;
            cpu.Memory[0x00AD] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1234]);
        }

        [TestMethod]
        public void STA_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x91;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x34;
            cpu.Memory[0x00AC] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1235]);
        }

        [TestMethod]
        public void LDX_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xA2;
            cpu.Memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xA6;
            cpu.Memory[0x8001] = 0x9B;
            cpu.Memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xB6;
            cpu.Memory[0x8001] = 0xFF;
            cpu.Memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xAE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xBE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x86;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AB]);
        }

        [TestMethod]
        public void STX_ZeroPageY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x96;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AC]);
        }

        [TestMethod]
        public void STX_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x8E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1234]);
        }

        [TestMethod]
        public void LDY_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xA0;
            cpu.Memory[0x8001] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xA4;
            cpu.Memory[0x8001] = 0x9B;
            cpu.Memory[0x009B] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xB4;
            cpu.Memory[0x8001] = 0xFF;
            cpu.Memory[0x0000] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xAC;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xBC;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0x84;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AB]);
        }

        [TestMethod]
        public void STY_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x94;
            cpu.Memory[0x8001] = 0xAB;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AC]);
        }

        [TestMethod]
        public void STY_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x8C;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1234]);
        }

        [TestMethod]
        public void TAX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xAA;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
        }

        [TestMethod]
        public void TXA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x8A;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
        }

        [TestMethod]
        public void TAY_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xA8;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.YRegister);
        }

        [TestMethod]
        public void TYA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x98;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
        }

        [TestMethod]
        public void ADC_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x69;
            cpu.Memory[0x8001] = 0b01111111;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x01;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x65;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x08;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x75;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x6D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0x1A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x2B;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x7D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x79;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x61;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1234] = 0x02;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x02;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x71;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CC] = 0x34;
            cpu.Memory[0x00CD] = 0x12;
            cpu.Memory[0x1235] = 0x02;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x02;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xE9;
            cpu.Memory[0x8001] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xE5;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x08;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xF5;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xED;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0x1A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x2B;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xFD;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0x03;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x10;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xF9;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0xE1;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1234] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.Accumulator = 0x05;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xF1;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CC] = 0x34;
            cpu.Memory[0x00CD] = 0x12;
            cpu.Memory[0x1235] = 0x01;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x01;
            cpu.Accumulator = 0x80;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xE6;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x08;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x09, cpu.Memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xF6;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x00, cpu.Memory[0x00AC]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xEE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0x7F;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x80, cpu.Memory[0x1234]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xFE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1236] = 0xFE;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x02;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x1236]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xC6;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x09;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x08, cpu.Memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xD6;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0x00;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x00AC]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xCE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0x81;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x80, cpu.Memory[0x1234]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void DEC_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xDE;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1236] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x02;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFE, cpu.Memory[0x1236]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void INX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xE8;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xCA;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x0;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0xC8;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0x88;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x0;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x0A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10101010;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x06;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b01000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10000000, cpu.Memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x16;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00100000, cpu.Memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x0E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x00, cpu.Memory[0x1234]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ASL_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x1E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b11111111;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111110, cpu.Memory[0x1235]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x4A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10101010;

            //Act
            cpu.Step();

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
           
            cpu.Memory[0x8000] = 0x46;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x00, cpu.Memory[0x00AB]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x56;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001000, cpu.Memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x4E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01000000, cpu.Memory[0x1234]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void LSR_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x5E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b11111111;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01111111, cpu.Memory[0x1235]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x2A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10110001;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x26;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00000010, cpu.Memory[0x00AB]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x36;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00100000, cpu.Memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x2E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10000000;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00000000, cpu.Memory[0x1234]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROL_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x3E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b00000000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00000001, cpu.Memory[0x1235]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_WithAccumulator_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
         
            cpu.Memory[0x8000] = 0x6A;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b10110001;

            //Act
            cpu.Step();

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
            
            cpu.Memory[0x8000] = 0x66;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b00000001;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00000000, cpu.Memory[0x00AB]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x76;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AA] = 0b00010000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001000, cpu.Memory[0x00AA]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x6E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b11111111;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01111111, cpu.Memory[0x1234]);
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ROR_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x7E;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b00000000;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x01;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10000000, cpu.Memory[0x1235]);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x29;
            cpu.Memory[0x8001] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x25;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x35;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x2D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x3D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100000, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x39;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x21;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1234] = 0b00000001;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00000001, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void AND_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x31;
            cpu.Memory[0x8001] = 0xCD;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x09;
            cpu.Memory[0x8001] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x05;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x15;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x0D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x1D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b11111010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x19;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10101111, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x01;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1234] = 0b00000001;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001111, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void ORA_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x11;
            cpu.Memory[0x8001] = 0xCD;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10101111, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_Immediate_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x49;
            cpu.Memory[0x8001] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01011010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_ZeroPage_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x45;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01011010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_ZeroPageX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x55;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01011010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_Absolute_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x4D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01011010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_AbsoluteX_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x5D;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b01011010, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_AbsoluteY_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x59;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100101, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_IndexedIndirect_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x41;
            cpu.Memory[0x8001] = 0xCC;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1234] = 0b00000001;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b00001110, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void EOR_IndirectIndexed_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x51;
            cpu.Memory[0x8001] = 0xCD;
            cpu.Memory[0x00CD] = 0x34;
            cpu.Memory[0x00CE] = 0x12;
            cpu.Memory[0x1235] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00001111;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10100101, cpu.Accumulator);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void BIT_ZeroPage_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x24;
            cpu.Memory[0x8001] = 0xCD;
            cpu.Memory[0x00CD] = 0b01010101;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b11110000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void BIT_Absolute_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0x2C;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0b10101010;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0b00000000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_Immediate_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xC9;
            cpu.Memory[0x8001] = 0x34;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_ZeroPage_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xC5;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x12;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_ZeroPageX_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xD5;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFF;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_Absolute_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xCD;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0xFE;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_AbsoluteX_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xDD;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0x04;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x06;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_AbsoluteY_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xD9;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1235] = 0x06;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x04;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_IndexedIndirect_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xC1;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AC] = 0x34;
            cpu.Memory[0x00AD] = 0x12;
            cpu.Memory[0x1234] = 0x06;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x04;
            cpu.XRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CMP_IndirectIndexed_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xD1;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0x34;
            cpu.Memory[0x00AC] = 0x12;
            cpu.Memory[0x1235] = 0x06;

            cpu.ProgramCounter = 0x8000;
            cpu.Accumulator = 0x04;
            cpu.YRegister = 0x01;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPX_Immediate_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xE0;
            cpu.Memory[0x8001] = 0x34;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPX_ZeroPage_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xE4;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0x12;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPX_Absolute_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xEC;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFE;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPY_Immediate_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xC0;
            cpu.Memory[0x8001] = 0x34;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPY_ZeroPage_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xC4;
            cpu.Memory[0x8001] = 0xAB;
            cpu.Memory[0x00AB] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0x12;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CPY_Absolute_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xCC;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.YRegister = 0xFE;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void BCC_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x90;
            cpu.Memory[0x8001] = 0b10000000;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('C', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7F82, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BCS_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
           
            cpu.Memory[0x8000] = 0xB0;
            cpu.Memory[0x8001] = 0b01111111;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x8081, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BEQ_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xF0;
            cpu.Memory[0x8001] = 0x02;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('Z', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x8004, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BNE_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xD0;
            cpu.Memory[0x8001] = 0xFC;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('Z', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7FFE, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BPL_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x10;
            cpu.Memory[0x8001] = 0xFC;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('N', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7FFE, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BMI_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x30;
            cpu.Memory[0x8001] = 0xFC;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('N', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7FFE, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BVC_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x50;
            cpu.Memory[0x8001] = 0xFC;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('V', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7FFE, cpu.ProgramCounter);
        }

        [TestMethod]
        public void BVS_Relative_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x70;
            cpu.Memory[0x8001] = 0xFC;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('V', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x7FFE, cpu.ProgramCounter);
        }

        [TestMethod]
        public void JMP_Absolute_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x4C;
            cpu.Memory[0x8001] = 0xCD;
            cpu.Memory[0x8002] = 0xAB;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xABCD, cpu.ProgramCounter);
        }

        [TestMethod]
        public void JMP_Indirect_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x6C;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;
            cpu.Memory[0x1234] = 0xCD;
            cpu.Memory[0x1235] = 0xAB;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xABCD, cpu.ProgramCounter);
        }

        [TestMethod]
        public void JSR_Absolute_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x20;
            cpu.Memory[0x8001] = 0x34;
            cpu.Memory[0x8002] = 0x12;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x1234, cpu.ProgramCounter);
            Assert.AreEqual(0xFD, cpu.StackPointer);
            Assert.AreEqual(0x80, cpu.Memory[0x01FF]);
            Assert.AreEqual(0x02, cpu.Memory[0x01FE]);
        }

        [TestMethod]
        public void RTS_Implicit_CorrectAddressLocated()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x60;
            cpu.Memory[0x01FE] = 0x02;
            cpu.Memory[0x01FF] = 0x80;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFD;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x8003, cpu.ProgramCounter);
            Assert.AreEqual(0xFF, cpu.StackPointer);
        }

        [TestMethod]
        public void BRK_Implicit_CorrectStatusSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x00;
            cpu.Memory[0xFFFE] = 0xFF;
            cpu.Memory[0xFFFF] = 0x80;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFF;
            cpu.SetStatusRegisterFlag('N', true);
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x80FF, cpu.ProgramCounter);
            Assert.AreEqual(0xFC, cpu.StackPointer);
            Assert.AreEqual(0x80, cpu.Memory[0x01FF]);
            Assert.AreEqual(0x02, cpu.Memory[0x01FE]);
            Assert.AreEqual(0b10000101, cpu.StatusRegister);
            Assert.AreEqual(0b10110001, cpu.Memory[0x01FD]);
        }

        [TestMethod]
        public void RTI_Implicit_CorrectStatusSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x40;
            cpu.Memory[0x01FD] = 0b10110001;
            cpu.Memory[0x01FE] = 0xFF;
            cpu.Memory[0x01FF] = 0x80;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFC;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x80FF, cpu.ProgramCounter);
            Assert.AreEqual(0xFF, cpu.StackPointer);
            Assert.AreEqual(0b10000001, cpu.StatusRegister);
        }

        [TestMethod]
        public void PHA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x48;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFF;
            cpu.Accumulator = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Memory[0x01FF]);
            Assert.AreEqual(0xFE, cpu.StackPointer);
        }

        [TestMethod]
        public void PLA_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x68;
            cpu.Memory[0x01FF] = 0xFF;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFE;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
            Assert.AreEqual(0xFF, cpu.StackPointer);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void PHP_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x08;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFF;
            cpu.SetStatusRegisterFlag('N', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10110000, cpu.Memory[0x01FF]);
            Assert.AreEqual(0xFE, cpu.StackPointer);
        }

        [TestMethod]
        public void PLP_Implicit_CorrectFlagsSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x28;
            cpu.Memory[0x01FF] = 0b10110000;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFE;
            cpu.SetStatusRegisterFlag('B', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0b10010000, cpu.StatusRegister);
            Assert.AreEqual(0xFF, cpu.StackPointer);
        }

        [TestMethod]
        public void TXS_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x9A;

            cpu.ProgramCounter = 0x8000;
            cpu.XRegister = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.StackPointer);
        }

        [TestMethod]
        public void TSX_Implicit_CorrectValueWritten()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xBA;

            cpu.ProgramCounter = 0x8000;
            cpu.StackPointer = 0xFF;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0xFF, cpu.XRegister);
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('Z'));
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('N'));
        }

        [TestMethod]
        public void CLC_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x18;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('C', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('C'));
        }

        [TestMethod]
        public void SEC_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x38;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('C', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('C'));
        }

        [TestMethod]
        public void CLI_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x58;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('I', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('I'));
        }

        [TestMethod]
        public void SEI_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0x78;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('I', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('I'));
        }

        [TestMethod]
        public void CLD_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xD8;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('D', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('D'));
        }

        [TestMethod]
        public void SED_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xF8;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('D', false);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(1, cpu.GetStatusRegisterFlag('D'));
        }

        [TestMethod]
        public void CLV_Implicit_CorrectFlagSet()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xB8;

            cpu.ProgramCounter = 0x8000;
            cpu.SetStatusRegisterFlag('V', true);

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0, cpu.GetStatusRegisterFlag('V'));
        }

        [TestMethod]
        public void NOP_Implicit_ProgramCounterIncremented()
        {
            //Arrange
            CPU cpu = new CPU();
            
            cpu.Memory[0x8000] = 0xEA;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step();

            //Assert
            Assert.AreEqual(0x8001, cpu.ProgramCounter);
        }
    }
}
