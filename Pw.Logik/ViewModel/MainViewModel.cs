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
using Logik.Pw.Logik.Messengers;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using static Logik.Pw.Logik.ViewModel.ImportSyncVM;

namespace Logik.Pw.Logik.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        enum DerzeitgeAnsicht { Verwaltung, Benutzer }
        enum BearbeitStatus { Neuanlage, Anderung}
        public enum SkinWahl { winsylte, Darkstyle }

        private BearbeitStatus _auswahlstatus = BearbeitStatus.Neuanlage;
        public static SkinWahl AktuellerSkin;
        public const string PasswortEndung = @"\bin.dat";

       // private bool nichtGespeichertAnders, nichtGespeichertNeu; // achtung keine integration mit NUR get;set;
        public bool ZwischenAblageAktivBool { get; set; }
        private RelayCommand _LoginBtn, _VerwaltungBtn, _RootOrdnerBtn, _ExportBtn, _ImportBtn, _WinstyleXamlBtn, _DarkstyleXamlBtn, _InfoBtn, _PrgEndeBtn, _LogOutBtn, _RndVerwaltBtn, _AnsichtWechselBtn;
        private RelayCommand _PWDelBtn, _AktBenutzerInZABtn, _AktPwInZABtn, _BenutzHinzuBtn, _BenutzAndersBtn; // , _PWAndersBtn _PWHinzuBtn
        private RelayCommand _PwUbernahmeBtn, _PwRndBtn;
        private PersonCenter _BenutzerListe; // alt dieGesamteListe
        private Person aktBenutzer;
        private Person AktBenutzer
        {
            get { return aktBenutzer; }
            set
            {
                aktBenutzer = value; 
                if(value == null)
                {
                    Passwörter = Visibility.Visible;
                    VisiDGPW = Visibility.Hidden;
                }
                else
                {
                    Passwörter = Visibility.Hidden;
                    VisiDGPW = Visibility.Visible;
                }
            }
        }

        public ObservableCollection<PwEintrag> MainListe { get; set; } // alt PasswortColl
        public ObservableCollection<PwEintrag> GefilterteListe { get; set; } // altKoListe
        public PwEintrag DetailAnzeigeEintrag { get; set; }
       private PwEintrag _AktEintrag;
        public PwEintrag AktEintrag     // alt -> PasswortSelItem
        {
            //get
            //{
            //    var _tmpErg = UiViewListe?.CurrentItem as PwEintrag;
            //    if (_tmpErg != null)
            //    {
            //        // logik zum einzel anzeigen
            //    }
            //    return _tmpErg;
            //}
            //set
            //{
            //    UiViewListe.MoveCurrentTo(value);
            //    RaisePropertyChanged();
            //}
            get { return _AktEintrag; }
            set
            {
                _AktEintrag = value;
                RaisePropertyChanged();
                AbfrageNichtGespeichertesPW();

                if(value != null)
                {
                    DetailAnzeigeEintrag = new PwEintrag(value);
                    ZwischenAblageAktivBool = true;
                    HinzuNeuString = "Neu";
                    Auswahlstatus = BearbeitStatus.Anderung;
                }
            }
        }


        public ObservableCollection<string> VerwaltungsListe { get; set; } // alt PWCBItem
        private string _VerwaltListItem;
        public string VerwaltListItem { get { return _VerwaltListItem; } set { VerwaltListItem = value; RaisePropertyChanged(); if (value != null) VisiBenutzGew = Visibility.Visible; else VisiBenutzGew = Visibility.Hidden;  } }
        private string _CbBenutzerWahl;
        public string CbBenutzerWahl { get { return _CbBenutzerWahl; } set { _CbBenutzerWahl = value; RaisePropertyChanged(); if (AktBenutzer != null) LogoutGedruckt(); Logingedruckt(); } } // alt PWAktBenutzer
        private string _PWSuche;
        public string PWSuche { get { return _PWSuche; } set { _PWSuche = value; RaisePropertyChanged(); GefilterteListe = PWListeFiltern(); if (GefilterteListe.Count == 0) Auswahlstatus = BearbeitStatus.Neuanlage; } } 
        public string BeschreibungMenu1 { get; set; }
        public SecureString PWEingabe { get; set; }
        public RelayCommand LoginBtn => _LoginBtn;
        public RelayCommand VerwaltungBtn => _VerwaltungBtn;
        public RelayCommand RootOrdnerBtn => _RootOrdnerBtn;
        public RelayCommand ExportBtn => _ExportBtn;
        public RelayCommand ImportBtn => _ImportBtn;
        public RelayCommand InfoBtn => _InfoBtn;
        public RelayCommand PrgEndeBtn => _PrgEndeBtn;
        public RelayCommand LogOutBtn => _LogOutBtn;
        public RelayCommand RndVerwaltBtn => _RndVerwaltBtn;
        public RelayCommand AnsichtWechselBtn => _AnsichtWechselBtn;
        public RelayCommand PwUbernahmeBtn => _PwUbernahmeBtn;
        public RelayCommand PwNeuAnzBtn => new RelayCommand(() => { Auswahlstatus = BearbeitStatus.Neuanlage; });
        //public RelayCommand PWHinzuBtn => _PWHinzuBtn;
        //public RelayCommand PWAndersBtn => _PWAndersBtn;
        //public RelayCommand PWZwischenBtn => _PWZwischenBtn;
        //public RelayCommand BenutzerZwischenBtn => _BenutzerZwischenBtn;
        public RelayCommand PWDelBtn => _PWDelBtn;
        public RelayCommand PwRndBtn => _PwRndBtn;
        public RelayCommand AktBenutzerInZABtn => _AktBenutzerInZABtn;
        public RelayCommand AktPwInZABtn => _AktPwInZABtn;
        public RelayCommand DarkstyleXamlBtn => _DarkstyleXamlBtn;
        public RelayCommand WinstyleXamlBtn => _WinstyleXamlBtn;
        public RelayCommand BenutzHinzuBtn => _BenutzHinzuBtn;
        public RelayCommand BenutzAndersBtn => _BenutzAndersBtn;

        public System.Windows.Controls.ToolTip ZwischenlageTooltip { get; set; }

        //public ObservableCollection<DockPanelKlasse> MeinOberMenu;
        public Visibility Passwörter { get; set; }
        public Visibility Verwaltung { get; set; }
        //public Visibility VisiHinzu { get; set; }
        public Visibility VisiDGPW { get; set; }
        //public Visibility VisiNeuOnly { get; set; }
        //public Visibility VisiÄndern { get; set; }
        //public Visibility VisiRndBtn1 { get; set; }
        //public Visibility VisiRndBtn2 { get; set; }
        //public Visibility VisiLöschen { get; set; }
        public Visibility VisiBenutzerCB { get; set; }
        public Visibility VerwaltungAnders { get; set; }
        public Visibility VisiAndersOnly { get; set; }
        public Visibility VisiBenutzGew { get; set; }
        public bool IsNeOnly { get; set; }

        public string PWNeuProgramm { get; set; }
        public string PWNeuAdresse { get; set; }
        public string PWNeuPW { get; set; }

        public bool BenutzerAktivBool { get; set; }
        public string BenutzerRootOrdnerString { get; set; }
        public string MeinHintergrund { get; set; }
        public string MeineSchriftFarbe1 { get; set; }
        public string MeineSchriftFarbe2 { get; set; }
        public string MeineKontrastFarbe1 { get; set; }
        public string MeineKontrastFarbe2 { get; set; }
        public string MeineSchriftArtNorm { get; set; }
        public string MeineSchriftArtFett { get; set; }
        public int MeineSchriftGrosseNorm { get; set; }
        public int MeineSchriftGrosseKlein { get; set; }
        public int MeineSchriftGrosseGross { get; set; }
        public string MeinIconRauf { get; set; }
        public string MeinIconRunter { get; set; }
        public string MeinSyncIcon { get; set; }
        public string MeinRndIcon { get; set; }
        public string OptionenUberschrift { get; set; }
        public string BeschreibungRndVerwalt { get; set; }
        public string BeschreibungMenuDateiExport { get; set; }
        public string BeschreibungMenuDateiImport { get; set; }
        public string ErrorMeldungString { get; set; }

        private int _tmpIndexNummerMin;

        public string HinzuNeuString { get; set; }
        //  private ListCollectionView _UiViewListe;
        public ListCollectionView UiViewListe { get; set; }

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                VisiDGPW = Visibility.Hidden;
                Verwaltung = Visibility.Visible;
                return;
            }



            Messenger.Default.Register<EmpfCenterMess>(this, empfang =>
            {
                EmpfangeNeuesCenterSync(empfang);
            });


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

            

            MainListe = new ObservableCollection<PwEintrag>();
            GefilterteListe = new ObservableCollection<PwEintrag>();
            BenutzerAktivBool = false;

            initzialize();

            HinzuNeuString = "Neu";
            _tmpIndexNummerMin = 100001;
            DetailAnzeigeEintrag = new PwEintrag();

            VerwaltungsListe = _BenutzerListe.VerwaltungListe();

            //   AktEintrag = new PwEintrag(); test
            // CbBenutzerWahl = ""; test
            PWEingabe = new SecureString();
            PWEingabe.Clear();
            _LoginBtn = new RelayCommand(Logingedruckt);
            _VerwaltungBtn = new RelayCommand(Verwaltunggedruckt);
            _PrgEndeBtn = new RelayCommand(Programmschließengedruckt);
            _BenutzHinzuBtn = new RelayCommand(sendeNeuerBenutzer);
         //   _PWHinzuBtn = new RelayCommand(PWHinzuGedruckt);
            _PwUbernahmeBtn = new RelayCommand(PWÄndernGedruckt);
            _LogOutBtn = new RelayCommand(LogoutGedruckt);
            _ImportBtn = new RelayCommand(ImportGedruckt);
            _PwRndBtn = new RelayCommand(() => { NeuesRndPWgedruckt(); });
            _AktBenutzerInZABtn = new RelayCommand(() => { ZwischenAgedruckt(false); });
            _AktPwInZABtn = new RelayCommand(() => { ZwischenAgedruckt(true); });
            _PWDelBtn = new RelayCommand(PWLöschenGedruckt);
            _RootOrdnerBtn = new RelayCommand(RootOrdnerGedruckt);
            //versionsInfos = new RelayCommand(InfoCenterAnzeigenGedruckt);
            _ExportBtn = new RelayCommand(ExportGedruckt);
            //_PWZwischenBtn = new RelayCommand(AktPwZwischenAgedruckt);
            //_BenutzerZwischenBtn = new RelayCommand(AktBenZwischenAgedruckt);
            //syncBtn = new RelayCommand(SyncenGedruckt);
            //neuesRAndomPWBtn = new RelayCommand(NeuesRndPWgedruckt);
            //pWRandVerwaltBtn = new RelayCommand(ZufallsKonfiguratorGedruckt);
            _BenutzAndersBtn = new RelayCommand(BenutzerAndersGedruckt);

            if (Properties.Settings.Default.AktuellerSkin == 0) // grundsätzlich keine schöne Lösung
            {
                AktuellerSkin = SkinWahl.winsylte;
            }
            else
            {
                AktuellerSkin = SkinWahl.Darkstyle;
            }
            SkinAnderung();
        }


        private void initzialize()
        {
            initzializeMenu();
            initzializeBenutzer();         
            initzializeIListColl();
        }

        private void initzializeBenutzer()
        {
            string tmpPfad;

            ZwischenAblageAktivBool = false;
            try
            {
                System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, "");
            }
            catch
            {
                System.Windows.MessageBox.Show("Zwischenablage-Fehler. Unmöglich Daten in ZW zu speichern");
            }

            _BenutzerListe = new PersonCenter();
            VerwaltungsListe = new ObservableCollection<string>();
            Person GelesenerBenutzer = new Person();
            PwEintrag GeleseneHauptView = new PwEintrag();
            KaudawelschGenerator ListEntschlüssler = new KaudawelschGenerator(new SecureString());

            //nichtGespeichertNeu = false;
            //nichtGespeichertAnders = false;

            try
            {
                BenutzerRootOrdnerString = Properties.Settings.Default.PfadZielOrdner;
                List<string> AlleOrdner = new List<string>(Directory.EnumerateDirectories(Properties.Settings.Default.PfadZielOrdner));
                foreach (string Ordna in AlleOrdner)
                {
                    tmpPfad = Ordna + PasswortEndung;

                    if (!File.Exists(tmpPfad))
                    {
                        continue;
                    }

                    try
                    {
                        GelesenerBenutzer = LeseDoppeltVerschlüsselteDatei(tmpPfad, ListEntschlüssler, Ordna);
                        if (GelesenerBenutzer == null || (GelesenerBenutzer.Name == null && GelesenerBenutzer.AktOrdnerName == null))
                        {
                            continue;
                        }
                        VerwaltungsListe.Add(GelesenerBenutzer.Name);
                        _BenutzerListe.Hinzufügen(GelesenerBenutzer);
                    }
                    catch
                    {

                    }
                }
            }
            catch
            {

            }
            if (_BenutzerListe.Durchzählen == 0)
            {
                Ansichtwechsel(DerzeitgeAnsicht.Verwaltung);
            }
            else
            {
                Ansichtwechsel(DerzeitgeAnsicht.Benutzer);
                CbBenutzerWahl = VerwaltungsListe[0];            
            }
        }

        private void initzializeIListColl()
        {
            #region ListCollectionView Initailisierung

            UiViewListe = CollectionViewSource.GetDefaultView(GefilterteListe) as ListCollectionView;
            foreach (var item in GefilterteListe) // die vom System rein geladenen Daten müssen das OnPropertyChangeEvent "registrieren"
            {
                item.PropertyChanged += PersonInfosPropertyAnders;
            }
            UiViewListe.CurrentChanged += (s, e) =>
            {
                RaisePropertyChanged(() => AktEintrag);
            };
            GefilterteListe.CollectionChanged += (s, e) =>
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


        //private bool MenuAnderung(ObservableCollection<DockPanelKlasse> MeinMenu, int Index, string neuerHead, bool NeuerZustand)
        //{
        //    foreach (DockPanelKlasse AktKnoten in MeinMenu)
        //    {
        //        if (AktKnoten.Index == Index)
        //        {
        //            AktKnoten.Header = neuerHead;
        //            AktKnoten.MenItemEnable = NeuerZustand;
        //            return true;
        //        }
        //        if (AktKnoten.SubItems != null)
        //        {
        //            if (MenuAnderung(AktKnoten.SubItems, Index, neuerHead, NeuerZustand))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        private void initzializeMenu()
        {
            BeschreibungRndVerwalt = "Zufall-Generator";
            BeschreibungMenuDateiExport = "Benutzer exportieren";
            BeschreibungMenuDateiImport = "Benutzer importieren";

        //#region Menu als Klasse 
        //MeinOberMenu = new ObservableCollection<DockPanelKlasse>();
        //    DockPanelKlasse tmpOberItems = new DockPanelKlasse(null, 1000);
        //    ObservableCollection<DockPanelKlasse> tmp1Untermenu;
        //    DockPanelKlasse tmpItems1Unter;
        //    ObservableCollection<DockPanelKlasse> tmp2Untermenu;
        //    DockPanelKlasse tmpItems2Unter;

        //    #region Datei Menu
        //    tmpOberItems.Header = "Datei";
        //    tmpOberItems.MenItemEnable = true;
        //    #region Untermenu Datei
        //    tmp1Untermenu = new ObservableCollection<DockPanelKlasse>();

        //    tmpItems1Unter = new DockPanelKlasse(AnsichtWechselBtn, 1100);
        //    tmpItems1Unter.Header = "Benutzerverwaltung";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpItems1Unter = new DockPanelKlasse(RndVerwaltBtn, 1200);
        //    tmpItems1Unter.Header = "Zufalls Konfigurator";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpItems1Unter = new DockPanelKlasse(_ExportBtn, 1300);
        //    tmpItems1Unter.Header = "Benutzer Exportieren";
        //    tmpItems1Unter.MenItemEnable = false;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpItems1Unter = new DockPanelKlasse(_ImportBtn, 1400);
        //    tmpItems1Unter.Header = "Benutzer Importieren";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpItems1Unter = new DockPanelKlasse(LogOutBtn, 1500);
        //    tmpItems1Unter.Header = "Benutzer abmelden";
        //    tmpItems1Unter.MenItemEnable = false;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpItems1Unter = new DockPanelKlasse(PrgEndeBtn, 1600);
        //    tmpItems1Unter.Header = "Schließen";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpOberItems.SubItems = tmp1Untermenu;
        //    #endregion
        //    MeinOberMenu.Add(tmpOberItems);
        //    #endregion

        //    #region Edit Menu
        //    tmpOberItems = new DockPanelKlasse(null, 2000);
        //    tmpOberItems.Header = "Edit";
        //    tmpOberItems.MenItemEnable = true;
        //    #region Untermneu Edit
        //    tmp1Untermenu = new ObservableCollection<DockPanelKlasse>();

        //    //tmpItems1Unter = new DockPanelKlasse(ZwischenAblageBtn, 2100);
        //    //tmpItems1Unter.Header = "In Zwischenablage";
        //    //tmpItems1Unter.MenItemEnable = false;       

        //    tmpItems1Unter = new DockPanelKlasse(null, 2100);
        //    tmpItems1Unter.Header = "Skin-Aussehen";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);
        //    #region Untermenu Skins

        //    tmp2Untermenu = new ObservableCollection<DockPanelKlasse>();

        //    tmpItems2Unter = new DockPanelKlasse(new RelayCommand(ThemeWinStyleGedruckt), 2210);
        //    tmpItems2Unter.Header = "Windows-Style";
        //    tmpItems2Unter.MenItemEnable = true;
        //    tmp2Untermenu.Add(tmpItems2Unter);
        //    tmpItems2Unter = new DockPanelKlasse(new RelayCommand(ThemeDarkStyleGedruckt), 2220);
        //    tmpItems2Unter.Header = "Dark-Style";
        //    tmpItems2Unter.MenItemEnable = true;
        //    tmp2Untermenu.Add(tmpItems2Unter);

        //    tmpItems1Unter.SubItems = tmp2Untermenu;
        //    #endregion

        //    tmp1Untermenu.Add(tmpItems1Unter);
        //    tmpOberItems.SubItems = tmp1Untermenu;

        //    #endregion
        //    MeinOberMenu.Add(tmpOberItems);
        //    #endregion

        //    #region Hilfe Menu
        //    tmpOberItems = new DockPanelKlasse(null, 3000);
        //    tmpOberItems.Header = "Hilfe";
        //    tmpOberItems.MenItemEnable = true;

        //    #region Unermenu Hilfe
        //    tmp1Untermenu = new ObservableCollection<DockPanelKlasse>();

        //    tmpItems1Unter = new DockPanelKlasse(InfoBtn, 3100);
        //    tmpItems1Unter.Header = "Info";
        //    tmpItems1Unter.MenItemEnable = true;
        //    tmp1Untermenu.Add(tmpItems1Unter);

        //    tmpOberItems.SubItems = tmp1Untermenu;
        //    #endregion
        //    MeinOberMenu.Add(tmpOberItems);

        //    #endregion

            //#endregion

            #region Menu direkt im Xaml
            _WinstyleXamlBtn = new RelayCommand(ThemeWinStyleGedruckt);
            _DarkstyleXamlBtn = new RelayCommand(ThemeDarkStyleGedruckt);
            #endregion
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
            if (LoginDaten.AktOrdnerName == null) return;
            KaudawelschGenerator LoginChecker = new KaudawelschGenerator(PWEingabe);

            if (LoginChecker.PwChecker(LoginDaten.PersiKauda))
            {
                ErrorMeldungString = "";
                string tmpOrdnerPfad = PfadFindung(LoginDaten.Name);
                MainListe = GefilterteListe = LoginChecker.LadePassworter(LoginDaten.PersiKauda);
                AktBenutzer = _BenutzerListe.NameSuchen(CbBenutzerWahl);
                BenutzerAktivBool = true; 
                Auswahlstatus = BearbeitStatus.Neuanlage;
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
                if (PWEingabe != null && PWEingabe.Length != 0)
                    ErrorMeldungString = "Falsches Passwort";
                if (PWEingabe != null) PWEingabe.Clear();
            }
        }


        public void ExportGedruckt()
        {
            Microsoft.Win32.SaveFileDialog WoSpeichern = new Microsoft.Win32.SaveFileDialog();
            WoSpeichern.InitialDirectory = Properties.Settings.Default.PfadImportOrdner;
            WoSpeichern.Filter = "Export Datei (*.exp)|*.exp";
            WoSpeichern.RestoreDirectory = true;
            Nullable<bool> result = WoSpeichern.ShowDialog();
            if (result == true)
            {
                _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, WoSpeichern.FileName);
            }
            System.Windows.MessageBox.Show("Export erfolgreich.");
        }

        private void Verwaltunggedruckt()
        {
            AbfrageNichtGespeichertesPW();
           // VerwaltungColl = new ObservableCollection<BenutzerView>(); alt

            if (Verwaltung == Visibility.Visible)
            {
                Ansichtwechsel(DerzeitgeAnsicht.Benutzer);
                /*VerwaltungAnders = Visibility.Hidden;*/ // achutng wil ich weg und 2 Btns machen!
            }
            else
            {
                if (AktBenutzer != null)
                {
                    LogoutGedruckt();
                }
                Ansichtwechsel(DerzeitgeAnsicht.Verwaltung);            
            }
        }

        public void LogoutGedruckt()
        {
            AbfrageNichtGespeichertesPW();
            GefilterteListe= null;
            MainListe = null;
            _tmpIndexNummerMin = 100001;
            AktBenutzer = null;
            BenutzerAktivBool = false;
            ZwischenAblageAktivBool = false;
            try
            {
                System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, "");
            }
            catch
            {
                System.Windows.MessageBox.Show("Zwischenablage-Fehler. Unmöglich Daten in ZW zu speichern");
            }
            PWEingabe.Clear();
        }

        private string PfadFindung(string BenutzerName) => Pw.Logik.Properties.Settings.Default.PfadZielOrdner + _BenutzerListe.OrdnerNameHolen(BenutzerName) + PasswortEndung;

        //private void VisiPWNeuAnlegenSetzen()
        //{
        //    VisiNeuOnly = Visibility.Visible;
        //    VisiÄndern = Visibility.Hidden;
        //    VisiRndBtn1 = Visibility.Visible;
        //    VisiRndBtn2 = Visibility.Hidden;
        //    HinzuNeuString = "Neu";
        //}


        private void AbfrageNichtGespeichertesPW()
        {
            if (DetailAnzeigeEintrag != null && !DetailAnzeigeEintrag.Aktuell)
            {
                if (System.Windows.MessageBox.Show("Änderungen nicht gespeichert. Übernehmen?", "Fehler", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                        PWÄndernGedruckt();
                }
                else
                {
                    //nichtGespeichertNeu = false;
                    //nichtGespeichertAnders = false;
                    DetailAnzeigeEintrag.Aktuell = true;
                }
            }
        }

        //private void PWÄndernGedruckt()
        //{
        //    if(_auswahlstatus == BearbeitStatus.Anderung)
        //    {
        //        PwEintrag tmpV = new PwEintrag();
        //        tmpV.Programm = AktEintrag.Programm;
        //        tmpV.Benutzer = AktEintrag.Benutzer;
        //        tmpV.Passwort = AktEintrag.Passwort;
        //        tmpV.Datum = DateTime.Today;
        //        tmpV.tmprndIndex = AktEintrag.tmprndIndex;
        //        int tmpIndex = IndexSuchen(tmpV.Programm, tmpV.tmprndIndex);
        //        _BenutzerListe.EintragCh(AktBenutzer.Name, tmpV, PWEingabe, tmpIndex);
        //        MainListe.RemoveAt(tmpIndex);
        //        MainListe.Insert(tmpIndex, tmpV);
        //        _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));
        //        if (PWSuche != null && PWSuche != "")
        //        {
        //            GefilterteListe = PWListeFiltern();
        //        }
        //        else
        //        {
        //            GefilterteListe = MainListe;
        //        }          
        //        nichtGespeichertAnders = false;
        //    }
        //    else
        //    {
        //        if (!TestaufLeerenInhalt(PWNeuProgramm))
        //        {
        //            System.Windows.MessageBox.Show("Bitte einen Programmnamen angeben");
        //        }
        //        else
        //        {
        //            PwEintrag PWeinfügenView = new PwEintrag();
        //            PWeinfügenView.Programm = PWNeuProgramm;
        //            PWeinfügenView.Benutzer = PWNeuAdresse;
        //            PWeinfügenView.Passwort = PWNeuPW;
        //            PWeinfügenView.Datum = System.DateTime.Today;
        //            PWeinfügenView.tmprndIndex = "n" + _tmpIndexNummerMin.ToString();
        //            _tmpIndexNummerMin++;
        //            if (ProgrammVorhanden(PWNeuProgramm))
        //            {
        //                if (ProgrammUndBenutzerVorhanden(PWeinfügenView))
        //                {
        //                    System.Windows.MessageBox.Show("Programm schon mit diesem Benutzernamen vorhanden.");
        //                }
        //                else
        //                {
        //                    if (System.Windows.MessageBox.Show("Programm schon mit einem anderem Benutzernamen vorhanden. Trotzdem anlegen?", "Snychonisieren", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //                    {
        //                        nichtGespeichertNeu = false;
        //                        _BenutzerListe.NormEintragVersHinzu(PWeinfügenView, PWEingabe, AktBenutzer.Name, false);
        //                        _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));
        //                        PWNeuAdresse = "";
        //                        PWNeuPW = "";
        //                        nichtGespeichertNeu = false;
        //                        PWNeuProgramm = "";
        //                        // IsPrgTxtBoxFocused = true; geht derzeit nicht
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                nichtGespeichertNeu = false;
        //                if (MainListe.Count == 0)
        //                {
        //                    _BenutzerListe.NormEintragVersHinzu(PWeinfügenView, PWEingabe, AktBenutzer.Name, true);
        //                }
        //                else
        //                {
        //                    _BenutzerListe.NormEintragVersHinzu(PWeinfügenView, PWEingabe, AktBenutzer.Name, false);
        //                }
        //                MainListe.Add(PWeinfügenView);                       
        //                _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));
        //                PWNeuAdresse = "";
        //                PWNeuPW = "";
        //                PWNeuProgramm = "";
        //                GefilterteListe = PWListeFiltern();
        //                nichtGespeichertNeu = false;
        //                //  IsPrgTxtBoxFocused = true; geht derzeit nicht
        //            }
        //        }
        //    }
        //}


        private void PWÄndernGedruckt()
        {
            if (AktEintrag == null) return;

            PwEintrag tmpV = new PwEintrag();
            tmpV.Programm = DetailAnzeigeEintrag.Programm;
            tmpV.Benutzer = DetailAnzeigeEintrag.Benutzer;
            tmpV.Passwort = DetailAnzeigeEintrag.Passwort;
            tmpV.Datum = DateTime.Today;

            switch (Auswahlstatus)
            {
                case BearbeitStatus.Anderung:
                    tmpV.tmprndIndex = DetailAnzeigeEintrag.tmprndIndex;
                    int tmpIndex = IndexSuchen(tmpV.Programm, tmpV.tmprndIndex);
                    _BenutzerListe.EintragCh(AktBenutzer.Name, tmpV, PWEingabe, tmpIndex);
                    MainListe.RemoveAt(tmpIndex);
                    MainListe.Insert(tmpIndex, tmpV);
                    _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));                  
                    break;
                case BearbeitStatus.Neuanlage:
                    if (!TestaufLeerenInhalt(tmpV.Programm))
                    {
                        System.Windows.MessageBox.Show("Bitte einen Programmnamen angeben");
                        return;
                    }
                    tmpV.tmprndIndex = "n" + _tmpIndexNummerMin.ToString();
                    _tmpIndexNummerMin++;
                    if (ProgrammVorhanden(tmpV.Programm))
                    {
                        if (ProgrammUndBenutzerVorhanden(tmpV))
                        {
                            System.Windows.MessageBox.Show("Programm schon mit diesem Benutzernamen vorhanden.");
                            return;
                        }

                        if (System.Windows.MessageBox.Show("Programm schon mit einem anderem Benutzernamen vorhanden. Trotzdem anlegen?", "Snychonisieren", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            _BenutzerListe.NormEintragVersHinzu(tmpV, PWEingabe, AktBenutzer.Name, false);
                        }
                    }
                    else
                    {
                        if (MainListe.Count == 0)
                        {
                            _BenutzerListe.NormEintragVersHinzu(tmpV, PWEingabe, AktBenutzer.Name, true);
                        }
                        else
                        {
                            _BenutzerListe.NormEintragVersHinzu(tmpV, PWEingabe, AktBenutzer.Name, false);
                        }
                        MainListe.Add(tmpV);
                    }
                    _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));               
                    break;
            }

            GefilterteListe = PWListeFiltern();
            DetailAnzeigeEintrag.Aktuell = true;
            AktEintrag = null;
            AktEintrag = GefilterteListe[0];
        }

        private void BenutzerAndersGedruckt()
        {
            if (VerwaltListItem != null)
            {
                MessengerInstance.Send(new SendImportMess(ImpMoglichkeit.PwAndern, _BenutzerListe.NameSuchen(VerwaltListItem), _BenutzerListe));
            }
        }

        private void sendeNeuerBenutzer()
        {
            // MessengerInstance.Send(new SendNeuBenutzerMess(SendNeuBenutzerMess.Zustand.Neueingabe, _BenutzerListe.AlleBenutzerAlsListe()));
            MessengerInstance.Send(new SendImportMess(ImpMoglichkeit.NeuAnlage, null, _BenutzerListe));
        }

        //public void EmpfangeNeuerBenutzer(NeuBenutzerMess NeuAnlage)
        //{
        //    // auch für import --> abchecken ob wir den Benutzer hinzufügen müssen.. dann auch liste eintragen wenn vorhanden
        //    PWEingabe.Clear();
        //    if (_BenutzerListe.NameSuchen(NeuAnlage.NeuePersondaten.Name).PersiKauda == null)
        //    {
        //        NeuAnlage.NeuePersondaten.AktOrdnerName = NeuenBenutzerOrdnerAnlegen();
        //        _BenutzerListe.Hinzufügen(NeuAnlage.NeuePersondaten);
        //        if(NeuAnlage.NeuePersondaten.PersiKauda == null)
        //        {
        //            _BenutzerListe.ErstEintrag(NeuAnlage.NeuePersondaten.Name, NeuAnlage.PwEintrag);
        //        }          
        //        _BenutzerListe.KomplettVerschlüsseln(NeuAnlage.NeuePersondaten.Name, PfadFindung(NeuAnlage.NeuePersondaten.Name));
        //    }
        //    else
        //    {
        //        // syncvorgang wurde durchgeführt. update die Kaudaliste
        //        _BenutzerListe.NameSuchen(NeuAnlage.NeuePersondaten.Name).PersiKauda = NeuAnlage.NeuePersondaten.PersiKauda;
        //    }

        // //   VerwaltungsListe.Add(NeuAnlage.NeuePersondaten.Name);
        //    VerwaltungsAnzeigeNeuLaden();
        //    Ansichtwechsel(DerzeitgeAnsicht.Benutzer);
        //}

        private Person LeseDoppeltVerschlüsselteDatei(string DateiPfad, KaudawelschGenerator DerKaudaGenerator, string Ordner)
        {
            Person BenutzAusDat = new Person();
            try
            {
                string AktZeile = "";
                StreamReader Leser1 = new StreamReader(DateiPfad);
                BenutzAusDat.PersiKauda = new List<string>();
                bool Name = false;
                while (Leser1.Peek() >= 0)
                {
                    AktZeile = Leser1.ReadLine();
                    byte[] KaudawelschZeile2 = new byte[48];

                    int AnzahlBytes2 = AktZeile.Split(';').Length - 1;
                    if (AnzahlBytes2 > 0)
                    {
                        KaudawelschZeile2 = new byte[AnzahlBytes2];
                        for (int i = 0; i < AnzahlBytes2; i++)
                        {
                            KaudawelschZeile2[i] = Convert.ToByte(AktZeile.Split(';')[i]);
                        }
                    }
                    else
                    {
                        return BenutzAusDat;
                    }
                    AktZeile = DerKaudaGenerator.Entschlüsselung(KaudawelschZeile2);
                    if (!Name)
                    {
                        BenutzAusDat.Name = DerKaudaGenerator.StrichpunktEntChecker(AktZeile, 0, 1);
                        BenutzAusDat.AktOrdnerName = new DirectoryInfo(Ordner).Name;
                        Name = true;
                    }
                    else
                    {
                        BenutzAusDat.PersiKauda.Add(AktZeile);
                    }
                }
                return BenutzAusDat;
            }
            catch
            {
                System.Windows.MessageBox.Show("Die verschlüsselte Datei ist beschädigt oder manipuliert und kann nicht eingelesen werden.");
                return null;
            }
        }


        private void VerwaltungsAnzeigeNeuLaden()
        {
            VerwaltungsListe = _BenutzerListe.VerwaltungListe();
            PWNeuAdresse = "";
            PWNeuPW = "";
            //nichtGespeichertAnders = false;
            //nichtGespeichertNeu = false;
        }

        private void Ansichtwechsel(DerzeitgeAnsicht neuerWert)
        {
            VerwaltListItem = null;

            switch (neuerWert)
            {
                case DerzeitgeAnsicht.Benutzer:
                    Passwörter = Visibility.Visible;
                    Verwaltung = Visibility.Hidden;
                    VisiDGPW = Visibility.Hidden;
                    //VisiNeuOnly = Visibility.Hidden;
                    //VisiLöschen = Visibility.Hidden;
                    VisiBenutzerCB = Visibility.Visible;
                    PWNeuAdresse = "";
                    PWNeuPW = "";
                    //nichtGespeichertNeu = false;
                    PWNeuProgramm = "";
                    //nichtGespeichertAnders = false;
                    //MenuAnderung(MeinOberMenu, 1100, "Benutzerverwaltung", true);
                    BeschreibungMenu1 = "Benutzerverwaltung";
                    Logingedruckt();
                    break;
                case DerzeitgeAnsicht.Verwaltung:
                        VisiBenutzerCB = Visibility.Hidden;
                        Passwörter = Visibility.Hidden;
                        Verwaltung = Visibility.Visible;
                        //VisiNeuOnly = Visibility.Hidden;
                        //VisiLöschen = Visibility.Hidden;
                        VisiDGPW = Visibility.Hidden;
                        PWNeuAdresse = "";
                        PWNeuPW = "";
                        //nichtGespeichertNeu = false;
                        PWNeuProgramm = "";
                        //nichtGespeichertAnders = false;
                    BeschreibungMenu1 = "Passwortverwaltung";
                    //MenuAnderung(MeinOberMenu, 1100, "Passwortverwaltung", true);
                        ZwischenAblageAktivBool = false;
                        try
                        {
                            System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, "");
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show("Zwischenablage-Fehler. Unmöglich Daten in ZW zu speichern");
                        }
                    VerwaltungsAnzeigeNeuLaden();
                    break;
            }
        }

        private BearbeitStatus Auswahlstatus
        {
            get { return _auswahlstatus; }
            set
            {
                _auswahlstatus = value;
                switch (value)
                {
                    case BearbeitStatus.Neuanlage:
                        IsNeOnly = true;
                        VisiAndersOnly = Visibility.Hidden;
                        DetailAnzeigeEintrag = new PwEintrag();
                        break;
                    case BearbeitStatus.Anderung:
                        IsNeOnly = false;
                        VisiAndersOnly = Visibility.Visible;
                        break;
                }
            }
        }

        private void NeuesRndPWgedruckt()
        {
            string NeuesPasswort = RandomPWGenerator();
            DetailAnzeigeEintrag.Passwort = NeuesPasswort;
        }

        public void ImportGedruckt()
        {
            Microsoft.Win32.OpenFileDialog SyncDatei = new Microsoft.Win32.OpenFileDialog();
            SyncDatei.InitialDirectory = Properties.Settings.Default.PfadSyncOrdner;
            SyncDatei.Filter = "Export Datei (*.exp)|*.exp";
            SyncDatei.RestoreDirectory = true;
            Nullable<bool> result = SyncDatei.ShowDialog();
            if (result == true)
            {
                Properties.Settings.Default.PfadSyncOrdner = Path.GetDirectoryName(SyncDatei.FileName);
                Properties.Settings.Default.Save();
                KaudawelschGenerator impchecker = new KaudawelschGenerator(new SecureString());
                Person impBen = LeseDoppeltVerschlüsselteDatei(SyncDatei.FileName, impchecker, Path.GetDirectoryName(SyncDatei.FileName));
                if (impBen != null)
                {
                    SendImportMess ubergabe;
                    if (_BenutzerListe.BenutzerVorhanden(impBen.Name))
                    {
                        ubergabe = new SendImportMess(ImpMoglichkeit.SyncNeuAnderer, impBen, _BenutzerListe, AktBenutzer, PWEingabe);
                       // Person EvtSyncBenutzer = _BenutzerListe.NameSuchen(impBen.Name);
                        // starte abfrage ob snc / neuer Name import da name schon vorhanden / mit anderen namen syncen
                        // anschließendes syncen.
                    }
                    else
                    {
                        if(VerwaltungsListe.Count() > 0)
                        {
                            ubergabe = new SendImportMess(ImpMoglichkeit.DirektAnderer, impBen, _BenutzerListe);
                        }
                        else
                        {

                            // ubergabe = new SendImportMess(SendImportMess.ImpMoglichkeit.Import, impBen, _BenutzerListe);
                            impBen.AktOrdnerName = NeuenBenutzerOrdnerAnlegen();
                            _BenutzerListe.Hinzufügen(impBen);
                            _BenutzerListe.KomplettVerschlüsseln(impBen.Name, PfadFindung(impBen.Name));
                            VerwaltungsAnzeigeNeuLaden();
                            //string tmpZielPfad = PfadFindung(_empfDaten.ImportPerson.Name, 2);
                            //File.Copy(DateiPfad, tmpZielPfad, true);
                    
                            return;
                        }
                      
                        // starte imp da name noch nicht vorhanden.. // frage ob man doch mit anderem namen syncen will?                    
                    }
                    MessengerInstance.Send(ubergabe);
                }
                else
                {
                    System.Windows.MessageBox.Show("Falsche Datei oder Datei zum importieren fehlerhaft.");
                    return;
                }
            }
        }

        private void PWLöschenGedruckt()
        {

            if (AktEintrag == null || AktEintrag.Programm == null)
            {
                return;
            }
            else
            {
                if (System.Windows.MessageBox.Show("Sicher das der Eintrag \n" + "Programm:  " + AktEintrag.Programm + "\n" + "Benutzer:   " + AktEintrag.Benutzer + "\n" + "gelöscht werden soll?", "Echt jetzt", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int tmpIndexStelle = IndexSuchen(AktEintrag.Programm, AktEintrag.tmprndIndex);
                    _BenutzerListe.EintragDel(AktBenutzer.Name, tmpIndexStelle, PWEingabe);
                    MainListe.RemoveAt(tmpIndexStelle);
                    _BenutzerListe.KomplettVerschlüsseln(AktBenutzer.Name, PfadFindung(AktBenutzer.Name));
                    //nichtGespeichertAnders = false;
                    //nichtGespeichertNeu = false;
                    GefilterteListe = PWListeFiltern();
                    if (GefilterteListe.Count > 0)
                    {
                        AktEintrag = GefilterteListe[0];
                    }
                    else
                    {
                        AktEintrag = null;
                    }

                }

            }
        }


        public void EmpfangeNeuesCenterSync(EmpfCenterMess NeuesCenter)
        {
            // eigentlich sollte nun die Observable erweitert werden und auch die _Benutzerliste.. neu initialiserien?
            _BenutzerListe = NeuesCenter.Center;         
            if (NeuesCenter.AktivesProfil != null)
            {
                // mit Passwort einloggen, im profil bleiben und liste anzeigen
                // combo muss bleiben?
            }
            else
            {
              //  initzializeBenutzer(); nicht notwendig?
                VerwaltungsAnzeigeNeuLaden();
            }
        }



        private void RootOrdnerGedruckt()
        {
            CommonOpenFileDialog HoleNeuenOrdner = new CommonOpenFileDialog();

            HoleNeuenOrdner.InitialDirectory = Properties.Settings.Default.PfadZielOrdner;
            HoleNeuenOrdner.Multiselect = false;
            HoleNeuenOrdner.IsFolderPicker = true;
            if (HoleNeuenOrdner.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Properties.Settings.Default.PfadZielOrdner = HoleNeuenOrdner.FileName + @"\";
                Properties.Settings.Default.Save();
                // initzialize();
                initzializeBenutzer();
            }
            VerwaltungsAnzeigeNeuLaden();
        }


 

        private string NeuenBenutzerOrdnerAnlegen()
        {
            string NeuerOrdner = "";
            for (int i = 0; i < 1000; i++)
            {
                if (!OrdnerVorhanden(i.ToString()))
                {
                    NeuerOrdner = Pw.Logik.Properties.Settings.Default.PfadZielOrdner + i;
                    System.IO.Directory.CreateDirectory(NeuerOrdner);
                    FileStream fs = File.Create(NeuerOrdner + PasswortEndung);
                    fs.Close();
                    return i.ToString();
                }
            }
            return "Maximale Benutzer erreicht";
        }

        private bool OrdnerVorhanden(string Ordnername)
        {
            string VorhandenerOrdnername;
            try
            {
                List<string> AlleOrdner = new List<string>(Directory.EnumerateDirectories(Pw.Logik.Properties.Settings.Default.PfadZielOrdner));
                foreach (var Ordna in AlleOrdner)
                {
                    VorhandenerOrdnername = Ordna.Substring(Ordna.LastIndexOf("\\") + 1);
                    if (VorhandenerOrdnername == Ordnername)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private bool TestaufLeerenInhalt(string ProgrammName)
        {
            if (ProgrammName == null)
            {
                return false;
            }

            if (ProgrammName.Length == ProgrammName.Count(c => c == ' '))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ProgrammVorhanden(string ProgrammName)
        {
            foreach (PwEintrag VorhanderPrgName in MainListe)
            {
                if (VorhanderPrgName.Programm.Equals(ProgrammName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ProgrammUndBenutzerVorhanden(PwEintrag DieDaten)
        {
            foreach (PwEintrag VorhanderPrgName in MainListe)
            {
                if (VorhanderPrgName.Programm.Equals(DieDaten.Programm, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (VorhanderPrgName.Benutzer.Equals(DieDaten.Benutzer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private ObservableCollection<PwEintrag> PWListeFiltern()
        {
            if (PWSuche == null || PWSuche == "") return MainListe;

            ObservableCollection<PwEintrag> ausgabe = new ObservableCollection<PwEintrag>();
            bool BitteNichtDoppelt = false;
            foreach (PwEintrag Filter1 in MainListe)
            {
                BitteNichtDoppelt = false;
                if (Filter1.Benutzer.IndexOf(PWSuche, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                        ausgabe.Add(new PwEintrag() { Benutzer = Filter1.Benutzer, Passwort = Filter1.Passwort, Programm = Filter1.Programm, tmprndIndex = Filter1.tmprndIndex });
                        BitteNichtDoppelt = true;
                }
                if (Filter1.Programm.IndexOf(PWSuche, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (!BitteNichtDoppelt)
                    {
                        ausgabe.Add(new PwEintrag() { Benutzer = Filter1.Benutzer, Passwort = Filter1.Passwort, Programm = Filter1.Programm, tmprndIndex = Filter1.tmprndIndex });
                    }
                }
            }
            return ausgabe;
        }

        private int IndexSuchen(string GesuchtesPrg, string GesuchtertmpI)
        {
            int ausgabe = 0;
            foreach (PwEintrag find in MainListe)
            {
                if (find.Programm == GesuchtesPrg && find.tmprndIndex == GesuchtertmpI)
                {
                    return ausgabe;
                }
                ausgabe++;
            }
            return -1;
        }

           private void ZeigeTooltipZischenablage(string Inhalt)
        {
            ZwischenlageTooltip = new System.Windows.Controls.ToolTip { Content = Inhalt };
            ZwischenlageTooltip.Opened += async delegate (object objTT, RoutedEventArgs args)
            {
                System.Windows.Controls.ToolTip tmpTT = objTT as System.Windows.Controls.ToolTip;
                await Task.Delay(1000);
                tmpTT.IsOpen = false;
            };
            ZwischenlageTooltip.IsOpen = true;
        }

        public void Programmschließengedruckt()
        {
            LogoutGedruckt();
            Environment.Exit(0);
        }

        private void SkinAnderung()
        {
            switch (Properties.Settings.Default.AktuellerSkin)
            {
                case 1:
                    MeinHintergrund = SkinFarben.DMHinterGrund;
                    MeineSchriftFarbe1 = SkinFarben.DMSchriftFarbe1;
                    MeineSchriftFarbe2 = SkinFarben.DMSchriftFarbe2;
                    MeineKontrastFarbe1 = SkinFarben.DMKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.DMKontrastFarbe2;
                    MeineSchriftArtNorm = SkinFarben.DMSchriftArtNorm;
                    MeineSchriftArtFett = SkinFarben.DMSchriftArtFett;
                    MeineSchriftGrosseNorm = SkinFarben.DMSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.DMSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.DMschriftGrosseGross;
                    MeinIconRauf = SkinFarben.DMPfeilUp;
                    MeinIconRunter = SkinFarben.DMPfeilDown;
                    MeinSyncIcon = SkinFarben.DMSyncIcon;
                    MeinRndIcon = SkinFarben.DMRndIcon;
                    break;
                case 0:
                    MeinHintergrund = SkinFarben.NormHinterGrund;
                    MeineSchriftFarbe1 = SkinFarben.NormSchriftFarbe1;
                    MeineSchriftFarbe2 = SkinFarben.NormSchriftFarbe2;
                    MeineKontrastFarbe1 = SkinFarben.NormaleKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.NormaleKontrastFarbe2;
                    MeineSchriftArtNorm = SkinFarben.NormalSchriftArtNorm;
                    MeineSchriftArtFett = SkinFarben.NormalSchriftArtFett;
                    MeineSchriftGrosseNorm = SkinFarben.NormalSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.NormalSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.NormalSchriftGrosseGross;
                    MeinIconRauf = SkinFarben.NormPfeilUp;
                    MeinIconRunter = SkinFarben.NormPfeilDown;
                    MeinSyncIcon = SkinFarben.NormSyncIcon;
                    MeinRndIcon = SkinFarben.NormRndIcon;
                    break;
                default:
                    MeinHintergrund = SkinFarben.DMHinterGrund;
                    MeineSchriftFarbe1 = SkinFarben.DMSchriftFarbe1;
                    MeineSchriftFarbe2 = SkinFarben.DMSchriftFarbe2;
                    MeineKontrastFarbe1 = SkinFarben.DMKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.DMKontrastFarbe2;
                    MeineSchriftArtNorm = SkinFarben.DMSchriftArtNorm;
                    MeineSchriftArtFett = SkinFarben.DMSchriftArtFett;
                    MeineSchriftGrosseNorm = SkinFarben.DMSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.DMSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.DMschriftGrosseGross;
                    MeinIconRauf = SkinFarben.DMPfeilUp;
                    MeinIconRunter = SkinFarben.DMPfeilDown;
                    MeinSyncIcon = SkinFarben.DMSyncIcon;
                    MeinRndIcon = SkinFarben.DMRndIcon;
                    break;
            }
          //  OptionenUberschrift = BerechneOptionenAnzeigestring(HauptFensterBreite); // rechnet sich aus wo der strich hin gehört?
        }

        private void ZwischenAgedruckt(bool Passwort)
        {
            string Inhalt;

            if (Passwort)
            {
                Inhalt = DetailAnzeigeEintrag.Passwort;
            }
            else
            {
                Inhalt = DetailAnzeigeEintrag.Benutzer;
            }

            try
            {
             //   System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, Inhalt);
                System.Windows.Clipboard.SetText(Inhalt);
                ZeigeTooltipZischenablage("In Zwischenablage kopiert.");
            }
            catch
            {
                System.Windows.Clipboard.SetText("");
                ZeigeTooltipZischenablage("Error!");
            }
        }


        private void ThemeWinStyleGedruckt()
        {

            Properties.Settings.Default.AktuellerSkin = 0;
            AktuellerSkin = SkinWahl.winsylte;
            Properties.Settings.Default.Save();
            SkinAnderung();
            // achtung bei anderenen Fenster muss auch der richtige skin kommen
        }

        private void ThemeDarkStyleGedruckt()
        {
            Properties.Settings.Default.AktuellerSkin = 1;
            AktuellerSkin = SkinWahl.Darkstyle;
            Properties.Settings.Default.Save();
            SkinAnderung();
            // achtung bei anderenen Fenster muss auch der richtige skin kommen
        }

        #region RandomGenerator
        private string RandomPWGenerator()
        {
            char[] SonderZeichen = new char[] { '#', '§', '!', '$', '%', '&', '?' };
            char[] ZahlZeichen = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            char[] GrossZeichen = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'O', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] NormalZeichen = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            char[] EinzelneSymb = new char[Properties.Settings.Default.PWLange];
            char[] KompletteMoglichkeit = NormalZeichen;
            char NeuesZeichen;
            int RndZahl;
            Random ZufallGen = new Random();
            StringBuilder GesamtBuilder = new StringBuilder();
            int[] nochFrei = WasIstNochFrei(EinzelneSymb);

            if (Properties.Settings.Default.PWSonderBool)
            {
                KompletteMoglichkeit = KompletteMoglichkeit.Concat(SonderZeichen).ToArray();
                for (int i = 0; i < Properties.Settings.Default.PWMinSonder; i++)
                {
                    if (nochFrei.Length == 0)
                    {
                        break;
                    }
                    NeuesZeichen = ErzeugeEinRndSymbol(ZufallGen, SonderZeichen);
                    RndZahl = ZufallGen.Next(0, nochFrei.Length);
                    EinzelneSymb[nochFrei[RndZahl]] = NeuesZeichen;
                    nochFrei = nochFrei.Where(val => val != nochFrei[RndZahl]).ToArray();
                }
            }

            if (Properties.Settings.Default.PWZahlBool)
            {
                KompletteMoglichkeit = KompletteMoglichkeit.Concat(ZahlZeichen).ToArray();
                for (int i = 0; i < Properties.Settings.Default.PWMinZahl; i++)
                {
                    if (nochFrei.Length == 0)
                    {
                        break;
                    }
                    NeuesZeichen = ErzeugeEinRndSymbol(ZufallGen, ZahlZeichen);
                    RndZahl = ZufallGen.Next(0, nochFrei.Length);
                    EinzelneSymb[nochFrei[RndZahl]] = NeuesZeichen;
                    nochFrei = nochFrei.Where(val => val != nochFrei[RndZahl]).ToArray();
                }
            }

            if (Properties.Settings.Default.PWGrossBool)
            {
                KompletteMoglichkeit = KompletteMoglichkeit.Concat(GrossZeichen).ToArray();
                for (int i = 0; i < Properties.Settings.Default.PWMinGross; i++)
                {
                    if (nochFrei.Length == 0)
                    {
                        break;
                    }
                    NeuesZeichen = ErzeugeEinRndSymbol(ZufallGen, GrossZeichen);
                    RndZahl = ZufallGen.Next(0, nochFrei.Length);
                    EinzelneSymb[nochFrei[RndZahl]] = NeuesZeichen;
                    nochFrei = nochFrei.Where(val => val != nochFrei[RndZahl]).ToArray();
                }
            }

            KompletteMoglichkeit = KompletteMoglichkeit.Concat(NormalZeichen).ToArray();
            while (nochFrei.Length != 0)
            {
                NeuesZeichen = ErzeugeEinRndSymbol(ZufallGen, KompletteMoglichkeit);
                RndZahl = ZufallGen.Next(0, nochFrei.Length);
                EinzelneSymb[nochFrei[RndZahl]] = NeuesZeichen;
                nochFrei = nochFrei.Where(val => val != nochFrei[RndZahl]).ToArray();
            }
            GesamtBuilder.Append(EinzelneSymb);
            return GesamtBuilder.ToString();
        }
        private int[] WasIstNochFrei(char[] DerzPw)
        {
            int[] Tmpfrei = new int[DerzPw.Length];
            int[] Ergebnis = null;
            int EchteGr = 0;
            for (int i = 0; i < DerzPw.Length; i++)
            {
                if (DerzPw[i] == '\0')
                {
                    Tmpfrei[EchteGr] = i;
                    EchteGr++;
                }
            }

            if (EchteGr == 0)
            {
                return null;
            }
            else
            {
                Ergebnis = new int[EchteGr];
                for (int i = 0; i < EchteGr; i++)
                {
                    Ergebnis[i] = Tmpfrei[i];
                }
                return Ergebnis;
            }
        }
        static char ErzeugeEinRndSymbol(Random Zufall, char[] pool)
        {
            return pool[Zufall.Next(0, pool.Length)];
        }
        #endregion

    }


}