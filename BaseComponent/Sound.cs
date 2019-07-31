using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// 音源
    /// </summary>
    public class Sound
    {
        private static float s_bgmVolume = 1;
        private static float s_seVolume = 1;

        public static Sound Bgm { get; private set; }

        /// <summary>
        /// BGMとして再生
        /// </summary>
        /// <param name="sound">音源</param>
        /// <param name="fade">フェード時間</param>
        /// <param name="startPoint">ループ開始</param>
        /// <param name="endPoint">ループ終了</param>
        public static void StartBgm(Sound sound, float fade = 0, float? startPoint = null, float? endPoint = null)
        {
            if (Bgm != null && asd.Engine.Sound.GetIsPlaying(Bgm.id))
            {
                asd.Engine.Sound.FadeOut(Bgm.id, fade);
            }
            if (sound.sound == null) return;
            sound.sound.IsLoopingMode = true;
            sound.sound.LoopStartingPoint = startPoint != null ? (float)startPoint : 0;
            sound.sound.LoopEndPoint = endPoint != null ? (float)endPoint : sound.sound.Length;
            Bgm = sound;
            Bgm.id = asd.Engine.Sound.Play(sound.sound);
            asd.Engine.Sound.SetVolume(Bgm.id, 0);
            asd.Engine.Sound.Fade(Bgm.id, fade, BgmVolume);
        }

        /// <summary>
        /// BGMを止める
        /// </summary>
        /// <param name="fade">フェード時間</param>
        public static void StopBgm(float fade = 0)
        {
            if (Bgm != null && asd.Engine.Sound.GetIsPlaying(Bgm.id))
            {
                asd.Engine.Sound.FadeOut(Bgm.id, fade);
            }
        }

        /// <summary>
        /// BGMボリューム
        /// </summary>
        public static float BgmVolume
        {
            get => s_bgmVolume;
            set
            {
                if (s_bgmVolume >= 0f && s_bgmVolume <= 1f) s_bgmVolume = value;
            }
        }

        /// <summary>
        /// SEボリューム
        /// </summary>
        public static float SeVolume
        {
            get => s_seVolume;
            set
            {
                if (s_seVolume >= 0f && s_seVolume <= 1f) s_seVolume = value;
            }
        }

        asd.SoundSource sound;
        int id;

        /// <summary>
        /// 多重再生認めるか
        /// </summary>
        public bool IsMultiplePlay { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public int ID => id;

        public Sound(string path, bool isMultiplePlay = true, bool isDecompressed = false)
        {
            if (path == null) return;
            sound = asd.Engine.Sound.CreateSoundSource(path, isDecompressed);
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <returns>再生ID</returns>
        public int Play()
        {
            if (IsMultiplePlay && GetIsPlaying() && sound == null) return -1;
            id = asd.Engine.Sound.Play(sound);
            asd.Engine.Sound.SetVolume(id, SeVolume);
            return id;
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="id">再生ID</param>
        /// <returns></returns>
        public int Stop(int? id = null)
        {
            asd.Engine.Sound.Stop(id != null ? (int)id : this.id);
            return this.id;
        }

        /// <summary>
        /// 音源が再生されているかを取得
        /// </summary>
        /// <returns></returns>
        public bool GetIsPlaying()
        {
            return asd.Engine.Sound.GetIsPlaying(id);
        }
    }
}
