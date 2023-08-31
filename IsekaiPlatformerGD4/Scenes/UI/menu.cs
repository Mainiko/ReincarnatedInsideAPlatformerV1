using Godot;
using System;

public partial class menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Levels/Hubs/hub1.tscn");
		GD.Print("Game start");
	}

	public void _on_quit_button_pressed()
	{
		GetTree().Quit();
		GD.Print("Quit game");
	}

}
