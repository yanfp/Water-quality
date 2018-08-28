using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class cal_ss : DevExpress.XtraEditors.XtraForm
    {
        public IMap pMap;
        public IRasterLayer pCreatRalyr;
        public cal_ss()
        {
            InitializeComponent();
        }

        private void cal_ss_Load(object sender, EventArgs e)
        {
            comboBoxOpen.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                comboBoxOpen.Items.Add(pMap.get_Layer(i).Name);
            if (comboBoxOpen.Items.Count > 0)
                comboBoxOpen.SelectedIndex = 0;
        }

        private void bt_openfile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.CheckPathExists = true;
            saveDlg.Filter = "IMAGINE|*.img|TIFF|*.tif|Raster|*.jpg";
            saveDlg.OverwritePrompt = true;
            saveDlg.Title = "输出栅格图像";
            saveDlg.RestoreDirectory = true;
            DialogResult dr = saveDlg.ShowDialog();
            if (dr == DialogResult.OK)
                textBoxOut.Text = saveDlg.FileName;
        }

        private void bt_cal_ss_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("正在计算悬浮物浓度");
            pCreatRalyr = (IRasterLayer)pMap.get_Layer(comboBoxOpen.SelectedIndex);
            //初始化ENVI
            COM_IDL_connectLib.COM_IDL_connectClass oComIDL = new COM_IDL_connectLib.COM_IDL_connectClass();
            oComIDL.CreateObject(0, 0, 0);
            oComIDL.ExecuteString(".compile '" + System.IO.Directory.GetCurrentDirectory() + @"\cal_ss.pro'");
            oComIDL.ExecuteString(@"math_doit_water,'" + pCreatRalyr.FilePath + "','" + textBoxOut.Text + "'");
            oComIDL.DestroyObject();
            openraster_1 openrasterfile = new openraster_1();
            openrasterfile.pMap = pMap;
            openrasterfile.OpenRaster(textBoxOut.Text);
            //OpenRaster(tb_output.Text);
            splashScreenManager1.CloseWaitForm();
            this.Close();
        }
    }
}