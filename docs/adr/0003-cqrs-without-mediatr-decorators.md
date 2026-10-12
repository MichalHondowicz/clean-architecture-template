# Własne handlery CQRS zamiast MediatR, przekroje przez dekoratory

Use case'y implementują własne `ICommandHandler<>`, `ICommandHandler<,>` i `IQueryHandler<,>`, a endpoint
wstrzykuje interfejs konkretnego handlera, zamiast wysyłać wiadomość przez mediator. Zależność endpointu od
use case'u jest jawna w jego sygnaturze i nie wymaga zewnętrznej biblioteki; ceną jest brak pipeline'u
MediatR, który zastępują dekoratory.

## Konsekwencje

- Scrutor rejestruje handlery skanem assembly i nakłada dekoratory w `src/Application/DependencyInjection.cs`:
  `ValidationDecorator` (tylko komendy) i `LoggingDecorator` (komendy i zapytania). Handler rejestrowany
  ręcznie dostałby drugą rejestrację obok skanu.
- Nowy przekrój (np. transakcja) to nowy dekorator w tym samym pliku. Kolejność `Decorate` ma znaczenie:
  dekorator dopisany później opakowuje wcześniejsze, więc dziś logowanie obejmuje walidację.
- Zapytania nie przechodzą przez walidację; walidator napisany dla zapytania nigdy się nie wykona.
