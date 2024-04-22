using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MainForm {
	public partial class ModelConverter : Form {
		const string STL_BIN = "STLバイナリ(.stl)";
		const string STL_TEXT = "STLテキスト(.stl)";
		const string FBX_BIN = "FBXバイナリ(.fbx)";
		const string FBX_TEXT = "FBXテキスト(.fbx)";
		const string WAVEFRONT_OBJ = "Wavefront OBJ(.obj)";
		const string ISO_STEP = "ISO10303 STEP(.step)";
		const string METASEQUOIA = "Metasequoia(.mqoz)";
		const string COLLADA = "Collada(.dae)";
		const string MMD_PMX = "MMD(.pmx)";

		static readonly List<string> TYPE_LIST = new List<string> {
			STL_BIN,
			STL_TEXT,
			FBX_BIN,
			FBX_TEXT,
			WAVEFRONT_OBJ,
			ISO_STEP,
			METASEQUOIA,
			COLLADA,
			MMD_PMX
		};

		List<BaseModel> mModelList = new List<BaseModel>();

		public ModelConverter() {
			InitializeComponent();
			foreach (var i in TYPE_LIST) {
				cmbType.Items.Add(i);
			}
			cmbType.SelectedIndex = 0;
		}

		private void dataGridView1_DragDrop(object sender, DragEventArgs e) {
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}
			foreach (string path in (string[])e.Data.GetData(DataFormats.FileDrop)) {
				setList(path);
			}
			if (dataGridView1.Rows.Count > 0) {
				lblMessage.Visible = false;
			}
		}

		private void dataGridView1_DragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.All;
		}

		private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
			if (e.Control is DataGridViewComboBoxEditingControl cmb) {
				var dgv = (DataGridView)sender;
				cmb.SelectedIndexChanged += new EventHandler((s2, e2) => {
					var cols = dataGridView1.Rows[dgv.CurrentCell.RowIndex].Cells;
					var model = mModelList[dgv.CurrentCell.RowIndex];
					if ("AxisO" == dgv.CurrentCell.OwningColumn.Name) {
						model.ChangeAxisOrder = (EAxisOrder)cmb.SelectedIndex;
						dispSize(cols, model.GetSize(), model.ChangeAxisOrder);
					}
					if ("AxisD" == dgv.CurrentCell.OwningColumn.Name) {
						model.ChangeAxisDir = (EAxisDir)cmb.SelectedIndex;
					}
				});
			}
		}

		private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
			if (dataGridView1.Rows.Count == 0) {
				lblMessage.Visible = true;
			}
		}

		private void chkResize_CheckedChanged(object sender, EventArgs e) {
			numSize.Enabled = chkResize.Checked;
			chkSizeWidth.Enabled = chkResize.Checked;
			chkSizeDepth.Enabled = chkResize.Checked;
			chkSizeHeight.Enabled = chkResize.Checked;
			chkSizeMax.Enabled = chkResize.Checked;
			chkSizeScale.Enabled = chkResize.Checked;
			chkResize.Text = chkResize.Checked ? "無効にする" : "有効にする";
		}

		private void chkSizeWidth_CheckedChanged(object sender, EventArgs e) {
			if (((CheckBox)sender).Checked) {
				chkSizeMax.Checked = false;
				chkSizeScale.Checked = false;
			}
		}

		private void chkSizeDepth_CheckedChanged(object sender, EventArgs e) {
			if (((CheckBox)sender).Checked) {
				chkSizeMax.Checked = false;
				chkSizeScale.Checked = false;
			}
		}

		private void chkSizeHeight_CheckedChanged(object sender, EventArgs e) {
			if (((CheckBox)sender).Checked) {
				chkSizeMax.Checked = false;
				chkSizeScale.Checked = false;
			}
		}

		private void chkSizeMax_CheckedChanged(object sender, EventArgs e) {
			if (((CheckBox)sender).Checked) {
				chkSizeWidth.Checked = false;
				chkSizeDepth.Checked = false;
				chkSizeHeight.Checked = false;
				chkSizeScale.Checked = false;
			}
		}

		private void chkSizeScale_CheckedChanged(object sender, EventArgs e) {
			if (((CheckBox)sender).Checked) {
				chkSizeWidth.Checked = false;
				chkSizeDepth.Checked = false;
				chkSizeHeight.Checked = false;
				chkSizeMax.Checked = false;
			}
		}

		private void btnConvertAll_Click(object sender, EventArgs e) {
			for (int i = 0; i < dataGridView1.Rows.Count; i++) {
				convert(i);
			}
		}

		private void btnConvert_Click(object sender, EventArgs e) {
			for (int i = 0; i < dataGridView1.Rows.Count; i++) {
				if (dataGridView1.Rows[i].Selected) {
					convert(i);
				}
			}
		}

		private void btnClear_Click(object sender, EventArgs e) {
			dataGridView1.Rows.Clear();
			mModelList.Clear();
			lblMessage.Visible = true;
		}

		void setList(string path) {
			if (!File.Exists(path)) {
				Console.WriteLine("ファイルが存在しません {0}", path);
				return;
			}
			var srcModel = performSourceModel(path);
			if (null == srcModel) {
				return;
			}
			dataGridView1.Rows.Add(
				path,
				Path.GetFileNameWithoutExtension(path),
				Path.GetExtension(path).ToLower()
			);
			var cols = dataGridView1.Rows[dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells;
			var axisO = (DataGridViewComboBoxCell)cols["AxisO"];
			var axisD = (DataGridViewComboBoxCell)cols["AxisD"];
			if (EAxisOrder.None == srcModel.AxisOrder) {
				axisO.Value = axisO.Items[0];
				axisO.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
				axisD.Value = axisD.Items[0];
				axisD.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
			} else {
				axisO.Value = axisO.Items[(int)srcModel.AxisOrder];
				axisO.ReadOnly = true;
				axisD.Value = axisD.Items[(int)srcModel.AxisDir];
				axisD.ReadOnly = true;
			}
			srcModel.ChangeAxisOrder = srcModel.AxisOrder;
			srcModel.ChangeAxisDir = srcModel.AxisDir;
			dispSize(cols, srcModel.GetSize(), srcModel.AxisOrder);
			mModelList.Add(srcModel);
		}

		void dispSize(DataGridViewCellCollection cols, vec3 size, EAxisOrder type) {
			var width = cols["Width"];
			var depth = cols["Depth"];
			var height = cols["Height"];
			switch (type) {
			case EAxisOrder.XZY:
				width.Value = size.x;
				height.Value = size.z;
				depth.Value = size.y;
				break;
			case EAxisOrder.YXZ:
				width.Value = size.y;
				height.Value = size.x;
				depth.Value = size.z;
				break;
			case EAxisOrder.YZX:
				width.Value = size.y;
				height.Value = size.z;
				depth.Value = size.x;
				break;
			case EAxisOrder.ZXY:
				width.Value = size.z;
				height.Value = size.x;
				depth.Value = size.y;
				break;
			case EAxisOrder.ZYX:
				width.Value = size.z;
				height.Value = size.y;
				depth.Value = size.x;
				break;
			default:
				width.Value = size.x;
				height.Value = size.y;
				depth.Value = size.z;
				break;
			}
			width.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
			height.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
			depth.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
		}

		BaseModel performSourceModel(string path) {
			var ext = Path.GetExtension(path).ToLower();
			BaseModel srcModel = null;
			try {
				switch (ext) {
				case ".stl":
					srcModel = new StlText(path);
					if (0 == srcModel.ObjectCount) {
						srcModel = new StlBin(path);
					}
					break;
				case ".fbx":
					break;
				case ".obj":
					srcModel = new WavefrontObj(path);
					break;
				case ".mqo":
				case ".mqoz":
					srcModel = new Metasequoia(path);
					break;
				case ".dae":
					srcModel = new Collada(path);
					break;
				case ".pmx":
					srcModel = new MmdPmx(path);
					break;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
				Console.ReadKey();
			}
			return srcModel;
		}

		void convert(int index) {
			/*** select converting type ***/
			BaseModel dstModel = null;
			var saveExt = "";
			var convertTo = TYPE_LIST[cmbType.SelectedIndex];
			switch (convertTo) {
			case STL_BIN:
				dstModel = new StlBin();
				saveExt = ".stl";
				break;
			case STL_TEXT:
				dstModel = new StlText();
				saveExt = ".stl";
				break;
			case FBX_BIN:
				saveExt = ".fbx";
				break;
			case FBX_TEXT:
				saveExt = ".fbx";
				break;
			case WAVEFRONT_OBJ:
				dstModel = new WavefrontObj();
				saveExt = ".obj";
				break;
			case METASEQUOIA:
				dstModel = new Metasequoia();
				saveExt = ".mqoz";
				break;
			case COLLADA:
				dstModel = new Collada();
				saveExt = ".dae";
				break;
			case MMD_PMX:
				dstModel = new MmdPmx();
				saveExt = ".pmx";
				break;
			}
			/*** savepath ***/
			var srcPath = (string)dataGridView1.Rows[index].Cells["FilePath"].Value;
			var saveDir = Path.GetDirectoryName(srcPath);
			var saveName = Path.GetFileNameWithoutExtension(srcPath);
			var saveFilePath = saveDir + "\\" + saveName;
			var tmpFilePath = saveFilePath + saveExt;
			for (int i = 1; File.Exists(tmpFilePath); i++) {
				tmpFilePath = saveFilePath + "_" + i + saveExt;
			}
			saveFilePath = tmpFilePath;
			/*** convert ***/
			var srcModel = mModelList[index];
			dstModel.Load(srcModel);
			if (chkResize.Checked) {
				dstModel.Normalize((float)numSize.Value);
			}
			dstModel.Save(saveFilePath);
		}
	}
}
