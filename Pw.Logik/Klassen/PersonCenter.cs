using Logik.Pw.Logik.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Klassen
{

    public class PersonCenter
    {
        private PwEintrag ErstlingDaten;
        private static Random random = new Random();
        private object _lockobj = new object();
        private Logger _logSystem;


        public class BenutzerKnoten
        {
            public Person DerBenutzer;
            public BenutzerKnoten folgender;
        }

        private int größe;
        private BenutzerKnoten StartKnoten;
        private BenutzerKnoten AktuellerKnoten;

        public int Durchzählen
        {
            get { return größe; }
        }

        public PersonCenter(Logger meinLogsystem)
        {
            _logSystem = meinLogsystem;
            größe = 0;
            StartKnoten = null;
            AktuellerKnoten = new BenutzerKnoten();
            ErstlingDaten = new PwEintrag();
            ErstlingDaten.Programm = "Fking";
            ErstlingDaten.Benutzer = "loving";
            ErstlingDaten.Passwort = "Dummies";
            ErstlingDaten.Datum = new DateTime(1983, 4, 12);
        }

        /// <summary>
        /// Benutzer wird dem Center Node-System hinzugefügt
        /// </summary>
        /// <param name="NeuerBenutzer"></param>
        public void Hinzufügen(Person NeuerBenutzer)
        {
            größe++;
            BenutzerKnoten NeuerKnoten = new BenutzerKnoten();
            NeuerKnoten.DerBenutzer = NeuerBenutzer;
            if (StartKnoten == null)
            {
                StartKnoten = NeuerKnoten;
            }
            else
            {
                AktuellerKnoten = StartKnoten;
                while (AktuellerKnoten.folgender != null)
                {
                    AktuellerKnoten = AktuellerKnoten.folgender;
                }
                AktuellerKnoten.folgender = NeuerKnoten;
            }
            _logSystem.SchreibeEintrag($"Benutzer {NeuerBenutzer.Name} hinzugefügt", Logger.LogLevel.Debug);
        }

        /// <summary>
        /// Person wird anhand des NAmesn gesucht Groß-Kleinschreibung ist egal
        /// </summary>
        /// <param name="GesuchterName"></param>
        /// <returns></returns>
        public Person NameSuchen(string GesuchterName)
        {
            Person GesuchteDaten = new Person();
            GesuchteDaten.Name = "NichtsGefunden";
            AktuellerKnoten = StartKnoten;
            for (int i = 0; i < größe; i++)
            {
                if (AktuellerKnoten.DerBenutzer.Name.Equals(GesuchterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return AktuellerKnoten.DerBenutzer;
                }
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            return GesuchteDaten;
        }

        /// <summary>
        /// Eintrag von einer Person wird gelöscht
        /// </summary>
        /// <param name="geNam">Eindeutiger Name der Person</param>
        /// <param name="Index">Indexn-ummer des Eintrages</param>
        /// <param name="seinK">aktueller Code</param>
        public void EintragDel(string geNam, int Index, SecureString seinK)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            GesuchteDaten.PersiKauda.RemoveAt(Index);
            _logSystem.SchreibeEintrag("Eintrag wurde entfernt", Logger.LogLevel.Debug);
            if (GesuchteDaten.PersiKauda.Count == 0)
            {
                NormEintragVersHinzu(ErstlingDaten, seinK, geNam, true);
            }
        }

        /// <summary>
        /// Benutzer wird anhand des Namens gelöscht
        /// </summary>
        /// <param name="GesuchterName"></param>
        public void BenutzerDel(string GesuchterName)
        {
            Person GesuchteDaten = new Person();
            größe = größe - 1;
            if (StartKnoten == null)
            {
                return;
            }
            if (StartKnoten.DerBenutzer.Name == GesuchterName)
            {
                StartKnoten = StartKnoten.folgender;
            }
            AktuellerKnoten = StartKnoten;
            while (AktuellerKnoten != null)
            {
                if (AktuellerKnoten.DerBenutzer.Name == GesuchterName)
                {
                    BenutzerKnoten Einfuger = StartKnoten;
                    while (Einfuger.folgender != AktuellerKnoten)
                    {
                        Einfuger = Einfuger.folgender;
                    }
                    Einfuger.folgender = AktuellerKnoten.folgender;

                }
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
        }

        /// <summary>
        /// Eintrag wird anhand des Namens gesucht, und dann durch den neuen geänderten Eintrag neu geschrieben
        /// </summary>
        /// <param name="geNam">Eindeutiger Name der Person</param>
        /// <param name="NeuItem">Neuer geänderter Eintrag</param>
        /// <param name="seinK">aktuelller Code</param>
        /// <param name="Index">Index des gesuchten Eintrages</param>
        public void EintragCh(string geNam, PwEintrag NeuItem, SecureString seinK, int Index)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            GesuchteDaten.PersiKauda.RemoveAt(Index);
            string NeuerEintrag = ErstelleZahlen(NeuItem, seinK);
            GesuchteDaten.PersiKauda.Insert(Index, NeuerEintrag);
            _logSystem.SchreibeEintrag("Eintrag wurde geändert", Logger.LogLevel.Debug);
        }

        /// <summary>
        /// Benutzer-daten werden verändert
        /// </summary>
        /// <param name="geNam">Eindeutiger Name der Person</param>
        /// <param name="neuK">Neuer Code</param>
        /// <param name="altK">Vorheriger Code</param>
        public void BenutzerCh(string geNam, SecureString neuK, SecureString altK)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            KaudawelschGenerator changecheck = new KaudawelschGenerator(altK, _logSystem);
            ObservableCollection<PwEintrag> huiList = changecheck.LadePassworter(GesuchteDaten.PersiKauda);
            if (huiList.Count() == 0)
            {
                huiList.Add(ErstlingDaten);
            }
            List<string> puhList = new List<string>();
            string puhstr;
            foreach (PwEintrag hv in huiList)
            {
                puhstr = ErstelleZahlen(hv, neuK);
                puhList.Add(puhstr);
            }
            GesuchteDaten.PersiKauda = puhList;
            _logSystem.SchreibeEintrag("Benutzer-Passwort wurde geändert", Logger.LogLevel.Info);
        }

        /// <summary>
        /// Erstellt die Liste der Benutzer in einer Observable Collection
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> VerwaltungListe()
        {
            ObservableCollection<string> NeueVerwaltung = new ObservableCollection<string>();
            AktuellerKnoten = StartKnoten;

            for (int i = 0; i < größe; i++)
            {
                NeueVerwaltung.Add(AktuellerKnoten.DerBenutzer.Name);
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            _logSystem.SchreibeEintrag("BenutzerListe erstellt", Logger.LogLevel.Debug);
            return NeueVerwaltung;
        }

        /// <summary>
        /// Eintrag wird verschlüsselt
        /// </summary>
        /// <param name="NeuerInhalt">Neuer Eintrag</param>
        /// <param name="seinK">aktueller Code</param>
        /// <returns></returns>
        public string ErstelleZahlen(PwEintrag NeuerInhalt, SecureString seinK)
        {
            #region Abfangen von Null Daten
            if (NeuerInhalt.Benutzer == null)
            {
                NeuerInhalt.Benutzer = "";
            }
            if (NeuerInhalt.Passwort == null)
            {
                NeuerInhalt.Passwort = "";
            }
            if (NeuerInhalt.Programm == null)
            {
                NeuerInhalt.Programm = "";
            }
            #endregion
            string VerschlüsselteZeile = string.Empty;
            byte[] VerschlüsselteZeileByte;
            KaudawelschGenerator NeuVerschlüsseler = new KaudawelschGenerator(seinK, _logSystem);
            VerschlüsselteZeile = StrichpunktPWChecker(NeuerInhalt.Programm, NeuerInhalt.Benutzer, NeuerInhalt.Passwort, NeuerInhalt.Datum);
            VerschlüsselteZeileByte = NeuVerschlüsseler.Verschlüsselung(VerschlüsselteZeile);
            VerschlüsselteZeile = string.Empty;
            for (int o = 0; o < VerschlüsselteZeileByte.Length; o++)
            {
                VerschlüsselteZeile += VerschlüsselteZeileByte[o] + KaudawelschGenerator.Trenner.ToString();
            }
            _logSystem.SchreibeEintrag("Passwort erfolgreich verschlüsselt", Logger.LogLevel.Info);
            return VerschlüsselteZeile;
        }

        /// <summary>
        /// Neuer Eintrag für einen Benutzer
        /// </summary>
        /// <param name="NeuerInhalt">Neuer Listen-Eintrag</param>
        /// <param name="seinK">aktueller Code</param>
        /// <param name="Benname">eindeutiger Name</param>
        /// <param name="Neuanlegen">falls alle alten Einträger überschrieben werden sollen</param>
        public void NormEintragVersHinzu(PwEintrag NeuerInhalt, SecureString seinK, string Benname, bool Neuanlegen)
        {
            Person DerBenutzer = NameSuchen(Benname);
            if (DerBenutzer.PersiKauda == null || Neuanlegen)
            {
                DerBenutzer.PersiKauda = new List<string>();
            }
            string VerschlüsselteZeile = ErstelleZahlen(NeuerInhalt, seinK);
            DerBenutzer.PersiKauda.Add(VerschlüsselteZeile);
            _logSystem.SchreibeEintrag("Neuer Eintrag wurde hinzugefügt", Logger.LogLevel.Info);
        }

        /// <summary>
        /// Abfrage, ob der Benutzer mit diesem Namen bereits vorhanden ist
        /// </summary>
        /// <param name="GesuchterName"></param>
        /// <returns></returns>
        public bool BenutzerVorhanden(string GesuchterName)
        {
            AktuellerKnoten = StartKnoten;
            for (int i = 0; i < größe; i++)
            {
                if (AktuellerKnoten.DerBenutzer.Name.Equals(GesuchterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logSystem.SchreibeEintrag("Name bereits vorhanden", Logger.LogLevel.Debug);
                    return true;
                }
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            _logSystem.SchreibeEintrag("Name verfügbar", Logger.LogLevel.Debug);
            return false;
        }

        /// <summary>
        /// Sucht anhand des eindeutigen BenutzerNamens den zugehörigen Ordner
        /// </summary>
        /// <param name="GesuchterName"></param>
        /// <returns></returns>
        public string OrdnerNameHolen(string GesuchterName)
        {
            AktuellerKnoten = StartKnoten;
            for (int i = 0; i < größe; i++)
            {
                if (AktuellerKnoten.DerBenutzer.Name == GesuchterName)
                {
                    return AktuellerKnoten.DerBenutzer.AktOrdnerName;
                }
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            _logSystem.SchreibeEintrag($"Ordner für Benutzer {GesuchterName} nicht gefunden", Logger.LogLevel.Error);
            return "";
        }

        /// <summary>
        /// Kontrolliert ob nicht erwünschte Zeichen sich im Input befinden und tauscht diese durch eine definierte Zeichenfolge
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private SonderzeichenBestimmer Strichpunktbestimmer(string Input)
        {
            string origName = Input;
            string StrichName = "";
            int Stelle;
            #region Name
            while (Input.Count(c => c == KaudawelschGenerator.Trenner) != 0)
            {
                Stelle = Input.IndexOf(KaudawelschGenerator.Trenner);
                Input = Input.Remove(Stelle, 1);
                StrichName += Stelle.ToString() + KaudawelschGenerator.SonderzeichenErsatz;
                _logSystem.SchreibeEintrag("Sonderzeichen abgeändert", Logger.LogLevel.Debug);
            }
            if (StrichName == "")
            {
                StrichName = KaudawelschGenerator.Platzhalter.ToString();
            }
            else
            {
                StrichName = StrichName.Remove(StrichName.Length - 1);
            }
            #endregion
            _logSystem.SchreibeEintrag("Alle Sonderzeichen abgeändert", Logger.LogLevel.Debug);
            return new SonderzeichenBestimmer(origName, Input, StrichName);
        }

        /// <summary>
        /// Kontrolliert ob nicht erwünschte Zeichen sich im Input befinden und tauscht diese durch eine definierte Zeichenfolge
        /// </summary>
        /// <param name="OrigProgramm"></param>
        /// <param name="OrigBenutzer"></param>
        /// <param name="OrigPasswort"></param>
        /// <param name="OrigDatum"></param>
        /// <returns></returns>
        private string StrichpunktPWChecker(string OrigProgramm, string OrigBenutzer, string OrigPasswort, DateTime OrigDatum)
        {
            SonderzeichenBestimmer PrgErgebnis, BenutzerErgebnis, PwErgebnis;
            PrgErgebnis = Strichpunktbestimmer(OrigProgramm);
            BenutzerErgebnis = Strichpunktbestimmer(OrigBenutzer);
            PwErgebnis = Strichpunktbestimmer(OrigPasswort);
            string DatumsNummer = DatumsErzeugung(OrigDatum);
            return $"{PrgErgebnis.NeuerName}{KaudawelschGenerator.Trenner}{BenutzerErgebnis.NeuerName}{KaudawelschGenerator.Trenner}{PwErgebnis.NeuerName}{KaudawelschGenerator.Trenner}{PrgErgebnis.Stellendefinition}{KaudawelschGenerator.Trenner}{BenutzerErgebnis.Stellendefinition}{KaudawelschGenerator.Trenner}{PwErgebnis.Stellendefinition}{KaudawelschGenerator.Trenner}{DatumsNummer}";
        }

        public List<string> AlleBenutzerAlsListe()
        {
            List<string> AlleNamenGelistet = new List<string>();
            AktuellerKnoten = StartKnoten;
            for (int i = 0; i < größe; i++)
            {
                AlleNamenGelistet.Add(AktuellerKnoten.DerBenutzer.Name);
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            return AlleNamenGelistet;
        }

        /// <summary>
        /// Erstellt das aktuelle Datum im gewünschten Format als string
        /// </summary>
        /// <param name="GesDat"></param>
        /// <returns></returns>
        private string DatumsErzeugung(DateTime GesDat)
        {
            DateTime StartDate = new DateTime(1983, 4, 13);
            System.TimeSpan diff = GesDat.Subtract(StartDate);
            _logSystem.SchreibeEintrag("Datumsstring erstellt", Logger.LogLevel.Debug);
            return (GesDat - StartDate).TotalDays.ToString();
        }

        /// <summary>
        /// Erstellt den ersten Eintrag, falls noch keiner Vorhanden
        /// </summary>
        /// <param name="Benname"></param>
        /// <param name="seinK"></param>
        public void ErstEintrag(string Benname, SecureString seinK)
        {
            NormEintragVersHinzu(ErstlingDaten, seinK, Benname, true);
        }

        public void ImportiereDirekt(string BenutzerName, string SpeicherPfad, string expDateiPfad)
        {
            lock (_lockobj)
            {
                File.Copy(expDateiPfad, SpeicherPfad, true);
            }
        }

        /// <summary>
        /// Verchlüsselt den kompletten Benutzer
        /// </summary>
        /// <param name="BenutzerName"></param>
        /// <param name="SpeicherPfad"></param>
        public void KomplettVerschlüsseln(string BenutzerName, string SpeicherPfad, SecureString meinWand)
        {
            lock (_lockobj)
            {
                Person Benutzer = NameSuchen(BenutzerName);
                List<string> TempKaudaListe = new List<string>();
             //   KaudawelschGenerator NeuCheck = new KaudawelschGenerator(meinWand, _logSystem);
                KaudawelschGenerator kauda0815 = new KaudawelschGenerator(new SecureString(), _logSystem);
                SonderzeichenBestimmer benutzerbest = Strichpunktbestimmer(Benutzer.Name);
                string Benutzerzeile = benutzerbest.Gesamtergebnis;
                byte[] BenutzerZeileByte = kauda0815.Verschlüsselung(Benutzerzeile);
                Benutzerzeile = string.Empty;
                for (int o = 0; o < BenutzerZeileByte.Length; o++)
                {
                    Benutzerzeile += BenutzerZeileByte[o];
                    Benutzerzeile += KaudawelschGenerator.Trenner.ToString();
                }
                TempKaudaListe.Add(Benutzerzeile);
                Benutzerzeile = string.Empty;
                _logSystem.SchreibeEintrag("Verschlüssle aktuelle Eintrags-Liste", Logger.LogLevel.Debug);
                foreach (string ListEintrag in Benutzer.PersiKauda)
                {
                    BenutzerZeileByte = kauda0815.Verschlüsselung(ListEintrag);
                    for (int o = 0; o < BenutzerZeileByte.Length; o++)
                    {
                        Benutzerzeile += BenutzerZeileByte[o] + KaudawelschGenerator.Trenner.ToString();
                    }
                    TempKaudaListe.Add(Benutzerzeile);
                    Benutzerzeile = string.Empty;
                }
                _logSystem.SchreibeEintrag("Schreibe verschlüsseltes Ergebnis", Logger.LogLevel.Debug);
                using (StreamWriter Neuschreiben = new StreamWriter(SpeicherPfad))
                {
                    foreach (string schreib in TempKaudaListe)
                    {
                        Neuschreiben.WriteLine(schreib);
                    }
                    Neuschreiben.Close();
                }
            }
        }
    }

    class SonderzeichenBestimmer
    {
        string _alterName;
        string _neuerName;
        string _stellendefinition;

        public string AlterName => _alterName;
        public string NeuerName => _neuerName;
        public string Stellendefinition => _stellendefinition;
        public string Gesamtergebnis => $"{_neuerName}{KaudawelschGenerator.Trenner}{_stellendefinition}";
        public SonderzeichenBestimmer(string alterName, string namenErgebnis, string stellendefinierer)
        {
            this._alterName = alterName;
            this._neuerName = namenErgebnis;
            this._stellendefinition = stellendefinierer;
        }
    }

}
