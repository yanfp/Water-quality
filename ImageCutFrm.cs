using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace water_quality
{
    public partial class ImageCutFrm : DevExpress.XtraEditors.XtraForm
    {
        public IMap m_pMap;
        public ImageCutFrm()
        {
            InitializeComponent();
        }

        private void ImageCutFrm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            int layerCount = m_pMap.LayerCount;
            for (int i = 0; i < layerCount; i++)
            {
                comboBox1.Items.Add(m_pMap.get_Layer(i).Name);
                comboBox2.Items.Add(m_pMap.get_Layer(i).Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_pMap.get_Layer(comboBox2.SelectedIndex) is IRasterLayer)
            {
                MessageBox.Show("选择数据错误！");
                return;
            }
            if (!(m_pMap.get_Layer(comboBox1.SelectedIndex) is IRasterLayer))
            {
                MessageBox.Show("选择数据错误！");
                return;
            }
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("正在裁剪.....");　　　　　// 信息
            //获得shp文件路径
            IDataLayer pShpLayer;
            IDatasetName pDatasetName;
            IWorkspaceName pWSName;
            pShpLayer = (IDataLayer)m_pMap.get_Layer(comboBox2.SelectedIndex);
            pDatasetName = (IDatasetName)pShpLayer.DataSourceName;
            pWSName = pDatasetName.WorkspaceName;
            string pFilePath = pWSName.PathName;
            string pFileName = pDatasetName.Name;
            string ShpPath = pFilePath + "\\" + pFileName + ".shp";

            //获得栅格文件
            IRasterLayer pDataLayer;
            pDataLayer = (IRasterLayer)m_pMap.get_Layer(comboBox1.SelectedIndex);
            //初始化ENVI
            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            //这个功能需要调用ENVI
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\startENVI.pro'");
            oComIDL.ExecuteString("startEnvi,/ShowProcess");

            ////执行图像裁剪
            //编译裁剪程序
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\cal_subset.pro'");
            //执行裁剪
            oComIDL.ExecuteString(@"cal_subset,'" + pDataLayer.FilePath + "','" + ShpPath + "','" + textBox1.Text + "'");

            //结束ENVI
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\endENVI.pro'");
            oComIDL.ExecuteString("endEnvi,");
            //加载图像裁剪后影像
            openraster_1 openrasterfile = new openraster_1();
            openrasterfile.pMap = m_pMap;
            openrasterfile.OpenRaster(textBox1.Text);
            //OpenRaster(textBox1.Text);
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

        //    m_pMap.AddLayer(pRasterLayer2 as ILayer);

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "浏览";
            saveDlg.Filter = "TIFF文件|*.tif|IMG文件|*.img";
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = saveDlg.FileName;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}