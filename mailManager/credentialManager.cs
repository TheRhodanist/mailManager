using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mailManager
{
    internal class CredentialManager
    {
        internal Credentials SessionCredentials { get; }
        /// <summary>
        /// Load saved credentials from the provided path, or the default credentials.json if none is provided
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Array of credentials, null if none is found</returns>
        internal static Credentials[]? LoadCredentialsFromJson(string path = "credentials.json")
        {

            Credentials[]? credentials;
            string json = "";
            Console.WriteLine(path);
            using StreamReader r = new(path);
            try
            {
                json = r.ReadToEnd();
            }
            catch (Exception)
            {
                Console.WriteLine("no file with credentials found");
                return null;
            }

            return credentials = JsonConvert.DeserializeObject<Credentials[]>(json);
        }
    }
}
