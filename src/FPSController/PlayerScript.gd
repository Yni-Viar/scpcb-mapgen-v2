extends CharacterBody3D
## Created by dzejpi. License - The Unlicense. 
## Some parts used from elmarcoh (this script is also public domain).
## Ported from C# in 04.2024
class_name PlayerScript


@onready var player_head = $PlayerHead
@onready var ray = $PlayerHead/PlayerRecoil/RayCast3D
@onready var watch_ray = $PlayerHead/PlayerRecoil/VisionRadius
@onready var walk_sounds = $WalkSounds
@onready var interact_sound = $InteractSound

## Player class manager properties
@export var health: float = 100
@export var current_health: float = 100
@export var speed: float = 4.5
@export var jump: float = 4.5
@export var sprint_enabled: bool = false
@export var move_sounds_enabled: bool = false
@export var footstep_sounds: Array[String]
@export var sprint_sounds: Array[String]
@export var custom_camera: bool = false
@export var can_move: bool = true
var gravity: float = 9.8

var acceleration: float = 8
var rng: RandomNumberGenerator = RandomNumberGenerator.new()
var mouse_sensitivity = 0.05

var direction = Vector3()
var vel = Vector3()
var movement = Vector3()
var gravity_vector = Vector3()

var is_sprinting: bool = false
var is_walking: bool = false

func _ready():
	$PlayerHead/PlayerRecoil/PlayerCamera.current = true
	floor_max_angle = 1.308996
	ray.add_exception(self)
	watch_ray.add_exception(self)
	if !OS.is_debug_build():
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	on_start()

func on_start():
	pass

func _input(event):
	if event is InputEventMouseMotion:
		rotate_y(-event.relative.x * mouse_sensitivity * 0.05)
		player_head.rotate_x(clamp(-event.relative.y * mouse_sensitivity * 0.05, -90, 90))
		
		var camera_rot = player_head.rotation_degrees
		camera_rot.x = clamp(player_head.rotation_degrees.x, -85, 85)
		player_head.rotation_degrees = camera_rot

	direction = Vector3()
	direction.z = -Input.get_action_strength("move_forward") + Input.get_action_strength("move_backward")
	direction.x = -Input.get_action_strength("move_left") + Input.get_action_strength("move_right")
	direction = direction.normalized().rotated(Vector3.UP, rotation.y)

func _physics_process(delta):
	if is_on_floor():
		gravity_vector = Vector3.ZERO
	else:
		gravity_vector += Vector3.DOWN * gravity * delta

	if Input.is_action_just_pressed("move_jump") and is_on_floor():
		gravity_vector = Vector3.UP * jump
	if can_move:
		if Input.is_action_pressed("move_sprint"):
			vel = vel.lerp(direction * speed * 2, acceleration * delta)
		else:
			vel = vel.lerp(direction * speed, acceleration * delta)
		movement.z = vel.z + gravity_vector.z
		movement.x = vel.x + gravity_vector.x
		movement.y = gravity_vector.y
		set_velocity(movement)
	else:
		set_velocity(Vector3.ZERO)
	set_up_direction(Vector3.UP)
	move_and_slide()

## Animation-based footstep system.
func footstep_animate():
	if move_sounds_enabled:
		if is_walking:
			call("play_footstep_sound", false)
		if is_sprinting:
			call("play_footstep_sound", true)
## Make footstep sounds audible to all.
func play_footstep_sound(sprinting: bool):
	if sprinting:
		walk_sounds.stream = load(sprint_sounds[rng.randi_range(0, sprint_sounds.size() - 1)])
		walk_sounds.play()
	else:
		walk_sounds.stream = load(footstep_sounds[rng.randi_range(0, footstep_sounds.size() - 1)])
		walk_sounds.play()
## Health manager.
func health_manage(amount: float):
	if current_health + amount <= health:
		current_health += amount
	else:
		current_health = health
	if current_health <= 0:
		get_tree().root.get_node("Game/GameOverLabel").show()
		set_physics_process(false)
## Applies shader to the player
func apply_shader(res: String):
	for node in get_node("PlayerHead/PlayerRecoil/PlayerCamera").get_children():
		if node is MeshInstance3D:
			node.visible = false
	if get_node_or_null("PlayerHead/PlayerRecoil/PlayerCamera" + res) != null && !res.is_empty():
		get_node("PlayerHead/PlayerRecoil/PlayerCamera" + res).visible = true
## Sets players view
func camera_manager(default_camera: bool):
	if default_camera:
		get_node("PlayerHead/PlayerRecoil/PlayerCamera").current = true
		if !OS.is_debug_build():
			Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
		custom_camera = false
	else:
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		custom_camera = true

