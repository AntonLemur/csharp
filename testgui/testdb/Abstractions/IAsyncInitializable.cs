using System.Threading.Tasks;

namespace testdb.Abstractions;

internal interface IAsyncInitializable
{
   Task InitializeAsync();    
}