using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace Project
{
    public partial class fmMain : Form
    {
        private int changeCount = 0;
        private String tableName = "Nhan Vien";
        private SqlConnection con = null;
        private delegate void NewHome();
        private event NewHome OnNewHome;


        public fmMain()
        {
            InitializeComponent();
            try
            {
                SqlClientPermission ss = new SqlClientPermission(PermissionState.Unrestricted);
                ss.Demand();
            }
            catch (Exception)
            {

                throw;
            }
            SqlDependency.Stop(Program.connstr);
            SqlDependency.Start(Program.connstr);
            con = new SqlConnection(Program.connstr);
        }

        private void LoadData()
        {
            DataTable dt = new DataTable();
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand cmd = new SqlCommand("SELECT MANV, HO, TEN, DIACHI, NGAYSINH, LUONG  from dbo.NhanVien", con);
            cmd.Notification = null;

            SqlDependency de = new SqlDependency(cmd);
            de.OnChange += new OnChangeEventHandler(de_OnChange);
            
            dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));
            dataGridView1.DataSource = dt;
        }

        private void de_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency de = sender as SqlDependency;
            de.OnChange -= de_OnChange;
            Invoke(new Action(() =>
            {
                changeMainMsg(sender, e);
            }));
            if (OnNewHome != null)
            {
                OnNewHome();
            }
        }

        private void changeMainMsg(object sender, SqlNotificationEventArgs e)
        {
            changeCount += 1;
            this.label.Text = "Đã có " + changeCount + " thay đổi";
            this.listBox1.Items.Clear();
            this.listBox1.Items.Add("Infor:    " + e.Info.ToString());
            this.listBox1.Items.Add("Source:    " + e.Source.ToString());
            this.listBox1.Items.Add("Type:   " + e.Type.ToString());
        }

        private void Form1_OnNewHome()
        {
            ISynchronizeInvoke i = (ISynchronizeInvoke)this;
            if (i.InvokeRequired)//tab
            {
                NewHome dd = new NewHome(Form1_OnNewHome);
                i.BeginInvoke(dd, null);
                return;
            }
            LoadData();
        }


        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == ftype)
                {
                    return f;
                }
            }
            return null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {     
            OnNewHome += new NewHome(Form1_OnNewHome);
            LoadData();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Form fm = this.CheckExists(typeof(fmNhanVien));
            if (fm != null)
            {
                fm.Activate();
            }
            else
            {
                fmNhanVien f = new fmNhanVien();
                f.Show();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn chắc chắn muốn thoát?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}
