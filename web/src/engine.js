import kaplay from "../lib/kaplay.mjs";
import ASSETS from "./assets.generated.js";

// The Unity UI was authored against a 640x400 reference resolution; the port uses 2x that.
export const W = 1280;
export const H = 800;

export const k = kaplay({
    width: W,
    height: H,
    letterbox: true,
    stretch: true,
    background: [0, 0, 0],
    font: "typewriter",
    global: false,
    debug: false,
    backgroundAudio: false,
    texFilter: "linear",
});

// `?debug` exposes the engine in the console for inspection.
if (new URLSearchParams(location.search).has("debug")) window.k = k;

export const FONT_JP = "japestyle"; // "Japestyle" headings/buttons (CJK falls back to the system font)
export const FONT_TYPE = "typewriter"; // "Typewriter Condensed" body text
// System Japanese fonts for kana drawn as text (Level 3 tiles), like Unity's OS font fallback.
export const FONT_KANA = "'Hiragino Kaku Gothic ProN', 'Yu Gothic', 'Meiryo', 'Noto Sans JP', sans-serif";

export function loadAll() {
    for (const [key, v] of Object.entries(ASSETS.images)) k.loadSprite(key, v.url);
    for (const [key, v] of Object.entries(ASSETS.sheets)) {
        const atlas = {};
        for (const [name, rect] of Object.entries(v.atlas)) atlas[`${key}:${name}`] = rect;
        k.loadSpriteAtlas(v.url, atlas);
    }
    for (const [key, url] of Object.entries(ASSETS.sfx)) k.loadSound(`sfx:${key}`, url);
    for (const [key, url] of Object.entries(ASSETS.voice)) k.loadSound(`voice:${key}`, url);
    for (const [key, url] of Object.entries(ASSETS.music)) k.loadMusic(`music:${key}`, url);
    k.loadFont(FONT_JP, ASSETS.fonts.japestyle);
    k.loadFont(FONT_TYPE, ASSETS.fonts.typewriter);
}

export const imageSize = (key) => ASSETS.images[key];

// ---------------------------------------------------------------------------------------
// Audio: scenes register what they start so it is stopped when the scene changes.

let sceneSounds = [];

export function playSound(name, opt = {}) {
    const s = k.play(name, opt);
    sceneSounds.push(s);
    return s;
}

export function stopSceneSounds() {
    for (const s of sceneSounds) s.stop();
    sceneSounds = [];
}

// Fades a playing sound out over `dur` seconds.
export function fadeSound(snd, dur) {
    const from = snd.volume;
    return new Promise((resolve) => {
        k.tween(from, 0, dur, (v) => (snd.volume = v)).onEnd(() => {
            snd.stop();
            resolve();
        });
    });
}

/** Switch scene, stopping all audio the current scene started. */
export function goto(scene, ...args) {
    stopSceneSounds();
    k.go(scene, ...args);
}

export const sleep = (sec) => new Promise((resolve) => k.wait(sec, resolve));
