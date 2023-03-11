using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public interface IDEFRAShimService
    {
        Task<ShimResponse?> GetDataFromShim(DEFRAMetadata metadata, DateTime? timestamp);
    }
}
