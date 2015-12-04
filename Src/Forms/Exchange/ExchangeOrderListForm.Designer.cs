namespace BtmI2p.BitMoneyClient.Gui.Forms.Exchange
{
    partial class ExchangeOrderListForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.contextMenu_noSelectedOrders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.columnAutowidthByHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnAutowidthByContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu_selectedOrder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cancelOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newOrderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.contextMenu_noSelectedOrders.SuspendLayout();
            this.contextMenu_selectedOrder.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(674, 214);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader11,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader10});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(3, 41);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(668, 170);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseUp);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "||Date";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "||Security code";
            this.columnHeader1.Width = 119;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "||Side";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "||Price";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "||Status";
            this.columnHeader4.Width = 70;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "||Qty";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "||Filled qty";
            this.columnHeader6.Width = 79;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "||Total volume";
            this.columnHeader11.Width = 112;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "||Base account";
            this.columnHeader7.Width = 117;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "||Second account";
            this.columnHeader8.Width = 145;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "||Order GUID";
            this.columnHeader10.Width = 112;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.checkBox4, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox5, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBox1, 6, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(668, 32);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Location = new System.Drawing.Point(297, 3);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(65, 22);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "||Buy";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(88, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(105, 22);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "||Cancelled";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(199, 3);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(92, 22);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "||Fullfilled";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Checked = true;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.Location = new System.Drawing.Point(368, 3);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(64, 22);
            this.checkBox5.TabIndex = 4;
            this.checkBox5.Text = "||Sell";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
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
            // comboBox1
            // 
            this.comboBox1.DisplayMember = "100";
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "10",
            "100",
            "1000"});
            this.comboBox1.Location = new System.Drawing.Point(544, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.ValueMember = "100";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // contextMenu_noSelectedOrders
            // 
            this.contextMenu_noSelectedOrders.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_noSelectedOrders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.columnAutowidthByHeaderToolStripMenuItem,
            this.columnAutowidthByContentToolStripMenuItem});
            this.contextMenu_noSelectedOrders.Name = "contextMenu_noSelectedOrders";
            this.contextMenu_noSelectedOrders.Size = new System.Drawing.Size(289, 56);
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
            // contextMenu_selectedOrder
            // 
            this.contextMenu_selectedOrder.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu_selectedOrder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelOrderToolStripMenuItem,
            this.newOrderToolStripMenuItem1});
            this.contextMenu_selectedOrder.Name = "contextMenu_selectedOrder";
            this.contextMenu_selectedOrder.Size = new System.Drawing.Size(177, 56);
            // 
            // cancelOrderToolStripMenuItem
            // 
            this.cancelOrderToolStripMenuItem.Name = "cancelOrderToolStripMenuItem";
            this.cancelOrderToolStripMenuItem.Size = new System.Drawing.Size(176, 26);
            this.cancelOrderToolStripMenuItem.Text = "||Cancel order";
            this.cancelOrderToolStripMenuItem.Click += new System.EventHandler(this.cancelOrderToolStripMenuItem_Click);
            // 
            // newOrderToolStripMenuItem1
            // 
            this.newOrderToolStripMenuItem1.Name = "newOrderToolStripMenuItem1";
            this.newOrderToolStripMenuItem1.Size = new System.Drawing.Size(176, 26);
            this.newOrderToolStripMenuItem1.Text = "||New order";
            this.newOrderToolStripMenuItem1.Click += new System.EventHandler(this.newOrderToolStripMenuItem1_Click);
            // 
            // ExchangeOrderListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 214);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExchangeOrderListForm";
            this.ShowIcon = false;
            this.Text = "||Order list";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExchangeOrderListForm_FormClosing);
            this.Shown += new System.EventHandler(this.ExchangeOrderListForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.contextMenu_noSelectedOrders.ResumeLayout(false);
            this.contextMenu_selectedOrder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ContextMenuStrip contextMenu_noSelectedOrders;
		private System.Windows.Forms.ContextMenuStrip contextMenu_selectedOrder;
		private System.Windows.Forms.ToolStripMenuItem cancelOrderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newOrderToolStripMenuItem1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem columnAutowidthByContentToolStripMenuItem;
    }
}