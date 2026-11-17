<#
  Converts the Unity project's assets into web-ready files for the Kaplay port.

  - Images are downscaled (some originals are 11811x8269, beyond WebGL texture limits)
    and saved as JPEG when fully opaque, PNG otherwise.
  - Sliced Unity sprite sheets (hiragana / katakana charts) are copied untouched and their
    slice rects are converted from Unity's bottom-left origin into Kaplay atlas data.
  - Audio and fonts are copied with web-friendly names.
  - Writes src/assets.generated.js, which the game imports to load everything.

  Usage (from the repo root):  powershell -ExecutionPolicy Bypass -File web/tools/build-assets.ps1
#>
$ErrorActionPreference = 'Stop'
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$src = Join-Path $repo 'Unity_Folder\Assets'
$web = Join-Path $repo 'web'
$out = Join-Path $web 'assets'

Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public static class ImgTool {
    // Returns the extension written (".png" or ".jpg").
    public static string Convert(string srcPath, string dstNoExt, int maxDim, out int w, out int h, out int ow, out int oh) {
        using (var img = Image.FromFile(srcPath)) {
            ow = img.Width; oh = img.Height;
            double s = Math.Min(1.0, (double)maxDim / Math.Max(img.Width, img.Height));
            w = Math.Max(1, (int)Math.Round(img.Width * s));
            h = Math.Max(1, (int)Math.Round(img.Height * s));
            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb)) {
                using (var g = Graphics.FromImage(bmp)) {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    using (var wrap = new ImageAttributes()) {
                        wrap.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(img, new Rectangle(0, 0, w, h), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrap);
                    }
                }
                if (HasAlpha(bmp)) {
                    bmp.Save(dstNoExt + ".png", ImageFormat.Png);
                    return ".png";
                }
                var enc = Array.Find(ImageCodecInfo.GetImageEncoders(), c => c.MimeType == "image/jpeg");
                var p = new EncoderParameters(1);
                p.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 88L);
                using (var rgb = bmp.Clone(new Rectangle(0, 0, w, h), PixelFormat.Format24bppRgb))
                    rgb.Save(dstNoExt + ".jpg", enc, p);
                return ".jpg";
            }
        }
    }

    static bool HasAlpha(Bitmap bmp) {
        var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try {
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            var buf = new byte[bytes];
            Marshal.Copy(data.Scan0, buf, 0, bytes);
            for (int i = 3; i < bytes; i += 4) if (buf[i] < 250) return true;
            return false;
        } finally { bmp.UnlockBits(data); }
    }
}
'@

