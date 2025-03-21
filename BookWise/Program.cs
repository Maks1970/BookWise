﻿using DataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace BookWise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool nenu = true;
            using var ctx = new BooksContext();

            while (true) 
            {
                Console.WriteLine("login(l) or Register(r)");
                var key = Console.ReadLine();
                
                while (key == "l")
                {
                    Console.Write("Login :");
                        var login = Console.ReadLine();
                    Console.Write("Password :");
                    var pass = Console.ReadLine();
                    var user = ctx.Employees
                        .FirstOrDefault(l => EF.Functions.Collate(l.Login, "SQL_Latin1_General_CP1_CS_AS") == login);
                    if (user != null && user.Password == pass)
                    {
                        var s = user.GetType().Name;
                        string discriminator = ctx.Entry(user).Property("Discriminator").CurrentValue as string;
                        if (discriminator == "Employee")
                        {
                            Console.WriteLine("Signing in successful! Librarian");
                           
                            var librarian = new LibrarianService(user,ctx);
                            while (nenu)
                            {
                                LibrarianService.ShowLibrarianMenu();
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        Console.WriteLine("b/a");
                                        switch (Console.ReadLine())
                                        {
                                            case "b":
                                                librarian.ConsoleShowBooks();
                                                Console.WriteLine("Search a/b?");
                                                librarian.SearchBoks(Console.ReadLine(),Console.ReadLine());
                                                break;
                                            case "a":
                                                librarian.ConsoleShowAuthors();
                                                break;
                                        }
                                        break;
                                   
                                    case "2":
                                        bool addKey = true;
                                        while (addKey)
                                        {
                                            Console.WriteLine("Books or Autors? (b/a)(or press Enter to keep current)");
                                            switch (Console.ReadLine())
                                            {
                                                case "b":
                                                    librarian.ConsoleAddBooks();
                                                    break;
                                                case "a":
                                                    librarian.ConsoleAddAuthor();
                                                    break;
                                                default:
                                                    addKey = false;
                                                    break;
                                            }
                                        }
                                        break;
                                   
                                    case "3":
                                        Console.WriteLine("Books or Autors? (b/a)");
                                        switch (Console.ReadLine())
                                            {
                                                case "b":
                                                    librarian.UpdateBooks();
                                                    break;
                                                case "a":
                                                librarian.ConsoleUpdateAuthor();
                                                break;
                                                default:
                                                    addKey = false;
                                                    break;
                                            }
                                        break;
                                    case "4":
                                        Console.WriteLine("Add/edit/delete?");
                                        switch (Console.ReadLine())
                                        {
                                            case "Add":
                                                while (!librarian.AddReader())
                                                {
                                                    Console.WriteLine("Try again? (y/n) ");
                                                    if (Console.ReadLine() == "n") break;
                                                }
                                                break;
                                            case "edit":
                                                librarian.EditteReader();
                                                break;
                                            case "delete":
                                                Console.WriteLine("Wat login");
                                                librarian.ConsoleDeleteReader(Console.ReadLine());
                                                break;
                                        }
                                        break;
                                    case "5":
                                        Console.WriteLine("1.Only debtors and what they should");
                                        Console.WriteLine("2.Everyone who took books and which books (including debtors)");
                                        Console.WriteLine("3.History of borrowing and returning books by specific reader ( number of overdues)");
                                        switch (Console.ReadLine())
                                        {
                                            case "1":
                                                librarian.ConsoleDebtorsReaders();
                                                break;
                                            case "2":
                                                librarian.ConsoleReaderTookBooks();
                                                break;
                                            case "3":
                                                librarian.ConsoleHistoryOfBorrowing();
                                                break;
                                        }
                                        break;
                                }
                            }
                        }
                        if (discriminator == "Reader")
                        {
                            Console.WriteLine("Signing in successful! Reader");
                            var reader = new ReaderService(user as Reader,ctx);
                            
                            while (nenu)
                            {
                                ReaderService.ShowReaderMenu();
                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        reader.ShowBooks(reader.BorrowBook());

                                        Console.WriteLine("Take a book?y/n");
                                        if (Console.ReadLine()=="y")
                                        {
                                            Console.WriteLine("What number?");
                                            reader.ConsoleTakeBook(Convert.ToInt32(Console.ReadLine()));
                                        }
                                        Console.WriteLine("Search book?y/n");
                                        if (Console.ReadLine() == "y")
                                        {
                                            Console.WriteLine("ConsoleSearchBoks by author(a), by title(t)(key somsth)");
                                            reader.ConsoleSearchBoks(Console.ReadLine(), Console.ReadLine());
                                        }
                                            
                                        break;
                                    case "2":
                                        reader.AboutAuthors();
                                        break;
                                    case "3":
                                        reader.ConsoleBorrowedBooks();
                                        break;
                                    case "4":
                                        reader.BorrowBook();
                                        Console.WriteLine("What number?");
                                        try
                                        {
                                            int bookId = Convert.ToInt32(Console.ReadLine());
                                            reader.ConsoleTakeBook(bookId);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Invalid book index!");
                                        }
                                        break;
                                    default:
                                        nenu = false;
                                        break;
                                }

                            }
                        }

                        key = Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Invalid login or password");
                    }
                }
                while (key == "r")
                {
                    Console.WriteLine("Librarian(l) or Reader(r) ");
                    key= Console.ReadLine();
                    while (key == "l")
                    {
                        var newEmployee = new Employee();
                        Console.WriteLine("Login");
                        newEmployee.Login = Console.ReadLine()!;
                        Console.WriteLine("Password");
                        newEmployee.Password = Console.ReadLine()!;
                        Console.WriteLine("Email");
                        newEmployee.Email = Console.ReadLine()!;
                        var existingEmployee = ctx.Employees.FirstOrDefault(u => u.Login == newEmployee.Login || u.Email == newEmployee.Email);
                        if (existingEmployee != null)
                        {
                            Console.WriteLine("A user with this login or email already exists.");
                        }
                        else
                        {
                            
                            ctx.Employees.Add(newEmployee);
                            ctx.SaveChanges();
                            Console.WriteLine("Registration was successful!");
                            key = "b";
                        }
                    }
                    while (key == "r")
                    {
                        key = ReaderService.ConsoleRegReader(ctx)? "b":"r";
                    }
                }
                Console.WriteLine();
            } 
        }
    }
}
