using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public struct JobShuffleDeck : IJobParallelFor
{
    public Random rand;
    public byte deckSize;

    [NativeDisableParallelForRestriction]
    public NativeArray<byte> deckCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> decks;

    public void Execute(int playerIdx)
    {
        if (deckCount[playerIdx] == 0)
        {
            int deckIdx = playerIdx * deckSize;
            int n = deckSize;
            for (int i = 0; i < n - 1; i++)
            {
                int r = i + rand.NextInt(n - i);

                int rCardIdx = r + deckIdx;
                int iCardIdx = i + deckIdx;

                ushort t = decks[rCardIdx];
                decks[rCardIdx] = decks[iCardIdx];
                decks[iCardIdx] = t;
            }

            deckCount[playerIdx] = deckSize;
        }
    }
}