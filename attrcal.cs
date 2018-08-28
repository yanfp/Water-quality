using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;


namespace water_quality
{
    public partial class attrcal : DevExpress.XtraEditors.XtraForm
    {
        public IMap pMap;
        public string text;
        public int Layerindex;
        public int fieldIndex;
        public string fieldname;
        public ITable pTable;
        public double BuffDistance = 0.1;
        public attrcal()
        {
            InitializeComponent();
        }

        private void attrcal_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            int i, layCount;
            layCount = pMap.LayerCount;
            for (i = 0; i < layCount; i++)
                comboBox1.Items.Add(pMap.get_Layer(i).Name);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Layerindex = comboBox1.SelectedIndex;
            IGeoFeatureLayer pGeoFeatureLayer = (IGeoFeatureLayer)pMap.get_Layer(Layerindex);

            pTable = (ITable)pGeoFeatureLayer;

            int num1;
            num1 = pTable.Fields.FieldCount;
            comboBox2.Items.Clear();
            IField item;

            for (int i = 0; i < num1; i++)
            {
                item = pTable.Fields.get_Field(i);
                comboBox2.Items.Add(item.Name.ToString());
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text += "=";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "<>";
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Like";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox1.Text += ">";
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            textBox1.Text += "And";
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            textBox1.Text += "<";
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Or";
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            textBox1.Text += "%";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Not";
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
            return;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            text = textBox1.Text;
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Text += comboBox2.Text;
            listBox1.Items.Clear();

            fieldIndex = comboBox2.SelectedIndex;
            fieldname = pTable.Fields.get_Field(fieldIndex).Name;

            IQueryFilter pQueryFilter;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.AddField(fieldname);

            int rowCount = pTable.RowCount(pQueryFilter);
            for (int i = 0; i < rowCount; i++)
            {
                listBox1.Items.Add(pTable.GetRow(i).get_Value(fieldIndex).ToString());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pTable.Fields.get_Field(fieldIndex).GetType() == typeof(string))
                textBox1.Text += ("'" + listBox1.SelectedItem + "'");
            else
                textBox1.Text += listBox1.SelectedItem;
        }

    }
}