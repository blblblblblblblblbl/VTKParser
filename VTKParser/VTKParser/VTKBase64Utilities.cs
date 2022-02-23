using System;
using System.Collections.Generic;
using System.Text;

namespace PetroGM.DataIO.VTK
{
    class VTKBase64Utilities
    {
        //------------------------------------------------------------------------------
        string vtkBase64UtilitiesEncodeTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        //------------------------------------------------------------------------------
        byte vtkBase64UtilitiesEncodeChar(byte c)
        {
            if (c > 65)
            {
                throw new Exception();
            }

            return (byte) vtkBase64UtilitiesEncodeTable[c];
        }

        //------------------------------------------------------------------------------
        void EncodeTriplet(byte i0, byte i1, byte i2,
            out byte o0, out byte o1, out byte o2, out byte o3)
        {
            o0 = vtkBase64UtilitiesEncodeChar((byte) ((i0 >> 2) & 0x3F));
            o1 = vtkBase64UtilitiesEncodeChar((byte) (((i0 << 4) & 0x30) | ((i1 >> 4) & 0x0F)));
            o2 = vtkBase64UtilitiesEncodeChar((byte) (((i1 << 2) & 0x3C) | ((i2 >> 6) & 0x03)));
            o3 = vtkBase64UtilitiesEncodeChar((byte) (i2 & 0x3F));
        }

        //------------------------------------------------------------------------------
        void EncodePair(byte i0, byte i1, out byte o0,
            out byte o1, out byte o2, out byte o3)
        {
            o0 = vtkBase64UtilitiesEncodeChar((byte) ((i0 >> 2) & 0x3F));
            o1 = vtkBase64UtilitiesEncodeChar((byte) (((i0 << 4) & 0x30) | ((i1 >> 4) & 0x0F)));
            o2 = vtkBase64UtilitiesEncodeChar((byte) (((i1 << 2) & 0x3C)));
            o3 = (byte) '=';
        }

        //------------------------------------------------------------------------------
        void EncodeSingle(
            byte i0, out byte o0, out byte o1, out byte o2, out byte o3)
        {
            o0 = vtkBase64UtilitiesEncodeChar((byte) ((i0 >> 2) & 0x3F));
            o1 = vtkBase64UtilitiesEncodeChar((byte) ((i0 << 4) & 0x30));
            o2 = (byte) '=';
            o3 = (byte) '=';
        }

        //------------------------------------------------------------------------------
        public ulong Encode(byte[] input, ulong length, byte[] output, int mark_end)
        {
            ulong ptr = 0;
            ulong end = length;
            ulong optr = 0;

            // Encode complete triplet

            while ((end - ptr) >= 3)
            {
                EncodeTriplet(
                    input[ptr + 0], input[ptr + 1], input[ptr + 2], out output[optr + 0], out output[optr + 1],
                    out output[optr + 2], out output[optr + 3]);
                ptr += 3;
                optr += 4;
            }

            // Encodes a 2-byte ending into 3 bytes and 1 pad byte and writes.

            if (end - ptr == 2)
            {
                EncodePair(input[ptr + 0], input[ptr + 1], out output[optr + 0], out output[optr + 1],
                    out output[optr + 2], out output[optr + 3]);
                optr += 4;
            }

            // Encodes a 1-byte ending into 2 bytes and 2 pad bytes

            else if (end - ptr == 1)
            {
                EncodeSingle(input[ptr + 0], out output[optr + 0], out output[optr + 1], out output[optr + 2],
                    out output[optr + 3]);
                optr += 4;
            }

            // Do we need to mark the end

            else if (mark_end != 0)
            {
                output[optr + 0] = output[optr + 1] = output[optr + 2] = output[optr + 3] = (byte) '=';
                optr += 4;
            }

            return optr;
        }

        //------------------------------------------------------------------------------
        int[] vtkBase64UtilitiesDecodeTable = new[]
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0x3E, 0xFF, 0xFF, 0xFF, 0x3F, //
            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, //
            0x3C, 0x3D, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, //
            0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, //
            0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, //
            0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, //
            0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, //
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, //
            0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, //
            0x31, 0x32, 0x33, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            //-------------------------------------
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF //
        };

        //------------------------------------------------------------------------------
        int vtkBase64UtilitiesDecodeChar(byte c)
        {
            return vtkBase64UtilitiesDecodeTable[c];
        }

        //------------------------------------------------------------------------------
        int DecodeTriplet(byte i0, byte i1, byte i2,
            byte i3, out byte o0, out byte o1, out byte o2)
        {
            int d0, d1, d2, d3;

            d0 = vtkBase64UtilitiesDecodeChar(i0);
            d1 = vtkBase64UtilitiesDecodeChar(i1);
            d2 = vtkBase64UtilitiesDecodeChar(i2);
            d3 = vtkBase64UtilitiesDecodeChar(i3);

            // Make sure all characters were valid
            byte zerok = 0xFF;
            if (d0 == 0xFF || d1 == 0xFF || d2 == 0xFF || d3 == 0xFF)
            {
                o0 = 0;
                o1 = 0;
                o2 = 0;
                return 0;
            }

            // Decode the 3 bytes

            o0 = (byte) (((d0 << 2) & 0xFC) | ((d1 >> 4) & 0x03));
            o1 = (byte) (((d1 << 4) & 0xF0) | ((d2 >> 2) & 0x0F));
            o2 = (byte) (((d2 << 6) & 0xC0) | ((d3 >> 0) & 0x3F));

            // Return the number of bytes actually decoded

            if (i2 == '=')
            {
                return 1;
            }

            if (i3 == '=')
            {
                return 2;
            }

            return 3;
        }

        //------------------------------------------------------------------------------
        public uint DecodeSafely(in byte[] input, uint inputLen, byte[] output, uint outputLen)
        {
            if (input == null || output == null)
            {
                throw new Exception();
            }

            // Nonsense small input or no space for any output
            if ((inputLen < 4) || (outputLen == 0))
            {
                return 0;
            }

            // Consume 4 ASCII chars of input at a time, until less than 4 left
            uint inIdx = 0, outIdx = 0;
            while (inIdx <= inputLen - 4)
            {
                // Decode 4 ASCII characters into 0, 1, 2, or 3 bytes
                byte o0, o1, o2;
                int bytesDecoded = DecodeTriplet(
                    input[inIdx + 0], input[inIdx + 1], input[inIdx + 2], input[inIdx + 3], out o0, out o1, out o2);
                if (!((bytesDecoded >= 0) && (bytesDecoded <= 3)))
                {
                    throw new Exception();
                }

                if ((bytesDecoded >= 1) && (outIdx < outputLen))
                {
                    output[outIdx++] = o0;
                }

                if ((bytesDecoded >= 2) && (outIdx < outputLen))
                {
                    output[outIdx++] = o1;
                }

                if ((bytesDecoded >= 3) && (outIdx < outputLen))
                {
                    output[outIdx++] = o2;
                }

                // If fewer than 3 bytes resulted from decoding (in this pass),
                // then the input stream has nothing else decodable, so end.
                if (bytesDecoded < 3)
                {
                    return outIdx;
                }

                // Consumed a whole 4 of input and got 3 bytes of output, continue
                inIdx += 4;
                if (bytesDecoded != 3)
                {
                    throw new Exception();
                }
            }

            return outIdx;
        }
    }
}