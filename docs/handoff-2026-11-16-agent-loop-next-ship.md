# Handoff: tickety pętli 12, 19 i 27 czekają na następny przebieg `/ship` w Koszyku

W template'cie nie ma pracy dla agenta. Otwarte są trzy tickety pętli, wszystkie `ready-for-human`, i wszystkie
czekają na obserwacje z kolejnego przebiegu `/ship` w Koszyku (`D:\Projects\Koszyk`). Ta sesja nie była
kompaktowana.

## 1. Stan wejściowy

- **Template** (`D:\Projects\clean-architecture_new`): gałąź `chore/agent-loop`. Sprawdź `git status --short`
  i `git branch -vv`: wpisy z 2026-11-15/16 w ticketach 12, 19 i 27 oraz ten plik miały pójść jednym commitem,
  a `main` miał zostać przesunięty na `chore/agent-loop` i wypchnięty, jeśli użytkownik się zgodził. Jeśli
  drzewo nie jest czyste, zapytaj o commit, zanim zrobisz cokolwiek innego.
- **Testy:** w tej i poprzedniej sesji nie uruchamiano builda ani testów; od 2026-11-15 zmieniały się tylko
  dokumenty.
- **Tickety pętli** (`.scratch/agent-loop/issues/`): `done` 01–11, 13–18, 20–26, 31–33; `wontfix` 28–30;
  `ready-for-human` 12, 19, 27.
- **Koszyk, stan z 2026-11-16** (odczytany z repozytorium):
  - `main` = `origin/main` = `9db2b11` (`ship: gate 2, conventions 1, 2`). Ficzer `mobile-smoke-hardening`
    scalony i wypchnięty przez krok `/ship` 2026-11-15 22:14; worktree usunięte. CI po scaleniu zielone
    (odpowiedź użytkownika).
  - Nowego planu (commit `plan: …`) nie ma: następny ficzer modułu 15 nie jest wybrany.
  - Najnowszy handoff Koszyka: `docs/tasks/handoff-15-smoke-hardening-and-next-feature.md` (sprzed przebiegu;
    jego sekcja 2, punkt 1 jest już zrobiona).

## 2. Co robimy dalej

Nic nie budujemy. Po zakończeniu następnego przebiegu `/ship` w Koszyku (plan, bramki 1 i 2 i scalenie należą
do użytkownika i do sesji Koszyka) zapisz jego obserwacje w ticketach template'u:

1. **Ticket 27:** przebieg według wzoru z wpisu „2026-11-15 (noc)” (źródła, tabela kosztu, kryteria) i postęp
   AC1–AC3: który ficzer modułu 15 doszedł do `main`.
2. **Ticket 19:** czy padła pauza na limit 5h (commit `ship: pause`) i czy przebieg wznowił się sam; czy padło
   pytanie o zgodę, czy przyszło powiadomienie i czy pytanie trafiło do allowlisty Koszyka.
3. **Ticket 12, AC5:** czy hook `Notification` wysłał powiadomienie ntfy przy pytaniu o zgodę.

Pliku zadania nie ma i nie trzeba: to wpisy w ticketach. Ticketów nie zamykaj bez decyzji użytkownika.

## 3. Co przeczytać na start

- `.scratch/agent-loop/issues/27-koszyk-module-15.md`: kryteria AC1–AC3 i dwa wpisy z 2026-11-15 (noc): wzór
  zapisu przebiegu i lista ficzerów modułu 15, które zostały.
- `.scratch/agent-loop/issues/19-pilot-leftovers-resume-and-permission-push.md`, `## Comments`: co już
  zaobserwowano (restart w trakcie czekania 2026-11-11) i czego nadal brak.
