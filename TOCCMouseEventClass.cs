using System;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

namespace water_quality
{
    
    class TOCCMouseEventClass
    {
        private AxTOCControl m_tocControl;
        private AxMapControl m_mapControl;
        private IToolbarMenu m_menuLayer=new ToolbarMenuClass();
        private IToolbarMenu m_menuMap = new ToolbarMenuClass();
        private ContextMenuStrip m_contextMemuLayer = new ContextMenuStrip();
        private ILayer  m_CurrentLayer=null;

        //标注要素
        private IGeoFeatureLayer m_pGeoFeaLayer = null;

        #region 菜单项
        ToolStripMenuItem iCopy = new ToolStripMenuItem("复制");
        ToolStripMenuItem iRemoveLayer = new ToolStripMenuItem("移除图层");
        ToolStripMenuItem iOpenTable = new ToolStripMenuItem("打开属性表");
        ToolStripMenuItem iZoomToLayer = new ToolStripMenuItem("缩放至图层");
        ToolStripMenuItem iVisible = new ToolStripMenuItem("可见隐藏");
        ToolStripMenuItem iLabel = new ToolStripMenuItem("标注要素");
        ToolStripMenuItem iData = new ToolStripMenuItem("数据");
        ToolStripMenuItem iAttribute = new ToolStripMenuItem("属性");
        #endregion

        private ILayer returnLayer(ref ILayer layer)
        {
            return layer;
 
        }
        //构造函数
        public TOCCMouseEventClass()
        {                        
        }       
        //绑定控件
        public void SetControls(AxTOCControl tocControl, AxMapControl mapControl)
        {
            m_tocControl = tocControl;
            m_mapControl = mapControl;
            this.Load();
        }
        //初始化函数
        public void Load()
        {
            #region 使用IToolbarMenu菜单
            m_tocControl.OnMouseDown+=new ITOCControlEvents_Ax_OnMouseDownEventHandler(m_tocControl_OnMouseDown);
            m_tocControl.OnDoubleClick+=new ITOCControlEvents_Ax_OnDoubleClickEventHandler(m_tocControl_OnDoubleClick); 
            
            m_menuLayer.SetHook((IMapControl3)m_mapControl.Object);
            m_menuMap.SetHook((IMapControl3)m_mapControl.Object);

            //添加数据菜单
            m_menuMap.AddItem(new ControlsAddDataCommandClass(), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            //打开全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 1, 1, false, esriCommandStyles.esriCommandStyleTextOnly);
            //关闭全部图层菜单
            m_menuMap.AddItem(new LayerVisibility(), 2, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            //以二级菜单的形式添加内置的“选择”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 3, true);
            //以二级菜单的形式添加内置的“地图浏览”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsMapViewMenu", 4, true);
            #endregion

            #region 加载ContextMenu菜单项
            //复制命令
            string str = System.IO.Directory.GetParent(System.IO.Directory.GetParent(Application.StartupPath).FullName).FullName;           
            iCopy.Click+=new EventHandler(iCopy_Click);
            //移除图层          
            iRemoveLayer.Image = Image.FromFile(str + "\\Resources\\RemoveLayer.bmp");
            //iRemoveLayer.Image=Image.FromFile(str+"\\RemoveLayer.bmp");
            iRemoveLayer.Click+=new EventHandler(iRemoveLayer_Click);
            //打开属性表
            iOpenTable.Image = Image.FromFile(str + "\\Resources\\OpenAttributeTable.bmp");
            //iOpenTable.Image=Image.FromFile(str+"\\OpenAttributeTable.bmp");
            iOpenTable.Click+=new EventHandler(iOpenTable_Click);
            //缩放至图层         
            iZoomToLayer.Image = Image.FromFile(str + "\\Resources\\ZoomToLayer.bmp");
            //iZoomToLayer.Image=Image.FromFile(str+"\\ZoomToLayer.bmp");
            iZoomToLayer.Click+=new EventHandler(iZoomToLayer_Click);
            //缩放至可见            
            iVisible.Click+=new EventHandler(iVisible_Click);
            //标注要素           
            iLabel.Click+=new EventHandler(iLabel_Click);
            //数据            
            iData.Click+=new EventHandler(iData_Click);
            //属性           
            iAttribute.Click+=new EventHandler(iAttribute_Click);

            m_contextMemuLayer.Items.Add(iCopy);//0
            m_contextMemuLayer.Items.Add(iRemoveLayer);//1
            m_contextMemuLayer.Items.Add(iOpenTable);//2
            m_contextMemuLayer.Items.Add(iZoomToLayer);//3
            m_contextMemuLayer.Items.Add(iVisible);//4  
            m_contextMemuLayer.Items.Add(iLabel);//5
            //m_contextMemuLayer.Items[5].Enabled = false;
            m_contextMemuLayer.Items.Add(iData);//6
            m_contextMemuLayer.Items[6].Enabled = false;
            m_contextMemuLayer.Items.Add(iAttribute);//7
            
            #endregion

        }

        #region 使用ContextMemuStrip
        private void iCopy_Click(object sender, EventArgs e)
        {
           
        }
        private void iRemoveLayer_Click(object sender, EventArgs e)
        {
            RemoveLayer r = new RemoveLayer();
            r.MyClick((IMapControl3)m_mapControl.Object);
        }
        private void iOpenTable_Click(object sender, EventArgs e)
        {
            OpenAttributeTable o = new OpenAttributeTable();
            o.MyClick((IMapControl3)m_mapControl.Object);
        }
        private void iZoomToLayer_Click(object sender, EventArgs e)
        {
            ZoomToLayer z = new ZoomToLayer();
            z.MyClick((IMapControl3)m_mapControl.Object);
        }
        private void iVisible_Click(object sender, EventArgs e)
        {
            if (m_CurrentLayer.Visible)
                m_CurrentLayer.Visible = false;
            else
                m_CurrentLayer.Visible = true;
            m_tocControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }
        private void iLabel_Click(object sender, EventArgs e)
        {
            if (m_pGeoFeaLayer != null)
            { 
                if (m_pGeoFeaLayer.DisplayAnnotation)
                    m_pGeoFeaLayer.DisplayAnnotation = false;
                else
                    m_pGeoFeaLayer.DisplayAnnotation = true;
            }
            m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);        
        }
        private void iData_Click(object sender, EventArgs e)
        {
        }
        private void iAttribute_Click(object sender, EventArgs e)
        {
            Attribute af = new Attribute();
            af.MyClick((IMapControl3)m_mapControl.Object);
            m_tocControl.Refresh();
            m_tocControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);    
        }
        #endregion


