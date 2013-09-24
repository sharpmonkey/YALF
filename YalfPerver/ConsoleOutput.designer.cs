namespace YalfPerver
{
    partial class ConsoleOutput
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleOutput));
            this.tbConsole = new System.Windows.Forms.RichTextBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnScrollToCaret = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.txtFindText = new System.Windows.Forms.ToolStripTextBox();
            this.btnFind = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbConsole
            // 
            this.tbConsole.BackColor = System.Drawing.Color.Black;
            this.tbConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbConsole.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.tbConsole.ForeColor = System.Drawing.Color.Chartreuse;
            this.tbConsole.Location = new System.Drawing.Point(0, 25);
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.Size = new System.Drawing.Size(467, 286);
            this.tbConsole.TabIndex = 0;
            this.tbConsole.Text = "";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnScrollToCaret,
            this.btnClear,
            this.toolStripSeparator1,
            this.txtFindText,
            this.btnFind});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(467, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnScrollToCaret
            // 
            this.btnScrollToCaret.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnScrollToCaret.Checked = true;
            this.btnScrollToCaret.CheckOnClick = true;
            this.btnScrollToCaret.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnScrollToCaret.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScrollToCaret.Image = ((System.Drawing.Image)(resources.GetObject("btnScrollToCaret.Image")));
            this.btnScrollToCaret.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollToCaret.Name = "btnScrollToCaret";
            this.btnScrollToCaret.Size = new System.Drawing.Size(83, 22);
            this.btnScrollToCaret.Text = "Scroll to caret";
            this.btnScrollToCaret.Click += new System.EventHandler(this.btnScrollToCaret_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(38, 22);
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtFindText
            // 
            this.txtFindText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFindText.Name = "txtFindText";
            this.txtFindText.Size = new System.Drawing.Size(250, 25);
            this.txtFindText.TextChanged += new System.EventHandler(this.txtFindText_TextChanged);
            // 
            // btnFind
            // 
            this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(34, 22);
            this.btnFind.Text = "Find";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ConsoleOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbConsole);
            this.Controls.Add(this.toolStrip);
            this.Name = "ConsoleOutput";
            this.Size = new System.Drawing.Size(467, 311);
            this.Load += new System.EventHandler(this.ConsoleOutput_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox tbConsole;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnScrollToCaret;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripTextBox txtFindText;
        private System.Windows.Forms.ToolStripButton btnFind;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
