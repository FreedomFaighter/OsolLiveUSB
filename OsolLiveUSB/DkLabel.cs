/*
 * CDDL HEADER START
 *
 * The contents of this file are subject to the terms of the
 * Common Development and Distribution License (the "License").
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the license at usr/src/OPENSOLARIS.LICENSE
 * or http://www.opensolaris.org/os/licensing.
 * See the License for the specific language governing permissions
 * and limitations under the License.
 *
 * When distributing Covered Code, include this CDDL HEADER in each
 * file and include the License file at usr/src/OPENSOLARIS.LICENSE.
 * If applicable, add the following below this CDDL HEADER, with the
 * fields enclosed by brackets "[]" replaced with your own identifying
 * information: Portions Copyright [yyyy] [name of copyright owner]
 *
 * CDDL HEADER END
 */

/*      Copyright (c) 2009 Hiroshi Chonan <chonan@pid0.org> */
/*        All Rights Reserved   */

using System.Runtime.InteropServices;
using System.Text;

namespace OsolLiveUSB
{

    class DkLabel
    {
        static public uint V_SANE = 0x600DDEEE;
        static public uint V_VERSION = 0x00000001;
        static public ushort DKL_MAGIC = 0xDABE;

        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct slice
        {
            [FieldOffset(0)]
            public fixed byte byteStream[12];
            [FieldOffset(0)]
            public ushort p_tag;
            [FieldOffset(2)]
            public ushort p_flag;
            [FieldOffset(4)]
            public uint p_start;
            [FieldOffset(8)]
            public uint p_size;
        }
        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct label
        {
            [FieldOffset(0)]
            public fixed byte byteStream[512];
            [FieldOffset(0)]
            public fixed uint v_bootinfo[3];
            [FieldOffset(12)]
            public uint v_sanity;
            [FieldOffset(16)]
            public uint v_version;
            [FieldOffset(20)]
            public fixed byte v_volume[8];
            [FieldOffset(28)]
            public ushort v_sectorsz;
            [FieldOffset(30)]
            public ushort v_nparts;
            [FieldOffset(32)]
            public fixed uint v_reserved[10];
            [FieldOffset(72)]
            public fixed byte v_slice[12 * 16];
            [FieldOffset(264)]
            public fixed uint v_timestamp[16];
            [FieldOffset(328)]
            public fixed byte v_asciilabel[128];
            [FieldOffset(456)]
            public uint dkl_pcyl;
            [FieldOffset(460)]
            public uint dkl_ncyl;
            [FieldOffset(464)]
            public ushort dkl_acyl;
            [FieldOffset(466)]
            public ushort dkl_bcyl;
            [FieldOffset(468)]
            public uint dkl_nhead;
            [FieldOffset(472)]
            public uint dkl_nsect;
            [FieldOffset(476)]
            public uint dkl_interlv;
            [FieldOffset(480)]
            public ushort dkl_skew;
            [FieldOffset(482)]
            public ushort dkl_apc;
            [FieldOffset(484)]
            public ushort dkl_rpm;
            [FieldOffset(486)]
            public ushort dkl_write_reinstruct;
            [FieldOffset(488)]
            public ushort dkl_read_reinstruct;
            [FieldOffset(490)]
            public fixed ushort dkl_extra[4];
            [FieldOffset(508)]
            public ushort dkl_magic;
            [FieldOffset(510)]
            public ushort dkl_cksum;
        }

        public enum vtoc_tag : ushort
        {
            V_UNASSIGNED = 0x0000,
            V_BOOT = 0x0001,
            V_ROOT = 0x0002,
            V_SWAP = 0x0003,
            V_USR = 0x0004,
            V_BACKUP = 0x0005,
            V_STAND = 0x0006,
            V_VAR = 0x0007,
            V_HOME = 0x0008,
            V_ALTSCTR = 0x0009,
            V_CACHE = 0x000A,
            V_RESERVED = 0x000B
        }
        public enum vtoc_flag : ushort
        {
            V_NORMAL = 0x0000,
            V_UNMOUNTABLE = 0x0001,
            V_READONLY = 0x0002
        }

        private label myLabel;

        private unsafe static void MemCopy(
            byte* dst,
            byte* src,
            int count)
        {
            while (count-- > 0)
            {
                *dst++ = *src++;
            }
        }

