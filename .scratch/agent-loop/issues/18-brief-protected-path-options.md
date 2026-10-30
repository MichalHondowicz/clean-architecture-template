# 18: Brief `needs-info` dla chronionej ścieżki proponuje opcję, która nie może przejść

**What to build:** Gdy ticket staje na chronionej ścieżce, brief podaje tylko drogi, które w tym repozytorium
mogą się skończyć `done`, albo mówi, że zmiana jest możliwa wyłącznie ręcznie, poza pętlą.

**Blocked by:** None

**Seams:** do ustalenia przy triage (możliwe: `ship/SKILL.md` w forku, sekcja briefu `needs-info`; opis
w `docs/workflow.md`; związek z ticketem 13).

**Status:** needs-triage

**Skąd:** pilot 10, przebieg 2, część 2 (`todo-comment-count`, 2026-10-30). Ticket 02 (test architektury
w `tests/ArchitectureTests`) dostał `needs-info` z trzema opcjami: użytkownik dopisuje test sam, zdejmuje
`tests/ArchitectureTests/**` z `protectedPaths` na czas ticketu, albo dopuszcza zmianę ścieżki. Użytkownik
wybrał zdjęcie ochrony. Implementer dopisał test z dowodem red, ale definicja done była czerwona, bo
`HookTests.ProtectedPathsGuardTests.PreToolUse_Should_Deny_WhenImplementerWritesPathProtectedByThisRepositoryDod`
sprawdza, że guard blokuje `tests/ArchitectureTests/Layers/LayerTests.cs` (82/83). Przy przywróconej
ochronie `check-diff.ps1` oznaczyłby zmianę chronionej ścieżki, a takiego werdyktu nie wolno pominąć.
Obie drogi przez pętlę są zamknięte, więc druga i trzecia opcja z briefu nie mogły zadziałać. Kosztowało
to jeden przebieg implementera (3 minuty, okno 5-godzinne z 29% do 33%) i trzy commity do cofnięcia.

- [ ] Brief `needs-info` z powodu chronionej ścieżki nie proponuje zdjęcia ochrony, dopóki testy repozytorium
      (tu `HookTests`) przypinają tę ścieżkę do guarda. Albo wymienia tę zależność wprost.
- [ ] Przewodnik `docs/workflow.md` mówi, jak zrobić zmianę chronionej ścieżki: ręcznie, poza pętlą, przed
      albo po `/ship`, albo przez `wontfix`.
