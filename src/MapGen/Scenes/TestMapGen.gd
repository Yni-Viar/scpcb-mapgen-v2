extends Node3D


# Called when the node enters the scene tree for the first time.
func _ready():
	var player: PlayerScript = load("res://FPSController/PlayerScene.tscn").instantiate()
	add_child(player)
	player.global_position = get_node("MapGen/LC_cont2_650/entityspawn").global_position


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
