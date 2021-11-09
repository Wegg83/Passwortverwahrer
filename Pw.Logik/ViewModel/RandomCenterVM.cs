using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Logik.Pw.Logik.Klassen;
using Logik.Pw.Logik.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Logik.Pw.Logik.ViewModel
{
    public class RandomCenterVM : ViewModelBase
    {
        private RelayCommand _langeRuntBtn, _langeRaufBtn, _zahlenRaufBtn, _zahlenRuntBtn, _grossRaufBtn, _grossRuntBtn, _sonderRaufBtn, _sonderRuntBtn, _zuruckBtn;
        private bool InArbeit;

        public RelayCommand LangeRuntBtn => _langeRuntBtn;
        public RelayCommand LangeRaufBtn => _langeRaufBtn;
        public RelayCommand ZahlenRaufBtn => _zahlenRaufBtn;
        public RelayCommand ZahlenRuntBtn => _zahlenRuntBtn;
        public RelayCommand GrossRaufBtn => _grossRaufBtn;
        public RelayCommand GrossRuntBtn => _grossRuntBtn;
        public RelayCommand SonderRaufBtn => _sonderRaufBtn;
        public RelayCommand SonderRuntBtn => _sonderRuntBtn;
        public RelayCommand ZuruckBtn => _zuruckBtn;

        public string ToolTipSonderzeichen { get; set; }
        public string PWLange { get; set; }
        public string MinZahlen { get; set; }
        public string MinGross { get; set; }
        public string MinSonder { get; set; }
        public string ZuruckBtnString { get; set; }

        public Visibility PWZahlenVisi { get; set; }
        public Visibility PWSonderVisi { get; set; }
        public Visibility PWGrossVisi { get; set; }
        public bool MinZahlenErlaubt { get; set; }
        public bool MinSonderErlaubt { get; set; }
        public bool MinGrossErlaubt { get; set; }
        public bool ZahlenCheck
        {
            get { return Properties.Settings.Default.PWZahlBool; }
            set
            {
                SetzeNeueZahlenErlaubnis(value);
                RaisePropertyChanged();
            }
        }
        public bool GrossCheck
        {
            get { return Properties.Settings.Default.PWGrossBool; }
            set
            {
                SetzeNeueGrossErlaubnis(value);
                RaisePropertyChanged();
            }
        }
        public bool SonderCheck
        {
            get { return Properties.Settings.Default.PWSonderBool; }
            set
            {
                SetzeNeueSonderErlaubnis(value);
                RaisePropertyChanged();
            }
        }

        public string MeinHintergrund { get; set; }
        public string MeineSchriftFarbe { get; set; }
        public string MeineKontrastFarbe1 { get; set; }
        public string MeineKontrastFarbe2 { get; set; }
        public string MeineSchriftArt { get; set; }
        public int MeineSchriftGrosseNorm { get; set; }
        public int MeineSchriftGrosseKlein { get; set; }
        public int MeineSchriftGrosseGross { get; set; }
        public string MeinIconRauf { get; set; }
        public string MeinIconRunter { get; set; }

        public RandomCenterVM()
        {            

            if (!IsInDesignMode)
            {
                ZuruckBtnString = "<";
                ToolTipSonderzeichen = "Sonderzeichen => '#,§,!,$,%,&,?'";
                PWLange = Properties.Settings.Default.PWLange.ToString();
                MinZahlen = Properties.Settings.Default.PWMinZahl.ToString();
                MinGross = Properties.Settings.Default.PWMinGross.ToString();
                MinSonder = Properties.Settings.Default.PWMinSonder.ToString();

                _langeRuntBtn = new RelayCommand(PWLangRunterGedruckt);
                _langeRaufBtn = new RelayCommand(PWLangRaufGedruckt);
                _zahlenRaufBtn = new RelayCommand(PWZahlenRaufGedruckt);
                _zahlenRuntBtn = new RelayCommand(PWZahlenRuntGedruckt);
                _grossRaufBtn = new RelayCommand(PWGrossRaufGedruckt);
                _grossRuntBtn = new RelayCommand(PWGrossRuntGedruckt);
                _sonderRaufBtn = new RelayCommand(PWSonderRaufGedruckt);
                _sonderRuntBtn = new RelayCommand(PWSonderRuntGedruckt);
                _zuruckBtn = new RelayCommand(ZuruckGedruckt);

                SetzeNeueZahlenErlaubnis(Properties.Settings.Default.PWZahlBool);
                SetzeNeueGrossErlaubnis(Properties.Settings.Default.PWGrossBool);
                SetzeNeueSonderErlaubnis(Properties.Settings.Default.PWSonderBool);
                SkinWechsel();
            }
            else
            {
                MeinHintergrund = SkinFarben.DMHinterGrund;
                MeineSchriftFarbe = SkinFarben.DMSchriftFarbe1;
                PWLange = "99";
                MinZahlen = "99";
                MinSonder = "99";
                MinGross = "99";
            }
        }

        private void PWLangRaufGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (Properties.Settings.Default.PWLange < 30)
            {
                Properties.Settings.Default.PWLange++;
                Properties.Settings.Default.Save();
                PWLange = Properties.Settings.Default.PWLange.ToString();
            }
            InArbeit = false;
        }
        private void PWLangRunterGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (Properties.Settings.Default.PWLange > 4 && DerzeitgerMinStand() < Properties.Settings.Default.PWLange)
            {
                Properties.Settings.Default.PWLange--;
                Properties.Settings.Default.Save();
                PWLange = Properties.Settings.Default.PWLange.ToString();
            }
            InArbeit = false;
        }
        private void PWGrossRaufGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (DerzeitgerMinStand() < Properties.Settings.Default.PWLange)
            {
                Properties.Settings.Default.PWMinGross++;
                Properties.Settings.Default.Save();
                MinGross = Properties.Settings.Default.PWMinGross.ToString();
            }
            InArbeit = false;
        }
        private void PWGrossRuntGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (Properties.Settings.Default.PWMinGross > 0)
            {
                Properties.Settings.Default.PWMinGross--;
                Properties.Settings.Default.Save();
                MinGross = Properties.Settings.Default.PWMinGross.ToString();
            }
            InArbeit = false;
        }
        private void PWSonderRaufGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (DerzeitgerMinStand() < Properties.Settings.Default.PWLange)
            {
                Properties.Settings.Default.PWMinSonder++;
                Properties.Settings.Default.Save();
                MinSonder = Properties.Settings.Default.PWMinSonder.ToString();
            }
            InArbeit = false;
        }
        private void PWSonderRuntGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (Properties.Settings.Default.PWMinSonder > 0)
            {
                Properties.Settings.Default.PWMinSonder--;
                Properties.Settings.Default.Save();
                MinSonder = Properties.Settings.Default.PWMinSonder.ToString();
            }
            InArbeit = false;
        }
        private void PWZahlenRaufGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (DerzeitgerMinStand() < Properties.Settings.Default.PWLange)
            {
                Properties.Settings.Default.PWMinZahl++;
                Properties.Settings.Default.Save();
                MinZahlen = Properties.Settings.Default.PWMinZahl.ToString();
            }
            InArbeit = false;
        }
        private void PWZahlenRuntGedruckt()
        {
            if (InArbeit)
            {
                return;
            }
            else
            {
                InArbeit = true;
            }
            if (Properties.Settings.Default.PWMinZahl > 0)
            {
                Properties.Settings.Default.PWMinZahl--;
                Properties.Settings.Default.Save();
                MinZahlen = Properties.Settings.Default.PWMinZahl.ToString();
            }
            InArbeit = false;
        }

        private void SetzeNeueZahlenErlaubnis(bool NeuerWert)
        {
            Properties.Settings.Default.PWZahlBool = NeuerWert;
            Properties.Settings.Default.Save();
            if (NeuerWert)
            {
                MinZahlenErlaubt = true;
                PWZahlenVisi = Visibility.Visible;
                if (DerzeitgerMinStand() > Properties.Settings.Default.PWLange)
                {
                    Properties.Settings.Default.PWMinZahl = 0;
                    Properties.Settings.Default.Save();
                    MinZahlen = Properties.Settings.Default.PWMinZahl.ToString();
                }
            }
            else
            {
                MinZahlenErlaubt = false;
                PWZahlenVisi = Visibility.Hidden;
            }
        }

        private void SetzeNeueSonderErlaubnis(bool NeuerWert)
        {
            Properties.Settings.Default.PWSonderBool = NeuerWert;
            Properties.Settings.Default.Save();
            if (NeuerWert)
            {
                MinSonderErlaubt = true;
                PWSonderVisi = Visibility.Visible;
                if (DerzeitgerMinStand() > Properties.Settings.Default.PWLange)
                {
                    Properties.Settings.Default.PWMinSonder = 0;
                    Properties.Settings.Default.Save();
                    MinSonder = Properties.Settings.Default.PWMinSonder.ToString();
                }
            }
            else
            {
                MinSonderErlaubt = false;
                PWSonderVisi = Visibility.Hidden;
            }
        }

        private void SetzeNeueGrossErlaubnis(bool NeuerWert)
        {
            Properties.Settings.Default.PWGrossBool = NeuerWert;
            Properties.Settings.Default.Save();
            if (NeuerWert)
            {
                MinGrossErlaubt = true;
                PWGrossVisi = Visibility.Visible;
                if (DerzeitgerMinStand() > Properties.Settings.Default.PWLange)
                {
                    Properties.Settings.Default.PWMinGross = 0;
                    Properties.Settings.Default.Save();
                    MinGross = Properties.Settings.Default.PWMinGross.ToString();
                }

            }
            else
            {
                MinGrossErlaubt = false;
                PWGrossVisi = Visibility.Hidden;
            }
        }


        private int DerzeitgerMinStand()
        {
            int tmpStand = 0;
            if (Properties.Settings.Default.PWGrossBool)
            {
                tmpStand += Properties.Settings.Default.PWMinGross;
            }
            if (Properties.Settings.Default.PWZahlBool)
            {
                tmpStand += Properties.Settings.Default.PWMinZahl;
            }
            if (Properties.Settings.Default.PWSonderBool)
            {
                tmpStand += Properties.Settings.Default.PWMinSonder;
            }
            return tmpStand;
        }

        public void SkinWechsel()
        {
            switch (Properties.Settings.Default.AktuellerSkin)
            {
                case 1:
                    MeinHintergrund = SkinFarben.DMHinterGrund;
                    MeineSchriftFarbe = SkinFarben.DMSchriftFarbe1;
                    MeineKontrastFarbe1 = SkinFarben.DMKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.DMKontrastFarbe2;
                    MeineSchriftArt = SkinFarben.DMSchriftArtFett;
                    MeineSchriftGrosseNorm = SkinFarben.DMSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.DMSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.DMschriftGrosseGross;
                    MeinIconRauf = SkinFarben.DMPfeilUp;
                    MeinIconRunter = SkinFarben.DMPfeilDown;
                    break;
                case 0:
                    MeinHintergrund = SkinFarben.NormHinterGrund;
                    MeineSchriftFarbe = SkinFarben.NormSchriftFarbe1;
                    MeineKontrastFarbe1 = SkinFarben.NormaleKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.NormaleKontrastFarbe2;
                    MeineSchriftArt = SkinFarben.NormalSchriftArtNorm;
                    MeineSchriftGrosseNorm = SkinFarben.NormalSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.NormalSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.NormalSchriftGrosseGross;
                    MeinIconRauf = SkinFarben.NormPfeilUp;
                    MeinIconRunter = SkinFarben.NormPfeilDown;
                    break;
                default:
                    MeinHintergrund = SkinFarben.DMHinterGrund;
                    MeineSchriftFarbe = SkinFarben.DMSchriftFarbe1;
                    MeineKontrastFarbe1 = SkinFarben.DMKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.DMKontrastFarbe2;
                    MeineSchriftArt = SkinFarben.DMSchriftArtFett;
                    MeineSchriftGrosseNorm = SkinFarben.DMSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.DMSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.DMschriftGrosseGross;
                    MeinIconRauf = SkinFarben.DMPfeilUp;
                    MeinIconRunter = SkinFarben.DMPfeilDown;
                    break;
            }
        }

        private void ZuruckGedruckt()
        {
            MessengerInstance.Send(new FensterCloseMess("Ui.Pw.Ui.RandomCenterFenster"));
        }
    }
}
