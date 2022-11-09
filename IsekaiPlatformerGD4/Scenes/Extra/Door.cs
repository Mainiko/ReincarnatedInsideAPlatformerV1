using Godot;
using System;

public partial class Door : Area2D
{
	[Export]private string target_level_pass = "";


	private void _on_area_2d_body_entered(CharacterBody2D body) // Get called when a chacarterBody2D collides with spike
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			if (target_level_pass == null || target_level_pass == "")
				return;
			else
				GetTree().ChangeSceneToFile(target_level_pass);
		}
	}

}
	
