# Sync Rush — Build Progress

Tracks implementation status against [GDD.md](GDD.md). Update this file whenever a
GDD system is added, changed, or a milestone gate is crossed — don't let it drift.

Last verified against repo: **2026-10-16** (end of session), commit `85d59ed`. Everything is committed (2026-10-16 work
in `9e4a296` rope/final stretch and `85d59ed` character/modes/lobby). The Editor reports no compile errors.

---
## 🎯 Playtest & tuning backlog (things to judge by feel, ideally with a real friend)

All values live on `PlayerPrefab` → `PlayerTether` / `SyncRushPlayerController` in the Inspector. Prefab values
override code defaults, so tune them there. ⭐ = most important for the M1 gate "is the rope fun?".

| # | What to test | Current values | Question to answer |
|---|---|---|---|
| 1 ⭐ | **Leash (heavy drag)**: user tested on one machine 2026-10-12, "feels good now" | `Leash Length` 36 m, `Leash Away Speed Factor` 0.5, `Leash Ramp Distance` 4 m, `Leash Drag Factor` 1, `Leash Respawn Grace` 3 s | What's the best leash length? Does 50% feel like mud? Does a strong player still ignore their partner? Does the drag feel fair to the dragged player? |
| 2 ⭐ | **Tow feel** | `Tow Speed Fraction` 1.5, `Tow Full Pull Length` 20 m, `Tow Slack Length` 10 m, `Tow Leader Speed Multiplier` 0.85 | Felt "a bit weak" with auto-run (2026-10-12). With a human partner: strong enough? If not, make it more than catch-up (e.g. water-ski burst for both) |
| 3 | **Tow path-following** | `Trail Spacing` 1 m, `Trail Reach Radius` 2 m | Does following the leader's path feel right, or is a straight-line pull better? (agreed fallback) |
| 4 | **Slingshot** | range 36 m, launch 12–40 m/s + 14 lift, tension 1.2 s | Felt right 2026-10-07. Re-check now that the leash exists: landing past the leash drags the anchor along. Good synergy or annoying? |
| 5 | **Rival slingshot cooldown** | `Rival Cooldown` 15 s (teammate 6 s), confirmed in play 2026-10-30 | Still abusable for sabotage? Too long? |
| 6 | **Rope look** | `RopeVisual`: slack ×1.2 / +3 m, damping 0.985, 24 points; `Snap Width Factor` 0.6 | Too floaty, stiff or long? Is the rigid thinning look clear enough past the leash? |
| 7 | **Aim targeting** | `Aim Angle` 25° | Easy to pick the intended player in a crowd? Marker simplified 2026-10-30 (below): still clear enough who you'll grab? |
| 8 | **Everything over real Relay** (also the M0 gate) | — | Leash and Tow both read the partner's position ~0.1 s late: any rubber-banding? Slingshot and hazards with real latency? |
| 9 | ~~**3-player teams**~~ | Decided 2026-10-16: the host picks Teams or Free-for-all in the lobby (see "Start here" item T) | — |
| 10 | **Course layout** | greybox (2026-09-16) | User said it doesn't fully match what they're picturing yet |
| 11 ⭐ | **Final stretch length** | `FinalStretchZone` in `GameScene`: x −242.4 → −255.1 (~12.6 m since the 0.8 course scale, 2026-10-30) | GDD says ~10 m (≈25 m at this scale). 17 m chosen so the snap happens on landing, not mid-air over the `Bounce (8)` gap. Long enough for shoving to matter? |
| 12 | **Shove** | `PlayerShove`: range 5 m (centre to centre), cone 60°, force 16 m/s + 6 up, stun 0.3 s, cooldown 1.5 s | Funny or frustrating? Can you get shoved off the platform edge (intended)? **Shoving teammates** (GDD design, user unsure 2026-10-16): fun betrayal or feels wrong? |
| 15 | ~~**Player vs course scale**~~ | Decided 2026-10-30: player stays 5 m, course shrunk to 0.8, pendulums/spinner enlarged | Done (see "Scale trial" under "Start here"). Still judge in play: bounce height, knockback distance |
| 14 | **Run animation speed** | `PlayerAnimator._runAnimSpeed` 9 m/s, blend tree Running at 7 m/s | Do the feet slide or skate when running / sprinting? |
| 13 | **Snap look & sound** | `RopeVisual`: recoil 30 m/s (35% when slack), fade 1.5 s; `RopeSnap_TEMP.wav` (synthesized placeholder) | Does the SNAP read clearly? Replace the sound in M4 |

