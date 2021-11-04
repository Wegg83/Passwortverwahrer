using Logik.Pw.Logik.Klassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Messengers
{
   public class EmpfCenterMess
    {
        public PersonCenter Center;
        public SecureString AktivesProfil;

        public EmpfCenterMess(PersonCenter Center)
        {
            this.Center = Center;
        }
    }
}
