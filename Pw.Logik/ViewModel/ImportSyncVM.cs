using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Logik.Pw.Logik.Items;
using Logik.Pw.Logik.Klassen;
using Logik.Pw.Logik.Messengers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Logik.Pw.Logik.Messengers.SendImportMess;

namespace Logik.Pw.Logik.ViewModel
{
    public class ImportSyncVM : ViewModelBase, IDataErrorInfo
    {
        public enum ImpMoglichkeit { WahlNeuAnderer, NeuAnlage, PwAndern, BenutzerDel, AndererBenutzer, NeuMitInhalt }

        private SendImportMess _empfDaten;

        private RelayCommand _okBtn, _auswahlNeuBtn, _auswahlAuswahlBtn;
        public SecureString Pw1Eingabe { get; set; }
        public SecureString Pw2Eingabe { get; set; }
        public SecureString Pw3Eingabe { get; set; }

        public ObservableCollection<string> ComboNamen  { get; set; }
        public string MoglichBenItem { get; set; }
        public string ErrorMeldung { get; set; }

        public string InputUberschrift { get; set; }
        public string InputV2Uberschrift { get; set; }
        public string InputV3Uberschrift { get; set; }


        private string _BenutzerNameTB;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Programmname darf nicht leer sein")]
        [MinLength(2, ErrorMessage = "Min 2 Stellen")]
        public string BenutzerNameTB { get { return _BenutzerNameTB; } set
            {
                _BenutzerNameTB = value; RaisePropertyChanged();
                bool vergeben = _empfDaten.Center.BenutzerVorhanden(BenutzerNameTB);
                if(BenutzerNameTB.Length > 2)
                {
                    if (_empfDaten.Center.BenutzerVorhanden(BenutzerNameTB)) { ErrorMeldung = "vergeben"; } else { ErrorMeldung = "Ok"; }
                }
                else
                {
                    ErrorMeldung = "";
                }
            } 
        }
        
        public Visibility VisiOKBtn { get; set; }
        public Visibility VisiButton2Stk { get; set; }
        public Visibility VisiTextBox { get; set; }
        public Visibility VisiPasswort1 { get; set; }
        public Visibility VisiPasswort2 { get; set; }
        public Visibility VisiPasswort3 { get; set; }
        public Visibility VisiComboBox { get; set; }
        //public Visibility Version5Visi { get; set; }

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

        public RelayCommand OkBtn => _okBtn;
        public RelayCommand AuswahlNeuBtn => _auswahlNeuBtn;
        public RelayCommand AuswahlAuswahlBtn => _auswahlAuswahlBtn;

        public ImportSyncVM()
        {
            _okBtn = new RelayCommand(EingabeErledigt);
            _auswahlNeuBtn = new RelayCommand(() => { _empfDaten.Anzeige = ImpMoglichkeit.NeuMitInhalt; AnzeigeWechsel(ImpMoglichkeit.NeuMitInhalt); });
            _auswahlAuswahlBtn = new RelayCommand(() => { _empfDaten.Anzeige = ImpMoglichkeit.AndererBenutzer; AnzeigeWechsel(ImpMoglichkeit.AndererBenutzer); });

        }

        public void Initialisiere(SendImportMess daten)
        {
            Pw1Eingabe = new SecureString();
            Pw2Eingabe = new SecureString();
            Pw3Eingabe = new SecureString();

            _empfDaten = daten;
            SkinWechsel();
            AnzeigeWechsel(_empfDaten.Anzeige);
          
        }

