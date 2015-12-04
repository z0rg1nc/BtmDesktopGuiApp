namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
	partial class ExchangeWithdrawListForm
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
            this.withdrawListView = new System.Windows.Forms.ListView();
            this.dateHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.currencyCodeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.valueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedPsFeeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedFeeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.totalValueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finalPsFeeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finalFeeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusCommentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.paymentDetailsHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.accountHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.withdrawGuidHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lockedFundsGuidHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu_withdrawEntry = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyStatusTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPaymentDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu_withdrawEmpty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.contextMenu_withdrawEntry.SuspendLayout();
            this.contextMenu_withdrawEmpty.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // withdrawListView
            // 
            this.withdrawListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dateHeader,
            this.currencyCodeHeader,
            this.valueHeader,
            this.estimatedPsFeeHeader,
            this.estimatedFeeHeader,
            this.totalValueHeader,
            this.finalPsFeeHeader,
            this.finalFeeHeader,
            this.statusHeader,
            this.statusCommentHeader,
            this.paymentDetailsHeader,
            this.accountHeader,
            this.withdrawGuidHeader,
            this.lockedFundsGuidHeader});
            this.withdrawListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.withdrawListView.FullRowSelect = true;
            this.withdrawListView.Location = new System.Drawing.Point(3, 41);
            this.withdrawListView.Name = "withdrawListView";
            this.withdrawListView.Size = new System.Drawing.Size(857, 203);
            this.withdrawListView.TabIndex = 0;
            this.withdrawListView.UseCompatibleStateImageBehavior = false;
            this.withdrawListView.View = System.Windows.Forms.View.Details;
            this.withdrawListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.withdrawListView_ColumnClick);
            this.withdrawListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.withdrawListView_MouseClick);
            this.withdrawListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.withdrawListView_MouseUp);
            // 
            // dateHeader
            // 
            this.dateHeader.Text = "||Date";
            // 
            // currencyCodeHeader
            // 
            this.currencyCodeHeader.Text = "||Currency code";
            // 
            // valueHeader
            // 
            this.valueHeader.Text = "||Value";
            // 
            // estimatedPsFeeHeader
            // 
            this.estimatedPsFeeHeader.Text = "||Estimated payment system fee";
            // 
            // estimatedFeeHeader
            // 
            this.estimatedFeeHeader.Text = "||Estimated fee";
            // 
            // totalValueHeader
            // 
            this.totalValueHeader.Text = "||Total value";
            // 
            // finalPsFeeHeader
            // 
            this.finalPsFeeHeader.Text = "||Final payment system fee";
            // 
            // finalFeeHeader
            // 
            this.finalFeeHeader.Text = "||Final fee";
            // 
            // statusHeader
            // 
            this.statusHeader.Text = "||Status";
            // 
            // statusCommentHeader
            // 
            this.statusCommentHeader.Text = "||Status comment";
            // 
            // paymentDetailsHeader
            // 
            this.paymentDetailsHeader.Text = "||Payment details";
            // 
            // accountHeader
            // 
            this.accountHeader.Text = "||Account GUID";
            // 
            // withdrawGuidHeader
            // 
            this.withdrawGuidHeader.Text = "||Withdraw GUID";
            // 
            // lockedFundsGuidHeader
            // 
            this.lockedFundsGuidHeader.Text = "||Locked fund GUID";
            // 
            // contextMenu_withdrawEntry
            // 
            this.contextMenu_withdrawEntry.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_withdrawEntry.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyStatusTextToolStripMenuItem,
            this.copyPaymentDetailsToolStripMenuItem});
            this.contextMenu_withdrawEntry.Name = "contextMenu_withdrawEntry";
            this.contextMenu_withdrawEntry.Size = new System.Drawing.Size(236, 56);
            // 
            // copyStatusTextToolStripMenuItem
            // 
            this.copyStatusTextToolStripMenuItem.Name = "copyStatusTextToolStripMenuItem";
            this.copyStatusTextToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.copyStatusTextToolStripMenuItem.Text = "||Copy status comment";
            this.copyStatusTextToolStripMenuItem.Click += new System.EventHandler(this.copyStatusTextToolStripMenuItem_Click);
            // 
            // copyPaymentDetailsToolStripMenuItem
            // 
            this.copyPaymentDetailsToolStripMenuItem.Name = "copyPaymentDetailsToolStripMenuItem";
            this.copyPaymentDetailsToolStripMenuItem.Size = new System.Drawing.Size(235, 26);
            this.copyPaymentDetailsToolStripMenuItem.Text = "||View payment details";
            this.copyPaymentDetailsToolStripMenuItem.Click += new System.EventHandler(this.copyPaymentDetailsToolStripMenuItem_Click);
            // 
            // contextMenu_withdrawEmpty
            // 
            this.contextMenu_withdrawEmpty.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_withdrawEmpty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByContentToolStripMenuItem});
            this.contextMenu_withdrawEmpty.Name = "contextMenu_withdrawEmpty";
            this.contextMenu_withdrawEmpty.Size = new System.Drawing.Size(289, 56);
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.withdrawListView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(863, 247);
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
            this.tableLayoutPanel2.Controls.Add(this.comboBox1, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(857, 32);
            this.tableLayoutPanel2.TabIndex = 1;
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
            this.comboBox1.Location = new System.Drawing.Point(733, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.ValueMember = "100";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(234, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(72, 22);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "||Fault";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 22);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "||Processing";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(124, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(104, 22);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "||Complete";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // ExchangeWithdrawListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 247);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ExchangeWithdrawListForm";
            this.ShowIcon = false;
            this.Text = "||Withdraw list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeWithdrawListForm_FormClosing);
            this.Shown += new System.EventHandler(this.ExchangeWithdrawListForm_Shown);
            this.contextMenu_withdrawEntry.ResumeLayout(false);
            this.contextMenu_withdrawEmpty.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView withdrawListView;
		private System.Windows.Forms.ColumnHeader dateHeader;
		private System.Windows.Forms.ColumnHeader currencyCodeHeader;
		private System.Windows.Forms.ColumnHeader valueHeader;
		private System.Windows.Forms.ColumnHeader estimatedPsFeeHeader;
		private System.Windows.Forms.ColumnHeader totalValueHeader;
		private System.Windows.Forms.ColumnHeader statusHeader;
		private System.Windows.Forms.ColumnHeader statusCommentHeader;
		private System.Windows.Forms.ColumnHeader paymentDetailsHeader;
		private System.Windows.Forms.ColumnHeader accountHeader;
		private System.Windows.Forms.ColumnHeader withdrawGuidHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenu_withdrawEntry;
        private System.Windows.Forms.ToolStripMenuItem copyStatusTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPaymentDetailsToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader lockedFundsGuidHeader;
        private System.Windows.Forms.ColumnHeader estimatedFeeHeader;
        private System.Windows.Forms.ColumnHeader finalPsFeeHeader;
        private System.Windows.Forms.ColumnHeader finalFeeHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenu_withdrawEmpty;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByContentToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}