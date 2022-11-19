using AvaloniaLoudnessMeter.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaLoudnessMeter.Services;

public interface IAudioInterfaceService
{
    /// <summary>
    /// Fetch the channel configurations
    /// </summary>
    /// <returns></returns>
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();
}