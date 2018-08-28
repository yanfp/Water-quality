using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace water_quality
{
    public partial class resample : DevExpress.XtraEditors.XtraForm
    {
        //private System.Windows.Forms.ComboBox comboBoxSelect;
        public IMap m_pMap;
        public resample()
        {
            InitializeComponent();
        }
        private void OpenRaster(string rasterFileName)
        {
            //文件名处理
            string ws = System.IO.Path.GetDirectoryName(rasterFileName);
            string fbs = System.IO.Path.GetFileName(rasterFileName);
            //创建工作空间
            IWorkspaceFactory pWork = new RasterWorkspaceFactoryClass();
            //打开工作空间路径，工作空间的参数是目录，不是具体的文件名
            IRasterWorkspace pRasterWS = (IRasterWorkspace)pWork.OpenFromFile(ws, 0);
            //打开工作空间下的文件，
            IRasterDataset pRasterDataset = pRasterWS.OpenRasterDataset(fbs);
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromDataset(pRasterDataset);
            //添加到图层控制中
            m_pMap.AddLayer(pRasterLayer as ILayer);
        }
        private void resample_Load(object sender, EventArgs e)
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

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxZoom_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void bt_OK_Click(object sender, EventArgs e)
        {

            {
                if (comboBoxSelect.SelectedItem == null)
                {
                    MessageBox.Show("文件不能为空！");
                    return;
                }
                if (textBoxZoom.Text == "")
                {
                    MessageBox.Show("请输入放缩比例！");
                    return;
                }
                if (textBoxOutPut.Text == "")
                {
                    MessageBox.Show("请选择输出路径！");
                    return;
                }
                string strRasterPath = GetFileNameByLayer.GetRasterFileName(m_pMap.get_Layer(comboBoxSelect.SelectedIndex));
                string strOutPut = textBoxOutPut.Text;
                //float zoom = Convert.ToSingle(textBoxZoom.Text);
                COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
                oComIDL.CreateObject(0, 0, 0);
                //执行重采样
                oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\object_envi_resize__define.pro'");
                oComIDL.ExecuteString(@"s = obj_new('object_envi_resize','" + strRasterPath + "','" + strOutPut + "')");
                oComIDL.ExecuteString("s.EXECUTERESIZE," + textBoxZoom.Text + "," + textBoxZoom.Text + ","+cobox_method.SelectedIndex.ToString());
                //oComIDL.ExecuteString("Obj_destroy,s");
                oComIDL.DestroyObject();
                //加载放大后影像
                OpenRaster(strOutPut);
                this.Close();
            }
        }
    }
}