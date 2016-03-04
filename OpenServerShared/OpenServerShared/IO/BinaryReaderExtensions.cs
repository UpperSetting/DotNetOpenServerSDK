/*
Copyright 2015-2016 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/


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
        /// Reads a bool that specifies if a DateTime value.  If present reads the DateTime.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>A DateTime? object.</returns>
        public static DateTime? ReadNullableDateTime(this BinaryReader br)
        {
            if (br.ReadBoolean())
                return br.ReadDateTime();
            else
                return null;
        }

        /// <summary>
        /// Reads an Int32 that contains the number of Int32s to read then reads each Int32.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>An array of Int32s.</returns>
        public static int[] ReadInts(this BinaryReader br)
        {
            int count = br.ReadInt32();
            int[] retVal = new int[count];
            for (int i = 0; i < count; i++)
                retVal[i] = br.ReadInt32();

            return retVal;
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
        /// Reads a bool that specifies if a TimeSpan value.  If present reads the TimeSpan.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>A TimeSpan? object.</returns>
        public static TimeSpan? ReadNullableTimeSpan(this BinaryReader br)
        {
            if (br.ReadBoolean())
                return br.ReadTimeSpan();
            else
                return null;
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

        /// <summary>
        /// Reads an Int32 that contains the number of UInt16s to read then reads each UInt16.
        /// </summary>
        /// <param name="br">The BinaryReader object to read from.</param>
        /// <returns>An array of UInt16s.</returns>
        public static ushort[] ReadUInt16s(this BinaryReader br)
        {
            int count = br.ReadInt32();
            ushort[] retVal = new ushort[count];
            for (int i = 0; i < count; i++)
                retVal[i] = br.ReadUInt16();

            return retVal;
        }
    }
}
