using Godot;
using System;
using System.Threading.Tasks;

public partial class schrodingers_box : StaticBody2D
{
    [Export] private int timeBeforeDisappear = 100;
    int lastTime = 0;
    int timepast = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public async override void _Process(double delta)
	{
        if (timepast == timeBeforeDisappear)
        {
            this.Visible = false;
            this.CollisionLayer = 0;
            lastTime = timepast;
        }


        else if (timepast == lastTime + 100 && lastTime > timeBeforeDisappear)
        {
            this.Visible = false;
            this.CollisionLayer = 0;
            lastTime = timepast;

        }

       else if(!this.Visible) 
        {
            await Task.Delay(2000);
            this.Visible = true;
            this.CollisionLayer = 1;
            lastTime = timepast;

        }
        timepast += 1;
        GD.Print("This is timepast: " + timepast);

        GD.Print("This is last time: " + lastTime);
    }
}
