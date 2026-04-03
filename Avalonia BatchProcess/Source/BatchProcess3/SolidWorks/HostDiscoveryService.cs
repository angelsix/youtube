using BatchProcess3.Core.SolidWorks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcess3.SolidWorks;

public class HostDiscoveryService
{
    public async Task<List<string>> DiscoverHostsAsync(CancellationToken cancellationToken = default)
    {
        var discoveredHosts = new List<string>();
        
        var request = Encoding.UTF8.GetBytes(BatchProcessHostUrls.DiscoveryRequest);
        using var udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        
        // Send the directed broadcast message to each active NIC
        var targets = GetSubnetBroadcastAddresses();
        targets.Add(IPAddress.Loopback);

        foreach (var target in targets)
        {
            try
            {
                // Send broadcast to targets on subnet
                await udpClient.SendAsync(request, new IPEndPoint(target, BatchProcessHostUrls.DiscoveryPort), cancellationToken);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(2000);

        try
        {
            while (true)
            {
                var result = await udpClient.ReceiveAsync(timeoutCts.Token);
                var response = Encoding.UTF8.GetString(result.Buffer);
                if (response == BatchProcessHostUrls.DiscoveryResponse)
                {
                    var ip = result.RemoteEndPoint.Address.ToString();
                    if (!discoveredHosts.Contains(ip))
                        discoveredHosts.Add(ip);
                }
            }
        }
        catch (Exception e)
        {
            // ignored
        }
        
        return discoveredHosts;
    }

    public static List<IPAddress> GetSubnetBroadcastAddresses()
    {
        var broadcasts = new List<IPAddress>();
        try
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                if (ni.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel) continue;
                foreach (var unicast in ni.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                    var ip = unicast.Address.GetAddressBytes();
                    var mask = unicast.IPv4Mask.GetAddressBytes();
                    var broadcast = new byte[4];
                    for (var i = 0; i < 4; i++)
                        broadcast[i] = (byte)(ip[i] | ~mask[i]);
                    broadcasts.Add(new IPAddress(broadcast));
                }
            }
        }
        catch
        {
            // ignored
        }

        return broadcasts;
    }
}