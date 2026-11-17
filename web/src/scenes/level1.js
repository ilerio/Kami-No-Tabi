// Level_01: hear a hiragana, tap the oni wearing it. Correct answers add time, mistakes take it.
import { k, W, H, goto, playSound, FONT_JP, FONT_TYPE } from "../engine.js";
import { fader, iconButton, label, useCursor, resetUI, worldView, redFlash, shake, pauseMenu, endScreen, chartOverlay, inputBlocked, makeClickable } from "../ui.js";
import { KANA, sheetSprite } from "../data/kana.js";
import { KEYS, unlock } from "../save.js";
import { shuffle, pickDistractors } from "./common.js";

const START_TIME = 20;
const TIME_BONUS = 5;
const TIME_PENALTY = -3;
const ONI_X = [-22, -2, 19];

export function level1Scene() {
    resetUI();
    useCursor();
    fader(true).in(0.4);
    playSound("music:clean_soul", { loop: true, volume: 0.5 });

    const V = worldView(18.2709, 0, -3);
    k.add([k.sprite("water", V.size("water")), k.pos(V.x(0), V.y(0)), k.anchor("center"), k.z(-100)]);
    k.add([k.sprite("boat_front", V.size("boat_front")), k.pos(V.x(0), V.y(-10.7)), k.anchor("center"), k.z(-10)]);
    const flash = redFlash();

    const order = shuffle(KANA.map((_, i) => i)); // pre-selected stack of answers
    let answer = null;
    let correctSlot = 0;
    let found = 0;
    let time = START_TIME;
    let paused = false;
    let chart = null;
    let over = false;
    let voice = null;
    let romajiOn = false;

    // --- HUD -------------------------------------------------------------------------------
    const timerText = label(String(START_TIME), { x: 120, y: 100, size: 100, z: 500 });
    const incDec = label("", { x: 250, y: 100, size: 80, z: 500 });
    incDec.hidden = true;
    const countText = label("0 / 46", { x: 54, y: 700, size: 80, anchor: "left", z: 500 });
    const romaji = label("", { x: 1060, y: 642, size: 150, font: FONT_JP, color: [200, 200, 200], z: 500 });
    romaji.hidden = true;

    const speakerOn = iconButton({ x: 760, y: 100, sprite: "ui_speaker_romaji", w: 110, z: 500, onClick: () => setRomaji(true) });
    const speakerOff = iconButton({ x: 760, y: 100, sprite: "ui_speaker", w: 110, z: 500, onClick: () => setRomaji(false) });
    speakerOff.hidden = true;
    const chartBtn = iconButton({ x: 960, y: 100, sprite: "ui_hiragana", w: 230, h: 120, z: 500, onClick: toggleChart });
    const pauseBtn = iconButton({ x: 1190, y: 100, sprite: "ui_pause", w: 110, z: 500, onClick: pause });
    const hud = [timerText, countText, speakerOn, speakerOff, chartBtn, pauseBtn];

    function setRomaji(on) {
        romajiOn = on;
        speakerOn.hidden = on;
        speakerOff.hidden = !on;
        romaji.hidden = !on || !!chart;
    }

    function toggleChart() {
        if (chart) {
            chart.destroy();
            chart = null;
        } else chart = chartOverlay("hiragana_chart");
        romaji.hidden = !romajiOn || !!chart;
    }

    function pause() {
        if (chart) toggleChart();
        paused = true;
        pauseMenu({ onResume: () => (paused = false), onRestart: () => goto("level1"), onQuit: () => goto("menu") });
    }

    const blocked = () => paused || over || !!chart || inputBlocked();

    // --- Rounds ----------------------------------------------------------------------------
    function sayAnswer() {
        if (voice) voice.stop();
        voice = playSound(`voice:${answer.voice}`, { volume: 0.7 });
    }

    function newRound() {
        k.get("oni").forEach((o) => o.destroy());
        answer = KANA[order.pop()];
        correctSlot = k.randi(0, 3);
        const kana = pickDistractors(answer, 3, correctSlot, (a, b) => {
            // お and を sound the same, so never show both in one round.
            const pair = [a.name, b.name].sort().join();
            return pair === "O,Wo";
        });
        kana.forEach((kn, slot) => addOni(slot, kn));
        romaji.text = answer.romaji;
        sayAnswer();
    }

    function addOni(slot, kana) {
        const x = V.x(ONI_X[slot]);
        const y = V.y(-3);
        const size = V.size("oni", 3);
        const oni = k.add([k.sprite("oni", size), k.pos(x, y), k.anchor("center"), k.z(1), "oni"]);
        // Hiragana painted on the oni's face (child at local (1, 0.5), scale 1.4)
        oni.add([k.sprite(sheetSprite("hiragana", kana), { height: size.height * 0.24 }), k.pos(size.width * 0.15, -size.height * 0.06), k.anchor("center")]);
        // Click target roughly matching the original polygon collider around the head
        const hit = oni.add([k.rect(size.width * 0.62, size.height * 0.72), k.pos(size.width * 0.08, -size.height * 0.05), k.anchor("center"), k.opacity(0), k.area()]);
        makeClickable(hit, () => {
            if (!blocked()) choose(slot);
        });
    }

    function choose(slot) {
        if (slot === correctSlot) {
            found++;
            countText.text = `${found} / 46`;
            if (found === KANA.length) return win();
            changeTime(TIME_BONUS);
            newRound();
        } else {
            sayAnswer();
            shake();
            flash.flash();
            changeTime(TIME_PENALTY);
        }
    }

    function changeTime(amount) {
        time += amount;
        incDec.text = amount > 0 ? `+${amount}` : `${amount}`;
        incDec.color = amount > 0 ? k.rgb(0, 255, 0) : k.rgb(255, 0, 0);
        incDec.hidden = false;
        k.wait(0.5, () => (incDec.hidden = true));
    }

    function hideHud() {
        for (const o of hud) o.hidden = true;
        romaji.hidden = true;
        incDec.hidden = true;
        if (chart) toggleChart();
        k.get("oni").forEach((o) => o.destroy());
    }

    function gameOver() {
        over = true;
        hideHud();
        shake();
        flash.on();
        endScreen({ title: "ざんねん...", onRestart: () => goto("level1"), onQuit: () => goto("menu") });
    }

    function win() {
        over = true;
        unlock(KEYS.dndHiragana);
        const score = Math.floor(time);
        hideHud();
        endScreen({
            title: `おめでとう!\nスコア: ${score}`,
            titleSize: 130,
            onRestart: () => goto("level1"),
            onQuit: () => goto("menu"),
            onNext: () => goto("level1Transition"),
        });
    }

    k.onUpdate(() => {
        if (paused || over) return;
        time -= k.dt();
        timerText.text = time > 0 ? String(Math.round(time)) : "0";
        if (Math.floor(time) <= 0) gameOver();
    });

    newRound();
}
