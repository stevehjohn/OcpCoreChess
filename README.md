# OCP Core Chess

3rd attempt at making a chess playing engine.

## Functionality TODOs

- Promote to all possibilities. Done but needs testing.
- Move ordering:
  - Done:
    - Captures preferred.
    - Lower value taking higher is better.
    - Utilise `PriorityQueue` before further depth processing.
  - TODO:
    - Moves that don't go onto an attacked square?
    - Randomness to break ties?

## Code Cleanup TODOs

- Find a nice way to integrate PerfTree.
- Flesh out `InlineData` test cases for pieces.

## Current Best Timings

From an initial board state.

```
 Ply | Time
-----+-----------
 1   | < 1 ms
 2   | < 1 ms
 3   | ~ 10 ms
 4   | ~ 100 ms
 5   | ~ 500 ms
 6   | ~ 1.5 s
 7   | ~ 30 s
 8   | ~ 12 m
 9   | ~ 1 d 2h
```

## Developer Notes

- Attack bitboards are indexed `[moveSet][cell]`.
- Diagonal: `(0, 0), (1, 1), (2, 2)...`. i.e. `/`. Anti-diagonal `(0, 7), (1, 6), (2, 5)...`. i.e. `\`.
- `WhiteScore - BlackScore` will be positive if white has more material.
- Move score is currently (remember `PriorityQueue` prefers lower values):
  - `(MoveOutcome.CheckMate - outcomes) * 1,000,000` (CheckMate == 0, Nothing but a simple move = 63,000,000).
  - `+ (10 - capturedPieceValue) * 1,000` (higher captured piece values will yield lower scores).
  - `+ movedPieceValue * 100` (lower moved piece value is good).
  - This leave space for an attacker count at digits 2 ans 3, i.e. `OOAACPRR` where:
    - OO is outcome value.
    - AA is number of attackers value.
    - C is captured piece value inverted.
    - P is player piece value.
    - R is randomness to break ties.

### Cell Arrangement

```
         File
         a  b  c  d  e  f  g  h
       +------------------------
Rank 8 | 56 57 58 59 60 61 62 63  Black
     7 | 48 49 50 51 52 53 54 55  Black
     6 | 40 41 42 43 44 45 46 47
     5 | 32 33 34 35 36 37 38 39
     4 | 24 25 26 27 28 29 30 31
     3 | 16 17 18 19 20 21 22 23
     2 | 8  9  10 11 12 13 14 15  White
     1 | 0  1  2  3  4  5  6  7   White
```

### Board State

```
(ulong) mmmm_mmmm_mmmm_hhhh_hhhh_kkkk_kkKK_KKKK_bbbb_bbbb_wwww_wwww_eeee_eeep_cccc
```

- c: Castling rights.
- p: Current player.
- e: En passant cell.
- b: Black score.
- w: White score.
- K: White King cell.
- k: Black king cell.
- h: Halfmove clock.
- m: Fullmove count.