//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by GWBas2CS.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Adventure;
using Adventure.Verbs;

internal sealed class adventure
{
    private const int MaxInventoryItems = 5;

    private readonly TextReader input;
    private readonly TextWriter output;

    private GameState state;

    public adventure(TextReader input, TextWriter output)
    {
        this.input = input;
        this.output = output;
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
        foreach (string dir in state.Map.Directions())
        {
            PRINT_n(dir + " ");
        }

        PRINT("");
    }

    private void PrintObjects()
    {
        PRINT("YOU CAN SEE: ");
        bool atLeastOne = false;
        foreach (string name in state.Objects.Here(state.Map.CurrentRoom))
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
        PRINT("YOU ARE " + state.Map.Describe());
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

        state = new GameState();

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
        Func<string, ObjectRef> byRef = s => state.Objects.Find(s);
        Func<string, ObjectId> byId = s => state.Objects.IdOf(s);
        verbRoutines.Add("GO", Go);
        verbRoutines.Add("GET", byRef, Get);
        verbRoutines.Add("TAK", byRef, Get);
        verbRoutines.Add("DRO", byRef, Drop);
        verbRoutines.Add("THR", byRef, Drop);
        verbRoutines.Add("INV", Inventory);
        verbRoutines.Add("I", Inventory);
        verbRoutines.Add("LOO", byId, Eq(ObjectId.Blank, LookBlank), Else<ObjectId>(Examine));
        verbRoutines.Add("L", byId, Eq(ObjectId.Blank, LookBlank), Else<ObjectId>(Examine));
        verbRoutines.Add("EXA", byId, Examine);
        verbRoutines.Add("QUI", Quit);
        verbRoutines.Add("REA", byId, Eq(ObjectId.Diary, ReadDiary), Eq(ObjectId.Dictionary, ReadDictionary), Eq(ObjectId.Bottle, ReadBottle), Else<ObjectId>(ReadUnknown));
        verbRoutines.Add("OPE", byId, Eq(ObjectId.Box, OpenBox), Eq(ObjectId.Cabinet, OpenCabinet), Eq(ObjectId.Case, OpenCase), Else<ObjectId>(OpenUnknown));
        verbRoutines.Add("POU", byId, Eq(ObjectId.Salt, PourSalt), Eq(ObjectId.Bottle, PourFormula), Else<ObjectId>(PourUnknown));
        verbRoutines.Add("CLI", byId, Eq(ObjectId.Tree, ClimbTree), Eq(ObjectId.Ladder, ClimbLadder), Else<ObjectId>(ClimbUnknown));
        verbRoutines.Add("JUM", Jump);
        verbRoutines.Add("DIG", byId, Eq(Any(ObjectId.Blank, ObjectId.Hole, ObjectId.Ground), DigHole), Else<ObjectId>(DigUnknown));

        Row row = new Row(state, PRINT);
        verbRoutines.Add("ROW", byId, Eq(Any(ObjectId.Boat, ObjectId.Blank), row.Boat), Else<ObjectId>(row.Unknown));

        Wave wave = new Wave(state, PRINT);
        verbRoutines.Add("WAV", byId, Eq(ObjectId.Fan, wave.Fan), Else<ObjectId>(wave.Unknown));

        Leave leave = new Leave(state, PRINT);
        verbRoutines.Add("LEA", byId, leave.Any);
        verbRoutines.Add("EXI", byId, leave.Any);

        Fight fight = new Fight(state, PRINT);
        verbRoutines.Add("FIG", byId, Eq(ObjectId.Blank, fight.Blank), Eq(ObjectId.Guard, fight.Guard), Else<ObjectId>(fight.Unknown));

        Wear wear = new Wear(state, PRINT);
        verbRoutines.Add("WEA", byId, Eq(ObjectId.Gloves, wear.Gloves), Else<ObjectId>(wear.Unknown));
    }

    private static Predicate<T> Any<T>(params T[] vals) where T : struct
    {
        return t =>
        {
            foreach (T val in vals)
            {
                if (Comparer<T>.Default.Compare(t, val) == 0)
                {
                    return true;
                }
            }

            return false;
        };
    }

