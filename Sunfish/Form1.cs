using DolphinWebXplorer2.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace DolphinWebXplorer2
{
    public partial class Form1 : Form
    {
        public Font smallfont;
        public Brush itembrushgray;
        private Screen myscreen;
        public Form1()
        {
            InitializeComponent();
            itembrushgray = new SolidBrush(Color.Gray);
            smallfont = new Font(Font.FontFamily, 7, FontStyle.Regular);
            Icon = Resources.sunfishWebServer;
            PopulateData();
            Text += " " + Program.VERSION;
            myscreen = Screen.FromControl(this);
        }

        private void PopulateData()
        {
            Enabled = false;
            nudPort.Value = Sunfish.Port;
            cbActive.Checked = Sunfish.Active;
            cbRootList.Checked = Sunfish.RootMenu;
            cbAdmin.Checked = Sunfish.AdminPanel;
            tbAdminPWD.Text = Sunfish.AdminPwd;
            lbPaths.Items.Clear();
            foreach (SunfishService s in Sunfish.Services)
                lbPaths.Items.Add(s);
            Enabled = true;
            tbAdminPWD.Enabled = cbAdmin.Checked;
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

        private void EditConfiguration(SunfishServiceConfiguration ssc, SunfishService oldService)
        {
            if (FServiceConf.Execute(ssc))
            {
                try
                {
                    if (oldService == null)
                        lbPaths.Items.Add(Sunfish.AddService(ssc));
                    else
                    {
                        SunfishService s = Sunfish.ReplaceService(oldService, ssc);
                        int idx = lbPaths.Items.IndexOf(oldService);
                        if (idx < 0)
                            lbPaths.Items.Add(s);
                        else
                            lbPaths.Items[idx] = s;
                    }
                }
                catch (Exception ex)
                {
                    ex.Show();
                }
            }
        }

        public Screen MyScreen { get { lock (this) return myscreen; } }

        private void button1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Sunfish.Save();
            }
            catch { };
            Sunfish.Active = false;
        }

        private void cbActive_CheckedChanged(object sender, EventArgs e)
        {
            nudPort.Enabled = !cbActive.Checked;
            if (!Enabled)
                return;
            try
            {
                Sunfish.Active = cbActive.Checked;
            }
            catch (Exception ex)
            {
                ex.Show();
            }
            cbActive.Checked = Sunfish.Active;
        }

        private void nudPort_ValueChanged(object sender, EventArgs e)
        {
            if (!Enabled)
                return;
            Sunfish.Port = (int)nudPort.Value;
            cbActive.Checked = Sunfish.Active;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            SunfishServiceConfiguration ssc = new SunfishServiceConfiguration();
            ssc.Type = "WebService";
            EditConfiguration(ssc, null);
        }

        private void clbPaths_DoubleClick(object sender, EventArgs e)
        {
            SunfishService s = (SunfishService)lbPaths.SelectedItem;
            if (s == null)
                return;
            EditConfiguration(s.Configuration, s);
        }

        private void lbPaths_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (e.Index < 0)
                return;
            SunfishService s = (SunfishService)lbPaths.Items[e.Index];
            Graphics g = e.Graphics;
            int y = e.Bounds.Top + 2;
            using (Brush itembrush = new SolidBrush(e.ForeColor))
            {
                g.DrawImage(s.Enabled ? Resources.foldericon : Resources.foldericond, 1, y, 24, 24);
                g.DrawString(s.Configuration.Name, lbPaths.Font, itembrush, 28, y);
                g.DrawString(s.Configuration.Location, smallfont, itembrushgray, 33, y + 12);
                //if (sh.AllowSubfolders)
                //    g.DrawImage(Resources.osubf, 16, y + 13);
                //if (sh.AllowUpload)
                //    g.DrawImage(Resources.ouplo, 3, y + 13);
            }
        }

        private void lbPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            btSub.Enabled = lbPaths.SelectedItem != null;
        }

        private void btSub_Click(object sender, EventArgs e)
        {
            SunfishService sh = (SunfishService)lbPaths.SelectedItem;
            if (sh == null)
                return;
            if (MessageBox.Show("Delete access " + sh.Configuration.Name + "? Can not be undone!", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Sunfish.DeleteService(sh);
                lbPaths.Items.Remove(sh);
            }
        }

        private void btShowIp_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder(Sunfish.Active ? "A" : "Ina");
            sb.Append("ctive.\r\n");
            foreach (IpInfo ip in ListInterfacesIPs())
            {
                sb.Append(ip.InterfaceName);
                sb.Append(" (");
                sb.Append(ip.InterfaceType);
                sb.Append("): ");
                sb.Append(ip.Address);
                sb.Append(":");
                sb.Append(Sunfish.Port);
                sb.Append("\r\n");
            }
            sb.Append("\r\nSunfish ");
            sb.Append(Program.VERSION);
            sb.Append(" (C) XWolfOverride@gmail.com 2007-2021\r\nEasy web server and folder shares.\r\nMIT license.");
            MessageBox.Show(sb.ToString(), "Sunfish information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmsItem_Opening(object sender, CancelEventArgs e)
        {
            SunfishService s = lbPaths.SelectedItem as SunfishService;
            editarToolStripMenuItem.Enabled = s != null;
            borrarToolStripMenuItem.Enabled = s != null;
            toolStripSeparator1.Visible = s != null;
            while (cmsItem.Items.Count > 4)
                cmsItem.Items.RemoveAt(cmsItem.Items.Count - 1);
            if (s == null)
                return;
            foreach (IpInfo ip in ListInterfacesIPs())
            {
                ToolStripItem tsi = cmsItem.Items.Add("Copy url for " + ip.Address + " (" + ip.InterfaceName + ", " + ip.InterfaceType + ")");
                tsi.Tag = "http://" + ip.Address + ":" + Sunfish.Port + "/" + s.Configuration.Location + "/";
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

        private void tstbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToLower(e.KeyChar);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            lock (this)
                myscreen = Screen.FromControl(this);
        }

        private void lbPaths_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void lbPaths_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                foreach (string file in files)
                {
                    SunfishServiceConfiguration ssc = new SunfishServiceConfiguration();
                    ssc.Type = "WebService";
                    string fil = file;
                    if (!Directory.Exists(file))
                        fil = Path.GetDirectoryName(fil);
                    ssc.Name = Path.GetFileName(fil);
                    ssc.Location = '/' + Path.GetFileName(fil);
                    ssc.Enabled = true;
                    ssc.Settings[Services.WebServiceConfigurator.CFG_PATH] = fil;
                    ssc.Settings[Services.WebServiceConfigurator.CFG_SHARE] = true;
                    EditConfiguration(ssc, null);
                }
                Activate();
            }
        }

        private void cbRootList_CheckedChanged(object sender, EventArgs e)
        {
            Sunfish.RootMenu = cbRootList.Checked;
        }

        private void cbAdmin_CheckedChanged(object sender, EventArgs e)
        {
            Sunfish.AdminPanel = cbAdmin.Checked;
            tbAdminPWD.Enabled = cbAdmin.Checked;
        }

        private void tbAdminPWD_TextChanged(object sender, EventArgs e)
        {
            Sunfish.AdminPwd = tbAdminPWD.Text;
        }
    }
}
