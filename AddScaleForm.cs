using System;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using stdole;

namespace water_quality
{
    public partial class AddScaleForm : Form
    {      
        public IStyleGalleryItem m_pStyleGalleryItem=null;
   
        public AddScaleForm()
        {
            InitializeComponent();
        }

        private void AddScaleForm_Load(object sender, EventArgs e)
        {
            try
            {
                axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;//SymbologyControl加载的symbol类别               
                string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;                
                axSymbologyControl1.LoadStyleFile(sInstall + "\\Styles\\ESRI.ServerStyle");               
            }
            catch
            {
                MessageBox.Show("加载符号类型库文件失败！");
                this.Close();
            }      
            button2.Enabled = false;
            axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassScaleBars;
        }

        private void axSymbologyControl1_OnItemSelected(object sender, ISymbologyControlEvents_OnItemSelectedEvent e)
        {
            button2.Enabled = true;
            m_pStyleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;
            if (m_pStyleGalleryItem == null) return;
            //获取新的指北针      
            this.PreviewImage();    
            IScaleBar pScaleBar=m_pStyleGalleryItem.Item as IScaleBar;
            txtBoxUnit.Text = pScaleBar.Units.ToString();
        }

        protected void PreviewImage()
        {
            ISymbologyStyleClass pSymbologyStyleClass = axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass);
            IPictureDisp pPicture = pSymbologyStyleClass.PreviewItem(m_pStyleGalleryItem, picBoxPreview.Width, picBoxPreview.Height);
            Image pImage = Image.FromHbitmap(new IntPtr(pPicture.Handle));
            picBoxPreview.Image = pImage;
        }
    }
}
