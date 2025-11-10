csvMerge v1.0
=============
--README--

+Polski
--Opis:
Jest to program działający jako proces, który łączy wiele plików tekstowych w jeden plik tekstowy.
Jest zaprojektowany do monitorowania określonego folderu wejściowego w poszukiwaniu nowych plików .csv i łączenia ich w jeden plik .csv w folderze wyjściowym,
a następnie usuwania oryginalnych plików z folderu wejściowego.

--1.Użytkowanie
Program można uruchomić za pomocą dostarczonego pliku .exe.
Program działa w tle i co 5 sekund (konfigurowalne) sprawdza, czy w folderze wejściowym znajdują się nowe pliki .csv.
Jeśli zostaną znalezione nowe pliki, są one łączone w jeden plik .csv w folderze wyjściowym i usuwane z folderu wejściowego.
Scalony plik jest nazywany aktualnym znacznikiem czasu "T-weeknumber"+"suffix", aby uniknąć nadpisywania poprzednich scalonych plików.

--2.Konfiguracja
Program wymaga pliku konfiguracyjnego o nazwie "config.json", aby określić folder wejściowy i folder wyjściowy.
Plik jest automatycznie tworzony w tym samym folderze co plik wykonywalny podczas pierwszego uruchomienia, jeśli nie istnieje.
Plik konfiguracyjny powinien być sformatowany w następujący sposób:

	{
	  "InputFolder": "C:\\csvMerge\inputfolder",
	  "OutputFolder": "C:\\csvMerge\outputfolder",
	  "FilePattern": "*.csv",
	  "LogFile": "C:\\csvMerge\\csvMerge.log",
	  "CheckInterval": 5000
	}

Gdzie:
- InputFolder: Folder, w którym program będzie szukał plików CSV do scalenia.
- OutputFolder: Folder, w którym zostanie zapisany scalony plik CSV.
- FilePattern: Wzorzec do dopasowania plików CSV (domyślnie "*.csv"), można go zmienić na .txt lub inne.
- LogFile: Plik, do którego będą zapisywane logi.
- CheckInterval: Interwał (w milisekundach), w jakim program sprawdza nowe pliki (domyślnie 5000 ms).

--3.Logowanie
Program rejestruje swoje działania w określonym pliku dziennika (LogFile).
Rejestruje, kiedy się uruchamia, kiedy znajduje nowe pliki, kiedy łączy pliki i wszelkie błędy, które wystąpiły podczas wykonywania.

--4.Obsługa błędów
Program zawiera podstawową obsługę błędów, aby zarządzać problemami, takimi jak brakujące pliki konfiguracyjne, niedostępne foldery oraz błędy odczytu/zapisu plików.
Błędy są rejestrowane w pliku dziennika do przeglądu.

--5.Zakończenie
Program można zakończyć, zatrzymując proces w menedżerze zadań.

--6.Uwagi
- Podczas pierwszego uruchomienia program tworzy foldery wejściowe i wyjściowe, jeśli nie istnieją.
- Upewnij się, że program ma niezbędne uprawnienia do odczytu z folderu wejściowego i zapisu do folderu wyjściowego.
- Program łączy pliki przez dołączanie ich zawartości. Nie sprawdza duplikatów ani nie waliduje formatu CSV.
- Upewnij się, że przed uruchomieniem programu wykonałeś kopię zapasową ważnych danych, ponieważ usuwa on przeniesione pliki po scaleniu.
- Możesz używać go z różnymi rozszerzeniami plików, zmieniając FilePattern w pliku konfiguracyjnym.
- Jeśli potrzebujesz, aby program uruchamiał się przy starcie lub jako usługa, może być konieczne skonfigurowanie go za pomocą dodatkowych narzędzi lub skryptów,
takich jak Harmonogram zadań systemu Windows lub NSSM (Non-Sucking Service Manager), lub po prostu umieść skrót do pliku wykonywalnego w folderze Autostart systemu Windows.

--7.Wsparcie
W przypadku wsparcia lub pytań skontaktuj się z programistą lub zapoznaj się z dokumentacją dostarczoną z programem.

--8.Licencja
Ten program jest dostarczany "tak
jak jest" bez żadnej gwarancji. Używasz go na własne ryzyko.

--9.Autor
Utworzony przez: Tomasz Kempa
Data: 8 listopada 2025

Strona internetowa:
 github.com/tomaszkempa/csvMerge

--Koniec polskiej wersji--


+English
--Description:
This is a program that runs as a process it merges multiple text files into a single text file.
It is designed to monitor a specified input folder for new .csv files, merge them into a single .csv file in an output folder,
and then delete the original files from the input folder.

--1.Usage 
The program can be run using the .exe file provided. 
Program is running in background and checks every 5 seconds (configurable) if there are new .csv files in the input folder. 
If new files are found, they are merged into a single .csv file in the output folder and deleted from the input folder.
The merged file is named with the current timestamp "T-weeknumber"+"suffix" to avoid overwriting previous merges.

--2.Configuration
The program requires a configuration file named "config.json" to specify the input folder and the output folder. 
The file is automatically created in the same folder as the executable during the first run, if it does not exist.
The configuration file should be formatted as follows:

	{
	  "InputFolder": "C:\\csvMerge\inputfolder",
	  "OutputFolder": "C:\\csvMerge\outputfolder",
	  "FilePattern": "*.csv",
	  "LogFile": "C:\\csvMerge\\csvMerge.log",
	  "CheckInterval": 5000
	}

