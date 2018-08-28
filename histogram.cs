using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace water_quality
{
    public partial class histogram : DevExpress.XtraEditors.XtraForm
    {
        public IMap pMap;
        public IRasterLayer pCreatRalyr;
        public histogram()
        {
            InitializeComponent();
        }

        private void bt_cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void histogram_Load(object sender, EventArgs e)
        {
            comboBoxOpen.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                comboBoxOpen.Items.Add(pMap.get_Layer(i).Name);
            if (comboBoxOpen.Items.Count > 0)
                comboBoxOpen.SelectedIndex = 0;
        }

        private void bt_openfile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "IMAGINE|*.img|TIFF|*.tif|Raster|*.jpg";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "输出栅格图像";
            saveDlg.RestoreDirectory = true;
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                textBoxOut.Text = saveDlg.FileName;
        }

        private void bt_histogram_ok_Click(object sender, EventArgs e)
        {
             pCreatRalyr = (IRasterLayer)pMap.get_Layer(comboBoxOpen.SelectedIndex);
            //初始化ENVI
            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            //执行线性拉伸
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\item_b_3.pro'");
            oComIDL.ExecuteString(@"item_b_3,'" + pCreatRalyr.FilePath + "','" + textBoxOut.Text + "'");
            oComIDL.DestroyObject();
            //加载线性拉伸后影像
            OpenRaster(textBoxOut.Text);
            this.Close();
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
            IRaster pRaster = pRasterLayer.Raster;
            IGeoDataset pGeodataset = pRaster as IGeoDataset;
            IRasterBandCollection pRsBandCol = pGeodataset as IRasterBandCollection;
            int bandCount;
            bandCount = pRsBandCol.Count;
            if (bandCount == 1)
            {
                IRasterBand pRasterBand1 = pRsBandCol.Item(0);
                pRasterBand1.ComputeStatsAndHist();
            }
            if (bandCount > 1)
            {
                IRasterBand pRasterBand1 = pRsBandCol.Item(0);
                pRasterBand1.ComputeStatsAndHist();
                IRasterBand pRasterBand2 = pRsBandCol.Item(1);
                pRasterBand2.ComputeStatsAndHist();
                IRasterBand pRasterBand3 = pRsBandCol.Item(2);
                pRasterBand3.ComputeStatsAndHist();
            }


            IRasterDataset pRasterDataset2 = pRasterWS.OpenRasterDataset(fbs);
            IRasterLayer pRasterLayer2 = new RasterLayerClass();
            pRasterLayer2.CreateFromDataset(pRasterDataset2);

            //添加到图层控制中

            pMap.AddLayer(pRasterLayer2 as ILayer);

        }
    }
}