using Godot;
using System;

public partial class spike_enemy : Node2D
{
	Timer timer = null;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void _on_area_2d_body_entered(object body)
	{
		GD.Print("Body: " + body + "has entered");

	}

	private void _on_area_2d_body_entered(CharacterBody2D body) // Get called when a chacarterBody2D collides with spike
	{
		GD.Print("Body: " + body.Name + "has entered");

		timer.Start();


		//Call function from player
		if(body.Name == "player") 
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");

			//var world = GetTree().GetRoot().GetNode<Node>("parent");
			//world.Call("SpawnPlayer");
			//GD.Print(GetTree().GetRoot().GetNode<Node>("parent").Name);
			//GetNode("../../")


		}





	}

	private void _on_timer_timeout()
	{
		GD.Print("cya im out");
		QueueFree();
	}



}
