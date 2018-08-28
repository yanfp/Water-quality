using System;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;


namespace water_quality
{
    public partial class mainform : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public mainform()
        {
            InitializeComponent();
        }
        //private IAoInitialize m_AoInitialize = new AoInitializeClass();
        #region 变量定义
        TOCCMouseEventClass m_toccMouse = new TOCCMouseEventClass();
        PageLayoutInsert m_pageLayoutInsert = new PageLayoutInsert();
        AchieveEagleEyeClass m_achieveEagleEye = new AchieveEagleEyeClass();
        private string sMapUnits;
        SelectByShape m_SelectByShape;
        public IRgbColor GetRGB(int a, int b, int c)
        {
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = a;
            pRgbColor.Green = b;
            pRgbColor.Blue = c;
            return pRgbColor;
        }
        #endregion
        private void mainform_Load(object sender, EventArgs e)
        {
            //#region 判断产品是否有效
            //// 创建新的AoInitialize对象

            //if (m_AoInitialize == null)
            //{

            //    System.Windows.Forms.MessageBox.Show(

            //        "初始化失败，程序不能运行！");

            //    this.Close();

            //}



            //// 判断产品是否有效

            //esriLicenseStatus licenseStatus = (esriLicenseStatus)

            //    m_AoInitialize.IsProductCodeAvailable(

            //    esriLicenseProductCode.esriLicenseProductCodeEngine);

            //if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
            //{

            //    licenseStatus = (esriLicenseStatus)

            //        m_AoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);

            //    if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
            //    {

            //        System.Windows.Forms.MessageBox.Show(

            //            "初始化失败，应用程序不能运行！");

            //        this.Close();

            //    }

            //}

            //else
            //{

            //    System.Windows.Forms.MessageBox.Show(

            //        "ArcGIS Engine产品无效，此程序不能运行！");

            //    this.Close();

            //}
            //#endregion

            //this.Show();
            //m_pageLayoutInsert.SetControls(axPageLayoutControl1);
            //m_toccMouse.SetControls(axTOCControl1, axMapControl1);
            //m_achieveEagleEye.SetControls(axMapControl2, axMapControl1);
            //sMapUnits = "Unknown";
            this.Hide();
            login lg2 = new login();
            lg2.ShowDialog();
            if (lg2.res == 1)
            {
                this.Show();
                m_pageLayoutInsert.SetControls(axPageLayoutControl1);
                m_toccMouse.SetControls(axTOCControl1, axMapControl1);
                m_achieveEagleEye.SetControls(axMapControl2, axMapControl1);
                m_SelectByShape = new SelectByShape(axMapControl1);
                sMapUnits = "Unknown";
            }
            else
            {
                Application.Exit();
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.OpenFile(axMapControl1,axMapControl2, axPageLayoutControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.NewDoc(axMapControl1, axPageLayoutControl1);
        }

        private void bt_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.SaveDoc(axMapControl1, axPageLayoutControl1);
        }

        private void bt_save_as_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.SaveDocAs(axMapControl1, axPageLayoutControl1);
        }

