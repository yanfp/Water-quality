using System;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using stdole;
using System.Drawing;



namespace water_quality
{
    /*用于向PageLayoutControl中插入图例、标题、指北针、文本等，并通过双击修改属性*/
    class PageLayoutInsert
    {
        //当前PageLayoutControl控件
        private AxPageLayoutControl m_pageLayoutControl;

        //构造函数
        public PageLayoutInsert()
        {
    
        }
        ///   <summary>   
        ///   设定当前PageLayoutControl  
        ///   </summary>  
        public void SetControls(AxPageLayoutControl pageLayoutControl)
        {
            m_pageLayoutControl = pageLayoutControl;
            this.Load();
        }

        private void Load()
        {
            m_pageLayoutControl.OnDoubleClick += new IPageLayoutControlEvents_Ax_OnDoubleClickEventHandler(m_pageLayoutControl_OnDoubleClick);
        }

        //设置当前工具（选择元素）
        private void SetCurrentTool()
        {
            ICommand pSelect = new ControlsSelectToolClass();
            pSelect.OnCreate(m_pageLayoutControl.Object);
            m_pageLayoutControl.CurrentTool = pSelect as ITool;
        }

        //插入标题
        public void InsertTitle()
        {
            this.SetCurrentTool();
            
            IActiveView pAV;
            IGraphicsContainer pGraphicsContainer;
            IPoint pPoint;
            ITextElement pTextElement;
            IElement pElement;
            ITextSymbol pTextSymbol;
            IRgbColor pColor;
            pAV = m_pageLayoutControl.ActiveView;
            pGraphicsContainer = m_pageLayoutControl.GraphicsContainer;
            pTextElement = new TextElementClass();

            IFontDisp pFont = new StdFontClass() as IFontDisp;
            pFont.Bold = true;
            pFont.Name = "宋体";
            pFont.Size = 23;

            pColor = new RgbColorClass();
            pColor.Red= 0;
            pColor.Blue = 0;
            pColor.Green = 0;

            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Color = (IColor)pColor;
            pTextSymbol.Font = pFont;
          
            pTextElement.Text = "新建图名";
            pTextElement.Symbol = pTextSymbol;

            pPoint = new PointClass();
            pPoint.X = 9;
            pPoint.Y = 25;

            pElement = (IElement)pTextElement;
            pElement.Geometry = (IGeometry)pPoint;
            pGraphicsContainer.AddElement(pElement, 0);

            pAV.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        //插入文本
        public void InsertText()
        {
            this.SetCurrentTool();

            IActiveView pAV;
            IGraphicsContainer pGraphicsContainer;
            IPoint pPoint;
            ITextElement pTextElement;
            IElement pElement;
            ITextSymbol pTextSymbol;
            IRgbColor pColor;
            pAV = m_pageLayoutControl.ActiveView;
            pGraphicsContainer = m_pageLayoutControl.GraphicsContainer;
            pTextElement = new TextElementClass();

            IFontDisp pFont = new StdFontClass() as IFontDisp;           
            pFont.Name = "宋体";
            pFont.Size = 23;

            pColor = new RgbColorClass();
            pColor.Red = 0;
            pColor.Blue = 0;
            pColor.Green = 0;

            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Color = (IColor)pColor;
            pTextSymbol.Font = pFont;

            pTextElement.Text = "新建文本";
            pTextElement.Symbol = pTextSymbol;

            pPoint = new PointClass();
            pPoint.X = 3;
            pPoint.Y = 15;

            pElement = (IElement)pTextElement;
            pElement.Geometry = (IGeometry)pPoint;
            pGraphicsContainer.AddElement(pElement, 0);

            pAV.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //插入图例
        public void InsertLegend()
        {
            this.SetCurrentTool();
            IGraphicsContainer graphicsContainer =this.m_pageLayoutControl.ActiveView.GraphicsContainer;
            
            //得到MapFrame对象
            IMapFrame mapFrame =(IMapFrame)graphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap);
            if (mapFrame == null)
                return;
            //生成一个图例
            UID uID = new UIDClass();
            uID.Value = "esriCarto.Legend";
            //从MapFrame中生成一个MapSurroundFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);
            if (mapSurroundFrame == null)
                return;
            if (mapSurroundFrame.MapSurround == null)
                return;
            //MapSurroundFrame名称
            mapSurroundFrame.MapSurround.Name = "图例";
            ILegend pleg;
            pleg = new Legend();
            pleg = mapSurroundFrame.MapSurround as ILegend;
            pleg.Title = "图例";
            //设置图例的显示范围
            IEnvelope envelop = new EnvelopeClass();
            envelop.PutCoords(3, 3, 13, 5);
            IElement element = (IElement)mapSurroundFrame;
            element.Geometry = envelop;
            
            //添加图例元素
            m_pageLayoutControl.ActiveView.GraphicsContainer.AddElement(element, 0);
            //PageLayOutControl
            m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            
        }
        //获取完整图例函数
        public ILegend GetCompleteLegend()
        {
            IGraphicsContainer graphicsContainer = this.m_pageLayoutControl.ActiveView.GraphicsContainer;

            //得到MapFrame对象
            IMapFrame mapFrame = (IMapFrame)graphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap);
            
            //生成一个图例
            UID uID = new UIDClass();
            uID.Value = "esriCarto.Legend";
            //从MapFrame中生成一个MapSurroundFrame
            IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame(uID, null);
            
            //MapSurroundFrame名称
            mapSurroundFrame.MapSurround.Name = "图例";
            ILegend pleg;
            pleg = new Legend();
            pleg = mapSurroundFrame.MapSurround as ILegend;
            pleg.Title = "图例";
            return pleg;
 
        }

