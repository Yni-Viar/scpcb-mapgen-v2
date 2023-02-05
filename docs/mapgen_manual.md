# Manual of spawning rooms in the SCP-CB mapgen.
I hope my guide will be easy for you.
## How to add your room?
1. You need to put your room in the folder: 
    1. ROOM1, if your room has only one exit; 
    2. ROOM2 - if the room has two exits; 
    3. ROOM2C, if the room has two exit and it is a corner; 
    4. ROOM3, if the room has three exits. 
    5. ROOM4, if the room has four exits.
2. You need to add to the MapGenerator.cs next lines (works only with unique rooms):
    1. > `MapRoom[(int)RoomType.ROOM<type>, 0] = "first_second_third";`
        - if it is for the first unique room (beginning with line 308).
    2. > `SetRoom(ref MapRoom, "first_second_third", RoomType.ROOM<type>, Mathf.FloorToInt(<float-number-ascending, e.g. 0.1f> * Room<type>Amount[0]), min_pos, max_pos);`
        - if it is for other unique rooms (beginning with line 310).
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
1. `int width = rng.Next(lower, higher);` at line 71
    - this is for not exiting an array.
2. The vector2 parameter in Godot Editor.
- As example.
    - if the map (vector2) is 12 x 12, width is random between 6 and 10,
    - else if the map (vector2) is 18 x 18 (original SCP-CB), width is random between 10 and 16,
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