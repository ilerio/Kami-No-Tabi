// Replacement for Unity's PlayerPrefs (stored in localStorage, same key names as the original).
const PREFIX = "kami-no-tabi:";

export const KEYS = {
    level1: "Level_01",
    dndHiragana: "DnD_Hiragana",
    level2: "Level_02",
    dndKatakana: "DnD_Katakana",
    level3: "Level_03",
    done: "Done",
    skipEnabled: "skipEnabled",
    englishSubtitles: "englishSubtitles",
    romajiSubtitles: "romanjiSubtitles",
};

export function getPref(key, fallback = 0) {
    try {
        const v = localStorage.getItem(PREFIX + key);
        return v === null ? fallback : Number(v);
    } catch {
        return fallback;
    }
}

export function setPref(key, value) {
    try {
        localStorage.setItem(PREFIX + key, String(value));
    } catch {
        // Storage can be unavailable (private mode); progress just isn't remembered.
    }
}

export const unlocked = (key) => getPref(key) === 1;
export const unlock = (key) => setPref(key, 1);
