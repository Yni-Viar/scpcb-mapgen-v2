using Godot;
using System;

public partial class MapGeneratorLarge : Node
{
    [Export] bool generateMoreR1s = false;
    [Export] bool generateMoreR4s = false;
    //(c) juanjp600. License - CC-BY-SA 3.0.
    internal enum RoomTypes { ROOM1, ROOM2, ROOM2C, ROOM3, ROOM4, EMPTY };
    int room1Amount, room2Amount, room2cAmount, room3Amount, room4Amount;
    internal struct TempRoom
    {
        internal RoomTypes type;
        /* angle can be:
		   * -1: do not spawn a room
		   * 0: means 0° rotation; facing east
		   * 1: means 90° rotation; facing north
		   * 2: means 180° rotation; facing west
		   * 3: means 270° rotation; facing south
		*/
        internal float angle;
    };

    void CreateMap()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        int x, y, temp;
        int x2, y2;
        int width, height;

        TempRoom[,] roomTemp = new TempRoom[20, 20];

        for (x = 0; x < 20; x++)
        {
            for (y = 0; y < 20; y++)
            {
                //roomArray[x][y] = nullptr; - does not work, but is not necessary
                roomTemp[x, y].type = RoomTypes.ROOM1; //fill the data with values
                roomTemp[x, y].angle = -1;
            }
        }

        x = 10;
        y = 18;

        for (int i = y; i < 20; i++)
        {
            roomTemp[x, i].angle = 0; //fill angles
        }

        while (y >= 2)
        {
            width = (rand.RandiRange(0, 5)) + 10; //map width

            if (x > 12)
            {
                width = -width;
            }
            else if (x > 8)
            {
                x = x - 10;
            }

            //make sure the hallway doesn't go outside the array
            if (x + width > 17)
            {
                width = 17 - x;
            }
            else if (x + width < 2)
            {
                width = -x + 2;
            }

            x = Math.Min(x, (x + width));
            width = Math.Abs(width);
            for (int i = x; i <= x + width; i++)
            {
                roomTemp[Math.Min(i, 19), y].angle = 0;
            }

            //height is random
            height = (rand.RandiRange(0, 1)) + 3;
            if (y - height < 1) height = y;
            //height for each zone
            int yhallways = (rand.RandiRange(0, 1)) + 4;

            for (int i = 1; i <= yhallways; i++)
            {
                x2 = Math.Max(Math.Min((rand.RandiRange(0, width - 2)) + x, 18), 2);
                while (roomTemp[x2, y - 1].angle >= 0 || roomTemp[x2 - 1, y - 1].angle >= 0 || roomTemp[x2 + 1, y - 1].angle >= 0)
                {
                    x2++;
                }

                if (x2 < x + width)
                {
                    int tempheight;
                    if (i == 1)
                    {
                        tempheight = height;
                        if (rand.RandiRange(0, 1) == 0) x2 = x; else x2 = x + width;
                    }
                    else
                    {
                        tempheight = (rand.RandiRange(0, height - 1)) + 1;
                    }

                    for (y2 = y - tempheight; y2 <= y; y2++)
                    {
                        roomTemp[x2, y2].angle = 0;
                    }

                    if (tempheight == height) temp = x2;
                }
            }

            y -= height;
        }

