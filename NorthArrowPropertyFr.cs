using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace water_quality
{
    public partial class NorthArrowPropertyFr : Form
    {
        INorthArrow m_northArrow;
        IElement m_pElement;
        public NorthArrowPropertyFr(IElement pEle)
        {
            InitializeComponent();
            m_pElement = pEle;
            IMapSurroundFrame pSurround = pEle as IMapSurroundFrame;
            m_northArrow = pSurround.MapSurround as INorthArrow;          
        }

        private void NorthArrowPropertyFr_Load(object sender, EventArgs e)
        {
            textBoxSize.Text = m_northArrow.Size.ToString();//大小
            textBoxAngle.Text = m_northArrow.CalibrationAngle.ToString();//角度
            buttonColor.BackColor =PageLayoutInsert.IColorToColor(m_northArrow.Color);//颜色  

            IEnvelope pEnvelop = m_pElement.Geometry.Envelope;
            textBoxX.Text = pEnvelop.XMin.ToString();
            textBoxY.Text = pEnvelop.YMin.ToString();
            textBoxWidth.Text = pEnvelop.Width.ToString();
            textBoxHeight.Text = pEnvelop.Height.ToString();


            
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                buttonColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_northArrow.Size = Convert.ToDouble(textBoxSize.Text);
            m_northArrow.Color =PageLayoutInsert.ColorToIColor(buttonColor.BackColor);
            m_northArrow.CalibrationAngle = Convert.ToDouble(textBoxAngle.Text);
            m_northArrow.Refresh();     
        }


    }
}
