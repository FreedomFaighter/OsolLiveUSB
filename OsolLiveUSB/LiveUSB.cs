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
using System.Collections;
using System.Management;
using System.Windows.Forms;

namespace OsolLiveUSB
{
    static class LiveUSB
    {
         
        /// Public variables:
        public static string strImgFile = String.Empty;
        public static DrvInfo curDrv = new DrvInfo();
        public static ArrayList arrDrv = new ArrayList();
        
       
        /// Public Methods:
        
        /// <summary>
        /// 
        /// </summary>
        
        public static void DetectUSBDrive()
        {
            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach ( ManagementObject mo in moc)
            {
                if ( mo.GetPropertyValue("InterfaceType").ToString() == "USB" ) {
                    DrvInfo drvInfo = new DrvInfo(
                        mo.GetPropertyValue("DeviceID").ToString(),
                        mo.GetPropertyValue("Model").ToString());
                    arrDrv.Add(drvInfo);
                }
            }
        }
        
        public static long GetFileSize( string fname )
        {
            if ( File.Exists( fname ) ) {
                FileInfo fi = new FileInfo(fname);
                return fi.Length;
            } else {
                return -1;                
            }         
        }

        private static System.Threading.Mutex _mutex;
        
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            _mutex = new System.Threading.Mutex(false, "OsolLiveUSB");
            if (_mutex.WaitOne(0, false) == false)
            {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LiveUSBForm());
        }
    }
}
