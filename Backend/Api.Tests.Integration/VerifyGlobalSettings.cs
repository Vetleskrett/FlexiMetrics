using System.Runtime.CompilerServices;

namespace Api.Tests.Integration;

public class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ScrubInlineGuids();
        UseSourceFileRelativeDirectory("snapshots");
        VerifyHttp.Initialize();
        Recording.Start();
    }
}
