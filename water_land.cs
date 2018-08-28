using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class water_land : DevExpress.XtraEditors.XtraForm
    {
        public IMap pMap;
        public IRasterLayer pCreatRalyr;
        public water_land()
        {
            InitializeComponent();
        }

        private void water_land_Load(object sender, EventArgs e)
        {
            combo_input.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                combo_input.Items.Add(pMap.get_Layer(i).Name);
            if (combo_input.Items.Count > 0)
                combo_input.SelectedIndex = 0;
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
                tb_output.Text = saveDlg.FileName;
        }

        private void bt_OK_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("正在提取水体信息");
            pCreatRalyr = (IRasterLayer)pMap.get_Layer(combo_input.SelectedIndex);
            //初始化ENVI
            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\math_doit_water.pro'");
            oComIDL.ExecuteString(@"math_doit_water,'" + pCreatRalyr.FilePath + "','" + tb_output.Text + "'");
            oComIDL.DestroyObject();
            openraster_1 openrasterfile = new openraster_1();
            openrasterfile.pMap = pMap;
            openrasterfile.OpenRaster(tb_output.Text);
            //OpenRaster(tb_output.Text);
            splashScreenManager1.CloseWaitForm();
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
        
    }
}