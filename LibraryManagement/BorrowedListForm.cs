using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagement
{
    public partial class BorrowedListForm : Form
    {
        public BorrowedListForm(List<BorrowedList> borrowedList)
        {
            InitializeComponent();
            this.Text = "Borrowed Books List";

            dgvBorrowedBooks.DataSource = borrowedList;
            dgvBorrowedBooks.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            if (dgvBorrowedBooks.Columns.Contains("Date Borrowed"))
            {
                dgvBorrowedBooks.Columns["DateBorrowed"].DefaultCellStyle.Format = "MM/dd/yyyy";
            }
            if (dgvBorrowedBooks.Columns.Contains("Penalty"))
            {
                dgvBorrowedBooks.Columns["Penalty"].DefaultCellStyle.Format = "C";
            }
        }
    }
}
