using Godot;
using IsekaiPlatformerGD4.Scripts;
using System;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;

public partial class player : CharacterBody2D
{

	[Signal] public delegate void OnPlayerDiedEventHandler();


	

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	AnimatedSprite2D animatedSprite2D = null;
	Timer coyoteTimer = null;


	private enum State { NORMAL, DASHING, INPUT_DISABLED }

	[Export]private float GroundSpeed = 125.0f;
	[Export]private int airTurnSpeed = 30;
	[Export] private int airTurnFinaleSpeed = 85;
	[Export]private int friction = 23;
	[Export]private int maxHorizontalSpeed = 200;
//	[Export]private int horizontalAcceleration = 2000;
	[Export]private float JumpVelocity = -250.0f;
	[Export]private int JUMP_RELESE_FORCE = -100;
//	[Export]private int jumpTerminationMultiplier = 4;
	[Export]private int addiditionalFallGravity = 1500;
	[Export]private float acceleration = 0.25f;
	[Export]private int wallGravity = 900;
	[Export]private bool hasDoubleJump = false;
	[Export]private bool CanGaineDoubleJump = false;
	[Export]private bool hasDash = false;
	[Export]private bool isDashing = false;
	[Export] private bool wallClimbJump = false;
	private State currentState = State.NORMAL;
	private bool isStateNew = true;
	private bool isDying = false;
	private bool isJumpingOnEnemy = false;
	private bool isInAirAfterJumpingOnEnemy = false;
	private bool isJumpingOnJumpPlatform = false;
	private bool hasJumped = false;
	private bool wasLastJumpWallJump = false;
	private bool wasLastJumpWallRight = false;
	private bool wasLastJumpWallLeft = false;

	private bool isInsideJumpBall = false;

	private float lastJumpDirection = 0;

	private float myPosition = 0;
	private float myLastPosition = 0;
	private bool isSprinting = false;
	[Export]private float walkSpeed = 125.0f;
	[Export]private float sprintSpeed = 200.0f;
	[Export]private int jumpSpeed = 300;
	[Export]private int dashSpeed = 1500;

