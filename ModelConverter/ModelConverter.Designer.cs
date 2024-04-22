
namespace MainForm {
	partial class ModelConverter {
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.FilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Height = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Depth = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AxisO = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.AxisD = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.btnClear = new System.Windows.Forms.Button();
			this.cmbType = new System.Windows.Forms.ComboBox();
			this.btnConvertAll = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chkSizeScale = new System.Windows.Forms.CheckBox();
			this.chkSizeHeight = new System.Windows.Forms.CheckBox();
			this.chkSizeDepth = new System.Windows.Forms.CheckBox();
			this.chkSizeMax = new System.Windows.Forms.CheckBox();
			this.chkSizeWidth = new System.Windows.Forms.CheckBox();
			this.numSize = new System.Windows.Forms.NumericUpDown();
			this.chkResize = new System.Windows.Forms.CheckBox();
			this.lblMessage = new System.Windows.Forms.Label();
			this.btnConvert = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowDrop = true;
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FilePath,
            this.FileName,
            this.Type,
            this.Width,
            this.Height,
            this.Depth,
            this.AxisO,
            this.AxisD});
			this.dataGridView1.Location = new System.Drawing.Point(12, 104);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(669, 218);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
			this.dataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
			this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
			this.dataGridView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
			// 
			// FilePath
			// 
			this.FilePath.HeaderText = "ファイルパス";
			this.FilePath.Name = "FilePath";
			this.FilePath.ReadOnly = true;
			this.FilePath.Visible = false;
			// 
			// FileName
			// 
			dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FileName.DefaultCellStyle = dataGridViewCellStyle1;
			this.FileName.HeaderText = "ファイル名";
			this.FileName.Name = "FileName";
			this.FileName.ReadOnly = true;
			this.FileName.Width = 200;
			// 
			// Type
			// 
			dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Type.DefaultCellStyle = dataGridViewCellStyle2;
			this.Type.HeaderText = "拡張子";
			this.Type.Name = "Type";
			this.Type.ReadOnly = true;
			this.Type.Width = 65;
			// 
			// Width
			// 
			dataGridViewCellStyle3.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle3.Format = "N3";
			dataGridViewCellStyle3.NullValue = null;
			this.Width.DefaultCellStyle = dataGridViewCellStyle3;
			this.Width.HeaderText = "幅";
			this.Width.Name = "Width";
			this.Width.ReadOnly = true;
			this.Width.Width = 70;
			// 
			// Height
			// 
			dataGridViewCellStyle4.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle4.Format = "N3";
			dataGridViewCellStyle4.NullValue = null;
			this.Height.DefaultCellStyle = dataGridViewCellStyle4;
			this.Height.HeaderText = "高さ";
			this.Height.Name = "Height";
			this.Height.ReadOnly = true;
			this.Height.Width = 70;
			// 
			// Depth
			// 
			dataGridViewCellStyle5.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle5.Format = "N3";
			dataGridViewCellStyle5.NullValue = null;
			this.Depth.DefaultCellStyle = dataGridViewCellStyle5;
			this.Depth.HeaderText = "奥行き";
			this.Depth.Name = "Depth";
			this.Depth.ReadOnly = true;
			this.Depth.Width = 70;
			// 
			// AxisO
			// 
			dataGridViewCellStyle6.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.AxisO.DefaultCellStyle = dataGridViewCellStyle6;
			this.AxisO.HeaderText = "幅,高,奥";
			this.AxisO.Items.AddRange(new object[] {
            "X,Y,Z",
            "X,Z,Y",
            "Y,X,Z",
            "Y,Z,X",
            "Z,X,Y",
            "Z,Y,X"});
			this.AxisO.Name = "AxisO";
			this.AxisO.Width = 70;
			// 
			// AxisD
			// 
			dataGridViewCellStyle7.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.AxisD.DefaultCellStyle = dataGridViewCellStyle7;
			this.AxisD.HeaderText = "軸反転";
			this.AxisD.Items.AddRange(new object[] {
            "+X,+Y,+Z",
            "+X,+Y,-Z",
            "+X,-Y,+Z",
            "+X,-Y,-Z",
            "-X,+Y,+Z",
            "-X,+Y,-Z",
            "-X,-Y,+Z",
            "-X,-Y,-Z"});
			this.AxisD.Name = "AxisD";
			this.AxisD.Width = 80;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(146, 65);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(45, 33);
			this.btnClear.TabIndex = 1;
			this.btnClear.Text = "クリア";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// cmbType
			// 
			this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbType.FormattingEnabled = true;
			this.cmbType.Location = new System.Drawing.Point(6, 18);
			this.cmbType.Name = "cmbType";
			this.cmbType.Size = new System.Drawing.Size(167, 20);
			this.cmbType.TabIndex = 2;
			// 
			// btnConvertAll
			// 
			this.btnConvertAll.Location = new System.Drawing.Point(12, 65);
			this.btnConvertAll.Name = "btnConvertAll";
			this.btnConvertAll.Size = new System.Drawing.Size(61, 33);
			this.btnConvertAll.TabIndex = 3;
			this.btnConvertAll.Text = "全変換";
			this.btnConvertAll.UseVisualStyleBackColor = true;
			this.btnConvertAll.Click += new System.EventHandler(this.btnConvertAll_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cmbType);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(179, 47);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "変換後の形式";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.chkSizeScale);
			this.groupBox2.Controls.Add(this.chkSizeHeight);
			this.groupBox2.Controls.Add(this.chkSizeDepth);
			this.groupBox2.Controls.Add(this.chkSizeMax);
			this.groupBox2.Controls.Add(this.chkSizeWidth);
			this.groupBox2.Controls.Add(this.numSize);
			this.groupBox2.Controls.Add(this.chkResize);
			this.groupBox2.Location = new System.Drawing.Point(197, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(199, 86);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "サイズ変更";
			// 
			// chkSizeScale
			// 
			this.chkSizeScale.AutoSize = true;
			this.chkSizeScale.Checked = true;
			this.chkSizeScale.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSizeScale.Enabled = false;
			this.chkSizeScale.Location = new System.Drawing.Point(111, 42);
			this.chkSizeScale.Name = "chkSizeScale";
			this.chkSizeScale.Size = new System.Drawing.Size(77, 16);
			this.chkSizeScale.TabIndex = 10;
			this.chkSizeScale.Text = "スケーリング";
			this.chkSizeScale.UseVisualStyleBackColor = true;
			this.chkSizeScale.CheckedChanged += new System.EventHandler(this.chkSizeScale_CheckedChanged);
			// 
			// chkSizeHeight
			// 
			this.chkSizeHeight.AutoSize = true;
			this.chkSizeHeight.Enabled = false;
			this.chkSizeHeight.Location = new System.Drawing.Point(111, 64);
			this.chkSizeHeight.Name = "chkSizeHeight";
			this.chkSizeHeight.Size = new System.Drawing.Size(44, 16);
			this.chkSizeHeight.TabIndex = 8;
			this.chkSizeHeight.Text = "高さ";
			this.chkSizeHeight.UseVisualStyleBackColor = true;
			this.chkSizeHeight.CheckedChanged += new System.EventHandler(this.chkSizeHeight_CheckedChanged);
			// 
			// chkSizeDepth
			// 
			this.chkSizeDepth.AutoSize = true;
			this.chkSizeDepth.Enabled = false;
			this.chkSizeDepth.Location = new System.Drawing.Point(48, 64);
			this.chkSizeDepth.Name = "chkSizeDepth";
			this.chkSizeDepth.Size = new System.Drawing.Size(57, 16);
			this.chkSizeDepth.TabIndex = 7;
			this.chkSizeDepth.Text = "奥行き";
			this.chkSizeDepth.UseVisualStyleBackColor = true;
			this.chkSizeDepth.CheckedChanged += new System.EventHandler(this.chkSizeDepth_CheckedChanged);
			// 
			// chkSizeMax
			// 
			this.chkSizeMax.AutoSize = true;
			this.chkSizeMax.Enabled = false;
			this.chkSizeMax.Location = new System.Drawing.Point(6, 42);
			this.chkSizeMax.Name = "chkSizeMax";
			this.chkSizeMax.Size = new System.Drawing.Size(84, 16);
			this.chkSizeMax.TabIndex = 9;
			this.chkSizeMax.Text = "最大値基準";
			this.chkSizeMax.UseVisualStyleBackColor = true;
			this.chkSizeMax.CheckedChanged += new System.EventHandler(this.chkSizeMax_CheckedChanged);
			// 
			// chkSizeWidth
			// 
			this.chkSizeWidth.AutoSize = true;
			this.chkSizeWidth.Enabled = false;
			this.chkSizeWidth.Location = new System.Drawing.Point(6, 64);
			this.chkSizeWidth.Name = "chkSizeWidth";
			this.chkSizeWidth.Size = new System.Drawing.Size(36, 16);
			this.chkSizeWidth.TabIndex = 6;
			this.chkSizeWidth.Text = "幅";
			this.chkSizeWidth.UseVisualStyleBackColor = true;
			this.chkSizeWidth.CheckedChanged += new System.EventHandler(this.chkSizeWidth_CheckedChanged);
			// 
			// numSize
			// 
			this.numSize.DecimalPlaces = 3;
			this.numSize.Enabled = false;
			this.numSize.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.numSize.Location = new System.Drawing.Point(95, 14);
			this.numSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.numSize.Name = "numSize";
			this.numSize.Size = new System.Drawing.Size(98, 22);
			this.numSize.TabIndex = 1;
			this.numSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// chkResize
			// 
			this.chkResize.AutoSize = true;
			this.chkResize.Location = new System.Drawing.Point(6, 20);
			this.chkResize.Name = "chkResize";
			this.chkResize.Size = new System.Drawing.Size(76, 16);
			this.chkResize.TabIndex = 0;
			this.chkResize.Text = "有効にする";
			this.chkResize.UseVisualStyleBackColor = true;
			this.chkResize.CheckedChanged += new System.EventHandler(this.chkResize_CheckedChanged);
			// 
			// lblMessage
			// 
			this.lblMessage.AllowDrop = true;
			this.lblMessage.AutoSize = true;
			this.lblMessage.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.lblMessage.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.lblMessage.ForeColor = System.Drawing.Color.Blue;
			this.lblMessage.Location = new System.Drawing.Point(205, 212);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(276, 19);
			this.lblMessage.TabIndex = 6;
			this.lblMessage.Text = "ここにファイルをドラッグアンドドロップしてください";
			this.lblMessage.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
			this.lblMessage.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
			// 
			// btnConvert
			// 
			this.btnConvert.Location = new System.Drawing.Point(79, 65);
			this.btnConvert.Name = "btnConvert";
			this.btnConvert.Size = new System.Drawing.Size(61, 33);
			this.btnConvert.TabIndex = 7;
			this.btnConvert.Text = "選択変換";
			this.btnConvert.UseVisualStyleBackColor = true;
			this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
			// 
			// ModelConverter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(693, 334);
			this.Controls.Add(this.btnConvert);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.btnConvertAll);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "ModelConverter";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.ComboBox cmbType;
		private System.Windows.Forms.Button btnConvertAll;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox chkResize;
		private System.Windows.Forms.NumericUpDown numSize;
		private System.Windows.Forms.CheckBox chkSizeDepth;
		private System.Windows.Forms.CheckBox chkSizeWidth;
		private System.Windows.Forms.CheckBox chkSizeHeight;
		private System.Windows.Forms.CheckBox chkSizeMax;
		private System.Windows.Forms.CheckBox chkSizeScale;
		private System.Windows.Forms.DataGridViewTextBoxColumn FileType;
		private System.Windows.Forms.DataGridViewTextBoxColumn SizeWidth;
		private System.Windows.Forms.DataGridViewTextBoxColumn SizeHeight;
		private System.Windows.Forms.DataGridViewTextBoxColumn SizeDepth;
		private System.Windows.Forms.DataGridViewTextBoxColumn FilePath;
		private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn Width;
		private System.Windows.Forms.DataGridViewTextBoxColumn Height;
		private System.Windows.Forms.DataGridViewTextBoxColumn Depth;
		private System.Windows.Forms.DataGridViewComboBoxColumn AxisO;
		private System.Windows.Forms.DataGridViewComboBoxColumn AxisD;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.Button btnConvert;
	}
}

