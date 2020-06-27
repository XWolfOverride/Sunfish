using DolphinWebXplorer2.Configurator;
using System;
using System.Drawing;
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
            cbType.Items.AddRange(SunfishService.GetTypes());
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
            if (ssc.Type != null)
                for (int i = 0; i < cbType.Items.Count; i++)
                {
                    object o = cbType.Items[i];
                    if (o != null && o.ToString() == ssc.Type)
                    {
                        cbType.SelectedIndex = i;
                        break;
                    }
                }
            cbActive.Checked = ssc.Enabled;
            tbName.Text = ssc.Name;
            tbLocation.Text = ssc.Location;
            LoadScreen();
        }

        private bool ValidateData()
        {
            bool valid = true;
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                valid = false;
                tbName.BackColor = Color.LightCoral;
            }
            else
                tbName.BackColor = Color.LightGoldenrodYellow;
            if (string.IsNullOrWhiteSpace(tbLocation.Text))
            {
                valid = false;
                tbLocation.BackColor = Color.LightCoral;
            }
            else
                tbLocation.BackColor = Color.LightGoldenrodYellow;
            return valid & ValidateDynamicScreen();
        }

        private void SaveData()
        {
            ssc.Type = cbType.SelectedItem == null ? null : cbType.SelectedItem.ToString();
            ssc.Enabled = cbActive.Checked;
            ssc.Name = tbName.Text;
            ssc.Location = tbLocation.Text;
            SaveDynamicScreen();
        }

        #region Dynamic properties
        private void AddElementLabel(ConfigurationElement ce, Panel p, ref int y)
        {
            if (string.IsNullOrEmpty(ce.Label))
                return;
            y += 3;
            Label l = new Label();
            l.Text = ce.Label;
            l.Top = y;
            l.AutoSize = true;
            p.Controls.Add(l);
            y += l.Height + 3;
        }

        private void FinishElement(Control c, ConfigurationElement ce, Panel p, ref int y)
        {
            if (!string.IsNullOrEmpty(ce.Tooltip))
                toolTip1.SetToolTip(c, ce.Tooltip);
            c.Top = y;
            c.Tag = ce;
            p.Controls.Add(c);
            y += c.Height + 3;
        }

        private void AddElementMessage(ConfigurationMessage ce, Panel p, ref int y)
        {
            AddElementLabel(ce, p, ref y);
            Label c = new Label();
            c.Text = ce.Message;
            c.BorderStyle = BorderStyle.FixedSingle;
            c.MaximumSize = new Size(p.ClientSize.Width - 28, int.MaxValue);
            c.MinimumSize = new Size(c.MaximumSize.Width, 0);
            c.Left = 14;
            c.AutoSize = true;
            FinishElement(c, ce, p, ref y);
            switch (ce.Type)
            {
                case ConfigurationMessage.MessageType.ERROR:
                    c.BackColor = Color.FromArgb(255, 200, 200);
                    break;
                case ConfigurationMessage.MessageType.INFO:
                    c.BackColor = Color.FromArgb(255, 255, 255);
                    break;
                case ConfigurationMessage.MessageType.WARNING:
                    c.BackColor = Color.PaleGoldenrod;
                    break;
            }
        }

        private void AddElementString(ConfigurationString ce, Panel p, ref int y)
        {
            AddElementLabel(ce, p, ref y);
            TextBox c = new TextBox();
            c.Text = ssc.GetConf(ce.Id, ce.DefaultValue);
            // WARNING: Values for standard DPI
            c.Left = 9;
            c.Width = p.ClientSize.Width - 16;
            if (ce.IsPassword)
                c.PasswordChar = '*';
            if (ce.Mandatory)
                c.BackColor = ce.UIMandatoryColor;
            //TODO: Handle isDirectory and IsFile properties
            FinishElement(c, ce, p, ref y);
        }

        private void AddElementBool(ConfigurationBool ce, Panel p, ref int y)
        {
            CheckBox c = new CheckBox();
            c.Text = ce.Label;
            c.Checked = ssc.GetConf<bool>(ce.Id, ce.DefaultValue);
            c.Left = 14;
            c.Width = p.ClientSize.Width - 28;
            FinishElement(c, ce, p, ref y);
        }

        private void LoadScreen()
        {
            int height = Height;
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
                    AddElementMessage((ConfigurationMessage)ce, pScreen, ref y);
                else if (ce is ConfigurationString)
                    AddElementString((ConfigurationString)ce, pScreen, ref y);
                else if (ce is ConfigurationBool)
                    AddElementBool((ConfigurationBool)ce, pScreen, ref y);
            }
            ClientSize = new Size(ClientSize.Width, y + pScreen.Top + panelOffset + 5);
            if (Visible)
                Top -= (Height - height) / 2;
        }

        private bool ValidateDynamicScreen()
        {
            bool valid = true;
            foreach (Control c in pScreen.Controls)
            {
                ConfigurationElement ce = c.Tag as ConfigurationElement;
                if (ce == null || !ce.Mandatory)
                    continue;
                if (ce.isEmpty(c))
                {
                    valid = false;
                    c.BackColor = Color.LightCoral;
                }
                else
                {
                    c.BackColor = ce.UIMandatoryColor;
                }
            }
            return valid;
        }

        private void SaveDynamicScreen()
        {
            foreach (Control c in pScreen.Controls)
            {
                ConfigurationElement ce = c.Tag as ConfigurationElement;
                if (ce == null || ce.Id == null)
                    continue;
                ssc.Settings[ce.Id] = ce.getValue(c);
            }
        }

        #endregion

        private void FServiceConf_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
                return;
            SaveData();
            DialogResult = DialogResult.OK;
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScreen();
        }
    }
}
