namespace Demo.Entity
{
    public interface IControllerComponent
    {
        void SetupController(ControllerBase controller);
        void SetupPlayer(EntityInitialiser playerObject);
    }
}
