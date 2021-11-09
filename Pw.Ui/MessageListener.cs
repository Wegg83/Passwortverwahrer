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
                bool Rndoffen = false;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.ToString() == "Ui.Pw.Ui.RandomCenterFenster")
                    {
                        Rndoffen = true;
                        window.Activate();
                    }
                }
                if (!Rndoffen)
                {
                    RandomCenterFenster Window = new RandomCenterFenster();
                    var vm = Window.DataContext as RandomCenterVM;
                    if (vm != null)
                    {
                        vm.SkinWechsel();
                    }
                    Window.Owner = Parent;
                    Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    Window.Show();
                }
            });

            Messenger.Default.Register<InfoCenterMess>(this, msg =>
            {
                bool Infooffen = false;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.ToString() == "Ui.Pw.Ui.InfoCenterFenster")
                    {
                        Infooffen = true;
                       // window.Activate();
                        window.WindowState = WindowState.Normal;               
                    }
                }
                if (!Infooffen)
                {
                    InfoCenterFenster Window = new InfoCenterFenster();
                    Window.Owner = Parent;
                    Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    var vmInfo = Window.DataContext as InfoCenterVM;
                    vmInfo.SkinWechsel();
                    Window.Show();
                }
               
            });

            Messenger.Default.Register<SendSkinMess>(this, msg =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    string tst = window.ToString();
                    if (window.ToString() == "Ui.Pw.Ui.RandomCenterFenster")
                    {
                        var vmRnd = window.DataContext as RandomCenterVM;
                        vmRnd.SkinWechsel();
                        continue;
                    }
                    if (window.ToString() == "Ui.Pw.Ui.InfoCenterFenster")
                    {
                        var vmInfo = window.DataContext as InfoCenterVM;
                        vmInfo.SkinWechsel();
                    }       
                }
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
