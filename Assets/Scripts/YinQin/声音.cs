using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-100)] // 确保在其他脚本之前初始化
public class 声音 : MonoBehaviour
{
    private static 声音 _instance;
    public static 声音 Instance
    {
        get
        {
            if (_instance == null)
            {
                // 自动创建实例
                var go = new GameObject("AudioManager (自动创建)");
                _instance = go.AddComponent<声音>();
                Instance._musicSource.volume = 0.5f;
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // 定义音效枚举
    public enum SoundEffect
    {
        空,
        // 在这里添加你的音效枚举
        sndBlockChange,
        sndBossHit,
        sndCherry,
        sndDeath,
        sndDJump,
        sndGlass,
        sndItem,
        sndJump,
        sndShoot,
        sndSpikeTrap,
        sndWallJump
        // 添加更多音效...
    }

    // 定义音乐枚举
    public enum MusicTrack
    {
        // 在这里添加你的音乐枚举
        S1BOSS,
        S2关卡,
        本家BGM,
        死亡音效,
        鸟之诗,
        默认BGM
        // 添加更多音乐...
    }

    private const string SOUND_EFFECTS_FOLDER = "SFX";
    private const string MUSIC_TRACKS_FOLDER = "Music";

    private AudioSource _soundEffectSource;
    private AudioSource _musicSource;

    private Dictionary<SoundEffect, AudioClip> _soundEffectDictionary;
    private Dictionary<MusicTrack, AudioClip> _musicTrackDictionary;

    public static AudioSource getBGMY()
    {
        return  _instance?._musicSource;
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        // 初始化音频源
        _soundEffectSource = gameObject.AddComponent<AudioSource>();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;

        // 加载音效
        LoadAudioAssets<SoundEffect>(SOUND_EFFECTS_FOLDER, out _soundEffectDictionary);
        
        // 加载音乐
        LoadAudioAssets<MusicTrack>(MUSIC_TRACKS_FOLDER, out _musicTrackDictionary);
    }

    private void LoadAudioAssets<T>(string folderPath, out Dictionary<T, AudioClip> dictionary) where T : System.Enum
    {
        dictionary = new Dictionary<T, AudioClip>();
        
        // 加载指定文件夹中的所有音频文件
        AudioClip[] clips = Resources.LoadAll<AudioClip>(folderPath);
        
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"没有在Resources/{folderPath}文件夹中找到音频文件！");
            return;
        }

        // 获取所有枚举值
        var enumValues = System.Enum.GetValues(typeof(T)).Cast<T>();

        foreach (var enumValue in enumValues)
        {
            string enumName = enumValue.ToString();
            if(enumName == "空") continue;
            AudioClip clip = clips.FirstOrDefault(c => c.name == enumName);
            
            if (clip != null)
            {
                dictionary[enumValue] = clip;
            }
            else
            {
                Debug.LogWarning($"在Resources/{folderPath}文件夹中找不到与枚举值'{enumName}'匹配的音频文件！");
            }
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="effect">音效枚举</param>
    public static void Play音效(SoundEffect effect)
    {
        if (effect == SoundEffect.空) return;
        if (Instance._soundEffectDictionary.TryGetValue(effect, out AudioClip clip))
        {
            Instance._soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect {effect} not found in dictionary!");
        }
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="track">音乐枚举</param>
    public static void Play音乐(MusicTrack track)
    {
        if (Instance._musicTrackDictionary.TryGetValue(track, out AudioClip clip))
        {
            if (Instance._musicSource.clip != clip)
            {
                Instance._musicSource.Stop();
                Instance._musicSource.clip = clip;
                Instance._musicSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"Music track {track} not found in dictionary!");
        }
    }


    public static void Play音乐(AudioClip clip)
    {

        if (Instance._musicSource.clip != clip)
        {
            Instance._musicSource.Stop();
            Instance._musicSource.clip = clip;
            Instance._musicSource.Play();
        }
    }

    public static void Play音效(AudioClip clip)
    {
        Instance._soundEffectSource.PlayOneShot(clip);
    }

    public static AudioClip getBGM()
    {
        return Instance._musicSource.clip;
    }


    /// <summary>
    /// 停止当前播放的音乐
    /// </summary>
    public static void StopMusic()
    {
        Instance._musicSource.Stop();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">0-1之间的值</param>
    public static void 设置音效音量(float volume)
    {
        Instance._soundEffectSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">0-1之间的值</param>
    public static void 设置音乐音量(float volume)
    {
        Instance._musicSource.volume = Mathf.Clamp01(volume);
    }

    public static void 设置音乐进度(float 进度)
    {
        Instance._musicSource.time = Instance._musicSource.clip.length * 进度;
    }
    /// <summary>
    /// 预加载所有音频资源
    /// </summary>
    public static void PreloadAudioAssets()
    {
        // 访问Instance属性会自动初始化
        var _ = Instance;
    }
}