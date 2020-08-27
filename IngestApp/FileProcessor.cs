using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using IdentityModel.Client;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.IO.Compression;
using System.Linq;
using IngestApp.Models;
using System.Diagnostics;

namespace IngestApp
{
    class FileProcessor
    {
        private string _watchedFolder = "";
        public FileProcessor(string watchedFolder)
        {
            _watchedFolder = watchedFolder;

        }

         // Metoder der processerer filer fra køen serielt
        public void ProcessFiles(Queue<string> fileQ)
        {
            Log("Overvåger mappen " + _watchedFolder + " for nye zip-filer");

            while(true){

                if(fileQ.Count > 0)
                {
                    var currentFile = fileQ.Peek();
                    ProcessFile(currentFile).Wait();
                    fileQ.Dequeue();        
                }

                Thread.Sleep(2000); 
            }
        }

        // Metoder der processerer en enkelt zip-fil     
        async Task ProcessFile(string filePath){

            var workingDirectoryPath = Path.GetDirectoryName(filePath) + "/work/";
 
            Console.WriteLine("\n");
            Log(Path.GetFileName(filePath) + " detekteret - processering igangsat");

            try
            {
                // Udpak zip-filen til arbejdsmappen
                ExtractToWorkingDirectory(filePath, workingDirectoryPath);

                // Er rodmappen korrekt navngivet i den udpakkede zip-fil?
                var packageRoot = ValidateZipRoot(filePath, workingDirectoryPath);

                // Findes metadata-fil?
                var metadataFile = MetadataFileExists(packageRoot);

                // Er metadatafilen wellformed og valid?
                var metadataXmlDoc = CheckForWellformedAndValidXml(metadataFile);

                // Deserialiser XML-fil
                var newAlbumRequest = DeserializeAlbums(metadataXmlDoc);

                // Validér pakkestruktur
                ValidateStructure(packageRoot, newAlbumRequest);

                // Hent access token hos autorisationserver
                var accessToken = await GetAccessToken();

                // Upload metadata til CatalogApi
                var newAlbum = await PostMetadata(newAlbumRequest, accessToken);

                // Transkod tracks
                TranscodeTracks(packageRoot, newAlbum);

                // Upload tracks til StreamingServer
                await UploadTracks(newAlbum, accessToken);

            }
            catch (Exception)
            {
                Log("Processeringen er afbrudt", true);
                return;
            }
            finally
            {
                CleanUp(filePath, workingDirectoryPath);
            }      
        }

        // Hjælpemetode til at udskrive statusbeskeder
        private void Log(string message, bool error = false)
        {  
            if(error){      
                ConsoleColor currentForeground = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(DateTime.Now.ToString() + " - FEJL - " + message);

                Console.ForegroundColor = currentForeground;
                
            } else {
                 Console.WriteLine(DateTime.Now.ToString() + " - INFO - " + message);
            }                
        }

        // Udpakker zip-filen til arbejdsmappen
        private void ExtractToWorkingDirectory(string filePath, string workingDirectoryPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(filePath, workingDirectoryPath);  
                Log("Zip-filen blev udpakket");

            } 
            catch(Exception ex)
            {
                Log("Zip-filen kunne ikke udpakkes", true);
                throw ex;
            }          
        }
        
        // Valider at zip-filen er udpakket til en mappe med samme navn
         private string ValidateZipRoot(string fileFullPath, string workingDirectoryPath)
        {
            // Zip-filens navn uden endelse
            string zipName = Path.GetFileNameWithoutExtension(fileFullPath);

            // Forventet udpakket rodmappe
            string expectedRootPath = workingDirectoryPath + zipName;

            if(Directory.Exists(expectedRootPath)){

                Log("Rodmappe i zip-fil navngivet korrekt");

                return expectedRootPath;
                
            } else {

                Log("Rodmappen i zip-filen er ikke navngivet korrekt", true);
                throw new Exception();
            }             
        }

