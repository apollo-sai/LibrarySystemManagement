> **LSM: A Library System Management Project**

_Features:_
 1. **Borrowing and Returning Books**
 2. **Admin Login (username, password, email)**
    - Opens an admin dashboard after a successful login.
    - **Admin Dashboard contains:**
      - **Add Book Button:** Allows the Admin to add a book with Title, Author, Year, and Genre.
      - **Remove Book Button:** Allows the Admin to remove a book using a book's ID.
      - **Add Admin Button:** Allows the Admin to add another Admin with Username, Password, and Email.
      - **Logout Button:** Allows the Admin to logout and return to the main dashboard.
 3. **Sending Email Confirmation for Borrowing Books**
    - When borrowing books, the borrower is asked for an email. If an email is invalid, it will not accept it.
 4. **Penalty System:**
    - **Uncharged Borrow Days: First 3-days**
    - **Penalty per day: 70**
    - **Automatic Email Notification every 3-days for penalties.**
 5. **Book Filter System:**
    - Filter by Genre, Author, Year, and Borrowed Status.

**Applications used:**
 1. **SendGrid:** Used for SMTP/Sending Borrow Confirmations and Penalty Notifications.