        private void AnzeigeWechsel(ImpMoglichkeit auswahl)
        {
            switch (auswahl)
            {
                case ImpMoglichkeit.NeuAnlage: // alt 4
                    ErrorMeldung = "";
                    InputUberschrift = "BenutzerName:";
                    InputV2Uberschrift = "Passwort:";
                    InputV3Uberschrift = "Passwort Wdh:";
                    VisiTextBox = Visibility.Visible;
                    VisiPasswort1 = Visibility.Hidden;
                    VisiPasswort2 = Visibility.Visible;
                    VisiPasswort3 = Visibility.Visible;
                    VisiButton2Stk = Visibility.Hidden;
                    VisiComboBox = Visibility.Hidden;
                    VisiOKBtn = Visibility.Visible;
                    break;
                case ImpMoglichkeit.BenutzerDel: // alt 0
                    InputUberschrift = "Passwort:";
                    VisiTextBox = Visibility.Hidden;
                    VisiPasswort1 = Visibility.Visible;
                    VisiPasswort2 = Visibility.Hidden;
                    VisiPasswort3 = Visibility.Hidden;
                    VisiButton2Stk = Visibility.Hidden;
                    VisiComboBox = Visibility.Hidden;
                    VisiOKBtn = Visibility.Visible;
                    break;
                case ImpMoglichkeit.WahlNeuAnderer: //alt 2
                    InputUberschrift = "Benutzer-Name vergeben.";
                    VisiTextBox = Visibility.Hidden;
                    VisiPasswort1 = Visibility.Hidden;
                    VisiPasswort2 = Visibility.Hidden;
                    VisiPasswort3 = Visibility.Hidden;
                    VisiButton2Stk = Visibility.Visible;
                    VisiComboBox = Visibility.Hidden;
                    VisiOKBtn = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.AndererBenutzer: // alt 5
                    ComboNamen = _empfDaten.Center.VerwaltungListe();
                    MoglichBenItem = ComboNamen[0];
                    InputUberschrift = "";
                    InputV2Uberschrift = "Benutzer-Passwort:";
                    InputV3Uberschrift = "Import-Passwort:";
                    VisiTextBox = Visibility.Hidden;
                    VisiPasswort1 = Visibility.Hidden;
                    VisiPasswort2 = Visibility.Visible;
                    VisiPasswort3 = Visibility.Visible;
                    VisiButton2Stk = Visibility.Hidden;
                    VisiComboBox = Visibility.Visible;
                    VisiOKBtn = Visibility.Visible;
                    break;
                case ImpMoglichkeit.PwAndern: // alt 6
                    InputUberschrift = "Altes Passwort:";
                    InputV2Uberschrift = "Neues Passwort:";
                    InputV3Uberschrift = "Neues Pwasswort wdh:";
                    VisiTextBox = Visibility.Hidden;
                    VisiPasswort1 = Visibility.Visible;
                    VisiPasswort2 = Visibility.Visible;
                    VisiPasswort3 = Visibility.Visible;
                    VisiButton2Stk = Visibility.Hidden;
                    VisiComboBox = Visibility.Hidden;
                    VisiOKBtn = Visibility.Visible;
                    break;
                case ImpMoglichkeit.NeuMitInhalt:
                    ErrorMeldung = "";
                    InputUberschrift = "BenutzerName:";
                    InputV2Uberschrift = "";
                    InputV3Uberschrift = "";
                    VisiTextBox = Visibility.Visible;
                    VisiPasswort1 = Visibility.Hidden;
                    VisiPasswort2 = Visibility.Hidden;
                    VisiPasswort3 = Visibility.Hidden;
                    VisiButton2Stk = Visibility.Hidden;
                    VisiComboBox = Visibility.Hidden;
                    VisiOKBtn = Visibility.Visible;
                    break;
                default:
                    return;
            }
        }

        private void SyncGedruckt()
        {

        }



        private bool NeuAnlageGedruckt()
        {
            if (!TestaufLeerenInhalt(BenutzerNameTB))
            {
                System.Windows.MessageBox.Show("Bitte einen Neuen Namen vergeben.");
                return false;
            }
            if (_empfDaten.Center.BenutzerVorhanden(BenutzerNameTB))
            {
                //System.Windows.MessageBox.Show("Name schon vergeben");
                ErrorMeldung = "vergeben";
                return false;
            }
            if (!Abfrage.PasswortIstKorrekt(Pw2Eingabe, Pw3Eingabe))
            {
                System.Windows.MessageBox.Show("Passwörter nicht ident.");
                return false;
            }
            Person _neuBen = new Person();
            _neuBen.Name = BenutzerNameTB;
            _neuBen.AktOrdnerName = NeuenBenutzerOrdnerAnlegen();
            _empfDaten.Center.Hinzufügen(_neuBen);
            _empfDaten.Center.ErstEintrag(_neuBen.Name, Pw2Eingabe);
            _empfDaten.Center.KomplettVerschlüsseln(_neuBen.Name, PfadFindung(_neuBen.Name));
            return true;
        }

