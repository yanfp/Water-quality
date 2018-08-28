using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;

namespace water_quality
{
    class OFile
    {
               
        public static void CopyAndOverwriteMap(AxMapControl mapControl)
        {
            IObjectCopy objectCopy = new ObjectCopyClass();
            object toCopyMap = mapControl.Map;
            object copiedMap = objectCopy.Copy(toCopyMap);
        }

        public static void Use_OnViewRefreshed(AxTOCControl toccControl,
            AxMapControl mapControl, AxPageLayoutControl axPageLayoutControl)
        {
            toccControl.Update();
            OFile.CopyAndOverwriteMap(mapControl);
        }

        public static void ArchieveEagleEyeWhenAddRaster(AxMapControl mapControl,string strFileName)
        {
            
            if (strFileName == string.Empty)
                return;
            string pathName = System.IO.Path.GetDirectoryName(strFileName);
            string fileName = System.IO.Path.GetFileName(strFileName);
            IWorkspaceFactory pWSF;
            pWSF = new RasterWorkspaceFactory();
            IWorkspace pWS;
            pWS = pWSF.OpenFromFile(pathName, 0);
            IRasterWorkspace pRWS;
            pRWS = pWS as IRasterWorkspace;
            IRasterDataset pRasterDataset;
            pRasterDataset = pRWS.OpenRasterDataset(fileName);
            //影像金字塔判断与创建
            IRasterPyramid pRasPyrmid;
            pRasPyrmid = pRasterDataset as IRasterDataset as IRasterPyramid;
            if (pRasPyrmid != null)
            {
                if (!(pRasPyrmid.Present))
                {
                    pRasPyrmid.Create();//在进度条中说明正在创建金字塔
                }
            }
            IRaster pRaster;
            pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer;
            pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            ILayer pLayer = pRasterLayer as ILayer;
            mapControl.AddLayer(pLayer, 0);
        }

        public static void OpenFile(AxMapControl mapControl)
        {
            OpenFileDialog OpenFdlg = new OpenFileDialog();
            OpenFdlg.Title = "选择需要加载的地理数据文件";
            OpenFdlg.Filter = "mxd文件|*.mxd|Shape文件|*.shp|所有文件|*.*";
            //OpenFdlg.InitialDirectory="C:\\Users\\Administrator\\Desktop";//设置初始打开的文件夹
            OpenFdlg.RestoreDirectory = true;
            if (OpenFdlg.ShowDialog() == DialogResult.OK)
            {
                
                string strFileName = OpenFdlg.FileName;//完全名
                if (strFileName == string.Empty)
                    return;
                string pathName = System.IO.Path.GetDirectoryName(strFileName);//位置
                string strFExtendN = System.IO.Path.GetExtension(strFileName);//后缀名
                string fileName = System.IO.Path.GetFileNameWithoutExtension(strFileName);//单独的文件名
                string fileNameE = System.IO.Path.GetFileName(strFileName);//文件名和扩展名
                switch (strFExtendN)
                {
                    case ".shp":
                        mapControl.AddShapeFile(pathName, fileName);
                        OFile.CopyAndOverwriteMap(mapControl);
                        break;
                    case ".mxd":
                        if (mapControl.CheckMxFile(strFileName))
                        {
                            mapControl.MousePointer = esriControlsMousePointer.esriPointerHourglass;
                            mapControl.LoadMxFile(strFileName, 0, Type.Missing);
                            mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                            OFile.CopyAndOverwriteMap(mapControl);
                        }
                        else
                        {
                            MessageBox.Show("所选文件不是地图文档文件！", "信息提示");
                            return;
                        }
                        break;
                    case ".bmp":
                    case ".tif":
                    case ".TIF":
                    case ".jpg":
                    case ".img":
                    case ".png":

                        IWorkspaceFactory pWSF;
                        pWSF = new RasterWorkspaceFactory();
                        IWorkspace pWS;
                        pWS = pWSF.OpenFromFile(pathName, 0);
                        IRasterWorkspace pRWS;
                        pRWS = pWS as IRasterWorkspace;
                        IRasterDataset pRasterDataset;
                        pRasterDataset = pRWS.OpenRasterDataset(fileNameE);
                        //影像金字塔判断与创建
                        IRasterPyramid pRasPyrmid;
                        pRasPyrmid = pRasterDataset as IRasterDataset as IRasterPyramid;
                        if (pRasPyrmid != null)
                        {
                            if (!(pRasPyrmid.Present))
                            {
                                pRasPyrmid.Create();//在进度条中说明正在创建金字塔
                            }
                        }
                        IRaster pRaster;
                        pRaster = pRasterDataset.CreateDefaultRaster();
                        IRasterLayer pRasterLayer;
                        pRasterLayer = new RasterLayerClass();
                        pRasterLayer.CreateFromRaster(pRaster);
                        ILayer pLayer = pRasterLayer as ILayer;
                        mapControl.AddLayer(pLayer, 0);
                        OFile.CopyAndOverwriteMap(mapControl);
                        break;

                }

            }
        }
    
    }
    }

