
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Crypto.Cryptography;
using Crypto.database;
using Crypto.database.Entity;

namespace Crypto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            Database database = new Database();
            User loggedUser = database.Read(BoxUsername.Text, HashingPassword.Hash(BoxPassword.Password),BoxCertPath.Text);
            if (loggedUser == null)
            {
                MessageBox.Show("Failed login.");
            }
            else
            {
                MessageBox.Show("Login succesfull.");

                MessageBoxResult result = MessageBox.Show("Do you want to enter decryption mode?",
                                              "Confirmation",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);
                Hide();
                if (result == MessageBoxResult.No)
                {
                    EncryptionWindow encryptionWindow = new EncryptionWindow
                    {
                        LoggedUser = loggedUser,
                        AvailableUsers = database.ReadAll()
                    };
                    encryptionWindow.Fill();
                    encryptionWindow.Show();
                }
                else
                {
                    DecryptionWindow decryptionWindow = new DecryptionWindow
                    {
                        LoggedUser=loggedUser,
                        AvailableUsers=database.ReadAll()
                    };
                    decryptionWindow.Fill();
                    decryptionWindow.Show();
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Database database = new Database();
           
            string userFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Users\\" + BoxUsername.Text;
            userFolder = new Uri(userFolder).LocalPath;
            Directory.CreateDirectory(userFolder);
            
            if (!database.Write(new User(BoxUsername.Text, HashingPassword.Hash(BoxPassword.Password), BoxCertPath.Text, userFolder)))
            {
                MessageBox.Show("Failed register. Username already exists.");
            }
            else MessageBox.Show("Register success.");
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)+"\\Certs"; //position in user directory
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".p12";
            dlg.Filter = "Certificate documents (.p12)|*.p12";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BoxCertPath.Text = filename;
            }

        }


    }
}

