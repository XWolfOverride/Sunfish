using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DolphinWebXplorer2.wx;
using System.Diagnostics;

namespace DolphinWebXplorer2
{
    public partial class FShared : Form
    {
        private bool valid;
        private string formerName;
        private FShared()
        {
            InitializeComponent();
        }

        public static bool Execute(WShared sh)
        {
            using (FShared f = new FShared())
            {
                f.tbId.DataBindings.Add("Text", sh, "Name");
                f.tbPath.DataBindings.Add("Text", sh, "Path");
                f.cbEnabled.DataBindings.Add("Checked", sh, "Enabled");
                f.cbSub.DataBindings.Add("Checked", sh, "AllowSubfolders");
                f.cbUpl.DataBindings.Add("Checked", sh, "AllowUpload");
                f.cbDele.DataBindings.Add("Checked", sh, "AllowDeletion");
                f.cbRena.DataBindings.Add("Checked", sh, "AllowRename");
                f.cbExec.DataBindings.Add("Checked", sh, "AllowExecution");
                f.cbMkDir.DataBindings.Add("Checked", sh, "AllowNewFolder");
                f.cbThumbnails.DataBindings.Add("Checked", sh, "SendThumbnails");
                f.formerName = sh.Name;
                f.btOpen.Visible=WebXplorer.Contains(sh);
                return f.ShowDialog() == DialogResult.OK;
            }
        }

        private void SaveData()
        {
            foreach (Control c in new Control[] { tbId, tbPath,cbEnabled,cbSub,cbUpl,cbDele,cbRena,cbExec })
            {
                c.DataBindings[0].WriteValue();
            }
        }

        private void tbId_TextChanged(object sender, EventArgs e)
        {
            if (tbId.Text.Contains(" ") || tbId.Text.Contains("|"))
            {
                tbId.BackColor = Color.FromArgb(255, 200, 200);
                valid = false;
                return;
            }
            tbId.BackColor = SystemColors.Window;
            valid = true;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if (valid)
            {
                SaveData();
                DialogResult = DialogResult.OK;
            }
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            Process.Start("http://localhost:" + WebXplorer.Port + "/" + formerName+"/");
        }
    }
}
