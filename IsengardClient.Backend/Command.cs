namespace IsengardClient.Backend
{
    public class CommandResultObject
    {
        public CommandResultObject(CommandResult result) : this(result, 0)
        {

        }

        public CommandResultObject(CommandResult result, int resultCode)
        {
            Result = result;
            ResultCode = resultCode;
        }
        /// <summary>
        /// command result
        /// </summary>
        public CommandResult Result { get; set; }

        /// <summary>
        /// specific result code
        /// </summary>
        public int ResultCode { get; set; }
    }

    /// <summary>
    /// result of a single command
    /// </summary>
    public enum CommandResult
    {
        /// <summary>
        /// the command completed successfully
        /// </summary>
        CommandSuccessful,

        /// <summary>
        /// the command was unsuccessful, but could succeed if run again (e.g. flee, search, knock, manashield)
        /// </summary>
        CommandUnsuccessfulThisTime,

        /// <summary>
        /// the command was unsuccessful, and it is expected the command would continue to not work if tried again
        /// </summary>
        CommandUnsuccessfulAlways,

        /// <summary>
        /// additional wait time is needed before the command can be run
        /// </summary>
        CommandMustWait,

        /// <summary>
        /// the background process was aborted, such as the user cancelling the background process or a hazy or flee was triggered
        /// </summary>
        CommandAborted,

        /// <summary>
        /// no response was processed from the server within the timeout interval
        /// </summary>
        CommandTimeout,

        /// <summary>
        /// background process was stopped because an escape is being attempted
        /// </summary>
        CommandEscaped,
    }

    public enum GetItemResult
    {
        Success = 1,
        TooMuchWeight = 2,
        ItemNotPresent = 3,
        QuestFulfilled = 4,
        FixedItem = 5,
        MobDisallowsTakingItems = 6,
    }

    public enum MovementResult
    {
        Success = 1,
        TotalFailure = 2,
        MapFailure = 3,
        StandFailure = 4,
        ClosedDoorFailure = 5,
        LockedDoorFailure = 6,
        FallFailure = 7,
        EquipmentFailure = 8,
    }

    public enum BackgroundCommandType
    {
        Movement,
        OpenDoor,
        UnlockExit,
        Look,
        LookAtItem,
        LookAtMob,
        Prepare,
        Search,
        Hide,
        Knock,
        Stand,
        Vigor,
        MendWounds,
        CurePoison,
        Bless,
        Protection,
        Manashield,
        Fireshield,
        Stun,
        StunWithWand,
        OffensiveSpell,
        Attack,
        Flee,
        DrinkHazy,
        DrinkNonHazyPotion,
        GetItem,
        SellItem,
        DropItem,
        WearEquipment,
        RemoveEquipment,
        WieldWeapon,
        HoldItem,
        Trade,
        Score,
        Inventory,
        Equipment,
        Quit,
    }
}
