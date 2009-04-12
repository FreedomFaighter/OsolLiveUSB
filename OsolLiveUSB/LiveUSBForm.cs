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
using System.Text;
using System.Windows.Forms;

namespace OsolLiveUSB
{
    public partial class LiveUSBForm : Form
    {
        public void ChangelblImgFileText(String text)
        {
            if(lblImgFile.InvokeRequired)
            {
                lblImgFile.Invoke(new Action(() => { 
                    lblImgFile.Text = text;
                }));
            }
            else
            {
                lblImgFile.Text = text;
            }
        }

        public LiveUSBForm()
        {
            InitializeComponent();
            ChangelblImgFileText(LiveUSB.strImgFile);
            this.refreshDrv();
        }

        public void AppendLog(string strAppend)
        {
            if (this.logText.InvokeRequired)
            {
                this.logText.Invoke(new Action(() =>
                {
                    this.logText.AppendText(strAppend);
                    this.logText.AppendText(Environment.NewLine);
                }));
            }
            else
            {
                this.logText.AppendText(strAppend);
                this.logText.AppendText(Environment.NewLine);
            }
        }
        public void ChangecmbDrvSelectedIndex(int index)
        {
            if(cmbDrv.InvokeRequired)
            {
                cmbDrv.Invoke(new Action(() => { cmbDrv.SelectedIndex = index; }));
            }
            else
            {
                cmbDrv.SelectedIndex = index;
            }
        }

        private void ChangecmbDrvEnabled(bool  enabled)
        {
            if (cmbDrv.InvokeRequired)
            {
                cmbDrv.Invoke(new Action(() =>
                {
                    cmbDrv.Enabled = enabled;
                }));
            }
            else
            {
                cmbDrv.Enabled = enabled;
            }
        }

        private void refreshDrv()
        {
            LiveUSB.arrDrv.Clear();
            LiveUSB.DetectUSBDrive();
            if (cmbDrv.InvokeRequired)
            {
                cmbDrv.Invoke(new Action(() =>
                {
                    this.cmbDrv.Items.Clear();
                    this.cmbDrv.Items.AddRange(LiveUSB.arrDrv.ToArray());
                }));
            }
            else {
                this.cmbDrv.Items.Clear();
                this.cmbDrv.Items.AddRange(LiveUSB.arrDrv.ToArray()); 
            }

            if ( cmbDrv.Items.Count > 0 ) {
                ChangecmbDrvSelectedIndex(0);
                LiveUSB.curDrv = (DrvInfo)cmbDrv.SelectedItem;
                ChangecmbDrvEnabled(true);
            } else {
                ChangecmbDrvEnabled(false);
            }
        }

        private void EnableAll(bool flag)
        {
            ChangeButtonEnablement(this.startBtn, flag);
            ChangeButtonEnablement(this.closeBtn, flag);
            ChangeButtonEnablement(this.refreshBtn, flag);
            ChangeButtonEnablement(this.browseBtn, flag);
            ChangecmbDrvEnabled(flag);
        }

        private void ChangeButtonEnablement(Button b, bool flag)
        {
            if(b.InvokeRequired)
            {
                b.Invoke(new Action(() =>
                {
                    b.Enabled = flag;
                }));
            }
            else
            {
                b.Enabled = flag;
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgImgFile = new OpenFileDialog();
            dlgImgFile.Filter = "USB Image (*.usb)|*.usb|All File (*.*)|*.*";
            if (dlgImgFile.ShowDialog().HasFlag(DialogResult.OK))
            {
                LiveUSB.strImgFile = dlgImgFile.FileName;
                ChangelblImgFileText(dlgImgFile.FileName);

                if (this.cmbDrv.Items.Count > 0)
                {
                    ChangeButtonEnablement(this.startBtn, true);
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
                
                if (dr.HasFlag(DialogResult.No))
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

        private void SetpgbWrtValue(int value)
        {
            if(pgbWrt.InvokeRequired)
            {
                pgbWrt.Invoke(new Action(() =>
                {
                    this.pgbWrt.Value = value;
                }));
            }
            else
            {
                this.pgbWrt.Value = value;
            }
        }

        private void ChangelblSym(String text)
        {
            if(lblSym.InvokeRequired)
            {
                lblSym.Invoke(new Action(() =>
                {
                    lblSym.Text = text;
                }));
            }
            else
            {
                lblSym.Text = text;
            }
        }


        private void SetMinMaxOnpgbWrt(int mbmax)
        {
            if(pgbWrt.InvokeRequired)
            {
                pgbWrt.Invoke(new Action(() =>
                {
                    this.pgbWrt.Minimum = 0;
                    this.pgbWrt.Maximum = mbmax;
                }));
            }
            else
            {
                this.pgbWrt.Minimum = 0;
                this.pgbWrt.Maximum = mbmax;
            }
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

            SetMinMaxOnpgbWrt(mbmax);

            UseWaitCursor = true;

            // Writing Header Image

            HeadImg myHeadImg = new HeadImg(LiveUSB.curDrv.size, LiveUSB.GetFileSize(LiveUSB.strImgFile));
            byte[] bufHeadImg = myHeadImg.GetByteArray();

            dw.DoWrite(bufHeadImg, 0, (uint) bufHeadImg.Length);
            mb += 4;
            SetpgbWrtValue(mb);
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

                SetpgbWrtValue(mb++);
                ChangelblSym(chrSym[mb % 4]);
                Application.DoEvents();
            }
            
            ChangelblSym(" ");
            this.UseWaitCursor = false;
            Application.DoEvents();

        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ChangellblWebLinkVisited(bool visited)
        {
            if (llblWeb.InvokeRequired)
            {
                llblWeb.Invoke(new Action(() =>
                {
                    llblWeb.LinkVisited = visited;
                }));
            }
            else
            {
                llblWeb.LinkVisited = visited;
            }
        }
        private void llblWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ChangellblWebLinkVisited(true);
            System.Diagnostics.Process.Start("http://devzone.sites.pid0.org/OpenSolaris/opensolaris-liveusb-creator");
        }
    }
}
