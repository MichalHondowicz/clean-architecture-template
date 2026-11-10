# 32: Krok CI bramki 2 przy panelu przeglądarki wylogowanym z GitHuba

**What to build:** Gdy panel przeglądarki aplikacji nie widzi przebiegów CI, bo przy prywatnym repozytorium jest
wylogowany z GitHuba, bramka 2 `/ship` od razu daje `CI not checked` i mówi, że trzeba się zalogować. Nie czeka
wtedy do 20 minut.

**Blocked by:** None

**Seams:** `ship/SKILL.md` (krok 10 bramki 2) i strona `docs/engineering/ship.md` w forku; przewodnik
`docs/workflow.md`; obserwacja przy pierwszej bramce 2 z wylogowanym panelem.

**Status:** done

**Skąd:** przegląd stanu 2026-11-10. Panel przeglądarki był wylogowany z GitHuba (pusty meta `user-login`, link
„Sign in”). Strona Actions prywatnego Koszyka odpowiadała „Page not found” (HTTP 404), nie stroną logowania.
Krok 10 wymieniał jako `CI not checked` tylko stronę logowania. 404 mógł więc zostać wzięty za przebieg, który
jeszcze się nie pojawił, co kosztowałoby 10 odczytów co 2 minuty. Koszyk zapisał ten sam objaw w swoim `AGENTS.md`
(`fee4e52`). Decyzja użytkownika: „Tak, popraw”.

- [x] AC1: Strona bez listy przebiegów workflow (GitHubowe „Page not found” albo strona logowania) daje od razu
      `CI not checked` z linkiem, bez czekania.
- [x] AC2: Raport końcowy mówi wtedy, że użytkownik ma się zalogować do GitHuba w panelu przeglądarki aplikacji.
- [x] AC3: Strona forka i przewodnik opisują, że przy prywatnym repozytorium panel musi być zalogowany i że loguje
      się w nim użytkownik.
- [x] Wersja forka podbita.

## Comments

**2026-11-10, wynik:** fork `c5d3193` (plugin `1.3.1-dotnet.32`), przewodnik w tym samym commicie co ten ticket.
Krok 10 liczy teraz jako stronę z przebiegami tylko stronę z listą przebiegów workflow. „Page not found” albo
strona logowania daje od razu `CI not checked`, a raport mówi o zalogowaniu. Zdanie „a sign-in page instead of the
runs” zniknęło z warunku końcowego, bo ten przypadek obsługuje wcześniejsze zdanie. Bez zmian zostają:
- czekanie na przebieg, który jest w kolejce, trwa albo jeszcze się nie pojawił;
- limit 10 odczytów.

**Niesprawdzone w przebiegu:** pierwsza bramka 2 z wylogowanym panelem. Koszyk dostanie ją po ticketach 24 i 26,
o ile użytkownik się wcześniej nie zaloguje.
