using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class MinimumDistance : DevExpress.XtraEditors.XtraForm
    {
        List<string> MiniDistName = new List<string>();
        public bool ShowResultOrNot = false;
        public string ResultFilePath = string.Empty;
        public IMap pMap;
        public MinimumDistance()
        {
            InitializeComponent();
        }

        private void MinimumDistance_Load(object sender, EventArgs e)
        {
            cmbInputFile.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                cmbInputFile.Items.Add(pMap.get_Layer(i).Name);
            if (cmbInputFile.Items.Count > 0)
                cmbInputFile.SelectedIndex = 0;
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择栅格图像";
            openFileDialog.Filter = "Img and Tif Files(*.img,*.tif)|*.img;*.tif|Img Files(*.img)|*.img|Tif Files(*.tif)|*.tif|All Files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                cmbInputFile.Text = openFileDialog.FileName;
            }

            try
            {
                //IRasterDataset rd = pMap.OpenFileRasterDataset(openFileDialog.FileName);
                IRasterLayer pRasterLayer = pMap.get_Layer(cmbInputFile.SelectedIndex) as IRasterLayer;
                //IRasterBandCollection rbc = (IRasterBandCollection)rd;
                for (int i = 0; i < pRasterLayer.BandCount; i++)
                {
                    listBox1.Items.Add("Band  " + Convert.ToString(i + 1));
                }
                lbSelectedBand.Text = "0//" + listBox1.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbInputFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            IRasterLayer pRasterLayer = pMap.get_Layer(cmbInputFile.SelectedIndex) as IRasterLayer;

            for (int i = 0; i < pRasterLayer.BandCount; i++)
            {
                listBox1.Items.Add("Band  " + Convert.ToString(i + 1));
            }
            lbSelectedBand.Text = "0//" + listBox1.Items.Count;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int listCount = listBox1.Items.Count;
            listBox1.SelectionMode = SelectionMode.MultiExtended;
            lbSelectedBand.Text = listBox1.SelectedItems.Count + "//" + listCount.ToString();
        }

        private void btnSelectROI_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择ROI文件";
            openFileDialog.Filter = "ROI File(*.roi)|*.roi";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtInputROI.Text = openFileDialog.FileName;
            }
        }

        private void btnSelectRuleFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Img Files(*.img)|*.img";
            saveFileDialog.Title = "保存规则图像";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputRuleFile.Text = saveFileDialog.FileName;
            }
        }

        private void btnSelectOutputImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Img and Tif Files(*.img,*.tif)|*.img;*.tif|Img Files(*.img)|*.img|Tif Files(*.tif)|*.tif|All Files(*.*)|*.*";
            saveFileDialog.Title = "保存分类图像";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputImage.Text = saveFileDialog.FileName;
                ResultFilePath = txtOutputImage.Text;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = true;
            txtOutputRuleFile.Visible = true;
            btnSelectRuleFile.Visible = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Visible = false;
            txtOutputRuleFile.Visible = false;
            btnSelectRuleFile.Visible = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            label4.Visible = false;
            txtOutputImage.Visible = false;
            btnSelectOutputImage.Visible = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            label4.Visible = true;
            txtOutputImage.Visible = true;
            btnSelectOutputImage.Visible = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbInputFile.Text == "" || txtInputROI.Text == "")
            {
                MessageBox.Show("输入为空，请选择输入文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else if (txtOutputImage.Visible == true && txtOutputImage.Text == "")
            {
                MessageBox.Show("输出为空，请选择输出文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else if (listBox1.SelectedItems.Count == 0)
            {
                MessageBox.Show("选择分类波段为空，请选择波段！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else
            {
                COM_IDL_connectLib.COM_IDL_connect oComIDL = new COM_IDL_connectLib.COM_IDL_connect();
                oComIDL.CreateObject(0, 0, 0);
                oComIDL.ExecuteString(".RESET_SESSION");
                string str = ".compile '" + Application.StartupPath + "\\RSImage_MinimumDistance.pro" + "'";
                oComIDL.ExecuteString(str);

                string inputfile = string.Empty;
                for (int i = 0; i < MiniDistName.Count(); i++)
                {
                    string selectedFileName = cmbInputFile.SelectedItem.ToString().Trim();
                    if (selectedFileName == System.IO.Path.GetFileName(MiniDistName[i]))
                    {
                        inputfile = MiniDistName[i];
                    }
                    else
                    {
                        inputfile = cmbInputFile.Text.ToString().Trim();
                    }
                }

                string pos = "[";
                for (int i = 0; i < listBox1.SelectedIndices.Count; i++)
                {
                    pos += (int)listBox1.SelectedIndices[i] + ",";
                }
                pos = pos.Substring(0, pos.Length - 1) + "]";
                if (txtOutputRuleFile.Text == "")
                {
                    txtOutputRuleFile.Text = "0";
                }
                string str1 = "RSImage_MinimumDistance" + ",'" + inputfile + "'" + "," + pos + ",'" + txtInputROI.Text + "'" + ",'" + txtOutputRuleFile.Text + "'" + ",'" + txtOutputImage.Text + "'";
                oComIDL.ExecuteString(str1);

                if (MessageBox.Show("分类成功，是否显示结果？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ShowResultOrNot = true;
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}