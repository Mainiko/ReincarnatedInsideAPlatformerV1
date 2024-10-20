using Godot;
using IsekaiPlatformerGD4.Scripts;
using System;

public partial class levelcompleted : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var completedLable = GetNode<Label>("LevelCompletedLabel");
		var deathLable = GetNode<Label>("DeathCountLable");
		var timelable = GetNode<Label>("TimeCountLable");

		var currentpath = LevelHandeler.CurrentLevel.Split('/');
		var currentLevel = currentpath[currentpath.Length - 1];
		completedLable.Text = "Level " + currentLevel.Substring(0, currentLevel.Length - 5) + " Completed";

		deathLable.Text = "Well done! " + "You died " + DeathCounter.DeathCount + " times!" ;
		timelable.Text = " And you finshed the level in " + GameTimer.Time;

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

