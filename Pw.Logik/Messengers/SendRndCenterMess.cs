using System.Windows;

namespace Logik.Pw.Logik.Messengers
{
    public class SendRndCenterMess
    {
        public WindowStartupLocation FensterPosition { get; set; }
        public int PositionLeft { get; set; }
        public int PositionTop { get; set; }

        public SendRndCenterMess(WindowStartupLocation FensterPosition, int PosiLeft, int PosiTop)
        {
            this.FensterPosition = FensterPosition;
            this.PositionLeft = PosiLeft;
            this.PositionTop = PositionTop;
        }
    }
}
