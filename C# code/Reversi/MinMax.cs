using System;
using System.Threading.Tasks;

namespace Reversi
{
    // In deze klasse is de AI geschreven, die probeert de optimale zet te spelen door minimax toe te passen
    public class MinMax
    {
        // Deze klasse geeft de boom van alle moves bij een bepaalde gamestate (en gevraagde boomdiepte)
        public static Tree GeefBoom(GameState gamestate, int reqdepth)
        {
            Tree boom = new Tree(reqdepth);

            // We maken de stam van de boom en verwijderen de nulls
            boom.Add(0, -1, gamestate);
            boom.RemoveNull(0);

            gamestate.SwitchBeurt();
            
            // Nu maken we alle levels van de boom
            for (int p = 1; p < reqdepth; p++)
            {
                // Voor ieder punt in het vorige level maken we de volgende level van de boom
                for (int i = 0; i < boom.index[p - 1]; i++)
                {                   
                        GameState[] statearr;

                        gamestate.statearray = boom.ReadMember(p - 1, i).MemberState.statearray;
                        gamestate.WiensBeurt = boom.ReadMember(p - 1, i).MemberState.WiensBeurt;

                        // En dan maken we de array met alle mogelijke moves op dit punt en gooien het in de boom
                        statearr = MogelijkeMoves(gamestate);

                        for (int j = 0; j < statearr.Length; j++)
                            boom.Add(p, i, statearr[j]);

                        boom.ReadMember(p - 1, i).RemoveNull();
                }

                // Iedere level van de boom verandert de beurt natuurlijk
                gamestate.SwitchBeurt();

                boom.RemoveNull(p);
            }

            for (int i = 0; i < boom.index[reqdepth - 1]; i++)
                boom.ReadMember(reqdepth - 1, i).RemoveNull();

            return boom;
        }

        // Deze methode gebruiken we buiten deze klasse om de beste move (die de computer kan vinden) te zetten
        // Dit doen we door de boom te maken van diepte depth vanaf de huidige gamestate en dan te scoren.
        // De beste move wordt dan als nieuwe gamestate teruggegeven
        public static GameState GeefBesteMove(ref GameState gamestate, int depth)
        {
            Tree boom = GeefBoom(gamestate, depth);
            GameState state = BestMove(boom, gamestate.WiensBeurt);

            for (int i = 0; i < gamestate.SizeX; i++)
                for (int j = 0; j < gamestate.SizeY; j++)
                    if (gamestate.statearray[i, j] != state.statearray[i, j] && gamestate.statearray[i, j] == 0)
                    {
                            ReversiFuncties.UpdateGamestate(ref gamestate, i, j);
                            return gamestate;
                    }

            return state;
        }

        // Methode om recursief de score te bepalen van een node uit de gamestate boom m.b.v. het MinMax algoritme met alpha/beta prunning.
        public static int ScoreBoom(Tree boom, int index, int level, int alpha, int beta, int speler) 
        {
           // Als de score opgevraagd wordt van een node die geen kinderen heeft is dit blijkaar het einde van een bepaalde tak en wordt de waarde terugegeven van die bepaalde gamestate.
           // De score van een gamestate is gedefinieerd als het aantal stenen van de speler min het aantal stenen van de tegenstander.
           if (boom.ReadMember(level, index).Children.Length == 0 )
                if (boom.ReadMember(0,0).MemberState.WiensBeurt == 1)
                    return boom.ReadMember(level, index).MemberState.BlauwN - boom.ReadMember(level, index).MemberState.RoodN;
                else
                    return boom.ReadMember(level, index).MemberState.RoodN - boom.ReadMember(level, index).MemberState.BlauwN;

           // Voor iedere node wordt gekeken naar zijn kinderen om de score te bepalen, het doorgeven van deze scores gebeurt aan de hand van het MinMax algoritme met alhpha beta prunning.
           // In het MinMax algoritme wordt gekeken naar de maximaal mogelijke score voor de beurten van de computer en naar de minimale score in beurten van de speler.
           // Alpha beta prunning wordt gebruikt om voordat een bepaalde tak terminate te zien dat die tak geen max/min op gaat leveren en dus dat die tak niet verder geevalueerd hoeft te worden.
            if  (boom.ReadMember(0,0).MemberState.WiensBeurt * speler == 1)
            {
                int score = -10000;

                for (int i = 0; i < boom.ReadMember(level, index).Children.Length; i++)
                {                
                    score = ScoreBoom(boom, boom.ReadMember(level, index).Children[i], level + 1, alpha, beta, -speler); // kijk naar de score van de kinderen van een node.
                    alpha = Math.Max(alpha, score);

                    if (beta <= alpha) // Alpha beta prunning
                        break;
                }

                return score;
            }
            else
            {
                int score = 10000;

                for (int i = 0; i < boom.ReadMember(level, index).Children.Length; i++)
                {
                    score = ScoreBoom(boom, boom.ReadMember(level, index).Children[i], level + 1, alpha, beta, -speler);
                    beta = Math.Min(beta, score);

                    if (beta <= alpha)
                        break;
                }

                return score;
            }
        }

        // Deze methode bepaalt de beste zet uit de boom (door de vernieuwde gamestate te gebruiken)
        public static GameState BestMove(Tree boom,  int speler)
        {
            int min = 0;
            int index = 0;
            GameState bestmove;
            int depth = boom.reqdepth;

            // Hier vinden we de tak met de hoogste score en returnen die gamestate
            for (int i = 0; i < boom.index[1]; i++)
            {
                if (boom.ReadMember(1, i).Score < min)
                {
                    min = ScoreBoom(boom, i, 1, -10000, 10000, speler);
                    index = i;
                }
            }

            bestmove = boom.ReadMember(1, index).MemberState;

            return bestmove;
        }
        
