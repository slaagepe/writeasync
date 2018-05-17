//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by GWBas2CS.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.IO;
using Adventure;

internal sealed class adventure
{
    private const int MaxInventoryItems = 5;

    private readonly TextReader input;
    private readonly TextWriter output;

    private int inventoryItems;
    private bool saltPoured;
    private bool formulaPoured;
    private int mixtureCount;
    private bool wearingGloves;

    private Objects objects;
    private Map map;

    public adventure(TextReader input, TextWriter output)
    {
        this.input = (input);
        this.output = (output);
    }

    public void Run()
    {
        while (this.Main() == VerbResult.RestartGame)
        {
        }
    }

    private void CLS()
    {
        this.output.Write('\f');
        Console.Clear();
    }

    private void PRINT(string expression)
    {
        this.output.WriteLine(expression);
    }

    private string INPUT_s(string prompt)
    {
        this.output.Write((prompt) + ("? "));
        string v = this.input.ReadLine();
        return v.Trim();
    }

    private void PRINT_n(string expression)
    {
        this.output.Write(expression);
    }

    private float INPUT_n(string prompt)
    {
        while (true)
        {
            this.output.Write((prompt) + ("? "));
            string v = this.input.ReadLine();
            v = (v.Trim());
            if ((v.Length) == (0))
            {
                return 0;
            }

            float r;
            if (float.TryParse(v, out r))
            {
                return r;
            }

            this.output.WriteLine("?Redo from start");
        }
    }

    private void PrintDirections()
    {
        PRINT_n("YOU CAN GO: ");
        foreach (string dir in map.Directions())
        {
            PRINT_n(dir + " ");
        }

        PRINT("");
    }

    private void PrintObjects()
    {
        PRINT("YOU CAN SEE: ");
        bool atLeastOne = false;
        foreach (string name in objects.Here(map.CurrentRoom))
        {
            PRINT(" " + name);
            atLeastOne = true;
        }

        if (!atLeastOne)
        {
            PRINT(" NOTHING OF INTEREST");
        }
    }

    private void PrintDescription()
    {
        PRINT("");
        PRINT("YOU ARE " + map.Describe());
    }

    private void PrintIntro()
    {
        PRINT("ALL YOUR LIFE YOU HAD HEARD THE STORIES");
        PRINT("ABOUT YOUR CRAZY UNCLE SIMON. HE WAS AN");
        PRINT("INVENTOR, WHO KEPT DISAPPEARING FOR");
        PRINT("LONG PERIODS OF TIME, NEVER TELLING");
        PRINT("ANYONE WHERE HE HAD BEEN.");
        PRINT("");
        PRINT("YOU NEVER BELIEVED THE STORIES, BUT");
        PRINT("WHEN YOUR UNCLE DIED AND LEFT YOU HIS");
        PRINT("DIARY, YOU LEARNED THAT THEY WERE TRUE.");
        PRINT("YOUR UNCLE HAD DISCOVERED A MAGIC");
        PRINT("LAND, AND A SECRET FORMULA THAT COULD");
        PRINT("TAKE HIM THERE. IN THAT LAND WAS A");
        PRINT("MAGIC RUBY, AND HIS DIARY CONTAINED");
        PRINT("THE INSTRUCTIONS FOR GOING THERE TO");
        PRINT("FIND IT.");
        INPUT_n("");
    }

    private VerbResult Main()
    {
        // ** THE QUEST **
        // **
        // ** An adventure game
        //
        CLS();

        PRINT("Please stand by .... ");
        PRINT("");
        PRINT("");

        objects = new Objects();
        map = new Map();

        inventoryItems = 0;
        saltPoured = false;
        formulaPoured = false;
        mixtureCount = 1;
        wearingGloves = false;

        PrintIntro();

        CLS();

        VerbRoutines verbRoutines = new VerbRoutines(UnknownVerb);
        InitHandlers(verbRoutines);

        while (true)
        {
            PrintDescription();

            PrintDirections();

            PrintObjects();

            while (true)
            {
                string cmd = "";
                do
                {
                    PRINT("");
                    cmd = (INPUT_s("WHAT NOW"));
                }
                while (cmd == "");

                Command command = Command.Parse(cmd);

                string noun = command.Noun;
                if (noun == "SHA")
                {
                    noun = "SAL";
                }

                if (noun == "FOR")
                {
                    noun = "BOT";
                }

                VerbResult ret = verbRoutines.Handle(command.Verb, noun);
                if (ret == VerbResult.Idle)
                {
                    // NO-OP
                }
                else if (ret == VerbResult.Proceed)
                {
                    break;
                }
                else
                {
                    return ret;
                }
            }
        }
    }

