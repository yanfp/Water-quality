using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class ExportToImageFr : Form
    {
        private AxPageLayoutControl m_pageLayoutControl;
        private string m_strFileName="";
        public ExportToImageFr(AxPageLayoutControl page)
        {
            InitializeComponent();
            m_pageLayoutControl = page;
        }
        private void ExportToImageFr_Load(object sender, EventArgs e)
        {
            m_strFileName = textBoxFileName.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "浏览";
            saveDlg.Filter = "JPG|*.jpg|BMP|*.bmp|PNG|*.png";
            //saveDlg.InitialDirectory = "C:\\Users\\Administrator\\Desktop";            
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFileName.Text = saveDlg.FileName;
                m_strFileName = saveDlg.FileName;//完全名
                if (m_strFileName == string.Empty)
                    return;
                string pathName = System.IO.Path.GetDirectoryName(m_strFileName);//位置
                string strFExtendN = System.IO.Path.GetExtension(m_strFileName);//后缀名
                string fileName = System.IO.Path.GetFileNameWithoutExtension(m_strFileName);//单独的文件名
                string fileNameE = System.IO.Path.GetFileName(m_strFileName);//文件名和扩展名  
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(textBoxFileName.Text.ToString()) == true)
            {
                MessageBox.Show("该文件已经存在，请重新命名！");
                textBoxFileName.SelectAll();               
            }
            else
            {
                IExport pExport = null;
                IWorldFileSettings pWorldFile = null;
                IExportImage pExportType;
                IEnvelope pDriverBounds = null;

                ESRI.ArcGIS.esriSystem.tagRECT userRECT = new ESRI.ArcGIS.esriSystem.tagRECT();
                IEnvelope pEnv = new EnvelopeClass();

                string FilePath = this.m_strFileName;
                string[] strFileName = FilePath.Split('.');
                string strFileType = strFileName[1];
                switch (strFileType)
                {
                    case "jpg":
                        pExport = new ExportJPEGClass();
                        break;
                    case "bmp":
                        pExport = new ExportBMPClass();
                        break;
                    case "gif":
                        pExport = new ExportGIFClass();
                        break;
                    case "tif":
                        pExport = new ExportTIFFClass();
                        break;
                    case "png":
                        pExport = new ExportPNGClass();
                        break;
                    case "emf":
                        pExport = new ExportEMFClass();
                        break;
                    case "pdf":
                        pExport = new ExportPDFClass();
                        break;
                    case ".ai":
                        pExport = new ExportAIClass();
                        break;
                    case "svg":
                        pExport = new ExportSVGClass();
                        break;
                    default:
                        pExport = new ExportJPEGClass();
                        break;
                }

                pExport.ExportFileName = this.m_strFileName;
                pExport.Resolution = Convert.ToInt32(numUDresolution.Value);
                pExportType = pExport as IExportImage;
                pExportType.ImageType = esriExportImageType.esriExportImageTypeTrueColor;
                pEnv = m_pageLayoutControl.ActiveView.Extent;
                pWorldFile = (IWorldFileSettings)pExport;
                pWorldFile.MapExtent = pEnv;
                pWorldFile.OutputWorldFile = false;
                userRECT.top = 0;
                userRECT.left = 0;
                userRECT.right = Convert.ToInt32(txtBoxWidth.Text);
                userRECT.bottom = Convert.ToInt32(txtBoxHeight.Text);
                pDriverBounds = new EnvelopeClass();
                pDriverBounds.PutCoords(userRECT.top, userRECT.bottom, userRECT.right, userRECT.top);
                pExport.PixelBounds = pDriverBounds;
                ITrackCancel pTrackCancel = new TrackCancelClass();
                m_pageLayoutControl.ActiveView.Output(pExport.StartExporting(), Convert.ToInt32(numUDresolution.Value), ref userRECT, m_pageLayoutControl.ActiveView.Extent, pTrackCancel);
                pExport.FinishExporting();
                MessageBox.Show("打印图片保存成功!", "保存", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private void textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            m_strFileName = textBoxFileName.Text;
        }

        
    }
}
