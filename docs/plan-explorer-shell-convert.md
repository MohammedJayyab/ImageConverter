# Plan: Explorer right-click convert (WinZip-style) + full GUI app

## Feature name

| Use | Name |
|-----|------|
| **Official feature name** | **Converter To** |
| User-facing (Explorer menu) | **Converter To** |
| User-facing (app checkbox) | Add **Converter To** to Windows Explorer right-click menu |
| README / help (short) | Explorer **Converter To** |
| Technical / plan alias | Explorer shell convert, shell convert |
| CLI flag | `--shell-convert` |
| Settings key | `EnableExplorerConvertMenu` |
| Registry shell key | `ConverterTo` (no spaces) |

Use **Converter To** in all user-visible text (menu, checkbox, About, help). Use *shell convert* only in code comments, commit messages, or developer docs when distinguishing from in-app **Convert to** context menu.

---

## Implementation status

| Phase | Status | Notes |
|-------|--------|--------|
| **1 — Headless convert** | Done | `ShellConvertCommandLine`, `ShellConvertRunner`, `Program.Main` branch, `SupportedFormats.TryGetFormatIndexFromShellName` |
| **2 — Explorer menu + checkbox** | Done | `ExplorerShellRegistry`, `EnableExplorerConvertMenu`, `grpExplorer` + checkbox, sync on load/toggle |
| **3 — Polish** | Partial | README updated; Inno install runs `--shell-register-menu`; uninstall cleanup in `setupScript.iss`; help RTF not updated |
| **Manual QA** | Pending | Explorer menu, multi-select, checkbox, CLI smoke test |
| **Registry fix** | Done | Win11 compact menu: **no registry submenus** — use **Converter To…** + format picker; classic menu: **Converter To** cascade |
| **Format picker** | Done | `--shell-convert-pick` + `ShellFormatPickerForm` for Windows 11 compact menu |

---

## Summary

Add a **Windows Explorer context menu** so users can right-click one or more image files → **Converter To** → (BMP, PNG, ICO, …) and convert **in place** without opening the main window.

The **same installed `ImageConverter.exe`** continues to work as today when launched from the Start menu or by double-clicking the executable (splash, main form, folder review).

| Entry point | User sees | Behavior |
|-------------|-----------|----------|
| Start menu / desktop shortcut / double-click exe | Full app (splash → `frmMain`) | Unchanged |
| Explorer → right-click → Converter To → … | No main UI (optional brief feedback) | Headless convert → exit |

---

## Goals

1. **Direct convert from Explorer** — output next to source file(s), same folder, same base name, new extension (existing `SupportedFormats.BuildOutputPath` rules).
2. **No main app UI** on the shell path — no splash, no `frmMain`, no single-instance “already running” block for shell invocations.
3. **Full GUI unchanged** for users who prefer the review pane, batch selection, resize, transparency options in context menu, undo, etc.
4. **User-controlled registration** — checkbox in the main app (**checked by default**) enables or disables the Explorer menu; app writes/removes `HKCU` registry keys when the setting changes and on startup.
5. **Reuse conversion engine** — `BatchConversionRunner`, `ImageConversion`, `ConversionRequest`, `SupportedFormats` (no duplicate Magick logic).
6. **Installer** — does not permanently own menu keys (optional: remove any legacy keys on uninstall); menu lifetime is driven by the app setting.

## Windows 11 limitation (important)

The **compact** right-click menu (Copilot, Clipchamp, Edit with Photos, …) does **not** support registry-based **submenus**. Items from `SubCommands` / `ExtendedSubCommandsKey` appear only in the **classic** menu (**Show more options** or **Shift+right-click**).

**Implemented workaround:** **Converter To…** (with ellipsis) runs `--shell-convert-pick` and opens a small **format picker** dialog. This works in the compact menu shown in user screenshots.

**One menu line only** (no duplicate, no registry cascade): **Converter To…** → format picker. Duplicate entries were caused by registering both `Converter To…` and `Converter To` plus double registration under `SystemFileAssociations` and `Software\Classes`.

True submenus in the **compact** Win11 menu require **IExplorerCommand** + **MSIX package identity** (see NanaZip / Microsoft samples) — out of scope for a simple registry-only tool.

---

## Non-goals (this phase)