    private void InitHandlers(VerbRoutines verbRoutines)
    {
        verbRoutines.Add("GO", Go);
        verbRoutines.Add("GET", Get);
        verbRoutines.Add("TAK", Get);
        verbRoutines.Add("DRO", Drop);
        verbRoutines.Add("THR", Drop);
        verbRoutines.Add("INV", Inventory);
        verbRoutines.Add("I", Inventory);
        verbRoutines.Add("LOO", Look);
        verbRoutines.Add("L", Look);
        verbRoutines.Add("EXA", Examine);
        verbRoutines.Add("QUI", Quit);
        verbRoutines.Add("REA", Read);
        verbRoutines.Add("OPE", Open);
        verbRoutines.Add("POU", Pour);
        verbRoutines.Add("CLI", Climb);
        verbRoutines.Add("JUM", Jump);
        verbRoutines.Add("DIG", Dig);
        verbRoutines.Add("ROW", Row);
        verbRoutines.Add("WAV", Wave);
        verbRoutines.Add("LEA", Leave);
        verbRoutines.Add("EXI", Leave);
        verbRoutines.Add("FIG", Fight);
        verbRoutines.Add("WEA", Wear);
    }

    private VerbResult UnknownVerb(string noun)
    {
        PRINT("I DON'T KNOW HOW TO DO THAT");
        return VerbResult.Idle;
    }

    private VerbResult Go(string noun)
    {
        int dir = Direction.Invalid;
        if (noun == "NOR")
        {
            dir = Direction.North;
        }
        else if (noun == "SOU")
        {
            dir = Direction.South;
        }
        else if (noun == "EAS")
        {
            dir = Direction.East;
        }
        else if (noun == "WES")
        {
            dir = Direction.West;
        }
        else if (noun == "UP")
        {
            dir = Direction.Up;
        }
        else if (noun == "DOW")
        {
            dir = Direction.Down;
        }

        if (dir == Direction.Invalid)
        {
            if ((noun == "BOA") && (objects.Ref(ObjectId.Boat).Room == map.CurrentRoom))
            {
                map.CurrentRoom = RoomId.Boat;
                return VerbResult.Proceed;
            }
            else
            {
                PRINT("YOU CAN'T GO THERE!");
                return VerbResult.Idle;
            }
        }

        return Go(dir);
    }

