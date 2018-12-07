using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    // Met deze klasse maken we een errorForm aan
    class ErrorForm : Form
    {   
        // De label en string die de foutmelding moet weergeven
        Label tekst;                
        string foutmeldingtekst;

        // De constructormethode, met een parameter voor de foutmeldingtekst
        public ErrorForm(string s)
        {
            foutmeldingtekst = s;
            this.Text = "Foutmelding!";
            this.ClientSize = new Size(400, 100);
            this.Paint += teken;
            this.ShowDialog();  // We willen dat de foutmelding wordt weergegeven totdat de user dit venster wegklikt
        }

        // De tekenmethode, hier gebruiken we weer de NewControl methode uit de FormFuncties om de label te maken
        public void teken(object o, PaintEventArgs pea)
        {
            tekst = new Label();
            FormFuncties.NewControl<Label>(ref tekst, new Point(60, 40), new Size(360, 50), foutmeldingtekst);
            tekst.Font = new Font("Arial", 15);
            Controls.Add(tekst);

            // En daarnaast tekenen we een uitroepteken, en spelen het foutmeldingsgeluid af
            pea.Graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(20, 30), new Size(10, 40)));
            pea.Graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(20, 75), new Size(10, 10)));
            System.Media.SystemSounds.Hand.Play();
        }

    }
}
