using System;
using System.Text;
using System.IO;

namespace GSTExtract
{
    /// <summary>
    /// A class designed to read information from a binary file.
    /// </summary>
    public class EndianBinaryReader : BinaryReader
    {
        public enum Endianess
        {
            Little,
            Big
        }

        /// <summary>
        /// Creates a new reader stream using the path to the file to create a stream for.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="e">The endianess of the stream, Big Endian by default.</param>
        public EndianBinaryReader(string path, Endianess e = Endianess.Big)
            : base(new MemoryStream(File.ReadAllBytes(path)))
        {
            mEndianess = e;
        }

        public EndianBinaryReader(Stream stream, Endianess e = Endianess.Big)
            : base(stream)
        {
            mEndianess = e;
        }

        public EndianBinaryReader(Stream stream, Encoding encoding, Endianess e = Endianess.Big)
            : base(stream, encoding)
        {
            mEndianess = e;
        }

        // We don't have to override reading bytes, as endianess doesn't matter in the case of a single byte

        /// <summary>
        /// Reads a signed 16-bit integer from the stream.
        /// </summary>
        /// <returns></returns>
        public override short ReadInt16()
        {
            UInt16 val = base.ReadUInt16();

            if (mEndianess == Endianess.Big)
                return (Int16)((val >> 8) | (val << 8));
            else
                return (Int16)val;
        }

        /// <summary>
        /// Reads a signed 32-bit integer from the stream.
        /// </summary>
        /// <returns></returns>
        public override int ReadInt32()
        {
            UInt32 val = base.ReadUInt32();

            if (mEndianess == Endianess.Big)
                return (Int32)((val >> 24) | ((val & 0xFF0000) >> 8) | ((val & 0xFF00) << 8) | (val << 24));
            else
                return (Int32)val;
        }

        /// <summary>
        /// Reads an unsigned 16-bit integer from the stream.
        /// </summary>
        /// <returns></returns>
        public override ushort ReadUInt16()
        {
            UInt16 val = base.ReadUInt16();
            if (mEndianess == Endianess.Big)
                return (UInt16)((val >> 8) | (val << 8));
            else
                return val;
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer from the stream.
        /// </summary>
        /// <returns></returns>
        public override uint ReadUInt32()
        {
            UInt32 val = base.ReadUInt32();

            if (mEndianess == Endianess.Big)
                return (UInt32)((val >> 24) | ((val & 0xFF0000) >> 8) | ((val & 0xFF00) << 8) | (val << 24));
            else
                return val;
        }

        /// <summary>
        /// Reads a 32-bit single from the stream.
        /// </summary>
        /// <returns></returns>
        public override float ReadSingle()
        {
            byte[] bytes = base.ReadBytes(4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            float val = BitConverter.ToSingle(bytes, 0);
            return val;
        }

        /// <summary>
        /// Reads a null termined ('0') string from the stream.
        /// </summary>
        /// <returns></returns>
        public string ReadStringNT()
        {
            string ret = "";
            char curChar;

            while ((curChar = base.ReadChar()) != '\0')
                ret += curChar;

            return ret;
        }

        /// <summary>
        /// Read a string that is prefixed by a single byte for the string length.
        /// </summary>
        /// <returns></returns>
        public string ReadLengthPrefixedString()
        {
            byte len = base.ReadByte();
            return ReadString(len);
        }

        /// <summary>
        /// Read a string specified by the length.
        /// </summary>
        /// <param name="len">The length of the string to read.</param>
        /// <returns></returns>
        public string ReadString(int len)
        {
            return Encoding.ASCII.GetString(base.ReadBytes(len));
        }

        /// <summary>
        /// Read a string with a prefixed length at a specified location in the stream.
        /// </summary>
        /// <param name="where">The location in the stream.</param>
        /// <returns></returns>
        public string ReadStringLengthPrefixedAt(long where)
        {
            long basePos = BaseStream.Position;
            Seek(where);
            string ret = ReadLengthPrefixedString();
            BaseStream.Position = basePos;
            return ret;
        }

        /// <summary>
        /// Read an unsigned 8-bit integer at a specified location in the stream.
        /// </summary>
        /// <param name="where">The location in the stream.</param>
        /// <returns></returns>
        public byte ReadByteAt(long where)
        {
            long basePos = BaseStream.Position;
            Seek(where);
            byte ret = ReadByte();
            BaseStream.Position = basePos;
            return ret;
        }

        /// <summary>
        /// Read an unsigned 32-bit integer at a specified location in the stream.
        /// </summary>
        /// <param name="where">The location in the stream.</param>
        /// <returns></returns>
        public uint ReadUInt32At(long where)
        {
            long basePos = BaseStream.Position;
            Seek(where);
            uint ret = ReadUInt32();
            BaseStream.Position = basePos;
            return ret;
        }

        /// <summary>
        /// Skip alignment in a binary file up to the nearest value specified.
        /// </summary>
        /// <param name="alignment">The specified value to align to.</param>
        public void SkipAlignment(int alignment)
        {
            while ((BaseStream.Position % alignment) != 0)
            {
                ReadByte();
            }
        }

        /// <summary>
        /// Move the current location of the reader in the stream.
        /// </summary>
        /// <param name="location">The location to move the stream.</param>
        public void Seek(long location)
        {
            BaseStream.Position = location;
        }

        /// <summary>
        /// Checks if the position of the stream is at the end of the file.
        /// </summary>
        /// <returns></returns>
        public bool IsEOF()
        {
            return BaseStream.Position == BaseStream.Length;
        }

        /// <summary>
        /// The stored endianess to read the binary file with.
        /// </summary>
        readonly Endianess mEndianess;
    }
}