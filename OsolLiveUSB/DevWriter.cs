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
using System.Collections.Generic;
using System.Text;

namespace OsolLiveUSB
{
    class DevWriter
    {
        private StringBuilder devname;
        private IntPtr hDev;

        public DevWriter( StringBuilder dn ) {
            devname = new StringBuilder(dn.ToString());

            hDev = RawIO.CreateFile(
                devname.ToString(),
                (uint) (    RawIO.DesiredAccess.GENERIC_READ |
                            RawIO.DesiredAccess.GENERIC_WRITE ),
                (uint) (    RawIO.ShareMode.FILE_SHARE_READ |
                            RawIO.ShareMode.FILE_SHARE_WRITE ),
                IntPtr.Zero,
                (uint) RawIO.CreationDisposition.OPEN_EXISTING,
                0,
                IntPtr.Zero
                );
        }

        public void DoWriteMB( uint nMB, byte[] buf) {
            uint len = 0;
            RawIO.SetFilePointer(
                hDev,
                nMB * 1024 * 1024,
                ref len,
                (uint) RawIO.MoveMethod.FileBegin);

            RawIO.WriteFile(
                hDev,
                buf, (uint)buf.Length,
                out len,
                IntPtr.Zero
                );
        }

        ~DevWriter() {
            RawIO.CloseHandle(hDev);
        }

    }
}