using System;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    // Dit is de form die de user vraagt om de gewenste nieuwe grootte van het spel
    public class NewGameForm : Form
    {
        public static bool vsai = false;

        // Als de user de applicatie start, maar weer sluit voordat hij een waarde heeft opgegeven,
        // zou het programma normaal crashen. Om dat te voorkomen is deze bool toegevoegd
        public static bool buttonexit = false;

        // We hebben weer wat members nodig om aan te kunnen passen
        TextBox gamesizexbox, gamesizeybox;
        Label gamesizexl, gamesizeyl, tegenail;
        Button nieuwspel;
        CheckBox tegenai;

        public NewGameForm(int xsize, int ysize)
        {
            this.Text = "Nieuw Spel!";
            this.ClientSize = new Size(380, 80);

            tegenai = new CheckBox();       // De checkbox om aan te vinken of je tegen de AI wil spelen of niet
            tegenai.Location = new Point(240,50);
            tegenai.Size = new Size(15, 15);
            this.Controls.Add(tegenai);

            // Hier volgen de initialisaties van de Buttons, Labels en Textboxes.
            gamesizexl = new Label();       // De label voor hoeveel kolommen de user wil
            FormFuncties.NewControl<Label>(ref gamesizexl, new Point(20, 10), new Size(200, 20), "Hoeveel kolommen volgend spel?");
            this.Controls.Add(gamesizexl);

            gamesizeyl = new Label();       // De label voor hoeveel rijen de user wil
            FormFuncties.NewControl<Label>(ref gamesizeyl, new Point(20, 30), new Size(200, 20), "Hoeveel rijen volgend spel?");
            this.Controls.Add(gamesizeyl);

            tegenail = new Label();         // De label die vraagt of de user tegen de computer wil spelen
            FormFuncties.NewControl<Label>(ref tegenail, new Point(20, 50), new Size(220, 20), "Wil je dit potje tegen de computer spelen?");
            this.Controls.Add(tegenail);

            gamesizexbox = new TextBox();   // De textbox waar kan worden ingevoerd hoeveel kolommen de user wil
            FormFuncties.NewControl<TextBox>(ref gamesizexbox, new Point(240, 7), new Size(40, 20), Convert.ToString(xsize));
            this.Controls.Add(gamesizexbox);

            gamesizeybox = new TextBox();   // De textbox waar kan worden ingevoerd hoeveel rijen de user wil
            FormFuncties.NewControl<TextBox>(ref gamesizeybox, new Point(240, 27), new Size(40, 20), Convert.ToString(ysize));
            this.Controls.Add(gamesizeybox);

            nieuwspel = new Button();       // En als laatst de knop om het nieuwe spel te starten
            FormFuncties.NewControl<Button>(ref nieuwspel, new Point(290, 17), new Size(80, 40), "Nieuw Spel");
            this.Controls.Add(nieuwspel);


            this.nieuwspel.Click += newgame;
            this.FormClosed += stopform;
        }

        // In deze methode wordt het maken van een nieuw spel uitgevoerd
        public void newgame(object o, EventArgs mea)
        {
            int gamesizex;
            int gamesizey;

            try
            {   // Als de user geen geldige getallen invoert, dan geven we een foutmelding
                gamesizex = int.Parse(gamesizexbox.Text);
                gamesizey = int.Parse(gamesizeybox.Text);
            }
            catch
            {
                new ErrorForm("Voer a.u.b. gehele getallen in");
                return;
            }

            // Een te klein (of groot) bord is niet speelbaar
            if (gamesizex < 3 || gamesizey < 3 || gamesizex > 30 || gamesizey > 30)
            {
                new ErrorForm("Voer a.u.b. getallen groter dan 3 en kleiner dan 31 in");
                return;
            }

            vsai = tegenai.Checked;

            // De gamestate wordt hier geïnitialiseerd
            MainForm.gamestate = new GameState(gamesizex, gamesizey);
            MainForm.gamestate.Initialise();

            buttonexit = true;  // De user heeft op de nieuw-spel-knop gedrukt
            this.Close(); // Deze hulpform heeft zijn taak volbracht, en wordt nu gesloten
        }

        // Deze methode stopt deze applicatie als de user op kruisje heeft gedrukt ipv de nieuw-spel-knop
        private void stopform(object sender, FormClosedEventArgs e)
        {
            if (!buttonexit)
            {
                Application.Exit();
            }

        }
    }
}
