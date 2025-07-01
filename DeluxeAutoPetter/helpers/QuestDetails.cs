using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace DeluxeAutoPetter.helpers
{
    internal static class QuestDetails
    {
        /** **********************
         * Class Variables
         ********************** **/
        private static string? QUEST_ID;
        private static string? QUEST_MAIL_ID;
        private static string? QUEST_REWARD_MAIL_ID;
        private static string? DELUXE_AUTO_PETTER_ID;

        private static readonly string AUTO_PETTER_ID = "(BC)272";
        private static readonly string HARDWOOD_ID = "(O)709";
        private static readonly string IRIDIUM_BAR_ID = "(O)337";

        private static readonly string DROPBOX_GAME_LOCATION = "Mountain";
        private static readonly Vector2 DROPBOX_LOCATION = new Vector2(18.5f, 25.5f) * Game1.tileSize;
        private static readonly Vector2 DROPBOX_INDICATOR_LOCATION = new(DROPBOX_LOCATION.X - 3, DROPBOX_LOCATION.Y - Game1.tileSize); // the indicator is 6px wide, so -3px to center it
        private static readonly (int, int)[] DROPBOX_TILE_LOCATIONS = new[] { (17, 25), (18, 25), (19, 25) };

        private static readonly Dictionary<string, int> DONATION_REQUIREMENTS = new()
        {
            { AUTO_PETTER_ID, 1 },
            { HARDWOOD_ID, 300 },
            { IRIDIUM_BAR_ID, 25 }
        };

        private static MultiplayerHandler.QuestData? QUEST_DATA;
        private static Inventory? DONATED_ITEMS;

        /** **********************
         * Variable Getters
         ********************** **/
        // ID Getters
        public static string GetAutoPetterID() { return AUTO_PETTER_ID; }

        public static string GetHardwoodID() { return HARDWOOD_ID; }

        public static string GetIridiumBarID() { return IRIDIUM_BAR_ID; }

        public static string GetQuestID()
        {
            if (QUEST_ID is null) throw new ArgumentNullException($"{nameof(QUEST_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_ID;
        }

        public static string GetQuestMailID()
        {
            if (QUEST_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_MAIL_ID;
        }

        public static string GetQuestRewardMailID()
        {
            if (QUEST_REWARD_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_REWARD_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_REWARD_MAIL_ID;
        }

        public static string GetDeluxeAutoPetterID()
        {
            if (DELUXE_AUTO_PETTER_ID is null) throw new ArgumentNullException($"{nameof(DELUXE_AUTO_PETTER_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return DELUXE_AUTO_PETTER_ID;
        }

        public static string GetDropBoxLocationName() { return DROPBOX_GAME_LOCATION; }

        // Data Getters
        public static Vector2 GetDropBoxIndicatorLocation() { return DROPBOX_INDICATOR_LOCATION; }
        public static (int, int)[] GetDropBoxTileLocations() { return DROPBOX_TILE_LOCATIONS; }

        /** **********************
         * Public Methods
         ********************** **/
        // Utility methods
        public static void Initialize(string modID)
        {
            QUEST_ID = $"{modID}.Quest";
            DELUXE_AUTO_PETTER_ID = $"{modID}.DeluxeAutoPetter";
            QUEST_MAIL_ID = $"{modID}.Mail0";
            QUEST_REWARD_MAIL_ID = $"{modID}.Mail1";
        }

        public static void LoadQuestData(long playerID)
        {
            QUEST_DATA = MultiplayerHandler.GetPlayerQuestData(playerID);
            CreateDonatedInventory(QUEST_DATA.DonationCounts);
        }

        public static bool DonationBoxTileAction(GameLocation _, string[] __, Farmer player, Point ___)
        {
            if (player.hasQuest(GetQuestID()))
            {
                Game1.activeClickableMenu ??= CreateQuestContainerMenu();
                return true;
            }
            return false;
        }

        public static bool IsQuestDataNull() { return QUEST_DATA is null; }

        // Visual Methods
        public static QuestContainerMenu CreateQuestContainerMenu()
        {
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            return new QuestContainerMenu(DONATED_ITEMS, 1, HighlightAcceptableItems, GetAcceptCount, null, UpdateDonationCounts);
        }

        /** **********************
         * Private Methods
         ********************** **/
        private static bool HighlightAcceptableItems(Item item) { return DONATION_REQUIREMENTS.ContainsKey(item.QualifiedItemId); }

        private static int GetAcceptCount(Item item)
        {
            if (DONATED_ITEMS is null)
            {
                string errorMessage = $"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            if (!HighlightAcceptableItems(item)) return 0; // basically means 'if not valid, then return 0'

            int totalNeeded = DONATION_REQUIREMENTS[item.QualifiedItemId];
            int donatedCount = DONATED_ITEMS.FirstOrDefault(donatedItem => donatedItem is not null && donatedItem.QualifiedItemId.Equals(item.QualifiedItemId), null)?.Stack ?? 0;

            return Math.Min(totalNeeded - donatedCount, item.Stack);
        }

        private static void UpdateDonationCounts()
        {
            if (QUEST_ID is null) throw new ArgumentNullException($"{nameof(QUEST_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");
            if (QUEST_REWARD_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_REWARD_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");
            if (QUEST_DATA is null) throw new ArgumentNullException($"{nameof(QUEST_DATA)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            foreach (Item? item in DONATED_ITEMS)
                if (item is not null)
                    QUEST_DATA.DonationCounts[item.QualifiedItemId] = item.Stack;

            if (AreDonationRequirementsMet())
            {
                Game1.player.completeQuest(QUEST_ID);
                Game1.player.mailForTomorrow.Add(QUEST_REWARD_MAIL_ID);
                DeluxeAutoPetter.IS_REFRESH_NEEDED = true;
            }
        }

        private static bool AreDonationRequirementsMet()
        {
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            bool completed = true; // assume quest is completed
            int i = DONATED_ITEMS.Count - 1;
            while (i >= 0 && completed)
            {
                Item? item = DONATED_ITEMS[i];
                if (item is null || item.Stack != DONATION_REQUIREMENTS[item.QualifiedItemId]) completed = false;
                else i--;
            }

            return completed;
        }

        private static void CreateDonatedInventory(Dictionary<string, int> donatedDetails)
        {
            Item? autoPetter = donatedDetails[AUTO_PETTER_ID] <= 0 ? null : ItemRegistry.Create(AUTO_PETTER_ID, donatedDetails[AUTO_PETTER_ID]);
            Item? hardwood = donatedDetails[HARDWOOD_ID] <= 0 ? null : ItemRegistry.Create(HARDWOOD_ID, donatedDetails[HARDWOOD_ID]);
            Item? iridiumBars = donatedDetails[IRIDIUM_BAR_ID] <= 0 ? null : ItemRegistry.Create(IRIDIUM_BAR_ID, donatedDetails[IRIDIUM_BAR_ID]);

            DONATED_ITEMS = new() { autoPetter, hardwood, iridiumBars };
        }
    }
}
