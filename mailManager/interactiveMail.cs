﻿using MailKit.Net.Imap;
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
        Credentials[]? creds;
        Credentials SessionCredentials;
        public InteractiveMail(MailManager mail)
        {
            mailManager = mail;
        }
        public void StartInteractiveWorkflow()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the interactive Mail Manager");
            Console.WriteLine("Saved Credentials:");
            LoadSavedCredentials();
            Console.ReadLine();
        }
        private void LoadSavedCredentials()
        {
            creds = CredentialManager.LoadCredentialsFromJson();
            if (creds == null)
            {
                Console.WriteLine("No saved Credentials found");

            }
            else
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
            Console.WriteLine("Enter the index for the Mail-Account to be used or type 'manual' to manually supply credentials");
            string input = Console.ReadLine();
            if (input == "manual")
            {
                Console.WriteLine("Host:");
                SessionCredentials.host = Console.ReadLine();
                Console.WriteLine("Port:");
                try
                {
                    SessionCredentials.port = Int32.Parse(Console.ReadLine());
                } catch (Exception e)
                {

                }
                Console.WriteLine("Username:");
                input = Console.ReadLine();
                SessionCredentials.name = input != null ? input : "";
                Console.WriteLine("Password:");
                var pass = string.Empty;
                ConsoleKey key;
                do
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        Console.Write("\b \b");
                        pass = pass[0..^1];
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write("*");
                        pass += keyInfo.KeyChar;
                    }
                } while (key != ConsoleKey.Enter);
                SessionCredentials.password = pass;
                Console.WriteLine();
                Console.WriteLine("The Password You entered is : " + pass);

                ConnectAccount(SessionCredentials);
            }
            else
            {
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
                MainMenu();
            }

        }
        /// <summary>
        /// Controls the workflow while the interactive user is in the main menu 
        /// </summary>
        private void MainMenu()
        {
            do
            {
                Console.WriteLine("Options:");
                Console.WriteLine("Write /q to quit");
                Console.WriteLine("Write /c to count the messages in a folder");
                Console.WriteLine("Write /a to add a folder");
                Console.WriteLine("Write /d to delete a folder");


                string input = Console.ReadLine();
                if (input == null) { MainMenu(); return; }
                switch (input)
                {
                    case "/q": mailManager.CloseConnection(); return;
                    case "/a": OptionFolderManipulation("Add a folder", mailManager.AddFolder); break;
                    case "/d": OptionFolderManipulation("Delete a folder", mailManager.DeleteFolder); break;
                    case "/c": OptionFolderManipulation("Count messages of folder", mailManager.CountFolder); break;
                    default:
                        Console.WriteLine("Not a recognized command");
                        break;
                }
            } while (mailManager != null);

        }
        private void OptionFolderManipulation(string description, Action<string> f)
        {
            Console.WriteLine(description);
            Console.WriteLine("Input Folder Name");
            string input = Console.ReadLine();
            f(input);
        }
        private void ConnectAccount(Credentials credentials)
        {
            mailManager.QueryServer(credentials);
        }

    }
}
