namespace Reversi
{
    // Deze klasse bevat functies die van belang zijn voor het checken van legaliteit van een zet en het updaten van de gamestate
    public class ReversiFuncties
    {
        // Deze methode bepaalt of een zet op plek (x,y) mag, gegeven de gamestate
        public static bool MagMove(GameState gamestate, int x, int y) 
        {
            // Als er al een bolletje zit in dat vakje, kun je daar natuurlijk geen zet doen
            if (gamestate.Read(x, y) != 0)  
                return false;  

            // Nu checken we in alle 8 richtingen of een zet geldig is. als dat zo is in tenminste één richting, is de zet geldig (return true)
            if (
            MagRichting(gamestate, x, y, -1, -1) ||
            MagRichting(gamestate, x, y, -1, 0) ||
            MagRichting(gamestate, x, y, -1, 1) ||
            MagRichting(gamestate, x, y, 0, -1) ||
            MagRichting(gamestate, x, y, 0, 1) ||
            MagRichting(gamestate, x, y, 1, -1) ||
            MagRichting(gamestate, x, y, 1, 0) ||
            MagRichting(gamestate, x, y, 1, 1)
            ) { return true; } 

            return false; // Anders niet
        }

        // Dit is de methode die bepaalt of een zet geldig is in één bepaalde richting, gegeven de gamestate
        // Hier is x, y de plek waar een poging tot zet gedaan wordt, en rx, ry zijn getallen die de richting bepalen die we op kijken
        public static bool MagRichting(GameState gamestate, int x, int y, int rx, int ry)
        {   
            bool anderekleur = false;  
            // De conditie in de for-loop zorgt ervoor dat we niet buiten het bord lopen
            for (int i = 1; x + i * rx < gamestate.SizeX && x + i * rx >= 0 && y + i * ry < gamestate.SizeY && y + i * ry >= 0; i++)
            {
                if (gamestate.Read(x + i * rx, y + i * ry) == -gamestate.WiensBeurt)
                    anderekleur = true;     // Als er tenminste één is van de andere kleur, wordt deze boolean true
                else if (anderekleur && gamestate.Read(x + i * rx, y + i * ry) == gamestate.WiensBeurt)
                    return true;    // Als er een andere kleur is én die richting wordt afgesloten door een steen van eigen kleur, is de zet geldig
                else break;         // Als er geen andere kleur is of die richting wordt niet afgesloten door een steen van eigen kleur, is de zet niet geldig.
            }
            return false;
        }

        // Deze methode update de gamestate in alle richtingen
        public static void UpdateGamestate(ref GameState gamestate, int x, int y)
        {
            // De volgende dubbele for-loop roept "UpdateRichting" aan in alle 8 richtingen. 
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (!(i == 0 && j == 0))
                        UpdateRichting(ref gamestate, x, y, i, j);

            gamestate.SwitchBeurt();

            // Misschien kan de andere kleur nu geen zet doen. We checken of dat kan, zo niet dan veranderen we de beurt weer.
            for (int i = 0; i < gamestate.SizeX; i++)
                for (int j = 0; j < gamestate.SizeY; j++)
                    if (MagMove(gamestate, i, j))
                        return; // Als er ten minste één geldige zet is, dan zijn we klaar.

            // Bij geen mogelijke zetten doen we de beurt weer terug
            gamestate.SwitchBeurt();

            for (int i = 0; i < gamestate.SizeX; i++)
                for (int j = 0; j < gamestate.SizeY; j++)
                    if (MagMove(gamestate, i, j))
                        return;

            // Als er nu wéér geen geldige zet is, is het spel geëindigd en kijken we naar de score! (zie de teken-methode)
            gamestate.EndGame();
        }

        // Deze methode update de gamestate in één richting, dit gaat analoog aan MagRichting(...), alleen nu updaten we de gamestate!
        public static void UpdateRichting(ref GameState gamestate, int x, int y, int rx, int ry)
        {   
            bool anderekleur = false;
            for (int i = 1; x + i * rx < gamestate.SizeX && x + i * rx >= 0 && y + i * ry < gamestate.SizeY && y + i * ry >= 0; i++)
            {
                if (gamestate.Read(x + i * rx, y + i * ry) == -gamestate.WiensBeurt)
                    anderekleur = true;
                else if (anderekleur && gamestate.Read(x + i * rx, y + i * ry) == gamestate.WiensBeurt)
                {
                    // In plaats van return true, tekenen we alles van (x,y) tot het eindpunt de juiste kleur
                    int kleur = gamestate.WiensBeurt;
                    for (int k = 0; k <= i; k++)
                    {
                        gamestate.Set(x + k * rx, y + k * ry, kleur);
                    }
                    break;
                }
                else break;
            }
        }
    }
}