    private VerbResult Go(int dir)
    {
        MoveResult result = map.Move(dir);
        if (result == MoveResult.OK)
        {
            return VerbResult.Proceed;
        }

        if (result == MoveResult.Blocked)
        {
            PRINT("THE GUARD WON'T LET YOU!");
        }
        else
        {
            PRINT("YOU CAN'T GO THERE!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Get(string noun)
    {
        ObjectRef obj = objects.Find(noun);

        if (obj == null)
        {
            PRINT("YOU CAN'T GET THAT!");
        }
        else if (obj.Room == RoomId.Inventory)
        {
            PRINT("YOU ALREADY HAVE IT!");
        }
        else if (!obj.CanGet)
        {
            PRINT("YOU CAN'T GET THAT!");
        }
        else if (obj.Room != map.CurrentRoom)
        {
            PRINT("THAT'S NOT HERE!");
        }
        else if (inventoryItems > MaxInventoryItems)
        {
            PRINT("YOU CAN'T CARRY ANY MORE.");
        }
        else if ((map.CurrentRoom == RoomId.LargeHall) && (obj.Id == ObjectId.Ruby))
        {
            PRINT("CONGRATULATIONS! YOU'VE WON!");
            return PlayAgain();
        }
        else
        {
            ++inventoryItems;
            objects.Take(obj.Id);
            PRINT("TAKEN.");
        }

        return VerbResult.Idle;
    }

    private VerbResult Drop(string noun)
    {
        ObjectRef obj = objects.Find(noun);

        if ((obj == null) || (obj.Room != RoomId.Inventory))
        {
            PRINT("YOU DON'T HAVE THAT!");
        }
        else
        {
            --inventoryItems;
            objects.Drop(obj.Id, map.CurrentRoom);
            PRINT("DROPPED.");
        }

        return VerbResult.Idle;
    }

    private VerbResult Inventory(string noun)
    {
        bool atLeastOne = false;
        PRINT("YOU ARE CARRYING:");
        foreach (string name in objects.Carrying())
        {
            PRINT(" " + name);
            atLeastOne = true;
        }

        if (!atLeastOne)
        {
            PRINT(" NOTHING");
        }

        return VerbResult.Idle;
    }

    private VerbResult Look(string noun)
    {
        if (noun == "")
        {
            return VerbResult.Proceed;
        }

        Examine(noun);
        return VerbResult.Idle;
    }

    private VerbResult Examine(string noun)
    {
        if (noun == "GRO")
        {
            if (map.CurrentRoom != RoomId.OpenField)
            {
                PRINT("IT LOOKS LIKE GROUND!");
            }
            else
            {
                PRINT("IT LOOKS LIKE SOMETHING'S BURIED HERE.");
            }
        }
        else
        {
            ObjectRef obj = objects.Find(noun);

            if ((obj.Room != map.CurrentRoom) && (obj.Room != RoomId.Inventory))
            {
                PRINT("IT'S NOT HERE!");
            }
            else if (obj.Id == ObjectId.Bottle)
            {
                PRINT("THERE'S SOMETHING WRITTEN ON IT!");
            }
            else if (obj.Id == ObjectId.Case)
            {
                PRINT("THERE'S A JEWEL INSIDE!");
            }
            else if (obj.Id == ObjectId.Barrel)
            {
                PRINT("IT'S FILLED WITH RAINWATER.");
            }
            else
            {
                PRINT("YOU SEE NOTHING UNUSUAL.");
            }
        }

        return VerbResult.Idle;
    }

    private VerbResult Quit(string noun)
    {
        PRINT_n("ARE YOU SURE YOU WANT TO QUIT (Y/N)");
        string quit = INPUT_s("");
        if (quit != "N")
        {
            return PlayAgain();
        }

        return VerbResult.Idle;
    }

    private VerbResult PlayAgain()
    {
        while (true)
        {
            PRINT_n("WOULD YOU LIKE TO PLAY AGAIN (Y/N)");
            string quit = INPUT_s("");
            if (quit == "Y")
            {
                return VerbResult.RestartGame;
            }

            if (quit == "N")
            {
                return VerbResult.EndGame;
            }
        }
    }

    private VerbResult Read(string noun)
    {
        if (noun == "DIA")
        {
            if (!objects.IsHere(ObjectId.Diary, map.CurrentRoom))
            {
                PRINT("THERE'S NO DIARY HERE!");
            }
            else
            {
                PRINT("IT SAYS: 'ADD SODIUM CHLORIDE PLUS THE");
                PRINT("FORMULA TO RAINWATER, TO REACH THE");
                PRINT("OTHER WORLD.' ");
            }
        }
        else if (noun == "DIC")
        {
            if (!objects.IsHere(ObjectId.Dictionary, map.CurrentRoom))
            {
                PRINT("YOU DON'T SEE A DICTIONARY!");
            }
            else
            {
                PRINT("IT SAYS: SODIUM CHLORIDE IS");
                PRINT("COMMON TABLE SALT.");
            }
        }
        else if (noun == "BOT")
        {
            if (!objects.IsHere(ObjectId.Bottle, map.CurrentRoom))
            {
                PRINT("THERE'S NO BOTTLE HERE!");
            }
            else
            {
                PRINT("IT READS: 'SECRET FORMULA'.");
            }
        }
        else
        {
            PRINT("YOU CAN'T READ THAT!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Open(string noun)
    {
        if (noun == "BOX")
        {
            if (!objects.IsHere(ObjectId.Box, map.CurrentRoom))
            {
                PRINT("THERE'S NO BOX HERE!");
            }
            else
            {
                objects.Drop(ObjectId.Bottle, map.CurrentRoom);
                PRINT("SOMETHING FELL OUT!");
            }
        }
        else if (noun == "CAB")
        {
            if (map.CurrentRoom != RoomId.Kitchen)
            {
                PRINT("THERE'S NO CABINET HERE!");
            }
            else
            {
                PRINT("THERE'S SOMETHING INSIDE!");
                objects.Drop(ObjectId.Salt, RoomId.Kitchen);
            }
        }
        else if (noun == "CAS")
        {
            if (map.CurrentRoom != RoomId.LargeHall)
            {
                PRINT("THERE'S NO CASE HERE!");
            }
            else if (!wearingGloves)
            {
                PRINT("THE CASE IS ELECTRIFIED!");
            }
            else
            {
                PRINT("THE GLOVES INSULATE AGAINST THE");
                PRINT("ELECTRICITY! THE CASE OPENS!");
                objects.Drop(ObjectId.Ruby, RoomId.LargeHall);
            }
        }
        else
        {
            PRINT("YOU CAN'T OPEN THAT!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Pour(string noun)
    {
        bool poured;
        if (noun == "SAL")
        {
            poured = PourSalt();
        }
        else if (noun == "BOT")
        {
            poured = PourFormula();
        }
        else
        {
            PRINT("YOU CAN'T POUR THAT!");
            poured = false;
        }

        if (poured)
        {
            poured = PourMixture();
        }

        if (poured)
        {
            return VerbResult.Proceed;
        }

        return VerbResult.Idle;
    }

    private bool PourFormula()
    {
        if (!objects.IsHere(ObjectId.Bottle, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE BOTTLE!");
            return false;
        }
        else if (formulaPoured)
        {
            PRINT("THE BOTTLE IS EMPTY!");
            return false;
        }
        else
        {
            formulaPoured = true;
            return true;
        }
    }

    private bool PourSalt()
    {
        if (!objects.IsHere(ObjectId.Salt, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE SALT!");
            return false;
        }
        else if (saltPoured)
        {
            PRINT("THE SHAKER IS EMPTY!");
            return false;
        }
        else
        {
            saltPoured = true;
            return true;
        }
    }

    private bool PourMixture()
    {
        if (map.CurrentRoom == RoomId.Garage)
        {
            ++mixtureCount;
        }

        PRINT("POURED!");

        if (mixtureCount < 3)
        {
            return false;
        }

        PRINT("THERE IS AN EXPLOSION!");
        PRINT("EVERYTHING GOES BLACK!");
        PRINT("SUDDENLY YOU ARE ... ");
        PRINT(" ... SOMEWHERE ELSE!");

        map.CurrentRoom = RoomId.OpenField;
        return true;
    }

    private VerbResult Climb(string noun)
    {
        if (noun == "TRE")
        {
            if (map.CurrentRoom != RoomId.EdgeOfForest)
            {
                PRINT("THERE'S NO TREE HERE!");
            }
            else
            {
                PRINT("YOU CAN'T REACH THE BRANCHES!");
            }
        }
        else if (noun == "LAD")
        {
            if (!objects.IsHere(ObjectId.Ladder, map.CurrentRoom))
            {
                PRINT("YOU DON'T HAVE THE LADDER!");
            }
            else if (map.CurrentRoom != RoomId.EdgeOfForest)
            {
                PRINT("WHATEVER FOR?");
            }
            else
            {
                PRINT("THE LADDER SINKS UNDER YOUR WEIGHT!");
                PRINT("IT DISAPPEARS INTO THE GROUND!");
                objects.Hide(ObjectId.Ladder);
            }
        }
        else
        {
            PRINT("IT WON'T DO ANY GOOD.");
        }

        return VerbResult.Idle;
    }

    private VerbResult Jump(string noun)
    {
        if ((map.CurrentRoom != RoomId.EdgeOfForest) && (map.CurrentRoom != RoomId.BranchOfTree))
        {
            PRINT("WHEE! THAT WAS FUN!");
            return VerbResult.Idle;
        }
        else if (map.CurrentRoom == RoomId.BranchOfTree)
        {
            PRINT("YOU GRAB A HIGHER BRANCH ON THE");
            PRINT("TREE AND PULL YOURSELF UP....");
            map.CurrentRoom = RoomId.TopOfTree;
            return VerbResult.Proceed;
        }
        else
        {
            PRINT("YOU GRAB THE LOWEST BRANCH OF THE");
            PRINT("TREE AND PULL YOURSELF UP....");
            map.CurrentRoom = RoomId.BranchOfTree;
            return VerbResult.Proceed;
        }
    }

    private VerbResult Dig(string noun)
    {
        if ((noun != "HOL") && (noun != "GRO") && (noun != ""))
        {
            PRINT("YOU CAN'T DIG THAT!");
        }
        else if (!objects.IsHere(ObjectId.Shovel, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE A SHOVEL!");
        }
        else if (map.CurrentRoom != RoomId.OpenField)
        {
            PRINT("YOU DON'T FIND ANYTHING.");
        }
        else if (objects.Ref(ObjectId.Sword).Room != RoomId.None)
        {
            PRINT("THERE'S NOTHING ELSE THERE!");
        }
        else
        {
            PRINT("THERE'S SOMETHING THERE!");
            objects.Drop(ObjectId.Sword, RoomId.OpenField);
        }

        return VerbResult.Idle;
    }

    private VerbResult Row(string noun)
    {
        if ((noun != "BOA") && (noun != ""))
        {
            PRINT("HOW CAN YOU ROW THAT?");
        }
        else if (map.CurrentRoom != RoomId.Boat)
        {
            PRINT("YOU'RE NOT IN THE BOAT!");
        }
        else
        {
            PRINT("YOU DON'T HAVE AN OAR!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Wave(string noun)
    {
        if (noun != "FAN")
        {
            PRINT("YOU CAN'T WAVE THAT!");
        }
        else if (!objects.IsHere(ObjectId.Fan, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE FAN!");
        }
        else if (map.CurrentRoom != RoomId.Boat)
        {
            PRINT("YOU FEEL A REFRESHING BREEZE!");
        }
        else
        {
            PRINT("A POWERFUL BREEZE PROPELS THE BOAT");
            PRINT("TO THE OPPOSITE SHORE!");
            if (objects.Ref(ObjectId.Boat).Room == RoomId.SouthBankOfRiver)
            {
                objects.Drop(ObjectId.Boat, RoomId.NorthBankOfRiver);
            }
            else
            {
                objects.Drop(ObjectId.Boat, RoomId.SouthBankOfRiver);
            }
        }

        return VerbResult.Idle;
    }

    private VerbResult Leave(string noun)
    {
        if (map.CurrentRoom != RoomId.Boat)
        {
            PRINT("PLEASE GIVE A DIRECTION!");
            return VerbResult.Idle;
        }
        else if ((noun != "BOA") && (noun != ""))
        {
            PRINT("HUH?");
            return VerbResult.Idle;
        }
        else
        {
            map.CurrentRoom = objects.Ref(ObjectId.Boat).Room;
            return VerbResult.Proceed;
        }
    }

    private VerbResult Fight(string noun)
    {
        if (noun == "")
        {
            PRINT("WHOM DO YOU WANT TO FIGHT?");
        }
        else if (noun != "GUA")
        {
            PRINT(("") + ("YOU CAN'T FIGHT HIM!"));
        }
        else if (map.CurrentRoom != RoomId.SouthOfCastle)
        {
            PRINT("THERE'S NO GUARD HERE!");
        }
        else if (!objects.Carrying(ObjectId.Sword))
        {
            PRINT("YOU DON'T HAVE A WEAPON!");
        }
        else
        {
            PRINT("THE GUARD, NOTICING YOUR SWORD,");
            PRINT("WISELY RETREATS INTO THE CASTLE.");
            map.SetMap(RoomId.SouthOfCastle, 0, RoomId.NarrowHall);
            objects.Hide(ObjectId.Guard);
        }

        return VerbResult.Idle;
    }

    private VerbResult Wear(string noun)
    {
        if (noun != "GLO")
        {
            PRINT("YOU CAN'T WEAR THAT!");
        }
        else if (!objects.IsHere(ObjectId.Gloves, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE GLOVES.");
        }
        else
        {
            PRINT("YOU ARE NOW WEARING THE GLOVES.");
            wearingGloves = true;
        }

        return VerbResult.Idle;
    }
}