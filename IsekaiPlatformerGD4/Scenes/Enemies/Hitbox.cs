using Godot;
using System;

public partial class Hitbox : Area2D
{
    // Called when the node enters the scene tree for the first time.
    private void _on_body_entered(CharacterBody2D body)
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
