using Godot;
using System;

public partial class SoundHandle : Node
{
    public AudioStreamPlayer musicinstance = new AudioStreamPlayer();
    Tween tween;
    private AudioStream musicname;
    private AudioStream sfxname;
    public static float musvolume;
    public static float sfxvolume;
    public static float mastervolume;
    public static float resumemsec = 0;
    private static int doorindex = AudioServer.GetBusIndex("door");
    private static int tableindex = AudioServer.GetBusIndex("table");
    private static int clockindex = AudioServer.GetBusIndex("clock");
    public static float doorpan = -0.5f;
    public static float tablepan = 0;
    public static float clockpan = 0.5f;
    AudioEffectPanner doorpanner = doorindex >= 0 ? AudioServer.GetBusEffect(doorindex, 0) as AudioEffectPanner : null;
    AudioEffectPanner tablepanner = tableindex >= 0 ? AudioServer.GetBusEffect(tableindex, 0) as AudioEffectPanner : null;
    AudioEffectPanner clockpanner = clockindex >= 0 ? AudioServer.GetBusEffect(clockindex, 0) as AudioEffectPanner : null;
    public static SoundHandle Instance { get; private set; }
    //BOOT
    public override void _Ready()
    {
        //INSTANCE ACTIVATION
        if (Instance == null) Instance = this;
        //MUSICINSTANCE ACTIVATION
        musicinstance.Bus = "music";
        AddChild(musicinstance);
    }
    //MAIN
    public void PlayMusic(string newmusicpath, float position = 0)
    {
        AudioStream musicname = GD.Load<AudioStream>(newmusicpath);
        musicinstance.Stream = musicname;
        musicinstance.Play(position);
    }
    public void PlaySFX(string newsfxpath)
    {
        var stream = GD.Load<AudioStream>(newsfxpath);
        var sfxPlayer = new AudioStreamPlayer();
        sfxPlayer.Stream = stream;
        sfxPlayer.Bus = "sfx";
        AddChild(sfxPlayer);
        sfxPlayer.Play();
        sfxPlayer.Connect("finished", Callable.From(() => sfxPlayer.QueueFree()));
    }
    public void PlaySFXPanned(string newsfxpath, int mode)
    {
        var stream = GD.Load<AudioStream>(newsfxpath);
        var sfxPanPlayer = new AudioStreamPlayer2D();
        sfxPanPlayer.Stream = stream;
        switch (mode)
        {
            case 1:
                sfxPanPlayer.Bus = "door";
                break;
            case 2:
                sfxPanPlayer.Bus = "table";
                break;
            case 3:
                sfxPanPlayer.Bus = "clock";
                break;
        }
        AddChild(sfxPanPlayer);
        sfxPanPlayer.Play();
        sfxPanPlayer.Connect("finished", Callable.From(() => sfxPanPlayer.QueueFree()));
    }
    //CHANGE VOLUME FUNCTIONS
    public static void ChangeMusVolume(double value)
    {
        SoundHandle.musvolume = (float)value;
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("music"), Mathf.LinearToDb(SoundHandle.musvolume));
    }
    public static void ChangeSFXVolume(double value)
    {
        SoundHandle.sfxvolume = (float)value;
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("sfx"), Mathf.LinearToDb(SoundHandle.sfxvolume));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("door"), Mathf.LinearToDb(SoundHandle.sfxvolume));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("table"), Mathf.LinearToDb(SoundHandle.sfxvolume));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("clock"), Mathf.LinearToDb(SoundHandle.sfxvolume));
    }
    public static void ChangeMasterVolume(double value)
    {
        SoundHandle.mastervolume = (float)value;
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(SoundHandle.mastervolume));
    }
    public static void FadeMusicOut(double value = 1.0f, bool instantnew = true)
    {
        var tween = Instance.CreateTween();
        float past_value = Instance.musicinstance.VolumeDb;
        tween.TweenProperty(Instance.musicinstance, "volume_db", -80f, 1.0f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.In);
        tween.TweenCallback(Callable.From(() =>
        {
            resumemsec = Instance.musicinstance.GetPlaybackPosition();
            Instance.musicinstance.Stop();
            Instance.musicinstance.VolumeDb = past_value;
            if (instantnew) NewBgMusic();
        }));
    }
    public static void NewBgMusic()
    {
        if (!Instance.musicinstance.Playing)
        {
            if (SceneHandle.Instance.curr_scene == "menu")
            {
                Instance.PlayMusic("res://mus/FrozenInPlace.ogg", resumemsec);
            }
            else if (SceneHandle.Instance.curr_scene == "cutscene")
            {
                Instance.PlayMusic("res://mus/Unraveling.ogg", resumemsec);
            }
            else if (GameProcessHandle.night_ongoing)
            {
                Instance.PlayMusic("res://sfx/amb.ogg");
            }
        }
    }
    public void TweakPanBusses(int mode)
    {
        switch (mode)
        {
            case 1:
                doorpan = 0f;
                tablepan = 0.5f;
                clockpan = 1f;
                break;
            case 2:
                doorpan = -0.5f;
                tablepan = 0f;
                clockpan = 0.5f;
                break;
            case 3:
                doorpan = -1f;
                tablepan = -0.5f;
                clockpan = 0f;
                break;
        }
        var tween = CreateTween();
        tween.SetParallel();
        tween.TweenProperty(doorpanner, "pan", doorpan, 1.0f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        tween.TweenProperty(tablepanner, "pan", tablepan, 1.0f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        tween.TweenProperty(clockpanner, "pan", clockpan, 1.0f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
    }
}