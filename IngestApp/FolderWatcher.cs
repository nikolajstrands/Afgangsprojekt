using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IngestApp
{
    class FolderWatcher
    {
        private static string _watchedFolder = "";
        private static Queue<string> _fileQ = new Queue<string>();

        public FolderWatcher(string watchedFolder)
        {
            _watchedFolder = watchedFolder;
        }

        public void Start()
        {
            if(Directory.Exists(_watchedFolder))
            {
                try
                {

                    Directory.CreateDirectory(_watchedFolder + "/trash");
                    
                    Directory.CreateDirectory(_watchedFolder + "/work");

                }
                catch(Exception)
                {
                    Console.WriteLine("Undermapper til den angivne mappe kunne ikke oprettes");
                    return;
                }                
            }
            else
            {
                    Console.WriteLine("Den angivne mappe eksisterer ikke");
                    return;          
            }
       
            try 
            {
                Task.Factory.StartNew(() => this.Watch());
            } 
            catch(ArgumentException)
            {
                Console.WriteLine("Den angivne mappe kunne ikke overvåges");
                return;
       
            }

            // Opret en fil-processor
            var processor = new FileProcessor(_watchedFolder);

            // Begynd processering på ny tråd
            Task.Factory.StartNew(() => processor.ProcessFiles(_fileQ));         

            Console.ReadKey();
        }

        private void Watch()
        {
            while(true)
            {
                // Indeholder nu alle filer i mappen
                var files = Directory.GetFiles(_watchedFolder, "*.zip", SearchOption.TopDirectoryOnly);

                foreach(string file in files)
                {
                        if(!_fileQ.Contains(file))
                        {
                            // Tilføj filen til processeringskø
                            _fileQ.Enqueue(file);
                        }           
                }

                Thread.Sleep(1000);         
            }
        }

    }
}
