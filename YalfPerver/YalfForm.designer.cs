namespace YalfPerver
{
    partial class YalfForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YalfForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node0");
            this.btnDump = new System.Windows.Forms.Button();
            this.btnClean = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtLogContext = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCsvDump = new System.Windows.Forms.Button();
            this.txtCsvFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRegexHelp = new System.Windows.Forms.TextBox();
            this.pnlMainOptions = new System.Windows.Forms.Panel();
            this.lblLastLogEntry = new System.Windows.Forms.Label();
            this.lblFirstLogEntry = new System.Windows.Forms.Label();
            this.txtTimeStampTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTimeStampFrom = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSaveFilters = new System.Windows.Forms.Button();
            this.btnLoadFilters = new System.Windows.Forms.Button();
            this.chkSingleLineFormat = new System.Windows.Forms.CheckBox();
            this.chkHideMethodReturnValue = new System.Windows.Forms.CheckBox();
            this.chkHodeMethodParameters = new System.Windows.Forms.CheckBox();
            this.chkHideDuration = new System.Windows.Forms.CheckBox();
            this.chkHideTimestamp = new System.Windows.Forms.CheckBox();
            this.chkHideExitMethod = new System.Windows.Forms.CheckBox();
            this.chkHideEnterMethod = new System.Windows.Forms.CheckBox();
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.ExcludedKeyList = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IncludedKeyList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveSettingsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openSettingsFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tvFilter = new YalfPerver.MyTreeView();
            this.consoleOutput = new YalfPerver.ConsoleOutput();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlMainOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(12, 8);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(41, 23);
            this.btnDump.TabIndex = 0;
            this.btnDump.Text = "dump";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(55, 8);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(41, 23);
            this.btnClean.TabIndex = 1;
            this.btnClean.Text = "clean";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 12);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(600, 400);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtStatus);
            this.splitContainer1.Panel1.Controls.Add(this.txtLogContext);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.btnCsvDump);
            this.splitContainer1.Panel1.Controls.Add(this.txtCsvFolder);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.tbRegexHelp);
            this.splitContainer1.Panel1.Controls.Add(this.pnlMainOptions);
            this.splitContainer1.Panel1.Controls.Add(this.tvFilter);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.consoleOutput);
            this.splitContainer1.Size = new System.Drawing.Size(1370, 796);
            this.splitContainer1.SplitterDistance = 686;
            this.splitContainer1.TabIndex = 3;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtStatus.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtStatus.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.txtStatus.Location = new System.Drawing.Point(3, 517);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(356, 20);
            this.txtStatus.TabIndex = 20;
            // 
            // txtLogContext
            // 
            this.txtLogContext.Location = new System.Drawing.Point(69, 465);
            this.txtLogContext.Name = "txtLogContext";
            this.txtLogContext.Size = new System.Drawing.Size(210, 20);
            this.txtLogContext.TabIndex = 19;
            this.txtLogContext.Text = "YalfDump";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 468);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Log Context";
            // 
            // btnCsvDump
            // 
            this.btnCsvDump.Location = new System.Drawing.Point(285, 488);
            this.btnCsvDump.Name = "btnCsvDump";
            this.btnCsvDump.Size = new System.Drawing.Size(75, 23);
            this.btnCsvDump.TabIndex = 17;
            this.btnCsvDump.Text = "Create [F12]";
            this.btnCsvDump.UseVisualStyleBackColor = true;
            this.btnCsvDump.Click += new System.EventHandler(this.btnCsvDump_Click);
            // 
            // txtCsvFolder
            // 
            this.txtCsvFolder.Location = new System.Drawing.Point(69, 491);
            this.txtCsvFolder.Name = "txtCsvFolder";
            this.txtCsvFolder.Size = new System.Drawing.Size(210, 20);
            this.txtCsvFolder.TabIndex = 14;
            this.txtCsvFolder.Text = "C:\\Temp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 494);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Save folder";
            // 
            // tbRegexHelp
            // 
            this.tbRegexHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRegexHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbRegexHelp.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRegexHelp.Location = new System.Drawing.Point(3, 534);
            this.tbRegexHelp.Multiline = true;
            this.tbRegexHelp.Name = "tbRegexHelp";
            this.tbRegexHelp.ReadOnly = true;
            this.tbRegexHelp.Size = new System.Drawing.Size(676, 255);
            this.tbRegexHelp.TabIndex = 12;
            this.tbRegexHelp.Text = resources.GetString("tbRegexHelp.Text");
            this.tbRegexHelp.WordWrap = false;
            // 
            // pnlMainOptions
            // 
            this.pnlMainOptions.Controls.Add(this.lblLastLogEntry);
            this.pnlMainOptions.Controls.Add(this.lblFirstLogEntry);
            this.pnlMainOptions.Controls.Add(this.txtTimeStampTo);
            this.pnlMainOptions.Controls.Add(this.label6);
            this.pnlMainOptions.Controls.Add(this.txtTimeStampFrom);
            this.pnlMainOptions.Controls.Add(this.label5);
            this.pnlMainOptions.Controls.Add(this.btnSaveFilters);
            this.pnlMainOptions.Controls.Add(this.btnLoadFilters);
            this.pnlMainOptions.Controls.Add(this.chkSingleLineFormat);
            this.pnlMainOptions.Controls.Add(this.chkHideMethodReturnValue);
            this.pnlMainOptions.Controls.Add(this.chkHodeMethodParameters);
            this.pnlMainOptions.Controls.Add(this.chkHideDuration);
            this.pnlMainOptions.Controls.Add(this.chkHideTimestamp);
            this.pnlMainOptions.Controls.Add(this.chkHideExitMethod);
            this.pnlMainOptions.Controls.Add(this.chkHideEnterMethod);
            this.pnlMainOptions.Controls.Add(this.btnApplyFilters);
            this.pnlMainOptions.Controls.Add(this.ExcludedKeyList);
            this.pnlMainOptions.Controls.Add(this.label2);
            this.pnlMainOptions.Controls.Add(this.IncludedKeyList);
            this.pnlMainOptions.Controls.Add(this.label1);
            this.pnlMainOptions.Controls.Add(this.btnLoad);
            this.pnlMainOptions.Controls.Add(this.btnSave);
            this.pnlMainOptions.Controls.Add(this.btnDump);
            this.pnlMainOptions.Controls.Add(this.btnClean);
            this.pnlMainOptions.Controls.Add(this.chkIgnoreCase);
            this.pnlMainOptions.Location = new System.Drawing.Point(3, 3);
            this.pnlMainOptions.MinimumSize = new System.Drawing.Size(212, 47);
            this.pnlMainOptions.Name = "pnlMainOptions";
            this.pnlMainOptions.Size = new System.Drawing.Size(362, 457);
            this.pnlMainOptions.TabIndex = 0;
            // 
            // lblLastLogEntry
            // 
            this.lblLastLogEntry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLastLogEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastLogEntry.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblLastLogEntry.Location = new System.Drawing.Point(235, 151);
            this.lblLastLogEntry.Name = "lblLastLogEntry";
            this.lblLastLogEntry.Size = new System.Drawing.Size(121, 13);
            this.lblLastLogEntry.TabIndex = 25;
            this.lblLastLogEntry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFirstLogEntry
            // 
            this.lblFirstLogEntry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFirstLogEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstLogEntry.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblFirstLogEntry.Location = new System.Drawing.Point(83, 151);
            this.lblFirstLogEntry.Name = "lblFirstLogEntry";
            this.lblFirstLogEntry.Size = new System.Drawing.Size(121, 13);
            this.lblFirstLogEntry.TabIndex = 24;
            this.lblFirstLogEntry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTimeStampTo
            // 
            this.txtTimeStampTo.Location = new System.Drawing.Point(235, 130);
            this.txtTimeStampTo.Name = "txtTimeStampTo";
            this.txtTimeStampTo.Size = new System.Drawing.Size(121, 20);
            this.txtTimeStampTo.TabIndex = 23;
            this.toolTip1.SetToolTip(this.txtTimeStampTo, "Enter time in hh:mm:ss.fff format or leave blank for all");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(207, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "and";
            // 
            // txtTimeStampFrom
            // 
            this.txtTimeStampFrom.Location = new System.Drawing.Point(83, 130);
            this.txtTimeStampFrom.Name = "txtTimeStampFrom";
            this.txtTimeStampFrom.Size = new System.Drawing.Size(121, 20);
            this.txtTimeStampFrom.TabIndex = 21;
            this.toolTip1.SetToolTip(this.txtTimeStampFrom, "Enter time in hh:mm:ss.fff format or leave blank for all");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Times between";
            // 
            // btnSaveFilters
            // 
            this.btnSaveFilters.Location = new System.Drawing.Point(86, 426);
            this.btnSaveFilters.Name = "btnSaveFilters";
            this.btnSaveFilters.Size = new System.Drawing.Size(75, 23);
            this.btnSaveFilters.TabIndex = 18;
            this.btnSaveFilters.Text = "Save Filters";
            this.btnSaveFilters.UseVisualStyleBackColor = true;
            this.btnSaveFilters.Click += new System.EventHandler(this.btnSaveFilters_Click);
            // 
            // btnLoadFilters
            // 
            this.btnLoadFilters.Location = new System.Drawing.Point(5, 426);
            this.btnLoadFilters.Name = "btnLoadFilters";
            this.btnLoadFilters.Size = new System.Drawing.Size(75, 23);
            this.btnLoadFilters.TabIndex = 17;
            this.btnLoadFilters.Text = "Load Filters";
            this.btnLoadFilters.UseVisualStyleBackColor = true;
            this.btnLoadFilters.Click += new System.EventHandler(this.btnLoadFilters_Click);
            // 
            // chkSingleLineFormat
            // 
            this.chkSingleLineFormat.AutoSize = true;
            this.chkSingleLineFormat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSingleLineFormat.Location = new System.Drawing.Point(37, 62);
            this.chkSingleLineFormat.Name = "chkSingleLineFormat";
            this.chkSingleLineFormat.Size = new System.Drawing.Size(113, 17);
            this.chkSingleLineFormat.TabIndex = 16;
            this.chkSingleLineFormat.Text = "&Single Line Format";
            this.chkSingleLineFormat.UseVisualStyleBackColor = true;
            this.chkSingleLineFormat.CheckedChanged += new System.EventHandler(this.chkSingleLineFormat_CheckedChanged);
            // 
            // chkHideMethodReturnValue
            // 
            this.chkHideMethodReturnValue.AutoSize = true;
            this.chkHideMethodReturnValue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHideMethodReturnValue.Checked = true;
            this.chkHideMethodReturnValue.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideMethodReturnValue.Location = new System.Drawing.Point(173, 84);
            this.chkHideMethodReturnValue.Name = "chkHideMethodReturnValue";
            this.chkHideMethodReturnValue.Size = new System.Drawing.Size(145, 17);
            this.chkHideMethodReturnValue.TabIndex = 7;
            this.chkHideMethodReturnValue.Text = "Hide method return &value";
            this.chkHideMethodReturnValue.UseVisualStyleBackColor = true;
            // 
            // chkHodeMethodParameters
            // 
            this.chkHodeMethodParameters.AutoSize = true;
            this.chkHodeMethodParameters.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHodeMethodParameters.Checked = true;
            this.chkHodeMethodParameters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHodeMethodParameters.Location = new System.Drawing.Point(9, 84);
            this.chkHodeMethodParameters.Name = "chkHodeMethodParameters";
            this.chkHodeMethodParameters.Size = new System.Drawing.Size(141, 17);
            this.chkHodeMethodParameters.TabIndex = 6;
            this.chkHodeMethodParameters.Text = "Hide method &parameters";
            this.chkHodeMethodParameters.UseVisualStyleBackColor = true;
            // 
            // chkHideDuration
            // 
            this.chkHideDuration.AutoSize = true;
            this.chkHideDuration.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHideDuration.Checked = true;
            this.chkHideDuration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideDuration.Location = new System.Drawing.Point(59, 106);
            this.chkHideDuration.Name = "chkHideDuration";
            this.chkHideDuration.Size = new System.Drawing.Size(91, 17);
            this.chkHideDuration.TabIndex = 8;
            this.chkHideDuration.Text = "Hide &Duration";
            this.chkHideDuration.UseVisualStyleBackColor = true;
            // 
            // chkHideTimestamp
            // 
            this.chkHideTimestamp.AutoSize = true;
            this.chkHideTimestamp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHideTimestamp.Checked = true;
            this.chkHideTimestamp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideTimestamp.Location = new System.Drawing.Point(216, 106);
            this.chkHideTimestamp.Name = "chkHideTimestamp";
            this.chkHideTimestamp.Size = new System.Drawing.Size(102, 17);
            this.chkHideTimestamp.TabIndex = 9;
            this.chkHideTimestamp.Text = "Hide &Timestamp";
            this.chkHideTimestamp.UseVisualStyleBackColor = true;
            // 
            // chkHideExitMethod
            // 
            this.chkHideExitMethod.AutoSize = true;
            this.chkHideExitMethod.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHideExitMethod.Checked = true;
            this.chkHideExitMethod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideExitMethod.Location = new System.Drawing.Point(212, 40);
            this.chkHideExitMethod.Name = "chkHideExitMethod";
            this.chkHideExitMethod.Size = new System.Drawing.Size(106, 17);
            this.chkHideExitMethod.TabIndex = 5;
            this.chkHideExitMethod.Text = "Hide E&xit method";
            this.chkHideExitMethod.UseVisualStyleBackColor = true;
            // 
            // chkHideEnterMethod
            // 
            this.chkHideEnterMethod.AutoSize = true;
            this.chkHideEnterMethod.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHideEnterMethod.Location = new System.Drawing.Point(36, 40);
            this.chkHideEnterMethod.Name = "chkHideEnterMethod";
            this.chkHideEnterMethod.Size = new System.Drawing.Size(114, 17);
            this.chkHideEnterMethod.TabIndex = 4;
            this.chkHideEnterMethod.Text = "Hide &Enter method";
            this.chkHideEnterMethod.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilters
            // 
            this.btnApplyFilters.Location = new System.Drawing.Point(282, 426);
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.Size = new System.Drawing.Size(75, 23);
            this.btnApplyFilters.TabIndex = 15;
            this.btnApplyFilters.Text = "&Apply [F5]";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // ExcludedKeyList
            // 
            this.ExcludedKeyList.Location = new System.Drawing.Point(5, 328);
            this.ExcludedKeyList.Multiline = true;
            this.ExcludedKeyList.Name = "ExcludedKeyList";
            this.ExcludedKeyList.Size = new System.Drawing.Size(352, 92);
            this.ExcludedKeyList.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 312);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Excluded &Keys";
            // 
            // IncludedKeyList
            // 
            this.IncludedKeyList.Location = new System.Drawing.Point(5, 190);
            this.IncludedKeyList.Multiline = true;
            this.IncludedKeyList.Name = "IncludedKeyList";
            this.IncludedKeyList.Size = new System.Drawing.Size(352, 92);
            this.IncludedKeyList.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "&Include Keys (executed first)";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(142, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(42, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(98, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(42, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkIgnoreCase
            // 
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIgnoreCase.Location = new System.Drawing.Point(274, 171);
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(83, 17);
            this.chkIgnoreCase.TabIndex = 14;
            this.chkIgnoreCase.Text = "Ignore &Case";
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "ylf";
            this.saveFileDialog.FileName = "dump";
            this.saveFileDialog.Filter = "Yalf dump files (*.ylf)|*.ylf";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "ylf";
            this.openFileDialog.Filter = "Yalf dump files (*.yalf, *.ylf)|*.yalf;*.ylf|(All Files *.*)|*.*";
            // 
            // saveSettingsFileDialog
            // 
            this.saveSettingsFileDialog.DefaultExt = "yfc";
            this.saveSettingsFileDialog.FileName = "yalfFilters.yfc";
            this.saveSettingsFileDialog.Filter = "Yalf filter config files (*.yfc)|*.yfc|(All Files *.*)|*.*";
            this.saveSettingsFileDialog.Title = "Save Yalf Config";
            // 
            // openSettingsFileDialog
            // 
            this.openSettingsFileDialog.DefaultExt = "yfc";
            this.openSettingsFileDialog.FileName = "yalfFilters.yfc";
            this.openSettingsFileDialog.Filter = "Yalf filter config files (*.yfc)|*.yfc|(All Files *.*)|*.*";
            this.openSettingsFileDialog.Title = "Load Yalf Config";
            // 
            // tvFilter
            // 
            this.tvFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFilter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvFilter.CheckBoxes = true;
            this.tvFilter.Location = new System.Drawing.Point(366, -2);
            this.tvFilter.Name = "tvFilter";
            treeNode1.Checked = true;
            treeNode1.Name = "Node0";
            treeNode1.Text = "Node0";
            this.tvFilter.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvFilter.Size = new System.Drawing.Size(313, 530);
            this.tvFilter.TabIndex = 4;
            this.tvFilter.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvFilter_AfterCheck);
            this.tvFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvFilter_KeyDown);
            // 
            // consoleOutput
            // 
            this.consoleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleOutput.Location = new System.Drawing.Point(0, 0);
            this.consoleOutput.Name = "consoleOutput";
            this.consoleOutput.Size = new System.Drawing.Size(676, 792);
            this.consoleOutput.TabIndex = 0;
            // 
            // YalfForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 808);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.Name = "YalfForm";
            this.Text = "Yalf dump";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.YalfForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.YalfForm_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlMainOptions.ResumeLayout(false);
            this.pnlMainOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ConsoleOutput consoleOutput;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlMainOptions;
        private MyTreeView tvFilter;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox ExcludedKeyList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IncludedKeyList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApplyFilters;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.CheckBox chkHideExitMethod;
        private System.Windows.Forms.CheckBox chkHideEnterMethod;
        private System.Windows.Forms.CheckBox chkHideTimestamp;
        private System.Windows.Forms.CheckBox chkHideDuration;
        private System.Windows.Forms.CheckBox chkHodeMethodParameters;
        private System.Windows.Forms.CheckBox chkHideMethodReturnValue;
        private System.Windows.Forms.TextBox tbRegexHelp;
        private System.Windows.Forms.CheckBox chkSingleLineFormat;
        private System.Windows.Forms.Button btnCsvDump;
        private System.Windows.Forms.TextBox txtCsvFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLogContext;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveFilters;
        private System.Windows.Forms.Button btnLoadFilters;
        private System.Windows.Forms.SaveFileDialog saveSettingsFileDialog;
        private System.Windows.Forms.OpenFileDialog openSettingsFileDialog;
        private System.Windows.Forms.TextBox txtTimeStampFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTimeStampTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblLastLogEntry;
        private System.Windows.Forms.Label lblFirstLogEntry;
        private System.Windows.Forms.TextBox txtStatus;
    }
}