# Handoff: po `sync-auth` tickety pętli 12, 19 i 27 czekają na następny przebieg `/ship` w Koszyku

W template'cie nie ma pracy dla agenta. Otwarte są trzy tickety pętli (`ready-for-human`), wszystkie czekają na
obserwacje z kolejnego przebiegu `/ship` w Koszyku (`D:\Projects\Koszyk`). Ta sesja nie była kompaktowana.

## 1. Stan wejściowy

- **Template** (`D:\Projects\clean-architecture_new`): `chore/agent-loop` = `main` = `origin/main` = `78bad8e`
  (wpisy o przebiegu `sync-auth` w ticketach 27, 19, 12). Ten plik i adnotacja w poprzednim handoffie miały pójść
  osobnym commitem po zgodzie użytkownika; sprawdź `git status --short` i `git branch -vv`. Jeśli drzewo nie jest
  czyste, zapytaj o commit, zanim zrobisz cokolwiek innego.
- **Testy:** od 2026-11-15 nie uruchamiano builda ani testów template'u; zmieniały się tylko dokumenty.
- **Tickety pętli** (`.scratch/agent-loop/issues/`): `done` 01–11, 13–18, 20–26, 31–33; `wontfix` 28–30;
  `ready-for-human` 12, 19, 27.
  - 19: pierwsze kryterium (wznowienie po pauzie bez użytkownika) **odhaczone 2026-11-18** decyzją użytkownika,
    z adnotacją o różnicach; drugie (push przy pytaniu o zgodę) otwarte.
  - 12: otwarte tylko AC5 (hook `Notification` → ntfy przy pytaniu o zgodę).
  - 27: AC1–AC3 otwarte; z modułu 15 Koszyka zostały reszta motywu i brandingu, `ux-cosmetics-2`, backend i schemat.
- **Koszyk, stan z 2026-11-18** (odczytany z repozytorium): `main` = `origin/main` = `9066b65`. Ficzer `sync-auth`
  scalony przez `/ship` 2026-11-16 (`5d2378f`), po nim ręczna poprawka po E2E (`bf41be9`) i ręczna zmiana nazwy na
  Basketkin (2026-11-18, cztery commity `chore(...)`). Nowego commita `plan: …` po `bd0e4dc` (plan `sync-auth`)
  nie ma: następny ficzer nie jest wybrany. Worktree przebiegu brak.

## 2. Co robimy dalej

Nic nie budujemy. Po zakończeniu następnego przebiegu `/ship` w Koszyku (wybór ficzera, plan, bramki i scalenie
należą do użytkownika i sesji Koszyka) zapisz jego obserwacje w ticketach template'u:

1. **Ticket 27:** przebieg według wzoru z wpisu „2026-11-18, przebieg `sync-auth`” (źródła, tabela kosztu, CI po
   scaleniu, poprawki po scaleniu, kryteria) i postęp AC1–AC3.
2. **Ticket 19, drugie kryterium:** czy padło pytanie o zgodę, czy przyszło powiadomienie i czy pytanie trafiło do
   allowlisty Koszyka. Pauzę (`ship: pause`) też odnotuj: pierwsze kryterium jest odhaczone, ale restart w trakcie
   czekania (fork `.35`) dalej nie był widziany.
3. **Ticket 12, AC5:** czy hook `Notification` wysłał ntfy przy pytaniu o zgodę.

Pliku zadania nie ma i nie trzeba. Ticketów nie zamykaj bez decyzji użytkownika.

## 3. Co przeczytać na start

- `.scratch/agent-loop/issues/27-koszyk-module-15.md`: AC1–AC3 i wpis „2026-11-18, przebieg `sync-auth`”: wzór
  zapisu i lista ficzerów, które zostały.
- `.scratch/agent-loop/issues/19-pilot-leftovers-resume-and-permission-push.md`: adnotacja przy odhaczonym
  kryterium i trzy wpisy z 2026-11-18.
