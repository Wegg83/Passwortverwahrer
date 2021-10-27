using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace Logik.Pw.Logik.Klassen
{
    public class DockPanelKlasse
    {
        public string Header { get; set; }
        public bool menItemEnable;
        public ObservableCollection<DockPanelKlasse> UnterMenus { get; set; }
        private RelayCommand tastendruck;
        private int index;

        public DockPanelKlasse(RelayCommand TastenEreignis, int index)
        {
            tastendruck = TastenEreignis;
            this.index = index;
        }

        #region Getter
        public int Index { get { return index; } }
        public RelayCommand TastenDruck
        {
            get { return tastendruck; }
        }
        public bool MenItemEnable
        {
            get { return menItemEnable; }
            set { menItemEnable = value; }
        }
        #endregion

    }
}
