using Logik.Pw.Logik.Klassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Messengers
{
    public class SendImportMess
    {
        public enum ImpMoglichkeit { SyncNeuAnderer, DirektAnderer, Sync, NeuAnlage, PwAndern, BenutzerDel }
        public ImpMoglichkeit Anzeige;
        public Person ImportPerson;
        public PersonCenter Center;
        public Person AktEingeloggt;
        public SecureString AktPw;

        public SendImportMess(ImpMoglichkeit Anzeige, Person ImportPerson, PersonCenter Center, Person AktEingeloggt = null, SecureString AktPw = null)
        {
            this.Anzeige = Anzeige;
            this.ImportPerson = ImportPerson;
            this.Center = Center;
            this.AktEingeloggt = AktEingeloggt;
            this.AktPw = AktPw;
        }
    }
}
