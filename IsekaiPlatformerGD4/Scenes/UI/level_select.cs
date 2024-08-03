using Godot;
using System;

public partial class level_select : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_item_list_item_clicked(long index, Vector2 at_position, long mouse_button_index)
	{

		GetTree().ChangeSceneToFile("res://Scenes/Levels/StartWorldtest.tscn");

		// Replace with function body.
		GD.Print("index: " +  index);
		GD.Print("at_position: " + at_position);
		GD.Print("mouse_button_index" +  mouse_button_index);

		switch (index)
		{
			case 0:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest7.tscn");
				break;
			case 1:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest8.tscn");
				break;
			case 2:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest9.tscn");
				break;
			case 3:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest10.tscn");
				break;
			case 4:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest11.tscn");
				break;
			case 5:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest12.tscn");
				break;
			case 6:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest13.tscn");
				break;
			case 7:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest14.tscn");
				break;
			case 8:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/levelTest15.tscn");
				break;
			case 9:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/bossStageTest1.tscn");
				break;
			case 10:
				GetTree().ChangeSceneToFile("res://Scenes/Levels/bossStageTest2.tscn");
				break;

			default:
				break;
		}

	}
}



