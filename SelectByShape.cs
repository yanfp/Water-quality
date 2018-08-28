using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;

namespace water_quality
{
    class SelectByShape
    {
        AxMapControl m_mapControl;
        string m_strSelectType = "";

        public string SelectType
        {
            get 
            {
                return m_strSelectType;    
            }
            set
            {
                m_strSelectType = value;
            }
        }

        public SelectByShape(AxMapControl mapControl)
        {
            m_mapControl = mapControl;
            this.Load();
        }
        private void Load()
        {
            m_mapControl.OnMouseDown+=new IMapControlEvents2_Ax_OnMouseDownEventHandler(m_mapControl_OnMouseDown);
        }

        private void m_mapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1)
            {
                switch (SelectType)
                {                  
                    case "Select by rectangle":
                        this.RectangleSelect(m_mapControl);
                        break;
                    case "Select by polygon":
                        this.PolygonSelect(m_mapControl);
                        break;
                    case "Select by circle":
                        this.CircleSelect(m_mapControl);
                        break;
                    case "Select by line":
                        this.LineSelect(m_mapControl);
                        break;
                }
            }

        }

        private IRgbColor pColor;

        public void init()
        {
            pColor = new RgbColor();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
        }



        public void RectangleSelect(AxMapControl mapControl)
        {
            this.init();
            IEnvelope pEnv;
            pEnv = mapControl.TrackRectangle();
            //新建选择集对象
            ISelectionEnvironment pSelectionEnvR;
            pSelectionEnvR = new SelectionEnvironmentClass();
            //改变选择集的默认颜色
            pSelectionEnvR.DefaultColor = pColor;
            //选择要素,并将其放入选择集
            mapControl.Map.SelectByShape(pEnv, pSelectionEnvR, false);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,
                null, null);
        }
        public void PolygonSelect(AxMapControl mapControl)
        {
            this.init();
            IGeometry pGeometryP;

            pGeometryP = mapControl.TrackPolygon();
            //新建选择集对象
            ISelectionEnvironment pSelectionEnvP;
            pSelectionEnvP = new SelectionEnvironmentClass();
            //改变选择集的默认颜色
            pSelectionEnvP.DefaultColor = pColor;
            //选择要素,并将其放入选择集
            mapControl.Map.SelectByShape(pGeometryP, pSelectionEnvP, false);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,
                null, null);
        }
        public void CircleSelect(AxMapControl mapControl)
        {
            this.init();
            IGeometry pGeometryC;

            pGeometryC = mapControl.TrackCircle();

            //新建选择集对象
            ISelectionEnvironment pSelectionEnvC;
            pSelectionEnvC = new SelectionEnvironmentClass();
            //改变选择集的默认颜色
            pSelectionEnvC.DefaultColor = pColor;
            //选择要素,并将其放入选择集
            mapControl.Map.SelectByShape(pGeometryC, pSelectionEnvC, false);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,
                null, null);
        }
        public void LineSelect(AxMapControl mapControl)
        {
            this.init();
            IGeometry pGeometryL;

            pGeometryL = mapControl.TrackLine();

            //新建选择集对象
            ISelectionEnvironment pSelectionEnvL;
            pSelectionEnvL = new SelectionEnvironmentClass();
            //改变选择集的默认颜色
            pSelectionEnvL.DefaultColor = pColor;
            //选择要素,并将其放入选择集
            mapControl.Map.SelectByShape(pGeometryL, pSelectionEnvL, false);
            mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,
                null, null);
        }
    }
}
