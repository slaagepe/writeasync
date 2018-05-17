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

                VerbResult ret = verbRoutines.Handle(command.Verb, command.Noun);
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
        Direction dir = GetDirection(noun);
        if (dir == Direction.Invalid)
        {
            ObjectId id = objects.IdOf(noun);
            if ((id == ObjectId.Boat) && (objects.Ref(id).Room == map.CurrentRoom))
            {
                map.CurrentRoom = RoomId.Boat;
                return VerbResult.Proceed;
            }

            PRINT("YOU CAN'T GO THERE!");
            return VerbResult.Idle;
        }

        return Go(dir);
    }

    private static Direction GetDirection(string noun)
    {
        switch (noun)
        {
            case "NOR": return Direction.North;
            case "SOU": return Direction.South;
            case "EAS": return Direction.East;
            case "WES": return Direction.West;
            case "UP": return Direction.Up;
            case "DOW": return Direction.Down;
            default: return Direction.Invalid;
        }
    }

    private VerbResult Go(Direction dir)
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
        return Get(obj);
    }

    private VerbResult Get(ObjectRef obj)
    {
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
        return Drop(obj);
    }

    private VerbResult Drop(ObjectRef obj)
    {
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
        ObjectId id = objects.IdOf(noun);
        if (id == ObjectId.Blank)
        {
            return VerbResult.Proceed;
        }

        return Examine(id);
    }

    private VerbResult Examine(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Examine(id);
    }

    private VerbResult Examine(ObjectId id)
    {
        if (id == ObjectId.Ground)
        {
            return ExamineGround();
        }

        if ((id == ObjectId.Invalid) || !objects.IsHere(id, map.CurrentRoom))
        {
            PRINT("IT'S NOT HERE!");
            return VerbResult.Idle;
        }

        if (id == ObjectId.Bottle)
        {
            return ExamineBottle();
        }

        if (id == ObjectId.Case)
        {
            return ExamineCase();
        }

        if (id == ObjectId.Barrel)
        {
            return ExamineBarrel();
        }

        PRINT("YOU SEE NOTHING UNUSUAL.");
        return VerbResult.Idle;
    }

    private VerbResult ExamineBarrel()
    {
        PRINT("IT'S FILLED WITH RAINWATER.");
        return VerbResult.Idle;
    }

    private VerbResult ExamineCase()
    {
        PRINT("THERE'S A JEWEL INSIDE!");
        return VerbResult.Idle;
    }

    private VerbResult ExamineBottle()
    {
        PRINT("THERE'S SOMETHING WRITTEN ON IT!");
        return VerbResult.Idle;
    }

    private VerbResult ExamineGround()
    {
        if (map.CurrentRoom != RoomId.OpenField)
        {
            PRINT("IT LOOKS LIKE GROUND!");
        }
        else
        {
            PRINT("IT LOOKS LIKE SOMETHING'S BURIED HERE.");
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
        ObjectId id = objects.IdOf(noun);
        return Read(id);
    }

    private VerbResult Read(ObjectId id)
    {
        if (id == ObjectId.Diary)
        {
            return ReadDiary(id);
        }

        if (id == ObjectId.Dictionary)
        {
            return ReadDictionary(id);
        }

        if (id == ObjectId.Bottle)
        {
            return ReadBottle(id);
        }

        PRINT("YOU CAN'T READ THAT!");
        return VerbResult.Idle;
    }

    private VerbResult ReadBottle(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
        {
            PRINT("THERE'S NO BOTTLE HERE!");
        }
        else
        {
            PRINT("IT READS: 'SECRET FORMULA'.");
        }

        return VerbResult.Idle;
    }

    private VerbResult ReadDictionary(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
        {
            PRINT("YOU DON'T SEE A DICTIONARY!");
        }
        else
        {
            PRINT("IT SAYS: SODIUM CHLORIDE IS");
            PRINT("COMMON TABLE SALT.");
        }

        return VerbResult.Idle;
    }

    private VerbResult ReadDiary(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
        {
            PRINT("THERE'S NO DIARY HERE!");
        }
        else
        {
            PRINT("IT SAYS: 'ADD SODIUM CHLORIDE PLUS THE");
            PRINT("FORMULA TO RAINWATER, TO REACH THE");
            PRINT("OTHER WORLD.' ");
        }

        return VerbResult.Idle;
    }

    private VerbResult Open(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Open(id);
    }

    private VerbResult Open(ObjectId id)
    {
        if (id == ObjectId.Box)
        {
            return OpenBox(id);
        }

        if (id == ObjectId.Cabinet)
        {
            return OpenCabinet();
        }

        if (id == ObjectId.Case)
        {
            return OpenCase();
        }

        PRINT("YOU CAN'T OPEN THAT!");
        return VerbResult.Idle;
    }

    private VerbResult OpenCase()
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

        return VerbResult.Idle;
    }

    private VerbResult OpenCabinet()
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

        return VerbResult.Idle;
    }

    private VerbResult OpenBox(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
        {
            PRINT("THERE'S NO BOX HERE!");
        }
        else
        {
            objects.Drop(ObjectId.Bottle, map.CurrentRoom);
            PRINT("SOMETHING FELL OUT!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Pour(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Pour(id);
    }

    private VerbResult Pour(ObjectId id)
    {
        if (id == ObjectId.Salt)
        {
            return PourSalt();
        }

        if (id == ObjectId.Bottle)
        {
            return PourFormula();
        }

        PRINT("YOU CAN'T POUR THAT!");
        return VerbResult.Idle;
    }

    private VerbResult PourFormula()
    {
        if (!objects.IsHere(ObjectId.Bottle, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE BOTTLE!");
            return VerbResult.Idle;
        }

        if (formulaPoured)
        {
            PRINT("THE BOTTLE IS EMPTY!");
            return VerbResult.Idle;
        }

        formulaPoured = true;
        return PourMixture();
    }

    private VerbResult PourSalt()
    {
        if (!objects.IsHere(ObjectId.Salt, map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE SALT!");
            return VerbResult.Idle;
        }

        if (saltPoured)
        {
            PRINT("THE SHAKER IS EMPTY!");
            return VerbResult.Idle;
        }

        saltPoured = true;
        return PourMixture();
    }

    private VerbResult PourMixture()
    {
        if (map.CurrentRoom == RoomId.Garage)
        {
            ++mixtureCount;
        }

        PRINT("POURED!");

        if (mixtureCount < 3)
        {
            return VerbResult.Idle;
        }

        PRINT("THERE IS AN EXPLOSION!");
        PRINT("EVERYTHING GOES BLACK!");
        PRINT("SUDDENLY YOU ARE ... ");
        PRINT(" ... SOMEWHERE ELSE!");

        map.CurrentRoom = RoomId.OpenField;
        return VerbResult.Proceed;
    }

    private VerbResult Climb(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Climb(id);
    }

    private VerbResult Climb(ObjectId id)
    {
        if (id == ObjectId.Tree)
        {
            return ClimbTree();
        }

        if (id == ObjectId.Ladder)
        {
            return ClimbLadder(id);
        }

        PRINT("IT WON'T DO ANY GOOD.");
        return VerbResult.Idle;
    }

    private VerbResult ClimbLadder(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
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
            objects.Hide(id);
        }

        return VerbResult.Idle;
    }

    private VerbResult ClimbTree()
    {
        if (map.CurrentRoom != RoomId.EdgeOfForest)
        {
            PRINT("THERE'S NO TREE HERE!");
        }
        else
        {
            PRINT("YOU CAN'T REACH THE BRANCHES!");
        }

        return VerbResult.Idle;
    }

    private VerbResult Jump(string noun)
    {
        if ((map.CurrentRoom == RoomId.EdgeOfForest) || (map.CurrentRoom == RoomId.BranchOfTree))
        {
            return JumpTree();
        }

        PRINT("WHEE! THAT WAS FUN!");
        return VerbResult.Idle;
    }

    private VerbResult JumpTree()
    {
        if (map.CurrentRoom == RoomId.BranchOfTree)
        {
            PRINT("YOU GRAB A HIGHER BRANCH ON THE");
            PRINT("TREE AND PULL YOURSELF UP....");
            map.CurrentRoom = RoomId.TopOfTree;
            return VerbResult.Proceed;
        }

        PRINT("YOU GRAB THE LOWEST BRANCH OF THE");
        PRINT("TREE AND PULL YOURSELF UP....");
        map.CurrentRoom = RoomId.BranchOfTree;
        return VerbResult.Proceed;
    }

    private VerbResult Dig(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Dig(id);
    }

    private VerbResult Dig(ObjectId id)
    {
        if ((id == ObjectId.Blank) || (id == ObjectId.Hole) || (id == ObjectId.Ground))
        {
            return DigHole();
        }

        PRINT("YOU CAN'T DIG THAT!");
        return VerbResult.Idle;
    }

    private VerbResult DigHole()
    {
        if (!objects.IsHere(ObjectId.Shovel, map.CurrentRoom))
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
        ObjectId id = objects.IdOf(noun);
        if ((id != ObjectId.Boat) && (id != ObjectId.Blank))
        {
            PRINT("HOW CAN YOU ROW THAT?");
            return VerbResult.Idle;
        }

        if (map.CurrentRoom != RoomId.Boat)
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
        ObjectId id = objects.IdOf(noun);
        return Wave(id);
    }

    private VerbResult Wave(ObjectId id)
    {
        if (id == ObjectId.Fan)
        {
            return WaveFan(id);
        }

        PRINT("YOU CAN'T WAVE THAT!");
        return VerbResult.Idle;
    }

    private VerbResult WaveFan(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
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
        if (map.CurrentRoom == RoomId.Boat)
        {
            ObjectId id = objects.IdOf(noun);
            if ((id != ObjectId.Boat) && (id != ObjectId.Blank))
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

        PRINT("PLEASE GIVE A DIRECTION!");
        return VerbResult.Idle;
    }

    private VerbResult Fight(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Fight(id);
    }

    private VerbResult Fight(ObjectId id)
    {
        if (id == ObjectId.Blank)
        {
            PRINT("WHOM DO YOU WANT TO FIGHT?");
            return VerbResult.Idle;
        }

        if (id == ObjectId.Guard)
        {
            return FightGuard(id);
        }

        PRINT("YOU CAN'T FIGHT HIM!");
        return VerbResult.Idle;
    }

    private VerbResult FightGuard(ObjectId id)
    {
        if (map.CurrentRoom != RoomId.SouthOfCastle)
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
            objects.Hide(id);
        }

        return VerbResult.Idle;
    }

    private VerbResult Wear(string noun)
    {
        ObjectId id = objects.IdOf(noun);
        return Wear(id);
    }

    private VerbResult Wear(ObjectId id)
    {
        if (id == ObjectId.Gloves)
        {
            return WearGloves(id);
        }

        PRINT("YOU CAN'T WEAR THAT!");
        return VerbResult.Idle;
    }

    private VerbResult WearGloves(ObjectId id)
    {
        if (!objects.IsHere(id, map.CurrentRoom))
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