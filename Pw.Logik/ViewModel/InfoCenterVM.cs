using GalaSoft.MvvmLight;
using Logik.Pw.Logik.Klassen;
using System;
using System.Reflection;

namespace Logik.Pw.Logik.ViewModel
{
    public class InfoCenterVM : ViewModelBase
    {
        public string Ersteller {get;set;}
        public string VersionsNummer { get; set; }
        public string InfoUberschrift { get; set; }
        public string KontaktInfo { get; set; }
        public string InfoText1 { get; set; }

        public string MeinHintergrund { get; set; }
        public string MeineSchriftFarbe { get; set; }
        public string MeineKontrastFarbe1 { get; set; }
        public string MeineKontrastFarbe2 { get; set; }
        public string MeineSchriftArt { get; set; }
        public int MeineSchriftGrosseNorm { get; set; }
        public int MeineSchriftGrosseKlein { get; set; }
        public int MeineSchriftGrosseGross { get; set; }

        public InfoCenterVM()
        {
            SkinWechsel();

            //string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
            //                    Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false))
            //                        .Company;
            Ersteller = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
            VersionsNummer = GetAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
            InfoUberschrift = GetAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);
            KontaktInfo = "wolfgang__eggenhofer@gmx.at";
            InfoText1 = "Verwaltet eine Sammlung an Passwörter von verschiedenen Benutzern.";
            
        }

        public string GetAssemblyAttribute<T>(Func<T, string> value)
                    where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
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
                    break;
                case 0:
                    MeinHintergrund = SkinFarben.NormHinterGrund;
                    MeineSchriftFarbe = SkinFarben.NormSchriftFarbe1;
                    MeineKontrastFarbe1 = SkinFarben.NormaleKontrastFarbe1;
                    MeineKontrastFarbe2 = SkinFarben.NormaleKontrastFarbe2;
                    MeineSchriftArt = SkinFarben.NormalSchriftArtNorm;
                    MeineSchriftGrosseNorm = SkinFarben.NormalSchriftGrosseNorm;
                    MeineSchriftGrosseKlein = SkinFarben.NormalSchriftGrosseKlein;
                    MeineSchriftGrosseGross = SkinFarben.NormalSchriftGrosseGross + 10;
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
                    break;
            }
        }
    }
}
