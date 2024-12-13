using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
internal class Program
{
    //обработчик событий который позволяет отслеживать закрытие программы любым способом
    #region Trap application termination
    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    private delegate bool EventHandler(CtrlType sig);
    static EventHandler? _handler;

    enum CtrlType
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }

    private static bool Handler(CtrlType sig)
    {
        //Console.Beep();
        Environment.Exit(-1);
        return true;
    }
    #endregion

    struct Publication
    {
        public long numberOfRegistration;
        public DateOnly dateOfRegistration;
        public short IDOfType;
        public short IDOfUDC;
        public string fullNameOfAuthor;
        public string title;
        public short IDOfFullNameOfReviewer;
        public int journalNumber;
        public DateOnly magazineReleaseDate;
    }

    struct ReferenceBook
    {
        public long ID;
        public string title;
    }

    private static void Main()
    {
        _handler += new EventHandler(Handler);
        SetConsoleCtrlHandler(_handler, true);
        Console.OutputEncoding = Encoding.UTF8;
        Console.ResetColor();
        Console.Clear();

        StreamReader srReferenceBookOfType = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            srReferenceBookOfUDC = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            srReferenceBookOfFullNameOfReviewer = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            srPublication = new StreamReader("..\\..\\..\\DataFiles\\Publications.txt");
        List<Publication> publications = ReadPublicationsFromFile(srPublication);
        List<ReferenceBook> referenceBookOfType = ReadReferenceBookFromFile(srReferenceBookOfType),
            referenceBookOfUDC = ReadReferenceBookFromFile(srReferenceBookOfUDC),
            referenceBookOfFullNameOfReviewer = ReadReferenceBookFromFile(srReferenceBookOfFullNameOfReviewer);
        srPublication.Close();
        srReferenceBookOfType.Close();
        srReferenceBookOfUDC.Close();
        srReferenceBookOfFullNameOfReviewer.Close();

        StreamWriter swReservReferenceBookOfType = new StreamWriter("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfType.txt"),
            swReservReferenceBookOfUDC = new StreamWriter("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfUDC.txt"),
            swReservReferenceBookOfFullNameOfReviewer = new StreamWriter("..\\..\\..\\DataFiles\\ReservFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            swReservPublication = new StreamWriter("..\\..\\..\\DataFiles\\ReservFiles\\Publications.txt");
        SaveAllChanges(swReservPublication, publications,
                       swReservReferenceBookOfType, referenceBookOfType,
                       swReservReferenceBookOfUDC, referenceBookOfUDC,
                       swReservReferenceBookOfFullNameOfReviewer, referenceBookOfFullNameOfReviewer);

        MainMenu(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);

        StreamWriter swReferenceBookOfType = new StreamWriter("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            swReferenceBookOfUDC = new StreamWriter("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            swReferenceBookOfFullNameOfReviewer = new StreamWriter("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            swPublication = new StreamWriter("..\\..\\..\\DataFiles\\Publications.txt");
        SaveAllChanges(swPublication, publications,
                       swReferenceBookOfType, referenceBookOfType,
                       swReferenceBookOfUDC, referenceBookOfUDC,
                       swReferenceBookOfFullNameOfReviewer, referenceBookOfFullNameOfReviewer);
    }

    #region Read
    static List<Publication> ReadPublicationsFromFile(StreamReader srPublication)
    {
        Type type = new Publication().GetType();
        int n = srPublication.ReadToEnd().Split('\n').Length / type.GetFields().Length;//получение количества объектов в файле
        srPublication.BaseStream.Position = 0;//возвращение указателя StreamReader в начало файла

        List<Publication> publications = new List<Publication>();
        for (int i = 0; i < n; i++)
        {
            publications.Add(ReadPublicationFromFile(srPublication));
        }
        return publications;
    }
    static Publication ReadPublicationFromFile(StreamReader srPublication)//нету проверок т.к. файл доверенный источник и в нём нет ошибок
    {
        Publication publication = new Publication();
        publication.numberOfRegistration = Convert.ToInt64(srPublication.ReadLine());
        publication.dateOfRegistration = DateOnly.Parse(srPublication.ReadLine()!);
        publication.IDOfType = Convert.ToInt16(srPublication.ReadLine());
        publication.IDOfUDC = Convert.ToInt16(srPublication.ReadLine());
        publication.fullNameOfAuthor = srPublication.ReadLine()!;
        publication.title = srPublication.ReadLine()!;
        publication.IDOfFullNameOfReviewer = Convert.ToInt16(srPublication.ReadLine());
        publication.journalNumber = Convert.ToInt32(srPublication.ReadLine());
        publication.magazineReleaseDate = DateOnly.Parse(srPublication.ReadLine()!);
        return publication;
    }
    static List<ReferenceBook> ReadReferenceBookFromFile(StreamReader srReferenceBook)
    {
        int n = srReferenceBook.ReadToEnd().Split('\n').Length;//получение количества объектов в файле
        srReferenceBook.BaseStream.Position = 0;//возвращение указателя StreamReader в начало файла

        List<ReferenceBook> referenceBook = new List<ReferenceBook>();
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("6) Востановить данные из резервного файла");
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
                        SearchMenu();
                        break;
                    case 5:
                        SortMenu();
                        break;
                    case 6:
                        RecoverDataFromFile(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
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
        Publication newPublication = new Publication();

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
        Publication newPublication = new Publication();

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
        newPublication.fullNameOfAuthor = text == null ? publication.fullNameOfAuthor : text;

        Console.Clear();
        do
        {
            Console.WriteLine("Введите заголовок публикации или нажмите Enter чтобы оставить прежний");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {publication.title}");
            Console.ResetColor();
        } while (!IsntNullWithConsoleOutput(out text!));
        newPublication.title = text == null ? publication.title : text;

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
                        EditChosenReferenceBooksMenu(referenceBookOfUDC, "УДК");
                        break;
                    case 3:
                        EditChosenReferenceBooksMenu(referenceBookOfFullNameOfReviewer, "ФИО рецензента");
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
    static void EditChosenReferenceBooksMenu(List<ReferenceBook> referenceBook, string text)
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
                        ReferenceBook newReferenceBook = ReadReferenceBookFromConsole(referenceBook);
                        referenceBook.Add(newReferenceBook);
                        break;
                    case 2:
                        do
                        {
                            Console.Clear();
                            WriteReferenceBook(referenceBook, text);
                            Console.WriteLine($"Введите ID справочника который хотите изменить");
                        } while (!IsValid(out option) || !referenceBook.Exists(x => x.ID == option));
                        referenceBook[referenceBook.FindIndex(x => x.ID == option)] = ReadReferenceBookFromConsoleOrLeaveAsItIs(referenceBook, option);
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
    static ReferenceBook ReadReferenceBookFromConsole(List<ReferenceBook> referenceBook)
    {

        ReferenceBook newReferenceBook = new ReferenceBook();
        string text;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите новое название");
        } while (!IsntNullWithConsoleOutput(out text!) || referenceBook.Exists(x => x.title.Equals(text)));
        newReferenceBook.title = text;
        newReferenceBook.ID = GetNewId(referenceBook);
        return newReferenceBook;
    }
    static ReferenceBook ReadReferenceBookFromConsoleOrLeaveAsItIs(List<ReferenceBook> referenceBook, int option)
    {
        ReferenceBook newReferenceBook = new ReferenceBook();
        string? text;
        Console.Clear();
        do
        {
            Console.WriteLine("Введите новое название или нажмите Enter чтобы оставить как есть");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"На данный момент: {referenceBook[option].title}");
            Console.ResetColor();
            text = Console.ReadLine();
        } while (referenceBook.Exists(x => x.title.Equals(text)));
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


    static void WritePublication(List<Publication> publications, //лучше перегрузка или два отдельных метода?
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

    static void SearchMenu()
    {

    }

    static void SortMenu()
    {

    }

    #region Save and Recover
    static void SaveAllChanges(StreamWriter swOfPublications, List<Publication> publications,
                           StreamWriter swOfRBOfType, List<ReferenceBook> RBOfType,
                           StreamWriter swOfRBOfUDC, List<ReferenceBook> RBOfUDC,
                           StreamWriter swOfRBOfReviewer, List<ReferenceBook> RBOfReviewer)
    {
        SavePublications(swOfPublications, publications);
        SaveReferenceBook(swOfRBOfType, RBOfType);
        SaveReferenceBook(swOfRBOfUDC, RBOfUDC);
        SaveReferenceBook(swOfRBOfReviewer, RBOfReviewer);
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

    static void SaveReferenceBook(StreamWriter swOfRB, List<ReferenceBook> ReferenceBook)
    {
        for (int i = 0; i < ReferenceBook.Count; i++)
        {
            if (i < ReferenceBook.Count - 1)
            {
                swOfRB.WriteLine(ReferenceBook[i].title);
            }
            else
            {
                swOfRB.Write(ReferenceBook[i].title);
            }
        }
        swOfRB.Close();
    }

    static void RecoverDataFromFile(List<Publication> publications,
                                    List<ReferenceBook> referenceBookOfType,
                                    List<ReferenceBook> referenceBookOfUDC,
                                    List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        StreamReader srReservReferenceBookOfType = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfType.txt"),
            srReservReferenceBookOfUDC = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfUDC.txt"),
            srReservReferenceBookOfFullNameOfReviewer = new StreamReader("..\\..\\..\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
            srReservPublication = new StreamReader("..\\..\\..\\DataFiles\\Publications.txt");
        publications = ReadPublicationsFromFile(srReservPublication);
        referenceBookOfType = ReadReferenceBookFromFile(srReservReferenceBookOfType);
        referenceBookOfUDC = ReadReferenceBookFromFile(srReservReferenceBookOfUDC);
        referenceBookOfFullNameOfReviewer = ReadReferenceBookFromFile(srReservReferenceBookOfFullNameOfReviewer);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Данные успешно востановленно");
        Console.ResetColor();
    }
    #endregion

    static void SortStudentsByFullName()
    {

    }

    #region short
    static bool IsValid(out short option, int? start = null, int? end = null)
    {
        if (short.TryParse(Console.ReadLine(), out option) && (start == null || option >= start) && (end == null || option <= end))
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
    static bool IsValidOrNull(out short option, int? start = null, int? end = null)
    {
        string? input = Console.ReadLine();
        option = -1;
        if (input == null || input.Length == 0 || short.TryParse(input, out option) && (start == null || option >= start) && (end == null || option <= end))
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
    static bool IsValid(out DateOnly output, int? start = null, int? end = null)
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
    static bool IsValidOrNull(out DateOnly output, int? start = null, int? end = null)
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

    static short GetChosenIDOfReferenceBook(List<ReferenceBook> referenceBook, string text)
    {
        short chosenOne = -1;
        do
        {
            Console.Clear();
            Console.WriteLine($"Выберете вариант {text} из справочника:");
            for (int i = 0; i < referenceBook.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {referenceBook[i].title}");
            }
        } while (!IsValid(out chosenOne, 1, referenceBook.Count));
        return Convert.ToInt16(chosenOne - 1);
    }
}