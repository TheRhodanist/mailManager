using System;

using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Net.Imap;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;

namespace mailManager
{
    

    public class mailManager
    {
        readonly bool isInteractive = false;
        public static void Main(String[] args)
        {
            if(args.Length != 0) {
                if (args[0]=="/i")
                {

                }
                Console.WriteLine("");
                Console.ReadLine();
            }




            try
            {
                mailManager mail = new();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }

        }

        private Credentials credentials;
        public mailManager()
        {
            isInteractive = true;
            GetCredentials();
            QueryServers();
        }
        private void QueryServers()
        {
            using (var client = new ImapClient())
            {
                client.Connect("secureimap.t-online.de", 993, true);

                client.Authenticate(credentials.name, credentials.password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);
                Console.WriteLine("\nSearch Folders:");
                foreach (var nmspace in client.PersonalNamespaces)
                {
                    PrintFolders(client,nmspace);
                }
                

                //for (int i = 0; i < inbox.Count; i++)
                //{
                //    var message = inbox.GetMessage(i);
                //    Console.WriteLine("Subject: {0}", message.Subject);
                //}

                client.Disconnect(true);
                Console.ReadLine();
            }

        }
        private void PrintFolders(ImapClient client, FolderNamespace nmspace)
        {
            Console.WriteLine("Printing for Namespace: "+nmspace.Path+" with seperator \""+nmspace.DirectorySeparator+"\"");
            var folder = client.GetFolders(nmspace, false);
            foreach (var fol in folder)
            {
                Console.WriteLine(fol.FullName);
            }
        }
        private void GetCredentials(string path = "credentials.json")
        {
            //string dir = System.IO.Path.GetDirectoryName(
                //System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string file = dir + path;
            Console.WriteLine(path);
            using StreamReader r = new(path);
            string json = r.ReadToEnd();
            credentials = JsonConvert.DeserializeObject<Credentials>(json);
            Console.WriteLine(credentials.name);
        }
    }
    public struct Credentials
    {
        public string name;
        public string password;
    }
    
}

