using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Events.Karma
{
    public interface IKarmaEventListener
    {
        Task StartAsync();
    }
}