        private bool NeuMitInhaltGedruckt()
        {
            if (!TestaufLeerenInhalt(BenutzerNameTB))
            {
                System.Windows.MessageBox.Show("Bitte einen Neuen Namen vergeben.");
                return false;
            }
            if (_empfDaten.Center.BenutzerVorhanden(BenutzerNameTB))
            {
                //System.Windows.MessageBox.Show("Name schon vergeben");
                ErrorMeldung = "vergeben";
                return false;
            }

            Person tmpimportNeu = new Person();
            tmpimportNeu.PersiKauda = _empfDaten.ImportPerson.PersiKauda;
            tmpimportNeu.Name = BenutzerNameTB;
            tmpimportNeu.AktOrdnerName = NeuenBenutzerOrdnerAnlegen();
            _empfDaten.Center.Hinzufügen(tmpimportNeu);
            _empfDaten.Center.KomplettVerschlüsseln(BenutzerNameTB, PfadFindung(BenutzerNameTB));
            return true;
        }

        private bool PWAndersGdruckt()
        {
            KaudawelschGenerator Checker = new KaudawelschGenerator(Pw1Eingabe);
            if (Checker.PwChecker(_empfDaten.ImportPerson.PersiKauda))
            {
                if (!Abfrage.PasswortIstKorrekt(Pw2Eingabe, Pw3Eingabe))
                {
                    MessageBox.Show("Neues Passwort nicht ident."); // als Errormessage neben wdh?
                    return false;
                }
                _empfDaten.Center.BenutzerCh(_empfDaten.ImportPerson.Name, Pw2Eingabe, Pw1Eingabe);
                _empfDaten.Center.KomplettVerschlüsseln(_empfDaten.ImportPerson.Name, PfadFindung(_empfDaten.ImportPerson.Name));
                return true;
            }
            else
            {
                MessageBox.Show("Altes Passwort nicht korrekt.");
                return false;
            }
        }

