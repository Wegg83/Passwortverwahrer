using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Logik.Pw.Logik.Klassen;
using Logik.Pw.Logik.Messengers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Logik.Pw.Logik.Messengers.SendImportMess;

namespace Logik.Pw.Logik.ViewModel
{
    public class ImportSyncVM : ViewModelBase
    {
        private SendImportMess _empfDaten;

        private RelayCommand _okBtn;
        public SecureString Pw1Eingabe { get; set; }

        public ObservableCollection<string> ComboNamen  { get; set; }
        public string MoglichBenItem { get; set; }

        public string InputUberschrift { get; set; }
        public string InputV2Uberschrift { get; set; }
        public string InputV3Uberschrift { get; set; }
        public Visibility Version1Visi { get; set; }
        public Visibility VisiSyncBtn { get; set; }
        public Visibility VisiNeuBtn { get; set; }
        public Visibility VisiAndererBtn { get; set; }
        public Visibility Version3Visi { get; set; }
        public Visibility Version4Visi { get; set; }
        public Visibility Version5Visi { get; set; }

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

        public ImportSyncVM()
        {
            _okBtn = new RelayCommand(EingabeErledigt);      
        }

        public void Initialisiere(SendImportMess daten)
        {
            _empfDaten = daten;
            SkinWechsel();

            switch (_empfDaten.Anzeige)
            {
                case ImpMoglichkeit.NeuAnlage: // alt 4
                    InputUberschrift = "BenutzerName:";
                    InputV2Uberschrift = "Passwort:";
                    InputV3Uberschrift = "Passwort Wdh:";
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Visible;
                    Version4Visi = Visibility.Visible;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.BenutzerDel: // alt 0
                    InputUberschrift = "Passwort:";
                    Version1Visi = Visibility.Visible;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Hidden;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.SyncNeuAnderer: //alt 2
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Visible;
                    VisiNeuBtn = Visibility.Visible;
                    VisiAndererBtn = Visibility.Visible;
                    Version3Visi = Visibility.Hidden;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.DirektAnderer:
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Visible;
                    VisiAndererBtn = Visibility.Visible;
                    Version3Visi = Visibility.Hidden;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.Sync: // 3
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Visible;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.AndererBenutzer: // alt 5
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Visible;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Visible;
                    ComboNamen = _empfDaten.Center.VerwaltungListe();
                    MoglichBenItem = ComboNamen[0];
                    InputV2Uberschrift = "Benutzer-Passwort:";
                    InputV3Uberschrift = "Import-Passwort:";
                    break;
                case ImpMoglichkeit.PwAndern: // alt 6
                    Version1Visi = Visibility.Hidden;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Visible;
                    Version4Visi = Visibility.Visible;
                    Version5Visi = Visibility.Hidden;
                    break;
                case ImpMoglichkeit.Import: // alt 0
                    InputUberschrift = "Passwort:";
                    Version1Visi = Visibility.Visible;
                    VisiSyncBtn = Visibility.Hidden;
                    VisiNeuBtn = Visibility.Hidden;
                    VisiAndererBtn = Visibility.Hidden;
                    Version3Visi = Visibility.Hidden;
                    Version4Visi = Visibility.Hidden;
                    Version5Visi = Visibility.Hidden;
                    break;
            }
        }

        private void SyncGedruckt()
        {

        }
        private void NeuAnlageGedruckt()
        {

        }
        private void AndererNutzerSyncGedruckt()
        {

        }

        public void EingabeErledigt()
        {
            bool tmpkorrekt = false;
            EmpfCenterMess tmpUbergabeDaten = null;
            switch (_empfDaten.Anzeige)
            {
                // code fehlt!
                case ImpMoglichkeit.Import:
                    // passwort korrekt?
                    // import 
                    // bool true
                    bool tmpPw = false;
                    KaudawelschGenerator Checker = new KaudawelschGenerator(Pw1Eingabe);
                    if (Checker.PwChecker(_empfDaten.ImportPerson.PersiKauda))
                    {
                        _empfDaten.ImportPerson.AktOrdnerName = NeuenBenutzerOrdnerAnlegen();
                        _empfDaten.Center.Hinzufügen(_empfDaten.ImportPerson);
                        //string tmpZielPfad = PfadFindung(_empfDaten.ImportPerson.Name, 2);
                        //File.Copy(DateiPfad, tmpZielPfad, true);
                        _empfDaten.Center.KomplettVerschlüsseln(_empfDaten.ImportPerson.Name, PfadFindung(_empfDaten.ImportPerson.Name));
                        tmpkorrekt = true;
                    }
                    else
                    {
                        MessageBox.Show("Passwort nicht korrekt"); // könnte man ohne Messagebox machen
                    }
                    break;
            }
            if(tmpkorrekt)
            {
                MessengerInstance.Send(tmpUbergabeDaten);
            }
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
        public void SkinWechsel()
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
    }
}
