using System;
using Unity.Collections;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

public struct CardGameSimData
{
    public Random rand;
    public int nSims;
    public int nRoundsPerFrame;
    public int nPlayers;
    public int deckSize;
    public int handSize;
    public int maxCardsInPlay;

    public NativeArray<int> turnCount;
    public NativeArray<byte> deckCount;
    public NativeArray<ushort> decks;
    public NativeArray<byte> handCount;
    public NativeArray<ushort> hands;
    public NativeArray<byte> inPlayCount;
    public NativeArray<ushort> inPlay;

    public NativeArray<float> cardAttributes1;
    public NativeArray<float> cardAttributes2;
    public NativeArray<float> cardAttributes3;


    JobGenDeck genDeck;
    JobShuffleDeck shuffleDeck;
    JobDrawHand drawHand;
    JobPlayCard playCard;
    JobEvaluateRound evaluateRound;
    JobRemoveDeadCardFromPlay removeDeadCards;
    
    public CardGameSimData(int argNSims, int argNPlayers, int argNRoundsPerFrame, int argDeckSize, int argHandSize, int argMaxCardsInPlay)
    {
        nSims = argNSims;
        nPlayers = argNPlayers;
        nRoundsPerFrame = argNRoundsPerFrame;
        deckSize = argDeckSize;
        handSize = argHandSize;
        maxCardsInPlay = argMaxCardsInPlay;


        rand = new Random((uint)(DateTime.Now.Millisecond)+1);

        turnCount = new NativeArray<int>(nSims, Allocator.Persistent);
        deckCount = new NativeArray<byte>(nSims * nPlayers, Allocator.Persistent);
        decks = new NativeArray<ushort>(nSims * nPlayers * deckSize, Allocator.Persistent);
        handCount = new NativeArray<byte>(nSims * nPlayers, Allocator.Persistent);
        hands = new NativeArray<ushort>(nSims * nPlayers * handSize, Allocator.Persistent);
        inPlayCount = new NativeArray<byte>(nSims * nPlayers, Allocator.Persistent);
        inPlay = new NativeArray<ushort>(nSims * nPlayers * maxCardsInPlay, Allocator.Persistent);
        cardAttributes1 = new NativeArray<float>(nSims * nPlayers * maxCardsInPlay, Allocator.Persistent);
        cardAttributes2 = new NativeArray<float>(nSims * nPlayers * maxCardsInPlay, Allocator.Persistent);
        cardAttributes3 = new NativeArray<float>(nSims * nPlayers * maxCardsInPlay, Allocator.Persistent);



         genDeck = new JobGenDeck()
        {
            deckSize = deckSize,
            deckCount = deckCount,
            decks = decks
        };

         shuffleDeck = new JobShuffleDeck()
        {
            rand = rand,
            deckSize = (byte)deckSize,
            deckCount = deckCount,
            decks = decks
        };

         drawHand = new JobDrawHand()
        {
            deckSize = deckSize,
            handSize = handSize,
            deckCount = deckCount,
            decks = decks,
            handCount = handCount,
            hands = hands
        };

         playCard = new JobPlayCard()
        {
            rand = rand,
            handSize = handSize,
            maxCardsInPlay = maxCardsInPlay,
            handCount = handCount,
            hands = hands,
            inPlayCount = inPlayCount,
            inPlay = inPlay,
            cardAttributes1 = cardAttributes1,
            cardAttributes2 = cardAttributes2,
            cardAttributes3 = cardAttributes3
        };

         evaluateRound = new JobEvaluateRound()
        {
            rand = rand,
            nPlayers = nPlayers,
            maxCardsInPlay = maxCardsInPlay,
            inPlayCount = inPlayCount,
            inPlay = inPlay,
            cardAttributes1 = cardAttributes1,
            cardAttributes2 = cardAttributes2,
            cardAttributes3 = cardAttributes3
        };

         removeDeadCards = new JobRemoveDeadCardFromPlay()
        {
            maxCardsInPlay = maxCardsInPlay,
            inPlayCount = inPlayCount,
            inPlay = inPlay,
            cardAttributes1 = cardAttributes1,
            cardAttributes2 = cardAttributes2,
            cardAttributes3 = cardAttributes3
        };
    }

    public JobHandle ScheduleJobsForGameRound(JobHandle precedingGameRoundHandle)
    {
        var handle1 = genDeck.Schedule(nSims * nPlayers, 32, precedingGameRoundHandle);
        var handle2 = shuffleDeck.Schedule(nSims * nPlayers, 32, handle1);
        var handle3 = drawHand.Schedule(nSims * nPlayers, 32, handle2);
        var handle4 = playCard.Schedule(nSims * nPlayers, 32, handle3);
        var handle5 = evaluateRound.Schedule(nSims * nPlayers, 32, handle4);
        var handle6 = removeDeadCards.Schedule(nSims * nPlayers, 32, handle5);
        
        return handle6;
    }

    public void Dispose()
    {
        turnCount.Dispose();
        deckCount.Dispose();
        decks.Dispose();
        handCount.Dispose();
        hands.Dispose();
        inPlayCount.Dispose();
        inPlay.Dispose();
        cardAttributes1.Dispose();
        cardAttributes2.Dispose();
        cardAttributes3.Dispose();
    }
}