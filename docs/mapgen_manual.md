# Manual of spawning rooms in the SCP-CB mapgen.
I hope my guide will be easy for you.

## How to add your room?
1. You need to put your room in the folder: 
    1. ROOM1, if your room has only one exit; 
    2. ROOM2 - if the room has two exits; 
    3. ROOM2C, if the room has two exits and it is a corner; 
    4. ROOM3, if the room has three exits. 
    5. ROOM4, if the room has four exits.
2. You need to add to the MapGenerator.cs next lines (works only with unique rooms):
    1. > `MapRoom[(int)RoomType.ROOM<type>, <position>] = "first_second_third";`
        - if it is for the first unique room (beginning with line 308).
    2. > `SetRoom(ref MapRoom, "first_second_third", RoomType.ROOM<type>, Mathf.FloorToInt(<float-number-ascending, e.g. 0.1f> * Room<type>Amount[0]), min_pos, max_pos);`
        - if it is for other unique rooms (beginning with line 310).
    3. > `int min_pos = 1;`
         `int max_pos = Room<type>Amount[0]-1;`
        - for normal spawn of a custom room. Each room type - each max_pos.
3. And the Room/Room.cs
    1. if you want to change a normal room, then you need change:
        1. string in line 27 if it is a ROOM1,
        2. string in line 31 if it is a ROOM2, 
        3. string in line 35 if it is a ROOM2C, 
        4. string in line 39 if it is a ROOM3,
        2. string in line 43 if it is a ROOM4
    2. or if you want to change an unique room:
        - > `case "first_second_third":`  
          > `        return ResourceLoader.Load<PackedScene>("res://Addons/MapGen/Resources/ROOM<type>/name_of_your_room).tscn");`

## How to change the map size?
You need to change two variables:
- > `int width = rng.Next(lower, higher);` at line 71
    - this is for not exiting an array.
- The vector2 parameter in Godot Editor.
- As example:
    - if the map (vector2) is 12 x 12, width is between 6 and 10,
    - else if the map (vector2) is 18 x 18 (original SCP-CB), width is random 10 and 16,

## How to change the zone size?
You need to change some variables...
- > `int ZoneAmount = 1`
    - this value means a quantity of zones. In the test, there is only one zone, but you can change this value.
- > `MaxRooms = Mathf.Max(MaxRooms, Room<type>Amount[0] + Room<type>Amount[n]+1)`
    - these values needed for properly zone spawning.
    - How much are zones, defined in ZoneAmount, so how much will be quantity of elements of the array.
    - In the code only one zone is being used, but there are two more zones commented. Why - because our array consist of only one item...
- If you want to add special rooms in a second zone, you need to move min_pos and max_pos, mentioned earlier to the next zone:
    - > `min_pos = Room<type>Amount[0];`
      > `max_pos = Room<type>Amount[0]+Room<type>Amount[n]-1;`

## How to name your room, if I want to use it for an SCP game?
`first_second_third`
1. first is a facility zone, where:
    1. LC is Light Containment Zone,
    2. HC is Heavy Containment Zone,
    3. RZ is Research Zone,
    4. EZ is Entrance Zone
    5. and others you can imagine :)
2. second is a room type.
    1. room< angle > is a regular room.
    2. cont< angle > is an SCP Containment Chamber
3. third (optional), is the room name