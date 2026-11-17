import { k, loadAll } from "./engine.js";
import { bootScene, splashScene, menuScene } from "./scenes/menu.js";
import {
    introScene,
    level1TransitionScene,
    hiraganaDoorTransitionScene,
    level2TransitionScene,
    katakanaGraveTransitionScene,
    level3TransitionScene,
    storyEndScene,
    creditsScene,
} from "./scenes/story.js";
import { level1Scene } from "./scenes/level1.js";
import { level2Scene } from "./scenes/level2.js";
import { level3Scene } from "./scenes/level3.js";
import { dndHiraganaScene, dndKatakanaScene } from "./scenes/dnd.js";

loadAll();

// Scene flow mirrors the Unity build order:
// Splash → Story → Menu → Level 1 → (story) → Hiragana door → (story) → Level 2 → (story)
// → Katakana gravestone → (story) → Level 3 → kappa bows → Ending → Credits → Menu
const scenes = {
    boot: bootScene,
    splash: splashScene,
    story: introScene,
    menu: menuScene,
    level1: level1Scene,
    level1Transition: level1TransitionScene,
    dndHiragana: dndHiraganaScene,
    dndHiraganaTransition: hiraganaDoorTransitionScene,
    level2: level2Scene,
    level2Transition: level2TransitionScene,
    dndKatakana: dndKatakanaScene,
    dndKatakanaTransition: katakanaGraveTransitionScene,
    level3: level3Scene,
    level3Transition: level3TransitionScene,
    storyEnd: storyEndScene,
    credits: creditsScene,
};
for (const [name, fn] of Object.entries(scenes)) k.scene(name, fn);

// `?scene=level2` jumps straight to a scene (handy while developing).
const requested = new URLSearchParams(location.search).get("scene");
k.onLoad(() => k.go(requested && scenes[requested] ? requested : "boot"));
