# Handoff: Koszyk na pętli, ticket 23 i dalej

Etap 2 pętli bez nadzoru jest rozpisany i zaczęty: najpierw przenosimy Koszyka (`D:\Projects\Koszyk`) na pętlę
`/plan-feature` → `/ship`, potem pilot 2 w Koszyku, na końcu warstwa MAUI w tym template'cie. Zrób ticket 23,
potem 25. Część tej sesji przed 2026-11-01 znam tylko ze streszczenia po kompaktowaniu; sekcje 4–8 dotyczą
pracy po nim i są z pierwszej ręki.

## 1. Stan wejściowy

- **Template** (`D:\Projects\clean-architecture_new`): gałąź `chore/agent-loop`, HEAD `8eba86f`, drzewo czyste
  poza nieśledzonymi `.agents/` i `.codex/` (zostają nieśledzone, decyzja z ticketu 09). `main` na GitHubie
  to `e16ee32` (fast-forward bez PR, decyzja użytkownika z 2026-11-04, CI #37 zielone). `chore/agent-loop` ma
  3 commity ponad `origin` (tylko dokumenty i tickety). Testy z mojego ostatniego pełnego przebiegu przed
  tym merge: 6/54/14/92 (architektura, jednostkowe, integracyjne, hooki). Potem zmieniały się tylko dokumenty.
- **Koszyk**: gałąź `chore/agent-loop`, HEAD `0613390`, bez remote (`git remote -v` pusty, ticket 20).
  Nieśledzony `docs/tasks/state-15-mobile-polish-hardening.md` jest użytkownika: nie commituj go bez decyzji.
  Mój przebieg 2026-11-04: `Koszyk.sln` i `Koszyk.Mobile.sln` 0 ostrzeżeń, testy 99 (Unit), 5 (Architecture),
  130 (Integration), 587 (Mobile).
- **Fork** `D:\Projects\skills`: gałąź `dotnet`, HEAD `7eed158`, 28 commitów ponad `backup/dotnet`. Plugin
  `mattpocock-skills` `1.3.1-dotnet.27` zainstalowany.
- **Sandbox** `D:\Projects\ship-sandbox`: `main` `96ca937`, 2 commity ponad swój lokalny origin.
- **Tickety** w `.scratch/agent-loop/issues/`: zrobione 01–10, 13–15, 17, 18, 21, 22. `ready-for-human`: 11
  (zostało kryterium decyzji o planie po pilocie 2), 12 i 16 (kod gotowy, czekają na wpisy użytkownika
  w `.claude/settings.json` i zmienną `CLAUDE_NTFY_URL`), 19 (obserwacja), 20 (repo Koszyka na GitHubie zakłada
  użytkownik), 26 i 27 (pilot 2 i moduł 15, uruchamia użytkownik). `ready-for-agent`: 23, 24 (czeka na 20),
  25, 28–30 (czekają na 26).

## 2. Co robimy dalej

1. **Ticket 23** (`.scratch/agent-loop/issues/23-koszyk-loop-setup.md`): `docs/agents/dod.json`, kontrakt
   implementera, kopie hooków i wzór rejestracji w Koszyku. Blokery 21 i 22 są zrobione. Plik ticketu istnieje,
   praca nie zaczęta.
2. **Ticket 25** (`25-koszyk-add-screen.md`): skill `add-screen` w Koszyku. Odblokowany.
3. Ticket 24 (CI Koszyka) dopiero po ticket 20.

## 3. Co przeczytać na start

- `.scratch/agent-loop/issues/11-plan-stage-2.md`, `## Comments`: fakty o Koszyku i 9 decyzji z bramki
  2026-11-04, tabela ticketów 20–30 z blokerami.
- `.scratch/agent-loop/issues/23-koszyk-loop-setup.md`: kryteria AC1–AC7.
- `.scratch/agent-loop/issues/21-koszyk-process-layer.md` i `22-koszyk-architecture-tests.md`, `## Comments`:
  co już jest w Koszyku i dlaczego (np. testy architektury sprawdzają `ServiceDefaults` po assembly).
- `docs/workflow.md`, „Istniejący projekt”: tabela elementów pętli (wiersze „tylko dla pętli”) i krok 5.
- Template: `docs/agents/dod.json`, `docs/agents/implementer-contract.md`, `.claude/settings.json` (sekcja
  `hooks` i allowlista `/ship`, wzór dla Koszyka), `.claude/hooks/dod-common.ps1` (`Get-DodProblems`: co jest
  wymagane w `dod.json`).
- `.scratch/agent-loop/issues/12-push-fallback-ntfy.md` i `16-windows-update-restart.md`, `## Comments`: gotowe
  wpisy `settings.json` dla `notify.ps1` i `check-reboot.ps1`; ten sam wzór trafia do Koszyka.
- Koszyk `AGENTS.md`: „Build / run / test”, „Pułapki środowiskowe”, „Agent skills”.

## 4. Otwarte pytania na bramkę

Ticket 23 ma te decyzje do rozstrzygnięcia przed pisaniem (pokaż je użytkownikowi razem, z rekomendacjami):

1. **Docker w `environmentChecks`.** Testy integracyjne Koszyka używają EF InMemory i `WebApplicationFactory`
   (w pakietach testów nie ma Testcontainers). Rekomendacja: `"environmentChecks": []`, po sprawdzeniu
   `git grep` na Testcontainers/Aspire testing i przebiegu testów przy zatrzymanym Dockerze, jeśli użytkownik
   się zgodzi go zatrzymać (AC2). `dod-common.ps1` przyjmuje `[]`, brak pola to błąd.
2. **Jak wymusić 0 ostrzeżeń.** Koszyk nie ma `TreatWarningsAsErrors`. Rekomendacja: `-warnaserror` w komendach
   DoD (`dotnet build Koszyk.sln -warnaserror`, to samo dla `Koszyk.Mobile.sln`), bez zmian w csproj.
   Sprawdź, że nie łapie ostrzeżeń NuGet z zewnątrz, które dziś przechodzą.
3. **Kolejność i koszt komend.** Build głowy Androida trwa dłużej niż backend. Rekomendacja: build backendu,
   test backendu (`--no-build`), build mobilny, test mobilny (`--no-build`); zmierz i zapisz czas `-Check commands`.
4. **`.claude/settings.json` w Koszyku nie istnieje** (jest tylko `settings.local.json`). Tryb auto blokuje
   agentowi edycję `settings.json`, prawdopodobnie także utworzenie. Rekomendacja: pełna treść pliku (hooki
   `PreToolUse`, `SubagentStop`, `Notification` i allowlista `/ship` jak w template'cie) w `## Comments`
   ticketu 23, a plik tworzy użytkownik (AC7).
5. **Kontrakt implementera.** Rekomendacja: kopia z template'u, a z niej usunięte to, co Koszyka nie dotyczy
   (`Result`, Testcontainers, `ca-review`, skille `add-feature`). Dodane: wyjątki domenowe (ADR 0001 Koszyka),
   commity backend osobno od mobile+docs (`AGENTS.md` Koszyka).
6. **`screenPaths`.** Rekomendacja jak w AC4: XAML z `src/Koszyk.Mobile/**`, `MauiProgram.cs`, csproj głowy,
   `Platforms/Android/AndroidManifest.xml`.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika:

- **Tickety 12 i 16**: wpisy w `.claude/settings.json` template'u i zmienna `CLAUDE_NTFY_URL` (użytkownik).
- **Ticket 20**: repo Koszyka na GitHubie i decyzja o nieśledzonym `state-15` (AC3).
- **Pliki `docs/tasks/handoff-15-*.md` i `state-15` w Koszyku** kierują do `/dotnet-workflow:implement`, który
  jest wyłączony. Są wejściem do ticketu 27 (moduł 15 jako ficzery), nie instrukcją do wykonania.
- **Walidatory w template'cie**: przykład w `.claude/skills/add-feature/references/command-slice.md` pokazuje
  `public class`, a 5 z 7 walidatorów to `internal sealed`. Decyzja użytkownika (z ticketu 15).
- **Push**: fork 28 commitów ponad `backup/dotnet`, sandbox 2 commity, template `chore/agent-loop` 3 commity.
  Pushujesz tylko na słowo użytkownika. `main` template'u użytkownik pozwala aktualizować fast-forwardem bez PR.
- **Sandbox** potrzebuje nowych hooków z ticketów 12, 16 i 17 (ticket 30 AC4).
- **Ticket 11**, ostatnie kryterium: decyzja o planie po pilocie 2 (reguła z historii 48 speca).
- Gym zostaje poza zakresem.

## 6. Środowisko i pułapki, które kosztowały czas

- **Tryb auto blokuje edycję `.claude/settings.json`.** Wpisy hooków i allowlisty dawaj użytkownikowi jako
  gotowy fragment w tickecie.
- **Skille tylko dla użytkownika** (`/setup-matt-pocock-skills`, `/plan-feature`, `/ship`): narzędzie Skill ich
  nie uruchomi. W ticket 21 efekt `setup` zrobiłem ręcznie i review to uznał.
- **`dotnet sln add` projektu testów dopisuje też projekty, do których się odwołuje.** W Koszyku dopisało
  `Koszyk.Mobile.Core` do `Koszyk.sln`, wbrew `AGENTS.md`. Po `sln add` sprawdź `git diff Koszyk.sln`.
- **NetArchTest dopasowuje po przestrzeni nazw, nie po assembly.** `ServiceDefaults` Koszyka ma rozszerzenia
  w `Microsoft.Extensions.Hosting`, a `AppHost` nie ma przestrzeni nazw: dla nich służy
  `GetReferencedAssemblies()`.
- **Długi polski tekst w PowerShell** (`„…”` w here-stringu z `$`) kończył się `Invoke-Expression: Unexpected
  token`. Pisz takie wpisy przez Edit albo Write.
- **Plugin po zmianie w forku**: podbij wersję w `.claude-plugin/plugin.json`, potem
  `claude plugin marketplace update mattpocock` i `claude plugin update mattpocock-skills@mattpocock`. Cache
  jest po wersji: poprawka po instalacji bez nowego numeru do niego nie trafi.
- **CI template'u** rusza tylko na `main` i na PR, więc gałąź `chore/agent-loop` nie ma własnego przebiegu.
- **GitHub** bez `gh`: wynik CI czytasz w przeglądarce aplikacji (`find` na „completed successfully”), tekst
  strony nie pokazuje statusu.

## 7. Workflow

Tickety pętli robisz w tej sesji, nie przez `/ship`: zmieniają chronione ścieżki (`.claude/**`, `docs/agents/**`,
`AGENTS.md`). Na każdy ticket: implementacja (skrypty test-first, z dowodem czerwieni), pełny build i testy,
commit, potem review Spec przez subagenta `mattpocock-skills:spec-reviewer` z wskazaniem ticketu, commitów
i faktów do sprawdzenia, poprawki, re-review przez `SendMessage` do tego samego agenta, na końcu wynik
w `## Comments` ticketu i `**Status:** done`. Praca w Koszyku idzie na jego gałęzi `chore/agent-loop`,
a tickety i ich wyniki leżą w trackerze template'u. Commituj w konwencji repo (Koszyk: backend osobno od
mobile+docs).

## 8. Lekcje metodyczne do powtórzenia

1. **Review Spec po każdym tickecie, także dokumentacyjnym.** W 12–22 prawie każdy ticket miał w pierwszej
   rundzie prawdziwe findings: `git mv` do nieistniejącego folderu, ADR-y z wymyślonymi „rozważanymi opcjami”,
   zdania o plikach, których jeszcze nie ma. Bez review weszłyby do repo jako fakty.
2. **Każde twierdzenie w słowniku i ADR-ze sprawdź w kodzie albo w specu, ze ścieżką.** Pierwsza wersja
   `PendingInvite` mówiła o rejestracji, a kod robi to przy pierwszym logowaniu. Agent czyta `GLOSSARY.md`
   jako prawdę.
3. **Sprawdź przesłankę speca w projekcie, na który się powołuje.** Spec opisywał `Contracts` „według wzorca
   Koszyka”, a Koszyk go nie ma. Wyszło dopiero przy badaniu repo.
4. **Dowód czerwieni dla każdej reguły**, także przez mutację, gdy graf projektów nie pozwala na prawdziwą
   zależność. Zapisz, których zależności pilnuje kompilator, żeby nikt nie szukał brakującego testu.
5. **Test skryptu, który wysyła coś na zewnątrz, ustawia środowisko jawnie** (`notify.ps1` i `CLAUDE_NTFY_URL`),
   żeby przebieg na maszynie z ustawioną zmienną nigdy nie trafił do prawdziwej usługi.
6. **Gdy zmiana dotyczy pliku, którego agent nie może edytować, gotowy fragment trafia do ticketu**, a status
   zostaje `ready-for-human`. Inaczej ticket wygląda na zrobiony, a funkcja nie działa.

## 9. Pierwszy krok i miejsce zatrzymania

Przeczytaj pliki z sekcji 3. Potem w Koszyku sprawdź `git grep -n "Testcontainers\|Aspire.Hosting.Testing"`
w `tests/` i zmierz czas obecnych komend (build i testy obu solucji). Pokaż użytkownikowi decyzje z sekcji 4
na jednej bramce i **zatrzymaj się na niej**. Po odpowiedzi zrób ticket 23 do końca, łącznie z treścią
`.claude/settings.json` dla użytkownika w `## Comments`, review Spec i zamknięciem. Potem zaproponuj ticket 25.
