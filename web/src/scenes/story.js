// Story scenes: intro (Story.unity), the between-level transitions, the ending and the
// Level 3 kappa-bow cutscene.
import { k, W, H, goto, playSound, sleep, fadeSound } from "../engine.js";
import { addCover, fader, storyBox, button, openModal, label, dim, useCursor, resetUI, worldView } from "../ui.js";
import { STORY } from "../data/story.js";
import { getPref, setPref, KEYS, unlocked } from "../save.js";
import { FONT_TYPE } from "../engine.js";

const subtitles = () => ({
    romaji: getPref(KEYS.romajiSubtitles) === 1,
    english: getPref(KEYS.englishSubtitles) === 1,
});

function slides(keys) {
    let index = 0;
    let obj = addCover(keys[0], { z: 0 });
    return {
        show(n) {
            index = Math.max(0, Math.min(n, keys.length - 1));
            obj.destroy();
            obj = addCover(keys[index], { z: 0 });
        },
        next() {
            this.show(index + 1);
        },
    };
}

function checkbox({ x, y, text, checked }) {
    let on = checked;
    const box = k.add([k.rect(44, 44, { radius: 4 }), k.pos(x, y), k.anchor("center"), k.color(255, 255, 255), k.area(), k.z(801), k.fixed()]);
    const tick = box.add([k.text("✓", { size: 40, font: "sans-serif" }), k.anchor("center"), k.color(20, 20, 20)]);
    tick.hidden = !on;
    const txt = label(text, { x: x + 44, y, size: 52, anchor: "left", z: 801 });
    txt.use(k.area());
    const toggle = () => {
        on = !on;
        tick.hidden = !on;
    };
    box.onClick(toggle);
    txt.onClick(toggle);
    box.use("clickable");
    txt.use("clickable");
    box.modalLayer = txt.modalLayer = "any";
    return { objs: [box, txt], get on() { return on; } };
}

/** Story.unity: subtitle options, then the opening story over five slides. */
export function introScene() {
    resetUI();
    useCursor();
    const fade = fader(true);
    fade.in(2);

    const show = slides(["bridge", "izanami_baby", "izanami_fire", "izanagi_grave", "izanagi_boat"]);

    const canSkip = unlocked(KEYS.skipEnabled);
    const skip = button({ x: W - 70, y: 50, w: 120, h: 56, label: "Skip", font: FONT_TYPE, size: 36, z: 900, always: true, onClick: () => goto("menu") });
    if (!canSkip) {
        skip.disabled = true;
        skip.opacity = 0.35;
        skip.textObj.opacity = 0.35;
    }

    // Subtitle options panel
    const prefs = subtitles();
    const m = openModal();
    m.add(dim(0.67, 800));
    const ro = checkbox({ x: 470, y: 250, text: "Ro-maji Subtitles", checked: prefs.romaji });
    const en = checkbox({ x: 470, y: 340, text: "English Subtitles", checked: prefs.english });
    for (const o of [...ro.objs, ...en.objs]) m.add(o);

    let box = null;
    m.add(
        button({
            x: W / 2,
            y: 520,
            w: 320,
            h: 120,
            label: "OK",
            font: FONT_TYPE,
            size: 80,
            z: 801,
            onClick: () => {
                setPref(KEYS.romajiSubtitles, ro.on ? 1 : 0);
                setPref(KEYS.englishSubtitles, en.on ? 1 : 0);
                m.close();
                box = storyBox({
                    story: STORY.intro,
                    romaji: ro.on,
                    english: en.on,
                    onLine: (i) => {
                        if (i >= 3 && i < 7) show.show(i - 2);
                    },
                    onDone: async () => {
                        setPref(KEYS.skipEnabled, 1);
                        await fade.out(0.5);
                        goto("menu");
                    },
                });
            },
        }),
    );

    k.onMousePress(() => box && box.advance());
}

/**
 * Shared shape of the between-level story scenes: background slides, typewriter text that
 * starts automatically, and a hand-off when the text ends.
 */
