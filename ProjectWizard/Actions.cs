using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Reactive.Linq;

namespace QuantConnect.ProjectWizard
{
    internal class Actions
    {
        private readonly ApiHelper _apiHelper;

        public Actions(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        
        [CommandLine("list-projects")]
        public void ListProjects()
        {
            var response = _apiHelper.Api.ListProjects();
            if (response.Success)
            {
                foreach (var project in response.Projects)
                {
                    Console.WriteLine($"{project.Name}: {project.ProjectId}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {string.Join("," ,response.Errors)}");
            }
        }

        [CommandLine("upload-on-change", "project id", "filename")]
        public void UploadOnChange(string projectId, string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("File not found: " + filename);
            }
            var fw = new FileSystemWatcher(Path.GetDirectoryName(filename))
            {
                Filter = Path.GetFileName(filename),
                NotifyFilter = NotifyFilters.LastWrite
            };
            var hasher = MD5.Create();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(File.ReadAllText(filename)));

            // The change event can be noisy, so let's debounce
            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(o => fw.Changed += o,
                    o => fw.Changed -= o)
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(args =>
                {
                    var contents = File.ReadAllText(filename);
                    var newHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(contents));
                    if (hash.SequenceEqual(newHash)) return;
                    hash = newHash;
                    var response = _apiHelper.AddOrUpdateFile(int.Parse(projectId), filename, contents);
                    if (!response.Success)
                    {
                        Console.WriteLine($"Error: {string.Join(",", response.Errors)}");
                    }
                });

            fw.EnableRaisingEvents = true;

            while (true) ; // todo: make this more elegant 
        }
    }
}