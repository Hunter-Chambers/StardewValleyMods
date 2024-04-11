namespace AutoGrabTruffles
{
    public sealed class AutoGrabTrufflesConfig
    {
        public bool ApplyGathererProfession { get; set; } = true;
        public string WhoseGathererProfessionToUse { get; set; } = "Owner";
        public bool ApplyBotanistProfession { get; set; } = true;
        public string WhoseBotanistProfessionToUse { get; set; } = "Owner";
        public bool GainExperience {  get; set; } = true;
        public string WhoGainsExperience { get; set; } = "Owner";
        public bool UpdateGameStats { get; set; } = true;
    }
}
