using Godot;
using Microsoft.VisualBasic;
using System;

public partial class info_hud : Control
{

	private Label _labelVelocityX;
	private Label _labelVelocityY;
	private Label _labelHasReachedMaxSpeed;

	private double timeElapsed = 0.0f;
	private double updateInterval = 1.0f / 15.0f; // 15 updates per second


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_labelVelocityX = GetNode<Label>("LabelVelocityX");
		_labelVelocityY = GetNode<Label>("LabelVelocityY");
		_labelHasReachedMaxSpeed = GetNode<Label>("LabelHasReachedMaxSpeed");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Node player = GetParent().GetNode("player"); //This works

		player player = GetParent().GetNode<player>("player"); //also works

		bool hasReachedMaxSpeed = player.HasReachedMaxSpeed();
		//player player = GetParent().GetNode("player");
		//player player = GetNodeOrNull<player>("/root/player");
		//Player player = GetParent().GetNode<Player>("Player");
		//var hasReachedMaxSpeed = ((CharacterBody2D)player.HasReachedMaxSpeed();
		//var hasReachedMaxSpeed = ((CharacterBody2D)player).HasReachedMaxSpeed();


		if (player != null)
		{
			//GD.Print("player not null");

			timeElapsed += delta;
			if (timeElapsed >= updateInterval)
			{
				// Update your velocity display here.
				Vector2 velocity = ((CharacterBody2D)player).Velocity;
				_labelVelocityX.Text = "Velocity X: " + velocity.X.ToString().Split(',')[0];
				_labelVelocityY.Text = "Velocity Y: " + velocity.Y.ToString().Split(',')[0];
				_labelHasReachedMaxSpeed.Text = "HasReachedMaxSpeed: " + hasReachedMaxSpeed;

				// Reset the time elapsed.
				timeElapsed = 0.0f;
			}
			
			
		}
	}
}
