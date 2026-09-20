# Magic Leap VAC Experiment — Group 6

Unity starter framework for the Group 6 experiment on **Vergence–Accommodation Conflict (VAC)** in optical see-through AR.

## Current scope

This branch implements the experiment core, not visual polish:

- participant-ID based counterbalancing;
- Low VAC / High VAC distances exposed as Unity Inspector parameters;
- constant apparent-size scaling for the Tetris board;
- stationary Tetris task;
- deterministic Sequence A / Sequence B;
- Tetris performance logging;
- stereoscopic depth-judgment task;
- balanced Left/Right closer-target randomization;
- four formal pre/post depth tests per participant;
- Training Mode;
- CSV export;
- pauses for questionnaires completed **outside the headset**.

## Headset model is still TBD

The exact Magic Leap model has not yet been confirmed, so the code intentionally does not hard-code Magic Leap 1 or Magic Leap 2 APIs or final VAC distances.

Once the model is confirmed, the remaining device-specific work is:

1. configure the correct Magic Leap Unity/OpenXR stack;
2. map Magic Leap controller input to the existing public methods;
3. confirm optical focal distance/focal planes;
4. replace placeholder Low/High VAC values after pilot testing.

## Participant auto-assignment

The numeric part of Participant ID determines both condition order and Tetris sequence:

| IDs | Condition 1 | Condition 2 |
|---|---|---|
| P01, P05, ... | Low VAC + A | High VAC + B |
| P02, P06, ... | High VAC + A | Low VAC + B |
| P03, P07, ... | Low VAC + B | High VAC + A |
| P04, P08, ... | High VAC + B | Low VAC + A |

## Inspector parameters

Create an ExperimentConfig asset:

Assets > Create > VAC Experiment > Experiment Config

The following can be changed without rewriting experiment logic:

- Low VAC distance
- High VAC distance
- Tetris duration
- Tetris drop interval
- formal depth-trial count
- training depth-trial count
- depth difference
- target horizontal angle
- target angular size

The default VAC distances are placeholders until headset optics are confirmed.

## Depth task randomization

For a 20-trial formal depth test, the code creates approximately:

- 10 Left-closer trials
- 10 Right-closer trials

and shuffles the order.

This keeps Left/Right exposure balanced while preventing a predictable pattern.

## Questionnaires

All questionnaires are completed **outside the Magic Leap**.

Formal questionnaire points:

1. baseline after setup/training;
2. after Condition 1;
3. after Condition 2.

Questionnaire data should use the same Participant ID so it can later be merged with Unity CSV output.

## Formal experiment flow

1. Participant screening
2. Magic Leap setup
3. Tetris training in headset
4. 3–5 depth-judgment practice trials
5. Baseline questionnaire outside headset
6. Condition 1 pre-depth test
7. Condition 1 Tetris
8. Condition 1 post-depth test
9. Post-condition questionnaire outside headset
10. Recovery
11. Condition 2 pre-depth test
12. Condition 2 Tetris
13. Condition 2 post-depth test
14. Post-condition questionnaire outside headset

## Script structure

Assets/VACExperiment/Scripts/

- Core/
  - ExperimentConfig.cs
  - ExperimentManager.cs
  - ExperimentTypes.cs
  - ParticipantAssignment.cs
  - VACController.cs
  - ExperimentDebugHUD.cs
- Tetris/
  - TetrominoType.cs
  - TetrominoShapes.cs
  - TetrisBoard.cs
  - TetrisPiece.cs
  - TetrisManager.cs
  - TetrisSequenceManager.cs
- Depth/
  - DepthJudgmentManager.cs
- Input/
  - KeyboardExperimentInput.cs
- Logging/
  - DataLogger.cs

## CSV output

Unity writes data under Application.persistentDataPath:

VACExperimentData/<ParticipantID>/

Files:

- events.csv
- tetris_summary.csv
- tetris_pieces.csv
- depth_trials.csv

## Editor controls

Tetris:
- Left: Left Arrow / A
- Right: Right Arrow / D
- Rotate: Up Arrow / W
- Soft drop: Down Arrow / S
- Hard drop: Space

Depth:
- Left response: Left Arrow / A
- Right response: Right Arrow / D

## Development priorities

1. Wire the Unity scene and test in Editor.
2. Verify Low/High virtual-depth switching.
3. Verify constant apparent board size.
4. Verify Tetris and CSV output.
5. Verify depth-task balance, accuracy, and reaction time.
6. Confirm exact Magic Leap model.
7. Add Magic Leap controller bindings.
8. Pilot VAC values and freeze final experiment parameters.

## Intentionally not implemented yet

- in-headset questionnaires;
- final Magic Leap SDK/controller binding;
- final VAC distances;
- fancy animations/effects;
- automated statistics.
