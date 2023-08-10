using Godot;
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

	[Export]private float Speed = 125.0f;
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
	private bool isJumpingOnJumpPlatform = false;

	//Celeste tutorial
	[Export]private int jumpHeight = 300;
	[Export]private int dashSpeed = 1500;

	[Export] private int defaultHazardMask = 0;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		coyoteTimer = GetNode<Timer>("CoyoteTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
		{   
			if (GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_left") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
			}
			else if (GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_right") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
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
	 

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || !coyoteTimer.IsStopped() || hasDoubleJump))
		{
			velocity.Y = JumpVelocity;
			if(!IsOnFloor() && coyoteTimer.IsStopped())
			{
				hasDoubleJump=false;
			}
		}

		if (isJumpingOnEnemy)
		{
			velocity.Y = JumpVelocity;
			isJumpingOnEnemy = false;
		}


		if (isJumpingOnJumpPlatform)
		{
			velocity.Y = (float)(JumpVelocity * 1.5);
			isJumpingOnJumpPlatform = false;
		}


		if (Input.IsActionJustReleased("jump") && (velocity.Y < JUMP_RELESE_FORCE))
		{
			
			GD.Print("jump release force");
			GD.Print(velocity.Y);
			velocity.Y = JUMP_RELESE_FORCE;
			GD.Print(velocity.Y);
			
		}


		Vector2 direction = GetDirectionVector();

		if (direction != Vector2.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, acceleration); // om denna kraschar skiten sa....// den gjorde det....
			FlipSprite(direction);
		}
		else
			//velocity.x = Mathf.Lerp(0, velocity.x, Mathf.Pow(2, -10 * (float)delta)); 
		{
			velocity.X = Mathf.Lerp(0, velocity.X, Mathf.Pow(5, -10 * (float)delta)); //This is friction
		}
		
		velocity.X = Mathf.Clamp(velocity.X, -maxHorizontalSpeed, maxHorizontalSpeed);


		bool wasOnFloor = IsOnFloor();

		//Handles walljump
		if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_right")) //&& !IsOnFloor()
		{
			velocity.Y = JumpVelocity;
			velocity.X = jumpHeight;
		}
		else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_left")) // && !IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			velocity.X = -jumpHeight;
		}
		else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_right") && wallClimbJump == true)
		{
			velocity.Y = JumpVelocity;
			velocity.X = jumpHeight;
		}
		else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_left") && wallClimbJump == true)
		{
			velocity.Y = JumpVelocity;
			velocity.X = -jumpHeight;
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

	private void PlayerDie()
	{
		GetTree().ReloadCurrentScene();
		QueueFree();
		GD.Print("I'm dead");

		//EmitSignal("OnPlayerDiedEventHandler");
	}

	private void PlayerJumpOnEnemy()
	{
		GD.Print("Jump");
		isJumpingOnEnemy = true;

	}

	private void PlayerJumpOnJumpPlatform()
	{
		GD.Print("Jump");
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
