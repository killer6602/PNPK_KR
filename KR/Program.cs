using System.Text;
using System.Text.RegularExpressions;
internal class Program
{

    struct Publication
    {
        public long numberOfRegistration;
        public DateOnly dateOfRegistration;
        public int IDOfType;
        public int IDOfUDC;
        public string fullNameOfAuthor;
        public string title;
        public int IDOfFullNameOfReviewer;
        public int journalNumber;
        public DateOnly magazineReleaseDate;
    }

    struct ReferenceBook
    {
        public long ID;
        public string title;
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.ResetColor();
        Console.Clear();

        StreamReader streamReader = new("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt");
        StreamReader srReferenceBookOfType = new("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            srReferenceBookOfUDC = new("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            srReferenceBookOfFullNameOfReviewer = streamReader,
            srPublication = new("..\\..\\..\\DataFiles\\Publications.txt");
        List<Publication> publications = ReadPublicationsFromFile(srPublication);
        List<ReferenceBook> referenceBookOfType = ReadReferenceBookFromFile(srReferenceBookOfType),
            referenceBookOfUDC = ReadReferenceBookFromFile(srReferenceBookOfUDC),
            referenceBookOfFullNameOfReviewer = ReadReferenceBookFromFile(srReferenceBookOfFullNameOfReviewer);
        srPublication.Close();
        srReferenceBookOfType.Close();
        srReferenceBookOfUDC.Close();
        srReferenceBookOfFullNameOfReviewer.Close();

        StreamWriter streamWriter = new("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfFullNameOfReviewer.txt");
        StreamWriter swReservReferenceBookOfType = new("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfType.txt"),
            swReservReferenceBookOfUDC = new("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfUDC.txt"),
            swReservReferenceBookOfFullNameOfReviewer = streamWriter,
            swReservPublication = new("..\\..\\..\\DataFiles\\ReservFiles\\Publications.txt");
        SaveAllChanges(swReservPublication, publications,
                       swReservReferenceBookOfType, referenceBookOfType,
                       swReservReferenceBookOfUDC, referenceBookOfUDC,
                       swReservReferenceBookOfFullNameOfReviewer, referenceBookOfFullNameOfReviewer);

        MainMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);

