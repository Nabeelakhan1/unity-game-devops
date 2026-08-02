# Candy Clicker — setup

## Scripts

```
Assets/CandyClicker/Scripts/
  Core/        IClickable, ClickInput, ScoreSystem, AudioService
  Gameplay/    MainCandy, BonusCandy, BonusCandySpawner
  UI/          HUDView, FloatingText, FloatingTextSpawner
  Utilities/   ObjectPool
```

Requires **TextMeshPro** (Window → TextMeshPro → Import TMP Essential Resources).

## Layers

Create one layer, `Clickable`. Put `MainCandy` and the `BonusCandy` prefab on it, and set
`ClickInput._clickableMask` to just that layer. That keeps `OverlapPoint` cheap and stops
background sprites from eating taps.

## Scene

**Camera** — Orthographic, size ~5, solid colour background.

**GameRoot** (empty GameObject)
- `ClickInput` — camera + Clickable mask
- `ScoreSystem` — points per level (25 is a fine start)
- `AudioService` — needs an AudioSource on the same object
- `FloatingTextSpawner` — FloatingText prefab, pool size 16
- `BonusCandySpawner` — BonusCandy prefab, camera, ScoreSystem, AudioService, text spawner, clip, particles

**MainCandy** (sprite at origin)
- SpriteRenderer + CircleCollider2D (no Rigidbody needed — `OverlapPoint` doesn't require one)
- `MainCandy` script, wire ScoreSystem / AudioService / FloatingTextSpawner / particles / clip

**ClickParticles** (ParticleSystem, one per use site)
- Play On Awake **off**, Looping **off**, short burst (~15 particles), Stop Action = None

**FloatingText prefab**
- Empty GameObject + `TextMeshPro` (the 3D one, not UGUI) + `FloatingText`
- Set alignment centre, sorting layer above the candy

**BonusCandy prefab**
- SpriteRenderer + CircleCollider2D + `BonusCandy`, on the Clickable layer

**Canvas** (Screen Space – Overlay, CanvasScaler = Scale With Screen Size, 1080×1920 or 1920×1080)
- `ProgressBar` Image → Image Type **Filled**, Fill Method Horizontal, Fill Origin Left
- `LevelLabel`, `ScoreLabel` (TextMeshProUGUI)
- `HUDView` on the Canvas, wired to ScoreSystem + those three

An EventSystem must exist in the scene, otherwise `ClickInput`'s UI blocking check is a no-op
(harmless, but UI buttons will click through to the candy).

## WebGL build checklist

Player Settings → WebGL:
- **Color Space: Gamma** — Linear needs WebGL2 and breaks on older mobile browsers
- **Compression Format: Brotli**, and turn **Decompression Fallback ON** if your host can't set
  `Content-Encoding` headers (itch.io, GitHub Pages). Gzip + fallback is the safest combo for CI artifacts.
- **Enable Exceptions: None** for release builds (smaller, faster); Explicit Only while debugging
- **Data Caching ON** so repeat loads come from IndexedDB
- **Strip Engine Code ON**, Managed Stripping Level: Low → Medium
- **Run In Background ON** if you don't want the game to freeze on tab blur
- Publishing Settings → **Auto Graphics API** off, WebGL2 only, if you don't care about WebGL1

Quality settings: drop to one level, disable shadows and anti-aliasing. Nothing here needs them.

Audio: browsers block audio until the first user gesture. Since the whole game *is* clicking,
this resolves itself on the first tap — but don't add a startup jingle before any input.

Input: uses the legacy `Input` class. If your project uses the new Input System package, set
Active Input Handling to **Both**, or swap the two lines in `ClickInput.Update`.

## CI notes

- Build target `WebGL`, method e.g. `CandyClicker.Editor.BuildScript.BuildWebGL` (add later).
- The output folder (`Build/`, `index.html`, `TemplateData/`) is the whole artifact — no server needed
  for a smoke test, but `file://` won't run it. Serve it over HTTP in the job if you want to verify.
- Keep the licence activation step before the build step; WebGL builds are slow (5–15 min cold),
  so cache `Library/` keyed on the Unity version + packages lock file.
