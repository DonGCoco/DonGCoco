# Design & implementation prompt — Connected Photo Clock

**Role:** You are a senior editorial art director, motion designer and front-end engineer. Recreate the composition grammar of the supplied restaurant photo-clock reference while using only the supplied original photographs and the swan video. Deliver real working HTML/CSS/JavaScript, not a generated image or a pre-rendered video.

**Visual goal:** An understated, slightly playful photographic clock assembled from 12 real rectangular photos, with no decorative UI. The reference is a tight, continuous, irregular ring of overlapping flat photographs around a clear empty center. Think casual cut-paper editorial collage, not a glossy scrapbook, gallery grid, Polaroid board or raised card design.

**1 — Clock composition**
- Precisely twelve photo tiles, one at each nominal hour, forming ONE LINKED 12-piece closed loop: 12→1→2→3→4→5→6→7→8→9→10→11→12.
- Every neighboring pair must physically overlap or touch in the composition; verify this with a visual or coordinate intersection check. The loop must not split into floating clusters. Prefer overlapping tile edges around 10–25% where composition permits.
- Uneven portrait and landscape crops with subtly varied tile dimensions. Asymmetric offsets and over/under ordering, like photos laid directly on a surface. Preserve meaningful subjects when cropping.
- Preserve a generous center opening for clock hands. Overall ring readable instantly as a clock, but never a rigid perfect circle or 12 isolated evenly spaced squares.
- The original photographic images are FLAT, front-facing, perfectly rectangular, hard-edged and unframed: no shadows, stroke borders, rounded corners, tape, embossing, bevels, 3D, gradients on tile, Polaroid margins, paper textures, glass or reflections.

**2 — Color art direction**
- Select 12 photographs for AESTHETIC COLOR RHYTHM, NOT actual month-of-capture chronology.
- Cool blue / cyan / neutral stone and white are the dominant palette. Sprinkle vivid temple red, fire orange, saffron yellow and pink smoke as intentional punctuations, alternating them with quieter photos around the clock.
- Balance visual weight between opposite quadrants. Avoid clustering all warm accents or all pale low-contrast images in one area. Do not recolor photographs heavily.
- The actual moving swan-on-blue-water video is the full-bleed BACKGROUND. Use the video directly (autoplay, muted, playsinline, loop). Only a subtle dark-blue dimming may be used to protect numeral legibility. Never invent a navy illustrated landscape or replace the swan with an AI-generated scene.

**3 — Typography**
- White casual handwritten marker numerals 1–12, using slender single-stroke imperfect shapes resembling the reference's simple scribbled numbers.
- Numbers sit directly on/near their corresponding photos, sometimes touching the edge. Irregular human spacing and modest tilt; all must remain readable.
- Absolutely no elegant calligraphy, serif typography, neon, thick brush script, faux chalk texture or 3D fonts.

**4 — Real clock mechanics**
- Exactly two slim white hand-drawn hour and minute hands, with tiny imperfect open arrow/marker tips and a small white central hub. They should feel drawn by a pen, not like a metallic or ornate watch mechanism.
- **MUST use the viewer's actual computer/browser local time:** JavaScript `new Date()`, including seconds/milliseconds for smooth interpolation. Hours and minutes determine each hand's angle. Refresh continuously with `requestAnimationFrame`; never hardcode 12:15, a demo rotation, or an animated GIF for the actual live product.
- Do not add an outer dial, tick marks, a digital time readout, or a sweeping seconds hand unless requested.

**5 — Implementation & quality gate**
- Responsive portrait layout (mobile-first), crisp photos and thin ink vectors, accessible text alternative, graceful static background fallback.
- Deliver runnable `index.html` and local `assets/` including the original real swan video, 12 crops and README preview.
- Acceptance criteria: (a) connected 12-tile collage loop with real overlaps; (b) zero card/emboss/shadow effect; (c) handwriting closely references supplied specimen; (d) swan video genuinely moves; (e) hour/minute hands show device local time at all moments.
- Include an animated preview image in GitHub README linking to the actual live GitHub Pages clock. Be explicit: GitHub README sanitizes JavaScript and cannot itself run a per-viewer local-time clock. Only the linked web page can do that.