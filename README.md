# 📒 Contact Manager - BitsOrchestra Test Task

## 📌 Description
This project is a **Contact Manager** implemented as a test task for a Trainee/Junior .NET Developer position.  

The application provides:
- user registration and authentication,
- importing contacts from CSV files with validation,
- viewing and editing contacts in a table,
- deleting contacts,
- redirecting unauthorized users to the login page.

---

## 📌 Tech Stack
- **ASP.NET MVC**
- **Entity Framework Core**
- **MS SQL Server**
- **jQuery + DataTables** for the table with inline editing
- **Bootstrap 5** for styling
-  IDE: JetBrains Rider

---

## 📌 How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/TrofimishynOleksandr/BitsOrchestraTestTask.git
   cd BitsOrchestraTestTask/BitsOrchestraTestTask
   ```
2. Apply migrations and create the database:
	```bash
    dotnet ef database update
   ```
4. Run the application:
	```bash
    dotnet run
   ```
6. Open in browser by clicking on link in terminal
7. Register new account
8. Use Contacts.csv file as example

---

## 📌 Features Implemented
- Authentication and Authorization
	- Pages are protected with `[Authorize]` attribute.
    - Unauthorized users are redirected to `Account/Login`.
- CSV Import
	- Import contacts for a specific user.
	- Validation of each contact (date, phone, salary, Married flag).
	- Invalid records are skipped, with errors reported back to the user.
- Contact Management
	- DataTables table with search, sorting, and pagination.
	- Inline cell editing with server-side validation.
	- Deletion of contacts.
