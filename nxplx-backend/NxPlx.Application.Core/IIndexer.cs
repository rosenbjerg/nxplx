using System.Threading.Tasks;

namespace NxPlx.Application.Core
{
    public interface IIndexer
    {
        Task IndexLibraries(int[] libraries);
    }
}