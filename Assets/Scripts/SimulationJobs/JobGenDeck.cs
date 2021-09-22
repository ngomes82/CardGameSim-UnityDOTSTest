using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobGenDeck : IJobParallelFor
{
    public int deckSize;

    [ReadOnly]
    [NativeDisableParallelForRestriction]
    public NativeArray<byte> deckCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> decks;

    public void Execute(int playerIdx)
    {
        if (deckCount[playerIdx] == 0)
        {
            int deckIdx = playerIdx * deckSize;

            //Choose arbitrary ascending id for now. Later seed this with actual deck cards to simulate
            for (int i = 0; i < deckSize; i++)
            {
                int cardIdx = deckIdx + i;
                decks[cardIdx] = (ushort)(i + 1); 
            }

            //TODO: When this is executed we should also clear out our hand and cards in play.
            //What to do about opposing player?
            //there should probably be an "end game check job" or something that handles this.
        }
    }
}