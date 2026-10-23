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

# Theme Lab

Shoots ONE gallery entry of the AvaloniaThemeLab exactly as the lab shows it (the LightDarkPreview with both panes, the same header), headless, into the gitignored `AvaloniaThemeLab/Scratch/ThemeLabProof/<Entry>/<tag>/`. Nothing else goes into the public lab.

## Start
- No app start: the shot harness renders offscreen through `guard xaml inspect` on the proof view `ThemeLabProofs.GalleryEntryView` (in ai-mecca's `Tools/XamlAnalyzer/ThemeLabProofs`). It builds the lab's theme projects first, so leave a couple of minutes for a cold build.

## Shot commands
- One entry: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-shot.sh <Entry> before|after` — e.g. `theme-lab-shot.sh CheckBox before`. Add `--tall` for a 1200x900 capture (judge clipping at the default 800).
- Every entry: `theme-lab-shot.sh --all after` (reports EmbeddableControlRoot as failed — it is a TopLevel — and carries on; exits non-zero if anything failed).
- Build only: `theme-lab-shot.sh build`. Never run `dotnet build` on the lab yourself: the script serialises build plus capture behind a lock and prints when it is waiting on it.
- Outputs per entry/tag: `frame.png`, `inspection.json`, crawl and state files. Re-shooting a tag moves the previous capture aside to `<tag>.prev-<timestamp>`.
- Proof page: `/Users/lukemalpass/Documents/GitHub/ai-mecca/Tools/XamlAnalyzer/ThemeLabProofs/Scripts/theme-lab-proof-page.sh` regenerates `AvaloniaThemeLab/Scratch/ThemeLabProof/index.html` (before/after side by side; status from each entry folder's optional `status` file: fixed / redirected / open, default open).

## Login or demo data
- None.

## Teardown
- None (nothing runs persistently). To clear proof output, delete the entry folders under `AvaloniaThemeLab/Scratch/ThemeLabProof/`, keeping `references/` (the visual reference pictures).
