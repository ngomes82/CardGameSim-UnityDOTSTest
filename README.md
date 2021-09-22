# CardGameSim

Quick test of Unity's Data Oriented Technology Stack. Using the Job System and Burst native code compilation to create a very simple card game simulation.


Current Benchmark 
----------------
(Intel i7-9700k CPU @ 3.6 GHz (8 cpus))

1.1ms per frame for 1000 games simulating 10 turns

Rounds consist of:
1) Generate Deck (if deck is empty)
2) Shuffle Deck (if deck is empty)
3) Draw Card to Hand
4) Put Random Card from Hand into play
5) Attack random opponent card in play (Each card has attack, health, defense values)
6) Discard dead cards from play