    private static Tuple<Predicate<T>, Func<T, VerbResult>> Eq<T>(Predicate<T> pred, Func<T, VerbResult> func) where T : struct
    {
        return Tuple.Create<Predicate<T>, Func<T, VerbResult>>(pred, func);
    }

    private static Tuple<Predicate<T>, Func<T, VerbResult>> Eq<T>(T val, Func<T, VerbResult> func) where T : struct
    {
        return Eq(Any(val), func);
    }

    private static Tuple<Predicate<T>, Func<T, VerbResult>> Else<T>(Func<T, VerbResult> func) where T : struct
    {
        return Tuple.Create<Predicate<T>, Func<T, VerbResult>>(t => true, func);
    }

    private VerbResult UnknownVerb()
    {
        PRINT("I DON'T KNOW HOW TO DO THAT");
        return VerbResult.Idle;
    }

    private VerbResult Go(string noun)
    {
        Direction dir = GetDirection(noun);
        if (dir == Direction.Invalid)
        {
            ObjectId id = state.Objects.IdOf(noun);
            return Go(id);
        }

        return Go(dir);
    }

    private VerbResult Go(ObjectId id)
    {
        if ((id == ObjectId.Boat) && (state.Objects.Ref(id).Room == state.Map.CurrentRoom))
        {
            state.Map.CurrentRoom = RoomId.Boat;
            return VerbResult.Proceed;
        }

        PRINT("YOU CAN'T GO THERE!");
        return VerbResult.Idle;
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
        MoveResult result = state.Map.Move(dir);
        if (result == MoveResult.OK)
        {
            return VerbResult.Proceed;
        }

        if (result == MoveResult.Blocked)
        {
            PRINT("THE GUARD WON'T LET YOU!");
            return VerbResult.Idle;
        }

        PRINT("YOU CAN'T GO THERE!");
        return VerbResult.Idle;
    }

    private VerbResult Get(ObjectRef obj)
    {
        if (obj == null)
        {
            PRINT("YOU CAN'T GET THAT!");
            return VerbResult.Idle;
        }

        if (obj.Room == RoomId.Inventory)
        {
            PRINT("YOU ALREADY HAVE IT!");
            return VerbResult.Idle;
        }

        if (!obj.CanGet)
        {
            PRINT("YOU CAN'T GET THAT!");
            return VerbResult.Idle;
        }

        if (obj.Room != state.Map.CurrentRoom)
        {
            PRINT("THAT'S NOT HERE!");
            return VerbResult.Idle;
        }

        if (state.InventoryItems > MaxInventoryItems)
        {
            PRINT("YOU CAN'T CARRY ANY MORE.");
            return VerbResult.Idle;
        }

        if ((state.Map.CurrentRoom == RoomId.LargeHall) && (obj.Id == ObjectId.Ruby))
        {
            PRINT("CONGRATULATIONS! YOU'VE WON!");
            return PlayAgain();
        }

        ++state.InventoryItems;
        state.Objects.Take(obj.Id);
        PRINT("TAKEN.");
        return VerbResult.Idle;
    }

    private VerbResult Drop(ObjectRef obj)
    {
        if ((obj == null) || (obj.Room != RoomId.Inventory))
        {
            PRINT("YOU DON'T HAVE THAT!");
            return VerbResult.Idle;
        }

        --state.InventoryItems;
        state.Objects.Drop(obj.Id, state.Map.CurrentRoom);
        PRINT("DROPPED.");
        return VerbResult.Idle;
    }

