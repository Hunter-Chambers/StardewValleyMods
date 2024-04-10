using StardewValley;
using StardewValley.Tools;

namespace FishingMod.Framework
{
    internal class FishingHelper
    {
        private static bool CanHook(FishingRod fishingRod)
        {
            return fishingRod.isNibbling &&
                   !fishingRod.hit &&
                   !fishingRod.isReeling &&
                   !fishingRod.pullingOutOfWater &&
                   !fishingRod.fishCaught &&
                   !fishingRod.showingTreasure;
        }

        public static void AutoHook(FishingRod fishingRod, bool doVibrate)
        {
            if (CanHook(fishingRod))
            {
                fishingRod.timePerBobberBob = 1f;
                fishingRod.timeUntilFishingNibbleDone = FishingRod.maxTimeToNibble;
                fishingRod.DoFunction(Game1.player.currentLocation, (int)fishingRod.bobber.X, (int)fishingRod.bobber.Y, 1, Game1.player);

                if (doVibrate)
                {
                    Rumble.rumble(0.95f, 200f);
                }
            }
        }

    }
}
