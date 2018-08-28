using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
namespace water_quality
{
    public partial class rad_calibration : DevExpress.XtraEditors.XtraForm
    {
        public IMap pMap;
        public rad_calibration()
        {
            InitializeComponent();
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_calibration_Click(object sender, EventArgs e)
        {
            //初始化ENVI

            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            //执行辐射定标
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\cal_calibration.pro'");

            IRasterLayer pDataLayer;
            pDataLayer = (IRasterLayer)pMap.get_Layer(comboBoxOpen.SelectedIndex);

            //执行辐射定标
            if (SensorcomboBox.SelectedIndex == 0)
            {
                oComIDL.ExecuteString(@"cal_calibration,'" + pDataLayer.FilePath + "','" + textBoxOut.Text + "'");
            }
            else if (SensorcomboBox.SelectedIndex == 1)
            {
                oComIDL.ExecuteString(@"cal_calibration,'" + pDataLayer.FilePath + "','" + textBoxOut.Text + "'");
            }


            oComIDL.DestroyObject();
            //加载辐射定标后影像
            //openraster_1 openrasterfile = new openraster_1();
            //openrasterfile.pMap = pMap;
            //openrasterfile.OpenRaster(textBoxOut.Text);
            OpenRaster(textBoxOut.Text);
            this.Close();
        }
        //private void OpenRaster(string rasterFileName)
        //{

        //    //文件名处理

        //    string ws = System.IO.Path.GetDirectoryName(rasterFileName);

        //    string fbs = System.IO.Path.GetFileName(rasterFileName);

        //    //创建工作空间

        //    IWorkspaceFactory pWork = new RasterWorkspaceFactoryClass();

        //    //打开工作空间路径，工作空间的参数是目录，不是具体的文件名

        //    IRasterWorkspace pRasterWS = (IRasterWorkspace)pWork.OpenFromFile(ws, 0);

        //    //打开工作空间下的文件，

        //    IRasterDataset pRasterDataset = pRasterWS.OpenRasterDataset(fbs);


        //    IRasterLayer pRasterLayer = new RasterLayerClass();

        //    pRasterLayer.CreateFromDataset(pRasterDataset);

        //    //添加到图层控制中
        //    IRaster pRaster = pRasterLayer.Raster;

        //    IGeoDataset pGeodataset = pRaster as IGeoDataset;
        //    IRasterBandCollection pRsBandCol = pGeodataset as IRasterBandCollection;
        //    int bandCount;
        //    bandCount = pRsBandCol.Count;
        //    if (bandCount == 1)
        //    {
        //        IRasterBand pRasterBand1 = pRsBandCol.Item(0);
        //        pRasterBand1.ComputeStatsAndHist();
        //    }
        //    if (bandCount > 1)
        //    {
        //        IRasterBand pRasterBand1 = pRsBandCol.Item(0);
        //        pRasterBand1.ComputeStatsAndHist();
        //        IRasterBand pRasterBand2 = pRsBandCol.Item(1);
        //        pRasterBand2.ComputeStatsAndHist();
        //        IRasterBand pRasterBand3 = pRsBandCol.Item(2);
        //        pRasterBand3.ComputeStatsAndHist();
        //    }
        //    IRasterDataset pRasterDataset2 = pRasterWS.OpenRasterDataset(fbs);
        //    IRasterLayer pRasterLayer2 = new RasterLayerClass();
        //    pRasterLayer2.CreateFromDataset(pRasterDataset2);

        //    //添加到图层控制中

        //    pMap.AddLayer(pRasterLayer2 as ILayer);

        //}

        private void rad_calibration_Load(object sender, EventArgs e)
        {
            comboBoxOpen.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                comboBoxOpen.Items.Add(pMap.get_Layer(i).Name);
            if (comboBoxOpen.Items.Count > 0)
                comboBoxOpen.SelectedIndex = 0;

            SensorcomboBox.Items.Add("Landsat4/5 TM");
            SensorcomboBox.Items.Add("Landsat7 ETM+");
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
    }
}