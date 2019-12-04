using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Models;

namespace NxPlx.Abstractions
{
    public interface IIndexer
    {
        Task IndexLibraries(IEnumerable<Library> libraries);
    }
}