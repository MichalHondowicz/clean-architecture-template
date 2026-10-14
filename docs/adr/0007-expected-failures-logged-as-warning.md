---
status: accepted
---

# Oczekiwana porażka handlera logowana jako Warning, `Failure` jako Error

`LoggingDecorator` loguje wynik każdego handlera, więc ustala poziom logu porażek dla całej aplikacji. Porażka
typu `Failure` to awaria po stronie serwera i zostaje na poziomie Error. Pozostałe typy (`Validation`,
`Problem`, `NotFound`, `Conflict`, `Forbidden`) to porażki przewidziane w use case'ie (ADR 0002), zawinione
przez klienta; logujemy je jako Warning. Error w logach znaczy wtedy „do naprawy po stronie serwera”, a porażki
klienta nadal da się znaleźć i policzyć. Wdrożenie: `.scratch/expected-failure-log-level/`.

## Considered options

- Każda porażka jako Error (stan przed tą decyzją): odrzucone, bo błąd klienta zaśmieca logi i monitoring jako
  awaria serwera (ten sam argument co w ADR 0005).
- Porażki klienta jako Information: odrzucone, bo zlewają się z wpisami o udanych żądaniach, a nagły wzrost
  (zepsuty klient, próby enumeracji) ma być widoczny na osobnym poziomie.