        // Tjek om XML-fil er well-formed og valid
        private XDocument CheckForWellformedAndValidXml(string metadataFilePath){

            // Tjek om filen indeholder velformet xml
            XDocument doc = new XDocument();

            using(XmlReader reader = XmlReader.Create(metadataFilePath))
            {
                try
                {
                    doc = XDocument.Load(reader);
                    Log("XML-dokumentet blev indlæst succesfuldt - det er velformet");
                }
                catch(Exception ex)
                {
                    Log("Dokumentet kunne ikke indlæses, det er ikke velformet XML", true);
                    throw ex;
                }
            }  

            // Tjek om filer indeholder valid xml i henhold til skema
            XmlSchemaSet schema = new XmlSchemaSet();  
            
            // Indlæs XML Schema-fil
            try
            {
                schema.Add("", "./input.xsd");  
            }
            catch(Exception ex)
            {
                Log("Der kunne ikke findes en XML Schema-fil med navnet 'input.xsd'", true);
                throw ex;
            }

            // Valider metadata-fil  
            try
            {            
                 doc.Validate(schema, null); 
                 Log("Dokumentet indeholder valid XML");
                 return doc;         
            }
            catch(XmlSchemaValidationException ex)
            {
                Log("Dokumentet indeholder ikke valid XML", true);
                throw ex;
            }                              
        }

        // Tjek om den nye rodmappe indeholder en metadatafil med korrekt navn
        private string MetadataFileExists(string packageRoot)
        {
            // Liste med filer i den nye rod-mappe
            string [] files = Directory.GetFiles(packageRoot);

            // Er der præcis én fil i pakkens rod?
            if(files.Length == 1)
            {
                // Er metadatafilen navngivet korrekt
                if(Path.GetFileName(files[0]) == "metadata.xml")
                {
                    Log("Metadatafilen findes.");
                    return files[0];       
                }
                else
                {
                    Log("Der findes en fil roden af pakken, men den er navngivet forkert - skal hedde 'metadata.xml'", true);
                    throw new Exception();
                } 
            } 
            else 
            { 
                Log("Der er ikke det korrekte antal filer i roden af pakken - der skal være netop én", true);
                throw new Exception();
            }
              
        }

        // Deserialiser albummet til et AlbumRequestDTO-objekt
        private AlbumRequestDTO DeserializeAlbums(XDocument metadataXmlDoc)
        {
            try
            {
                // Deserialiser XML til objekt 
                AlbumRequestDTO newAlbum = new AlbumRequestDTO();

                XmlSerializer xmlFormat = new XmlSerializer(typeof(AlbumRequestDTO));

                using(XmlReader xmlReader = metadataXmlDoc.Root.CreateReader())
                {
                    newAlbum = (AlbumRequestDTO)xmlFormat.Deserialize(xmlReader);
                }

                Log("XML-filen blev deserialiseret korrekt");
                return newAlbum;
            }
            catch(Exception ex)
            {
                Log("XML-filen kunne ikke deserialiseres:\n" + ex, true);
                throw ex;  
            }         

        }
        
        // Validér pakkestruktur (mappenavn og track-numre)
        private void ValidateStructure(string packageRoot, AlbumRequestDTO album)
        {
            // Track-numre fra metadata
            List<int> trackNumbers = album.Tracks.Select(a => a.Number).ToList();

            // Liste med mapper i rod-mappen
            string [] directories = Directory.GetDirectories(packageRoot);

            if(directories.Length == 1)
            {
                // Pakken indeholder netop én mappe

                var dataFolder = directories[0];

                if(Path.GetRelativePath(packageRoot, dataFolder) == "data")
                {
 
                    // Mappen er navngivet korrekt;
                    string [] musicFiles = Directory.GetFiles(dataFolder);
                    
                    List<int> fileNames = new List<int>();
                    
                    foreach(string file in musicFiles)
                    {
                        // Tjek filendelse
                        if(Path.GetExtension(file) != ".wav")
                        {
                            Log("En track-fil har ikke endelse '.wav'", true);
                            throw new Exception();
                        }

                        try
                        {
                            // Forsøg at parse track-filnavn til integer
                            fileNames.Add(Int32.Parse(Path.GetFileNameWithoutExtension(file)));
                        }
                        catch(Exception ex)
                        {
                            Log("Et filnavn er ikke et tal", true);
                            throw ex;
                        }                     
                    }

                    // Sammenlign tracknumre fra metadata med filnavne
                    trackNumbers.Sort();
                    fileNames.Sort();
                    bool areEqual = trackNumbers.SequenceEqual(fileNames);

                    if(areEqual)
                    {
                        Log("Datapakkens struktur er korrekt");
                    }
                    else
                    {
                        Log("Track-numre i metadata og i datamappen stemmer ikke overens" , true);
                        throw new Exception();
                    }
                }
                else
                {
                    Log("Datamappen er navngivet forkert - skal hedde 'data'" , true);
                    throw new Exception();
                }
            } 
            else 
            { 
                Log("Der er ikke det korrekte antal mapper i pakkens rod - der skal være én", true);
                throw new Exception();
            }   
        }

