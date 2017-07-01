using System;
using System.IO;

namespace QuantConnect.ProjectWizard
{
    using Api;
    
    public class ApiHelper
    {
        public Api Api { get; }

        public ApiHelper(Api api)
        {
            Api = api;
        }

        public RestResponse AddOrUpdateFile(int projectId, string path, string contents)
        {
            var response = Api.UpdateProjectFileContent(projectId, Path.GetFileName(path), contents);

            if (response.Success) return response;
            
            if (response.Errors[0].Contains("File not found"))
            {
                response = Api.AddProjectFile(projectId, Path.GetFileName(path), contents);
            }

            return response;
        }
    }
}