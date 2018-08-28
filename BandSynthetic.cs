using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Controls;

namespace water_quality
{
    public partial class BandSynthetic : DevExpress.XtraEditors.XtraForm
    {
        public IMap m_pMap=null;
        public AxMapControl m_mapControl;
        IRasterBandCollection m_pRasterBandCollection = null;
        IRasterDataset m_pRasterDataset = null;
        public BandSynthetic()
        {
            InitializeComponent();
        }

        private void bt_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_openfile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "IMG文件|*.img|TIF文件|*.tif";
            saveDlg.FileName = "result";
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                textBoxOutPut.Text = saveDlg.FileName;
            }
        }
        public static IRasterDataset OpenFileRasterDataset(string fullpath)
        {
            IWorkspaceFactory WorkspaceFactory = new RasterWorkspaceFactoryClass();
            IWorkspace Workspace = WorkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(fullpath), 0);
            IRasterWorkspace rasterWorkspace = (IRasterWorkspace)Workspace;
            IRasterDataset rasterSet = (IRasterDataset)rasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(fullpath));
            return rasterSet;
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            if (comboBoxSelect.SelectedItem == null)
            {
                MessageBox.Show("图层不能为空");
                return;
            }
            if (comboBoxRed.SelectedItem == null || comboBoxGreen.SelectedItem == null || comboBoxBlue.SelectedItem == null)
            {
                MessageBox.Show("波段不能为空!");
                return;
            }
            if (textBoxOutPut.Text == "")
            {
                MessageBox.Show("存储路径不能为空!");
                return;
            }
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromDataset(m_pRasterDataset);
            IRasterRGBRenderer pRasterRGBRen = new RasterRGBRendererClass();
            IRasterRenderer pRasterRen = (IRasterRenderer)pRasterRGBRen;

            pRasterRen.Raster = pRasterLayer.Raster;
            pRasterRGBRen.RedBandIndex = comboBoxRed.SelectedIndex;
            pRasterRGBRen.GreenBandIndex = comboBoxGreen.SelectedIndex;
            pRasterRGBRen.BlueBandIndex = comboBoxBlue.SelectedIndex;
            pRasterRen.Update();
            pRasterLayer.Renderer = (IRasterRenderer)pRasterRGBRen;
            IRaster pRaster = pRasterLayer.Raster;
            if (pRaster != null)
            {
                IWorkspaceFactory WF = new RasterWorkspaceFactoryClass();
                string filename = textBoxOutPut.Text;
                string filepath = System.IO.Path.GetDirectoryName(filename);
                string name = System.IO.Path.GetFileName(filename);
                try
                {
                    IWorkspace rasterWorkspace = WF.OpenFromFile(filepath, 0);
                    ISaveAs saveAs = (ISaveAs)pRaster;
                    saveAs.SaveAs(name, rasterWorkspace, "");
                    if (MessageBox.Show("保存成功，是否打开图层？") == DialogResult.OK)
                    {
                        pRasterLayer.Name = name;
                        m_mapControl.AddLayer(pRasterLayer);
                        m_mapControl.Extent = pRasterLayer.AreaOfInterest;
                        m_mapControl.ActiveView.Refresh();
                        m_mapControl.Refresh();
                        m_mapControl.Update();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            splashScreenManager1.CloseWaitForm();
            this.Close();
        }
        private void comboBoxSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_pRasterDataset = OpenFileRasterDataset(GetFileNameByLayer.GetRasterFileName(m_pMap.get_Layer(comboBoxSelect.SelectedIndex)));
            m_pRasterBandCollection = (IRasterBandCollection)m_pRasterDataset;
            comboBoxRed.Items.Clear();
            comboBoxGreen.Items.Clear();
            comboBoxBlue.Items.Clear();
            for (int i = 0; i < m_pRasterBandCollection.Count; i++)
            {
                IRasterBand band = m_pRasterBandCollection.Item(i);
                comboBoxRed.Items.Add(band.Bandname);
                comboBoxGreen.Items.Add(band.Bandname);
                comboBoxBlue.Items.Add(band.Bandname);
            }
        }

        private void BandSynthetic_Load(object sender, EventArgs e)
        {
            m_pMap = m_mapControl.Map;
            int LayerCount = m_pMap.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                comboBoxSelect.Items.Add(m_pMap.get_Layer(i).Name);
            }
        }  

        }
}