        //插入指北针
        public void InsertNorthArrow()
        {
            SetCurrentTool();
            AddNorthArrowForm addNorthArrow = new AddNorthArrowForm();
            if (addNorthArrow.ShowDialog() == DialogResult.OK)
            {
                IStyleGalleryItem pStyleGalleryItemTemp = addNorthArrow.m_pStyleGalleryItem;
                if (pStyleGalleryItemTemp == null)
                {
                    return;
                }

                IMapFrame pMapframe = m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap) as IMapFrame;
                IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
                pMapSurroundFrame.MapFrame = pMapframe;
                pMapSurroundFrame.MapSurround = (IMapSurround)pStyleGalleryItemTemp.Item;
                //在pageLayout中根据名称查要Element，找到之后删除已经存在的指北针
                IElement pElement = m_pageLayoutControl.FindElementByName("NorthArrows");
                if (pElement != null)
                {
                    m_pageLayoutControl.ActiveView.GraphicsContainer.DeleteElement(pElement);  //删除已经存在的指北针
                }
                IEnvelope pEnvelope = new EnvelopeClass();
                pEnvelope.PutCoords(16, 24, 20, 28);

                pElement = (IElement)pMapSurroundFrame;
                pElement.Geometry = (IGeometry)pEnvelope;

                m_pageLayoutControl.ActiveView.GraphicsContainer.AddElement(pElement, 0);
                m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                
            }
        }

        //插入比例尺
        public void InsertScale()
        {
            this.SetCurrentTool();
            AddScaleForm scaleForm = new AddScaleForm();
            if (scaleForm.ShowDialog() == DialogResult.OK)
            {
                IStyleGalleryItem pStyleGalleryItem = scaleForm.m_pStyleGalleryItem;

                IMapFrame pMapframe = m_pageLayoutControl.ActiveView.GraphicsContainer.FindFrame(m_pageLayoutControl.ActiveView.FocusMap) as IMapFrame;
                IMapSurroundFrame pSurroundFrame = new MapSurroundFrameClass();
                pSurroundFrame.MapFrame = pMapframe;
                pSurroundFrame.MapSurround = (IMapSurround)pStyleGalleryItem.Item;
                //在pageLayout中根据名称查要Element，找到之后删除已经存在的比例尺
                IElement pelement = m_pageLayoutControl.FindElementByName("ScaleBars");
                if (pelement != null)
                {
                    m_pageLayoutControl.ActiveView.GraphicsContainer.DeleteElement(pelement);  //删除已经存在的指北针
                }

                IEnvelope pEnvelope = new EnvelopeClass();
                pEnvelope.PutCoords(12,2, 20, 3);

                pelement = (IElement)pSurroundFrame;
                pelement.Geometry = (IGeometry)pEnvelope;

                m_pageLayoutControl.ActiveView.GraphicsContainer.AddElement(pelement, 0);
                m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            }  
        }

        //鼠标双击被选中的元素时将出现属性对话框更改属性
        private void m_pageLayoutControl_OnDoubleClick(object sender, IPageLayoutControlEvents_OnDoubleClickEvent e)
        {
            IGraphicsContainer pSelected = m_pageLayoutControl.ActiveView as IGraphicsContainer;
            IPoint pt = new PointClass();
            pt.X = e.pageX; pt.Y = e.pageY;
            IEnumElement pEnumElement = pSelected.LocateElements(pt, 0);
            IElement pEle = pEnumElement.Next();

            if (pEle is IMapSurroundFrame)
            {
                IMapSurroundFrame pSurround = pEle as IMapSurroundFrame;
                if (pSurround.MapSurround is ILegend)
                {
                    LegendPropertyForm legendForm = new LegendPropertyForm(GetCompleteLegend(),pEle);
                    legendForm.ShowDialog();
                }
                if (pSurround.MapSurround is INorthArrow)
                {
                    NorthArrowPropertyFr nap = new NorthArrowPropertyFr(pEle);
                    nap.ShowDialog();                  
                }
                if (pSurround.MapSurround is IScaleBar)
                {
                    ScalePropertyFr scaleForm = new ScalePropertyFr(pEle);
                    scaleForm.ShowDialog();
                }
            }
            if (pEle is ITextElement)
            {
                TitlePropertyFr titleForm = new TitlePropertyFr(pEle as ITextElement,pt);
                titleForm.Text = "文本属性";
                titleForm.ShowDialog();
                m_pageLayoutControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
               
            }
        }