- Shell menu for **resize**, **transparent background**, or **PDF/SVG** special flows beyond what a simple “To format X” implies.
- **Folder** right-click (“convert all images in folder”) — files only.
- Separate **COM shell extension DLL** — registry `command` lines only.
- **“Open with”** default app registration.
- Forwarding shell commands to an **already-running** GUI instance (optional later).

---

## User experience

### Explorer menu structure (target)

Top-level submenu label: **Converter To** (not “Image Converter” with a nested “To”).

```
Right-click file(s)
  └── Converter To
        ├── BMP (.bmp)
        ├── JPEG (.jpg)
        ├── PNG (.png)
        ├── GIF (.gif)
        ├── WEBP (.webp)
        ├── ICO (.ico)
        ├── SVG (.svg)
        └── PDF (.pdf)
```

- Registry shell key name (implementation): e.g. `ConverterTo` under `SystemFileAssociations\.png\shell\ConverterTo` with display name **Converter To**.
- Menu appears only when the feature is **enabled** in app settings (see checkbox below).
- Menu appears for **supported image extensions** (align with `SupportedFormats.IsPreviewFile` / conversion input rules).
- **Multi-select**: all selected files converted when Explorer passes multiple paths.
- **Output location**: same directory as each source (current app behavior).
- **Skip same-format**: if `photo.png` → To PNG, skip or no-op (match GUI: `SelectPathsNeedingTargetFormat`).
- **Overwrite**: follow existing `ImageConversion` / file write behavior (if destination exists, same as in-app convert).
- **Feedback**:
  - **Success, all files**: silent exit (WinZip-like), or optional short tray balloon (decide in implementation).
  - **Partial failure**: MessageBox listing failed file names + error summary.
  - **Total failure / bad args**: MessageBox with clear message; exit code ≠ 0.

### Normal app (unchanged)

- Start menu → **Image Converter** → splash → main window.
- Folder review, context menu convert, ICO size, canvas background, undo, etc. — no regression.

### In-app setting: enable Explorer menu (default on)

| UI | Detail |
|----|--------|
| Control | `CheckBox` — e.g. **“Add ‘Converter To’ to Windows Explorer right-click menu”** |
| Default | **Checked** (`true`) |
| Location | `grpBackground` (“Canvas & background”) or a small new group **“Windows Explorer”** under the review pane — keep visible without cluttering toolbar |
| On check | Register all `Converter To` shell keys under `HKCU` (see registry section) |
| On uncheck | Delete those registry keys immediately; Explorer menu disappears after refresh / new Explorer window |
| On save / exit | Persist to `config.ini` (same as other settings) |
| On startup (GUI) | Load setting → **sync registry** (register if enabled, remove if disabled) so state matches disk even after manual registry edits or upgrade |

**Why app-owned registry (not only Inno):** A checkbox must turn the menu on/off without reinstalling. Inno uninstall should still delete any keys the app may have created (shared key path documented below).

**First run after install:** If default is checked, first GUI launch registers the menu (installer does not need to duplicate format × extension keys).

---

## Architecture

### Single executable, two startup paths

```
Program.Main(string[] args)
│
├─ ShellConvertCommandLine.TryParse(args) == true
│     └─ ShellConvertRunner.Run(parsed)
│           ├─ Load AppSettings (ICO size, solid/transparent background index)
│           ├─ BatchConversionRunner.Run(paths, formatIndex, …)
│           ├─ Show errors if needed
│           └─ Environment.Exit(exitCode)   // never Application.Run
│
└─ else (no shell args / empty args)
      └─ Existing GUI path:
            mutex (single instance)
            Application.Run(StartupApplicationContext)
                  └─ frmMain load settings
                  └─ ExplorerShellRegistry.Sync(enabled, exePath)  // from checkbox + default
```

### Why one exe

- One publish folder for Inno.
- One version, one code path for Magick/native dependencies.
- Registry always points at `{app}\ImageConverter.exe`.

### Single-instance mutex

| Launch | Mutex |
|--------|--------|
| GUI (no shell args) | **Keep** current global mutex — second GUI shows “already running”. |
| Shell convert (`--shell-convert` or agreed flag) | **Do not** take GUI mutex — each right-click may spawn a short-lived process (WinZip-style). |

---

## Command-line contract

Proposed stable interface (adjust names during implementation):