- `.scratch/agent-loop/issues/12-push-fallback-ntfy.md`, AC5 i dwa ostatnie wpisy: co jest ustawione.
- `D:\Projects\Koszyk\docs\tasks\`: najnowszy `handoff-*.md` i `state-15-mobile-polish-hardening.md`, zanim
  cokolwiek doradzisz w Koszyku.

## 4. Otwarte pytania na bramkę

1. **Następny ficzer modułu 15 Koszyka** (AC1 ticketu 27). Decyzja zapada w sesji Koszyka, nie tutaj. Handoff Koszyka
   (`docs/tasks/handoff-15-smoke-hardening-and-next-feature.md`, sekcja 4) rekomenduje sync i auth, bo to jedyna
   grupa z ryzykiem utraty danych. Tutaj tylko odnotuj wybór w tickecie 27.
2. **Błąd AppHost z E2E Release** (ficzer TLS przeszedł DoD, review i bramkę 2, a `api` nie startowało). Warianty:
   (a) zostawić jako wniosek w tickecie 26; (b) ticket `needs-triage` o smoke'u backendu w DoD. Rekomendacja: (a),
   dopóki się nie powtórzy: reguła zamrożenia pętli wymaga nieudanego przebiegu `/ship`.
3. **Co z ticketami 12 i 19, jeśli kolejne przebiegi dalej nie dadzą pytania o zgodę.** Żaden przebieg `/ship` z
   zapisem (pilot 10, pilot 2, `mobile-smoke-hardening`) nie miał pytania o zgodę. Warianty: (a) czekać dalej;
   (b) po końcu modułu 15 uznać, że allowlista je wyeliminowała, i zamknąć tę obserwację jako „nie występuje”.
   Rekomendacja: (a) do końca modułu 15, potem wróć do pytania z użytkownikiem.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Ticket 12, AC5** i **ticket 19**: same obserwacje, bez pracy w kodzie, dopóki obserwacja nie pokaże błędu.
- **Poprawka forka `1.3.1-dotnet.35`** (komunikat o przerwanym `wait-for-reset.ps1` oznacza „wznów z trackera”)
  nadal nie sprawdzona na żywo. Należy do ticketu 19.
- **Koszt ficzera:** `mobile-smoke-hardening` kosztował 8 pp tygodnia i 62 pp okna 5h bez planu (ticket 27).
  „Ok. 5% tygodnia z planem” z ticketu 11 to dolna granica. Decyzja „zostaje Pro” stoi; wróć do niej tylko, gdy
  dwa ficzery przestaną się mieścić w tygodniu.
- **Reguła zamrożenia pętli** (2026-11-11): zmiany w pętli tylko po nieudanym realnym przebiegu. Warstwa MAUI
  template'u (tickety 28–30, `wontfix`) zostaje zaparkowana.
- **Tickety Koszyka 06 i 07 z `mobile-smoke-hardening`** (`needs-triage`) i reszta ticketów Koszyka należą do
  Koszyka.

## 6. Środowisko i pułapki, które kosztowały czas

- **Nie dotykaj Koszyka, gdy trwa `/ship`.** Commit na `main` Koszyka w trakcie przebiegu zatrzyma bramkę 2
  (przesunięty `main`). Czytaj wyłącznie: `git log`, `git worktree list`, pliki w worktree.
- **Stan przebiegu w toku jest w worktree, nie na `main`:**
  `D:\Projects\Koszyk\.claude\worktrees\<slug>\.scratch\<slug>\metrics.md`, `brief.md` i `issues/*.md`. Po scaleniu
  worktree znika, a te same pliki są w `.scratch/<slug>/` na `main`.
- **Czy przebieg się skończył:** commit `ship: gate 2` na `main` Koszyka i brak worktree `<slug>` w
  `git worktree list`. Reflog `origin/main` (`git reflog show origin/main --date=iso`) pokazuje push ok. 20 s po
  commicie bramki, gdy scala krok `/ship`.
- **Pauza na limit:** commit `ship: pause` w historii przebiegu. Bez niego wznowienia nie było czego obserwować.
- **Wiersz `metrics.md` obejmujący pauzę nie mierzy kosztu** (ticket 26, wpis o przebiegach 2 i 3).
- **Allowlista Koszyka:** `git log <commit planu>..main -- .claude/` pokazuje, czy w trakcie przebiegu coś do niej
  doszło.
- **Wynik CI i pytania o zgodę zna tylko użytkownik:** logów CI nie da się odczytać z sesji (prywatne repozytoria,
  bez `gh`, przeglądarka wylogowana z GitHuba), a powiadomienie ntfy przychodzi na jego telefon. Zapytaj o oba.
- **Klasyfikator trybu auto** 2026-11-16 wielokrotnie nie dał werdyktu dla Basha, Write i Edit („gave no verdict
  (error)”); duży Write nie przeszedł ani razu. To błąd przejściowy: ponów raz, dziel duże zapisy na mniejsze
  Edit, a w międzyczasie rób odczyty (Read, Grep, Glob go nie wymagają).
- **`cat >>` i `sed -i` z Git Basha** zmieniają pliki poza narzędziem Edit; po nich Edit wymaga ponownego Read.
- **Przesunięcie `main` template'u bez checkoutu:** `git merge-base --is-ancestor origin/main HEAD`, potem
  `git fetch . chore/agent-loop:main` i `git push origin main`. Push z sesji agenta działa po „tak” użytkownika.

## 7. Workflow

Bez zmian. Bramka 1 po `/plan-feature`, bramka 2 po briefie `/ship`; oba polecenia uruchamia użytkownik w sesji
Koszyka. W tej sesji template'u nie działali żadni agenci w tle. Commity i pushe w template'cie tylko po słowie
użytkownika.

## 8. Lekcje metodyczne do powtórzenia

1. **Zanim doradzisz krok w innym repozytorium, przeczytaj jego `git log` i ostatni handoff.** Ticket opisuje
   zamiar, repozytorium opisuje stan; 2026-11-15 stan Koszyka był dwa razy dalej, niż mówiły tickety.
2. **Brak zdarzenia zapisuj jako „brak obserwacji” razem z dowodem, nie jako spełnione kryterium.** W
   `mobile-smoke-hardening` nie było pauzy (brak `ship: pause`, okno 5h skończyło na 66%) ani pytania o zgodę;
   wpis mówi to wprost, więc nikt nie odhaczy ticketu 19 na podstawie przebiegu, który go nie sprawdził.
3. **Najpierw zapisz fakty z repozytorium, potem zadaj użytkownikowi tylko pytania, na które repo nie odpowie, po
   jednym.** 2026-11-15 wynik CI i „pytanie o zgodę nie padło” przyszły w dwóch krótkich odpowiedziach, bez
   przeszukiwania logów sesji.
4. **Kryterium z dwiema częściami odhacza użytkownik, nie agent.** Opisz różnicę między zapisem a stanem, zapytaj
   i dopiero po „tak” odhacz z adnotacją (wzór: zamknięcie ticketu 26).

## 9. Pierwszy krok i miejsce zatrzymania

Zacznij od `git status --short` w template'cie, a potem `git -C D:\Projects\Koszyk worktree list` i
`git -C D:\Projects\Koszyk log --format="%h %ad %s" --date=iso -15 main`.

- Jeśli od `9db2b11` nie przybył commit `plan: …`: następny ficzer nie jest zaplanowany. Powiedz to użytkownikowi
  (wybór ficzera i `/plan-feature` należą do sesji Koszyka) i zatrzymaj się.
- Jeśli istnieje worktree w `Koszyk\.claude\worktrees\`: przebieg trwa albo czeka na bramkę 2. Podaj stan
  z `metrics.md` w worktree i zatrzymaj się. Niczego nie zapisuj w Koszyku.
- Jeśli nowy ficzer jest w `main` (commit `ship: gate 2`): dopisz przebieg do ticketów 27 i 19 (i 12, gdy padło
  pytanie o zgodę), zapytaj użytkownika o wynik CI po scaleniu i o pytanie o zgodę, i zatrzymaj się na
  odpowiedziach. Ticketów nie zamykaj bez decyzji użytkownika.
