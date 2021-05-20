using System.Threading.Tasks;

namespace NxPlx.Application.Core
{
    public interface IIndexingService
    {
        Task IndexLibraries(int[] libraries);
    }
}