using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models;

namespace COMP3000_Project_Backend_API.Services
{
    public interface IMetadataService
    {
        public Task<List<DEFRAMetadata>> GetAsync(BoundingBox bbox);
    }
}
