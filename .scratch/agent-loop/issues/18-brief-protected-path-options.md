# 18: Brief `needs-info` dla chronionej ścieżki proponuje opcję, która nie może przejść

**What to build:** Gdy ticket staje na chronionej ścieżce, brief podaje tylko drogi, które w tym repozytorium
mogą się skończyć `done`, albo mówi, że zmiana jest możliwa wyłącznie ręcznie, poza pętlą.

**Blocked by:** None

**Seams:** `ship/SKILL.md` w forku (brief `needs-info`); przewodnik `docs/workflow.md`; obserwacja przy
następnym tickecie zatrzymanym na chronionej ścieżce.

**Status:** done

**Skąd:** pilot 10, przebieg 2, część 2 (`todo-comment-count`, 2026-10-30). Ticket 02 (test architektury
w `tests/ArchitectureTests`) dostał `needs-info` z trzema opcjami: użytkownik dopisuje test sam, zdejmuje
`tests/ArchitectureTests/**` z `protectedPaths` na czas ticketu, albo dopuszcza zmianę ścieżki. Użytkownik
wybrał zdjęcie ochrony. Implementer dopisał test z dowodem red, ale definicja done była czerwona, bo
`HookTests.ProtectedPathsGuardTests.PreToolUse_Should_Deny_WhenImplementerWritesPathProtectedByThisRepositoryDod`
sprawdza, że guard blokuje `tests/ArchitectureTests/Layers/LayerTests.cs` (82/83). Przy przywróconej
ochronie `check-diff.ps1` oznaczyłby zmianę chronionej ścieżki, a takiego werdyktu nie wolno pominąć.
Obie drogi przez pętlę są zamknięte, więc druga i trzecia opcja z briefu nie mogły zadziałać. Kosztowało
to jeden przebieg implementera (3 minuty, okno 5-godzinne z 29% do 33%) i trzy commity do cofnięcia.

- [x] Brief `needs-info` z powodu chronionej ścieżki nie proponuje zdjęcia ochrony ani dopuszczenia zmiany
      tej ścieżki przez pętlę. Podaje dwie drogi: zmiana ręcznie, poza pętlą (z planowaną zmianą w briefie),
      albo `wontfix`.
- [x] Przewodnik `docs/workflow.md` mówi, jak zrobić zmianę chronionej ścieżki: ręcznie, poza pętlą, przed
      albo po `/ship`, albo przez `wontfix`.
- [x] Wersja forka podbita.

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, czwarty w kolejce. Ochrona chronionych ścieżek
w pętli się nie zmienia. Zdjęcie ochrony na czas ticketu nie jest drogą, bo `HookTests` przypinają ścieżki
do guarda, a `check-diff` i tak oznaczy zmianę.

**2026-11-01, wynik:** fork `736d0b6`, `327c769` (plugin `1.3.1-dotnet.24`, zainstalowany), template `d7ab2c3`,
`0a714ec`. „Stuck” w `ship/SKILL.md`: ticket stojący na chronionej ścieżce dostaje w **Needs** dokładnie dwie
drogi, obie jako commit w worktree ficzera na `feat/<feature>`, bo niezacommitowany status kolejny przebieg
zestashuje: (1) ręczna zmiana razem z ticketem ustawionym na `done`, planowana zmiana w **Where it stands**;
(2) ticket `wontfix`, a brief wymienia tickety, które mają go w `Blocked by` (trzeba je odblokować w tym samym
commicie). Pętla nie proponuje zdjęcia ochrony. Na prośbę o nie w żywej sesji odpowiada tymi dwiema drogami,
zamiast to robić (tak było w pilocie 10). Przewodnik: nowy punkt „Ticket na chronionej ścieżce”
w „Wznawianie i stany pośrednie”. Review Spec (Sonnet): runda 1 `fail` z 2 findings (status niezacommitowany
w worktree, zależne tickety przy `wontfix`), poprawione, re-review `pass`.
