using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsekaiPlatformerGD4.Scripts
{
	partial class AudioManager : Node
	{
		private AudioStreamPlayer _musicPlayer;

		public override void _Ready()
		{
			// Create the AudioStreamPlayer instance
			_musicPlayer = new AudioStreamPlayer();
			AddChild(_musicPlayer);

			// Load your music file here
			_musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/Music/OSTFONDEWUTANINTRO.mp3");
			_musicPlayer.Autoplay = true;
			_musicPlayer.Play();
		}

		public void PlayMusic()
		{
			if (!_musicPlayer.Playing)
			{
				_musicPlayer.Play();
			}
		}

		public void StopMusic()
		{
			_musicPlayer.Stop();
		}
	}

}
