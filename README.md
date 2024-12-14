# OCP Core Chess

3rd attempt at making a chess playing engine.

Passes all Pertf tests up to ply 9.
Passes all Etherial tests.

## Status

[![.NET](https://github.com/stevehjohn/OcpCoreChess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/stevehjohn/OcpCoreChess/actions/workflows/dotnet.yml)

## Functionality TODOs

- Figure out why adjusting the priority for attackers hits performance so much.
- Add undo move to core using a stack.
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

- Unit test promotions.
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
 6   | ~ 1 s
 7   | ~ 20 s
 8   | ~ 10 m
 9   | ~ 5 h
 10  | ~ 1 month
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

## Useful Resources

- https://github.com/AndyGrant/Ethereal
- https://lichess.org/analysis

## Perf Test Results

```
  ✓ PASS  Depth:  1  Combinations:                 20  Expected:                 20
      Capture:                0 ✓
      En Passant:             0 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:                  0 ✓
      Check Mate:             0 ✓
  ✓ PASS  Depth:  2  Combinations:                400  Expected:                400
      Capture:                0 ✓
      En Passant:             0 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:                  0 ✓
      Check Mate:             0 ✓
  ✓ PASS  Depth:  3  Combinations:              8,902  Expected:              8,902
      Capture:               34 ✓
      En Passant:             0 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:                 12 ✓
      Check Mate:             0 ✓
  ✓ PASS  Depth:  4  Combinations:            197,281  Expected:            197,281
      Capture:            1,576 ✓
      En Passant:             0 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:                469 ✓
      Check Mate:             8 ✓
  ✓ PASS  Depth:  5  Combinations:          4,865,609  Expected:          4,865,609
      Capture:           82,719 ✓
      En Passant:           258 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:             27,351 ✓
      Check Mate:           347 ✓
  ✓ PASS  Depth:  6  Combinations:        119,060,324  Expected:        119,060,324
      Capture:        2,812,008 ✓
      En Passant:         5,248 ✓
      Castle:                 0 ✓
      Promotion:              0 ✓
      Check:            809,099 ✓
      Check Mate:        10,828 ✓
  ✓ PASS  Depth:  7  Combinations:      3,195,901,860  Expected:      3,195,901,860
      Capture:      108,329,926 ✓
      En Passant:       319,617 ✓
      Castle:           883,453 ✓
      Promotion:              0 ✓
      Check:         33,103,848 ✓
      Check Mate:       435,767 ✓
  ✓ PASS  Depth:  8  Combinations:     84,998,978,956  Expected:     84,998,978,956
      Capture:    3,523,740,106 ✓
      En Passant:     7,187,977 ✓
      Castle:        23,605,205 ✓
      Promotion:              0 ✓
      Check:        968,981,593 ✓
      Check Mate:     9,852,036 ✓
  ✓ PASS  Depth:  9  Combinations:  2,439,530,234,167  Expected:  2,439,530,234,167
      Capture:  125,208,536,153 ✓
      En Passant:   319,496,827 ✓
      Castle:     1,784,356,000 ✓
      Promotion:     17,334,376 ✓
      Check:     36,095,901,903 ✓
      Check Mate:   400,191,963 ✓
 ```