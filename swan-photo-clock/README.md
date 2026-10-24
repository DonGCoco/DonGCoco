# Time, collected — live cloud photo clock

**[▶ Open the live clock (current version)](https://dongcoco.github.io/DonGCoco/swan-photo-clock/live.html?v=3)**

A connected ring of twelve flat photographs over the user's **moving cloud video**. The hour, minute **and fine seconds** hands use your browser's local device time.

[Open the live interactive clock →](https://dongcoco.github.io/DonGCoco/swan-photo-clock/live.html?v=3)

> The old GIF preview used a swan background and is retained as an **archive of v1**, not a preview of the current cloud version. GitHub README Markdown cannot render a local-time JavaScript clock; click the link above to see the actual clock.

## What's new
- Replaced the swan video with the supplied cloud and sun-ray footage (muted grade).
- Fixed 12 photo positions into a geometrically verified continuous overlap ring.
- Added an understated live seconds hand so continuous clock movement is obvious.
- Added iOS inline-video autoplay and resume handlers.
- Added `live.html` as a separate cache-safe link to the current version.

**[Design brief](DESIGN_PROMPT.md)** · [Archived v1 preview](assets/readme-preview.gif)

## Run locally

Serve the repository root: `python3 -m http.server 8000`, then browse to `/swan-photo-clock/live.html`.
