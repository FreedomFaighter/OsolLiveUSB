﻿/*
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

namespace OsolLiveUSB
{
    class DrvInfo
    {
        public string devname = "";
        public string model = "";
        public long size = 0;

        public DrvInfo() {
            this.devname = "";
            this.model = "";
            this.size = 0;
        }

        public DrvInfo(string newdev) {
            this.devname = newdev;
            this.model = "";
            this.size = GetSize();
        }

        public DrvInfo(string newdev, string newmodel) {
            this.devname = newdev;
            this.model = newmodel;
            this.size = GetSize();
        }

        long GetSize() {
            byte[] obuf = new byte[255];
            bool result = false;
            uint outlen = 0;
            long length = 0;

            IntPtr hDrv = RawIO.CreateFile(
                devname,
                (uint) RawIO.DesiredAccess.GENERIC_READ,
                (uint) RawIO.ShareMode.FILE_SHARE_READ,
                IntPtr.Zero,
                (uint) RawIO.CreationDisposition.OPEN_EXISTING,
                0,
                IntPtr.Zero
                );

            if ( hDrv != new IntPtr(-1) ) {
                result = RawIO.DeviceIoControl(
                    hDrv, (uint)RawIO.IoControlCode.IOCTL_DISK_GET_LENGTH_INFO,
                    (new byte[0]), 0,
                    obuf, (uint)(obuf.Length),
                    out outlen, IntPtr.Zero
                    );

                if ( result ) {
                    length = (long)BitConverter.ToInt64(obuf, 0);
                }
                RawIO.CloseHandle(hDrv);
            }
            return length;
        }

        public override string ToString() {
            string numstr = devname.ToUpper();
            numstr = numstr.Replace("PHYSICALDRIVE", "");
            numstr = numstr.Replace(".", "");
            numstr = numstr.Replace("\\", "");
            return ( string.Format("{0}: {1}", numstr, model ) );
        }
    }
}
