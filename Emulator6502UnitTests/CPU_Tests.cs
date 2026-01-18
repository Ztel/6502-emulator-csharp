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
                Assert.AreEqual(cpu.GetStatusRegisterFlag(flag), false);
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
                Assert.AreEqual(cpu.GetStatusRegisterFlag(flag), true);
            }
        }
    }
}
