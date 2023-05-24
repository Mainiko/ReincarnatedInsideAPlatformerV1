using Godot;
using System;

public partial class walking_spike_enemy : CharacterBody2D
{
    AnimatedSprite2D animatedSprite2D = null;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    Vector2 direction = Vector2.Right;

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        var LedgeCheckRight = GetNode<RayCast2D>("LedgeCheckRight");
        var ledgeCheckLeft = GetNode<RayCast2D>("LedgeCheckLeft");

        if (!IsOnFloor())
            velocity.y += gravity * (float)delta;

         bool found_wall = IsOnWall();
         var found_ledge = LedgeCheckRight.IsColliding() && ledgeCheckLeft.IsColliding();
        if (found_wall || !found_ledge)
        {
            GD.Print(direction.x);
            direction *= -1;
            GD.Print(direction.x);
            GD.Print(found_ledge);
            GD.Print(animatedSprite2D);
        }


        if (direction.x > 0)
            animatedSprite2D.FlipH = true;
        if (direction.x < 0)
           animatedSprite2D.FlipH = false;


        float ost = 25.0f;
	    velocity.x = direction.x * ost;
		Velocity = velocity;
		MoveAndSlide();
    }

    private void _on_hitbox_body_entered(CharacterBody2D body)
    {
        GD.Print("Body: " + body.Name + "has entered");

        //Call function from player
        if (body.Name == "player")
        {
            var player = GetNode<CharacterBody2D>(body.GetPath());
            player.Call("PlayerDie");
        }
    }
}
