using GalaSoft.MvvmLight;
using Logik.Pw.Logik.Klassen;
using Logik.Pw.Logik.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Logik.Pw.Logik.Messengers.SendImportMess;

namespace Logik.Pw.Logik.ViewModel
{
    public class ImportSyncVM : ViewModelBase
    {
        private ImpMoglichkeit Variante;

        public string InputUberschrift { get; set; }
        public string InputV2Uberschrift { get; set; }
        public string InputV3Uberschrift { get; set; }
        public Visibility Version1Visi { get; set; }
        public Visibility Version2Visi { get; set; }
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

        public void Initialisiere(SendImportMess daten)
        {
            Variante = daten.Anzeige;
            SkinWechsel();

            switch (Variante)
            {
                case ImpMoglichkeit.NeuAnlage:
                    InputUberschrift = "BenutzerName:";
                    InputV2Uberschrift = "Passwort:";
                    InputV3Uberschrift = "Passwort Wdh:";
                    Version1Visi = Visibility.Hidden;
                    Version2Visi = Visibility.Hidden;
                    Version3Visi = Visibility.Visible;
                    Version4Visi = Visibility.Visible;
                    Version5Visi = Visibility.Hidden;
                    break;
            }
        }

        public void EingabeErledigt()
        {
            switch (Variante)
            {

            }
        }

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
