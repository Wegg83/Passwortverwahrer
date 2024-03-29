﻿using System;
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

namespace Ui.Pw.Ui
{
    /// <summary>
    /// Interaktionslogik für ImpSyncFenster.xaml
    /// </summary>
    public partial class ImpSyncFenster : Window
    {
        public ImpSyncFenster()
        {
            InitializeComponent();
        }
        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Pw1Eingabe = ((PasswordBox)sender).SecurePassword; }
        }
        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Pw2Eingabe = ((PasswordBox)sender).SecurePassword; }
        }
        private void PasswordBox3_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Pw3Eingabe = ((PasswordBox)sender).SecurePassword; }
        }
    }
}
