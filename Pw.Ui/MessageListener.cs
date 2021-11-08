using GalaSoft.MvvmLight.Messaging;
using Logik.Pw.Logik.Messengers;
using Logik.Pw.Logik.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ui.Pw.Ui
{
    class MessageListener
    {
        public bool ListenerBool => true;
        private Window Parent;

        public MessageListener()
        {
            Parent = SetzeParent();
            InitMessenger();
        }

        private void InitMessenger()
        {
            Messenger.Default.Register<SendImportMess>(this, msg =>
            {
                ImpSyncFenster Window = new ImpSyncFenster();
                var vm = Window.DataContext as ImportSyncVM;
                if (vm != null)
                {
                    vm.Initialisiere(msg);
                }
                Window.Owner = Parent;
                Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                Window.ShowDialog();
            });

            Messenger.Default.Register<FensterCloseMess>(this, msg =>
            {
                
                foreach (Window window in Application.Current.Windows)
                {
                    string tst = window.ToString();
                    if (window.ToString() == msg.Fenstername)
                    {
                        window.Close();                    
                    }
                }
            });

            Messenger.Default.Register<SendRndCenterMess>(this, msg => 
            {
                RandomCenterFenster Window = new RandomCenterFenster();
                var vm = Window.DataContext as RandomCenterVM;
                if(vm != null)
                {
                    vm.Inizialise();
                }
                Window.Owner = Parent;
                Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                Window.ShowDialog();
            });

            Messenger.Default.Register<InfoCenterMess>(this, msg =>
            {
                InfoCenterFenster Window = new InfoCenterFenster();
                var vm = Window.DataContext as InfoCenterVM;
                if (vm != null)
                {
                    vm.SkinWechsel();
                }
                Window.Owner = Parent;
                Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                Window.ShowDialog();
            });
        }

        private Window SetzeParent()
        {
            foreach (Window tmp in Application.Current.Windows)
            {
                string tst = tmp.ToString();
                if (tmp.ToString() == "Ui.Pw.Ui.MainWindow")
                {
                    return tmp;
                }
            }
            return null;
        }
    }
}