        private bool BenutzerDelGedruckt()
        {
            KaudawelschGenerator Checker = new KaudawelschGenerator(Pw1Eingabe);
            if (Checker.PwChecker(_empfDaten.ImportPerson.PersiKauda))
            {
                _empfDaten.Center.BenutzerDel(_empfDaten.ImportPerson.Name);
                Directory.Delete(Properties.Settings.Default.PfadZielOrdner + @"\" + _empfDaten.ImportPerson.AktOrdnerName, true);
                return true;
            }
            MessageBox.Show("Falsche Eingabe");
            return false;
        }

        private bool SyncMitNutzer(Person ImportDat, SecureString ImpPw, Person SyncDat, SecureString SyncPw)
        {
            // pw check von beiden
            // syncen
            KaudawelschGenerator CheckerImp = new KaudawelschGenerator(ImpPw);
            KaudawelschGenerator CheckerSync = new KaudawelschGenerator(SyncPw);
            if (CheckerImp.PwChecker(ImportDat.PersiKauda) && CheckerSync.PwChecker(SyncDat.PersiKauda))
            {
                Synce(ImportDat, ImpPw, SyncDat, SyncPw).Wait();
                return true;
            }
            return false;
        }

        private async Task Synce(Person ImportDat, SecureString ImpPw, Person SyncDat, SecureString SyncPw)
        {
            KaudawelschGenerator KaudaChecker = new KaudawelschGenerator(ImpPw);
            ObservableCollection<PwEintrag> ImportiertePws = KaudaChecker.LadePassworter(ImportDat.PersiKauda);
            KaudaChecker = new KaudawelschGenerator(SyncPw);
            ObservableCollection<PwEintrag> SyncPws = KaudaChecker.LadePassworter(SyncDat.PersiKauda);
            foreach (PwEintrag SyncMe in ImportiertePws)
            {
                SyncPws = await SynchronisiereDaten(SyncMe, SyncPws);
            }
            SyncDat.PersiKauda = new List<string>();
            foreach (PwEintrag Eintrag in SyncPws)
            {
                _empfDaten.Center.NormEintragVersHinzu(Eintrag, SyncPw, SyncDat.Name, false);
            }
            _empfDaten.Center.KomplettVerschlüsseln(SyncDat.Name, PfadFindung(SyncDat.Name));
        }

        private async Task<ObservableCollection<PwEintrag>> SynchronisiereDaten(PwEintrag SyncDaten, ObservableCollection<PwEintrag> BenutzerListe)
        {
            bool Datengefunden = false;
            foreach (PwEintrag AktDaten in BenutzerListe)
            {
                if (AktDaten.Programm == SyncDaten.Programm)
                {
                    if (AktDaten.Benutzer == SyncDaten.Benutzer)
                    {
                        Datengefunden = true;
                        if (AktDaten.Passwort == SyncDaten.Passwort)
                        {
                            continue;
                        }
                        else
                        {
                            if (SyncDaten.Datum > AktDaten.Datum)
                            {
                                AktDaten.Passwort = SyncDaten.Passwort;
                                AktDaten.Datum = SyncDaten.Datum;
                            }
                        }
                    }
                }
            }
            if (!Datengefunden)
            {
                BenutzerListe.Add(SyncDaten);
            }
            return BenutzerListe;
        }

        public void EingabeErledigt()
        {
            bool tmpkorrekt = false;
            EmpfCenterMess tmpUbergabeDaten = null;
            switch (_empfDaten.Anzeige)
            {
                case ImpMoglichkeit.NeuAnlage:
                    tmpkorrekt = NeuAnlageGedruckt();
                    break;
                case ImpMoglichkeit.PwAndern:
                    tmpkorrekt = PWAndersGdruckt();
                    break;
                case ImpMoglichkeit.BenutzerDel:
                    tmpkorrekt = BenutzerDelGedruckt();
                    break;
                case ImpMoglichkeit.NeuMitInhalt:
                    tmpkorrekt = NeuMitInhaltGedruckt();
                    break;
                case ImpMoglichkeit.AndererBenutzer:
                    tmpkorrekt = SyncMitNutzer(_empfDaten.ImportPerson, Pw3Eingabe, _empfDaten.Center.NameSuchen(MoglichBenItem), Pw2Eingabe);
                    break;
            }
            if(tmpkorrekt)
            {
                DelEingaben();
                tmpUbergabeDaten = new EmpfCenterMess(_empfDaten.Center);
                MessengerInstance.Send(tmpUbergabeDaten);
                MessengerInstance.Send(new FensterCloseMess("Ui.Pw.Ui.ImpSyncFenster"));
            }
        }

        private void DelEingaben()
        {
            _BenutzerNameTB = "";
        }

        private string NeuenBenutzerOrdnerAnlegen()
        {
            string NeuerOrdner = "";
            for (int i = 0; i < 1000; i++)
            {
                if (!OrdnerVorhanden(i.ToString()))
                {
                    NeuerOrdner = Properties.Settings.Default.PfadZielOrdner + i;
                    System.IO.Directory.CreateDirectory(NeuerOrdner);
                    FileStream fs = File.Create(NeuerOrdner + MainViewModel.PasswortEndung);
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
                List<string> AlleOrdner = new List<string>(Directory.EnumerateDirectories(Properties.Settings.Default.PfadZielOrdner));
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
        private string PfadFindung(string BenutzerName) => Pw.Logik.Properties.Settings.Default.PfadZielOrdner + _empfDaten.Center.OrdnerNameHolen(BenutzerName) + MainViewModel.PasswortEndung;
        private void SkinWechsel()
        {
          
            switch (MainViewModel.AktuellerSkin)
            {
                case MainViewModel.SkinWahl.Darkstyle:
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
                    break;
                case MainViewModel.SkinWahl.winsylte:
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
                    break;
            }
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

        #region IDataErrorInfo Management
        public bool HasErrors => Errors.Any();
        private Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
        public string Error => string.Empty;
        public string this[string PropertyName]
        {
            get
            {
                CollectErrors();
                return Errors.ContainsKey(PropertyName) ? Errors[PropertyName] : string.Empty;
            }
        }
        private static List<PropertyInfo> _propertyInfos;
        protected List<PropertyInfo> PropertyInfos
        {
            get
            {
                if (_propertyInfos == null)
                {
                    Trace.TraceInformation("Sammel type Infos");
                    _propertyInfos =
                        GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.IsDefined(typeof(RequiredAttribute), true) || prop.IsDefined(typeof(MinLengthAttribute), true)) // DAtaAnnotation
                        .ToList();
                }
                return _propertyInfos;
            }
        }

        private void CollectErrors()
        {
            Errors.Clear();
            foreach (PropertyInfo prop in PropertyInfos)
            {
                var currentValue = prop.GetValue(this);
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                var minLenAttr = prop.GetCustomAttribute<MinLengthAttribute>();
                if (requiredAttr != null)
                {
                    if (string.IsNullOrEmpty(currentValue?.ToString() ?? string.Empty))
                    {
                        Errors.Add(prop.Name, requiredAttr.ErrorMessage);
                    }
                }
                if (minLenAttr != null)
                {
                    if ((currentValue?.ToString() ?? string.Empty).Length < minLenAttr.Length)  // ?? wenn das links von ?? nicht null ist dann dann links sonst rechts.
                    {
                        if (Errors.ContainsKey(prop.Name)) continue;
                        Errors.Add(prop.Name, minLenAttr.ErrorMessage);
                    }
                }
            }
        }
        #endregion

    }
}
