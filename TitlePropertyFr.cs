using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using stdole;
using ESRI.ArcGIS.Geometry;

namespace water_quality
{
    public partial class TitlePropertyFr : Form
    {
        ITextElement m_textEle;
        IPoint m_pt;//坐标
        ITextSymbol pTextSymbol;//标题属性
        IFontDisp pFont= new StdFontClass() as IFontDisp;//字体属性
        IColor pColor=new RgbColorClass();
        public TitlePropertyFr(ITextElement textEle,IPoint pt)
        {
            InitializeComponent();
            m_textEle = textEle;
            m_pt = pt;
        }

        private void TitlePropertyFr_Load(object sender, EventArgs e)
        {
            textBoxTitle.Text = m_textEle.Text;
            btnColor.BackColor = PageLayoutInsert.IColorToColor(m_textEle.Symbol.Color);
            zitiBox.Text = m_textEle.Symbol.Font.Name.ToString();
            daxiaoBox.Text = m_textEle.Symbol.Font.Size.ToString();
            pFont.Size = 23;
            chkBoxBold.Checked = m_textEle.Symbol.Font.Bold;
            chkBoxUnderline.Checked = m_textEle.Symbol.Font.Underline;
            textBoxX.Text = m_pt.X.ToString();
            textBoxY.Text = m_pt.Y.ToString();

            
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();           
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {               
                zitiBox.Text = fontDlg.Font.Name;             
                daxiaoBox.Text = fontDlg.Font.Size.ToString();
                chkBoxBold.Checked = fontDlg.Font.Bold;
                chkBoxUnderline.Checked = fontDlg.Font.Underline;  
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = btnColor.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                btnColor.BackColor = colorDlg.Color;      
                pColor=PageLayoutInsert.ColorToIColor(colorDlg.Color);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_textEle.Text = textBoxTitle.Text;

            pFont.Bold = chkBoxBold.Checked;
            pFont.Name = zitiBox.Text;
            pFont.Size = Convert.ToDecimal(daxiaoBox.Text);
            pFont.Underline = chkBoxUnderline.Checked;

            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Color =pColor;
            pTextSymbol.Font = pFont;

            m_textEle.Symbol = pTextSymbol;//更改后的属性赋给标题
            
        }
    }
}

