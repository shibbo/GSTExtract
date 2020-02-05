using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTExtract
{
    class GhostPacket
    {
        /// <summary>
        /// Reads a single ghost packet, and reads a proceeding packet if needed.
        /// </summary>
        /// <param name="reader">The reader stream.</param>
        public GhostPacket(ref EndianBinaryReader reader, ref List<string> output)
        {
            byte nextIdx = 0;

            mPacketIndex = reader.ReadByte();
            mPacketLength = reader.ReadByte();

            output.Add($"Loaded Packet #{mPacketIndex} with length {mPacketLength}");

            mFlags = reader.ReadInt16();

            if ((reader.BaseStream.Position - 4) + mPacketLength != reader.BaseStream.Length)
            {
                nextIdx = reader.ReadByteAt((reader.BaseStream.Position - 4) + mPacketLength);
            }

            /*
            if (PacketIndex + 1 == nextIdx)
            {
                output.Add("Adding to existing group...");
            }
            else
            {
                output.Add("New group!");
            }
            */

            // http://shibboleet.us.to/wiki/index.php/GST_(File_Format)

            if ((mFlags & 0x1) != 0)
            {
                float x = reader.ReadInt16();
                float y = reader.ReadInt16();
                float z = reader.ReadInt16();

                output.Add($"Set Translation: (X: {x} Y: {y} Z: {z})");
            }

            if ((mFlags & 0x800) != 0)
            {
                float x = reader.ReadByte();
                float y = reader.ReadByte();
                float z = reader.ReadByte();

                output.Add($"Set Velocity: (X: {x} Y: {y} Z: {z})");
            }

            if ((mFlags & 0x400) != 0)
            {
                float x = reader.ReadByte();
                float y = reader.ReadByte();
                float z = reader.ReadByte();

                output.Add($"Set Scale: (X: {x} Y: {y} Z: {z})");
            }

            if ((mFlags & 0x2) != 0)
            {
                float z = reader.ReadByte();

                output.Add($"Z Rotation: {z}");
            }

            if ((mFlags & 0x4) != 0)
            {
                float y = reader.ReadByte();

                output.Add($"Y Rotation: {y}");
            }

            if ((mFlags & 0x8) != 0)
            {
                float x = reader.ReadByte();

                output.Add($"X Rotation: {x}");
            }

            if ((mFlags & 0x10) != 0)
            {
                output.Add($"Set Animation: {reader.ReadStringNT()}");
            }

            if ((mFlags & 0x2000) != 0)
            {
                output.Add($"Animation Hash: {reader.ReadUInt32()}");
            }

            if ((mFlags & 0x20) != 0)
            {
                float val = reader.ReadInt16();
                output.Add($"Sets BCKCtrl value to: {val}");
            }

            for (int i = 0; i < 4; i++)
            {
                int val = 0x40 << i;
                val &= mFlags;

                if (val == 0)
                    continue;

                // we don't really do much with this one
                byte otherVal = reader.ReadByte();

                if (otherVal == 0x80)
                {
                    otherVal = 0x7F;
                }

                float weight = otherVal;

                output.Add($"Set Track Weight #{i} to {weight}");
            }

            if ((mFlags & 0x1000) != 0)
            {
                float rate = reader.ReadByte();
                output.Add($"Changed BCK Rate to: {rate}");
            }

            output.Add("=========================");
        }

        public byte PacketIndex
        {
            get
            {
                return mPacketIndex;
            }
        }

        public byte PacketLength
        {
            get
            {
                return mPacketLength;
            }
        }

        byte mPacketIndex;
        byte mPacketLength;
        short mFlags;
    }
}