	[Export] private int defaultHazardMask = 0;


	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		coyoteTimer = GetNode<Timer>("CoyoteTimer");

	

	}

	public override  void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;
		Vector2 direction = GetDirectionVector();

		// Handle the animation.
		if (!IsOnFloor())
		{
			//GD.Print("in air: " + isInAirAfterJumpingOnEnemy);
			if(velocity.Y < 0)
			{
				hasJumped = true;
				animatedSprite2D.Play("JumpUp");

			}
			else if(velocity.Y > 0)
			{
				animatedSprite2D.Play("JumpDown");

			}
		}
		else if (animatedSprite2D.Animation == "JumpDown" && IsOnFloor())
		{
			animatedSprite2D.Play("JumpImpact");
		}
		else if (direction == Vector2.Zero && (animatedSprite2D.IsPlaying() && animatedSprite2D.Animation == "JumpImpact" ) == false)
		{
			animatedSprite2D.Play("Idle");
		}
		
		else if (IsOnFloor() && direction != Vector2.Zero)
		{
			animatedSprite2D.Play("Run");
		}
		

		if (IsOnFloor())
		{
			walkSpeed = GroundSpeed;
			lastJumpDirection = 0;

		}
		// Add the gravity.
		if (!IsOnFloor())
		{


			if ((GetNode<RayCast2D>("RayCastLeft").IsColliding() || GetNode<RayCast2D>("RayCastLeft2").IsColliding()) && Input.IsActionPressed("move_left") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
				animatedSprite2D.Play("WallHug");
			}
			else if ((GetNode<RayCast2D>("RayCastRight").IsColliding() || GetNode<RayCast2D>("RayCastLeft2").IsColliding()) &&  Input.IsActionPressed("move_right") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
				animatedSprite2D.Play("WallHug");
			}
			else if (Velocity.Y > 0) //If player is falling 
			{
				velocity.Y += addiditionalFallGravity * (float)delta; //Apply fast fall
			}
			else
			{
				velocity.Y += gravity * (float)delta;
			}

		}

		if (IsOnFloor())
		{
			hasDoubleJump = CanGaineDoubleJump;
			hasDash = true;

		}

		if (isJumpingOnEnemy)
		{
			velocity.Y = (float)(JumpVelocity * 1.035);
			GD.Print("Player Jumped on enemey two");
			GD.Print(velocity.Y);
			isJumpingOnEnemy = false;
		}
		// Handle Jump.

		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || !coyoteTimer.IsStopped() || hasDoubleJump) || isInsideJumpBall)
		{
			velocity.Y = JumpVelocity;

			if (!IsOnFloor() && coyoteTimer.IsStopped())
			{
				hasDoubleJump=false;
			}
		}





		if (isJumpingOnJumpPlatform)
		{
			velocity.Y = (float)(JumpVelocity * 1.5);
			isJumpingOnJumpPlatform = false;
		}


		if (Input.IsActionJustReleased("jump") && (velocity.Y < JUMP_RELESE_FORCE) && !isInAirAfterJumpingOnEnemy)
		{
			velocity.Y = JUMP_RELESE_FORCE;
		}


		// Handle movement direction and speed
		if (direction != Vector2.Zero)
		{
			isSprinting = Input.IsActionPressed("sprint");
			float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

			velocity.X = Mathf.Lerp(velocity.X, direction.X * currentSpeed, acceleration);
			FlipSprite(direction);
		}
		else
		{
			velocity.X = Mathf.Lerp(0, velocity.X, Mathf.Pow(friction, -10 * (float)delta)); // This is friction
		}

		velocity.X = Mathf.Clamp(velocity.X, -maxHorizontalSpeed, maxHorizontalSpeed);


		bool wasOnFloor = IsOnFloor();


		//Handles walljump
		if (Input.IsActionJustPressed("jump") && !IsOnFloor())
		{
			GD.Print(GetNode<RayCast2D>("RayCastLeft2").IsColliding());
			if (Input.IsActionJustPressed("jump") && !IsOnFloor() && (GetNode<RayCast2D>("RayCastLeft").IsColliding() || GetNode<RayCast2D>("RayCastLeft2").IsColliding()))
			{
				if (!wasLastJumpWallRight)
				{
					velocity.Y = JumpVelocity;
					velocity.X = jumpSpeed;
					wasLastJumpWallRight = true;
					wasLastJumpWallLeft = false;
					isInAirAfterJumpingOnEnemy = false;
				}

			}
			else if (Input.IsActionJustPressed("jump") && !IsOnFloor() && (GetNode<RayCast2D>("RayCastRight").IsColliding() || GetNode<RayCast2D>("RayCastRight2").IsColliding()))
			{
				if (!wasLastJumpWallLeft)
				{
					velocity.Y = JumpVelocity;
					velocity.X = -jumpSpeed;
					wasLastJumpWallLeft = true;
					wasLastJumpWallRight = false;
					isInAirAfterJumpingOnEnemy = false;

				}

			}

		}

		if (wasOnFloor)
		{
			wasLastJumpWallRight = false;
			wasLastJumpWallLeft = false;
		}

		//Handle Dashing
		if (Input.IsActionJustPressed("dash") && hasDash && !IsOnFloor())
		{

			//Create timer that will signal when the dash is done?
			//So we can start applaying gravity again 
			isDashing = true;
			velocity.Y = -100;


			//Makes player dash at the facing direction 
			bool currentFacingDirection = animatedSprite2D.FlipH;
			int facingDirValue = 0;
			if (currentFacingDirection)
			{
				facingDirValue = -1;
			}
			else
			{
				facingDirValue = 1;
			}

			direction.X = facingDirValue;

			//velocity.x = Mathf.Lerp(velocity.x, direction.x * dashSpeed, acceleration);
			velocity.X = Mathf.Lerp(velocity.X, direction.X * dashSpeed, 0.15f);
			hasDash = false;
			isDashing = false;

		}

		//velocity.y = FastFall(delta);


		Velocity = velocity;
		MoveAndSlide();

		if (wasOnFloor && !IsOnFloor())
		{
			coyoteTimer.Start();
		}

		if (IsOnFloor())
		{
			hasDoubleJump = true;
			isInAirAfterJumpingOnEnemy = false;
			wasLastJumpWallJump = false;
		}

	  
	}


	public Vector2 GetDirectionVector() //Get player input as vector
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
		return direction;
	}

	private void FlipSprite(Vector2 direction) //Flip sprite so it faces input direction
	{
		if (direction.X < 0)
		{
			animatedSprite2D.FlipH = true;
		}
		else
		{
			animatedSprite2D.FlipH = false;
		}
	}

	private async void PlayerDie()
	{

		DeathCounter.IncrementDeathCount();
		GD.Print("DeathCount" + DeathCounter.DeathCount);

		//var blackScreen = GetNode<ColorRect>("/root/Level1/CanvasLayer/ColorRect");
		//blackScreen.Visible = true;
	   await ToSignal(GetTree().CreateTimer(0.1), "timeout");
		//blackScreen.Visible = false;

		foreach (var child in GetTree().CurrentScene.GetChildren())
		{
			GetTree().QueueDelete(child);
		}
		GetTree().ReloadCurrentScene();
		GD.Print("Player dead");
	}

	private void PlayerJumpOnEnemy()
	{
		GD.Print("Player Jumped on enemey");
		isJumpingOnEnemy = true;
		wasLastJumpWallRight = false;
		wasLastJumpWallLeft = false;
		isInAirAfterJumpingOnEnemy = true;
	}

	private void PlayerJumpOnJumpPlatform()
	{
		GD.Print("Player Jumped on platform");
		isJumpingOnJumpPlatform = true;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().ChangeSceneToFile("res://Scenes/UI/menu.tscn");
			}
			   
	}

}
