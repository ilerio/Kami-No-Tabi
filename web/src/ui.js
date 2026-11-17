import { k, W, H, FONT_JP, FONT_TYPE, imageSize, playSound } from "./engine.js";

// ---------------------------------------------------------------------------------------
// Placement helpers

/** Adds a sprite scaled to cover the whole screen. */
export function addCover(key, { zoom = 1, dx = 0, dy = 0, z = 0 } = {}) {
    const { w, h } = imageSize(key);
    const s = Math.max(W / w, H / h) * zoom;
    return k.add([k.sprite(key, { width: w * s, height: h * s }), k.pos(W / 2 + dx, H / 2 + dy), k.anchor("center"), k.z(z)]);
}

/**
 * Maps Unity world coordinates of an orthographic camera onto the 1280x800 canvas.
 * Sprites in the project are 100 px per unit, measured on the original image size.
 */
export function worldView(orthoSize, camX = 0, camY = 0) {
    const ppu = H / (2 * orthoSize);
    return {
        ppu,
        x: (wx) => W / 2 + (wx - camX) * ppu,
        y: (wy) => H / 2 - (wy - camY) * ppu,
        /** Screen size of an image at a given Unity scale. */
        size: (key, scale = 1) => {
            const { ow, oh } = imageSize(key);
            return { width: (ow / 100) * scale * ppu, height: (oh / 100) * scale * ppu };
        },
    };
}

// ---------------------------------------------------------------------------------------
// Input layering. Buttons only respond when they belong to the top-most open modal, so a
// pause menu blocks everything underneath it. Scenes can also ask `inputBlocked()`.

let modalStack = [];
export const resetUI = () => {
    modalStack = [];
};
export const inputBlocked = () => modalStack.length > 0;
const topModal = () => (modalStack.length ? modalStack[modalStack.length - 1] : null);

/** Opens a modal layer. Objects added via `m.add` are destroyed by `m.close()`. */
export function openModal() {
    const id = Symbol("modal");
    const objs = [];
    modalStack.push(id);
    return {
        id,
        add(o) {
            objs.push(o);
            return o;
        },
        close() {
            for (const o of objs) o.destroy();
            modalStack = modalStack.filter((m) => m !== id);
        },
    };
}

const ANY_LAYER = "any";
let handledAt = -1;
const live = (o) => !o.hidden && !o.disabled && (o.modalLayer === ANY_LAYER || o.modalLayer === topModal());

/** `always`: keep working while a modal is open (e.g. the story's Skip button). */
function clickable(obj, onClick, always = false) {
    obj.modalLayer = always ? ANY_LAYER : topModal();
    obj.bornAt = k.time();
    obj.disabled = false;
    obj.use("clickable");
    obj.onClick(() => {
        // Ignore the press that created this object, anything under a modal, and a second
        // object reacting to the same press (e.g. a toggle that swaps two stacked buttons).
        if (!live(obj) || k.time() - obj.bornAt < 0.05 || handledAt === k.time()) return;
        handledAt = k.time();
        onClick(obj);
    });
    return obj;
}

/** Shows a pointer cursor over anything clickable. Call once per scene. */
export function useCursor() {
    k.onUpdate(() => {
        const hot = k.get("clickable", { recursive: true }).some((o) => live(o) && o.isHovering());
        k.setCursor(hot ? "pointer" : "default");
    });
}

/** A Unity-style white UI button with black text and an optional icon on the left. */
export function button({ x, y, w, h, label = "", icon, size = 40, font = FONT_JP, z = 100, onClick, color = [255, 255, 255], always = false }) {
    const base = k.rgb(...color);
    const hover = base.lerp(k.rgb(150, 185, 210), 0.35);
    const b = k.add([
        k.rect(w, h, { radius: 10 }),
        k.pos(x, y),
        k.anchor("center"),
        k.color(base),
        k.outline(3, k.rgb(40, 40, 40)),
        k.area(),
        k.z(z),
        k.fixed(),
    ]);
    if (icon) {
        const s = h * 0.7;
        b.add([k.sprite(icon, { width: s, height: s }), k.pos(-w / 2 + h * 0.55, 0), k.anchor("center")]);
    }
    b.textObj = b.add([k.text(label, { size, font }), k.anchor("center"), k.pos(icon ? h * 0.35 : 0, 3), k.color(0, 0, 0)]);
    b.onHover(() => (b.color = hover));
    b.onHoverEnd(() => (b.color = base));
    return clickable(b, onClick, always);
}

