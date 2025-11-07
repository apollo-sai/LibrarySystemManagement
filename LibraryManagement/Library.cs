using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;


public class Library
{
    private string filePath = "books.json";
    public List<Book> Books { get; set; }

    public Library()
    {
        LoadBooks();
    }
    private int nextId = 1;
    public void AssignIds()
    {
        foreach (var book in Books)
        {
            if (book.Id == 0) 
                book.Id = nextId++;
        }
    }


    public void LoadBooks()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
            AssignIds();
        }
        else
        {
            Books = new List<Book>();
            SaveBooks();
        }
    }

    public void SaveBooks()
    {
        string json = JsonConvert.SerializeObject(Books, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
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
