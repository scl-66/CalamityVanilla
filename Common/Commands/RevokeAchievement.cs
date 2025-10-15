using CalamityVanilla.Content.Achievements;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityVanilla.Common.Commands;

public class RevokeAchievement : ModCommand
{
    public static LocalizedText DescriptionText { get; private set; }

    public override void SetStaticDefaults()
    {
        DescriptionText = Mod.GetLocalization($"Commands.{nameof(RevokeAchievement)}.Description");
    }

    // CommandType.Chat means that command can be used in Chat in SP and MP
    public override CommandType Type
        => CommandType.Chat;

    // The desired text to trigger this command
    public override string Command
        => "revoke";

    // A short description of this command
    public override string Description
        => DescriptionText.Value;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        ModContent.GetInstance<ThrowBrickAtGlass>().Condition.Clear();
    }
}