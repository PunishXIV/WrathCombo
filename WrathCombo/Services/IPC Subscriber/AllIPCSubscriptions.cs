namespace WrathCombo.Services.IPC_Subscriber;

public static class AllIPCSubscriptions
{
    public static void Dispose()
    {
        OrbwalkerIPC.Dispose();
    }
}