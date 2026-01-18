using Emulator6502;

namespace Emulator6502UnitTests
{
    [TestClass]
    public class EmulatorTests
    {
        [TestMethod]
        public void GetInstruction_InvalidIndex_ExceptionThrown()
        {
            //Arrange
            Emulator emulator = new Emulator();
            CPU cpu = new CPU();
            byte[] rom = new byte[1024];
            ushort index = 1025;

            //Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => emulator.GetInstruction(rom, index, cpu.Opcodes));
        }

        [TestMethod]
        public void GetInstruction_InvalidOpcode_ExceptionThrown()
        {
            //Arrange
            Emulator emulator = new Emulator();
            CPU cpu = new CPU();
            byte[] rom = [0x07];
            ushort index = 0;

            //Act and Assert
            Assert.ThrowsException<ArgumentException>(() => emulator.GetInstruction(rom, index, cpu.Opcodes));
        }

        [TestMethod]
        public void GetInstruction_ValidOpcode_CorrectValueReturned()
        {
            //Arrange
            Emulator emulator = new Emulator();
            CPU cpu = new CPU();
            byte[] rom = [0xEA, 0xA9];
            ushort index = 0;

            //Act and Assert
            CollectionAssert.AreEqual(emulator.GetInstruction(rom, index, cpu.Opcodes), (byte[])[0xEA]);
        }
    }
}