# key = output name, s = source (relative to Assets), m = max dimension in px
$images = @(
    # story slides / full-screen backgrounds
    @{k='bridge';                 s='Images/StoryImages/Bridge scene.png';                        m=1600},
    @{k='izanami_baby';           s='Images/StoryImages/izanami with baby.png';                   m=1600},
    @{k='izanami_fire';           s='Images/StoryImages/izanami on fire.png';                     m=1600},
    @{k='izanagi_grave';          s='Images/StoryImages/izanagi at grave.png';                    m=1600},
    @{k='izanagi_boat';           s='Images/StoryImages/izanagi in the boat.png';                 m=1600},
    @{k='door_opening';           s='Images/StoryImages/door opening.png';                        m=1600},
    @{k='tunnel_katakana';        s='Images/StoryImages/tunnel1katakana.png';                     m=1600},
    @{k='katakana_done';          s='Images/StoryImages/katakana game finished with black.png';   m=1600},
    @{k='katakana_done_grave';    s='Images/StoryImages/katakana game finished_gravestone.png';   m=1600},
    @{k='izanami_1';              s='Images/StoryImages/izanami 1.png';                           m=1600},
    @{k='izanami_2';              s='Images/StoryImages/izanami 2.png';                           m=1600},
    @{k='izanami_dead';           s='Images/StoryImages/izanami dead.png';                        m=1600},
    @{k='nope';                   s='Images/StoryImages/Nope.png';                                m=1600},
    @{k='runaway';                s='Images/StoryImages/runaway .png';                            m=1600},
    @{k='water';                  s='Images/Art4Game/water.png';                                  m=1600},
    @{k='izanagi_shore';          s='Images/Art4Game/door2.png';                                  m=1600},
    @{k='gate';                   s='Images/Art4Game/gate without bars.png';                      m=1600},
    @{k='kappa_bg';               s='Images/Art4Game/Kappa game new.png';                         m=1600},
    @{k='kappa_transition';       s='Images/Art4Game/kappa-game-transition.png';                  m=1600},
    @{k='kappa_bow1';             s='Images/Art4Game/Kappa_trasition_bow1.png';                   m=1600},
    @{k='kappa_bow2';             s='Images/Art4Game/Kappa_trasition_bow2.png';                   m=1600},
    @{k='kappa_bow3';             s='Images/Art4Game/Kappa_trasition_bow3.png';                   m=1600},
    @{k='kappa_bow4';             s='Images/Art4Game/Kappa_trasition_bow4.png';                   m=1600},
    @{k='credits_bg';             s='Images/Art4Game/just background.png';                        m=1600},
    @{k='credits_couple';         s='Images/Art4Game/Bridge scene just izanami and izanagi.png';  m=1600},
    @{k='mist';                   s='Images/Sprites/mist.png';                                    m=1600},
    @{k='ubc_logo';               s='Images/Sprites/ubc_logo.jpg';                                m=1024},
    @{k='hiragana_chart';         s='Images/Sprites/hiragana_chart.jpg';                          m=1600},
    @{k='katakana_chart';         s='Images/Sprites/katakana_chart_temp.jpg';                     m=1600},
    # props
    @{k='boat';                   s='Images/Art4Game/boat.png';                                   m=1400},
    @{k='boat_reflection';        s='Images/Art4Game/boat_reflection.png';                        m=1400},
    @{k='boat_front';             s='Images/Sprites/boat from front.png';                         m=1400},
    @{k='izanagi';                s='Images/Art4Game/izanagi.png';                                m=800},
    @{k='oni';                    s='Images/Sprites/Oni.png';                                     m=650},
    @{k='lock';                   s='Images/Sprites/Lock_Skined.png';                             m=600},
    @{k='grave';                  s='Images/Art4Game/grave1.png';                                 m=800},
    @{k='grave_stone';            s='Images/Art4Game/new grave2.png';                             m=1400},
    @{k='flower';                 s='Images/Art4Game/Flower.png';                                 m=800},
    @{k='kappa';                  s='Images/Art4Game/New kappa.png';                              m=900},
    @{k='izanami_small';          s='Images/Art4Game/izanami no background .png';                 m=600},
    # UI
    @{k='ui_pause';               s='Images/UI/Pause.png';                                        m=256},
    @{k='ui_gear';                s='Images/UI/Gear_Button_Selected.png';                         m=256},
    @{k='ui_heart';               s='Images/UI/heart 1.png';                                      m=256},
    @{k='ui_hiragana';            s='Images/UI/Hiragana_Button.png';                              m=256},
    @{k='ui_katakana';            s='Images/UI/Katakana_Button.png';                              m=256},
    @{k='ui_next';                s='Images/UI/next.png';                                         m=256},
    @{k='ui_play';                s='Images/UI/play80 (1).png';                                   m=256},
    @{k='ui_power';               s='Images/UI/power_icon.png';                                   m=256},
    @{k='ui_refresh';             s='Images/UI/refresh-128.png';                                  m=256},
    @{k='ui_select';              s='Images/UI/SelectScene.png';                                  m=256},
    @{k='ui_speaker';             s='Images/UI/Speaker_Icon.png';                                 m=256},
    @{k='ui_speaker_romaji';      s='Images/UI/SpeakerAndRomanji_Icon.png';                       m=256}
)
# Level 3 word pictures (named after the word key used by the game)
Get-ChildItem (Join-Path $src 'Images/Art4Game/L03') -Filter *.png | ForEach-Object {
    $images += @{k = 'word_' + $_.BaseName.ToLower(); s = 'Images/Art4Game/L03/' + $_.Name; m = 512}
}

$sheets = @(
    @{k='hiragana';       s='Images/Sprites/hiragana chart.png'},
    @{k='hiragana_white'; s='Images/Sprites/hiragana chart white.png'},
    @{k='katakana';       s='Images/Sprites/katakana chart.png'},
    @{k='katakana_white'; s='Images/Sprites/katakana chart white.png'}
)

