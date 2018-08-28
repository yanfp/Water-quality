using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace water_quality
{
    public partial class BandSelectionFrm : Form
    {
        private AxMapControl m_mapControl=null;
        private IMap m_pMap;//可以不要
        
        IRasterRGBRenderer2 rasterRGBRen=null;
        IRasterRenderer rasterRen=null;
        IRasterLayer m_rasterLayer = null;

        public BandSelectionFrm(AxMapControl mapControl)
        {
            InitializeComponent();
            m_mapControl = mapControl;
            m_pMap = mapControl.Map;

        }

        private void BandSelectionFrm_Load(object sender, EventArgs e)
        {
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;

            rasterRGBRen = new RasterRGBRendererClass();
            rasterRen = (IRasterRenderer)rasterRGBRen;

            int layerCount = m_pMap.LayerCount;
            for (int i = 0; i < layerCount; i++)
            {
                comboBox1.Items.Add(m_pMap.get_Layer(i).Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_rasterLayer = (IRasterLayer)m_pMap.get_Layer(comboBox1.SelectedIndex);
            if (m_pMap.get_Layer(comboBox1.SelectedIndex) is IRasterLayer)
            {
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
            int i;
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            if (m_pMap.LayerCount != 0)
            {
                for (i = 0; i < m_rasterLayer.BandCount; i++)
                {
                    comboBox2.Items.Add("Band " + Convert.ToString(i + 1));
                }
                for (i = 0; i < m_rasterLayer.BandCount; i++)
                {
                    comboBox3.Items.Add("Band " + Convert.ToString(i + 1));
                }
                for (i = 0; i < m_rasterLayer.BandCount; i++)
                {
                    comboBox4.Items.Add("Band " + Convert.ToString(i + 1));
                }
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;
            comboBox4.SelectedIndex = 2;

            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            rasterRen.Raster = m_rasterLayer.Raster;
            rasterRen.Update();
            rasterRGBRen.RedBandIndex = comboBox2.SelectedIndex;
            rasterRGBRen.GreenBandIndex = comboBox3.SelectedIndex;
            rasterRGBRen.BlueBandIndex = comboBox4.SelectedIndex;
            rasterRen.Update();
            m_rasterLayer.Renderer = (IRasterRenderer)rasterRGBRen;
            m_mapControl.Extent = m_rasterLayer.AreaOfInterest;
            m_mapControl.ActiveView.Refresh();
            m_mapControl.Refresh();
            m_mapControl.Update();

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_rasterLayer = (IRasterLayer)m_pMap.get_Layer(comboBox1.SelectedIndex);
            rasterRen.Raster = m_rasterLayer.Raster;
            rasterRen.Update();
            rasterRGBRen.RedBandIndex = comboBox2.SelectedIndex;
            rasterRGBRen.GreenBandIndex = comboBox3.SelectedIndex;
            rasterRGBRen.BlueBandIndex = comboBox4.SelectedIndex;
            rasterRen.Update();
            m_rasterLayer.Renderer = (IRasterRenderer)rasterRGBRen;
            m_mapControl.Extent = m_rasterLayer.AreaOfInterest;
            m_mapControl.ActiveView.Refresh();
            m_mapControl.Refresh();
            m_mapControl.Update();

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_rasterLayer = (IRasterLayer)m_pMap.get_Layer(comboBox1.SelectedIndex);
            rasterRen.Raster = m_rasterLayer.Raster;
            rasterRen.Update();
            rasterRGBRen.RedBandIndex = comboBox2.SelectedIndex;
            rasterRGBRen.GreenBandIndex = comboBox3.SelectedIndex;
            rasterRGBRen.BlueBandIndex = comboBox4.SelectedIndex;
            rasterRen.Update();
            m_rasterLayer.Renderer = (IRasterRenderer)rasterRGBRen;
            m_mapControl.Extent = m_rasterLayer.AreaOfInterest;
            m_mapControl.ActiveView.Refresh();
            m_mapControl.Refresh();
            m_mapControl.Update();

        }
    }
}
