# 01: Copy publikuje zdarzenie utworzenia z id kopii

**What to build:** Copy todo itemu publikuje zdarzenie utworzenia todo itemu z id nowo powstałej kopii. Dziś
kopia dostaje id dopiero przy dodaniu do kontekstu, już po `Raise(...)`, więc zdarzenie niesie pusty
identyfikator (ADR 0006: id musi być nadane przed `Raise(...)`). Utworzenie todo itemu nadaje id wcześniej
i nie ma tego problemu.

**Blocked by:** None (can start immediately)

**Status:** needs-triage

- [ ] Zdarzenie utworzenia po copy niesie id kopii, równe id zwróconemu klientowi.
- [ ] Test unit handlera copy sprawdza id w zdarzeniu i pada na obecnym kodzie (red proof).