        unsafe private void calcCksum()
        {
            ushort cksum = 0x0000;
            ushort current = 0x0000;
            byte[] curbuf = new byte[2];

            fixed (byte* src = myLabel.byteStream, dst = curbuf)
            {
                for (int i = 0; i < sizeof(label) - 2; i += 2)
                {
                    MemCopy(dst, src + i, 2);
                    current = (ushort)((dst[1] << 8) | dst[0]);
                    cksum ^= current;
                }
            }

            this.myLabel.dkl_cksum = cksum;
        }

        /// <summary>
        /// DkLabel constructor
        /// </summary>
        public DkLabel()
        {
            this.myLabel = new label();

            // Setting Initial Values
            this.myLabel.v_sanity = V_SANE;
            this.myLabel.v_version = V_VERSION;
            this.myLabel.v_sectorsz = (ushort)512;
            this.myLabel.v_nparts = (ushort)16;

            this.myLabel.dkl_acyl = (ushort)2;               // Alternate cylinders
            this.myLabel.dkl_bcyl = (ushort)0;               // Cylinder Offset
            this.myLabel.dkl_nhead = (uint)128;              // Number of heads
            this.myLabel.dkl_nsect = (uint)32;               // Number of sector
            this.myLabel.dkl_interlv = (ushort)1;            // Interleave factor
            this.myLabel.dkl_skew = (ushort)0;               // Skew factor
            this.myLabel.dkl_apc = (ushort)0;                // Alternates per cylinder
            this.myLabel.dkl_rpm = (ushort)0;                // revolutions per cylinder
            this.myLabel.dkl_write_reinstruct = (ushort)0;   // Sector to skip(write)
            this.myLabel.dkl_read_reinstruct = (ushort)0;    // Sector to skip(read)
            this.myLabel.dkl_magic = DKL_MAGIC;              // Disklabel MagicNumber

            // Slice 8: Alternatice boot partition
            this.SetSlice(8, (ushort)vtoc_tag.V_BOOT, (ushort)vtoc_flag.V_UNMOUNTABLE, 0, 4096);         // Slice 8: Altanative

            // Generate Checksum
            this.calcCksum();
        }
        public void SetPCyl(uint PhyCyl)
        {
            this.myLabel.dkl_pcyl = PhyCyl;
            this.calcCksum();
        }
        public void SetNCyl(uint DatCyl)
        {
            this.myLabel.dkl_ncyl = DatCyl;
            this.calcCksum();
        }
        /// <summary>
        /// Dump ByteArray from this class
        /// </summary>
        /// <returns> Disklabel contents ( byte[] ) </returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] retBuf = new byte[sizeof(label)];

            fixed (byte* src = myLabel.byteStream, dst = retBuf)
            {
                MemCopy(dst, src, sizeof(label));
            }
            return retBuf;
        }

        /// <summary>
        ///  Set Slice parameters
        /// </summary>
        /// <param name="p_no">Slice number ( 0-15 )</param>
        /// <param name="p_tag">Slice tag</param>
        /// <param name="p_flag">Slice flag</param>
        /// <param name="p_start">Starting position</param>
        /// <param name="p_size">Slice Size</param>
        public void SetSlice(
            int p_no,
            ushort p_tag,
            ushort p_flag,
            uint p_start,
            uint p_size)
        {
            // Initialize Slice struct
            slice sl = new slice();

            // Setting Slice values
            sl.p_tag = p_tag;
            sl.p_flag = p_flag;
            sl.p_start = p_start;
            sl.p_size = p_size;

            // Copy Slice to Disklabel
            unsafe
            {
                fixed (byte* dst = myLabel.v_slice)
                {
                    MemCopy(dst + (p_no * sizeof(slice)), sl.byteStream, sizeof(slice));
                }
            }
            // Calc checksum again
            this.calcCksum();
        }

        public void SetAsciiLabel(string strLabel)
        {
            strLabel += new string((char)0x00, 128);
            strLabel = strLabel.Substring(0, 127);
            strLabel.PadRight(128, (char)0x00);

            byte[] buf = Encoding.ASCII.GetBytes(strLabel);

            unsafe
            {
                fixed (byte* dst = myLabel.v_asciilabel, src = buf)
                {
                    MemCopy(dst, src, 128);
                }
            }
            // Calc checksum again
            this.calcCksum();
        }
    }
}