        //添加菜单
        public void LoadMenu()
        {
            m_menuLayer.AddItem(new ZoomToLayer(), 0, -1, true, esriCommandStyles.esriCommandStyleIconAndText);
            m_menuLayer.AddItem(new RemoveLayer(), 1, -1, true, esriCommandStyles.esriCommandStyleIconAndText);            
            m_menuLayer.AddItem(new OpenAttributeTable(), 2, -1, true, esriCommandStyles.esriCommandStyleIconOnly);
            m_menuLayer.AddItem(new Attribute(), 3, -1, true, esriCommandStyles.esriCommandStyleMenuBar);
           
            
        }

        public void ChangeItemText()
        {
            if (m_pGeoFeaLayer!= null)
            {
                if (m_pGeoFeaLayer.DisplayAnnotation)
                    iLabel.Text = "隐藏标注";
                else
                    iLabel.Text = "标注要素";
            }
            if (m_CurrentLayer.Visible == true)
                iVisible.Text = "关闭图层";
            else
                iVisible.Text = "缩放至可见";
            if (m_CurrentLayer is IRasterLayer)
            {
                iOpenTable.Enabled = false;
                iLabel.Enabled = false;
            }
            else
            {
                iOpenTable.Enabled = true;
                iLabel.Enabled = true;
            }
        }
        
        //TOCControl控件的鼠标单击事件
        public void m_tocControl_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {               
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            //判断所选菜单的类型
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            
            if (e.button == 1)
            {
                if (item == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    try
                    {
                        ESRI.ArcGIS.Carto.ILegendClass pLC = new LegendClassClass();
                        ESRI.ArcGIS.Carto.ILegendGroup pLG = new LegendGroupClass();
                        if (other is ILegendGroup)
                        {
                            pLG = (ILegendGroup)other;
                        }
                        pLC = pLG.get_Class((int)index);
                        ISymbol pSym;
                        pSym = pLC.Symbol;


                        ISymbolSelector pSS = new SymbolSelectorClass();

                        bool bOK = false;
                        pSS.AddSymbol(pSym);
                        bOK = pSS.SelectSymbol(0);
                        if (bOK)
                        {
                            pLC.Symbol = pSS.GetSymbolAt(0);
                        }
                        this.m_mapControl.ActiveView.Refresh();
                        this.m_tocControl.Refresh();
                    }
                    catch (Exception exce)
                    { }
                }
            }
            if (e.button == 2)
            {
                //设置CustomProperty为layer (用于自定义的Layer命令)                  
                m_mapControl.CustomProperty = layer;
                //弹出右键菜单
                if (item == esriTOCControlItem.esriTOCControlItemMap)
                {
                    m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                }
                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    #region
                    /*                 this.LoadMenu(layer);                   
                    m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                    m_menuLayer.RemoveAll();      */
                    #endregion
                    m_CurrentLayer = layer;
                    m_pGeoFeaLayer = m_CurrentLayer as IGeoFeatureLayer;
                    this.ChangeItemText();
                    Rectangle rect=new Rectangle();
                    rect=m_tocControl.RectangleToScreen(rect);               
                    m_contextMemuLayer.Show(e.x+rect.Left, e.y+rect.Top);
                    
                }
            }
            
        }
        //TOCControl控件的鼠标双击事件
        public void m_tocControl_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {
            if (e.button == 1)
            {
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                object other = null;
                object index = null;
                //判断所选菜单的类型
                m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);

                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {                    
 /*                   AttributeForm af = new AttributeForm((IMapControl3)m_mapControl);
                    af.m_layer = layer;
                    af.ShowDialog();*/

                   // iAttribute_Click(null, null);
                }

                this.m_mapControl.ActiveView.Refresh();
                this.m_tocControl.Refresh();
            }
            
        }
        

        public AxMapControl returnMapControl()
        {
            return m_mapControl;
        }
        public AxTOCControl returnTOCControl()
        {
            return m_tocControl;
        }

        
    }
}


