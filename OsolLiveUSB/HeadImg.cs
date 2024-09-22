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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace OsolLiveUSB
{
    class HeadImg
    {
        static int SectorPerCylinder = 4096;
        static int BytePerSector = 512;
        static int HeadCylinder = 2;
        static int headimgsize = BytePerSector * SectorPerCylinder * HeadCylinder;
        
        long totalsize;
        long totalsec;
        long totalcyl;
        long usbimgsize;
        long usbimgsec;
        long usbimgcyl;

        byte[] headbuf;
        byte[] BootRecord;
        byte[] DiskLabel;

        public HeadImg(long totalsize, long imgsize)
        {
            // Initialize Buffers
            string strHead = new string((char)0x00, headimgsize);
            this.headbuf = Encoding.ASCII.GetBytes(strHead);
            
            string strZeroSector = new string((char)0x00, BytePerSector);
            this.BootRecord = Encoding.ASCII.GetBytes(strZeroSector);
            this.DiskLabel = Encoding.ASCII.GetBytes(strZeroSector);

            // Set length
            this.totalsize = totalsize;
            this.totalsec = totalsize / BytePerSector ;
            this.totalcyl = this.totalsec / SectorPerCylinder ;

            this.usbimgsize = imgsize;
            this.usbimgsec = (imgsize / BytePerSector) + 1;
            this.usbimgcyl = (this.usbimgsec / SectorPerCylinder) + 1;

            // Generate BootRecord
            this.GenerateBootRecord();

            Array.Copy(
                this.BootRecord, 0,
                this.headbuf, 0,
                BytePerSector);
            Array.Copy(
                this.BootRecord, 0,
                this.headbuf, BytePerSector * SectorPerCylinder,
                BytePerSector);

            // Generate DiskLabel
            this.GenerateDiskLabel();

            Array.Copy(
                this.DiskLabel, 0,
                this.headbuf, BytePerSector * (SectorPerCylinder + 1),
                BytePerSector);


            // Copy stage2 Image
            byte[] stage2 = this.GetStage2Img();
            Array.Copy(
                stage2, 0,
                this.headbuf, BytePerSector * (SectorPerCylinder + 50),
                stage2.Length);
        }

        public void GenerateBootRecord()
        {
            byte[] stage1 = ReadStageFileFromResourceStream("OsolLiveUSB.Resources.stage1.gz");

            Mbr myMBR = new Mbr(stage1);

            myMBR.SetPartition(
                0,
                (byte)Mbr.boot_indicator.B_ACTIVE,
                (byte)Mbr.partition_id.P_SOLARIS,
                (uint)(1 * SectorPerCylinder),
                (uint)((this.totalcyl - 1) * SectorPerCylinder)
                );

            myMBR.SetBootDrive(0xFF);
            myMBR.SetForceLBA(0x01);
            myMBR.SetStage2Sector((uint)(SectorPerCylinder + 50));

            // Copy Boot Record image to my class buffer
            Array.Copy(myMBR.ToByteArray(), 0, this.BootRecord, 0, BytePerSector);
        }

        public void GenerateDiskLabel()
        {
            DkLabel myLbl = new DkLabel();

            
            // Slice 0 - ROOT
            myLbl.SetSlice(0,
                (ushort)DkLabel.vtoc_tag.V_ROOT,
                (ushort)DkLabel.vtoc_flag.V_NORMAL,
                (uint)4096,
                (uint)(this.usbimgcyl * SectorPerCylinder)
                );
            // Slice 2 - BACKUP
            myLbl.SetSlice(2,
                (ushort)DkLabel.vtoc_tag.V_BACKUP,
                (ushort)DkLabel.vtoc_flag.V_UNMOUNTABLE,
                (uint)0,
                (uint)((this.usbimgcyl+1) * SectorPerCylinder)
            );

            myLbl.SetPCyl((uint) ((this.totalsec - 4096) / 4096 ));
            myLbl.SetNCyl((uint) ((this.totalsec - 4096) / 4096 )-2);

            myLbl.SetAsciiLabel(
                string.Format(
                    "DEFAULT cyl {0} alt 2 hd 128 sec 32",
                    ((this.totalsec - 4096) / 4096 )-2 )
                );

            // Copy DiskLabel to my class buffer
            Array.Copy(myLbl.ToByteArray(), 0, this.DiskLabel, 0, 512);
            

        }

        public byte[] ReadStageFileFromResourceStream(String resourceName)
        {
            // Read Stagge2 image from manifest resource
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            Stream Stage2GZStream = assembly.GetManifestResourceStream(resourceName);
            GZipStream Stage2Stream = new GZipStream(
                Stage2GZStream, CompressionMode.Decompress, false);

            int readByte;
            byte[] bytesFromStream = new byte[Stage2Stream.Length];
            while ((readByte = Stage2Stream.ReadByte()) != -1)
                bytesFromStream.Append((byte)readByte);
            return bytesFromStream;
        }

        public byte[] GetStage2Img()
        {
            return ReadStageFileFromResourceStream("OsolLiveUSB.Resources.stage2.gz");
        }

        public byte[] GetByteArray()
        {
            return this.headbuf;
        }


    }
}
