using Godot;
using System;
using System.Threading.Tasks;

//Made by Virtual + some changes by Yni (License - CC-BY-SA)
public partial class MapGenerator : Node
{

    //LCZ - HCZ - EZ transitions
    //public int[] Transitions = new int[] { 12 };
    static System.Random rnd = new Random();
    //seed
    public int RandomSeed = rnd.Next(0, 2147483647);
    
    [Export] public Vector2I MapSize = new Vector2I(12, 12);

    public enum RoomType : int
    {
        ROOM1,
        ROOM2,
        ROOM2C,
        ROOM3,
        ROOM4,
    }

    public int GetZone(int y) //LCZ, HCZ or EZ.
    {
        int zone = 1;
        /*
        if (y >= Transitions[1] && y < Transitions[0])
        {
            zone = 2;
        }
        if (y >= Transitions[0])
        {
            zone = 3;
        }
        */ // if needed
        return zone;
    }

    //checks
    public bool IsInBounds(int[,] arr, int d1)
    {
        return d1 >= 0 && d1 < arr.GetLength(0);
    }

    public async Task CreateMap()
    {
        System.Random rng = new System.Random();
        //output map size
        string[,] MapName = new string[MapSize.X, MapSize.Y];
        //actual map size
        int[,] MapTemp = new int[MapSize.X, MapSize.Y];
        //???
        int[] MapRoomId = new int[5];
        //needed to be not out-of-bounds
        int x = MapSize.X / 2;
        int y = MapSize.Y - 2;
        int temp = 0;

        //probably check
        for (int i = y; i < MapSize.Y; i++)
        {
            MapTemp[x, i] = 1;
        }

        while (y >= 2)
        {
            //map width
            int width = rng.Next(6, 10);

            if (x > MapSize.X * 0.6f)
            {
                width = -width;
            }
            else if (x > MapSize.X * 0.4f)
            {
                x = x - width / 2;
            }

            if (x + width > MapSize.X - 3)
            {
                width = MapSize.X - 3 - x;
            }
            else if (x + width < 2)
            {
                width = -x + 2;
            }

            x = Mathf.Min(x, x + width);
            width = Mathf.Abs(width);
            //probably map generation
            for (int i = x; i < x + width; i++)
            {
                MapTemp[Mathf.Min(i, MapSize.X), y] = 1;
            }
            //map zone height
            int height = rng.Next(3, 5);

            if (y - height < 1)
                height = y - 1;
            //yhallways is important in spawning corridors not in line
            int yhallways = rng.Next(4, 6);

            if (GetZone(y - height) != GetZone(y - height - 1))
                height--;

            for (int i = 0; i < yhallways; i++) //spawning corridors not in line but in a labyrinth
            {
                int x2 = Mathf.Max(Mathf.Min(rng.Next(x, x + width), MapSize.X - 2), 2);
                //checks
                while (IsInBounds(MapTemp, x2) && IsInBounds(MapTemp, x2 - 1) && IsInBounds(MapTemp, x2 + 1) && (MapTemp[x2, y - 1] >= 1 || MapTemp[x2 - 1, y - 1] >= 1 || MapTemp[x2 + 1, y - 1] >= 1))
                {
                    x2++;
                }

                if (x2 < x + width)
                {
                    int tempheight;
                    if (i == 0)
                    {
                        tempheight = height;
                        if (rng.Next(1, 3) == 1)
                            x2 = x;
                        else
                            x2 = x + width;
                    }
                    else
                    {
                        tempheight = rng.Next(1, height + 1);
                    }

                    for (int y2 = y - tempheight; y2 < y; y2++)
                    {
                        if (GetZone(y2) != GetZone(y2 + 1))
                            MapTemp[x2, y2] = 255;
                        else
                            MapTemp[x2, y2] = 1;
                    }

                    if (tempheight == height)
                    {
                        temp = x2;
                    }
                }
            }

            x = temp;
            y = y - height;
        }
        //rooms and zone amount
        int ZoneAmount = 1; //if you want more zones (CB-style), you can add this value.
        int[] Room1Amount = new int[ZoneAmount];
        int[] Room2Amount = new int[ZoneAmount];
        int[] Room2CAmount = new int[ZoneAmount];
        int[] Room3Amount = new int[ZoneAmount];
        int[] Room4Amount = new int[ZoneAmount];

        for (y = 1; y < MapSize.Y - 1; y++)
        {
            int zone = GetZone(y) - 1;

            for (x = 1; x < MapSize.X - 1; x++)
            {
                if (MapTemp[x, y] > 0)
                {
                    temp = Mathf.Min(MapTemp[x + 1, y], 1) + Mathf.Min(MapTemp[x - 1, y], 1);
                    temp = temp + Mathf.Min(MapTemp[x, y + 1], 1) + Mathf.Min(MapTemp[x, y - 1], 1);
                    if (MapTemp[x, y] < 255)
                        MapTemp[x, y] = temp;
                    switch (MapTemp[x, y])
                    {
                        case 1:
                            Room1Amount[zone]++;
                            break;
                        case 2:
                            if (Mathf.Min(MapTemp[x + 1, y], 1) + Mathf.Min(MapTemp[x - 1, y], 1) == 2)
                            {
                                Room2Amount[zone]++;
                            }
                            else if (Mathf.Min(MapTemp[x, y + 1], 1) + Mathf.Min(MapTemp[x, y - 1], 1) == 2)
                            {
                                Room2Amount[zone]++;
                            }
                            else
                            {
                                Room2CAmount[zone]++;
                            }
                            break;
                        case 3:
                            Room3Amount[zone]++;
                            break;
                        case 4:
                            Room4Amount[zone]++;
                            break;
                    }
                }
            }
        }
        for (x = 0; x < MapTemp.GetLength(0); x++)
        {
            for (y = 0; y < MapTemp.GetLength(1); y++)
            {
                //Console.Write(Convert.ToInt32(MapTemp[y, x]));
                GD.Print(MapTemp[y, x]);
            }
            GD.Print("\n");
        }
        // force more room1s (if needed)
        /*for (int i = 0; i < 3; i++)
        {
            temp = -Room1Amount[i] + 5;

            if (temp > 0)
            {
                for (y = (MapSize.y / ZoneAmount) * (2 - i) + 1; y < ((MapSize.y / ZoneAmount) * ((2 - i) + 1.0f)) - 2; y++)
                {
                    for (x = 2; x < MapSize.x - 2; x++)
                    {
                        if (MapTemp[x, y] == 0)
                        {
                            if ((Mathf.Min(MapTemp[x + 1, y], 1) + Mathf.Min(MapTemp[x - 1, y], 1) + Mathf.Min(MapTemp[x, y + 1], 1) + Mathf.Min(MapTemp[x, y - 1], 1)) == 1)
                            {
                                int x2 = 0;
                                int y2 = 0;
                                if (MapTemp[x + 1, y] != 0)
                                {
                                    x2 = x + 1;
                                    y2 = y;
                                }
                                else if (MapTemp[x - 1, y] != 0)
                                {
                                    x2 = x - 1;
                                    y2 = y;
                                }
                                if (MapTemp[x, y + 1] != 0)
                                {
                                    x2 = x;
                                    y2 = y + 1;
                                }
                                else if (MapTemp[x, y - 1] != 0)
                                {
                                    x2 = x;
                                    y2 = y - 1;
                                }

                                bool placed = false;
                                if (MapTemp[x2, y2] > 1 && MapTemp[x2, y2] < 4)
                                {
                                    switch (MapTemp[x2, y2])
                                    {
                                        case 2:
                                            if (Mathf.Min(MapTemp[x2 + 1, y2], 1) + Mathf.Min(MapTemp[x2 - 1, y2], 1) == 2)
                                            {
                                                Room2Amount[i]--;
                                                Room3Amount[i]++;
                                                placed = true;
                                            }
                                            else if (Mathf.Min(MapTemp[x2, y2 + 1], 1) + Mathf.Min(MapTemp[x2, y2 - 1], 1) == 2)
                                            {
                                                Room2Amount[i]--;
                                                Room3Amount[i]++;
                                                placed = true;
                                            }
                                            break;
                                        case 3:
                                            Room3Amount[i]--;
                                            Room4Amount[i]++;
                                            placed = true;
                                            break;
                                    }

                                    if (placed)
                                    {
                                        MapTemp[x2, y2] = MapTemp[x2, y2] + 1;
                                        MapTemp[x, y] = 1;
                                        Room1Amount[i]++;

                                        temp--;
                                    }
                                }
                            }
                        }
                        if (temp == 0)
                            break;
                    }
                    if (temp == 0)
                        break;
                }
            }
        }
        */

        int MaxRooms = 55 * MapSize.X / 20;
        MaxRooms = Mathf.Max(MaxRooms, Room1Amount[0]+1);//+Room1Amount[1]+Room1Amount[2]
        MaxRooms = Mathf.Max(MaxRooms, Room2Amount[0]+1);//+Room2Amount[1]+Room2Amount[2]
        MaxRooms = Mathf.Max(MaxRooms, Room2CAmount[0]+1);//+Room2CAmount[1]+Room2CAmount[2]
        MaxRooms = Mathf.Max(MaxRooms, Room3Amount[0]+1);//+Room3Amount[1]+Room3Amount[2]
        MaxRooms = Mathf.Max(MaxRooms, Room4Amount[0]+1);//+Room4Amount[1]+Room4Amount[2]
        string[,] MapRoom = new string[(int)RoomType.ROOM4 + 1, MaxRooms];

        int min_pos = 1;
        int max_pos = Room1Amount[0]-1;

        //there you need to put special rooms. MapRoom spawns the first room, SetRoom - others (more randomly)

        MapRoom[(int)RoomType.ROOM1, 0] = "LC_room1_archive";

        SetRoom(ref MapRoom, "LC_cont1_079", RoomType.ROOM1, Mathf.FloorToInt(0.3f * Room1Amount[0]), min_pos, max_pos);

        MapRoom[(int)RoomType.ROOM2, 0] = "LC_room2_hall";

        SetRoom(ref MapRoom, "LC_cont2_012", RoomType.ROOM2, Mathf.FloorToInt(0.1f * Room2Amount[0]), min_pos, max_pos);
        SetRoom(ref MapRoom, "LC_cont2_650", RoomType.ROOM2, Mathf.FloorToInt(0.2f * Room2Amount[0]), min_pos, max_pos);
        SetRoom(ref MapRoom, "LC_room2_vent", RoomType.ROOM2, Mathf.FloorToInt(0.4f * Room2Amount[0]), min_pos, max_pos);
        SetRoom(ref MapRoom, "LC_room2_sl", RoomType.ROOM2, Mathf.FloorToInt(0.8f * Room2Amount[0]), min_pos, max_pos);


        await SpawnMap(MapTemp, MapRoom, MaxRooms, rng);
    }

