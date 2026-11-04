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

## Comments

**2026-11-04, wynik:** Koszyk, gałąź `chore/agent-loop`, commit `53bfc1c`.

- **AC1.** `.claude/skills/add-screen/SKILL.md` (skill wywoływany przez model, z opisem, bo implementer `/ship`
  ma po niego sięgać sam). Siedem kroków: źródło danych (store i Outbox albo `IApiClient`), test ViewModelu
  najpierw, ViewModel w `Koszyk.Mobile.Core`, strona XAML z `x:DataType`, rejestracja w `MauiProgram`
  i `AppShell`, strażnik źródła dla pułapki głowy, kryterium done. Ekran wzorcowy: Members (`MembersViewModel`,
  `MembersPage`, `MembersViewModelTests`), najmniejszy: Profile. Napisany według `writing-for-agents`: kroki
  z kryterium końca, pułapki jako osobna sekcja referencyjna, reszta pułapek głowy przez wskaźnik do
  `AGENTS.md`.
- **AC2.** Sekcja „Traps confirmed on the emulator”: `TapGestureRecognizer.Tapped`, `CheckBox.CheckedChanged`
  przy recyklingu, `RefreshView` i `IsRefreshing`, transient `DelegatingHandler` i `AppShell`, realny
  `StrongReferenceMessenger` w testach, do tego `IQueryAttributable` zamiast `[QueryProperty]` dla `Guid`
  (komentarze w `ListDetailPage.xaml.cs` i `MembersPage.xaml.cs`). Każda pułapka wskazuje kod z działającą
  formą albo test, który jej pilnuje. Pozostałe pułapki głowy (biały ekran, cleartext, `pm clear`) zostają
  w `AGENTS.md`.
- **AC3.** 36 wzorców z przykładów i nazw w skillu sprawdzonych `git grep -F` w `src` i `tests`: każdy ma
  co najmniej jedno trafienie. Jedna poprawka po sprawdzeniu: test `ApiClient` tworzy `HttpClient` w helperze
  `Build()` z `BaseAddress`, a pierwsza wersja opisywała to inaczej. Rozjazdy z pluginem
  `implementing-maui-mvvm` rozstrzygnięte według kodu Koszyka: ViewModel w `Koszyk.Mobile.Core`, nie w głowie;
  bez bazowego ViewModelu (dziedziczy `ObservableObject`); bez `INavigation` w ViewModelu (flaga, np.
  `AccessLost`, a nawigacja w stronie); `RefreshView` nie na `IsBusy`; serwis z `IApiClient` transient, nie
  singleton; błąd jako polskie zdanie w `StatusMessage`, nie `ex.Message`; dane offline przez store i Outbox.
- **AC4.** `AGENTS.md` Koszyka, „Process skills in this project”: `add-screen` dla każdego nowego albo
  przebudowanego ekranu (strona, popup, ViewModel, nowe wywołanie API z aplikacji).
- Zmiana dotyczy tylko dokumentów (skill i `AGENTS.md`), bez kodu, więc buildu i testów nie powtarzałem;
  ostatni przebieg na `0ccbedb`: 0 ostrzeżeń, 99/5/130/587. `.claude/**` jest w `protectedPaths`, więc
  implementer skilla nie zmieni. Tekst skilla sprawdzi pilot (ticket 26).
