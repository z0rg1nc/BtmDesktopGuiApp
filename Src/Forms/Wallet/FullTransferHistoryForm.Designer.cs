namespace BtmI2p.BitMoneyClient.Gui.Forms.Wallet
{
    partial class FullTransferHistoryForm
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
            this.baseWalletGuidLabel = new System.Windows.Forms.Label();
            this.baseWalletGuidTextBox = new System.Windows.Forms.TextBox();
            this.fromDtPickerDay = new System.Windows.Forms.DateTimePicker();
            this.untilDtPickerDay = new System.Windows.Forms.DateTimePicker();
            this.getDataButton = new System.Windows.Forms.Button();
            this.sentCheckBox = new System.Windows.Forms.CheckBox();
            this.receivedCheckBox = new System.Windows.Forms.CheckBox();
            this.untilDtPickerTimeOfDay = new System.Windows.Forms.DateTimePicker();
            this.transferListView = new System.Windows.Forms.ListView();
            this.statusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.amountHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.feeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.walletFromHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.walletToHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.commentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sentTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.authenticationsHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_ShowTransferInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showTranferInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyWalletToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyWalletFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCommentStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repeatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transfersLabel = new System.Windows.Forms.Label();
            this.fromDtPickerTimeOfDay = new System.Windows.Forms.DateTimePicker();
            this.updateFiterButton = new System.Windows.Forms.Button();
            this.anonymousFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.commentFilterTextBox = new System.Windows.Forms.TextBox();
            this.commentFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.amountConditionComboBox = new System.Windows.Forms.ComboBox();
            this.amountFilterTextBox = new System.Windows.Forms.TextBox();
            this.amountCheckBox = new System.Windows.Forms.CheckBox();
            this.walletToTextBox = new System.Windows.Forms.TextBox();
            this.walletToCheckBox = new System.Windows.Forms.CheckBox();
            this.walletFromTextBox = new System.Windows.Forms.TextBox();
            this.walletFromCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.untilDbLabel = new System.Windows.Forms.Label();
            this.fromDtLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.csvExportButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.dataLabel = new System.Windows.Forms.Label();
            this.filtersLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip_ShowTransferInfo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // baseWalletGuidLabel
            // 
            this.baseWalletGuidLabel.AutoSize = true;
            this.baseWalletGuidLabel.Location = new System.Drawing.Point(3, 0);
            this.baseWalletGuidLabel.Name = "baseWalletGuidLabel";
            this.baseWalletGuidLabel.Size = new System.Drawing.Size(104, 18);
            this.baseWalletGuidLabel.TabIndex = 0;
            this.baseWalletGuidLabel.Text = "||Wallet GUID:";
            // 
            // baseWalletGuidTextBox
            // 
            this.baseWalletGuidTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.baseWalletGuidTextBox.Location = new System.Drawing.Point(113, 3);
            this.baseWalletGuidTextBox.Name = "baseWalletGuidTextBox";
            this.baseWalletGuidTextBox.ReadOnly = true;
            this.baseWalletGuidTextBox.Size = new System.Drawing.Size(823, 24);
            this.baseWalletGuidTextBox.TabIndex = 1;
            // 
            // fromDtPickerDay
            // 
            this.fromDtPickerDay.Dock = System.Windows.Forms.DockStyle.Top;
            this.fromDtPickerDay.Location = new System.Drawing.Point(3, 3);
            this.fromDtPickerDay.Name = "fromDtPickerDay";
            this.fromDtPickerDay.Size = new System.Drawing.Size(175, 24);
            this.fromDtPickerDay.TabIndex = 2;
            // 
            // untilDtPickerDay
            // 
            this.untilDtPickerDay.Dock = System.Windows.Forms.DockStyle.Top;
            this.untilDtPickerDay.Location = new System.Drawing.Point(3, 3);
            this.untilDtPickerDay.Name = "untilDtPickerDay";
            this.untilDtPickerDay.Size = new System.Drawing.Size(175, 24);
            this.untilDtPickerDay.TabIndex = 4;
            // 
            // getDataButton
            // 
            this.getDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.getDataButton.AutoSize = true;
            this.getDataButton.Location = new System.Drawing.Point(265, 145);
            this.getDataButton.Name = "getDataButton";
            this.getDataButton.Size = new System.Drawing.Size(101, 30);
            this.getDataButton.TabIndex = 8;
            this.getDataButton.Text = "||Get data";
            this.getDataButton.UseVisualStyleBackColor = true;
            this.getDataButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // sentCheckBox
            // 
            this.sentCheckBox.AutoSize = true;
            this.sentCheckBox.Checked = true;
            this.sentCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sentCheckBox.Location = new System.Drawing.Point(3, 3);
            this.sentCheckBox.Name = "sentCheckBox";
            this.sentCheckBox.Size = new System.Drawing.Size(70, 22);
            this.sentCheckBox.TabIndex = 9;
            this.sentCheckBox.Text = "||Sent";
            this.sentCheckBox.UseVisualStyleBackColor = true;
            // 
            // receivedCheckBox
            // 
            this.receivedCheckBox.AutoSize = true;
            this.receivedCheckBox.Checked = true;
            this.receivedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.receivedCheckBox.Location = new System.Drawing.Point(79, 3);
            this.receivedCheckBox.Name = "receivedCheckBox";
            this.receivedCheckBox.Size = new System.Drawing.Size(101, 22);
            this.receivedCheckBox.TabIndex = 10;
            this.receivedCheckBox.Text = "||Received";
            this.receivedCheckBox.UseVisualStyleBackColor = true;
            // 
            // untilDtPickerTimeOfDay
            // 
            this.untilDtPickerTimeOfDay.Dock = System.Windows.Forms.DockStyle.Top;
            this.untilDtPickerTimeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.untilDtPickerTimeOfDay.Location = new System.Drawing.Point(184, 3);
            this.untilDtPickerTimeOfDay.Name = "untilDtPickerTimeOfDay";
            this.untilDtPickerTimeOfDay.Size = new System.Drawing.Size(176, 24);
            this.untilDtPickerTimeOfDay.TabIndex = 12;
            this.untilDtPickerTimeOfDay.Value = new System.DateTime(2014, 10, 30, 0, 0, 0, 0);
            // 
            // transferListView
            // 
            this.transferListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.statusHeader,
            this.amountHeader,
            this.feeHeader,
            this.walletFromHeader,
            this.walletToHeader,
            this.commentHeader,
            this.sentTimeHeader,
            this.authenticationsHeader});
            this.transferListView.ContextMenuStrip = this.contextMenuStrip_ShowTransferInfo;
            this.transferListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transferListView.FullRowSelect = true;
            this.transferListView.Location = new System.Drawing.Point(3, 299);
            this.transferListView.Name = "transferListView";
            this.transferListView.Size = new System.Drawing.Size(939, 161);
            this.transferListView.TabIndex = 13;
            this.transferListView.UseCompatibleStateImageBehavior = false;
            this.transferListView.View = System.Windows.Forms.View.Details;
            this.transferListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView4_ColumnClick);
            this.transferListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView4_MouseClick);
            this.transferListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView4_MouseDoubleClick);
            // 
            // statusHeader
            // 
            this.statusHeader.Text = "||Status";
            this.statusHeader.Width = 105;
            // 
            // amountHeader
            // 
            this.amountHeader.Text = "||Amount";
            this.amountHeader.Width = 122;
            // 
            // feeHeader
            // 
            this.feeHeader.Text = "||Fee";
            // 
            // walletFromHeader
            // 
            this.walletFromHeader.Text = "||From";
            this.walletFromHeader.Width = 128;
            // 
            // walletToHeader
            // 
            this.walletToHeader.Text = "||To";
            this.walletToHeader.Width = 129;
            // 
            // commentHeader
            // 
            this.commentHeader.Text = "||Comment";
            this.commentHeader.Width = 216;
            // 
            // sentTimeHeader
            // 
            this.sentTimeHeader.Text = "||Time";
            this.sentTimeHeader.Width = 95;
            // 
            // authenticationsHeader
            // 
            this.authenticationsHeader.Text = "||Authentications";
            this.authenticationsHeader.Width = 76;
            // 
            // contextMenuStrip_ShowTransferInfo
            // 
            this.contextMenuStrip_ShowTransferInfo.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_ShowTransferInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTranferInfoToolStripMenuItem,
            this.copyWalletToToolStripMenuItem,
            this.copyWalletFromToolStripMenuItem,
            this.copyCommentStringToolStripMenuItem,
            this.repeatToolStripMenuItem,
            this.toolStripSeparator1,
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByContentToolStripMenuItem});
            this.contextMenuStrip_ShowTransferInfo.Name = "contextMenuStrip_ShowTransferInfo";
            this.contextMenuStrip_ShowTransferInfo.Size = new System.Drawing.Size(289, 192);
            // 
            // showTranferInfoToolStripMenuItem
            // 
            this.showTranferInfoToolStripMenuItem.Name = "showTranferInfoToolStripMenuItem";
            this.showTranferInfoToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.showTranferInfoToolStripMenuItem.Text = "||Show full transfer info";
            this.showTranferInfoToolStripMenuItem.Click += new System.EventHandler(this.showTranferInfoToolStripMenuItem_Click);
            // 
            // copyWalletToToolStripMenuItem
            // 
            this.copyWalletToToolStripMenuItem.Name = "copyWalletToToolStripMenuItem";
            this.copyWalletToToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.copyWalletToToolStripMenuItem.Text = "||Copy wallet to";
            this.copyWalletToToolStripMenuItem.Click += new System.EventHandler(this.copyWalletToToolStripMenuItem_Click);
            // 
            // copyWalletFromToolStripMenuItem
            // 
            this.copyWalletFromToolStripMenuItem.Name = "copyWalletFromToolStripMenuItem";
            this.copyWalletFromToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.copyWalletFromToolStripMenuItem.Text = "||Copy wallet from";
            this.copyWalletFromToolStripMenuItem.Click += new System.EventHandler(this.copyWalletFromToolStripMenuItem_Click);
            // 
            // copyCommentStringToolStripMenuItem
            // 
            this.copyCommentStringToolStripMenuItem.Name = "copyCommentStringToolStripMenuItem";
            this.copyCommentStringToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.copyCommentStringToolStripMenuItem.Text = "||Copy comment string";
            this.copyCommentStringToolStripMenuItem.Click += new System.EventHandler(this.copyCommentStringToolStripMenuItem_Click);
            // 
            // repeatToolStripMenuItem
            // 
            this.repeatToolStripMenuItem.Name = "repeatToolStripMenuItem";
            this.repeatToolStripMenuItem.Size = new System.Drawing.Size(288, 26);
            this.repeatToolStripMenuItem.Text = "||Repeat";
            this.repeatToolStripMenuItem.Click += new System.EventHandler(this.repeatToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(285, 6);
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
            // transfersLabel
            // 
            this.transfersLabel.AutoSize = true;
            this.transfersLabel.Location = new System.Drawing.Point(3, 0);
            this.transfersLabel.Name = "transfersLabel";
            this.transfersLabel.Size = new System.Drawing.Size(81, 18);
            this.transfersLabel.TabIndex = 12;
            this.transfersLabel.Text = "||Transfers";
            // 
            // fromDtPickerTimeOfDay
            // 
            this.fromDtPickerTimeOfDay.Dock = System.Windows.Forms.DockStyle.Top;
            this.fromDtPickerTimeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.fromDtPickerTimeOfDay.Location = new System.Drawing.Point(184, 3);
            this.fromDtPickerTimeOfDay.Name = "fromDtPickerTimeOfDay";
            this.fromDtPickerTimeOfDay.Size = new System.Drawing.Size(176, 24);
            this.fromDtPickerTimeOfDay.TabIndex = 11;
            this.fromDtPickerTimeOfDay.Value = new System.DateTime(2014, 10, 30, 0, 0, 0, 0);
            // 
            // updateFiterButton
            // 
            this.updateFiterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateFiterButton.AutoSize = true;
            this.updateFiterButton.Location = new System.Drawing.Point(469, 159);
            this.updateFiterButton.Name = "updateFiterButton";
            this.updateFiterButton.Size = new System.Drawing.Size(86, 28);
            this.updateFiterButton.TabIndex = 11;
            this.updateFiterButton.Text = "||Update";
            this.updateFiterButton.UseVisualStyleBackColor = true;
            this.updateFiterButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // anonymousFilterCheckBox
            // 
            this.anonymousFilterCheckBox.AutoSize = true;
            this.anonymousFilterCheckBox.Location = new System.Drawing.Point(3, 131);
            this.anonymousFilterCheckBox.Name = "anonymousFilterCheckBox";
            this.anonymousFilterCheckBox.Size = new System.Drawing.Size(150, 22);
            this.anonymousFilterCheckBox.TabIndex = 10;
            this.anonymousFilterCheckBox.Text = "||Anonymous only";
            this.anonymousFilterCheckBox.UseVisualStyleBackColor = true;
            // 
            // commentFilterTextBox
            // 
            this.commentFilterTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commentFilterTextBox.Location = new System.Drawing.Point(175, 101);
            this.commentFilterTextBox.Name = "commentFilterTextBox";
            this.commentFilterTextBox.Size = new System.Drawing.Size(380, 24);
            this.commentFilterTextBox.TabIndex = 8;
            // 
            // commentFilterCheckBox
            // 
            this.commentFilterCheckBox.AutoSize = true;
            this.commentFilterCheckBox.Location = new System.Drawing.Point(3, 101);
            this.commentFilterCheckBox.Name = "commentFilterCheckBox";
            this.commentFilterCheckBox.Size = new System.Drawing.Size(166, 22);
            this.commentFilterCheckBox.TabIndex = 7;
            this.commentFilterCheckBox.Text = "||Comment contains";
            this.commentFilterCheckBox.UseVisualStyleBackColor = true;
            // 
            // amountConditionComboBox
            // 
            this.amountConditionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.amountConditionComboBox.FormattingEnabled = true;
            this.amountConditionComboBox.Items.AddRange(new object[] {
            "<",
            "<=",
            "==",
            ">=",
            ">"});
            this.amountConditionComboBox.Location = new System.Drawing.Point(3, 3);
            this.amountConditionComboBox.Name = "amountConditionComboBox";
            this.amountConditionComboBox.Size = new System.Drawing.Size(58, 26);
            this.amountConditionComboBox.TabIndex = 6;
            // 
            // amountFilterTextBox
            // 
            this.amountFilterTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.amountFilterTextBox.Location = new System.Drawing.Point(67, 3);
            this.amountFilterTextBox.Name = "amountFilterTextBox";
            this.amountFilterTextBox.Size = new System.Drawing.Size(310, 24);
            this.amountFilterTextBox.TabIndex = 5;
            // 
            // amountCheckBox
            // 
            this.amountCheckBox.AutoSize = true;
            this.amountCheckBox.Location = new System.Drawing.Point(3, 63);
            this.amountCheckBox.Name = "amountCheckBox";
            this.amountCheckBox.Size = new System.Drawing.Size(91, 22);
            this.amountCheckBox.TabIndex = 4;
            this.amountCheckBox.Text = "||Amount";
            this.amountCheckBox.UseVisualStyleBackColor = true;
            // 
            // walletToTextBox
            // 
            this.walletToTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.walletToTextBox.Location = new System.Drawing.Point(175, 33);
            this.walletToTextBox.Name = "walletToTextBox";
            this.walletToTextBox.Size = new System.Drawing.Size(380, 24);
            this.walletToTextBox.TabIndex = 3;
            // 
            // walletToCheckBox
            // 
            this.walletToCheckBox.AutoSize = true;
            this.walletToCheckBox.Location = new System.Drawing.Point(3, 33);
            this.walletToCheckBox.Name = "walletToCheckBox";
            this.walletToCheckBox.Size = new System.Drawing.Size(98, 22);
            this.walletToCheckBox.TabIndex = 2;
            this.walletToCheckBox.Text = "||Wallet to";
            this.walletToCheckBox.UseVisualStyleBackColor = true;
            // 
            // walletFromTextBox
            // 
            this.walletFromTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.walletFromTextBox.Location = new System.Drawing.Point(175, 3);
            this.walletFromTextBox.Name = "walletFromTextBox";
            this.walletFromTextBox.Size = new System.Drawing.Size(380, 24);
            this.walletFromTextBox.TabIndex = 1;
            // 
            // walletFromCheckBox
            // 
            this.walletFromCheckBox.AutoSize = true;
            this.walletFromCheckBox.Location = new System.Drawing.Point(3, 3);
            this.walletFromCheckBox.Name = "walletFromCheckBox";
            this.walletFromCheckBox.Size = new System.Drawing.Size(116, 22);
            this.walletFromCheckBox.TabIndex = 0;
            this.walletFromCheckBox.Text = "||Wallet from";
            this.walletFromCheckBox.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.walletFromCheckBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.commentFilterTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.anonymousFilterCheckBox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.walletToCheckBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.amountCheckBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.walletToTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.commentFilterCheckBox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.walletFromTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.updateFiterButton, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(378, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 190);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.amountFilterTextBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.amountConditionComboBox, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(175, 63);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(380, 32);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // untilDbLabel
            // 
            this.untilDbLabel.AutoSize = true;
            this.untilDbLabel.Location = new System.Drawing.Point(3, 54);
            this.untilDbLabel.Name = "untilDbLabel";
            this.untilDbLabel.Size = new System.Drawing.Size(47, 18);
            this.untilDbLabel.TabIndex = 5;
            this.untilDbLabel.Text = "||Until";
            // 
            // fromDtLabel
            // 
            this.fromDtLabel.AutoSize = true;
            this.fromDtLabel.Location = new System.Drawing.Point(3, 0);
            this.fromDtLabel.Name = "fromDtLabel";
            this.fromDtLabel.Size = new System.Drawing.Size(195, 18);
            this.fromDtLabel.TabIndex = 3;
            this.fromDtLabel.Text = "||From (UTC time, not local)";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.getDataButton, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.fromDtLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.untilDbLabel, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(369, 178);
            this.tableLayoutPanel3.TabIndex = 16;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.untilDtPickerTimeOfDay, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.untilDtPickerDay, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 75);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(363, 30);
            this.tableLayoutPanel5.TabIndex = 17;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.fromDtPickerTimeOfDay, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.fromDtPickerDay, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(363, 30);
            this.tableLayoutPanel4.TabIndex = 17;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.sentCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.receivedCheckBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 111);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(363, 28);
            this.flowLayoutPanel1.TabIndex = 18;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel9, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.transferListView, 0, 3);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 4;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(945, 463);
            this.tableLayoutPanel6.TabIndex = 17;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.AutoSize = true;
            this.tableLayoutPanel9.ColumnCount = 3;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel9.Controls.Add(this.csvExportButton, 2, 0);
            this.tableLayoutPanel9.Controls.Add(this.transfersLabel, 0, 0);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(3, 259);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(939, 34);
            this.tableLayoutPanel9.TabIndex = 19;
            // 
            // csvExportButton
            // 
            this.csvExportButton.AutoSize = true;
            this.csvExportButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.csvExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.csvExportButton.Location = new System.Drawing.Point(814, 3);
            this.csvExportButton.Name = "csvExportButton";
            this.csvExportButton.Size = new System.Drawing.Size(122, 28);
            this.csvExportButton.TabIndex = 0;
            this.csvExportButton.Text = "||Export to CSV";
            this.csvExportButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.csvExportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.csvExportButton.UseVisualStyleBackColor = true;
            this.csvExportButton.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel8.Controls.Add(this.dataLabel, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.filtersLabel, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel1, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 39);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(939, 214);
            this.tableLayoutPanel8.TabIndex = 18;
            // 
            // dataLabel
            // 
            this.dataLabel.AutoSize = true;
            this.dataLabel.Location = new System.Drawing.Point(3, 0);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(49, 18);
            this.dataLabel.TabIndex = 0;
            this.dataLabel.Text = "||Data";
            // 
            // filtersLabel
            // 
            this.filtersLabel.AutoSize = true;
            this.filtersLabel.Location = new System.Drawing.Point(378, 0);
            this.filtersLabel.Name = "filtersLabel";
            this.filtersLabel.Size = new System.Drawing.Size(58, 18);
            this.filtersLabel.TabIndex = 1;
            this.filtersLabel.Text = "||Filters";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.AutoSize = true;
            this.tableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.baseWalletGuidLabel, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.baseWalletGuidTextBox, 1, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(939, 30);
            this.tableLayoutPanel7.TabIndex = 18;
            // 
            // FullTransferHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 463);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MinimizeBox = false;
            this.Name = "FullTransferHistoryForm";
            this.ShowIcon = false;
            this.Text = "||Transfers history";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FullTransferHistoryForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FullTransferHistoryForm_FormClosed);
            this.Shown += new System.EventHandler(this.FullTransferHistoryForm_Shown);
            this.contextMenuStrip_ShowTransferInfo.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label baseWalletGuidLabel;
        private System.Windows.Forms.TextBox baseWalletGuidTextBox;
        private System.Windows.Forms.DateTimePicker fromDtPickerDay;
        private System.Windows.Forms.DateTimePicker untilDtPickerDay;
        private System.Windows.Forms.Button getDataButton;
        private System.Windows.Forms.CheckBox sentCheckBox;
        private System.Windows.Forms.CheckBox receivedCheckBox;
        private System.Windows.Forms.ListView transferListView;
        private System.Windows.Forms.ColumnHeader statusHeader;
        private System.Windows.Forms.ColumnHeader amountHeader;
        private System.Windows.Forms.ColumnHeader walletFromHeader;
        private System.Windows.Forms.ColumnHeader walletToHeader;
        private System.Windows.Forms.ColumnHeader commentHeader;
        private System.Windows.Forms.ColumnHeader sentTimeHeader;
        private System.Windows.Forms.Label transfersLabel;
        private System.Windows.Forms.DateTimePicker untilDtPickerTimeOfDay;
        private System.Windows.Forms.DateTimePicker fromDtPickerTimeOfDay;
        private System.Windows.Forms.CheckBox walletFromCheckBox;
        private System.Windows.Forms.CheckBox walletToCheckBox;
        private System.Windows.Forms.TextBox walletFromTextBox;
        private System.Windows.Forms.TextBox walletToTextBox;
        private System.Windows.Forms.ComboBox amountConditionComboBox;
        private System.Windows.Forms.TextBox amountFilterTextBox;
        private System.Windows.Forms.CheckBox amountCheckBox;
        private System.Windows.Forms.CheckBox commentFilterCheckBox;
        private System.Windows.Forms.TextBox commentFilterTextBox;
        private System.Windows.Forms.CheckBox anonymousFilterCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ShowTransferInfo;
        private System.Windows.Forms.ToolStripMenuItem showTranferInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyWalletToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyWalletFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCommentStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repeatToolStripMenuItem;
        private System.Windows.Forms.Button updateFiterButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label untilDbLabel;
        private System.Windows.Forms.Label fromDtLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.Label filtersLabel;
        private System.Windows.Forms.ColumnHeader feeHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.Button csvExportButton;
        private System.Windows.Forms.ColumnHeader authenticationsHeader;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByContentToolStripMenuItem;
    }
}