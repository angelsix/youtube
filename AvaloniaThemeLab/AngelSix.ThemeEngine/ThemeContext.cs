using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AngelSix.ThemeEngine;

public partial class ThemeContext : ObservableObject
{
    [ObservableProperty]
    private Theme _active = new DefaultTheme();

    /// <summary>
    /// Resolves the active <see cref="ThemeContext"/> for generated markup extensions.
    /// Set by the constructor; consumers can override to plug in a custom DI lookup.
    /// </summary>
    public static Func<ThemeContext>? Resolver { get; set; }

    /// <summary>
    /// Creates a new <see cref="ThemeContext"/> and registers it as the resolver for the
    /// <c>{theme:...}</c> markup extensions emitted by the AngelSix.ThemeEngine source
    /// generator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The constructor verifies that the source generator has run in the project that
    /// contains your theme axaml (the project where
    /// <c>xmlns:theme="using:AngelSix.ThemeEngine.Generated"</c> is used). If the marker
    /// type emitted by the generator is not found, construction throws an
    /// <see cref="InvalidOperationException"/> whose message tells you exactly how to wire
    /// it.
    /// </para>
    /// <para><b>Recommended — install via NuGet.</b> In the project that owns your theme
    /// axaml, add:</para>
    /// <code>
    /// dotnet add package AngelSix.ThemeEngine.SourceGen
    /// </code>
    /// <para>The NuGet package places the generator under <c>analyzers/dotnet/cs/</c>, so
    /// the build automatically loads it as a Roslyn source generator — no extra csproj
    /// attributes required.</para>
    /// <para><b>Alternative — local <c>ProjectReference</c>.</b> If you've cloned the
    /// generator source rather than using NuGet, reference it with the analyzer-marker
    /// attributes (adjust the relative path):</para>
    /// <code>
    /// &lt;ItemGroup&gt;
    ///   &lt;ProjectReference Include="..\AngelSix.ThemeEngine.SourceGen\AngelSix.ThemeEngine.SourceGen.csproj"
    ///                     OutputItemType="Analyzer"
    ///                     ReferenceOutputAssembly="false" /&gt;
    /// &lt;/ItemGroup&gt;
    /// </code>
    /// <para><c>OutputItemType="Analyzer"</c> is what marks the reference as a Roslyn
    /// source generator rather than a normal library reference;
    /// <c>ReferenceOutputAssembly="false"</c> keeps the generator DLL out of the runtime
    /// output.</para>
    /// </remarks>
    public ThemeContext()
    {
        EnsureSourceGeneratorWired();
        Resolver = () => this;
    }

    private const string MarkerFullName = "AngelSix.ThemeEngine.Generated.__ThemeEngineGenerated";

    private static void EnsureSourceGeneratorWired()
    {
        if (MarkerExistsInLoadedAssemblies())
            return;

        // Force-load any assemblies referenced by the entry app — the marker may live in one
        // that hasn't been touched yet.
        var entry = Assembly.GetEntryAssembly();
        if (entry is not null)
        {
            foreach (var reference in entry.GetReferencedAssemblies())
            {
                try { Assembly.Load(reference); }
                catch { /* best-effort */ }
            }

            if (MarkerExistsInLoadedAssemblies())
                return;
        }

        throw new InvalidOperationException(BuildMissingGeneratorMessage());
    }

    private static bool MarkerExistsInLoadedAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic) continue;
            try
            {
                if (assembly.GetType(MarkerFullName, throwOnError: false) is not null)
                    return true;
            }
            catch
            {
                // best-effort
            }
        }
        return false;
    }

    private static string BuildMissingGeneratorMessage() =>
        """
        AngelSix.ThemeEngine source generator was not detected.

        The generator emits the {theme:...} markup extensions used by your XAML.
        Wire it into the project that owns your theme axaml (the project where
        xmlns:theme="using:AngelSix.ThemeEngine.Generated" is used) one of two ways:

        1. NuGet (recommended). The package places the generator under analyzers/dotnet/cs
           in your build, so no extra csproj attributes are required:

               dotnet add package AngelSix.ThemeEngine.SourceGen

           or in the csproj:

               <PackageReference Include="AngelSix.ThemeEngine.SourceGen" Version="1.0.0" />

        2. Local ProjectReference (for when you've cloned the generator source). The
           OutputItemType="Analyzer" attribute is what registers the reference as a
           Roslyn source generator; ReferenceOutputAssembly="false" keeps the
           generator DLL out of the runtime output. Adjust the relative path:

               <ItemGroup>
                 <ProjectReference Include="..\AngelSix.ThemeEngine.SourceGen\AngelSix.ThemeEngine.SourceGen.csproj"
                                   OutputItemType="Analyzer"
                                   ReferenceOutputAssembly="false" />
               </ItemGroup>

        Rebuild the solution after either of the above. The generator emits a marker
        type (AngelSix.ThemeEngine.Generated.__ThemeEngineGenerated) that this check
        looks for; if you still see this message after rebuilding, the analyzer is
        not loading — confirm the package restored, or that the ProjectReference path
        is correct.
        """;
}
