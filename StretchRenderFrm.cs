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
    public partial class StretchRenderFrm : DevExpress.XtraEditors.XtraForm
    {
        AxMapControl m_mapControl;
        AxPageLayoutControl m_pageControl;

        public ILayer m_layer;
        public ILayer m_pageLayer;

        private IRasterDataset m_pRasterDataset = null;
        private IRasterBandCollection m_pRasterBandCollection = null;

        //实现色带用
        private ArrayList EnumStyleItem = new ArrayList();
        private IGradientFillSymbol m_FillSymbol;
        private IColorRamp m_ColorRamp;

        public StretchRenderFrm(AxMapControl mapControl, AxPageLayoutControl pageControl)
        {
            InitializeComponent();
            //成员变量赋值
            m_mapControl = mapControl;
            m_pageControl = pageControl;
            //初始化色带
            DrawColorRamp();
            comboBoxColor.SelectedIndex = 21;
            pictureBox1.Image = comboBoxColor.SelectedItem as Image;
            pictureBox2.Image = pictureBox1.Image;
        }

        private void StretchRenderFrm_Load(object sender, System.EventArgs e)
        {
            this.Left = 300; this.Top = 100;
            int LayerCount = m_mapControl.LayerCount;
            for (int i = 0; i < LayerCount; i++)
            {
                if (m_mapControl.get_Layer(i) is IRasterLayer)
                    comboBoxSelectRaster.Items.Add(m_mapControl.get_Layer(i).Name);
            }
            comboBoxStretchType.SelectedIndex = 0;
        }
        //打开栅格函数
        public static IRasterDataset OpenFileRasterDataset(string fullpath)
        {
            IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(fullpath), 0);
            IRasterWorkspace pRasterWorkspace = (IRasterWorkspace)pWorkspace;
            IRasterDataset pRasterDataset = (IRasterDataset)pRasterWorkspace.OpenRasterDataset(System.IO.Path.GetFileName(fullpath));
            return pRasterDataset;
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

        private void comboBoxSelectRaster_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_layer = this.GetSelectedRasterByName();
            m_pageLayer = this.GetPageLayerByName();
            this.AddBand();
            textBoxMax.Text = "255";
            textBoxMin.Text = "0";
            //if (this.AddBand() > 1)
                //this.SetMaxMinValue(1);
            //else
               // this.SetMaxMinValue(0);
        }

        private void comboBoxStretchType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (comboBoxStretchType.SelectedIndex != 0)
            {
                label8.Visible = false; textBoxStandardValue.Visible = false;
            }
            else
            {
                label8.Visible = true; textBoxStandardValue.Visible = true;
            }
        }
        //选择当前栅格函数
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
        //添加波段函数
        private int AddBand()
        {
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
            return BandCount;
        }
        //执行拉伸渲染函数
        private void StretchRender(ILayer layer)
        {
            if (comboBoxBand.SelectedItem == null)
            {
                MessageBox.Show("波段不能为空！");
                return;
            }
            IColorRamp pColorRamp = (IColorRamp)EnumStyleItem[comboBoxColor.SelectedIndex];
            IRasterLayer pRasterLayr = layer as IRasterLayer;
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

            m_mapControl.Refresh();

        }
        //获取最大最小栅格值
        private void SetMaxMinValue(int flag)
        {
            if (flag == 1)
            {
                textBoxMax.Text = "高：255";
                textBoxMin.Text = "低：0";
                return;
            }
            uint valueMax = 50;
            uint valueMin = 50;

            IRasterLayer pRasterLayer = m_layer as IRasterLayer;
            IRaster pRaster = pRasterLayer.Raster;
            IRasterProps pRasterProps = pRaster as IRasterProps;
            int Height = pRasterProps.Height;
            int Width = pRasterProps.Width;
            double dX = pRasterProps.MeanCellSize().X;
            double dY = pRasterProps.MeanCellSize().Y; //栅格的高度
            IEnvelope extent = pRasterProps.Extent; //当前栅格数据集的范围
            rstPixelType pixelType = pRasterProps.PixelType; //当前栅格像素类型
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(dX, dY);


            IPixelBlock pixelBlock = pRaster.CreatePixelBlock(pntSize);
            IPnt pnt = new PntClass();
            for (int i = 0; i < Height; i += 10)
                for (int j = 0; j < Width; j += 10)
                {
                    pnt.SetCoords(i, j);
                    pRaster.Read(pnt, pixelBlock);
                    if (pixelBlock != null)
                    {
                        object obj = pixelBlock.GetVal(0, 0, 0);
                        uint temp = Convert.ToUInt32(obj);

                        if (temp > valueMax)
                            valueMax = temp;
                        else if (temp < valueMin)
                            valueMin = temp;
                    }
                }
            textBoxMax.Text = "高：" + valueMax.ToString();
            textBoxMin.Text = "低：" + valueMin.ToString();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (comboBoxSelectRaster.SelectedItem == null)
            {
                MessageBox.Show("栅格图层不能为空");
                return;
            }
            this.StretchRender(m_layer);
            this.StretchRender(m_pageLayer);
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
            this.StretchRender(m_layer);
            this.StretchRender(m_pageLayer);
        }
    }
}