**Name (2026-10-12):** "Sync Rush" stays the working title for now. **"Rope Rush" is the top contender for the official
name** (no Steam game with that name, per the user's check); "Strung Along" is the runner-up. Before deciding, also
check the app stores and trademarks. The code namespace `SyncRush` can stay internal after a rename.

**Debug left on, to remove later:** `[PushDebug]` logging (`SyncRushPlayerController._debugLogPushes`), F8
auto-run (`_debugAutoRun`), and F9 skip-to-end (`PlayerRaceProgress`, added 2026-10-16: moves **every** player onto
the last bounce pad `Bounce (8)`, marks all checkpoints before Finish as passed and makes the pad the respawn point;
bounce and hold W to land in the FinalStretchZone; **user-tested 2026-10-16: works well**). Keep F8/F9 until two-machine testing exists.

---
## ▶ Start here next session (updated 2026-10-16, end of session)

### Next session, in order
1. **Test pass on the untested 2026-10-16 work** (host + MPPM; restart the virtual players, the prefab changed):
   - ~~**Game modes** (item T)~~: ✅ user-tested 2026-10-30, dropdown resized the same day and approved.
   - ~~**Y Bot character** (item C)~~: ✅ user-tested 2026-10-30, spawn rope fix ✅ too.
   - ~~**Lobby lineup**~~: ✅ user-tested 2026-10-30 ("working perfectly").
   - Also ✅ 2026-10-30 (user): spawn points (`SpawnPointRegistry`), rope past 36 m (rigid, thinning, darkening) and
     the 15 s rival / 6 s teammate Slingshot cooldown. **Every one-machine test is now done.**
2. ~~**Decide player vs course scale**~~: done 2026-10-30, course at 0.8 (see "Scale trial" below). **Play-test the
   rescaled course** (restart the VP): full race, falls/respawns at each checkpoint, bounce pads (incl. `Bounce (8)` →
   final platform), pendulum/spinner hits, F9 skip.
3. **Ragdoll** (`RagdollController`, GDD §5.2) on the Y Bot skeleton, then **Pull (Recover)**. The last big M1 pieces.
   **Present a plan first** (the user wants plans before big features, see "Working with the user").
4. **Jump/fall animation** (small): Y Bot idles/runs in mid-air today. Needs a Mixamo jump/fall clip from the user
   (FBX for Unity, 30 fps, without skin, keyframe reduction none, into `Assets/Characters/Mixamo/`).
5. Still blocked: **two-machine Relay test + friend playtest** (M0 and M1 gates; settles backlog ⭐ rows 1, 2, 11, 12).

**Housekeeping, when convenient:** remove `[PushDebug]` logging; delete `Assets/Scenes/CharacterTest.unity` and
`Assets/Characters/Mixamo/Preview/` once the character is settled; the `PlayerPrefab` `CapsuleCollider` height 6.41
question (Scale note); Coplay AI generation (`generate_sfx`, 3D/animation tools) returns 401 / not found, so check
the Coplay account if generated assets are wanted; official name check (app stores, trademarks) before renaming.

**Slingshot target marker simplified 2026-10-30 (user choice). ✅ User-tested same day: "working well", "definitely less distracting":** the user found the
always-on preview too busy (green/amber distance text over the aimed player out to 72 m, plus a line to them). Now:
nothing until someone is inside Slingshot range (36 m); then a small green `▼` over the player a press would pick, no
text, no line. Removed from `PlayerTether`: the preview `LineRenderer`, `_previewRangeFactor`, `_previewWidth`, the
amber out-of-range colour. (Their stale values stay in the prefab YAML until it's next saved; harmless.)
Also clarified for the user: no rope between rivals / in FFA is by design; the rival rope only shows during a Slingshot.

### Working with the user (learned 2026-10-16)
- For big features: write up the plan and wait for "go" before building; post short progress updates during long
  builds; flag anything uncertain or any deviation from the plan instead of silently choosing.
- The user tests in Play Mode and reports back; record results in the relevant item and the backlog.
- The user commits themselves; don't commit unless asked.


**Scale trial (2026-10-30), now adopted: course at 0.8, made playable (last bullet). History:** the user likes the course but wants the player bigger relative to it. Agreed
that shrinking the course is the cheaper route (keeps every tuned player/rope/camera value). A visual trial is **saved
in `GameScene`** (the user saved it at 0.7 when switching scenes; then saved again at the new values): all course
objects under `_ScaleTrial_CourseRoot` (pivot at the spawn centre, floor top y 32.36), plus two 5 m Y Bot references
`_ScaleTrial_PlayerRef_0/1` (start, pendulums). User feedback on 0.7: "doesn't look too bad", but asked for **0.8**, and
obstacles looked less intimidating, "especially the pendulum". Now: course 0.8, pendulums ×1.3 inside it (ball 10.3 m,
pivot raised to keep the 0.32 m floor gap; they swing across the course, so no overlap), spinner `RotationX` ×1.15.
Pendulums then spread evenly along `Platform (1)` (user request): x −10.74 / −30.38 / −50.02, 10 m in from each end,
19.6 m apart (was 14.7 m, bunched toward the start). **User: "the scale is looking pretty good right now"** (2026-10-30),
so 0.8 + bigger obstacles is the direction; next is the plan to make it playable (respawn points, hazard values,
remove the trial wrapper/refs).
- Pendulum timing (2026-10-30, user: 2nd and 3rd swung nearly in step): `Pendulum.cs` gained `phaseOffsetDegrees`
  (like `Rotator`/`RotationPlatform`). The three scene pendulums now use 0° / 120° / 240° with `randomStart` off.
  Root cause: the old offset was `random × speed` = 0–2 rad from a position hash, under a third of a cycle, so moving
  the pendulums happened to put two nearly in step. Also found: `speed` doesn't change swing speed (every pendulum
  swings once per 2π ≈ 6.3 s); left as is, noted in the code. ✅ User: "looks good" (2026-10-30).
- **Trial wrapper removed + sizes pushed to prefabs (2026-10-30, user request):** `_ScaleTrial_CourseRoot` removed,
  its 0.8 baked into each object (checked: no world position/size changed). Pendulum balls ×1.2 (ball 12.3 m, pivots
  raised to y 63.43 to keep the 0.32 m floor gap). Then sizes applied to the prefabs in `Assets/Prefabs/Obstacles/`
  (only `GameScene` uses them; 0 renderers changed world size): `Pendulum` root 3 → 10.29, `Ball` 1 → 1.2; `Bounce`
  root (2.28, 1, 1.83) → (5.34, 0.8, 6.34); `RotationX` root → (77.82, 0.37, 10.72) + its bar child; `RotationPlat`
  root (15, 0.25, 5) → (12, 0.2, 28.82), the middle platform's length (the other two keep their own as overrides).
  **Kept as scene overrides on purpose:** pendulum phase offsets + `randomStart`, positions, `RotationPlat` speed 40
  (prefab still 60, pre-existing). Diffs also contain Unity 6 reserialization noise (new default fields, Rigidbody
  `m_Drag` → `m_LinearDamping`), not real changes. Prefab backups: `Backups/Prefabs_before-scale-trial_2026-10-30/`.
  The user removed the `_ScaleTrial_PlayerRef` figures and made `Assets/Prefabs/Player reference.prefab` (5 m Y Bot)
  for future size checks.
- **0.8 course made playable (2026-10-30, user go-ahead).** Positions mapped around the trial pivot (49.4, 32.36, 0),
  distances ×0.8. Angle-based hazards (`RotationPlatform`, `Rotator`, `Pendulum` swing) need nothing.
  - `RaceCheckpoint._respawnPoint` (scene): Starting (47.00, 32.84, 0.53), Checkpoint (−67.89, …, 0.53), Checkpoint2
    (−123.41, …, 0.53), Finish (−233.32, 32.84, 5.60). Raycast check: every respawn and spawn point is over solid platform.
  - `PlayerPrefab` `_debugSkipPoint` (F9) → (−212.68, 32.84, 6.16), on the moved `Bounce (8)` pad (checked); code default too.
  - `Bounce.prefab`: pad `force` 30.8 → 27.55, outer trigger 10 → 8.94 (×√0.8, so apex height ×0.8: ~24 → ~19 m).
    Note: the outer `Bounce` script is **enabled** in the prefab (PROGRESS 2026-09-23 said disabled). Harmless, since
    `Launch` takes the stronger upward speed, but worth knowing.
  - `PendulumHazard` (Pendulum.prefab Ball) and `SpinnerHazard` (RotationX.prefab) `_knockbackForce` 25 → 20 m/s
    (knocked 80% as far); `_upwardBoost` 7 kept so hits still arc. Spinner's tip-speed minimum scales by itself.
  - Spawn points and `FinalStretchZone` scaled with the course already (spawns now 8 m apart, was 10).
  - **Not yet played.** Restart the MPPM virtual player (PlayerPrefab changed).
- Pendulum ball 1.2 → **1.1** in the prefab (2026-10-30, user: standing gap between pendulums too narrow). Ball 11.3 m,
  standing gap along the course 7.3 → **8.3 m**, pivots lowered to y 62.92 (floor gap 0.32 m kept).
  Gotcha found: `Pendulum (1)/(2)` had their own Ball scale overrides, so they ignored the prefab change. Cleaned up:
  37 scene scale overrides on obstacle instances that just repeated the prefab value were reverted (no size changed),
  so instances now follow future prefab edits. Only real ones kept: `RotationPlat` and `RotationPlat (2)` lengths.
- ✅ **User playtest 2026-10-30: "the whole course seems to function well"** at 0.8 (respawns, pads, hazards, F9).
  Feedback: the `RotationX` area got much easier to pass. `RotationX.prefab` `Rotator.speed` 0.4 → **0.5** (40 → 50°/s,
  +25%, offsetting the 0.8 shrink); no scene override, so the instance follows. Knockback minimum (tip speed × 1.3)
  rises with it. Not yet re-played.
Rotating gauntlet platforms and bounce pads left at 0.8 (enlarging them closes the jump gaps). **Not playable yet**
(respawn points are world-space, hazard values unscaled), view in the Scene view only. **Backup of the
pre-trial scene:** `Backups/GameScene_before-scale-trial_2026-10-30.unity` (outside `Assets/`, hash-identical to commit
`9e4a296`'s copy). If it was accidentally saved: copy that file back over `Assets/Scenes/GameScene.unity`, or
`git checkout -- Assets/Scenes/GameScene.unity`. If adopted for real, still to handle: `RaceCheckpoint._respawnPoint`
(world-space), and hazard values in world units (bounce force, mover distances/speeds, spinner knockback).

**Scale note (2026-10-16, decision pending):** the user finds the player too small for the course now that it has a
real model (backlog row 15). A 7 m player (model + collision, ×1.4) was started and **reverted at the user's request**;
5 m stays for testing. Either the player or the course has to change so everything fits. If the player is scaled:
- **Fix spawns first:** spawn/respawn/skip points sit ~0.6–1.1 m above the floor, but the root pivot is the capsule
  centre, so every spawn already starts the capsule ~2 m inside the floor and Unity pushes it out. At 7 m it would start
  ~3 m deep, through the 2.6 m-thick floor. Fix: `PlayerRaceProgress.Teleport` should place the *feet* at the point
  (lift by `height / 2 - center.y`).
- Other size-tied values: `Model/YBot` local y (−height/2) and scale (height / 1.88), shove range (`_range`, touching =
  2 × radius), camera `_distance`/`_height` on the `GameScene` camera (10 / 5.5), `CharacterController.stepOffset`.
  Rope/leash lengths, launch and run speeds, and jump height were tuned to the 5 m player and would feel different.
- Course alternative: scale the course down instead (gaps, gate and spinner heights, platform sizes), which keeps every
  tuned player value. Worth deciding before M3 (procedural chunks), since chunks will be authored at one scale.
- Unexplained: `PlayerPrefab`'s extra `CapsuleCollider` (non-trigger, alongside the `CharacterController`) is
  height **6.41** in the working copy vs 5 in git. Not changed by Claude; user said leave it as is for testing.

**C. Character: Mixamo Y Bot chosen as the default character (user, 2026-10-16). Committed `85d59ed`. ✅ User-tested
2026-10-30: "everything functioning well", animations fine.**
- Rope bug found in that test (2026-10-30): at spawn the slack partner rope hung in the air ending above the partner's
  head, on both peers, until a Tow/Slingshot pulled it straight. Cause: `RopeVisual.RestOnGround` skipped only the
  `CharacterController`, so slack points rested on top of `PlayerPrefab`'s extra root `CapsuleCollider` (6.41 m, top
  ~0.7 m above the head). Fix: skip any collider under a `CharacterController` (`GetComponentInParent`), which also
  covers future ragdoll limb colliders. ✅ User-confirmed 2026-10-30: rope now placed correctly at spawn.
- Files: `Assets/Characters/Mixamo/Y Bot.fbx` (Humanoid, own avatar), `Y Bot@Idle.fbx`, `Y Bot@Running.fbx` (Humanoid,
  copy Y Bot's avatar, loop, root locked = in place, 30 fps). The user downloaded them; they'd landed in the project
  root (outside `Assets/`, not imported) and were moved.
- `PlayerPrefab/Model` is still the interpolated visual (camera, rope attach point), now scale 1 with no capsule mesh.
  `Model/YBot`: Y Bot at the capsule's feet (local y −2.5), scale 2.67 (1.88 m → 5 m), Animator with
  `PlayerLocomotion.controller` (1D blend tree on `Speed`: Idle 0 → Running 7 m/s) and `PlayerAnimator.cs`, which
  sets `Speed` from the Model's measured ground speed on every peer (no networking) and plays faster above
  `_runAnimSpeed` (9 m/s, **a guess**: the clip's natural speed reads 0 with root motion locked; tune by foot slide).
- Collision unchanged: the 2.5 m-wide `CharacterController` capsule, wider than Y Bot. Consider a narrower radius later
  so shoves and bumps match the visual.
- `NetworkPlayer` team colour now tints the body renderers under `Model` (Y Bot's `Alpha_Surface`), not `Alpha_Joints`.
- `Assets/Scenes/CharacterTest.unity` + `Assets/Characters/Mixamo/Preview/*.controller`: preview scene (capsule vs idle
  vs running Y Bot). Can be deleted once the character is settled.
- Known: no jump/fall animation yet (runs or idles in the air); a moving platform's carry counts as running.
Test (restart the MPPM VP, the prefab changed): Y Bot in team colour, idles standing still, runs when moving, faster
when sprinting, feet on the ground, rope still attaches around the hips, camera unchanged.
**Next: ragdoll (`RagdollController`, GDD §5.2) on this skeleton, then Pull (Recover).**

**T. Game modes: host picks the mode in the lobby (user design 2026-10-16, replacing the same day's automatic
"3 players = FFA via a GREEN team" version). Committed `85d59ed`. ✅ User-tested 2026-10-30: modes work.**
- 2026-10-30, user feedback "dropdown way too small, options too grouped up": the default TMP option rows were 20 px
  with 26 pt text, so the options overlapped. `RebuildLobbyUI` now makes the dropdown 320×70 (matches START GAME), with
  64 px option rows, a 6 px gap and a small green marker on the selected mode. `LobbyScene` regenerated. ✅ User: "new UI looks good" (2026-10-30).
- `GameMode.cs`: `GameModeId` + `GameModes.All`, one entry per mode (name, team count, max team size, whether players
  choose teams). **Add a future mode by adding an entry.** `GameModes.Current` resets to the default (Teams) for each
  new lobby; not remembered after a race (agreed for now).
  - **Teams:** BLUE vs RED, max 2 each, auto-balanced, JOIN buttons to switch. 4 = 2v2, 3 = 2v1, 2 = 1v1 or partners.
  - **Free-for-all:** every player on their own team (colours BLUE/RED/GREEN/YELLOW), no JOIN buttons.
- Host: **Mode dropdown** left of START GAME (`LobbyManager.SetMode`, lobby only). Changing the mode reassigns every
  team (`NetworkPlayer.ReassignAllTeams`). The mode rides in the existing player-count named message, so clients
  (including late joiners) get it with the count. Everyone sees it after the count, e.g. "Players: 3 / 4 · Teams · 2v1".
- Game code never reads the mode, only teams (`Partner` / `IsTeammateOf`): FFA = all solo, so no partner rope, leash
  or Tow, and the results popup skips the team line.
- `NetworkPlayer.MaxTeams` 4 (was `TeamCount`). `LobbyUI`: team columns shown only in modes where players choose
  teams, otherwise `PlayerList` (players in their colours). `RebuildLobbyUI`: 2 columns, dropdown (text "v" arrow,
  the lobby font has no ▼), `PlayerList`.
- Lobby placeholder (2026-10-16, untested): players stand in a row 4 m apart by client slot
  (`PlayerRaceProgress._lobbySpacing`, `OwnerClientId % 4`) instead of all at the origin. Until a real lobby scene.
- Gotcha: the Editor marks `LobbyScene` dirty after every script reload with no real change (a saved copy was
  byte-identical to disk), so a "LobbyScene has unsaved changes" prompt is usually not real work.
Test (host + 2 VPs): default "Teams", columns shown, 3 players → 2v1; host switches to Free-for-all → columns hide,
coloured list, 3 different capsule/Y Bot colours; back to Teams → re-balanced; clients have no dropdown but see the
mode; in game, FFA players have no partner rope and the results show no team line.

**F. Final stretch built 2026-10-16 (GDD §5.6). ✅ All 6 test steps passed (user, host + MPPM, 2026-10-16). Committed `9e4a296`.** Feel still to judge with a real friend (backlog rows 11–13).
- `RaceState.FinalStretch` (between `Racing` and `Results`). **Global**: the first player into `FinalStretchZone`
  starts it for everyone (user decision 2026-10-16). Reported like a finish: owner trigger → `PlayerRaceProgress`
  ServerRpc → `RaceStateMachine.ReportFinalStretch`. Finishes are accepted during `FinalStretch`.
  `RaceStateMachine.RopesCut` is true from `FinalStretch` on.
- `FinalStretchZone.cs`: trigger placed in `GameScene` over the whole last platform (`Platform (4)`), backlog row 11.
- `PlayerTether`: once ropes are cut, any Slingshot/Tow ends (an anchored Slingshot does **not** fire), no new move
  starts, leash and Tow pull are off, its HUD hides. Every peer snaps its ropes locally (`SnapRope`), nothing new is
  networked. The rival rope remembers its last target so it can still snap if the Idle state arrives first.
- `RopeVisual.Snap/TickSnapped/Restore`: breaks at the middle, each half recoils toward its player (harder when
  taut), falls and fades over 1.5 s. The second half is a separate `LineRenderer` (`SnappedHalf`), since one line
  can't draw a gap. `Restore` re-joins when a new race starts. Edit-mode sim checked the split, finite values and
  restore; the fade can't advance in Edit Mode, so check it in play.
- Snap sound: `Assets/Audio/RopeSnap_TEMP.wav`, **synthesized in Python as a placeholder** because Coplay
  `generate_sfx` returned `401 Unauthorized` (check the Coplay account if a generated one is wanted). Played via a
  mostly-2D `AudioSource` (spatial 0.3) on the player, because a fully 3D one-shot at the rope's middle was near
  silent. Assigned on `PlayerPrefab` → `PlayerTether._snapSound`.
- `PlayerShove.cs` (new NetworkBehaviour on `PlayerPrefab`, so **restart the MPPM virtual player**): LMB after the
  ropes are cut (uses the previously unused `OnAttackPressed`) shoves the nearest player in a 60° cone within 5 m,
  teammates included. Knockback sent to the target's owner (`ShoveRpc`), same path as the Slingshot launch. HUD:
  "ROPES CUT!" banner for 2.5 s, then a shove cooldown bar.
- `RaceResultsPopup`: MVP tag on 1st place, rows in team colours, and the team result: best **average** place
  (same as combined places in 2v2, but fair in a 2v1), or "TEAMS DRAW".
- **Not done:** music cut (there's no music yet, M4). The Tow 54 m break was planned to reuse the snap, but dropped:
  the always-on partner rope just goes slack there, so a snap followed by the rope reappearing would look wrong.
- Saving the prefab also serialized the leash fields for the first time; values are the code defaults the user
  tested (36 / 0.5 / 4 / 1 / 3).
**User test 2026-10-16:** rope snap ✅, snap SFX on time ✅, Tow/Slingshot disabled after the snap ✅, Shove works on
teammates and rivals ✅. Later the same day: steps 2 (move in progress cancels), 5 (results popup) and 6 (rival rope snap) ✅.
The user questioned shoving teammates ("isn't that a bit contradicting?"). It's by design (GDD pillar 3 "every round
ends in a betrayal", and the §5.6 MVP/team scoring split exists to reward it). Kept for now; judge with a real friend
(backlog row 12). Fallbacks if it feels bad: rivals only, or a weaker teammate shove.
Test (host + 1 VP, **same team**, VP restarted; press F9 to skip to the end, ✅ confirmed working 2026-10-16):
1. Race to the last platform: banner shows, both ropes snap on **both** screens with the sound, halves fade out.
2. Hold RMB (Tow) or LMB (Slingshot anchor) just before landing on the last platform: it ends cleanly, no launch.
3. After the snap: LMB and RMB start no rope move; walking away from your partner isn't slowed or dragged.
4. LMB near the VP: they're knocked back on their screen; cooldown bar on yours. LMB with nobody near: nothing.
5. Both finish: popup shows MVP and "BLUE/RED TEAM WINS" (or a draw on the same team).
6. Different teams: a Slingshot anchored on the rival at the moment of the snap also snaps.

**0. Heavy-drag leash implemented 2026-10-12. User tested it on one machine and said it "feels good now". Committed `da4ed74`.** Leash length is still to be found by playtesting (backlog row 1). It's the parked "soft leash" idea in a
stronger form; the user chose it as better for the concept (cooperation enforced, and it gives the final-stretch snap
its weight). How it works (`PlayerTether.UpdatePartnerPull`, each player's own machine, nothing new networked):
- Teammates further apart than `_leashLength` (36 m, separate from slingshot range): the input-driven part of moving
  **away** from your partner is scaled to 50% (`SyncRushPlayerController.LeashAwayDir/LeashAwayFactor`). Moving toward
  them or sideways is unaffected. It ramps in over 4 m so it isn't a wall.
- When your partner moves away from you past the leash, you're **dragged after them** along their path (Tow's
  breadcrumbs) at their speed away from you. Their velocity is estimated from replicated positions.
- **Off** during a Slingshot/Tow between the pair, while flying from a launch (`IsLaunched`: knockback, or airborne
  with >5 m/s outside velocity, so slingshots and pads still gain ground), and for 3 s after either player teleports
  more than 8 m in a tick (respawn).
- The partner rope goes fully tense exactly at the leash length, so you see when the drag starts.
- Tow now shares the partner path tracking (`RecordTrail` / `PullAlongTrail`); its behaviour is unchanged.
Test (same team, VP on F8 auto-run as the partner who isn't moving / walking away): walk away → slows at 36 m and the
partner is dragged; walk back → normal speed; Slingshot your partner past 36 m → no slowdown in flight; fall and
respawn → no yank for 3 s.

**1. Test pass on the 2026-10-07 work: done (2026-10-12, host + MPPM).** That work was committed in `d749c2c`.

| Change | Tested? |
|---|---|
| Lobby COPY button + fix for being stuck on the Create Lobby panel | ✅ Play Mode and user |
| Aim targeting (pick the player nearest screen centre, 25° cone) | ✅ User, 3 players |
| Range preview line + distance marker (green in range, amber out of range) | ✅ User |
| HUD labels renamed "Tether" → "Slingshot" | ✅ User |
| Rope visuals: sag that straightens with tension, white→red tint | ✅ User screenshot from the partner's (client) view, so rope state replicates. Past-36 m look (now rigid + thinning, not shake) ✅ user 2026-10-30 |

**Fixed 2026-10-12 (since committed):** the rope was invisible in the first test. It attaches at capsule centre (2.5 m up)
and sagged up to 4 m, so its middle was buried in the floor. Sag is now capped at 12% of rope length (`_sagPerMetre`)
and kept 0.4 m above the ground under the rope's midpoint (`_groundClearance`, a downward raycast, so it can
still hang into gaps). Also, a player being held on someone's rope no longer draws their own range preview, which
overlapped the rope. The preview-hiding change compiles but hasn't been seen in play yet.

**2. Tow implemented 2026-10-12 (committed `35c169c`). It works on one machine, and feel is deferred.** The user tested drag, strain
and snap with host + MPPM, including an auto-running partner (F8). Verdict: it works as intended, but the pull still
feels a bit weak. They chose to keep the current values (`_towSpeedFraction` 1.5, full pull at 20 m) and judge feel in
a live two-player test, since auto-run can't show a human steering or real latency. If it still feels weak then,
consider whether Tow should be more than a catch-up tool (e.g. a water-ski style burst that boosts the leader too).
See the `TetherSystem, Tow` row in the M1 table.
Test with host + 1 MPPM virtual player (restart it first; the input asset changed):
- Partner stands still, host holds RMB and runs: partner is dragged along the host's path, held taut at ~33 m
  (after the 0.6 → 1.1 tuning below).
- Host runs around an obstacle: partner follows the path, not a straight line.
- Both run: partner catches up, then settles ~10 m behind.
If following the path plays badly, the agreed fallback is a straight-line pull toward the leader.

**Tow tuning after first test (2026-10-12):** the user confirmed the partner is dragged and the rope strains past
36 m, but the pull felt weak on a partner who isn't moving. `_towSpeedFraction` 0.6 → 1.1, so a still partner is held taut at
~33 m instead of falling behind and snapping. The strain shake (since replaced by a rigid, thinning rope, see item 4) now builds from max length to the snap point:
shake 15%→100% of `_strainShake` (now 1 m), shake speed 0.6×→1.5×, width up toward `_strainedRopeWidth`, colour
red → dark red (`_snapColor`). **Second tuning pass (same day):** still weak with the partner auto-running. Cause: a running partner (7 m/s) already
outruns a towing leader (~6 m/s), and the pull ramped all the way to 36 m, so it faded as they closed in. Now the pull
reaches full at 20 m (`_towFullPullLength`) and `_towSpeedFraction` is 1.5: a still partner is held at ~17 m, and a running
partner closes 36 → 10 m in ~3–4 s. The rope also turns fully red at 20 m during a tow now. **TEMP DEBUG:** F8 toggles auto-run forward on the local player
(`SyncRushPlayerController._debugAutoRun`, shows `AUTO-RUN (F8)` top-left), so a virtual player can keep running
while being towed on one machine. Remove it once two-machine testing exists.

**3. Teams implemented 2026-10-12 (committed `35a45d8`), since user-tested.** See `PlayerRegistry` in the Foundation table. Also:
lobby team columns (`LobbyUI._teamButtons/_teamLists`, built by `RebuildLobbyUI`, so `LobbyScene` was regenerated),
and rope targeting now prefers your partner. Slingshot: aimed, else partner, else nearest. Tow: partner, else nearest.
Saving the prefab serialized every `PlayerTether` field, so tune Tow/rope values on the prefab now, not via code defaults.
Test (restart the MPPM virtual player, because the prefab gained a NetworkBehaviour):
- Host + 1 VP: auto-assigned to different teams (blue / red capsules, lobby columns).
- Switch the VP to BLUE: both columns and the capsule colour update on both screens; the full team's button greys out.
- In game: teams can't change. On different teams, RMB does nothing (Tow is teammates only). On the same team, RMB tows.
- Tow: partner runs past you while towed → pull stops (no drag-back).
- Slingshot a rival → 15 s cooldown bar; a teammate → 6 s.

**User test 2026-10-12: teams, teammates-only Tow and the rival cooldown all work.** The rival cooldown was raised
12 → 15 s afterwards because 12 s still felt abusable (not re-tested; it's a single value).

**Rope rules decided 2026-10-12 (after the user found they could still rope the other team, which is intended per
GDD §5.4):** (1) **Tow is teammates only.** It's a catch-up move, so with no partner (duel, FFA, other team) RMB does
nothing. This deviates from GDD §5.4's "the tether can target any player"; Slingshot keeps that rule. (2) **Tow only
pulls while the partner is behind the leader**, measured against the leader's run direction (from the last two path
points), not facing, so glancing back doesn't cut it. Before this, a towed player ahead of the leader was dragged
backward, which with rivals was a strong grief. (3) **Slingshot cooldown on a rival is 15 s** (`_rivalCooldown`, first 12 s), 6 s
on a teammate, because slingshotting a rival is mainly sabotage (launching them into hazards).

**4. Always-on partner rope + simulated rope implemented 2026-10-12 (committed `da4ed74`).**
- `Assets/Scripts/RopeVisual.cs`: a visual-only verlet rope with 24 points (`_ropeSegments`) and gravity. Constraints only
  pull segments together, so slack piles up. Points rest on the ground via a short raycast (players' capsules ignored),
  with friction. Rope length adapts like a retractable leash: slack = max(gap × 1.2, gap + 3 m), capped at 36 m, and
  pulled in to the gap as tension → 1. Gravity fades with tension, so taut rope is straight. If an end jumps more than
  8 m in a frame (respawn), the rope resets. Each peer simulates its own copy; nothing is networked.
- `PlayerTether`: teammates are roped together the whole time in `GameScene` (GDD §5.3 `Idle`). The lower
  NetworkObjectId of the pair draws it. Slingshot/Tow between partners feed tension into that same rope, so release
  lets the slack back out and it whips. A Slingshot on a rival uses a separate `RivalRope`, shown only while
  anchored. The old fixed sag curve (`DrawLine`, `ClampSag`, `_maxSag`, `_sagPerMetre`, `_groundClearance`) was removed. The
  range preview is now its own straight 2-point line.
- **Rope is black when slack** (user request, for visibility on the light floor), red under tension. **Past 36 m it no
  longer wobbles** (user request after first look: it should read as "extremely tense"). It's rigidly straight, thinning to
  60% width (`_snapWidthFactor`) and darkening to dark red at the 54 m snap point. `_strainShake`/`_strainedRopeWidth` were removed. `_slackColor` is set to black on the prefab.
- Checked in the Editor with a scripted sim run over a floor: values stay finite, slack rope rests on the floor, an
  overstretched rope is straight, and a teleport resets cleanly. Not yet seen in Play Mode.
- **Deferred:** the final-stretch SNAP (no `FinalStretch` race state yet; I didn't add a `Snap()` stub, since it's easy
  to add to RopeVisual when needed) and the soft leash (since built as the heavy-drag leash, item 0). Ropes attach at capsule centre until there's a real character.
Test (host + VP on the **same** team): rope lies on the ground when you're close; swings and trails when
running/jumping; drapes off platform edges; taut and shaking past 36 m; slingshotting your partner tightens it,
then it whips; respawning resets it; on different teams there's no rope until you Slingshot.

**Design decisions 2026-10-12:** no separate selectable modes for now. Mode follows player count as GDD §5.4 says,
and 2v2 is the focus. A free-for-all would be "every player on their own team" if playtests want it later.
Teams: auto-assign plus a manual switch, as above.

**Feedback from the test:** the user expected the rope to behave like real rope physics. The current rope is a
fixed tension-driven curve, which reads as fairly subtle. That makes the "Rope physics" option in item 2 more
relevant than first thought.

**Agreed order (2026-10-12):** Tow first (M1 fun question), then teams + always-on `Idle` rope + visual rope
physics together, since they depend on each other (an always-visible rope needs to drape and swing to look right).

**2. ~~Pick the next feature~~:** done (Tow and the simulated rope were both built 2026-10-12).

**3. ~~Decision blocking ragdoll + Pull (Recover)~~:** decided 2026-10-16: Mixamo Y Bot (see "Start here" item C).

**4. Still deferred:**
- **Two-machine Relay test** (no second machine yet). It's also needed for the M1 kill gate: playtest the tether with a real friend.
- ~~**Parked soft-leash idea**~~: built 2026-10-12 as the heavy-drag leash (item 0).

**Known quirks:** a one-off Multiplayer Tools `Ngo1Adapter` error can leave `NetworkManager` disabled. Restart Play Mode
if Start Lobby says "Something went wrong connecting". `[PushDebug]` logging is still on in `SyncRushPlayerController`
(`_debugLogPushes`). Remove it once the pushing issues are confirmed gone.

---
## Milestone status (GDD §9)

| # | Deliverable | Status | Notes |
|---|---|---|---|
| **M0** | Race state machine, checkpoints, placement, greybox straight course | 🟨 In progress | Greybox straight course built by hand in ProBuilder (`GameScene`: `Platform`, `Starting`/`Checkpoint`/`Finish` pole gates, `Deathzone`). `RaceStateMachine`, `RaceCheckpoint`, `PlayerRaceProgress`, `DeathZone`, `SpawnPointRegistry` written and wired onto those objects. **2026-09-04:** first real two-client test (host + Multiplayer Play Mode virtual player) — host/join/countdown/GO all fired correctly and remote player movement replicated well on both sides. Since then, every M0 system has been verified with host + MPPM on one machine. The only M0 item left is the two-machine Relay test, deferred 2026-10-07 because no second machine is available |
| **M1** | Tether + ragdoll-on-impact, 4 players over Relay | 🟨 In progress | Slingshot tether works on one machine (user playtest 2026-10-07, see M1 section). Tow works on one machine (2026-10-12), feel deferred to a live test. Teams (2026-10-12, user-tested), the always-on simulated partner rope and the heavy-drag leash are built. Final stretch (rope SNAP, Shove, MVP/team result) built and user-tested on one machine 2026-10-16. Mixamo Y Bot is the character (2026-10-16), host-chosen game modes (Teams / Free-for-all) built. Not built yet: ragdoll (on the Y Bot skeleton), Recover |
| **M2** | Darts, status effects, ammo economy | ⬜ Not started | |
| **M3** | Course generator, 15 chunks | ⬜ Not started | |
| **M4** | Presentation pass | ⬜ Not started | |
| **M5** | Proximity voice, MVP cinematic, bots, 2–4 scaling | ⬜ Not started | |

## Foundation (GDD §7.1) — pre-M0 groundwork already underway

| Piece | Status | Where |
|---|---|---|
| Join-by-code host/client session (`ConnectionManager` equivalent) | 🟨 Implemented over Unity Relay (`576fa1f`), not yet tested across two machines | `Assets/Scripts/LobbyManager.cs` — anonymous UGS sign-in, host creates a Relay allocation (`MaxPlayers - 1` = 3 slots, `dtls`), and Relay generates the 6-char join code (the host no longer types one; `LobbyUI`'s host code input was removed). Clients join via `JoinAllocationAsync`. Async Host/Join show `CREATING...`/`CONNECTING...` and can be cancelled (Back) through an attempt-ID guard. Each MPPM virtual player gets its own auth profile (`vp_<id>`) so it doesn't sign in as the main Editor. Relay errors are mapped to user-facing messages (bad code, Relay not enabled on the dashboard, services unreachable). `PlayerCount` is still synced via named message. Requires Relay enabled for the project in the Unity Cloud dashboard |
| Lobby ↔ Game scene split | ✅ Implemented | `Assets/Scenes/LobbyScene.unity`, `Assets/Scenes/GameScene.unity` (commit `ab0fc98`) |
| Lobby UI | ✅ Implemented | `Assets/Scripts/LobbyUI.cs` (+ `Assets/Editor/RebuildLobbyUI.cs`) |
| Owner-simulated movement (`CharacterController`) | ✅ Implemented, GDD values wired in Inspector | `Assets/Scripts/SyncRushPlayerController.cs` — base 7 m/s, sprint 10 m/s, air control 0.4, jump 1.8 m, coyote 0.12 s, stamina drain/recover |
| `PlayerInputReader` snapshot | ✅ Implemented | `Assets/Scripts/PlayerInputReader.cs`, `Assets/Scripts/InputSystem_Actions.cs` |
| Camera controller | ✅ Implemented, reworked 2026-09-23 | `Assets/Scripts/PlayerCameraController.cs` — mouse X yaws the player, mouse/stick Y now pitches the camera (see 2026-09-23 section) |
| `PlayerRegistry` / team ID / placement / dart count | 🟨 Team ID implemented 2026-10-12, untested; placement/darts not yet | `Assets/Scripts/NetworkPlayer.cs` on `PlayerPrefab` (replaces the Multiplayer Center sample `SetColorBasedOnOwnerId` on `Model`, which coloured capsules randomly by client ID). A server-written `TeamId` NetworkVariable: two teams, BLUE (0) and RED (1), max 2 each. **Rules agreed with the user:** the server auto-assigns each joiner to the smaller team; in the lobby, JOIN BLUE / JOIN RED buttons ask the server to switch (`RequestTeamRpc`, refused if full); teams lock once in `GameScene`. Capsules take the team colour. Code asks only `Partner` / `IsTeammateOf`, never fixed team numbers, so a later free-for-all can put each player on their own team. **Modes (2026-10-16):** the host picks Teams or Free-for-all in the lobby (`GameModes`, see "Start here" item T). Buttons stay on at every count so 2 players on one machine can still test as partners |
| `SpawnPointRegistry` | ⬜ Not yet | |

## M0 systems (GDD §7.2, this milestone's slice)

| System | Status | Where |
|---|---|---|
| `RaceStateMachine` | ✅ Verified on one machine, host + MPPM (user-reported, by 2026-10-07) | Full `Racing → Results` run with every player finishing reported working. | `Assets/Scripts/RaceStateMachine.cs` — server-authoritative `WaitingToStart → Countdown → Racing → FinalStretch → Results`. `FinalStretch` added 2026-10-16 (see "Start here" item F) |
| `RaceCheckpoint` | ✅ Verified, host + MPPM virtual client (2026-09-23) | `Assets/Scripts/RaceCheckpoint.cs` — trigger gate with `Kind`/`Order`; gates in `GameScene` run `Starting`(0) → `Checkpoint`(1) → `Checkpoint2`(2) → `Finish`(3). `Checkpoint3`–`Checkpoint6` were removed in `8b23a84` (duplicates); `Finish` was left at order 7, so finishing was silently rejected by the anti-shortcut check until it was renumbered to 3 on 2026-10-02. Crossing in order confirmed normal. **Not tried over a real remote connection** (Multiplayer Play Mode's virtual client is same-machine, no real network latency) |
| `PlayerRaceProgress` | ✅ Verified on one machine, host + MPPM (user-reported, by 2026-10-07) | `Assets/Scripts/PlayerRaceProgress.cs`, added to `PlayerPrefab` — owner-writable checkpoint order (anti-shortcut: rejects out-of-sequence crossings), spawn/respawn placement, reports finish to `RaceStateMachine` via ServerRpc. The user reports the out-of-order rejection working when tested deliberately |
| `DeathZone` | ✅ Verified single-player and non-host client via MPPM (2026-09-23) | `Assets/Scripts/DeathZone.cs`, attached to `Deathzone` (now a trigger) — respawns the player at their last checkpoint. Confirmed for the non-host client using the Multiplayer Play Mode virtual client. **Not confirmed over an actual remote/online connection** — MPPM's virtual client shares the host's machine, so real latency/replication timing hasn't been exercised |
| `SpawnPointRegistry` | ✅ Verified on one machine (user, 2026-10-30) | `Assets/Scripts/SpawnPointRegistry.cs`, attached to `SpawnPoints` in `GameScene`; reads its own children (`Spawn_0`–`Spawn_3`) as spawn points, assigned per client by `OwnerClientId % count` |

**Fixed during playtesting (2026-09-02):** respawn point was reading `checkpoint.transform.position` directly, but each gate's own transform pivot isn't at its visual/trigger center (`Checkpoint`'s pivot was at world x≈-16.17, past the Finish line) — falling near the checkpoint respawned the player behind Finish. `RaceCheckpoint` now carries an explicit `_respawnPoint` world-space field set to each gate's real center instead of trusting the transform.

**Fixed and verified (2026-09-04):** `PlayerPrefab`'s `ClientNetworkTransform` had `AuthorityMode` set to `Server` (0), which conflicted with the owner-authoritative movement model `SyncRushPlayerController` and the GDD (§7.3) both assume — non-owner clients would not have seen a remote player's position replicate correctly. Set to `Owner` (1). Confirmed working in a live host + Multiplayer Play Mode two-client test the same day: both sides saw remote movement update smoothly.

**Testing gotcha (2026-09-04):** editing a prefab from outside the Editor (e.g. a direct file edit) while a Multiplayer Play Mode virtual player is already running can leave that VP instance's `AssetDatabase` with a stale/null import for the changed asset, even though the file on disk is correct — it showed up as a spurious `[Netcode] NetworkConfig mismatch` on connect. Fix was to touch/resave the file so the VP instance's file watcher picked up the change and reimported it. If this recurs, restart the Multiplayer Play Mode virtual player rather than debugging the network code.

**Next up (2026-09-23):** checkpoint crossing in order and respawn-to-last-checkpoint both confirmed for the non-host client via Multiplayer Play Mode's virtual client. Still open:
- ~~**Anti-shortcut rejection**~~ — **verified** (user-reported, last session before 2026-10-07).
- ~~**Finish/placement reporting**~~ — **verified 2026-10-02** (host + MPPM virtual client): after renumbering `Finish` to order 3, the host logged `Client 1 finished in place 1.` ~~`Racing → Results` once every player finishes~~ — **verified** (user-reported, last session before 2026-10-07).
- **Real remote play — ⏸ DEFERRED (2026-10-07):** no second machine available for now. M0 work continues without it, but M0 isn't closed until this passes. If the open items here keep piling up, prioritise this test. everything so far is host + MPPM virtual client on one machine, which has no real network latency and shares the same `Application.dataPath`/asset state as the host (see the 2026-09-04 testing gotcha above). None of this has been exercised over an actual remote connection (e.g. two machines over Relay, or even two machines on direct connect) — that's the remaining gap before calling M0's kill gate closed. Relay is now implemented (`576fa1f`, see the Foundation table above), so the next step is a two-machine Relay session that runs the full race: checkpoints, respawn, hazards and finish.

**Course expanded (2026-09-16):** the greybox course now has obstacle variety past the `RotationPlat` gauntlet instead of running straight into `Finish`. `Start`, `Platform`, `Platform (1)`/Pendulums, `Checkpoint`/`Platform (3)`, and the gauntlet itself are unchanged. `Platform (2)` is disabled (`MeshRenderer`/`MeshCollider` `enabled=false` — not deleted, reversible) since discrete tiles now serve as the floor instead. Sequence from the gauntlet exit (x≈-154.85), all tiles sized to match `RotationPlat`'s own proportions (12×0.25×5) with its ~2.2m gap convention:

- **Launch pad tiles ×2** (`Tile_LaunchPad_1-2`, `Bounce.cs`) — single lane (z=7), x=-166/-180.2
- **`Checkpoint3`** (`_order=3`, renumbered 2026-09-18) — rest platform + gate, x=-193.4
- **Spin bar tiles ×2** (`Tile_SpinBar_A/B`, 2 parallel lanes z=7/-8) — x=-206.6
- **`Checkpoint4`** (`_order=4`, was 3) — x=-219.8
- **Hill** (continuous ramp/plateau/ramp, spans both lanes) — x=-229 to -237
- **`Checkpoint5`** (`_order=5`, was 4) — x=-246.2
- **Sliding pillar tiles ×2** (2 parallel lanes) — x=-259.4
- **`Checkpoint6`** (`_order=6`, was 5) — x=-272.6
- **Timing gate tiles ×2** (2 parallel lanes) — x=-285.8
- **`Finish`** (`_order` bumped 2→6, then 6→7 when the hand-placed big `Checkpoint2` gate at x≈-166.6 became order 2 with its own respawn point) — x=-304; `Deathzone` extended (was to x=-258.43, now to -334) to cover it

`Bounce.cs`/`Rotator.cs`/`MovableObs.cs`/`WallMovable.cs` (originally from `Assets/ObstacleCoursePack/`; as of 2026-09-23 the scripts live in `Assets/Scripts/` and the prefabs in `Assets/Prefabs/Obstacles/`) were rewritten from the pack's originals to work against `CharacterController` and this project's netcode/physics setup — each needs a kinematic `Rigidbody` (Unity only raises trigger/collision callbacks when at least one side of an overlap has one, and `CharacterController` has none) and `Rigidbody.Move*` rather than raw `transform` writes (`Physics.autoSyncTransforms` is disabled here). `Bounce` also moved from `OnCollisionEnter` (never fires against `CharacterController`) to a trigger + `SyncRushPlayerController.AddImpulse`. `RotationPlat` gauntlet `speed` reduced 60→40 after measuring via a scripted `CharacterController` rig that the real safe crossing window was ~0.62s/3s cycle.

Verified live in Play Mode and by screenshot; **not yet verified by the user in-editor, and the layout still doesn't fully match what they're picturing** — kept for now per their call, but expect further changes here. Also not yet tested: real checkpoint/anti-shortcut interaction against the new mid-course checkpoints or the moved `Finish`, and none of it has been tried in a multi-client session.

## Playtest fixes and tuning (2026-09-23)

All confirmed in single-player playtesting per the user (2026-09-23). **Not yet tried over a real remote connection** — a moving-platform carry, a launch/knockback, or the camera's player-collider exclusion could all behave differently with actual network latency (owner-authoritative movement, replication timing); the Multiplayer Play Mode virtual client used for the checkpoint/respawn checks above doesn't exercise that.

**Player (`SyncRushPlayerController.cs`)**
- Moving platforms: player is carried by a kinematic platform while its surface is within 15° of level, and slides down it at 6 m/s when tilted (`_platformCarryAngle`, `_platformSlideSpeed`). `CharacterController` doesn't ride kinematic colliders on its own.
- `Launch(velocity)` sets horizontal knockback and takes the max of current/new upward speed, so overlapping pads resolve to the stronger one instead of stacking. `AddImpulse` still adds (pendulum). Used by `Bounce` and `SpinnerHazard`.
- Jitter fix: the `Model` child is drawn at `InterpolatedPosition` in `LateUpdate`, so the mesh and camera share one smooth path (the root only moves on 50 Hz physics ticks). Interpolation restarts after a teleport (respawn).
- **TEMP DEBUG still on:** `[PushDebug]` logging (`_debugLogPushes = true`) — remove once pushing issues are confirmed gone.

**Camera (`PlayerCameraController.cs`)**
- Pitch: mouse Y / right stick Y tilts the camera around a pivot at the player (`_minPitch -30°`, `_maxPitch 70°`, `_invertY`). Resting view unchanged.
- Yaw is taken straight from the player (was smoothed, which low-passed fast swipes so the camera lagged/stalled). Only the follow anchor is still smoothed.
- Collision probe ignores the player's own colliders; previously sprinting + turning cast through the player's body and collapsed the camera.
- Known, not fixed: mouse vs gamepad is guessed from input size (`sqrMagnitude <= 1.01`), so 1-pixel mouse moves get gamepad sensitivity.

**`RotationX` spinner (`SpinnerHazard.cs`, `RotationX.prefab`)**
- Hit on the bar launches the player (25 m/s sideways + 7 up), once per 0.5 s per player. Bar has solid colliders (can't be walked through) plus trigger boxes for the hit.
- Trigger boxes are re-fitted at runtime to the solid collider plus a fixed 0.3 m margin (`_contactMargin`), so they no longer balloon with object scale (the old local-unit sizes had become metres of overreach on the scaled scene instance).
- Scene instance in `GameScene`: both arms 0.4 m thick and ~84.6 m long (scale overrides on the instance and its child arm), so the top is far narrower than the player and hard to stand on.

**`Bounce` pad**
- Active `Bounce` script is on the child pad (outer object's script is disabled to avoid double launches). `force` raised 27 → 30.8 (~1.3× height; apex ≈ force² / 40 at gravity 20, ≈ 24 m).

**Housekeeping**
- Obstacle scripts/prefabs moved out of `Assets/ObstacleCoursePack/` — since committed, with their `.meta` files.

## M1 systems (GDD §7.2)

| System | Status | Where |
|---|---|---|
| `TetherSystem`, slingshot slice | ✅ Works on one machine, host and client each anchoring (user playtest 2026-10-07, host + MPPM); not yet fun-tested with a friend (M1 kill gate) or over real Relay | `Assets/Scripts/PlayerTether.cs`, on `PlayerPrefab`. Hold Attack (LMB) to anchor to a player within 36 m. **Aim targeting (2026-10-07, untested):** picks the player closest to screen centre within 25° (`_aimAngle`), else the nearest in range, and a green `▼ TETHER` marker above their head shows who a press would pick (owner-only OnGUI). **Range preview (2026-10-07, untested):** added after the user found it hard to judge tether range without a rope. While idle, the owner sees a thin line to the aimed player and their distance on the marker: green `▼ SLINGSHOT 24m` in range, amber `▼ 52m / 36m` out to 2× range (was grey, which vanished against the light grey floor; preview width 0.08→0.15, 85% opacity) (`_previewRangeFactor`). **Naming (2026-10-07):** on-screen text says "Slingshot" (the move) instead of "Tether", which the user found confusing. Planned player-facing names: Rope (the system), Slingshot, Tow, Pull (for Recover). Code and GDD keep "Tether" (`PlayerTether`, `TetherState`) (tripled from the GDD's 12 m on 2026-10-07 after playtest: 12 m needed players nearly touching at this game's scale), release to fire. While anchored, the anchor's move and jump input are locked (`SyncRushPlayerController.InputLocked`). Tension moves toward `distance / 36 m` at 1/1.2 s. On release the partner launches toward the anchor at `lerp(12, 40, tension)` m/s plus 14 m/s lift (raised 2026-10-07 from the GDD's 6–22 + 5 after the user found the launch barely noticeable: the player is 5 m tall, 2.5× human scale. Lift then went 10 → 14 so the partner overshoots the anchor, ~56 m flight from a full 36 m stretch, instead of landing on them: the slingshot should gain ground, not just reel the partner in). **User playtest 2026-10-07: this tuning feels right, like a proper slingshot rather than a pull.** For 1 s after launch the partner ignores collisions with the anchor's colliders (`_passThroughTime`), because the launch aims straight at the anchor and was stopping dead against them. The launch goes through `Knockback` with no stun, so they keep steering and the launch holds until landing. The anchor owns the state (`NetworkVariable`s) and sends the launch to the partner's owner as `[Rpc(SendTo.Owner)]`. The rope is a `LineRenderer` (`Assets/Materials/TetherRope.mat`) that tints white→red with tension. **Rope visuals pass (2026-10-07, untested):** it's drawn as a 24-point curve that droops up to 4 m at the middle with no tension (`_maxSag`) and straightens as tension builds. Past 36 m it thickens and shakes sideways (`_strainShake` 0.35 m). Widths went up for the 5 m-tall player: rope 0.08→0.2, strained 0.18→0.35, preview 0.05→0.15. The curve is computed locally on each peer from the replicated tension, with no extra network traffic. The preview line stays straight and thin so it never reads as a real rope. A TEMP OnGUI HUD at bottom centre (owner only) shows tension % while anchoring, otherwise a cooldown bar for each partner still cooling down, or `SLINGSHOT READY`. It's meant to be replaced in M4. **Deviations:** the 6 s cooldown is tracked per anchor→partner direction (so B can still anchor A straight after A fires B); releasing under 0.1 tension cancels with no launch or cooldown; with no teams yet, the nearest-player fallback stands in for GDD §5.4's "partner by default". |

| `TetherSystem`, Tow | 🟨 Works on one machine (host + MPPM, 2026-10-12). Feel judged "a bit weak" and deferred to a live two-player test | `Assets/Scripts/PlayerTether.cs` (`TetherState.Towing`). Hold **Tow** (new input action: RMB / gamepad left shoulder) to hook the nearest free player within 36 m, ignoring aim. The leader runs at 85% speed (`SyncRushPlayerController.SpeedMultiplier`). The towed player's own machine records the leader's replicated position as breadcrumbs every 1 m and adds `TowVelocity` toward the oldest point not yet reached or passed: `leaderSpeed × 0.6 × pull`, where pull ramps 0→1 from 10 m (`_towSlackLength`) to 36 m and holds at 1 until it snaps at 54 m (`_towBreakLength`). It's horizontal only, so the towed player keeps full control and jumps gaps themselves. Replication: the leader owns `Towing`/partner state plus a new `_leaderSpeed` NetworkVariable (from `SyncRushPlayerController.HorizontalSpeed`), with no per-tick RPCs. `_tension` doubles as tow pull, so the rope sags and tints the same way. HUD: `TOWING Player N` / `TOWED BY Player N` in blue. **Rule added to both moves:** you can't hook a player who is anchoring, towing or already on a rope, and you can't start a move while you're on someone's rope. **Input:** `InputSystem_Actions.inputactions.meta` now sets `wrapperCodePath` to `Assets/Scripts/InputSystem_Actions.cs`. It was blank, which would have generated a duplicate class next to the asset on reimport |

**Design note, tether vs leash (2026-10-07; the soft leash was built 2026-10-12 as the heavy-drag leash, see "Start here" item 0):** the user asked whether the rope should just keep partners together, as a leash. Decision: keep the GDD §5.3 action tether (Slingshot now, Tow/Recover later), not a physics leash. A hard leash mostly punishes, it's the GDD's top netcode risk (rubber-banding across two owners), and it undercuts the final-stretch betrayal. Open gap: with an optional tether, the pitch's "cooperation is mechanically enforced" is weak, since a strong player can ignore their partner. **Parked idea, not built:** an always-on partner rope (the GDD's `Idle` state) with a soft leash. When it's stretched past max length, the player out in front slows (e.g. −20% past 36 m). Each player only slows themselves, so it stays netcode-safe with no yank. Revisit if playtests show players ignoring their partner or the rope feeling optional, or once teams exist (2v2).

**Deathzone expanded (2026-10-07):** the trigger was a 1 m slab ending ~30–40 m past the course, and big launches carried players past its edge. Only the `BoxCollider` changed: local size `(4, 40, 8)`, center `(0, -19.5, 0)`, so world bounds are now x -1054..866, z ±624, y -19..21.2. The top surface and visible slab are unchanged.

**Lobby: COPY button + stuck-on-Create-Lobby fix (2026-10-07, verified in Play Mode and confirmed by the user):** the waiting panel has a COPY button that puts the code on the clipboard (`LobbyUI`, built by `RebuildLobbyUI`). Rebuilding the canvas changed the scene's object order and exposed a latent bug: `LobbyUI` subscribed to `LobbyManager` events only in `OnEnable`, and skipped that if `LobbyManager.Instance` wasn't set yet. Start Lobby then succeeded but the UI never left the Host panel. It now also subscribes in `Start`, which runs after every `Awake`. One-off seen while testing: a `NullReferenceException` in Multiplayer Tools' `Ngo1Adapter` during `NetworkManager.Awake` left the `NetworkManager` disabled for that run, so the host failed with "Something went wrong connecting". It didn't happen on restart. If it recurs, restart Play Mode.

## Relay + hazard physics pass (2026-10-02, commit `576fa1f`)

Read from the code. Nothing here is recorded as playtested, except the Finish renumber, which has its own entry above.

- **Unity Relay**, replacing the direct connection (see the Foundation table).
- **Hit-stun knockback.** `SyncRushPlayerController.Knockback(velocity, stunTime)`: no movement input during the stun, and knockback doesn't decay while airborne until the player lands. `PendulumHazard` and `SpinnerHazard` use it (0.6 s stun). Bounce pads keep plain `Launch`, so players can still steer. It overwrites rather than adds, so a hit registered by two colliders no longer counts double. Owner-only.
- **`SpinnerHazard`**: changed from `OnTriggerEnter` to `OnTriggerStay`, so a player still inside the hit zone gets hit again once the lockout ends. Knockback is now at least 1.3× the bar's tip speed (`_outrunFactor`), so the bar can't catch up and "carry" the player. Spinners are also excluded from moving-platform carry.
- **Footing.** Grounded now requires a contact surface within the `CharacterController` slope limit, or a downward raycast at a ledge edge. Before this, players could jump off the near-vertical side of a flipping `RotationPlat`.
- **`RaceResultsPopup`**, a TEMP OnGUI debug popup on `RaceManager` that shows placements when the race reaches `Results`. `RaceStateMachine` gained `FinishCount`/`GetFinisher(i)` for it. To be replaced in M4.
- `Finish` renumbered 7 → 3.

## Remaining core systems (GDD §7.2) — not started

`TetherSystem`, `RagdollController`, `CourseGenerator`, `StatusEffectSystem`,
`DartProjectile`, `ProximityVoice`, `BotRacer`, presentation layer (cel shader,
impact frames, SFX text) — none present in `Assets/Scripts` as of last verification.

---
## Reading this file
- ✅ = implemented and roughly matches the GDD spec
- 🟨 = started / partial, needs follow-up
- ⬜ = not started
- When you finish a system, move it here with the file path and a one-line note on
  any deviation from the GDD spec (deviations are expected and fine — just record
  them so the GDD and the code don't silently diverge).
