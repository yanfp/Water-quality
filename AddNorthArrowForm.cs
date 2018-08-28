using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class AddNorthArrowForm : DevExpress.XtraEditors.XtraForm
    {
        //AxPageLayoutControl m_pageLayout;
        public IStyleGalleryItem m_pStyleGalleryItem;
        IMarkerNorthArrow pMarkerNorthArrow = null;
        ICharacterMarkerSymbol pCharacterMarkerSymbol = null;
        public AddNorthArrowForm()
        {
            InitializeComponent();
        }

        private void AddNorthArrowForm_Load(object sender, EventArgs e)
        {
            try
            {
                axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassNorthArrows;
                //Get the ArcGIS install location
                string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;
                //Load the ESRI.ServerStyle file into the SymbologyControl
                axSymbologyControl1.LoadStyleFile(sInstall + "\\Styles\\ESRI.ServerStyle");
            }
            catch
            {
                MessageBox.Show("加载符号类型库文件失败！");
                this.Close();
            }
            button2.Enabled = false;
            axSymbologyControl1.StyleClass = esriSymbologyStyleClass.esriStyleClassNorthArrows;
        }

        private void axSymbologyControl1_OnItemSelected(object sender, ISymbologyControlEvents_OnItemSelectedEvent e)
        {
            button2.Enabled = true;
            m_pStyleGalleryItem = (IStyleGalleryItem)e.styleGalleryItem;
            if (m_pStyleGalleryItem == null) return;
            //获取新的指北针
            pMarkerNorthArrow = m_pStyleGalleryItem.Item as IMarkerNorthArrow;
            if (pMarkerNorthArrow == null) return;
            pCharacterMarkerSymbol = pMarkerNorthArrow.MarkerSymbol as ICharacterMarkerSymbol;
            pCharacterMarkerSymbol.Size = 100;
            System.Drawing.Image image = PageLayoutInsert.CreatePictureFromSymbol((ISymbol)this.pCharacterMarkerSymbol, this.picBoxPreview.Width, this.picBoxPreview.Height, 0);
            this.picBoxPreview.Image = image;

            textBoxSize.Text = pCharacterMarkerSymbol.Size.ToString();
            textBoxAngle.Text = pCharacterMarkerSymbol.Angle.ToString();
        }
    }
}