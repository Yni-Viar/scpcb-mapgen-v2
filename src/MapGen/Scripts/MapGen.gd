extends Node
class_name MapGeneration

var rng: RandomNumberGenerator = RandomNumberGenerator.new()

enum RoomTypes {ROOM1, ROOM2, ROOM2C, ROOM3, ROOM4, EMPTY}

## Rooms that will be used
@export var rooms: Array[MapGenRoom]
#currentluy not ported to gdscript
#@export var generate_more_endrooms: bool = false
#@export var generate_more_crossrooms: bool = false
## Map size
@export var size: int = 12
## Room in grid size
@export var grid_size: float = 20.48
## Divide the map on zones
@export var enable_zones: bool = false
## Zone transitions (first transition MUST be 0, last - more then size value, and the rest divide the zones)
@export var zone_transitions: Array[int] = []
# currently buggy
# @export var checkpoints: Array[PackedScene] = []
var endroom_amount: int
var hallway_amount: int
var corner_amount: int
var troom_amount: int
var crossroom_amount: int

class TempRoom:
	var type: RoomTypes
	var angle: float

func get_zone(y)->int:
	var zone = 0
	if enable_zones:
		for i in range(zone_transitions.size() - 1):
			if y >= zone_transitions[i] && y < zone_transitions[i + 1]:
				zone = i
				break
	return zone

