using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HolidayMaster
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            string yearStart = "";
            string yearEnd = "";
            string StartMonth = "April_";
            string currentMonth = "";
            string EndMonth = "April_";

            currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            int year = DateTime.Now.Year;
            if (currentMonth.Contains("January") || currentMonth.Contains("February") || currentMonth.Contains("March"))
            {
                StartMonth = StartMonth + (year - 1).ToString();
                EndMonth = EndMonth + year.ToString();
                yearStart = (year - 1).ToString() + "-04-01";
                yearEnd = year.ToString() + "-04-01";
            }
            else
            {
                StartMonth = StartMonth + year.ToString();
                EndMonth = EndMonth + (year + 1).ToString();
                yearStart = year.ToString() + "-04-01";
                yearEnd = (year + 1).ToString() + "-04-01";
            }
            currentMonth = currentMonth + "_" + year.ToString();

            string sql = "SELECT " + StartMonth + "," + currentMonth + "," + EndMonth + " FROM [dbo].[aaa_holiday_master] where staff_id = " + CONNECT.staff_id;
            using (SqlConnection conn = new SqlConnection(CONNECT.ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql,conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    lblStart.Text = dt.Rows[0][0].ToString();
                    lblCurrent.Text = dt.Rows[0][1].ToString();
                    lblEnd.Text = dt.Rows[0][2].ToString();
                    lblTitle.Text = CONNECT.staff_name;
                }

                sql = " select date_absent AS [Date],CASE WHEN absent_type = 2 then 'FULL DAY' WHEN absent_type = 3 then 'HALF DAY'" +
                    " WHEN absent_type = 8 then 'FULL DAY' WHEN absent_type = 9 then 'UNPAID' WHEN absent_type = 5 then 'ABSENT' END AS[Type] " +
                    " from dbo.absent_holidays where absent_type <> 7 AND  staff_id = " + CONNECT.staff_id.ToString() + " AND date_absent >= '" + yearStart + "' AND date_absent <= '" + yearEnd + "' ORDER BY date_absent asc";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;

                    dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                    conn.Close();
            }

        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            //colour anything before today as green
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToDateTime(row.Cells[0].Value) <= DateTime.Now)
                {
                    row.DefaultCellStyle.BackColor = Color.YellowGreen;
                }
            }
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}
