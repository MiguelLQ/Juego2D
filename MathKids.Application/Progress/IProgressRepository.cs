using MathKids.Domain.Progress;

namespace MathKids.Application.Progress;

public interface IProgressRepository { PlayerProgress Get(Guid profileId); void Save(PlayerProgress progress); }

//hola mundo
