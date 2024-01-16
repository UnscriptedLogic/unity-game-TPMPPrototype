using static C_PlayerController;
using static UnscriptedEngine.UObject;

public interface IPlayerState 
{
    Bindable<PlayerState> CurrentPlayerState { get; }
}