/** A clickable image (icons such as pause, chart toggles, speaker). */
export function iconButton({ x, y, sprite, w, h, z = 100, onClick }) {
    const b = k.add([k.sprite(sprite, { width: w, height: h ?? w }), k.pos(x, y), k.anchor("center"), k.area(), k.z(z), k.fixed(), k.scale(1)]);
    b.onHover(() => (b.scale = k.vec2(1.08)));
    b.onHoverEnd(() => (b.scale = k.vec2(1)));
    return clickable(b, onClick);
}

/** Makes any game object with an `area()` clickable through the modal-aware handler. */
export const makeClickable = clickable;

export function label(str, { x, y, size = 40, font = FONT_TYPE, color = [255, 255, 255], anchor = "center", z = 100, width, align } = {}) {
    return k.add([k.text(str, { size, font, width, align }), k.pos(x, y), k.anchor(anchor), k.color(...color), k.z(z), k.fixed()]);
}

export function dim(alpha = 0.55, z = 800) {
    return k.add([k.rect(W, H), k.pos(0, 0), k.color(0, 0, 0), k.opacity(alpha), k.z(z), k.fixed(), k.area()]);
}

// ---------------------------------------------------------------------------------------
// Screen effects

/** Full-screen black fader (Fading.cs). Scenes start black and fade in. */
export function fader(startOpaque = true) {
    const f = k.add([k.rect(W, H), k.pos(0, 0), k.color(0, 0, 0), k.opacity(startOpaque ? 1 : 0), k.z(10000), k.fixed()]);
    const to = (target, dur) =>
        new Promise((resolve) => {
            k.tween(f.opacity, target, dur, (v) => (f.opacity = v)).onEnd(resolve);
        });
    return { obj: f, in: (dur) => to(0, dur), out: (dur) => to(1, dur) };
}

/** Red screen flash (RedFlash.cs). */
export function redFlash() {
    const r = k.add([k.rect(W, H), k.pos(0, 0), k.color(200, 0, 0), k.opacity(0), k.z(750), k.fixed()]);
    return {
        flash() {
            r.opacity = 0.45;
            k.wait(0.2, () => (r.opacity = 0));
        },
        on() {
            r.opacity = 0.45;
        },
    };
}

export const shake = () => k.shake(8);

// ---------------------------------------------------------------------------------------
// Menus shared by the levels

/** Pause menu (PauseMenu.cs): もどる / やりなおし / やめる. */
export function pauseMenu({ onResume, onRestart, onQuit }) {
    const m = openModal();
    m.add(dim(0.55, 800));
    m.add(label("ポーズ", { x: W / 2, y: 130, size: 150, font: FONT_JP, z: 801 }));
    const close = () => {
        m.close();
        onResume();
    };
    m.add(button({ x: W / 2, y: 340, w: 470, h: 110, label: "もどる", icon: "ui_play", size: 64, z: 801, onClick: close }));
    m.add(button({ x: W / 2, y: 490, w: 470, h: 110, label: "やりなおし", icon: "ui_refresh", size: 64, z: 801, onClick: onRestart }));
    m.add(button({ x: W / 2, y: 640, w: 470, h: 110, label: "やめる", icon: "ui_power", size: 64, z: 801, onClick: onQuit }));
    return m;
}

