using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class fmNhanVien : Form
    {
        public fmNhanVien()
        {
            InitializeComponent();
        }

        private void fmNhanVien_Load(object sender, EventArgs e)
        {
            this.nhanVienTableAdapter.Fill(this.dS.NhanVien);
            defaultStatus();
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn chắc chắn muốn thoát?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void updateTableAdapter()
        {
            bdsNV.EndEdit();
            bdsNV.ResetCurrentItem();
            this.nhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.nhanVienTableAdapter.Update(this.dS.NhanVien);
        }

        private void defaultStatus()
        {
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
            btnLuu.Enabled = btnHuy.Enabled = false;
            txtMaNV.Enabled = txtHo.Enabled = txtTen.Enabled = txtDiaChi.Enabled = txtNgaySinh.Enabled = txtLuong.Enabled = false;
        }

        private void secondStatus()
        {
            btnThem.Enabled = btnSua.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
            btnLuu.Enabled = btnHuy.Enabled = true;
            txtHo.Enabled = txtTen.Enabled = txtDiaChi.Enabled = txtNgaySinh.Enabled = txtLuong.Enabled = true;
            txtMaNV.Enabled = false;
        }

        private int sinhMaNV()
        {
            int maNV = 1; ;
            string strLenh = "EXEC SP_SINHMA_NV";
            Program.myReader = Program.ExecSqlDataReader(strLenh);
            if (Program.myReader == null)
            {
                return maNV;
            }

            if (Program.myReader.Read())
            {
                maNV = Program.myReader.GetInt32(0) + 1;
            }

            Program.myReader.Close();
            return maNV;
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bdsNV.AddNew();
            txtMaNV.Text = sinhMaNV() + "";
            secondStatus();
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(txtHo.Text == "")
            {
                MessageBox.Show("Họ không được trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHo.Focus();
                return;
            }
            if(txtTen.Text == "")
            {
                MessageBox.Show("Tên không được trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTen.Focus();
                return;
            }
            if(txtDiaChi.Text == "")
            {
                MessageBox.Show("Địa chỉ không được trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChi.Focus();
                return;
            }
            txtNgaySinh.Value = DateTime.Today;
            if(txtLuong.Text == "")
            {
                MessageBox.Show("Lương không được trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuong.Focus();
                return;
            }
            else if(int.Parse(txtLuong.Text.ToString()) < 4000000)
            {
                MessageBox.Show("Lương phải lớn hơn 4 triệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuong.Focus();
                return;
            }
            try
            {
                this.updateTableAdapter();
                MessageBox.Show("Thao tác thành công!", "Thông báo", MessageBoxButtons.OK);             
                defaultStatus();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi ghi: " + ex.Message, "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult r = MessageBox.Show("Bạn có chắc chắn muốn xóa vĩnh viễn?", "THÔNG BÁO XÓA", MessageBoxButtons.YesNo);
            if(r == DialogResult.Yes)
            {
                try
                {
                    bdsNV.EndEdit();
                    bdsNV.RemoveCurrent();
                    this.updateTableAdapter();
                    MessageBox.Show("Đã xóa thành công!", "THÔNG BÁO XÓA", MessageBoxButtons.OK);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Lỗi xóa nhân viên " + ex.Message + " ", "THÔNG BÁO", MessageBoxButtons.OK);
                    return;
                }
            }
        }

        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            secondStatus();
        }

        private void btnHuy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bdsNV.RemoveCurrent();
            defaultStatus();
        }
    }
}
