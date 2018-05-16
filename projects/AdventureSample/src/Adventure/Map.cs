// <copyright file="Map.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace Adventure
{
    using System.Collections.Generic;

    internal sealed class Map
    {
        private const int NumberOfRooms = 19;
        private const int NumberOfDirections = 6;

        private string[] roomDescriptions;
        private string[] directions;
        private int[,] map;

        public Map()
        {
            this.directions = new string[11];
            this.InitMap();
            this.CurrentRoom = 1;
        }

        public int CurrentRoom { get; set; }

        public IEnumerable<string> Directions()
        {
            for (int i = 0; i <= 5; ++i)
            {
                if (this.map[this.CurrentRoom, i] > 0)
                {
                    yield return this.directions[i];
                }
            }
        }

        public string Describe()
        {
            return this.roomDescriptions[this.CurrentRoom];
        }

        public void SetMap(int room, int dir, int next)
        {
            this.map[room, dir] = next;
        }

        public MoveResult Move(int dir)
        {
            int next = this.map[this.CurrentRoom, dir];
            if (next == 128)
            {
                return MoveResult.Blocked;
            }

            if ((next <= 0) || (next > 128))
            {
                return MoveResult.Invalid;
            }

            this.CurrentRoom = next;
            return MoveResult.OK;
        }

        private void InitMap()
        {
            this.map = new int[NumberOfRooms + 1, NumberOfDirections + 1];

            this.directions[0] = "NORTH";
            this.directions[1] = "SOUTH";
            this.directions[2] = "EAST";
            this.directions[3] = "WEST";
            this.directions[4] = "UP";
            this.directions[5] = "DOWN";

            Queue<int> data = new Queue<int>();

            // LIVING ROOM
            data.Enqueue(4);
            data.Enqueue(3);
            data.Enqueue(2);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // KITCHEN
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(1);
            data.Enqueue(0);
            data.Enqueue(0);

            // LIBRARY
            data.Enqueue(1);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // FRONT YARD
            data.Enqueue(0);
            data.Enqueue(1);
            data.Enqueue(0);
            data.Enqueue(5);
            data.Enqueue(0);
            data.Enqueue(0);

            // GARAGE
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(4);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // OPEN FIELD
            data.Enqueue(9);
            data.Enqueue(7);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // EDGE OF FOREST
            data.Enqueue(6);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // BRANCH OF TREE
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(7);

            // LONG, WINDING ROAD (1)
            data.Enqueue(0);
            data.Enqueue(6);
            data.Enqueue(10);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // LONG, WINDING ROAD (2)
            data.Enqueue(11);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(9);
            data.Enqueue(0);
            data.Enqueue(0);

            // LONG, WINDING ROAD (3)
            data.Enqueue(0);
            data.Enqueue(10);
            data.Enqueue(0);
            data.Enqueue(12);
            data.Enqueue(0);
            data.Enqueue(0);

            // SOUTH BANK OF RIVER
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(11);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // BOAT
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // NORTH BANK OF RIVER
            data.Enqueue(15);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // WELL-TRAVELED ROAD
            data.Enqueue(16);
            data.Enqueue(14);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // SOUTH OF CASTLE
            data.Enqueue(128);
            data.Enqueue(15);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);

            // NARROW HALL
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(18);
            data.Enqueue(0);

            // LARGE HALL
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(17);

            // TOP OF TREE
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(0);
            data.Enqueue(8);

            for (int i = 1; i <= NumberOfRooms; ++i)
            {
                for (int j = 0; j < NumberOfDirections; ++j)
                {
                    this.SetMap(i, j, data.Dequeue());
                }
            }

            this.InitDescriptions();
        }

        private void InitDescriptions()
        {
            this.roomDescriptions = new string[NumberOfRooms + 1]
            {
                string.Empty,
                "IN YOUR LIVING ROOM.",
                "IN THE KITCHEN.",
                "IN THE LIBRARY.",
                "IN THE FRONT YARD.",
                "IN THE GARAGE.",
                "IN AN OPEN FIELD.",
                "AT THE EDGE OF A FOREST.",
                "ON A BRANCH OF A TREE.",
                "ON A LONG, WINDING ROAD.",
                "ON A LONG, WINDING ROAD.",
                "ON A LONG, WINDING ROAD.",
                "ON THE SOUTH BANK OF A RIVER.",
                "INSIDE THE WOODEN BOAT.",
                "ON THE NORTH BANK OF A RIVER.",
                "ON A WELL-TRAVELED ROAD.",
                "IN FRONT OF A LARGE CASTLE.",
                "IN A NARROW HALL.",
                "IN A LARGE HALL.",
                "ON THE TOP OF A TREE."
            };
        }
    }
}