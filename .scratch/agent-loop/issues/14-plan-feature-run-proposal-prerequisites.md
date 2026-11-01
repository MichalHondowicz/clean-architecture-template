# 14: Propozycja `/ship` w `/plan-feature` nie każe przygotowywać środowiska

**What to build:** Komunikat, którym `/plan-feature` proponuje start `/ship`, nie każe użytkownikowi niczego
uruchamiać ani włączać, co `/ship` robi sam albo co aplikacja ustawia sama.

**Blocked by:** None

**Seams:** `plan-feature/SKILL.md` w forku, krok 9; obserwacja w kolejnym przebiegu `/plan-feature`.

**Status:** done

**Skąd:** pilot 10, przebieg 1 (`todo-comments`, 2026-10-29). Na koniec `/plan-feature` napisał „Testy
integracyjne potrzebują działającego Dockera” i „Remote Control włącz w niej sam”. Krok 9 skilla żadnego z tych
zdań nie przewiduje. Użytkownik uruchomił Docker Desktop o 21:32:18, 40 s przed startem sesji `/ship`, więc
`environmentChecks` w `dod.json` nie miały czego uruchamiać, a scenariusz (b) pilota nie został sprawdzony.
Zdanie o Remote Control przeczy przewodnikowi: ustawienie aplikacji „Connect new sessions to Remote Control”
łączy nowe sesje samo.

- [x] Krok 9 `plan-feature/SKILL.md` mówi, że środowisko (`environmentChecks`) przygotowuje `/ship`, i nie
      każe go przygotowywać użytkownikowi.
- [x] Remote Control: komunikat mówi, że aplikacja łączy nową sesję sama (ustawienie „Connect new sessions to
      Remote Control”), a użytkownik sprawdza tylko plakietkę Remote Control w sesji `/ship`. Bez polecenia
      włączenia.
- [x] Wersja forka podbita.

## Comments

**2026-11-01, triage (decyzja użytkownika):** `ready-for-agent`, trzeci w kolejce. Treść zdania o Remote
Control ustalona jak w drugim kryterium.

**2026-11-01, wynik:** fork `023881e` (plugin `1.3.1-dotnet.23`, zainstalowany). Krok 9 `plan-feature/SKILL.md`
dostał akapit: poza komendą nie trzeba niczego przygotowywać. `/ship` sam sprawdza środowisko i uruchamia to,
co ma `start` w `environmentChecks`, więc komunikat nie każe uruchamiać Dockera ani innej usługi. Usługa
uruchomiona ręcznie ukrywa, czy pętla umie ją uruchomić. O Remote Control: sesja go potrzebuje, aplikacja łączy
nową sesję sama przy ustawieniu „Connect new sessions to Remote Control”, a użytkownik sprawdza tylko
plakietkę. Review Spec (Sonnet): `pass` bez findings. W `plan-feature.md` i `docs/workflow.md` nie ma innych
poleceń przygotowania przed `/ship`. Obserwacja na żywo: następny `/plan-feature`.
