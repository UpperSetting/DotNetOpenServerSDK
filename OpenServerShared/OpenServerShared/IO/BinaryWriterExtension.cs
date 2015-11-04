/*
Copyright 2015 Upper Setting Corporation

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
    /// Static class that extends the BinaryWriter class.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes an Int32 that contains the number of bytes then writes the bytes.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">An array of bytes to write.</param>
        public static void WriteBytes(this BinaryWriter bw, byte[] value)
        {
            if (value == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write(value.Length);
                bw.Write(value);
            }
        }

        /// <summary>
        /// Writes an Int64 that contains the passed DateTime's ticks.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">The DateTime object to write.</param>
        public static void WriteDateTime(this BinaryWriter bw, DateTime value)
        {
            bw.Write(value.Ticks);
        }

        /// <summary>
        /// Writes a bool that specifies if the DateTime? value is not NULL. If not NULL
        /// writes the DateTime's ticks.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">The DateTime? object to write.</param>
        public static void Write(this BinaryWriter bw, DateTime? value)
        {
            if (value == null || !value.HasValue)
                bw.Write(false);
            else
            {
                bw.Write(true);
                bw.WriteDateTime(value.Value);
            }
        }

        /// <summary>
        /// Writes an Int32 that contains the number of Int32s then writes each Int32.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">An array of Int32s to write.</param>
        public static void WriteInts(this BinaryWriter bw, int[] value)
        {
            if (value == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write(value.Length);
                foreach (int i in value)
                    bw.Write(i);
            }
        }

        /// <summary>
        /// Writes a string. If the passed value is null, an empty string is written.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">The string to write.</param>
        public static void WriteString(this BinaryWriter bw, string value)
        {
            if (value == null)
                value = string.Empty;

            bw.Write(value);
        }

        /// <summary>
        /// Writes an Int32 that contains the number of strings then writes each string.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">An array of strings to write.</param>
        public static void WriteStrings(this BinaryWriter bw, string[] value)
        {
            if (value == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write(value.Length);
                foreach (string v in value)
                    bw.WriteString(v);
            }            
        }

        /// <summary>
        /// Writes an Int64 that contains the passed TimeSpan's ticks.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">The TimeSpan object to write.</param>
        public static void WriteTimeSpan(this BinaryWriter bw, TimeSpan value)
        {
            bw.Write(value.Ticks);
        }

        /// <summary>
        /// Writes a bool that specifies if the TimeSpan? value is not NULL. If not NULL
        /// writes the TimeSpan's ticks.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">The TimeSpan? object to write.</param>
        public static void Write(this BinaryWriter bw, TimeSpan? value)
        {
            if (value == null || !value.HasValue)
                bw.Write(false);
            else
            {
                bw.Write(true);
                bw.WriteTimeSpan(value.Value);
            }
        }

        /// <summary>
        /// Writes an Int32 that contains the number of UInt32s then writes each UInt32.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">An array of UInt32s to write.</param>
        public static void WriteUInts(this BinaryWriter bw, uint[] value)
        {
            if (value == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write(value.Length);
                foreach (uint i in value)
                    bw.Write(i);
            }
        }

        /// <summary>
        /// Writes an Int32 that contains the number of UInt16s then writes each UInt16.
        /// </summary>
        /// <param name="bw">The BinaryWriter to write to.</param>
        /// <param name="value">An array of UInt16s to write.</param>
        public static void WriteUInt16s(this BinaryWriter bw, ushort[] value)
        {
            if (value == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write(value.Length);
                foreach (ushort i in value)
                    bw.Write(i);
            }
        }
    }
}
