using Godot;
using IsekaiPlatformerGD4.Scripts;
using System;

public partial class levelcompleted : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var deathLable = GetNode<Label>("DeathCountLable");
		deathLable.Text = "Well done!You died " + DeathCounter.DeathCount + " times";
		DeathCounter.ResetDeathCount();

		Button playAgain = GetNode<Button>("PlayAgainButton");
		playAgain.GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_play_again_button_pressed()
	{
		GetTree().ChangeSceneToFile(LevelHandeler.CurrentLevel);
	}

	public void _on_next_level_button_pressed()
	{
		GetTree().ChangeSceneToFile(LevelHandeler.NextLevel);
	}


}