```text
ImageConverter.exe --shell-convert <format> [--] <file1> [file2 ...]
```

| Part | Description |
|------|-------------|
| `--shell-convert` | Marks headless Explorer invocation (not GUI). |
| `<format>` | Short id: `jpg`, `jpeg`, `png`, `bmp`, `gif`, `webp`, `ico`, `svg`, `pdf` (case-insensitive). |
| `<fileN>` | Full paths from Explorer (`"%1"` or `"%*"`). |

**Examples (registry):**

```text
"{app}\ImageConverter.exe" --shell-convert png "%1"
"{app}\ImageConverter.exe" --shell-convert ico "%*""
```

**Parsing rules:**

- Ignore empty args.
- Reject unknown format → exit 1 + message.
- Reject non-existent paths → count as failures in batch.
- Reject non-image extensions → skip or fail per file (document choice; prefer skip with summary).
- `"%~1"` not required if paths are always quoted in registry.

**Exit codes (suggested):**

| Code | Meaning |
|------|---------|
| 0 | All requested conversions succeeded (or nothing to do) |
| 1 | One or more failures / invalid args |
| 2 | Unexpected exception |

---

## Settings used by shell path

Load via existing `AppSettingsStore` (`%AppData%\Image Converter\config.ini`):

| Setting | Shell use |
|---------|-----------|
| `IcoOutputSizeIndex` | Map to pixel size (same table as `frmMain`: 16…256) for ICO output. |
| `SolidColorIndex` | Map to `IconBackgroundKind` (white / black / transparent) — same as `GetIconBackgroundFromUi()` in `frmMain`. |

**Not configurable from shell menu in v1:** per-invocation transparency, resize, custom ICO size — user opens full app for that.

Optional later: `ShellConvertSilentSuccess=true` in config.ini.

### GUI setting (shell + checkbox)

| Setting | Type | Default | Purpose |
|---------|------|---------|---------|
| `EnableExplorerConvertMenu` | `bool` | `true` | Checkbox; when false, no Explorer menu |
| `IcoOutputSizeIndex` | `int` | (existing) | ICO output for shell convert |
| `SolidColorIndex` | `int` | (existing) | Background for shell convert |

Persist in `config.ini` via `AppSettingsStore` (new key line, migration: missing key → `true`).

---

## Windows registry (app-managed, HKCU)

**Scope:** `HKCU\Software\Classes\SystemFileAssociations\<ext>\shell\ConverterTo\...`  
**Display name:** `Converter To`  
**Internal key:** `ConverterTo` (no spaces)

Registration runs only when `EnableExplorerConvertMenu == true`.  
`ExplorerShellRegistry` centralizes create/delete so checkbox, startup, and uninstall stay consistent.

### Cascade layout (implemented — required on Windows 10/11)

Do **not** use nested `ConverterTo\shell\toPng` only; Explorer shows an empty submenu and clicking the parent gives “no app associated with this action”.

**1. Command store (once per format)** — `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\ImageConverter.ConverterTo.png`  
- `(Default)` = menu label, e.g. `PNG (.png)`  
- `command` = `"<exe>" --shell-convert png %*`

**2. Per extension parent** — `HKCU\Software\Classes\SystemFileAssociations\.png\shell\ConverterTo`  
- `MUIVerb` = `Converter To` (not `(Default)` on parent)  
- `SubCommands` = `ImageConverter.ConverterTo.jpg;ImageConverter.ConverterTo.png;…`  
- `Icon` = optional path to `ImageConverter.ico`

**Exe path:** `Application.ExecutablePath` (or published path) at register time — avoids broken menu after move if user re-enables checkbox.

**Multi-select:** use `"%*"` in `Command` (test on Windows 10/11).

### Option B — single `*` menu (not recommended)

Menu on all files; worse UX. Not used if per-extension registration is implemented.

### Inno Setup (`Setup/setupScript.iss`)

- **Install:** `[Run]` with `--shell-register-menu` (elevated setup) calls `ExplorerShellRegistry.Sync(true, exePath)` so `HKLM` CommandStore verbs are written without the user running the app as admin.
- **Do not** duplicate static `[Registry]` blocks (app + checkbox own HKCU parent; uninstall must stay in sync).
- **Uninstall:** `ConverterTo_UninstallRegistry` deletes HKCU association keys and HKLM/HKCU CommandStore verbs.
- **App:** checkbox on/off and **Refresh Explorer menu** still call `Sync` (path updates, disable removes keys).

