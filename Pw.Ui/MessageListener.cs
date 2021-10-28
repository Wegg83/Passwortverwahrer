using GalaSoft.MvvmLight.Messaging;
using Logik.Pw.Logik.Messengers;
using Logik.Pw.Logik.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ui.Pw.Ui
{
    class MessageListener
    {
        public bool ListenerBool => true;

        public MessageListener()
        {
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
                Window.ShowDialog();
            });
        }
    }
}
