using DolphinWebXplorer2.Configurator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DolphinWebXplorer2
{
    public partial class FServiceConf : Form
    {
        private SunfishServiceConfiguration ssc;
        private int panelOffset;

        private FServiceConf()
        {
            InitializeComponent();
            panelOffset = ClientSize.Height - pScreen.Top;
        }

        public static bool Execute(SunfishServiceConfiguration ssc)
        {
            using (FServiceConf f = new FServiceConf())
            {
                f.ssc = ssc;
                f.LoadData();
                return f.ShowDialog() == DialogResult.OK;
            }
        }

        private void LoadData()
        {
            cbType.SelectedItem = ssc.Type;
            cbActive.Checked = ssc.Enabled;
            tbName.Text = ssc.Name;
            tbLocation.Text = ssc.Name;
            LoadScreen();
        }

        private void SaveData()
        {
            ssc.Type = cbType.SelectedItem == null ? null : cbType.SelectedItem.ToString();
            ssc.Enabled = cbActive.Checked;
            ssc.Name = tbName.Text;
            ssc.Location = tbLocation.Text;
            // TODO Save from dynamic screen
        }

        private void AddElementLabel(ConfigurationElement ce, Panel p, ref int y)
        {
            if (string.IsNullOrEmpty(ce.Label))
                return;
            y += 3;
            Label l = new Label();
            l.Text = ce.Label;
            l.Top = y;
            l.AutoSize = true;
            y += l.Height + 3;
            p.Controls.Add(l);
        }

        private void FinishElement(Control c, ConfigurationElement ce, Panel p, ref int y)
        {
            if (!string.IsNullOrEmpty(ce.Tooltip))
                toolTip1.SetToolTip(c, ce.Tooltip);
            c.Top = y;
            y += c.Height + 3;
            c.Tag = ce;
            p.Controls.Add(c);
        }

        private void AddElementMessage(ConfigurationElement ce, Panel p, ref int y)
        {
            AddElementLabel(ce, p, ref y);
            ConfigurationMessage cm = (ConfigurationMessage)ce;
            Label c = new Label();
            c.Text = cm.Message;
            c.BorderStyle = BorderStyle.FixedSingle;
            switch (cm.Type)
            {
                case ConfigurationMessage.MessageType.ERROR:
                    c.BackColor = Color.FromArgb(255, 200, 200);
                    break;
                case ConfigurationMessage.MessageType.INFO:
                    c.BackColor = Color.FromArgb(255, 255, 255);
                    break;
                case ConfigurationMessage.MessageType.WARNING:
                    c.BackColor = Color.WhiteSmoke;
                    break;
            }
            c.MaximumSize = new Size(p.ClientSize.Width - 28, int.MaxValue);
            c.MinimumSize = new Size(c.MaximumSize.Width, 0);
            c.Left = 14;
            c.AutoSize = true;
            FinishElement(c, ce, p, ref y);
        }

        private void AddElementString(ConfigurationElement ce, Panel p, ref int y)
        {
            AddElementLabel(ce, p, ref y);
            ConfigurationString cs = (ConfigurationString)ce;
            TextBox c = new TextBox();
            c.Text = ssc.GetConf(ce.Id, cs.DefaultValue);
            c.Left = 14;
            c.Width = p.ClientSize.Width - 28;
            if (cs.IsPassword)
                c.PasswordChar = '*';
            //TODO: Handle isDirectory and IsFile properties
            FinishElement(c, ce, p, ref y);
        }

        private void LoadScreen()
        {
            // Remove all
            pScreen.Controls.Clear();
            string type = cbType.SelectedItem == null ? null : cbType.SelectedItem.ToString();
            if (string.IsNullOrEmpty(type))
            {
                ClientSize = new Size(ClientSize.Width, pScreen.Top + panelOffset);
                btAdv.Visible = false;
                return;
            }
            ConfigurationScreen cs = SunfishService.GetConfigurator(type).GetConfigurationScreen();
            btAdv.Visible = cs.Advanced != null;
            // Create new
            int y = 0;
            foreach (ConfigurationElement ce in cs.Elements)
            {
                if (ce is ConfigurationMessage)
                    AddElementMessage(ce, pScreen, ref y);
                else if (ce is ConfigurationString)
                    AddElementString(ce, pScreen, ref y);
            }
            ClientSize = new Size(ClientSize.Width, y + pScreen.Top + panelOffset);
        }

        private void FServiceConf_Load(object sender, EventArgs e)
        {
            cbType.Items.AddRange(SunfishService.GetTypes());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveData();
            DialogResult = DialogResult.OK;
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScreen();
        }
    }
}
