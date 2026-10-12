# Domain events publikowane po zapisie, poza transakcją komendy

`ApplicationDbContext.SaveChangesAsync` zbiera zdarzenia z encji, zapisuje zmiany i dopiero potem wywołuje
handlery zdarzeń, w tym samym procesie. Zapis use case'u nie zależy więc od skutków ubocznych; ceną jest
eventual consistency między zapisem a tym, co robią handlery zdarzeń.

## Konsekwencje

- Handler zdarzenia nie dzieli transakcji z komendą: jego porażka nie cofa zapisu. Wyjątek z niego wraca
  jednak do klienta jako błąd żądania, choć dane są już zapisane.
- Zdarzenia żyją tylko w pamięci. Skutek uboczny, który musi się wydarzyć (np. email), wymaga outboxa;
  obecny mechanizm go nie gwarantuje.
- Zdarzenie niesie id encji, więc id musi być nadane przed `Raise(...)`.
