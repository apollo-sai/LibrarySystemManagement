using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Mail;
using System.Net; 


namespace LibraryManagement
{
    public partial class MainForm : Form
    {
        private bool isAdmin = false;

        private Library library;

        //Book Info
        public class BookDisplayItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string Genre { get; set; }
            public int YearPublished { get; set; }
            public string Status { get; set; }
        }

        public MainForm()
        {
            InitializeComponent();
            library = new Library();

            FilterDropDown();
            BooksToGrid();
            LoadAdmins();

            btnAddBook.Visible = false;
            btnRemoveBook.Visible = false;
            btnAddAdmin.Visible = false;
            btnAdminLogout.Visible = false;



            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbGenre.SelectedIndexChanged += CmbGenre_SelectedIndexChanged;
            cmbAuthor.SelectedIndexChanged += CmbAuthor_SelectedIndexChanged;
            cmbYear.SelectedIndexChanged += CmbYear_SelectedIndexChanged;
            cmbAvailability.SelectedIndexChanged += CmbAvailability_SelectedIndexChanged;
            btnClearFilters.Click += BtnClearFilters_Click;
        }
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.ContextMenuStrip cmsSort;

        private string currentSortCategory = "Id";
        private bool isAscending = true;


        private void SendBorrowEmail(string recipientEmail, Book book, string borrowerName)
        {
            try
            {
                string Mail = "noreply.lsm.notification@gmail.com";
                string APIKey = "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"; 
                string smtpHost = "smtp.sendgrid.net";
                int smtpPort = 587;
                string currentDate = DateTime.Now.ToString("MMMM dd, yyyy");
                MailMessage message = new MailMessage();
                message.From = new MailAddress(Mail);
                message.Subject = "Book Borrowed Successfully!";
                message.To.Add(new MailAddress(recipientEmail));
                message.Body = $"Dear {borrowerName},\n\n" +
                               $"This is a confirmation that you have successfully borrowed the book:\n\n" +
                               $"Title: {book.Title}\n" +
                               $"Author: {book.Author}\n\n" +
                               $"Date: {currentDate}\n\n" +
                               $"Please return the book on time.\n\n" +
                               $"Thank you,\n" +
                               $"The LSM Team";
                message.IsBodyHtml = false;

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential("apikey", APIKey),
                    EnableSsl = true,
                };
                smtpClient.Send(message);
                MessageBox.Show($"Book borrowed successfully and a confirmation email has been sent to {recipientEmail}. Kindly check your spam folder!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Book borrowed successfully, but the email failed to send: {ex.Message}", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
        private bool isAdminLoggedIn = false;

        private void CmbGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbAuthor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbAvailability_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            ClearFilters();
        }
        private List<Admin> admins;

        private void LoadAdmins()
        {
            string file = "admins.json";
            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);
                admins = JsonConvert.DeserializeObject<List<Admin>>(json);
            }
            else
            {
                admins = new List<Admin>();
            }
        }


        private void BooksToGrid(List<Book> books=null)
        {
            var listToShow = books ?? library.Books;

            List<BookDisplayItem> displayList = new List<BookDisplayItem>();

            foreach (Book b in listToShow)
            {
                string statusText;
                if (b.IsBorrowed)
                {
                    statusText = "Borrowed by: " + b.BorrowedBy;
                }
                else
                {
                    statusText = "Available";
                }

                displayList.Add(new BookDisplayItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    YearPublished = b.YearPublished,
                    Status = statusText
                });
            }
            

            dataGridView1.DataSource = displayList;
            

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }


        private void ApplyFilters()
        {
            List<Book> filtered = new List<Book>();

            foreach (Book book in library.Books)
            {
                filtered.Add(book);
            }
            //anlopit
            if (!string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != "Search title/author...")
            {
                string query = txtSearch.Text.ToLower();
                List<Book> temp = new List<Book>();

                foreach (Book book in filtered)
                {
                    if (book.Title.ToLower().Contains(query) || book.Author.ToLower().Contains(query))
                    {
                        temp.Add(book);
                    }
                }

                filtered = temp;
            }
            if (cmbGenre.SelectedIndex > 0)
            {
                string selectedGenre = cmbGenre.SelectedItem.ToString();
                List<Book> temp = new List<Book>();

                foreach (Book book in filtered)
                {
                    if (book.Genre == selectedGenre)
                    {
                        temp.Add(book);
                    }
                }

                filtered = temp;
            }
            if (cmbAuthor.SelectedIndex > 0)
            {
                string selectedAuthor = cmbAuthor.SelectedItem.ToString();
                List<Book> temp = new List<Book>();

                foreach (Book book in filtered)
                {
                    if (book.Author == selectedAuthor)
                    {
                        temp.Add(book);
                    }
                }

                filtered = temp;
            }
            if (cmbYear.SelectedIndex > 0)
            {
                string selectedYear = cmbYear.SelectedItem.ToString();
                List<Book> temp = new List<Book>();

                foreach (Book book in filtered)
                {
                    if (book.YearPublished.ToString() == selectedYear)
                    {
                        temp.Add(book);
                    }
                }

                filtered = temp;
            }

            if (cmbAvailability.SelectedIndex > 0)
            {
                string selectedAvailability = cmbAvailability.SelectedItem.ToString();
                List<Book> temp = new List<Book>();

                foreach (Book book in filtered)
                {
                    if (selectedAvailability == "Available" && !book.IsBorrowed)
                    {
                        temp.Add(book);
                    }
                    else if (selectedAvailability == "Borrowed" && book.IsBorrowed)
                    {
                        temp.Add(book);
                    }
                }

                filtered = temp;
            }List<object> displayList = new List<object>();

            foreach (Book book in filtered)
            {
                string status = book.IsBorrowed ? $"Borrowed by {book.BorrowedBy}" : "Available";

                displayList.Add(new
                {
                    book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    YearPublished = book.YearPublished,
                    Status = status
                });
            }

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = displayList;
        }


        private void btnSort_Click(object sender, EventArgs e)
        {
            cmsSort.Show(btnSort, new System.Drawing.Point(0, btnSort.Height));
        }

        private void cmsSort_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string clickedCategory = e.ClickedItem.Text;

            if (currentSortCategory == clickedCategory)
            {
                isAscending = !isAscending; // pangtoggle order
            }
            else
            {
                currentSortCategory = clickedCategory;
                isAscending = true; // default ascending
            }

            ApplySort();
        }

        private void ApplySort()
        {
            IEnumerable<Book> sorted;

            switch (currentSortCategory)
            {
                case "Title":
                    sorted = isAscending ? library.Books.OrderBy(b => b.Title) : library.Books.OrderByDescending(b => b.Title);
                    break;
                case "Author":
                    sorted = isAscending ? library.Books.OrderBy(b => b.Author) : library.Books.OrderByDescending(b => b.Author);
                    break;
                case "Genre":
                    sorted = isAscending ? library.Books.OrderBy(b => b.Genre) : library.Books.OrderByDescending(b => b.Genre);
                    break;
                case "YearPublished":
                    sorted = isAscending ? library.Books.OrderBy(b => b.YearPublished) : library.Books.OrderByDescending(b => b.YearPublished);
                    break;
                case "Status":
                    sorted = isAscending ? library.Books.OrderBy(b => b.IsBorrowed) : library.Books.OrderByDescending(b => b.IsBorrowed);
                    break;
                default:
                    sorted = isAscending ? library.Books.OrderBy(b => b.Id) : library.Books.OrderByDescending(b => b.Id);
                    break;
            }

            BooksToGrid(sorted.ToList());
        }


        private void FilterDropDown()
        {   cmbGenre.Items.Clear();
            cmbGenre.Items.Add("All");

            List<string> genres = new List<string>();
            foreach (Book book in library.Books)
            {
                if (!genres.Contains(book.Genre))
                {
                    genres.Add(book.Genre);
                }
            }

            foreach (string genre in genres)
            {
                cmbGenre.Items.Add(genre);
            }

            cmbGenre.SelectedIndex = 0;
            cmbAuthor.Items.Clear();
            cmbAuthor.Items.Add("All");

            List<string> authors = new List<string>();
            foreach (Book book in library.Books)
            {
                if (!authors.Contains(book.Author))
                {
                    authors.Add(book.Author);
                }
            }

            foreach (string author in authors)
            {
                cmbAuthor.Items.Add(author);
            }

            cmbAuthor.SelectedIndex = 0;
            cmbYear.Items.Clear();
            cmbYear.Items.Add("All");

            List<string> years = new List<string>();
            foreach (Book book in library.Books)
            {
                string year = book.YearPublished.ToString();
                if (!years.Contains(year))
                {
                    years.Add(year);
                }
            }

            foreach (string year in years)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedIndex = 0;
            cmbAvailability.Items.Clear();
            cmbAvailability.Items.Add("All");
            cmbAvailability.Items.Add("Available");
            cmbAvailability.Items.Add("Borrowed");
            cmbAvailability.SelectedIndex = 0;
        }

        private void ClearFilters()
        {
            txtSearch.Clear();
            cmbGenre.SelectedIndex = 0;
            cmbAuthor.SelectedIndex = 0;
            cmbYear.SelectedIndex = 0;
            cmbAvailability.SelectedIndex = 0;
            ApplyFilters();
        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int bookId = (int)dataGridView1.CurrentRow.Cells["Id"].Value;
            Book selectedBook = library.Books.First(b => b.Id == bookId);

            if (selectedBook.IsBorrowed)
            {
                MessageBox.Show("This book is already borrowed.");
                return;
            }

            string borrower = Interaction.InputBox("Enter borrower's name:", "Borrow Book", "");
            if (string.IsNullOrWhiteSpace(borrower))
            {
                MessageBox.Show("Borrower name is required.");
                return;
            }

            string borrowerEmail = Interaction.InputBox("Enter borrower's email address for confirmation:", "Borrow Book", "");
            if (string.IsNullOrWhiteSpace(borrowerEmail))
            {
                MessageBox.Show("Email address is required for confirmation.");
                return;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(borrowerEmail);
            }
            catch
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            library.BorrowBook(selectedBook, borrower);
            selectedBook.BorrowDate = DateTime.Now;
            library.SaveBooks();
            BooksToGrid();
            SendBorrowEmail(borrowerEmail, selectedBook, borrower);

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int bookId = (int)dataGridView1.CurrentRow.Cells["Id"].Value;
            Book selectedBook = library.Books.First(b => b.Id == bookId);

            if (!selectedBook.IsBorrowed)
            {
                MessageBox.Show("This book is not borrowed.");
                return;
            }

            library.ReturnBook(selectedBook);
            selectedBook.BorrowDate = null;
            library.SaveBooks();
            BooksToGrid();
            MessageBox.Show("Book returned successfully!");
        }
        private void btnAdminLogin_Click(object sender, EventArgs e)
        {
            string username = Interaction.InputBox("Enter admin username:", "Admin Login", "");
            string password = Interaction.InputBox("Enter password:", "Admin Login", "");

            if (admins.Any(a => a.Username == username && a.Password == password))
            {
                isAdminLoggedIn = true;
                btnAddBook.Visible = true;
                btnAddBook.Enabled = true;
                btnRemoveBook.Visible = true;
                btnRemoveBook.Enabled = true;
                btnAddAdmin.Visible = true;
                btnAdminLogout.Visible = true;
                btnAdminLogin.Visible = true;
                btnAdminLogin.Enabled = false;
                MessageBox.Show("Admin login successful! You can now add/remove admins and books.");
            }
            else
            {
                MessageBox.Show("Invalid admin credentials.");
            }
        }
        //private void btnAdminLogout_Click(object sender, EventArgs e)
        //{
        //    btnAddBook.Visible = false;
        //    btnRemoveBook.Visible = false;

        //    btnAdminLogin.Visible = true;

        //    MessageBox.Show("Admin logged out successfully.", "Logout", MessageBoxButtons.OK);
        //}
        private void btnAdminLogout_Click(object sender, EventArgs e)
        {
            isAdminLoggedIn = false;
            btnAddBook.Visible = false;
            btnRemoveBook.Visible = false;
            btnAddAdmin.Visible = false;
            btnAdminLogout.Visible = false;

            btnAdminLogin.Visible = true;
            btnAdminLogin.Enabled = true;

            MessageBox.Show("Admin logged out successfully.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            admins.Add(new Admin { Username = newUser, Password = newPass });

            File.WriteAllText("admins.json", JsonConvert.SerializeObject(admins, Formatting.Indented));

            MessageBox.Show($"Admin '{newUser}' added successfully!");
        }



        private void btnAddBook_Click(object sender, EventArgs e)
        {
            //hide nalang
            //if (!isAdminLoggedIn)
            //{
            //    MessageBox.Show("You must log in as Admin first.");
            //    return;
            //}

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
                Title = title,
                Author = author,
                Genre = genre,
                YearPublished = year,
                IsBorrowed = false,
                BorrowedBy = null
            };

            library.Books.Add(newBook);
            library.SaveBooks();
            FilterDropDown();
            BooksToGrid();

            MessageBox.Show($"Book '{title}' added successfully!");
        }
        private void btnRemoveBook_Click(object sender, EventArgs e)
        {
            //if (!isAdminLoggedIn)
            //{
            //    MessageBox.Show("You must log in as Admin first.");
            //    return;
            //}

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a book from the list to remove.");
                return;
            }

            string title = dataGridView1.SelectedRows[0].Cells["Title"].Value.ToString();

            var confirm = MessageBox.Show($"Are you sure you want to remove '{title}'?",
                                          "Confirm Remove", MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                Book bookToRemove = library.Books
                    .FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

                if (bookToRemove != null)
                {
                    library.Books.Remove(bookToRemove);
                    library.SaveBooks();
                    BooksToGrid();
                    FilterDropDown();
                    MessageBox.Show($"Book '{title}' removed successfully.");
                }
            }
        }

        private void cmsSort_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cmsSort = new ContextMenuStrip();
            cmsSort.Items.Add("Id");
            cmsSort.Items.Add("Title");
            cmsSort.Items.Add("Author");
            cmsSort.Items.Add("Genre");
            cmsSort.Items.Add("YearPublished");
            cmsSort.Items.Add("Status");

            btnSort.Click += btnSort_Click;
            cmsSort.ItemClicked += cmsSort_ItemClicked;
        }
    }
}
