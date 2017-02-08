namespace FractalServer
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.factalPictureBox = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ClientDatagridview = new System.Windows.Forms.DataGridView();
            this.IPTxtbox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ThreadsTxtbox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisconnectBtn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.IPConnectTxtbox = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.RenderBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.factalPictureBox)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClientDatagridview)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(508, 526);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.factalPictureBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(500, 500);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // factalPictureBox
            // 
            this.factalPictureBox.Location = new System.Drawing.Point(0, 0);
            this.factalPictureBox.Name = "factalPictureBox";
            this.factalPictureBox.Size = new System.Drawing.Size(500, 500);
            this.factalPictureBox.TabIndex = 0;
            this.factalPictureBox.TabStop = false;
            this.factalPictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.factalPictureBox_MouseDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ClientDatagridview);
            this.tabPage2.Controls.Add(this.ConnectBtn);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.IPConnectTxtbox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(500, 500);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ClientDatagridview
            // 
            this.ClientDatagridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ClientDatagridview.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IPTxtbox,
            this.ThreadsTxtbox,
            this.DisconnectBtn});
            this.ClientDatagridview.Location = new System.Drawing.Point(42, 161);
            this.ClientDatagridview.Name = "ClientDatagridview";
            this.ClientDatagridview.Size = new System.Drawing.Size(343, 150);
            this.ClientDatagridview.TabIndex = 4;
            // 
            // IPTxtbox
            // 
            this.IPTxtbox.HeaderText = "IP";
            this.IPTxtbox.Name = "IPTxtbox";
            this.IPTxtbox.ReadOnly = true;
            // 
            // ThreadsTxtbox
            // 
            this.ThreadsTxtbox.HeaderText = "Threads";
            this.ThreadsTxtbox.Name = "ThreadsTxtbox";
            this.ThreadsTxtbox.ReadOnly = true;
            // 
            // DisconnectBtn
            // 
            this.DisconnectBtn.HeaderText = "Disconnect";
            this.DisconnectBtn.Name = "DisconnectBtn";
            this.DisconnectBtn.ReadOnly = true;
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(87, 45);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(75, 23);
            this.ConnectBtn.TabIndex = 3;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP of render pc:";
            // 
            // IPConnectTxtbox
            // 
            this.IPConnectTxtbox.Location = new System.Drawing.Point(87, 19);
            this.IPConnectTxtbox.Name = "IPConnectTxtbox";
            this.IPConnectTxtbox.Size = new System.Drawing.Size(100, 20);
            this.IPConnectTxtbox.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(527, 146);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(96, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // RenderBtn
            // 
            this.RenderBtn.Location = new System.Drawing.Point(527, 117);
            this.RenderBtn.Name = "RenderBtn";
            this.RenderBtn.Size = new System.Drawing.Size(96, 23);
            this.RenderBtn.TabIndex = 2;
            this.RenderBtn.Text = "Render";
            this.RenderBtn.UseVisualStyleBackColor = true;
            this.RenderBtn.Click += new System.EventHandler(this.RenderBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(527, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(527, 88);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(96, 23);
            this.SaveBtn.TabIndex = 4;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 551);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RenderBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "f";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.factalPictureBox)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClientDatagridview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox factalPictureBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IPConnectTxtbox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button RenderBtn;
        private System.Windows.Forms.DataGridView ClientDatagridview;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPTxtbox;
        private System.Windows.Forms.DataGridViewTextBoxColumn ThreadsTxtbox;
        private System.Windows.Forms.DataGridViewButtonColumn DisconnectBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button SaveBtn;
    }
}

