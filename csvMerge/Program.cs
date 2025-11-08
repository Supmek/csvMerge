using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

class Program
{
    private static string inputFolder;
    private static string outputFolder;
    private static string filePattern;
    private static string logFile;
    private static int checkInterval;

    // Klasa do deserializacji konfiguracji
    public class AppConfig
    {
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string FilePattern { get; set; }
        public string LogFile { get; set; }
        public int CheckInterval { get; set; }
    }

    static void Main()
    {
       
        // Ładujemy konfigurację na starcie
        if (!WczytajKonfiguracje())
        {
            Console.WriteLine("Błąd podczas wczytywania konfiguracji. Program zostanie zatrzymany.");
            Thread.Sleep(5000);
            return;
        }
        static bool WczytajKonfiguracje()
        {
            try
            {
                string configFile = "config.json";
                string programFolder = AppDomain.CurrentDomain.BaseDirectory;

                if (!File.Exists(configFile))
                {
                    // Tworzymy domyślny plik konfiguracyjny jeśli nie istnieje
                    var defaultConfig = new AppConfig
                    {
                        InputFolder = Path.Combine(programFolder, "input"),
                        OutputFolder = Path.Combine(programFolder, "output"),
                        FilePattern = "*.csv",
                        LogFile = Path.Combine(programFolder, "csvMerge.log"),
                        CheckInterval = 5000
                    };

                    string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    File.WriteAllText(configFile, json);

                    Console.WriteLine($"Utworzono domyślny plik konfiguracyjny: {configFile}");
                }

                // Wczytaj konfigurację
                string jsonConfig = File.ReadAllText(configFile);
                var config = JsonSerializer.Deserialize<AppConfig>(jsonConfig);

                // Przypisz wartości do zmiennych statycznych
                inputFolder = config.InputFolder;
                outputFolder = config.OutputFolder;
                filePattern = config.FilePattern;
                logFile = config.LogFile;
                checkInterval = config.CheckInterval;

                // Utwórz foldery jeśli nie istnieją
                Directory.CreateDirectory(inputFolder);
                Directory.CreateDirectory(outputFolder);
                Directory.CreateDirectory(Path.GetDirectoryName(logFile));

                Log("Konfiguracja wczytana pomyślnie");
                Log($"Folder programu: {programFolder}");
                Log($"InputFolder: {inputFolder}");
                Log($"OutputFolder: {outputFolder}");
                Log($"FilePattern: {filePattern}");
                Log($"LogFile: {logFile}");
                Log($"CheckInterval: {checkInterval}ms");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wczytywania konfiguracji: {ex.Message}");
                return false;
            }
        }
        Log("Program uruchomiony i działa w tle...");

        while (true)
        {
            try
            {
                LaczeniePlikow();
                Thread.Sleep(checkInterval); // Czekaj 5 sekund przed kolejnym sprawdzeniem
            }
            catch (Exception ex)
            {
                Log($"Błąd: {ex.Message}");
                Thread.Sleep(10000); // W przypadku błędu czekaj dłużej
            }
        }
    }

    static void LaczeniePlikow()
    {
        if (!Directory.Exists(inputFolder))
        {
            Log($"Folder {inputFolder} nie istnieje.");
            return;
        }

        var weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            DateTime.Now,
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);

        int suffix = 1;

        while (File.Exists(Path.Combine(outputFolder, "T"+ weekNumber + "-" + suffix + ".csv")))
        {
            suffix++;
        }
        string weekNumberStr = "T" + weekNumber + "-" + suffix;
        string outputFilePath = Path.Combine(outputFolder, weekNumberStr + ".csv");
        

        var pliki = Directory.GetFiles(inputFolder, filePattern)
                            .OrderBy(f => f)
                            .ToArray();

        if (pliki.Length == 0)
        {
            // Nie loguj braku plików żeby nie zaśmiecać loga
            return;
        }

        string message = "";

        using (var writer = new StreamWriter(outputFilePath, append: false))
        {
            message += "\n-------> Łączenie plików.\n";
            foreach (var plik in pliki)
            {
                try
                {
                    var zawartosc = File.ReadAllText(plik);
                    zawartosc = zawartosc.TrimEnd('\r', '\n');
                    writer.WriteLine(zawartosc);
                    File.Delete(plik);
                    message += $"       Dodano: {Path.GetFileName(plik)}\n";
                }
                catch (Exception ex)
                {
                    message += $"Błąd przy przetwarzaniu {Path.GetFileName(plik)}: {ex.Message}\n";
                }
            }
        }

        message += $"Pliki zostały połączone do: {outputFilePath}\n";
        message += $"Wykonano: {DateTime.Now}\n";
        message += "---------------------KONIEC.";

        Log(message);
    }

    static void Log(string wiadomosc)
    {
        try
        {
            using (var writer = new StreamWriter(logFile, append: true))
            {
                writer.WriteLine(wiadomosc);
            }
        }
        catch (Exception ex)
        {
            // Awaryjne logowanie - jeśli nie można zapisać do pliku, ignoruj
        }
    }
}