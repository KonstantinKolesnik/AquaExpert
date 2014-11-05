
namespace MySensors.Controllers.Automation
{
    public interface IAutomationService
    {
        void Start(Controller controller);
        void Stop();
    }
}
