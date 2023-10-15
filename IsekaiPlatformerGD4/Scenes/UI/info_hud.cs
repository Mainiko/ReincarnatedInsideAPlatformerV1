using Godot;
using Microsoft.VisualBasic;
using System;

public partial class info_hud : Control
{

	private Label _labelVelocityX;
	private Label _labelVelocityY;
	private Label _labelHasReachedMaxSpeed;
	private Label _labelIsWallJumping;
	private Label _labelLastJumpDirection;
	private Label _labelInputDirectionX;
	private Label _labelInputDirectionY;
	private Label _labelIsNormalJumping;

	private double timeElapsed = 0.0f;
	private double updateInterval = 1.0f / 15.0f; // 15 updates per second


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_labelVelocityX = GetNode<Label>("LabelVelocityX");
		_labelVelocityY = GetNode<Label>("LabelVelocityY");
		_labelHasReachedMaxSpeed = GetNode<Label>("LabelHasReachedMaxSpeed");
		_labelIsWallJumping = GetNode<Label>("LabelIsWallJumping");
		_labelLastJumpDirection = GetNode<Label>("LabelLastJumpDirection");
		_labelInputDirectionX = GetNode<Label>("LabelInputDirectionX");
		_labelInputDirectionY = GetNode<Label>("LabelInputDirectionY");
		_labelIsNormalJumping = GetNode<Label>("LabelIsNormalJumping");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Node player = GetParent().GetNode("player"); //This works

		player player = GetParent().GetNode<player>("player"); //also works

		bool hasReachedMaxSpeed = player.HasReachedMaxSpeed();
		bool isWallJumping = player.IsWallJumping();
		float lastJumpDirection = player.LastJumpDirection();
		bool isNormalJumping = player.IsNormalJumping();

		Vector2 playerDirection = player.GetDirectionVector();



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
				_labelIsWallJumping.Text = "IsWallJumping: " + isWallJumping;
				_labelLastJumpDirection.Text = "LastJumpDirection: " + lastJumpDirection;

				string playerDirectionX = playerDirection.X.ToString();
				int lengthToExtract = playerDirectionX.StartsWith("-") ? 6 : 5;
				string formatedPlayerDirectionX = playerDirectionX.Substring(0, Math.Min(lengthToExtract, playerDirectionX.Length));

				_labelInputDirectionX.Text = "InputDirection X: " + formatedPlayerDirectionX;

				_labelIsNormalJumping.Text = "IsNormalJumping: " + isNormalJumping;


				// Reset the time elapsed.
				timeElapsed = 0.0f;
			}
			
			
		}
	}
}
