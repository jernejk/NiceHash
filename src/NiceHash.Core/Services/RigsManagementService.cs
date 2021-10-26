using NiceHash.Core.Models;

namespace NiceHash.Core;

public interface IRigsManagementService
{
    Task<WorkersResponse?> GetActiveWorkers();
    Task<RigsResponse?> GetRigs();
    Task<ActionResponse?> RunRigAction(string rigId, string action);
    Task<ActionResponse?> StartRig(string rigId);
    Task<ActionResponse?> StopRig(string rigId);
}

internal class RigsManagementService : IRigsManagementService
{
    private readonly INiceHashService _niceHashService;

    public RigsManagementService(INiceHashService niceHashService)
    {
        _niceHashService = niceHashService;
    }

    public async Task<WorkersResponse?> GetActiveWorkers()
        => await _niceHashService.Get<WorkersResponse>("/main/api/v2/mining/rigs/activeWorkers");

    public async Task<RigsResponse?> GetRigs()
        => await _niceHashService.Get<RigsResponse>("/main/api/v2/mining/rigs2");

    public async Task<ActionResponse?> StopRig(string rigId)
        => await RunRigAction(rigId, "STOP");

    public async Task<ActionResponse?> StartRig(string rigId)
        => await RunRigAction(rigId, "START");

    public async Task<ActionResponse?> RunRigAction(string rigId, string action)
        => await _niceHashService.Post<RigActionRequest, ActionResponse>("/main/api/v2/mining/rigs/status2",
            new RigActionRequest
            {
                RigId = rigId,
                Action = action
            });
}
