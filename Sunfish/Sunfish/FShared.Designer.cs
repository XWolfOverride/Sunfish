namespace DolphinWebXplorer2
{
    partial class FShared
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbId = new System.Windows.Forms.TextBox();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.cbSub = new System.Windows.Forms.CheckBox();
            this.cbUpl = new System.Windows.Forms.CheckBox();
            this.btOk = new System.Windows.Forms.Button();
            this.cbDele = new System.Windows.Forms.CheckBox();
            this.cbRena = new System.Windows.Forms.CheckBox();
            this.cbExec = new System.Windows.Forms.CheckBox();
            this.btOpen = new System.Windows.Forms.Button();
            this.cbMkDir = new System.Windows.Forms.CheckBox();
            this.cbThumbnails = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Identifier:";
            // 
            // tbId
            // 
            this.tbId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbId.Location = new System.Drawing.Point(68, 12);
            this.tbId.Name = "tbId";
            this.tbId.Size = new System.Drawing.Size(323, 20);
            this.tbId.TabIndex = 1;
            this.tbId.TextChanged += new System.EventHandler(this.tbId_TextChanged);
            // 
            // tbPath
            // 
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.Location = new System.Drawing.Point(68, 39);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(323, 20);
            this.tbPath.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path:";
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(15, 68);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(65, 17);
            this.cbEnabled.TabIndex = 4;
            this.cbEnabled.Text = "Enabled";
            this.cbEnabled.UseVisualStyleBackColor = true;
            // 
            // cbSub
            // 
            this.cbSub.AutoSize = true;
            this.cbSub.Location = new System.Drawing.Point(15, 91);
            this.cbSub.Name = "cbSub";
            this.cbSub.Size = new System.Drawing.Size(149, 17);
            this.cbSub.TabIndex = 5;
            this.cbSub.Text = "Allow subfolder navigation";
            this.cbSub.UseVisualStyleBackColor = true;
            // 
            // cbUpl
            // 
            this.cbUpl.AutoSize = true;
            this.cbUpl.Location = new System.Drawing.Point(15, 114);
            this.cbUpl.Name = "cbUpl";
            this.cbUpl.Size = new System.Drawing.Size(91, 17);
            this.cbUpl.TabIndex = 6;
            this.cbUpl.Text = "Allow uploads";
            this.cbUpl.UseVisualStyleBackColor = true;
            // 
            // btOk
            // 
            this.btOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOk.Location = new System.Drawing.Point(316, 162);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 7;
            this.btOk.Text = "Ok";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // cbDele
            // 
            this.cbDele.AutoSize = true;
            this.cbDele.Location = new System.Drawing.Point(170, 68);
            this.cbDele.Name = "cbDele";
            this.cbDele.Size = new System.Drawing.Size(83, 17);
            this.cbDele.TabIndex = 8;
            this.cbDele.Text = "Allow delete";
            this.cbDele.UseVisualStyleBackColor = true;
            // 
            // cbRena
            // 
            this.cbRena.AutoSize = true;
            this.cbRena.Location = new System.Drawing.Point(170, 91);
            this.cbRena.Name = "cbRena";
            this.cbRena.Size = new System.Drawing.Size(89, 17);
            this.cbRena.TabIndex = 9;
            this.cbRena.Text = "Allow rename";
            this.cbRena.UseVisualStyleBackColor = true;
            // 
            // cbExec
            // 
            this.cbExec.AutoSize = true;
            this.cbExec.Location = new System.Drawing.Point(170, 114);
            this.cbExec.Name = "cbExec";
            this.cbExec.Size = new System.Drawing.Size(229, 17);
            this.cbExec.TabIndex = 10;
            this.cbExec.Text = "Allow remote execution (execute on server)";
            this.cbExec.UseVisualStyleBackColor = true;
            // 
            // btOpen
            // 
            this.btOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOpen.Location = new System.Drawing.Point(207, 162);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(103, 23);
            this.btOpen.TabIndex = 11;
            this.btOpen.Text = "Open in browser";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // cbMkDir
            // 
            this.cbMkDir.AutoSize = true;
            this.cbMkDir.Location = new System.Drawing.Point(15, 137);
            this.cbMkDir.Name = "cbMkDir";
            this.cbMkDir.Size = new System.Drawing.Size(118, 17);
            this.cbMkDir.TabIndex = 12;
            this.cbMkDir.Text = "Allow create folders";
            this.cbMkDir.UseVisualStyleBackColor = true;
            // 
            // cbThumbnails
            // 
            this.cbThumbnails.AutoSize = true;
            this.cbThumbnails.Location = new System.Drawing.Point(170, 137);
            this.cbThumbnails.Name = "cbThumbnails";
            this.cbThumbnails.Size = new System.Drawing.Size(200, 17);
            this.cbThumbnails.TabIndex = 13;
            this.cbThumbnails.Text = "Send image previews on icons (slow)";
            this.cbThumbnails.UseVisualStyleBackColor = true;
            // 
            // FShared
            // 
            this.AcceptButton = this.btOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 197);
            this.Controls.Add(this.cbThumbnails);
            this.Controls.Add(this.cbMkDir);
            this.Controls.Add(this.btOpen);
            this.Controls.Add(this.cbExec);
            this.Controls.Add(this.cbRena);
            this.Controls.Add(this.cbDele);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.cbUpl);
            this.Controls.Add(this.cbSub);
            this.Controls.Add(this.cbEnabled);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbId);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FShared";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shared";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbId;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbEnabled;
        private System.Windows.Forms.CheckBox cbSub;
        private System.Windows.Forms.CheckBox cbUpl;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.CheckBox cbDele;
        private System.Windows.Forms.CheckBox cbRena;
        private System.Windows.Forms.CheckBox cbExec;
        private System.Windows.Forms.Button btOpen;
        private System.Windows.Forms.CheckBox cbMkDir;
        private System.Windows.Forms.CheckBox cbThumbnails;
    }
}