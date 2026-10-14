# Oczekiwane porażki jako Result, nie wyjątki

Handler zwraca `Result` / `Result<T>` z `Error` (kod `"{Feature}.{Reason}"` plus `ErrorType`) zamiast rzucać
wyjątek dla porażki przewidzianej w use case'ie. Ścieżki porażki są wtedy widoczne w sygnaturze i testowalne
bez łapania wyjątków, a tłumaczenie na HTTP ma jedno miejsce: `CustomResults.Problem`.

## Konsekwencje

- Mapowanie: `Validation` i `Problem` → 400, `NotFound` → 404, `Conflict` → 409, `Forbidden` → 403, `Failure` → 500. `Failure`
  ukrywa kod i opis błędu (tytuł „Server failure”), więc nie nadaje się na porażkę, którą klient ma zrozumieć.
- Wyjątek zostaje dla błędu programisty i awarii (np. odczyt `Result.Value` z porażki).
- `Forbidden` służy cudzemu `userId` podanemu w żądaniu (ADR 0005). Wariantu dla 401 nie ma: żądanie bez ważnego
  tokenu odrzuca uwierzytelnianie, zanim dotrze do handlera.
