extends Resource
class_name MapGenRoom

@export var endrooms: Array[PackedScene]
@export var endrooms_single: Array[PackedScene]
@export var hallways: Array[PackedScene]
@export var hallways_single: Array[PackedScene]
@export var corners: Array[PackedScene]
@export var corners_single: Array[PackedScene]
@export var trooms: Array[PackedScene]
@export var trooms_single: Array[PackedScene]
@export var crossrooms: Array[PackedScene]
@export var crossrooms_single: Array[PackedScene]

func _init(p_endrooms: Array[PackedScene] = [], p_hallways: Array[PackedScene] = [], p_corners: Array[PackedScene] = [],
p_trooms: Array[PackedScene] = [], p_crossrooms: Array[PackedScene] = []):
	endrooms = p_endrooms
	hallways = p_hallways
	corners = p_corners
	trooms = p_trooms
	crossrooms = p_crossrooms
