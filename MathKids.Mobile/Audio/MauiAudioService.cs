using MathKids.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MathKids.Mobile.Audio;

public sealed class MauiAudioService(IAudioManager audioManager, ILocalSettings settings, ILogger<MauiAudioService> logger) : IAudioService, IDisposable
{
    private const string MutedSetting = "audio.muted";
    private const string VolumeSetting = "audio.volume";
    private static readonly IReadOnlyDictionary<AudioCue, string> EffectPaths = new Dictionary<AudioCue, string>
    {
        [AudioCue.Tap] = "Audio/Effects/tap.wav",
        [AudioCue.Correct] = "Audio/Effects/correct.wav",
        [AudioCue.TryAgain] = "Audio/Effects/try_again.wav",
        [AudioCue.Star] = "Audio/Effects/star.wav",
        [AudioCue.Whoosh] = "Audio/Effects/whoosh.wav"
    };
    private static readonly IReadOnlyDictionary<MusicCue, string> MusicPaths = new Dictionary<MusicCue, string>
    {
        [MusicCue.Home] = "Audio/Music/menu_theme.mp3",
        [MusicCue.Addition] = "Audio/Music/addition_theme.mp3",
        [MusicCue.Bingo] = "Audio/Music/condor_theme.mp3",
        [MusicCue.Puma] = "Audio/Music/puma_theme.mp3",
        [MusicCue.ChankaLaboratory] = "Audio/Music/chanka_lab_theme.mp3"
    };
    private readonly Dictionary<AudioCue, AudioResource> _effects = [];
    private readonly Dictionary<MusicCue, AudioResource> _music = [];
    private readonly SemaphoreSlim _initializeLock = new(1, 1);
    private IAudioPlayer? _currentMusic;
    private MusicCue? _currentMusicCue;
    private bool _initialized;
    private bool _musicWasPlaying;

    public bool IsMuted { get; private set; } = bool.TryParse(settings.Get(MutedSetting), out var muted) && muted;
    public float Volume { get; private set; } = ParseVolume(settings.Get(VolumeSetting));

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized) return;
        await _initializeLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized) return;
            foreach (var pair in EffectPaths) await TryLoadAsync(pair.Key, pair.Value, _effects, cancellationToken);
            foreach (var pair in MusicPaths) await TryLoadAsync(pair.Key, pair.Value, _music, cancellationToken);
            _initialized = true;
            ApplyVolume();
        }
        finally
        {
            _initializeLock.Release();
        }
    }

    public void PlayEffect(AudioCue cue)
    {
        if (IsMuted || !_effects.TryGetValue(cue, out var resource)) return;
        if (resource.Player.IsPlaying) resource.Player.Stop();
        resource.Player.Volume = Volume;
        resource.Player.Play();
    }

    public void PlayMusic(MusicCue cue, bool loop = true)
    {
        if (!_music.TryGetValue(cue, out var resource)) return;
        if (_currentMusicCue == cue && _currentMusic?.IsPlaying == true) return;
        _currentMusic?.Stop();
        _currentMusic = resource.Player;
        _currentMusicCue = cue;
        _currentMusic.Loop = loop;
        _currentMusic.Volume = IsMuted ? 0d : Volume * 0.55d;
        if (!IsMuted) _currentMusic.Play();
    }

    public void PauseMusic()
    {
        _musicWasPlaying = _currentMusic?.IsPlaying == true;
        if (_musicWasPlaying) _currentMusic!.Pause();
    }

    public void ResumeMusic()
    {
        if (!IsMuted && _musicWasPlaying && _currentMusic is not null) _currentMusic.Play();
        _musicWasPlaying = false;
    }

    public void StopMusic()
    {
        _currentMusic?.Stop();
        _currentMusic = null;
        _currentMusicCue = null;
        _musicWasPlaying = false;
    }

    public void SetMuted(bool muted)
    {
        if (IsMuted == muted) return;
        IsMuted = muted;
        settings.Set(MutedSetting, muted.ToString());
        if (muted) _currentMusic?.Pause();
        else if (_currentMusic is not null) _currentMusic.Play();
        ApplyVolume();
    }

    public void SetVolume(float volume)
    {
        Volume = Math.Clamp(volume, 0f, 1f);
        settings.Set(VolumeSetting, Volume.ToString(System.Globalization.CultureInfo.InvariantCulture));
        ApplyVolume();
    }

    private async Task TryLoadAsync<TKey>(TKey key, string path, Dictionary<TKey, AudioResource> target, CancellationToken cancellationToken) where TKey : notnull
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var stream = await FileSystem.OpenAppPackageFileAsync(path);
            target[key] = new AudioResource(stream, audioManager.CreatePlayer(stream));
        }
        catch (FileNotFoundException)
        {
            logger.LogInformation("Audio opcional no encontrado: {AudioPath}", path);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "No se pudo cargar el audio {AudioPath}", path);
        }
    }

    private void ApplyVolume()
    {
        foreach (var resource in _effects.Values) resource.Player.Volume = IsMuted ? 0d : Volume;
        foreach (var resource in _music.Values) resource.Player.Volume = IsMuted ? 0d : Volume * 0.55d;
    }

    private static float ParseVolume(string? value) => float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var volume)
        ? Math.Clamp(volume, 0f, 1f)
        : 0.8f;

    public void Dispose()
    {
        StopMusic();
        foreach (var resource in _effects.Values) resource.Dispose();
        foreach (var resource in _music.Values) resource.Dispose();
        _effects.Clear(); _music.Clear(); _initializeLock.Dispose();
    }

    private sealed class AudioResource(Stream stream, IAudioPlayer player) : IDisposable
    {
        public IAudioPlayer Player { get; } = player;
        public void Dispose() { Player.Dispose(); stream.Dispose(); }
    }
}
