// The 46 basic kana in chart (gojūon) order.
// `name` matches the slice names in the Unity sprite sheets, `voice` the hiragana audio file,
// `romaji` is what the player sees.
export const KANA = [
    ["A", "a", "あ", "ア"], ["I", "i", "い", "イ"], ["U", "u", "う", "ウ"], ["E", "e", "え", "エ"], ["O", "o", "お", "オ"],
    ["Ka", "ka", "か", "カ"], ["Ki", "ki", "き", "キ"], ["Ku", "ku", "く", "ク"], ["Ke", "ke", "け", "ケ"], ["Ko", "ko", "こ", "コ"],
    ["Sa", "sa", "さ", "サ"], ["Shi", "shi", "し", "シ"], ["Su", "su", "す", "ス"], ["Se", "se", "せ", "セ"], ["So", "so", "そ", "ソ"],
    ["Ta", "ta", "た", "タ"], ["Chi", "chi", "ち", "チ"], ["Tsu", "tsu", "つ", "ツ"], ["Te", "te", "て", "テ"], ["To", "to", "と", "ト"],
    ["Na", "na", "な", "ナ"], ["Ni", "ni", "に", "ニ"], ["Nu", "nu", "ぬ", "ヌ"], ["Ne", "ne", "ね", "ネ"], ["No", "no", "の", "ノ"],
    ["Ha", "ha", "は", "ハ"], ["Hi", "hi", "ひ", "ヒ"], ["Hu", "fu", "ふ", "フ"], ["He", "he", "へ", "ヘ"], ["Ho", "ho", "ほ", "ホ"],
    ["Ma", "ma", "ま", "マ"], ["Mi", "mi", "み", "ミ"], ["Mu", "mu", "む", "ム"], ["Me", "me", "め", "メ"], ["Mo", "mo", "も", "モ"],
    ["Ya", "ya", "や", "ヤ"], ["Yu", "yu", "ゆ", "ユ"], ["Yo", "yo", "よ", "ヨ"],
    ["Ra", "ra", "ら", "ラ"], ["Ri", "ri", "り", "リ"], ["Ru", "ru", "る", "ル"], ["Re", "re", "れ", "レ"], ["Ro", "ro", "ろ", "ロ"],
    ["Wa", "wa", "わ", "ワ"], ["Wo", "wo", "を", "ヲ"], ["N", "n", "ん", "ン"],
].map(([name, voice, hira, kata]) => ({
    name,
    voice,
    hira,
    kata,
    romaji: voice === "wo" ? "Wo" : voice[0].toUpperCase() + voice.slice(1),
}));

// Where each chart position sits in the 5-column drag-and-drop grid ([row, col]).
// The ya and wa rows have gaps, like a printed kana chart.
export const CHART_CELLS = KANA.map((_, i) => {
    if (i < 35) return [Math.floor(i / 5), i % 5];
    return [
        [7, 0], [7, 2], [7, 4],
        [8, 0], [8, 1], [8, 2], [8, 3], [8, 4],
        [9, 0], [9, 4],
        [10, 0],
    ][i - 35];
});

export const sheetSprite = (sheet, kana) => `${sheet}:${kana.name}`;

// Level 3 palettes (Characters.cs), in the original insertion order.
export const HIRAGANA_PALETTE = [..."あいうえおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろわをん"];
export const KATAKANA_PALETTE = [..."ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロワヲンー"];
