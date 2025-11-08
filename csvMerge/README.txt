csvMerge v1.0
=================================
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
- Make sure to back up important data before running the program, as it deletes the original files after merging.
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


--End of README.txt--
