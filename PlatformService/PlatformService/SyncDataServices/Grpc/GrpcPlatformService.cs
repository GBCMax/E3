using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo p_repository;
        private readonly IMapper p_mapper;

        public GrpcPlatformService(IPlatformRepo _repository, IMapper _mapper)
        {
            p_repository = _repository;
            p_mapper = _mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest _request, ServerCallContext _context)
        {
            var response = new PlatformResponse();

            var platforms = p_repository.GetAllPlatforms();

            foreach (var platform in platforms)
            {
                response.Platform.Add(p_mapper.Map<GrpcPlatformModel>(platform));
            }

            return Task.FromResult(response);
        }
    }
}