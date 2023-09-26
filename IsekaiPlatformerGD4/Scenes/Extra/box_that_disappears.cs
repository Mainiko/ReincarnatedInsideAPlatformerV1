using Godot;
using System;
using System.Threading.Tasks;

public partial class box_that_disappears : StaticBody2D
{

	// Called when the node enters the scene tree for the first time.
	public  override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public  override void _Process(double delta)
	{
	   
	}

	private async void _on_hitbox_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			await Task.Delay(250);
			this.QueueFree();

		}
	}
}
