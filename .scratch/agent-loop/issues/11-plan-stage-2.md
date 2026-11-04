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
- [ ] Historia 63 (Koszyk na pętli) jest w kryteriach ticketów: plik DoD Koszyka, hooki i ich rejestracja,
      konwencje, sprawdzenie na małym ficzerze, moduł 15 przez `/plan-feature`.
- [ ] Po pilocie 2 decyzja według reguły: mniej niż 2 ficzery po 3 tickety w tygodniowym limicie Pro oznacza
      Max 5x albo zewnętrzny implementer, w przeciwnym razie zostaje Pro (historia 48).

## Comments

**2026-11-04, decyzja użytkownika:** etap 2 obejmuje też przeniesienie Koszyka na pętlę (nowa historia 63
w specu; „Out of Scope” zostawia tylko Gym). Powód: moduł 15 Koszyka (`D:\Projects\Koszyk\docs\tasks\
state-15-mobile-polish-hardening.md`) czekał na wyłączony plugin `dotnet-workflow`. Użytkownik wybrał nową
pętlę zamiast włączania starego pluginu, więc moduł 15 rusza dopiero po przeniesieniu.