    private VerbResult Inventory()
    {
        bool atLeastOne = false;
        PRINT("YOU ARE CARRYING:");
        foreach (string name in state.Objects.Carrying())
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

    private VerbResult LookBlank(ObjectId id) => VerbResult.Proceed;

    private VerbResult Examine(ObjectId id)
    {
        if (id == ObjectId.Ground)
        {
            return ExamineGround();
        }

        if ((id == ObjectId.Invalid) || !state.Objects.IsHere(id, state.Map.CurrentRoom))
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

        return ExamineUnknown();
    }

    private VerbResult ExamineUnknown()
    {
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
        if (state.Map.CurrentRoom != RoomId.OpenField)
        {
            PRINT("IT LOOKS LIKE GROUND!");
            return VerbResult.Idle;
        }

        PRINT("IT LOOKS LIKE SOMETHING'S BURIED HERE.");
        return VerbResult.Idle;
    }

    private VerbResult Quit()
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

    private VerbResult ReadUnknown(ObjectId id)
    {
        PRINT("YOU CAN'T READ THAT!");
        return VerbResult.Idle;
    }

    private VerbResult ReadBottle(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("THERE'S NO BOTTLE HERE!");
            return VerbResult.Idle;
        }

        PRINT("IT READS: 'SECRET FORMULA'.");
        return VerbResult.Idle;
    }

    private VerbResult ReadDictionary(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("YOU DON'T SEE A DICTIONARY!");
            return VerbResult.Idle;
        }

