using StardewModdingAPI;

namespace DeluxeAutoPetter.helpers
{
    internal static class MultiplayerHandler
    {
        public class QuestData
        {
            public Dictionary<string, int> DonationCounts { get; set; } = new() {
                { QuestDetails.GetAutoPetterID(), 0 },
                { QuestDetails.GetHardwoodID(), 0 },
                { QuestDetails.GetIridiumBarID(), 0 }
            };
        }

        private static string? PER_PLAYER_QUEST_DATA_KEY;
        private static Dictionary<long, QuestData>? PER_PLAYER_QUEST_DATA;

        public static void Initialize(string modID)
        {
            PER_PLAYER_QUEST_DATA_KEY = $"{modID}.Quest.PER_PLAYER_QUEST_DATA_KEY";
        }

        public static void LoadPerPlayerQuestData(IModHelper helper, long hostID)
        {
            if (PER_PLAYER_QUEST_DATA_KEY is null)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA_KEY)} has not been initialized! The {nameof(Initialize)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            PER_PLAYER_QUEST_DATA = helper.Data.ReadSaveData<Dictionary<long, QuestData>>(PER_PLAYER_QUEST_DATA_KEY);
            PER_PLAYER_QUEST_DATA ??= new() { { hostID, new() } };
        }

        public static void SavePerPlayerQuestData(IModHelper helper)
        {
            if (PER_PLAYER_QUEST_DATA_KEY is null)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA_KEY)} has not been initialized! The {nameof(Initialize)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            if (PER_PLAYER_QUEST_DATA is null)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA)} has not been initialized! The {nameof(LoadPerPlayerQuestData)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            helper.Data.WriteSaveData(PER_PLAYER_QUEST_DATA_KEY, PER_PLAYER_QUEST_DATA);
        }

        public static QuestData GetPlayerQuestData(long playerID)
        {
            if (PER_PLAYER_QUEST_DATA is null)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA)} has not been initialized! The {nameof(LoadPerPlayerQuestData)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            if (!PER_PLAYER_QUEST_DATA.ContainsKey(playerID) && !Context.IsMainPlayer)
            {
                string errorMessage = $"Your {nameof(PER_PLAYER_QUEST_DATA)} does not have any quest data for your player ID!";
                throw new ArgumentNullException(errorMessage);
            }

            if (!PER_PLAYER_QUEST_DATA.ContainsKey(playerID) && Context.IsMainPlayer) CreatePlayerQuestData(playerID);

            return PER_PLAYER_QUEST_DATA[playerID];
        }

        public static void SetPlayerQuestData(long playerID, QuestData questData)
        {
            if (PER_PLAYER_QUEST_DATA is null && Context.IsMainPlayer)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA)} has not been initialized! The {nameof(LoadPerPlayerQuestData)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }
            else PER_PLAYER_QUEST_DATA ??= new() { { playerID, questData } };

            if (!PER_PLAYER_QUEST_DATA.ContainsKey(playerID)) throw new ArgumentNullException($"{nameof(PER_PLAYER_QUEST_DATA)} has no data associated with the playerID '{playerID}'.");

            PER_PLAYER_QUEST_DATA[playerID] = questData;
        }

        private static void CreatePlayerQuestData(long playerID)
        {
            if (PER_PLAYER_QUEST_DATA is null)
            {
                string errorMessage = $"{nameof(PER_PLAYER_QUEST_DATA)} has not been initialized! The {nameof(LoadPerPlayerQuestData)} method must be called first!";
                throw new ArgumentNullException(errorMessage);
            }

            PER_PLAYER_QUEST_DATA.Add(playerID, new());
        }
    }
}
