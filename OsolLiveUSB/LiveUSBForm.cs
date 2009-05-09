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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace OsolLiveUSB
{
    public partial class LiveUSBForm : Form
    {
        public LiveUSBForm()
        {
            InitializeComponent();
            this.lblImgFile.Text = LiveUSB.strImgFile;
            this.refreshDrv();
        }

        public void AppendLog(string strAppend)
        {
            this.logText.AppendText(strAppend);
            this.logText.AppendText(Environment.NewLine);
        }


        private void refreshDrv()
        {
            LiveUSB.arrDrv.Clear();
            LiveUSB.DetectUSBDrive();

            this.cmbDrv.Items.Clear();
            foreach ( DrvInfo drvname in LiveUSB.arrDrv ) {
                this.cmbDrv.Items.Add(drvname);
            }

            if ( cmbDrv.Items.Count > 0 ) {
                this.cmbDrv.SelectedIndex = 0;
                LiveUSB.curDrv = (DrvInfo)cmbDrv.SelectedItem;
                this.cmbDrv.Enabled = true;
            } else {
                this.cmbDrv.Enabled = false;
            }
        }

        private void EnableAll(bool flag)
        {
            this.startBtn.Enabled = flag;
            this.closeBtn.Enabled = flag;
            this.refreshBtn.Enabled = flag;
            this.browseBtn.Enabled = flag;
            this.cmbDrv.Enabled = flag;
        }


        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgImgFile = new OpenFileDialog();
            dlgImgFile.Filter = "USB Image (*.usb)|*.usb|All File (*.*)|*.*";
            if (dlgImgFile.ShowDialog() == DialogResult.OK )
            {
                LiveUSB.strImgFile = dlgImgFile.FileName;
                this.lblImgFile.Text = dlgImgFile.FileName;

                if (this.cmbDrv.Items.Count > 0)
                {
                    this.startBtn.Enabled = true;
                }
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            this.refreshDrv();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            DrvInfo drv = (DrvInfo)cmbDrv.SelectedItem;

            if ((Control.ModifierKeys & Keys.Shift ) != Keys.Shift )
            {
                DialogResult dr = MessageBox.Show(
                    string.Format(
                        "All of data stored in \n\"{0}\"\n will be destroyed.\n\nDo you continue the operation?",
                        drv.model
                    ),
                    "Write confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                    );
                
                if (dr == DialogResult.No)
                {
                    return;
                }
            }
                        
            this.EnableAll(false);

            this.AppendLog("**USB Image");
            
            // Get length of usb image file
            long imgSize = LiveUSB.GetFileSize(LiveUSB.strImgFile);
            if ( imgSize >= 0 ) {
                this.AppendLog(string.Format(" File: {0}", LiveUSB.strImgFile));
                this.AppendLog(string.Format(" Size: {0}", imgSize));
            } else {
                this.AppendLog(string.Format("**ERROR** Imgfile {0} not exist!", LiveUSB.strImgFile));
                EnableAll(true);
                return;
            }

            // Check Target Drive
            long devSize = drv.size;
            if ( devSize <= (imgSize + ( 1024*1024*8 ))) 
            {
                this.AppendLog("**ERROR** Device capacity is too small for USB Image");
                this.EnableAll(true);
                return;
            }
            


            this.AppendLog("** Target Device");
            this.AppendLog(" Device: " + drv.devname);
            this.AppendLog(" Model: " + drv.model);
            this.AppendLog(" Total Size: " + drv.size);

            this.AppendLog("");
            this.AppendLog("** Writting image to USB Stick ...");

            this.DoWrite();

            this.AppendLog("** Writting Completed !");

            MessageBox.Show(
                        "Operation Completed!!",
                        "Completed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );

            this.EnableAll(true);

        }

        private void cmbDrv_SelectedIndexChanged(object sender, EventArgs e)
        {
            LiveUSB.curDrv = (DrvInfo) cmbDrv.SelectedItem;
        }

        private void DoWrite()
        {
            string[] chrSym = { "|", "/", "-", "\\" };
            DevWriter dw = new DevWriter(new StringBuilder(LiveUSB.curDrv.devname));

            FileStream imgFileStream = new FileStream(
                LiveUSB.strImgFile, FileMode.Open, FileAccess.Read);

            byte[] buf = new byte[1024 * 1024];

            int mb = 0;
            int mbmax = (int)(LiveUSB.GetFileSize(LiveUSB.strImgFile) / 1024 / 1024) + 4;

            this.pgbWrt.Minimum = 0;
            this.pgbWrt.Maximum = mbmax;

            UseWaitCursor = true;

            // Writing Header Image

            HeadImg myHeadImg = new HeadImg(LiveUSB.curDrv.size, LiveUSB.GetFileSize(LiveUSB.strImgFile));
            byte[] bufHeadImg = myHeadImg.GetByteArray();

            dw.DoWrite(bufHeadImg, 0, (uint) bufHeadImg.Length);
            mb += 4;
            this.pgbWrt.Value = mb;
            Application.DoEvents();

            // Writing USB Image
            
            while (true)
            {
                int readSize = imgFileStream.Read(buf, 0, buf.Length);

                if (readSize == 0)
                {
                    break;
                }
                dw.DoWriteMB((uint)mb, buf);

                this.pgbWrt.Value = mb++;
                this.lblSym.Text = chrSym[mb % 4];
                Application.DoEvents();
            }
            
            this.lblSym.Text = " ";
            this.UseWaitCursor = false;
            Application.DoEvents();

        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
           
        }

        private void llblWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.llblWeb.LinkVisited = true;
            System.Diagnostics.Process.Start("http://devzone.sites.pid0.org/OpenSolaris/opensolaris-liveusb-creator");

        }


    }
}
