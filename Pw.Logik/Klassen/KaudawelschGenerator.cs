using Logik.Pw.Logik.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
namespace Logik.Pw.Logik.Klassen
{
    class KaudawelschGenerator
    {
        private SecureString MeinWunsch = new SecureString();

        public KaudawelschGenerator(SecureString MeinWunsch) // WARUM so einen konstruktor?
        {
            this.MeinWunsch = MeinWunsch;
            if(MeinWunsch == null)
            {
                MeinWunsch = new SecureString();
            }
            SecureString tmp = new SecureString();

            if (Abfrage.PasswortIstKorrekt(tmp, MeinWunsch))
            {
                this.MeinWunsch = new SecureString();
                string tmp2 = "MieseEntscheidung";
                foreach (char neu in tmp2)
                {
                    this.MeinWunsch.AppendChar(neu);
                }
            }
        }


        public byte[] Verschlüsselung(string LesbareZeile)
        {
            byte[] VerscErgebnis;
            AesManaged NeuerVerschString = new AesManaged();
            NeuerVerschString.Key = LieferHash(MeinWunsch, false);
            VerscErgebnis = LieferHash(MeinWunsch, true);
            Array.Resize(ref VerscErgebnis, 16);
            NeuerVerschString.IV = VerscErgebnis;
            VerscErgebnis = null;


            ICryptoTransform encryptor = NeuerVerschString.CreateEncryptor(NeuerVerschString.Key, NeuerVerschString.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(LesbareZeile);
                    }
                    VerscErgebnis = msEncrypt.ToArray();
                }
            }
            return VerscErgebnis;
        }

        public string Entschlüsselung(byte[] UnlesbareZeile)
        {
            string KlarText = null;
            AesManaged Entschlüsseler = new AesManaged();
            byte[] tmpkurzer;
            Entschlüsseler.Key = LieferHash(MeinWunsch, false);
            tmpkurzer = LieferHash(MeinWunsch, true);
            Array.Resize(ref tmpkurzer, 16);
            Entschlüsseler.IV = tmpkurzer;
            tmpkurzer = null;

            ICryptoTransform decryptor = Entschlüsseler.CreateDecryptor(Entschlüsseler.Key, Entschlüsseler.IV);
            try
            {
                using (MemoryStream msDecrypt = new MemoryStream(UnlesbareZeile))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            KlarText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return KlarText;
        }

        private byte[] LieferHash(SecureString wand, bool mi)
        {
            IntPtr valuePtr = IntPtr.Zero;
            SecureString ausgabe = new SecureString();
            SecureString langer = new SecureString();

            while (langer.Length < 15)
            {
                try
                {
                    valuePtr = Marshal.SecureStringToGlobalAllocUnicode(wand);
                    for (int i = 0; i < wand.Length; i++)
                    {
                        short unicodeChar = Marshal.ReadInt16(valuePtr, i * 2);
                        langer.AppendChar((char)unicodeChar);
                    }
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);

                }
            }


            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(langer);
                for (int i = 0; i < langer.Length; i++)
                {
                    short unicodeChar = Marshal.ReadInt16(valuePtr, i * 2);
                    if (i % 2 == 0)
                    {
                        if (!mi)
                        {
                            ausgabe.AppendChar((char)unicodeChar);
                        }
                    }
                    else
                    {
                        if (mi)
                        {
                            ausgabe.AppendChar((char)unicodeChar);
                        }
                    }
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);

            }

            byte[] Eingabe = new byte[1];
            Encoding encoding = null;
            encoding = encoding ?? Encoding.UTF8;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(ausgabe);


                Eingabe = encoding.GetBytes(Marshal.PtrToStringUni(unmanagedString));
            }
            finally
            {
                if (unmanagedString != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }

            using (SHA256 meinHashi = SHA256.Create())
            {
                return meinHashi.ComputeHash(Eingabe);
            }
        }

        public bool PwChecker(List<string> Pws)
        {
            if (Pws == null)
            {
                return false;
            }
            byte[] KaudawelschAkt;

            foreach (string AktZeile in Pws)
            {
                int AnzahlBytes2 = AktZeile.Split(';').Length - 1;
                if (AnzahlBytes2 > 0)
                {
                    KaudawelschAkt = new byte[AnzahlBytes2];
                    for (int i = 0; i < AnzahlBytes2; i++)
                    {
                        KaudawelschAkt[i] = Convert.ToByte(AktZeile.Split(';')[i]);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Beim entschlüsseln der Passwörter ist etwas schief gelaufen");
                    return false;
                }

                string Test = Entschlüsselung(KaudawelschAkt);
                if (Test == null || Test.Split(';').Length - 1 != 6)
                {
                    return false;
                }
                else
                {
                    string tst2 = Test.Split(';')[6];
                    if (!Int32.TryParse(tst2, out int PseudZahl))
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public string StrichpunktEntChecker(string TextmitSchlussel, int StelleText, int StelleStrichCode)
        {
            if (TextmitSchlussel.Split(';')[StelleStrichCode].ToString() == "X")
            {
                return TextmitSchlussel.Split(';')[StelleText].ToString();
            }

            string TempStrichCodeKlar = TextmitSchlussel.Split(';')[StelleStrichCode].ToString();
            string Endergebnis = TextmitSchlussel.Split(';')[StelleText].ToString();

            for (int i = 0; i <= TempStrichCodeKlar.Count(c => c == '-'); i++)
            {
                int StelleX = Convert.ToInt32(TempStrichCodeKlar.Split('-')[i]);
                Endergebnis = Endergebnis.Insert(StelleX + i, ";");
            }
            return Endergebnis;
        }

        private DateTime DatumAusKlartext(string Klartxt)
        {
            int TageSeitStart;
            if (Int32.TryParse(Klartxt, out int ergebnis))
            {
                TageSeitStart = ergebnis;
            }
            else
            {
                TageSeitStart = 0;
            }
            DateTime StartDate = new DateTime(1983, 4, 13);
            return StartDate.AddDays(TageSeitStart);
        }

        public ObservableCollection<PwEintrag> LadePassworter(List<string> Pws)
        {
            int tmpIndesstart = 100001;
            ObservableCollection<PwEintrag> NeueListe = new ObservableCollection<PwEintrag>();
            PwEintrag NeusP;
            int EntschlusstelteZeile = 0;
            byte[] KaudawelschZeile = new byte[96];
            foreach (string Endergebnis in Pws)
            {
                string Klartext = "";
                EntschlusstelteZeile++;
                int AnzahlBytes2 = Endergebnis.Split(';').Length - 1;
                if (AnzahlBytes2 > 0)
                {
                    KaudawelschZeile = new byte[AnzahlBytes2];
                    for (int i = 0; i < AnzahlBytes2; i++)
                    {
                        KaudawelschZeile[i] = Convert.ToByte(Endergebnis.Split(';')[i]);
                    }
                }
                else
                {
                    return NeueListe;
                }
                Klartext = Entschlüsselung(KaudawelschZeile);
                NeusP = new PwEintrag();
                NeusP.Programm = StrichpunktEntChecker(Klartext, 0, 3);
                NeusP.Benutzer = StrichpunktEntChecker(Klartext, 1, 4);
                NeusP.Passwort = StrichpunktEntChecker(Klartext, 2, 5);
                NeusP.Datum = DatumAusKlartext(Klartext.Split(';')[6].ToString());
                NeusP.tmprndIndex = "a" + tmpIndesstart.ToString();
                tmpIndesstart++;
                if (Klartext.Split(';')[6].ToString() != "-1")
                {
                    NeueListe.Add(NeusP);
                }
            }
            return NeueListe;
        }
    }
}
