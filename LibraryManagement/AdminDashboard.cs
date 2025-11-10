using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;


namespace LibraryManagement
{
    public partial class AdminDashboard : Form
    {
        private MainForm mainForm;
        private Library library;
        private List<Admin> admins;

        public event EventHandler AdminAction;
        public AdminDashboard(MainForm caller, Library lib, List<Admin> adminlist)
        {
            InitializeComponent();
            this.mainForm = caller;
            this.library = lib;
            this.admins = adminlist;


        }

        private void MainFormRefresh()
        {
            AdminAction?.Invoke(this, EventArgs.Empty);
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {

        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            string title = Interaction.InputBox("Enter book title:", "Add Book", "");

            if (library.Books.Any(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This book title already exists in the system.");
                return;
            }

            string author = Interaction.InputBox("Enter author:", "Add Book", "");
            string genre = Interaction.InputBox("Enter genre:", "Add Book", "");
            string yearInput = Interaction.InputBox("Enter year published:", "Add Book", "");

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre) || string.IsNullOrWhiteSpace(yearInput))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (!int.TryParse(yearInput, out int year))
            {
                MessageBox.Show("Invalid year.");
                return;
            }

            Book newBook = new Book
            {
                Id = library.NextBookID(),
                Title = title,
                Author = author,
                Genre = genre,
                YearPublished = year,
                IsBorrowed = false,
                BorrowedBy = null
            };

            library.Books.Add(newBook);
            library.SaveBooks();
            MainFormRefresh();

            MessageBox.Show($"Book '{title}' added successfully!");
        }

        private void btnRemoveBook_Click(object sender, EventArgs e)
        {
            string bookId = Interaction.InputBox("Enter the ID of the book to remove:", "Remove Book", "");

            if (string.IsNullOrWhiteSpace(bookId))
            {
                return;
            }

            if (!int.TryParse(bookId, out int bookIdRem))
            {
                MessageBox.Show("Invalid input. Please enter a valid book ID.");
                return;
            }

            Book bookToRemove = library.Books.FirstOrDefault(b => b.Id == bookIdRem);

            if (bookToRemove == null)
            {
                MessageBox.Show($"Book ID not found in library.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string bookTitle = bookToRemove.Title;

            var confirm = MessageBox.Show($"Are you sure you want to remove '{bookTitle}'?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                library.Books.Remove(bookToRemove);
                library.SaveBooks();
                MainFormRefresh();

                MessageBox.Show($"Book '{bookTitle}' removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAddAdmin_Click(object sender, EventArgs e)
        {
            string newUser = Interaction.InputBox("Enter new admin username:", "Add Admin", "");
            string newPass = Interaction.InputBox("Enter new admin password:", "Add Admin", "");


            if (string.IsNullOrWhiteSpace(newUser) || string.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("Fields cannot be empty.");
                return;
            }

            if (admins.Any(a => a.Username == newUser))
            {
                MessageBox.Show("That username already exists!");
                return;
            }

            string newEmail = Interaction.InputBox("Enter new admin email address:", "Add Admin", "");
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                MessageBox.Show("Email address is required for confirmation.");
                return;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(newEmail);
            }
            catch
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            admins.Add(new Admin { Username = newUser, Password = newPass, Email = newEmail });

            string rootDIR = mainForm.RootPathAccessor();
            string filePath = Path.Combine(rootDIR, "admins.json");

            File.WriteAllText(filePath, JsonConvert.SerializeObject(admins, Formatting.Indented));

            MessageBox.Show($"Admin '{newUser}' added successfully!");
        }

        private void btnAdminLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            mainForm.Show();
            MessageBox.Show("Successfully logged out.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
