using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class K_Means : DevExpress.XtraEditors.XtraForm
    {
        public bool ShowResultOrNot = false;
        public string ResultFilePath = string.Empty;
        public IMap pMap;
        public K_Means()
        {
            InitializeComponent();
        }

        private void K_Means_Load(object sender, EventArgs e)
        {
            cmbInputFile.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                cmbInputFile.Items.Add(pMap.get_Layer(i).Name);
            if (cmbInputFile.Items.Count > 0)
                cmbInputFile.SelectedIndex = 0;
        }

        private void btnOpenOuputFileDialog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Img and Tif Files(*.img,*.tif)|*.img;*.tif|Img Files(*.img)|*.img|Tif Files(*.tif)|*.tif|All Files(*.*)|*.*";
            saveFileDialog.Title = "保存分类图像";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtbOutput.Text = saveFileDialog.FileName;
                ResultFilePath = txtbOutput.Text;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (cmbInputFile.Text == "")
            {
                MessageBox.Show("输入为空，请选择输入文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else if (txtbOutput.Visible == true && txtbOutput.Text == "")
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
                string str = ".compile '" + Application.StartupPath + "\\RSImage_KMeans.pro" + "'";
                oComIDL.ExecuteString(str);

                string inputfile = string.Empty;
                IRasterLayer pRasterLayer = pMap.get_Layer(cmbInputFile.SelectedIndex) as IRasterLayer;
                for (int i = 0; i < pRasterLayer.BandCount; i++)
                {
                    inputfile = pRasterLayer.FilePath;
                }

                string pos = "[";
                for (int i = 0; i < listBox1.SelectedIndices.Count; i++)
                {
                    pos += (int)listBox1.SelectedIndices[i] + ",";
                }
                pos = pos.Substring(0, pos.Length - 1) + "]";

                string str1 = "RSImage_KMeans" + ",'" + inputfile + "'" + "," + pos + ",'" + txtbOutput.Text + "'" + ",7,'" + dmudMinClass.Text + "'" + ",'" + dmudIterations.Text + "'" + ",'" + txtChangeThresh.Text + "'";
                oComIDL.ExecuteString(str1);

                if (MessageBox.Show("分类成功，是否显示结果？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    openraster_1 openrasterfile = new openraster_1();
                    openrasterfile.pMap = pMap;
                    openrasterfile.OpenRaster(txtbOutput.Text);
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
    }
}