        // Hent access token hos autorisationsserver
        private async Task<string> GetAccessToken()
        {

            // Undersøg discovery endpoint for metadata
            var authClient = new HttpClient(); 

            // Hent autorisationsserveren name fra environment-variable
            var authUrl = System.Environment.GetEnvironmentVariable("VM_AUTH_SERVER");
            
            var disco = await authClient.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest {
                    Address = authUrl,
                    Policy =
                    {   
                        // Vi slår HTTPS fra til test
                        RequireHttps = false
                    }
                }
            );
            
            if(disco.IsError)
            {
                Log("Kan ikke få adgang til autorisationsserveren", true);
                throw new Exception();
            } 
            else
            {
                Log("Discovery-dokument hentet hos autorisationsserver");
            }

            // Anmod om access token
            var tokenResponse = await authClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {   
                    // Discovery endpointet har givet info om alle endpoints
                    Address = disco.TokenEndpoint,

                    // Credentials og scope
                    ClientId = "IngestApp",
                    ClientSecret = "secret",
                    Scope = "CatalogApi:Write StreamingServer:Write"
                }
            );

            if (tokenResponse.IsError)
            {
                Log("Kunne ikke hente access token hos autorisationsserver:\n" + tokenResponse.Error, true);
                throw new Exception();
            }
            else
            {
                Log("Access token hentet hos autorisationsserver");
                return tokenResponse.AccessToken;
            }
        }

        // Upload data til CatalogApi
        private async Task<AlbumDTO> PostMetadata(AlbumRequestDTO newAlbumRequest, string accessToken)
        {
            
            // Send albummet til CatalogApi
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);

            string newAlbumRequestAsJson = JsonConvert.SerializeObject(newAlbumRequest);

            // Hent url i environment-variable
            var catalogApiUrl = System.Environment.GetEnvironmentVariable("CATALOG_LOAD_BALANCER");
            
            try
            {
                var response = await apiClient.PostAsync(catalogApiUrl, 
                                new StringContent(newAlbumRequestAsJson, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    Log("XML-data kunne ikke sendes til katalog-API'et", true);
                    throw new Exception();   
                }
                else
                {
                    try
                    {
                        var newAlbum = JsonConvert.DeserializeObject<AlbumDTO>(await response.Content.ReadAsStringAsync());
                        Log("XML-data blevet sendt succesfuldt til album-API'et");  
                        return newAlbum;

                        }
                        catch(Exception e)
                        {
                            Log("Data fra katalog-API'et kunne ikke deserialiseres", true);
                            Console.WriteLine(e);
                            throw new Exception();   
                        }          
                }       
    
            } 
            catch(System.Net.Http.HttpRequestException)
            {
                Log("Der kunne ikke oprettes forbindelse til katalog-API'et", true);
                throw new Exception();
            }
        }

        // Her transkodes tracks
        private void TranscodeTracks(string packageRoot, AlbumDTO newAlbum)
        {
            // Lav ny mappe til transkodede tracks
            try
            {
                var trackDirectory = Directory.CreateDirectory(_watchedFolder + "/work/tracks");

            }
            catch(Exception ex)
            {
                Log("Der kunne ikke oprettes en 'track'-mappe", true);
                throw ex;
            }

            Log("Transkodning igangsat ...");

            foreach(TrackDTO track in newAlbum.Tracks)
            {              
                TranscodeTrack(packageRoot, track);
            }

            Log("Transkodning afsluttet");          

        }

        // Her transkodes et enkelt track
        private void TranscodeTrack(string packageRoot, TrackDTO track)
        {
            var trackPath = packageRoot + "/data/" + track.Number + ".wav";
            
            Log("Transkodning af " + track.Number + ".wav" + " påbegyndt ...");

            try
            {
                // Vi opretter en specifik mappe til dette track
                var singleTrackDirectory = Directory.CreateDirectory(_watchedFolder + "/work/tracks/" + track.Id);

                // FFmpeg-kommando konstrueres
                var cmdText = "-i \"" + trackPath + 
                "\" -c:a aac -b:a:0 128k -b:a:1 256k -map 0:a -map 0:a -f hls -hls_time 15 -hls_list_size 0 -var_stream_map \"a:0 a:1\"" +
                " -master_pl_name " + track.Id + ".m3u8 -hls_segment_filename " + singleTrackDirectory.FullName + "/" + track.Id + "_v%v/%03d.ts " +
                singleTrackDirectory.FullName + "/" + track.Id + "_v%v/output.m3u8";
                
                // En ny FFmpeg-proces defineres
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/Users/nikolajstrands/IT/Afgangsprojekt/IngestApp/ffmpeg",
                        Arguments = cmdText,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                
                // Processen startes
                process.Start();
                string errorOutput = process.StandardError.ReadToEnd();
                string standardOutput = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception();
                }

                Log("Transkodning af " + track.Number + ".wav" + " afsluttet");
            }
            catch (Exception ex)
            {
                Log("Noget gik galt under transkodning af " + trackPath, true);
                throw ex;
            }

        }

        // Tracks uploades til StreamingServer
        private async Task UploadTracks(AlbumDTO newAlbum, string accessToken)
        {
            Log("Upload af tracks igangsat ...");

            foreach (TrackDTO track in newAlbum.Tracks)
            {
                await UploadTrack(track, accessToken);
            }

            Log("Upload af tracks afsluttet");
        }

        // Et enkelt track uploades til StreamingServeren
        private async Task UploadTrack(TrackDTO track, string accessToken)
        {
                var trackFolder = _watchedFolder + "/work/tracks/" + track.Id;
                var zipFilePath = trackFolder + ".zip";

                try
                {
                    // Zip track-mappen
                    ZipFile.CreateFromDirectory(trackFolder, zipFilePath);
                }
                catch(Exception ex)
                {
                    Log(zipFilePath + " kunne ikke oprettes", true);
                    throw ex;
                }
                
                // Opret HTTP-klient
                var uploadClient = new HttpClient();
                // Access token sættes som authorization header
                uploadClient.SetBearerToken(accessToken);

                // Nyt HTTP-formular-dataindhold oprettes
                var form = new MultipartFormDataContent();    

                // Zip-files indlæses som byte-array
                var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(zipFilePath));

                // Filheader sættes
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                // Fil tilføjes til formular-data med navnet 'file'
                form.Add(fileContent, "file", Path.GetFileName(zipFilePath));

                // Streamingserver-url hentes fra environment-variabel
                var streamingServerUrl = Environment.GetEnvironmentVariable("STREAMING_SERVER");

                // Data sendes til StreamingServer med POST
                var response = await uploadClient.PostAsync(streamingServerUrl, form);

                // Tjek om det gik godt
                if(!response.IsSuccessStatusCode)
                {
                    Log(zipFilePath + " kunne ikke uploades til streamingserveren.", true);
                    throw new Exception();
                }
                else
                {
                    Log(track.Number + ".wav blev uploadet");
                }             
        }

        private void CleanUp(string filePath, string workingDirectoryPath)
        {

            // Rodmappen i zip-filen
            string zipRoot = Path.GetFileNameWithoutExtension(filePath);

            // Sti til den udpakkede rod-mappe
            string zipRootPath = workingDirectoryPath + zipRoot;
        
            // Slet mappen i work-mappen
            Directory.Delete(zipRootPath, true);

            // Sti til 'tracks'-mappe
            string trackFolderPath = workingDirectoryPath +"/tracks/";

            // Slet 'tracks'-mappen 
            Directory.Delete(trackFolderPath, true);

            // Flyt zip-filen til 'trash'
            try
            {
                string fileName = Path.GetFileName(filePath);
                string newPath = Path.GetDirectoryName(filePath) + "/trash/" + fileName;
                
                File.Move(filePath, newPath);
                
                Log(Path.GetFileName(filePath) + " blev flytte til mappen 'trash'");
                Log("Processering af " + Path.GetFileName(filePath) + " afsluttet");

            }
            catch(Exception)
            {
                Log("Filen kunne ikke rykkes til mappen 'trash'", true);
                return;
            }
        }
    }
}