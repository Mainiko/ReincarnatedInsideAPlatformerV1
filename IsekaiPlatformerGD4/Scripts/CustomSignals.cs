using Godot;
using System;

public partial class CustomSignals : Node
{
	//[Signal] public delegate void OnPlayerDiedEventHandler();

	[Signal] public delegate void EventChangedEventHandler(string eventName);

}
