# 11: Rozpisanie etapu 2: warstwa MAUI

**What to build:** Po pilocie 1 etap 2 zostaje rozpisany na tickety: warstwa mobilna według wzorca Koszyka,
`add-screen`, DoD mobilny, konwencje mobilne, CI z Androidem i pilot 2 z pomiarem nr 2. Po pilocie 2 zapada
decyzja o planie według reguły pomiaru.

**Blocked by:** 10

**Seams:** brak (ticket planistyczny).

**Status:** ready-for-human

- [x] Tickety etapu 2 są opublikowane w trackerze, każdy z `Seams:` i kryteriami `AC1…n`.
- [x] Historie 49–56 i 58 ze speca są w kryteriach tych ticketów: warstwa `Mobile`, `Mobile.Core`
      i `Contracts`; `Mobile.Core` bez zależności od MAUI, z regułą w testach architektury; `Contracts`
      współdzielone z Web.Api; `add-screen`; DoD mobilny z buildem `net10.0-android` i `x:DataType`; zmienione
      ekrany w briefie; konwencje mobilne w `AGENTS.md`; build Androida w CI; pilot 2 full-stack z ekranem.
- [x] Wnioski z pilota 1, które zmieniają kształt pętli, są uwzględnione przed rozpisaniem.
- [x] Historia 63 (Koszyk na pętli) jest w kryteriach ticketów: plik DoD Koszyka, hooki i ich rejestracja,
      konwencje, sprawdzenie na małym ficzerze, moduł 15 przez `/plan-feature`.
- [ ] Po pilocie 2 decyzja według reguły: mniej niż 2 ficzery po 3 tickety w tygodniowym limicie Pro oznacza
      Max 5x albo zewnętrzny implementer, w przeciwnym razie zostaje Pro (historia 48).

## Comments

**2026-11-04, decyzja użytkownika:** etap 2 obejmuje też przeniesienie Koszyka na pętlę (nowa historia 63
w specu; „Out of Scope” zostawia tylko Gym). Powód: moduł 15 Koszyka (`D:\Projects\Koszyk\docs\tasks\
state-15-mobile-polish-hardening.md`) czekał na wyłączony plugin `dotnet-workflow`. Użytkownik wybrał nową
pętlę zamiast włączania starego pluginu, więc moduł 15 rusza dopiero po przeniesieniu.

**2026-11-04, fakty z Koszyka przed bramką** (`D:\Projects\Koszyk`, `main` `8251296`):

- Konwencje backendu inne niż w template'cie: wyjątki (`KoszykException`) zamiast `Result`, Carter, Aspire,
  Keycloak, warstwy `Core → Application → Infrastructure → Api`. Skille projektu template'u do niego nie pasują.
- Nie ma projektu `Contracts`: `Mobile.Core` ma własne modele. Spec opisywał `Contracts` jako część wzorca
  Koszyka, a ten element w Koszyku nie istnieje.
- Nie ma remote (`git remote -v` pusty) ani CI. Dwie solucje: `Koszyk.sln` (backend) i `Koszyk.Mobile.sln`
  (`Koszyk.Mobile` `net10.0-android`, `Koszyk.Mobile.Core` `net10.0`, `Koszyk.Mobile.Tests`).
- Bez `Directory.Build.props` i analizatorów jako błędów. Build 0/0, testy mobilne 587/587. Testy integracyjne
  backendu używają EF InMemory i `WebApplicationFactory`, bez Testcontainers.
- XAML: 11 plików, `x:DataType` w 7. Pułapki głowy MAUI w `AGENTS.md` wymagają sprawdzenia na emulatorze.

**2026-11-04, bramka (decyzja użytkownika: „Tak” na rekomendacje 1–9):**

1. Kolejność: najpierw Koszyk na pętli, jego mały ficzer to pilot 2, potem moduł 15. Warstwa MAUI w template'cie
   na końcu, ze sprawdzonej konfiguracji Koszyka. Zmienia kolejność etapów ze speca (historie 57–58).
2. Remote Koszyka: prywatne repozytorium na GitHubie, zakłada je użytkownik.
3. Konwencje Koszyka zostają jego własne; `AGENTS.md` Koszyka jest standardem recenzenta (bez `standardsSkill`).
4. DoD Koszyka: build obu solucji z `-warnaserror`, testy backendu i mobilne, Docker tylko gdy potrzebny,
   chronione ścieżki, `screenPaths` z plikami do sprawdzenia na emulatorze.
5. Testy architektury w Koszyku (reguła warstw i `Mobile.Core` bez MAUI).
6. Hooki w Koszyku jako kopie z template'u, bez `HookTests`; template zostaje testowanym źródłem.
7. `add-screen` najpierw w Koszyku, potem w template'cie.
8. Moduł 15 dzielony na kilka ficzerów; pilotem są dwie lub trzy pozycje kosmetyki UX.
9. Warstwa MAUI template'u: jedna solucja, `Contracts` jako nowy element (bez przenoszenia do Koszyka),
   przykład logowanie i lista todo, analizatory jak w backendzie bez wyciszeń, CI na Ubuntu z `maui-android`.

**Tickety** (historie w nawiasach):

| Ticket | Repozytorium | Status | Blokowany przez |
|---|---|---|---|
| 20 remote Koszyka (63) | Koszyk | `ready-for-human` | — |
| 21 warstwa procesu Koszyka (55, 63) | Koszyk | `ready-for-agent` | — |
| 22 testy architektury Koszyka (50) | Koszyk | `ready-for-agent` | — |
| 23 pętla w Koszyku: DoD, kontrakt, hooki (53, 54, 63) | Koszyk | `ready-for-agent` | 21, 22 |
| 24 CI Koszyka z Androidem (56) | Koszyk | `ready-for-agent` | 20 |
| 25 `add-screen` w Koszyku (52) | Koszyk | `ready-for-agent` | 21 |
| 26 pilot 2 w Koszyku (54, 58, 63) | Koszyk | `ready-for-human` | 20, 23, 24, 25 |
| 27 moduł 15 jako ficzery (63) | Koszyk | `ready-for-human` | 26 |
| 28 warstwa MAUI template'u (49, 50, 51) | template | `ready-for-agent` | 26 |
| 29 `add-screen` i konwencje mobilne template'u (52, 55) | template | `ready-for-agent` | 28 |
| 30 DoD mobilny i CI template'u (53, 54, 56) | template | `ready-for-agent` | 28 |

Wnioski z pilota 1 (kryterium 3) poszły w tickety 12–19 przed rozpisaniem. Ostatnie kryterium tego ticketu
(decyzja o planie po pilocie 2) czeka na ticket 26.
