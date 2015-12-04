namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    partial class ExchangeSecurityListForm
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
            this.securityListView = new System.Windows.Forms.ListView();
            this.codeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.parentCodeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.baseCurrencyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.scaleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.priceStepHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lotHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.expirationHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.minPriceHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.maxPriceHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenu_SelectedSecurity = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.depthOfMarketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu_Empty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenu_SelectedSecurity.SuspendLayout();
            this.contextMenu_Empty.SuspendLayout();
            this.SuspendLayout();
            // 
            // securityListView
            // 
            this.securityListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.codeHeader,
            this.typeHeader,
            this.parentCodeHeader,
            this.statusHeader,
            this.descriptionHeader,
            this.baseCurrencyHeader,
            this.scaleHeader,
            this.priceStepHeader,
            this.lotHeader,
            this.expirationHeader,
            this.minPriceHeader,
            this.maxPriceHeader});
            this.securityListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.securityListView.FullRowSelect = true;
            this.securityListView.Location = new System.Drawing.Point(3, 3);
            this.securityListView.Name = "securityListView";
            this.securityListView.Size = new System.Drawing.Size(837, 211);
            this.securityListView.TabIndex = 0;
            this.securityListView.UseCompatibleStateImageBehavior = false;
            this.securityListView.View = System.Windows.Forms.View.Details;
            this.securityListView.SelectedIndexChanged += new System.EventHandler(this.securityListView_SelectedIndexChanged);
            this.securityListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.securityListView_MouseUp);
            // 
            // codeHeader
            // 
            this.codeHeader.Text = "||Code";
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "||Type";
            // 
            // parentCodeHeader
            // 
            this.parentCodeHeader.Text = "||Parent code";
            this.parentCodeHeader.Width = 94;
            // 
            // statusHeader
            // 
            this.statusHeader.Text = "||Status";
            // 
            // descriptionHeader
            // 
            this.descriptionHeader.Text = "||Description";
            this.descriptionHeader.Width = 88;
            // 
            // baseCurrencyHeader
            // 
            this.baseCurrencyHeader.Text = "||Base currency code";
            this.baseCurrencyHeader.Width = 141;
            // 
            // scaleHeader
            // 
            this.scaleHeader.Text = "||Scale";
            // 
            // priceStepHeader
            // 
            this.priceStepHeader.Text = "||Price step";
            this.priceStepHeader.Width = 81;
            // 
            // lotHeader
            // 
            this.lotHeader.Text = "||Lot";
            this.lotHeader.Width = 42;
            // 
            // expirationHeader
            // 
            this.expirationHeader.Text = "||Expiration";
            this.expirationHeader.Width = 85;
            // 
            // minPriceHeader
            // 
            this.minPriceHeader.Text = "||Min price";
            this.minPriceHeader.Width = 77;
            // 
            // maxPriceHeader
            // 
            this.maxPriceHeader.Text = "||Max price";
            this.maxPriceHeader.Width = 78;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.securityListView, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(843, 217);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // contextMenu_SelectedSecurity
            // 
            this.contextMenu_SelectedSecurity.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_SelectedSecurity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.depthOfMarketToolStripMenuItem,
            this.chartToolStripMenuItem});
            this.contextMenu_SelectedSecurity.Name = "contextMenu_SelectedSecurity";
            this.contextMenu_SelectedSecurity.Size = new System.Drawing.Size(202, 56);
            // 
            // depthOfMarketToolStripMenuItem
            // 
            this.depthOfMarketToolStripMenuItem.Name = "depthOfMarketToolStripMenuItem";
            this.depthOfMarketToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.depthOfMarketToolStripMenuItem.Text = "||Depth of market";
            this.depthOfMarketToolStripMenuItem.Click += new System.EventHandler(this.depthOfMarketToolStripMenuItem_Click);
            // 
            // chartToolStripMenuItem
            // 
            this.chartToolStripMenuItem.Name = "chartToolStripMenuItem";
            this.chartToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.chartToolStripMenuItem.Text = "||Chart";
            this.chartToolStripMenuItem.Click += new System.EventHandler(this.chartToolStripMenuItem_Click);
            // 
            // contextMenu_Empty
            // 
            this.contextMenu_Empty.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_Empty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByContentToolStripMenuItem});
            this.contextMenu_Empty.Name = "contextMenu_Empty";
            this.contextMenu_Empty.Size = new System.Drawing.Size(289, 56);
            // 
            // columnAutowidthByHeaderToolStripMenuItem
            // 
            this.columnAutowidthByHeaderToolStripMenuItem.Name = "columnAutowidthByHeaderToolStripMenuItem";
            this.columnAutowidthByHeaderToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.columnAutowidthByHeaderToolStripMenuItem.Text = "||Column autowidth by header";
            this.columnAutowidthByHeaderToolStripMenuItem.Click += new System.EventHandler(this.columnAutowidthByHeaderToolStripMenuItem_Click);
            // 
            // columnAutowidthByContentToolStripMenuItem
            // 
            this.columnAutowidthByContentToolStripMenuItem.Name = "columnAutowidthByContentToolStripMenuItem";
            this.columnAutowidthByContentToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.columnAutowidthByContentToolStripMenuItem.Text = "||Column autowidth by content";
            this.columnAutowidthByContentToolStripMenuItem.Click += new System.EventHandler(this.columnAutowidthByContentToolStripMenuItem_Click);
            // 
            // ExchangeSecurityListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 217);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExchangeSecurityListForm";
            this.ShowIcon = false;
            this.Text = "||Security list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeSecurityListForm_FormClosing);
            this.Shown += new System.EventHandler(this.ExchangeSecurityListForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenu_SelectedSecurity.ResumeLayout(false);
            this.contextMenu_Empty.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView securityListView;
        private System.Windows.Forms.ColumnHeader codeHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.ColumnHeader parentCodeHeader;
        private System.Windows.Forms.ColumnHeader statusHeader;
        private System.Windows.Forms.ColumnHeader descriptionHeader;
        private System.Windows.Forms.ColumnHeader baseCurrencyHeader;
        private System.Windows.Forms.ColumnHeader scaleHeader;
        private System.Windows.Forms.ColumnHeader priceStepHeader;
        private System.Windows.Forms.ColumnHeader lotHeader;
        private System.Windows.Forms.ColumnHeader expirationHeader;
        private System.Windows.Forms.ColumnHeader minPriceHeader;
        private System.Windows.Forms.ColumnHeader maxPriceHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ContextMenuStrip contextMenu_SelectedSecurity;
		private System.Windows.Forms.ToolStripMenuItem depthOfMarketToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Empty;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByContentToolStripMenuItem;
    }
}