### Icons

- Parent **Converter To** item: `ImageConverter.ico` next to exe (or full path in registry `Icon` value).

---

## Files to create or change

### New files

| File | Responsibility |
|------|----------------|
| `ImageConverter/Shell/ShellConvertCommandLine.cs` | Parse/validate `args`; expose format index + file paths. |
| `ImageConverter/Shell/ShellConvertRunner.cs` | Load settings, run `BatchConversionRunner`, aggregate errors, exit code. |
| `ImageConverter/Shell/ExplorerShellRegistry.cs` | Create/delete Convert to menu keys; `Sync(bool enabled, string exePath)`. |
| `ImageConverter/Shell/ShellHost.cs` | CLI entry for `--shell-register-menu`, `--shell-convert`, `--shell-scale`. |
| `ImageConverter/Shell/ExplorerShellMenuUi.cs` | Main-window sync/refresh and HKLM elevation prompts. |

### Modify

| File | Changes |
|------|---------|
| `ImageConverter/Program.cs` | `Main(string[] args)`; branch shell vs GUI; mutex only on GUI branch. |
| `ImageConverter/SupportedFormats.cs` | Optional: `TryGetFormatIndexFromShellName(string, out int)` for CLI map. |
| `ImageConverter/AppSettings.cs` | `EnableExplorerConvertMenu` property, default `true`. |
| `ImageConverter/AppSettingsStore.cs` | Read/write `EnableExplorerConvertMenu`; default true when key missing. |
| `ImageConverter/frmMain.Designer.cs` | `chkEnableExplorerConvertMenu` (checked by default). |
| `ImageConverter/frmMain.cs` | Load/save checkbox; `CheckedChanged` → `ExplorerShellRegistry.Sync`; startup sync after settings load. |
| `Setup/setupScript.iss` | Post-install `--shell-register-menu`; uninstall cleanup of Explorer keys. |
| `ImageConverter/Program.cs` | `--shell-register-menu` for installer (no GUI). |
| `README.md` | Explorer **Converter To** menu + checkbox setting. |
| `ImageConverter/HelpHowToUse.rtf` | Optional: Converter To + how to disable in app. |

### Unchanged (shell path does not use)

| File | Note |
|------|------|
| `StartupApplicationContext.cs` | GUI only |
| `StartupApplicationContext.cs` | GUI only |
| `SplashForm.*` | GUI only |
| `ImageConversion.cs` | Called via existing runner |
| `BatchConversionRunner.cs` | Called as-is |
| `cleanup-after-publish.ps1` | No change |

---

## Implementation phases

### Phase 1 — Headless convert — **Done**

1. ~~Add `ShellConvertCommandLine` + `ShellConvertRunner`.~~
2. ~~Update `Program.cs` branching.~~
3. Manual test:  
   `ImageConverter.exe --shell-convert png "C:\Pictures\test.jpg"`
4. Verify: no window (or error-only MessageBox), output file created, GUI still starts with no args.

### Phase 2 — Explorer menu + checkbox — **Done**

1. ~~Implement `ExplorerShellRegistry` (register **Converter To** + format children).~~
2. ~~Add `EnableExplorerConvertMenu` + checkbox (default checked).~~
3. ~~Sync on startup and on checkbox toggle.~~
4. Test: checked → menu visible; unchecked → menu gone; restart app → state preserved.
5. Inno: `Setup/converter-to-registry-uninstall.iss` — paste `[Code]` into `setupScript.iss`.

### Phase 3 — Polish — **In progress**

1. ~~Error messages (partial batch failure).~~ — summary MessageBox on failure.
2. ~~Help/README (menu name **Converter To**, checkbox location).~~ — README done; RTF optional.
3. ~~Silent success~~ — implemented (exit 0, no dialog).
4. ~~Exe moved~~ — registry re-written on startup/toggle via `Application.ExecutablePath`.

---

## Behavior alignment with GUI

| Behavior | GUI today | Shell should |
|----------|-----------|--------------|
| Output path | `SupportedFormats.BuildOutputPath` | Same |
| ICO size / background | From UI + settings | From `config.ini` |
| Skip already target format | Yes | Yes |
| Transparent / resize | Context menu | **Out of scope** v1 |
| Undo | Yes | **No** (direct write; user uses Explorer undo/delete) |
| Progress | Status bar | None v1 (fast files OK; large batches: optional wait cursor in future) |

