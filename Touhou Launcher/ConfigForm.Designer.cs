namespace Touhou_Launcher
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.openFolder = new System.Windows.Forms.Button();
            this.openvpatch = new System.Windows.Forms.Button();
            this.jpDir = new System.Windows.Forms.TextBox();
            this.enDir = new System.Windows.Forms.TextBox();
            this.customDir = new System.Windows.Forms.TextBox();
            this.browseJP = new System.Windows.Forms.Button();
            this.jpLabel = new System.Windows.Forms.Label();
            this.enLabel = new System.Windows.Forms.Label();
            this.customLabel = new System.Windows.Forms.Label();
            this.openReplays = new System.Windows.Forms.Button();
            this.browseEN = new System.Windows.Forms.Button();
            this.browseCustom = new System.Windows.Forms.Button();
            this.jpApplocale = new System.Windows.Forms.CheckBox();
            this.defaultExec = new System.Windows.Forms.ComboBox();
            this.defaultLabel = new System.Windows.Forms.Label();
            this.launchCustom = new System.Windows.Forms.Button();
            this.launchEN = new System.Windows.Forms.Button();
            this.launchJP = new System.Windows.Forms.Button();
            this.launchcrap = new System.Windows.Forms.Button();
            this.crapLabel = new System.Windows.Forms.Label();
            this.customApplocale = new System.Windows.Forms.CheckBox();
            this.defaultApplocale = new System.Windows.Forms.CheckBox();
            this.crapApplocale = new System.Windows.Forms.CheckBox();
            this.enApplocale = new System.Windows.Forms.CheckBox();
            this.chkCustomBanner = new System.Windows.Forms.CheckBox();
            this.bannerOnLabel = new System.Windows.Forms.Label();
            this.browseBannerOn = new System.Windows.Forms.Button();
            this.bannerOnDir = new System.Windows.Forms.TextBox();
            this.bannerOffLabel = new System.Windows.Forms.Label();
            this.browseBannerOff = new System.Windows.Forms.Button();
            this.bannerOffDir = new System.Windows.Forms.TextBox();
            this.pc98Settings = new System.Windows.Forms.GroupBox();
            this.openNP2Folder = new System.Windows.Forms.Button();
            this.launchHDI = new System.Windows.Forms.Button();
            this.hdiLabel = new System.Windows.Forms.Label();
            this.browseHDI = new System.Windows.Forms.Button();
            this.hdiDir = new System.Windows.Forms.TextBox();
            this.bannerSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.windowsSettings = new System.Windows.Forms.GroupBox();
            this.crapCfg = new System.Windows.Forms.ComboBox();
            this.crapGame = new System.Windows.Forms.ComboBox();
            this.openAppdata = new System.Windows.Forms.Button();
            this.pc98Settings.SuspendLayout();
            this.bannerSettings.SuspendLayout();
            this.windowsSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFolder
            // 
            this.openFolder.Location = new System.Drawing.Point(337, 235);
            this.openFolder.Name = "openFolder";
            this.openFolder.Size = new System.Drawing.Size(242, 23);
            this.openFolder.TabIndex = 0;
            this.openFolder.Text = "Open Folder";
            this.openFolder.UseVisualStyleBackColor = true;
            this.openFolder.Click += new System.EventHandler(this.openFolder_Click);
            // 
            // openvpatch
            // 
            this.openvpatch.Location = new System.Drawing.Point(231, 227);
            this.openvpatch.Name = "openvpatch";
            this.openvpatch.Size = new System.Drawing.Size(91, 23);
            this.openvpatch.TabIndex = 1;
            this.openvpatch.Text = "Edit vpatch.ini";
            this.openvpatch.UseVisualStyleBackColor = true;
            this.openvpatch.Click += new System.EventHandler(this.openvpatch_Click);
            // 
            // jpDir
            // 
            this.jpDir.AllowDrop = true;
            this.jpDir.Location = new System.Drawing.Point(9, 32);
            this.jpDir.Name = "jpDir";
            this.jpDir.Size = new System.Drawing.Size(181, 20);
            this.jpDir.TabIndex = 2;
            this.jpDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.jpDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.jpDir.LostFocus += new System.EventHandler(this.Dir_LostFocus);
            // 
            // enDir
            // 
            this.enDir.AllowDrop = true;
            this.enDir.Location = new System.Drawing.Point(9, 80);
            this.enDir.Name = "enDir";
            this.enDir.Size = new System.Drawing.Size(181, 20);
            this.enDir.TabIndex = 3;
            this.enDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.enDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.enDir.LostFocus += new System.EventHandler(this.Dir_LostFocus);
            // 
            // customDir
            // 
            this.customDir.AllowDrop = true;
            this.customDir.Location = new System.Drawing.Point(9, 128);
            this.customDir.Name = "customDir";
            this.customDir.Size = new System.Drawing.Size(181, 20);
            this.customDir.TabIndex = 4;
            this.customDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.customDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.customDir.LostFocus += new System.EventHandler(this.Dir_LostFocus);
            // 
            // browseJP
            // 
            this.browseJP.Location = new System.Drawing.Point(196, 32);
            this.browseJP.Name = "browseJP";
            this.browseJP.Size = new System.Drawing.Size(62, 22);
            this.browseJP.TabIndex = 5;
            this.browseJP.Text = "Browse";
            this.browseJP.UseVisualStyleBackColor = true;
            this.browseJP.Click += new System.EventHandler(this.browse_Click);
            // 
            // jpLabel
            // 
            this.jpLabel.AutoSize = true;
            this.jpLabel.Location = new System.Drawing.Point(6, 16);
            this.jpLabel.Name = "jpLabel";
            this.jpLabel.Size = new System.Drawing.Size(150, 13);
            this.jpLabel.TabIndex = 6;
            this.jpLabel.Text = "Japanese/vpatch Executable:";
            // 
            // enLabel
            // 
            this.enLabel.AutoSize = true;
            this.enLabel.Location = new System.Drawing.Point(6, 64);
            this.enLabel.Name = "enLabel";
            this.enLabel.Size = new System.Drawing.Size(100, 13);
            this.enLabel.TabIndex = 7;
            this.enLabel.Text = "English Executable:";
            // 
            // customLabel
            // 
            this.customLabel.AutoSize = true;
            this.customLabel.Location = new System.Drawing.Point(6, 112);
            this.customLabel.Name = "customLabel";
            this.customLabel.Size = new System.Drawing.Size(65, 13);
            this.customLabel.TabIndex = 8;
            this.customLabel.Text = "Custom.exe:";
            // 
            // openReplays
            // 
            this.openReplays.Location = new System.Drawing.Point(106, 227);
            this.openReplays.Name = "openReplays";
            this.openReplays.Size = new System.Drawing.Size(119, 23);
            this.openReplays.TabIndex = 9;
            this.openReplays.Text = "Open Replays Folder";
            this.openReplays.UseVisualStyleBackColor = true;
            this.openReplays.Click += new System.EventHandler(this.openReplays_Click);
            // 
            // browseEN
            // 
            this.browseEN.Location = new System.Drawing.Point(196, 80);
            this.browseEN.Name = "browseEN";
            this.browseEN.Size = new System.Drawing.Size(62, 22);
            this.browseEN.TabIndex = 11;
            this.browseEN.Text = "Browse";
            this.browseEN.UseVisualStyleBackColor = true;
            this.browseEN.Click += new System.EventHandler(this.browse_Click);
            // 
            // browseCustom
            // 
            this.browseCustom.Location = new System.Drawing.Point(196, 126);
            this.browseCustom.Name = "browseCustom";
            this.browseCustom.Size = new System.Drawing.Size(62, 22);
            this.browseCustom.TabIndex = 12;
            this.browseCustom.Text = "Browse";
            this.browseCustom.UseVisualStyleBackColor = true;
            this.browseCustom.Click += new System.EventHandler(this.browse_Click);
            // 
            // jpApplocale
            // 
            this.jpApplocale.AutoSize = true;
            this.jpApplocale.Location = new System.Drawing.Point(221, 12);
            this.jpApplocale.Name = "jpApplocale";
            this.jpApplocale.Size = new System.Drawing.Size(98, 17);
            this.jpApplocale.TabIndex = 14;
            this.jpApplocale.Text = "With Applocale";
            this.jpApplocale.UseVisualStyleBackColor = true;
            this.jpApplocale.CheckedChanged += new System.EventHandler(this.Applocale_CheckedChanged);
            // 
            // defaultExec
            // 
            this.defaultExec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defaultExec.FormattingEnabled = true;
            this.defaultExec.Items.AddRange(new object[] {
            "Japanese/vpatch.ini",
            "English (non-THCRAP)",
            "Custom.exe",
            "thcrap"});
            this.defaultExec.Location = new System.Drawing.Point(109, 198);
            this.defaultExec.Name = "defaultExec";
            this.defaultExec.Size = new System.Drawing.Size(113, 21);
            this.defaultExec.TabIndex = 15;
            this.defaultExec.SelectedIndexChanged += new System.EventHandler(this.defaultExec_SelectedIndexChanged);
            // 
            // defaultLabel
            // 
            this.defaultLabel.AutoSize = true;
            this.defaultLabel.Location = new System.Drawing.Point(6, 198);
            this.defaultLabel.Name = "defaultLabel";
            this.defaultLabel.Size = new System.Drawing.Size(100, 13);
            this.defaultLabel.TabIndex = 16;
            this.defaultLabel.Text = "Default Executable:";
            // 
            // launchCustom
            // 
            this.launchCustom.Location = new System.Drawing.Point(264, 126);
            this.launchCustom.Name = "launchCustom";
            this.launchCustom.Size = new System.Drawing.Size(55, 22);
            this.launchCustom.TabIndex = 17;
            this.launchCustom.Text = "Launch";
            this.launchCustom.UseVisualStyleBackColor = true;
            this.launchCustom.Click += new System.EventHandler(this.launch_Click);
            // 
            // launchEN
            // 
            this.launchEN.Location = new System.Drawing.Point(264, 80);
            this.launchEN.Name = "launchEN";
            this.launchEN.Size = new System.Drawing.Size(55, 22);
            this.launchEN.TabIndex = 18;
            this.launchEN.Text = "Launch";
            this.launchEN.UseVisualStyleBackColor = true;
            this.launchEN.Click += new System.EventHandler(this.launch_Click);
            // 
            // launchJP
            // 
            this.launchJP.Location = new System.Drawing.Point(264, 32);
            this.launchJP.Name = "launchJP";
            this.launchJP.Size = new System.Drawing.Size(55, 22);
            this.launchJP.TabIndex = 19;
            this.launchJP.Text = "Launch";
            this.launchJP.UseVisualStyleBackColor = true;
            this.launchJP.Click += new System.EventHandler(this.launch_Click);
            // 
            // launchcrap
            // 
            this.launchcrap.Location = new System.Drawing.Point(264, 171);
            this.launchcrap.Name = "launchcrap";
            this.launchcrap.Size = new System.Drawing.Size(55, 22);
            this.launchcrap.TabIndex = 23;
            this.launchcrap.Text = "Launch";
            this.launchcrap.UseVisualStyleBackColor = true;
            this.launchcrap.Click += new System.EventHandler(this.launchcrap_Click);
            // 
            // crapLabel
            // 
            this.crapLabel.AutoSize = true;
            this.crapLabel.Location = new System.Drawing.Point(6, 155);
            this.crapLabel.Name = "crapLabel";
            this.crapLabel.Size = new System.Drawing.Size(72, 13);
            this.crapLabel.TabIndex = 21;
            this.crapLabel.Text = "thcrap Profile:";
            // 
            // customApplocale
            // 
            this.customApplocale.AutoSize = true;
            this.customApplocale.Location = new System.Drawing.Point(221, 108);
            this.customApplocale.Name = "customApplocale";
            this.customApplocale.Size = new System.Drawing.Size(98, 17);
            this.customApplocale.TabIndex = 24;
            this.customApplocale.Text = "With Applocale";
            this.customApplocale.UseVisualStyleBackColor = true;
            this.customApplocale.CheckedChanged += new System.EventHandler(this.Applocale_CheckedChanged);
            // 
            // defaultApplocale
            // 
            this.defaultApplocale.AutoSize = true;
            this.defaultApplocale.Location = new System.Drawing.Point(228, 202);
            this.defaultApplocale.Name = "defaultApplocale";
            this.defaultApplocale.Size = new System.Drawing.Size(98, 17);
            this.defaultApplocale.TabIndex = 25;
            this.defaultApplocale.Text = "With Applocale";
            this.defaultApplocale.UseVisualStyleBackColor = true;
            this.defaultApplocale.CheckedChanged += new System.EventHandler(this.defaultApplocale_CheckedChanged);
            // 
            // crapApplocale
            // 
            this.crapApplocale.AutoSize = true;
            this.crapApplocale.Location = new System.Drawing.Point(221, 154);
            this.crapApplocale.Name = "crapApplocale";
            this.crapApplocale.Size = new System.Drawing.Size(98, 17);
            this.crapApplocale.TabIndex = 26;
            this.crapApplocale.Text = "With Applocale";
            this.crapApplocale.UseVisualStyleBackColor = true;
            this.crapApplocale.CheckedChanged += new System.EventHandler(this.Applocale_CheckedChanged);
            // 
            // enApplocale
            // 
            this.enApplocale.AutoSize = true;
            this.enApplocale.Location = new System.Drawing.Point(221, 60);
            this.enApplocale.Name = "enApplocale";
            this.enApplocale.Size = new System.Drawing.Size(98, 17);
            this.enApplocale.TabIndex = 27;
            this.enApplocale.Text = "With Applocale";
            this.enApplocale.UseVisualStyleBackColor = true;
            this.enApplocale.CheckedChanged += new System.EventHandler(this.Applocale_CheckedChanged);
            // 
            // chkCustomBanner
            // 
            this.chkCustomBanner.AutoSize = true;
            this.chkCustomBanner.Location = new System.Drawing.Point(120, 12);
            this.chkCustomBanner.Name = "chkCustomBanner";
            this.chkCustomBanner.Size = new System.Drawing.Size(118, 17);
            this.chkCustomBanner.TabIndex = 28;
            this.chkCustomBanner.Text = "Use custom banner";
            this.chkCustomBanner.UseVisualStyleBackColor = true;
            // 
            // bannerOnLabel
            // 
            this.bannerOnLabel.AutoSize = true;
            this.bannerOnLabel.Location = new System.Drawing.Point(6, 16);
            this.bannerOnLabel.Name = "bannerOnLabel";
            this.bannerOnLabel.Size = new System.Drawing.Size(83, 13);
            this.bannerOnLabel.TabIndex = 31;
            this.bannerOnLabel.Text = "Colored Banner:";
            // 
            // browseBannerOn
            // 
            this.browseBannerOn.Location = new System.Drawing.Point(176, 30);
            this.browseBannerOn.Name = "browseBannerOn";
            this.browseBannerOn.Size = new System.Drawing.Size(62, 22);
            this.browseBannerOn.TabIndex = 30;
            this.browseBannerOn.Text = "Browse";
            this.browseBannerOn.UseVisualStyleBackColor = true;
            this.browseBannerOn.Click += new System.EventHandler(this.browseBannerOn_Click);
            // 
            // bannerOnDir
            // 
            this.bannerOnDir.AllowDrop = true;
            this.bannerOnDir.Location = new System.Drawing.Point(9, 32);
            this.bannerOnDir.Name = "bannerOnDir";
            this.bannerOnDir.Size = new System.Drawing.Size(161, 20);
            this.bannerOnDir.TabIndex = 29;
            this.bannerOnDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.bannerOnDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.bannerOnDir.LostFocus += new System.EventHandler(this.bannerDir_LostFocus);
            // 
            // bannerOffLabel
            // 
            this.bannerOffLabel.AutoSize = true;
            this.bannerOffLabel.Location = new System.Drawing.Point(6, 64);
            this.bannerOffLabel.Name = "bannerOffLabel";
            this.bannerOffLabel.Size = new System.Drawing.Size(69, 13);
            this.bannerOffLabel.TabIndex = 34;
            this.bannerOffLabel.Text = "Grey Banner:";
            // 
            // browseBannerOff
            // 
            this.browseBannerOff.Location = new System.Drawing.Point(176, 78);
            this.browseBannerOff.Name = "browseBannerOff";
            this.browseBannerOff.Size = new System.Drawing.Size(62, 22);
            this.browseBannerOff.TabIndex = 33;
            this.browseBannerOff.Text = "Browse";
            this.browseBannerOff.UseVisualStyleBackColor = true;
            this.browseBannerOff.Click += new System.EventHandler(this.browseBannerOff_Click);
            // 
            // bannerOffDir
            // 
            this.bannerOffDir.AllowDrop = true;
            this.bannerOffDir.Location = new System.Drawing.Point(9, 80);
            this.bannerOffDir.Name = "bannerOffDir";
            this.bannerOffDir.Size = new System.Drawing.Size(161, 20);
            this.bannerOffDir.TabIndex = 32;
            this.bannerOffDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.bannerOffDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.bannerOffDir.LostFocus += new System.EventHandler(this.bannerDir_LostFocus);
            // 
            // pc98Settings
            // 
            this.pc98Settings.Controls.Add(this.openNP2Folder);
            this.pc98Settings.Controls.Add(this.launchHDI);
            this.pc98Settings.Controls.Add(this.hdiLabel);
            this.pc98Settings.Controls.Add(this.browseHDI);
            this.pc98Settings.Controls.Add(this.hdiDir);
            this.pc98Settings.Location = new System.Drawing.Point(335, 119);
            this.pc98Settings.Name = "pc98Settings";
            this.pc98Settings.Size = new System.Drawing.Size(244, 115);
            this.pc98Settings.TabIndex = 35;
            this.pc98Settings.TabStop = false;
            this.pc98Settings.Text = "PC-98 Game Settings";
            // 
            // openNP2Folder
            // 
            this.openNP2Folder.Location = new System.Drawing.Point(9, 88);
            this.openNP2Folder.Name = "openNP2Folder";
            this.openNP2Folder.Size = new System.Drawing.Size(229, 23);
            this.openNP2Folder.TabIndex = 37;
            this.openNP2Folder.Text = "Open Neko Project II Folder";
            this.openNP2Folder.UseVisualStyleBackColor = true;
            this.openNP2Folder.Click += new System.EventHandler(this.openNP2Folder_Click);
            // 
            // launchHDI
            // 
            this.launchHDI.Location = new System.Drawing.Point(183, 66);
            this.launchHDI.Name = "launchHDI";
            this.launchHDI.Size = new System.Drawing.Size(55, 22);
            this.launchHDI.TabIndex = 23;
            this.launchHDI.Text = "Launch";
            this.launchHDI.UseVisualStyleBackColor = true;
            this.launchHDI.Click += new System.EventHandler(this.launchHDI_Click);
            // 
            // hdiLabel
            // 
            this.hdiLabel.AutoSize = true;
            this.hdiLabel.Location = new System.Drawing.Point(6, 24);
            this.hdiLabel.Name = "hdiLabel";
            this.hdiLabel.Size = new System.Drawing.Size(60, 13);
            this.hdiLabel.TabIndex = 22;
            this.hdiLabel.Text = "Game HDI:";
            // 
            // browseHDI
            // 
            this.browseHDI.Location = new System.Drawing.Point(9, 66);
            this.browseHDI.Name = "browseHDI";
            this.browseHDI.Size = new System.Drawing.Size(62, 22);
            this.browseHDI.TabIndex = 21;
            this.browseHDI.Text = "Browse";
            this.browseHDI.UseVisualStyleBackColor = true;
            this.browseHDI.Click += new System.EventHandler(this.browseHDI_Click);
            // 
            // hdiDir
            // 
            this.hdiDir.AllowDrop = true;
            this.hdiDir.Location = new System.Drawing.Point(9, 40);
            this.hdiDir.Name = "hdiDir";
            this.hdiDir.Size = new System.Drawing.Size(229, 20);
            this.hdiDir.TabIndex = 20;
            this.hdiDir.DragDrop += new System.Windows.Forms.DragEventHandler(this.Dir_DragDrop);
            this.hdiDir.DragEnter += new System.Windows.Forms.DragEventHandler(this.Dir_DragEnter);
            this.hdiDir.LostFocus += new System.EventHandler(this.Dir_LostFocus);
            // 
            // bannerSettings
            // 
            this.bannerSettings.Controls.Add(this.label1);
            this.bannerSettings.Controls.Add(this.bannerOnLabel);
            this.bannerSettings.Controls.Add(this.chkCustomBanner);
            this.bannerSettings.Controls.Add(this.bannerOffLabel);
            this.bannerSettings.Controls.Add(this.bannerOnDir);
            this.bannerSettings.Controls.Add(this.browseBannerOff);
            this.bannerSettings.Controls.Add(this.browseBannerOn);
            this.bannerSettings.Controls.Add(this.bannerOffDir);
            this.bannerSettings.Location = new System.Drawing.Point(335, 3);
            this.bannerSettings.Name = "bannerSettings";
            this.bannerSettings.Size = new System.Drawing.Size(244, 110);
            this.bannerSettings.TabIndex = 0;
            this.bannerSettings.TabStop = false;
            this.bannerSettings.Text = "Banner Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "* 120 x 44";
            // 
            // windowsSettings
            // 
            this.windowsSettings.Controls.Add(this.crapCfg);
            this.windowsSettings.Controls.Add(this.crapGame);
            this.windowsSettings.Controls.Add(this.openAppdata);
            this.windowsSettings.Controls.Add(this.jpLabel);
            this.windowsSettings.Controls.Add(this.openvpatch);
            this.windowsSettings.Controls.Add(this.enApplocale);
            this.windowsSettings.Controls.Add(this.jpDir);
            this.windowsSettings.Controls.Add(this.crapApplocale);
            this.windowsSettings.Controls.Add(this.enDir);
            this.windowsSettings.Controls.Add(this.defaultApplocale);
            this.windowsSettings.Controls.Add(this.customDir);
            this.windowsSettings.Controls.Add(this.customApplocale);
            this.windowsSettings.Controls.Add(this.browseJP);
            this.windowsSettings.Controls.Add(this.launchcrap);
            this.windowsSettings.Controls.Add(this.enLabel);
            this.windowsSettings.Controls.Add(this.customLabel);
            this.windowsSettings.Controls.Add(this.crapLabel);
            this.windowsSettings.Controls.Add(this.openReplays);
            this.windowsSettings.Controls.Add(this.browseEN);
            this.windowsSettings.Controls.Add(this.launchJP);
            this.windowsSettings.Controls.Add(this.browseCustom);
            this.windowsSettings.Controls.Add(this.launchEN);
            this.windowsSettings.Controls.Add(this.jpApplocale);
            this.windowsSettings.Controls.Add(this.launchCustom);
            this.windowsSettings.Controls.Add(this.defaultExec);
            this.windowsSettings.Controls.Add(this.defaultLabel);
            this.windowsSettings.Location = new System.Drawing.Point(3, 3);
            this.windowsSettings.Name = "windowsSettings";
            this.windowsSettings.Size = new System.Drawing.Size(328, 255);
            this.windowsSettings.TabIndex = 36;
            this.windowsSettings.TabStop = false;
            this.windowsSettings.Text = "Windows Game Settings";
            // 
            // crapCfg
            // 
            this.crapCfg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.crapCfg.FormattingEnabled = true;
            this.crapCfg.Items.AddRange(new object[] {
            "None"});
            this.crapCfg.Location = new System.Drawing.Point(128, 171);
            this.crapCfg.Name = "crapCfg";
            this.crapCfg.Size = new System.Drawing.Size(130, 21);
            this.crapCfg.TabIndex = 30;
            this.crapCfg.SelectedIndexChanged += new System.EventHandler(this.crapCfg_SelectedIndexChanged);
            // 
            // crapGame
            // 
            this.crapGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.crapGame.FormattingEnabled = true;
            this.crapGame.Items.AddRange(new object[] {
            "None"});
            this.crapGame.Location = new System.Drawing.Point(6, 171);
            this.crapGame.Name = "crapGame";
            this.crapGame.Size = new System.Drawing.Size(116, 21);
            this.crapGame.TabIndex = 29;
            this.crapGame.SelectedIndexChanged += new System.EventHandler(this.crapCfg_SelectedIndexChanged);
            // 
            // openAppdata
            // 
            this.openAppdata.Location = new System.Drawing.Point(9, 227);
            this.openAppdata.Name = "openAppdata";
            this.openAppdata.Size = new System.Drawing.Size(91, 23);
            this.openAppdata.TabIndex = 28;
            this.openAppdata.Text = "Appdata Folder";
            this.openAppdata.UseVisualStyleBackColor = true;
            this.openAppdata.Click += new System.EventHandler(this.openAppdata_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 262);
            this.Controls.Add(this.windowsSettings);
            this.Controls.Add(this.bannerSettings);
            this.Controls.Add(this.openFolder);
            this.Controls.Add(this.pc98Settings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Configuration: ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_Closing);
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.pc98Settings.ResumeLayout(false);
            this.pc98Settings.PerformLayout();
            this.bannerSettings.ResumeLayout(false);
            this.bannerSettings.PerformLayout();
            this.windowsSettings.ResumeLayout(false);
            this.windowsSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openFolder;
        private System.Windows.Forms.Button openvpatch;
        private System.Windows.Forms.TextBox jpDir;
        private System.Windows.Forms.TextBox enDir;
        private System.Windows.Forms.TextBox customDir;
        private System.Windows.Forms.Button browseJP;
        private System.Windows.Forms.Label jpLabel;
        private System.Windows.Forms.Label enLabel;
        private System.Windows.Forms.Label customLabel;
        private System.Windows.Forms.Button openReplays;
        private System.Windows.Forms.Button browseEN;
        private System.Windows.Forms.Button browseCustom;
        private System.Windows.Forms.CheckBox jpApplocale;
        private System.Windows.Forms.ComboBox defaultExec;
        private System.Windows.Forms.Label defaultLabel;
        private System.Windows.Forms.Button launchCustom;
        private System.Windows.Forms.Button launchEN;
        private System.Windows.Forms.Button launchJP;
        private System.Windows.Forms.Button launchcrap;
        private System.Windows.Forms.Label crapLabel;
        private System.Windows.Forms.CheckBox customApplocale;
        private System.Windows.Forms.CheckBox defaultApplocale;
        private System.Windows.Forms.CheckBox crapApplocale;
        private System.Windows.Forms.CheckBox enApplocale;
        private System.Windows.Forms.CheckBox chkCustomBanner;
        private System.Windows.Forms.Label bannerOnLabel;
        private System.Windows.Forms.Button browseBannerOn;
        private System.Windows.Forms.TextBox bannerOnDir;
        private System.Windows.Forms.Label bannerOffLabel;
        private System.Windows.Forms.Button browseBannerOff;
        private System.Windows.Forms.TextBox bannerOffDir;
        private System.Windows.Forms.GroupBox pc98Settings;
        private System.Windows.Forms.Button launchHDI;
        private System.Windows.Forms.Label hdiLabel;
        private System.Windows.Forms.Button browseHDI;
        private System.Windows.Forms.TextBox hdiDir;
        private System.Windows.Forms.GroupBox bannerSettings;
        private System.Windows.Forms.GroupBox windowsSettings;
        private System.Windows.Forms.Button openAppdata;
        private System.Windows.Forms.Button openNP2Folder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox crapGame;
        private System.Windows.Forms.ComboBox crapCfg;
    }
}