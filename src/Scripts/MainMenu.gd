extends Control


#Made by Yni (License - CC-BY-SA)

## Called when the node enters the scene tree for the first time.
#func _ready():
#	pass # Replace with function body.
#
#
## Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass


func _on_play_pressed():
	get_tree().change_scene_to_file("res://Scenes/LoadingScreen.tscn")


func _on_credits_pressed():
	if $CreditPanel.visible == false:
		$CreditPanel.show()
	else:
		$CreditPanel.hide()


func _on_exit_pressed():
	get_tree().quit()