        private void bt_linear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            linear form_linear = new linear();
            form_linear.pMap = axMapControl1.Map;
            form_linear.Show();
        }
        private void barButtonItem12_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.ExpotTo(axPageLayoutControl1);
        }
        #region 制图菜单功能
        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_pageLayoutInsert.InsertTitle();
        }

        private void bt_insert_text_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_pageLayoutInsert.InsertText();
        }

        private void bt_insert_pic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_pageLayoutInsert.InsertLegend();
        }

        private void bt_insert_north_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_pageLayoutInsert.InsertNorthArrow();
        }

        private void bt_insert_scale_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_pageLayoutInsert.InsertScale();
        }
        #endregion

        #region 地图操作
        private void bt_zoomin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.MapZoomInTool(axMapControl1);
        }

        private void barButtonItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.MapZoomOutTool(axMapControl1);
        }

        private void bt_fullextend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.ZoomToLayer(axMapControl1);
        }

        private void bt_move_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.MapPanTool(axMapControl1);
            //axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerPan;
        }

        private void barButtonItem20_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.AddData(axMapControl1);
        }

        private void barButtonItem20_ItemClick_2(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Commands.Arrow(axMapControl1);
           
        }

        private void bt_layout_zoomin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Commands.PageZoomInTool(axPageLayoutControl1);
        }

        private void bt_layout_zoomout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Commands.PageZoomOutTool(axPageLayoutControl1);
        }

        private void bt_layout_pan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Commands.PagePanTool(axPageLayoutControl1);
        }

        private void bt_layout_toall_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Commands.ZoomToLayer(axPageLayoutControl1);
        }
        #endregion

        private void barButtonItem21_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //axToolbarControl1.Visible = false;
            splitContainer2.Panel1Collapsed = true;
        }

        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //axToolbarControl1.Visible = true;
            splitContainer2.Panel1Collapsed = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                axToolbarControl1.SetBuddyControl(axMapControl1);
            }
            else
            {
                axToolbarControl1.SetBuddyControl(axPageLayoutControl1);
            }
        }

        //关闭鹰眼
        private void bt_close_eye_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splitContainer3.Panel2Collapsed = true;
        }
        //打开鹰眼
        private void bt_open_eye_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splitContainer3.Panel2Collapsed = false;
        }

        private void bt_set_frame_color_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(colorDialog1.ShowDialog()==DialogResult.OK)
            {
                m_achieveEagleEye.SetColor(colorDialog1.Color);
                
            }
            
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImageCutFrm shape_cut_frm = new ImageCutFrm();
            shape_cut_frm.m_pMap = axMapControl1.Map;
            shape_cut_frm.ShowDialog();
        }

        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandSelectionFrm bsf = new BandSelectionFrm(axMapControl1);
            bsf.ShowDialog();
        }

        #region 状态栏显示
        //在状态栏显示当前工具信息
        private void axToolbarControl1_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IToolbarControlEvents_OnMouseMoveEvent e)
        {
            //取得鼠标所在工具的索引号
            int index = axToolbarControl1.HitTest(e.x, e.y, false);
            if (index != -1)
            {
                // 取得鼠标所在工具的ToolbarItem
                IToolbarItem toolbarItem = axToolbarControl1.GetItem(index);
                // 设置状态栏信息
                MessageLabel.Text = toolbarItem.Command.Message;
            }
            else
            {
                MessageLabel.Text = " 就绪";
            }
        }
        //显示当前比例尺和坐标
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 显示当前比例尺
            scalelabel.Text = " 比例尺1:" + ((long)this.axMapControl1.MapScale).ToString();
            // 显示当前坐标
            coordinatelabel.Text = " 当前坐标X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits;
        }

        #endregion

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            #region 坐标单位
            esriUnits mapUnits = axMapControl1.MapUnits;
            switch (mapUnits)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "Centimeters";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "Decimal Degrees";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "Decimeters";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "Feet";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "Inches";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "Kilometers";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "Meters";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "Miles";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "Millimeters";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "NauticalMiles";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "Points";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "Unknown";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "Yards";
                    break;
            }
