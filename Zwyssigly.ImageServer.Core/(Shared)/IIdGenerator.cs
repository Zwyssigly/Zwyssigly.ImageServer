using System.Threading.Tasks;

namespace Zwyssigly.ImageServer
{

    public interface IIdGenerator
    {
        Task<Id> Generate();
    }
}
