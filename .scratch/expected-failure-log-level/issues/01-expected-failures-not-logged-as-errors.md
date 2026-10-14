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

**Status:** needs-triage

- [ ] Porażka typu innego niż `Failure` jest logowana poniżej poziomu Error.
- [ ] Porażka typu `Failure` nadal jest logowana jako Error.
- [ ] Test sprawdza poziom logu dla obu przypadków i pada na obecnym kodzie (red proof).
