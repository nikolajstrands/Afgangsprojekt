using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using IdentityModel.Client;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using System.Linq;
using IngestApp.Models;
using System.Diagnostics;


namespace IngestApp
{
    class Program
    {

        static void Main(string[] args){

            var authUrl = System.Environment.GetEnvironmentVariable("VM_AUTH_SERVER");
            var catalogApiUrl = System.Environment.GetEnvironmentVariable("CATALOG_LOAD_BALANCER");
            var streamingServerUrl = Environment.GetEnvironmentVariable("STREAMING_SERVER");
            
            var watchedFolder = System.Environment.GetEnvironmentVariable("INGEST_FOLDER_PATH");

            if (authUrl == null || catalogApiUrl == null || streamingServerUrl == null)
            {
                Console.WriteLine("Appen kan ikke åbnes. Autorisationsserver, katalog-api'ets og streaming-serverens URL'er skal være sat som environment-variable.");
                return;
            }
            
            if(watchedFolder == null)
            {
                Console.WriteLine("Appen kan ikke åbnes. Der skal være sat en mappe til overvågning i environment-variable");                 
                return;
            }
            
            Console.WriteLine("IngestApp er klar (tryk 'q' for at afslutte)\n");

            var watcher = new FolderWatcher(watchedFolder);
            Task.Factory.StartNew(() => watcher.Start());    
               
            while(Console.ReadKey().Key != ConsoleKey.Q){}
            Console.WriteLine();

        }
    }

}
