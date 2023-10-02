using Godot;
using System;

public partial class Facility : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        CharacterBody3D player = GD.Load<PackedScene>("res://FPSController/PlayerScene.tscn").Instantiate<CharacterBody3D>();
        player.Position = GetNode<Marker3D>("MapGen/LC_room1_archive/entityspawn").GlobalPosition;
        AddChild(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
