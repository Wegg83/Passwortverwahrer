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
    // Benutzerview ist nur ein string?

    class PersonCenter
    {
        private PwEintrag ErstlingDaten;
        private static Random random = new Random();

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

        public PersonCenter()
        {
            größe = 0;
            StartKnoten = null;
            AktuellerKnoten = new BenutzerKnoten();
            ErstlingDaten = new PwEintrag();
            ErstlingDaten.Programm = "Fking";
            ErstlingDaten.Benutzer = "loving";
            ErstlingDaten.Passwort = "Dummies";
            ErstlingDaten.Datum = new DateTime(1983, 4, 12);
        }

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
        }

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

        public void EintragDel(string geNam, int Index, SecureString seinK)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            GesuchteDaten.PersiKauda.RemoveAt(Index);
            if (GesuchteDaten.PersiKauda.Count == 0)
            {
                NormEintragVersHinzu(ErstlingDaten, seinK, geNam, true);
            }
        }

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

        public void EintragCh(string geNam, PwEintrag NeuItem, SecureString seinK, int Index)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            GesuchteDaten.PersiKauda.RemoveAt(Index);
            string NeuerEintrag = ErstelleZahlen(NeuItem, seinK);
            GesuchteDaten.PersiKauda.Insert(Index, NeuerEintrag);
        }

        public void BenutzerCh(string geNam, SecureString seinK, SecureString seinAK)
        {
            Person GesuchteDaten = NameSuchen(geNam);
            KaudawelschGenerator changecheck = new KaudawelschGenerator(seinAK);
            ObservableCollection<PwEintrag> huiList = changecheck.LadePassworter(GesuchteDaten.PersiKauda);
            if (huiList.Count() == 0)
            {
                huiList.Add(ErstlingDaten);
            }
            List<string> puhList = new List<string>();
            string puhstr;
            foreach (PwEintrag hv in huiList)
            {
                puhstr = ErstelleZahlen(hv, seinK);
                puhList.Add(puhstr);
            }
            GesuchteDaten.PersiKauda = puhList;
        }

        public ObservableCollection<string> VerwaltungListe()
        {
            ObservableCollection<string> NeueVerwaltung = new ObservableCollection<string>();
            AktuellerKnoten = StartKnoten;

            for (int i = 0; i < größe; i++)
            {
                NeueVerwaltung.Add(AktuellerKnoten.DerBenutzer.Name);
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            return NeueVerwaltung;
        }

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
            KaudawelschGenerator NeuVerschlüsseler = new KaudawelschGenerator(seinK);
            VerschlüsselteZeile = StrichpunktPWChecker(NeuerInhalt.Programm, NeuerInhalt.Benutzer, NeuerInhalt.Passwort, NeuerInhalt.Datum);
            VerschlüsselteZeileByte = NeuVerschlüsseler.Verschlüsselung(VerschlüsselteZeile);
            VerschlüsselteZeile = string.Empty;
            for (int o = 0; o < VerschlüsselteZeileByte.Length; o++)
            {
                VerschlüsselteZeile += VerschlüsselteZeileByte[o] + ";";
            }
            return VerschlüsselteZeile;
        }

        public void NormEintragVersHinzu(PwEintrag NeuerInhalt, SecureString seinK, string Benname, bool Neuanlegen)
        {
            Person DerBenutzer = NameSuchen(Benname);
            if (DerBenutzer.PersiKauda == null || Neuanlegen)
            {
                DerBenutzer.PersiKauda = new List<string>();
            }
            string VerschlüsselteZeile = ErstelleZahlen(NeuerInhalt, seinK);
            DerBenutzer.PersiKauda.Add(VerschlüsselteZeile);
        }

        public bool BenutzerVorhanden(string GesuchterName)
        {
            AktuellerKnoten = StartKnoten;
            for (int i = 0; i < größe; i++)
            {
                if (AktuellerKnoten.DerBenutzer.Name.Equals(GesuchterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                AktuellerKnoten = AktuellerKnoten.folgender;
            }
            return false;
        }

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
            return "";
        }

        private string StrichpunktBenutzerChecker(string Input)
        {
            string StrichName = "";
            int Stelle;
            #region Name
            while (Input.Count(c => c == ';') != 0)
            {
                Stelle = Input.IndexOf(';');
                Input = Input.Remove(Stelle, 1);
                StrichName += Stelle.ToString() + "-";
            }
            if (StrichName == "")
            {
                StrichName = "X";
            }
            else
            {
                StrichName = StrichName.Remove(StrichName.Length - 1);
            }
            #endregion
            return Input + ";" + StrichName;
        }

        private string StrichpunktPWChecker(string OrigProgramm, string OrigBenutzer, string OrigPasswort, DateTime OrigDatum)
        {
            string StrichProgram = "", StrichBenutzer = "", StrichPasswort = "";
            int Stelle;
            #region Program
            while (OrigProgramm.Count(c => c == ';') != 0)
            {
                Stelle = OrigProgramm.IndexOf(';');
                OrigProgramm = OrigProgramm.Remove(Stelle, 1);
                StrichProgram += Stelle.ToString() + "-";
            }
            if (StrichProgram == "")
            {
                StrichProgram = "X";
            }
            else
            {
                StrichProgram = StrichProgram.Remove(StrichProgram.Length - 1);
            }
            #endregion

            #region Benutzer
            while (OrigBenutzer.Count(c => c == ';') != 0)
            {
                Stelle = OrigBenutzer.IndexOf(';');
                OrigBenutzer = OrigBenutzer.Remove(Stelle, 1);
                StrichBenutzer += Stelle.ToString() + "-";
            }
            if (StrichBenutzer == "")
            {
                StrichBenutzer = "X";
            }
            else
            {
                StrichBenutzer = StrichBenutzer.Remove(StrichBenutzer.Length - 1);
            }
            #endregion

            #region Passwort
            while (OrigPasswort.Count(c => c == ';') != 0)
            {
                Stelle = OrigPasswort.IndexOf(';');
                OrigPasswort = OrigPasswort.Remove(Stelle, 1);
                StrichPasswort += Stelle.ToString() + "-";
            }
            if (StrichPasswort == "")
            {
                StrichPasswort = "X";
            }
            else
            {
                StrichPasswort = StrichPasswort.Remove(StrichPasswort.Length - 1);
            }
            #endregion

            string DatumsNummer = DatumsErzeugung(OrigDatum);
            return OrigProgramm + ";" + OrigBenutzer + ";" + OrigPasswort + ";" + StrichProgram + ";" + StrichBenutzer + ";" + StrichPasswort + ";" + DatumsNummer;
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

        private string DatumsErzeugung(DateTime GesDat)
        {
            DateTime StartDate = new DateTime(1983, 4, 13);
            System.TimeSpan diff = GesDat.Subtract(StartDate);
            return (GesDat - StartDate).TotalDays.ToString();
        }

        public void ErstEintrag(string Benname, SecureString seinK)
        {
            NormEintragVersHinzu(ErstlingDaten, seinK, Benname, true);
        }

        public void KomplettVerschlüsseln(string BenutzerName, string SpeicherPfad)
        {
            Person Benutzer = NameSuchen(BenutzerName);
            List<string> TempKaudaListe = new List<string>();
            KaudawelschGenerator NeuCheck = new KaudawelschGenerator(new SecureString());
            string Benutzerzeile = StrichpunktBenutzerChecker(Benutzer.Name);
            byte[] BenutzerZeileByte = NeuCheck.Verschlüsselung(Benutzerzeile);
            Benutzerzeile = string.Empty;
            for (int o = 0; o < BenutzerZeileByte.Length; o++)
            {
                Benutzerzeile += BenutzerZeileByte[o] + ";";
            }
            TempKaudaListe.Add(Benutzerzeile);
            Benutzerzeile = string.Empty;
            foreach (string ListEintrag in Benutzer.PersiKauda)
            {
                BenutzerZeileByte = NeuCheck.Verschlüsselung(ListEintrag);
                for (int o = 0; o < BenutzerZeileByte.Length; o++)
                {
                    Benutzerzeile += BenutzerZeileByte[o] + ";";
                }
                TempKaudaListe.Add(Benutzerzeile);
                Benutzerzeile = string.Empty;
            }
            StreamWriter Neuschreiben = new StreamWriter(SpeicherPfad);
            foreach (string schreib in TempKaudaListe)
            {
                Neuschreiben.WriteLine(schreib);
            }
            Neuschreiben.Close();
        }
    }
}
