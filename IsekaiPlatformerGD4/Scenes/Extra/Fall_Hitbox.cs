using Godot;
using System;

public partial class Fall_Hitbox : Node2D
{
	private void _on_area_2d_body_entered(CharacterBody2D body) // Get called when a chacarterBody2D collides with spike
	{
		GD.Print("Body: " + body.Name + "has entered");


		//Call function from player
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");

			//var world = GetTree().GetRoot().GetNode<Node>("parent");
			//world.Call("SpawnPlayer");
			//GD.Print(GetTree().GetRoot().GetNode<Node>("parent").Name);
			//GetNode("../../")


		}

	}
}
