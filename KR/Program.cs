﻿using System.Text;
using System.Text.RegularExpressions;
internal class Program
{

    struct Publication
    {
        int numberOfRegistration;
        DateOnly dateOfRegistration;
        string type, UDC, fullNameOfAuthor, title, fullNameOfReviewer;
        int journalNumber;
        DateOnly magazineReleaseDate;
    }

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.ResetColor();
        Console.Clear();

    }
    static Publication[] ReadPublicationsFromFile(StreamReader srPublication)
    {

    }
    static Publication ReadPublicationFromFile(StreamReader srPublication)
    {
        
    }
    static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("Выберете один из вариантов работы с Базой данных:");
            Console.WriteLine("1) Редактирование БД");
            Console.WriteLine("2) Вывод данных");
            Console.WriteLine("3) Поиск в БД");
            Console.WriteLine("4) Сортировка записей");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("5) Востановить данные из резервного файла");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Выход");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        EditMenu();
                        break;
                    case 2:
                        OutputMenu();
                        break;
                    case 3:
                        SearchMenu();
                        break;
                    case 4:
                        SortMenu();
                        break;
                    case 5:
                        RecoverDataFromReserv();
                        break;
                    case 0:
                        Console.WriteLine("Выход из программы");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }

    static void EditMenu()
    {
        
    }

    static void OutputMenu()
    {

    }

    static void SearchMenu()
    {
        
    }

    static void SortMenu()
    {
        
    }

    static void SaveInFile()
    {

    }
    static void SaveInReservFile()
    {

    }
    static void RecoverDataFromReserv()
    {
        
    }
    static void AddToDataBase()
    {
        
    }
    static void EditInDataBase()
    {
        
    }
    static void DeleteInDataBase()
    {
        
    }
    static void SortStudentsByFullName()
    {
        
    }
    static bool IsValidOption(out int option)
    {
        return IsValidInt(out option, 0);
    }
    static bool IsValidInt(out int option, int? start = null, int? end = null)
    {
        if (int.TryParse(Console.ReadLine(), out option) && (start == null || option >= start) && (end == null || option <= end))
        {
            return true;
        }
        else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверный ввод, повторите попытку ввода");
            Console.ResetColor();
            return false;
        }
    }
    static bool IsValidIntOrNull(out int option, int? start = null, int? end = null)
    {
        string? input = Console.ReadLine();
        option = -1;
        if (input == null || input.Length == 0 || int.TryParse(input, out option) && (start == null || option >= start) && (end == null || option <= end))
        {
            return true;
        }
        else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Неверный ввод, повторите попытку ввода");
            Console.ResetColor();
            return false;
        }
    }
    static bool IsValidByRegexWithConsoleOutput(out string? input, Regex pattern)
    {
        input = Console.ReadLine();
        if (pattern.IsMatch(input!))
        {
            return true;
        }
        else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ввод не соответствует требуемому формату, повторите попытку ввода");
            Console.ResetColor();
            return false;
        }
    }
    static bool IsValidByRegexOrNull(out string? input, Regex pattern)
    {
        input = Console.ReadLine();
        if (pattern.IsMatch(input!) || input!.Length == 0)
        {
            return true;
        }
        else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ввод не соответствует требуемому формату, повторите попытку ввода");
            Console.ResetColor();
            return false;
        }
    }
    static bool IsntNullWithConsoleOutput(out string? input)
    {
        input = Console.ReadLine();
        if (input != null && input.Length != 0)
        {
            return true;
        }
        else
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Это поле не может быть пустым, повторите попытку ввода");
            Console.ResetColor();
            return false;
        }
    }
}