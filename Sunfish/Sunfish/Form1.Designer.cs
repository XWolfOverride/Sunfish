﻿namespace DolphinWebXplorer2
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.cbActive = new System.Windows.Forms.CheckBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.il16 = new System.Windows.Forms.ImageList(this.components);
            this.btSub = new System.Windows.Forms.Button();
            this.lbPaths = new System.Windows.Forms.ListBox();
            this.cmsItem = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.añadirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btShowIp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.cmsItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(213, 514);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Ok";
            this.toolTip1.SetToolTip(this.button1, "Hide");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(268, 9);
            this.nudPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nudPort.Maximum = new decimal(new int[] {
            64000,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(82, 26);
            this.nudPort.TabIndex = 3;
            this.toolTip1.SetToolTip(this.nudPort, "Listen port");
            this.nudPort.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
            // 
            // cbActive
            // 
            this.cbActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbActive.AutoSize = true;
            this.cbActive.Location = new System.Drawing.Point(119, 12);
            this.cbActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbActive.Name = "cbActive";
            this.cbActive.Size = new System.Drawing.Size(132, 24);
            this.cbActive.TabIndex = 4;
            this.cbActive.Text = "Listen on port";
            this.toolTip1.SetToolTip(this.cbActive, "Enable whole server");
            this.cbActive.UseVisualStyleBackColor = true;
            this.cbActive.CheckedChanged += new System.EventHandler(this.cbActive_CheckedChanged);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btAdd.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAdd.ImageIndex = 0;
            this.btAdd.ImageList = this.il16;
            this.btAdd.Location = new System.Drawing.Point(9, 514);
            this.btAdd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(34, 35);
            this.btAdd.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btAdd, "Add new access");
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // il16
            // 
            this.il16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il16.ImageStream")));
            this.il16.TransparentColor = System.Drawing.Color.White;
            this.il16.Images.SetKeyName(0, "bt_plus.bmp");
            this.il16.Images.SetKeyName(1, "bt_minus.bmp");
            // 
            // btSub
            // 
            this.btSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSub.Enabled = false;
            this.btSub.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSub.ImageKey = "bt_minus.bmp";
            this.btSub.ImageList = this.il16;
            this.btSub.Location = new System.Drawing.Point(52, 514);
            this.btSub.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btSub.Name = "btSub";
            this.btSub.Size = new System.Drawing.Size(34, 35);
            this.btSub.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btSub, "Remove >selected access");
            this.btSub.UseVisualStyleBackColor = true;
            this.btSub.Click += new System.EventHandler(this.btSub_Click);
            // 
            // lbPaths
            // 
            this.lbPaths.AllowDrop = true;
            this.lbPaths.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPaths.ContextMenuStrip = this.cmsItem;
            this.lbPaths.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbPaths.FormattingEnabled = true;
            this.lbPaths.IntegralHeight = false;
            this.lbPaths.ItemHeight = 28;
            this.lbPaths.Location = new System.Drawing.Point(0, 49);
            this.lbPaths.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lbPaths.Name = "lbPaths";
            this.lbPaths.Size = new System.Drawing.Size(358, 453);
            this.lbPaths.TabIndex = 7;
            this.lbPaths.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbPaths_DrawItem);
            this.lbPaths.SelectedIndexChanged += new System.EventHandler(this.lbPaths_SelectedIndexChanged);
            this.lbPaths.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbPaths_DragDrop);
            this.lbPaths.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbPaths_DragEnter);
            this.lbPaths.DoubleClick += new System.EventHandler(this.clbPaths_DoubleClick);
            // 
            // cmsItem
            // 
            this.cmsItem.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.añadirToolStripMenuItem,
            this.editarToolStripMenuItem,
            this.borrarToolStripMenuItem,
            this.toolStripSeparator1});
            this.cmsItem.Name = "cmsItem";
            this.cmsItem.Size = new System.Drawing.Size(193, 106);
            this.cmsItem.Opening += new System.ComponentModel.CancelEventHandler(this.cmsItem_Opening);
            // 
            // añadirToolStripMenuItem
            // 
            this.añadirToolStripMenuItem.Name = "añadirToolStripMenuItem";
            this.añadirToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.añadirToolStripMenuItem.Size = new System.Drawing.Size(192, 32);
            this.añadirToolStripMenuItem.Text = "Añadir";
            this.añadirToolStripMenuItem.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // editarToolStripMenuItem
            // 
            this.editarToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.editarToolStripMenuItem.Name = "editarToolStripMenuItem";
            this.editarToolStripMenuItem.Size = new System.Drawing.Size(192, 32);
            this.editarToolStripMenuItem.Text = "Editar";
            // 
            // borrarToolStripMenuItem
            // 
            this.borrarToolStripMenuItem.Name = "borrarToolStripMenuItem";
            this.borrarToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.borrarToolStripMenuItem.Size = new System.Drawing.Size(192, 32);
            this.borrarToolStripMenuItem.Text = "Borrar";
            this.borrarToolStripMenuItem.Click += new System.EventHandler(this.btSub_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(189, 6);
            // 
            // btShowIp
            // 
            this.btShowIp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btShowIp.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btShowIp.Image = global::DolphinWebXplorer2.Properties.Resources.messagebox_info;
            this.btShowIp.Location = new System.Drawing.Point(170, 514);
            this.btShowIp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btShowIp.Name = "btShowIp";
            this.btShowIp.Size = new System.Drawing.Size(34, 35);
            this.btShowIp.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btShowIp, "Show network information");
            this.btShowIp.UseVisualStyleBackColor = true;
            this.btShowIp.Click += new System.EventHandler(this.btShowIp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 558);
            this.Controls.Add(this.btShowIp);
            this.Controls.Add(this.lbPaths);
            this.Controls.Add(this.btSub);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.cbActive);
            this.Controls.Add(this.nudPort);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sunfish";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.LocationChanged += new System.EventHandler(this.Form1_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.cmsItem.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.CheckBox cbActive;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btSub;
        private System.Windows.Forms.ListBox lbPaths;
        private System.Windows.Forms.Button btShowIp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ImageList il16;
        private System.Windows.Forms.ContextMenuStrip cmsItem;
        private System.Windows.Forms.ToolStripMenuItem editarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem añadirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