---

## Critical cases and decisions

| Case | Risk | Mitigation |
|------|------|------------|
| User opens app while shell convert running | None if separate processes | Shell skips GUI mutex |
| Second GUI instance | Duplicate windows | Keep mutex on GUI-only |
| Paths with spaces | Broken command | Always quote in registry: `""{app}\..."" ""%1""` |
| Very long paths | Command line limit | Rare; document; same as other tools |
| Read-only / locked file | Write fails | Report in failure summary |
| Convert to same extension | Pointless work | Skip file (GUI behavior) |
| Destination already exists | Overwrite? | Match `ImageConversion` today; document |
| Unsupported extension in selection | Menu shouldn’t show | Registry per-type; exe validates anyway |
| `svg` / `pdf` as **source** from Explorer | May be valid | Use same input rules as `ImageConversion` |
| Multi-select mixed folders | Unusual | Convert each file to its own directory |
| UAC / elevated Explorer | Different user | Same as other context menus; install scope HKCU vs HKLM |
| App moved but registry stale | Broken menu | Startup/checkbox sync rewrites `Command` with current `ExecutablePath` |
| User disables checkbox | Menu still visible until Explorer refresh | Delete keys immediately; document “new window may be needed” |
| Upgrade from build without checkbox | No keys | Default `true` → register on first launch |
| Manual registry edit | Drift | Next startup sync overwrites when enabled |

---

## Testing checklist

### Shell path

- [ ] Single `.jpg` → Converter To → PNG → `photo.png` beside source, no main window.
- [ ] Multi-select 3 files → To WEBP → 3 outputs.
- [ ] `.png` → To PNG → skip / no error storm.
- [ ] Invalid path in args → error message, exit 1.
- [ ] Unknown `--shell-convert foo` → error, exit 1.
- [ ] ICO convert uses `IcoOutputSizeIndex` from config after changing in GUI.

### GUI path

- [ ] Double-click exe → splash → main form (unchanged).
- [ ] Second GUI instance → “already running”.
- [ ] Right-click convert while GUI open → shell still works (no mutex block).

### Explorer menu + setting

- [ ] Checkbox default **checked** on fresh config.
- [ ] Uncheck → `Converter To` menu removed from `.png` / `.jpg` right-click.
- [ ] Re-check → menu returns with correct labels.
- [ ] Setting survives restart; registry matches setting after startup.
- [ ] Uninstall → `ConverterTo` keys removed even if checkbox was off.

### Installer

- [ ] Fresh install + first app launch (checkbox on) → menu appears.
- [ ] Uninstall → menu removed.
- [ ] Menu command points to current exe path after re-enable checkbox.

---

## Documentation updates

**README.md** — add subsection:

- Explorer: right-click image → **Converter To** → pick format.
- Toggle in app: **Add ‘Converter To’ to Windows Explorer right-click menu** (on by default).
- Converts in the same folder; uses ICO/background settings from the app’s settings file.
- Normal app still opened from Start menu.

**Help RTF (optional)** — same content in user-facing guide.

---

## Future enhancements (not in v1)

- Submenu **“Open Image Converter”** on same parent (opens GUI with file’s folder selected).
- Tray icon “Convert completed” for silent success.
- `HKCR\*\shell` with exe-side filtering only if Inno maintenance is too heavy.
- Queue shell jobs into running GUI instance (advanced; needs IPC).
- Per-format **“Transparent PNG”** shell entries (duplicate menu complexity).

---

## Estimated touch surface

| Category | Count |
|----------|--------|
| New C# files | 3–4 (`ShellConvert*`, `ExplorerShellRegistry`) |
| Modified C# | `Program.cs`, `AppSettings*`, `frmMain` + Designer, maybe `SupportedFormats.cs` |
| Installer | 1 (`setupScript.iss` — uninstall cleanup only) |
| Docs | 1–2 (`README.md`, optional RTF) |

**Risk level:** Low–medium — isolated entry point; core conversion unchanged; main risks are **registry sync** (enable/disable checkbox) and command-line quoting.


IMPORTANT : Please always update this file.