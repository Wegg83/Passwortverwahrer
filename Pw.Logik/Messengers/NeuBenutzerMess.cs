using Logik.Pw.Logik.Klassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Messengers
{
    public class NeuBenutzerMess
    {
        public Person NeuePersondaten;
        public SecureString PwEintrag;

        public NeuBenutzerMess(Person NeuePersondaten, SecureString PwEintrag)
        {
            this.NeuePersondaten = NeuePersondaten;
            this.PwEintrag = PwEintrag;
        }
    }
}
