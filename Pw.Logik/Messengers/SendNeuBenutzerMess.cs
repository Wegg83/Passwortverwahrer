using Logik.Pw.Logik.Klassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Messengers
{
    public class SendNeuBenutzerMess
    {
        public enum Zustand { Neueingabe, PwAnderung };
        public Zustand Auswahl;
        public List<string> VorhandeneNamen;
        public Person ImportPerson;
        public Person GesuchtePerson;

        public string TextFeld1, TextFeld2, TextFeld3;

        public SendNeuBenutzerMess(Zustand Auswahl, List<string> VorhandeneNamen = null, Person ImportPerson = null, Person GesuchtePerson = null )
        {
            this.Auswahl = Auswahl;
            this.VorhandeneNamen = VorhandeneNamen;
            this.ImportPerson = ImportPerson;
            this.GesuchtePerson = GesuchtePerson;

            switch (Auswahl)
            {
                case Zustand.Neueingabe:
                    TextFeld1 = "BenutzerName:";
                    TextFeld2 = "Passwort:";
                    TextFeld3 = "Passwort Wdh:";
                        break;
            }
        }
    }
}
