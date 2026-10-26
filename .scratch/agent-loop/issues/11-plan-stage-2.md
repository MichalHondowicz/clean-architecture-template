# 11: Rozpisanie etapu 2: warstwa MAUI

**What to build:** Po pilocie 1 etap 2 zostaje rozpisany na tickety: warstwa mobilna według wzorca Koszyka,
`add-screen`, DoD mobilny, konwencje mobilne, CI z Androidem i pilot 2 z pomiarem nr 2. Po pilocie 2 zapada
decyzja o planie według reguły pomiaru.

**Blocked by:** 10

**Seams:** brak (ticket planistyczny).

**Status:** ready-for-human

- [ ] Tickety etapu 2 są opublikowane w trackerze, każdy z `Seams:` i kryteriami `AC1…n`.
- [ ] Historie 49–56 i 58 ze speca są w kryteriach tych ticketów: warstwa `Mobile`, `Mobile.Core`
      i `Contracts`; `Mobile.Core` bez zależności od MAUI, z regułą w testach architektury; `Contracts`
      współdzielone z Web.Api; `add-screen`; DoD mobilny z buildem `net10.0-android` i `x:DataType`; zmienione
      ekrany w briefie; konwencje mobilne w `AGENTS.md`; build Androida w CI; pilot 2 full-stack z ekranem.
- [ ] Wnioski z pilota 1, które zmieniają kształt pętli, są uwzględnione przed rozpisaniem.
- [ ] Po pilocie 2 decyzja według reguły: mniej niż 2 ficzery po 3 tickety w tygodniowym limicie Pro oznacza
      Max 5x albo zewnętrzny implementer, w przeciwnym razie zostaje Pro (historia 48).
