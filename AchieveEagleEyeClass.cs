using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using System.Drawing;

namespace water_quality
{
    class AchieveEagleEyeClass
    {
        private AxMapControl m_mapControlEye;
        private AxMapControl m_mapControlMain;
        public IRgbColor m_color=new RgbColorClass();
        public IActiveView pAv;
        public AchieveEagleEyeClass()
        { }

        ///   <summary>   
        ///   绑定鹰眼控件和主控件   
        ///   </summary>      
        public void SetControls(AxMapControl mapControlEye,AxMapControl mapControlMain)
        {
            m_mapControlEye = mapControlEye;
            m_mapControlMain = mapControlMain;
            this.Load();
        }

        private void Load()
        {
            m_mapControlEye.OnMouseDown+=new IMapControlEvents2_Ax_OnMouseDownEventHandler(m_mapControl_OnMouseDown);
            m_mapControlEye.OnMouseMove+=new IMapControlEvents2_Ax_OnMouseMoveEventHandler(m_mapControl_OnMouseMove);
            m_mapControlEye.OnExtentUpdated+=new IMapControlEvents2_Ax_OnExtentUpdatedEventHandler(m_mapControlEye_OnExtentUpdated);
            m_mapControlMain.OnMapReplaced+=new IMapControlEvents2_Ax_OnMapReplacedEventHandler(m_mapControlMain_OnMapReplaced);
            m_mapControlMain.OnExtentUpdated+=new IMapControlEvents2_Ax_OnExtentUpdatedEventHandler(m_mapControlMain_OnExtentUpdated);
            
            m_color.Red = 0;
            m_color.Green = 0;
            m_color.Blue = 0;
            m_color.Transparency = 225;
        }

        public void SetColor(Color color)
        {
            m_color = (IRgbColor)PageLayoutInsert.ColorToIColor(color);
        }

        private void m_mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            AchieveEagleEyeClass.RectangleClick(m_mapControlMain,m_mapControlEye,e);
        }

        private void m_mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            AchieveEagleEyeClass.RectangleMove(m_mapControlMain, e);
        }
        private void m_mapControlEye_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            AchieveEagleEyeClass.KeepExtent(m_mapControlMain, m_mapControlEye);
        }
        private void m_mapControlMain_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            AchieveEagleEyeClass.AddEagleEye(m_mapControlMain, m_mapControlEye);
        }

        private void m_mapControlMain_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            this.AddRedRectangle(m_mapControlEye, e);
        }

        //在主控件的OnMapReplaced事件中使用
        public static void AddEagleEye(AxMapControl mapControl1,AxMapControl mapControl2)
        {
            mapControl2.Map = new MapClass();
            for (int i = 0; i < mapControl1.LayerCount; i++)
            {
                mapControl2.Map.AddLayer(mapControl1.get_Layer(i));
            }
            mapControl2.Extent = mapControl1.FullExtent;
            mapControl2.Refresh();
        }
        //在主控件的OnExtendUpdate中用
        public void AddRedRectangle(AxMapControl mapControl2,IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = mapControl2.Map as IGraphicsContainer;
            pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = m_color;
            // 设置颜色属性
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 255;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        //在鹰眼控件的OnMouseDown里使用
        public static void RectangleClick(AxMapControl mapControl1,AxMapControl mapControl2,IMapControlEvents2_OnMouseDownEvent e)
        {
            if (mapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = mapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    mapControl1.Extent = pEnvelope;
                    mapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = mapControl2.TrackRectangle();
                    mapControl1.Extent = pEnvelop;
                    mapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }
        //在鹰眼控件的OnMouseMove中使用
        public static void RectangleMove(AxMapControl mapControl1,IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            mapControl1.CenterAt(pPoint);
            mapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        public static void KeepExtent(AxMapControl mapControl1, AxMapControl mapControl2)
        {
            mapControl2.Extent = mapControl1.FullExtent;
            mapControl1.Refresh();
        }
    }
}
