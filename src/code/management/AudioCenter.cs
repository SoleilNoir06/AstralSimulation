using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astral_simulation
{
    public static class AudioCenter
    {
        private static Dictionary<string, Music> _musics = new Dictionary<string, Music>();
        private static Dictionary<string, Sound> _sounds = new Dictionary<string, Sound>();

        private static List<string> _playingMusics = new List<string>();

        public static void Init()
        {
            _musics = RLoading.LoadMusics();
            _sounds = RLoading.LoadSounds();
            // Play ambient music
            PlayMusic("ambient");
        }

        public static void Update()
        {
            _playingMusics.ForEach(music => 
            {
                Raylib.UpdateMusicStream(_musics[music]);
            });
        }

        public static void PlaySound(string name)
        {
            Raylib.PlaySound(_sounds[name]);
        }

        public static void StopSound(string name) 
        { 
            Raylib.StopSound(_sounds[name]);
        }

        public static void PauseSound(string name)
        {
            Raylib.PauseSound(_sounds[name]);
        }

        public static void PlayMusic(string name)
        {
            Raylib.PlayMusicStream(_musics[name]);
            _playingMusics.Add(name);
        }

        public static void StopMusic(string name) 
        {
            Raylib.StopMusicStream(_musics[name]);
            _playingMusics.Remove(name);
        }
    }
}
