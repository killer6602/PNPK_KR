using Newtonsoft.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using static Program;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFiles()
        {
            string path;

            path = "..\\..\\..\\..\\KR\\DataFiles\\Publications.txt";
            Assert.IsTrue(File.Exists(path));
            Type type = new Publication().GetType();
            Assert.IsTrue(File.ReadAllLines(path).Length % type.GetFields().Length == 0 &&
                File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfType.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfUDC.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);


            path = "..\\..\\..\\..\\KR\\DataFiles\\ReservFiles\\Publications.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length % type.GetFields().Length == 0 &&
                File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReservFiles\\ReferenceBookOfType.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReservFiles\\ReferenceBookOfUDC.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\DataFiles\\ReservFiles\\ReferenceBookOfFullNameOfReviewer.txt";
            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);
        }

        [TestMethod]
        public void TestReadPublicationsFromFile()
        {
            StreamReader srPublication = new("..\\..\\..\\..\\KR\\DataFiles\\Publications.txt");

            Type type = new Publication().GetType();
            int n = srPublication.ReadToEnd().Split('\n').Length / type.GetFields().Length;//получение количества объектов в файле
            srPublication.BaseStream.Position = 0;//возвращение указателя StreamReader в начало файла

            List<Publication> result = ReadPublicationsFromFile(srPublication);
            Assert.AreEqual(n, result.Count, "Неверное количество данных");
            for (int i = 0; i < result.Count; i++)
            {
                Assert.IsTrue(!result[i].numberOfRegistration.Equals(0) &&
                    result[i].dateOfRegistration != new DateOnly(1, 1, 1) &&
                    result[i].IDOfType >= 0 &&
                    result[i].IDOfUDC >= 0 &&
                    result[i].fullNameOfAuthor.Length >= 8 &&
                    result[i].title.Length > 0 &&
                    result[i].IDOfFullNameOfReviewer >= 0 &&
                    result[i].journalNumber > 0 &&
                    result[i].magazineReleaseDate != new DateOnly(1, 1, 1), "Поле не считано/приняло значение по умолчанию");
            }
        }

        [TestMethod]
        public void TestReport()
        {
            string path;
            List<Publication> nullPublications = [];
            List<ReferenceBook> references = [];
            StreamReader srReferenceBookOfType = new("..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfType.txt"),
                         srReferenceBookOfUDC = new("..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfUDC.txt"),
                         srReferenceBookOfFullNameOfReviewer = new("..\\..\\..\\..\\KR\\DataFiles\\ReferenceBookOfFullNameOfReviewer.txt"),
                         srPublication = new("..\\..\\..\\..\\KR\\DataFiles\\Publications.txt");
            List<Publication> publications = ReadPublicationsFromFile(srPublication);
            List<ReferenceBook> referenceBookOfType = ReadReferenceBookFromFile(srReferenceBookOfType),
                                referenceBookOfUDC = ReadReferenceBookFromFile(srReferenceBookOfUDC),
                                referenceBookOfFullNameOfReviewer = ReadReferenceBookFromFile(srReferenceBookOfFullNameOfReviewer);

            path = "..\\..\\..\\..\\KR\\Reports\\JournalNumber.txt";
            CreateReportByJournalNumber(nullPublications, references, references, path);
            Assert.AreEqual(0, File.ReadAllLines(path).Length, "При отсутствующих входных данных файл содержит выводные данные");
            CreateReportByJournalNumber(publications, referenceBookOfType, referenceBookOfUDC, path);
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\Reports\\Author.txt";
            CreateReportByAuthor(nullPublications, references, references, path);
            Assert.AreEqual(0, File.ReadAllLines(path).Length, "При отсутствующих входных данных файл содержит выводные данные");
            CreateReportByAuthor(publications, referenceBookOfType, referenceBookOfUDC, path);
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);

            path = "..\\..\\..\\..\\KR\\Reports\\Reviewer.txt";
            CreateReportByReviewer(nullPublications, references, path);
            Assert.AreEqual(0, File.ReadAllLines(path).Length, "При отсутствующих входных данных файл содержит выводные данные");
            CreateReportByReviewer(publications, referenceBookOfFullNameOfReviewer, path);
            Assert.IsTrue(File.ReadAllLines(path).Length != 0);
        }
    }
}