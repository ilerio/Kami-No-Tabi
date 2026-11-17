// DnD_Hiragana / DnD_Katakana: drag the shuffled kana into chart order, then tap the lock
// (hiragana door) or the flower (katakana gravestone) to check the answer.
import { k, W, H, goto, playSound, sleep } from "../engine.js";
import { fader, iconButton, useCursor, resetUI, worldView, pauseMenu, inputBlocked, makeClickable } from "../ui.js";
import { KANA, CHART_CELLS, sheetSprite } from "../data/kana.js";
import { KEYS, unlock } from "../save.js";
import { shuffle } from "./common.js";

const CELL = 60;
const GAP = 8;
const HOLDER_W = 402; // left 31.4% of the screen, as in the original
const HOLDER_COLS = 5;
const GRID_X = 470;
const GRID_Y = 34;
const WHITE = k.rgb(255, 255, 255);
const RED = k.rgb(255, 40, 40);

const CONFIG = {
    hiragana: {
        scene: "dndHiragana",
        sheet: "hiragana_white",
        music: "music:march_of_the_mind",
        winSound: "sfx:door_opening",
        winDelay: 8,
        unlockKey: KEYS.level2,
        next: "dndHiraganaTransition",
    },
    katakana: {
        scene: "dndKatakana",
        sheet: "katakana_white",
        music: "music:measured_paces",
        winSound: "sfx:cinderblock",
        winDelay: 1.7,
        unlockKey: KEYS.level3,
        next: "dndKatakanaTransition",
    },
};

export const dndHiraganaScene = () => dndScene(CONFIG.hiragana);
export const dndKatakanaScene = () => dndScene(CONFIG.katakana);

