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


            if (args.Length != 0)
            {
                if (args[0] == "/i")
                {
                    InteractiveMail inter = new(mail);
                    inter.StartInteractiveWorkflow();
                }
                Console.WriteLine("");
                Console.ReadLine();
            }
            else
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

        internal Credentials[]? credentials;
        private ImapClient? imapClient;
        private IMailFolder? inbox;
        public MailManager()
        {
            credentials = Array.Empty<Credentials>();
            isInteractive = true;

        }
        internal void AutomatedLogin()
        {
            credentials = CredentialManager.LoadCredentialsFromJson();
            if (credentials == null)
            {
                Console.WriteLine("No saved Credentials found");
                return;
            }
            for (int i = 0; i < credentials.Length; i++)
            {
                QueryServer(credentials[i],true);
                CloseConnection();
            }

        }
        internal void QueryServer(Credentials cred, Boolean automated = false)
        {
            imapClient = new ImapClient();
            try
            {
                imapClient.Connect(cred.host, 993, true);
                imapClient.Authenticate(cred.name, cred.password);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection could not be established:");
                Console.WriteLine(e.Message);
                imapClient.Dispose();
                imapClient = null;
                return;
            }

            // The Inbox folder is always available on all IMAP servers...
            inbox = imapClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            Console.WriteLine("Total messages: {0}", inbox.Count);
            Console.WriteLine("Recent messages: {0}", inbox.Recent);
            Console.WriteLine("\nSearch Folders:");
            foreach (var nmspace in imapClient.PersonalNamespaces)
            {
                PrintFolders(imapClient, nmspace);
            }


            //for (int i = 0; i < inbox.Count; i++)
            //{
            //    var message = inbox.GetMessage(i);
            //    Console.WriteLine("Subject: {0}", message.Subject);
            //}



        }

        /// <summary>
        /// Prints a list of folder for a given namespace using the provided client
        /// </summary>
        /// <param name="client">The Client/Connection on which this should be executed</param>
        /// <param name="nmspace">Namespace for which its folders are to be printed</param>
        internal static void PrintFolders(ImapClient client, FolderNamespace nmspace)
        {
            Console.WriteLine("Printing for Namespace: " + nmspace.Path + " with seperator \"" + nmspace.DirectorySeparator + "\"");
            var folder = client.GetFolders(nmspace, false);
            foreach (var fol in folder)
            {
                Console.WriteLine(fol.FullName);
            }
        }


        /// <summary>
        /// Adds a folder under inbox with the specified name
        /// </summary>
        /// <param name="folderName">Name of the folder to be added</param>
        /// <returns>The newly created folder</returns>
        internal void AddFolder(string folderName)
        {
            inbox.Create(folderName, true);
        }


        /// <summary>
        /// Deletes a specified folder under inbox, provided the folder is empty
        /// </summary>
        /// <param name="folderName">Name of the folder to delete</param>
        internal void DeleteFolder(string folderName)
        {
            IMailFolder folder = inbox.GetSubfolder(folderName);
            if (folder == null)
            {
                Console.WriteLine("No such Folder");
                return;
            }
            folder.Open(FolderAccess.ReadOnly);
            if (folder.Count != 0) { Console.WriteLine("Folder not empty"); return; }
            folder?.Delete();
        }


        /// <summary>
        /// Outputs the number of messages in a specified folder
        /// </summary>
        /// <param name="folderName">Name of the folder from which messages are to be counted</param>
        internal void CountFolder(string folderName)
        {
            IMailFolder folder = inbox.GetSubfolder(folderName);
            if (folder == null)
            {
                Console.WriteLine("No such Folder");
                return;
            }
            folder.Open(FolderAccess.ReadOnly);
            Console.WriteLine($"There are {folder.Count} messages in this folder");
        }
        internal void CloseConnection()
        {
            imapClient?.Disconnect(true);
        }
    }
    public struct Credentials
    {
        public string host;
        public int port;
        public string name;
        public string password;
    }

}

