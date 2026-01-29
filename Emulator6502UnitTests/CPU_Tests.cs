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
                Assert.AreEqual(false, cpu.GetStatusRegisterFlag(flag));
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
                Assert.AreEqual(true, cpu.GetStatusRegisterFlag(flag));
            }
        }

        [TestMethod]
        public void LDA_Immediate_CorrectValueRead()
        {
            //Arrange
            CPU cpu = new CPU();

            byte[] memory = new byte[65536];
            memory[0x8000] = 0xA9;
            memory[0x8000] = 0xFF;

            cpu.ProgramCounter = 0x8000;

            //Act
            cpu.Step(ref memory);

            //Assert
            Assert.AreEqual(0xFF, cpu.Accumulator);
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
        }
    }
}