function dndScene(cfg) {
    resetUI();
    useCursor();
    fader(true).in(0.4);
    playSound(cfg.music, { loop: true, volume: 0.5 });

    // Background: the gate, framed like the original camera (ortho 18.19 at (4, 18)).
    const V = worldView(18.1891, 4, 18);
    k.add([k.sprite("gate", V.size("gate")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(0)]);

    // Panels: item holder on the left, chart area on the right.
    k.add([k.rect(HOLDER_W, H), k.pos(0, 0), k.color(255, 255, 255), k.opacity(0.39), k.z(10)]);
    k.add([k.rect(W - HOLDER_W, H), k.pos(HOLDER_W, 0), k.color(30, 24, 0), k.opacity(0.39), k.z(10)]);

    let paused = false;
    let won = false;
    const blocked = () => paused || won || inputBlocked();

    // --- The "lock" ------------------------------------------------------------------------
    let lock;
    let idleColor;
    if (cfg === CONFIG.hiragana) {
        idleColor = k.rgb(255, 234, 0);
        lock = k.add([k.sprite("lock", { height: 300 }), k.pos(1050, 420), k.anchor("center"), k.color(idleColor), k.area(), k.z(20)]);
    } else {
        idleColor = k.rgb(255, 255, 255);
        k.add([k.sprite("grave_stone", { width: 470 }), k.pos(1048, 380), k.anchor("center"), k.z(12)]);
        lock = k.add([k.sprite("flower", { width: 230 }), k.pos(1048, 640), k.anchor("center"), k.color(idleColor), k.area(), k.z(20)]);
    }
    let lockHover = false;
    lock.onHover(() => {
        lockHover = true;
        lock.color = k.rgb(0, 255, 0);
    });
    lock.onHoverEnd(() => {
        lockHover = false;
        lock.color = idleColor;
    });
    makeClickable(lock, () => {
        if (!blocked()) checkAnswer();
    });
    lock.onUpdate(() => {
        if (!lockHover && allFilled()) {
            // Lock.cs Glow(): pulse while the chart is complete
            const t = 0.5 + 0.5 * Math.sin(k.time() * 3);
            lock.color = cfg === CONFIG.hiragana ? k.rgb(255 * (0.4 + 0.6 * t), 234 * (0.4 + 0.6 * t), 0) : k.rgb(255, 255, 255 * t);
        } else if (!lockHover) lock.color = idleColor;
    });

    const pauseBtn = iconButton({
        x: W - 60, y: 60, sprite: "ui_pause", w: 84, z: 500,
        onClick: () => {
            paused = true;
            pauseMenu({ onResume: () => (paused = false), onRestart: () => goto(cfg.scene), onQuit: () => goto("menu") });
        },
    });

    // --- Slots -----------------------------------------------------------------------------
    const slots = CHART_CELLS.map(([row, col], i) => {
        const x = GRID_X + col * (CELL + GAP) + CELL / 2;
        const y = GRID_Y + row * (CELL + GAP) + CELL / 2;
        const box = k.add([k.rect(CELL, CELL, { radius: 4 }), k.pos(x, y), k.anchor("center"), k.color(WHITE), k.opacity(0.39), k.z(15)]);
        return { index: i, x, y, box, item: null };
    });
    const slotAt = (p) => slots.find((s) => Math.abs(p.x - s.x) <= CELL / 2 + GAP / 2 && Math.abs(p.y - s.y) <= CELL / 2 + GAP / 2) ?? null;
    const allFilled = () => slots.every((s) => s.item);

    // --- Items -----------------------------------------------------------------------------
    const holderCell = (i) => ({
        x: 32 + (i % HOLDER_COLS) * (CELL + 14) + CELL / 2,
        y: 40 + Math.floor(i / HOLDER_COLS) * (CELL + 14) + CELL / 2,
    });
    const items = shuffle(KANA).map((kana, i) => {
        const home = holderCell(i);
        const it = k.add([k.sprite(sheetSprite(cfg.sheet, kana), { height: CELL - 8 }), k.pos(home.x, home.y), k.anchor("center"), k.area(), k.scale(1), k.z(30), "item"]);
        Object.assign(it, { kana, cell: i, slot: null });
        return it;
    });

    const moveTo = (it, x, y) => k.tween(it.pos.clone(), k.vec2(x, y), 0.12, (p) => (it.pos = p), k.easings.easeOutQuad);

    function putInSlot(it, slot) {
        if (it.slot && it.slot !== slot && it.slot.item === it) it.slot.item = null;
        slot.item = it;
        it.slot = slot;
        slot.box.color = WHITE;
        moveTo(it, slot.x, slot.y);
    }
    function putInHolder(it) {
        // Only clear the slot if it still holds this item (after a swap it may already hold another).
        if (it.slot && it.slot.item === it) it.slot.item = null;
        it.slot = null;
        const home = holderCell(it.cell);
        moveTo(it, home.x, home.y);
    }

    // --- Dragging --------------------------------------------------------------------------
    let drag = null;
    k.onMousePress(() => {
        if (blocked() || drag) return;
        const under = items.filter((it) => it.isHovering());
        if (!under.length) return;
        drag = under[under.length - 1];
        drag.z = 40;
        if (!drag.slot) drag.scale = k.vec2(1.6); // magnify small holder items (Dragable.OnPointerDown)
    });
    k.onMouseMove(() => {
        if (drag) drag.pos = k.mousePos();
    });
    k.onMouseRelease(() => {
        if (!drag) return;
        const it = drag;
        drag = null;
        it.z = 30;
        it.scale = k.vec2(1);
        const m = k.mousePos();
        const target = slotAt(m);
        const from = it.slot;
        if (target && target !== from) {
            const other = target.item;
            if (from) from.item = null;
            putInSlot(it, target);
            if (other) {
                if (from) putInSlot(other, from);
                else putInHolder(other);
            }
        } else if (!target && from && m.x < HOLDER_W) {
            putInHolder(it);
        } else if (from) {
            moveTo(it, from.x, from.y);
        } else {
            putInHolder(it);
        }
    });

    // --- Checking --------------------------------------------------------------------------
    async function checkAnswer() {
        const wrong = slots.filter((s) => !s.item || s.item.kana !== KANA[s.index]);
        if (wrong.length === 0) {
            won = true;
            lock.hidden = true;
            pauseBtn.hidden = true;
            playSound(cfg.winSound, { volume: 0.7 });
            unlock(cfg.unlockKey);
            await sleep(cfg.winDelay);
            goto(cfg.next);
            return;
        }
        for (const s of wrong) {
            s.box.color = RED;
            if (s.item) putInHolder(s.item);
        }
    }
}
