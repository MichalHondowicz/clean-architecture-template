# 20: Koszyk: remote na GitHubie

**What to build:** Koszyk ma prywatne repozytorium na GitHubie jako `origin`, a `main` jest wypchnięty. Bramka 2
pętli kończy się `git push origin main`, a CI (ticket 24) potrzebuje repozytorium na GitHubie.

**Blocked by:** None

**Seams:** `git remote -v` i `git ls-remote origin main` w `D:\Projects\Koszyk`.

**Status:** ready-for-human

**Skąd:** ticket 11, decyzja 2 z bramki 2026-11-04. Dziś `git remote -v` w Koszyku nic nie zwraca.

- [ ] AC1: Użytkownik zakłada prywatne repozytorium na GitHubie (agent nie zakłada kont ani repozytoriów).
- [ ] AC2: `origin` w Koszyku wskazuje to repozytorium, a `git ls-remote origin main` daje ten sam commit co
      lokalny `main`.
- [ ] AC3: Nieśledzony `docs/tasks/state-15-mobile-polish-hardening.md` jest zacommitowany przed pushem albo
      świadomie zostawiony (decyzja użytkownika).
