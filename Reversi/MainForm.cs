using System;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{

    public class MainForm : Form
    {
        // We hebben wat members nodig om aan te kunnen passen.

        int xstart = 30;        // De x-positie van de linkerbovenhoek van het speelbord
        int ystart = 150;       // De y-positie van de linkerbovenhoek van het speelbord
        int diam = 30;          // De diameter van de rondjes
        public static GameState gamestate;  // We gebruiken een object van type GameState

        public static bool helpshown = false;
        Color beurtkleur = Color.Blue;      // Blauw begint

        // De volgende knoppen en labels declareren we hier alvast zodat we ze buiten de constructormethode kunnen veranderen
        Button newgameb, helpb;        
        public static Label beurtl, roodl, blauwl;

        // Het hart van het programma, we roepen de constructormethode aan van MainForm en maken de knoppen mooier
        static void Main()
        {
            Application.Run(new MainForm());
        }

        // De constructormethode
        public MainForm()
        {
            DoubleBuffered = true;
            this.Text = "Reversi";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Verstop de form, roep een NewGameForm aan om aan de user de gewenste grootte te vragen
            this.Hide();
            Form nieuwspelform = new NewGameForm(6,6); 
            nieuwspelform.ShowDialog();

            if (NewGameForm.buttonexit == false)
                return; // Als de user die form wegklikt (ipv nieuwspel-knop), stop

            // De volgende knoppen en labels worden gemaakt met behulp van een methode uit FormFuncties
            newgameb = new Button();    // De nieuw-spel-knop
            FormFuncties.NewControl<Button>(ref newgameb, new Point(10, 20), new Size(70, 20), "Nieuw Spel");
            this.Controls.Add(newgameb);

            helpb = new Button();       // De help-knop
            FormFuncties.NewControl<Button>(ref helpb, new Point(85, 20), new Size(40, 20), "Help");
            this.Controls.Add(helpb);

            roodl = new Label();        // De label die aangeeft hoeveel rode stenen op het veld liggen
            FormFuncties.NewControl<Label>(ref roodl, new Point(80, 60), new Size(60, 20), gamestate.RoodN + " stenen");
            roodl.ForeColor = Color.Red;
            this.Controls.Add(roodl);

            blauwl = new Label();       // De label die aangeeft hoeveel blauwe stenen op het veld liggen
            FormFuncties.NewControl<Label>(ref blauwl, new Point(80, 90), new Size(60, 20), gamestate.BlauwN + " stenen");
            blauwl.ForeColor = Color.Blue;
            this.Controls.Add(blauwl);

            beurtl = new Label();       // De label die aangieft wiens beurt het is
            FormFuncties.NewControl<Label>(ref beurtl, new Point(30, 110), new Size(160, 40), "Blauw is aan zet");
            beurtl.ForeColor = beurtkleur;
            beurtl.Font = new Font("Arial", 12);
            this.Controls.Add(beurtl);

            this.newgameb.Click += nieuwspel;
            this.helpb.Click += togglehelp;
            this.MouseClick += klik;
            this.Paint += teken;

            gamestate.Initialise();
        }

        // Simpel, dit togglet of de mogelijke zetten moeten worden laten zien of niet
        private void togglehelp(object o, EventArgs mea)
        {
            helpshown = !helpshown;
            gamestate = MinMax.GeefBesteMove(ref gamestate, 4);
            this.Invalidate();
        }


        // Als de user een nieuw spel wil starten, maken we een nieuwe NewGameForm. Hierin kan de user de gewenste grootte aanpassen
        public void nieuwspel(object o, EventArgs mea)
        {
            Form nieuwspel = new NewGameForm(gamestate.SizeX, gamestate.SizeY);
            nieuwspel.ShowDialog();
            this.Invalidate();
        }

        // Dit is waar het speelveld wordt getekend
        // We tekenen het veld en alle stenen in dezelfde dubbele for-loop. (het klinkt inefficiënt om iedere
        // keer het bord en alle stenen opnieuw te tekenen, maar het kost zo weinig rekenkracht dat het (bijna) niet merkbaar is,
        // zeker met DoubleBuffered = true;. Daarnaast kost dit het minste regels code)
        private void teken(object o, PaintEventArgs pea)
        {
            // De grootte van de form wordt aangepast aan grootte van spel
            this.ClientSize = new Size(120 + diam * gamestate.SizeX, 180 + diam * gamestate.SizeY);

            Graphics g = pea.Graphics;
            g.FillEllipse(Brushes.Red, 45, 50, diam, diam);      // Dit is om het er leuker
            g.FillEllipse(Brushes.Blue, 45, 80, diam, diam);     // uit te laten zien

            // In de volgende forloops lopen we ieder vakje na om te tekenen én om te tellen.
            gamestate.RoodN = 0; gamestate.BlauwN = 0; 
            for (int x = 0; x < gamestate.SizeX; x++)
            {
                for (int y = 0; y < gamestate.SizeY; y++)
                {
                    // Nu zijn we op plek (x,y) op het bord.
                    Rectangle rect = new Rectangle(xstart + diam * x, ystart + diam * y, diam, diam);
                    Rectangle rectsmall = new Rectangle(xstart + diam * x + 3, ystart + diam * y + 3, diam - 6, diam - 6);
                    pea.Graphics.DrawRectangle(Pens.Black, rect);

                    // Nu tekenen we het bord afhankelijk van de gamestate (0 = leeg, 1 = blauw, -1 = rood)
                    if (gamestate.Read(x, y) == 0) { }
                    else if (gamestate.Read(x, y) == 1) { g.FillEllipse(Brushes.Blue, rect); gamestate.BlauwN++; }  // teken blauw en tel één op bij BlauwN
                    else if (gamestate.Read(x, y) == -1) { g.FillEllipse(Brushes.Red, rect); gamestate.RoodN++; }   // teken rood en tel één op bij RoodN
                    else new ErrorForm("Deze melding zou nooit zichtbaar moeten zijn, gamestate moet altijd -1, 0 of 1 zijn");

                    // Nu gaan we ook afhankelijk van of helpshown == true hier tekenen als dit vakje een mogelijke zet is.
                    if (ReversiFuncties.MagMove(gamestate, x, y) && gamestate.WiensBeurt == 1 && helpshown) { g.FillEllipse(Brushes.LightSkyBlue, rect); }
                    else if (ReversiFuncties.MagMove(gamestate, x, y) && gamestate.WiensBeurt == -1 && helpshown) { g.FillEllipse(Brushes.LightSalmon, rect); }
                }
            }


            roodl.Text = gamestate.RoodN + " stenen";
            blauwl.Text = gamestate.BlauwN + " stenen";

            if (gamestate.LastTurn)
            {
                if (gamestate.BlauwN > gamestate.RoodN)
                { beurtl.Text = "Blauw heeft gewonnen!"; beurtl.ForeColor = Color.Blue; }
                else if (gamestate.BlauwN < gamestate.RoodN)
                { beurtl.Text = "Rood heeft gewonnen!"; beurtl.ForeColor = Color.Red; }
                else
                { beurtl.Text = "Remise!"; beurtl.ForeColor = Color.Black; }
                return;
            }

            // Als laatste wordt nog even de tekst van wie er aan de beurt is omgewisseld. (als gamestate.LastTurn == false)
            if (gamestate.WiensBeurt == 1) { beurtl.Text = "Blauw is aan zet"; beurtl.ForeColor = Color.Blue; }
            else { beurtl.Text = "Rood is aan zet"; beurtl.ForeColor = Color.Red; }
        }

        // Deze methode tovert een klik op de form om in een klik op plek (x,y) in het speelveld
        // Dit doen we door de array af te gaan en te checken of de muis in die bepaalde plek zit
        private void klik(object o, MouseEventArgs mea)
        {
            for (int x = 0; x < gamestate.SizeX; x++)
            {
                for (int y = 0; y < gamestate.SizeY; y++)
                {
                    // De volgende conditie vraagt of de muis zich in vakje (x,y) bevindt.
                    if (mea.X > xstart + diam * x && mea.X <= xstart + diam * (x + 1) && mea.Y > ystart + diam * y && mea.Y <= ystart + diam * (y + 1))
                    {
                        // Als we hier zijn, hebben we het vakje gevonden waar de muis klikte!
                        if (gamestate.Read(x, y) == 1 || gamestate.Read(x, y) == -1)
                            return; // Doe niets als er al een bolletje in dat vakje zit

                        if (ReversiFuncties.MagMove(gamestate, x, y)) // Als de zet geldig is, ga de gamestate dan updaten
                        {
                            ReversiFuncties.UpdateGamestate(ref gamestate, x, y);
                            this.Invalidate();

                            while (NewGameForm.vsai == true && gamestate.WiensBeurt == -1 && gamestate.LastTurn == false)
                            {
                                gamestate = MinMax.GeefBesteMove(ref gamestate, 4);
                                this.Invalidate();
                            }
                        }
                        else
                            return;             // Zo niet, doe niets.
                    }
                }
            }


        }

    }

    // Met deze hulpklasse kunnen we de locatie, grootte, en tekst van Buttons, Labels en Textboxes in één regel opgeven.
    // Dit kan door die <T> en ref T. (where T : Control zorgt ervoor dat T wel een Control moet zijn (zoals een Button))
    public class FormFuncties
    {
        public static void NewControl<T>(ref T control, Point loc, Size size, string tekst) where T : Control
        {
            control.Location = loc;
            control.Size = size;
            control.Text = tekst;
        }
    }
}
