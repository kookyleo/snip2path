namespace Snip2Path;

static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main()
    {
        const string mutexName = "Global\\Snip2Path_SingleInstance";
        _mutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            // Another instance is already running
            return;
        }

        try
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplicationContext());
        }
        finally
        {
            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
    }
}
