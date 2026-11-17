// Level_03: spell the pictured word by dragging hiragana/katakana into the slots.
// Every mistake costs a heart and the kappa creeps closer.
import { k, W, H, goto, playSound, imageSize, FONT_TYPE, FONT_KANA } from "../engine.js";
import { fader, button, iconButton, label, useCursor, resetUI, worldView, redFlash, shake, pauseMenu, endScreen, inputBlocked } from "../ui.js";
import { HIRAGANA_PALETTE, KATAKANA_PALETTE } from "../data/kana.js";
import { WORDS, WORDS_TO_WIN } from "../data/words.js";
import { KEYS, unlock } from "../save.js";
import { shuffle } from "./common.js";

const LIVES = 5;
const PAL_TOP = 400; // holder spans 18.5%..50% of the screen height from the bottom
const PAL_H = 252;
const PAL_CELL = 50;
const PAL_GAP = 10;
const SLOT = 92;
const SLOT_GAP = 16;
const SLOT_Y = 722;

export function level3Scene() {
    resetUI();
    useCursor();
    fader(true).in(0.4);
    playSound("music:despair_and_triumph", { loop: true, volume: 0.6 });

    const V = worldView(2.8431, 0, 0);
    k.add([k.sprite("kappa_bg", V.size("kappa_bg")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(0)]);
    k.add([k.sprite("izanami_small", V.size("izanami_small", 0.1)), k.pos(V.x(0.9), V.y(1.5)), k.anchor("center"), k.z(1)]);
    const picture = k.add([k.pos(V.x(-3.3), V.y(1.17)), k.z(1)]);
    let kappaScale = 0.3;
    const kappa = k.add([k.sprite("kappa", V.size("kappa", 1)), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.scale(kappaScale), k.z(2)]);
    const flash = redFlash();

    const queue = shuffle(Object.keys(WORDS));
    let found = 0;
    let lives = LIVES;
    let paused = false;
    let over = false;
    let word = null;
    let slots = [];

    // --- HUD -------------------------------------------------------------------------------
    const hearts = Array.from({ length: LIVES }, (_, i) => k.add([k.sprite("ui_heart", { width: 62, height: 62 }), k.pos(70 + i * 76, 66), k.anchor("center"), k.z(500), k.fixed()]));
    const counter = label(`0 / ${WORDS_TO_WIN}`, { x: 24, y: 720, size: 60, anchor: "left", z: 500 });
    let showingHiragana = true;
    // Hiragana_Button / Katakana_Button: only the one for the other palette is shown.
    const toKatakana = iconButton({ x: 985, y: 66, sprite: "ui_katakana", w: 210, h: 110, z: 500, onClick: togglePalette });
    const toHiragana = iconButton({ x: 985, y: 66, sprite: "ui_hiragana", w: 210, h: 110, z: 500, onClick: togglePalette });
    const pauseBtn = iconButton({
        x: 1195, y: 66, sprite: "ui_pause", w: 100, z: 500,
        onClick: () => {
            paused = true;
            pauseMenu({ onResume: () => (paused = false), onRestart: () => goto("level3"), onQuit: () => goto("menu") });
        },
    });
    const clearBtn = button({ x: 1035, y: SLOT_Y, w: 140, h: 96, label: "Clear", font: FONT_TYPE, size: 42, z: 500, onClick: () => !blocked() && clearSlots() });
    const submitBtn = button({ x: 1195, y: SLOT_Y, w: 140, h: 96, label: "Submit", font: FONT_TYPE, size: 42, z: 500, onClick: () => !blocked() && checkAnswer() });

    const blocked = () => paused || over || inputBlocked();

    // --- Character palettes ----------------------------------------------------------------
    const paletteBg = k.add([k.rect(W, PAL_H), k.pos(0, PAL_TOP), k.color(255, 255, 255), k.opacity(0.39), k.z(100), k.fixed()]);
    const makePalette = (chars) => {
        const perRow = Math.floor((W - 20) / (PAL_CELL + PAL_GAP));
        return chars.map((ch, i) => {
            const x = 16 + (i % perRow) * (PAL_CELL + PAL_GAP) + PAL_CELL / 2;
            const y = PAL_TOP + 14 + Math.floor(i / perRow) * (PAL_CELL + PAL_GAP) + PAL_CELL / 2;
            const t = k.add([k.text(ch, { size: 44, font: FONT_KANA }), k.pos(x, y), k.anchor("center"), k.area(), k.color(0, 0, 0), k.z(110), k.fixed(), "char"]);
            t.ch = ch;
            return t;
        });
    };
    const hiraganaChars = makePalette(HIRAGANA_PALETTE);
    const katakanaChars = makePalette(KATAKANA_PALETTE);
    const showPalette = () => {
        hiraganaChars.forEach((c) => (c.hidden = !showingHiragana));
        katakanaChars.forEach((c) => (c.hidden = showingHiragana));
        toKatakana.hidden = !showingHiragana;
        toHiragana.hidden = showingHiragana;
    };
    function togglePalette() {
        if (blocked()) return;
        showingHiragana = !showingHiragana;
        showPalette();
    }
    showPalette();

    // --- Drag a copy of a character into a slot ---------------------------------------------
    let ghost = null;
    k.onMousePress(() => {
        if (blocked() || ghost) return;
        const src = [...hiraganaChars, ...katakanaChars].find((c) => !c.hidden && c.isHovering());
        if (!src) return;
        ghost = k.add([k.text(src.ch, { size: 88, font: FONT_KANA }), k.pos(k.mousePos()), k.anchor("center"), k.color(0, 0, 0), k.z(900), k.fixed()]);
        ghost.ch = src.ch;
    });
    k.onMouseMove(() => {
        if (ghost) ghost.pos = k.mousePos();
    });
    k.onMouseRelease(() => {
        if (!ghost) return;
        const ch = ghost.ch;
        ghost.destroy();
        ghost = null;
        const m = k.mousePos();
        const slot = slots.find((s) => Math.abs(m.x - s.x) <= SLOT / 2 + 6 && Math.abs(m.y - SLOT_Y) <= SLOT / 2 + 6);
        if (slot) setSlot(slot, ch);
    });

    // --- Words -----------------------------------------------------------------------------
    function setSlot(slot, ch) {
        slot.ch = ch;
        slot.text.text = ch ?? "";
        slot.box.color = k.rgb(255, 255, 255);
    }

    function clearSlots() {
        for (const s of slots) setSlot(s, null);
    }

    function nextWord() {
        word = queue.pop();
        const chars = [...WORDS[word]];

        // Picture of the word (Pictures.cs), scale 0.3, kept within a sensible box.
        picture.removeAll();
        const key = imageSize(`word_${word}`) ? `word_${word}` : "word_noimage";
        const size = V.size(key, 0.3);
        const fit = Math.min(1, 340 / Math.max(size.width, size.height));
        picture.add([k.sprite(key, { width: size.width * fit, height: size.height * fit }), k.anchor("center")]);

        for (const s of slots) {
            s.box.destroy();
        }
        const total = chars.length * SLOT + (chars.length - 1) * SLOT_GAP;
        const left = 540 - total / 2;
        slots = chars.map((answer, i) => {
            const x = left + i * (SLOT + SLOT_GAP) + SLOT / 2;
            const box = k.add([k.rect(SLOT, SLOT, { radius: 6 }), k.pos(x, SLOT_Y), k.anchor("center"), k.color(255, 255, 255), k.opacity(0.39), k.z(120), k.fixed()]);
            const text = box.add([k.text("", { size: 70, font: FONT_KANA }), k.anchor("center"), k.color(0, 0, 0)]);
            return { x, box, text, answer, ch: null };
        });
    }

    function checkAnswer() {
        const wrong = slots.filter((s) => s.ch !== s.answer);
        if (wrong.length === 0) {
            found++;
            counter.text = `${found} / ${WORDS_TO_WIN}`;
            playSound("sfx:correct", { volume: 0.7 });
            if (found >= WORDS_TO_WIN || queue.length === 0) return win();
            nextWord();
            return;
        }
        shake();
        flash.flash();
        playSound("sfx:wrong", { volume: 0.7 });
        // The kappa comes closer
        kappa.pos.y += 1.15 * V.ppu;
        kappaScale += 0.16;
        kappa.scale = k.vec2(kappaScale);
        lives--;
        hearts.forEach((h, i) => (h.hidden = i >= lives));
        for (const s of wrong) s.box.color = k.rgb(255, 0, 0);
        if (lives <= 0) die();
    }

    function hideUi() {
        [...hearts, counter, toKatakana, toHiragana, pauseBtn, clearBtn, submitBtn, paletteBg, ...hiraganaChars, ...katakanaChars].forEach((o) => (o.hidden = true));
        for (const s of slots) s.box.hidden = true;
    }

    function die() {
        over = true;
        hideUi();
        endScreen({ title: "ざんねん...", onRestart: () => goto("level3"), onQuit: () => goto("menu") });
    }

    function win() {
        over = true;
        unlock(KEYS.done);
        hideUi();
        endScreen({ title: "おめでとう!", onNext: () => goto("level3Transition"), nextLabel: "つぎへ" });
    }

    nextWord();
}
