namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    partial class ExchangeCurrencyListForm
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
            this.currencyListView = new System.Windows.Forms.ListView();
            this.codeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.scaleHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.minDepositHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.maxDepositHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.depositInaccuracyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.newDepositFeeBtmHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.depositFeeConstHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.depositFeePercentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.minWithdrawHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.maxWithdrawHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.newWithdrawFeeBtmHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.withdrawFeeConstHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.withdrawFeePercentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // currencyListView
            // 
            this.currencyListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.codeHeader,
            this.descriptionHeader,
            this.scaleHeader,
            this.minDepositHeader,
            this.maxDepositHeader,
            this.depositInaccuracyHeader,
            this.newDepositFeeBtmHeader,
            this.depositFeeConstHeader,
            this.depositFeePercentHeader,
            this.minWithdrawHeader,
            this.maxWithdrawHeader,
            this.newWithdrawFeeBtmHeader,
            this.withdrawFeeConstHeader,
            this.withdrawFeePercentHeader});
            this.currencyListView.ContextMenuStrip = this.contextMenuStrip1;
            this.currencyListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currencyListView.Location = new System.Drawing.Point(3, 3);
            this.currencyListView.Name = "currencyListView";
            this.currencyListView.Size = new System.Drawing.Size(585, 177);
            this.currencyListView.TabIndex = 0;
            this.currencyListView.UseCompatibleStateImageBehavior = false;
            this.currencyListView.View = System.Windows.Forms.View.Details;
            // 
            // codeHeader
            // 
            this.codeHeader.Text = "||Code";
            // 
            // descriptionHeader
            // 
            this.descriptionHeader.Text = "||Description";
            // 
            // scaleHeader
            // 
            this.scaleHeader.Text = "||Scale";
            // 
            // minDepositHeader
            // 
            this.minDepositHeader.Text = "||Min deposit";
            // 
            // maxDepositHeader
            // 
            this.maxDepositHeader.Text = "||Max deposit";
            // 
            // depositInaccuracyHeader
            // 
            this.depositInaccuracyHeader.Text = "||Deposit inaccuracy";
            // 
            // newDepositFeeBtmHeader
            // 
            this.newDepositFeeBtmHeader.Text = "||New deposit fee BTM";
            // 
            // depositFeeConstHeader
            // 
            this.depositFeeConstHeader.DisplayIndex = 12;
            this.depositFeeConstHeader.Text = "||Deposit fee const*";
            // 
            // depositFeePercentHeader
            // 
            this.depositFeePercentHeader.DisplayIndex = 13;
            this.depositFeePercentHeader.Text = "||Deposit fee %*";
            // 
            // minWithdrawHeader
            // 
            this.minWithdrawHeader.DisplayIndex = 7;
            this.minWithdrawHeader.Text = "||Min withdraw";
            // 
            // maxWithdrawHeader
            // 
            this.maxWithdrawHeader.DisplayIndex = 8;
            this.maxWithdrawHeader.Text = "||Max withdraw";
            // 
            // newWithdrawFeeBtmHeader
            // 
            this.newWithdrawFeeBtmHeader.DisplayIndex = 9;
            this.newWithdrawFeeBtmHeader.Text = "||New withdraw fee BTM";
            // 
            // withdrawFeeConstHeader
            // 
            this.withdrawFeeConstHeader.DisplayIndex = 10;
            this.withdrawFeeConstHeader.Text = "||Withdraw fee const*";
            // 
            // withdrawFeePercentHeader
            // 
            this.withdrawFeePercentHeader.DisplayIndex = 11;
            this.withdrawFeePercentHeader.Text = "||Withdraw fee %*";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByContentToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(289, 56);
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
            this.tableLayoutPanel1.Controls.Add(this.currencyListView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(591, 201);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 183);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(305, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "||* Doesn\'t include own payment system fees";
            // 
            // ExchangeCurrencyListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 201);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ExchangeCurrencyListForm";
            this.ShowIcon = false;
            this.Text = "||Currency list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeCurrencyListForm_FormClosing);
            this.Shown += new System.EventHandler(this.ExchangeCurrencyListForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView currencyListView;
        private System.Windows.Forms.ColumnHeader codeHeader;
        private System.Windows.Forms.ColumnHeader descriptionHeader;
        private System.Windows.Forms.ColumnHeader scaleHeader;
        private System.Windows.Forms.ColumnHeader minDepositHeader;
        private System.Windows.Forms.ColumnHeader maxDepositHeader;
        private System.Windows.Forms.ColumnHeader depositInaccuracyHeader;
        private System.Windows.Forms.ColumnHeader newDepositFeeBtmHeader;
        private System.Windows.Forms.ColumnHeader minWithdrawHeader;
        private System.Windows.Forms.ColumnHeader maxWithdrawHeader;
        private System.Windows.Forms.ColumnHeader newWithdrawFeeBtmHeader;
        private System.Windows.Forms.ColumnHeader withdrawFeeConstHeader;
        private System.Windows.Forms.ColumnHeader withdrawFeePercentHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByContentToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader depositFeeConstHeader;
        private System.Windows.Forms.ColumnHeader depositFeePercentHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
    }
}