        StreamWriter swReferenceBookOfType = new("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            swReferenceBookOfUDC = new("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            swReferenceBookOfFullNameOfReviewer = new("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            swPublication = new("..\\..\\..\\DataFiles\\Publications.txt");
        SaveAllChanges(swPublication, publications,
                       swReferenceBookOfType, referenceBookOfType,
                       swReferenceBookOfUDC, referenceBookOfUDC,
                       swReferenceBookOfFullNameOfReviewer, referenceBookOfFullNameOfReviewer);
    }

    static void MainMenu(List<Publication> publications,
                         List<ReferenceBook> referenceBookOfType,
                         List<ReferenceBook> referenceBookOfUDC,
                         List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        while (true)
        {
            Console.WriteLine("Выберете один из вариантов работы с Базой данных:");
            Console.WriteLine("1) Редактирование БД");
            Console.WriteLine("2) Редактирование справочников");
            Console.WriteLine("3) Вывод данных");
            Console.WriteLine("4) Поиск в БД");
            Console.WriteLine("5) Сортировка записей");
            Console.WriteLine("6) Создать отчёт");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("7) Востановить данные из резервного файла");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Сохранить и выйти");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        EditDBMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 2:
                        EditReferenceBooksMenu(referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 3:
                        OutputMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 4:
                        SearchMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 5:
                        SortMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 6:
                        ReportMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 7:
                        RecoverDataFromFile(ref publications, ref referenceBookOfType, ref referenceBookOfUDC, ref referenceBookOfFullNameOfReviewer);
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

    #region Report
    static void ReportMenu(List<Publication> publications,
                         List<ReferenceBook> referenceBookOfType,
                         List<ReferenceBook> referenceBookOfUDC,
                         List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Выберете один из вариантов создания отчёта:");
            Console.WriteLine("1) По номеру журнала");
            Console.WriteLine("2) По автору");
            Console.WriteLine("3) По рецензенту");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        CreateReportByJournalNumber(publications, referenceBookOfType, referenceBookOfUDC, "..\\..\\..\\Reports\\JournalNumber.txt");
                        break;
                    case 2:
                        CreateReportByAuthor(publications, referenceBookOfType, referenceBookOfUDC, "..\\..\\..\\Reports\\Author.txt");
                        break;
                    case 3:
                        CreateReportByReviewer(publications, referenceBookOfFullNameOfReviewer, "..\\..\\..\\Reports\\Reviewer.txt");
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }

    static void CreateReportByJournalNumber(List<Publication> publications, 
                                            List<ReferenceBook> referenceBookOfType,
                                            List<ReferenceBook> referenceBookOfUDC, 
                                            string path)
    {
        StreamWriter sw = new(path, false, Encoding.UTF8);
        List<Publication> publicationsCopy = publications;
        SortPublicationByJournalNumber(publicationsCopy);
        List<int> journalNumberVariaty = [];
        for (int i = 0; i < publicationsCopy.Count; i++)
        {
            if (!journalNumberVariaty.Exists(x => x == publicationsCopy[i].journalNumber))
            {
                journalNumberVariaty.Add(publicationsCopy[i].journalNumber);
            }
        }
        for (int i = 0; i < journalNumberVariaty.Count; i++)
        {
            List<Publication> result = publicationsCopy.FindAll(x => x.journalNumber == journalNumberVariaty[i]);
            SortPublicationByUDC(result);
            SortPublicationByType(result);
            SortPublicationByFullNameOfAuthor(result);
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine(journalNumberVariaty[i]);
            sw.WriteLine(new string('-', 115));
            sw.WriteLine("|  № |                  ФИО автора                  | УДК |     Тип публикации     |           Название           |");
            sw.WriteLine(new string('-', 115));
            for (int j = 0; j < result.Count; j++)
            {
                sw.WriteLine($"|{j + 1:D4}|" +
                    $"{new String(' ', (47 - result[j].fullNameOfAuthor.Length) / 2) + result[j].fullNameOfAuthor + new String(' ', (46 - result[j].fullNameOfAuthor.Length) / 2)}|" +
                    $" {referenceBookOfUDC.Find(x => x.ID == result[j].IDOfUDC).title} |" +
                    $"{new String(' ', (25 - referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title.Length) / 2) + referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title + new String(' ', (24 - referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title.Length) / 2)}|" +
                    $"{(result[j].title.Trim(' ').Length > 30 ? (result[j].title.Substring(0, 27).Trim() + new string('.', 30 - result[j].title.Substring(0, 27).Trim().Length)) : new String(' ', (30 - result[j].title.Length) / 2) + result[j].title + new String(' ', (31 - result[j].title.Length) / 2))}|");
                sw.WriteLine(new string('-', 115));
            }
            sw.WriteLine();
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine();
        }
        sw.Close();
    }

    static void CreateReportByAuthor(List<Publication> publications, 
                                     List<ReferenceBook> referenceBookOfType, 
                                     List<ReferenceBook> referenceBookOfUDC, 
                                     string path)
    {
        StreamWriter sw = new(path, false, Encoding.UTF8);
        List<Publication> publicationsCopy = publications;
        SortPublicationByFullNameOfAuthor(publications);
        List<string> fullNameOfAuthorVariaty = [];
        for (int i = 0; i < publicationsCopy.Count; i++)
        {
            if (!fullNameOfAuthorVariaty.Exists(x => x == publicationsCopy[i].fullNameOfAuthor))
            {
                fullNameOfAuthorVariaty.Add(publicationsCopy[i].fullNameOfAuthor);
            }
        }
        for (int i = 0; i < fullNameOfAuthorVariaty.Count; i++)
        {
            List<Publication> result = publicationsCopy.FindAll(x => x.fullNameOfAuthor == fullNameOfAuthorVariaty[i]);
            SortPublicationByJournalNumber(result);
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine(fullNameOfAuthorVariaty[i]);
            sw.WriteLine(new string('-', 84));
            sw.WriteLine("|  № | УДК |     Тип публикации     |           Название           | Номер журнала |");
            sw.WriteLine(new string('-', 84));
            for (int j = 0; j < result.Count; j++)
            {
                sw.WriteLine($"|{j + 1:D4}|" +
                    $" {referenceBookOfUDC.Find(x => x.ID == result[j].IDOfUDC).title} |" +
                    $"{new String(' ', (25 - referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title.Length) / 2) + referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title + new String(' ', (24 - referenceBookOfType.Find(x => x.ID == result[j].IDOfType).title.Length) / 2)}|" +
                    $"{(result[j].title.Trim(' ').Length > 30 ? (result[j].title.Substring(0, 27).Trim() + new string('.', 30 - result[j].title.Substring(0, 27).Trim().Length)) : new String(' ', (30 - result[j].title.Length) / 2) + result[j].title + new String(' ', (31 - result[j].title.Length) / 2))}|" +
                    $"{new String(' ', (16 - result[j].journalNumber.ToString().Length) / 2) + result[j].journalNumber + new String(' ', (15 - result[j].journalNumber.ToString().Length) / 2)}|");
                sw.WriteLine(new string('-', 84));
            }
            sw.WriteLine();
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine();
        }
        sw.Close();
    }

    static void CreateReportByReviewer(List<Publication> publications, 
                                       List<ReferenceBook> referenceBookOfFullNameOfReviewer,
                                       string path)
    {
        StreamWriter sw = new(path, false, Encoding.UTF8);
        List<Publication> publicationsCopy = publications;
        SortPublicationByFullNameOfReviewer(publicationsCopy, referenceBookOfFullNameOfReviewer);
        List<string> fullNameOfReviewerVariaty = [];
        for (int i = 0; i < publicationsCopy.Count; i++)
        {
            if (!fullNameOfReviewerVariaty.Exists(x => x == referenceBookOfFullNameOfReviewer.Find(x => x.ID == publicationsCopy[i].IDOfFullNameOfReviewer).title))
            {
                fullNameOfReviewerVariaty.Add(referenceBookOfFullNameOfReviewer.Find(x => x.ID == publicationsCopy[i].IDOfFullNameOfReviewer).title);
            }
        }
        for (int i = 0; i < fullNameOfReviewerVariaty.Count; i++)
        {
            List<Publication> result = publicationsCopy.FindAll(x => referenceBookOfFullNameOfReviewer.Find(y => y.ID == x.IDOfFullNameOfReviewer).title == fullNameOfReviewerVariaty[i]);
            SortPublicationByJournalNumber(result);
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine(fullNameOfReviewerVariaty[i]);
            sw.WriteLine(new string('-', 35));
            sw.WriteLine("|  № | Номер журнала | Количество |");
            sw.WriteLine(new string('-', 35));
            for (int j = 0; j < result.Count; j++)
            {
                if (j > 0 && result[j].journalNumber == result[j - 1].journalNumber)
                {
                    continue;
                }
                sw.WriteLine($"|{j + 1:D4}|" +
                    $"{new String(' ', (16 - result[j].journalNumber.ToString().Length) / 2) + result[j].journalNumber + new String(' ', (15 - result[j].journalNumber.ToString().Length) / 2)}|" +
                    $"{new String(' ', (13 - result.FindAll(x => x.journalNumber == result[j].journalNumber).Count.ToString().Length) / 2) + result.FindAll(x => x.journalNumber == result[j].journalNumber).Count + new String(' ', (12 - result.FindAll(x => x.journalNumber == result[j].journalNumber).Count.ToString().Length) / 2)}|");
                sw.WriteLine(new string('-', 35));
            }
            sw.WriteLine();
            sw.WriteLine(new string('▬', 120));
            sw.WriteLine();
            sw.WriteLine();
        }
        sw.Close();
    }
    #endregion

    #region Read
    static List<Publication> ReadPublicationsFromFile(StreamReader srPublication)
    {
        Type type = new Publication().GetType();
        int n = srPublication.ReadToEnd().Split('\n').Length / type.GetFields().Length;//получение количества объектов в файле
        srPublication.BaseStream.Position = 0;//возвращение указателя StreamReader в начало файла

        List<Publication> publications = [];
        for (int i = 0; i < n; i++)
        {
            publications.Add(ReadPublicationFromFile(srPublication));
        }
        return publications;
    }
    static Publication ReadPublicationFromFile(StreamReader srPublication)//нету проверок т.к. файл доверенный источник и в нём нет ошибок
    {
        Publication publication = new()
        {
            numberOfRegistration = Convert.ToInt64(srPublication.ReadLine()),
            dateOfRegistration = DateOnly.Parse(srPublication.ReadLine()!),
            IDOfType = Convert.ToInt16(srPublication.ReadLine()),
            IDOfUDC = Convert.ToInt16(srPublication.ReadLine()),
            fullNameOfAuthor = srPublication.ReadLine()!,
            title = srPublication.ReadLine()!,
            IDOfFullNameOfReviewer = Convert.ToInt16(srPublication.ReadLine()),
            journalNumber = Convert.ToInt32(srPublication.ReadLine()),
            magazineReleaseDate = DateOnly.Parse(srPublication.ReadLine()!)
        };
        return publication;
    }
    static List<ReferenceBook> ReadReferenceBookFromFile(StreamReader srReferenceBook)
    {
        int n = srReferenceBook.ReadToEnd().Split('\n').Length;//получение количества объектов в файле
        srReferenceBook.BaseStream.Position = 0;//возвращение указателя StreamReader в начало файла

        List<ReferenceBook> referenceBook = [];
        ReferenceBook item;
        for (int i = 0; i < n; i++)
        {
            item.title = srReferenceBook.ReadLine()!;
            item.ID = i;
            referenceBook.Add(item);//при считывании пустые строки считаются валидными!!!!
        }
        return referenceBook;
    }
    #endregion

    #region Edit
    static void EditDBMenu(List<Publication> publications,
                           List<ReferenceBook> referenceBookOfType,
                           List<ReferenceBook> referenceBookOfUDC,
                           List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        while (flag)
        {
            Console.Clear();
            Console.WriteLine("Выберете один из вариантов изменения данных:");
            Console.WriteLine("1) Добавление");
            Console.WriteLine("2) Изменение");
            Console.WriteLine("3) Удаление");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        Publication publication = ReadPublicationFromConsole(referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        publications.Add(publication);
                        break;
                    case 2:
                        do
                        {
                            Console.Clear();
                            WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                            Console.WriteLine($"Введите номер публикации которую хотите изменить (1-{publications.Count})");
                        } while (!IsValid(out option, 1, publications.Count));
                        option--;
                        publications[option] = ReadPublicationFromConsoleOrLeaveAsItIs(publications[option], referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 3:
                        do
                        {
                            Console.Clear();
                            WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                            Console.WriteLine($"Введите номер публикации которую хотите удалить (1-{publications.Count})");
                        } while (!IsValid(out option, 1, publications.Count));
                        option--;
                        publications.RemoveAt(option);
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
    static Publication ReadPublicationFromConsole(List<ReferenceBook> referenceBookOfType,
                                                  List<ReferenceBook> referenceBookOfUDC,
                                                  List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        Regex pattern = new(@"([А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\b)|([A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\b)");
        Publication newPublication = new();

        long number;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите регистрационный номер");
        } while (!IsValid(out number));
        newPublication.numberOfRegistration = number;

        DateOnly date;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите дату регистрации публикации");
        } while (!IsValid(out date));
        newPublication.dateOfRegistration = date;

        newPublication.IDOfType = GetChosenIDOfReferenceBook(referenceBookOfType, "Типа публикации");

        newPublication.IDOfUDC = GetChosenIDOfReferenceBook(referenceBookOfUDC, "УДК");

        string text;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите ФИО автора");
        } while (!IsValidByRegex(out text!, pattern));
        newPublication.fullNameOfAuthor = text;

        Console.Clear();
        do
        {
            Console.WriteLine("Введите заголовок публикации");
        } while (!IsntNullWithConsoleOutput(out text!));
        newPublication.title = text;

        newPublication.IDOfFullNameOfReviewer = GetChosenIDOfReferenceBook(referenceBookOfFullNameOfReviewer, "ФИО рецензента");

        int journalNumber;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите номер журнала");
        } while (!IsValid(out journalNumber));
        newPublication.journalNumber = journalNumber;

        Console.Clear();
        do
        {
            Console.WriteLine("Введите дату релиза в магазин");
        } while (!IsValid(out date));
        newPublication.magazineReleaseDate = date;
        return newPublication;
    }
    static Publication ReadPublicationFromConsoleOrLeaveAsItIs(Publication publication,
                                                               List<ReferenceBook> referenceBookOfType,
                                                               List<ReferenceBook> referenceBookOfUDC,
                                                               List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        Regex pattern = new(@"([А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\b)|([A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\b)");
        Publication newPublication = new();

        long number;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите регистрационный номер или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.numberOfRegistration}");
            Console.ResetColor();
        } while (!IsValidOrNull(out number));
        newPublication.numberOfRegistration = number == -1 ? publication.numberOfRegistration : number;

        DateOnly date;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите дату регистрации публикации или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.dateOfRegistration}");
            Console.ResetColor();
        } while (!IsValidOrNull(out date));
        newPublication.dateOfRegistration = date == new DateOnly(1, 1, 1) ? publication.dateOfRegistration : date;

        newPublication.IDOfType = GetChosenIDOfReferenceBook(referenceBookOfType, "Типа публикации");

        newPublication.IDOfUDC = GetChosenIDOfReferenceBook(referenceBookOfUDC, "УДК");

        string text;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите ФИО автора или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.fullNameOfAuthor}");
            Console.ResetColor();
        } while (!IsValidByRegexOrNull(out text!, pattern));
        newPublication.fullNameOfAuthor = text ?? publication.fullNameOfAuthor;

