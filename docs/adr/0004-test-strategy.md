# Strategia testów: handler na EF InMemory, baza relacyjna przez HTTP

Logikę handlera sprawdzają testy unit na `TestDbContext` (EF Core InMemory przez `BaseHandlerTest`),
z NSubstitute tylko na interfejsach granicy warstwy z ADR 0001; `DbContext` nie jest mockowany. Wszystko, co
zależy od bazy relacyjnej (unikalność, klucze obce i kaskady, transakcje, migracje, tłumaczenie zapytań przez
Npgsql), sprawdzają testy integracyjne przez HTTP na PostgreSQL 17 w Testcontainers. Test przez HTTP jest
preferowanym seamem dla `tdd`, bo obejmuje cały use case od żądania do odpowiedzi.

## Konsekwencje

- Zielony test unit nic nie mówi o ograniczeniach bazy, bo InMemory ich nie egzekwuje. Testy integracyjne
  odtwarzają migracje (`MigrateAsync`). Zmiana modelu bez migracji kończy się przy starcie
  `PendingModelChangesWarning`, więc pada cały suite integracyjny.
- Test unit handlera nie łamie zasady `tdd` „testuj przez interfejs, nie przez wnętrze”: endpoint tylko
  mapuje żądanie na komendę, więc handler jest wejściem use case'u. Testy unit pokrywają kombinatorykę ścieżek
  porażki, której nie opłaca się przechodzić przez HTTP.
- Każdy nowy lub zmieniony handler ma test unit na każdą ścieżkę porażki i na ścieżkę sukcesu (checklista
  `ca-review`). Test HTTP to drugi seam, a nie zastępstwo testu handlera.
- Testy integracyjne wymagają Dockera. Bez niego praca nie jest zielona, tylko niezweryfikowana.
