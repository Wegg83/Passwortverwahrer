using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows.Input;
using Logik.Pw.Logik.Items;
using System.Windows.Data;
using System.ComponentModel;
using Logik.Pw.Logik.Klassen;

namespace Logik.Wo.Logik.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private PersonCenter _BenutzerListe;
        public ObservableCollection<PwEintrag> MainListe { get; set; }
        public PwEintrag AktEintrag
        {
            get
            {
                var _tmpErg = UiViewListe?.CurrentItem as PwEintrag;
                if (_tmpErg != null)
                {
                    // logik zum einzel anzeigen
                }
                return _tmpErg;
            }
            set
            {
                UiViewListe.MoveCurrentTo(value);
                RaisePropertyChanged();
            }
        }

        //  private ListCollectionView _UiViewListe;
        public ListCollectionView UiViewListe { get; set; }



        public MainViewModel()
        {
            try
            {
                if (Pw.Logik.Properties.Settings.Default.HauptFensterBreite <= 0 || Pw.Logik.Properties.Settings.Default.HauptFensterBreite >= 9000)
                {
                    Pw.Logik.Properties.Settings.Default.HauptFensterBreite = 525;
                }
                if (Pw.Logik.Properties.Settings.Default.HauptFensterHohe <= 0 || Pw.Logik.Properties.Settings.Default.HauptFensterHohe >= 9000)
                {
                    Pw.Logik.Properties.Settings.Default.HauptFensterHohe = 450;
                }
            }
            catch
            {
                Pw.Logik.Properties.Settings.Default.HauptFensterBreite = 525;
                Pw.Logik.Properties.Settings.Default.HauptFensterHohe = 450;
            }
            Pw.Logik.Properties.Settings.Default.Save();

            _BenutzerListe = new PersonCenter();




            MainListe = new ObservableCollection<PwEintrag>();
            initzialise();
        }


        public void initzialise()
        {

            #region ListCollectionView Initailisierung

            UiViewListe = CollectionViewSource.GetDefaultView(MainListe) as ListCollectionView;
            foreach (var item in MainListe) // die vom System rein geladenen Daten müssen das OnPropertyChangeEvent "registrieren"
            {
                item.PropertyChanged += PersonInfosPropertyAnders;
            }
            UiViewListe.CurrentChanged += (s, e) =>
            {
                RaisePropertyChanged(() => AktEintrag);
            };
            MainListe.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged added in e.NewItems)
                    {
                        added.PropertyChanged += PersonInfosPropertyAnders;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged wiederweg in e.OldItems)
                    {
                        wiederweg.PropertyChanged -= PersonInfosPropertyAnders;
                    }
                }
            };
            #endregion

            AktEintrag = UiViewListe?.CurrentItem as PwEintrag;
        }

        private void PersonInfosPropertyAnders(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AktEintrag.HasErrors)) // würde die ganze zeit sonst beim initialisieren anschlagen und den RAM voll laufen
            {
                return;
            }
            if (UiViewListe.IsEditingItem || UiViewListe.IsAddingNew) // nicht wechseln solange jemand darin rum schreibt
            {
                return;
            }
            UiViewListe.Refresh();
        }

    }
}