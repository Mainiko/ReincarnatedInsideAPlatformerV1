using Godot;
using System;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;

public partial class player : CharacterBody2D
{

    [Signal] public delegate void OnPlayerDiedEventHandler();


    public const float Speed = 125.0f;
    public const float JumpVelocity = -250.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    AnimatedSprite2D animatedSprite2D = null;
    Timer coyoteTimer = null;


    private enum State { NORMAL, DASHING, INPUT_DISABLED }

    private Vector2 velocity = Vector2.Zero;
    private int maxHorizontalSpeed = 200;
    private int horizontalAcceleration = 2000;
    private int jumpSpeed = 300;
    private int jumpTerminationMultiplier = 4;
    private bool hasDoubleJump = false;
    private bool hasDash = false;
    private State currentState = State.NORMAL;
    private bool isStateNew = true;
    private bool isDying = false;
    private float acceleration = 0.25f;
    //private float acceleration = 0.25f;
    private int wallGravity = 900;
    private bool isDashing = false;
    private int addiditionalFallGravity = 1500;

    //Celeste tutorial
    private int jumpHeight = 300;
    [Export]private int dashSpeed = 1500;

    private int defaultHazardMask = 0;

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
            if (GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_left") && velocity.y > 0)
            {
                velocity.y += (gravity - wallGravity) * (float)delta;
            }
            else if (GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_right") && velocity.y > 0)
            {
                velocity.y += (gravity - wallGravity) * (float)delta;
            }
            else if (Velocity.y > 0) //If player is falling 
            {
                velocity.y += addiditionalFallGravity * (float)delta; //Apply fast fall
            }
            else
            {
                velocity.y += gravity * (float)delta;
            }

        }

        if (IsOnFloor())
        {
            hasDoubleJump = false;
            hasDash = true;

        }
     

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && (IsOnFloor() || !coyoteTimer.IsStopped() || hasDoubleJump))
        {
            velocity.y = JumpVelocity;
            if(!IsOnFloor() && coyoteTimer.IsStopped())
            {
                hasDoubleJump=false;
            }
        }

        Vector2 direction = GetDirectionVector();

        if (direction != Vector2.Zero)
        {
            //velocity.x = direction.x * Speed;
            velocity.x = Mathf.Lerp(velocity.x, direction.x * Speed, acceleration); // om denna kraschar skiten sa....// den gjorde det....
            FlipSprite(direction);
        }
        else
            //velocity.x = Mathf.Lerp(0, velocity.x, Mathf.Pow(2, -10 * (float)delta)); 
        {
            velocity.x = Mathf.Lerp(0, velocity.x, Mathf.Pow(5, -10 * (float)delta)); //This is friction
            //velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
        }
        
        velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);


        bool wasOnFloor = IsOnFloor();

        //Handles walljump
        if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_right"))
        {
            velocity.y = JumpVelocity;
            velocity.x = jumpHeight;
        }
        else if(Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_left"))
        {
            velocity.y = JumpVelocity;
            velocity.x = -jumpHeight;
        }



        //Handle Dashing
        if (Input.IsActionJustPressed("dash") && hasDash && !IsOnFloor())
        {

            //Create timer that will signal when the dash is done?
            //So we can start applaying gravity again
            isDashing = true;
            velocity.y = -100;


            //Makes player dash at the facing direction 
            bool currentFacingDirection = animatedSprite2D.IsFlippedH();
            int facingDirValue = 0;
            if (currentFacingDirection)
            {
                facingDirValue = -1;
            }
            else
            {
                facingDirValue = 1;
            }

            direction.x = facingDirValue;

            //velocity.x = Mathf.Lerp(velocity.x, direction.x * dashSpeed, acceleration);
            velocity.x = Mathf.Lerp(velocity.x, direction.x * dashSpeed, 0.15f);
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
        if (direction.x < 0)
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
        QueueFree();
        GD.Print("I ded");

        //EmitSignal("OnPlayerDiedEventHandler");



    }

}