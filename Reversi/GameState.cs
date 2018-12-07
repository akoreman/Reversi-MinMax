namespace Reversi
{
    // Deze struct houdt de status van het huidige spel bij (in de vorm van een 2D array van integers)
    // Hierbij is 0 = leeg, 1 = blauw, -1 = rood. We gebruiken een struct omdat we vele 
    // verschillende gamestate objects willen (niet veel verschillende references) maken voor de AI
    public struct GameState
    {
        public int SizeX, SizeY;    // De gebruiker moet de grootte van het spel kunnen aanpassen
        public int RoodN, BlauwN;   // Dit is om bij te houden hoeveel rode en blauwe stenen er zijn

        public int WiensBeurt;      // 1 = blauw, -1 = rood
        private int[,] _statearray; // Voor intern gebruik
        public int[,] statearray    // Dit is de 2D array die de status van het spel geeft
        {
            get
            {
                return _statearray;
            }
            set
            {   // Set minimethod om te zorgen dat de statearray by value wordt doorgegeven i.p.v. by reference.
                int[,] array = new int[this.SizeX, this.SizeY];

                for (int i = 0; i < this.SizeX; i++)
                    for (int j = 0; j < this.SizeY; j++)
                        array[i, j] = value[i, j];

                _statearray = array;
            }
        }

        public bool LastTurn;       // Op een gegeven moment is het spel afgelopen. Om dan te laten zien wie er gewonnen heeft, moet er iets omklappen. Dat is dit.

        // De constructormethode, waarmee een nieuw speelbord wordt aangevraagd
        public GameState(int sizex, int sizey)
        {
            this._statearray = new int[sizex, sizey];
            //this.statearray = new int[sizex, sizey];
            this.WiensBeurt = 1;
            this.SizeX = sizex;
            this.SizeY = sizey;
            this.RoodN = 0;
            this.BlauwN = 0;
            this.LastTurn = false;
        }

        // Deze methode verandert plek (x,y) in de array in de huidige kleur
        public void Set(int x, int y, int kleur)
        {
            this.statearray[x, y] = kleur;
        }

        // Deze methode vertelt je wat er op plek (x,y) in de array zit
        public int Read(int x, int y)
        {
            return this.statearray[x, y];
        }

        // Deze methode wisselt de beurt om
        public void SwitchBeurt()
        {
            this.WiensBeurt *= -1;
        }

        // Deze methode wordt één keer gedraaid aan het begin van het spel, om het spel te initialiseren
        public void Initialise()
        {
            this.Set(this.SizeX / 2, this.SizeY / 2, 1);
            this.Set(this.SizeX / 2, this.SizeY / 2 - 1, -1);
            this.Set(this.SizeX / 2 - 1, this.SizeY / 2, -1);
            this.Set(this.SizeX / 2 - 1, this.SizeY / 2 - 1, 1);
        }

        // Deze methode wordt aangeroepen om het einde van het spel aan te breken
        public void EndGame()
        {
            this.LastTurn = true;
        }

    }
    
}
