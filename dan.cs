using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;

namespace water_quality
{
    public partial class dan : DevExpress.XtraEditors.XtraForm
    {
        public IMap m_pMap;
        public dan()
        {
            InitializeComponent();
        }

        private void dan_Load(object sender, EventArgs e)
        {
            int LayerCount = m_pMap.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                comboBoxSelect.Items.Add(m_pMap.get_Layer(i).Name);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "IMG文件|*.img|TIF文件|*.tif|PNG文件|*.png|JPG文件|*.jpg";
            saveDlg.FileName = "result";
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                textBoxOutPut.Text = saveDlg.FileName;
            }
        }

        private void bt_OK_Click(object sender, EventArgs e)
        {
            if (comboBoxSelect.SelectedItem == null)
            {
                MessageBox.Show("文件不能为空！");
                return;
            }
            //pCreatRalyr = (IRasterLayer)pMap.get_Layer(comboBoxOpen.SelectedIndex);
            string strFilePath = GetFileNameByLayer.GetRasterFileName(m_pMap.get_Layer(comboBoxSelect.SelectedIndex));
            //初始化ENVI

            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\dan.pro'");
            oComIDL.ExecuteString(@"green,'" + strFilePath + "','" + textBoxOutPut.Text + "'");
            oComIDL.DestroyObject();
            this.Close();
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}