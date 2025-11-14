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
    private static bool modifyLastColumns;
    private static string logFile;
    private static int checkInterval;
    private static Mutex singleInstanceMutex;
    private static EventWaitHandle reloadEvent;

    // Klasa do deserializacji konfiguracji
    public class AppConfig
    {
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string FilePattern { get; set; }
        public bool ModifyLastColumns { get; set; }
        public string LogFile { get; set; }
        public int CheckInterval { get; set; }
    }

    static void Main()
    {
        // Stałe nazwy synchronizacji (muszą być takie same dla obu instancji)
        string mutexName = "csvMerge_singleton_mutex";
        string eventName = "csvMerge_reload_event";

        bool createdNewMutex;
        try
        {
            // Tworzymy mutex bez przejmowania własności — tylko sprawdzamy czy już istnieje
            singleInstanceMutex = new Mutex(false, mutexName, out createdNewMutex);
        }
        catch
        {
            createdNewMutex = false;
        }

        if (!createdNewMutex)
        {
            // Druga instancja: sygnalizuj pierwszej, żeby zaczytała konfigurację, i zakończ.
            try
            {
                var existing = EventWaitHandle.OpenExisting(eventName);
                existing.Set();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // Jeżeli event jeszcze nie istnieje, utwórz tymczasowy i ustaw go (nie ma nasłuchującego wtedy — bezpieczne)
                try
                {
                    using (var temp = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
                    {
                        temp.Set();
                    }
                }
                catch { }
            }
            catch { }

            // Fallback log (gdy logFile nie jest jeszcze ustawione)
            try
            {
                string fallbackLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csvMerge.log");
                File.AppendAllText(fallbackLog, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}    ^--> Instancja programu już aktywna, reload konfiguracji...");
            }
            catch { }

            return;
        }

        // Pierwsza (główna) instancja: utwórz event do nasłuchu sygnałów reload
        bool createdNewEvent;
        try
        {
            reloadEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName, out createdNewEvent);
        }
        catch
        {
            try { reloadEvent = EventWaitHandle.OpenExisting(eventName); } catch { reloadEvent = null; }
        }

        // Uruchom wątek nasłuchujący sygnałów przeładowania konfiguracji
        if (reloadEvent != null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    while (true)
                    {
                        reloadEvent.WaitOne();
                        // Po otrzymaniu sygnału spróbuj przeładować konfigurację
                        if (WczytajKonfiguracje())
                        {
                            Log($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ^--> Konfiguracja przeładowana.");
                        }
                        else
                        {
                            Log($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} -> Błąd podczas przeładowania konfiguracji na żądanie.");
                        }
                    }
                }
                catch
                {
                    // ignoruj błędy nasłuchiwania
                }
            });
        }

        // Upewnijmy się, że zasoby zostaną zwolnione przy zamykaniu procesu
        AppDomain.CurrentDomain.ProcessExit += (_, __) =>
        {
            try { reloadEvent?.Dispose(); } catch { }
            try { singleInstanceMutex?.Dispose(); } catch { }
        };

        // Pierwotne wczytanie konfiguracji
        if (!WczytajKonfiguracje())
        {
            Console.WriteLine("Błąd podczas wczytywania konfiguracji. Program zostanie zatrzymany.");
            Thread.Sleep(5000);
            return;
        }

        Log("Program uruchomiony i działa w tle...\n");

        // Prosta pętla główna
        while (true)
        {
            try
            {
                LaczeniePlikow();
                Thread.Sleep(checkInterval); // Czekaj określony interwał przed kolejnym sprawdzeniem
            }
            catch (Exception ex)
            {
                Log($"Błąd: {ex.Message}");
                Thread.Sleep(10000); // W przypadku błędu czekaj dłużej
            }
        }
    }

    // Przeniesione na poziom klasy, aby mogła być wywoływana z wątku nasłuchującego
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
                    ModifyLastColumns = false,
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
            if (config == null) throw new Exception("Nieprawidłowa zawartość config.json");

            // Przypisz wartości do zmiennych statycznych
            inputFolder = config.InputFolder;
            outputFolder = config.OutputFolder;
            filePattern = config.FilePattern;
            modifyLastColumns = config.ModifyLastColumns;
            logFile = config.LogFile;
            checkInterval = config.CheckInterval;

            // Utwórz foldery jeśli nie istnieją
            try { Directory.CreateDirectory(inputFolder); } catch { }
            try { Directory.CreateDirectory(outputFolder); } catch { }
            try
            {
                var dir = Path.GetDirectoryName(logFile);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            }
            catch { }

            Log($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Konfiguracja wczytana pomyślnie");
            Log($"Folder programu: {programFolder}");
            Log($"InputFolder: {inputFolder}");
            Log($"OutputFolder: {outputFolder}");
            Log($"FilePattern: {filePattern}");
            Log($"ModifyLastColumns: {modifyLastColumns}");
            Log($"LogFile: {logFile}");
            Log($"CheckInterval: {checkInterval}ms\n");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd wczytywania konfiguracji: {ex.Message}");
            return false;
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

        while (File.Exists(Path.Combine(outputFolder, "T" + weekNumber + "-" + suffix + ".csv")))
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
            if (modifyLastColumns)
            {
                message += "-------> Modyfikacja ostatnich kolumn.\n";
                foreach (var plik in pliki)
                {
                    try
                    {
                        var linie = File.ReadAllLines(plik);
                        for (int i = 0; i < linie.Length; i++)
                        {
                            linie[i] += ";0;1"; // Dodaj dwie nowe kolumny z wartościami 0 i 1
                        }
                        File.WriteAllLines(plik, linie);
                        message += $"       Zmodyfikowano: {Path.GetFileName(plik)}\n";
                    }
                    catch (Exception ex)
                    {
                        message += $"Błąd przy modyfikacji {Path.GetFileName(plik)}: {ex.Message}\n";
                    }
                }
            }
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
            string target = logFile;
            if (string.IsNullOrEmpty(target))
            {
                target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csvMerge.log");
            }

            using (var writer = new StreamWriter(target, append: true))
            {
                writer.WriteLine(wiadomosc);
            }
        }
        catch
        {
            // Awaryjne logowanie - jeśli nie można zapisać do pliku, ignoruj
        }
    }
}