// Level_02: a hiragana floats in front of three graves; tap the grave with the same katakana.
import { k, W, H, goto, playSound } from "../engine.js";
import { fader, iconButton, label, useCursor, resetUI, worldView, redFlash, shake, pauseMenu, endScreen, chartOverlay, inputBlocked, makeClickable } from "../ui.js";
import { KANA, sheetSprite } from "../data/kana.js";
import { KEYS, unlock } from "../save.js";
import { shuffle, pickDistractors } from "./common.js";

const MAX_LIVES = 4;
const STREAK_FOR_LIFE = 15;
const GRAVES = [
    [-9.9, 0],
    [0, 2],
    [9.9, 0],
];

export function level2Scene() {
    resetUI();
    useCursor();
    fader(true).in(0.4);
    playSound("music:wet_riffs", { loop: true, volume: 0.3 });

    const V = worldView(7.6842, 0, 0);
    k.add([k.sprite("katakana_done", V.size("katakana_done")), k.pos(V.x(0), V.y(-3)), k.anchor("center"), k.z(0)]);
    const flash = redFlash();

    // The hiragana to match drifts gently (ObjectFloating.cs)
    const promptSize = 4 * 1.06 * V.ppu;
    // Uses the white sheet: the original's black glyph is hard to see on the dark floor.
    const prompt = k.add([k.pos(V.x(0), V.y(-4.8)), k.z(5)]);
    let glyph = null;
    const promptBase = prompt.pos.clone();
    const seed = Math.random() * 10;
    prompt.onUpdate(() => {
        const t = k.time() + seed;
        prompt.pos = promptBase.add(Math.sin(t * 1.6) * 8, Math.sin(t * 2.1) * 6);
    });

    const order = shuffle(KANA.map((_, i) => i));
    let answer = null;
    let correctSlot = 0;
    let found = 0;
    let streak = 0;
    let lives = MAX_LIVES;
    let paused = false;
    let chart = null;
    let over = false;

    // --- HUD -------------------------------------------------------------------------------
    const hearts = Array.from({ length: MAX_LIVES }, (_, i) => k.add([k.sprite("ui_heart", { width: 100, height: 100 }), k.pos(120 + i * 110, 100), k.anchor("center"), k.z(500), k.fixed()]));
    const countText = label("0 / 46", { x: 54, y: 700, size: 80, anchor: "left", z: 500 });
    const chartBtn = iconButton({ x: 975, y: 100, sprite: "ui_katakana", w: 230, h: 120, z: 500, onClick: toggleChart });
    const pauseBtn = iconButton({ x: 1190, y: 100, sprite: "ui_pause", w: 110, z: 500, onClick: pause });

    const showLives = () => hearts.forEach((h, i) => (h.hidden = i >= lives));

    function toggleChart() {
        if (chart) {
            chart.destroy();
            chart = null;
        } else chart = chartOverlay("katakana_chart");
    }

    function pause() {
        if (chart) toggleChart();
        paused = true;
        pauseMenu({ onResume: () => (paused = false), onRestart: () => goto("level2"), onQuit: () => goto("menu") });
    }

    const blocked = () => paused || over || !!chart || inputBlocked();

    function newRound() {
        k.get("grave").forEach((o) => o.destroy());
        answer = KANA[order.pop()];
        correctSlot = k.randi(0, 3);
        if (glyph) glyph.destroy();
        glyph = prompt.add([k.sprite(sheetSprite("hiragana_white", answer), { height: promptSize }), k.anchor("center")]);
        pickDistractors(answer, 3, correctSlot).forEach((kana, slot) => addGrave(slot, kana));
    }

    function addGrave(slot, kana) {
        const [wx, wy] = GRAVES[slot];
        const size = V.size("grave", 0.7);
        const g = k.add([k.sprite("grave", size), k.pos(V.x(wx), V.y(wy)), k.anchor("center"), k.z(2), "grave"]);
        // katakana child at local (0, -1.8), scale 3 → on the lantern's light panel
        g.add([k.sprite(sheetSprite("katakana", kana), { height: 1.06 * 2.1 * V.ppu }), k.pos(0, 1.8 * 0.7 * V.ppu), k.anchor("center")]);
        const hit = g.add([k.rect(size.width * 0.75, size.height * 0.85), k.pos(0, size.height * 0.05), k.anchor("center"), k.opacity(0), k.area()]);
        makeClickable(hit, () => {
            if (!blocked()) choose(slot);
        });
    }

    function choose(slot) {
        if (slot === correctSlot) {
            found++;
            countText.text = `${found} / 46`;
            if (found === KANA.length) return win();
            streak++;
            if (streak % STREAK_FOR_LIFE === 0 && lives < MAX_LIVES) lives++;
            showLives();
            playSound("sfx:correct", { volume: 0.7 });
            newRound();
        } else {
            streak = 0;
            playSound("sfx:wrong", { volume: 0.7 });
            shake();
            flash.flash();
            lives--;
            showLives();
            if (lives <= 0) gameOver();
        }
    }

    function hideHud() {
        [countText, chartBtn, pauseBtn, prompt, ...hearts].forEach((o) => (o.hidden = true));
        if (chart) toggleChart();
        k.get("grave").forEach((o) => o.destroy());
    }

    function gameOver() {
        over = true;
        hideHud();
        shake();
        flash.on();
        endScreen({ title: "ざんねん...", onRestart: () => goto("level2"), onQuit: () => goto("menu") });
    }

    function win() {
        over = true;
        unlock(KEYS.dndKatakana);
        hideHud();
        endScreen({ title: "おめでとう!", onRestart: () => goto("level2"), onQuit: () => goto("menu"), onNext: () => goto("level2Transition") });
    }

    newRound();
}
