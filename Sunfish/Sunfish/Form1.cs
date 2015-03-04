﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DolphinWebXplorer2.wx;
using DolphinWebXplorer2.Properties;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DolphinWebXplorer2
{
    public partial class Form1 : Form
    {
        public const string conffile = "sunfish";
        public Font smallfont;
        public Brush itembrushgray;
        private Screen myscreen;
        public Form1()
        {
            InitializeComponent();
            itembrushgray = new SolidBrush(Color.Gray);
            smallfont = new Font(Font.FontFamily,7, FontStyle.Regular);
            Icon = Resources.sunfishWebServer;
            if (File.Exists(conffile))
            {
                WebXplorer.Load(conffile);
            }
            PopulateData();
            Text += " "+Program.VERSION;
            myscreen = Screen.FromControl(this);
        }

        private void PopulateData()
        {
            Enabled = false;
            nudPort.Value = WebXplorer.Port;
            cbActive.Checked=WebXplorer.Active;
            shareScreenToolStripMenuItem.Checked = WebXplorer.SharedScreen;
            tstbPassword.Text = WebXplorer.SharedScreenPassword;
            lbPaths.Items.Clear();
            foreach (WShared sh in WebXplorer.Shares)
               lbPaths.Items.Add(sh);
            Enabled = true;
        }

        private List<IpInfo> ListInterfacesIPs()
        {
            List<IpInfo> result = new List<IpInfo>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                if (item.NetworkInterfaceType != NetworkInterfaceType.Loopback && item.OperationalStatus == OperationalStatus.Up)
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            result.Add(new IpInfo(item, ip.Address.ToString()));
            return result;
        }

        public Screen MyScreen { get { lock (this) return myscreen; } }

        private void button1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            WebXplorer.Save(conffile);
            WebXplorer.Stop();
        }

        private void cbActive_CheckedChanged(object sender, EventArgs e)
        {
            nudPort.Enabled = !cbActive.Checked;
            if (!Enabled)
                return;
            if (cbActive.Checked)
                WebXplorer.Start();
            else
                WebXplorer.Stop();
            cbActive.Checked = WebXplorer.Active;
        }

        private void nudPort_ValueChanged(object sender, EventArgs e)
        {
            if (!Enabled)
                return;
            WebXplorer.Port = (int) nudPort.Value;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            WShared sh = new WShared("NewShared",@"C:\");
            sh.Enabled = true;
            if (FShared.Execute(sh))
            {
                WebXplorer.Add(sh);
                lbPaths.Items.Add(sh);
            }
        }

        private void clbPaths_DoubleClick(object sender, EventArgs e)
        {
            WShared sh = (WShared)lbPaths.SelectedItem;
            if (sh==null)
                return;
            if (FShared.Execute(sh))
                lbPaths.Update();
        }

        private void lbPaths_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (e.Index < 0)
                return;
            WShared sh = (WShared)lbPaths.Items[e.Index];
            Graphics g=e.Graphics;
            int y=e.Bounds.Top+2;
            using (Brush itembrush = new SolidBrush(e.ForeColor))
            {
                g.DrawImage(sh.Enabled ? Resources.foldericon : Resources.foldericond, 1, y, 24, 24);
                g.DrawString(sh.Name, lbPaths.Font, itembrush, 28, y);
                g.DrawString(sh.Path, smallfont, itembrushgray, 33, y + 12);
                if (sh.AllowSubfolders)
                    g.DrawImage(Resources.osubf, 16, y + 13);
                if (sh.AllowUpload)
                    g.DrawImage(Resources.ouplo, 3, y + 13);
            }
        }

        private void lbPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            btSub.Enabled = lbPaths.SelectedItem != null;
        }

        private void btSub_Click(object sender, EventArgs e)
        {
            WShared sh = (WShared)lbPaths.SelectedItem;
            if (sh==null)
                return;
            if (MessageBox.Show("Delete access " + sh.Name + "? Can not be undone!", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                WebXplorer.Delete(sh);
                lbPaths.Items.Remove(sh);
            }
        }

        private void btShowIp_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder(WebXplorer.Active?"A":"ina");
            sb.Append("ctive.\r\n");
            foreach (IpInfo ip in ListInterfacesIPs())
            {
                sb.Append(ip.InterfaceName);
                sb.Append(" (");
                sb.Append(ip.InterfaceType);
                sb.Append("): ");
                sb.Append(ip.Address);
                sb.Append(":");
                sb.Append(WebXplorer.Port);
                sb.Append("\r\n");
            }
            MessageBox.Show(sb.ToString(),"Network information",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void cmsItem_Opening(object sender, CancelEventArgs e)
        {
            WShared sh = lbPaths.SelectedItem as WShared;            
            editarToolStripMenuItem.Enabled = sh != null;
            while (cmsItem.Items.Count > 1)
                cmsItem.Items.RemoveAt(cmsItem.Items.Count - 1);
            if (sh == null)
                return;
            foreach (IpInfo ip in ListInterfacesIPs())
            {
                ToolStripItem tsi = cmsItem.Items.Add("Copy url for " + ip.Address + " (" + ip.InterfaceName + ", " + ip.InterfaceType + ")");
                tsi.Tag = "http://" + ip.Address + ":" + WebXplorer.Port + "/" + sh.Name + "/";
                tsi.Click += tsi_Click;
            }
        }

        void tsi_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = sender as ToolStripItem;
            if (tsi == null)
                return;
            Clipboard.SetText(tsi.Tag.ToString());
        }

        private void btGeneralOptions_Click(object sender, EventArgs e)
        {
            cmsGOptions.Show(btGeneralOptions, new Point(0, btGeneralOptions.Height));
        }

        private void shareScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebXplorer.SharedScreen = !WebXplorer.SharedScreen;
            shareScreenToolStripMenuItem.Checked = WebXplorer.SharedScreen;
        }

        private void tstbPassword_TextChanged(object sender, EventArgs e)
        {
            WebXplorer.SharedScreenPassword = tstbPassword.Text;
        }

        private void tstbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToLower(e.KeyChar);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            lock (this)
                myscreen = Screen.FromControl(this);
        }
    }
}
