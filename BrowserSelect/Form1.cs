﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrowserSelect.Properties;

namespace BrowserSelect {
    public partial class Form1 : Form {
        List<Browser> browsers = BrowserFinder.find().Where(b => !Settings.Default.HideBrowsers.Contains(b.exec)).ToList();
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            int i = 0;
            foreach(var browser in browsers) {
                var buc = new BrowserUC(browser,i);
                buc.Left = 128 * i++;
                buc.Click += browser_click;
                this.Controls.Add(buc);
            }
            this.Width = i * 128 + 20 + 20;
            this.KeyPreview = true;
            this.Text = Program.url;

            this.Icon = IconExtractor.fromFile(Application.ExecutablePath);

            add_button("About", show_about, 0);
            add_button("Settings", show_setting, 1);

            center_me();
        }

        private void show_setting(object sender, EventArgs e) {
            new frm_settings().ShowDialog();
        }

        private void show_about(object sender, EventArgs e) {
            new frm_About().ShowDialog();
        }

        private void add_button(string text,EventHandler evt,int index) {
            var btn = new VButton();
            btn.Text = text;
            btn.Left = this.Width - 35;
            btn.Top = index*80;
            btn.Anchor = AnchorStyles.Right;
            btn.Width = 20;
            btn.Height = 75;
            Controls.Add(btn);
            btn.Click += evt;
        }

        private void browser_click(object sender, EventArgs e) {
            BrowserUC uc;
            if (sender is BrowserUC)
                uc = (BrowserUC)sender;
            else if (((Control)sender).Parent is BrowserUC)
                uc = (BrowserUC)((Control)sender).Parent;
            else
                throw new Exception("this should not happen");
            open_url(uc.browser);
        }

        private void open_url(Browser b) {
            if (b.exec == "edge"){
                //edge is a universal app , which means we can't just run it like other browsers
                Process.Start("microsoft-edge:"+Program.url
                    .Replace(" ","%20")
                    .Replace("\"", "%22"));
            }
            else
                Process.Start(b.exec, "\""+Program.url.Replace("\"","%22")+"\"");
            Application.Exit();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e) {
            int i = 1;
            foreach (var browser in browsers) {
                if (browser.shortcuts.Contains(e.KeyChar) || e.KeyChar==(Convert.ToString(i++))[0]) {
                    open_url(browser);
                    return;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
        }

        private void center_me() {
            var wa = Screen.PrimaryScreen.WorkingArea;
            var left = wa.Width / 2 + wa.Left - Width / 2;
            var top = wa.Height / 2 + wa.Top - Height / 2;
            
            this.Location = new Point(left, top);
        }
    }
}
