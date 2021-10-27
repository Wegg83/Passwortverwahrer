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
        private RelayCommand _LoginBtn;
        private PersonCenter _BenutzerListe; // alt dieGesamteListe
        private string PasswortEndung = @"\bin.dat";
        private Person AktBenutzer;
        public ObservableCollection<PwEintrag> MainListe { get; set; } // alt PasswortColl
        public ObservableCollection<PwEintrag> GefilterteListe { get; set; } // altKoListe
        public PwEintrag AktEintrag     // alt -> PasswortSelItem
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
        public ObservableCollection<string> AllCbBenutzer { get; set; } // alt PWVBItem
        public string CbBenutzerWahl { get; set; } // alt PWAktBenutzer
        public SecureString PWEingabe { get; set; }
        public RelayCommand LoginBtn => _LoginBtn;

        public Visibility Passwörter { get; set; }
        public Visibility Verwaltung { get; set; }
        public Visibility VisiHinzu { get; set; }
        public Visibility VisiDGPW { get; set; }
        public Visibility VisiHinzu2 { get; set; }
        public Visibility VisiÄndern { get; set; }
        public Visibility VisiRndBtn1 { get; set; }
        public Visibility VisiRndBtn2 { get; set; }



        public bool IsLoginTxtBoxFocused { get; set; }

        private int _tmpIndexNummerMin;

        public string LoginHilfeText { get; set; }
        public string HinzuNeuString { get; set; }
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
            LoginHilfeText = "";
            HinzuNeuString = "Neu";
            _tmpIndexNummerMin = 100001;

            AllCbBenutzer = new ObservableCollection<string>();
            MainListe = new ObservableCollection<PwEintrag>();
            GefilterteListe = new ObservableCollection<PwEintrag>();
            AktEintrag = new PwEintrag();
           // CbBenutzerWahl = "";
            PWEingabe.Clear();
            _LoginBtn = new RelayCommand(Logingedruckt);


            initzialize();
        }


        public void initzialize()
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

        private void Logingedruckt()
        {
            if (CbBenutzerWahl == null || CbBenutzerWahl == "")
            {
                return;
            }

            Person LoginDaten = _BenutzerListe.NameSuchen(CbBenutzerWahl);
            KaudawelschGenerator LoginChecker = new KaudawelschGenerator(PWEingabe);

            if (LoginChecker.PwChecker(LoginDaten.PersiKauda))
            {
                string tmpOrdnerPfad = PfadFindung(LoginDaten.Name);
                MainListe = GefilterteListe = LoginChecker.LadePassworter(LoginDaten.PersiKauda);
              //  GefilterteListe = MainListe;
                AktBenutzer = _BenutzerListe.NameSuchen(CbBenutzerWahl);
               // AktEintrag = null;
                VisiPWNeuAnlegenSetzen();
                VisiDGPW = Visibility.Visible;
                VisiHinzu = Visibility.Visible;
                Passwörter = Visibility.Hidden;
                try
                {
                    System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, "");
                }
                catch
                {
                    System.Windows.MessageBox.Show("Zwischenablage-Fehler. Unmöglich Daten in ZW zu speichern");
                }
            }
            else
            {
                if (PWEingabe.Length == 0)
                {
                    IsLoginTxtBoxFocused = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Falsches Passwort");
                }
            }
        }

        private string PfadFindung(string BenutzerName) => Pw.Logik.Properties.Settings.Default.PfadZielOrdner + _BenutzerListe.OrdnerNameHolen(BenutzerName) + PasswortEndung;

        private void VisiPWNeuAnlegenSetzen()
        {
            VisiHinzu2 = Visibility.Visible;
            VisiÄndern = Visibility.Hidden;
            VisiRndBtn1 = Visibility.Visible;
            VisiRndBtn2 = Visibility.Hidden;
            HinzuNeuString = "Neu";
        }
    }
}