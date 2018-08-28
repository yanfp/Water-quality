using System;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using System.Collections;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace water_quality
{
    public partial class AttributeForm : DevExpress.XtraEditors.XtraForm
    {
        public IMapControl3 m_mapControl3;
        public ILayer m_layer;
        private IRasterDataset m_pRasterDataset = null;
        private IRasterBandCollection m_pRasterBandCollection = null;

        //实现色带用
        private ArrayList EnumStyleItem = new ArrayList();
        private IGradientFillSymbol m_FillSymbol;
        private IColorRamp m_ColorRamp;

        public AttributeForm(IMapControl3 mapControl3)
        {
            InitializeComponent();
            m_mapControl3 = mapControl3;
            DrawColorRamp();
            comboBoxColor.SelectedIndex = 0;
            pictureBox1.Image = comboBoxColor.SelectedItem as Image;
            pictureBox2.Image = pictureBox1.Image;
        }

        private void AttributeForm_Load(object sender, EventArgs e)
        {
            this.Text = m_layer.Name+"属性";
            txtBoxTitle.Text = m_layer.Name;
            chkBoxVisible.Checked = m_layer.Visible;
            comboBoxStretchType.SelectedIndex = 0;
           
        }

        //MyFunction
        private void CommonSet()
        {
            m_layer.Name = txtBoxTitle.Text;
            m_layer.Visible = chkBoxVisible.Checked;
        }
        private void StretchRender()
        {
            if (comboBoxBand.SelectedItem == null)
            {
                MessageBox.Show("波段不能为空！");
                return;
            }
            IColorRamp pColorRamp = (IColorRamp)EnumStyleItem[comboBoxColor.SelectedIndex];
            IRasterLayer pRasterLayr = m_layer as IRasterLayer;
            IRasterStretchColorRampRenderer pStretchRenderer = new RasterStretchColorRampRendererClass();
            IRasterRenderer pRasterRenderer = (IRasterRenderer)pStretchRenderer;
            IRaster pRaster = pRasterLayr.Raster;
            pRasterRenderer.Raster = pRaster;
            pRasterRenderer.Update();
            pStretchRenderer.BandIndex = comboBoxBand.SelectedIndex;
            pStretchRenderer.ColorRamp = pColorRamp;
            IRasterStretch pStretchType = (IRasterStretch)pRasterRenderer;
            pStretchType.StretchType = esriRasterStretchTypesEnum.esriRasterStretch_StandardDeviations;
            pStretchType.StandardDeviationsParam = Convert.ToDouble(textBoxStandardValue.Text);

            pRasterLayr.Renderer = pRasterRenderer;

            m_mapControl3.Refresh();

        }

        private IColorRamp GetCurrentColorRamp()
        {
            IRasterLayer pRasterLayr = m_layer as IRasterLayer;
            IRasterStretchColorRampRenderer pStretchRenderer = new RasterStretchColorRampRendererClass();
            IRasterRenderer pRasterRenderer = (IRasterRenderer)pStretchRenderer;
            IRaster pRaster = pRasterLayr.Raster;
            pRasterRenderer.Raster = pRaster;
            pRasterRenderer.Update();
            pStretchRenderer.BandIndex = comboBoxBand.SelectedIndex;
            return pStretchRenderer.ColorRamp;

        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (m_layer is IRasterLayer)
                {
                    panel1.Visible = false;
                    panel2.Visible = true;
                    listBox1.SelectedIndex = 0;
                    panel3.Visible = true;
                }
                else
                {
                    panel1.Visible = true;
                    panel2.Visible = false;
                }
            }
            if (panel3.Visible == true)
            {
                //加载波段
                comboBoxBand.Items.Clear();
                string strFilepath = GetFileNameByLayer.GetRasterFileName(m_layer);
                m_pRasterDataset = OpenFileRasterDataset(strFilepath);
                m_pRasterBandCollection = (IRasterBandCollection)m_pRasterDataset;
                int BandCount = m_pRasterBandCollection.Count;
                for (int i = 0; i < BandCount; i++)
                {
                    IRasterBand band = m_pRasterBandCollection.Item(i);
                    comboBoxBand.Items.Add(band.Bandname);
                }
                comboBoxBand.SelectedIndex = 0;



            }
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (listBox1.SelectedIndex == 0)
            {
                panel3.Visible = true;
            }
            if (listBox1.SelectedIndex == 1)
            {
                panel3.Visible = false;
            }
        }

        public static IRasterDataset OpenFileRasterDataset(string fullpath)
        {
            IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(fullpath), 0);
            IRasterWorkspace pRasterWorkspace = (IRasterWorkspace)pWorkspace;
            IRasterDataset pRasterDataset = (IRasterDataset)pRasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(fullpath));
            return pRasterDataset;
        }

        private Image SymbolToBitmap(IGradientFillSymbol iSymbol, int iStyle, int iWidth, int iHeight)
        {
            IntPtr iHDC = new IntPtr();
            Bitmap iBitmap = new Bitmap(iWidth, iHeight);
            Graphics iGraphics = System.Drawing.Graphics.FromImage(iBitmap);
            tagRECT itagRECT;
            IEnvelope iEnvelope = new EnvelopeClass() as IEnvelope;
            IDisplayTransformation iDisplayTransformation;
            IPoint iPoint;
            IGeometryCollection iPolyline;
            IGeometryCollection iPolygon;
            IRing iRing;
            ISegmentCollection iSegmentCollection;
            IGeometry iGeometry = null;
            object Missing = Type.Missing;
            iEnvelope.PutCoords(0, 0, iWidth, iHeight);
            itagRECT.left = 0;
            itagRECT.right = iWidth;
            itagRECT.top = 0;
            itagRECT.bottom = iHeight;
            iDisplayTransformation = new DisplayTransformationClass();
            iDisplayTransformation.VisibleBounds = iEnvelope;
            iDisplayTransformation.Bounds = iEnvelope;
            iDisplayTransformation.set_DeviceFrame(ref itagRECT);
            iDisplayTransformation.Resolution = iGraphics.DpiX / 100000;
            iHDC = iGraphics.GetHdc();
            //获取Geometry;

            if (iSymbol is ESRI.ArcGIS.Display.IMarkerSymbol)
            {
                switch (iStyle)
                {
                    case 0:
                        iPoint = new ESRI.ArcGIS.Geometry.Point();
                        iPoint.PutCoords(iWidth / 2, iHeight / 2);
                        iGeometry = iPoint;
                        break;
                    default:
                        break;

                }
            }
            else
            {
                if (iSymbol is ESRI.ArcGIS.Display.ILineSymbol)
                {
                    iSegmentCollection = new ESRI.ArcGIS.Geometry.Path() as ISegmentCollection;
                    iPolyline = new ESRI.ArcGIS.Geometry.Polyline() as IGeometryCollection;
                    switch (iStyle)
                    {
                        case 0:
                            iSegmentCollection.AddSegment(CreateLine(0, iHeight / 2, iWidth, iHeight / 2) as ISegment, ref Missing, ref Missing);

                            iPolyline.AddGeometry(iSegmentCollection as IGeometry, ref Missing, ref Missing);
                            iGeometry = iPolyline as IGeometry;
                            break;

                        case 1:
                            iSegmentCollection.AddSegment(CreateLine(0, iHeight / 4, iWidth / 4, 3 * iHeight / 4) as ISegment, ref Missing, ref Missing);
                            iSegmentCollection.AddSegment(CreateLine(iWidth / 4, 3 * iHeight / 4, 3 * iWidth / 4, iHeight / 4) as ISegment, ref Missing, ref Missing);
                            iSegmentCollection.AddSegment(CreateLine(3 * iWidth / 4, iHeight / 4, iWidth, 3 * iHeight / 4) as ISegment, ref Missing, ref Missing);
                            iPolyline.AddGeometry(iSegmentCollection as IGeometry, ref Missing, ref Missing);
                            iGeometry = iPolyline as IGeometry;
                            break;
                        default:
                            break;
                    }
                }
                else
                    if (iSymbol is ESRI.ArcGIS.Display.IFillSymbol)
                    {
                        iSegmentCollection = new ESRI.ArcGIS.Geometry.Ring() as ISegmentCollection;
                        iPolygon = new ESRI.ArcGIS.Geometry.Polygon() as IGeometryCollection;
                        switch (iStyle)
                        {
                            case 0:
                                iSegmentCollection.AddSegment(CreateLine(5, iHeight - 5, iWidth - 6, iHeight - 5) as ISegment, ref Missing, ref Missing);
                                iSegmentCollection.AddSegment(CreateLine(iWidth - 6, iHeight - 5, iWidth - 6, 6) as ISegment, ref Missing, ref Missing);
                                iSegmentCollection.AddSegment(CreateLine(iWidth - 6, 6, 5, 6) as ISegment, ref Missing, ref Missing);
                                iRing = iSegmentCollection as IRing;
                                iRing.Close();
                                iPolygon.AddGeometry(iSegmentCollection as IGeometry, ref Missing, ref Missing);
                                iGeometry = iPolygon as IGeometry;
                                break;
                            default:
                                break;

                        }
                    }
                    else
                        if (iSymbol is ESRI.ArcGIS.Display.ISimpleTextSymbol)
                        {
                            switch (iStyle)
                            {
                                case 0:
                                    iPoint = new ESRI.ArcGIS.Geometry.Point();
                                    iPoint.PutCoords(iWidth / 2, iHeight / 2);
                                    iGeometry = iPoint;
                                    break;
                                default:
                                    break;
                            }
                        }

            }////////////////////////
            if (iGeometry == null)
            {
                MessageBox.Show("几何对象不符合！", "错误");
                return null;
            }
            ISymbol pOutputSymbol = iSymbol as ISymbol;
            pOutputSymbol.SetupDC(iHDC.ToInt32(), iDisplayTransformation);
            pOutputSymbol.Draw(iGeometry);
            pOutputSymbol.ResetDC();
            iGraphics.ReleaseHdc(iHDC);
            iGraphics.Dispose();
            return iBitmap;
        }

        private ILine CreateLine(double x1, double y1, double x2, double y2)
        {
            IPoint pnt1 = new PointClass();
            pnt1.PutCoords(x1, y1);
            IPoint pnt2 = new PointClass();
            pnt2.PutCoords(x2, y2);
            Line ln = new LineClass();
            ln.PutCoords(pnt1, pnt2);
            return ln;
        }

        private void DrawColorRamp()
        {
            string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;
            string styleFilePath = sInstall + "\\Styles\\ESRI.ServerStyle";
            IStyleGallery styleGallery = new ServerStyleGalleryClass();
            IStyleGalleryItem styleGalleryItem = null;
            IStyleGalleryStorage styleGalleryStorage = styleGallery as IStyleGalleryStorage;
            styleGalleryStorage.AddFile(styleFilePath);

            IEnumStyleGalleryItem enumStyleGalleryItem = styleGallery.get_Items("Color Ramps", styleFilePath, "");
            enumStyleGalleryItem.Reset();

            styleGalleryItem = enumStyleGalleryItem.Next();

            while (styleGalleryItem != null)
            {
                m_ColorRamp = (IColorRamp)styleGalleryItem.Item;
                EnumStyleItem.Add(m_ColorRamp);
                m_FillSymbol = new GradientFillSymbol();
                m_FillSymbol.GradientAngle = 0;
                m_FillSymbol.ColorRamp = m_ColorRamp;
                pictureBox1.Image = SymbolToBitmap(m_FillSymbol, 0, pictureBox1.Width, pictureBox1.Height);
                imageList1.Images.Add(pictureBox1.Image);
                comboBoxColor.Items.Add(pictureBox1.Image);
                styleGalleryItem = enumStyleGalleryItem.Next();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(enumStyleGalleryItem);
        }

        private void comboBoxColor_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            Rectangle iRectangle = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, 217, 14);
            Bitmap getBitmap = new Bitmap(imageList1.Images[e.Index]);
            e.Graphics.DrawImage(getBitmap, iRectangle);
        }

        private void comboBoxColor_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            pictureBox1.Image = comboBoxColor.SelectedItem as Image;
            pictureBox2.Image = comboBoxColor.SelectedItem as Image;
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.CommonSet();
            this.StretchRender();
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, System.EventArgs e)
        {
            this.CommonSet();
            this.StretchRender();
        }


    }
}