import { k } from "../engine.js";
import { KANA } from "../data/kana.js";

export function shuffle(arr) {
    const a = [...arr];
    for (let i = a.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [a[i], a[j]] = [a[j], a[i]];
    }
    return a;
}

/**
 * Builds a round of `count` kana with `answer` at `answerSlot` and random, distinct
 * distractors elsewhere. `conflicts(a, b)` can veto pairs that shouldn't appear together.
 */
export function pickDistractors(answer, count, answerSlot, conflicts = () => false) {
    const round = [];
    round[answerSlot] = answer;
    for (let slot = 0; slot < count; slot++) {
        if (slot === answerSlot) continue;
        let pick;
        do {
            pick = KANA[k.randi(0, KANA.length)];
        } while (round.some((r) => r && (r === pick || conflicts(r, pick))));
        round[slot] = pick;
    }
    return round;
}
