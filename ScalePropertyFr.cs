using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace water_quality
{
    public partial class ScalePropertyFr : Form
    {
        IElement m_pElement;
        IScaleBar m_ScaleBar;
        public ScalePropertyFr(IElement pEle )
        {
            InitializeComponent();
            m_pElement = pEle;
            IMapSurroundFrame pSurround = m_pElement as IMapSurroundFrame;
            m_ScaleBar = pSurround.MapSurround as IScaleBar;
        }

        private void ScalePropertyFr_Load(object sender, EventArgs e)
        {
            comBoxUnits.Text = m_ScaleBar.Units.ToString();
            numUpDown2.Value = Convert.ToDecimal(m_ScaleBar.Division);
            numUpDown1.Value = Convert.ToDecimal(m_ScaleBar.Divisions);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_ScaleBar.UnitLabel = comBoxUnits.Text;
            m_ScaleBar.Divisions = Convert.ToInt16(numUpDown1.Value);
            m_ScaleBar.Division = Convert.ToDouble(numUpDown2.Value);
            
        }

       
    }
}
