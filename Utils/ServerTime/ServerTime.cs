using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Replay.Utils;

namespace Replay.BackEnd
{
    public static class ServerTime
    {
        private const int TIMEOUT_MS = 3000;
        private const int MAX_RETRIES = 3;
        
        static string logTag => "ServerTime";
        
        private static readonly Dictionary<TimeServer, string> TimeServers = new Dictionary<TimeServer, string>
        {
            { TimeServer.google, "time.google.com" },
            { TimeServer.apple, "time.apple.com" },
            { TimeServer.windows, "time.windows.com" },
            { TimeServer.nist, "time.nist.gov" },     // Added NIST as additional fallback
            { TimeServer.cloudflare, "time.cloudflare.com" }  // Added Cloudflare as additional fallback
        };

        public enum TimeServer
        {
            google = 0,
            apple = 1,
            windows = 2,
            nist = 3,
            cloudflare = 4
        }

        public static async void GetNetworkTimeAsync(Action<bool, DateTime> onComplete, bool allowLocalTimeFallback = true)
        {
            bool success = false;
            DateTime dateTime = default;
            int retryCount = 0;

            // Try all servers once, then increment retryCount and try all servers again
            while (retryCount < MAX_RETRIES)
            {
                List<TimeServer> shuffledServers = new List<TimeServer>(TimeServers.Keys);
                
                // Simple shuffle to avoid always trying the same server first
                shuffledServers.Shuffle();
                
                foreach (var server in shuffledServers)
                {
                    try
                    {
                        dateTime = await Task.Run(() => GetNetworkTime(server));
                        success = true;
                        Dev.Log($"Successfully retrieved time from {server} on attempt {retryCount + 1}/{MAX_RETRIES}", logTag, true);
                        break;
                    }
                    catch (SocketException e)
                    {
                        Dev.LogWarning($"Server {server} failed on attempt {retryCount + 1}/{MAX_RETRIES}: {e.Message}", logTag, true);
                        await Task.Delay(100); // Increased delay before trying next server
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Unexpected error with {server} on attempt {retryCount + 1}/{MAX_RETRIES}: {e.Message}", logTag, true);
                        // Continue to next server instead of breaking
                    }
                }

                if (success)
                    break;

                retryCount++;

                // Add a delay between retry rounds to avoid overwhelming servers
                if (retryCount < MAX_RETRIES)
                {
                    await Task.Delay(500); // Add delay between retry rounds
                }
            }

            // Only fall back to local system time if allowed
            if (!success && allowLocalTimeFallback)
            {
                Dev.LogWarning("All time servers failed. Falling back to local system time.", logTag, true);
                dateTime = DateTime.UtcNow;
                success = true;
            }

            onComplete?.Invoke(success, dateTime);
        }

        static DateTime GetNetworkTime(TimeServer timeServer)
        {
            if (!TimeServers.TryGetValue(timeServer, out string ntpServer))
            {
                throw new ArgumentException($"Invalid time server: {timeServer}");
            }

            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.ReceiveTimeout = TIMEOUT_MS;
                socket.SendTimeout = TIMEOUT_MS;

                try
                {
                    var addresses = Dns.GetHostEntry(ntpServer).AddressList;
                    var ipEndPoint = new IPEndPoint(addresses[0], 123);
                    
                    socket.Connect(ipEndPoint);
                    socket.Send(ntpData);
                    socket.Receive(ntpData);
                }
                finally
                {
                    socket.Close();
                }
            }

            const byte serverReplyTime = 40;
            ulong intPart = SwapEndianness(BitConverter.ToUInt32(ntpData, serverReplyTime));
            ulong fractPart = SwapEndianness(BitConverter.ToUInt32(ntpData, serverReplyTime + 4));
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            return (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
        }

        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                         ((x & 0x0000ff00) << 8) +
                         ((x & 0x00ff0000) >> 8) +
                         ((x & 0xff000000) >> 24));
        }
    }
}
