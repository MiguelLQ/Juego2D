namespace MathKids.Application.Abstractions;

public interface IAudioService
{
    bool IsMuted { get; }
    float Volume { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
    void PlayEffect(AudioCue cue);
    void PlayMusic(MusicCue cue, bool loop = true);
    void PauseMusic();
    void ResumeMusic();
    void StopMusic();
    void SetMuted(bool muted);
    void SetVolume(float volume);
}

public enum AudioCue { Tap, Correct, TryAgain, Star, Whoosh }
public enum MusicCue { Home, Addition, Bingo, Puma, ChankaLaboratory }
