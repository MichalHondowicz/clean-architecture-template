# Interfejsy w Application wynikają z odwrócenia zależności między warstwami

`Application` nie może zależeć od `Infrastructure` (pilnuje tego `ArchitectureTests`), więc każda zdolność
infrastruktury, której potrzebuje handler, ma interfejs po stronie `Application` (`IApplicationDbContext`,
`IPasswordHasher`, `ITokenProvider`, `IUserContext`) albo `SharedKernel` (`IDateTimeProvider`) i jedną
implementację w `Infrastructure`. Taki interfejs jest granicą warstwy, a nie hipotetycznym punktem zmienności,
więc pojedyncza implementacja nie jest powodem, żeby go usunąć.

## Konsekwencje

- Nowa zdolność infrastruktury dla handlera: interfejs w `src/Application/Abstractions/`, implementacja
  i rejestracja w `Infrastructure`.
- Te same interfejsy są granicą mockowania w testach unit (patrz ADR 0004).
- Reguła „jeden adapter to hipotetyczny seam” obowiązuje dalej wewnątrz jednej warstwy: interfejs, którego
  obie strony leżą w tej samej warstwie, potrzebuje drugiego adaptera, żeby istnieć.
