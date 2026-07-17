namespace MathKids.Application.Abstractions;

public interface IAudioService { void PlayEffect(AudioCue cue); }
public enum AudioCue { Tap, Correct, TryAgain }
