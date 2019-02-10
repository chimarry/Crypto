using Crypto.Cryptography;
using Crypto.database.Entity;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Crypto
{
    /// <summary>
    /// Interaction logic for DecryptionWindow.xaml
    /// </summary>
    public partial class DecryptionWindow : Window
    {
        private User loggedUser;
        private List<User> availableUsers;
        private string filePath;
        public DecryptionWindow()
        {
            InitializeComponent();
        }

        internal User LoggedUser { get => loggedUser; set => loggedUser = value; }
        internal List<User> AvailableUsers { get => availableUsers; set => availableUsers = value; }
        internal void Fill()
        {
            availableUsers.RemoveAll(x => x.Username == loggedUser.Username);
            FillSenderComboBox();
        }
        internal void FillSenderComboBox()
        {
            foreach (User user in availableUsers)
                SenderComboBox.Items.Add(user.Username);
            BtnCompile.Visibility = Visibility.Hidden;
            BtnExecute.Visibility = Visibility.Hidden;
        }
        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {

            EncDec.receiver = loggedUser;
            if (SenderComboBox.SelectedItem == null)
            {
                MessageBox.Show("No selected sender.");
                return;
            }
            EncDec.sender = database.Database.GetUser(SenderComboBox.SelectedItem.ToString());
            if (AsymmetricEncDec.CheckValidation(EncDec.sender))
            {
                MessageBox.Show("Decryption failed. Certificate of selected user is not valid and its identity could not be confirmed.");
            }
            else
            {
                filePath = EncDec.Decrypt(BoxFilePath.Text);
                if (filePath!=null)
                {
                    MessageBox.Show("Decryption successful.");
                    BtnCompile.Visibility = Visibility.Visible;
                    BtnExecute.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Decryption failed.Data could not be decrypted.");
            }

        }

        private void BtnCompile_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Users"; //position in user directory
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".cs";
            dlg.Filter = "Encrypted files (.cs)|*.cs";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BoxFilePath.Text = filename;
            }
        }


    }
}