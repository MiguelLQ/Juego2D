# Team workflow

## Branches

- `main`: stable, releasable code. Never work directly here.
- `develop`: integration branch for the next milestone.
- `feature/*`: isolated gameplay, UI or content work.
- `fix/*`: focused defect corrections.

Keep pull requests small and require at least one teammate review. Add unit tests for mathematical rules. Attach screenshots or short videos to visual changes. Avoid mixing asset, architecture and gameplay changes in one pull request when they can be reviewed independently.

Suggested ownership for three developers: one on Domain/Application and tests, one on Game scenes/components, and one on Mobile/assets/audio. Shared contracts should be reviewed before parallel implementations begin.