- `.scratch/agent-loop/issues/12-push-fallback-ntfy.md`: AC5 i wpisy od 2026-11-11.
- `D:\Projects\Koszyk\docs\tasks\`: najnowszy `handoff-*.md` i `state-15-mobile-polish-hardening.md`, zanim
  cokolwiek doradzisz w Koszyku.

## 4. Otwarte pytania na bramkę

1. **Następny ficzer modułu 15 Koszyka** (AC1 ticketu 27): motyw i branding (część zrobiona ręcznie: Basketkin),
   `ux-cosmetics-2`, backend i schemat. Decyzja zapada w sesji Koszyka; tutaj tylko ją odnotuj.
2. **Ręczne zmiany w Koszyku poza pętlą** (Basketkin, poprawka `bf41be9` po E2E). Warianty: (a) odnotowywać w
   tickecie 27 jako pracę poza pętlą; (b) liczyć do AC2 jako „ficzer doszedł do `main`”. Rekomendacja: (a). AC2
   mówi o ficzerach przez `/ship`.
3. **Ticket 19 i 12, jeśli pytanie o zgodę dalej nie padnie.** Cztery przebiegi z zapisem (pilot 10, pilot 2,
   `mobile-smoke-hardening`, `sync-auth`) bez pytania. Warianty: (a) czekać; (b) po końcu modułu 15 uznać, że
   allowlista je wyeliminowała, i zamknąć jako „nie występuje”. Rekomendacja: (a) do końca modułu 15, potem wróć do
   pytania z użytkownikiem.
4. **Smoke w DoD Koszyka** (`run-dod.ps1 -Check smoke` zawiesił się na ponad 10 minut w `sync-auth`, brief
   „Smoke”) i **błąd AppHost z E2E Release** (ticket 26). Oba należą do Koszyka albo do pętli tylko, jeśli się
   powtórzą; reguła zamrożenia pętli wymaga nieudanego przebiegu. Rekomendacja: odnotować, nie zakładać ticketu.

## 5. Dług i ustalenia przeniesione

Nie spłacaj ich tutaj bez decyzji użytkownika.

- **Ticket 12, AC5** i **ticket 19, drugie kryterium**: same obserwacje, bez pracy w kodzie.
- **Poprawka forka `1.3.1-dotnet.35`** (przerwany `wait-for-reset.ps1` = „wznów z trackera”) nie sprawdzona na
  żywo; należy do ticketu 19.
- **Koszt ficzera:** `sync-auth` 12 pp tygodnia, `mobile-smoke-hardening` 8 pp, oba bez planu (ticket 27). Decyzja
  „zostaje Pro” (ticket 11) stoi; wróć do niej tylko, gdy dwa ficzery przestaną się mieścić w tygodniu.
- **Review gałęzi kosztował w `sync-auth` tyle co siedem ticketów** (spec padał trzy razy). To obserwacja, nie
  zmiana pętli, dopóki przebieg się nie wyłoży.
- **Reguła zamrożenia pętli** (2026-11-11): zmiany w pętli tylko po nieudanym realnym przebiegu; warstwa MAUI
  template'u (28–30, `wontfix`) zaparkowana.
- **Ticket Koszyka 08 z `sync-auth`** (`needs-triage`) i reszta ticketów Koszyka należą do Koszyka.

## 6. Środowisko i pułapki, które kosztowały czas

- **Najpierw `git log` Koszyka, potem wnioski.** 2026-11-18 ta sesja na początku powiedziała użytkownikowi, że
  przebieg się nie zaczął, na podstawie stanu sprzed dwóch dni; cały `sync-auth` był już w `main`. Zawsze odczytaj
  `git -C D:\Projects\Koszyk log --format="%h %ad %s" --date=iso -15 main` w tej samej turze, w której odpowiadasz.
- **`gh` jest na hoście od 2026-11-16:** `"/c/Program Files/GitHub CLI/gh.exe" run list --branch main --limit 6`
  uruchomiony z katalogu Koszyka pokazuje wynik CI. Nie pytaj użytkownika o CI.
- **Nie dotykaj Koszyka, gdy trwa `/ship`.** Commit na `main` w trakcie przebiegu zatrzyma bramkę 2. Czytaj tylko
  `git log`, `git worktree list` i pliki w worktree.
- **Stan przebiegu w toku jest w worktree:** `D:\Projects\Koszyk\.claude\worktrees\<slug>\.scratch\<slug>\`
  (`metrics.md`, `brief.md`, `issues/`). Po scaleniu te pliki są w `.scratch/<slug>/` na `main`. W
  `.claude\worktrees\` leżą puste katalogi po starych worktree (`ux-cosmetics-1`, `mobile-smoke-hardening`); o
  trwającym przebiegu mówi tylko `git worktree list`.
- **Czy przebieg się skończył:** commit `ship: gate 2` na `main` i brak worktree w `git worktree list`. Reflog
  `origin/main` (`git reflog show origin/main --date=iso`) pokazuje push ok. 20–60 s po commicie bramki.
- **Pauza:** commit `ship: pause` i wiersz `pause` w `metrics.md` z godziną resetu. Wiersz `metrics.md`, który
  obejmuje pauzę, nie mierzy minut ani okna 5h; tydzień tak.
- **Wznowienia ani pytania o zgodę nie widać w repozytorium.** O oba pytaj użytkownika, po jednym pytaniu.
- **Allowlista Koszyka:** `git log <commit planu>..<commit bramki 2> -- .claude/` pokazuje, czy coś do niej doszło.
- **Przesunięcie `main` template'u bez checkoutu:** `git merge-base --is-ancestor origin/main HEAD`, potem
  `git fetch . chore/agent-loop:main` i `git push origin chore/agent-loop main`. Push działa po „tak” użytkownika.

## 7. Workflow

Bez zmian. Bramka 1 po `/plan-feature`, bramka 2 po briefie `/ship`; oba polecenia uruchamia użytkownik w sesji
Koszyka. W tej sesji template'u nie działali żadni agenci w tle. Commity i pushe w template'cie tylko po słowie
użytkownika.

## 8. Lekcje metodyczne do powtórzenia

1. **Stan innego repozytorium czytaj na świeżo, w tej samej turze.** Odczyt sprzed godziny albo z poprzedniego
   handoffu dał 2026-11-18 błędną odpowiedź „nic się nie wydarzyło”.
2. **Brak zdarzenia zapisuj jako „brak obserwacji” z dowodem**, nie jako spełnione kryterium. Dzięki temu nikt nie
   odhaczy ticketu 19 na podstawie przebiegu, który go nie sprawdził.
3. **Najpierw fakty z repozytorium, potem pytania do użytkownika, po jednym.** 2026-11-18 dwie odpowiedzi („ruszył
   sam”, „nie padło”) wystarczyły do domknięcia zapisu.
4. **Kryterium, którego zdarzenie różni się od brzmienia, odhacza użytkownik.** Opisz różnice, zapytaj, odhacz z
   adnotacją po „tak” (wzór: pierwsze kryterium ticketu 19).

## 9. Pierwszy krok i miejsce zatrzymania

Zacznij od `git status --short` w template'cie, potem `git -C D:\Projects\Koszyk worktree list` i
`git -C D:\Projects\Koszyk log --format="%h %ad %s" --date=iso -15 main`.

- Jeśli od `bd0e4dc` nie przybył commit `plan: …`: następny ficzer nie jest zaplanowany. Powiedz to użytkownikowi
  (wybór ficzera i `/plan-feature` należą do sesji Koszyka) i zatrzymaj się.
- Jeśli `git worktree list` pokazuje worktree przebiegu: przebieg trwa albo czeka na bramkę 2. Podaj stan z
  `metrics.md` w worktree i zatrzymaj się. Niczego nie zapisuj w Koszyku.
- Jeśli nowy ficzer jest w `main` (nowy commit `ship: gate 2` po `5d2378f`): dopisz przebieg do ticketów 27 i 19
  (i 12, gdy padło pytanie o zgodę), sprawdź CI przez `gh`, zapytaj użytkownika o wznowienie po pauzie (jeśli była)
  i o pytanie o zgodę, po jednym. Ticketów nie zamykaj bez decyzji użytkownika.