func create_map():
	var x: int
	var y: int
	var x2: int
	var width: int
	var height: int
	var temp: int
	
	var temp_room: Array[Array]
	for i in range(size):
		temp_room.append([])
		for j in range(size):
			temp_room[i].append(TempRoom.new())
	
	for i in range(size):
		for j in range(size):
			temp_room[i][j].type = RoomTypes.ROOM1
			temp_room[i][j].angle = -1
	
	x = size / 2
	y = size - 1
	
	while y >= 2:
		width = rng.randi_range(0, int(size / 4)) + int(size / 2)
		if x > int(size * 0.6):
			width = -width
		elif x > int(size / 2):
			x = x - int(size / 2)
		
		## make sure the hallway doesn't go outside the array
		if (x + width > int(size / (size / 6 * 5))):
			width = (size / 6 * 5) - x
		elif (x + width < int(size / 6)):
			width = -x + int(size / 6)
		
		x = min(x, x + width)
		width = abs(width)
		
		for k in range(x, x + width + 1):
			temp_room[min(k, size - 1)][y].angle = 0
		# height is random
		height = rng.randi_range(0, 1) + int(size / 6)
		if y - height < 1:
			height = y
		# height for each zone
		var yhallways: int
		if !enable_zones:
			yhallways == height
		else:
			yhallways = rng.randi_range(0, 1) + int(size / 5)
			if get_zone(y - height) != get_zone(y - height - 1):
				height -= 1
		
		# this FOR loop cleans the mapgen from loops.
		for j in range(1, yhallways):
			x2 = max(min(rng.randi_range(0, width - 2) + x, int(size * 0.8)), 2)
			while temp_room[x2][y - 1].angle >= 0 || temp_room[x2 - 1][y - 1].angle >= 0 || temp_room[x2 + 1][y - 1].angle >= 0:
				x2 += 1
			
			if x2 < x + width:
				var tmpheight: int
				if j == 1:
					tmpheight = height;
					if (rng.randi_range(0, 1) == 0):
						x2 = x
					else:
						x2 = x + width;
				else:
					tmpheight = (rng.randi_range(0, height - 1)) + 1;
				
				for y2 in range(y - tmpheight, y + 1):
					#if enable_zones && get_zone(y2) != get_zone(y2 + 1):
						#temp_room[x2][y2].angle = 180
					#else:
					temp_room[x2][y2].angle = 0;

				if (tmpheight == height):
					temp = x2;
		y -= height
	
	for q in range(size):
		for r in range(size):
			print(temp_room[q][r].angle)
		print()
	
	var room1_amount: int = 0
	var room2_amount: int = 0
	var room2c_amount: int = 0
	var room3_amount: int = 0
	var room4_amount: int = 0
	
	for l in range(size):
		for m in range(size):
			var north: bool
			var east: bool
			var south: bool
			var west: bool
			#if temp_room[l][m].angle == 180:
				#continue
			if temp_room[l][m].angle == 0:
				if l > 0:
					west = (temp_room[l - 1][m].angle > -1)
				if l < size - 1:
					east = (temp_room[l + 1][m].angle > -1)
				if m > 0:
					north = (temp_room[l][m - 1].angle > -1)
				if m < 11:
					south = (temp_room[l][m + 1].angle > -1)
				if north && south:
					if east && west:
						#room4
						var room_angle: Array[float] = [0, 90, 180, 270]
						temp_room[l][m].type = RoomTypes.ROOM4
						temp_room[l][m].angle = room_angle[rng.randi_range(0, 3)]
						room4_amount += 1
					elif east && !west:
						#room3, pointing east
						temp_room[l][m].type = RoomTypes.ROOM3
						temp_room[l][m].angle = 90
						room3_amount += 1
					elif !east && west:
						#room3, pointing west
						temp_room[l][m].type = RoomTypes.ROOM3
						temp_room[l][m].angle = 270
						room3_amount += 1
					else:
						#vertical room2
						var room_angle: Array[float] = [0, 180]
						temp_room[l][m].type = RoomTypes.ROOM2
						temp_room[l][m].angle = room_angle[rng.randi_range(0, 1)]
						room2_amount += 1
				elif east && west:
					if north && !south:
						#room3, pointing north
						temp_room[l][m].type = RoomTypes.ROOM3
						temp_room[l][m].angle = 180
						room3_amount += 1
					elif !north && south:
					#room3, pointing south
						temp_room[l][m].type = RoomTypes.ROOM3
						temp_room[l][m].angle = 0
						room3_amount += 1
					else:
					#horizontal room2
						var room_angle: Array[float] = [90, 270]
						temp_room[l][m].type = RoomTypes.ROOM2;
						temp_room[l][m].angle = room_angle[rng.randi_range(0, 1)]
						room2_amount += 1
				elif north:
					if east:
					#room2c, north-east
						temp_room[l][m].type = RoomTypes.ROOM2C;
						temp_room[l][m].angle = 90;
						room2c_amount += 1
					elif west:
					#room2c, north-west
						temp_room[l][m].type = RoomTypes.ROOM2C;
						temp_room[l][m].angle = 180;
						room2c_amount += 1
					else:
					#room1, north
						temp_room[l][m].type = RoomTypes.ROOM1;
						temp_room[l][m].angle = 180;
						room1_amount += 1
				elif south:
					if east:
					#room2c, south-east
						temp_room[l][m].type = RoomTypes.ROOM2C;
						temp_room[l][m].angle = 0;
						room2c_amount += 1
					elif west:
					#room2c, south-west
						temp_room[l][m].type = RoomTypes.ROOM2C;
						temp_room[l][m].angle = 270;
						room2c_amount += 1
					else:
					#room1, south
						temp_room[l][m].type = RoomTypes.ROOM1;
						temp_room[l][m].angle = 0;
						room1_amount += 1
				elif east:
					#room1, east
					temp_room[l][m].type = RoomTypes.ROOM1;
					temp_room[l][m].angle = 90;
					room1_amount += 1
				else:
					#room1, west
					temp_room[l][m].type = RoomTypes.ROOM1
					temp_room[l][m].angle = 270
					room1_amount += 1
			else:
				temp_room[l][m].type = RoomTypes.EMPTY
	
	var selected_room: PackedScene
	var room1_count: Array[int] = [0]
	var room2_count: Array[int] = [0]
	var room2c_count: Array[int] = [0]
	var room3_count: Array[int] = [0]
	var room4_count: Array[int] = [0]
	
	if enable_zones:
		for i in range(zone_transitions.size()):
			room1_count.append(0)
			room2_count.append(0)
			room2c_count.append(0)
			room3_count.append(0)
			room4_count.append(0)
	#spawn a room
	for n in range(size):
		for o in range(size):
			var room: StaticBody3D
			match temp_room[n][o].type:
				RoomTypes.ROOM1:
					if (room1_count[get_zone(o)] >= rooms[get_zone(o)].endrooms_single.size()):
						selected_room = rooms[get_zone(o)].endrooms[rng.randi_range(0, rooms[get_zone(o)].endrooms.size() - 1)]
					else:
						selected_room = rooms[get_zone(o)].endrooms_single[room1_count[get_zone(o)]]
					room1_count[get_zone(o)] += 1
					room = selected_room.instantiate()
					room.position = Vector3(n * grid_size, 0, o * grid_size)
					room.rotation_degrees = Vector3(0, temp_room[n][o].angle, 0)
					add_child(room, true)
				RoomTypes.ROOM2:
					#if enable_zones && get_zone(o) != get_zone(o + 1):
						#selected_room = checkpoints[get_zone(o) - 1]
					#else:
					if (room2_count[get_zone(o)] >= rooms[get_zone(o)].hallways_single.size()):
						selected_room = rooms[get_zone(o)].hallways[rng.randi_range(0, rooms[get_zone(o)].hallways.size() - 1)]
					else:
						selected_room = rooms[get_zone(o)].hallways_single[room2_count[get_zone(o)]]
					room2_count[get_zone(o)] += 1
					room = selected_room.instantiate()
					room.position = Vector3(n * grid_size, 0, o * grid_size)
					room.rotation_degrees = Vector3(0, temp_room[n][o].angle, 0)
					add_child(room, true)
				RoomTypes.ROOM2C:
					if (room2c_count[get_zone(o)] >= rooms[get_zone(o)].corners_single.size()):
						selected_room = rooms[get_zone(o)].corners[rng.randi_range(0, rooms[get_zone(o)].corners.size() - 1)]
					else:
						selected_room = rooms[get_zone(o)].corners_single[room2c_count[get_zone(o)]]
					room2c_count[get_zone(o)] += 1
					room = selected_room.instantiate()
					room.position = Vector3(n * grid_size, 0, o * grid_size)
					room.rotation_degrees = Vector3(0, temp_room[n][o].angle, 0)
					add_child(room, true)
				RoomTypes.ROOM3:
					if (room3_count[get_zone(o)] >= rooms[get_zone(o)].trooms_single.size()):
						selected_room = rooms[get_zone(o)].trooms[rng.randi_range(0, rooms[get_zone(o)].trooms.size() - 1)]
					else:
						selected_room = rooms[get_zone(o)].trooms_single[room3_count[get_zone(o)]]
					room3_count[get_zone(o)] += 1
					room = selected_room.instantiate()
					room.position = Vector3(n * grid_size, 0, o * grid_size)
					room.rotation_degrees = Vector3(0, temp_room[n][o].angle, 0)
					add_child(room, true)
				RoomTypes.ROOM4:
					if (room4_count[get_zone(o)] >= rooms[get_zone(o)].crossrooms_single.size()):
						selected_room = rooms[get_zone(o)].crossrooms[rng.randi_range(0, rooms[get_zone(o)].crossrooms.size() - 1)]
					else:
						selected_room = rooms[get_zone(o)].crossrooms_single[room4_count[get_zone(o)]]
					room4_count[get_zone(o)] += 1
					room = selected_room.instantiate()
					room.position = Vector3(n * grid_size, 0, o * grid_size)
					room.rotation_degrees = Vector3(0, temp_room[n][o].angle, 0)
					add_child(room, true)
	#for q in range(size):
		#for r in range(size):
			#print(int(temp_room[q][r].type))
		#print()

# Called when the node enters the scene tree for the first time.
func _ready():
	create_map()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
