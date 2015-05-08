
namespace System.IO
{
    /// <summary>
    /// Static class that extends the BinaryReader class.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads an Int64 that contains ticks.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>A DateTime object.</returns>
        public static DateTime ReadDateTime(this BinaryReader br)
        {
            return new DateTime(br.ReadInt64());
        }

        /// <summary>
        /// Reads an Int32 that contains the number of strings to read then reads each
        /// string.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>An array of strings.</returns>
        public static string[] ReadStrings(this BinaryReader br)
        {
            int count = br.ReadInt32();
            string[] retVal = new string[count];
            for (int i = 0; i < count; i++)
                retVal[i] = br.ReadString();

            return retVal;
        }

        /// <summary>
        /// Reads an Int64 that contains ticks.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>A TimeSpan object.</returns>
        public static TimeSpan ReadTimeSpan(this BinaryReader br)
        {
            return new TimeSpan(br.ReadInt64());
        }

        /// <summary>
        /// Reads an Int32 that contains the number of UInt32s to read then reads each UInt32.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>An array of UInt32s.</returns>
        public static uint[] ReadUInts(this BinaryReader br)
        {
            int count = br.ReadInt32();
            uint[] retVal = new uint[count];
            for (int i = 0; i < count; i++)
                retVal[i] = br.ReadUInt32();

            return retVal;
        }
    }
}