    public async Task SpawnMap(int[,] MapTemp, string[,] MapRoom, int MaxRooms, System.Random rng)
    {
        //OS.DelayMsec(1000);
        int[] MapRoomID = new int[(int)RoomType.ROOM4 + 1];
        string[,] MapName = new string[MapTemp.GetLength(0), MapTemp.GetLength(1)];
        int i = 0;
        for (int y = 1; y < MapTemp.GetLength(1) - 1; y++)
        {
            for (int x = 1; x < MapTemp.GetLength(0) - 1; x++)
            {
                i++;
                if (MapTemp[x, y] > 0)
                {
                    int temp = Mathf.Min(MapTemp[x + 1, y], 1) + Mathf.Min(MapTemp[x - 1, y], 1) + Mathf.Min(MapTemp[x, y + 1], 1) + Mathf.Min(MapTemp[x, y - 1], 1);
                    switch (temp)
                    {
                        case 1:
                        {
                            RoomType type = RoomType.ROOM1;
                            if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                            {
                                if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                {
                                    MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                }
                            }

                            Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                            if (MapTemp[x, y+1] > 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 0f, 0f);
                            }
                            else if (MapTemp[x-1, y] > 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 270f, 0f);
                            }
                            else if (MapTemp[x+1, y] > 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 90f, 0f);
                            }
                            else
                            {
                                r.RotationDegrees = new Vector3(0f, 180f, 0f);
                            }
                            MapRoomID[(int)type]++;
                        }
                        break;
                        case 2:
                        {
                            if (MapTemp[x - 1, y] > 0 && MapTemp[x + 1, y] > 0)
                            {
                                RoomType type = RoomType.ROOM2;
                                if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                                {
                                    if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                    {
                                        MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                    }
                                }
                                Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                                if (rng.Next(1, 3) == 1)
                                {
                                    r.RotationDegrees = new Vector3(0f, 270f, 0f);
                                }
                                else
                                {
                                    r.RotationDegrees = new Vector3(0f, 90f, 0f);
                                }
                                MapRoomID[(int)type]++;
                            }
                            else if (MapTemp[x, y - 1] > 0 && MapTemp[x, y + 1] > 0)
                            {
                                RoomType type = RoomType.ROOM2;
                                if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                                {
                                    if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                    {
                                        MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                    }
                                }
                                Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                                if (rng.Next(1, 3) == 1)
                                {
                                    r.RotationDegrees = new Vector3(0f, 180f, 0f);
                                }
                                else
                                {
                                    r.RotationDegrees = new Vector3(0f, 0f, 0f);
                                }
                                MapRoomID[(int)type]++;
                            }
                            else
                            {
                                RoomType type = RoomType.ROOM2C;
                                if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                                {
                                    if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                    {
                                        MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                    }
                                }
                                Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                                if (MapTemp[x - 1, y] > 0 && MapTemp[x, y + 1] > 0)
                                {
                                    r.RotationDegrees = new Vector3(0f, 270f, 0f);
                                }
                                else if (MapTemp[x + 1, y] > 0 && MapTemp[x, y + 1] > 0)
                                {
                                    r.RotationDegrees = new Vector3(0f, 0f, 0f);
                                }
                                else if (MapTemp[x - 1, y] > 0 && MapTemp[x, y - 1] > 0)
                                {
                                    r.RotationDegrees = new Vector3(0f, 180f, 0f);
                                }
                                else
                                {
                                    r.RotationDegrees = new Vector3(0f, 90f, 0f);
                                }
                                MapRoomID[(int)type]++;
                            }
                        }
                        break;
                        case 3:
                        {
                            RoomType type = RoomType.ROOM3;
                            if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                            {
                                if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                {
                                    MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                }
                            }
                            Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                            if (MapTemp[x, y - 1] <= 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 0f, 0f);
                            }
                            else if (MapTemp[x - 1, y] <= 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 90f, 0f);
                            }
                            else if (MapTemp[x + 1, y] <= 0)
                            {
                                r.RotationDegrees = new Vector3(0f, 270f, 0f);
                            }
                            else
                            {
                                r.RotationDegrees = new Vector3(0f, 180f, 0f);
                            }
                            MapRoomID[(int)type]++;
                        }
                        break;
                        case 4:
                        {
                            RoomType type = RoomType.ROOM4;
                            if (MapRoomID[(int)type] < MaxRooms && string.IsNullOrEmpty(MapName[x, y]))
                            {
                                if (!string.IsNullOrEmpty(MapRoom[(int)type, MapRoomID[(int)type]]))
                                {
                                    MapName[x, y] = MapRoom[(int)type, MapRoomID[(int)type]];
                                }
                            }
                            Node3D r = await CreateRoom(GetZone(y), type, x, 0, y, MapName[x, y], rng);
                            MapRoomID[(int)type]++;
                        }
                        break;
                    }
                }
            }
        }
    }

    public async Task<Node3D> CreateRoom(int zone, RoomType type, int x, int y, int z, string room_name, System.Random rng)
    {
        Room room = new Room(zone);
        Node3D go = (Node3D)room.GetRoomMesh(room_name, type).Instantiate(); //Instantiate(room.gameObject, transform);
        go.Translate(new Vector3(x * 20.48f, y * 20.48f, z * 20.48f));
        AddChild(go);
        return go;
    }

    public bool SetRoom(ref string[,] MapRoom, string room_name, RoomType type, int pos, int min_pos, int max_pos)
    {
        if (max_pos < min_pos)
        {
           GD.PrintErr($"Can't place {room_name}");
            return false;
        }
        
        bool looped = false;
        bool can_place = true;
        while (!string.IsNullOrEmpty(MapRoom[(int)type, pos]))
        {
            pos++;
            if (pos > max_pos)
            {
                if (!looped)
                {
                    pos = min_pos + 1;
                    looped = true;
                }
                else
                {
                    can_place = false;
                    break;
                }
            }
        }
        if (can_place)
        {
            MapRoom[(int)type, pos] = room_name;
            return true;
        }
        else
        {
            GD.PrintErr($"Can't place {room_name}");
            return false;
        }
    }

    public override async void _Ready()
    {
        await CreateMap();
    }

    /*public override void _Process(double delta)
    {
        
    }*/
}