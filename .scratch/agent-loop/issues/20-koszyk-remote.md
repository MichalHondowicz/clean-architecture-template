# 20: Koszyk: remote na GitHubie

**What to build:** Koszyk ma prywatne repozytorium na GitHubie jako `origin`, a `main` jest wypchnięty. Bramka 2
pętli kończy się `git push origin main`, a CI (ticket 24) potrzebuje repozytorium na GitHubie.

**Blocked by:** None

**Seams:** `git remote -v` i `git ls-remote origin main` w `D:\Projects\Koszyk`.

**Status:** done

**Skąd:** ticket 11, decyzja 2 z bramki 2026-11-04. Dziś `git remote -v` w Koszyku nic nie zwraca.

- [x] AC1: Użytkownik zakłada prywatne repozytorium na GitHubie (agent nie zakłada kont ani repozytoriów).
- [x] AC2: `origin` w Koszyku wskazuje to repozytorium, a `git ls-remote origin main` daje ten sam commit co
      lokalny `main`.
- [x] AC3: Nieśledzony `docs/tasks/state-15-mobile-polish-hardening.md` jest zacommitowany przed pushem albo
      świadomie zostawiony (decyzja użytkownika).

## Comments

**2026-11-05, wynik** (sprawdzone w `D:\Projects\Koszyk`, ticket zamknięty na słowo użytkownika):

- AC1: repozytorium `https://github.com/MichalHondowicz/koszyk` założył użytkownik. Jest prywatne: API GitHuba
  bez logowania odpowiada na nie 404, a `git ls-remote` z poświadczeniami je widzi.
- AC2: `origin` wskazuje to repozytorium. Pierwszy push `main` poszedł 2026-11-05 o 21:00 (`b4b7b04`, ficzer
  `ux-cosmetics-1`). Teraz `git ls-remote origin refs/heads/main` i lokalny `main` dają ten sam commit
  `fee4e52`, już z ticketem 04 `ux-cosmetics-1`.
- AC3: `docs/tasks/state-15-mobile-polish-hardening.md` jest w `main`. Ostatnio zmienił go `fee4e52` (handoff
  do planowania `emulator-tls-release-e2e`).

Odblokowuje ticket 24 (CI Koszyka).
