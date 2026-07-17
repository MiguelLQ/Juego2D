using MathKids.Application.Abstractions;

namespace MathKids.Infrastructure.Audio;

public sealed class NullAudioService : IAudioService
{
    public void PlayEffect(AudioCue cue) { }
}
