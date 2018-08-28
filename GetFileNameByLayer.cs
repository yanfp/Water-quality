using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace water_quality
{
    class GetFileNameByLayer
    {
        public static string GetShpFileName(ILayer layer)
        {
            IDataLayer pShpLayer;
            IDatasetName pDatasetName;
            IWorkspaceName pWSName;
            pShpLayer = (IDataLayer)layer;
            pDatasetName = (IDatasetName)pShpLayer.DataSourceName;
            pWSName = pDatasetName.WorkspaceName;
            string pFilePath = pWSName.PathName;
            string pFileName = pDatasetName.Name;
            string ShpPath = pFilePath + "\\" + pFileName + ".shp";
            return ShpPath;
        }

        public static string GetRasterFileName(ILayer layer)
        {
            IRasterLayer pDataLayer;
            pDataLayer = (IRasterLayer)layer;
            return pDataLayer.FilePath;      
        }
    }
}
