using System.Threading.Tasks;

namespace NxPlx.Application.Core
{
    public interface ICacheClearer
    {
        Task Clear(string pattern);
    }
}