        Console.Clear();
        do
        {
            Console.WriteLine("Введите заголовок публикации или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.title}");
            Console.ResetColor();
        } while (!IsntNullWithConsoleOutput(out text!));
        newPublication.title = text ?? publication.title;

        newPublication.IDOfFullNameOfReviewer = GetChosenIDOfReferenceBook(referenceBookOfFullNameOfReviewer, "ФИО рецензента");

        int journalNumber;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите номер журнала или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.journalNumber}");
            Console.ResetColor();
        } while (!IsValidOrNull(out journalNumber));
        newPublication.journalNumber = journalNumber == -1 ? publication.journalNumber : journalNumber;

        Console.Clear();
        do
        {
            Console.WriteLine("Введите дату релиза в магазин или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.magazineReleaseDate}");
            Console.ResetColor();
        } while (!IsValidOrNull(out date));
        newPublication.magazineReleaseDate = date == new DateOnly(1, 1, 1) ? publication.magazineReleaseDate : date;
        return newPublication;
    }

    static void EditReferenceBooksMenu(List<ReferenceBook> referenceBookOfType,
                                       List<ReferenceBook> referenceBookOfUDC,
                                       List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        while (flag)
        {
            Console.Clear();
            Console.WriteLine("Выберете с каким справочником работать:");
            Console.WriteLine("1) Типы публикации");
            Console.WriteLine("2) УДК");
            Console.WriteLine("3) Имя рецензента");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        EditChosenReferenceBooksMenu(referenceBookOfType, "Тип публикации");
                        break;
                    case 2:
                        EditChosenReferenceBooksMenu(referenceBookOfUDC, "УДК", @"\b\d{3}\b");
                        break;
                    case 3:
                        EditChosenReferenceBooksMenu(referenceBookOfFullNameOfReviewer, "ФИО рецензента",
                            @"([А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\s{1}[А-Я]{1}[а-я]{1,}\b)|([A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\s{1}[A-Z]{1}[a-z]{1,}\b)");
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
    static void EditChosenReferenceBooksMenu(List<ReferenceBook> referenceBook, 
                                             string text, 
                                             string pattern = @"\w")
    {
        bool flag = true;
        while (flag)
        {
            Console.Clear();
            Console.WriteLine("Выберете один из вариантов изменения данных справочников:");
            Console.WriteLine("1) Добавление");
            Console.WriteLine("2) Изменение");
            Console.WriteLine("3) Удаление");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        ReferenceBook newReferenceBook = ReadReferenceBookFromConsole(referenceBook, text, pattern);
                        referenceBook.Add(newReferenceBook);
                        break;
                    case 2:
                        do
                        {
                            Console.Clear();
                            WriteReferenceBook(referenceBook, text);
                            Console.WriteLine($"Введите ID справочника который хотите изменить");
                        } while (!IsValid(out option) || !referenceBook.Exists(x => x.ID == option));
                        referenceBook[referenceBook.FindIndex(x => x.ID == option)] = ReadReferenceBookFromConsoleOrLeaveAsItIs(referenceBook, option, text, pattern);
                        break;
                    case 3:
                        do
                        {
                            Console.Clear();
                            WriteReferenceBook(referenceBook, text);
                            Console.WriteLine($"Введите ID справочника который хотите удалить");
                        } while (!IsValid(out option));
                        referenceBook.RemoveAt(option);
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
    static ReferenceBook ReadReferenceBookFromConsole(List<ReferenceBook> referenceBook, 
                                                      string name, 
                                                      string pattern = @"\w")
    {
        Regex regex = new(pattern);
        ReferenceBook newReferenceBook = new();
        string text;
        Console.Clear();
        do
        {
            Console.WriteLine($"Введите новое название \"{name}\"");
        } while (!IsValidByRegex(out text!, regex) || referenceBook.Exists(x => x.title.Equals(text)));
        newReferenceBook.title = text;
        newReferenceBook.ID = GetNewId(referenceBook);
        return newReferenceBook;
    }
    static ReferenceBook ReadReferenceBookFromConsoleOrLeaveAsItIs(List<ReferenceBook> referenceBook,
                                                                   int option,
                                                                   string name,
                                                                   string pattern = @"\w")
    {
        Regex regex = new(pattern);
        ReferenceBook newReferenceBook = new();
        string? text;
        Console.Clear();
        do
        {
            Console.WriteLine($"Введите новое название \"{name}\" или нажмите Enter чтобы оставить как есть");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {referenceBook[option].title}");
            Console.ResetColor();
        } while (!IsValidByRegex(out text!, regex) || referenceBook.Exists(x => x.title.Equals(text)));
        newReferenceBook.title = text == null || text.Length == 0 ? referenceBook[option].title : text;
        newReferenceBook.ID = referenceBook[option].ID;
        return newReferenceBook;
    }
    #endregion

    #region Output
    static void OutputMenu(List<Publication> publications,
                           List<ReferenceBook> referenceBookOfType,
                           List<ReferenceBook> referenceBookOfUDC,
                           List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Выберете один из вариантов вывода данных:");
            Console.WriteLine("1) Вывод содержимого базы данных");
            Console.WriteLine("2) Вывод содержимого справочника о типах публикации");
            Console.WriteLine("3) Вывод содержимого справочника о УДК");
            Console.WriteLine("4) Вывод содержимого справочника о рецензентах");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 2:
                        WriteReferenceBook(referenceBookOfType, "Тип публикации");
                        break;
                    case 3:
                        WriteReferenceBook(referenceBookOfUDC, "УДК");
                        break;
                    case 4:
                        WriteReferenceBook(referenceBookOfFullNameOfReviewer, "ФИО рецензента");
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }


    static void WritePublication(List<Publication> publications,
                                 List<ReferenceBook> referenceBookOfType,
                                 List<ReferenceBook> referenceBookOfUDC,
                                 List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        for (int i = 0; i < publications.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{new string('▬', Console.BufferWidth)}");
            Console.ResetColor();
            Console.WriteLine($"№{i + 1}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{new string('.', Console.BufferWidth)}");
            Console.ResetColor();
            WritePublication(publications[i], referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{new string('#', Console.BufferWidth)}");
        Console.ResetColor();
        Console.WriteLine();
    }
    static void WritePublication(Publication publication,
                                 List<ReferenceBook> referenceBookOfType,
                                 List<ReferenceBook> referenceBookOfUDC,
                                 List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        Console.WriteLine();
        Console.WriteLine($"Регистрационный номер: {publication.numberOfRegistration}");
        Console.WriteLine($"Дата регистрации: {publication.dateOfRegistration}");
        Console.WriteLine($"Тип публикации: {referenceBookOfType[referenceBookOfType.FindIndex(x => x.ID == publication.IDOfType)].title}");
        Console.WriteLine($"УДК: {referenceBookOfUDC[referenceBookOfUDC.FindIndex(x => x.ID == publication.IDOfUDC)].title}");
        Console.WriteLine($"ФИО автора: {publication.fullNameOfAuthor}");
        Console.WriteLine($"Название: {publication.title}");
        Console.WriteLine($"ФИО рецензента: {referenceBookOfFullNameOfReviewer[referenceBookOfFullNameOfReviewer.FindIndex(x => x.ID == publication.IDOfFullNameOfReviewer)].title}");
        Console.WriteLine($"Номер журнала: {publication.journalNumber}");
        Console.WriteLine($"Дата выхода выпуска в магазин: {publication.magazineReleaseDate}");
        Console.WriteLine();
    }

    static void WriteReferenceBook(List<ReferenceBook> referenceBook, string text)
    {
        Console.WriteLine($"| ID |\t{text}");
        Console.WriteLine($"{new string('-', Console.BufferWidth)}");
        for (int i = 0; i < referenceBook.Count; i++)
        {
            WriteReferenceBook(referenceBook[i]);
        }
        Console.WriteLine();
    }
    static void WriteReferenceBook(ReferenceBook referenceBook)
    {
        Console.WriteLine($"|{referenceBook.ID:D4}|\t{referenceBook.title}");
    }
    #endregion

    #region Search
    static void SearchMenu(List<Publication> publications,
                           List<ReferenceBook> referenceBookOfType,
                           List<ReferenceBook> referenceBookOfUDC,
                           List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        List<Publication> result;
        while (flag)
        {
            Console.WriteLine("Выберете один из вариантов поиска:");
            Console.WriteLine("1) По дате регистрации");
            Console.WriteLine("2) По типу публикации");
            Console.WriteLine("3) По УДК");
            Console.WriteLine("4) По названию");
            Console.WriteLine("5) По номеру журнала");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        DateOnly startDate, endDate;
                        Console.Clear();
                        do
                        {
                            Console.WriteLine("Введите с какой даты искать");
                        } while (!IsValid(out startDate));
                        Console.Clear();
                        do
                        {
                            Console.WriteLine("Введите до какой даты искать");
                        } while (!IsValid(out endDate));

                        result = FindAllPublicationByRegestrationDate(publications, startDate, endDate);
                        Console.Clear();
                        if (result.Count == 0)
                        {
                            Console.WriteLine("Не найдёно ни одной записи соответствующей поиску :(");
                        }
                        else
                        {
                            WritePublication(result, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        }
                        break;
                    case 2:
                        int typeOfPublication = GetChosenIDOfReferenceBook(referenceBookOfType, "Тип публикации");

                        result = FindAllPublicationByType(publications, typeOfPublication);
                        Console.Clear();
                        if (result.Count == 0)
                        {
                            Console.WriteLine("Не найдёно ни одной записи соответствующей поиску :(");
                        }
                        else
                        {
                            WritePublication(result, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        }
                        break;
                    case 3:
                        int UDC = GetChosenIDOfReferenceBook(referenceBookOfUDC, "Тип публикации");

                        result = FindAllPublicationByUDC(publications, UDC);
                        Console.Clear();
                        if (result.Count == 0)
                        {
                            Console.WriteLine("Не найдёно ни одной записи соответствующей поиску :(");
                        }
                        else
                        {
                            WritePublication(result, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        }
                        break;
                    case 4:
                        string text;
                        Console.Clear();
                        do
                        {
                            Console.WriteLine("Введите что искать в названии статьи");
                        } while (!IsntNullWithConsoleOutput(out text!));

                        result = FindAllPublicationByTitle(publications, text);
                        Console.Clear();
                        if (result.Count == 0)
                        {
                            Console.WriteLine("Не найдёно ни одной записи соответствующей поиску :(");
                        }
                        else
                        {
                            WritePublication(result, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        }
                        break;
                    case 5:
                        int journalNumber;
                        Console.Clear();
                        do
                        {
                            Console.WriteLine("Введите номер журнала");
                        } while (!IsValid(out journalNumber));

                        result = FindAllPublicationByJournalNumber(publications, journalNumber);
                        Console.Clear();
                        if (result.Count == 0)
                        {
                            Console.WriteLine("Не найдёно ни одной записи соответствующей поиску :(");
                        }
                        else
                        {
                            WritePublication(result, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        }
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
    static List<Publication> FindAllPublicationByRegestrationDate(List<Publication> publications, DateOnly startDate, DateOnly endDate)
    {
        return publications.FindAll(x => x.dateOfRegistration >= startDate && x.dateOfRegistration <= endDate);
    }
    static List<Publication> FindAllPublicationByType(List<Publication> publications, int typeOfPublication)
    {
        return publications.FindAll(x => x.IDOfType == typeOfPublication);
    }
    static List<Publication> FindAllPublicationByUDC(List<Publication> publications, int UDC)
    {
        return publications.FindAll(x => x.IDOfUDC == UDC);
    }
    static List<Publication> FindAllPublicationByTitle(List<Publication> publications, string text)
    {
        return publications.FindAll(x => x.title.ToLower().Contains(text.ToLower()));
    }
    static List<Publication> FindAllPublicationByJournalNumber(List<Publication> publications, int journalNumber)
    {
        return publications.FindAll(x => x.journalNumber == journalNumber);
    }
    #endregion

    #region Sort
    static void SortMenu(List<Publication> publications,
                         List<ReferenceBook> referenceBookOfType,
                         List<ReferenceBook> referenceBookOfUDC,
                         List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        bool flag = true;
        while (flag)
        {
            Console.WriteLine("Выберете один из вариантов поиска:");
            Console.WriteLine("1) По дате регистрации");
            Console.WriteLine("2) По типу публикации");
            Console.WriteLine("3) По УДК");
            Console.WriteLine("4) По ФИО автора");
            Console.WriteLine("5) По названию");
            Console.WriteLine("6) По номеру журнала");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0) Назад");
            Console.ResetColor();
            if (IsValidOption(out int option))
            {
                Console.Clear();
                switch (option)
                {
                    case 1:
                        SortPublicationByRegestrationDate(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 2:
                        SortPublicationByType(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 3:
                        SortPublicationByUDC(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 4:
                        SortPublicationByFullNameOfAuthor(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 5:
                        SortPublicationByTitle(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 6:
                        SortPublicationByJournalNumber(publications);
                        Console.Clear();
                        WritePublication(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 0:
                        flag = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Отсутствует опция под номером {option}");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }

    static void SortPublicationByRegestrationDate(List<Publication> publications)
    {
        publications.Sort((x, y) => x.numberOfRegistration.CompareTo(y.numberOfRegistration));
    }
    static void SortPublicationByType(List<Publication> publications)
    {
        publications.Sort((x, y) => x.IDOfType.CompareTo(y.IDOfType));
    }
    static void SortPublicationByUDC(List<Publication> publications)
    {
        publications.Sort((x, y) => x.IDOfUDC.CompareTo(y.IDOfUDC));
    }
    static void SortPublicationByFullNameOfAuthor(List<Publication> publications)
    {
        publications.Sort((x, y) => x.fullNameOfAuthor.CompareTo(y.fullNameOfAuthor));
    }
    static void SortPublicationByTitle(List<Publication> publications)
    {
        publications.Sort((x, y) => x.title.CompareTo(y.title));
    }
    static void SortPublicationByJournalNumber(List<Publication> publications)
    {
        publications.Sort((x, y) => x.journalNumber.CompareTo(y.journalNumber));
    }
    static void SortPublicationByFullNameOfReviewer(List<Publication> publications, List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        publications.Sort((x, y) => referenceBookOfFullNameOfReviewer.Find(z => z.ID == x.IDOfFullNameOfReviewer).title.CompareTo(referenceBookOfFullNameOfReviewer.Find(z => z.ID == y.IDOfFullNameOfReviewer).title));
    }
    #endregion

    #region Save and Recover
    static void SaveAllChanges(StreamWriter swOfPublications, List<Publication> publications,
                               StreamWriter swOfReferenceBookOfType, List<ReferenceBook> ReferenceBookOfType,
                               StreamWriter swOfReferenceBookOfUDC, List<ReferenceBook> ReferenceBookOfUDC,
                               StreamWriter swOfReferenceBookOfReviewer, List<ReferenceBook> ReferenceBookOfReviewer)
    {
        SavePublications(swOfPublications, publications);
        SaveReferenceBook(swOfReferenceBookOfType, ReferenceBookOfType);
        SaveReferenceBook(swOfReferenceBookOfUDC, ReferenceBookOfUDC);
        SaveReferenceBook(swOfReferenceBookOfReviewer, ReferenceBookOfReviewer);
    }

    static void SavePublications(StreamWriter swOfPublications, List<Publication> publications)
    {
        for (int i = 0; i < publications.Count; i++)
        {
            swOfPublications.WriteLine(publications[i].numberOfRegistration);
            swOfPublications.WriteLine(publications[i].dateOfRegistration);
            swOfPublications.WriteLine(publications[i].IDOfType);
            swOfPublications.WriteLine(publications[i].IDOfUDC);
            swOfPublications.WriteLine(publications[i].fullNameOfAuthor);
            swOfPublications.WriteLine(publications[i].title);
            swOfPublications.WriteLine(publications[i].IDOfFullNameOfReviewer);
            swOfPublications.WriteLine(publications[i].journalNumber);
            if (i < publications.Count - 1)
            {
                swOfPublications.WriteLine(publications[i].magazineReleaseDate);
            }
            else
            {
                swOfPublications.Write(publications[i].magazineReleaseDate);
            }
        }
        swOfPublications.Close();
    }

    static void SaveReferenceBook(StreamWriter swOfReferenceBook, List<ReferenceBook> ReferenceBook)
    {
        for (int i = 0; i < ReferenceBook.Count; i++)
        {
            if (i < ReferenceBook.Count - 1)
            {
                swOfReferenceBook.WriteLine(ReferenceBook[i].title);
            }
            else
            {
                swOfReferenceBook.Write(ReferenceBook[i].title);
            }
        }
        swOfReferenceBook.Close();
    }

    static void RecoverDataFromFile(ref List<Publication> publications,
                                    ref List<ReferenceBook> referenceBookOfType,
                                    ref List<ReferenceBook> referenceBookOfUDC,
                                    ref List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        StreamReader srReservReferenceBookOfType = new("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            srReservReferenceBookOfUDC = new("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            srReservReferenceBookOfFullNameOfReviewer = new("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            srReservPublication = new("..\\..\\..\\DataFiles\\Publications.txt");
        publications = ReadPublicationsFromFile(srReservPublication);
        referenceBookOfType = ReadReferenceBookFromFile(srReservReferenceBookOfType);
        referenceBookOfUDC = ReadReferenceBookFromFile(srReservReferenceBookOfUDC);
        referenceBookOfFullNameOfReviewer = ReadReferenceBookFromFile(srReservReferenceBookOfFullNameOfReviewer);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Данные успешно востановленно");
        Console.ResetColor();
    }
    #endregion

    #region IsVAlid

    #region int
    static bool IsValid(out int option, int? start = null, int? end = null)
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
    static bool IsValidOrNull(out int option, int? start = null, int? end = null)
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
    #endregion

    #region long
    static bool IsValid(out long option, int? start = null, int? end = null)
    {
        if (long.TryParse(Console.ReadLine(), out option) && (start == null || option >= start) && (end == null || option <= end))
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
    static bool IsValidOrNull(out long option, int? start = null, int? end = null)
    {
        string? input = Console.ReadLine();
        option = -1;
        if (input == null || input.Length == 0 || long.TryParse(input, out option) && (start == null || option >= start) && (end == null || option <= end))
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
    #endregion

    #region DateOnly
    static bool IsValid(out DateOnly output)
    {
        if (DateOnly.TryParse(Console.ReadLine(), out output))
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
    static bool IsValidOrNull(out DateOnly output)
    {
        string? input = Console.ReadLine();
        output = new DateOnly(1, 1, 1);
        if (input == null || input.Length == 0 || DateOnly.TryParse(input, out output))
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
    #endregion

    #region Regex
    static bool IsValidByRegex(out string? input, Regex pattern)
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
    #endregion

    static bool IsValidOption(out int option)
    {
        return IsValid(out option, 0);
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
    #endregion

    #region Get
    static int GetNewId(List<ReferenceBook> list)
    {
        int number = -1;
        for (int i = 0; number == -1; i++)
        {
            if (!list.Exists(x => x.ID == i))
            {
                number = i;
            }
        }
        return number;
    }

    static int GetChosenIDOfReferenceBook(List<ReferenceBook> referenceBook, string text)
    {
        int chosenOne;
        do
        {
            Console.Clear();
            Console.WriteLine($"Выберете вариант {text} из справочника:");
            for (int i = 0; i < referenceBook.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {referenceBook[i].title}");
            }
        } while (!IsValid(out chosenOne, 1, referenceBook.Count));
        return chosenOne - 1;
    }
    #endregion
}