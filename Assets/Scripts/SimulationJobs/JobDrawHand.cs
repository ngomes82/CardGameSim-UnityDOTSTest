using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


[BurstCompile]
public struct JobDrawHand : IJobParallelFor
{
    public int deckSize;
    public int handSize;

    [NativeDisableParallelForRestriction]
    public NativeArray<byte> deckCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> decks;

    [NativeDisableParallelForRestriction]
    public NativeArray<byte> handCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> hands;


    public void Execute(int playerIdx)
    {
        while (deckCount[playerIdx] > 0 && handCount[playerIdx] < handSize)
        {
            int deckIdx = playerIdx * deckSize;
            int handIdx = playerIdx * handSize;

            int deckTopCardIdx = deckIdx + (deckCount[playerIdx] - 1);
            int openHandIdx = handIdx + (handCount[playerIdx]);

            hands[openHandIdx] = decks[deckTopCardIdx];
            decks[deckTopCardIdx] = 0;

            deckCount[playerIdx]--;
            handCount[playerIdx]++;
        }
    }
}