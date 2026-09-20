# Group 6 — Technical Handoff

## What the code already includes

- Participant ID -> automatic condition/sequence assignment
- Low/High VAC distances as Inspector parameters
- Constant apparent-size scaling
- Tetris task
- Deterministic Sequence A/B
- Tetris performance logging
- Depth-judgment task
- Balanced Left/Right closer-target schedule
- Reaction-time and accuracy logging
- Training Mode
- Formal pre/post depth tests
- CSV export
- External-questionnaire pauses

## What still needs to be done in Unity

- Wire all scripts into one scene
- Connect Main/XR Camera references
- Create the ExperimentConfig asset
- Confirm the Tetris board placement looks correct
- Test full experiment state transitions
- Verify CSV files on device

## After Magic Leap model is confirmed

- Configure the correct Magic Leap/OpenXR packages
- Bind Magic Leap controller buttons to:
  - TetrisManager.MoveLeft()
  - TetrisManager.MoveRight()
  - TetrisManager.Rotate()
  - TetrisManager.SoftDrop()
  - TetrisManager.HardDrop()
  - DepthJudgmentManager.SubmitLeft()
  - DepthJudgmentManager.SubmitRight()
- Confirm focal distance/focal planes
- Pilot and update Low/High VAC distances in ExperimentConfig

## Questionnaire

All questionnaires are outside the headset.

Use the same Participant ID in the external form and Unity experiment data.

## Pilot items

- VAC distances
- exposure duration
- depth difference
- depth trial count
- Tetris Sequence A/B comparability
- recovery rule
