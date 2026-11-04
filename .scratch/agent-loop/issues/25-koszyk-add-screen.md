# 25: Koszyk: skill `add-screen`

**What to build:** Skill projektu `add-screen` w Koszyku: ViewModel w `Koszyk.Mobile.Core`, strona XAML
z `x:DataType` w `Koszyk.Mobile`, typowany klient HTTP, rejestracja w `MauiProgram` i testy w
`Koszyk.Mobile.Tests`. Implementer dodaje ekrany według jednej konwencji, takiej jak w kodzie Koszyka.

**Blocked by:** 21

**Seams:** tekst skilla sprawdza pilot (ticket 26); przykłady w skillu zgodne z kodem (`git grep` na wzorcach).

**Status:** ready-for-agent

**Skąd:** ticket 11, decyzja 7 z bramki 2026-11-04 (historia 52). Wiedza z
`D:\Projects\dotnet-workflow\plugins\dotnet-workflow\skills\implementing-maui-mvvm\SKILL.md` i z pułapek
w `AGENTS.md` Koszyka.

- [ ] AC1: `.claude/skills/add-screen/SKILL.md` opisuje kroki od ViewModelu do rejestracji i testów, z
      przykładami wziętymi z istniejącego ekranu Koszyka (nie z pluginu), napisany według `writing-for-agents`.
- [ ] AC2: Skill zawiera pułapki potwierdzone w E2E Koszyka (`TapGestureRecognizer.Tapped`, `CheckBox` przy
      recyklingu, `RefreshView`, transient `DelegatingHandler` i `AppShell`, realny `IMessenger` w testach),
      bez kopiowania całego `AGENTS.md`.
- [ ] AC3: Każdy wzorzec w przykładach ma odpowiednik w kodzie (`git grep`), a rozjazd wzorca pluginu
      z Koszykiem rozstrzyga kod Koszyka.
- [ ] AC4: `AGENTS.md` Koszyka wskazuje `add-screen` dla każdego nowego ekranu.
