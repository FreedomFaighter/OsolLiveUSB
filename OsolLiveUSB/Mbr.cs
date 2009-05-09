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

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace OsolLiveUSB
{

    class Mbr
    {
        static public ushort MBR_MAGIC = 0xAA55;
        static public uint STAGE2_ADDRESS = 0x8000;

        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct partition
        {
            [FieldOffset(0)]
            public fixed byte byteStream[16];
            [FieldOffset(0)]
            public byte p_boot;
            [FieldOffset(1)]
            public byte p_startHead;
            [FieldOffset(2)]
            public ushort p_startCylSec;
            [FieldOffset(4)]
            public byte p_id;
            [FieldOffset(5)]
            public byte p_endHead;
            [FieldOffset(6)]
            public ushort p_endCylSec;
            [FieldOffset(8)]
            public uint p_startLba;
            [FieldOffset(12)]
            public uint p_totalLba;

        }
        [StructLayout(LayoutKind.Explicit)]
        unsafe private struct mbr
        {
            [FieldOffset(0)]
            public fixed byte byteStream[512];
            [FieldOffset(64)]
            public byte p_bootDrive;
            [FieldOffset(65)]
            public byte p_forceLBA;
            [FieldOffset(66)]
            public ushort p_st2Adr;
            [FieldOffset(68)]
            public uint p_st2Sec;
            [FieldOffset(72)]
            public ushort p_st2Seg;
            [FieldOffset(446)]
            public fixed byte p_table[16 * 4];
            [FieldOffset(510)]
            public ushort p_magic;
        }

        public enum boot_indicator : byte
        {
            B_DONTBOOT = 0x00,
            B_BOOT = 0x80,
            B_SYSTEM = 0x80,
            B_ACTIVE = 0x80
        }
        public enum partition_id : byte
        {
            P_FAT32_LBA = 0x0C,
            P_SOLARIS = 0xBF
        }

        private mbr myMbr;

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


        public Mbr(byte[] imgMbr)
        {
            string strMbr = new string((char)0x00, 512 * 2);
            byte[] bufMbr = Encoding.ASCII.GetBytes(strMbr);

            this.myMbr = new mbr();

            unsafe
            {
                fixed (byte* src = imgMbr, tmpbuf = bufMbr, dst = myMbr.byteStream)
                {
                    MemCopy(tmpbuf, src, sizeof(mbr));
                    MemCopy(dst, tmpbuf, sizeof(mbr));
                }
            }

            this.SetMagic(MBR_MAGIC);

            this.SetBootDrive(0xFF);
            this.SetForceLBA(0x01);
            this.SetStage2Sector(4096 + 50);
            this.SetStage2Address(STAGE2_ADDRESS);

            this.SetPartition(0, 0, 0, 0, 0);
            this.SetPartition(1, 0, 0, 0, 0);
            this.SetPartition(2, 0, 0, 0, 0);
            this.SetPartition(3, 0, 0, 0, 0);

        }

        public void SetMagic(ushort signature)
        {
            this.myMbr.p_magic = signature;
        }

        public void SetBootDrive(byte bootDrive)
        {
            this.myMbr.p_bootDrive = bootDrive;
        }

        public void SetForceLBA(byte forceLBA)
        {
            this.myMbr.p_forceLBA = forceLBA;
        }

        public void SetStage2Sector(uint sector)
        {
            this.myMbr.p_st2Sec = sector;
        }
        public void SetStage2Address(uint address)
        {
            this.myMbr.p_st2Sec = (ushort)((address >> 4) & 0xffff);
            this.myMbr.p_st2Adr = (ushort)(address & 0xffff);
        }

        public void SetPartition(
            int p_no,
            byte p_boot,
            byte p_id,
            uint p_start,
            uint p_size)
        {
            partition pt = new partition();
            uint p_end = 0x00000000;

            ushort p_start_cyl = 0x0000;
            byte p_start_head = 0x00;
            ushort p_start_sec = 0x0000;

            ushort p_end_cyl = 0x0000;
            byte p_end_head = 0x00;
            ushort p_end_sec = 0x0000;


            if (p_size > 0)
            {
                p_end = p_start + p_size - 1;

                p_start_cyl = (ushort)(p_start / 16065);
                p_start_head = (byte)((p_start % 16065) / 63);
                p_start_sec = (ushort)(((p_start % 16065) % 63) + 1);

                p_end_cyl = (ushort)(p_end / 16065);
                p_end_head = (byte)((p_end % 16065) / 63);
                p_end_sec = (ushort)(((p_end % 16065) % 63) + 1);
            }

            if (p_start_cyl > 0x03FF)
            {
                pt.p_startCylSec = 0xFFFF;
                pt.p_startHead = 0xFF;
            }
            else
            {
                pt.p_startCylSec = (ushort)((p_start_cyl & 0x00FF) << 8);
                pt.p_startCylSec |= (ushort)((p_start_cyl & 0x0300) >> 2 | p_start_sec);
                pt.p_startHead = p_start_head;
            }

            if (p_end_cyl > 0x03FF)
            {
                pt.p_endCylSec = 0xFFFF;
                pt.p_endHead = 0xFF;
            }
            else
            {
                pt.p_endCylSec = (ushort)((p_end_cyl & 0x00FF) << 8);
                pt.p_endCylSec |= (ushort)((p_end_cyl & 0x0300) >> 2 | p_end_sec);
                pt.p_endHead = p_end_head;
            }

            pt.p_boot = p_boot;
            pt.p_id = p_id;
            pt.p_startLba = p_start;
            pt.p_totalLba = p_size;

            unsafe
            {
                fixed (byte* dst = this.myMbr.p_table)
                {
                    MemCopy(dst + (p_no * sizeof(partition)), pt.byteStream, sizeof(partition));
                }
            }
        }

        /// <summary>
        /// Dump ByteArray from this class
        /// </summary>
        /// <returns> Mbr contents ( byte[] ) </returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] retBuf = new byte[sizeof(mbr)];
            fixed (byte* src = myMbr.byteStream, dst = retBuf)
            {
                MemCopy(dst, src, sizeof(mbr));
            }
            return retBuf;
        }

    }

}
