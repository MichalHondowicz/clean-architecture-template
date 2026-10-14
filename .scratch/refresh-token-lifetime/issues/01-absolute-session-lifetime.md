# 01: Bezwzględny limit życia sesji

**What to build:** Refresh token wydany przy jednym logowaniu przestaje działać po ustalonym czasie od tego
logowania, niezależnie od tego, jak często user go odświeża; potem user loguje się ponownie. Dziś każda
rotation przesuwa termin ważności o 7 dni od wymiany, więc sesja używana co tydzień nie wygasa nigdy.
Wylogowanie i unieważnianie tokenów są poza zakresem, do osobnej decyzji.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [x] Wartość limitu ustalona przy triage: 30 dni od logowania.
- [ ] Odświeżenie po przekroczeniu limitu daje ten sam błąd co nieważny refresh token.
- [ ] Rotation działa jak dotąd w granicach limitu.
- [ ] Test z kontrolowanym czasem pokrywa wygaśnięcie po limicie mimo regularnych odświeżeń.

## Comments

**Triage (2026-10-14), decyzja użytkownika (bramka T2):** limit 30 dni od logowania, `ready-for-agent`.

Stan kodu sprawdzony przy triage: `RefreshToken` nie zapisuje momentu logowania, bo rotation nadpisuje `Token`
i `ExpiresOnUtc` w tym samym wierszu (`RefreshTokenCommandHandler`: `ExpiresOnUtc = UtcNow + 7 dni`). Limit
wymaga więc nowego pola istniejącej encji i migracji (`add-entity`). „Sesja” w tytule to ciąg rotacji od
jednego logowania, nie synonim refresh tokenu (`GLOSSARY.md` unika „Session”). Wpis „Refresh token” w
`GLOSSARY.md` mówi dziś, że termin ważności przesuwa się od chwili wymiany; zmienia się razem z tym ticketem.
