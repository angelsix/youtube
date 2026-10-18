Not verified live 2026-10-16: rewritten for plan 06 from `StreamElectronics/Software/StreamElectronics.csproj`; not yet followed by --live.

## Start
- Build: `dotnet build StreamElectronics/Software/StreamElectronics.csproj -c Debug` (.NET 10 SDK)
- Run: `./StreamElectronics/Software/bin/Debug/net10.0/StreamElectronics </dev/null >/dev/null 2>&1 & echo $! > "$TMPDIR/stream-electronics.pid"`

## URL
- N/A — desktop window (Avalonia).

## Login or demo data
- None. The multimeter gauges animate from simulated readings; no device or account is needed.

## Shot commands
- `guard shot avalonia StreamElectronics/Software/Views/MainWindow.axaml --out "$TMPDIR/stream-electronics-mainwindow.png"`
- Note: the BatchProcess3Host project (under Avalonia BatchProcess/Source) is another net10.0 host in this repo, but its MainWindow embeds ASP.NET Core assemblies the headless harness cannot load, so it does not render offscreen.

## Teardown
- `kill $(cat "$TMPDIR/stream-electronics.pid") 2>/dev/null || true`
