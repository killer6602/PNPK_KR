using System.Linq;
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
        List<ReferenceBook> referenceBookOfType = ReadReferenceBook(srReferenceBookOfType),
            referenceBookOfUDC = ReadReferenceBook(srReferenceBookOfUDC),
            referenceBookOfFullNameOfReviewer = ReadReferenceBook(srReferenceBookOfFullNameOfReviewer);
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
    static List<ReferenceBook> ReadReferenceBook(StreamReader srReferenceBook)
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
    static void MainMenu(List<Publication> publications,
                         List<ReferenceBook> referenceBookOfType,
                         List<ReferenceBook> referenceBookOfUDC,
                         List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        while (true)
        {
            Console.WriteLine("Выберете один из вариантов работы с Базой данных:");
            Console.WriteLine("1) Редактирование БД");
            Console.WriteLine("2) Редактирование БД");
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
                        EditDBMenu();
                        break;
                    case 2:
                        EditReferenceBooksMenu();
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

    static void EditDBMenu()
    {

    }

    static void EditReferenceBooksMenu()
    {

    }

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
                        WritePublications(publications, referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
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

    static void WritePublications(List<Publication> publications,
                                  List<ReferenceBook> referenceBookOfType,
                                  List<ReferenceBook> referenceBookOfUDC,
                                  List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        for (int i = 0; i < publications.Count; i++)
        {
            WritePublication(publications[i], referenceBookOfType, referenceBookOfUDC, referenceBookOfFullNameOfReviewer);
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("########################################################################################################################");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void WritePublication(Publication publication,
                                  List<ReferenceBook> referenceBookOfType,
                                  List<ReferenceBook> referenceBookOfUDC,
                                  List<ReferenceBook> referenceBookOfFullNameOfReviewer)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"Регистрационный номер: {publication.numberOfRegistration}");
        Console.WriteLine($"Дата регистрации: {publication.dateOfRegistration}");
        Console.WriteLine($"Тип публикации: {referenceBookOfType[referenceBookOfType.FindIndex(x => x.ID == publication.IDOfType)].title}");
        Console.WriteLine($"УДК: {referenceBookOfUDC[referenceBookOfUDC.FindIndex(x => x.ID == publication.IDOfUDC)].title}");
        Console.WriteLine($"ФИО автора: {publication.fullNameOfAuthor}");
        Console.WriteLine($"Название: {publication.title}");
        Console.WriteLine($"ФИО рецензента{referenceBookOfFullNameOfReviewer[referenceBookOfFullNameOfReviewer.FindIndex(x => x.ID == publication.IDOfFullNameOfReviewer)].title}");
        Console.WriteLine($"Номер журнала{publication.journalNumber}");
        Console.WriteLine($"Дата выхода выпуска в магазин: {publication.magazineReleaseDate}");
        Console.WriteLine();
    }
    static void SearchMenu()
    {

    }

    static void SortMenu()
    {

    }

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
        referenceBookOfType = ReadReferenceBook(srReservReferenceBookOfType);
        referenceBookOfUDC = ReadReferenceBook(srReservReferenceBookOfUDC);
        referenceBookOfFullNameOfReviewer = ReadReferenceBook(srReservReferenceBookOfFullNameOfReviewer);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Данные успешно востановленно");
        Console.ResetColor();
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