﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crypto.Core.Strategies;
using CryptoMarketClient.Strategies;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Crypto.UI.Forms {
    public partial class DataVisualiserForm : XtraForm {
        public DataVisualiserForm() {
            InitializeComponent();
        }

        IStrategyDataItemInfoOwner visual;
        public IStrategyDataItemInfoOwner Visual {
            get { return visual; }
            set {
                if(Visual == value)
                    return;
                visual = value;
                OnVisualChanged();
            }
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            OnVisualChanged();
        }

        protected virtual void OnVisualChanged() {
            if(!Visible)
                return;

            Text = Visual.Name + " - Data";

            int tableItemCount = Visual.DataItemInfos.Count(i => i.Visibility == DataVisibility.Both || i.Visibility == DataVisibility.Table);
            int chartItemCount = Visual.DataItemInfos.Count(i => i.Visibility == DataVisibility.Both || i.Visibility == DataVisibility.Chart); ;

            StrategyDataVisualiser visualizer = new StrategyDataVisualiser();
            if(tableItemCount > 0)
                ShowTableForm(Visual);
            if(chartItemCount > 0)
                ShowChartForm(Visual);
            
            //visualizer.Visualize(Visual, this.gcData, this.chartControl);

            //if(File.Exists(ChartSettingsFileName)) {
            //    DetachePoints();
            //    this.chartControl.LoadFromFile(ChartSettingsFileName);
            //    AttachPoints();
            //}
            //if(this.chartControl.Series.Count == 0)
            //    this.tpChartPage.Visible = false;

        }

        private void ShowChartForm(IStrategyDataItemInfoOwner visual) {
            XtraForm form = new XtraForm();
            ChartDataControl control = new ChartDataControl();
            control.Dock = DockStyle.Fill;
            control.Visual = visual;
            form.Controls.Add(control);
            StrategyDataVisualiser visualiser = new StrategyDataVisualiser();
            visualiser.Visualize(visual, null, control.Chart);

            form.Text = visual.Name + " - Data Chart";
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void ShowTableForm(IStrategyDataItemInfoOwner visual) {
            XtraForm form = new XtraForm();
            GridDataControl control = new GridDataControl();
            control.Grid.DoubleClick += OnGridControlDoubleClick;
            control.Dock = DockStyle.Fill;
            form.Controls.Add(control);
            StrategyDataVisualiser visualiser = new StrategyDataVisualiser();
            visualiser.Visualize(visual, control.Grid, null);

            form.Text = visual.Name + " - Data Table";
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void OnGridControlDoubleClick(object sender, EventArgs e) {
            GridControl grid = (GridControl)sender;
            GridView view = (GridView)grid.MainView;
            if(view.FocusedRowHandle != GridControl.InvalidRowHandle && view.FocusedColumn != null) {
                StrategyDataItemInfo info = (StrategyDataItemInfo)view.FocusedColumn.Tag;
                if(info.DetailInfo != null)
                    info = info.DetailInfo;
                if(info.Type != DataType.ChartData)
                    return;
                info.Value = view.GetFocusedRow();
                ShowChartForm(info);
            }
        }
    }
}
