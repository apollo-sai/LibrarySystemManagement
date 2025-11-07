using System;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int YearPublished { get; set; }
    public bool IsBorrowed { get; set; }
    public string BorrowedBy { get; set; }

    public DateTime? BorrowDate { get; set; }
}
