---
status: accepted
---

# Bez .NET Aspire: lokalne środowisko to docker-compose, testy to Testcontainers

Template i projekty z niego nie mają projektu AppHost ani pakietów `Aspire.*`. Lokalnie API, PostgreSQL i Seq
startuje `docker-compose.yml`, testy integracyjne biorą bazę z Testcontainers, a telemetria idzie przez
OpenTelemetry (OTLP) i Serilog do Seq. Jedna usługa z jedną bazą nie potrzebuje orkiestratora, a pętla bez
nadzoru potrzebuje środowiska, które startuje przewidywalnie i nie wymaga wyciszeń analizatora. Agent nie dodaje
Aspire sam ani „przy okazji” ficzera. Projekt, który naprawdę go potrzebuje (kilka usług, które muszą się
odnajdywać, albo wdrożenie przez `azd`), zapisuje to jako własny ADR zastępujący ten, z decyzją użytkownika.

Koszyk (`D:\Projects\Koszyk`) powstał wcześniej, ma AppHost z Keycloakiem i zostaje na Aspire; ta decyzja go nie
dotyczy.

## Considered options

- Aspire jako domyślny element template'u: odrzucone po doświadczeniu z Koszyka (2026-11). Diagnostyka
  eksperymentalnego API (`ASPIRECERTIFICATES001`) wymusiła `#pragma` i zderzyła się z zakazem wyciszeń; Aspire
  13.4 sam wstrzyknął dev-cert, przez co emulator przestał się logować; pierwszy start Keycloaka trwał około
  10 minut bez logów; AppHost wymagał własnego projektu testów. Dashboard służy człowiekowi, agent czyta wyjście
  komend.
- Dwa warianty template'u (z Aspire i bez): odrzucone, bo skille i DoD musiałyby opisywać oba.