/** Game over / win screen (GameOver_Screen prefab). */
export function endScreen({ title, onRestart, onQuit, onNext, nextLabel = "次のレベル", titleSize = 170 }) {
    const m = openModal();
    m.add(dim(0.45, 800));
    m.add(label(title, { x: W / 2, y: onNext ? 250 : 300, size: titleSize, font: FONT_JP, z: 801, align: "center" }));
    if (onRestart) m.add(button({ x: W / 2 - 270, y: 520, w: 480, h: 100, label: "やりなおし", icon: "ui_refresh", size: 60, z: 801, onClick: onRestart }));
    if (onQuit) m.add(button({ x: W / 2 + 270, y: 520, w: 480, h: 100, label: "やめる", icon: "ui_power", size: 60, z: 801, onClick: onQuit }));
    if (onNext) m.add(button({ x: W / 2, y: 660, w: 480, h: 100, label: nextLabel, icon: "ui_next", size: 60, z: 801, onClick: onNext }));
    return m;
}

/** Full-screen kana chart shown by the chart buttons (Hiragana_Chart / Katakana_Chart prefabs). */
export function chartOverlay(sprite) {
    const { w, h } = imageSize(sprite);
    const s = (H - 40) / h;
    const bg = k.add([k.rect(W, H), k.pos(0, 0), k.color(0, 0, 0), k.opacity(0.6), k.z(700), k.fixed()]);
    const img = k.add([k.sprite(sprite, { width: w * s, height: h * s }), k.pos(W / 2, H / 2), k.anchor("center"), k.z(701), k.fixed()]);
    return {
        destroy() {
            bg.destroy();
            img.destroy();
        },
    };
}

// ---------------------------------------------------------------------------------------
// Story text box (StoryScript.cs and the *_Transition scripts)

/**
 * Typewriter text in a translucent box along the bottom of the screen. Each story line is
 * shown in hiragana, then (if enabled) romaji, then English; click/tap to finish the current
 * line or advance. `onLine(i)` fires when line i starts, `onDone()` after the last line.
 */
export function storyBox({ story, romaji, english, onLine = () => {}, onDone = () => {}, speed = 0.08, autoStart = true }) {
    const boxH = H * 0.225;
    k.add([k.rect(W, boxH), k.pos(0, H - boxH), k.color(0, 0, 0), k.opacity(0.59), k.z(600), k.fixed()]);
    const out = k.add([k.text("", { size: 38, font: FONT_TYPE, width: W - 48, lineSpacing: 6 }), k.pos(24, H - boxH + 20), k.color(255, 255, 255), k.z(601), k.fixed()]);

    const segments = [];
    story.h.forEach((line, i) => {
        segments.push({ text: line, line: i });
        if (romaji && story.r) segments.push({ text: story.r[i], line: i, sub: true });
        if (english && story.e) segments.push({ text: story.e[i], line: i, sub: true });
    });

    const typing = playSound("sfx:typewriter", { loop: true, volume: 0.9, speed: 0.8, paused: true });
    let seg = -1;
    let shown = 0;
    let elapsed = 0;
    let finished = false;
    let startedAt = 0;

    const current = () => segments[seg];
    const scrolling = () => seg >= 0 && shown < current().text.length;

    function next() {
        seg++;
        if (seg >= segments.length) {
            finished = true;
            out.text = "";
            typing.paused = true;
            onDone();
            return;
        }
        if (!current().sub) onLine(current().line);
        shown = 0;
        elapsed = 0;
        out.text = "";
    }

    const ctrl = {
        start() {
            startedAt = k.time();
            next();
        },
        advance() {
            if (finished || seg < 0 || k.time() - startedAt < 0.1) return;
            if (scrolling()) {
                shown = current().text.length;
                out.text = current().text;
            } else next();
        },
        get done() {
            return finished;
        },
    };

    k.onUpdate(() => {
        if (seg < 0 || finished) return;
        if (scrolling()) {
            elapsed += k.dt();
            const n = Math.min(current().text.length, Math.floor(elapsed / speed) + 1);
            if (n !== shown) {
                shown = n;
                out.text = current().text.slice(0, n);
            }
        }
        typing.paused = !scrolling();
    });

    if (autoStart) ctrl.start();
    return ctrl;
}
