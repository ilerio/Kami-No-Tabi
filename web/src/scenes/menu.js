import { k, W, H, goto, playSound, sleep, FONT_JP, FONT_TYPE } from "../engine.js";
import { fader, button, label, openModal, dim, useCursor, resetUI, worldView, iconButton } from "../ui.js";
import { KEYS, unlocked, unlock } from "../save.js";

/** Browsers only allow audio after a user gesture, so the game opens on a tap-to-start card. */
export function bootScene() {
    resetUI();
    label("かみ の たび", { x: W / 2, y: 300, size: 110, font: FONT_JP });
    label("Kami No Tabi — Journey of the Gods", { x: W / 2, y: 410, size: 46 });
    const hint = label("Click or tap to begin", { x: W / 2, y: 560, size: 40, color: [200, 200, 200] });
    hint.onUpdate(() => (hint.opacity = 0.55 + 0.45 * Math.sin(k.time() * 3)));
    k.onMousePress(() => goto("splash"));
    k.onKeyPress(() => goto("splash"));
}

/** SplashScreen.unity: the UBC logo, then on to the story. */
export function splashScene() {
    resetUI();
    const fade = fader(true);
    playSound("sfx:welcome", { volume: 0.5 });
    const V = worldView(3.1517, 0, 0);
    k.add([k.sprite("ubc_logo", V.size("ubc_logo", 1.5)), k.pos(V.x(0.3), V.y(0)), k.anchor("center")]);
    (async () => {
        await fade.in(1.25);
        await sleep(2);
        await fade.out(0.6);
        goto("story");
    })();
}

const LEVELS = [
    { key: KEYS.level1, scene: "level1", label: "レベル 1", sub: "Hiragana Oni" },
    { key: KEYS.dndHiragana, scene: "dndHiragana", label: "ひらがな の ドア", sub: "Hiragana Door" },
    { key: KEYS.level2, scene: "level2", label: "レベル 2", sub: "Katakana Graves" },
    { key: KEYS.dndKatakana, scene: "dndKatakana", label: "カタカナ の おはか", sub: "Katakana Gravestone" },
    { key: KEYS.level3, scene: "level3", label: "レベル 3", sub: "Kappa Word Game" },
];

/** Same priority order as MainMenueManager.cs: continue from the furthest unlocked level. */
function continueScene() {
    if (unlocked(KEYS.done)) return "level1";
    for (const l of [...LEVELS].reverse()) if (unlocked(l.key)) return l.scene;
    return "level1";
}

/** MainMenue.unity */
export function menuScene() {
    resetUI();
    useCursor();
    const fade = fader(true);
    fade.in(1.25);
    playSound("music:lullaby", { loop: true, volume: 0.3 });

    const V = worldView(16.5907, 0, 4);
    k.add([k.sprite("water", V.size("water")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(0)]);

    // boat_Izanagi bobs up and down (Floating.cs)
    const group = k.add([k.pos(V.x(4), V.y(-0.3)), k.z(1)]);
    group.add([k.sprite("boat_reflection", V.size("boat_reflection")), k.pos(-9.8 * V.ppu, -0.1 * V.ppu), k.anchor("center"), k.opacity(0.9)]);
    group.add([k.sprite("izanagi", V.size("izanagi", 1.3)), k.pos(-11.5 * V.ppu, -7.8 * V.ppu), k.anchor("center")]);
    group.add([k.sprite("boat", V.size("boat")), k.pos(-12.6 * V.ppu, -12.4 * V.ppu), k.anchor("center")]);
    const baseY = group.pos.y;
    const phase = Math.random() * Math.PI * 2;
    group.onUpdate(() => (group.pos.y = baseY + Math.sin(k.time() * 1.3 + phase) * 6));

    label("かみ の たび", { x: 60, y: 690, size: 84, font: FONT_JP, anchor: "left" });
    label("Kami No Tabi", { x: 64, y: 755, size: 40, anchor: "left", color: [220, 230, 240] });

    let busy = false;
    button({
        x: 1080, y: 90, w: 300, h: 100, label: "スタート", icon: "ui_play", size: 46,
        onClick: async () => {
            if (busy) return;
            busy = true;
            unlock(KEYS.level1);
            await fade.out(0.5);
            goto(continueScene());
        },
    });

    const available = LEVELS.filter((l) => unlocked(l.key));
    if (available.length > 0) {
        button({ x: 1080, y: 240, w: 300, h: 100, label: "レベル", icon: "ui_gear", size: 46, onClick: () => levelSelect(available) });
    }

    button({ x: 80, y: 60, w: 50, h: 50, label: "c", size: 40, color: [180, 200, 215], onClick: () => goto("credits") });
}

function levelSelect(levels) {
    const m = openModal();
    m.add(dim(0.6, 800));
    m.add(label("レベル", { x: W / 2, y: 90, size: 96, font: FONT_JP, z: 801 }));
    levels.forEach((l, i) => {
        const b = m.add(button({ x: W / 2, y: 210 + i * 105, w: 620, h: 88, label: l.label, size: 46, z: 801, onClick: () => goto(l.scene) }));
        b.add([k.text(l.sub, { size: 24, font: FONT_TYPE }), k.pos(300, 0), k.anchor("right"), k.color(90, 90, 90)]);
        b.textObj.pos.x = -60;
    });
    m.add(button({ x: W - 70, y: 60, w: 64, h: 64, label: "X", font: FONT_TYPE, size: 44, z: 801, onClick: () => m.close() }));
}
