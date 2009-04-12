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
            lblImgFile.Text = LiveUSB.strImgFile;
            refreshDrv();
            cmbCap.SelectedIndex = 0;
            LiveUSB.strHeadImgFile = string.Format(
                "{0}\\headimg\\{1}.dat",
                Application.StartupPath,
                cmbCap.SelectedIndex
            );
        }

        public void AppendLog(string strAppend)
        {
            logText.AppendText(strAppend);
            logText.AppendText(Environment.NewLine);
        }


        private void refreshDrv()
        {
            LiveUSB.arrDrv.Clear();
            LiveUSB.DetectUSBDrive();

            cmbDrv.Items.Clear();
            foreach ( DrvInfo drvname in LiveUSB.arrDrv ) {
                cmbDrv.Items.Add(drvname);
            }

            if ( cmbDrv.Items.Count > 0 ) {
                cmbDrv.SelectedIndex = 0;
                LiveUSB.curDrv = (DrvInfo)cmbDrv.SelectedItem;
                cmbDrv.Enabled = true;
            } else {
                cmbDrv.Enabled = false;
            }
        }

        private void EnableAll(bool flag)
        {
            createBtn.Enabled = flag;
            refreshBtn.Enabled = flag;
            browseBtn.Enabled = flag;
            cmbCap.Enabled = flag;
            cmbDrv.Enabled = flag;
        }


        private void browseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgImgFile = new OpenFileDialog();
            dlgImgFile.Filter = "USB Image (*.usb)|*.usb|All File (*.*)|*.*";
            if (dlgImgFile.ShowDialog() == DialogResult.OK )
            {
                LiveUSB.strImgFile = dlgImgFile.FileName;
                lblImgFile.Text = dlgImgFile.FileName;

                if (cmbDrv.Items.Count > 0)
                {
                    createBtn.Enabled = true;
                }
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            refreshDrv();
        }

        private void createBtn_Click(object sender, EventArgs e)
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
                        
            EnableAll(false);

            logText.AppendText("**USB Image");
            logText.AppendText(Environment.NewLine);
            
            // Get length of usb image file
            long imgSize = LiveUSB.GetFileSize(LiveUSB.strImgFile);
            if ( imgSize >= 0 ) {
                logText.AppendText(string.Format(" File: {0}", LiveUSB.strImgFile));
                logText.AppendText(Environment.NewLine);
                logText.AppendText(string.Format(" Size: {0}", imgSize));
                logText.AppendText(Environment.NewLine);
            } else {
                logText.AppendText(string.Format("**ERROR** Imgfile {0} not exist!", LiveUSB.strImgFile));
                logText.AppendText(Environment.NewLine);
                EnableAll(true);
                return;
            }

            // Check Target Drive
            long devSize = drv.size;
            long tmplSize = (long)(1 << (cmbCap.SelectedIndex)) * 1000000000L;
            
            if ( drv.size <= tmplSize ) {
                logText.AppendText("**ERROR** Device capacity is too small for template");
                EnableAll(true);
                return;
            }

            if ( tmplSize <= (imgSize + ( 1024*1024*4 ))) 
            {
                logText.AppendText("**ERROR** Template capacity is too small for USB Image");
                EnableAll(true);
                return;
            }


            logText.AppendText("** Target Device");
            logText.AppendText(Environment.NewLine);
            logText.AppendText(" Device: " + drv.devname);
            logText.AppendText(Environment.NewLine);
            logText.AppendText(" Model: " + drv.model);
            logText.AppendText(Environment.NewLine);
            logText.AppendText(" Total Size: " + drv.size);
            logText.AppendText(Environment.NewLine);

            logText.AppendText(Environment.NewLine);
            logText.AppendText("** Writting image to USB Stick ...");
            logText.AppendText(Environment.NewLine);

            DoWrite();

            logText.AppendText("** Writting Completed !");
            logText.AppendText(Environment.NewLine);

            MessageBox.Show(
                        "Operation Completed!!",
                        "Completed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );

            EnableAll(true);

        }

        private void cmbCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            LiveUSB.strHeadImgFile = string.Format(
                "{0}\\headimg\\{1}.dat",
                Application.StartupPath,
                cmbCap.SelectedIndex
                );
        }

        private void cmbDrv_SelectedIndexChanged(object sender, EventArgs e)
        {
            LiveUSB.curDrv = (DrvInfo) cmbDrv.SelectedItem;
        }

        private void DoWrite()
        {
            string[] chrSym = { "|", "/", "-", "\\" };
            DevWriter dw = new DevWriter(new StringBuilder(LiveUSB.curDrv.devname));

            FileStream hdFileStream = new FileStream(
                LiveUSB.strHeadImgFile, FileMode.Open, FileAccess.Read);

            GZipStream hdGzipStream = new GZipStream(
                hdFileStream, CompressionMode.Decompress);

            FileStream imgFileStream = new FileStream(
                LiveUSB.strImgFile, FileMode.Open, FileAccess.Read);

            byte[] buf = new byte[1024 * 1024];

            int mb = 0;
            int mbmax = (int)(LiveUSB.GetFileSize(LiveUSB.strImgFile) / 1024 / 1024) + 4;

            pgbWrt.Minimum = 0;
            pgbWrt.Maximum = mbmax;

            UseWaitCursor = true;

            while (true)
            {
                int readSize = hdGzipStream.Read(buf, 0, buf.Length);

                if (readSize == 0)
                {
                    break;
                }
                dw.DoWriteMB((uint)mb, buf);

                pgbWrt.Value = mb++;
                lblSym.Text = chrSym[mb % 4];
                Application.DoEvents();
            }
            while (true)
            {
                int readSize = imgFileStream.Read(buf, 0, buf.Length);

                if (readSize == 0)
                {
                    break;
                }
                dw.DoWriteMB((uint)mb, buf);

                pgbWrt.Value = mb++;
                lblSym.Text = chrSym[mb % 4];
                Application.DoEvents();
            }
            lblSym.Text = " ";
            UseWaitCursor = false;

        }

        private void llblWeb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llblWeb.LinkVisited = true;
            System.Diagnostics.Process.Start("http://devzone.sites.pid0.org/OpenSolaris/opensolaris-liveusb-creator");

        }
    }
}
