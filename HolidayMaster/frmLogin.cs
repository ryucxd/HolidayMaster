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

namespace HolidayMaster
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            //shopfloor
            //string sql = "select u.forename + ' ' + u.surname from dbo.[aaa_holiday_master]  a left join [user_info].dbo.[user] u on a.staff_id = u.id where ShopFloor = -1   order by u.forename + ' ' + u.surname ";
            
            //office ver
            string sql = "select u.forename + ' ' + u.surname from dbo.[aaa_holiday_master]  a left join [user_info].dbo.[user] u on a.staff_id = u.id where [current] = 1 AND (ShopFloor = 0 or ShopFloor is null)  order by u.forename + ' ' + u.surname ";

            lblTitle.Text = "Office Holidays";
            using (SqlConnection conn = new SqlConnection(CONNECT.ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                        cmbStaff.Items.Add(row[0].ToString());

                }

                conn.Close();
            }
        }

        private void btnViewHoliday_Click(object sender, EventArgs e)
        {

          
            if (txtPassword.Text == "")
            {
                MessageBox.Show("Please enter the password before attempting to view your holidays", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbStaff.Text == "" || cmbStaff.Items.Contains(cmbStaff.Text) == false)
            {
                MessageBox.Show("Please select your name before attempting to view your holidays", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbStaff.Text = "";
                return;
            }

            string sql = "SELECT id  FROM [user_info].[dbo].[user] WHERE forename + ' ' + surname  = '" + cmbStaff.Text + "' and id = " + txtPassword.Text;

            using (SqlConnection conn = new SqlConnection(CONNECT.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    var data = cmd.ExecuteScalar();
                    if (data != null)
                    {
                        CONNECT.staff_id = Convert.ToInt32(data);
                        CONNECT.staff_name = cmbStaff.Text;
                        frmMain frm = new frmMain();
                        this.Hide();
                        frm.ShowDialog();
                        txtPassword.Text = "";
                        cmbStaff.Text = "";
                        cmbStaff.Focus();
                        this.Show();
                    } 
                    else
                    {
                        MessageBox.Show("Wrong password!", "Try again...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPassword.Text = "";
                        txtPassword.Focus();
                    }
                    conn.Close();
                }


            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnViewHoliday.PerformClick();
            
        }

        private void cmbStaff_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => { cmbStaff.Select(0, 0); }));
        }
    }
}
