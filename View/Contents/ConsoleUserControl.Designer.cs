namespace Server_HTML_Handler.View.Contents
{
    partial class ConsoleUserControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.rtbGuideText = new System.Windows.Forms.RichTextBox();
            this.pnlGuideHeader = new System.Windows.Forms.Panel();
            this.lblGuideTitle = new System.Windows.Forms.Label();
            this.rtbConsoleOutput = new System.Windows.Forms.RichTextBox();
            this.pnlInputBar = new System.Windows.Forms.Panel();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.txtConsoleInput = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnClearConsole = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.pnlGuideHeader.SuspendLayout();
            this.pnlInputBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.rtbGuideText);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlGuideHeader);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.rtbConsoleOutput);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlInputBar);
            this.splitContainerMain.Size = new System.Drawing.Size(820, 600);
            this.splitContainerMain.SplitterDistance = 280;
            this.splitContainerMain.TabIndex = 0;
            // 
            // rtbGuideText
            // 
            this.rtbGuideText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.rtbGuideText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbGuideText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbGuideText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbGuideText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rtbGuideText.Location = new System.Drawing.Point(0, 32);
            this.rtbGuideText.Name = "rtbGuideText";
            this.rtbGuideText.ReadOnly = true;
            this.rtbGuideText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbGuideText.Size = new System.Drawing.Size(820, 248);
            this.rtbGuideText.TabIndex = 1;
            this.rtbGuideText.Text = "";
            // 
            // pnlGuideHeader
            // 
            this.pnlGuideHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlGuideHeader.Controls.Add(this.lblGuideTitle);
            this.pnlGuideHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlGuideHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlGuideHeader.Name = "pnlGuideHeader";
            this.pnlGuideHeader.Size = new System.Drawing.Size(820, 32);
            this.pnlGuideHeader.TabIndex = 0;
            // 
            // lblGuideTitle
            // 
            this.lblGuideTitle.AutoSize = true;
            this.lblGuideTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuideTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblGuideTitle.Location = new System.Drawing.Point(10, 7);
            this.lblGuideTitle.Name = "lblGuideTitle";
            this.lblGuideTitle.Size = new System.Drawing.Size(323, 17);
            this.lblGuideTitle.TabIndex = 0;
            this.lblGuideTitle.Text = "Command Quick Reference & Syntax Documentation";
            // 
            // rtbConsoleOutput
            // 
            this.rtbConsoleOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.rtbConsoleOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbConsoleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsoleOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbConsoleOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.rtbConsoleOutput.Location = new System.Drawing.Point(0, 0);
            this.rtbConsoleOutput.Name = "rtbConsoleOutput";
            this.rtbConsoleOutput.ReadOnly = true;
            this.rtbConsoleOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbConsoleOutput.Size = new System.Drawing.Size(820, 276);
            this.rtbConsoleOutput.TabIndex = 0;
            this.rtbConsoleOutput.Text = "";
            // 
            // pnlInputBar
            // 
            this.pnlInputBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlInputBar.Controls.Add(this.lblPrompt);
            this.pnlInputBar.Controls.Add(this.txtConsoleInput);
            this.pnlInputBar.Controls.Add(this.btnExecute);
            this.pnlInputBar.Controls.Add(this.btnClearConsole);
            this.pnlInputBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInputBar.Location = new System.Drawing.Point(0, 276);
            this.pnlInputBar.Name = "pnlInputBar";
            this.pnlInputBar.Size = new System.Drawing.Size(820, 40);
            this.pnlInputBar.TabIndex = 1;
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrompt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblPrompt.Location = new System.Drawing.Point(8, 10);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(16, 18);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = ">";
            // 
            // txtConsoleInput
            // 
            this.txtConsoleInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtConsoleInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConsoleInput.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsoleInput.ForeColor = System.Drawing.Color.White;
            this.txtConsoleInput.Location = new System.Drawing.Point(35, 8);
            this.txtConsoleInput.Name = "txtConsoleInput";
            this.txtConsoleInput.Size = new System.Drawing.Size(610, 23);
            this.txtConsoleInput.TabIndex = 1;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnExecute.FlatAppearance.BorderSize = 0;
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.ForeColor = System.Drawing.Color.White;
            this.btnExecute.Location = new System.Drawing.Point(652, 7);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 25);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Run";
            this.btnExecute.UseVisualStyleBackColor = false;
            // 
            // btnClearConsole
            // 
            this.btnClearConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnClearConsole.FlatAppearance.BorderSize = 0;
            this.btnClearConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearConsole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearConsole.ForeColor = System.Drawing.Color.White;
            this.btnClearConsole.Location = new System.Drawing.Point(733, 7);
            this.btnClearConsole.Name = "btnClearConsole";
            this.btnClearConsole.Size = new System.Drawing.Size(75, 25);
            this.btnClearConsole.TabIndex = 3;
            this.btnClearConsole.Text = "Clear";
            this.btnClearConsole.UseVisualStyleBackColor = false;
            // 
            // ConsoleUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMain);
            this.Name = "ConsoleUserControl";
            this.Size = new System.Drawing.Size(820, 600);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.pnlGuideHeader.ResumeLayout(false);
            this.pnlGuideHeader.PerformLayout();
            this.pnlInputBar.ResumeLayout(false);
            this.pnlInputBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel pnlGuideHeader;
        private System.Windows.Forms.Label lblGuideTitle;
        private System.Windows.Forms.RichTextBox rtbGuideText;
        private System.Windows.Forms.RichTextBox rtbConsoleOutput;
        private System.Windows.Forms.Panel pnlInputBar;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.TextBox txtConsoleInput;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnClearConsole;
    }
}