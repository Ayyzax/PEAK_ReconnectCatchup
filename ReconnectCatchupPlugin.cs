using BepInEx;
using BepInEx.Logging;

namespace Ayzax.ReconnectCatchup;

[BepInProcess("PEAK.exe")]
[BepInPlugin("Ayzax.ReconnectCatchup", "Reconnect Catchup", "1.0.0")]
public class ReconnectCatchupPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Reconnect Catchup is loaded!");

        ReconnectCatchupHandler catchupHandler = gameObject.AddComponent<ReconnectCatchupHandler>();
        catchupHandler.InitLogger(Logger);
    }
}
