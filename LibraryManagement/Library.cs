using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;


public class Library
{
    private readonly string _rootPath;
    private const string BooksData = "books.json";
    public List<Book> Books { get; set; }

    public Library(string rootPath)
    {
        _rootPath = rootPath;
        LoadBooks();
    }
    private int nextId = 1;

    public void FixLoadedBooks()
    {
        if (Books != null && Books.Any())
        {
            nextId = Books.Max(b => b.Id) + 1;
        }
        else
        {
            nextId = 1;

        }
    }

    public int NextBookID()
    {
        return nextId++;
    }
    


    public void LoadBooks()
    {
        string filePath = Path.Combine(_rootPath, BooksData);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
        }
        else
        {
            Books = new List<Book>();
            File.WriteAllText(filePath, "[]");
        }
        FixLoadedBooks();
    }

    public void SaveBooks()
    {
        string filePath = Path.Combine(_rootPath, BooksData);
        File.WriteAllText(filePath, JsonConvert.SerializeObject(Books, Newtonsoft.Json.Formatting.Indented));
    }

    public void BorrowBook(Book book, string borrower)
    {
        book.IsBorrowed = true;
        book.BorrowedBy = borrower;
        SaveBooks();
    }

    public void ReturnBook(Book book)
    {
        book.IsBorrowed = false;
        book.BorrowedBy = null;
        SaveBooks();
    }
}