        #region 用于实现预览功能
        public static Image CreatePictureFromSymbol(ISymbol pSymbol, double dblWidth, double dblHeight, double dblGap)
        {
            Bitmap bitmap = new Bitmap((int)dblWidth, (int)dblHeight);
            Graphics gImage = Graphics.FromImage(bitmap);
            IntPtr hdc = new IntPtr();
            hdc = gImage.GetHdc();

            DrawToDC(hdc.ToInt32(), dblWidth, dblHeight, dblGap, pSymbol, false);

            gImage.ReleaseHdc(hdc);
            gImage.Dispose();
            return bitmap;


        }
        private static void DrawToDC(int hDC, double dblWidth, double dblHeight, double dblGap, ISymbol pSymbol, bool blnLine)
        {
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(dblGap, dblGap, dblWidth - dblGap, dblHeight - dblGap);
            ITransformation pTransformation = CreateTransFromDC(hDC, dblWidth, dblHeight);

            IGeometry pGeom = CreateSymShape(pSymbol, pEnvelope, blnLine);

            pSymbol.SetupDC(hDC, pTransformation);
            pSymbol.Draw(pGeom);
            pSymbol.ResetDC();
        }
        private static IGeometry CreateSymShape(ISymbol pSymbol, IEnvelope pEnvelope, bool blnLine)
        {
            if (pSymbol is IMarkerSymbol)
            {
                IArea pArea = pEnvelope as IArea;
                return pArea.Centroid;
            }
            else if (pSymbol is ILineSymbol || pSymbol is ITextSymbol)
            {
                if (blnLine)
                {
                    IPointCollection pPC = new PolylineClass();

                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(pEnvelope.XMin, pEnvelope.YMax);
                    object obj = Type.Missing;
                    pPC.AddPoint(pPoint, ref obj, ref obj);

                    pPoint = new PointClass();
                    pPoint.PutCoords(pEnvelope.XMin + pEnvelope.Width / 3, pEnvelope.YMin);
                    obj = Type.Missing;
                    pPC.AddPoint(pPoint, ref obj, ref obj);

                    pPoint = new PointClass();
                    pPoint.PutCoords(pEnvelope.XMax - pEnvelope.Width / 3, pEnvelope.YMax);
                    obj = Type.Missing;
                    pPC.AddPoint(pPoint, ref obj, ref obj);

                    pPoint = new PointClass();
                    pPoint.PutCoords(pEnvelope.XMax, pEnvelope.YMin);
                    obj = Type.Missing;
                    pPC.AddPoint(pPoint, ref obj, ref obj);

                    return pPC as IPolyline;
                }
                else
                {
                    IPolyline pPolyline = new PolylineClass();
                    IPoint pFromPoint = new PointClass();
                    pFromPoint.PutCoords(pEnvelope.XMin, pEnvelope.YMin + pEnvelope.Height / 2);

                    IPoint pToPoint = new PointClass();
                    pToPoint.PutCoords(pEnvelope.XMax, pEnvelope.YMin + pEnvelope.Height / 2);

                    pPolyline.FromPoint = pFromPoint;
                    pPolyline.ToPoint = pToPoint;
                    return pPolyline;
                }
            }
            else
                return pEnvelope;
        }
        private static ITransformation CreateTransFromDC(int hDC, double dblWidth, double dblHeight)
        {
            IEnvelope pBoundsEnvelope = new EnvelopeClass();
            pBoundsEnvelope.PutCoords(0, 0, dblWidth, dblHeight);

            tagRECT deviceRect = new tagRECT();
            deviceRect.left = 0;
            deviceRect.top = 0;
            deviceRect.right = (int)dblWidth;
            deviceRect.bottom = (int)dblHeight;

            IDisplayTransformation pDisplayTransformation = new DisplayTransformationClass();
            pDisplayTransformation.VisibleBounds = pBoundsEnvelope;
            pDisplayTransformation.Bounds = pBoundsEnvelope;
            pDisplayTransformation.set_DeviceFrame(ref deviceRect);
            pDisplayTransformation.Resolution = 96;

            return pDisplayTransformation as ITransformation;
        }
        #endregion

        #region 用于颜色对象转换
        public static Color IColorToColor(IColor pColor)
        {

            int R = pColor.RGB & 0xff;
            int G = (pColor.RGB & 0xff00) / 0x100;
            int B = (pColor.RGB & 0xff0000) / 0x10000;
            Color color = Color.FromArgb(R, G, B);
            return color;
        }
        public static IColor ColorToIColor(Color color)
        {
            IColor pColor = new RgbColorClass();
            pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            return pColor;
        }
        #endregion
    }
}
