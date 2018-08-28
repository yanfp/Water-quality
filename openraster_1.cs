using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;

namespace water_quality
{
    class openraster_1
    {
        public IMap pMap;
        public void OpenRaster(string rasterFileName)
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
