namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
	partial class ExchangeDepositListForm
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
            this.depositListView = new System.Windows.Forms.ListView();
            this.dateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.currencyCodeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.valueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedPsFeeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedFeeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.totalColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accountColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusCommentColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.paymentDetailsColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.validUntilColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.depositGuidColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu_Entry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyPaymentDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStatusTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prolongDepositFor1DayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu_Empty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.contextMenu_Entry.SuspendLayout();
            this.contextMenu_Empty.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // depositListView
            // 
            this.depositListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateColumnHeader,
            this.currencyCodeColumnHeader,
            this.valueColumnHeader,
            this.estimatedPsFeeColumnHeader,
            this.estimatedFeeColumnHeader,
            this.totalColumnHeader,
            this.accountColumnHeader,
            this.statusColumnHeader,
            this.statusCommentColumnHeader,
            this.paymentDetailsColumnHeader,
            this.validUntilColumnHeader,
            this.depositGuidColumnHeader});
            this.depositListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.depositListView.FullRowSelect = true;
            this.depositListView.Location = new System.Drawing.Point(3, 41);
            this.depositListView.Name = "depositListView";
            this.depositListView.Size = new System.Drawing.Size(758, 209);
            this.depositListView.TabIndex = 0;
            this.depositListView.UseCompatibleStateImageBehavior = false;
            this.depositListView.View = System.Windows.Forms.View.Details;
            this.depositListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.depositListView_ColumnClick);
            this.depositListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.depositListView_MouseUp);
            // 
            // dateColumnHeader
            // 
            this.dateColumnHeader.Text = "||Date";
            this.dateColumnHeader.Width = 79;
            // 
            // currencyCodeColumnHeader
            // 
            this.currencyCodeColumnHeader.Text = "||Currency code";
            // 
            // valueColumnHeader
            // 
            this.valueColumnHeader.Text = "||Value";
            // 
            // estimatedPsFeeColumnHeader
            // 
            this.estimatedPsFeeColumnHeader.Text = "||Estimated payment system fee";
            // 
            // estimatedFeeColumnHeader
            // 
            this.estimatedFeeColumnHeader.Text = "||Estimated fee";
            // 
            // totalColumnHeader
            // 
            this.totalColumnHeader.Text = "||Total";
            // 
            // accountColumnHeader
            // 
            this.accountColumnHeader.Text = "||Account GUID";
            // 
            // statusColumnHeader
            // 
            this.statusColumnHeader.Text = "||Status";
            // 
            // statusCommentColumnHeader
            // 
            this.statusCommentColumnHeader.Text = "||Status comment";
            // 
            // paymentDetailsColumnHeader
            // 
            this.paymentDetailsColumnHeader.Text = "||Payment details";
            // 
            // validUntilColumnHeader
            // 
            this.validUntilColumnHeader.Text = "||Valid until";
            // 
            // depositGuidColumnHeader
            // 
            this.depositGuidColumnHeader.Text = "||Deposit GUID";
            // 
            // contextMenu_Entry
            // 
            this.contextMenu_Entry.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_Entry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPaymentDetailsToolStripMenuItem,
            this.copyStatusTextToolStripMenuItem,
            this.prolongDepositFor1DayToolStripMenuItem});
            this.contextMenu_Entry.Name = "contextMenuStrip1";
            this.contextMenu_Entry.Size = new System.Drawing.Size(301, 110);
            // 
            // copyPaymentDetailsToolStripMenuItem
            // 
            this.copyPaymentDetailsToolStripMenuItem.Name = "copyPaymentDetailsToolStripMenuItem";
            this.copyPaymentDetailsToolStripMenuItem.Size = new System.Drawing.Size(300, 26);
            this.copyPaymentDetailsToolStripMenuItem.Text = "||View payment details";
            this.copyPaymentDetailsToolStripMenuItem.Click += new System.EventHandler(this.copyPaymentDetailsToolStripMenuItem_Click);
            // 
            // copyStatusTextToolStripMenuItem
            // 
            this.copyStatusTextToolStripMenuItem.Name = "copyStatusTextToolStripMenuItem";
            this.copyStatusTextToolStripMenuItem.Size = new System.Drawing.Size(300, 26);
            this.copyStatusTextToolStripMenuItem.Text = "||Copy status comment";
            this.copyStatusTextToolStripMenuItem.Click += new System.EventHandler(this.copyStatusTextToolStripMenuItem_Click);
            // 
            // prolongDepositFor1DayToolStripMenuItem
            // 
            this.prolongDepositFor1DayToolStripMenuItem.Name = "prolongDepositFor1DayToolStripMenuItem";
            this.prolongDepositFor1DayToolStripMenuItem.Size = new System.Drawing.Size(300, 26);
            this.prolongDepositFor1DayToolStripMenuItem.Text = "||Prolong deposit for 1 day more";
            this.prolongDepositFor1DayToolStripMenuItem.Click += new System.EventHandler(this.prolongDepositFor1DayToolStripMenuItem_Click);
            // 
            // contextMenu_Empty
            // 
            this.contextMenu_Empty.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_Empty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByColumnToolStripMenuItem});
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
            // columnAutowidthByColumnToolStripMenuItem
            // 
            this.columnAutowidthByColumnToolStripMenuItem.Name = "columnAutowidthByColumnToolStripMenuItem";
            this.columnAutowidthByColumnToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.columnAutowidthByColumnToolStripMenuItem.Text = "||Column autowidth by content";
            this.columnAutowidthByColumnToolStripMenuItem.Click += new System.EventHandler(this.columnAutowidthByColumnToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.depositListView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(764, 253);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBox1, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(758, 32);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(79, 22);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "||Active";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(88, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(104, 22);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "||Complete";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(198, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(141, 22);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "||Fault or expired";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.DisplayMember = "100";
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "10",
            "100",
            "1000"});
            this.comboBox1.Location = new System.Drawing.Point(634, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.ValueMember = "100";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // ExchangeDepositListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 253);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ExchangeDepositListForm";
            this.ShowIcon = false;
            this.Text = "||Deposit list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeDepositListForm_FormClosing);
            this.Load += new System.EventHandler(this.ExchangeDepositListForm_Load);
            this.Shown += new System.EventHandler(this.ExchangeDepositListForm_Shown);
            this.contextMenu_Entry.ResumeLayout(false);
            this.contextMenu_Empty.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView depositListView;
		private System.Windows.Forms.ColumnHeader dateColumnHeader;
		private System.Windows.Forms.ColumnHeader currencyCodeColumnHeader;
		private System.Windows.Forms.ColumnHeader valueColumnHeader;
		private System.Windows.Forms.ColumnHeader estimatedPsFeeColumnHeader;
		private System.Windows.Forms.ColumnHeader accountColumnHeader;
		private System.Windows.Forms.ColumnHeader statusColumnHeader;
		private System.Windows.Forms.ColumnHeader statusCommentColumnHeader;
		private System.Windows.Forms.ColumnHeader paymentDetailsColumnHeader;
		private System.Windows.Forms.ColumnHeader validUntilColumnHeader;
		private System.Windows.Forms.ColumnHeader totalColumnHeader;
		private System.Windows.Forms.ContextMenuStrip contextMenu_Entry;
		private System.Windows.Forms.ToolStripMenuItem copyPaymentDetailsToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader depositGuidColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem copyStatusTextToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader estimatedFeeColumnHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Empty;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prolongDepositFor1DayToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}