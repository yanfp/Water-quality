using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace water_quality
{
    public partial class LegendPropertyForm : Form
    {
        ILegend m_legend;
        ILegend m_legend_Complete;
        IElement m_pElement;
        public LegendPropertyForm(ILegend legendComplete,IElement pEle)
        {
            InitializeComponent();
            m_legend_Complete = legendComplete;
            m_pElement = pEle;
            IMapSurroundFrame pSurround = pEle as IMapSurroundFrame;
            m_legend = pSurround.MapSurround as ILegend;             
        }

        private void LegendPropertyForm_Load(object sender, EventArgs e)
        {
            this.InitDisplay();
            
            IEnvelope pEnvelop = m_pElement.Geometry.Envelope;         
        }

        public void InitDisplay()
        {
            textBoxTitle.Text = m_legend.Title;
            if(m_legend.Title==string.Empty)
                checkBoxIsTitleShow.Checked = false;
            else
                checkBoxIsTitleShow.Checked = true;
            int itemCount=m_legend.ItemCount;
            int itemAll = m_legend_Complete.ItemCount;
            for (int i = 0; i < itemCount; i++)
            {
                listBoxLegendItem.Items.Add(this.m_legend.get_Item(i).Layer.Name);
            }
            for (int j = 0; j < itemAll; j++)
            {              
                listBoxMapItem.Items.Add(m_legend_Complete.get_Item(j).Layer.Name);
            }         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            listBoxLegendItem.Items.Add(listBoxMapItem.SelectedItem);
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBoxLegendItem.Items.RemoveAt(listBoxLegendItem.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int itemAll =listBoxMapItem.Items.Count;
            for (int j = 0; j < itemAll; j++)
            {
                listBoxLegendItem.Items.Add(m_legend_Complete.get_Item(j).Layer.Name);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBoxLegendItem.Items.Clear();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_legend.Title = textBoxTitle.Text;
            
            m_legend.ClearItems();
            int mapCount = m_legend_Complete.ItemCount;
            int legentCount = listBoxLegendItem.Items.Count;
            for (int i = 0; i < legentCount; i++)
            {
                for (int j = 0; j < mapCount; j++)
                {
                    if (listBoxLegendItem.Items[i].ToString() == listBoxMapItem.Items[j].ToString())
                    {
                        m_legend.AddItem(m_legend_Complete.get_Item(j));
                    }

                }
            }
            if (checkBoxIsTitleShow.Checked == false)
                m_legend.Title = "";         
            m_legend.Refresh();
                
        }


        

    }
}
