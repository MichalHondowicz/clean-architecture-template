# 01: Oczekiwana porażka handlera nie jest logowana jako błąd

**What to build:** `LoggingDecorator` (`src/Application/Abstractions/Behaviors/LoggingDecorator.cs`) loguje
każdą porażkę handlera przez `LogError("Completed ... with error")`, bez względu na `ErrorType`. Walidacja,
`NotFound`, `Conflict` i od ADR 0005 także `Forbidden` to porażki przewidziane w use case'ie (ADR 0002), więc
zaśmiecają logi i monitoring jako błędy, choć zawinił klient. Po tickecie
`.scratch/ownership-forbidden/issues/01-ownership-mismatch-returns-403.md` request log zapisuje 403 na poziomie
Information, ale ten sam wpis z dekoratora nadal ma poziom Error. Do rozstrzygnięcia przy triage: jaki poziom
dostaje porażka danego typu (np. `Failure` jako Error, pozostałe jako Warning lub Information) i czy decyzja
wymaga ADR-a.

**Blocked by:** None (can start immediately)

**Status:** done

- [x] Porażka typu innego niż `Failure` jest logowana jako Warning (ADR 0007).
- [x] Porażka typu `Failure` nadal jest logowana jako Error.
- [x] Test sprawdza poziom logu dla obu przypadków i pada na obecnym kodzie (red proof).

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** `Failure` → Error, pozostałe typy (`Validation`,
`Problem`, `NotFound`, `Conflict`, `Forbidden`) → Warning. Decyzja dotyczy wszystkich handlerów, więc jest
zapisana jako ADR 0007; pierwsze kryterium zawężone z „poniżej Error” do Warning. `ready-for-agent`.

Stan kodu sprawdzony przy triage: `LoggingDecorator` ma trzy warianty (komenda z odpowiedzią, komenda bez
odpowiedzi, query) i każdy woła `LogError` przy porażce, więc zmiana obejmuje wszystkie trzy. Poziomu logu nie
widać przez HTTP; seam to test dekoratora.

**Wdrożenie (2026-10-14):** kroki `/implement` wykonane bezpośrednio (polecenie użytkownika „zrób wszystko
kompleksowo”; `/implement` uruchamia tylko użytkownik): `tdd`, `code-review` (tylko oś Spec, reguła F4), potem
`ca-review` gałęzi.

Decyzje użytkownika przed pierwszym testem: S1, testy unit trzech wariantów `LoggingDecorator` (query, komenda
z odpowiedzią, komenda bez odpowiedzi); Theory po pięciu typach innych niż `Failure` oczekuje Warning, Fact
dla `Failure` oczekuje Error. Logger testowy: `FakeLogger<T>` z `Microsoft.Extensions.Diagnostics.Testing`
10.11.0 (nowy pakiet testów przez CPM, objęty grupą Dependabota `microsoft-dotnet`). Asercja na
`LatestRecord`, bo dekorator pisze najpierw wpis „Processing” (Information).

Cykle red → green, po dwa na wariant (query, komenda z odpowiedzią, komenda bez odpowiedzi):

1. Theory Warning: red (Error zamiast Warning na obecnym kodzie), green minimalny `LogWarning`.
2. Fact `Failure`: red (Warning zamiast Error na kodzie z kroku 1), green: poziom zależny od typu błędu.

Po sześciu cyklach refaktor: warunek powtórzony trzy razy przeszedł do jednej metody
`LoggingDecorator.FailureLevel(Error)` z odwołaniem do ADR 0007. Red proof na niej (warunek `Failure`
osłabiony: `&& error.Code.Length == 0`): padły wszystkie trzy testy `Failure`, więc każdy wariant z niej
korzysta; hash SHA-256 pliku przed złamaniem równy hashowi po przywróceniu. Kryterium 3 („pada na obecnym
kodzie”) dotyczy z natury testów Warning (15 przypadków czerwonych na `main`); testy `Failure` bronią
zachowania, które zostaje, więc ich red to cykle i red proof na `FailureLevel`.

Pełny przebieg na świeżym buildzie gałęzi (Docker 29.8.1): build 0 błędów, 0 ostrzeżeń; UnitTests 54/54
(36 + 18), ArchitectureTests 6/6, IntegrationTests 14/14.

`code-review`, oś Spec (diff `git diff HEAD` z `git add -N` dla nowego pliku testów): 0 defektów. Sprawdzone:
porażka walidacji też trafia do dekoratora logowania (jest zewnętrzny, `ValidationError` ma typ `Validation`,
więc Warning); ścieżka sukcesu dalej loguje Information; właściwość `Error` dalej trafia do kontekstu logu.
Ryzyko domknięte w tym tickecie: nowy `ErrorType` dostałby Warning bez niczyjej decyzji, a ADR 0007 wymieniał
pięć typów jak zamkniętą listę. ADR 0007 dostał zdanie: nowy typ domyślnie Warning, a typ oznaczający awarię
serwera trzeba dopisać obok `Failure` w `FailureLevel` i w testach.

**`ca-review` gałęzi (2026-10-14), scope `git diff 92b271d` (commit `2db3ed8`), `.agents/` wyłączone:** blockers
0, convention violations 0, test gaps 0; każde kryterium ma implementację i test; nowy pakiet zgodny z CPM.
Uwagi bez wagi blokera: powtórzony Arrange w sześciu testach (czytelność testu); publiczne typy testowe
`TestQuery`, `TestCommand`, `TestCommandWithoutResponse` do ponownego użycia przy kolejnych testach dekoratorów.
Przebieg na świeżym buildzie: build 0/0; UnitTests 54/54, ArchitectureTests 6/6, IntegrationTests 14/14.
Werdykt: gotowe.

Reguła F4 (czwarty przebieg): oś Spec znalazła jedyne istotne ryzyko (nowy `ErrorType`), `ca-review` tylko
uwagi o stylu testów.
