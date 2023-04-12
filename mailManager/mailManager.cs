using System;

using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Net.Imap;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;

namespace mailManager
{
    

    public class MailManager
    {
        readonly bool isInteractive = false;

        public static void Main(String[] args)
        {
            MailManager mail = new();
            

            if (args.Length != 0) {
                if (args[0]=="/i")
                {
                    InteractiveMail inter = new(mail);
                    inter.StartInteractiveWorkflow();
                }
                Console.WriteLine("");
                Console.ReadLine();
            } else
            {
                try
                {
                    mail.AutomatedLogin();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                
            }




            

        }

        internal Credentials[] credentials;
        public MailManager()
        {
            credentials = Array.Empty<Credentials>();
            isInteractive = true;
            
        }
        internal void AutomatedLogin()
        {
            credentials = GetCredentials();
            if(credentials == null)
            {
                Console.WriteLine("No saved Credentials found");
                return;
            }
            QueryServers();
        }
        internal void QueryServers()
        {
            using (var client = new ImapClient())
            {
                client.Connect("secureimap.t-online.de", 993, true);

                client.Authenticate(credentials[0].name, credentials[0].password);

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
        internal void PrintFolders(ImapClient client, FolderNamespace nmspace)
        {
            Console.WriteLine("Printing for Namespace: "+nmspace.Path+" with seperator \""+nmspace.DirectorySeparator+"\"");
            var folder = client.GetFolders(nmspace, false);
            foreach (var fol in folder)
            {
                Console.WriteLine(fol.FullName);
            }
        }
        internal static Credentials[] GetCredentials(string path = "credentials.json")
        {
            Credentials[] credentials;
            string json = "";
            //string dir = System.IO.Path.GetDirectoryName(
            //System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string file = dir + path;
            Console.WriteLine(path);
            using StreamReader r = new(path);
            try
            {
                json = r.ReadToEnd();
            }
            catch (Exception)
            {
                Console.WriteLine("Credentials.json not found");
            }
            
            return credentials = JsonConvert.DeserializeObject<Credentials[]>(json);
        }
    }
    public struct Credentials
    {
        public string name;
        public string password;
    }
    
}

