using Godot;
using System;

public partial class MapGeneratorSmall : Node
{
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

        TempRoom[,] roomTemp = new TempRoom[12, 12];

        for (x = 0; x < 12; x++)
        {
            for (y = 0; y < 12; y++)
            {
                //roomArray[x][y] = nullptr; - does not work, but is not necessary
                roomTemp[x, y].type = RoomTypes.ROOM1; //fill the data with values
                roomTemp[x, y].angle = -1;
            }
        }

        x = 6;
        y = 11;

        /*for (int i = y; i < 16; i++)
        {
            roomTemp[x, i].angle = 0; //fill angles
        }*/

        while (y >= 2)
        {
            width = (rand.RandiRange(0, 2)) + 6; //map width

            if (x > 8)
            {
                width = -width;
            }
            else if (x > 6)
            {
                x = x - 6;
            }

            //make sure the hallway doesn't go outside the array
            if (x + width > 10)
            {
                width = 10 - x;
            }
            else if (x + width < 2)
            {
                width = -x + 2;
            }

            x = Mathf.Min(x, (x + width));
            width = Mathf.Abs(width);
            for (int i = x; i <= x + width; i++)
            {
                roomTemp[Mathf.Min(i, 11), y].angle = 0;
            }

            //height is random
            height = (rand.RandiRange(0, 1)) + 2;
            if (y - height < 1) height = y;
            //height for each zone
            int yhallways = (rand.RandiRange(0, 1)) + 1;

            //this FOR loop cleans the mapgen from loops.
            for (int i = 1; i <= yhallways; i++)
            {
                x2 = Mathf.Max(Mathf.Min((rand.RandiRange(0, width - 2)) + x, 8), 2);
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
        for (x = 0; x < 12; x++)
        {
            for (y = 0; y < 12; y++)
            {
                bool hasNorth, hasSouth, hasEast, hasWest;
                hasNorth = hasSouth = hasEast = hasWest = false;
                if (roomTemp[x,y].angle == 0)
                { //this is not a checkpoint room
                    if (x > 0)
                    {
                        hasWest = (roomTemp[x - 1,y].angle > -1);
                    }
                    if (x < 11)
                    {
                        hasEast = (roomTemp[x + 1,y].angle > -1);
                    }
                    if (y > 0)
                    {
                        hasNorth = (roomTemp[x,y - 1].angle > -1);
                    }
                    if (y < 11)
                    {
                        hasSouth = (roomTemp[x,y + 1].angle > -1);
                    }
                    if (hasNorth && hasSouth)
                    {
                        if (hasEast && hasWest)
                        { //room4
                            float[] avAngle = new float[] {0, 90, 180, 270};
                            roomTemp[x,y].type = RoomTypes.ROOM4;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 3)];
                        }
                        else if (hasEast && !hasWest)
                        { //room3, pointing east
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x,y].angle = 90;
                        }
                        else if (!hasEast && hasWest)
                        { //room3, pointing west
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x,y].angle = 270;
                        }
                        else
                        { //vertical room2
                            float[] avAngle = new float[] {0, 180};
                            roomTemp[x,y].type = RoomTypes.ROOM2;
                            roomTemp[x,y].angle = avAngle[rand.RandiRange(0, 1)];
                        }
                    }
                    else if (hasEast && hasWest)
                    {
                        if (hasNorth && !hasSouth)
                        { //room3, pointing north
                            roomTemp[x,y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 180;
                        }
                        else if (!hasNorth && hasSouth)
                        { //room3, pointing south
                            roomTemp[x, y].type = RoomTypes.ROOM3;
                            roomTemp[x, y].angle = 0;
                        }
                        else
                        { //horizontal room2
                            float[] avAngle = new float[] {90, 270};
                            roomTemp[x, y].type = RoomTypes.ROOM2;
                            roomTemp[x, y].angle = avAngle[rand.RandiRange(0, 1)];
                        }
                    }
                    else if (hasNorth)
                    {
                        if (hasEast)
                        { //room2c, north-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 90;
                        }
                        else if (hasWest)
                        { //room2c, north-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 180;
                        }
                        else
                        { //room1, north
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 180;
                        }
                    }
                    else if (hasSouth)
                    {
                        if (hasEast)
                        { //room2c, south-east
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 0;
                        }
                        else if (hasWest)
                        { //room2c, south-west
                            roomTemp[x, y].type = RoomTypes.ROOM2C;
                            roomTemp[x, y].angle = 270;
                        }
                        else
                        { //room1, south
                            roomTemp[x, y].type = RoomTypes.ROOM1;
                            roomTemp[x, y].angle = 0;
                        }
                    }
                    else if (hasEast)
                    { //room1, east
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 90;
                    }
                    else
                    { //room1, west
                        roomTemp[x, y].type = RoomTypes.ROOM1;
                        roomTemp[x, y].angle = 270;
                    }
                }
                else
                {
                    roomTemp[x, y].type = RoomTypes.EMPTY;
                }
            }
        }

        /*for (x = 0; x < roomTemp.GetLength(0); x++)
        {
            for (y = 0; y < roomTemp.GetLength(1); y++)
            {
                switch (roomTemp[y, x].angle)
                {
                    case -1:
                        Console.Write("#" + " ");
                        break;
                    default:
                        Console.Write((int)roomTemp[y, x].type + " ");
                        break;
                }
                
            }
            Console.WriteLine();
        }*/

        int currRoom1 = 0;
        int currRoom2 = 0;
        
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                StaticBody3D rm;
                switch (roomTemp[i, j].type)
                {
                    case RoomTypes.ROOM1:
                        
                        switch (currRoom1)
                        {
                            case 0:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_cont_1_079.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom1++;
                                break;
                            case 1:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_archive.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom1++;
                                break;
                            default:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM1/lc_room_1_endroom.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                        }
                        break;
                    case RoomTypes.ROOM2:
                        
                        switch (currRoom2)
                        {
                            case 0:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2_hall.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom2++;
                                break;
                            case 1:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_cont_2_650.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom2++;
                                break;
                            case 2:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_cont_2_012.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom2++;
                                break;
                            case 3:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2_vent.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom2++;
                                break;
                            case 4:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2_sl.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                currRoom2++;
                                break;
                            default:
                                rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2/lc_room_2.tscn").Instantiate();
                                rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                                rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                                AddChild(rm);
                                break;
                        }
                        break;
                    case RoomTypes.ROOM2C:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM2C/lc_room_2c.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM3:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM3/lc_room_3.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
                        rm.RotationDegrees = new Vector3(0, roomTemp[i, j].angle, 0);
                        AddChild(rm);
                        break;
                    case RoomTypes.ROOM4:
                        rm = (StaticBody3D)ResourceLoader.Load<PackedScene>("res://MapGen/Resources/ROOM4/lc_room_4.tscn").Instantiate();
                        rm.Position = new Vector3(i * 20.48f, 0, j*20.48f);
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