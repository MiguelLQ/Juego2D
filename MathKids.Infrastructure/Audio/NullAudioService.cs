using MathKids.Application.Abstractions;

namespace MathKids.Infrastructure.Audio;

public sealed class NullAudioService : IAudioService
{
    public bool IsMuted { get; private set; }
    public float Volume { get; private set; } = 1f;
    public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void PlayEffect(AudioCue cue) { }
    public void PlayMusic(MusicCue cue, bool loop = true) { }
    public void PauseMusic() { }
    public void ResumeMusic() { }
    public void StopMusic() { }
    public void SetMuted(bool muted) => IsMuted = muted;
    public void SetVolume(float volume) => Volume = Math.Clamp(volume, 0f, 1f);
}
