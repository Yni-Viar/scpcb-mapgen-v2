using Godot;
using System;

public partial class PlayerScript : CharacterBody3D
{
    /* This script was created by dzejpi. License - The Unlicense. 
     * Some parts used from elmarcoh (this script is also public domain).
     */

    Node3D playerHead;
    RayCast3D ray;
    RayCast3D bottomRaycast;
    RayCast3D topRaycast;

    float speed = 4.5f;
    float jump = 4.5f;
    float gravity = 9.8f;

    float groundAcceleration = 8.0f;
    float airAcceleration = 8.0f;
    float acceleration;

    float slidePrevention = 10.0f;
    Vector2 mouseSensivity = new Vector2(0.125f, 2f);

    Vector3 direction = new Vector3();
    Vector3 vel = new Vector3();
    Vector3 movement = new Vector3();
    Vector3 gravityVector = new Vector3();

    bool isOnGround = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        playerHead = GetNode<Node3D>("PlayerHead");
        ray = GetNode<RayCast3D>("PlayerHead/RayCast3D");
        bottomRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck");
        topRaycast = GetNode<RayCast3D>("PlayerFeet/StairCheck2");
        acceleration = groundAcceleration;
        Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            InputEventMouseMotion m = (InputEventMouseMotion) @event;
            this.RotateY(Mathf.DegToRad(-m.Relative.X * mouseSensivity.Y / 10));
            playerHead.RotateX(Mathf.Clamp(-m.Relative.Y * mouseSensivity.X / 10, -90, 90));
        }
        direction = new Vector3();
        direction.Z = -Input.GetActionStrength("move_forward") + Input.GetActionStrength("move_backward");
        direction.X = -Input.GetActionStrength("move_left") + Input.GetActionStrength("move_right");
        direction = direction.Normalized().Rotated(Vector3.Up, Rotation.Y);

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsOnFloor())
        {
            gravityVector = -GetFloorNormal() * slidePrevention;
            acceleration = groundAcceleration;
            isOnGround = true;
        }
        else
        {
            if (isOnGround)
            {
                gravityVector = Vector3.Zero;
                isOnGround = false;
            }
            else
            {
                gravityVector += Vector3.Down * gravity * (float)delta;
			    acceleration = airAcceleration;
            }
        }
        if (Input.IsActionJustPressed("move_jump"))
        {
            isOnGround = false;
            gravityVector = Vector3.Up * jump;
        }
        if (Input.IsActionPressed("move_sprint"))
        {
            vel = vel.Lerp(direction * speed * 2, acceleration * (float)delta);
        }
        else
        {
            vel = vel.Lerp(direction * speed, acceleration * (float)delta);
        }

        if (bottomRaycast.IsColliding() && !topRaycast.IsColliding())
        {
            if (Input.IsActionPressed("move_backward") || Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right"))
            {
                FloorMaxAngle = 0.785398f; //45 degrees
            }
            else
            {
                FloorMaxAngle = 1.308996f; //75 degrees, need to climb stairs
            }
        }

        movement.Z = vel.Z + gravityVector.Z;
        movement.X = vel.X + gravityVector.X;
	    movement.Y = gravityVector.Y;

        Velocity = movement;
        UpDirection = Vector3.Up;
        MoveAndSlide();
    }
}
