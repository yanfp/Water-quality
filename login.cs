using System;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Runtime.InteropServices;

   
namespace water_quality
{

    public partial class login : DevExpress.XtraEditors.XtraForm
    {
        //实现无边框窗体的拖动
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        
        //实现无边框窗体的拖动
        private void login2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }

        public int res=-1;
        public login()
        {
            InitializeComponent();
        }

        private void bt_quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
 
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bt_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void bt_login_Click(object sender, EventArgs e)
        {
            
            if (tb_uname.Text == "" || tb_passwd.Text == "")
            {
                MessageBox.Show("用户名或密码不能为空", "提示", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                return;
            }
            string cnnstring = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=user.mdb";//创建连接字符串
            OleDbConnection cnn = new OleDbConnection(cnnstring);//定义一个OledbConnection 的对象
            try
            {
                cnn.Open();//打开数据库
                //MessageBox.Show("打开数据库成功");
            }
            catch (Exception ex)// 常规的 catch 处理程序，捕捉异常
            {
                //throw;
                string[] str = ex.ToString().Split(' ');
                MessageBox.Show("打开数据库失败" + str[1], "提示", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                return;
            }
            user user1 = new user();
            user1.uname = tb_uname.Text.ToString();
            user1.password = tb_passwd.Text.ToString();
            OleDbCommand sqlcmdn = new OleDbCommand();
            sqlcmdn.CommandText = "select username,password from [user] where username=" + "'" + user1.uname + "'and password =" + "'" + user1.password + "' ";
            sqlcmdn.Connection = cnn;
            OleDbDataReader reader1 = sqlcmdn.ExecuteReader();
            if (reader1.HasRows)                                  //这个read调用很重要！不写的话运行时将提示找不到数据
            {
                //MessageBox.Show("登陆成功");
                res = 1;
                //MessageBox.Show("用户名："+reader1.GetString(0));
            }
            else
            {
                MessageBox.Show("用户名或密码错误", "提示", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                goto Error1;
            }

            //mainform mainform1 = new mainform();
            //this.Hide();
            //mainform1.Show();          
        Error1:
                cnn.Close();

        }
       
        
    }
}