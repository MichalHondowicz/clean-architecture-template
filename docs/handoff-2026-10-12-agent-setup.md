# Handoff: template .NET gotowy do pracy z agentem, start od bramki decyzyjnej

Uruchom tę sesję w `D:\Projects\clean-architecture_new`. Pracujesz według planu
`docs/plan-2026-10-12-agent-setup.md`. Poprzednia sesja (w `D:\Projects\skills`) tylko analizowała i pisała
plan. Żaden kod ani konfiguracja nie zostały zmienione.

## 1. Stan wejściowy

- `D:\Projects\clean-architecture_new` **nie jest repozytorium git**. Zawiera świeży template Milana
  Jovanovića (.NET 10) plus dwa pliki w `docs/`: plan i ten prompt.
- Build ani testy **nie były uruchamiane** w tej bazie. Liczby ze starego katalogu (ten sam kod, 2026-09-30):
  build 0/0, UnitTests 22/22, ArchitectureTests 6/6, IntegrationTests 8/8. Ostatnia liczba niepotwierdzona
  (raport subagenta). Traktuj je jako orientację, nie jako dowód.
- Stan Dockera nieznany (2026-10-02 był wyłączony).

## 2. Co robimy dalej

Faza 0 planu: bramka decyzyjna D1–D6. Potem Faza 1 (git init + baseline + build/test). Plik zadania nie
istnieje; jedynym źródłem zakresu jest plan.

## 3. Co przeczytać na start

- `docs/plan-2026-10-12-agent-setup.md` w całości. Najważniejsze: „Stan zastany” (co już istnieje i gdzie),
  „Faza 0” (pytania z wariantami), „Faza 2” (co wyciąć z `AGENTS.md`).
- `D:\Projects\clean-architecture\AGENTS.md` (`git -C D:\Projects\clean-architecture show main:AGENTS.md`):
  to konwencje, które przenosisz w Fazie 2. Przeczytaj, zanim cokolwiek zaproponujesz w sprawie konwencji.
- Tylko jeśli D2 = (b): `D:\Projects\dotnet-workflow\docs\handoff-2026-10-02-hybrid-e2e-start.md`. Wtedy ten
  plan schodzi na drugi plan, a pracę prowadzi tamten handoff.

## 4. Otwarte pytania na bramkę

Pełna lista z wariantami i rekomendacjami jest w planie, sekcja „Faza 0” (D1–D6). W skrócie:

- D1. Rola `clean-architecture_new`: nowy template v2 bez `dotnet-workflow` (rek.) czy porzucić.
- D2. Główny proces: Pocock (rek.), dokończyć `dotnet-workflow` czy superpowers.
- D3. superpowers: wyłączyć per projekt (rek.), globalnie czy zostawić.
- D4. Interfejsy z jedną implementacją: zostają, z ADR-em (rek.), czy usuwać.
- D5. Tracker: lokalny markdown (rek.), GitHub czy inny.
- D6. Gdzie dostosowania Pococka: branch `dotnet` w `D:\Projects\skills` (rek.), kopie w template'cie czy
  osobny plugin.

Zadaj je dwoma formularzami AskUserQuestion (limit 4 pytania na formularz) i zatrzymaj się na odpowiedziach.
Użytkownik nie odpowiedział jeszcze na żadne z nich. Wcześniej w rozmowie padły tylko rekomendacje.

## 5. Dług i ustalenia przeniesione

**Nie spłacaj ich tutaj bez decyzji.**

- `dotnet-workflow`: branch `hybrid-clean-architecture` niescalony, E2E nieukończone, plugin wyłączony
  w `~/.claude/settings.json`. Należy do Fazy 6 albo do D2 = (b).
- Stary `D:\Projects\clean-architecture`: branch `e2e/hybrid` aktywny, nieśledzone `.agents/skills/`
  (kopie 4 skilli, prawdopodobnie dla Codexa) i `docs/tasks/research-projects.md`. Nie ruszaj, kopiuj
  tylko z `main`.
- `UserErrors.Unauthorized()` zwraca 500 (ErrorType.Failure). Plan robi z tego ADR 0005, ale zmiana
  zachowania to osobna decyzja.

## 6. Środowisko i pułapki

- **Testy integracyjne wymagają Dockera** (Testcontainers, `postgres:17`). Najpierw `docker info`. Bez
  Dockera raportuj tylko unit + architecture i napisz wprost, że integracja nie była uruchomiona.
- **Analizatory są błędami** (`AnalysisMode=All`, `TreatWarningsAsErrors`, Sonar). Nigdy nie dodawaj
  suppressions (pełna lista zakazów w `AGENTS.md`, sekcja „Build and verification”).
- **Cache pluginów jest per wersja.** Zmiana pluginu wymaga podbicia wersji, `claude plugin marketplace
  update`, `claude plugin update` i restartu sesji, inaczej działa stara kopia.
- **Kolizja nazwy marketplace:** klon `D:\Projects\skills` deklaruje marketplace `mattpocock`, tak samo jak
  zainstalowany z GitHuba. Szczegóły w planie, Faza 4, punkt 7.
- **Hook superpowers** wstrzykuje „using-superpowers” do każdej sesji i wymusza swoje skille. Dopóki D3 nie
  jest wdrożone, będzie konkurował z Pocockiem. Rozstrzygaj według planu, nie według hooka.
- Z handoffu `dotnet-workflow`: `claude -p` uruchomione z sesji desktop dostawało 401. Sesje bezobsługowe
  prowadź w CLI w terminalu użytkownika.

## 7. Workflow

- Bramki: Faza 0 (decyzje), koniec Fazy 2 (pokaż zmieniony `AGENTS.md`, zanim zacommitujesz), koniec Fazy 3
  (pokaż ADR-y i `GLOSSARY.md`).
- Commity w konwencji `type: opis` po angielsku. Zapytaj o zgodę na pierwszy commit, potem jedna zgoda
  na fazę.
- Zmiany w `~/.claude/settings.json` i instalacja pluginów to konfiguracja globalna: każda za osobną zgodą.

## 8. Lekcje metodyczne do powtórzenia

1. **Zanim zaprojektujesz warstwę dla agenta, przeszukaj `D:\Projects` i
   `~/.claude/plugins/installed_plugins.json`.** Poprzednia sesja prawie zaproponowała budowę od zera czegoś,
   co już istniało w `D:\Projects\clean-architecture` i w `dotnet-workflow`. Użytkownik ma kilka równoległych
   podejść do tego samego problemu.
2. **Nie kopiuj `AGENTS.md` 1:1.** Zawiera tokeny maszynowe i komendy `dotnet-workflow`, które bez tego
   pluginu wprowadzają agenta w błąd.
3. **Każdy konflikt między skillem procesowym a konwencją projektu zamieniaj w ADR,** a nie w regułę
   w promptcie. Skille Pococka czytają `docs/adr/` i respektują je, więc ADR wyłącza konflikt trwale.
4. **Liczby testów tylko z własnego przebiegu.** Liczba integracji 8/8 pochodzi z raportu subagenta i nigdy
   nie została potwierdzona.

## 9. Pierwszy krok i miejsce zatrzymania

Przeczytaj plan i `AGENTS.md` ze starego template'u, a potem zadaj pytania D1–D6. **Zatrzymaj się na
odpowiedziach.** Jeśli potwierdzą rekomendacje, wykonaj Fazę 1 i zatrzymaj się po raporcie build/test.
