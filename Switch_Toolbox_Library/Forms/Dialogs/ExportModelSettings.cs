﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ExportModelSettings : STForm
    {
        public DAE.ExportSettings Settings = new DAE.ExportSettings();

        public ExportModelSettings()
        {
            InitializeComponent();

            chkFlipUvsVertical.Checked = Settings.FlipTexCoordsVertical;
            exportTexturesChkBox.Checked = Settings.ExportTextures;
        }

        private void exportTexturesChkBox_CheckedChanged(object sender, EventArgs e) {
            Settings.ExportTextures = exportTexturesChkBox.Checked;
        }

        private void chkFlipUvsVertical_CheckedChanged(object sender, EventArgs e) {
            Settings.FlipTexCoordsVertical = chkFlipUvsVertical.Checked;
        }

        private void stCheckBox1_CheckedChanged(object sender, EventArgs e) {
            Settings.UseOldExporter = chkOldExporter.Checked;
        }
    }
}
