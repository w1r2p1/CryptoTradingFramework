﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoMarketClient {
    public partial class TelegramRegistrationForm : DevExpress.XtraEditors.XtraForm {
        public TelegramRegistrationForm() {
            InitializeComponent();
        }

        public string Command {
            get { return this.textEdit1.Text.Trim(); }
            set { this.textEdit1.Text = value; }
        }
    }
}
