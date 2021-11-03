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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pw.Ui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool nichtlöschen = false;

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && !nichtlöschen)
            {                      
                ((dynamic)this.DataContext).PWEingabe = ((PasswordBox)sender).SecurePassword;
            }
            nichtlöschen = false;
        }

        private void PasswordBox_DelPW(object sender, RoutedEventArgs e)
        {
            nichtlöschen = true;
            LoginBox.Clear();
        }

    }
}
