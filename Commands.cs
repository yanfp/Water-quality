using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace water_quality
{
    class Commands
    {
        //map移动
        public static void MapPanTool(AxMapControl mapControl)
        {
            ICommand command1 = new ControlsMapPanTool();
            command1.OnCreate(mapControl.Object);
            mapControl.CurrentTool = command1 as ITool;
        }
        //page移动
        public static void PagePanTool(AxPageLayoutControl pageLayoutControl)
        {
            ICommand command1 = new ControlsMapPanTool();
            command1.OnCreate(pageLayoutControl.Object);
            pageLayoutControl.CurrentTool = command1 as ITool;
        }
        //map放大
        public static void MapZoomInTool(AxMapControl mapControl)
        {
            ICommand command1 = new ControlsMapZoomInTool();
            command1.OnCreate(mapControl.Object);
            mapControl.CurrentTool = command1 as ITool;
 
        }
        //map缩小
        public static void MapZoomOutTool(AxMapControl mapControl)
        {
            ICommand command1 = new ControlsMapZoomOutTool();
            command1.OnCreate(mapControl.Object);
            mapControl.CurrentTool = command1 as ITool;

        }
        //page缩小
        public static void PageZoomOutTool(AxPageLayoutControl pageLayoutControl)
        {
            ICommand command1 = new ControlsMapZoomOutTool();
            command1.OnCreate(pageLayoutControl.Object);
            pageLayoutControl.CurrentTool = command1 as ITool;
        }
        //page放大
        public static void PageZoomInTool(AxPageLayoutControl pageLayoutControl)
        {
            ICommand command1 = new ControlsMapZoomInTool();
            command1.OnCreate(pageLayoutControl.Object);
            pageLayoutControl.CurrentTool = command1 as ITool;

        }
        //map缩放至图层
        public static void ZoomToLayer(AxMapControl mapControl)
        {
            mapControl.Extent = mapControl.FullExtent;
        }
        //page缩放至图层
        public static void ZoomToLayer(AxPageLayoutControl pageLayoutControl)
        {
            pageLayoutControl.Extent = pageLayoutControl.FullExtent;
        }
        //添加数据
        public static void AddData(AxMapControl mapControl)
        {
            ControlsAddDataCommand addDataCmd = new ControlsAddDataCommandClass();
            addDataCmd.OnCreate(mapControl.Object);
            addDataCmd.OnClick();
        }
        public static void AddData(AxPageLayoutControl pageLayoutControl)
        {
            ControlsAddDataCommand addDataCmd = new ControlsAddDataCommandClass();
            addDataCmd.OnCreate(pageLayoutControl.Object);
            addDataCmd.OnClick();
        }
        //选择元素
        public static void SelectElementTool(AxMapControl mapControl,AxPageLayoutControl pageLayoutControl)
        {
            ICommand pSelect1 = new ControlsSelectToolClass();
            pSelect1.OnCreate(mapControl.Object);
            mapControl.CurrentTool = pSelect1 as ITool;
            ICommand pSelect2 = new ControlsSelectToolClass();
            pSelect2.OnCreate(pageLayoutControl.Object);
            pageLayoutControl.CurrentTool = pSelect2 as ITool;
        }

        //箭头
        public static void Arrow(AxMapControl mapControl)
        {
            mapControl.CurrentTool = null;
        }
        public static void Arrow(AxPageLayoutControl pageLayoutControl)
        {
            pageLayoutControl.CurrentTool = null;
        }

    }
}