        // Deze methode geeft alle mogelijke zetten gegeven een array van toekomstige gamestates, gegeven de huidige gamestate
        public static GameState[] MogelijkeMoves(GameState gamestate)
        {
            // Dit doen we door een array te maken en op iedere mogelijke plek een zet te proberen, en als een zet mag,
            // dan voegen we de vernieuwde gamestate toe aan gamearr
            GameState[] gamearr = new GameState[gamestate.SizeX * gamestate.SizeY];

            int k = 0;

            for (int i = 0; i < gamestate.SizeX; i++)
            {
                for (int j = 0; j < gamestate.SizeY; j++)
                {
                    if (ReversiFuncties.MagMove(gamestate, i, j))
                    {
                        GameState gamevar = new GameState(gamestate.SizeX, gamestate.SizeY);

                        gamearr[k] = new GameState(gamestate.SizeX, gamestate.SizeY);

                        gamevar.statearray = gamestate.statearray;
                        gamevar.WiensBeurt = gamestate.WiensBeurt;

                        ReversiFuncties.UpdateGamestate(ref gamevar, i, j);
                        gamearr[k] = gamevar;

                        k++;
                    }
                }
            }

            if (k == 0)
            {
                GameState[] arr = new GameState[1];
                arr[0] = new GameState(gamestate.SizeX, gamestate.SizeY);
                return arr;
            }

           
            // Nu zijn er nog een heleboel plekken in de gamearr niet geïnitialiseerd (omdat daar geen move mogelijk was)
            // Dus die verwijderen we en maken een nieuwe array aan met de juiste grootte
            int l = 0;

            while (gamearr[l].statearray != null)
                l++;

            GameState[] gamearrcopy = gamearr; // Normaal zou dit een reference maken, maar dit is voorkomen door de set-minimethode in de struct Gamestate

            gamearr = new GameState[l];

            for (int i = 0; i < k; i++)
                gamearr[i] = gamearrcopy[i];
                
            return gamearr;
        }
    }

    // Deze klasse bevat de boom die het algoritme afgaat
    public class Tree
    {
        public int reqdepth;
        private TreeLevel[] boom;
        public int[] index;

        // De constructormethode, deze maakt dus de boom aan van de gewenste grootte
        public Tree(int reqdepth)
        {
            this.reqdepth = reqdepth;
            this.index = new int[reqdepth];
            this.index[0] = 0;

            this.boom = new TreeLevel[reqdepth];
        }

        // Deze methode maakt een nieuwe level aan
        public void CreateLevel(int level, int levelsize)
        {
            boom[level] = new TreeLevel(level, levelsize);
        }

        // Met deze methode maken we een nieuwe "tak" aan in onze boom
        public void Add(int level, int parent, GameState gamestate)
        {
            if (index[level] == 0)
                if (level == 0)
                    CreateLevel(0, gamestate.SizeX * gamestate.SizeY);
                else    
                    CreateLevel(level, index[level - 1] * (gamestate.SizeX * gamestate.SizeY));

            TreeMember member = new TreeMember(gamestate, parent);

            boom[level].Add(member);

            if (level != 0)
            {
                boom[level - 1].members[parent].AddChild(index[level]);
            }

            index[level]++;
        }

        // Deze methode geeft een punt in de boom
        public TreeMember ReadMember(int level, int index)
        {
            return boom[level].Read(index);
        }

        // Met deze methode verwijderen we alle nulls uit de boom op punten op diepte "level"
        public void RemoveNull(int level)
        {
            boom[level].RemoveNull();
        }    
    }

    // Ieder punt in de boom noemen we een TreeMember, en iedere TreeMember heeft een Parent (vorige gamestate)
    public class TreeMember
    {
        public int Parent;
        public GameState MemberState;
        public int Score;
        public int[] Children;

        // De constructormethode waarmee we een TreeMember aanmaken
        public TreeMember(GameState gamestate, int parent)
        {
            this.MemberState = gamestate;
            this.Parent = parent;
            this.Score = -1;
            this.Children = new int[gamestate.SizeX * gamestate.SizeY];

            for (int i = 0; i < Children.Length; i++)
                Children[i] = -1;          
        }

        public void RemoveNull()
        {
            int k = 0;

            while (this.Children[k] != -1)
                k++;

            int[] arrcopy = this.Children;

            this.Children = new int[k];

            for (int i = 0; i < k; i++)
                this.Children[i] = arrcopy[i];
        }

        public void AddChild(int index)
        {
            int k = 0;

            while (this.Children[k] != -1) { k++; }

            Children[k] = index;
        }
    }

    // We moeten ook bijhouden waar we zijn in de boom, dit doen we met een TreeLevel class
    public class TreeLevel
    {
        public int level;
        public TreeMember[] members;

        // De constructormethode, die vraagt op welke level je zit en hoeveel takken er op dat level zitten
        public TreeLevel(int level, int levelsize)
        {
            this.level = level;
            this.members = new TreeMember[levelsize];
        }

        // Met deze methode kunnen we punten op dit level toevoegen
        public void Add(TreeMember member)
        {
            int k = 0;           

            while (members[k] != null) { k++; }

            members[k] = member;
        }

        // Hiermee kunnen we kijken hoeveel takken er aan het punt zit die zit op plek "index"
        public TreeMember Read(int index)
        {
            return members[index];
        }

        // Met deze methode verwijderen we alle nulls in de array van takken vanaf huidig punt
        public void RemoveNull()
        {
            int k = 0;

            while (this.members[k] != null)
                k++;

            TreeMember[] membercopy = members;

            this.members = new TreeMember[k];

            for (int i = 0; i < k; i++)
                this.members[i] = membercopy[i];     
        }
    }
}