Where:
- InputFolder: The folder where the program will look for CSV files to merge.
- OutputFolder: The folder where the merged CSV file will be saved.
- FilePattern: The pattern to match CSV files (default is "*.csv"), you can change it for .txt or others.
- LogFile: The file where logs will be written.
- CheckInterval: The interval (in milliseconds) at which the program checks for new files (default is 5000 ms).

--3.Logging
The program logs its activities to the specified log file. 
It records when it starts, when it finds new files, when it merges files, and any errors that occur during execution.

--4.Error Handling
The program includes basic error handling to manage issues such as missing configuration files, inaccessible folders, and file read/write errors.
Errors are logged to the log file for review.

--5.Termination
The program can be terminated by stopping the process from the task manager.

--6.Notes
- In the first run, program creates input and output folders if they do not exist.
- Ensure that the program has the necessary permissions to read from the input folder and write to the output folder.
- The program merges files by appending their contents. It does not check for duplicate entries or validate the CSV format.
- Make sure to back up important data before running the program, as it deletes the moved files after merging.
- You can use it with different file extensions by changing the FilePattern in the config file.
- If you need program to run at startup or as a service, you may need to set it up using additional tools or scripts, 
like Windows Task Scheduler or NSSM (Non-Sucking Service Manager), or simply put a shortcut to the executable in the Windows Startup folder.

--7.Support
For support or questions, please contact the developer or refer to the documentation provided with the program.

--8.License
This program is provided "as is" without warranty of any kind. Use it at your own risk.

--9.Author
Created by: Tomasz Kempa
Date: 8 November 2025

Website:
github.com/tomaszkempa/csvMerge

--End of English version--


+Italiano
--Descrizione:
Questo è un programma che funziona come processo e unisce più file di testo in un unico
file di testo.
È progettato per monitorare una cartella di input specificata per nuovi file .csv, unirli in un unico file .csv in una cartella di output,
e quindi eliminare i file originali dalla cartella di input.
--1.Utilizzo
Il programma può essere eseguito utilizzando il file .exe fornito.
Il programma viene eseguito in background e controlla ogni 5 secondi (configurabile) se ci sono nuovi file .csv nella cartella di input.
Se vengono trovati nuovi file, vengono uniti in un unico file .csv nella cartella di output e eliminati dalla cartella di input.
Il file unito viene denominato con il timestamp corrente "T-weeknumber"+"suffix" per evitare di sovrascrivere le fusioni precedenti.
--2.Configurazione
Il programma richiede un file di configurazione denominato "config.json" per specificare la cartella di input e la cartella di output.
Il file viene creato automaticamente nella stessa cartella dell'eseguibile durante la prima esecuzione, se non esiste.
Il file di configurazione deve essere formattato come segue:
	
	{
	  "InputFolder": "C:\\csvMerge\inputfolder",
	  "OutputFolder": "C:\\csvMerge\outputfolder",
	  "FilePattern": "*.csv",
	  "LogFile": "C:\\csvMerge\\csvMerge.log",
	  "CheckInterval": 5000
	}

Dove:
- InputFolder: La cartella in cui il programma cercherà i file CSV da unire.
- OutputFolder: La cartella in cui verrà salvato il file CSV unito.
- FilePattern: Il modello per abbinare i file CSV (il valore predefinito è "*.csv"), è possibile modificarlo in .txt o altri.
- LogFile: Il file in cui verranno scritti i log.
- CheckInterval: L'intervallo (in millisecondi) con cui il programma controlla la presenza di nuovi file (il valore predefinito è 5000 ms).

--3.Logging
Il programma registra le sue attività nel file di log specificato.
Registra quando viene avviato, quando trova nuovi file, quando unisce i file e eventuali errori che si verificano durante l'esecuzione.

--4.Gestione degli errori
Il programma include una gestione di base degli errori per gestire problemi come file di configurazione mancanti, cartelle inaccessibili e errori di lettura/scrittura dei file.
Gli errori vengono registrati nel file di log per la revisione.

--5.Terminazione
Il programma può essere terminato interrompendo il processo dal task manager.

--6.Note
- Alla prima esecuzione, il programma crea le cartelle di input e output se non esistono.
- Assicurarsi che il programma disponga delle autorizzazioni necessarie per leggere dalla cartella di input e scrivere nella cartella di output.
- Il programma unisce i file aggiungendo i loro contenuti. Non verifica le voci duplicate o convalida
 il formato CSV.
- Assicurarsi di eseguire il backup dei dati importanti prima di eseguire il programma, poiché elimina i file spostati dopo la fusione.
- Puoi usarlo con diverse estensioni di file modificando il FilePattern nel file di configurazione.
- Se è necessario che il programma venga eseguito all'avvio o come servizio, potrebbe essere necessario configurarlo utilizzando strumenti o script aggiuntivi,
 come Utilità di pianificazione di Windows o NSSM (Non-Sucking Service Manager), oppure semplicemente posizionare un collegamento all'eseguibile nella cartella di avvio di Windows.

--7.Supporto
Per supporto o domande, contattare lo sviluppatore o fare riferimento alla documentazione fornita con il programma.

--8.Licenza
Questo programma è fornito "così com'è" senza alcuna garanzia. Usalo a tuo rischio.

--9.Autore
Creato da: Tomasz Kempa
Data: 8 novembre 2025

Sito web:
github.com/tomaszkempa/csvMerge

--Fine della versione italiana--

