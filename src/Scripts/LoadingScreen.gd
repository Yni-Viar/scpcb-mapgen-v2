extends Control


#Written from tutorials ;D
var path: String = "res://Addons/MapGen/Scenes/TestMapGen.tscn"
var progress = []

# Called when the node enters the scene tree for the first time.
#func _ready():
#	pass

func _process(delta):
	var loader = ResourceLoader.load_threaded_request(path)
	
	match ResourceLoader.load_threaded_get_status(path, progress):
#		ResourceLoader.THREAD_LOAD_IN_PROGRESS:
#			pass
		ResourceLoader.THREAD_LOAD_LOADED:
			get_tree().change_scene_to_packed(ResourceLoader.load_threaded_get(path))
		ResourceLoader.THREAD_LOAD_FAILED:
			print('error ocurred loading :(')
		ResourceLoader.THREAD_LOAD_INVALID_RESOURCE:
			print('no resource is loading. error. :(')



# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#
