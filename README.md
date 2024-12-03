# OCP Core Chess

3rd attempt at making a chess playing engine.

## Functionality TODOs

- Promote to Knight?
- Move ordering?
  - Captures preferred.
  - Lower value taking higher even better.
  - Moves that don't go onto an attacked square.

## Code Cleanup TODOs

- Horizontal move generation seems to work, but could really do with a clean up.
- Rename `Game` to `Board` once current `Board` can be replaced.
- One instance of `Moves` class.
- Rethink `Moves` implementation.

## Current Best Timings

```
 Ply | Time
-----+-----------
 1   | < 1 ms
 2   | < 1 ms
 3   | < 10 ms
 4   | ~ 100 ms
 5   | < 1 s
 6   | ~ 15 ms
 7   | ~ 7 m
 8   | ~ 3 h
 9   | ~ 3.5 d
```

## Developer Notes

- Attack bitboards are indexed `[piece][cell][direction]`.
- Diagonal: `(0, 0), (1, 1), (2, 2)...`. i.e. `/`. Anti-diagonal `(0, 7), (1, 6), (2, 5)...`. i.e. `\`.
- `WhiteScore - BlackScore` will be positive if white has more material.

### Cell Arrangement

```
         File
         a  b  c  d  e  f  g  h
       +------------------------~~~~
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