using Godot;
using System;
using System.IO;

public partial class Door : Area2D
{
	[Export]private string target_level_pass = "";


	private void _on_body_entered(CharacterBody2D body) // Get called when a chacarterBody2D collides with spike
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			GD.Print(target_level_pass);
			if (target_level_pass == null || target_level_pass == "")
			{
				GD.Print("funkar inte");

			}

			else
			{
				GD.Print("fungarar ish");
				GetTree().ChangeSceneToFile(target_level_pass);

			}
		}
	}

}
	