        PRINT("IT SAYS: SODIUM CHLORIDE IS");
        PRINT("COMMON TABLE SALT.");
        return VerbResult.Idle;
    }

    private VerbResult ReadDiary(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("THERE'S NO DIARY HERE!");
            return VerbResult.Idle;
        }

        PRINT("IT SAYS: 'ADD SODIUM CHLORIDE PLUS THE");
        PRINT("FORMULA TO RAINWATER, TO REACH THE");
        PRINT("OTHER WORLD.' ");
        return VerbResult.Idle;
    }

    private VerbResult OpenUnknown(ObjectId id)
    {
        PRINT("YOU CAN'T OPEN THAT!");
        return VerbResult.Idle;
    }

    private VerbResult OpenCase(ObjectId id)
    {
        if (state.Map.CurrentRoom != RoomId.LargeHall)
        {
            PRINT("THERE'S NO CASE HERE!");
            return VerbResult.Idle;
        }

        if (!state.WearingGloves)
        {
            PRINT("THE CASE IS ELECTRIFIED!");
            return VerbResult.Idle;
        }

        PRINT("THE GLOVES INSULATE AGAINST THE");
        PRINT("ELECTRICITY! THE CASE OPENS!");
        state.Objects.Drop(ObjectId.Ruby, RoomId.LargeHall);
        return VerbResult.Idle;
    }

    private VerbResult OpenCabinet(ObjectId id)
    {
        if (state.Map.CurrentRoom != RoomId.Kitchen)
        {
            PRINT("THERE'S NO CABINET HERE!");
            return VerbResult.Idle;
        }

        PRINT("THERE'S SOMETHING INSIDE!");
        state.Objects.Drop(ObjectId.Salt, RoomId.Kitchen);
        return VerbResult.Idle;
    }

    private VerbResult OpenBox(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("THERE'S NO BOX HERE!");
            return VerbResult.Idle;
        }

        state.Objects.Drop(ObjectId.Bottle, state.Map.CurrentRoom);
        PRINT("SOMETHING FELL OUT!");
        return VerbResult.Idle;
    }

    private VerbResult PourUnknown(ObjectId id)
    {
        PRINT("YOU CAN'T POUR THAT!");
        return VerbResult.Idle;
    }

    private VerbResult PourFormula(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE BOTTLE!");
            return VerbResult.Idle;
        }

        if (state.FormulaPoured)
        {
            PRINT("THE BOTTLE IS EMPTY!");
            return VerbResult.Idle;
        }

        state.FormulaPoured = true;
        return PourMixture();
    }

    private VerbResult PourSalt(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE SALT!");
            return VerbResult.Idle;
        }

        if (state.SaltPoured)
        {
            PRINT("THE SHAKER IS EMPTY!");
            return VerbResult.Idle;
        }

        state.SaltPoured = true;
        return PourMixture();
    }

    private VerbResult PourMixture()
    {
        if (state.Map.CurrentRoom == RoomId.Garage)
        {
            ++state.MixtureCount;
        }

        PRINT("POURED!");

        if (state.MixtureCount < 2)
        {
            return VerbResult.Idle;
        }

        PRINT("THERE IS AN EXPLOSION!");
        PRINT("EVERYTHING GOES BLACK!");
        PRINT("SUDDENLY YOU ARE ... ");
        PRINT(" ... SOMEWHERE ELSE!");

        state.Map.CurrentRoom = RoomId.OpenField;
        return VerbResult.Proceed;
    }

    private VerbResult ClimbUnknown(ObjectId id)
    {
        PRINT("IT WON'T DO ANY GOOD.");
        return VerbResult.Idle;
    }

    private VerbResult ClimbLadder(ObjectId id)
    {
        if (!state.Objects.IsHere(id, state.Map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE THE LADDER!");
            return VerbResult.Idle;
        }

        if (state.Map.CurrentRoom != RoomId.EdgeOfForest)
        {
            PRINT("WHATEVER FOR?");
            return VerbResult.Idle;
        }

        PRINT("THE LADDER SINKS UNDER YOUR WEIGHT!");
        PRINT("IT DISAPPEARS INTO THE GROUND!");
        state.Objects.Hide(id);
        return VerbResult.Idle;
    }

    private VerbResult ClimbTree(ObjectId id)
    {
        if (state.Map.CurrentRoom != RoomId.EdgeOfForest)
        {
            PRINT("THERE'S NO TREE HERE!");
            return VerbResult.Idle;
        }

        PRINT("YOU CAN'T REACH THE BRANCHES!");
        return VerbResult.Idle;
    }

    private VerbResult Jump()
    {
        if ((state.Map.CurrentRoom == RoomId.EdgeOfForest) || (state.Map.CurrentRoom == RoomId.BranchOfTree))
        {
            return JumpTree();
        }

        return JumpUnknown();
    }

    private VerbResult JumpUnknown()
    {
        PRINT("WHEE! THAT WAS FUN!");
        return VerbResult.Idle;
    }

    private VerbResult JumpTree()
    {
        if (state.Map.CurrentRoom == RoomId.BranchOfTree)
        {
            PRINT("YOU GRAB A HIGHER BRANCH ON THE");
            PRINT("TREE AND PULL YOURSELF UP....");
            state.Map.CurrentRoom = RoomId.TopOfTree;
            return VerbResult.Proceed;
        }

        PRINT("YOU GRAB THE LOWEST BRANCH OF THE");
        PRINT("TREE AND PULL YOURSELF UP....");
        state.Map.CurrentRoom = RoomId.BranchOfTree;
        return VerbResult.Proceed;
    }

    private VerbResult DigUnknown(ObjectId id)
    {
        PRINT("YOU CAN'T DIG THAT!");
        return VerbResult.Idle;
    }

    private VerbResult DigHole(ObjectId id)
    {
        if (!state.Objects.IsHere(ObjectId.Shovel, state.Map.CurrentRoom))
        {
            PRINT("YOU DON'T HAVE A SHOVEL!");
            return VerbResult.Idle;
        }

        if (state.Map.CurrentRoom != RoomId.OpenField)
        {
            PRINT("YOU DON'T FIND ANYTHING.");
            return VerbResult.Idle;
        }

        if (state.Objects.Ref(ObjectId.Sword).Room != RoomId.None)
        {
            PRINT("THERE'S NOTHING ELSE THERE!");
            return VerbResult.Idle;
        }

        PRINT("THERE'S SOMETHING THERE!");
        state.Objects.Drop(ObjectId.Sword, RoomId.OpenField);
        return VerbResult.Idle;
    }

    internal sealed class Row : Verb
    {
        public Row(GameState state, Action<string> print)
            : base(state, print)
        {
        }

        public VerbResult Unknown(ObjectId id)
        {
            this.Print("HOW CAN YOU ROW THAT?");
            return VerbResult.Idle;
        }

        public VerbResult Boat(ObjectId id)
        {
            if (this.State.Map.CurrentRoom != RoomId.Boat)
            {
                this.Print("YOU'RE NOT IN THE BOAT!");
                return VerbResult.Idle;
            }

            this.Print("YOU DON'T HAVE AN OAR!");
            return VerbResult.Idle;
        }
    }
}