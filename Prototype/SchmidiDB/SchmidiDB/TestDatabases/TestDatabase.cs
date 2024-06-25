using System.Xml;
using SchmidiDB.Storage;

namespace SchmidiDB.TestDatabases;

public class TestDatabase
{
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\books.xml";
    public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\example.xml";
    
    
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark90.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark75.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark50.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark40.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark30.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark25.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark20.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark10.xml";
    
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark10min.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark25min.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark50min.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark75min.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark90min.xml";
    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmarkmin.xml";


    // public static string BookDb =
    // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark90max.xml";
    // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark75max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark60max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark50max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark40max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark35max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark30max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark25max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark20max.xml";
        // "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmark10max.xml";

    // public static string BookDb = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\indexBenchmarkmax.xml";



    
    public static void UseTestDatabase(string? testDataBasePath = null)
    {
        if (testDataBasePath == null)
        {
            testDataBasePath = BookDb;
        }

        var systemCatalog = SystemCatalog.Instance;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(testDataBasePath);

        var tables = xmlDoc.SelectSingleNode($"/data");
        foreach (XmlNode table in tables)
        {
            if (table.NodeType == XmlNodeType.Comment)
            {
                continue;
            }
            List<Row> data = new List<Row>();
            foreach (XmlNode tableEntry in table.ChildNodes)
            {
                var row = new Row();
                foreach (XmlNode column in tableEntry.ChildNodes)
                {
                    bool isInt = int.TryParse(column.InnerText, out int intValue);
                    row.Add(column.Name, isInt ? intValue : (column.InnerText.Length == 0 ? null : column.InnerText));
                }
                data.Add(row);
            }
            
            systemCatalog.CreateTable(table.Name, data);
        }
    }
    
