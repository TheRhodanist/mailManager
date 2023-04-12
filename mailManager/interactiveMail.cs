using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace mailManager
{
    internal class InteractiveMail
    {
        readonly MailManager mailManager;
        Credentials[] creds;
        public InteractiveMail(MailManager mail) {
            mailManager = mail;
        }
        public void StartInteractiveWorkflow()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the interactive MailManager");
            Console.WriteLine("Saved Credentials:");
            LoadSavedCredentials();
            Console.ReadLine();
        }
        private void LoadSavedCredentials()
        {
            creds = MailManager.GetCredentials();
            if(creds == null)
            {
                Console.WriteLine("No saved Credentials found");

            } else
            {
                ChooseAccount();
            }
        }
        private void ChooseAccount()
        {
            Console.WriteLine("Credentials found for:");
            int indexCounter = 0;
            foreach (var credential in creds)
            {
                Console.WriteLine($"({indexCounter})" + credential.name);
                indexCounter++;
            }
            Console.WriteLine("Enter the index for the Mail-Account to be used");
            string input = Console.ReadLine();
            int index = 0;
            try
            {
               index = Int32.Parse(input);
                if (index < 0 || index >= indexCounter)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Not a valid index");
                ChooseAccount();
                return;
            }
            Console.WriteLine($"you choose \"{creds[index].name}\"");
            ConnectAccount(creds[index]);
        }
        private void ConnectAccount(Credentials credentials)
        {

        }
    }
}
