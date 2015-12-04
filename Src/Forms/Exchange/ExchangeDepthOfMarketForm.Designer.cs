namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    partial class ExchangeDepthOfMarketForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.domListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu_domEntry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelOrdersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenu_domEntry.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // domListView
            // 
            this.domListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.domListView.ContextMenuStrip = this.contextMenu_domEntry;
            this.domListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.domListView.FullRowSelect = true;
            this.domListView.Location = new System.Drawing.Point(3, 21);
            this.domListView.Name = "domListView";
            this.domListView.Size = new System.Drawing.Size(245, 432);
            this.domListView.TabIndex = 0;
            this.domListView.UseCompatibleStateImageBehavior = false;
            this.domListView.View = System.Windows.Forms.View.Details;
            this.domListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.domListView_MouseUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "||Bid";
            this.columnHeader1.Width = 74;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "||Price";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "||Ask";
            this.columnHeader3.Width = 81;
            // 
            // contextMenu_domEntry
            // 
            this.contextMenu_domEntry.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_domEntry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newOrderToolStripMenuItem,
            this.cancelOrdersToolStripMenuItem,
            this.chartToolStripMenuItem});
            this.contextMenu_domEntry.Name = "contextMenu_domEntry";
            this.contextMenu_domEntry.Size = new System.Drawing.Size(183, 82);
            // 
            // newOrderToolStripMenuItem
            // 
            this.newOrderToolStripMenuItem.Name = "newOrderToolStripMenuItem";
            this.newOrderToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.newOrderToolStripMenuItem.Text = "||New order";
            this.newOrderToolStripMenuItem.Click += new System.EventHandler(this.newOrderToolStripMenuItem_Click);
            // 
            // cancelOrdersToolStripMenuItem
            // 
            this.cancelOrdersToolStripMenuItem.Name = "cancelOrdersToolStripMenuItem";
            this.cancelOrdersToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.cancelOrdersToolStripMenuItem.Text = "||Cancel orders";
            this.cancelOrdersToolStripMenuItem.Click += new System.EventHandler(this.cancelOrdersToolStripMenuItem_Click);
            // 
            // chartToolStripMenuItem
            // 
            this.chartToolStripMenuItem.Name = "chartToolStripMenuItem";
            this.chartToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.chartToolStripMenuItem.Text = "||Chart";
            this.chartToolStripMenuItem.Click += new System.EventHandler(this.chartToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.domListView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 474);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 456);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "||Actions:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "||Depth of market:";
            // 
            // ExchangeDepthOfMarketForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 474);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExchangeDepthOfMarketForm";
            this.ShowIcon = false;
            this.Text = "||Depth of market";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeDepthOfMarketForm_FormClosing);
            this.Shown += new System.EventHandler(this.ExchangeDepthOfMarketForm_Shown);
            this.contextMenu_domEntry.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView domListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ContextMenuStrip contextMenu_domEntry;
		private System.Windows.Forms.ToolStripMenuItem newOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelOrdersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartToolStripMenuItem;
    }
}