#region 继承于BaseCommand的类
namespace water_quality
{
    #region Layer菜单
    public sealed class ZoomToLayer : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;

        public ZoomToLayer()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text
            base.m_caption = "缩放至图层";  //localizable text
            base.m_message = "";  //localizable text 
            base.m_toolTip = "";  //localizable text 
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add ZoomToLayer.OnClick implementation
            IMapControl3 mapControl3 = m_hookHelper.Hook as IMapControl3;
            ILayer layer = (ILayer)mapControl3.CustomProperty;
            mapControl3.Extent = layer.AreaOfInterest;
        }

        #endregion

        public void MyClick(IMapControl3 ImapControl3)
        {
            this.OnCreate(ImapControl3);
            this.OnClick();
        }
    }

    public sealed class RemoveLayer : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;

        public RemoveLayer()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text
            base.m_caption = "移除图层";  //localizable text
            base.m_message = "";  //localizable text 
            base.m_toolTip = "";  //localizable text 
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add RemoveLayer.OnClick implementation
            IMapControl3 mapControl3 = m_hookHelper.Hook as IMapControl3;
            ILayer layer = (ILayer)mapControl3.CustomProperty;
            mapControl3.Map.DeleteLayer(layer);
        }

        #endregion

        public void MyClick(IMapControl3 ImapControl3)
        {
            this.OnCreate(ImapControl3);
            this.OnClick();
        }
    }

    public sealed class OpenAttributeTable : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;

        public OpenAttributeTable()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text
            base.m_caption = "打开属性表";  //localizable text
            base.m_message = "";  //localizable text 
            base.m_toolTip = "";  //localizable text 
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add OpenAttributeTable.OnClick implementation
            IMapControl3 mapControl3 = m_hookHelper.Hook as IMapControl3;
            ILayer layer = (ILayer)mapControl3.CustomProperty;
            AttributeTableFrom atf = new AttributeTableFrom();
            atf.CreateAttributeTable(layer);
            atf.ShowDialog();
        }

        #endregion

        public void MyClick(IMapControl3 ImapControl3)
        {
            this.OnCreate(ImapControl3);
            this.OnClick();
        }
    }

    public class Attribute : BaseCommand
    {
        private IMapControl3 m_mapControl;

        public Attribute()
        {
            base.m_caption = "属性";

        }
        public override void OnClick()
        {
            AttributeForm af = new AttributeForm(m_mapControl);
            af.m_layer = (ILayer)m_mapControl.CustomProperty;
            af.ShowDialog();
        }
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl3)hook;
        }

        public void MyClick(IMapControl3 ImapControl3)
        {
            this.OnCreate(ImapControl3);
            this.OnClick();
        }
    }

    public class IsVisible : BaseCommand
    {
        private IMapControl3 m_mapControl3;

        public IsVisible()
        { }
        public override void OnClick()
        {
            ILayer layer = (ILayer)m_mapControl3.CustomProperty;
            if (layer.Visible)
                layer.Visible = false;
            else
                layer.Visible = true;
        }
        public override void OnCreate(object hook)
        {
            m_mapControl3 = (IMapControl3)hook;
        }
        public void MyClick(IMapControl3 ImapControl3)
        {
            this.OnCreate(ImapControl3);
            this.OnClick();
        }
    }
    #endregion


    #region Map菜单
    public sealed class LayerVisibility : BaseCommand, ICommandSubType
    {
        private IHookHelper m_hookHelper = new HookHelperClass();
        private long m_subType;
        public LayerVisibility()
        {
        }

        public override void OnClick()
        {
            for (int i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
            {
                if (m_subType == 1) m_hookHelper.FocusMap.get_Layer(i).Visible = true;
                if (m_subType == 2) m_hookHelper.FocusMap.get_Layer(i).Visible = false;
            }
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        public override void OnCreate(object hook)
        {
            m_hookHelper.Hook = hook;
        }

        public int GetCount()
        {
            return 2;
        }

        public void SetSubType(int SubType)
        {
            m_subType = SubType;
        }

        public override string Caption
        {
            get
            {
                if (m_subType == 1) return "打开所有图层";
                else return "关闭所有图层";
            }
        }

        public override bool Enabled
        {
            get
            {
                bool enabled = false; int i;
                if (m_subType == 1)
                {
                    for (i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
                    {
                        if (m_hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == false)
                        {
                            enabled = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
                    {
                        if (m_hookHelper.ActiveView.FocusMap.get_Layer(i).Visible == true)
                        {
                            enabled = true;
                            break;
                        }
                    }
                }
                return enabled;
            }
        }
    }
    #endregion
}
#endregion