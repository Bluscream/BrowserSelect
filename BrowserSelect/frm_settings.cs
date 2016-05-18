﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrowserSelect.Properties;
using Microsoft.Win32;

namespace BrowserSelect {
    public partial class frm_settings : Form {
        public frm_settings() {
            InitializeComponent();
        }

        private void frm_settings_Load(object sender, EventArgs e) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice")) {
                var default_browser = key.GetValue("ProgId");

                //disable the set default if already default
                if (default_browser != null && (string)default_browser == "bselectURL")
                    btn_setdefault.Enabled = false;
            }

            var browsers = BrowserFinder.find();
            foreach (Browser b in browsers)
            {
                browser_filter.Items.Add(b,!Settings.Default.HideBrowsers.Contains(b.exec));
            }
        }

        private void btn_setdefault_Click(object sender, EventArgs e) {
            //http
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice")) {
                key.SetValue("ProgId", "bselectURL");
            }
            //https
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice")) {
                key.SetValue("ProgId", "bselectURL");
            }

            btn_setdefault.Enabled = false;
        }

        private void browser_filter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                Settings.Default.HideBrowsers.Remove(((Browser) browser_filter.Items[e.Index]).exec);
            }
            else
            {
                Settings.Default.HideBrowsers.Add(((Browser)browser_filter.Items[e.Index]).exec);
            }
            Settings.Default.Save();
        }
    }
}
