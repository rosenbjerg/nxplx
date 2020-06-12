using System.Collections.Generic;
using System.Threading.Tasks;
using NxPlx.Models;

namespace NxPlx.Application.Core
{
    public interface IIndexer
    {
        Task IndexLibraries(IEnumerable<Library> libraries);
    }
}