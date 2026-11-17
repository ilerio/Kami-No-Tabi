# Kami No Tabi — web version (KAPLAY)

A browser port of the Unity 2018 project in `../Unity_Folder`, built with
[KAPLAY](https://kaplayjs.com) 3001.0.19. No build step: plain ES modules plus a vendored
copy of KAPLAY in `lib/`.

## Run it

Any static file server works. One with no dependencies is included:

```bash
node web/tools/serve.mjs
```

Then open http://localhost:8123. (Python's `http.server` on Windows can serve `.js` as
`text/plain`, which browsers refuse for modules.)

To deploy, upload the `web/` folder as-is (GitHub Pages, itch.io, Netlify…).

Dev shortcuts: `?scene=level2` jumps to a scene (names are in `src/main.js`), and `?debug`
exposes the engine as `window.k` in the console.

## Layout

| Path | What |
| --- | --- |
| `src/main.js` | Registers scenes; flow follows the Unity build order |
| `src/engine.js` | KAPLAY setup (1280×800, letterboxed), asset loading, audio helpers |
| `src/ui.js` | Buttons, modal input layering, fades, pause/end screens, typewriter story box |
| `src/save.js` | `PlayerPrefs` replacement (localStorage, same key names) |
| `src/data/` | Story text, kana tables, Level 3 vocabulary |
| `src/scenes/` | One module per Unity scene group |
| `assets/` | Generated from the Unity assets — see below |
| `tools/build-assets.ps1` | Regenerates `assets/` and `src/assets.generated.js` |

## Scene mapping

| Unity scene | Web scene | Script(s) ported |
| --- | --- | --- |
| SplashScreen | `splash` (after a tap-to-start `boot` card, needed for browser audio) | SplashScreen.cs |
| Story | `story` | StoryScript.cs |
| MainMenue | `menu` | MainMenueManager.cs, LevelSelection.cs, Floating.cs |
| Level_01 | `level1` | GameController.cs, Timer.cs, PauseMenu.cs, CameraShake.cs, RedFlash.cs |
| Level_01_Transition | `level1Transition` | L01Transition.cs, BoatMover.cs |
| DnD_Hiragana / DnD_Katakana | `dndHiragana` / `dndKatakana` | Dragable.cs, DropZone*.cs, Lock*.cs, GameControllerDnD*.cs |
| DnD_Hiragana_Transition | `dndHiraganaTransition` | DnD_Hiragana_Transition.cs |
| Level_02 | `level2` | GameController_L02.cs, Lives.cs, ObjectFloating.cs |
| Level_02_Transition | `level2Transition` | Level02_Transition.cs |
| DnD_Katakana_Transition | `dndKatakanaTransition` | DnD_Katakana_Transition.cs |
| Level_03 | `level3` | GameController_L03.cs, Words.cs, Characters.cs, Pictures.cs |
| Level_03_Transition | `level3Transition` | Level3_Transition.cs |
| Story_End | `storyEnd` | EndStoryScript.cs |
| Credits | `credits` | Credits.cs, MistMover.cs |

Positions and sizes come from the Unity scenes: `worldView()` in `src/ui.js` maps each scene's
orthographic camera onto the canvas, and sprites are sized from their original pixel
dimensions at 100 px/unit, as in the project's import settings.

## Deliberate differences from the Unity build

- Level 2's floating prompt uses the white hiragana sheet; the original's black glyph was hard
  to read on the dark floor. The original also showed katakana for は/ひ/ふ/へ/ほ there
  (a mis-assigned sprite array); the port always shows the hiragana.
- Level 1's rōmaji hint shows "Fu" for ふ (the original showed its sprite name, "Hu",
  while the audio says "fu").
- Level 3 is won after 10 words, as the story and counter say (the original needed 11).
- Drag-and-drop puzzles: a kana can be dragged from the chart back to the holder.
- A tap-to-start card comes first so browsers allow audio.
- Layout of the drag-and-drop screens was tidied so all 46 slots fit without scrolling.

## Regenerating assets

```powershell
powershell -ExecutionPolicy Bypass -File web/tools/build-assets.ps1
```

Windows only (uses System.Drawing). It downsizes oversized art (some originals are 11811 px
wide, beyond WebGL texture limits), writes JPEG for opaque images and PNG otherwise, copies
audio/fonts, and converts the sliced hiragana/katakana sprite sheets into KAPLAY atlas data.
Audio is copied unchanged; the music and long WAV ambiences are about 80 MB and would shrink
a lot as compressed OGG/MP3.