function transitionScene({ story, slideKeys, slideOnLine, allowRomaji = true, music, setup, onDone }) {
    resetUI();
    useCursor();
    const fade = fader(true);
    fade.in(0.4);
    if (music) playSound(music.name, { loop: true, volume: music.volume });

    const show = slideKeys ? slides(slideKeys) : null;
    const extra = setup ? setup() : {};
    const subs = subtitles();
    let box = null;
    k.wait(0.1, () => {
        box = storyBox({
            story,
            romaji: allowRomaji && subs.romaji,
            english: subs.english,
            onLine: (i) => show && slideOnLine && slideOnLine(i, show),
            onDone: () => onDone({ fade, ...extra }),
        });
    });
    k.onMousePress(() => box && box.advance());
}

/** Level_01_Transition: Izanagi rows across the water, then reaches the far shore. */
export function level1TransitionScene() {
    transitionScene({
        story: STORY.level1Done,
        setup() {
            const V = worldView(17.4938, 0, 0);
            k.add([k.sprite("water", V.size("water")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(0)]);
            // Boat_Izanagi moves right at 2 units/s (BoatMover.cs)
            const boat = k.add([k.pos(V.x(-23), V.y(0)), k.z(1)]);
            boat.add([k.sprite("izanagi", V.size("izanagi", 0.7)), k.pos(-20 * V.ppu, -2 * V.ppu), k.anchor("center")]);
            boat.add([k.sprite("boat", V.size("boat", 0.5)), k.pos(-20 * V.ppu, -3.3 * V.ppu), k.anchor("center")]);
            boat.onUpdate(() => (boat.pos.x += 2 * V.ppu * k.dt()));
            return { V, boat };
        },
        async onDone({ fade, V, boat }) {
            await fade.out(0.4);
            boat.destroy();
            k.get("*").forEach((o) => o !== fade.obj && o.destroy());
            k.add([k.sprite("izanagi_shore", V.size("izanagi_shore", 0.7)), k.pos(W / 2, H / 2), k.anchor("center")]);
            await fade.in(0.4);
            await sleep(5);
            goto("dndHiragana");
        },
    });
}

/** DnD_Hiragana_Transition: the door opens onto the katakana catacombs. */
export function hiraganaDoorTransitionScene() {
    transitionScene({
        story: STORY.hiraganaDoorDone,
        slideKeys: ["door_opening", "tunnel_katakana"],
        slideOnLine: (i, show) => i > 0 && show.show(i),
        onDone: () => goto("level2"),
    });
}

/** Level_02_Transition: a big gravestone blocks the way. */
export function level2TransitionScene() {
    transitionScene({
        story: STORY.level2Done,
        slideKeys: ["katakana_done", "katakana_done_grave"],
        slideOnLine: (i, show) => i > 0 && show.show(i),
        onDone: () => goto("dndKatakana"),
    });
}

/** DnD_Katakana_Transition: the kappa challenges you to a word game. */
export function katakanaGraveTransitionScene() {
    transitionScene({
        story: STORY.katakanaGraveDone,
        slideKeys: ["kappa_transition"],
        allowRomaji: false,
        onDone: () => goto("level3"),
    });
}

/** Level_03_Transition: the kappa bows and the water spills from its head. */
export function level3TransitionScene() {
    resetUI();
    const fade = fader(true);
    fade.in(0.4);
    const show = slides(["kappa_bow1", "kappa_bow2", "kappa_bow3", "kappa_bow4"]);
    (async () => {
        await sleep(2);
        let water = null;
        for (let i = 0; i < 2; i++) {
            if (i === 1) water = playSound("music:water", { volume: 1 });
            await sleep(1);
            show.next();
            await sleep(1);
        }
        await sleep(7);
        await Promise.all([fade.out(2), water ? fadeSound(water, 1) : null]);
        goto("storyEnd");
    })();
}

/** Story_End: Izanami is found... and it's time to run. */
export function storyEndScene() {
    resetUI();
    const fade = fader(true);
    playSound("music:come_play_with_me", { loop: true, volume: 0.5 });
    const show = slides(["izanami_2", "izanami_1", "izanami_dead", "nope", "runaway"]);
    let box = null;

    (async () => {
        await fade.in(4);
        await sleep(2);
        // Prolouge(): two slow dips to half-black while the image changes.
        for (let i = 0; i < 2; i++) {
            await new Promise((r) => k.tween(0, 0.5, 2, (v) => (fade.obj.opacity = v)).onEnd(r));
            show.next();
            await new Promise((r) => k.tween(0.5, 0, 2, (v) => (fade.obj.opacity = v)).onEnd(r));
        }
        await sleep(0.2);
        box = storyBox({
            story: STORY.ending,
            romaji: false,
            english: subtitles().english,
            onLine: (i) => i > 0 && i < 3 && show.next(),
            onDone: async () => {
                await fade.out(2);
                goto("credits");
            },
        });
    })();
    k.onMousePress(() => box && box.advance());
}

/** Credits.unity: scrolling credits over the bridge with drifting mist. */
export function creditsScene() {
    resetUI();
    useCursor();
    const fade = fader(true);
    fade.in(0.4);
    const music = playSound("music:gymnopedie", { loop: true, volume: 0.15 });

    const V = worldView(7.5827, 0.5, 1.26);
    k.add([k.sprite("credits_bg", V.size("credits_bg")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(-10)]);
    k.add([k.sprite("credits_couple", V.size("credits_couple")), k.pos(V.x(-6), V.y(0)), k.anchor("center"), k.z(0)]);

    // MistMover.cs: two copies of the mist per layer, wrapping every 27 units.
    const mist = (y, speed, z) => {
        const s = V.size("mist");
        for (const off of [0, 27]) {
            const o = k.add([k.sprite("mist", s), k.pos(V.x(off), V.y(y)), k.anchor("center"), k.z(z)]);
            const startX = o.pos.x;
            o.onUpdate(() => {
                const shift = (k.time() * speed) % 27;
                o.pos.x = startX - shift * V.ppu;
            });
        }
    };
    mist(3, 0.2, -5);
    mist(1, 0.5, 5);

    const credits = [
        "Credits", "", "", "",
        "Game Producer", "Nina Langton", "",
        "Art", "Kate Eggleston", "",
        "Programming", "Ilerioluwa Oyedele", "",
        "Story", "Babaloluwa Oyedele", "Julia Lin", "",
        "Testing", "Julia Lin", "",
        "Voice Over", "Julia Lin", "Ryo Abe", "",
        "-Music-", "",
        "\"Clean Soul\"", "Kevin MacLeod", "",
        "\"Wet Riffs\"", "Kevin MacLeod", "",
        "\"Clear Waters\"", "Kevin MacLeod", "",
        "\"Measured Paces\"", "Kevin MacLeod", "",
        "\"Come Play with Me\"", "Kevin MacLeod", "",
        "\"Gymnopedie No. 1\"", "Kevin MacLeod", "",
        "\"Music Box Lullaby Clip\"", "rockerfaerie", "",
        "-Sound Effects-", "",
        "\"manual typewriter\"", "theshaggyfreak", "",
        "-Fonts-", "",
        "\"Japestyle\"", "Fonttia", "",
        "\"Typewriter Condensed\"", "Volker Busse", "",
        "", "", "", "", "",
        "Kami No Tabi", "", "by", "", "UBCO Japanese", "Games",
        "", "", "Web port built with KAPLAY",
    ].join("\n");
    const text = k.add([k.text(credits, { size: 50, font: FONT_TYPE, width: 560, align: "center", lineSpacing: 8 }), k.pos(960, H + 20), k.anchor("top"), k.color(0, 0, 0), k.z(10)]);

    let leaving = false;
    const leave = async () => {
        if (leaving) return;
        leaving = true;
        await Promise.all([fade.out(2), fadeSound(music, 2)]);
        goto("menu");
    };
    text.onUpdate(() => {
        text.pos.y -= 50 * k.dt();
        if (text.pos.y + text.height < 0) leave();
    });

    button({ x: 48, y: 46, w: 54, h: 54, label: "X", font: FONT_TYPE, size: 40, z: 900, onClick: () => goto("menu") });
}