#endregion
            OperateFile.CopyAndOverwriteMap(axMapControl1, axPageLayoutControl1);
        }

        private void axMapControl1_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            OperateFile.Use_OnViewRefreshed(axTOCControl1, axMapControl1, axPageLayoutControl1);
        }

        private void axMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            OperateFile.Use_OnAfterScreenDraw(axMapControl1, axPageLayoutControl1);
        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            resample resize = new resample();
            resize.m_pMap = axMapControl1.Map;
            resize.Show();
        }

        private void bt_principal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            histogram histo = new histogram();
            histo.pMap = axMapControl1.Map;
            histo.Show();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandSynthetic bandsyn = new BandSynthetic();
            bandsyn.m_mapControl = axMapControl1;
            bandsyn.ShowDialog();


        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Bitmap bit = new Bitmap(axMapControl1.Width, axMapControl1.Height);//实例化一个和窗体一样大的bitmap
            Graphics g = Graphics.FromImage(bit);
            //g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            //g.CopyFromScreen(this.Left, this.Top, 0, 0, new Size(this.Width, this.Height));//保存整个窗体为图片
            g.CopyFromScreen(axMapControl1.PointToScreen(System.Drawing.Point.Empty), System.Drawing.Point.Empty, axMapControl1.Size);//只保存某个控件（这里是panel游戏区）
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "JPG|*.jpg";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "选择保存截图路径";
            saveDlg.RestoreDirectory = true;
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bit.Save(saveDlg.FileName);//默认保存格式为PNG，保存成jpg格式质量不是很好
            }
            
        }

        private void bt_principal_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            principal prin = new principal();
            prin.pMap = axMapControl1.Map;
            prin.ShowDialog();
            
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            water_land getwater = new water_land();
            getwater.pMap = axMapControl1.Map;
            getwater.ShowDialog();
        }

        private void bt_rad_calibration_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rad_calibration rad_cal = new rad_calibration();
            rad_cal.pMap = axMapControl1.Map;
            rad_cal.Show();
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            cal_ss calculate_ss = new cal_ss();
            calculate_ss.pMap = axMapControl1.Map;
            calculate_ss.Show();
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GreenPick greenpick = new GreenPick();
            greenpick.m_pMap = axMapControl1.Map;
            greenpick.ShowDialog();
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            parallelepiped fmParall = new parallelepiped();
            fmParall.pMap = axMapControl1.Map;
            fmParall.ShowDialog();
        }

        private void barButtonItem15_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MinimumDistance fmMinDis = new MinimumDistance();
            fmMinDis.pMap = axMapControl1.Map;
            fmMinDis.ShowDialog();
        }

        private void bt_define_ROI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            drawROI drawROIFrm = new drawROI();
            drawROIFrm.mapcontrol = axMapControl1;
            drawROIFrm.ShowDialog();
        }

        private void barButtonItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MahalanobisDistance fmMaha = new MahalanobisDistance();
            fmMaha.pMap = axMapControl1.Map;
            fmMaha.ShowDialog();
        }

        private void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IsoData fmiso = new IsoData();
            fmiso.pMap = axMapControl1.Map;
            fmiso.ShowDialog();
        }

        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            K_Means fmkmeans =new K_Means();
            fmkmeans.pMap = axMapControl1.Map;
            fmkmeans.ShowDialog();
        }

        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StretchRenderFrm stretchrender = new StretchRenderFrm(axMapControl1, axPageLayoutControl1);
            stretchrender.ShowDialog();
        }

        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClassRenderFrm classRender = new ClassRenderFrm(axMapControl1, axPageLayoutControl1);
            classRender.ShowDialog();
        }

        private void barButtonItem31_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            linear form_linear = new linear();
            form_linear.pMap = axMapControl1.Map;
            form_linear.Show();
        }

        private void barButtonItem32_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            histogram histo = new histogram();
            histo.pMap = axMapControl1.Map;
            histo.Show();
        }

        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            calquac quac = new calquac();
            quac.pMap = axMapControl1.Map;
            quac.Show();
        }

        private void barButtonItem33_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandSynthetic bandsyn = new BandSynthetic();
            bandsyn.m_mapControl = axMapControl1;
            bandsyn.ShowDialog();
        }

        private void barButtonItem34_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BandSelectionFrm bsf = new BandSelectionFrm(axMapControl1);
            bsf.ShowDialog();
        }

        private void barButtonItem35_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StretchRenderFrm stretchrender = new StretchRenderFrm(axMapControl1, axPageLayoutControl1);
            stretchrender.ShowDialog();
        }

        private void barButtonItem36_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClassRenderFrm classRender = new ClassRenderFrm(axMapControl1, axPageLayoutControl1);
            classRender.ShowDialog();
        }

        private void barButtonItem39_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            axMapControl1.CurrentTool = null;
            m_SelectByShape.SelectType = "Select by rectangle";
        }

        private void barButtonItem40_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {           
            m_SelectByShape.SelectType = "Select by polygon";
        }

        private void barButtonItem41_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            m_SelectByShape.SelectType = "Select by line";
        }

        private void barButtonItem42_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            m_SelectByShape.SelectType = "Select by circle";
        }

        private void barButtonItem43_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            m_SelectByShape.SelectType = "null";
        }

        private void barButtonItem45_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.OpenFile(axMapControl1, axMapControl2, axPageLayoutControl1);
            axTOCControl1.SetBuddyControl(axMapControl1);
        }

        private void barButtonItem44_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_SelectByShape.SelectType = "";
            Commands.AddData(axMapControl1);
        }

        private void barButtonItem47_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            attrcal newform = new attrcal();
            newform.pMap = axMapControl1.Map;
            newform.Show();
            newform.Visible = false;
            DialogResult result = newform.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            string pQuerySentence;
            pQuerySentence = newform.text;

            IQueryFilter pQueryFilter;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = pQuerySentence;

            IFeatureSelection pFeatureSelection;
            pFeatureSelection = axMapControl1.Map.get_Layer(newform.Layerindex) as IFeatureSelection;
            pFeatureSelection.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
            pFeatureSelection.SelectionChanged();
            pFeatureSelection.SelectionColor = GetRGB(255, 0, 0);

            ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;
            ICursor pCursor = null;
            pSelectionSet.Search(null, false, out pCursor);
            IFeatureCursor pFeatureCursor = pCursor as IFeatureCursor;
            IFeature pFeature = pFeatureCursor.NextFeature();
            IEnvelope pEnvelope = new EnvelopeClass();
            int index = 0;
            bool b = false;
            IFeature pFea = null;
            while (pFeature != null)
            {
                index++;
                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    b = true;
                pFea = pFeature;
                pEnvelope.Union(pFeature.Extent);
                pFeature = pFeatureCursor.NextFeature();
            }
            if (index == 1 && b)
            {
                IEnvelope pEnv = new EnvelopeClass();
                IPoint pPoint = pFea.Shape as IPoint;
                pEnv.XMax = pPoint.X + 200;
                pEnv.XMin = pPoint.X - 200;
                pEnv.YMax = pPoint.Y + 200;
                pEnv.YMin = pPoint.Y - 200;

                axMapControl1.ActiveView.Extent = pEnv;
            }
            else
            {
                pEnvelope.Expand(2, 2, true);
                axMapControl1.ActiveView.Extent = pEnvelope;
            }

            axMapControl1.Refresh();
            axTOCControl1.ActiveView.ContentsChanged();
            axTOCControl1.Update();
            axTOCControl1.ActiveView.Refresh();

        }

        private void barButtonItem48_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Bitmap bit = new Bitmap(axMapControl1.Width, axMapControl1.Height);//实例化一个和窗体一样大的bitmap
            Graphics g = Graphics.FromImage(bit);
            //g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            //g.CopyFromScreen(this.Left, this.Top, 0, 0, new Size(this.Width, this.Height));//保存整个窗体为图片
            g.CopyFromScreen(axMapControl1.PointToScreen(System.Drawing.Point.Empty), System.Drawing.Point.Empty, axMapControl1.Size);//只保存某个控件（这里是panel游戏区）
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "JPG|*.jpg";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "选择保存截图路径";
            saveDlg.RestoreDirectory = true;
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bit.Save(saveDlg.FileName);//默认保存格式为PNG，保存成jpg格式质量不是很好
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            lin cal_lin = new lin();
            cal_lin.m_pMap = axMapControl1.Map;
            cal_lin.Show();
        }

        private void barButtonItem50_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dan cal_dan = new dan();
            cal_dan.m_pMap = axMapControl1.Map;
            cal_dan.Show();
        }

        private void barButtonItem51_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gmsy cal_mn = new gmsy();
            cal_mn.m_pMap = axMapControl1.Map;
            cal_mn.Show();
        }

        private void barButtonItem49_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OperateFile.ExpotTo(axPageLayoutControl1);
        }


    }
}

