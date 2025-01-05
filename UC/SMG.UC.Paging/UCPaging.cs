using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.UC.Paging
{
    public partial class UCPaging : UserControl
    {// Delegate và event cho các nút phân trang
        public delegate void PageChangedHandler(int currentPage, int rowsPerPage,int dataTotal);
        public event PageChangedHandler PageChanged;

        [Browsable(true)]
        [Category("Paging")]
        [Description("Tổng số trang.")]
        public int TotalPages { get; set; }

        [Browsable(true)]
        [Category("Paging")]
        [Description("Số dòng trên mỗi trang.")]
        public int RowsPerPage { get; set; }

        [Browsable(true)]
        [Category("Paging")]
        [Description("Trang hiện tại.")]
        public int CurrentPage { get; set; }

        [Browsable(true)]
        [Category("Paging")]
        [Description("Tổng dòng dữ liệu.")]
        public int DataTotal { get; set; }

        public UCPaging()
        {
            InitializeComponent();
            SetupDefaultValues();
        }

        private void SetupDefaultValues()
        {
            CurrentPage = 1;
            RowsPerPage = 10;
            TotalPages = 1;
            UpdateUI();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateUI();
                PageChanged?.Invoke(CurrentPage, RowsPerPage, DataTotal);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateUI();
                PageChanged?.Invoke(CurrentPage, RowsPerPage, DataTotal);
            }
        }

        private void txtCurrent_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtCurrent.Text, out int page))
            {
                if (page > 0 && page <= TotalPages)
                {
                    CurrentPage = page;
                    UpdateUI();
                    PageChanged?.Invoke(CurrentPage, RowsPerPage, DataTotal);
                }
            }
        }

        private void cboRowperPage_EditValueChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cboRowperPage.EditValue?.ToString(), out int rows))
            {
                RowsPerPage = rows;
                PageChanged?.Invoke(CurrentPage, RowsPerPage, DataTotal);
            }
        }

        private void UpdateUI()
        {
            txtCurrent.Text = CurrentPage.ToString();
            lblTotal.Text = $"/ {TotalPages}";
            lblDataTotal.Text = $"{DataTotal}";
        }
    }
}