        room1Amount = room2Amount = room2cAmount = room3Amount = room4Amount = 0;
        for (x = 0; x < 20; x++)
        {
            for (y = 0; y < 20; y++)
            {
                bool hasNorth, hasSouth, hasEast, hasWest;
                hasNorth = hasSouth = hasEast = hasWest = false;
                if (roomTemp[x, y].angle == 0)
                { //this is not a checkpoint Room
                    if (x > 0)
                    {
                        hasWest = (roomTemp[x - 1, y].angle > -1);
                    }
                    if (x < 19)
                    {
                        hasEast = (roomTemp[x + 1, y].angle > -1);
                    }
                    if (y > 0)
                    {
                        hasNorth = (roomTemp[x, y - 1].angle > -1);
                    }
                    if (y < 19)
                    {
                        hasSouth = (roomTemp[x, y + 1].angle > -1);
                    }
                    if (hasNorth && hasSouth)
                    {
                        if (hasEast && hasWest)
                        { //room4
                            float[] avAngle = new float[] { 0, 90, 180, 270 };
                            roomTemp[x, y].type = RoomTypes.ROOM4;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 3)];
                            room4Amount++;
                        }
                        else if (hasEast && !hasWest)
                        { //room3, pointing east
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 90;
                            room3Amount++;
                        }
                        else if (!hasEast && hasWest)
                        { //room3, pointing west
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 270;
                            room3Amount++;
                        }
                        else
                        { //vertical room2
                            float[] avAngle = new float[] { 0, 180 };
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                            room2Amount++;
                        }
                    }
                    else if (hasEast && hasWest)
                    {
                        if (hasNorth && !hasSouth)
                        { //room3, pointing north
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 180;
                            room3Amount++;
                        }
                        else if (!hasNorth && hasSouth)
                        { //room3, pointing south
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 0;
                            room3Amount++;
                        }
                        else
                        { //horizontal room2
                            float[] avAngle = new float[] { 90, 270 };
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                            room2Amount++;
                        }
                    }
                    else if (hasNorth)
                    {
                        if (hasEast)
                        { //room2c, north-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 90;
                            room2cAmount++;
                        }
                        else if (hasWest)
                        { //room2c, north-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 180;
                            room2cAmount++;
                        }
                        else
                        { //room1, north
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 180;
                            room1Amount++;
                        }
                    }
                    else if (hasSouth)
                    {
                        if (hasEast)
                        { //room2c, south-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 0;
                            room2cAmount++;
                        }
                        else if (hasWest)
                        { //room2c, south-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 270;
                            room2cAmount++;
                        }
                        else
                        { //room1, south
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 0;
                            room1Amount++;
                        }
                    }
                    else if (hasEast)
                    { //room1, east
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 90;
                        room1Amount++;
                    }
                    else
                    { //room1, west
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 270;
                        room1Amount++;
                    }
                }
                else
                {
                    roomTemp[x, y].type = RoomTypes.EMPTY;
                }
            }
        }

        if (room1Amount < 5 && generateMoreR1s)
        {
            GD.Print("Forcing some ROOM1s");
            for (y = 2; y < 19 && room1Amount < 5; y++)
            {
                //if (getZone(y+2) == i && getZone(y-2) == i) {
                for (x = 2; x < 19 && room1Amount < 5; x++)
                {
                    if (roomTemp[x,y].angle < 0)
                    {
                        bool freeSpace = ((roomTemp[x + 1,y].angle >= 0) != (roomTemp[x - 1,y].angle >= 0)) != ((roomTemp[x,y + 1].angle >= 0) != (roomTemp[x,y - 1].angle >= 0));
                        freeSpace = freeSpace && (((roomTemp[x + 2,y].angle >= 0) != (roomTemp[x - 2,y].angle >= 0)) != ((roomTemp[x,y + 2].angle >= 0) != (roomTemp[x,y - 2].angle >= 0)));
                        freeSpace = freeSpace && (((roomTemp[x + 1,y + 1].angle >= 0) != (roomTemp[x - 1,y - 1].angle >= 0)) != ((roomTemp[x - 1, y + 1].angle >= 0) != (roomTemp[x + 1,y - 1].angle >= 0)));
                        if (freeSpace)
                        {
                            TempRoom adjRoom;
                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x + 1,y];
                                roomTemp[x, y].angle = 90;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x - 1,y];
                                roomTemp[x, y].angle = 270;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                adjRoom = roomTemp[x,y + 1];
                                roomTemp[x, y].angle = 0;
                            }
                            else
                            {
                                adjRoom = roomTemp[x, y - 1];
                                roomTemp[x, y].angle = 180;
                            }

                            switch (adjRoom.type)
                            {
                                /*case RoomTypes.ROOM2:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    room1Amount++;
                                    room2Amount--;
                                    room3Amount++;
                                    adjRoom.type = RoomTypes.ROOM3;
                                    switch (roomTemp[x, y].angle)
                                    {
                                        case 0:
                                            adjRoom.angle = 0;
                                            break;
                                        case 1:
                                            adjRoom.angle = 90;
                                            break;
                                        case 2:
                                            adjRoom.angle = 180;
                                            break;
                                        case 3:
                                            adjRoom.angle = 270;
                                            break;
                                    }
                                    break;*/
                                case RoomTypes.ROOM3:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    adjRoom.type = RoomTypes.ROOM4;
                                    room1Amount++;
                                    room3Amount--;
                                    room4Amount++;
                                    break;
                                default:
                                    roomTemp[x, y].angle = -1;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        if (room4Amount < 3 && generateMoreR4s)
        {
            GD.Print("Forcing some ROOM4s\n");
            for (y = 2; y < 19 && room4Amount < 3; y++)
            {
                //if (getZone(y+2) == i && getZone(y-2) == i) {
                for (x = 2; x < 19 && room4Amount < 3; x++)
                {
                    if (roomTemp[x, y].angle < 0)
                    {
                        bool freeSpace = ((roomTemp[x + 1, y].angle >= 0) != (roomTemp[x - 1, y].angle >= 0)) != ((roomTemp[x, y + 1].angle >= 0) != (roomTemp[x, y - 1].angle >= 0));
                        freeSpace = freeSpace && (((roomTemp[x + 2, y].angle >= 0) != (roomTemp[x - 2, y].angle >= 0)) != ((roomTemp[x, y + 2].angle >= 0) != (roomTemp[x, y - 2].angle >= 0)));
                        freeSpace = freeSpace && (((roomTemp[x + 1, y + 1].angle >= 0) != (roomTemp[x - 1, y - 1].angle >= 0)) != ((roomTemp[x - 1, y + 1].angle >= 0) != (roomTemp[x + 1, y - 1].angle >= 0)));
                        if (freeSpace)
                        {
                            TempRoom adjRoom;

                            if (roomTemp[x + 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x + 1, y];
                                roomTemp[x, y].angle = 3;
                            }
                            else if (roomTemp[x - 1, y].angle >= 0)
                            {
                                adjRoom = roomTemp[x - 1, y];
                                roomTemp[x, y].angle = 1;
                            }
                            else if (roomTemp[x, y + 1].angle >= 0)
                            {
                                adjRoom = roomTemp[x, y + 1];
                                roomTemp[x, y].angle = 2;
                            }
                            else
                            {
                                adjRoom = roomTemp[x, y - 1];
                                roomTemp[x, y].angle = 0;
                            }

                            switch (adjRoom.type)
                            {
                                case RoomTypes.ROOM3:
                                    roomTemp[x, y].type = RoomTypes.ROOM1;
                                    adjRoom.type = RoomTypes.ROOM4;
                                    room1Amount++;
                                    room3Amount--;
                                    room4Amount++;
                                    break;
                                default:
                                    roomTemp[x, y].angle = -1;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        for (x = 0; x < roomTemp.GetLength(0); x++)
        {
            for (y = 0; y < roomTemp.GetLength(1); y++)
            {
                switch (roomTemp[y, x].angle)
                {
                    case -1:
                        GD.Print("#" + " ");
                        break;
                    default:
                        GD.Print((int)roomTemp[y, x].type + " ");
                        break;
                }
                
            }
            GD.Print();
        }

        string selectedRoom;
        int currRoom1 = 0;
        int currRoom2 = 0;

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                StaticBody3D rm;
                switch (roomTemp[i, j].type)
                {
                    case RoomTypes.ROOM1:
                        if (currRoom1 >= RoomParser.ReadJson("user://rooms.json")["LczSingle1"].Count)
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms.json")["LczCommon1"][rand.RandiRange(0, RoomParser.ReadJson("user://rooms.json")["LczCommon1"].Count - 1)];
                        }
                        else
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms.json")["LczSingle1"][currRoom1];
                        }
                        currRoom1++;
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/" + selectedRoom + ".tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM2:
                        if (currRoom2 >= RoomParser.ReadJson("user://rooms.json")["LczSingle2"].Count)
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms.json")["LczCommon2"][rand.RandiRange(0, RoomParser.ReadJson("user://rooms.json")["LczCommon1"].Count - 1)];
                        }
                        else
                        {
                            selectedRoom = RoomParser.ReadJson("user://rooms.json")["LczSingle2"][currRoom2];
                        }
                        currRoom2++;
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/" + selectedRoom + ".tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM2C:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/lc_room_2c.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM3:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/lc_room_3.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM4:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/lc_room_4.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j * 20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                }
            }
        }
    }

    public override void _Ready()
    {
        CreateMap();
    }
}