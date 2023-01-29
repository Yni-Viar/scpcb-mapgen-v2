extends CharacterBody3D

#Created by dzejpi. License - The Unlicense. 
#Some parts used from elmarcoh (this script is also public domain).
@onready var player_head = $PlayerHead
@onready var ray = $PlayerHead/RayCast3D
@onready var step_checker = $PlayerHead/StepChecker

var speed = 4.5
var jump = 4.5
var gravity = 16

var ground_acceleration = 8
var air_acceleration = 8
var acceleration = ground_acceleration

var slide_prevention = 10
var mouse_sensitivity = 0.75

var direction = Vector3()
var vel = Vector3()
var movement = Vector3()
var gravity_vector = Vector3()

var is_on_ground = true
#var is_options_menu_displayed = false

# Name of the observed object for debugging purposes
var observed_object = "" 


func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)


func _input(event):
	if event is InputEventMouseMotion:
		rotation_degrees.y -= event.relative.x * mouse_sensitivity / 10
		player_head.rotation_degrees.x = clamp(player_head.rotation_degrees.x - event.relative.y * mouse_sensitivity / 10, -90, 90)

	direction = Vector3()
	direction.z = -Input.get_action_strength("move_up") + Input.get_action_strength("move_down")
	direction.x = -Input.get_action_strength("move_left") + Input.get_action_strength("move_right")
	direction = direction.normalized().rotated(Vector3.UP, rotation.y)
	
	# Handling the options menu
	if Input.is_action_just_pressed("ui_cancel"):
		if Input.MOUSE_MODE_CAPTURED:
			Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
#			is_options_menu_displayed = true
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
#			is_options_menu_displayed = false


#func _process(delta):
#
#	# If player is looking at something
#	if ray.is_colliding():
#		var collision_object = ray.get_collider().name
#
#		if collision_object != observed_object:
#			observed_object = collision_object
#			print("Player is looking at: " + observed_object + ".")
#	else:
#		var collision_object = "nothing"
#		if collision_object != observed_object:
#			observed_object = collision_object
#			print("Player is looking at: nothing.")
#

func _physics_process(delta):
	if is_on_floor():
		gravity_vector = -get_floor_normal() * slide_prevention
		acceleration = ground_acceleration
		is_on_ground = true
	else:
		if is_on_ground:
			gravity_vector = Vector3.ZERO
			is_on_ground = false
		else:
			gravity_vector += Vector3.DOWN * gravity * delta
			acceleration = air_acceleration

	if Input.is_action_just_pressed("move_jump") and is_on_floor():
		is_on_ground = false
		gravity_vector = Vector3.UP * jump

	if Input.is_action_pressed("move_sprint"):
		vel = vel.lerp(direction * speed * 2, acceleration * delta)
	else:
		vel = vel.lerp(direction * speed, acceleration * delta)
	
	movement.z = vel.z + gravity_vector.z
	movement.x = vel.x + gravity_vector.x
	movement.y = gravity_vector.y
	
	set_velocity(movement)
	set_up_direction(Vector3.UP)
	move_and_slide()
	
