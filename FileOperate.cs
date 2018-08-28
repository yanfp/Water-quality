using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Geometry;


namespace MAP
{
    
    class OperateFile
    {  
        
        //复制地图
        public static void CopyAndOverwriteMap(AxMapControl mapControl, AxPageLayoutControl axPageLayoutControl)
        {
            IObjectCopy objectCopy = new ObjectCopyClass();
            object toCopyMap = mapControl.Map;
            object copiedMap = objectCopy.Copy(toCopyMap);
            object toOverwriteMap = axPageLayoutControl.ActiveView.FocusMap;
            objectCopy.Overwrite(copiedMap,ref toOverwriteMap);
        }

        //在OnAfterScreenDraw事件下使用
        public static void Use_OnAfterScreenDraw(AxMapControl mapControl, AxPageLayoutControl axPageLayoutControl)
        {
            IActiveView pActiveView =axPageLayoutControl.ActiveView.FocusMap as IActiveView;
            IDisplayTransformation displayTransformation = pActiveView.ScreenDisplay.DisplayTransformation;
            displayTransformation.VisibleBounds = mapControl.Extent;
            axPageLayoutControl.ActiveView.Refresh();
            OperateFile.CopyAndOverwriteMap(mapControl, axPageLayoutControl);
        }

        //在OnviewRfeshed事件下使用（联动）
        public static void Use_OnViewRefreshed(AxTOCControl toccControl,AxMapControl mapControl, AxPageLayoutControl axPageLayoutControl)
        {
            toccControl.Update();
            OperateFile.CopyAndOverwriteMap(mapControl, axPageLayoutControl);
        }

        //添加栅格文件时实现鹰眼要用
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

        //打开文件函数
        public static void OpenFile(AxMapControl mapControl, AxPageLayoutControl pageLayoutControl)
        {
            OpenFileDialog OpenFdlg = new OpenFileDialog();
            OpenFdlg.Title = "选择需要加载的地理数据文件";
            OpenFdlg.Filter = "mxd文件|*.mxd|Shape文件|*.shp|所有文件|*.*";
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
                        OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                        break;
                    case ".mxd":
                        if (mapControl.CheckMxFile(strFileName))
                        {
                            mapControl.MousePointer = esriControlsMousePointer.esriPointerHourglass;
                            mapControl.LoadMxFile(strFileName, 0, Type.Missing);
                            mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                            OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                        }
                        else
                        {
                            MessageBox.Show("所选文件不是地图文档文件！", "信息提示");
                            return;
                        }
                        break;
                    case ".bmp":
                    case ".BMP":
                    case ".tif":
                    case ".TIF":
                    case ".jpg":
                    case ".JPG":
                    case ".img":
                    case ".IMG":
                    case ".png":
                    case ".PNG":              
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
                        OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                        //OperateFile.ArchieveEagleEyeWhenAddRaster(mapControl, strFileName);
                        break;

                }

            }
        }

        //新建
        public static void NewDoc(AxMapControl mapControl, AxPageLayoutControl pageLayoutControl = null)
        {
            
            SaveFileDialog NewDlg = new SaveFileDialog();
            NewDlg.Title = "新建文档";
            NewDlg.InitialDirectory = "C:\\Users\\Administrator\\Desktop";
            NewDlg.FileName = "Default";
            NewDlg.Filter = "mxd文件|*.mxd";
            if (NewDlg.ShowDialog() == DialogResult.OK)
            {
                
                if (MessageBox.Show("        是否保存当前文档?", "信息提示", MessageBoxButtons.YesNo) == DialogResult.OK)
                {
                    OperateFile.SaveDoc(mapControl);
                }
                IMapDocument pMapDocument = new MapDocumentClass();
                
                pMapDocument.New(NewDlg.FileName);
                pMapDocument.Open(NewDlg.FileName);
                mapControl.Map = pMapDocument.get_Map(0);             
            }
            
        }
        //保存
        public static void SaveDoc(AxMapControl mapControl, AxPageLayoutControl pageLayoutControl=null)
        {
            //判断文档是否为只读文档
            IMapDocument pMapDocument = new MapDocumentClass();
            if (mapControl.Map.LayerCount != 0 && mapControl.DocumentFilename!=null)
            {
                
                pMapDocument.Open(mapControl.DocumentFilename);
                if (pMapDocument.get_IsReadOnly(pMapDocument.DocumentFilename))
                {
                    MessageBox.Show("此地图文档为只读文档", "信息提示");
                    return;
                }
                //用相对路径保存地图文档
                pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                MessageBox.Show("保存成功！", "信息提示");
            }
            else
            {
                OperateFile.SaveDocAs(mapControl, pageLayoutControl);
            }
            
        }

        //另存为
        public static void SaveDocAs(AxMapControl mapControl, AxPageLayoutControl pageLayoutControl=null)
        {
            IMapDocument pMapDocument = new MapDocumentClass();

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "地图文档另存为";
            saveDlg.Filter = "mxd文件|*.mxd";
            saveDlg.ShowDialog();
            string strDocFileN = saveDlg.FileName;
            if (strDocFileN == string.Empty)
                return;

            pMapDocument.Open(mapControl.DocumentFilename);
            if (strDocFileN == mapControl.DocumentFilename)
            {
                pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                return;
            }
            else
            {
                pMapDocument.SaveAs(strDocFileN, true, true);
                MessageBox.Show("保存成功！", "信息提示");
            }
          
            
            
        }

        //输出
        public static void ExpotTo(AxPageLayoutControl pageLayoutControl = null)
        {
            ExportToImageFr export = new ExportToImageFr(pageLayoutControl);
            export.ShowDialog();
        }


        public static void OpenFileMult(AxMapControl mapControl, AxPageLayoutControl pageLayoutControl)
        {
            OpenFileDialog OpenFdlg = new OpenFileDialog();
            OpenFdlg.Title = "选择需要加载的地理数据文件";
            OpenFdlg.Filter = "mxd文件|*.mxd|Shape文件|*.shp|所有文件|*.*";
            OpenFdlg.RestoreDirectory = true;
            OpenFdlg.Multiselect = true;
            if (OpenFdlg.ShowDialog() == DialogResult.OK)
            {
                string[] FileNameArray = OpenFdlg.FileNames;
                int fileCount = FileNameArray.Count();
                for (int i = 0; i < fileCount; i++)
                {

                    string strFileName = FileNameArray[i];//完全名
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
                            OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                            break;
                        case ".mxd":
                            if (mapControl.CheckMxFile(strFileName))
                            {
                                mapControl.MousePointer = esriControlsMousePointer.esriPointerHourglass;
                                mapControl.LoadMxFile(strFileName, 0, Type.Missing);
                                mapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                                OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                            }
                            else
                            {
                                MessageBox.Show("所选文件不是地图文档文件！", "信息提示");
                                return;
                            }
                            break;
                        case ".bmp":
                        case ".BMP":
                        case ".tif":
                        case ".TIF":
                        case ".jpg":
                        case ".JPG":
                        case ".img":
                        case ".IMG":
                        case ".png":
                        case ".PNG":
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
                            OperateFile.CopyAndOverwriteMap(mapControl, pageLayoutControl);
                            //OperateFile.ArchieveEagleEyeWhenAddRaster(mapControl, strFileName);
                            break;

                    }
                }
            }
 
        }
    
    }

}
