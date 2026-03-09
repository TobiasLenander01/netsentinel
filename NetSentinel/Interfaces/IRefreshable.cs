using System.Threading.Tasks;

namespace NetSentinel.Interfaces;

public interface IRefreshable
{
    Task RefreshAsync();
}
