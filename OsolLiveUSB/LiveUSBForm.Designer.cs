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

namespace OsolLiveUSB
{
    partial class LiveUSBForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveUSBForm));
            this.pgbWrt = new System.Windows.Forms.ProgressBar();
            this.logText = new System.Windows.Forms.TextBox();
            this.grpImageFile = new System.Windows.Forms.GroupBox();
            this.lblImgFile = new System.Windows.Forms.Label();
            this.browseBtn = new System.Windows.Forms.Button();
            this.grpTgtDrv = new System.Windows.Forms.GroupBox();
            this.cmbDrv = new System.Windows.Forms.ComboBox();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.lblSym = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.llblWeb = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.closeBtn = new System.Windows.Forms.Button();
            this.grpImageFile.SuspendLayout();
            this.grpTgtDrv.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pgbWrt
            // 
            resources.ApplyResources(this.pgbWrt, "pgbWrt");
            this.pgbWrt.Name = "pgbWrt";
            // 
            // logText
            // 
            this.logText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.logText.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.logText, "logText");
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            // 
            // grpImageFile
            // 
            this.grpImageFile.Controls.Add(this.lblImgFile);
            this.grpImageFile.Controls.Add(this.browseBtn);
            resources.ApplyResources(this.grpImageFile, "grpImageFile");
            this.grpImageFile.Name = "grpImageFile";
            this.grpImageFile.TabStop = false;
            // 
            // lblImgFile
            // 
            this.lblImgFile.AutoEllipsis = true;
            resources.ApplyResources(this.lblImgFile, "lblImgFile");
            this.lblImgFile.Name = "lblImgFile";
            // 
            // browseBtn
            // 
            resources.ApplyResources(this.browseBtn, "browseBtn");
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // grpTgtDrv
            // 
            this.grpTgtDrv.Controls.Add(this.cmbDrv);
            this.grpTgtDrv.Controls.Add(this.refreshBtn);
            resources.ApplyResources(this.grpTgtDrv, "grpTgtDrv");
            this.grpTgtDrv.Name = "grpTgtDrv";
            this.grpTgtDrv.TabStop = false;
            // 
            // cmbDrv
            // 
            this.cmbDrv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrv.FormattingEnabled = true;
            resources.ApplyResources(this.cmbDrv, "cmbDrv");
            this.cmbDrv.Name = "cmbDrv";
            this.cmbDrv.SelectedIndexChanged += new System.EventHandler(this.cmbDrv_SelectedIndexChanged);
            // 
            // refreshBtn
            // 
            resources.ApplyResources(this.refreshBtn, "refreshBtn");
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // startBtn
            // 
            resources.ApplyResources(this.startBtn, "startBtn");
            this.startBtn.Name = "startBtn";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // lblSym
            // 
            resources.ApplyResources(this.lblSym, "lblSym");
            this.lblSym.Name = "lblSym";
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.Name = "lblVersion";
            // 
            // llblWeb
            // 
            resources.ApplyResources(this.llblWeb, "llblWeb");
            this.llblWeb.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.llblWeb.Name = "llblWeb";
            this.llblWeb.TabStop = true;
            this.llblWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblWeb_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = global::OsolLiveUSB.Properties.Resources.opensolaris_bar;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // closeBtn
            // 
            resources.ApplyResources(this.closeBtn, "closeBtn");
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // LiveUSBForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.llblWeb);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblSym);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.grpTgtDrv);
            this.Controls.Add(this.grpImageFile);
            this.Controls.Add(this.logText);
            this.Controls.Add(this.pgbWrt);
            this.MaximizeBox = false;
            this.Name = "LiveUSBForm";
            this.grpImageFile.ResumeLayout(false);
            this.grpTgtDrv.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpImageFile;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.Label lblImgFile;
        private System.Windows.Forms.GroupBox grpTgtDrv;
        private System.Windows.Forms.ComboBox cmbDrv;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.Button startBtn;
        public System.Windows.Forms.ProgressBar pgbWrt;
        public System.Windows.Forms.TextBox logText;
        private System.Windows.Forms.Label lblSym;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel llblWeb;
        private System.Windows.Forms.Button closeBtn;
    }
}

