using Crypto.Cryptography;
using Crypto.database.Entity;
using System;
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
    /// Interaction logic for EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        private User loggedUser;
        private List<User> availableUsers;
        public EncryptionWindow()
        {
            InitializeComponent();
            
        }
        internal void Fill() {
            availableUsers.RemoveAll(x => x.Username == loggedUser.Username);
            FillUserComboBox();
            FillEncAlgComboBox();
            FillHashAlgComboBox();
        }
        internal void FillUserComboBox()
        {
            foreach(User user in availableUsers)
                UserComboBox.Items.Add(user.Username);
        }
        internal void FillEncAlgComboBox()
        {
            foreach (var x in Enum.GetValues(typeof(Cryptography.SymetricEncDec.Algs)))
               AlgComboBox.Items.Add(x.ToString());
        }
        internal void FillHashAlgComboBox()
        {
            foreach (var x in Enum.GetValues(typeof(Cryptography.HashAlg.Algs)))
                HashComboBox.Items.Add(x.ToString());
        }
        internal User LoggedUser { get => loggedUser; set => loggedUser = value; }
        internal List<User> AvailableUsers { get => availableUsers; set => availableUsers = value; }

        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            EncDec.sender = loggedUser;
            EncDec.receiver = database.Database.GetUser(UserComboBox.SelectedItem.ToString());
            if (AsymmetricEncDec.CheckValidation(EncDec.receiver))
            {
                MessageBox.Show("Encryption failed. Certificate of selected user is not valid.");
            }
            else
            {
                try
                {
                    if (EncDec.Encrypt(AlgComboBox.SelectedItem.ToString(), HashComboBox.SelectedItem.ToString(), BoxFilePath.Text))
                    {
                        MessageBox.Show("Encryption successful.");
                    }
                    else
                        MessageBox.Show("Encryption failed.Data could not be encrypted.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Key usage is not valid.");
                }
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Users"; //position in user directory
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".cs";
            dlg.Filter = "Source files (.cs)|*.cs";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BoxFilePath.Text = filename;
            }

        }
    }
}
