# 01: Plik DoD i blokada zakończenia implementera

**What to build:** Projekt ma jeden plik DoD. Z niego hooki i weryfikator biorą komendy (build, pełne testy),
wymaganie Dockera, nazwę roli implementera, listę chronionych ścieżek i progi limitu. Gdy sub-agent
implementera kończy pracę, a którakolwiek komenda DoD się nie powiodła, zakończenie jest zablokowane
i implementer dostaje treść porażki. Inne role i główna sesja kończą bez blokady. To pierwszy kawałek pętli,
który działa samodzielnie: nawet ręcznie wywołany implementer nie ogłosi sukcesu na czerwonym DoD.

**Blocked by:** None (can start immediately)

**Seams:** skrypt hooka jako proces. Wejście: JSON zdarzenia hooka na stdin i plik DoD w repozytorium
przygotowanym w katalogu tymczasowym (komendy DoD w teście mogą być trywialnymi komendami z ustalonym kodem
wyjścia). Wyjście: kod wyjścia i decyzja według protokołu hooków Claude Code. Testy xUnit w nowym projekcie
testów hooków.

**Status:** ready-for-agent

- [ ] Plik DoD projektu zawiera komendę buildu, komendę pełnych testów, wymaganie Dockera, nazwę roli
      implementera, listę chronionych ścieżek i progi limitu (tygodniowy 80%, okno 5-godzinne 90%). Fork
      skilli nie zawiera żadnej z tych komend (historie 42, 59).
- [ ] Hook `SubagentStop`, zarejestrowany w ustawieniach projektu, blokuje zakończenie implementera przy
      niezerowym kodzie wyjścia którejkolwiek komendy DoD i podaje treść porażki. W tym projekcie obejmuje to
      błąd buildu, ostrzeżenie buildu (analizatory są błędami) i czerwony test (historie 21, 23).
- [ ] Ten sam hook przepuszcza zakończenie implementera, gdy wszystkie komendy DoD są zielone (historia 21).
- [ ] Hook blokuje także przy powtórnej próbie (`stop_hook_active`). Nieskończoną pętlę ogranicza `maxTurns`
      roli implementera, nie hook.
- [ ] Hook przepuszcza zakończenie, gdy `agent_type` nie jest rolą implementera z pliku DoD (weryfikator,
      review, inne sub-agenty).
- [ ] Gdy Docker jest wymagany i nie odpowiada, hook nie blokuje, tylko zwraca informację o awarii
      środowiska. Rozstrzyga orkiestrator (ticket 03).
- [ ] Hook czyta komendy i nazwę roli z pliku DoD: zmiana w pliku zmienia działanie hooka bez zmiany skryptu.
- [ ] Testy hooków są częścią `dotnet test CleanArchitecture.slnx` i przechodzą lokalnie oraz w CI na
      Ubuntu (`pwsh`).
- [ ] Build: 0 błędów, 0 ostrzeżeń, bez wyciszeń.
