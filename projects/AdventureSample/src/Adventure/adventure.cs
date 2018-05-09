//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by GWBas2CS.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;

internal sealed class adventure
{
    private readonly TextReader input;
    private readonly TextWriter output;
    private Queue DATA;
    private string[] roomDescriptions;
    private string[] objectNames;
    private string[] objectTags;
    private string[] directions;
    private float[] objectRooms;
    private float[,] map;
    private string verb;
    private string noun;
    private string quit;
    private float numberOfRooms;
    private float numberOfObjects;
    private float numberOfDirections;
    private float maxInventoryItems;
    private float currentRoom;
    private float inventoryItems;
    private float saltPoured;
    private float formulaPoured;
    private float mixtureCount;
    private float wearingGloves;
    private float I_n;
    private float FL_n;
    private float RO_n;

    public adventure(TextReader input, TextWriter output)
    {
        this.input = (input);
        this.output = (output);
    }

    public void Run()
    {
        while ((this.Main()) == (1))
        {
        }
    }

    private void Init()
    {
        DATA = (new Queue());

        // LIVING ROOM
        DATA.Enqueue((float)(4));
        DATA.Enqueue((float)(3));
        DATA.Enqueue((float)(2));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // KITCHEN
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(1));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // LIBRARY
        DATA.Enqueue((float)(1));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // FRONT YARD
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(1));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(5));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // GARAGE
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(4));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // OPEN FIELD
        DATA.Enqueue((float)(9));
        DATA.Enqueue((float)(7));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // EDGE OF FOREST
        DATA.Enqueue((float)(6));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // BRANCH OF TREE
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(7));

        // LONG, WINDING ROAD (1)
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(6));
        DATA.Enqueue((float)(10));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // LONG, WINDING ROAD (2)
        DATA.Enqueue((float)(11));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(9));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // LONG, WINDING ROAD (3)
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(10));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(12));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // SOUTH BANK OF RIVER
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(11));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // BOAT
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // NORTH BANK OF RIVER
        DATA.Enqueue((float)(15));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // WELL-TRAVELED ROAD
        DATA.Enqueue((float)(16));
        DATA.Enqueue((float)(14));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // SOUTH OF CASTLE
        DATA.Enqueue((float)(128));
        DATA.Enqueue((float)(15));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));

        // NARROW HALL
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(18));
        DATA.Enqueue((float)(0));

        // LARGE HALL
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(17));

        // TOP OF TREE
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(0));
        DATA.Enqueue((float)(8));

        // OBJECT #0
        DATA.Enqueue("AN OLD DIARY");
        DATA.Enqueue("DIA");
        DATA.Enqueue((float)(1));

        // OBJECT #1
        DATA.Enqueue("A SMALL BOX");
        DATA.Enqueue("BOX");
        DATA.Enqueue((float)(1));

        // OBJECT #2
        DATA.Enqueue("CABINET");
        DATA.Enqueue("CAB");
        DATA.Enqueue((float)(130));

        // OBJECT #3
        DATA.Enqueue("A SALT SHAKER");
        DATA.Enqueue("SAL");
        DATA.Enqueue((float)(0));

        // OBJECT #4
        DATA.Enqueue("A DICTIONARY");
        DATA.Enqueue("DIC");
        DATA.Enqueue((float)(3));

        // OBJECT #5
        DATA.Enqueue("WOODEN BARREL");
        DATA.Enqueue("BAR");
        DATA.Enqueue((float)(133));

        // OBJECT #6
        DATA.Enqueue("A SMALL BOTTLE");
        DATA.Enqueue("BOT");
        DATA.Enqueue((float)(0));

        // OBJECT #7
        DATA.Enqueue("A LADDER");
        DATA.Enqueue("LAD");
        DATA.Enqueue((float)(4));

        // OBJECT #8
        DATA.Enqueue("A SHOVEL");
        DATA.Enqueue("SHO");
        DATA.Enqueue((float)(5));

        // OBJECT #9
        DATA.Enqueue("A TREE");
        DATA.Enqueue("TRE");
        DATA.Enqueue((float)(135));

        // OBJECT #10
        DATA.Enqueue("A GOLDEN SWORD");
        DATA.Enqueue("SWO");
        DATA.Enqueue((float)(0));

        // OBJECT #11
        DATA.Enqueue("A WOODEN BOAT");
        DATA.Enqueue("BOA");
        DATA.Enqueue((float)(140));

        // OBJECT #12
        DATA.Enqueue("A MAGIC FAN");
        DATA.Enqueue("FAN");
        DATA.Enqueue((float)(8));

        // OBJECT #13
        DATA.Enqueue("A NASTY-LOOKING GUARD");
        DATA.Enqueue("GUA");
        DATA.Enqueue((float)(144));

        // OBJECT #14
        DATA.Enqueue("A GLASS CASE");
        DATA.Enqueue("CAS");
        DATA.Enqueue((float)(146));

        // OBJECT #15
        DATA.Enqueue("A GLOWING RUBY");
        DATA.Enqueue("RUB");
        DATA.Enqueue((float)(0));

        // OBJECT #16
        DATA.Enqueue("A PAIR OF RUBBER GLOVES");
        DATA.Enqueue("GLO");
        DATA.Enqueue((float)(19));

        verb = ("");
        noun = ("");
        quit = ("");
        numberOfRooms = (0);
        numberOfObjects = (0);
        numberOfDirections = (0);
        maxInventoryItems = (0);
        currentRoom = (0);
        inventoryItems = (0);
        saltPoured = (0);
        formulaPoured = (0);
        mixtureCount = (0);
        wearingGloves = (0);
        I_n = (0);
        FL_n = (0);
        RO_n = (0);
        roomDescriptions = (new string[11]);
        objectNames = (new string[11]);
        objectTags = (new string[11]);
        directions = (new string[11]);
        objectRooms = (new float[11]);
    }

    private void CLS()
    {
        this.output.Write('\f');
        Console.Clear();
    }

    private void DIM1_sa(out string[] a, int d1)
    {
        a = (new string[(d1) + (1)]);
        Array.Fill(a, "");
    }

    private void DIM1_na(out float[] a, int d1)
    {
        a = (new float[(d1) + (1)]);
    }

    private void DIM2_na(out float[,] a, int d1, int d2)
    {
        a = (new float[(d1) + (1), (d2) + (1)]);
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

    private string MID_s(string x, int n, int m)
    {
        if ((n) > (x.Length))
        {
            return "";
        }

        int l = ((x.Length) - (n)) + (1);
        if ((m) > (l))
        {
            m = (l);
        }

        return x.Substring((n) - (1), m);
    }

    private string LEFT_s(string x, int n)
    {
        if ((n) > (x.Length))
        {
            return x;
        }

        return x.Substring(0, n);
    }

    private void PRINT_n(string expression)
    {
        this.output.Write(expression);
    }

    private float READ_n()
    {
        return (float)(DATA.Dequeue());
    }

    private string READ_s()
    {
        return (string)(DATA.Dequeue());
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
        for (int i = 0; i <= 5; ++i)
        {
            if (map[(int)(currentRoom), i] > 0)
            {
                PRINT_n(directions[i] + " ");
            }
        }

        PRINT("");
    }

    private void PrintObjects()
    {
        PRINT("YOU CAN SEE: ");
        bool atLeastOne = false;
        for (int i = 0; i < numberOfObjects; ++i)
        {
            if (currentRoom == ((int)objectRooms[i] & 127))
            {
                PRINT(" " + objectNames[i]);
                atLeastOne = true;
            }
        }

        if (!atLeastOne)
        {
            PRINT(" NOTHING OF INTEREST");
        }
    }

    private void PrintDescription()
    {
        PRINT("");
        PRINT((("") + ("YOU ARE ")) + (roomDescriptions[(int)(currentRoom)]));
    }

    private void FindRoomForObject()
    {
        if (numberOfObjects == 0)
        {
            return;
        }

        FL_n = 0;
        for (I_n = 0; I_n < numberOfObjects; ++I_n)
        {
            if (objectTags[(int)(I_n)] == noun)
            {
                FL_n = 1;
                RO_n = objectRooms[(int)(I_n)];
                break;
            }
        }

        if (FL_n != 0)
        {
            RO_n = objectRooms[(int)(I_n)];
            if (RO_n > 127)
            {
                RO_n -= 128;
            }
        }
    }

    private void InitMap()
    {
        if (numberOfRooms == 0)
        {
            return;
        }

        directions[0] = "NORTH";
        directions[1] = "SOUTH";
        directions[2] = "EAST";
        directions[3] = "WEST";
        directions[4] = "UP";
        directions[5] = "DOWN";

        for (int i = 1; i <= numberOfRooms; ++i)
        {
            for (int j = 0; j < numberOfDirections; ++j)
            {
                map[i, j] = READ_n();
            }
        }
    }

    private void InitObjects()
    {
        if (numberOfObjects == 0)
        {
            return;
        }

        for (int i = 0; i < numberOfObjects; ++i)
        {
            objectNames[i] = READ_s();
            objectTags[i] = READ_s();
            objectRooms[i] = READ_n();
        }
    }

    private void InitDescriptions()
    {
        roomDescriptions[1] = "IN YOUR LIVING ROOM.";
        roomDescriptions[2] = "IN THE KITCHEN.";
        roomDescriptions[3] = "IN THE LIBRARY.";
        roomDescriptions[4] = "IN THE FRONT YARD.";
        roomDescriptions[5] = "IN THE GARAGE.";
        roomDescriptions[6] = "IN AN OPEN FIELD.";
        roomDescriptions[7] = "AT THE EDGE OF A FOREST.";
        roomDescriptions[8] = "ON A BRANCH OF A TREE.";
        roomDescriptions[9] = "ON A LONG, WINDING ROAD.";
        roomDescriptions[10] = "ON A LONG, WINDING ROAD.";
        roomDescriptions[11] = "ON A LONG, WINDING ROAD.";
        roomDescriptions[12] = "ON THE SOUTH BANK OF A RIVER.";
        roomDescriptions[13] = "INSIDE THE WOODEN BOAT.";
        roomDescriptions[14] = "ON THE NORTH BANK OF A RIVER.";
        roomDescriptions[15] = "ON A WELL-TRAVELED ROAD.";
        roomDescriptions[16] = "IN FRONT OF A LARGE CASTLE.";
        roomDescriptions[17] = "IN A NARROW HALL.";
        roomDescriptions[18] = "IN A LARGE HALL.";
        roomDescriptions[19] = "ON THE TOP OF A TREE.";
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

    private int Main()
    {
        this.Init();
        ; // ** THE QUEST **
        ; // **
        ; // ** An adventure game
        ; // 
        CLS() // Put a statement here to clear the screen. If you are using a Radio Shack Model I, III, or 4, add a CLEAR statement. (See text.)
        ;
        numberOfRooms = (19);
        numberOfObjects = (17);
        numberOfDirections = (6);
        maxInventoryItems = (5);
        DIM1_sa(out roomDescriptions, (int)(numberOfRooms));
        DIM1_na(out objectRooms, (int)(numberOfObjects));
        DIM1_sa(out objectNames, (int)(numberOfObjects));
        DIM1_sa(out objectTags, (int)(numberOfObjects));
        DIM2_na(out map, (int)(numberOfRooms), (int)(numberOfDirections));
        PRINT(("") + ("Please stand by .... "));
        PRINT("");
        PRINT("");

        InitMap();

        InitObjects();

        InitDescriptions();

        currentRoom = (1);
        inventoryItems = (0);
        saltPoured = (0);
        formulaPoured = (0);
        mixtureCount = (1);
        wearingGloves = (0);

        PrintIntro();

        CLS();

        while (true)
        {
            PrintDescription();

            PrintDirections();

            PrintObjects();

            while (true)
            {
                Parser();

                if (verb == "GO")
                {
                    if (Go())
                    {
                        break;
                    }
                }
                else if ((verb == "GET") || (verb == "TAK"))
                {
                    if (Get())
                    {
                        return PlayAgain();
                    }
                }
                else if ((verb == "DRO") || (verb == "THR"))
                {
                    Drop();
                }
                else if ((verb == "INV") || (verb == "I"))
                {
                    Inventory();
                }
                else if ((verb == "LOO") || (verb == "L"))
                {
                    if (Look())
                    {
                        break;
                    }
                }
                else if (verb == "EXA")
                {
                    Examine();
                }
                else if (verb == "QUI")
                {
                    int q = Quit();
                    if (q != 0)
                    {
                        return q;
                    }
                }
                else if (verb == "REA")
                {
                    Read();
                }
                else if (verb == "OPE")
                {
                    Open();
                }
                else if (verb == "POU")
                {
                    if (Pour())
                    {
                        break;
                    }
                }
                else if (verb == "CLI")
                {
                    Climb();
                }
                else if (verb == "JUM")
                {
                    if (Jump())
                    {
                        break;
                    }
                }
                else if (verb == "DIG")
                {
                    Dig();
                }
                else if (verb == "ROW")
                {
                    Row();
                }
                else if (verb == "WAV")
                {
                    Wave();
                }
                else if ((verb == "LEA") || (verb == "EXI"))
                {
                    if (Leave())
                    {
                        break;
                    }
                }
                else if (verb == "FIG")
                {
                    Fight();
                }
                else if (verb == "WEA")
                {
                    Wear();
                }
                else
                {
                    PRINT(("") + ("I DON'T KNOW HOW TO DO THAT"));
                }
            }
        }
    }

    private void Parser()
    {
        string command = "";
        do
        {
            PRINT("");
            command = (INPUT_s("WHAT NOW"));
        }
        while (command == "");

        int c = 0;
        verb = "";
        noun = "";

        while (true)
        {
            c = c + 1;
            if (c > command.Length)
            {
                break;
            }

            string wordPart = MID_s(command, c, 1);
            if (wordPart == " ")
            {
                break;
            }

            verb += wordPart;
        }

        while (true)
        {
            c = c + 1;
            if (c > command.Length)
            {
                break;
            }

            string wordPart = MID_s(command, c, 1);
            if (wordPart == " ")
            {
                break;
            }

            noun += wordPart;
        }

        if (verb.Length > 3)
        {
            verb = LEFT_s(verb, 3);
        }

        if (noun.Length > 3)
        {
            noun = LEFT_s(noun, 3);
        }

        if (noun == "SHA")
        {
            noun = "SAL";
        }

        if (noun == "FOR")
        {
            noun = "BOT";
        }
    }

    private bool Go()
    {
        int dir;
        if (noun == "NOR")
        {
            dir = 0;
        }
        else if (noun == "SOU")
        {
            dir = 1;
        }
        else if (noun == "EAS")
        {
            dir = 2;
        }
        else if (noun == "WES")
        {
            dir = 3;
        }
        else if (noun == "UP")
        {
            dir = 4;
        }
        else if (noun == "DOW")
        {
            dir = 5;
        }
        else if ((noun == "BOA") && (objectRooms[11] == (currentRoom + 128)))
        {
            currentRoom = 13;
            return true;
        }
        else
        {
            PRINT("YOU CAN'T GO THERE!");
            return false;
        }

        return Move(dir);
    }

    private bool Move(int dir)
    {
        bool ret = false;
        if ((map[(int)(currentRoom), dir] > 0) && (map[(int)(currentRoom), dir] < 128))
        {
            currentRoom = map[(int)(currentRoom), dir];
            ret = true;
        }
        else if (map[(int)(currentRoom), dir] == 128)
        {
            PRINT("THE GUARD WON'T LET YOU!");
        }
        else
        {
            PRINT("YOU CAN'T GO THERE!");
        }

        return ret;
    }

    private bool Get()
    {
        FindRoomForObject();

        if ((((FL_n.CompareTo(0)) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T GET THAT!"));
        }
        else if ((((RO_n.CompareTo(-(1))) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU ALREADY HAVE IT!"));
        }
        else if ((((objectRooms[(int)(I_n)].CompareTo(127)) > (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T GET THAT!"));
        }
        else if ((((RO_n.CompareTo(currentRoom)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THAT'S NOT HERE!"));
        }
        else if ((((inventoryItems.CompareTo(maxInventoryItems)) > (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T CARRY ANY MORE."));
        }
        else if ((((int)(((currentRoom.CompareTo(18)) == (0)) ? (-1) : (0))) & ((int)(((noun.CompareTo("RUB")) == (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("CONGRATULATIONS! YOU'VE WON!"));
            return true;
        }
        else
        {
            inventoryItems = ((inventoryItems) + (1));
            objectRooms[(int)(I_n)] = (-(1));
            PRINT(("") + ("TAKEN."));
        }

        return false;
    }

    private void Drop()
    {
        FindRoomForObject();

        if ((((int)(((FL_n.CompareTo(0)) == (0)) ? (-1) : (0))) | ((int)(((RO_n.CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE THAT!"));
        }
        else
        {
            inventoryItems = ((inventoryItems) - (1));
            objectRooms[(int)(I_n)] = (currentRoom);
            PRINT(("") + ("DROPPED."));
        }
    }

    private void Inventory()
    {
        FL_n = (0);
        PRINT(("") + ("YOU ARE CARRYING:"));
        I_n = (0);
        while ((I_n) <= ((numberOfObjects) - (1)))
        {
            if ((((objectRooms[(int)(I_n)].CompareTo(-(1))) == (0)) ? (-1) : (0)) != (0))
            {
                PRINT((("") + (" ")) + (objectNames[(int)(I_n)]));
                FL_n = (1);
            }

            I_n = ((I_n) + (1));
        }

        if ((((FL_n.CompareTo(0)) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + (" NOTHING"));
        }
    }

    private bool Look()
    {
        bool ret = true;
        if ((((noun.CompareTo("")) != (0)) ? (-1) : (0)) != (0))
        {
            Examine();
            ret = false;
        }

        return ret;
    }

    private void Examine()
    {
        if ((((noun.CompareTo("GRO")) != (0)) ? (-1) : (0)) != (0))
        {
            FindRoomForObject();

            if ((((int)(((RO_n.CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((RO_n.CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
            {
                PRINT(("") + ("IT'S NOT HERE!"));
            }
            else if ((((noun.CompareTo("BOT")) == (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("THERE'S SOMETHING WRITTEN ON IT!"));
            }
            else if ((((noun.CompareTo("CAS")) == (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("THERE'S A JEWEL INSIDE!"));
            }
            else if ((((noun.CompareTo("BAR")) == (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("IT'S FILLED WITH RAINWATER."));
            }
            else
            {
                PRINT(("") + ("YOU SEE NOTHING UNUSUAL."));
            }
        }
        else if ((((currentRoom.CompareTo(6)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("IT LOOKS LIKE GROUND!"));
        }
        else
        {
            PRINT(("") + ("IT LOOKS LIKE SOMETHING'S BURIED HERE."));
        }
    }

    private int Quit()
    {
        PRINT_n(("") + ("ARE YOU SURE YOU WANT TO QUIT (Y/N)"));
        quit = (INPUT_s(""));
        if ((((quit.CompareTo("N")) == (0)) ? (-1) : (0)) == (0))
        {
            return PlayAgain();
        }

        return 0;
    }

    private int PlayAgain()
    {
        while (true)
        {
            PRINT_n(("") + ("WOULD YOU LIKE TO PLAY AGAIN (Y/N)"));
            quit = (INPUT_s(""));
            if ((((quit.CompareTo("Y")) == (0)) ? (-1) : (0)) != (0))
            {
                return 1;
            }

            if ((((quit.CompareTo("N")) == (0)) ? (-1) : (0)) != (0))
            {
                return 2;
            }
        }
    }

    private void Read()
    {
        if ((((noun.CompareTo("DIA")) != (0)) ? (-1) : (0)) != (0))
        {
            if ((((noun.CompareTo("DIC")) != (0)) ? (-1) : (0)) != (0))
            {
                if ((((noun.CompareTo("BOT")) != (0)) ? (-1) : (0)) != (0))
                {
                    PRINT(("") + ("YOU CAN'T READ THAT!"));
                }
                else if ((((int)(((objectRooms[(int)(6)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(6)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
                {
                    PRINT(("") + ("THERE'S NO BOTTLE HERE!"));
                }
                else
                {
                    PRINT(("") + ("IT READS: 'SECRET FORMULA'."));
                }
            }
            else if ((((int)(((objectRooms[(int)(4)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(4)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
            {
                PRINT(("") + ("YOU DON'T SEE A DICTIONARY!"));
            }
            else
            {
                PRINT(("") + ("IT SAYS: SODIUM CHLORIDE IS"));
                PRINT(("") + ("COMMON TABLE SALT."));
            }
        }
        else if ((((int)(((objectRooms[(int)(0)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(0)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("THERE'S NO DIARY HERE!"));
        }
        else
        {
            PRINT(("") + ("IT SAYS: 'ADD SODIUM CHLORIDE PLUS THE"));
            PRINT(("") + ("FORMULA TO RAINWATER, TO REACH THE"));
            PRINT(("") + ("OTHER WORLD.' "));
        }
    }

    private void Open()
    {
        if ((((noun.CompareTo("BOX")) != (0)) ? (-1) : (0)) != (0))
        {
            if ((((noun.CompareTo("CAB")) != (0)) ? (-1) : (0)) != (0))
            {
                if ((((noun.CompareTo("CAS")) != (0)) ? (-1) : (0)) != (0))
                {
                    PRINT(("") + ("YOU CAN'T OPEN THAT!"));
                }
                else if ((((currentRoom.CompareTo(18)) != (0)) ? (-1) : (0)) != (0))
                {
                    PRINT(("") + ("THERE'S NO CASE HERE!"));
                }
                else if ((((wearingGloves.CompareTo(1)) != (0)) ? (-1) : (0)) != (0))
                {
                    PRINT(("") + ("THE CASE IS ELECTRIFIED!"));
                }
                else
                {
                    PRINT(("") + ("THE GLOVES INSULATE AGAINST THE"));
                    PRINT(("") + ("ELECTRICITY! THE CASE OPENS!"));
                    objectRooms[(int)(15)] = (18);
                }
            }
            else if ((((currentRoom.CompareTo(2)) != (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("THERE'S NO CABINET HERE!"));
            }
            else
            {
                PRINT(("") + ("THERE'S SOMETHING INSIDE!"));
                objectRooms[(int)(3)] = (2);
            }
        }
        else if ((((int)(((objectRooms[(int)(1)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(1)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("THERE'S NO BOX HERE!"));
        }
        else
        {
            objectRooms[(int)(6)] = (currentRoom);
            PRINT(("") + ("SOMETHING FELL OUT!"));
        }
    }

    private bool Pour()
    {
        bool ret;
        if ((((noun.CompareTo("SAL")) != (0)) ? (-1) : (0)) != (0))
        {
            ret = PourFormula();
        }
        else
        {
            ret = PourSalt();
        }

        if (ret)
        {
            ret = PourMixture();
        }

        return ret;
    }

    private bool PourFormula()
    {
        if ((((noun.CompareTo("BOT")) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T POUR THAT!"));
            return false;
        }
        else if ((((int)(((objectRooms[(int)(6)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(6)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE THE BOTTLE!"));
            return false;
        }
        else if ((((formulaPoured.CompareTo(1)) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THE BOTTLE IS EMPTY!"));
            return false;
        }
        else
        {
            formulaPoured = (1);
            return true;
        }
    }

    private bool PourSalt()
    {
        if ((((int)(((objectRooms[(int)(3)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(3)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE THE SALT!"));
            return false;
        }
        else if ((((saltPoured.CompareTo(1)) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THE SHAKER IS EMPTY!"));
            return false;
        }
        else
        {
            saltPoured = (1);
            return true;
        }
    }

    private bool PourMixture()
    {
        if ((((currentRoom.CompareTo(5)) == (0)) ? (-1) : (0)) != (0))
        {
            mixtureCount = ((mixtureCount) + (1));
        }

        PRINT(("") + ("POURED!"));

        bool ret;
        if ((((mixtureCount.CompareTo(3)) < (0)) ? (-1) : (0)) != (0))
        {
            ret = false;
        }
        else
        {
            PRINT(("") + ("THERE IS AN EXPLOSION!"));
            PRINT(("") + ("EVERYTHING GOES BLACK!"));
            PRINT(("") + ("SUDDENLY YOU ARE ... "));
            PRINT(("") + (" ... SOMEWHERE ELSE!"));
            currentRoom = (6);
            ret = true;
        }

        return ret;
    }

    private void Climb()
    {
        if ((((noun.CompareTo("TRE")) != (0)) ? (-1) : (0)) != (0))
        {
            if ((((noun.CompareTo("LAD")) != (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("IT WON'T DO ANY GOOD."));
            }
            else if ((((int)(((objectRooms[(int)(7)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(7)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
            {
                PRINT(("") + ("YOU DON'T HAVE THE LADDER!"));
            }
            else if ((((currentRoom.CompareTo(7)) != (0)) ? (-1) : (0)) != (0))
            {
                PRINT(("") + ("WHATEVER FOR?"));
            }
            else
            {
                PRINT(("") + ("THE LADDER SINKS UNDER YOUR WEIGHT!"));
                PRINT(("") + ("IT DISAPPEARS INTO THE GROUND!"));
                objectRooms[(int)(7)] = (0);
            }
        }
        else if ((((currentRoom.CompareTo(7)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THERE'S NO TREE HERE!"));
        }
        else
        {
            PRINT(("") + ("YOU CAN'T REACH THE BRANCHES!"));
        }
    }

    private bool Jump()
    {
        if ((((int)(((currentRoom.CompareTo(7)) != (0)) ? (-1) : (0))) & ((int)(((currentRoom.CompareTo(8)) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("WHEE! THAT WAS FUN!"));
            return false;
        }
        else if ((((currentRoom.CompareTo(8)) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU GRAB A HIGHER BRANCH ON THE"));
            PRINT(("") + ("TREE AND PULL YOURSELF UP...."));
            currentRoom = (19);
            return true;
        }
        else
        {
            PRINT(("") + ("YOU GRAB THE LOWEST BRANCH OF THE"));
            PRINT(("") + ("TREE AND PULL YOURSELF UP...."));
            currentRoom = (8);
            return true;
        }
    }

    private void Dig()
    {
        if ((((int)(((int)(((noun.CompareTo("HOL")) != (0)) ? (-1) : (0))) & ((int)(((noun.CompareTo("GRO")) != (0)) ? (-1) : (0))))) & ((int)(((noun.CompareTo("")) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU CAN'T DIG THAT!"));
        }
        else if ((((int)(((objectRooms[(int)(8)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(8)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE A SHOVEL!"));
        }
        else if ((((currentRoom.CompareTo(6)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU DON'T FIND ANYTHING."));
        }
        else if ((((objectRooms[(int)(10)].CompareTo(0)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THERE'S NOTHING ELSE THERE!"));
        }
        else
        {
            PRINT(("") + ("THERE'S SOMETHING THERE!"));
            objectRooms[(int)(10)] = (6);
        }
    }

    private void Row()
    {
        if ((((int)(((noun.CompareTo("BOA")) != (0)) ? (-1) : (0))) & ((int)(((noun.CompareTo("")) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("HOW CAN YOU ROW THAT?"));
        }
        else if ((((currentRoom.CompareTo(13)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU'RE NOT IN THE BOAT!"));
        }
        else
        {
            PRINT(("") + ("YOU DON'T HAVE AN OAR!"));
        }
    }

    private void Wave()
    {
        if ((((noun.CompareTo("FAN")) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T WAVE THAT!"));
        }
        else if ((((int)(((objectRooms[(int)(12)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(12)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE THE FAN!"));
        }
        else if ((((currentRoom.CompareTo(13)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU FEEL A REFRESHING BREEZE!"));
        }
        else
        {
            PRINT(("") + ("A POWERFUL BREEZE PROPELS THE BOAT"));
            PRINT(("") + ("TO THE OPPOSITE SHORE!"));
            if ((((objectRooms[(int)(11)].CompareTo(140)) == (0)) ? (-1) : (0)) != (0))
            {
                objectRooms[(int)(11)] = (142);
            }
            else
            {
                objectRooms[(int)(11)] = (140);
            }
        }
    }

    private bool Leave()
    {
        if ((((currentRoom.CompareTo(13)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("PLEASE GIVE A DIRECTION!"));
            return false;
        }
        else if ((((int)(((noun.CompareTo("BOA")) != (0)) ? (-1) : (0))) & ((int)(((noun.CompareTo("")) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("HUH?"));
            return false;
        }
        else
        {
            currentRoom = ((objectRooms[(int)(11)]) - (128));
            return true;
        }
    }

    private void Fight()
    {
        if ((((noun.CompareTo("")) == (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("WHOM DO YOU WANT TO FIGHT?"));
        }
        else if ((((noun.CompareTo("GUA")) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T FIGHT HIM!"));
        }
        else if ((((currentRoom.CompareTo(16)) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("THERE'S NO GUARD HERE!"));
        }
        else if ((((objectRooms[(int)(10)].CompareTo(-(1))) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE A WEAPON!"));
        }
        else
        {
            PRINT(("") + ("THE GUARD, NOTICING YOUR SWORD,"));
            PRINT(("") + ("WISELY RETREATS INTO THE CASTLE."));
            map[(int)(16), (int)(0)] = (17);
            objectRooms[(int)(13)] = (0);
        }
    }

    private void Wear()
    {
        if ((((noun.CompareTo("GLO")) != (0)) ? (-1) : (0)) != (0))
        {
            PRINT(("") + ("YOU CAN'T WEAR THAT!"));
        }
        else if ((((int)(((objectRooms[(int)(16)].CompareTo(currentRoom)) != (0)) ? (-1) : (0))) & ((int)(((objectRooms[(int)(16)].CompareTo(-(1))) != (0)) ? (-1) : (0)))) != (0))
        {
            PRINT(("") + ("YOU DON'T HAVE THE GLOVES."));
        }
        else
        {
            PRINT(("") + ("YOU ARE NOW WEARING THE GLOVES."));
            wearingGloves = (1);
        }
    }
}