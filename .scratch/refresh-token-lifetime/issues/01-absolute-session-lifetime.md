# 01: Bezwzględny limit życia sesji

**What to build:** Refresh token wydany przy jednym logowaniu przestaje działać po ustalonym czasie od tego
logowania, niezależnie od tego, jak często user go odświeża; potem user loguje się ponownie. Dziś każda
rotation przesuwa termin ważności o 7 dni od wymiany, więc sesja używana co tydzień nie wygasa nigdy.
Wylogowanie i unieważnianie tokenów są poza zakresem, do osobnej decyzji.

**Blocked by:** None (can start immediately)

**Status:** needs-triage

- [ ] Wartość limitu ustalona przy triage.
- [ ] Odświeżenie po przekroczeniu limitu daje ten sam błąd co nieważny refresh token.
- [ ] Rotation działa jak dotąd w granicach limitu.
- [ ] Test z kontrolowanym czasem pokrywa wygaśnięcie po limicie mimo regularnych odświeżeń.
