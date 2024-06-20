using System;
using System.IO;
//using client;
//using TimingLoggerLib;
using Serilog;
using System.Configuration;
//using System.Collections.Specialized;
using clientSocket;
//using System.Reflection;


namespace Watchdog
{
    class Watchdog
    {
        static void Main()
        {
            string logPath = ConfigurationManager.AppSettings.Get("logPath")!;
            string shareFolder = ConfigurationManager.AppSettings.Get("shareFolder")!;
            string resultsFolder = ConfigurationManager.AppSettings.Get("resultsFolder")!;
            string serverUrl = ConfigurationManager.AppSettings.Get("serverUrl")!;

            WebSocketImageSender wbSender = new WebSocketImageSender(serverUrl, resultsFolder);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logPath)
                .CreateLogger();

            Log.Information("Start watchdog");
            try
            {
                wbSender.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            using var watcher = new FileSystemWatcher(shareFolder);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            //
            //watcher.Changed += OnChanged;
            watcher.Created += (sender, e) => OnCreated(sender, e, resultsFolder, serverUrl, wbSender);
            watcher.Deleted += OnDeleted;
            watcher.Error += OnError;

            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static async void OnCreated(object sender, FileSystemEventArgs e, 
            string resultsFolder, string serverUrl, WebSocketImageSender wbSender)
        {
            // var watch = System.Diagnostics.Stopwatch.StartNew();
            //var timingLogger = new TimingLogger();

            string file_path = e.FullPath;
            //Console.WriteLine(file_path);
            Log.Information(file_path);
            string fileExtension = Path.GetExtension(e.FullPath).ToLower();
            if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg" || fileExtension == ".bmp")
            {
                try{
                    //timingLogger.LogTimingStart(1, "Post");
                    Thread.Sleep(1000);
                    string imageName = Path.GetFileName(file_path);
                    byte[] imageData = File.ReadAllBytes(file_path);
                    List<IElaboratedImageCoordinates> coordinates = await wbSender.sendImageAsync(imageData, imageName, file_path);
                    
                    foreach (var coordinate in coordinates)
                    {
                        Console.WriteLine($"X: {coordinate.CoordinateX}, Y: {coordinate.CoordinateY}");
                    }

                    // Console.WriteLine("Elapsed time: " + elapsedMs + "ms");
                }
                catch (Exception ex) { 
                    Log.Error(ex.ToString());
                    //Console.WriteLine("Error uploading image: " + ex.Message);
                }
                finally
                {
                    await Log.CloseAndFlushAsync(); // ensure all logs written before app exits
                }
            }
                     
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
