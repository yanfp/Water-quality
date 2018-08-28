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
    public partial class ClassRenderFrm : DevExpress.XtraEditors.XtraForm
    {
        public AxMapControl m_mapControl;
        public AxPageLayoutControl m_pageControl;

        public ILayer m_layer;
        public ILayer m_pageLayer;
        //实线色带用
        private ArrayList EnumStyleItem = new ArrayList();
        private IGradientFillSymbol m_FillSymbol;
        private IColorRamp m_ColorRamp;
        public ClassRenderFrm(AxMapControl mapControl, AxPageLayoutControl pageControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            m_pageControl = pageControl;

            DrawColorRamp();
            comboBoxColor.SelectedIndex = 21;
            pictureBox1.Image = comboBoxColor.SelectedItem as Image; 
        }

        private void ClassRenderFrm_Load(object sender, EventArgs e)
        {
            this.Left = 300; this.Top = 100;

            int LayerCount = m_mapControl.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                if (m_mapControl.get_Layer(i) is IRasterLayer)
                    comboBoxSelectRaster.Items.Add(m_mapControl.get_Layer(i).Name);
            }
            comboBoxClassValue.SelectedIndex = 3;

        }

        private void comboBoxSelectRaster_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_layer = this.GetSelectedRasterByName();
            m_pageLayer = this.GetPageLayerByName();
        }
        private ILayer GetSelectedRasterByName()
        {
            int LayerCount = m_mapControl.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                if (comboBoxSelectRaster.SelectedItem.ToString() == m_mapControl.get_Layer(i).Name)
                    return m_mapControl.get_Layer(i);
            }
            return null;
        }
        private ILayer GetPageLayerByName()
        {
            int LayerCount = m_pageControl.ActiveView.FocusMap.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                if (comboBoxSelectRaster.SelectedItem.ToString() == m_pageControl.ActiveView.FocusMap.get_Layer(i).Name)
                    return m_pageControl.ActiveView.FocusMap.get_Layer(i);
            }
            return null;
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
                imageList1.Images.Add(m_ColorRamp.Name, pictureBox1.Image);
                comboBoxColor.Items.Add(pictureBox1.Image);
                styleGalleryItem = enumStyleGalleryItem.Next();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(enumStyleGalleryItem);
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

        private void comboBoxColor_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            pictureBox1.Image = comboBoxColor.SelectedItem as Image;
            this.SetClassView();
        }

        private void comboBoxClassValue_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.SetClassView();
        }
        private void SetClassView()
        {
            listView1.Items.Clear();
            IColorRamp pColorRamp = (IColorRamp)EnumStyleItem[comboBoxColor.SelectedIndex];
            int ClassCount = comboBoxClassValue.SelectedIndex + 1;
            int ColorNumber = pColorRamp.Size;
            int Count = Math.Min(ClassCount, ColorNumber);
            if ((comboBoxClassValue.SelectedIndex + 1) > Count)
                comboBoxClassValue.SelectedIndex = Count - 1;

            for (int i = 0; i < Count; i++)
            {

                ListViewItem lv = new ListViewItem();
                int nclass = i + 1;
                lv.SubItems.Add("");
                int n1 = 255 / Count * i; int n2 = 255 / Count * (i + 1);
                lv.SubItems.Add(n1.ToString() + "---" + n2.ToString());
                lv.UseItemStyleForSubItems = false;
                lv.SubItems[0].BackColor = PageLayoutInsert.IColorToColor(pColorRamp.get_Color(i));
                lv.SubItems[1].BackColor = PageLayoutInsert.IColorToColor(pColorRamp.get_Color(i));
                listView1.Items.Add(lv);
            }

        }

        private void comboBoxColor_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            Rectangle iRectangle = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, 217, 14);
            Bitmap getBitmap = new Bitmap(imageList1.Images[e.Index]);
            e.Graphics.DrawImage(getBitmap, iRectangle);
        }
        //分级渲染函数
        private void RasterClassifyRender(IRasterLayer pRasterLayer)
        {
            try
            {
                IColorRamp pColorRamp = (IColorRamp)EnumStyleItem[comboBoxColor.SelectedIndex];

                IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRenderer() as IRasterClassifyColorRampRenderer;
                IRasterRenderer pRRend = pRClassRend as IRasterRenderer;

                IRaster pRaster = pRasterLayer.Raster;
                IRasterBandCollection pRBandCol = pRaster as IRasterBandCollection;
                IRasterBand pRBand = pRBandCol.Item(0);
                if (pRBand.Histogram == null)
                {
                    pRBand.ComputeStatsAndHist();
                }
                pRRend.Raster = pRaster;

                pRRend.Update();

                IRgbColor pFromColor = new RgbColor() as IRgbColor;
                pFromColor.Red = 255;
                pFromColor.Green = 0;
                pFromColor.Blue = 0;
                IRgbColor pToColor = new RgbColor() as IRgbColor;
                pToColor.Red = 0;
                pToColor.Green = 0;
                pToColor.Blue = 255;

                IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRamp() as IAlgorithmicColorRamp;

                //colorRamp = pColorRamp as IAlgorithmicColorRamp;///////
                colorRamp.Size = comboBoxClassValue.SelectedIndex + 1;

                //colorRamp.FromColor = pFromColor;
                //colorRamp.ToColor = pToColor;
                int nClass = comboBoxClassValue.SelectedIndex + 1;
                colorRamp.FromColor = pColorRamp.get_Color(0);
                colorRamp.ToColor = pColorRamp.get_Color(nClass - 1);
                bool createColorRamp;


                colorRamp.CreateRamp(out createColorRamp);

                IFillSymbol fillSymbol = new SimpleFillSymbol() as IFillSymbol;

                for (int i = 0; i < nClass; i++)
                {
                    fillSymbol.Color = colorRamp.get_Color(i);
                    pRClassRend.set_Symbol(i, fillSymbol as ISymbol);
                    pRClassRend.set_Label(i, pRClassRend.get_Break(i).ToString("0.00"));
                }
                pRasterLayer.Renderer = pRRend;
                m_mapControl.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show("创建失败！");
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (comboBoxSelectRaster.SelectedItem == null)
            {
                MessageBox.Show("栅格图层不能为空");
                return;
            }
            this.RasterClassifyRender((IRasterLayer)m_layer);
            this.RasterClassifyRender((IRasterLayer)m_pageLayer);
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, System.EventArgs e)
        {
            if (comboBoxSelectRaster.SelectedItem == null)
            {
                MessageBox.Show("栅格图层不能为空");
                return;
            }
            this.RasterClassifyRender((IRasterLayer)m_layer);
            this.RasterClassifyRender((IRasterLayer)m_pageLayer);
        }
    }
}