$music = @{
    'clean_soul' = 'Sound/Music/Clean Soul.mp3';            'wet_riffs' = 'Sound/Music/Wet Riffs.mp3'
    'march_of_the_mind' = 'Sound/Music/March of the Mind.mp3'; 'measured_paces' = 'Sound/Music/MeasuredPaces.mp3'
    'despair_and_triumph' = 'Sound/Music/Despair and Triumph.mp3'; 'come_play_with_me' = 'Sound/Music/Come Play with Me.mp3'
    'gymnopedie' = 'Sound/Music/Gymnopedie No 1.mp3';         'lullaby' = 'Sound/SoundEffects/163644__rockerfaerie__music-box-lullaby-clip.wav'
    'water' = 'Sound/SoundEffects/17648__rupert1073__water.wav'
}
$sfx = @{
    'typewriter' = 'Sound/SoundEffects/118591__theshaggyfreak__manual-typewriter-old1.wav'
    'cinderblock' = 'Sound/SoundEffects/95000__j1987__cinderblockmove.wav'
    'correct' = 'Sound/SoundEffects/correct.mp3'; 'wrong' = 'Sound/SoundEffects/wrong.mp3'
    'door_opening' = 'Sound/SoundEffects/door_opening.wav'; 'welcome' = 'Sound/SoundEffects/welcome.wav'
}
$fonts = @{ 'japestyle' = 'Fonts/Japestyle Plain.ttf'; 'typewriter' = 'Fonts/typewcond_demi.otf' }

foreach ($d in 'img', 'sheets', 'music', 'sfx', 'voice', 'fonts') {
    New-Item -ItemType Directory -Force (Join-Path $out $d) | Out-Null
}

$manifest = [ordered]@{ images = [ordered]@{}; sheets = [ordered]@{}; music = [ordered]@{}; sfx = [ordered]@{}; voice = [ordered]@{}; fonts = [ordered]@{} }

foreach ($i in $images) {
    $p = Join-Path $src $i.s
    Get-ChildItem (Join-Path $out 'img') -Filter ($i.k + '.*') | Remove-Item
    $w = 0; $h = 0; $ow = 0; $oh = 0
    $ext = [ImgTool]::Convert($p, (Join-Path $out ('img/' + $i.k)), $i.m, [ref]$w, [ref]$h, [ref]$ow, [ref]$oh)
    # w/h: size of the web file; ow/oh: original size (Unity world size = ow/100 x oh/100 units)
    $manifest.images[$i.k] = [ordered]@{ url = "assets/img/$($i.k)$ext"; w = $w; h = $h; ow = $ow; oh = $oh }
    Write-Host ("img   {0,-22} {1}x{2}{3}" -f $i.k, $w, $h, $ext)
}

foreach ($sh in $sheets) {
    $p = Join-Path $src $sh.s
    Copy-Item $p (Join-Path $out "sheets/$($sh.k).png") -Force
    $img = [System.Drawing.Image]::FromFile($p); $H = $img.Height; $img.Dispose()
    $meta = Get-Content ($p + '.meta') -Raw
    $atlas = [ordered]@{}
    $rx = [regex]'(?m)^\s+name: (\S+)\s*\n\s+rect:\s*\n\s+serializedVersion: \d+\s*\n\s+x: ([\d.]+)\s*\n\s+y: ([\d.]+)\s*\n\s+width: ([\d.]+)\s*\n\s+height: ([\d.]+)'
    foreach ($m in $rx.Matches($meta)) {
        $x = [double]$m.Groups[2].Value; $y = [double]$m.Groups[3].Value
        $rw = [double]$m.Groups[4].Value; $rh = [double]$m.Groups[5].Value
        $atlas[$m.Groups[1].Value] = [ordered]@{ x = $x; y = $H - $y - $rh; width = $rw; height = $rh }
    }
    $manifest.sheets[$sh.k] = [ordered]@{ url = "assets/sheets/$($sh.k).png"; atlas = $atlas }
    Write-Host ("sheet {0,-22} {1} slices" -f $sh.k, $atlas.Count)
}

function Copy-Named($table, $dir, $section) {
    foreach ($k in ($table.Keys | Sort-Object)) {
        $p = Join-Path $src $table[$k]
        $ext = [IO.Path]::GetExtension($p).ToLower()
        Copy-Item $p (Join-Path $out "$dir/$k$ext") -Force
        $manifest[$section][$k] = "assets/$dir/$k$ext"
    }
}
Copy-Named $music 'music' 'music'
Copy-Named $sfx 'sfx' 'sfx'
Copy-Named $fonts 'fonts' 'fonts'

Get-ChildItem (Join-Path $src 'Sound/Hiragana_Audio') -Filter *.mp3 | ForEach-Object {
    Copy-Item $_.FullName (Join-Path $out "voice/$($_.Name)") -Force
    $manifest.voice[$_.BaseName] = "assets/voice/$($_.Name)"
}

$json = $manifest | ConvertTo-Json -Depth 6
$js = "// Generated by web/tools/build-assets.ps1 - do not edit by hand.`nexport default $json;`n"
[IO.File]::WriteAllText((Join-Path $web 'src/assets.generated.js'), $js, (New-Object System.Text.UTF8Encoding $false))
Write-Host "Wrote src/assets.generated.js"