    public static void WriteDatabaseToFile(int testDataBase = 100000)
    {
        string testDataBasePath;
        if (testDataBase == 100000)
        {
            testDataBasePath = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\example.xml";
        }
        else
        {
            testDataBasePath =
                "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\BookDBs\\TestDatabase" +
                testDataBase + ".xml";
        }
        string fileName = "D:\\FH\\Master\\Masterarbeit\\The Real Shit\\schmidiDB\\SchmidiDB\\SchmidiDB\\TestDatabases\\BookDBs\\SQL\\" +
                          testDataBase + ".sql"; 
        var systemCatalog = SystemCatalog.Instance;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(testDataBasePath);
        
        try
        {
            // Create a new StreamWriter instance to write to the file
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Write some text to the file
                writer.WriteLine($"CREATE TABLE IF NOT EXISTS categories (id INT PRIMARY KEY, name VARCHAR(255) NOT NULL);");
                writer.WriteLine($"CREATE TABLE  IF NOT EXISTS continents (id INT PRIMARY KEY, name VARCHAR(255) NOT NULL);");
                writer.WriteLine($"CREATE TABLE IF NOT EXISTS nationalities (id INT PRIMARY KEY, name VARCHAR(255) NOT NULL, continent INT NOT NULL, CONSTRAINT fk_continent FOREIGN KEY(continent) REFERENCES continents);");
                writer.WriteLine($"CREATE TABLE IF NOT EXISTS authors{testDataBase} (id INT PRIMARY KEY, firstName VARCHAR(255) NOT NULL, lastName VARCHAR(255) NOT NULL, gender VARCHAR(255) NOT NULL, nationality INT, CONSTRAINT fk_nationality FOREIGN KEY(nationality) REFERENCES nationalities(id));");
                writer.WriteLine($"CREATE TABLE IF NOT EXISTS books{testDataBase} (id INT PRIMARY KEY, title VARCHAR(255) NOT NULL, year INT NOT NULL, category INT, author INT NOT NULL,  CONSTRAINT fk_category FOREIGN KEY(category) REFERENCES categories(id), CONSTRAINT fk_author FOREIGN KEY(author) REFERENCES authors{testDataBase}(id));");
                var tables = xmlDoc.SelectSingleNode($"/data");
                foreach (XmlNode table in tables)
                {
                    if (table.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    foreach (XmlNode tableEntry in table.ChildNodes)
                    {
                        writer.Write($"INSERT INTO {(table.Name is "authors" or "books" ? table.Name+testDataBase : table.Name)} {(table.Name is "books" ? "(id, title, year, category, author)" : "")} VALUES (");
                        bool isFirst = true;
                        foreach (XmlNode column in tableEntry.ChildNodes)
                        {
                            if (!isFirst) writer.Write(", ");
                            string value = column.InnerText;
                            if (value == "null" || value.Length == 0) writer.Write("NULL");
                            else if (int.TryParse(column.InnerText, out int intValue)) writer.Write(intValue);
                            else writer.Write("'"+value+"'");
                            isFirst = false;
                        }
                        writer.WriteLine(");");
                    }
            
                }
            }
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during file operations
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public static void GenerateIndexBenchmarkDatabase()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(xmlDeclaration);
        XmlElement rootElement = doc.CreateElement("data");
        doc.AppendChild(rootElement);
        var size = 100000;
        XmlElement tests = doc.CreateElement("tests");
        rootElement.AppendChild(tests);
        for (int i = 0; i < size; i++)
        {
            XmlElement test = doc.CreateElement("test");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            XmlElement attr = doc.CreateElement("attr");
            attr.InnerText =  i > size * 0.25 ? 16.ToString() : 42.ToString();
            test.AppendChild(id);
            test.AppendChild(attr);
            tests.AppendChild(test);
        }
        doc.Save("indexBenchmark25.xml");
        Console.WriteLine("XML file created successfully.");
    }

    public static void GenerateTestDb()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(xmlDeclaration);
        XmlElement rootElement = doc.CreateElement("data");
        doc.AppendChild(rootElement);
        var numOfAuthors = 700000;
        var numOfBooks = 1000000;
        
        Random random = new Random();
        
        XmlElement continents = doc.CreateElement("continents");
        rootElement.AppendChild(continents);

        var continentsNames = new List<string>
            {"Asia", "Africa", "Europe", "North America", "South America", "Oceania", "Antarctica"};
        for (int i = 0; i < continentsNames.Count; i++)
        {
            XmlElement continent = doc.CreateElement("continent");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            XmlElement name = doc.CreateElement("name");
            name.InnerText = continentsNames[i];
            continent.AppendChild(id);
            continent.AppendChild(name);
            continents.AppendChild(continent);
        }
        
        XmlElement nationalities = doc.CreateElement("nationalities");
        rootElement.AppendChild(nationalities);
        
        var nationalitiesNames = new List<string>{"USA", "Canada", "Australia", "New Zealand", "India", "United Kingdom", "South Africa"};
        var correspondingContinent = new List<int> {4, 4, 6, 6, 1, 3, 2 };
        for (int i = 0; i < nationalitiesNames.Count; i++)
        {
            XmlElement nationality = doc.CreateElement("nationality");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            XmlElement name = doc.CreateElement("name");
            name.InnerText = nationalitiesNames[i];
            XmlElement continent = doc.CreateElement("continent");
            continent.InnerText = correspondingContinent[i].ToString();
            nationality.AppendChild(id);
            nationality.AppendChild(name);
            nationality.AppendChild(continent);
            nationalities.AppendChild(nationality);
        }
        
        var genders = new List<string> {"female", "male"};
        var firstNamesF = new List<string>
        {
            "Pamela", "Sarah", "Sara", "Jane", "Taylor", "Leslie", "Lena", "Hannah", "Shelley", "Mary", "Kate", "Tasha",
            "Ann", "Lily", "Sue", "Violet", "Nicole", "Victoria", "Margareth", "Elizabeth", "Madeleine", "Edith", "Blair", "Laura"
        };
        var firstNamesM = new List<string>
        {
            "Max", "Ryan", "Paul", "Henry", "Josh", "Charles", "Brian", "William", "Harry", "Taylor", "Leslie",
            "Thomas", "John", "Andrew", "Leo", "Frank", "Chuck", "Nate"
        };
        var lastNames = new List<string>
            {"Owen", "Brown", "Miller", "Sullivan", "Potter", "Smith", "Jones", "Williams",
        "Evans", "Taylor", "Andrews", "Davies", "Lanchester", "Hollens", "O'Connor", "Redgwick", "Jefferson", "Forbes", "Wild", "Hart", "Berry", "Bolton", "Simon", "Flack", "Franklin",
        "Waldorf", "Santiago"
        };
        XmlElement authors = doc.CreateElement("authors");
        rootElement.AppendChild(authors);
        for (int i = 0; i < numOfAuthors; i++)
        {
            XmlElement author = doc.CreateElement("author");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            ;
            XmlElement gender = doc.CreateElement("gender");
            string g = genders[random.Next(0, 2)];
            gender.InnerText = g;
            XmlElement firstName = doc.CreateElement("firstName");
            XmlElement lastName = doc.CreateElement("lastName");
            firstName.InnerText = g == "female"
                ? firstNamesF[random.Next(0, firstNamesF.Count)]
                : firstNamesM[random.Next(0, firstNamesM.Count)];
            lastName.InnerText = lastNames[random.Next(0, lastNames.Count)];
            XmlElement nationality = doc.CreateElement("nationality");
            nationality.InnerText = random.Next(1, nationalitiesNames.Count + 1).ToString();
            author.AppendChild(id);
            author.AppendChild(firstName);
            author.AppendChild(lastName);
            author.AppendChild(gender);
            author.AppendChild(nationality);
            authors.AppendChild(author);
        }

        var categoriesNames = new List<string>
        {
            "Fantasy", "Romance", "Sci-Fi", "Thriller", "Horror", "YA", "Historical Fiction", "null"
        };
        XmlElement categories = doc.CreateElement("categories");
        rootElement.AppendChild(categories);
        for (int i = 0; i < categoriesNames.Count - 1; i++)
        {
            XmlElement category = doc.CreateElement("category");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            XmlElement name = doc.CreateElement("name");
            name.InnerText = categoriesNames[i];
            category.AppendChild(id);
            category.AppendChild(name);
            categories.AppendChild(category);
        }
        
        //Title
        var firstTerm = new List<string>
        {
            "A Tale of ",
            "Standing between ",
            "The ", "The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ","The ",
            "A Story of ",
            "The World of ",
            "Meeting ",
            "Surviving ",
            "In the Land of ",
            "The Journey of ",
            "Beware the ",
            "Loving the ",
            "","","","","","","","","","","","","","","","","","","","","","","","","",
        };
        var secondTerm = new List<string>
        {
            "Two",
            "Three",
            "Pretty",
            "Ugly",
            "Handsome",
            "Dangerous",
            "Bleeding",
            "Coughing",
            "Singing",
            "Dancing",
            "Screaming",
            "Talking",
            "Hiding",
            "Lying",
            "Magnificent",
            "Awesome",
            "Amazing",
            "Golden", "Silver", "Blue", "Green"
        };
        var thirdTerm = new List<string>
        {
            "Countries",
            "Worlds",
            "Universes",
            "Cities",
            "Courts",
            "Castles",
            "Houses",
            "Forests", "Flowers", "Leaves", "Stones", "Gems", "Stars", "Moons",
            "Princes",
            "Princesses",
            "Pirates", "Ships", "Anchors",
            "Knights",
            "Queens",
            "Kings",
            "Vampires",
            "Zombies",
            "Aliens",
            "Coins", "Keys", "Songs", "Books", "Tales", "Legends",
            "Monkeys", "Cats", "Dogs", "Fish", "Mice", "Capybara", "Turtles",
            "Bakers", "Farmers", "Fishermen", "Astronauts", "Chefs"
        };

        XmlElement books = doc.CreateElement("books");
        rootElement.AppendChild(books);
        for (int i = 0; i < numOfBooks; i++)
        {
            XmlElement book = doc.CreateElement("book");
            XmlElement id = doc.CreateElement("id");
            id.InnerText = (i + 1).ToString();
            XmlElement title = doc.CreateElement("title");
            title.InnerText = firstTerm[random.Next(0, firstTerm.Count)]+secondTerm[random.Next(0, secondTerm.Count)]+" "+thirdTerm[random.Next(0, thirdTerm.Count)];
            XmlElement year = doc.CreateElement("year");
            year.InnerText = random.Next(1950, 2024).ToString();
            XmlElement category = doc.CreateElement("category");
            var randCategory = random.Next(1, categoriesNames.Count + 1);
            category.InnerText = randCategory.ToString();
            if (categoriesNames[randCategory - 1] == "null") category.InnerText = "";
            XmlElement author = doc.CreateElement("author");
            author.InnerText = random.Next(1, numOfAuthors + 1).ToString();
            book.AppendChild(id);
            book.AppendChild(title);
            book.AppendChild(year);
            book.AppendChild(category);
            book.AppendChild(author);
            books.AppendChild(book);
        }
        
        doc.Save("TestDatabase1000000.xml");
        Console.WriteLine("XML file created successfully.");
    }
}