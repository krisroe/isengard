using IsengardClient.Backend;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace IsengardClient
{
    internal partial class frmMain : Form
    {
        private NetworkStream _tcpClientNetworkStream;
        private bool _fullLogFinished;

        private IsengardSettingData _settingsData;
        private bool _processedUIWithSettings;

        private string _mob;
        private string _wand;

        private const int SECONDS_PER_GAME_HOUR = 150;
        private const int SUNRISE_GAME_HOUR = 6;
        private const int SUNSET_GAME_HOUR = 20;
        private int _timeUI = -1;

        private byte _hpColorR;
        private byte _hpColorG;
        private byte _hpColorB;
        private byte _hpColorRUI;
        private byte _hpColorGUI;
        private byte _hpColorBUI;
        private byte _mpColorR;
        private byte _mpColorG;
        private byte _mpColorB;
        private byte _mpColorRUI;
        private byte _mpColorGUI;
        private byte _mpColorBUI;
        private SexEnum _sex;
        private ClassType _class;
        private ClassTypeFlags _classFlags;
        private int _level = 0;
        private string _currentPlayerHeader = null;
        private string _currentPlayerHeaderUI = null;

        private InitializationStep _initializationSteps;
        private InitialLoginInfo _loginInfo;

        private static DateTime? _currentStatusLastComputed;
        private static DateTime? _lastPollTick;

        private RealmTypeFlags _currentRealm;

        private List<SpellsEnum> _spellsCast = new List<SpellsEnum>();
        private bool _refreshSpellsCast = false;

        private PermRun _currentPermRun;
        private PermRun _currentPermRunUI;
        private PermRun _nextPermRun;
        private PermRun _nextPermRunUI;

        /// <summary>
        /// current list of players. This list is not necessarily accurate since invisible players
        /// are hidden from the who output when you cannot detect them.
        /// </summary>
        private HashSet<string> _players = null;

        private object _fullLogLock;
        private List<string> _fullLogLines;
        private string _username;
        private string _password;
        private bool _promptedUserName;
        private bool _promptedPassword;
        private bool _enteredUserName;
        private bool _enteredPassword;
        private int _totalhp = 0;
        private int _totalmp = 0;
        private int _gold = -1;
        private int _goldUI = -1;
        private int _experience = -1;
        private int _experienceUI = -1;
        private int _tnl = -1;
        private int _tnlUI = -1;
        private PlayerStatusFlags _playerStatusFlags;

        private const int HP_OR_MP_UNKNOWN = -1;

        /// <summary>
        /// hitpoints from the output. if unknown the value is -1.
        /// </summary>
        private int _autohp = HP_OR_MP_UNKNOWN;

        /// <summary>
        /// hitpoints from the output. if unknown the value is -1.
        /// </summary>
        private int _automp = HP_OR_MP_UNKNOWN;

        /// <summary>
        /// current mana from current combat
        /// </summary>
        private int _currentMana = HP_OR_MP_UNKNOWN;

        private int _autoEscapeThresholdUI;
        private AutoEscapeType _autoEscapeTypeUI;
        private bool _autoEscapeActiveUI;
        private bool _autoEscapeActiveSaved;
        private bool _fleeing;
        private bool _fleeingUI;
        private bool _hazying;
        private bool _hazyingUI;
        private object _escapeLock = new object();

        private IsengardMap _gameMap;

        private CurrentEntityInfo _currentEntityInfo = new CurrentEntityInfo();
        private List<string> _foundSearchedExits;
        private bool _programmaticUI = false;

        private bool _setCurrentArea = false;
        private BackgroundWorker _bwBackgroundProcess;
        private BackgroundWorker _bwFullLog;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private BackgroundProcessPhase _backgroundProcessPhase;
        private PleaseWaitSequence _pleaseWaitSequence;

        private object _queuedCommandLock = new object();
        private object _consoleTextLock = new object();
        private object _writeToNetworkStreamLock = new object();
        private object _broadcastMessagesLock = new object();
        private List<string> _newConsoleText = new List<string>();
        private Dictionary<char, int> _asciiMapping;
        private List<string> _broadcastMessages = new List<string>();

        private List<EmoteButton> _emoteButtons = new List<EmoteButton>();
        private bool _showingWithTarget = false;
        private bool _showingWithoutTarget = false;
        private int _waitSeconds = 0;
        private bool _initiatedEmotesTab;
        private bool _initiatedHelpTab;
        private CommandResult? _commandResult;
        private int _commandSpecificResult;
        private SelectedInventoryOrEquipmentItem _commandInventoryItem;
        private int _lastCommandDamage;
        private string _lastCommand;
        private bool _runningHiddenCommand;
        private BackgroundCommandType? _backgroundCommandType;
        private Exit _currentBackgroundExit;

        private string _currentlyFightingMobText;
        private MobTypeEnum? _currentlyFightingMobType;
        private int _currentlyFightingMobCounter;
        private string _currentlyFightingMobTextUI;
        private MobTypeEnum? _currentlyFightingMobTypeUI;
        private int _currentlyFightingMobCounterUI;

        private MonsterStatus _currentMonsterStatus;
        private MonsterStatus _currentMonsterStatusUI;
        private int _monsterDamage;
        private int _monsterDamageUI;
        private DateTime? _monsterStunnedSince;
        private bool _monsterKilled;
        private MobTypeEnum? _monsterKilledType;
        private List<ItemEntity> _monsterKilledItems = new List<ItemEntity>();

        private DateTime _serverStartTime;

        private HashSet<ItemTypeEnum> _itemsSeen = new HashSet<ItemTypeEnum>();

        /// <summary>
        /// number of times to try to attempt a background command before giving up
        /// </summary>
        private const int MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND = 20;

        private List<BackgroundCommandType> _backgroundSpells = new List<BackgroundCommandType>()
        {
            BackgroundCommandType.Vigor,
            BackgroundCommandType.MendWounds,
            BackgroundCommandType.Protection,
            BackgroundCommandType.CurePoison,
            BackgroundCommandType.Bless,
            BackgroundCommandType.Stun,
            BackgroundCommandType.StunWithWand,
            BackgroundCommandType.OffensiveSpell
        };

        public bool Logout { get; set; }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        internal frmMain(string userName, string password, bool generateFullLog)
        {
            InitializeComponent();
            this.MinimumSize = this.Size;

            _pleaseWaitSequence = new PleaseWaitSequence(OnWaitXSeconds);

            SetButtonTags();

            _asciiMapping = AsciiMapping.GetAsciiMapping();

            //prettify user name to be Pascal case
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (char c in userName)
            {
                string sChar = c.ToString();
                string sAppend;
                if (isFirst)
                {
                    isFirst = false;
                    sAppend = sChar.ToUpper();
                }
                else
                {
                    sAppend = sChar.ToLower();
                }
                sb.Append(sAppend);
            }
            _username = sb.ToString();
            _password = password;

            _bwBackgroundProcess = new BackgroundWorker();
            _bwBackgroundProcess.WorkerSupportsCancellation = true;
            _bwBackgroundProcess.DoWork += _bwBackgroundProcess_DoWork;
            _bwBackgroundProcess.RunWorkerCompleted += _bwBackgroundProcess_RunWorkerCompleted;

            if (generateFullLog)
            {
                _bwFullLog = new BackgroundWorker();
                _bwFullLog.WorkerSupportsCancellation = true;
                _bwFullLog.DoWork += _bwFullLog_DoWork;
                _fullLogLock = new object();
                _fullLogLines = new List<string>();
                _bwFullLog.RunWorkerAsync();
            }

            ReloadMap(false);

            cboSetOption.SelectedIndex = 0;

            DoConnect();
        }

        //CSRTODO: this is player-specific
        /// <summary>
        /// retrieves healing room tick mp
        /// </summary>
        /// <returns>healing room tick MP</returns>
        public int GetHealingRoomTickMP()
        {
            return 7;
        }

        //CSRTODO: this is player-specific
        /// <summary>
        /// retrieves healing room tick hp
        /// </summary>
        /// <returns>healing room tick HP</returns>
        public int GetHealingRoomTickHP()
        {
            return 6;
        }

        private void ReloadMap(bool isReload)
        {
            List<string> errorMessages = new List<string>();
            IsengardMap newMap = new IsengardMap(errorMessages);
            if (isReload)
            {
                List<Area> areasRemoved = new List<Area>();
                foreach (Area a in _settingsData.EnumerateAreas())
                {
                    bool isValid = true;
                    if (a.InventorySinkRoomObject != null)
                    {
                        a.InventorySinkRoomObject = newMap.GetRoomFromTextIdentifier(a.InventorySinkRoomIdentifier);
                        if (a.InventorySinkRoomObject == null)
                        {
                            isValid = false;
                            errorMessages.Add("Inventory sink room not found for area after reload.");
                        }
                    }
                    a.IsValid = isValid;
                    if (!isValid)
                    {
                        areasRemoved.Add(a);
                    }
                }
                if (_settingsData.HomeArea != null)
                {
                    if (_settingsData.HomeArea.IsValid)
                    {
                        _settingsData.RemoveInvalidAreas(_settingsData.HomeArea);
                    }
                    else
                    {
                        _settingsData.HomeArea = null;
                    }
                }
                for (int i = _settingsData.PermRuns.Count - 1; i >= 0; i--)
                {
                    bool isValid = true;
                    PermRun pr = _settingsData.PermRuns[i];
                    if (pr.Areas != null)
                    {
                        foreach (Area a in pr.Areas)
                        {
                            if (areasRemoved.Contains(a))
                            {
                                isValid = false;
                                errorMessages.Add("Perm run removed because area became invalid.");
                            }
                        }
                    }
                    if (pr.TargetRoomObject != null)
                    {
                        pr.TargetRoomObject = newMap.GetRoomFromTextIdentifier(pr.TargetRoomIdentifier);
                        if (pr.TargetRoomObject == null)
                        {
                            isValid = false;
                            errorMessages.Add("Target room not found for perm run after reload.");
                        }
                    }
                    if (pr.ThresholdRoomObject != null)
                    {
                        pr.ThresholdRoomObject = newMap.GetRoomFromTextIdentifier(pr.ThresholdRoomIdentifier);
                        if (pr.ThresholdRoomObject == null)
                        {
                            isValid = false;
                            errorMessages.Add("Threshold room not found for perm run after reload.");
                        }
                    }
                    if (!isValid)
                    {
                        _settingsData.PermRuns.RemoveAt(i);
                    }
                }
                RefreshAreaDropdown();
                ProcessLocationRoomsAfterReload(_settingsData.Locations, newMap, errorMessages);
            }
            AddBroadcastMessages(errorMessages);
            lock (_currentEntityInfo.EntityLock)
            {
                Room r = _currentEntityInfo.CurrentRoom;
                Room newRoom = null;
                if (r != null)
                {
                    string backendName = r.BackendName;
                    if (!string.IsNullOrEmpty(backendName))
                    {
                        newMap.UnambiguousRoomsByBackendName.TryGetValue(backendName, out newRoom);
                    }
                    if (newRoom == null && _gameMap.AmbiguousRoomsByBackendName.TryGetValue(backendName, out List<Room> possibleRooms))
                    {
                        newRoom = TryDisambiguateRoomPerObviousExits(possibleRooms, _currentEntityInfo.CurrentObviousExits);
                    }
                }
                _gameMap = newMap;
                _currentEntityInfo.CurrentRoom = newRoom;
            }
        }

        private void ProcessLocationRoomsAfterReload(List<LocationNode> locations, IsengardMap newMap, List<string> errorMessages)
        {
            for (int i = locations.Count - 1; i >= 0; i--)
            {
                bool isValid = true;
                LocationNode ln = locations[i];
                if (ln.RoomObject != null)
                {
                    ln.RoomObject = newMap.GetRoomFromTextIdentifier(ln.Room);
                    if (ln.RoomObject == null)
                    {
                        isValid = false;
                        errorMessages.Add("Room not found for location after reload: " + ln.Room);
                        locations.RemoveAt(i);
                    }
                }
                if (isValid && ln.Children != null)
                {
                    ProcessLocationRoomsAfterReload(ln.Children, newMap, errorMessages);
                    if (ln.Children.Count == 0)
                    {
                        ln.Children = null;
                    }
                }
            }
        }

        private void InitializeEmotes()
        {
            List<Emote> emotes = new List<Emote>()
            {
                new Emote("attention", "snap to attention", null),
                new Emote("beam", "beam happily", null),
                new Emote("beckon", "beckon to everyone", "beckon to X"),
                new Emote("bird", "gesture indignantly", "flip off X"),

                new Emote("blah", "Something got you down?", null),
                new Emote("bleed", "bleed profusely", null),
                new Emote("blush", "blush", null),
                new Emote("bounce", "bounce around wildly", null),

                new Emote("bow", "make a full-sweeping bow", "bow before X"),
                new Emote("cackle", "cackle gleefully", null),
                new Emote("cheer", "cheer", "cheer for X"),
                new Emote("chortle", "chortle mirthfully", null),

                new Emote("chuckle", "chuckle", null),
                new Emote("clap", "clap your hands", "applaud X"),
                new Emote("collapse", "collapse from exhaustion", null),
                new Emote("comfort", null, "comfort X"),

                //skip coin since it is duplicative of flip
                new Emote("confused", "look bewildered", null),
                new Emote("cough", "cough politely", null),
                new Emote("cower", "cower in fear", "cower in fear before X"),

                new Emote("cringe", "cringe painfully", null),
                new Emote("craps", "roll dice", null),
                new Emote("cry", "burst into tears", null),
                new Emote("cuddle", null, "cuddle up with X"),

                new Emote("curtsy", "curtsy rather daintily", "curtsy before X"),
                new Emote("dance", "dance about", "dance with X"),
                //skip dice since it is duplicative of craps
                new Emote("drool", "drool", null),

                new Emote("eye", null, "eye X suspiciously"),
                new Emote("faint", "faint", null),
                new Emote("fart", "fart", "fart on X"),
                new Emote("flaugh", "fall down laughing", null),

                new Emote("flex", "flex your muscles", null),
                new Emote("flip", "flip a coin: heads/tails", null),
                new Emote("frown", "frown", null),
                new Emote("frustrate", "pull your hair out", null),

                new Emote("fume", "fume", null),
                new Emote("gasp", "jaw drops", null),
                new Emote("giggle", "giggle inanely", null),
                new Emote("glare", null, "glare menacingly at X"),

                new Emote("goose", null, "goose X"),
                new Emote("grab", null, "pull on X"),
                new Emote("greet", "greet everyone cordially", "greet X warmly"),
                new Emote("grin", "grin evilly", null),

                new Emote("grind", "grind your teeth", null),
                new Emote("groan", "groan miserably", null),
                new Emote("grovel", "throw yourself on the ground and grovel pathetically", "grovel before X"),
                new Emote("growl", "growl", "growl at X"),

                new Emote("grr", "start looking angry", "stare menacingly at X"),
                new Emote("grumble", "grumble darkly", null),
                new Emote("grunt", "grunt agonizingly", null),
                new Emote("hiccup", "hiccup", null),

                new Emote("high5", null, "slap X a triumphant highfive"),
                new Emote("hlaugh", "laugh hysterically", null),
                new Emote("hug", null, "hug X"),
                new Emote("hum", "hum a little tune", null),

                new Emote("idea", "eyes light up with a brilliant idea", null),
                new Emote("imitate", null, "imitate X"),
                new Emote("jump", "jump for joy", null),
                new Emote("kiss", null, "kiss X gently"),

                new Emote("kisshand", null, "kiss X on the hand"),
                new Emote("laugh", "laugh", "laugh at X"),
                new Emote("lick", "lick your lips in anticipation", null),
                new Emote("moon", "moon the world", "moon X"),

                new Emote("msmile", "smile mischieviously", null),
                new Emote("mutter", "mutter", null),
                new Emote("nervous", "fidget nervously", null),
                new Emote("nod", "nod", null),

                new Emote("nose", "pick your nose", null),
                new Emote("ogle", null, "ogle X with carnal intent"),
                new Emote("pace", "pace to and fro", null),
                new Emote("pat", "can't quite manage to pat this person", null),

                new Emote("pet", null, "pet X on the back of the head"),
                new Emote("plead", "plead pathetically", "plead before X"),
                new Emote("point", "point at yourself", "point at X"),
                new Emote("poke", "poke everyone", "poke X"),

                new Emote("pout", "pout", null),
                //cannot seem to use punch since it is ambiguous with power attack without weapon
                new Emote("purr", "purr provocatively", "purr at X"),
                new Emote("raise", "raise an eyebrow questioningly", null),

                new Emote("relax", "breath deeply", null),
                new Emote("roll", "roll your eyes in exasperation", null),
                new Emote("salute", null, "salute X"),
                new Emote("scold", "scold everyone around you", "scold X for being naughty"),

                //scowl does not appear to be implemented
                new Emote("scratch", "scratch your head cluelessly", null),
                new Emote("shake", "shake your head", "shake X's hand"),
                new Emote("shove", null, "push X away"),

                new Emote("sit", "sit down", null),
                new Emote("skip", "skip like a girl", null),
                new Emote("slap", null, "slap X"),
                new Emote("sleep", "take a nap", null),

                new Emote("smell", "sniff the air", null),
                new Emote("smile", "smile happily", null),
                new Emote("smirk", "smirk wryly", null),
                new Emote("snap", "snap your fingers", null),

                new Emote("snarl", "snarl threateningly", "snarl threateningly at X"),
                new Emote("sneer", "sneer contemptuously", "sneer contemptuously at X"),
                new Emote("sneeze", "sneeze", null),
                new Emote("sniff", "start to cry", null),

                new Emote("snivel", "snivel in despair", null),
                new Emote("snort", "snort indignantly", null),
                new Emote("spit", "spit", "spit on X"),
                new Emote("squirm", "squirm uncomfortably", null),

                new Emote("ssmile", "smile with satisfaction", null),
                //stand is not an emote
                new Emote("strut", "strut around vainly", null),
                new Emote("suck", "suck your thumb", "suck X"),

                new Emote("sulk", "sulk", null),
                new Emote("sweat", "begin to sweat nervously", null),
                new Emote("tap", "tap your foot impatiently", null),
                new Emote("taunt", null, "taunt and jeer at X"),

                new Emote("thank", "thank everyone cordially", "heartily thank X"),
                new Emote("thumb", "give a thumbs up", "give X the thumbs up sign"),
                new Emote("tnl", "tnl", null),
                new Emote("twiddle", "twiddle your thumbs", null),

                new Emote("twirl", "twirl about in joy", null),
                //skip warm since is duplicative of wsmile
                new Emote("wave", "wave happily", "wave to X"),
                new Emote("whimper", "whimper like a beat dog", null),

                new Emote("whine", "whine annoyingly", null),
                new Emote("whistle", "whistle innocently", "whistle appreciatively at X"),
                new Emote("wince", "wince painfully", null),
                new Emote("wink", "wink mischieviously", "wink at X"),

                new Emote("worship", "bow down and give praise", "worship X"),
                new Emote("wsmile", "smile warmly", null),
                new Emote("yawn", "yawn loudly", null),
            };
            foreach (Emote nextEmote in emotes)
            {
                EmoteButton btn;
                if (!string.IsNullOrEmpty(nextEmote.NoTargetText))
                {
                    btn = new EmoteButtonWithoutTarget(nextEmote);
                    btn.Click += btnEmoteButton_Click;
                    _emoteButtons.Add(btn);
                }
                if (!string.IsNullOrEmpty(nextEmote.WithTargetText))
                {
                    btn = new EmoteButtonWithTarget(nextEmote);
                    btn.Click += btnEmoteButton_Click;
                    _emoteButtons.Add(btn);
                }
            }
            RefreshEmoteButtons();
        }

        private void InitializeHelp()
        {
            List<string> helpCommands = new List<string>()
            {
                "accursed",
                "advice",
                "alignment",
                "attack", //also kill
                "avatar",
                "backstab",
                "bait",
                "barbarian",
                "bard",
                "bash",
                "basics",
                "berserk",
                "blaspheme",
                "bless",
                "blister",
                "bloodboil",
                "bounty",
                "bribe",
                "broadcast",
                "burn",
                "burstflame",
                "buy",
                //"camoflage", //missing
                "cast",
                "chant",
                "chaotic",
                "charge",
                "charm",
                "circle",
                "clairvoyance",
                "clan",
                "clans",
                "clanappend",
                "clandelete",
                "clankick",
                "clannameroom",
                "clanpetition",
                "clanpledge",
                "clanrating",
                "clanreplace",
                "clanrescind",
                "clanrooms",
                "clantalk",
                "clear",
                "cleave",
                "combat",
                "communicate",
                "conjure",
                "constitution",
                "crusader",
                "crush",
                "cure-disease",
                "cure-malady",
                "cure-poison",
                "curse",
                "darkelf",
                "death",
                "description",
                "detect-invis",
                "detect-magic",
                "detect-relics",
                "dexterity",
                "disarm",
                "disguise",
                "dispel",
                "dodge",
                "drain",
                "drink", //also quaff
                "drop",
                "dual",
                "dustgust",
                "dwarf",
                "earthquake",
                "elf",
                "emote",
                "enchant",
                "endure-cold",
                "endure-earth",
                "endure-fire",
                "endure-water",
                "engulf",
                "envenom",
                "equipment",
                "evasion",
                "experience",
                "farsight",
                "fear",
                "featherfall",
                "finger",
                "fireball",
                "fireshield",
                "flamefill",
                "flaming", //also fist
                "flee", //also run
                "flood",
                "flurry",
                "fly",
                "focus",
                "follow",
                "forge",
                "fortune",
                "fumble",
                "get", //also take
                "gnome",
                "goblin",
                "grapple",
                "group", //also party
                "gtalk",
                "guilds",
                "halfelf",
                "halfgiant",
                "halforc",
                "harm",
                "heal",
                "healing",
                "health", //also score
                "help",
                "hide",
                "hold",
                "hone",
                "human",
                "hunter",
                "hurt",
                "iceblade",
                "identify",
                "ignore",
                "immolate",
                "impale",
                "incinerate",
                "information",
                "intelligence",
                "inventory",
                "invisibility",
                "keen",
                "killer",
                "knock",
                "know-aura",
                "lawful",
                "laying", //also hand
                "learn",
                "leave", //covers north/northwest/northeast/west/east/southwest/south/southeast/up/down/n/nw/ne/w/e/sw/s/se/u/d
                "levitate",
                "light",
                "lightning",
                "list",
                "lock",
                "longshot",
                "look", //also examine
                "lose",
                "mage",
                "magic",
                "manashield",
                "meditate",
                "mend",
                "misc",
                "monk",
                "movement",
                "mute",
                "objects",
                "offer",
                "ogre",
                "open",
                "orc",
                "outlaw",
                "parry",
                "peek",
                "pick",
                "piety",
                "pkill",
                "play", //also hymn, march, serenade, ditty
                "pledge",
                "policy",
                "portal",
                "power", //also special
                "pray",
                "prepare",
                "priest",
                "protection",
                "punch",
                "purchase",
                "put",
                "quickshot",
                "quit", //also goodbye
                "races",
                "rapidstrike",
                "read",
                "ready",
                "recharge",
                "regenerate",
                "relics",
                "remove",
                "remove-curse",
                "repair",
                "requiem",
                "resist-earth",
                "resist-fire",
                "resist-magic",
                "resist-water",
                "resist-wind",
                "restore",
                "revive",
                "rogue",
                "rumble",
                "rumors",
                "sanctuary",
                "say",
                "scout",
                "search",
                "selection",
                "send", //also tell
                "set",
                "shatterstone",
                "shimmer",
                "shockbolt",
                "short",
                "sing",
                "skills",
                "smash",
                "sneak",
                "social",
                "sorcerer",
                "spells",
                "stats",
                "status",
                "steal",
                "steamblast",
                "strength",
                "study",
                "stun",
                "suicide",
                "summon",
                "taunt",
                "teach",
                "teleport",
                "thaumaturgy",
                "thief",
                "throw",
                "thunderbolt",
                "time",
                "tornado",
                "touch",
                "track",
                //"tracking", //file could not be opened
                "trade",
                "transform",
                "transmute",
                "transport",
                "tremor",
                "trip",
                "troll",
                "turn",
                "unarmed",
                "uptime",
                "use",
                "value", //also cost
                "vampyric",
                "vigor",
                "volley",
                "warrior",
                "waterbolt",
                "weapons",
                "wear",
                "welcome",
                //"whirlwind", //file could not be opened
                "who",
                "whois",
                "wield",
                "wither",
                "wizard",
                "word-of-recall",
                "yell",
            };
            foreach (string nextHelpCommand in helpCommands)
            {
                Button btn = new Button();
                btn.AutoSize = true;
                btn.Text = nextHelpCommand;
                btn.Click += btnHelpCommand_Click;
                flpHelp.Controls.Add(btn);
            }
        }

        private void btnHelpCommand_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SendCommand("help " + btn.Text, InputEchoType.On);
            txtOneOffCommand.Text = string.Empty;
            txtOneOffCommand.Focus();
        }

        private void btnEmoteButton_Click(object sender, EventArgs e)
        {
            EmoteButton source = (EmoteButton)sender;
            string command = source.Emote.Command;
            if (source.HasTarget)
            {
                command += " " + txtEmoteTarget.Text;
            }
            SendCommand(command, InputEchoType.On);
        }

        private class EmoteButton : Button
        {
            public Emote Emote;
            public bool HasTarget;
            public EmoteButton(Emote emote, bool HasTarget)
            {
                this.Emote = emote;
                this.AutoSize = true;
                this.Text = HasTarget ? emote.WithTargetText : emote.NoTargetText;
                this.HasTarget = HasTarget;
            }
        }

        private class EmoteButtonWithTarget : EmoteButton
        {
            public EmoteButtonWithTarget(Emote emote) : base(emote, true)
            {
            }
        }

        private class EmoteButtonWithoutTarget : EmoteButton
        {
            public EmoteButtonWithoutTarget(Emote emote) : base(emote, false)
            {
            }
        }

        private class Emote
        {
            public Emote(string Command, string NoTargetText, string WithTargetText)
            {
                this.Command = Command;
                this.NoTargetText = NoTargetText;
                this.WithTargetText = WithTargetText;
            }
            public string Command { get; set; }
            public string NoTargetText { get; set; }
            public string WithTargetText { get; set; }
        }

        private void DoConnect()
        {
            _initializationSteps = InitializationStep.None;
            _loginInfo = null;
            _players = null;
            _timeUI = -1;
            _level = 0;
            _totalhp = 0;
            _totalmp = 0;
            _autohp = HP_OR_MP_UNKNOWN;
            _automp = HP_OR_MP_UNKNOWN;
            _currentMana = HP_OR_MP_UNKNOWN;
            _currentPlayerHeaderUI = null;
            _gold = -1;
            _goldUI = -1;
            _experience = -1;
            _experienceUI = -1;
            _tnl = -1;
            _tnlUI = -1;
            lock (_currentEntityInfo.EntityLock)
            {
                _currentEntityInfo.SkillsCooldowns.Clear();
                _spellsCast.Clear();
                _refreshSpellsCast = true;
            }
            ClearConsole();
            _currentStatusLastComputed = null;
            _promptedUserName = false;
            _promptedPassword = false;
            _enteredUserName = false;
            _enteredPassword = false;

            _tcpClientNetworkStream = new TcpClient(Program.HOST_NAME, Program.PORT).GetStream();
            BackgroundWorker _bwNetwork = new BackgroundWorker();
            _bwNetwork.DoWork += _bwNetwork_DoWork;
            _bwNetwork.RunWorkerCompleted += _bwNetwork_RunWorkerCompleted;
            _bwNetwork.RunWorkerAsync();
        }

        private void _bwNetwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((!_backgroundCommandType.HasValue || _backgroundCommandType != BackgroundCommandType.Quit) && !this.IsDisposed)
            {
                DisconnectedAction action;
                bool saveSettings;
                using (frmDisconnected frm = new frmDisconnected(_settingsData != null && _settingsData.SaveSettingsOnQuit))
                {
                    frm.ShowDialog(this);
                    action = frm.Action;
                    saveSettings = frm.SaveSettings;
                }
                if (saveSettings)
                {
                    SaveSettings();
                }
                if (action == DisconnectedAction.Reconnect)
                {
                    DoConnect();
                }
                else
                {
                    Logout = action == DisconnectedAction.Logout;
                    Close();
                }
            }
        }

        private void OnInformation(FeedLineParameters flParams, SexEnum sex, int earth, int wind, int fire, int water, int divination, int arcana, int life, int sorcery, int experience, int tnl)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Information) == InitializationStep.None;

            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Earth] = earth;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Wind] = wind;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Fire] = fire;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Water] = water;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Divination] = divination;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Arcana] = arcana;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Life] = life;
            _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Sorcery] = sorcery;
            _experience = experience;
            _tnl = tnl;
            _sex = sex;

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Information, flParams);
            }
        }

        /// <summary>
        /// handler for the output of score
        /// </summary>
        private void OnScore(FeedLineParameters flParams, ClassType playerClass, int level, int maxHP, int maxMP, decimal armorClass, bool armorClassIsExact, int gold, int tnl, List<SkillCooldown> cooldowns, List<SpellsEnum> spells, PlayerStatusFlags playerStatusFlags)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Score) == InitializationStep.None;

            lock (_currentEntityInfo.EntityLock)
            {
                bool clear = false;
                if (cooldowns.Count != _currentEntityInfo.SkillsCooldowns.Count)
                {
                    clear = true;
                }
                else
                {
                    for (int i = 0; i < cooldowns.Count; i++)
                    {
                        if (cooldowns[i].SkillType != _currentEntityInfo.SkillsCooldowns[i].SkillType)
                        {
                            clear = true;
                            break;
                        }
                    }
                }
                if (clear)
                {
                    _currentEntityInfo.SkillsCooldowns.Clear();
                    _currentEntityInfo.SkillsCooldowns.AddRange(cooldowns);
                }
                else //copy into the existing structures
                {
                    for (int i = 0; i < cooldowns.Count; i++)
                    {
                        SkillCooldown oExisting = _currentEntityInfo.SkillsCooldowns[i];
                        SkillCooldown oNew = cooldowns[i];
                        oExisting.Status = oNew.Status;
                        oExisting.NextAvailable = oNew.NextAvailable;
                    }
                }
                _spellsCast.Clear();
                _spellsCast.AddRange(spells);
                _refreshSpellsCast = true;
            }

            _class = playerClass;
            _classFlags = (ClassTypeFlags)Enum.Parse(typeof(ClassTypeFlags), playerClass.ToString());
            _level = level;
            _totalhp = maxHP;
            _totalmp = maxMP;

            lock (_currentEntityInfo.EntityLock)
            {
                _currentEntityInfo.ArmorClassScore = armorClass;
                _currentEntityInfo.ArmorClassScoreExact = armorClassIsExact;
                CalculateArmorClass();
            }

            _gold = gold;
            _tnl = tnl;
            _currentPlayerHeader = _username + " (lvl " + level + ")";
            _playerStatusFlags = playerStatusFlags;

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Score, flParams);
            }

            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            bool runningHiddenCommand = false;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Score)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                runningHiddenCommand = _runningHiddenCommand;
            }

            flParams.SetSuppressEcho(forInit || runningHiddenCommand);
        }

        private void OnSpells(FeedLineParameters flParams, List<SpellsEnum> SpellsList)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Spells) == InitializationStep.None;

            lock (_currentEntityInfo.EntityLock)
            {
                _currentEntityInfo.SpellsKnown.Clear();
                _currentEntityInfo.SpellsKnown.AddRange(SpellsList);
            }

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Spells, flParams);
            }
        }

        private void OnUptime(FeedLineParameters flParams, TimeSpan uptime)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Uptime) == InitializationStep.None;
            _serverStartTime = DateTime.UtcNow.Subtract(uptime);
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Uptime, flParams);
            }
        }

        private void OnWho(FeedLineParameters flParams, HashSet<string> playerNames)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Who) == InitializationStep.None;

            _players = playerNames;

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Who, flParams);
            }
        }

        private void AfterProcessInitializationStep(InitializationStep finishedSteps, InitializationStep currentStep, FeedLineParameters flp)
        {
            finishedSteps |= currentStep;
            _initializationSteps = finishedSteps;
            if (finishedSteps == InitializationStep.BeforeFinalization)
            {
                IsengardSettings sets = IsengardSettings.Default;
                sets.UserName = _username;
                sets.Save();

                InitialLoginInfo info = _loginInfo;
                string sRoomName = info.RoomName;
                if (RoomTransitionSequence.ProcessRoom(sRoomName, info.ObviousExits, info.List1, info.List2, info.List3, OnRoomTransition, flp, RoomTransitionType.Initial, 0, TrapType.None, false, null, null, null))
                {
                    _initializationSteps |= InitializationStep.Finalization;
                    Room r = _currentEntityInfo.CurrentRoom;
                    _setCurrentArea = r != null;
                }
                else
                {
                    flp.ErrorMessages.Add("Initial login failed!");
                }
                lock (_currentEntityInfo.EntityLock)
                {
                    CalculateArmorClass();
                }
            }
            flp.SetSuppressEcho(true);
        }

        private void OnEquipment(FeedLineParameters flParams, List<KeyValuePair<string, string>> equipment, int equipmentWeight)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Equipment) == InitializationStep.None;

            List<ItemTypeEnum> itemEnums = new List<ItemTypeEnum>();

            lock (_currentEntityInfo.EntityLock)
            {
                _currentEntityInfo.TotalEquipmentWeight = equipmentWeight;
                EntityChange changes = new EntityChange();
                changes.ChangeType = EntityChangeType.RefreshEquipment;
                for (int i = 0; i < _currentEntityInfo.Equipment.Length; i++)
                {
                    _currentEntityInfo.Equipment[i] = null;
                }
                _currentEntityInfo.UnknownEquipment.Clear();
                foreach (var nextEntry in equipment)
                {
                    var itemInfo = Entity.GetEntity(nextEntry.Key, EntityTypeFlags.Item, flParams.ErrorMessages, null, false) as ItemEntity;
                    if (itemInfo is UnknownItemEntity)
                    {
                        _currentEntityInfo.UnknownEquipment.Add(itemInfo);
                    }
                    else if (itemInfo.Count != 1)
                    {
                        _currentEntityInfo.UnknownEquipment.Add(itemInfo);
                    }
                    else
                    {
                        ItemTypeEnum itemType = itemInfo.ItemType.Value;
                        itemEnums.Add(itemType);
                        if (Enum.TryParse(nextEntry.Value, out EquipmentType eqType))
                        {
                            StaticItemData sid = ItemEntity.StaticItemData[itemType];
                            if (sid.EquipmentType == EquipmentType.Unknown)
                            {
                                flParams.ErrorMessages.Add("Found " + itemType + " equipment type " + eqType);
                                sid.EquipmentType = eqType;
                            }
                            bool foundSlot = false;
                            foreach (EquipmentSlot nextSlot in CurrentEntityInfo.GetSlotsForEquipmentType(eqType, false))
                            {
                                int iNextSlot = (int)nextSlot;
                                if (_currentEntityInfo.Equipment[iNextSlot] == null)
                                {
                                    _currentEntityInfo.Equipment[iNextSlot] = itemInfo;
                                    foundSlot = true;
                                    break;
                                }
                            }
                            if (foundSlot)
                            {
                                EntityChangeEntry entry = new EntityChangeEntry();
                                entry.Item = new ItemEntity(itemType, 1, 1);
                                entry.EquipmentAction = true;
                                entry.EquipmentIndex = -1;
                                changes.Changes.Add(entry);
                            }
                            else
                            {
                                _currentEntityInfo.UnknownEquipment.Add(itemInfo);
                            }
                        }
                        else
                        {
                            _currentEntityInfo.UnknownEquipment.Add(itemInfo);
                        }
                    }
                }
                foreach (ItemEntity ie in _currentEntityInfo.UnknownEquipment)
                {
                    EntityChangeEntry entry = new EntityChangeEntry();
                    entry.Item = ie;
                    entry.EquipmentAction = true;
                    entry.EquipmentIndex = -1;
                    changes.Changes.Add(entry);
                }
                if (changes.Changes.Count > 0)
                {
                    _currentEntityInfo.CurrentEntityChanges.Add(changes);
                }
            }

            List<ItemEntity> ies = new List<ItemEntity>();
            foreach (ItemTypeEnum nextItemType in itemEnums)
            {
                ies.Add(new ItemEntity(nextItemType, 1, 1));
            }
            ProcessItemSeen(ies);

            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            bool runningHiddenCommand = false;
            if (bct.HasValue && bct == BackgroundCommandType.Equipment)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                runningHiddenCommand = _runningHiddenCommand;
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Equipment, flParams);
            }
            flParams.SetSuppressEcho(forInit || runningHiddenCommand);
        }

        private void OnInventory(FeedLineParameters flParams, List<ItemEntity> items, int TotalWeight)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Inventory) == InitializationStep.None;

            ProcessItemSeen(items);

            lock (_currentEntityInfo.EntityLock)
            {
                EntityChange changes = new EntityChange();
                changes.ChangeType = EntityChangeType.RefreshInventory;
                _currentEntityInfo.InventoryItems.Clear();
                foreach (ItemEntity nextItemEntity in items)
                {
                    for (int i = 0; i < nextItemEntity.Count; i++)
                    {
                        _currentEntityInfo.InventoryItems.Add(nextItemEntity);
                        EntityChangeEntry entry = new EntityChangeEntry();
                        entry.Item = nextItemEntity;
                        entry.InventoryAction = true;
                        changes.Changes.Add(entry);
                    }
                }
                _currentEntityInfo.CurrentEntityChanges.Add(changes);
                _currentEntityInfo.TotalInventoryWeight = TotalWeight;
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            bool runningHiddenCommand = false;
            if (bct.HasValue && bct == BackgroundCommandType.Inventory)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                runningHiddenCommand = _runningHiddenCommand;
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Inventory, flParams);
            }
            flParams.SetSuppressEcho(forInit || runningHiddenCommand);
        }

        private void OnFailFlee(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Flee)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
                _waitSeconds = 0;
            }
        }

        private string GetDatabasePath()
        {
            return Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "Isengard.sqlite");
        }

        private void OnInitialLogin(InitialLoginInfo initialLoginInfo)
        {
            string localDatabase = GetDatabasePath();
            bool newDatabase = !File.Exists(localDatabase);
            if (newDatabase) SQLiteConnection.CreateFile(localDatabase);
            using (SQLiteConnection conn = IsengardSettingData.GetSqliteConnection(localDatabase))
            {
                conn.Open();
                if (newDatabase) //generate database schema
                {
                    IsengardSettingData.CreateNewDatabaseSchema(conn);
                }
                int userID = IsengardSettingData.GetUserID(conn, _username, false);
                if (userID == 0)
                    _settingsData = IsengardSettingData.GetDefaultSettings();
                else
                    _settingsData = LoadSettingsForUser(conn, userID);
            }
            Invoke(new Action(AfterLoadSettings));

            if (_settingsData.RemoveAllOnStartup)
                _initializationSteps = InitializationStep.None;
            else
                _initializationSteps = InitializationStep.RemoveAll;
            List<string> startupCommands = new List<string>()
            {
                "score",
                "information",
                "who",
                "uptime",
                "inventory",
                "equipment",
                "spells",
            };
            if ((_initializationSteps & InitializationStep.RemoveAll) == InitializationStep.None)
            {
                startupCommands.Add("remove all");
            }
            foreach (string nextStartupCommand in startupCommands)
            {
                SendCommand(nextStartupCommand, InputEchoType.Off);
            }
            _initializationSteps |= InitializationStep.Initialization;
            _loginInfo = initialLoginInfo;
        }

        private void AfterLoadSettings()
        {
            tsmiQuitWithoutSaving.Visible = _settingsData.SaveSettingsOnQuit;

            RefreshAreaDropdown();

            RealmTypeFlags newRealms = _settingsData.Realms;
            foreach (RealmTypeFlags nextRealm in IsengardSettingData.EnumerateRealmsFromStartingPoint(_currentRealm))
            {
                if ((newRealms & nextRealm) != RealmTypeFlags.None)
                {
                    _currentRealm = nextRealm;
                    break;
                }
            }
        }

        private void RefreshAreaDropdown()
        {
            Area previousArea = cboArea.SelectedItem as Area;
            Area newlySelectedArea = null;
            cboArea.Items.Clear();
            foreach (Area a in _settingsData.EnumerateAreas())
            {
                if (previousArea != null && previousArea.DisplayName == a.DisplayName)
                {
                    newlySelectedArea = a;
                }
                cboArea.Items.Add(a);
            }
            if (newlySelectedArea == null && cboArea.Items.Count > 0)
                cboArea.SelectedIndex = 0;
            else
                cboArea.SelectedItem = newlySelectedArea;
        }

        private Room GetCurrentRoomIfUnambiguous(string sRoomName)
        {
            Room ret;
            _gameMap.UnambiguousRoomsByBackendName.TryGetValue(sRoomName, out ret);
            return ret;
        }

        private void OnRoomTransition(FeedLineParameters flParams, RoomTransitionInfo roomTransitionInfo, int damage, TrapType trapType, List<string> broadcastMessages, List<string> addedPlayers, List<string> removedPlayers)
        {
            ProcessItemSeen(roomTransitionInfo.Items);

            Exit currentBackgroundExit = _currentBackgroundExit;
            RoomTransitionType rtType = roomTransitionInfo.TransitionType;
            string sRoomName = roomTransitionInfo.RoomName;
            List<string> obviousExits = roomTransitionInfo.ObviousExits;

            if ((trapType & TrapType.PoisonDart) != TrapType.None)
            {
                _playerStatusFlags |= PlayerStatusFlags.Poisoned;
            }
            if ((trapType & TrapType.Fall) != TrapType.None)
            {
                _playerStatusFlags |= PlayerStatusFlags.Prone;
            }

            BackgroundCommandType? bct = null;
            if (flParams != null)
            {
                bct = flParams.BackgroundCommandType;
            }
            bool fromBackgroundLook = false;
            bool fromBackgroundFlee = false;
            bool fromBackgroundMove = false;
            bool fromBackgroundHazy = false;
            bool fromAnyBackgroundCommand = bct.HasValue;
            if (fromAnyBackgroundCommand)
            {
                BackgroundCommandType bctValue = bct.Value;
                fromBackgroundFlee = bctValue == BackgroundCommandType.Flee;
                fromBackgroundLook = bctValue == BackgroundCommandType.Look;
                fromBackgroundMove = bctValue == BackgroundCommandType.Movement;
                fromBackgroundHazy = bctValue == BackgroundCommandType.DrinkHazy;
            }
            
            Room previousRoom = _currentEntityInfo.CurrentRoom;

            //first try is always to look for an unambiguous name across the whole map
            Room newRoom = GetCurrentRoomIfUnambiguous(sRoomName);
            List<Room> disambiguationRooms = null;
            bool lookForRoomsByRoomName = false;
            if (rtType == RoomTransitionType.FleeWithoutDropWeapon || rtType == RoomTransitionType.FleeWithDropWeapon)
            {
                _fleeing = false;
                if (fromBackgroundFlee)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                    _waitSeconds = 0;
                }
                if (newRoom == null && previousRoom != null) //determine the room that was fled to if possible
                {
                    List<Exit> fleeableExits = new List<Exit>();
                    foreach (Exit e in IsengardMap.GetRoomExits(previousRoom, FleeExitDiscriminator))
                    {
                        if (e.Target.BackendName == sRoomName)
                        {
                            fleeableExits.Add(e);
                        }
                    }
                    if (fleeableExits.Count == 0)
                    {
                        lookForRoomsByRoomName = true;
                    }
                    else
                    {
                        if (fleeableExits.Count == 1)
                        {
                            newRoom = fleeableExits[0].Target;
                        }
                        else
                        {
                            disambiguationRooms = new List<Room>();
                            foreach (Exit nextFleeExit in fleeableExits)
                            {
                                disambiguationRooms.Add(nextFleeExit.Target);
                            }
                        }
                    }
                }
            }
            else if (rtType == RoomTransitionType.WordOfRecall || rtType == RoomTransitionType.Death)
            {
                _hazying = false;
                _fleeing = false;
                if (roomTransitionInfo.DrankHazy)
                {
                    AddOrRemoveItemsFromInventoryOrEquipment(flParams, new List<ItemEntity>() { new ItemEntity(ItemTypeEnum.HazyPotion, 1, 1) }, ItemManagementAction.ConsumeItem, null);
                }
                if (fromAnyBackgroundCommand) //abort whatever background command is currently running
                {
                    if (fromBackgroundHazy && rtType == RoomTransitionType.WordOfRecall)
                    {
                        flParams.CommandResult = CommandResult.CommandSuccessful;
                        _waitSeconds = 0;
                    }
                    else
                    {
                        flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                    }
                }
            }
            else if (rtType == RoomTransitionType.Initial)
            {
                if (newRoom == null)
                {
                    lookForRoomsByRoomName = true;
                }
            }
            else if (fromBackgroundMove)
            {
                if (newRoom == null && currentBackgroundExit?.Target != null)
                {
                    Room targetRoom = currentBackgroundExit.Target;
                    if (string.Equals(targetRoom.BackendName, sRoomName))
                    {
                        newRoom = targetRoom;
                    }
                }
                if (newRoom == null)
                {
                    lookForRoomsByRoomName = true;
                }
                flParams.CommandResult = CommandResult.CommandSuccessful;
                _waitSeconds = 0;
            }
            else if (fromBackgroundLook)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                _waitSeconds = 0;
                lookForRoomsByRoomName = previousRoom == null;
            }

            if (fromBackgroundFlee || fromBackgroundMove) //not sure if you can flee to a trap room
            {
                _lastCommandDamage = damage;
                flParams.CommandSpecificResult = Convert.ToInt32(MovementResult.Success);
            }

            if (lookForRoomsByRoomName)
            {
                disambiguationRooms = new List<Room>();
                if (_gameMap.AmbiguousRoomsByBackendName.TryGetValue(sRoomName, out List<Room> possibleRooms))
                {
                    disambiguationRooms.AddRange(possibleRooms);
                }
            }

            if (disambiguationRooms != null) //disambiguate based on obvious exits
            {
                Room foundRoom = TryDisambiguateRoomPerObviousExits(disambiguationRooms, obviousExits);
                if (foundRoom != null)
                {
                    newRoom = foundRoom;
                }
            }
            if (newRoom == null && fromBackgroundLook && previousRoom != null)
            {
                newRoom = previousRoom;
            }

            ProcessBroadcastMessages(broadcastMessages, addedPlayers, removedPlayers);

            lock (_currentEntityInfo.EntityLock) //update the room change list with the next room
            {
                EntityChange rc;
                if (rtType == RoomTransitionType.FleeWithDropWeapon)
                {
                    int weapon1Slot = (int)EquipmentSlot.Weapon1;
                    int weapon2Slot = (int)EquipmentSlot.Weapon2;
                    bool hasWeapon1 = _currentEntityInfo.Equipment[weapon1Slot] != null;
                    bool hasWeapon2 = _currentEntityInfo.Equipment[weapon2Slot] != null;
                    if (hasWeapon1 || hasWeapon2)
                    {
                        rc = new EntityChange();
                        rc.ChangeType = EntityChangeType.DestroyEquipment;
                        int iEquipmentRemovalPoint;
                        if (hasWeapon1)
                        {
                            iEquipmentRemovalPoint = _currentEntityInfo.FindEquipmentRemovalPoint(EquipmentSlot.Weapon1);
                            if (iEquipmentRemovalPoint >= 0)
                            {
                                rc.Changes.Add(new EntityChangeEntry()
                                {
                                    EquipmentAction = false,
                                    EquipmentIndex = iEquipmentRemovalPoint
                                });
                            }
                            _currentEntityInfo.Equipment[weapon1Slot] = null;
                        }
                        if (hasWeapon2)
                        {
                            iEquipmentRemovalPoint = _currentEntityInfo.FindEquipmentRemovalPoint(EquipmentSlot.Weapon2);
                            if (iEquipmentRemovalPoint >= 0)
                            {
                                rc.Changes.Add(new EntityChangeEntry()
                                {
                                    EquipmentAction = false,
                                    EquipmentIndex = iEquipmentRemovalPoint
                                });
                            }
                            _currentEntityInfo.Equipment[weapon2Slot] = null;
                        }
                        if (rc.Changes.Count > 0)
                        {
                            _currentEntityInfo.CurrentEntityChanges.Add(rc);
                        }
                    }
                }
                _currentEntityInfo.CurrentObviousExits.Clear();
                _currentEntityInfo.CurrentObviousExits.AddRange(obviousExits);
                rc = new EntityChange();
                rc.Room = newRoom;
                rc.ChangeType = EntityChangeType.RefreshRoom;
                rc.Exits = new List<string>(obviousExits);
                if (newRoom != null)
                {
                    rc.MappedExits = new Dictionary<string, Exit>();
                    Dictionary<string, List<Exit>> periodicExits = new Dictionary<string, List<Exit>>();
                    foreach (Exit nextExit in newRoom.Exits)
                    {
                        string nextExitText = nextExit.ExitText;
                        if (nextExit.BoatExitType != BoatExitType.None)
                        {
                            List<Exit> nextPeriodicExits;
                            if (!periodicExits.TryGetValue(nextExitText, out nextPeriodicExits))
                            {
                                nextPeriodicExits = new List<Exit>();
                                periodicExits[nextExitText] = nextPeriodicExits;
                            }
                            periodicExits[nextExitText].Add(nextExit);
                        }
                        else if (nextExit.PresenceType != ExitPresenceType.RequiresSearch && ExitExistsInList(obviousExits, nextExitText, out string nextExitTextOut)) //requires search exits behave like hidden ones
                        {
                            rc.MappedExits[nextExitTextOut] = nextExit;
                        }
                        else
                        {
                            rc.OtherExits.Add(nextExit);
                        }
                    }
                    foreach (var nextPeriodic in periodicExits)
                    {
                        string exitText = nextPeriodic.Key;
                        List<Exit> nextPeriodicList = nextPeriodic.Value;
                        if (nextPeriodicList.Count > 1)
                        {
                            rc.MappedExits[exitText] = null; //indicate ambiguous
                            foreach (var nextPeriodicExit in nextPeriodicList)
                            {
                                rc.OtherExits.Add(nextPeriodicExit);
                            }
                        }
                        else
                        {
                            Exit singlePeriodicExit = nextPeriodicList[0];
                            if (obviousExits.Contains(exitText))
                            {
                                rc.MappedExits[exitText] = singlePeriodicExit;
                            }
                            else
                            {
                                rc.OtherExits.Add(singlePeriodicExit);
                            }
                        }
                    }

                    List<MobTypeEnum> currentRoomMobs = _currentEntityInfo.CurrentRoomMobs;
                    currentRoomMobs.Clear();
                    List<MobEntity> mobs = roomTransitionInfo.Mobs;
                    if (mobs != null)
                    {
                        foreach (var nextMob in mobs)
                        {
                            MobTypeEnum? mobType = nextMob.MobType;
                            if (mobType.HasValue)
                            {
                                MobTypeEnum mobTypeValue = mobType.Value;
                                for (int i = 0; i < nextMob.Count; i++)
                                {
                                    currentRoomMobs.Add(mobTypeValue);
                                    EntityChangeEntry changeEntry = new EntityChangeEntry();
                                    changeEntry.MobType = mobTypeValue;
                                    changeEntry.RoomMobAction = true;
                                    changeEntry.RoomMobIndex = -1;
                                    rc.Changes.Add(changeEntry);
                                }
                            }
                        }
                    }

                    _currentEntityInfo.CurrentRoomItems.Clear();
                    List<ItemEntity> items = roomTransitionInfo.Items;
                    if (items != null)
                    {
                        foreach (var nextItem in items)
                        {
                            foreach (ItemEntity nextSplitEntity in ItemEntity.SplitItemEntity(nextItem, false, null))
                            {
                                _currentEntityInfo.CurrentRoomItems.Add(nextSplitEntity);
                                EntityChangeEntry changeEntry = new EntityChangeEntry();
                                changeEntry.Item = nextSplitEntity;
                                changeEntry.RoomItemAction = true;
                                changeEntry.RoomItemIndex = -1;
                                rc.Changes.Add(changeEntry);
                            }
                        }
                    }

                    _currentEntityInfo.CurrentUnknownEntities.Clear();
                    List<UnknownTypeEntity> unknownEntities = roomTransitionInfo.UnknownEntities;
                    if (unknownEntities != null)
                    {
                        foreach (var nextUnknownEntity in unknownEntities)
                        {
                            foreach (UnknownTypeEntity nextSplitEntity in UnknownTypeEntity.SplitUnknownTypeEntity(nextUnknownEntity, false, null))
                            {
                                _currentEntityInfo.CurrentUnknownEntities.Add(nextUnknownEntity);
                                EntityChangeEntry changeEntry = new EntityChangeEntry();
                                changeEntry.UnknownTypeEntity = nextSplitEntity;
                                changeEntry.RoomUnknownEntityAction = true;
                                changeEntry.RoomUnknownEntityIndex = -1;
                                rc.Changes.Add(changeEntry);
                            }
                        }
                    }

                    _currentEntityInfo.CurrentEntityChanges.Add(rc);
                    _currentEntityInfo.CurrentRoom = newRoom;

                    List<string> errorMessages = new List<string>();
                    ValidateObviousExits(newRoom, _currentEntityInfo.CurrentObviousExits, errorMessages);
                    AddBroadcastMessages(errorMessages);
                }
            }
        }

        private void ValidateObviousExits(Room r, List<string> obviousExits, List<string> errorMessages)
        {
            List<string> toProcess = new List<string>(obviousExits);
            string sBackendName = r.BackendName;
            foreach (Exit e in r.Exits)
            {
                string sExitText = e.ExitText;
                bool seeExit = obviousExits.Contains(sExitText);
                if (e.Hidden)
                {
                    if (seeExit)
                    {
                        errorMessages.Add("Can see hidden exit " + sExitText + " for " + sBackendName);
                    }
                }
                else if (seeExit)
                {
                    if (toProcess.Contains(e.ExitText))
                    {
                        toProcess.Remove(e.ExitText);
                    }
                }
                else if (e.BoatExitType == BoatExitType.None)
                {
                    errorMessages.Add("Cannot see visible exit " + sExitText + " for " + sBackendName);
                }
            }
            foreach (string s in toProcess)
            {
                errorMessages.Add("See unexpected exit " + s + " for " + sBackendName);
            }
        }

        private Room TryDisambiguateRoomPerObviousExits(List<Room> disambiguationRooms, List<string> obviousExits)
        {
            Room foundRoom = null;
            foreach (Room nextDisambigRoom in disambiguationRooms)
            {
                bool matches = true;
                List<string> nextCheckObviousExits = IsengardMap.GetObviousExits(nextDisambigRoom, out List<string> optionalExits);
                if (obviousExits.Count == 1 && string.Equals(obviousExits[0], "None", StringComparison.OrdinalIgnoreCase))
                {
                    matches = nextCheckObviousExits.Count == 0;
                }
                else
                {
                    foreach (string nextExitText in nextCheckObviousExits)
                    {
                        if (!obviousExits.Contains(nextExitText))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        foreach (string nextExitText in obviousExits)
                        {
                            if (!nextCheckObviousExits.Contains(nextExitText) && (optionalExits == null || !optionalExits.Contains(nextExitText)))
                            {
                                matches = false;
                                break;
                            }
                        }
                    }
                }
                if (matches)
                {
                    if (foundRoom == null)
                    {
                        foundRoom = nextDisambigRoom;
                    }
                    else //still ambiguous
                    {
                        foundRoom = null;
                        break;
                    }
                }
            }
            return foundRoom;
        }

        private bool ExitExistsInList(List<string> displayedObviousExits, string mapExitText, out string mapExitTextOut)
        {
            mapExitTextOut = null;
            bool ret = false;
            string sExitTextForCheck = mapExitText.Trim();
            int iFirstSpace = sExitTextForCheck.IndexOf(' ');
            if (iFirstSpace > 0)
            {
                sExitTextForCheck = sExitTextForCheck.Substring(0, iFirstSpace);
            }
            foreach (string sNextExit in displayedObviousExits)
            {
                //use the first word for the mapping since subsequent words aren't used anyway
                string sCheck = sNextExit.Trim();
                iFirstSpace = sCheck.IndexOf(' ');
                if (iFirstSpace > 0)
                {
                    sCheck = sCheck.Substring(0, iFirstSpace);
                }
                if (sCheck == sExitTextForCheck)
                {
                    mapExitTextOut = sExitTextForCheck;
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private static void OnFailManashield(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Manashield)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private void OnManashieldOn(FeedLineParameters flParams)
        {
            EnsureManashieldStatus(true);
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Manashield)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void EnsureManashieldStatus(bool active)
        {
            lock (_currentEntityInfo.EntityLock)
            {
                if (ChangeSkillActive(SkillWithCooldownType.Manashield, active))
                {
                    decimal armorClassCalculated = _currentEntityInfo.ArmorClassCalculated;
                    if (armorClassCalculated >= 0)
                    {
                        decimal armorClassManashield = GetManashieldArmorClass();
                        if (armorClassManashield > armorClassCalculated)
                        {
                            decimal delta;
                            if (active)
                                delta = armorClassManashield - armorClassCalculated;
                            else
                                delta = armorClassCalculated - armorClassManashield;
                            OnDeltaArmorClass(delta);
                        }
                    }
                }
            }
        }

        private void OnFireshieldOn(FeedLineParameters flParams)
        {
            ChangeSkillActive(SkillWithCooldownType.Fireshield, true);
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Fireshield)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        /// <summary>
        /// changes if a skill is active
        /// </summary>
        /// <param name="skill">skill to change</param>
        /// <param name="active">true to make active, false to make inactive</param>
        /// <returns>true if the status actually changed, false otherwise</returns>
        private bool ChangeSkillActive(SkillWithCooldownType skill, bool active)
        {
            bool ret = false;
            lock (_currentEntityInfo.EntityLock)
            {
                foreach (SkillCooldown nextCooldown in _currentEntityInfo.SkillsCooldowns)
                {
                    if (nextCooldown.SkillType == skill)
                    {
                        if (active)
                            ret = nextCooldown.Status != SkillCooldownStatus.Active;
                        else
                            ret = nextCooldown.Status == SkillCooldownStatus.Active;
                        nextCooldown.Status = active ? SkillCooldownStatus.Active : SkillCooldownStatus.Inactive;
                        nextCooldown.NextAvailable = DateTime.MinValue;
                        break;
                    }
                }
            }
            return ret;
        }

        private void OnWaitXSeconds(int waitSeconds, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                flParams.CommandResult = CommandResult.CommandMustWait;
                _waitSeconds = waitSeconds;
            }
        }

        private void OnSelfSpellCast(FeedLineParameters flParams, BackgroundCommandType? commandType, SpellsEnum? activeSpell, List<ItemEntity> consumedItems)
        {
            if (commandType == BackgroundCommandType.CurePoison)
            {
                _playerStatusFlags &= ~PlayerStatusFlags.Poisoned;
            }
            if (activeSpell.HasValue)
            {
                AddActiveSpell(activeSpell.Value);
            }
            if (consumedItems != null && consumedItems.Count > 0)
            {
                AddOrRemoveItemsFromInventoryOrEquipment(flParams, consumedItems, ItemManagementAction.ConsumeItem, null);
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && commandType.HasValue && bct.Value == commandType.Value)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void AddActiveSpell(SpellsEnum spell)
        {
            AddActiveSpells(new List<SpellsEnum>() { spell });
        }

        private void AddActiveSpells(List<SpellsEnum> spells)
        {
            bool changed = false;
            if (spells != null && spells.Count > 0)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    bool addedProtection = false;
                    foreach (SpellsEnum nextSpell in spells)
                    {
                        if (!_spellsCast.Contains(nextSpell))
                        {
                            addedProtection |= nextSpell == SpellsEnum.protection;
                            _spellsCast.Add(nextSpell);
                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        _refreshSpellsCast = true;
                    }
                    if (addedProtection)
                    {
                        OnDeltaArmorClass(1);
                    }
                }
            }
        }

        private void FailMovement(FeedLineParameters flParams, MovementResult movementResult, int damage)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;

                if (bctValue == BackgroundCommandType.Movement)
                {
                    _lastCommandDamage = damage;
                    flParams.CommandSpecificResult = Convert.ToInt32(movementResult);

                    //even though some of these results are fixable (e.g. trap rooms), return full failure to allow the caller
                    //to decide to heal.
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
                else if (bctValue == BackgroundCommandType.OpenDoor && movementResult == MovementResult.LockedDoorFailure)
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private static void OpenDoorSuccess(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.OpenDoor)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void CantSeeExit(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.OpenDoor)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void FailSearch(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Search)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private void SuccessfulSearch(List<string> exits, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Search)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                _foundSearchedExits = exits;
            }
        }

        private static void FailHide(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Hide)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private static void FailStunWithWandUsedUp(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.UnlockExit)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime; //could succeed with a different wand
            }
        }

        private static void FailUnlockAlways(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.UnlockExit)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void FailUnlockThisTime(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.UnlockExit)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private static void SucceedUnlock(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.UnlockExit)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void SuccessfulHide(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Hide)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void SuccessfulKnock(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Knock)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void ExitIsNotLocked(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.Knock || bctValue == BackgroundCommandType.UnlockExit)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;

                }
            }
        }

        private static void FailKnock(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Knock)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private void PotionEvaporatesBeforeDrinking(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.DrinkHazy || bctValue == BackgroundCommandType.DrinkNonHazyPotion)
                {
                    if (bctValue == BackgroundCommandType.DrinkHazy)
                    {
                        _hazying = false;
                    }
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private void FailItemAction(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.DrinkHazy || bctValue == BackgroundCommandType.DrinkNonHazyPotion || bctValue == BackgroundCommandType.SellItem || bctValue == BackgroundCommandType.DropItem || bctValue == BackgroundCommandType.Trade || bctValue == BackgroundCommandType.WieldWeapon || bctValue == BackgroundCommandType.HoldItem || bctValue == BackgroundCommandType.UnlockExit || bctValue == BackgroundCommandType.WearEquipment)
                {
                    if (bctValue == BackgroundCommandType.DrinkHazy)
                    {
                        _hazying = false;
                    }
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private static void OnSuccessfulPrepare(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Prepare)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        /// <summary>
        /// happens when looking at a hidden mob
        /// </summary>
        private void OnSeeNothingSpecial(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.LookAtMob)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                flParams.SetSuppressEcho(_runningHiddenCommand);
            }
        }

        private static void OnCannotPickUpItemNotHere(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.ItemNotPresent);
            }
        }

        private static void OnCannotPickUpItemQuestFulfilled(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.QuestFulfilled);
            }
        }

        private static void OnCannotPickUpItemFixedItem(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.FixedItem);
            }
        }

        private static void OnCannotPickUpItemMobDisallows(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.MobDisallowsTakingItems);
            }
        }


        private static void OnCannotSellItem(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.SellItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void OnCannotRemoveEquipment(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.RemoveEquipment)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void OnCannotWieldWeapon(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.WieldWeapon)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void OnCannotHoldItem(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.HoldItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        /// <summary>
        /// handles the result of the stand command. Either the user stands up or is already standing, and in
        /// either case maintain the current player state as standing.
        /// </summary>
        private void OnStandUp(FeedLineParameters flParams)
        {
            _playerStatusFlags &= ~PlayerStatusFlags.Prone;
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Stand)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OnCannotCarryAnymore(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime; //fixable if the user drops stuff
                flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.TooMuchWeight);
            }
        }

        private void OnEntityAttacksYou(FeedLineParameters flParams)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp != null && flParams.IsFightingMob)
            {
                DateTime? stunStart = _monsterStunnedSince;
                _monsterStunnedSince = null;
                if (stunStart.HasValue)
                {
                    double seconds = (DateTime.UtcNow - stunStart.Value).TotalSeconds;
                    if (seconds > 0)
                    {
                        flParams.Lines.Insert(0, "Stunned for " + Math.Round(seconds, 2).ToString() + " seconds.");
                    }
                }
            }
        }

        /// <summary>
        /// when a spell fails (e.g. alignment out of whack)
        /// </summary>
        private void OnSpellFails(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (_backgroundSpells.Contains(bctValue))
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }
        
        private void OnAttack(bool fumbled, int damage, bool killedMonster, MobTypeEnum? eMobType, int experience, bool powerAttacked, List<ItemEntity> monsterItems, FeedLineParameters flParams)
        {
            ProcessItemSeen(monsterItems);
            lock (_currentEntityInfo.EntityLock)
            {
                if (powerAttacked)
                {
                    ChangeSkillActive(SkillWithCooldownType.PowerAttack, false);
                }
                _experience += experience;
                _tnl = Math.Max(0, _tnl - experience);
                if (fumbled)
                {
                    ItemEntity ieWeapon = _currentEntityInfo.Equipment[(int)EquipmentSlot.Weapon1];
                    if (ieWeapon != null)
                    {
                        AddOrRemoveItemsFromInventoryOrEquipment(flParams, new List<ItemEntity>() { ieWeapon }, ItemManagementAction.Unequip, null);
                    }
                }
                bool hasMonsterItems = monsterItems.Count > 0;
                if (flParams.IsFightingMob)
                {
                    if (!fumbled && killedMonster)
                    {
                        _monsterKilled = true;
                        _monsterKilledType = eMobType;
                    }
                    _monsterDamage += damage;
                    _monsterKilledItems.AddRange(monsterItems);
                }
                if (eMobType.HasValue)
                {
                    RemoveMobs(eMobType.Value, 1);
                }
                if (hasMonsterItems)
                {
                    AddRoomItems(monsterItems);
                }
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
            {
                _lastCommandDamage = damage;
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void OnCastOffensiveSpell(int damage, bool killedMonster, MobTypeEnum? mobType, int experience, List<ItemEntity> monsterItems, FeedLineParameters flParams)
        {
            ProcessItemSeen(monsterItems);
            _experience += experience;
            _tnl = Math.Max(0, _tnl - experience);
            bool hasMonsterItems = monsterItems.Count > 0;
            lock (_currentEntityInfo.EntityLock)
            {
                if (flParams.IsFightingMob)
                {
                    _monsterDamage += damage;
                    if (killedMonster)
                    {
                        _monsterKilled = true;
                        _monsterKilledType = mobType;
                    }
                    if (hasMonsterItems)
                    {
                        _monsterKilledItems.AddRange(monsterItems);
                    }
                }
                if (mobType.HasValue)
                {
                    RemoveMobs(mobType.Value, 1);
                }
                if (hasMonsterItems)
                {
                    AddRoomItems(monsterItems);
                }
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.OffensiveSpell)
            {
                _lastCommandDamage = damage;
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        /// <summary>
        /// triggered by looking or attacking a mob that is not present
        /// </summary>
        private void OnYouDontSeeThatHere(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                bool isLookAtMob = bctValue == BackgroundCommandType.LookAtMob;
                if (isLookAtMob || bctValue == BackgroundCommandType.Attack)
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                    if (isLookAtMob)
                    {
                        flParams.SetSuppressEcho(_runningHiddenCommand);
                    }
                }
            }
        }

        /// <summary>
        /// triggered by failed power attack
        /// </summary>
        private static void OnThatIsNotHere(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.Attack || bctValue == BackgroundCommandType.Trade)
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private static void OnCastOffensiveSpellMobNotPresent(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.OffensiveSpell || bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.StunWithWand)
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private static void OnTryAttackUnharmableMob(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.OffensiveSpell || bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.StunWithWand || bctValue == BackgroundCommandType.Attack)
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
        }

        private void OnMobStatusSequence(MonsterStatus status, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.LookAtMob)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                    if (flParams.IsFightingMob)
                    {
                        _currentMonsterStatus = status;
                    }
                }
            }
        }

        /// <summary>
        /// handles changing armor class by the specific delta. assumes the entity lock.
        /// </summary>
        /// <param name="delta">delta</param>
        private void OnDeltaArmorClass(decimal? delta)
        {
            if (delta.HasValue)
            {
                decimal deltaValue = delta.Value;
                if ((int)deltaValue == deltaValue)
                {
                    if (_currentEntityInfo.ArmorClassScore >= 0)
                    {
                        _currentEntityInfo.ArmorClassScore += deltaValue;
                    }
                }
                else //fractional change in delta
                {
                    _currentEntityInfo.ArmorClassScore = -1;
                    _currentEntityInfo.ArmorClassScoreExact = false;
                }
                CalculateArmorClass();
            }
            else
            {
                _currentEntityInfo.ArmorClassScore = -1;
                _currentEntityInfo.ArmorClassScoreExact = false;
                _currentEntityInfo.ArmorClassCalculated = -1;
            }
        }

        /// <summary>
        /// calculates armor class from current spells, skills, and equipment. assumes the entity lock.
        /// </summary>
        private void CalculateArmorClass()
        {
            bool hasProtection = _spellsCast.Contains(SpellsEnum.protection);
            bool hasManashield = false;
            foreach (SkillCooldown sc in _currentEntityInfo.SkillsCooldowns)
            {
                if (sc.SkillType == SkillWithCooldownType.Manashield)
                {
                    hasManashield = sc.Status == SkillCooldownStatus.Active;
                    break;
                }
            }
            bool calculatedArmorClassSuccessful = true;
            decimal calculatedArmorClass = hasProtection ? 1 : 0;
            for (int i = 0; i < (int)EquipmentSlot.Count; i++)
            {
                if (i != (int)EquipmentSlot.Held && i != (int)EquipmentSlot.Weapon1 && i != (int)EquipmentSlot.Weapon2)
                {
                    ItemEntity nextEquipmentEntity = _currentEntityInfo.Equipment[i];
                    if (nextEquipmentEntity != null && nextEquipmentEntity.ItemType.HasValue)
                    {
                        StaticItemData sid = ItemEntity.StaticItemData[nextEquipmentEntity.ItemType.Value];
                        if (sid.ArmorClass > 0)
                        {
                            calculatedArmorClass += sid.ArmorClass;
                        }
                        else //unknown
                        {
                            calculatedArmorClassSuccessful = false;
                            break;
                        }
                    }
                }
            }
            decimal calculatedArmorClassToSet = -1;
            if (calculatedArmorClassSuccessful)
            {
                calculatedArmorClassToSet = calculatedArmorClass;
                if (hasManashield)
                {
                    decimal armorClassManashield = GetManashieldArmorClass();
                    if (armorClassManashield > calculatedArmorClassToSet)
                    {
                        calculatedArmorClassToSet = armorClassManashield;
                    }
                }
            }
            _currentEntityInfo.ArmorClassCalculated = calculatedArmorClassToSet;
        }

        private decimal GetManashieldArmorClass()
        {
            return 7 + (_level / 3);
        }

        private void OnInformationalMessages(FeedLineParameters flp, List<string> broadcasts, List<string> addedPlayers, List<string> removedPlayers)
        {
            List<InformationalMessages> infoMsgs = flp.InfoMessages;
            List<SpellsEnum> spellsOff = null;
            bool finishedProcessing = false;
            Exit currentBackgroundExit = _currentBackgroundExit;
            EntityChange rc;
            BackgroundCommandType? bct = flp.BackgroundCommandType;
            bool isFightingMob = flp.IsFightingMob;
            List<string> messagesToRemove = null;
            foreach (InformationalMessages next in infoMsgs)
            {
                InformationalMessageType nextMessage = next.MessageType;
                switch (nextMessage)
                {
                    case InformationalMessageType.DayStart:
                        if (broadcasts == null) broadcasts = new List<string>();
                        broadcasts.Add(InformationalMessagesSequence.TIME_SUN_RISES);
                        if (messagesToRemove == null) messagesToRemove = new List<string>();
                        messagesToRemove.Add(InformationalMessagesSequence.TIME_SUN_RISES);
                        break;
                    case InformationalMessageType.NightStart:
                        if (broadcasts == null) broadcasts = new List<string>();
                        broadcasts.Add(InformationalMessagesSequence.TIME_SUN_SETS);
                        if (messagesToRemove == null) messagesToRemove = new List<string>();
                        messagesToRemove.Add(InformationalMessagesSequence.TIME_SUN_SETS);
                        break;
                    case InformationalMessageType.BlessOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.bless);
                        break;
                    case InformationalMessageType.ProtectionOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.protection);
                        break;
                    case InformationalMessageType.FlyOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.fly);
                        break;
                    case InformationalMessageType.LevitationOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.levitate);
                        break;
                    case InformationalMessageType.InvisibilityOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.invisibility);
                        break;
                    case InformationalMessageType.DetectInvisibleOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.detectinvis);
                        break;
                    case InformationalMessageType.KnowAuraOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.knowaura);
                        break;
                    case InformationalMessageType.EndureFireOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.endurefire);
                        break;
                    case InformationalMessageType.EndureColdOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.endurecold);
                        break;
                    case InformationalMessageType.EndureEarthOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.endureearth);
                        break;
                    case InformationalMessageType.EndureWaterOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.endurewater);
                        break;
                    case InformationalMessageType.DetectMagicOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.detectmagic);
                        break;
                    case InformationalMessageType.LightOver:
                        if (spellsOff == null) spellsOff = new List<SpellsEnum>();
                        spellsOff.Add(SpellsEnum.light);
                        break;
                    case InformationalMessageType.ManashieldOff:
                        EnsureManashieldStatus(false);
                        break;
                    case InformationalMessageType.FireshieldOff:
                        ChangeSkillActive(SkillWithCooldownType.Fireshield, false);
                        break;
                    case InformationalMessageType.FireshieldInflictsDamageAndDissipates:
                        if (isFightingMob)
                        {
                            _monsterDamage += next.Damage;
                        }
                        ChangeSkillActive(SkillWithCooldownType.Fireshield, false);
                        break;
                    case InformationalMessageType.FleeFailed:
                        finishedProcessing = true;
                        break;
                    case InformationalMessageType.StunCastOnEnemy:
                        BackgroundWorkerParameters bwp = _currentBackgroundParameters;
                        if (bwp != null && isFightingMob)
                        {
                            _monsterStunnedSince = DateTime.UtcNow;
                        }
                        if (bct.HasValue)
                        {
                            BackgroundCommandType bctValue = bct.Value;
                            if (bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.StunWithWand)
                            {
                                flp.CommandResult = CommandResult.CommandSuccessful;
                            }
                        }
                        finishedProcessing = true;
                        break;
                    case InformationalMessageType.BullroarerInMithlond:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.MithlondExitBullroarer));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.MithlondEnterBullroarer));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerInNindamos:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.NindamosExitBullroarer));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.NindamosEnterBullroarer));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerReadyForBoarding:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.MithlondEnterBullroarer));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.NindamosEnterBullroarer));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.OmaniPrincessInMithlond:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType == BoatEmbarkOrDisembark.OmaniPrincessMithlondBoat)
                            {
                                _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.MithlondExitOmaniPrincess));
                            }
                        }
                        break;
                    case InformationalMessageType.OmaniPrincessInUmbar:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType == BoatEmbarkOrDisembark.OmaniPrincessUmbarBoat)
                            {
                                _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.UmbarExitOmaniPrincess));
                            }
                        }
                        break;
                    case InformationalMessageType.OmaniPrincessReadyForBoarding:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.OmaniPrincessMithlondDock:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.MithlondEnterOmaniPrincess));
                                        break;
                                    case BoatEmbarkOrDisembark.OmaniPrincessUmbarDock:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.UmbarEnterOmaniPrincess));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressInBree:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            bool removeMessage = true;
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.BreeExitCelduinExpress));
                                        removeMessage = false;
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, true, BoatExitType.BreeEnterCelduinExpress));
                                        removeMessage = false;
                                        break;
                                }
                            }
                            if (removeMessage)
                            {
                                if (messagesToRemove == null) messagesToRemove = new List<string>();
                                messagesToRemove.Add(InformationalMessagesSequence.CELDUIN_EXPRESS_IN_BREE_MESSAGE);
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressLeftBree:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, false, BoatExitType.BreeExitCelduinExpress));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, false, BoatExitType.BreeEnterCelduinExpress));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressLeftMithlond:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            Room currentRoom = _currentEntityInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, false, BoatExitType.MithlondExitCelduinExpress));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressMithlond:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, false, BoatExitType.MithlondEnterCelduinExpress));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.HarbringerInPort:
                        HandleHarbringerStatusChange(true);
                        break;
                    case InformationalMessageType.HarbringerSailed:
                        HandleHarbringerStatusChange(false);
                        break;
                    case InformationalMessageType.MobArrived:
                        rc = new EntityChange();
                        rc.ChangeType = EntityChangeType.AddMob;
                        MobTypeEnum nextMob = next.Mob;
                        List<MobTypeEnum> currentRoomMobs = _currentEntityInfo.CurrentRoomMobs;
                        lock (_currentEntityInfo.EntityLock)
                        {
                            int index = currentRoomMobs.LastIndexOf(nextMob);
                            int iInsertionPoint;
                            if (index >= 0)
                            {
                                iInsertionPoint = index + 1;
                            }
                            else
                            {
                                iInsertionPoint = _currentEntityInfo.FindNewMobInsertionPoint(nextMob);
                            }
                            bool insertAtEnd = iInsertionPoint == -1;
                            for (int i = 0; i < next.MobCount; i++)
                            {
                                if (insertAtEnd)
                                    currentRoomMobs.Add(nextMob);
                                else
                                    currentRoomMobs.Insert(iInsertionPoint, nextMob);
                                EntityChangeEntry changeEntry = new EntityChangeEntry();
                                changeEntry.MobType = nextMob;
                                changeEntry.RoomMobAction = true;
                                changeEntry.RoomMobIndex = iInsertionPoint;
                                rc.Changes.Add(changeEntry);
                            }
                            _currentEntityInfo.CurrentEntityChanges.Add(rc);
                        }
                        break;
                    case InformationalMessageType.MobWanderedAway:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            RemoveMobs(next.Mob, next.MobCount);
                        }
                        break;
                    case InformationalMessageType.EquipmentDestroyed:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.DestroyEquipment, null);
                        break;
                    case InformationalMessageType.EquipmentFellApart:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.Unequip, null);
                        break;
                    case InformationalMessageType.WeaponIsBroken:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.Unequip, null);
                        if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
                        {
                            _lastCommandDamage = 0;
                            flp.CommandResult = CommandResult.CommandSuccessful;
                        }
                        break;
                    case InformationalMessageType.ItemMagicallySentToYou:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.MagicallySentItem, null);
                        break;
                    case InformationalMessageType.MobPickedUpItem:
                        lock (_currentEntityInfo.EntityLock)
                        {
                            EntityChange ec = new EntityChange();
                            ec.ChangeType = EntityChangeType.RemoveRoomItems;
                            EntityChangeEntry entry = new EntityChangeEntry();
                            entry.Item = next.Item;
                            if (ec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, next.Item, false, entry))
                            {
                                ec.Changes.Add(entry);
                                _currentEntityInfo.CurrentEntityChanges.Add(ec);
                            }
                        }
                        break;
                    case InformationalMessageType.SomethingPoisoned:
                    case InformationalMessageType.PoisonDamage:
                        _playerStatusFlags |= PlayerStatusFlags.Poisoned;
                        break;
                    case InformationalMessageType.SomethingDiseased:
                    case InformationalMessageType.DiseaseDamage:
                        _playerStatusFlags |= PlayerStatusFlags.Diseased;
                        break;
                }
            }

            if (spellsOff != null)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    bool removedProtection = false;
                    foreach (SpellsEnum nextSpell in spellsOff)
                    {
                        if (_spellsCast.Contains(nextSpell))
                        {
                            removedProtection |= nextSpell == SpellsEnum.protection;
                            _spellsCast.Remove(nextSpell);
                            _refreshSpellsCast = true;
                        }
                    }
                    if (removedProtection)
                    {
                        OnDeltaArmorClass(-1);
                    }
                }
            }

            if (messagesToRemove != null)
            {
                for (int i = flp.Lines.Count - 1; i >= 0; i--)
                {
                    if (messagesToRemove.Contains(flp.Lines[i]))
                    {
                        flp.Lines.RemoveAt(i);
                    }
                }
            }

            ProcessBroadcastMessages(broadcasts, addedPlayers, removedPlayers);

            if (finishedProcessing)
            {
                flp.FinishedProcessing = true;
            }
        }

        private void ProcessBroadcastMessages(List<string> broadcasts, List<string> addedPlayers, List<string> removedPlayers)
        {
            AddBroadcastMessages(broadcasts);

            //add/remove players logging in or out
            //don't worry if the list doesn't match up since the player list isn't necessarily accurate
            if (addedPlayers != null)
            {
                foreach (string s in addedPlayers)
                {
                    if (_players == null) _players = new HashSet<string>();
                    if (!_players.Contains(s))
                    {
                        _players.Add(s);
                    }
                }
            }

            if (removedPlayers != null)
            {
                foreach (string s in removedPlayers)
                {
                    if (_players != null && _players.Contains(s))
                    {
                        _players.Remove(s);
                    }
                }
            }
        }

        /// <summary>
        /// adds items to the room. assumes the entity lock
        /// </summary>
        /// <param name="items">items to add</param>
        private void AddRoomItems(List<ItemEntity> items)
        {
            EntityChange rc = new EntityChange();
            rc.ChangeType = EntityChangeType.CreateRoomItems;
            foreach (ItemEntity nextItem in items)
            {
                EntityChangeEntry entry = new EntityChangeEntry();
                entry.Item = nextItem;
                entry.RoomItemAction = true;
                int iInsertionPoint = _currentEntityInfo.FindNewRoomItemInsertionPoint(nextItem);
                entry.RoomItemIndex = iInsertionPoint;
                rc.Changes.Add(entry);
                if (iInsertionPoint == -1)
                    _currentEntityInfo.CurrentRoomItems.Add(nextItem);
                else
                    _currentEntityInfo.CurrentRoomItems.Insert(iInsertionPoint, nextItem);
            }
            if (rc.Changes.Count > 0)
            {
                _currentEntityInfo.CurrentEntityChanges.Add(rc);
            }
        }

        /// <summary>
        /// removes mobs from the room. assumes the entity lock.
        /// </summary>
        /// <param name="mobType">mob type</param>
        /// <param name="Count">number of mobs</param>
        private void RemoveMobs(MobTypeEnum mobType, int Count)
        {
            EntityChange rc = new EntityChange();
            rc.ChangeType = EntityChangeType.RemoveMob;
            List<MobTypeEnum> currentRoomMobs = _currentEntityInfo.CurrentRoomMobs;
            int index = currentRoomMobs.LastIndexOf(mobType);
            if (index >= 0)
            {
                int iLastIndexCount = Count - 1;
                for (int i = 0; i < Count; i++)
                {
                    if (currentRoomMobs[index] == mobType)
                    {
                        currentRoomMobs.RemoveAt(index);
                        EntityChangeEntry changeEntry = new EntityChangeEntry();
                        changeEntry.MobType = mobType;
                        changeEntry.RoomMobIndex = index;
                        changeEntry.RoomMobAction = false;
                        rc.Changes.Add(changeEntry);
                    }
                    else
                    {
                        break;
                    }
                    if (index == 0)
                    {
                        break;
                    }
                    else if (i != iLastIndexCount)
                    {
                        index--;
                    }
                }
                if (rc.Changes.Count > 0)
                {
                    _currentEntityInfo.CurrentEntityChanges.Add(rc);
                }
            }
        }

        private void HandleHarbringerStatusChange(bool inPort)
        {
            lock (_currentEntityInfo.EntityLock)
            {
                Room currentRoom = _currentEntityInfo.CurrentRoom;
                if (currentRoom.BoatLocationType.HasValue)
                {
                    BoatExitType? boatExitType = null;
                    switch (currentRoom.BoatLocationType.Value)
                    {
                        case BoatEmbarkOrDisembark.Harbringer:
                            boatExitType = BoatExitType.MithlondExitHarbringer;
                            break;
                        case BoatEmbarkOrDisembark.HarbringerMithlond:
                            boatExitType = BoatExitType.MithlondEnterHarbringer;
                            break;
                        case BoatEmbarkOrDisembark.HarbringerTharbad:
                            boatExitType = BoatExitType.TharbadEnterHarbringer;
                            break;
                    }
                    if (boatExitType.HasValue)
                    {
                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForBoatExitOrEntrance(currentRoom, inPort, boatExitType.Value));
                    }
                }
            }
        }

        /// <summary>
        /// gets a room change object for a boat entrance/exit
        /// </summary>
        /// <param name="currentRoom">current room</param>
        /// <param name="add">true to add the exit, false to remove the exit</param>
        /// <param name="exitText">exit text</param>
        /// <returns>room change object</returns>
        private EntityChange GetAddExitRoomChangeForBoatExitOrEntrance(Room currentRoom, bool add, BoatExitType boatExitType)
        {
            EntityChange rc = new EntityChange();
            rc.ChangeType = add ? EntityChangeType.AddExit : EntityChangeType.RemoveExit;
            Exit e = IsengardMap.GetRoomExits(currentRoom, (exit) => { return exit.BoatExitType == boatExitType; }).First();
            rc.Exits.Add(e.ExitText);
            if (add)
            {
                rc.MappedExits[e.ExitText] = e;
            }
            return rc;
        }

        private void OnInventoryManagement(FeedLineParameters flParams, List<ItemEntity> items, ItemManagementAction action, int? gold, int sellGold, List<SpellsEnum> activeSpells, bool potionConsumed, PlayerStatusFlags statusFlagsRemoved)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            bool inBackgroundCommand = bct.HasValue;
            BackgroundCommandType bctValue = BackgroundCommandType.Quit;
            if (bct.HasValue) bctValue = bct.Value;
            SelectedInventoryOrEquipmentItem sioei = bctValue == BackgroundCommandType.Trade ? _commandInventoryItem : null;

            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.RemoveAll) == InitializationStep.None;
            bool hasItems = items != null && items.Count > 0;
            bool hasSpells = activeSpells != null && activeSpells.Count > 0;
            bool hasGold = gold.HasValue || sellGold > 0;
            bool couldBeRemoveAll = !hasSpells && !hasGold && action == ItemManagementAction.Unequip;
            if (hasItems)
            {
                AddOrRemoveItemsFromInventoryOrEquipment(flParams, items, action, sioei);
            }
            if (gold.HasValue)
            {
                _gold = gold.Value;
            }
            else if (sellGold > 0)
            {
                _gold += sellGold;
            }
            if (hasSpells)
            {
                AddActiveSpells(activeSpells);
            }
            if (potionConsumed && flParams.BackgroundCommandType.HasValue && flParams.BackgroundCommandType.Value == BackgroundCommandType.DrinkNonHazyPotion)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
            if (statusFlagsRemoved != PlayerStatusFlags.None)
            {
                _playerStatusFlags &= ~statusFlagsRemoved;
            }
            if (inBackgroundCommand)
            {
                if (action == ItemManagementAction.PickUpItem && bctValue == BackgroundCommandType.GetItem)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                    flParams.CommandSpecificResult = Convert.ToInt32(GetItemResult.Success);
                }
                else if ((action == ItemManagementAction.SellItem && bctValue == BackgroundCommandType.SellItem) ||
                    (action == ItemManagementAction.DropItem && bctValue == BackgroundCommandType.DropItem) ||
                    (action == ItemManagementAction.Unequip && bctValue == BackgroundCommandType.RemoveEquipment) ||
                    (action == ItemManagementAction.Trade && bctValue == BackgroundCommandType.Trade) ||
                    (action == ItemManagementAction.WieldItem && bctValue == BackgroundCommandType.WieldWeapon) ||
                    (action == ItemManagementAction.HoldItem && bctValue == BackgroundCommandType.HoldItem) ||
                    (action == ItemManagementAction.WearItem && bctValue == BackgroundCommandType.WearEquipment))
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                }
            }
            if (forInit && couldBeRemoveAll)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.RemoveAll, flParams);
            }
        }

        private void ProcessItemSeen(IEnumerable<ItemEntity> items)
        {
            if (items != null)
            {
                foreach (ItemEntity ie in items)
                {
                    if (ie.ItemType.HasValue && !_itemsSeen.Contains(ie.ItemType.Value))
                    {
                        ItemTypeEnum itemType = ie.ItemType.Value;
                        StaticItemData sid = ItemEntity.StaticItemData[itemType];

                        List<string> messages = new List<string>();

                        if (sid.ItemClass == ItemClass.Other)
                        {
                            messages.Add("Unknown item type for " + itemType);
                        }
                        if (sid.LookTextType == LookTextType.None)
                        {
                            messages.Add("No look text information for " + itemType);
                        }
                        else if (sid.LookTextType == LookTextType.Known && string.IsNullOrEmpty(sid.LookText))
                        {
                            messages.Add("Missing look text information for " + itemType);
                        }
                        if (sid.WeaponType.HasValue && sid.WeaponType == WeaponType.Unknown)
                        {
                            messages.Add("Unknown weapon type for " + itemType);
                        }
                        if ((!sid.SexRestriction.HasValue || _sex == sid.SexRestriction.Value) && 
                            ((sid.DisallowedClasses & _classFlags) == ClassTypeFlags.None))
                        {
                            if (sid.EquipmentType == EquipmentType.Unknown)
                            {
                                messages.Add("Unknown equipment type for " + itemType);
                            }
                            else if (sid.EquipmentType != EquipmentType.Wielded && sid.EquipmentType != EquipmentType.Holding && sid.ArmorClass <= 0)
                            {
                                messages.Add("Missing armor class for " + itemType);
                            }
                        }
                        if (!sid.IsCurrency() && sid.ItemClass != ItemClass.Fixed && sid.ItemClass != ItemClass.Chest)
                        {
                            if (!sid.Weight.HasValue)
                            {
                                messages.Add("Unknown weight for " + itemType);
                            }
                            if (sid.Sellable == SellableEnum.Unknown)
                            {
                                messages.Add("Unknown sellable for " + itemType);
                            }
                        }
                        if (messages.Count > 0)
                        {
                            AddBroadcastMessages(messages);
                        }
                        _itemsSeen.Add(itemType);
                    }
                }
            }
        }

        private void AddOrRemoveItemsFromInventoryOrEquipment(FeedLineParameters flParams, List<ItemEntity> items, ItemManagementAction action, SelectedInventoryOrEquipmentItem sioei)
        {
            EntityChangeType changeType;
            if (action == ItemManagementAction.WearItem || action == ItemManagementAction.WieldItem || action == ItemManagementAction.HoldItem)
            {
                changeType = EntityChangeType.EquipItem;
            }
            else if (action == ItemManagementAction.Unequip)
            {
                changeType = EntityChangeType.UnequipItem;
            }
            else if (action == ItemManagementAction.PickUpItem)
            {
                changeType = EntityChangeType.PickUpItem;
            }
            else if (action == ItemManagementAction.Trade)
            {
                changeType = EntityChangeType.Trade;
            }
            else if (action == ItemManagementAction.MagicallySentItem)
            {
                changeType = EntityChangeType.MagicallySentItem;
            }
            else if (action == ItemManagementAction.DropItem)
            {
                changeType = EntityChangeType.DropItem;
            }
            else if (action == ItemManagementAction.ConsumeItem || action == ItemManagementAction.SellItem)
            {
                changeType = EntityChangeType.ConsumeItem;
            }
            else if (action == ItemManagementAction.DestroyEquipment)
            {
                changeType = EntityChangeType.DestroyEquipment;
            }
            else
            {
                throw new InvalidOperationException();
            }

            ProcessItemSeen(items);

            EntityChange iec = new EntityChange();
            iec.ChangeType = changeType;
            lock (_currentEntityInfo.EntityLock)
            {
                EntityChangeEntry changeEntry;
                decimal? armorClassDelta = 0;
                foreach (ItemEntity nextItemEntity in items)
                {
                    bool addChange = false;
                    changeEntry = new EntityChangeEntry();
                    changeEntry.Item = nextItemEntity;
                    bool removeEquipment = changeType == EntityChangeType.UnequipItem || changeType == EntityChangeType.DestroyEquipment;
                    StaticItemData sid = null;
                    ItemTypeEnum nextItem = ItemTypeEnum.GoldCoins;
                    if (nextItemEntity.ItemType.HasValue)
                    {
                        nextItem = nextItemEntity.ItemType.Value;
                        sid = ItemEntity.StaticItemData[nextItem];
                    }
                    if (changeType == EntityChangeType.EquipItem || removeEquipment)
                    {
                        if (sid == null)
                        {
                            armorClassDelta = null;
                        }
                        else if (armorClassDelta.HasValue)
                        {
                            if (sid.EquipmentType != EquipmentType.Wielded && sid.EquipmentType != EquipmentType.Holding)
                            {
                                if (sid.ArmorClass <= 0)
                                    armorClassDelta = null;
                                else if (removeEquipment)
                                    armorClassDelta = armorClassDelta.Value - sid.ArmorClass;
                                else
                                    armorClassDelta = armorClassDelta + sid.ArmorClass;
                            }
                        }
                        if (sid != null && sid.EquipmentType != EquipmentType.Unknown)
                        {
                            foreach (EquipmentSlot nextSlot in CurrentEntityInfo.GetSlotsForEquipmentType(sid.EquipmentType, removeEquipment))
                            {
                                int iSlotIndex = (int)nextSlot;
                                if (changeType == EntityChangeType.EquipItem)
                                {
                                    if (_currentEntityInfo.Equipment[iSlotIndex] == null)
                                    {
                                        changeEntry.EquipmentIndex = _currentEntityInfo.FindNewEquipmentInsertionPoint(nextSlot);
                                        changeEntry.EquipmentAction = true;
                                        _currentEntityInfo.Equipment[iSlotIndex] = nextItemEntity;
                                        addChange = true;
                                        break;
                                    }
                                }
                                else //remove
                                {
                                    ItemEntity existingEquipment = _currentEntityInfo.Equipment[iSlotIndex];
                                    if (existingEquipment != null && existingEquipment.ItemType == nextItem)
                                    {
                                        changeEntry.EquipmentIndex = _currentEntityInfo.FindEquipmentRemovalPoint(nextSlot);
                                        changeEntry.EquipmentAction = false;
                                        _currentEntityInfo.Equipment[iSlotIndex] = null;
                                        addChange = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else //don't know what slot to put the equipment in, so it must go in the unknown list
                        {
                            string sItemName;
                            if (sid == null)
                                sItemName = ((UnknownItemEntity)nextItemEntity).Name;
                            else
                                sItemName = sid.SingularName;
                            int index = _currentEntityInfo.GetTotalKnownEquipmentCount();
                            bool found = false;
                            for (int i = 0; i < _currentEntityInfo.UnknownEquipment.Count; i++)
                            {
                                ItemEntity ie = _currentEntityInfo.UnknownEquipment[i];
                                string sNextItemName;
                                if (ie.ItemType.HasValue)
                                    sNextItemName = ItemEntity.StaticItemData[ie.ItemType.Value].SingularName;
                                else
                                    sNextItemName = ((UnknownItemEntity)ie).Name;
                                if (removeEquipment)
                                {
                                    if (sNextItemName == sItemName)
                                    {
                                        changeEntry.EquipmentAction = false;
                                        changeEntry.EquipmentIndex = index;
                                        _currentEntityInfo.UnknownEquipment.RemoveAt(i);
                                        addChange = true;
                                        found = true;
                                        break;
                                    }
                                    else
                                    {
                                        index++;
                                    }
                                }
                                else //equip
                                {
                                    if (sNextItemName.CompareTo(sItemName) > 0)
                                    {
                                        changeEntry.EquipmentAction = true;
                                        changeEntry.EquipmentIndex = index;
                                        _currentEntityInfo.UnknownEquipment.Insert(index, nextItemEntity);
                                        addChange = true;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (changeType == EntityChangeType.EquipItem && !found)
                            {
                                changeEntry.EquipmentAction = true;
                                changeEntry.EquipmentIndex = -1;
                                _currentEntityInfo.UnknownEquipment.Add(nextItemEntity);
                                addChange = true;
                            }
                        }
                        if (changeType == EntityChangeType.EquipItem)
                        {
                            addChange |= iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, false, changeEntry);
                        }
                        else if (changeType == EntityChangeType.UnequipItem)
                        {
                            addChange |= iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, true, changeEntry);
                        }
                    }
                    else //equipment not involved
                    {
                        if (sid == null || !sid.IsCurrency()) //add/remove from inventory if not currency
                        {
                            bool isAddToInventory = changeType == EntityChangeType.PickUpItem || changeType == EntityChangeType.MagicallySentItem || changeType == EntityChangeType.Trade;
                            addChange |= iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, isAddToInventory, changeEntry);
                        }
                        if (changeType == EntityChangeType.PickUpItem) //remove from room items
                        {
                            addChange |= iec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, nextItemEntity, false, changeEntry);
                        }
                        if (changeType == EntityChangeType.DropItem) //add to room items
                        {
                            addChange |= iec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, nextItemEntity, true, changeEntry);
                        }
                    }
                    if (addChange)
                    {
                        iec.Changes.Add(changeEntry);
                    }
                }
                if (armorClassDelta.HasValue && armorClassDelta.Value != 0)
                {
                    OnDeltaArmorClass(armorClassDelta.Value);
                }
                if (sioei != null && action == ItemManagementAction.Trade) //remove traded item from inventory
                {
                    int iActualIndex = _currentEntityInfo.PickActualIndexFromItemCounter(ItemLocationType.Inventory, sioei.ItemType.Value, sioei.Counter, false);
                    if (iActualIndex >= 0)
                    {
                        _currentEntityInfo.InventoryItems.RemoveAt(iActualIndex);
                        changeEntry = new EntityChangeEntry();
                        changeEntry.InventoryAction = false;
                        changeEntry.InventoryIndex = iActualIndex;
                        iec.Changes.Add(changeEntry);
                    }
                }
                if (iec.Changes.Count > 0)
                {
                    bool changedInventory = false;
                    bool changedEquipment = false;
                    foreach (var nextChange in iec.Changes)
                    {
                        changedInventory |= nextChange.InventoryAction.HasValue;
                        changedEquipment |= nextChange.EquipmentAction.HasValue;
                    }
                    _currentEntityInfo.CurrentEntityChanges.Add(iec);
                    if (changedInventory) _currentEntityInfo.ComputeTotalInventoryWeight();
                    if (changedEquipment) _currentEntityInfo.ComputeTotalEquipmentWeight();
                }
            }
        }

        private void _bwFullLog_DoWork(object sender, DoWorkEventArgs e)
        {
            string logPath = Path.Combine(Path.GetTempPath(), "Isengard");
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                DateTime dtUTCNow = DateTime.UtcNow;
                string sFileName = Path.Combine(logPath, StringProcessing.GetDateTimeForDisplay(dtUTCNow, false, true) + ".txt");
                List<string> nextLines = new List<string>();
                using (StreamWriter sw = new StreamWriter(sFileName, true))
                {
                    while (!_bwFullLog.CancellationPending)
                    {
                        bool didSomething = false;
                        nextLines.Clear();
                        lock (_fullLogLock)
                        {
                            if (_fullLogLines.Count > 0)
                            {
                                didSomething = true;
                                nextLines.AddRange(_fullLogLines);
                                _fullLogLines.Clear();
                            }
                        }
                        foreach (string s in nextLines)
                        {
                            sw.Write(s);
                        }
                        if (!didSomething)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddConsoleMessage("Full log error: " + ex.ToString());
            }
            finally
            {
                _fullLogFinished = true;
            }
        }

        private void _bwNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> currentOutputItemData = new List<int>();
            List<int> nextCharacterQueue = new List<int>();

            List<IOutputItemSequence> outputItemSequences = new List<IOutputItemSequence>()
            {
                new ConstantOutputItemSequence(OutputItemSequenceType.UserNamePrompt, "Please enter name: ", _asciiMapping),
                new ConstantOutputItemSequence(OutputItemSequenceType.PasswordPrompt, "Please enter password: ", _asciiMapping),
                new ConstantOutputItemSequence(OutputItemSequenceType.ContinueToNextScreen, "[Hit Return, Q to Quit]: ", _asciiMapping),
                new ConstantOutputItemSequence(OutputItemSequenceType.Goodbye, "Goodbye!", _asciiMapping),
                new HPMPSequence(),
            };
            List<AOutputProcessingSequence> seqs = GetProcessingSequences();
            while (true)
            {
                int nextByte = _tcpClientNetworkStream.ReadByte();
                if (nextByte == -1) //closed connection
                {
                    return;
                }
                else
                {
                    OutputItemInfo oii = null;
                    foreach (IOutputItemSequence nextSequence in outputItemSequences)
                    {
                        oii = nextSequence.FeedByte(nextByte);
                        if (oii != null) break;
                    }
                    currentOutputItemData.Add(nextByte);
                    InputEchoType echoType = InputEchoType.On;
                    if (oii != null)
                    {
                        bool hpormpchanged = false;
                        OutputItemSequenceType oist = oii.SequenceType;
                        if (oist == OutputItemSequenceType.UserNamePrompt)
                        {
                            _promptedUserName = true;
                        }
                        else if (oist == OutputItemSequenceType.PasswordPrompt)
                        {
                            _promptedPassword = true;
                        }
                        else if (oist == OutputItemSequenceType.Goodbye)
                        {
                            if (_backgroundCommandType == BackgroundCommandType.Quit)
                            {
                                _commandResult = CommandResult.CommandSuccessful;
                                _commandSpecificResult = 0;
                                _commandInventoryItem = null;
                            }
                        }
                        else if (oist == OutputItemSequenceType.HPMPStatus)
                        {
                            int iNewHP = oii.HP;
                            int iNewMP = oii.MP;
                            bool hpChanged = iNewHP != _autohp;
                            bool mpChanged = iNewMP != _automp;
                            if (hpChanged || mpChanged)
                            {
                                OnHPOrMPChanged(iNewHP, hpChanged, iNewMP, mpChanged);
                            }
                            _currentStatusLastComputed = DateTime.UtcNow;
                        }

                        StringBuilder sb = new StringBuilder();
                        foreach (int nextOutputItemByte in currentOutputItemData)
                        {
                            ProcessInputCharacter(nextOutputItemByte, nextCharacterQueue, sb);
                        }

                        string sNewLine = sb.ToString();
                        if (_fullLogLock != null)
                        {
                            lock (_fullLogLock)
                            {
                                _fullLogLines.Add(sNewLine);
                            }
                        }

                        bool haveStatus = oist == OutputItemSequenceType.HPMPStatus;
                        if (haveStatus) //strip status "(HP X, MP Y)"
                        {
                            int lastParenthesisLocation = sNewLine.LastIndexOf('(');
                            if (lastParenthesisLocation == 0)
                                sNewLine = string.Empty;
                            else
                                sNewLine = sNewLine.Substring(0, lastParenthesisLocation);
                        }

                        bool haveContent = false;
                        if (!string.IsNullOrEmpty(sNewLine))
                        {
                            List<string> sNewLinesList = new List<string>(sNewLine.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                            int initialCount = sNewLinesList.Count;
                            FeedLineParameters flParams = new FeedLineParameters(sNewLinesList);
                            flParams.BackgroundCommandType = _backgroundCommandType;
                            flParams.IsFightingMob = !string.IsNullOrEmpty(_currentlyFightingMobText) || _currentlyFightingMobType.HasValue;
                            IsengardSettingData sets = _settingsData;
                            flParams.ConsoleVerbosity = sets == null ? ConsoleOutputVerbosity.Maximum : sets.ConsoleVerbosity;
                            flParams.PlayerNames = _players;
                            flParams.RunningHiddenCommand = _runningHiddenCommand;
                            flParams.SIOEI = _commandInventoryItem;
                            CommandResult? previousCommandResult = _commandResult;

                            foreach (AOutputProcessingSequence nextProcessingSequence in seqs)
                            {
                                try
                                {
                                    nextProcessingSequence.FeedLine(flParams);
                                }
                                catch (Exception ex)
                                {
                                    lock (_consoleTextLock)
                                    {
                                        _newConsoleText.Add(ex.ToString());
                                    }
                                }
                                if (flParams.SuppressEcho)
                                {
                                    echoType = InputEchoType.Off;
                                }
                                if (flParams.FinishedProcessing)
                                {
                                    break;
                                }
                            }

                            AddBroadcastMessages(flParams.ErrorMessages);

                            //recompute the lines if they were changed by sequence logic
                            bool linesChanged = sNewLinesList.Count != initialCount;
                            if (linesChanged)
                            {
                                sNewLine = string.Join(Environment.NewLine, sNewLinesList);
                            }
                            haveContent = !string.IsNullOrWhiteSpace(sNewLine);
                            if (haveContent)
                            {
                                //if this set of output completed the command, include the command input in the console output,
                                //except for maximum verbosity in which case the input was sent to the console immediately
                                if (_settingsData != null && _settingsData.ConsoleVerbosity == ConsoleOutputVerbosity.Minimum && !previousCommandResult.HasValue && flParams.CommandResult.HasValue)
                                {
                                    sNewLine = _lastCommand + Environment.NewLine + sNewLine;
                                }
                                if (flParams.CommandResult.HasValue)
                                {
                                    _commandResult = flParams.CommandResult.Value;
                                    _commandSpecificResult = flParams.CommandSpecificResult;
                                }
                            }
                        }

                        bool showMessageWithStatus = haveContent || hpormpchanged;
                        if (haveStatus && showMessageWithStatus)
                        {
                            sNewLine = sNewLine + ": ";
                        }

                        if (echoType == InputEchoType.On && (!haveStatus || showMessageWithStatus))
                        {
                            lock (_consoleTextLock)
                            {
                                _newConsoleText.Add(sNewLine);
                            }
                        }

                        currentOutputItemData.Clear();
                        nextCharacterQueue.Clear();
                    }
                }
            }
        }

        private void OnHPOrMPChanged(int newHP, bool hpChanged, int newMP, bool mpChanged)
        {
            lock (_escapeLock)
            {
                if (_settingsData != null && _settingsData.AutoEscapeActive && _settingsData.AutoEscapeThreshold > 0 && newHP <= _settingsData.AutoEscapeThreshold)
                {
                    if (_settingsData.AutoEscapeType == AutoEscapeType.Flee)
                        _fleeing = true;
                    else if (_settingsData.AutoEscapeType == AutoEscapeType.Hazy)
                        _hazying = true;
                    _settingsData.AutoEscapeActive = false;
                }
            }
            if (_currentBackgroundParameters == null)
            {
                bool gotFullHP = hpChanged && newHP == _totalhp;
                bool gotFullMP = mpChanged && newMP == _totalmp;
                if (gotFullHP || gotFullMP)
                {
                    List<string> messages = new List<string>();
                    if (gotFullHP) messages.Add("Your hitpoints are full.");
                    if (gotFullMP) messages.Add("Your mana is full.");
                    AddConsoleMessages(messages);
                }
            }
            _autohp = newHP;
            _automp = newMP;
        }

        private List<AOutputProcessingSequence> GetProcessingSequences()
        {
            List<AOutputProcessingSequence> seqs = new List<AOutputProcessingSequence>
            {
                new SearchSequence(SuccessfulSearch, FailSearch),
                new MobStatusSequence(OnMobStatusSequence), //goes before informational messages since informational messages can hang off look at mob results
                new ItemStatusSequence(),
                new AttackSequence(OnAttack), //goes before informational messages since informational messages can hang off attack results
                new CastOffensiveSpellSequence(OnCastOffensiveSpell),
                new RoomTransitionSequence(_username, OnRoomTransition),
                new InformationalMessagesSequence(_username, OnInformationalMessages),
                new InitialLoginSequence(OnInitialLogin),
                new ScoreOutputSequence(_username, OnScore),
                new InformationOutputSequence(OnInformation),
                new WhoOutputSequence(OnWho),
                new UptimeOutputSequence(OnUptime),
                new SpellsSequence(OnSpells),
                new InventorySequence(OnInventory),
                new EquipmentSequence(OnEquipment),
                _pleaseWaitSequence,
                new FailMovementSequence(FailMovement),
                new EntityAttacksYouSequence(OnEntityAttacksYou),
                new InventoryEquipmentManagementSequence(OnInventoryManagement),
                new ConstantOutputSequence("You creative a protective manashield.", OnManashieldOn, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Manashield }),
                new ConstantOutputSequence("You already have a manashield!", OnManashieldOn, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("You create a protective fireshield.", OnFireshieldOn, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("You already have a fireshield active!", OnFireshieldOn, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains, null, null), //could be prefixed by "Scared of going X"*
                new SelfSpellCastSequence(OnSelfSpellCast),
                new ConstantOutputSequence("Your spell fails.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. alignment out of whack
                new ConstantOutputSequence("You don't know that spell.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells),
                new ConstantOutputSequence("Nothing happens.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. casting a spell from the tree of life
                new ConstantOutputSequence("You don't see that here.", OnYouDontSeeThatHere, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Attack, BackgroundCommandType.LookAtMob }),
                new ConstantOutputSequence("That is not here.", OnThatIsNotHere, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Attack, BackgroundCommandType.Trade }), //triggered by power attack
                new ConstantOutputSequence("That's not here.", OnCastOffensiveSpellMobNotPresent, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun, BackgroundCommandType.StunWithWand }),
                new ConstantOutputSequence("You cannot harm him.", OnTryAttackUnharmableMob, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun, BackgroundCommandType.StunWithWand, BackgroundCommandType.Attack }),
                new ConstantOutputSequence("You cannot harm her.", OnTryAttackUnharmableMob, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun, BackgroundCommandType.StunWithWand, BackgroundCommandType.Attack }),
                new ConstantOutputSequence("It's not locked.", ExitIsNotLocked, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Knock, BackgroundCommandType.UnlockExit }),
                new ConstantOutputSequence("You successfully open the lock.", SuccessfulKnock, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Knock }),
                new ConstantOutputSequence("You failed.", FailKnock, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Knock }),
                new ConstantOutputSequence("You don't have that.", FailItemAction, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion, BackgroundCommandType.SellItem, BackgroundCommandType.DropItem, BackgroundCommandType.Trade, BackgroundCommandType.WieldWeapon, BackgroundCommandType.HoldItem, BackgroundCommandType.UnlockExit, BackgroundCommandType.StunWithWand, BackgroundCommandType.WearEquipment }),
                new ConstantOutputSequence(" starts to evaporates before you drink it.", PotionEvaporatesBeforeDrinking, ConstantSequenceMatchType.EndsWith, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion }),
                new ConstantOutputSequence("You prepare yourself for traps.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Prepare }),
                new ConstantOutputSequence("You've already prepared.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Prepare }),
                new ConstantOutputSequence("I don't see that exit.", CantSeeExit, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OpenDoor }),
                new ConstantOutputSequence("You open the ", OpenDoorSuccess, ConstantSequenceMatchType.StartsWith, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OpenDoor }),
                new ConstantOutputSequence("It's already open.", OpenDoorSuccess, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OpenDoor }),
                new ConstantOutputSequence("You see nothing special about it.", OnSeeNothingSpecial, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.LookAtMob }),
                new ConstantOutputSequence("That isn't here.", OnCannotPickUpItemNotHere, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.GetItem }),
                new ConstantOutputSequence("You may not take that. You have already fulfilled that quest.", OnCannotPickUpItemQuestFulfilled, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.GetItem }),
                new ConstantOutputSequence("You can't carry anymore.", OnCannotCarryAnymore, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.GetItem }),
                new ConstantOutputSequence("You can't take that!", OnCannotPickUpItemFixedItem, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.GetItem }),
                new ConstantOutputSequence(" won't let you take anything.", OnCannotPickUpItemMobDisallows, ConstantSequenceMatchType.EndsWith, 0, new List<BackgroundCommandType>() { BackgroundCommandType.GetItem }),
                new ConstantOutputSequence("The shopkeep says, \"I won't buy that crap from you.\"", OnCannotSellItem, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.SellItem }),
                new ConstantOutputSequence("The shopkeep won't buy that from you.", OnCannotSellItem, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.SellItem }),
                new ConstantOutputSequence("You aren't using that.", OnCannotRemoveEquipment, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.RemoveEquipment }),
                new ConstantOutputSequence("You can't.  It's cursed!", OnCannotRemoveEquipment, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.RemoveEquipment }),
                new ConstantOutputSequence("You stand up.", OnStandUp, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("You are already standing.", OnStandUp, ConstantSequenceMatchType.ExactMatch, 0, null),
                new ConstantOutputSequence("You're already wielding something.", OnCannotWieldWeapon, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.WieldWeapon }),
                new ConstantOutputSequence("You need to train in the ", " proficiency in order to use this weapon.", OnCannotWieldWeapon, 0, new List<BackgroundCommandType>() { BackgroundCommandType.WieldWeapon }),
                new ConstantOutputSequence("You need a ", " to use this weapon effectively.", OnCannotWieldWeapon, 0, new List<BackgroundCommandType>() { BackgroundCommandType.WieldWeapon}),
                new ConstantOutputSequence("You can't wield that.", OnCannotWieldWeapon, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.WieldWeapon }),
                new ConstantOutputSequence("You can't hold that.", OnCannotHoldItem, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.HoldItem }),
                new ConstantOutputSequence("You attempt to hide in the shadows.", FailHide, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Hide }),
                new ConstantOutputSequence("You slip into the shadows unnoticed.", SuccessfulHide, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Hide }),
                new ConstantOutputSequence("You are already hidden.", SuccessfulHide, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Hide }),
                new ConstantOutputSequence("Unlock what?", FailUnlockAlways, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() {  BackgroundCommandType.UnlockExit}),
                new ConstantOutputSequence("Wrong key.", FailUnlockAlways, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() {  BackgroundCommandType.UnlockExit}),
                new ConstantOutputSequence("Click.", SucceedUnlock, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() {  BackgroundCommandType.UnlockExit}),
                new ConstantOutputSequence("*click*", SucceedUnlock, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() {  BackgroundCommandType.UnlockExit}), //factory key
                new ConstantOutputSequence(" is broken.", FailUnlockThisTime, ConstantSequenceMatchType.EndsWith, 0, new List<BackgroundCommandType>() {  BackgroundCommandType.UnlockExit}), //e.g. The tomb key is broken.
                new ConstantOutputSequence("It's used up.", FailStunWithWandUsedUp, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.StunWithWand }),
            };
            return seqs;
        }

        private void ProcessInputCharacter(int nextByte, List<int> queue, StringBuilder textBuilder)
        {
            if (nextByte == 13) //carriage return
            {
                if (queue.Count == 1 && queue[0] == 10)
                {
                    textBuilder.AppendLine();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else if (nextByte == 10) //line feed
            {
                queue.Clear();
                queue.Add(10);
            }
            else if (nextByte == 27) //escape
            {
                queue.Clear();
                queue.Add(27);
            }
            else if (queue.Count > 0 && queue[0] == 27)
            {
                if (nextByte == 109) //ends the escape sequence
                {
                    ConsoleColor? cc;
                    if (queue[1] == 91 && queue[2] == 51)
                    {
                        switch (queue[3])
                        {
                            case 48:
                                cc = ConsoleColor.Black; //30
                                break;
                            case 49:
                                cc = ConsoleColor.Red; //31
                                break;
                            case 50:
                                cc = ConsoleColor.Green; //32
                                break;
                            case 51:
                                cc = ConsoleColor.Yellow; //33
                                break;
                            case 52:
                                cc = ConsoleColor.Blue; //34
                                break;
                            case 53:
                                cc = ConsoleColor.Magenta; //35
                                break;
                            case 54:
                                cc = ConsoleColor.Cyan; //36
                                break;
                            case 55:
                                cc = ConsoleColor.White; //37
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    if (cc.HasValue)
                    {
                        Console.ForegroundColor = cc.Value;
                    }
                    queue.Clear();
                }
                else
                {
                    queue.Add(nextByte);
                }
            }
            else
            {
                char c = '?';
                bool isUnknown = false;
                switch (nextByte)
                {
                    case 0:
                        isUnknown = true;
                        break;
                    case 1:
                        isUnknown = true;
                        break;
                    case 9:
                        c = '\t';
                        break;
                    case AsciiMapping.ASCII_SPACE:
                        c = ' ';
                        break;
                    case 33:
                        c = '!';
                        break;
                    case 34:
                        c = '"';
                        break;
                    case 35:
                        c = '#';
                        break;
                    case 36:
                        c = '$';
                        break;
                    case 37:
                        c = '%';
                        break;
                    case 38:
                        c = '&';
                        break;
                    case 39:
                        c = '\'';
                        break;
                    case AsciiMapping.ASCII_LEFT_PAREN:
                        c = '(';
                        break;
                    case AsciiMapping.ASCII_RIGHT_PAREN:
                        c = ')';
                        break;
                    case 42:
                        c = '*';
                        break;
                    case 43:
                        c = '+';
                        break;
                    case 44:
                        c = ',';
                        break;
                    case 45:
                        c = '-';
                        break;
                    case 46:
                        c = '.';
                        break;
                    case 47:
                        c = '/';
                        break;
                    case AsciiMapping.ASCII_NUMBER_ZERO:
                        c = '0';
                        break;
                    case 49:
                        c = '1';
                        break;
                    case 50:
                        c = '2';
                        break;
                    case 51:
                        c = '3';
                        break;
                    case 52:
                        c = '4';
                        break;
                    case 53:
                        c = '5';
                        break;
                    case 54:
                        c = '6';
                        break;
                    case 55:
                        c = '7';
                        break;
                    case 56:
                        c = '8';
                        break;
                    case AsciiMapping.ASCII_NUMBER_NINE:
                        c = '9';
                        break;
                    case AsciiMapping.ASCII_COLON:
                        c = ':';
                        break;
                    case 59:
                        c = ';';
                        break;
                    case 60:
                        c = '<';
                        break;
                    case 61:
                        c = '=';
                        break;
                    case 62:
                        c = '>';
                        break;
                    case 63:
                        c = '?';
                        break;
                    case 64:
                        c = '@';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_A:
                        c = 'A';
                        break;
                    case 66:
                        c = 'B';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_C:
                        c = 'C';
                        break;
                    case 68:
                        c = 'D';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_E:
                        c = 'E';
                        break;
                    case 70:
                        c = 'F';
                        break;
                    case 71:
                        c = 'G';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_H:
                        c = 'H';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_I:
                        c = 'I';
                        break;
                    case 74:
                        c = 'J';
                        break;
                    case 75:
                        c = 'K';
                        break;
                    case 76:
                        c = 'L';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_M:
                        c = 'M';
                        break;
                    case 78:
                        c = 'N';
                        break;
                    case 79:
                        c = 'O';
                        break;
                    case 80:
                        c = 'P';
                        break;
                    case 81:
                        c = 'Q';
                        break;
                    case 82:
                        c = 'R';
                        break;
                    case 83:
                        c = 'S';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_T:
                        c = 'T';
                        break;
                    case 85:
                        c = 'U';
                        break;
                    case AsciiMapping.ASCII_UPPERCASE_V:
                        c = 'V';
                        break;
                    case 87:
                        c = 'W';
                        break;
                    case 88:
                        c = 'X';
                        break;
                    case 89:
                        c = 'Y';
                        break;
                    case 90:
                        c = 'Z';
                        break;
                    case AsciiMapping.ASCII_LEFT_BRACKET:
                        c = '[';
                        break;
                    case 92:
                        c = '\\';
                        break;
                    case AsciiMapping.ASCII_RIGHT_BRACKET:
                        c = ']';
                        break;
                    case 95:
                        c = '_';
                        break;
                    case 96:
                        c = '`';
                        break;
                    case 97:
                        c = 'a';
                        break;
                    case 98:
                        c = 'b';
                        break;
                    case 99:
                        c = 'c';
                        break;
                    case 100:
                        c = 'd';
                        break;
                    case 101:
                        c = 'e';
                        break;
                    case 102:
                        c = 'f';
                        break;
                    case 103:
                        c = 'g';
                        break;
                    case 104:
                        c = 'h';
                        break;
                    case 105:
                        c = 'i';
                        break;
                    case 106:
                        c = 'j';
                        break;
                    case 107:
                        c = 'k';
                        break;
                    case 108:
                        c = 'l';
                        break;
                    case 109:
                        c = 'm';
                        break;
                    case 110:
                        c = 'n';
                        break;
                    case 111:
                        c = 'o';
                        break;
                    case 112:
                        c = 'p';
                        break;
                    case 113:
                        c = 'q';
                        break;
                    case 114:
                        c = 'r';
                        break;
                    case 115:
                        c = 's';
                        break;
                    case 116:
                        c = 't';
                        break;
                    case 117:
                        c = 'u';
                        break;
                    case 118:
                        c = 'v';
                        break;
                    case 119:
                        c = 'w';
                        break;
                    case 120:
                        c = 'x';
                        break;
                    case 121:
                        c = 'y';
                        break;
                    case 122:
                        c = 'z';
                        break;
                    case 123:
                        c = '{';
                        break;
                    case 124:
                        c = '|';
                        break;
                    case 125:
                        c = '}';
                        break;
                    case 126:
                        c = '~';
                        break;
                    case 127:
                        isUnknown = true;
                        break;
                    case 170:
                        isUnknown = true;
                        break;
                    case 173:
                        isUnknown = true;
                        break;
                    case 187:
                        isUnknown = true;
                        break;
                    case 195:
                        isUnknown = true;
                        break;
                    case 235:
                        c = 'ë';
                        break;
                    case 251:
                        isUnknown = true;
                        break;
                    case 252:
                        isUnknown = true;
                        break;
                    case 255:
                        isUnknown = true;
                        break;
                    default:
                       throw new InvalidOperationException();
                }
                string sNewString = isUnknown ? "<" + nextByte + ">" : c.ToString();
                textBuilder.Append(sNewString);
            }
        }

        private void SetButtonTags()
        {
            btnLevel1OffensiveSpell.Tag = new CommandButtonTag(btnLevel1OffensiveSpell, null, CommandType.Magic, DependentObjectType.Mob);
            btnLevel2OffensiveSpell.Tag = new CommandButtonTag(btnLevel2OffensiveSpell, null, CommandType.Magic, DependentObjectType.Mob);
            btnLevel3OffensiveSpell.Tag = new CommandButtonTag(btnLevel3OffensiveSpell, null, CommandType.Magic, DependentObjectType.Mob);
            btnLookAtMob.Tag = new CommandButtonTag(btnLookAtMob, null, CommandType.None, DependentObjectType.Mob);
            btnCastVigor.Tag = new CommandButtonTag(btnCastVigor, null, CommandType.Magic, DependentObjectType.None);
            btnCastCurePoison.Tag = new CommandButtonTag(btnCastCurePoison, null, CommandType.Magic, DependentObjectType.None);
            btnAttackMob.Tag = new CommandButtonTag(btnAttackMob, null, CommandType.Melee, DependentObjectType.Mob);
            btnDrinkVigor.Tag = new CommandButtonTag(btnDrinkVigor, null, CommandType.Potions, DependentObjectType.None);
            btnDrinkCurepoison.Tag = new CommandButtonTag(btnDrinkCurepoison, null, CommandType.Potions, DependentObjectType.None);
            btnDrinkMend.Tag = new CommandButtonTag(btnDrinkMend, null, CommandType.Potions, DependentObjectType.None);
            btnUseWandOnMob.Tag = new CommandButtonTag(btnUseWandOnMob, null, CommandType.Magic, DependentObjectType.Wand | DependentObjectType.Mob);
            btnPowerAttackMob.Tag = new CommandButtonTag(btnPowerAttackMob, null, CommandType.Melee, DependentObjectType.Mob);
            btnCastMend.Tag = new CommandButtonTag(btnCastMend, null, CommandType.Magic, DependentObjectType.None);
            btnStunMob.Tag = new CommandButtonTag(btnStunMob, null, CommandType.Magic, DependentObjectType.Mob);
            tsbTime.Tag = new CommandButtonTag(tsbTime, "time", CommandType.None, DependentObjectType.None);
            tsbInformation.Tag = new CommandButtonTag(tsbInformation, "information", CommandType.None, DependentObjectType.None);
            tsbInventoryAndEquipment.Tag = new CommandButtonTag(tsbInventoryAndEquipment, null, CommandType.None, DependentObjectType.None);
            tsbRemoveAll.Tag = new CommandButtonTag(tsbRemoveAll, "remove all", CommandType.None, DependentObjectType.None);
            tsbWearAll.Tag = new CommandButtonTag(tsbWearAll, "wear all", CommandType.None, DependentObjectType.None);
            tsbWho.Tag = new CommandButtonTag(tsbWho, "who", CommandType.None, DependentObjectType.None);
            tsbUptime.Tag = new CommandButtonTag(tsbUptime, "uptime", CommandType.None, DependentObjectType.None);
            tsbSpells.Tag = new CommandButtonTag(tsbSpells, "spells", CommandType.None, DependentObjectType.None);
        }

        private void _bwBackgroundProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp.SingleCommandType.HasValue && bwp.SingleCommandType.Value == BackgroundCommandType.Quit && bwp.SingleCommandResultObject != null && bwp.SingleCommandResultObject.Result == CommandResult.CommandSuccessful)
            {
                if (bwp.SaveSettingsOnQuit)
                {
                    SaveSettings();
                }
                this.Logout = bwp.LogoutOnQuit;
                this.Close();
            }
            else
            {
                PermRun pr = bwp.PermRun;
                if (bwp.Success  && pr != null)
                {
                    CompletePermRun(pr);
                }
                _commandResult = null;
                _commandSpecificResult = 0;
                _commandInventoryItem = null;
                _lastCommand = null;
                _lastCommandDamage = 0;
                _runningHiddenCommand = false;
                _backgroundProcessPhase = BackgroundProcessPhase.None;
                bool setMobToFirstAvailable = true;
                if (bwp.AtDestination && (!string.IsNullOrEmpty(bwp.MobText) || bwp.MobType.HasValue))
                {
                    if (bwp.MonsterKilled)
                    {
                        if (bwp.MonsterKilledType.HasValue) //choose first monster of the same type
                        {
                            MobTypeEnum eMobValue = bwp.MonsterKilledType.Value;
                            List<MobTypeEnum> currentRoomMobs = _currentEntityInfo.CurrentRoomMobs;
                            string sFound = null;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                if (currentRoomMobs.IndexOf(eMobValue) >= 0) sFound = eMobValue.ToString();
                            }
                            if (!string.IsNullOrEmpty(sFound))
                            {
                                txtMob.Text = sFound;
                                setMobToFirstAvailable = false;
                            }
                        }
                    }
                    else //presumably the mob is still there so leave it selected
                    {
                        string sText;
                        if (bwp.MobType.HasValue)
                        {
                            sText = bwp.MobType.ToString();
                            if (bwp.MobTypeCounter > 1) sText += " " + bwp.MobTypeCounter;
                        }
                        else if (!string.IsNullOrEmpty(bwp.MobText))
                        {
                            sText = bwp.MobText;
                            if (bwp.MobTextCounter > 1) sText += " " + bwp.MobTextCounter;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        txtMob.Text = sText;
                        setMobToFirstAvailable = false;
                    }
                }
                if (setMobToFirstAvailable)
                {
                    string sText = null;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        MobTypeEnum? firstAttackableMob = _currentEntityInfo.GetFirstAttackableMob();
                        if (firstAttackableMob.HasValue)
                        {
                            sText = _currentEntityInfo.PickMobTextFromMobCounter(null, MobLocationType.CurrentRoomMobs, firstAttackableMob.Value, 1, false, true);
                        }
                    }
                    if (!string.IsNullOrEmpty(sText))
                    {
                        txtMob.Text = sText;
                    }
                }

                if (setMobToFirstAvailable && _currentEntityInfo.tnObviousMobs.Nodes.Count > 0)
                {
                    treeCurrentRoom.SelectedNode = _currentEntityInfo.tnObviousMobs.Nodes[0];
                }

                PermRun nextPR = _nextPermRun;
                if (nextPR == null)
                {
                    ToggleBackgroundProcessUI(bwp, false);
                    _currentBackgroundParameters = null;
                    RefreshEnabledForSingleMoveButtons();
                }
                else
                {
                    _nextPermRun = null;
                    _currentPermRun = nextPR;
                    DoPermRun(nextPR, false);
                }
            }
        }

        private void CompletePermRun(PermRun pr)
        {
            _currentPermRun = null;
            if (pr.PermRunStart != DateTime.MinValue && pr.Flow != PermRunFlow.AdHocCombat && pr.Flow != PermRunFlow.AdHocNonCombat)
            {
                TimeSpan permRunTime = DateTime.UtcNow - pr.PermRunStart;
                int goldDiff = _gold - pr.BeforeGold;
                int xpDiff = _experience - pr.BeforeExperience;
                double seconds = permRunTime.TotalSeconds;
                List<string> messages = new List<string>();
                bool gainedGold = goldDiff > 0;
                bool gainedXP = xpDiff > 0;
                if (gainedGold || gainedXP)
                {
                    messages.Add("Perm run complete in " + seconds.ToString("N1") + " seconds.");
                    if (gainedGold)
                    {
                        messages.Add("Gold: " + goldDiff + " (" + (goldDiff / seconds * 60).ToString("N1") + " per minute)");
                    }
                    if (gainedXP)
                    {
                        messages.Add("XP: " + xpDiff + " (" + (xpDiff / seconds * 60).ToString("N1") + " per minute)");
                    }
                }
                pr.SourcePermRun.LastCompleted = DateTime.UtcNow;
                AddConsoleMessages(messages);
            }
        }

        private void _bwBackgroundProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            try
            {
                if (pms.SingleCommandType.HasValue)
                {
                    BackgroundCommandType cmdType = pms.SingleCommandType.Value;
                    CommandResultObject commandResultObj;
                    if (cmdType == BackgroundCommandType.Look)
                    {
                        commandResultObj = RunSingleCommandForCommandResult(pms.SingleCommandType.Value, "look", pms, AbortIfFleeingOrHazying, false);
                    }
                    else
                    {
                        if (cmdType == BackgroundCommandType.Search)
                        {
                            commandResultObj = RunSingleCommand(BackgroundCommandType.Search, "search", pms, AbortIfFleeingOrHazying, false);
                        }
                        else if (cmdType == BackgroundCommandType.Hide)
                        {
                            commandResultObj = RunSingleCommand(BackgroundCommandType.Hide, "hide", pms, AbortIfFleeingOrHazying, false);
                        }
                        else if (cmdType == BackgroundCommandType.Quit)
                        {
                            commandResultObj = RunSingleCommand(BackgroundCommandType.Quit, "quit", pms, AbortIfFleeingOrHazying, false);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    if (commandResultObj.Result != CommandResult.CommandEscaped)
                    {
                        pms.SingleCommandResultObject = commandResultObj;
                        return;
                    }
                }

                CommandResultObject backgroundCommandResultObject = null;
                PermRun pr = pms.PermRun;
                bool hasMob = pms.HasTargetMob();
                PromptedSkills skillsToRun = pr == null ? PromptedSkills.None : pr.SkillsToRun;
                WorkflowSpells spellsToCast = pr == null ? WorkflowSpells.None : pr.SpellsToCast;

                //rehome inventory sink room contents
                if (!_hazying && !_fleeing && pr != null && pr.Rehome && pms.CurrentArea != null && pms.NewArea != null && pms.CurrentArea != pms.NewArea && pms.CommonParentArea != null && pms.CommonParentArea.InventorySinkRoomObject != null && pms.CurrentArea.InventorySinkRoomObject != null && pms.CurrentArea.InventorySinkRoomObject != pms.CommonParentArea.InventorySinkRoomObject)
                {
                    backgroundCommandResultObject = NavigateToSpecificRoom(pms.CurrentArea.InventorySinkRoomObject, pms, true, pr);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                    {
                        return;
                    }
                    if (!_hazying && !_fleeing)
                    {
                        backgroundCommandResultObject = DoInventoryManagement(pms, ItemsToProcessType.ProcessAllItemsInRoom, true, pms.CurrentArea.InventorySinkRoomObject, pms.CommonParentArea.InventorySinkRoomObject, true, pr, null);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                        {
                            return;
                        }
                    }
                }

                if (!_hazying && !_fleeing && ((pr != null && pr.BeforeFull != FullType.None) || pms.CureIfPoisoned))
                {
                    _backgroundProcessPhase = BackgroundProcessPhase.Heal;
                    PlayerStatusFlags psf = _playerStatusFlags;
                    if (pms.CureIfPoisoned && ((psf & PlayerStatusFlags.Poisoned) == PlayerStatusFlags.None)) //validate not poisoned if the user initiated the cure poisoned
                    {
                        backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, AbortIfFleeingOrHazying, true);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return;
                        psf = _playerStatusFlags;
                        if ((psf & PlayerStatusFlags.Poisoned) == PlayerStatusFlags.None)
                        {
                            AddConsoleMessage("Not poisoned, thus cure-poison not cast.");
                            return;
                        }
                    }
                    if (!_fleeing && !_hazying && ((psf & PlayerStatusFlags.Poisoned) != PlayerStatusFlags.None))
                    {
                        SpellInformationAttribute sia = SpellsStatic.SpellsByEnum[SpellsEnum.curepoison];
                        if (_automp < sia.Mana)
                        {
                            return;
                        }
                        else
                        {
                            backgroundCommandResultObject = CastSpellOnSelf(SpellsEnum.curepoison, pms, AbortIfFleeingOrHazying);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                            {
                                return;
                            }
                        }
                    }
                    if (pms.CureIfPoisoned) return; //for standalone cure-poison that's all we need to do

                    if (!_fleeing && !_hazying && !IsFull(pr.BeforeFull, spellsToCast, out _))
                    {
                        backgroundCommandResultObject = NavigateToTickRoom(pms, true, pr);
                        if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                        {
                            return;
                        }
                        if (!_hazying && !_fleeing)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Heal;
                            backgroundCommandResultObject = GetFullInBackground(pms, pr.BeforeFull, spellsToCast, AbortIfFleeingOrHazying);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                            {
                                return;
                            }
                        }
                    }
                }
                if (hasMob && !_fleeing && !_hazying && pr != null && !pms.Resume)
                {
                    pr.PermRunStart = DateTime.UtcNow;
                }

                bool haveThreshold = pr != null && pr.ThresholdRoomObject != null;
                if (hasMob || pr == null)
                {
                    if (!_fleeing && !_hazying)
                    {
                        bool moved = true;
                        if (haveThreshold)
                            backgroundCommandResultObject = NavigateToSpecificRoom(pr.ThresholdRoomObject, pms, true, pr);
                        else if (pms.TargetRoom != null)
                            backgroundCommandResultObject = NavigateToSpecificRoom(pms.TargetRoom, pms, true, pr);
                        else if (pms.Exits != null && pms.Exits.Count > 0)
                            backgroundCommandResultObject = TraverseExitsAlreadyInBackground(pms.Exits, pms, true, pr);
                        else
                            moved = false;
                        if (moved)
                        {
                            if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                            {
                                pms.AtDestination = !haveThreshold;
                            }
                            else if (backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                            {
                                return;
                            }
                        }
                    }
                }

                bool failedToEquip = false;

                Strategy effectiveStrategy = pms.Strategy;
                GetInformationFromEffectiveStrategy(effectiveStrategy, out bool useManaPool, out AfterKillMonsterAction onMonsterKilledAction, out int usedAutoSpellMin, out int usedAutoSpellMax, out RealmTypeFlags availableRealms, out bool haveMagicStrategySteps, out bool haveMeleeStrategySteps, out bool haveMagicStunWandStep, out bool havePotionsStrategySteps);
                List<SelectedInventoryOrEquipmentItem> stunWands = null;

                if (hasMob)
                {

                    //activate potions/skills at the threshold if there is a threshold
                    if (!_fleeing && !_hazying && haveThreshold)
                    {
                        backgroundCommandResultObject = PerformPostTickPreCombatActions(pms, skillsToRun, pr, ref failedToEquip, haveMeleeStrategySteps, haveMagicStunWandStep, out stunWands);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                        {
                            return;
                        }
                    }
                }

                if ((hasMob || pr == null) && !_fleeing && !_hazying && haveThreshold)
                {
                    backgroundCommandResultObject = NavigateToSpecificRoom(pms.TargetRoom, pms, true, pr);
                    if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                    {
                        pms.AtDestination = true;
                    }
                    else if (backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                    {
                        return;
                    }
                }

                if (hasMob)
                {
                    //verify the mob is present and attackable before activating skills
                    if (_bwBackgroundProcess.CancellationPending) return;
                    if (!_hazying && !_fleeing && hasMob)
                    {
                        if (FoundMob(pms))
                        {
                            if (pms.MobType.HasValue && _settingsData.DynamicMobData.TryGetValue(pms.MobType.Value, out DynamicMobData dmd))
                            {
                                if (dmd.HasData() && (pms.Strategy != null || dmd.Strategy != null))
                                {
                                    effectiveStrategy = new Strategy(pms.Strategy, dmd);
                                    GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                }
                            }
                        }
                        else
                        {
                            AddConsoleMessage("Target mob not present.");
                            return;
                        }
                    }

                    if (!_hazying && !_fleeing && pms.InventoryItems != null) //trading
                    {
                        string sMobTarget = GetMobTargetFromMobType(pms.MobType.Value, pms.MobTypeCounter, false);
                        if (string.IsNullOrEmpty(sMobTarget))
                        {
                            AddConsoleMessage("Unable to construct trade mob target for " + pms.MobType.Value);
                            return;
                        }
                        for (int i = pms.InventoryItems.Count - 1; i >= 0; i--)
                        {
                            SelectedInventoryOrEquipmentItem sioei = pms.InventoryItems[i];
                            ItemTypeEnum eItemType = sioei.ItemType.Value;
                            string sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, eItemType, sioei.Counter, false, false);
                            if (string.IsNullOrEmpty(sItemText))
                            {
                                AddConsoleMessage("Unable to construct trade command for " + eItemType);
                                return;
                            }
                            _commandInventoryItem = sioei;
                            backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.Trade, $"trade {sItemText} {sMobTarget}", pms, AbortIfFleeingOrHazying, false);
                            if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                            {
                                break;
                            }
                            else if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                            {
                                return;
                            }
                        }
                        //remove the mob from the background process so combat will not occur on the mob.
                        pms.MobText = string.Empty;
                        pms.MobTextCounter = 0;
                        pms.MobType = null;
                        pms.MobTypeCounter = 0;
                    }

                    //activate potions/skills at the target if there is no threshold
                    if (!_fleeing && !_hazying && !haveThreshold)
                    {
                        backgroundCommandResultObject = PerformPostTickPreCombatActions(pms, skillsToRun, pr, ref failedToEquip, haveMeleeStrategySteps, haveMagicStunWandStep, out stunWands);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful && backgroundCommandResultObject.Result != CommandResult.CommandEscaped)
                        {
                            return;
                        }
                    }
                }

                string sMobText;
                if (hasMob)
                {
                    if (pms.MobType.HasValue)
                    {
                        sMobText = pms.MobType.ToString();
                        if (pms.MobTypeCounter > 1) sMobText += " " + pms.MobTypeCounter;
                    }
                    else
                    {
                        sMobText = pms.MobText;
                        if (pms.MobTextCounter > 1) sMobText += " " + pms.MobTextCounter;
                    }
                }
                else
                {
                    sMobText = string.Empty;
                }
                bool hasInitialQueuedMagicStep;
                bool hasInitialQueuedPotionsStep;
                lock (_queuedCommandLock)
                {
                    hasInitialQueuedMagicStep = pms.QueuedMagicStep.HasValue;
                    hasInitialQueuedPotionsStep = pms.QueuedPotionsStep.HasValue;
                }
                if (_hazying || _fleeing || effectiveStrategy != null || hasInitialQueuedMagicStep)
                {
                    try
                    {
                        _mob = sMobText;
                        _currentlyFightingMobText = pms.MobText;
                        _currentlyFightingMobType = pms.MobType;
                        _currentlyFightingMobCounter = pms.MobType.HasValue ? pms.MobTypeCounter : pms.MobTextCounter;
                        _monsterDamage = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunnedSince = null;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        _monsterKilledItems.Clear();
                        ItemTypeEnum? weaponItem = _settingsData.Weapon;
                        ItemTypeEnum? heldItem = _settingsData.HeldItem;
                        int? stunWandIndex = stunWands == null ? (int?)null : 0;
                        SelectedInventoryOrEquipmentItem currentWand = stunWands?[0];
BeginCombat:
                        if (useManaPool) _currentMana = effectiveStrategy.ManaPool;
                        if (haveMagicStrategySteps || haveMeleeStrategySteps || havePotionsStrategySteps || hasInitialQueuedMagicStep || hasInitialQueuedPotionsStep)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Combat;
                            bool doPowerAttack = false;
                            doPowerAttack = (skillsToRun & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
                            IEnumerator<MagicStrategyStep> magicSteps = effectiveStrategy?.GetMagicSteps().GetEnumerator();
                            IEnumerator<MeleeStrategyStep> meleeSteps = effectiveStrategy?.GetMeleeSteps(doPowerAttack).GetEnumerator();
                            IEnumerator<PotionsStrategyStep> potionsSteps = effectiveStrategy?.GetPotionsSteps().GetEnumerator();
                            MagicStrategyStep? nextMagicStep = null;
                            MeleeStrategyStep? nextMeleeStep = null;
                            PotionsStrategyStep? nextPotionsStep = null;
                            bool magicStepsEnabled = effectiveStrategy != null && ((effectiveStrategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None);
                            bool meleeStepsEnabled = effectiveStrategy != null && ((effectiveStrategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None);
                            bool potionsStepsEnabled = effectiveStrategy != null && ((effectiveStrategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None);
                            bool magicStepsFinished = magicSteps == null || !magicSteps.MoveNext();
                            bool meleeStepsFinished = meleeSteps == null || !meleeSteps.MoveNext();
                            bool potionsStepsFinished = potionsSteps == null || !potionsSteps.MoveNext();
                            int? magicOnlyWhenStunnedAfterXMS = effectiveStrategy?.MagicOnlyWhenStunnedForXMS;
                            int? meleeOnlyWhenStunnedAfterXMS = effectiveStrategy?.MeleeOnlyWhenStunnedForXMS;
                            int? potionsOnlyWhenStunnedAfterXMS = effectiveStrategy?.PotionsOnlyWhenStunnedForXMS;
                            if (!magicStepsFinished) nextMagicStep = magicSteps.Current;
                            if (!meleeStepsFinished) nextMeleeStep = meleeSteps.Current;
                            if (!potionsStepsFinished) nextPotionsStep = potionsSteps.Current;
                            DateTime? dtNextMagicCommand = null;
                            DateTime? dtNextMeleeCommand = null;
                            DateTime? dtNextPotionsCommand = null;
                            DateTime dtUtcNow;
                            bool allowBasedOnStun;
                            string command;
                            Strategy resetStrategy;
                            while (true) //combat cycle
                            {
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                bool didDamage = false;

                                dtUtcNow = DateTime.UtcNow;
                                if (magicOnlyWhenStunnedAfterXMS.HasValue)
                                {
                                    DateTime? stunnedSince = _monsterStunnedSince;
                                    if (stunnedSince.HasValue)
                                    {
                                        allowBasedOnStun = (dtUtcNow - stunnedSince.Value).TotalMilliseconds >= magicOnlyWhenStunnedAfterXMS.Value;
                                    }
                                    else //monster isn't stunned, can't use
                                    {
                                        allowBasedOnStun = false;
                                    }
                                }
                                else //no stun restriction
                                {
                                    allowBasedOnStun = true;
                                }

                                if (nextMagicStep.HasValue && allowBasedOnStun &&
                                    (!dtNextMagicCommand.HasValue || dtUtcNow > dtNextMagicCommand.Value))
                                {
                                    string sMobTarget = string.Empty;
                                    if (Strategy.GetMagicStrategyStepIsCombat(nextMagicStep.Value))
                                    {
                                        sMobTarget = GetMobTarget(false);
                                        if (string.IsNullOrEmpty(sMobTarget))
                                        {
                                            AddConsoleMessage("Target mob not found.");
                                            return;
                                        }
                                    }

                                    int currentMana = useManaPool ? _currentMana : _automp;
                                    int currentHP = _autohp;
                                    int manaDrain;
                                    BackgroundCommandType? bct;
                                    RealmTypeFlags? realmToUse;
                                    MagicCommandChoiceResult result = GetMagicCommand(nextMagicStep.Value, currentHP, _totalhp, currentMana, out manaDrain, out bct, out command, usedAutoSpellMin, usedAutoSpellMax, sMobTarget, _settingsData, _currentEntityInfo, _currentRealm, out realmToUse, availableRealms, currentWand);
                                    if (result == MagicCommandChoiceResult.Skip)
                                    {
                                        if (!magicStepsFinished)
                                        {
                                            if (magicSteps.MoveNext())
                                            {
                                                nextMagicStep = magicSteps.Current;
                                                dtNextMagicCommand = null;
                                            }
                                            else //no more steps
                                            {
                                                nextMagicStep = null;
                                                magicStepsFinished = true;
                                            }
                                        }
                                    }
                                    else if (result == MagicCommandChoiceResult.OutOfMana)
                                    {
                                        nextMagicStep = null;
                                        magicStepsFinished = true;
                                    }
                                    else if (result == MagicCommandChoiceResult.Cast)
                                    {
CastNextOffensiveSpell:
                                        BackgroundCommandType bctValue = bct.Value;
                                        backgroundCommandResultObject = RunBackgroundMagicStep(bctValue, command, pms, useManaPool, manaDrain, magicSteps, ref magicStepsFinished, ref nextMagicStep, ref dtNextMagicCommand, ref didDamage);
                                        if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                        {
                                            break; //break out of combat
                                        }
                                        else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                        {
                                            return;
                                        }
                                        else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                                        {
                                            if (realmToUse.HasValue)
                                            {
                                                _currentRealm = IsengardSettingData.GetNextRealmFromStartingPoint(realmToUse.Value, availableRealms);
                                            }
                                        }
                                        if (bctValue == BackgroundCommandType.StunWithWand && (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulThisTime))
                                        {
                                            bool retry = false;
                                            bool switchWand = false;
                                            if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulThisTime)
                                            {
                                                retry = true;
                                                switchWand = true;
                                            }
                                            else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                            {
                                                CheckIfWandOKToUse(currentWand, pms, AbortIfFleeingOrHazying, out ItemStatus itemStatus);
                                                switchWand = itemStatus == ItemStatus.Broken;
                                            }
                                            if (switchWand)
                                            {
                                                int iNewStunWandIndex = stunWandIndex.Value + 1;
                                                if (iNewStunWandIndex >= stunWands.Count) //this was the last wand
                                                {
                                                    stunWandIndex = null;
                                                    currentWand = null;
                                                }
                                                else
                                                {
                                                    stunWandIndex = iNewStunWandIndex;
                                                    currentWand = stunWands[iNewStunWandIndex];
                                                }
                                                if (retry && currentWand != null)
                                                {
                                                    command = "zap " + GetStunWandItemText(_currentEntityInfo, currentWand) + " " + sMobTarget;
                                                    goto CastNextOffensiveSpell;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);

                                //flee or stop combat once steps complete
                                if (!nextMagicStep.HasValue && magicStepsFinished && magicStepsEnabled)
                                {
                                    FinalStepAction finalAction = effectiveStrategy.FinalMagicAction;
                                    if (finalAction != FinalStepAction.None)
                                    {
                                        if (finalAction == FinalStepAction.Flee)
                                        {
                                            _fleeing = true;
                                        }
                                        else if (finalAction == FinalStepAction.Hazy)
                                        {
                                            _hazying = true;
                                        }
                                        else if (finalAction == FinalStepAction.FinishCombat)
                                        {
                                            //will quit out of combat
                                        }
                                        break;
                                    }
                                }

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                dtUtcNow = DateTime.UtcNow;
                                if (meleeOnlyWhenStunnedAfterXMS.HasValue)
                                {
                                    DateTime? stunnedSince = _monsterStunnedSince;
                                    if (stunnedSince.HasValue)
                                    {
                                        allowBasedOnStun = (dtUtcNow - stunnedSince.Value).TotalMilliseconds >= meleeOnlyWhenStunnedAfterXMS.Value;
                                    }
                                    else //monster isn't stunned, can't use
                                    {
                                        allowBasedOnStun = false;
                                    }
                                }
                                else //no stun restriction
                                {
                                    allowBasedOnStun = true;
                                }

                                if (nextMeleeStep.HasValue && allowBasedOnStun &&
                                    (!dtNextMeleeCommand.HasValue || dtUtcNow > dtNextMeleeCommand.Value))
                                {
                                    string sMobTarget = GetMobTarget(false);
                                    if (string.IsNullOrEmpty(sMobTarget))
                                    {
                                        AddConsoleMessage("Target mob not found.");
                                        return;
                                    }

                                    GetMeleeCommand(nextMeleeStep.Value, out command, sMobTarget);

                                    //wield the weapon in case it was fumbled
                                    if (!failedToEquip)
                                    {
                                        backgroundCommandResultObject = EquipSingleItem(weaponItem, EquipmentSlot.Weapon1, BackgroundCommandType.WieldWeapon, pms, false, ref failedToEquip);
                                        if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                        {
                                            break;
                                        }
                                        else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                        {
                                            return;
                                        }
                                    }

                                    backgroundCommandResultObject = RunBackgroundMeleeStep(BackgroundCommandType.Attack, command, pms, meleeSteps, ref meleeStepsFinished, ref nextMeleeStep, ref dtNextMeleeCommand, ref didDamage);
                                    if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                    {
                                        break; //break out of combat
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                }

                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;

                                //flee or stop combat once steps complete
                                if (!nextMeleeStep.HasValue && meleeStepsFinished && meleeStepsEnabled)
                                {
                                    FinalStepAction finalAction = effectiveStrategy.FinalMeleeAction;
                                    if (finalAction != FinalStepAction.None)
                                    {
                                        if (finalAction == FinalStepAction.Flee)
                                        {
                                            _fleeing = true;
                                        }
                                        else if (finalAction == FinalStepAction.Hazy)
                                        {
                                            _hazying = true;
                                        }
                                        else if (finalAction == FinalStepAction.FinishCombat)
                                        {
                                            //will quit out of combat
                                        }
                                        break;
                                    }
                                }

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);
                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                dtUtcNow = DateTime.UtcNow;
                                if (potionsOnlyWhenStunnedAfterXMS.HasValue)
                                {
                                    DateTime? stunnedSince = _monsterStunnedSince;
                                    if (stunnedSince.HasValue)
                                    {
                                        allowBasedOnStun = (dtUtcNow - stunnedSince.Value).TotalMilliseconds >= potionsOnlyWhenStunnedAfterXMS.Value;
                                    }
                                    else //monster isn't stunned, can't use
                                    {
                                        allowBasedOnStun = false;
                                    }
                                }
                                else //no stun restriction
                                {
                                    allowBasedOnStun = true;
                                }

                                if (nextPotionsStep.HasValue && allowBasedOnStun &&
                                    (!dtNextPotionsCommand.HasValue || dtUtcNow > dtNextPotionsCommand.Value))
                                {
                                    PotionsCommandChoiceResult potionChoice = GetPotionsCommand(nextPotionsStep.Value, out command, _autohp, _totalhp, _settingsData);
                                    if (potionChoice == PotionsCommandChoiceResult.Fail)
                                    {
                                        nextPotionsStep = null;
                                        potionsStepsFinished = true;
                                    }
                                    else if (potionChoice == PotionsCommandChoiceResult.Skip)
                                    {
                                        if (!potionsStepsFinished)
                                        {
                                            if (potionsSteps.MoveNext())
                                            {
                                                nextPotionsStep = potionsSteps.Current;
                                                dtNextPotionsCommand = null;
                                            }
                                            else //no more steps
                                            {
                                                nextPotionsStep = null;
                                                potionsStepsFinished = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        backgroundCommandResultObject = RunBackgroundPotionsStep(BackgroundCommandType.DrinkNonHazyPotion, command, pms, potionsSteps, ref potionsStepsFinished, ref nextPotionsStep, ref dtNextPotionsCommand);
                                        if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                        {
                                            break; //break out of combat
                                        }
                                        else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                        {
                                            return;
                                        }
                                    }
                                }

                                if (SelectMobAfterKillMonster(onMonsterKilledAction, pms, out resetStrategy))
                                {
                                    if (resetStrategy != null)
                                    {
                                        effectiveStrategy = resetStrategy;
                                        GetInformationFromEffectiveStrategy(effectiveStrategy, out useManaPool, out onMonsterKilledAction, out usedAutoSpellMin, out usedAutoSpellMax, out availableRealms, out haveMagicStrategySteps, out haveMeleeStrategySteps, out haveMagicStunWandStep, out havePotionsStrategySteps);
                                        goto BeginCombat;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);

                                //flee or stop combat once steps complete
                                if (!nextPotionsStep.HasValue && potionsStepsFinished && potionsStepsEnabled)
                                {
                                    FinalStepAction finalAction = effectiveStrategy.FinalPotionsAction;
                                    if (finalAction != FinalStepAction.None)
                                    {
                                        if (finalAction == FinalStepAction.Flee)
                                        {
                                            _fleeing = true;
                                        }
                                        else if (finalAction == FinalStepAction.Hazy)
                                        {
                                            _hazying = true;
                                        }
                                        else if (finalAction == FinalStepAction.FinishCombat)
                                        {
                                            //will quit out of combat
                                        }
                                        break;
                                    }
                                }

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;

                                if (didDamage && _settingsData.QueryMonsterStatus)
                                {
                                    string sMobTextForLook = GetMobTarget(true);
                                    backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + sMobTextForLook, pms, AbortIfFleeingOrHazying, true);
                                    if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                    {
                                        break; //break out of combat
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                                    {
                                        magicStepsFinished = true;
                                        meleeStepsFinished = true;
                                        nextMagicStep = null;
                                        nextMeleeStep = null;
                                        nextPotionsStep = null;
                                        _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                                        _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                                        _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                                    }
                                    else if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }

                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);

                                //stop combat if all combat types are finished
                                if (!nextMagicStep.HasValue && !nextMeleeStep.HasValue && !nextPotionsStep.HasValue) break;

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                RunQueuedCommandWhenBackgroundProcessRunning(pms);
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;

                                Thread.Sleep(50);
                            }
                        }

                        //perform flee logic
                        if (_fleeing && (onMonsterKilledAction != AfterKillMonsterAction.StopCombat || !_monsterKilled) && !_hazying)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Flee;
                            backgroundCommandResultObject = RemoveWeaponBeforeFlee(pms);
                            if (backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                            {
                                goto BeforeHazy;
                            }
                            else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                            {
                                return;
                            }

                            //determine the flee exit if there is only one place to flee to
                            Room r = _currentEntityInfo.CurrentRoom;

                            //run the preexit logic for all target exits, since it won't be known beforehand
                            //which exit will be used.
                            bool canFlee = true;
                            List<Exit> exits = new List<Exit>(r.Exits);
                            if (r != null)
                            {
                                foreach (Exit nextExit in exits)
                                {
                                    PreOpenDoorExit(nextExit, GetExitWord(nextExit, out _), pms);
                                }
                                canFlee = !r.NoFlee;
                            }

                            if (canFlee)
                            {
                                backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Flee, "flee", pms, AbortIfHazying, false);
                                if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                {
                                    return;
                                }
                                else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                {
                                    pms.Fled = true;
                                }
                            }

                            //as a fallback try to simply walk out of the room via each exit
                            if (!pms.Fled)
                            {
                                exits.Sort((e1, e2) => 
                                {
                                    return e1.Hidden.CompareTo(e2.Hidden); //process non-hidden exits before hidden exits
                                });
                                foreach (Exit nextExit in exits)
                                {
                                    string sExitWord = GetExitWord(nextExit, out bool useGo);
                                    string nextCommand;
                                    if (useGo)
                                    {
                                        nextCommand = "go " + sExitWord;
                                    }
                                    else
                                    {
                                        nextCommand = sExitWord;
                                    }
                                    backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, AbortIfHazying, false);
                                    if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                    {
                                        _fleeing = false; //won't get a flee room transition in this case so clear the fleeing flag directly
                                        pms.Fled = true;
                                        break;
                                    }
                                }
                            }

                            //if failed to flee, we are done
                            if (!pms.Fled)
                            {
                                AddConsoleMessage("Flee attempt failed.");
                                if (!_hazying)
                                {
                                    return;
                                }
                            }
                        }

BeforeHazy:

                        //perform hazy logic
                        if (_hazying && (onMonsterKilledAction != AfterKillMonsterAction.StopCombat || !_monsterKilled))
                        {
                            if (TryDrinkHazy(pms).Result == CommandResult.CommandSuccessful)
                            {
                                pms.Hazied = true;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    finally
                    {
                        pms.MonsterKilled = _monsterKilled;
                        pms.MonsterKilledType = _monsterKilledType;
                        _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                        _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                        _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                        _currentlyFightingMobType = null;
                        _currentlyFightingMobText = null;
                        _currentlyFightingMobCounter = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunnedSince = null;
                        if (_monsterKilled)
                        {
                            pms.MonsterKilled = true;
                            pms.MonsterKilledType = _monsterKilledType;
                        }
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        _currentMana = HP_OR_MP_UNKNOWN;
                        //clear the mob out of the perm run so if resumed it won't try to attack the mob again
                        if (pms.MonsterKilled && !pms.Fled && !pms.Hazied && pr != null)
                        {
                            pr.MobType = null;
                            pr.MobText = null;
                            pr.MobIndex = 0;
                            pr.BeforeFull = FullType.None;
                            pr.Strategy = null;
                        }
                    }
                }

                if (!pms.Fled && !pms.Hazied)
                {
                    backgroundCommandResultObject = DoInventoryManagement(pms, pms.InventoryProcessInputType, false, pms.TargetRoom, pms.InventorySinkRoom, false, pr, pms.SelectedItemsWithTargets);
                    if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                    {
                        return;
                    }
                    if (!_hazying && pr != null && !IsFull(pr.AfterFull, spellsToCast, out _))
                    {
                        backgroundCommandResultObject = NavigateToTickRoom(pms, false, pr);
                        if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                        {
                            return;
                        }
                        if (!_hazying)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Heal;
                            backgroundCommandResultObject = GetFullInBackground(pms, pr.AfterFull, spellsToCast, AbortIfHazying);
                            if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                            {
                                return;
                            }
                        }
                    }

                    if (_hazying)
                    {
                        if (TryDrinkHazy(pms).Result == CommandResult.CommandSuccessful)
                        {
                            pms.Hazied = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    bool userInitiatedScore = pms.DoScore;
                    bool runScore = userInitiatedScore;
                    if (!runScore)
                    {
                        lock (_currentEntityInfo.EntityLock)
                        {
                            foreach (SkillCooldown next in _currentEntityInfo.SkillsCooldowns)
                            {
                                if (next.Status == SkillCooldownStatus.Inactive)
                                {
                                    runScore = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (runScore)
                    {
                        _backgroundProcessPhase = BackgroundProcessPhase.Score;
                        backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null, !userInitiatedScore);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                        {
                            return;
                        }
                    }
                }

                if (!pms.Success) //determine success
                {
                    pms.Success = !pms.Fled && !pms.Hazied && (!hasMob || pms.MonsterKilled);
                }
            }
            catch (Exception ex)
            {
                if (pms.Flee)
                {
                    _fleeing = false;
                }
                if (pms.Hazy)
                {
                    _fleeing = false;
                    _hazying = false;
                }
                lock (_consoleTextLock)
                {
                    _newConsoleText.Add(ex.ToString());
                }
            }
        }



        /// <summary>
        /// runs logic that runs after fulling is finished but before combatstarts
        /// </summary>
        /// <param name="pms">background worker parameters</param>
        /// <param name="skillsToRun">skills to run</param>
        /// <param name="pr">perm run</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject PerformPostTickPreCombatActions(BackgroundWorkerParameters pms, PromptedSkills skillsToRun, PermRun pr, ref bool failedToEquip, bool useMelee, bool useStunWand, out List<SelectedInventoryOrEquipmentItem> stunWands)
        {
            stunWands = null;
            _backgroundProcessPhase = BackgroundProcessPhase.PostHealPreCombatLogic;
            CommandResultObject backgroundCommandResultObject;
            bool removeAllEquipment = pr != null && pr.RemoveAllEquipment;
            if (!_hazying && !_fleeing && removeAllEquipment)
            {
                backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, null, "all", pms, AbortIfFleeingOrHazying);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
                failedToEquip = true;
            }
            if (!_hazying && !_fleeing) //remove wand in case it was held (e.g. for recharging). This also clears up the slot for the usual held item.
            {
                RemoveHeldItem(pms, AbortIfFleeingOrHazying, (ie) => 
                {
                    StaticItemData sid = ItemEntity.StaticItemData[ie.ItemType.Value];
                    return sid.ItemClass == ItemClass.Wand;
                });
            }
            if (!_hazying && !_fleeing && useMelee && !removeAllEquipment)
            {
                ItemTypeEnum? weaponItem = _settingsData.Weapon;
                ItemTypeEnum? heldItem = _settingsData.HeldItem;
                backgroundCommandResultObject = EquipSingleItem(weaponItem, EquipmentSlot.Weapon1, BackgroundCommandType.WieldWeapon, pms, true, ref failedToEquip);
                if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                {
                    AddConsoleMessage("Failed to equip " + weaponItem.Value);
                    return backgroundCommandResultObject;
                }
                else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                {
                    return backgroundCommandResultObject;
                }
                backgroundCommandResultObject = EquipSingleItem(heldItem, EquipmentSlot.Held, BackgroundCommandType.HoldItem, pms, true, ref failedToEquip);
                if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                {
                    AddConsoleMessage("Failed to hold " + weaponItem.Value);
                    return backgroundCommandResultObject;
                }
                else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout)
                {
                    return backgroundCommandResultObject;
                }
            }
            if (!_hazying && !_fleeing && useStunWand) //determine available stun wands
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    stunWands = _currentEntityInfo.GetInvEqItems((ie) => 
                    {
                        StaticItemData sid = ItemEntity.StaticItemData[ie.ItemType.Value];
                        return sid.ItemClass == ItemClass.Wand && sid.Spell == SpellsEnum.stun;
                    }, true, false);
                }
                for (int i = stunWands.Count - 1; i >= 0; i--)
                {
                    backgroundCommandResultObject = CheckIfWandOKToUse(stunWands[i], pms, AbortIfFleeingOrHazying, out ItemStatus itemStatus);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResultObject;
                    }
                    if (itemStatus == ItemStatus.Broken)
                    {
                        stunWands.RemoveAt(i);
                    }
                }
                if (stunWands.Count == 0) //no stun wands to use
                {
                    AddConsoleMessage("No available stun wands found.");
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
            }
            WorkflowSpells spellsToPot = pr == null ? WorkflowSpells.None : pr.SpellsToPotion;
            spellsToPot &= ~WorkflowSpells.CurePoison;
            if (spellsToPot != WorkflowSpells.None)
            {
                List<SpellsEnum> spellsCast = new List<SpellsEnum>();
                lock (_currentEntityInfo.EntityLock) //validate spells are either active or have a potion for them
                {
                    spellsCast.AddRange(_spellsCast);
                    foreach (WorkflowSpells nextPotSpell in Enum.GetValues(typeof(WorkflowSpells)))
                    {
                        if (nextPotSpell != WorkflowSpells.None && ((nextPotSpell & spellsToPot) != WorkflowSpells.None))
                        {
                            SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextPotSpell];
                            if (spellsCast.Contains(sia.SpellType))
                            {
                                spellsToPot &= ~nextPotSpell;
                            }
                            else
                            {
                                if (!_currentEntityInfo.HasPotionForSpell(sia.SpellType, out _, out _)) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                            }
                        }
                    }
                }
                foreach (WorkflowSpells nextPotSpell in Enum.GetValues(typeof(WorkflowSpells)))
                {
                    if (_hazying || _fleeing) return new CommandResultObject(CommandResult.CommandEscaped);
                    if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);
                    if (nextPotSpell != WorkflowSpells.None && ((nextPotSpell & spellsToPot) != WorkflowSpells.None))
                    {
                        backgroundCommandResultObject = DrinkPotionForSpell(SpellsStatic.WorkflowSpellsByEnum[nextPotSpell], pms, AbortIfFleeingOrHazying);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResultObject;
                        }
                    }
                }
            }
            if (!_hazying && !_fleeing && ((skillsToRun & PromptedSkills.Manashield) == PromptedSkills.Manashield))
            {
                backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Manashield, "manashield", pms, AbortIfFleeingOrHazying, false);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
            }
            if (!_hazying && !_fleeing && ((skillsToRun & PromptedSkills.Fireshield) == PromptedSkills.Fireshield))
            {
                backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Fireshield, "fireshield", pms, AbortIfFleeingOrHazying, false);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
            }
            if (_hazying || _fleeing) backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandEscaped);
            else if (_bwBackgroundProcess.CancellationPending) backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandAborted);
            else backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandSuccessful);
            return backgroundCommandResultObject;
        }

        private void GetInformationFromEffectiveStrategy(Strategy strategy, out bool useManaPool, out AfterKillMonsterAction onMonsterKilledAction, out int usedAutoSpellMin, out int usedAutoSpellMax, out RealmTypeFlags availableRealms, out bool haveMagicStrategySteps, out bool haveMeleeStrategySteps, out bool haveMagicStunWandStep, out bool havePotionsStrategySteps)
        {
            useManaPool = false;
            onMonsterKilledAction = AfterKillMonsterAction.StopCombat;
            usedAutoSpellMin = _settingsData.AutoSpellLevelMin;
            usedAutoSpellMax = _settingsData.AutoSpellLevelMax;
            availableRealms = _settingsData.Realms;
            haveMeleeStrategySteps = false;
            haveMagicStrategySteps = false;
            haveMagicStunWandStep = false;
            havePotionsStrategySteps = false;
            if (strategy != null)
            {
                useManaPool = strategy.ManaPool > 0;
                haveMagicStrategySteps = strategy.HasAnyMagicSteps(null);
                haveMagicStunWandStep = strategy.HasAnyMagicSteps(MagicStrategyStep.StunWand);
                haveMeleeStrategySteps = strategy.HasAnyMeleeSteps(null);
                havePotionsStrategySteps = strategy.HasAnyPotionsSteps(null);
                onMonsterKilledAction = strategy.AfterKillMonsterAction.GetValueOrDefault(AfterKillMonsterAction.StopCombat);
                if (strategy.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && strategy.AutoSpellLevelMax != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                {
                    usedAutoSpellMin = strategy.AutoSpellLevelMin;
                    usedAutoSpellMax = strategy.AutoSpellLevelMax;
                }
                if (strategy.Realms.HasValue) availableRealms = strategy.Realms.Value;
            }
        }

        private CommandResultObject CheckIfWandOKToUse(SelectedInventoryOrEquipmentItem sioei, BackgroundWorkerParameters pms, Func<bool> abortLogic, out ItemStatus itemStatus)
        {
            itemStatus = ItemStatus.Broken;
            string sItemText;
            lock (_currentEntityInfo.EntityLock)
            {
                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, sioei.ItemType.Value, sioei.Counter, false, false);
            }
            if (string.IsNullOrEmpty(sItemText))
            {
                return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            }
            _commandInventoryItem = sioei;
            CommandResultObject ret = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtItem, "look " + sItemText, pms, abortLogic, false);
            if (ret.Result == CommandResult.CommandSuccessful)
            {
                itemStatus = (ItemStatus)ret.ResultCode;
            }
            return ret;
        }

        private CommandResultObject TryDrinkHazy(BackgroundWorkerParameters pms)
        {
            _backgroundProcessPhase = BackgroundProcessPhase.Hazy;
            return RunSingleCommand(BackgroundCommandType.DrinkHazy, "drink hazy", pms, null, false);
        }

        /// <summary>
        /// runs inventory management logic
        /// </summary>
        /// <param name="pms">background worker parameters</param>
        /// <param name="eInvProcessInputs">what items to process</param>
        /// <param name="isFerry">whether to ferry or manage inventory items</param>
        /// <param name="inventorySourceRoom">source room. it is assumed the player is present in this room.</param>
        /// <param name="inventorySinkRoom">target room for a ferry or inventory sink room for item management</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject DoInventoryManagement(BackgroundWorkerParameters pms, ItemsToProcessType eInvProcessInputs, bool isFerry, Room inventorySourceRoom, Room inventorySinkRoom, bool beforeTargetRoom, PermRun pr, List<SelectedItemWithTarget> SelectedItemsWithTargets)
        {
            if (pr != null && pr.InventoryManagementFinished)
            {
                return new CommandResultObject(CommandResult.CommandSuccessful);
            }
            CommandResultObject backgroundCommandResultObject;
            List<string> failedForRoomMessages = new List<string>();
            bool success;
            if (eInvProcessInputs == ItemsToProcessType.ProcessAllItemsInRoom || (pms.MonsterKilled && eInvProcessInputs == ItemsToProcessType.ProcessMonsterDrops))
            {
                NavigateToSpecificRoom(inventorySourceRoom, pms, false, pr);
                _backgroundProcessPhase = BackgroundProcessPhase.InventoryManagement;
                if (SelectedItemsWithTargets == null)
                {
                    List<SelectedInventoryOrEquipmentItem> sioeis = new List<SelectedInventoryOrEquipmentItem>();
                    lock (_currentEntityInfo.EntityLock)
                    {
                        string sPreviousValue = null;
                        ItemTypeEnum? ePreviousValue = null;
                        int iCounter = 0;
                        if (eInvProcessInputs == ItemsToProcessType.ProcessAllItemsInRoom)
                        {
                            foreach (ItemEntity ie in _currentEntityInfo.CurrentRoomItems)
                            {
                                string sNextValue = null;
                                ItemTypeEnum? eNextValue = null;
                                if (ie.ItemType.HasValue)
                                    eNextValue = ie.ItemType.Value;
                                else
                                    sNextValue = ((UnknownItemEntity)ie).Name;
                                if (string.IsNullOrEmpty(sPreviousValue) && !ePreviousValue.HasValue) //first item
                                {
                                    sPreviousValue = sNextValue;
                                    ePreviousValue = eNextValue;
                                    iCounter = 0;
                                }
                                else if (!string.IsNullOrEmpty(sPreviousValue) && !string.Equals(sPreviousValue, sNextValue))
                                {
                                    sPreviousValue = sNextValue;
                                    ePreviousValue = eNextValue;
                                    iCounter = 0;
                                }
                                else if (ePreviousValue.HasValue && (!eNextValue.HasValue || eNextValue != ePreviousValue.Value))
                                {
                                    sPreviousValue = sNextValue;
                                    ePreviousValue = eNextValue;
                                    iCounter = 0;
                                }
                                iCounter++;
                                sioeis.Add(new SelectedInventoryOrEquipmentItem(ie, ie.ItemType, iCounter, ItemLocationType.Room));
                            }
                        }
                        else if (eInvProcessInputs == ItemsToProcessType.ProcessMonsterDrops)
                        {
                            List<ItemEntity> monsterItems = new List<ItemEntity>(_monsterKilledItems);
                            if (monsterItems.Count > 0)
                            {
                                backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, AbortIfHazying, true);
                                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                                foreach (ItemEntity ie in _currentEntityInfo.CurrentRoomItems)
                                {
                                    string sNextValue = null;
                                    ItemTypeEnum? eNextValue = null;
                                    if (ie.ItemType.HasValue)
                                        eNextValue = ie.ItemType.Value;
                                    else
                                        sNextValue = ((UnknownItemEntity)ie).Name;
                                    if (string.IsNullOrEmpty(sPreviousValue) && !ePreviousValue.HasValue) //first item
                                    {
                                        sPreviousValue = sNextValue;
                                        ePreviousValue = eNextValue;
                                        iCounter = 0;
                                    }
                                    else if (!string.IsNullOrEmpty(sPreviousValue) && !string.Equals(sPreviousValue, sNextValue))
                                    {
                                        sPreviousValue = sNextValue;
                                        ePreviousValue = eNextValue;
                                        iCounter = 0;
                                    }
                                    else if (ePreviousValue.HasValue && (!eNextValue.HasValue || eNextValue != ePreviousValue.Value))
                                    {
                                        sPreviousValue = sNextValue;
                                        ePreviousValue = eNextValue;
                                        iCounter = 0;
                                    }
                                    iCounter++;
                                    bool include = false;
                                    for (int i = monsterItems.Count - 1; i >= 0; i--)
                                    {
                                        ItemEntity nextDrop = monsterItems[i];
                                        string sNextDropValue = null;
                                        ItemTypeEnum? eNextDropValue = null;
                                        if (nextDrop.ItemType.HasValue)
                                            eNextDropValue = nextDrop.ItemType.Value;
                                        else
                                            sNextDropValue = ((UnknownItemEntity)nextDrop).Name;
                                        if (sNextDropValue == sNextValue && eNextDropValue == eNextValue && ie.Count == nextDrop.Count)
                                        {
                                            monsterItems.RemoveAt(i);
                                            include = true;
                                            break;
                                        }
                                    }
                                    if (include)
                                    {
                                        sioeis.Add(new SelectedInventoryOrEquipmentItem(ie, ie.ItemType, iCounter, ItemLocationType.Room));
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    SelectedItemsWithTargets = new List<SelectedItemWithTarget>();
                    foreach (SelectedInventoryOrEquipmentItem sioei in sioeis)
                    {
                        if (sioei.ItemType.HasValue)
                        {
                            ItemTypeEnum eItemType = sioei.ItemType.Value;
                            StaticItemData sid = ItemEntity.StaticItemData[eItemType];
                            DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, eItemType);
                            ColumnType ct;
                            if (sid.ItemClass == ItemClass.Fixed || sid.ItemClass == ItemClass.Chest)
                                ct = ColumnType.None;
                            else if (didWithInherit.OverflowAction == ItemInventoryOverflowAction.Ignore)
                                ct = ColumnType.None;
                            else if (isFerry || (didWithInherit.KeepCount > 0 || didWithInherit.SinkCount > 0))
                                ct = ColumnType.Target;
                            else if (didWithInherit.OverflowAction == ItemInventoryOverflowAction.SellOrJunk)
                                ct = ColumnType.SellOrJunk;
                            else
                            {
                                failedForRoomMessages.Add($"Unconfigured inventory management for item: {eItemType}");
                                continue;
                            }
                            if (ct == ColumnType.None)
                            {
                                continue;
                            }
                            else
                            {
                                SelectedItemWithTarget siwt = new SelectedItemWithTarget();
                                siwt.ItemEntity = sioei.ItemEntity;
                                siwt.ItemType = eItemType;
                                siwt.LocationType = ItemManagementLocationType.SourceRoom;
                                siwt.Target = ct;
                                SelectedItemsWithTargets.Add(siwt);
                            }
                        }
                        else
                        {
                            failedForRoomMessages.Add("Unknown item encountered: " + ((UnknownItemEntity)sioei.ItemEntity).Name);
                        }
                    }
                    if (failedForRoomMessages.Count > 0)
                    {
                        AddConsoleMessages(failedForRoomMessages);
                        return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                    }
                }
                HashSet<ItemTypeEnum> processedItemTypesAtTarget = new HashSet<ItemTypeEnum>();
            NextItemCycle:
                success = true;
                bool somethingDone = false;
                bool anythingCouldNotBePickedUpFromSourceRoom = false;
                failedForRoomMessages.Clear();
                List<SelectedItemWithTarget> itemsToRemoveFromProcessing = new List<SelectedItemWithTarget>();
                int weightFailed = int.MaxValue;
                foreach (var nextItemWithTargets in SelectedItemsWithTargets)
                {
                    if (nextItemWithTargets.LocationType == ItemManagementLocationType.SourceRoom)
                    {
                        ItemEntity nextItem = nextItemWithTargets.ItemEntity;
                        ItemTypeEnum eItemType = nextItem.ItemType.Value;
                        StaticItemData sid = ItemEntity.StaticItemData[eItemType];
                        DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, eItemType);
                        if (!sid.Weight.HasValue || sid.Weight.Value < weightFailed) //if heavier than something that already couldn't be picked up skip
                        {
                            string sItemText;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, eItemType, nextItemWithTargets.Counter, false, false);
                            }
                            if (string.IsNullOrEmpty(sItemText))
                            {
                                failedForRoomMessages.Add($"Failed to construct selection text for item: {eItemType}");
                            }
                            else
                            {
                                backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.GetItem, eItemType, sItemText, pms, AbortIfHazying);
                                if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                {
                                    lock (_currentEntityInfo.EntityLock)
                                    {
                                        nextItemWithTargets.LocationType = ItemManagementLocationType.Inventory;
                                        nextItemWithTargets.Counter = _currentEntityInfo.GetTotalInventoryCount(eItemType, false, true);
                                    }
                                    somethingDone = true;
                                }
                                else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulThisTime)
                                {
                                    if (sid.Weight.HasValue && sid.Weight.Value < weightFailed)
                                    {
                                        weightFailed = sid.Weight.Value;
                                    }
                                    anythingCouldNotBePickedUpFromSourceRoom = true;
                                }
                                else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                {
                                    return backgroundCommandResultObject;
                                }
                                else
                                {
                                    GetItemResult failureResult = (GetItemResult)backgroundCommandResultObject.ResultCode;
                                    if (failureResult == GetItemResult.FixedItem || failureResult == GetItemResult.QuestFulfilled)
                                    {
                                        itemsToRemoveFromProcessing.Add(nextItemWithTargets); //skip, as the item can't be taken at all
                                    }
                                    else if (failureResult == GetItemResult.ItemNotPresent)
                                    {
                                        failedForRoomMessages.Add($"Missing item: {eItemType}");
                                    }
                                    else if (failureResult == GetItemResult.MobDisallowsTakingItems)
                                    {
                                        failedForRoomMessages.Add($"Cannot proceed with inventory management.");
                                        break; //no way to pick up more items so stop picking up more
                                    }
                                    else //unexpected result code
                                    {
                                        failedForRoomMessages.Add($"Unexpected result code for item inventory management for: {eItemType}");
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (SelectedItemWithTarget nextItem in itemsToRemoveFromProcessing) SelectedItemsWithTargets.Remove(nextItem);
                if (failedForRoomMessages.Count > 0)
                {
                    AddConsoleMessages(failedForRoomMessages);
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }

                //sell/junk items that merit being sold or junked
                if (SelectedItemsWithTargets.Any((siwt) => { return siwt.Target == ColumnType.SellOrJunk && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment); }))
                {
                    List<SelectedItemWithTarget> siwts = new List<SelectedItemWithTarget>();
                    for (int i = SelectedItemsWithTargets.Count - 1; i >= 0; i--)
                    {
                        SelectedItemWithTarget siwt = SelectedItemsWithTargets[i];
                        if (siwt.Target == ColumnType.SellOrJunk && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment))
                        {
                            siwts.Add(siwt);
                            SelectedItemsWithTargets.RemoveAt(i);
                        }
                    }
                    backgroundCommandResultObject = SellOrJunkItems(siwts, pms, ref somethingDone, pr);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                }

                //drop off items at the tick room
                success = true;
                if (SelectedItemsWithTargets.Any((siwt) => { return siwt.Target == ColumnType.Tick && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment); }))
                {
                    List<SelectedItemWithTarget> siwts = new List<SelectedItemWithTarget>();
                    for (int i = SelectedItemsWithTargets.Count - 1; i >= 0; i--)
                    {
                        SelectedItemWithTarget siwt = SelectedItemsWithTargets[i];
                        if (siwt.Target == ColumnType.Tick && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment))
                        {
                            siwts.Add(siwt);
                            SelectedItemsWithTargets.RemoveAt(i);
                        }
                    }
                    backgroundCommandResultObject = NavigateToTickRoom(pms, false, pr);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                    foreach (SelectedItemWithTarget siwt in siwts)
                    {
                        backgroundCommandResultObject = EnsureItemInInventory(siwt, pms);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                        string sItemText;
                        lock (_currentEntityInfo.EntityLock)
                        {
                            sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, siwt.ItemType, siwt.Counter, false, false);
                        }
                        if (string.IsNullOrEmpty(sItemText))
                        {
                            AddConsoleMessage("Unable to construct drop selection text for " + siwt.ItemType);
                            success = false;
                            continue;
                        }
                        if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, siwt.ItemType, sItemText, pms, AbortIfHazying).Result == CommandResult.CommandSuccessful)
                        {
                            somethingDone = true;
                            continue;
                        }
                        else
                        {
                            AddConsoleMessage("Failed to drop " + siwt.ItemType);
                            success = false;
                        }
                    }
                }
                if (!success) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);

                //go to the target room and determine what to do with the items
                success = true;
                if (SelectedItemsWithTargets.Any((siwt) => { return siwt.Target == ColumnType.Target && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment); }))
                {
                    List<SelectedItemWithTarget> siwts = new List<SelectedItemWithTarget>();
                    for (int i = SelectedItemsWithTargets.Count - 1; i >= 0; i--)
                    {
                        SelectedItemWithTarget siwt = SelectedItemsWithTargets[i];
                        if (siwt.Target == ColumnType.Target && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment))
                        {
                            siwts.Add(siwt);
                            SelectedItemsWithTargets.RemoveAt(i);
                        }
                    }
                    if (inventorySinkRoom == null)
                    {
                        AddConsoleMessage("No inventory sink room specified.");
                        return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                    }
                    backgroundCommandResultObject = NavigateToSpecificRoom(inventorySinkRoom, pms, false, pr);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResultObject;
                    }
                    _backgroundProcessPhase = BackgroundProcessPhase.InventoryManagement;
                    HashSet<ItemTypeEnum> itemTypesToProcess = new HashSet<ItemTypeEnum>();
                    foreach (SelectedItemWithTarget siwt in siwts)
                    {
                        ItemTypeEnum itemType = siwt.ItemType;
                        StaticItemData sid = ItemEntity.StaticItemData[itemType];
                        if (!sid.IsCurrency() && !processedItemTypesAtTarget.Contains(siwt.ItemType) && !itemTypesToProcess.Contains(siwt.ItemType))
                        {
                            itemTypesToProcess.Add(siwt.ItemType);
                        }
                    }
                    foreach (ItemTypeEnum itemType in itemTypesToProcess) //determine what to do with the items
                    {
                        int iInventoryCount;
                        int iEquipmentCount;
                        int iTargetCount;
                        lock (_currentEntityInfo.EntityLock)
                        {
                            iInventoryCount = _currentEntityInfo.GetTotalInventoryCount(itemType, false, true);
                            iEquipmentCount = _currentEntityInfo.GetTotalInventoryCount(itemType, true, false);
                            iTargetCount = _currentEntityInfo.GetTotalRoomItemsCount(itemType);
                        }
                        List<SelectedItemWithTarget> sourceEntries = SelectedItemsWithTargets.FindAll((entry) => { return entry.ItemType == itemType && entry.LocationType == ItemManagementLocationType.SourceRoom; });
                        int iSourceCount = sourceEntries.Count;
                        int iTotalInventoryCount = iInventoryCount + iEquipmentCount;

                        DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, itemType);
                        int iKeepCount = didWithInherit.KeepCount;
                        int iSinkCount = didWithInherit.SinkCount;

                        for (int i = iTotalInventoryCount + 1; i <= iKeepCount; i++) //fill inventory from source or target
                        {
                            if (iSourceCount > 0)
                            {
                                iSourceCount--;
                                SelectedItemsWithTargets[iSourceCount].Target = ColumnType.Inventory;
                            }
                            else if (iTargetCount > 0)
                            {
                                SelectedItemWithTarget entry = new SelectedItemWithTarget();
                                entry.ItemEntity = new ItemEntity(itemType, 1, 1);
                                entry.ItemType = itemType;
                                entry.Counter = iTargetCount;
                                entry.LocationType = ItemManagementLocationType.TargetRoom;
                                entry.Target = ColumnType.Inventory;
                                SelectedItemsWithTargets.Add(entry);
                                iTargetCount--;
                            }
                        }
                        for (int i = iTargetCount; i < iSinkCount; i++) //fill target from inventory or source
                        {
                            if (iSourceCount > 0)
                            {
                                iSourceCount--;
                                SelectedItemsWithTargets[iSourceCount].Target = ColumnType.Target;
                            }
                            else if (iTotalInventoryCount > iKeepCount)
                            {
                                if (iInventoryCount > 0)
                                {
                                    SelectedItemWithTarget entry = new SelectedItemWithTarget();
                                    entry.ItemEntity = new ItemEntity(itemType, 1, 1);
                                    entry.ItemType = itemType;
                                    entry.Counter = iInventoryCount;
                                    entry.LocationType = ItemManagementLocationType.Inventory;
                                    entry.Target = ColumnType.Target;
                                    SelectedItemsWithTargets.Add(entry);
                                    iInventoryCount--;
                                    iTotalInventoryCount--;
                                }
                                else if (iEquipmentCount > 0)
                                {
                                    SelectedItemWithTarget entry = new SelectedItemWithTarget();
                                    entry.ItemEntity = new ItemEntity(itemType, 1, 1);
                                    entry.ItemType = itemType;
                                    entry.Counter = iEquipmentCount;
                                    entry.LocationType = ItemManagementLocationType.Equipment;
                                    entry.Target = ColumnType.Target;
                                    SelectedItemsWithTargets.Add(entry);
                                    iEquipmentCount--;
                                    iTotalInventoryCount--;
                                }
                            }
                        }
                        for (int i = iTotalInventoryCount; i > iKeepCount; i--) //dispose of excess inventory
                        {
                            if (iInventoryCount > 0)
                            {
                                SelectedItemWithTarget entry = new SelectedItemWithTarget();
                                entry.ItemEntity = new ItemEntity(itemType, 1, 1);
                                entry.ItemType = itemType;
                                entry.Counter = iInventoryCount;
                                entry.LocationType = ItemManagementLocationType.Inventory;
                                entry.Target = ColumnType.SellOrJunk;
                                SelectedItemsWithTargets.Add(entry);
                                iInventoryCount--;
                                iTotalInventoryCount--;
                            }
                            else if (iEquipmentCount > 0)
                            {
                                SelectedItemWithTarget entry = new SelectedItemWithTarget();
                                entry.ItemEntity = new ItemEntity(itemType, 1, 1);
                                entry.ItemType = itemType;
                                entry.Counter = iEquipmentCount;
                                entry.LocationType = ItemManagementLocationType.Equipment;
                                entry.Target = ColumnType.SellOrJunk;
                                SelectedItemsWithTargets.Add(entry);
                                iEquipmentCount--;
                                iTotalInventoryCount--;
                            }
                        }
                    }

                    for (int i = siwts.Count - 1; i >= 0; i--)
                    {
                        SelectedItemWithTarget siwt = siwts[i];
                        if (siwt.Target == ColumnType.Target && (siwt.LocationType == ItemManagementLocationType.Inventory || siwt.LocationType == ItemManagementLocationType.Equipment))
                        {
                            backgroundCommandResultObject = EnsureItemInInventory(siwt, pms);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                            string sItemText;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, siwt.ItemType, siwt.Counter, false, false);
                            }
                            if (string.IsNullOrEmpty(sItemText))
                            {
                                AddConsoleMessage("Unable to construct drop selection text for " + siwt.ItemType);
                                success = false;
                                continue;
                            }
                            if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, siwt.ItemType, sItemText, pms, AbortIfHazying).Result == CommandResult.CommandSuccessful)
                            {
                                somethingDone = true;
                                continue;
                            }
                            else
                            {
                                AddConsoleMessage("Failed to drop " + siwt.ItemType);
                                success = false;
                            }
                            siwts.RemoveAt(i);
                        }
                    }
                }
                if (!success) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);

                if (anythingCouldNotBePickedUpFromSourceRoom)
                {
                    backgroundCommandResultObject = NavigateToSpecificRoom(inventorySourceRoom, pms, false, pr);
                    if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways && beforeTargetRoom)
                    {
                        //getting back to the inventory source room may not work, such as if there is a day-only exit in between. in that case
                        //treat as successful as whatever has been ferried so far is the best we can do.
                        return new CommandResultObject(CommandResult.CommandSuccessful);
                    }
                    else if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResultObject;
                    }
                    _backgroundProcessPhase = BackgroundProcessPhase.InventoryManagement;
                    pms.AtDestination = true;
                    if (somethingDone)
                    {
                        goto NextItemCycle;
                    }
                    else
                    {
                        return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                    }
                }
            }

            if (SelectedItemsWithTargets == null)
            {
                return new CommandResultObject(CommandResult.CommandSuccessful);
            }

            //at this point all items to be processed from the source room have been picked up, and the only thing remaining is to deal with inventory items
            if (SelectedItemsWithTargets.Any((entry) => { return entry.LocationType == ItemManagementLocationType.TargetRoom; }))
            {
                backgroundCommandResultObject = NavigateToSpecificRoom(inventorySinkRoom, pms, false, pr);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                _backgroundProcessPhase = BackgroundProcessPhase.InventoryManagement;
            }
            failedForRoomMessages.Clear();
            success = true;
            for (int i = SelectedItemsWithTargets.Count - 1; i >= 0; i--)
            {
                SelectedItemWithTarget siwt = SelectedItemsWithTargets[i];
                if (siwt.Target == ColumnType.Inventory && siwt.LocationType == ItemManagementLocationType.Equipment)
                {
                    backgroundCommandResultObject = EnsureItemInInventory(siwt, pms);
                    if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                    {
                        SelectedItemsWithTargets.RemoveAt(i);
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (siwt.LocationType == ItemManagementLocationType.TargetRoom && (siwt.Target == ColumnType.Inventory || siwt.Target == ColumnType.Equipment))
                {
                    string sItemText;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, siwt.ItemType, siwt.Counter, false, false);
                    }
                    if (string.IsNullOrEmpty(sItemText))
                    {
                        failedForRoomMessages.Add($"Failed to construct selection text for item: {siwt.ItemType}");
                    }
                    else
                    {
                        backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.GetItem, siwt.ItemType, sItemText, pms, AbortIfHazying);
                        if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                        {
                            if (siwt.Target == ColumnType.Inventory)
                            {
                                SelectedItemsWithTargets.RemoveAt(i);
                            }
                            else
                            {
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    siwt.LocationType = ItemManagementLocationType.Inventory;
                                    siwt.Counter = _currentEntityInfo.GetTotalInventoryCount(siwt.ItemType, false, true);
                                }
                            }
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
            }
            if (failedForRoomMessages.Count > 0)
            {
                AddConsoleMessages(failedForRoomMessages);
                return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            }
            if (!success) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            foreach (SelectedItemWithTarget siwt in SelectedItemsWithTargets)
            {
                if (siwt.Target == ColumnType.Equipment && siwt.LocationType == ItemManagementLocationType.Inventory)
                {
                    StaticItemData sid = ItemEntity.StaticItemData[siwt.ItemType];
                    string sItemText;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, siwt.ItemType, siwt.Counter, false, false);
                    }
                    if (string.IsNullOrEmpty(sItemText))
                    {
                        failedForRoomMessages.Add($"Failed to construct selection text for item: {siwt.ItemType}");
                    }
                    BackgroundCommandType bct;
                    if (sid.WeaponType.HasValue)
                        bct = BackgroundCommandType.WieldWeapon;
                    else if (sid.EquipmentType != EquipmentType.Holding)
                        bct = BackgroundCommandType.WearEquipment;
                    else
                        bct = BackgroundCommandType.HoldItem;
                    backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(bct, siwt.ItemType, sItemText, pms, AbortIfHazying);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        success = false;
                    }
                }
            }
            if (failedForRoomMessages.Count > 0)
            {
                AddConsoleMessages(failedForRoomMessages);
                return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            }
            if (!success) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);

            //dispose of junk broken keys
            if (pr != null && pr.SupportedKeys != SupportedKeysFlags.None && pms.NewArea != null && pms.NewArea.PawnShop.HasValue && !beforeTargetRoom)
            {
                backgroundCommandResultObject = NavigateToPawnShop(pms, false, pr);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
                bool bTemp = false;
                backgroundCommandResultObject = JunkBrokenKeys(pms, ref bTemp);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
            }
            CommandResultObject ret;
            if (_bwBackgroundProcess.CancellationPending)
            {
                ret = new CommandResultObject(CommandResult.CommandAborted);
            }
            else if (_hazying)
            {
                ret = new CommandResultObject(CommandResult.CommandEscaped);
            }
            else
            {
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
                if (pr != null)
                {
                    pr.InventoryManagementFinished = true;
                }
            }
            return ret;
        }

        private CommandResultObject EnsureItemInInventory(SelectedItemWithTarget siwt, BackgroundWorkerParameters pms)
        {
            if (siwt.LocationType == ItemManagementLocationType.Equipment)
            {
                string sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Equipment, siwt.ItemType, siwt.Counter, false, false);
                if (string.IsNullOrEmpty(sItemText))
                {
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
                CommandResultObject backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, siwt.ItemType, sItemText, pms, AbortIfHazying);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
                lock (_currentEntityInfo.EntityLock)
                {
                    siwt.LocationType = ItemManagementLocationType.Inventory;
                    siwt.Counter = _currentEntityInfo.GetTotalInventoryCount(siwt.ItemType, false, true);
                }
            }
            return new CommandResultObject(CommandResult.CommandSuccessful);
        }

        private void RemoveKeyFromHeldSlot(ItemTypeEnum? keyItemType, BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            RemoveHeldItem(pms, abortLogic, (ie) => 
            {
                bool ret;
                if (keyItemType.HasValue) //only remove the specified key item type
                {
                    ret = keyItemType == ie.ItemType;
                }
                else //remove any key item type
                {
                    StaticItemData sid = ItemEntity.StaticItemData[ie.ItemType.Value];
                    ret = sid.ItemClass == ItemClass.Key;
                }
                return ret;
            });
        }

        private CommandResultObject RemoveHeldItem(BackgroundWorkerParameters pms, Func<bool> abortLogic, Func<ItemEntity, bool> removeCheck)
        {
            string sItemText = null;
            int iHeldSlot = (int)EquipmentSlot.Held;
            ItemTypeEnum eItemType = ItemTypeEnum.GoldCoins;
            lock (_currentEntityInfo.EntityLock)
            {
                ItemEntity ie = _currentEntityInfo.Equipment[iHeldSlot];
                if (ie != null && ie.ItemType.HasValue && (removeCheck == null || removeCheck(ie)))
                {
                    eItemType = ie.ItemType.Value;
                    sItemText = _currentEntityInfo.PickItemTextFromActualIndex(ItemLocationType.Equipment, eItemType, iHeldSlot, false);
                    if (string.IsNullOrEmpty(sItemText)) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
            }
            CommandResultObject ret;
            if (string.IsNullOrEmpty(sItemText))
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else
                ret = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, eItemType, sItemText, pms, abortLogic);
            return ret;
        }

        private CommandResultObject DrinkPotionForSpell(SpellInformationAttribute spellInfo, BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            string sItemText;
            bool removeHeldPotion = false;
            ItemTypeEnum? potItem;
            lock (_currentEntityInfo.EntityLock)
            {
                if (!_currentEntityInfo.HasPotionForSpell(spellInfo.SpellType, out potItem, out bool? inInventory)) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                ItemLocationType ilt = inInventory.Value ? ItemLocationType.Inventory : ItemLocationType.Equipment;
                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ilt, potItem.Value, 1, false, ilt == ItemLocationType.Equipment);
                if (string.IsNullOrEmpty(sItemText) && ilt == ItemLocationType.Equipment)
                {
                    removeHeldPotion = true;
                }
            }
            if (removeHeldPotion)
            {
                CommandResultObject backgroundCommandResult = RemoveHeldItem(pms, abortLogic, null);
                if (backgroundCommandResult.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResult;
                }
                lock (_currentEntityInfo.EntityLock)
                {
                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, potItem.Value, 1, false, false);
                }
                if (string.IsNullOrEmpty(sItemText)) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            }
            return RunSingleCommand(BackgroundCommandType.DrinkNonHazyPotion, "drink " + sItemText, pms, abortLogic, false);
        }

        /// <summary>
        /// navigates to a tick room. assumed to be in the context of a perm run
        /// </summary>
        /// <param name="pms">background parameter</param>
        /// <param name="beforeGetToTargetRoom">whether flee is allowed</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject NavigateToTickRoom(BackgroundWorkerParameters pms, bool beforeGetToTargetRoom, PermRun pr)
        {
            CommandResultObject ret;
            Room r = _currentEntityInfo.CurrentRoom;
            if (r == null)
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else if (r.HealingRoom.HasValue)
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
            else if (!pms.HealingRoom.HasValue)
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else
                ret = NavigateToSpecificRoom(_gameMap.HealingRooms[pms.HealingRoom.Value], pms, beforeGetToTargetRoom, pr);
            return ret;
        }

        private CommandResultObject NavigateToPawnShop(BackgroundWorkerParameters pms, bool beforeGetToTargetRoom, PermRun pr)
        {
            CommandResultObject ret;
            Room r = _currentEntityInfo.CurrentRoom;
            if (r == null)
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else if (r.PawnShoppe.HasValue)
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
            else if (!pms.PawnShop.HasValue)
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else
                ret = NavigateToSpecificRoom(_gameMap.PawnShoppes[pms.PawnShop.Value], pms, beforeGetToTargetRoom, pr);
            return ret;
        }

        private CommandResultObject NavigateToSpecificRoom(Room r, BackgroundWorkerParameters pms, bool beforeGetToTargetRoom, PermRun pr)
        {
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            CommandResultObject backgroundCommandResultObject;
            if (currentRoom != r)
            {
                var nextRoute = CalculateRouteExits(currentRoom, r);
                if (nextRoute == null) return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                backgroundCommandResultObject = TraverseExitsAlreadyInBackground(nextRoute, pms, beforeGetToTargetRoom, pr);
            }
            else
            {
                backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandSuccessful);
            }
            pms.AtDestination = r == pms.TargetRoom;
            return backgroundCommandResultObject;
        }

        private bool IsFull(FullType fullType, WorkflowSpells castWorkflowSpells, out WorkflowSpells needWorkflowSpells)
        {
            needWorkflowSpells = WorkflowSpells.None;
            if (fullType == FullType.None) return true;
            needWorkflowSpells = castWorkflowSpells & ~WorkflowSpells.CurePoison;
            bool ret = true;
            if (_autohp < _totalhp)
            {
                ret = false;
            }
            else if (fullType == FullType.Total)
            {
                ret = _automp >= _totalmp;
            }
            else if (fullType == FullType.Almost)
            {
                ret = _automp + GetHealingRoomTickMP() > _totalmp;
            }
            castWorkflowSpells &= ~WorkflowSpells.CurePoison;
            if (castWorkflowSpells != WorkflowSpells.None)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    foreach (WorkflowSpells nextWorkflowSpell in Enum.GetValues(typeof(WorkflowSpells)))
                    {
                        if ((nextWorkflowSpell & castWorkflowSpells) != WorkflowSpells.None)
                        {
                            SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextWorkflowSpell];
                            if (_spellsCast.Contains(sia.SpellType))
                                needWorkflowSpells &= ~nextWorkflowSpell;
                            else
                                ret = false;
                        }
                    }
                }
            }
            return ret;
        }

        private CommandResultObject SellOrJunkItems(List<SelectedItemWithTarget> siwts, BackgroundWorkerParameters pms, ref bool somethingDone, PermRun pr)
        {
            CommandResultObject backgroundCommandResultObject;
            bool success = true;
            if (siwts.Count > 0)
            {
                backgroundCommandResultObject = NavigateToPawnShop(pms, false, pr);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
                _backgroundProcessPhase = BackgroundProcessPhase.InventoryManagement;
                foreach (SelectedItemWithTarget next in siwts)
                {
                    if (next.LocationType == ItemManagementLocationType.Equipment)
                    {
                        backgroundCommandResultObject = EnsureItemInInventory(next, pms);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                    }
                    ItemEntity nextItem = next.ItemEntity;
                    ItemTypeEnum itemType = nextItem.ItemType.Value;
                    string sItemText;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, itemType, next.Counter, true, false);
                    }
                    if (string.IsNullOrEmpty(sItemText))
                    {
                        AddConsoleMessage("Unable to construct drop selection text for " + itemType);
                        success = false;
                        continue;
                    }
                    StaticItemData sid = ItemEntity.StaticItemData[itemType];
                    bool trySell = sid.SellGold > 0 || sid.Sellable == SellableEnum.Unknown;
                    if (trySell && TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.SellItem, itemType, sItemText, pms, AbortIfHazying).Result == CommandResult.CommandSuccessful)
                    {
                        somethingDone = true;
                        continue;
                    }

                    if (sid.Sellable == SellableEnum.Junk)
                    {
                        if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, itemType, sItemText, pms, AbortIfHazying).Result == CommandResult.CommandSuccessful)
                        {
                            somethingDone = true;
                            continue;
                        }
                        else
                        {
                            AddConsoleMessage("Failed to junk " + itemType);
                            success = false;
                        }
                    }
                    else
                    {
                        AddConsoleMessage("Failed to sell non-junk item: " + itemType);
                        success = false;
                    }
                }
            }
            CommandResultObject ret;
            if (_bwBackgroundProcess.CancellationPending)
                ret = new CommandResultObject(CommandResult.CommandAborted);
            else if (_hazying)
                ret = new CommandResultObject(CommandResult.CommandEscaped);
            else if (!success)
                ret = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
            else
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
            return ret;
        }

        private CommandResultObject JunkBrokenKeys(BackgroundWorkerParameters pms, ref bool somethingDone)
        {
            CommandResultObject backgroundCommandResultObject;
            RemoveKeyFromHeldSlot(null, pms, AbortIfHazying);
            List<SelectedInventoryOrEquipmentItem> lst;
            lock (_currentEntityInfo.EntityLock)
            {
                lst = _currentEntityInfo.GetInvEqItems((ie) => { return ie.IsItemClass(ItemClass.Key); }, true, false);
            }
            lst.Reverse(); //handle in reverse order to prevent index shifting
            foreach (SelectedInventoryOrEquipmentItem sioei in lst)
            {
                string sItemText;
                ItemTypeEnum eKeyType = sioei.ItemType.Value;
                lock (_currentEntityInfo.EntityLock)
                {
                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, eKeyType, sioei.Counter, false, false);
                }
                _commandInventoryItem = sioei;
                backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtItem, "look " + sItemText, pms, AbortIfHazying, false);
                if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                {
                    return backgroundCommandResultObject;
                }
                else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                {
                    ItemStatus eStatus = (ItemStatus)backgroundCommandResultObject.ResultCode;
                    if (eStatus == ItemStatus.Broken)
                    {
                        if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, eKeyType, sItemText, pms, AbortIfHazying).Result == CommandResult.CommandSuccessful)
                            somethingDone = true;
                        else
                            AddConsoleMessage("Failed to junk " + sItemText);
                    }
                }
            }
            return new CommandResultObject(CommandResult.CommandSuccessful);
        }

        private bool FoundMob(BackgroundWorkerParameters pms)
        {
            bool ret = false;
            string mobTarget = string.Empty;
            if (pms.MobType.HasValue)
            {
                StaticMobData smd = MobEntity.StaticMobData[pms.MobType.Value];
                mobTarget = GetMobTargetFromMobType(pms.MobType.Value, pms.MobTypeCounter, false);
                if (string.IsNullOrEmpty(mobTarget))
                {
                    return false;
                }
                if (smd.Visibility == MobVisibility.Visible)
                {
                    return true;
                }
            }
            else if (!string.IsNullOrEmpty(pms.MobText))
            {
                mobTarget = pms.MobText;
            }
            else if (pms.MobTypeCounter >= 1) //attacking any monster based on what's in the room
            {
                int iCounter = 1;
                MobTypeEnum? foundMob = null;
                lock (_currentEntityInfo)
                {
                    if (_currentEntityInfo.CurrentRoomMobs.Count >= pms.MobTypeCounter)
                    {
                        foundMob = _currentEntityInfo.CurrentRoomMobs[pms.MobTypeCounter - 1];
                        for (int i = 0; i < pms.MobTypeCounter - 1; i++)
                        {
                            if (_currentEntityInfo.CurrentRoomMobs[i] == foundMob)
                            {
                                iCounter++;
                            }
                        }
                    }
                }
                ret = foundMob.HasValue;
                if (ret)
                {
                    pms.MobText = string.Empty;
                    pms.MobTextCounter = 0;
                    pms.MobType = foundMob.Value;
                    pms.MobTypeCounter = iCounter;
                }
            }
            else //not attacking anything
            {
                ret = false;
            }
            if (!string.IsNullOrEmpty(mobTarget)) //look at the mob to verify it exists
            {
                CommandResultObject resultObject = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + mobTarget, pms, AbortIfFleeingOrHazying, true);
                ret = resultObject.Result == CommandResult.CommandSuccessful;
            }
            return ret;
        }

        private string GetMobTarget(bool forLook)
        {
            string sMobTextForTarget;
            if (_currentlyFightingMobType.HasValue)
                sMobTextForTarget = GetMobTargetFromMobType(_currentlyFightingMobType.Value, _currentlyFightingMobCounter, forLook);
            else
                sMobTextForTarget = _mob;
            return sMobTextForTarget;
        }

        private string GetMobTargetFromMobType(MobTypeEnum eMobType, int mobTypeIndex, bool forLook)
        {
            string ret;
            StaticMobData smd = MobEntity.StaticMobData[eMobType];
            if (smd.Visibility == MobVisibility.Visible)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    ret = _currentEntityInfo.PickMobTextFromMobCounter(null, MobLocationType.CurrentRoomMobs, eMobType, mobTypeIndex, false, forLook);
                }
            }
            else
            {
                List<MobTypeEnum> tempList = new List<MobTypeEnum>();
                for (int i = 0; i < mobTypeIndex; i++)
                {
                    tempList.Add(eMobType);
                }
                ret = _currentEntityInfo.PickMobTextFromMobCounter(tempList, MobLocationType.PickFromList, eMobType, tempList.Count, false, forLook);
            }
            return ret;
        }

        /// <summary>
        /// runs a command adding or removing an item from inventory
        /// </summary>
        /// <param name="commandType">command type: get/drop/sell/remove item</param>
        /// <param name="itemType">item type</param>
        /// <param name="itemText">precomputed item text respecting the item's current location</param>
        /// <param name="pms">background command parameters</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject TryCommandAddingOrRemovingFromInventory(BackgroundCommandType commandType, ItemTypeEnum? itemType, string itemText, BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            CommandResultObject backgroundCommandResultObject;
            StaticItemData sid = itemType.HasValue ? ItemEntity.StaticItemData[itemType.Value] : null;
            BackgroundCommandType checkWeightCommandType = BackgroundCommandType.Quit;
            bool checkWeightIsEquipment = false;
            string checkWeightCommand = null;
            bool checkWeight = sid != null && !sid.Weight.HasValue && !sid.IsCurrency();
            int beforeWeight = 0;
            int afterWeight;
            int beforeGold = _gold;
            if (checkWeight)
            {
                checkWeightIsEquipment = commandType == BackgroundCommandType.WieldWeapon || commandType == BackgroundCommandType.HoldItem || commandType == BackgroundCommandType.WearEquipment;
                if (checkWeightIsEquipment)
                {
                    checkWeightCommandType = BackgroundCommandType.Equipment;
                    checkWeightCommand = "equipment";
                }
                else
                {
                    checkWeightCommandType = BackgroundCommandType.Inventory;
                    checkWeightCommand = "inventory";
                }
                
                backgroundCommandResultObject = RunSingleCommandForCommandResult(checkWeightCommandType, checkWeightCommand, pms, abortLogic, true);
                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResultObject;
                }
                lock (_currentEntityInfo.EntityLock)
                {
                    beforeWeight = (checkWeightIsEquipment ? _currentEntityInfo.TotalEquipmentWeight : _currentEntityInfo.TotalInventoryWeight).Value;
                }
            }
            string command;
            switch (commandType)
            {
                case BackgroundCommandType.GetItem:
                    command = "get";
                    break;
                case BackgroundCommandType.DropItem:
                    command = "drop";
                    break;
                case BackgroundCommandType.SellItem:
                    command = "sell";
                    break;
                case BackgroundCommandType.RemoveEquipment:
                    command = "remove";
                    break;
                case BackgroundCommandType.WearEquipment:
                    command = "wear";
                    break;
                case BackgroundCommandType.WieldWeapon:
                    command = "wield";
                    break;
                case BackgroundCommandType.HoldItem:
                    command = "hold";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            CommandResultObject ret = RunSingleCommandForCommandResult(commandType, command + " " + itemText, pms, abortLogic, false);
            if (ret.Result == CommandResult.CommandSuccessful)
            {
                List<string> broadcastMessages = null;
                if (commandType == BackgroundCommandType.SellItem && sid != null)
                {
                    int goldDifference = _gold - beforeGold;
                    if (goldDifference > 0 && (sid.SellGold == 0 || goldDifference > sid.SellGold))
                    {
                        broadcastMessages = new List<string>()
                        {
                            "Sold " + itemType.ToString() + " for " + goldDifference
                        };
                        if (sid.SellGold == 0) sid.SellGold = goldDifference;
                    }
                }
                if (checkWeight)
                {
                    if (RunSingleCommandForCommandResult(checkWeightCommandType, checkWeightCommand, pms, abortLogic, true).Result != CommandResult.CommandSuccessful)
                    {
                        return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                    }
                    lock (_currentEntityInfo.EntityLock)
                    {
                        afterWeight = (checkWeightIsEquipment ? _currentEntityInfo.TotalEquipmentWeight : _currentEntityInfo.TotalInventoryWeight).Value;
                    }
                    int weightDifference = afterWeight - beforeWeight;
                    if (commandType == BackgroundCommandType.DropItem || commandType == BackgroundCommandType.SellItem)
                    {
                        weightDifference = -weightDifference;
                    }
                    if (weightDifference >= 0) //zero is a valid case since zero weight objects are possible
                    {
                        if (broadcastMessages == null) broadcastMessages = new List<string>();
                        broadcastMessages.Add("Found weight for " + sid.ItemType.ToString() + ": " + weightDifference);
                        sid.Weight = weightDifference;
                    }
                }
                AddBroadcastMessages(broadcastMessages);
            }
            return ret;
        }

        /// <summary>
        /// gets full hitpoints in a background process, optionally with cure-poison
        /// This is used when traversing exits after a trap room is encountered.
        /// </summary>
        /// <param name="pms">background worker parameters</param>
        /// <param name="needCurePoison">whether cure-poison is needed</param>
        /// <param name="abortLogic">abort logic</param>
        /// <returns>true if cure-poison was successfully cast if needed and hitpoints got to maximum</returns>
        private CommandResultObject GetFullHitpoints(BackgroundWorkerParameters pms, bool needCurePoison, Func<bool> abortLogic, PermRun pr)
        {
            CommandResultObject nextCommandResultObject;
            WorkflowSpells spellsToCast = pr == null ? WorkflowSpells.CurePoison : pr.SpellsToCast;
            WorkflowSpells spellsToPotion = pr == null ? WorkflowSpells.CurePoison : pr.SpellsToPotion;
            if (needCurePoison)
            {
                SpellInformationAttribute siaCurePoison = SpellsStatic.SpellsByEnum[SpellsEnum.curepoison];
                if ((spellsToPotion & WorkflowSpells.CurePoison) != WorkflowSpells.None)
                {
                    nextCommandResultObject = DrinkPotionForSpell(siaCurePoison, pms, abortLogic);
                    if (nextCommandResultObject.Result == CommandResult.CommandSuccessful)
                    {
                        needCurePoison = false;
                    }
                    else if (nextCommandResultObject.Result == CommandResult.CommandAborted || nextCommandResultObject.Result == CommandResult.CommandTimeout || nextCommandResultObject.Result == CommandResult.CommandEscaped)
                    {
                        return nextCommandResultObject;
                    }
                }
                if (needCurePoison && ((spellsToCast & WorkflowSpells.CurePoison) != WorkflowSpells.None))
                {
                    if (_automp >= siaCurePoison.Mana)
                    {
                        nextCommandResultObject = CastSpellOnSelf(SpellsEnum.curepoison, pms, abortLogic);
                        if (nextCommandResultObject.Result == CommandResult.CommandSuccessful)
                        {
                            needCurePoison = false;
                        }
                        else if (nextCommandResultObject.Result == CommandResult.CommandAborted || nextCommandResultObject.Result == CommandResult.CommandTimeout || nextCommandResultObject.Result == CommandResult.CommandEscaped)
                        {
                            return nextCommandResultObject;
                        }
                    }
                }
                if (needCurePoison)
                {
                    AddConsoleMessage("Unable to cure poison.");
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
            }
            SpellInformationAttribute siaVigor = SpellsStatic.SpellsByEnum[SpellsEnum.vigor];
            while (_autohp < _totalhp && _automp >= siaVigor.Mana)
            {
                nextCommandResultObject = CastSpellOnSelf(SpellsEnum.vigor, pms, abortLogic);
                if (nextCommandResultObject.Result != CommandResult.CommandSuccessful)
                {
                    return nextCommandResultObject;
                }
            }
            return new CommandResultObject(CommandResult.CommandSuccessful);
        }

        /// <summary>
        /// gets full in a background process
        /// </summary>
        /// <param name="pms">background parameters</param>
        /// <param name="fullType">full type</param>
        /// <param name="spellsToCast">spells to maintain via casting</param>
        /// <returns>true if successfully got to full, false otherwise</returns>
        private CommandResultObject GetFullInBackground(BackgroundWorkerParameters pms, FullType fullType, WorkflowSpells spellsToCast, Func<bool> abortLogic)
        {
            int iTickHP = GetHealingRoomTickHP();
            int iTickMP = GetHealingRoomTickMP();
            CommandResultObject backgroundCommandResultObject;
            while (!IsFull(fullType, spellsToCast, out WorkflowSpells needWorkflowSpells))
            {
                int automp = _automp;
                int autohp = _autohp;
                int numTicksForFullMP = (_totalmp - automp - 1) / iTickMP + 1;
                int numTicksForFullHP = (_totalhp - autohp - 1) / iTickHP + 1;

                bool castSomething = false;
                if (numTicksForFullHP > numTicksForFullMP)
                {
                    backgroundCommandResultObject = CastSpellOnSelf(SpellsEnum.vigor, pms, abortLogic);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResultObject;
                    }
                    castSomething = true;
                }
                else
                {
                    if (automp + iTickMP > _totalmp) //wait until almost full mp before casting spells
                    {
                        foreach (WorkflowSpells nextSpell in Enum.GetValues(typeof(WorkflowSpells)))
                        {
                            if ((nextSpell & needWorkflowSpells) != WorkflowSpells.None)
                            {
                                SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextSpell];
                                SpellsEnum spellType = sia.SpellType;
                                bool hasSpellActive;
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    hasSpellActive = _spellsCast.Contains(spellType);
                                }
                                if (!hasSpellActive && _automp >= sia.Mana)
                                {
                                    backgroundCommandResultObject = CastSpellOnSelf(spellType, pms, abortLogic);
                                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                                    {
                                        return backgroundCommandResultObject;
                                    }
                                    castSomething = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!castSomething)
                {
                    RunPollTickIfNecessary(_autohp, _automp, DateTime.UtcNow);
                    int waitInterval = numTicksForFullMP <= 2 && numTicksForFullHP <= 1 ? 500 : 5000;
                    while (waitInterval > 0)
                    {
                        Thread.Sleep(50);
                        waitInterval -= 50;
                        if (_fleeing || _hazying) return new CommandResultObject(CommandResult.CommandEscaped);
                        if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);
                        RunQueuedCommandWhenBackgroundProcessRunning(pms);
                    }
                }
                if (_fleeing || _hazying) return new CommandResultObject(CommandResult.CommandEscaped);
                if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);
                RunQueuedCommandWhenBackgroundProcessRunning(pms);
            }
            return new CommandResultObject(CommandResult.CommandSuccessful);
        }

        public PotionsCommandChoiceResult GetPotionsCommand(PotionsStrategyStep nextPotionsStep, out string command, int currentHP, int totalHP, IsengardSettingData settings)
        {
            command = null;
            lock (_currentEntityInfo.EntityLock)
            {
                bool supportsMend = settings.PotionsMendOnlyWhenDownXHP > 0;
                bool supportsVigor = settings.PotionsVigorOnlyWhenDownXHP > 0;
                if (nextPotionsStep == PotionsStrategyStep.Vigor && !supportsVigor) return PotionsCommandChoiceResult.Fail;
                if (nextPotionsStep == PotionsStrategyStep.MendWounds && !supportsMend) return PotionsCommandChoiceResult.Fail;
                if (nextPotionsStep == PotionsStrategyStep.GenericHeal && !supportsVigor && !supportsMend) return PotionsCommandChoiceResult.Fail;
                bool canMend = supportsMend && currentHP + settings.PotionsMendOnlyWhenDownXHP <= totalHP;
                bool canVigor = supportsVigor && currentHP + settings.PotionsVigorOnlyWhenDownXHP <= totalHP;
                if (nextPotionsStep == PotionsStrategyStep.Vigor && !canVigor) return PotionsCommandChoiceResult.Skip;
                if (nextPotionsStep == PotionsStrategyStep.MendWounds && !canMend) return PotionsCommandChoiceResult.Skip;
                if (nextPotionsStep == PotionsStrategyStep.GenericHeal && !canVigor && !canMend) return PotionsCommandChoiceResult.Skip;

                //check inventory for potions
                foreach (int inventoryIndex in GetValidPotionsIndices(nextPotionsStep, _currentEntityInfo, canVigor, canMend))
                {
                    ItemEntity itemEntity = _currentEntityInfo.InventoryItems[inventoryIndex];
                    if (itemEntity.ItemType.HasValue)
                    {
                        string sText = _currentEntityInfo.PickItemTextFromActualIndex(ItemLocationType.Inventory, itemEntity.ItemType.Value, inventoryIndex, false);
                        if (!string.IsNullOrEmpty(sText))
                        {
                            command = "drink " + sText;
                            break;
                        }
                    }
                }

                //check held equipment slot for a potion
                if (!string.IsNullOrEmpty(command))
                {
                    int iHeldSlot = (int)EquipmentSlot.Held;
                    ItemEntity heldItemEntity = _currentEntityInfo.Equipment[iHeldSlot];
                    if (heldItemEntity != null && heldItemEntity.ItemType.HasValue)
                    {
                        ItemTypeEnum eHeldItem = heldItemEntity.ItemType.Value;
                        StaticItemData sid = ItemEntity.StaticItemData[eHeldItem];
                        ValidPotionType potionValidity = GetPotionValidity(sid, nextPotionsStep, canMend, canVigor);
                        if (potionValidity == ValidPotionType.Primary || potionValidity == ValidPotionType.Secondary)
                        {
                            string sText = _currentEntityInfo.PickItemTextFromActualIndex(ItemLocationType.Equipment, eHeldItem, iHeldSlot, true);
                            if (!string.IsNullOrEmpty(sText))
                            {
                                command = "drink " + sText;
                            }
                        }
                    }
                }
            }
            return string.IsNullOrEmpty(command) ? PotionsCommandChoiceResult.Fail : PotionsCommandChoiceResult.Drink;
        }

        private CommandResultObject TraverseExitsAlreadyInBackground(List<Exit> exits, BackgroundWorkerParameters pms, bool beforeGetToTargetRoom, PermRun pr)
        {
            _backgroundProcessPhase = BackgroundProcessPhase.Movement;
            Func<bool> abortLogic;
            if (beforeGetToTargetRoom)
                abortLogic = AbortIfFleeingOrHazying;
            else
                abortLogic = AbortIfHazying;
            Exit previousExit = null;
            List<Exit> exitList = new List<Exit>(exits);
            Room oTarget = exitList[exitList.Count - 1].Target;
            bool needHeal = false;
            bool needCurepoison = false;
            CommandResultObject backgroundCommandResultObject;
            while (exitList.Count > 0)
            {
                Exit nextExit = exitList[0];
                if (previousExit != null && previousExit == nextExit)
                {
                    AddConsoleMessage("Movement recalculation produced the same path.");
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
                Room nextExitTarget = nextExit.Target;
                string exitText = nextExit.ExitText;
                _currentBackgroundExit = nextExit;
                try
                {
                    //for exits that aren't always present, ensure the exit exists
                    ExitPresenceType presenceType = nextExit.PresenceType;
                    BoatExitType boatExitType = nextExit.BoatExitType;
                    if (boatExitType != BoatExitType.None) //wait until the exit is available
                    {
                        double timeRemainingSeconds = GetTimeRemainingSecondsForExit(boatExitType);
                        if (timeRemainingSeconds != 0)
                        {
                            AddConsoleMessage("Waiting " + timeRemainingSeconds.ToString("N1") + " seconds for boat exit.");
                            WaitUntilNextCommandTry(Convert.ToInt32((timeRemainingSeconds + 1) * 1000), BackgroundCommandType.Look);
                            if (abortLogic()) return new CommandResultObject(CommandResult.CommandEscaped);
                            if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);
                        }
                    }
                    else if (presenceType == ExitPresenceType.RequiresSearch) //search until the exit is found
                    {
                        bool foundExit = false;
                        do
                        {
                            _foundSearchedExits = null;
                            backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Search, "search", pms, abortLogic, false);
                            if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                            {
                                if (_foundSearchedExits.Contains(exitText))
                                {
                                    foundExit = true;
                                }
                            }
                            else
                            {
                                return backgroundCommandResultObject;
                            }
                        }
                        while (!foundExit);
                    }

                    if (beforeGetToTargetRoom && _fleeing) return new CommandResultObject(CommandResult.CommandEscaped);
                    if (_hazying) return new CommandResultObject(CommandResult.CommandEscaped);
                    if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);

                    bool useGo;
                    string sExitWord = GetExitWord(nextExit, out useGo);

                    SupportedKeysFlags keyType = nextExit.KeyType;
                    if (keyType != SupportedKeysFlags.None || nextExit.IsUnknownKnockableKeyType)
                    {
                        bool triedPickingUpKey = false;
                        bool exitAvailable = false;
                        backgroundCommandResultObject = null;
                        if (keyType != SupportedKeysFlags.None && pr != null && (((pr.SupportedKeys & keyType) == keyType) || keyType == SupportedKeysFlags.RustyKey))
                        {
                            ItemTypeEnum keyItemType = (ItemTypeEnum)Enum.Parse(typeof(ItemTypeEnum), keyType.ToString());
                            RemoveKeyFromHeldSlot(keyItemType, pms, abortLogic); //can't unlock an exit using a key in the held slot, so remove if there
TryUnlockExit:
                            int numberInInventory;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                numberInInventory = _currentEntityInfo.GetTotalInventoryCount(keyItemType, false, true);
                            }
                            for (int i = 1; i <= numberInInventory; i++)
                            {
                                string sItemText;
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, keyItemType, i, false, false);
                                }
                                if (!string.IsNullOrEmpty(sItemText))
                                {
                                    backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.UnlockExit, "unlock " + sExitWord + " " + sItemText, pms, abortLogic, false);
                                    if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                    {
                                        pr.SupportedKeys |= keyType;
                                        exitAvailable = true;
                                        break;
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                    {
                                        return backgroundCommandResultObject;
                                    }
                                    else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (!exitAvailable && keyType == SupportedKeysFlags.RustyKey && !triedPickingUpKey)
                            {
                                triedPickingUpKey = true;
                                backgroundCommandResultObject = RunSingleCommandForCommandResult(BackgroundCommandType.GetItem, "get rusty", pms, abortLogic, false);
                                if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                                {
                                    goto TryUnlockExit;
                                }
                                else if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                                {
                                    return backgroundCommandResultObject;
                                }
                            }
                        }
                        if (!exitAvailable && !nextExit.RequiresKey()) //knock
                        {
                            backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Knock, "knock " + exitText, pms, abortLogic, false);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResultObject;
                            }
                        }
                    }
                    if (nextExit.RequiresNoItems)
                    {
                        backgroundCommandResultObject = TryDropEverything(pms, abortLogic);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                    }
                    if (nextExit.IsTrapExit || (nextExitTarget != null && nextExitTarget.IsTrapRoom))
                    {
                        backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Prepare, "prepare", pms, abortLogic, false);
                        if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResultObject;
                        }
                    }
                    backgroundCommandResultObject = PreOpenDoorExit(nextExit, sExitWord, pms);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResultObject;
                    }
                    string nextCommand;
                    if (useGo)
                    {
                        nextCommand = "go " + sExitWord;
                    }
                    else
                    {
                        nextCommand = sExitWord;
                    }
                    bool targetIsDamageRoom = nextExitTarget != null && nextExitTarget.DamageType.HasValue;

                    bool keepTryingMovement = true;
                    while (keepTryingMovement)
                    {
                        backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, abortLogic, false);
                        MovementResult eMovementResult = (MovementResult)backgroundCommandResultObject.ResultCode;
                        if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful) //successfully traversed the exit to the new room
                        {
                            exitList.RemoveAt(0);
                            keepTryingMovement = false;
                            PlayerStatusFlags psf = _playerStatusFlags;
                            if ((psf & PlayerStatusFlags.Poisoned) != PlayerStatusFlags.None)
                            {
                                needCurepoison = true;
                            }
                            if (_lastCommandDamage != 0) //trap room
                            {
                                needHeal = true;
                            }
                            if ((psf & PlayerStatusFlags.Prone) != PlayerStatusFlags.None)
                            {
                                backgroundCommandResultObject = TryStand(pms, abortLogic);
                                if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                            }
                        }
                        else if (eMovementResult == MovementResult.MapFailure)
                        {
                            List<Exit> newRoute = CalculateRouteExits(nextExit.Source, oTarget);
                            if (newRoute != null && newRoute.Count > 0)
                            {
                                exitList.Clear();
                                exitList.AddRange(newRoute);
                            }
                            else //couldn't recalculate a new route
                            {
                                return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                            }
                            keepTryingMovement = false;
                        }
                        else if (eMovementResult == MovementResult.StandFailure)
                        {
                            backgroundCommandResultObject = TryStand(pms, abortLogic);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                            keepTryingMovement = true;
                        }
                        else if (eMovementResult == MovementResult.ClosedDoorFailure)
                        {
                            backgroundCommandResultObject = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitText, pms, abortLogic, false);
                            if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
                            {
                                keepTryingMovement = true;
                            }
                            else
                            {
                                return backgroundCommandResultObject;
                            }
                        }
                        else if (eMovementResult == MovementResult.EquipmentFailure)
                        {
                            backgroundCommandResultObject = TryDropEverything(pms, abortLogic);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                            keepTryingMovement = true;
                        }
                        else if (eMovementResult == MovementResult.FallFailure)
                        {
                            backgroundCommandResultObject = GetFullHitpoints(pms, needCurepoison, abortLogic, pr);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResultObject;
                            }
                            backgroundCommandResultObject = TryStand(pms, abortLogic);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
                            keepTryingMovement = true;
                        }
                        else //total failure, abort the background process
                        {
                            keepTryingMovement = false;
                            return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                        }
                    }
                    if (needHeal || needCurepoison)
                    {
                        bool doHealingLogic = !targetIsDamageRoom;
                        if (doHealingLogic)
                        {
                            lock (_currentEntityInfo.EntityLock)
                            {
                                foreach (var nextMob in _currentEntityInfo.CurrentRoomMobs)
                                {
                                    if (MobEntity.StaticMobData[nextMob].Aggressive)
                                    {
                                        doHealingLogic = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (doHealingLogic)
                        {
                            backgroundCommandResultObject = GetFullHitpoints(pms, needCurepoison, abortLogic, pr);
                            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResultObject;
                            }
                            needHeal = false;
                            needCurepoison = false;
                        }
                    }
                }
                finally
                {
                    _currentBackgroundExit = null;
                    _foundSearchedExits = null;
                }
            }
            CommandResultObject ret;
            if (_bwBackgroundProcess.CancellationPending)
                ret = new CommandResultObject(CommandResult.CommandAborted);
            else if (_hazying || _fleeing)
                ret = new CommandResultObject(CommandResult.CommandEscaped);
            else
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
            return ret;
        }

        private CommandResultObject TryDropEverything(BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            CommandResultObject backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, null, "all", pms, abortLogic);
            if (backgroundCommandResultObject.Result != CommandResult.CommandSuccessful) return backgroundCommandResultObject;
            lock (_currentEntityInfo.EntityLock)
            {
                if (_currentEntityInfo.GetTotalKnownEquipmentCount() != 0) //something still equipped, e.g. a cursed piece of equipment
                {
                    return new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
            }
            return TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, null, "all", pms, abortLogic);
        }

        private CommandResultObject TryStand(BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            return RunSingleCommandForCommandResult(BackgroundCommandType.Stand, "stand", pms, abortLogic, false);
        }

        private IEnumerable<int> GetValidPotionsIndices(PotionsStrategyStep nextPotionsStep, CurrentEntityInfo inventoryEquipment, bool canVigor, bool canMend)
        {
            int iIndex = 0;
            List<int> savedIndexes = new List<int>();
            foreach (ItemEntity nextItemEntity in inventoryEquipment.InventoryItems)
            {
                if (nextItemEntity.ItemType.HasValue)
                {
                    StaticItemData sid = ItemEntity.StaticItemData[nextItemEntity.ItemType.Value];
                    ValidPotionType potionValidity = GetPotionValidity(sid, nextPotionsStep, canMend, canVigor);
                    if (potionValidity == ValidPotionType.Primary)
                    {
                        yield return iIndex;
                    }
                    else if (potionValidity == ValidPotionType.Secondary)
                    {
                        savedIndexes.Add(iIndex);
                    }
                }
                iIndex++;
            }
            foreach (int nextIndex in savedIndexes)
            {
                yield return nextIndex;
            }
        }

        private ValidPotionType GetPotionValidity(StaticItemData sid, PotionsStrategyStep nextPotionsStep, bool canMend, bool canVigor)
        {
            ValidPotionType ret = ValidPotionType.Invalid;
            if (sid.ItemClass == ItemClass.Potion)
            {
                SpellsEnum itemSpell = sid.Spell.Value;
                if (nextPotionsStep == PotionsStrategyStep.CurePoison)
                {
                    if (itemSpell == SpellsEnum.curepoison)
                    {
                        ret = ValidPotionType.Primary;
                    }
                }
                else if (nextPotionsStep == PotionsStrategyStep.Vigor)
                {
                    if (itemSpell == SpellsEnum.vigor)
                    {
                        ret = ValidPotionType.Primary;
                    }
                }
                else if (nextPotionsStep == PotionsStrategyStep.MendWounds)
                {
                    if (itemSpell == SpellsEnum.mend)
                    {
                        ret = ValidPotionType.Primary;
                    }
                }
                else if (nextPotionsStep == PotionsStrategyStep.GenericHeal)
                {
                    if (canMend && itemSpell == SpellsEnum.mend)
                    {
                        ret = ValidPotionType.Primary;
                    }
                    if (canVigor && itemSpell == SpellsEnum.vigor)
                    {
                        ret = ValidPotionType.Secondary;
                    }
                }
            }
            return ret;
        }

        public void GetMeleeCommand(MeleeStrategyStep nextMeleeStep, out string command, string mobTarget)
        {
            string sAttackType;
            if (nextMeleeStep == MeleeStrategyStep.PowerAttack)
            {
                sAttackType = "power";
            }
            else if (nextMeleeStep == MeleeStrategyStep.RegularAttack)
            {
                sAttackType = "attack";
            }
            else
            {
                throw new InvalidOperationException();
            }
            command = sAttackType + " " + mobTarget;
        }

        private static string GetStunWandItemText(CurrentEntityInfo cei, SelectedInventoryOrEquipmentItem sioei)
        {
            string sItemText = null;
            lock (cei.EntityLock)
            {
                sItemText = cei.PickItemTextFromItemCounter(ItemLocationType.Inventory, sioei.ItemType.Value, sioei.Counter, false, false);
            }
            return sItemText;
        }

        public static MagicCommandChoiceResult GetMagicCommand(MagicStrategyStep nextMagicStep, int currentHP, int totalHP, int currentMP, out int manaDrain, out BackgroundCommandType? bct, out string command, int usedAutoSpellMin, int usedAutoSpellMax, string mobTarget, IsengardSettingData settingsData, CurrentEntityInfo cei, RealmTypeFlags currentRealm, out RealmTypeFlags? realmToUse, RealmTypeFlags availableRealms, SelectedInventoryOrEquipmentItem wandItemInfo)
        {
            MagicCommandChoiceResult ret = MagicCommandChoiceResult.Cast;
            bool doCast;
            command = null;
            manaDrain = 0;
            bct = null;
            realmToUse = null;
            if (nextMagicStep == MagicStrategyStep.Stun)
            {
                command = "cast stun " + mobTarget;
                manaDrain = 10;
                bct = BackgroundCommandType.Stun;
            }
            else if (nextMagicStep == MagicStrategyStep.StunWand)
            {
                bool canZap = false;
                if (wandItemInfo != null)
                {
                    string sItemText = GetStunWandItemText(cei, wandItemInfo);
                    canZap = !string.IsNullOrEmpty(sItemText);
                    if (canZap)
                    {
                        command = "zap " + sItemText + " " + mobTarget;
                        manaDrain = 0;
                        ret = MagicCommandChoiceResult.Cast;
                        realmToUse = null;
                        bct = BackgroundCommandType.StunWithWand;
                    }
                }
                if (!canZap)
                {
                    manaDrain = 0;
                    bct = null;
                    ret = MagicCommandChoiceResult.OutOfMana;
                    realmToUse = null;
                }
            }
            else if (nextMagicStep == MagicStrategyStep.CurePoison)
            {
                command = Strategy.CAST_CUREPOISON_SPELL;
                manaDrain = 4;
                bct = BackgroundCommandType.CurePoison;
            }
            else if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.Vigor || nextMagicStep == MagicStrategyStep.MendWounds)
            {
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (settingsData.MagicMendOnlyWhenDownXHP > 0)
                        doCast = currentHP + settingsData.MagicMendOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.MendWounds;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (settingsData.MagicVigorOnlyWhenDownXHP > 0)
                        doCast = currentHP + settingsData.MagicVigorOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.Vigor;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    command = Strategy.CAST_MENDWOUNDS_SPELL;
                    manaDrain = 6;
                    bct = BackgroundCommandType.MendWounds;
                }
                else if (nextMagicStep == MagicStrategyStep.Vigor)
                {
                    command = Strategy.CAST_VIGOR_SPELL;
                    manaDrain = 2;
                    bct = BackgroundCommandType.Vigor;
                }
                else
                {
                    ret = MagicCommandChoiceResult.Skip;
                }
            }
            else
            {
                int? iLevel = null;
                SpellsEnum? spellToRun = null;
                RealmTypeFlags? realmTemp = null;
                bool isAuto = nextMagicStep == MagicStrategyStep.OffensiveSpellAuto;
                lock (cei.EntityLock)
                {
                    foreach (RealmTypeFlags nextRealm in IsengardSettingData.GetAvailableRealmsFromStartingPoint(currentRealm, availableRealms))
                    {
                        int? iNextLevel = null;
                        SpellsEnum? nextSpell = null;
                        List<SpellsEnum> offensiveSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(nextRealm);
                        if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel5)
                        {
                            iNextLevel = 5;
                            nextSpell = offensiveSpells[4];
                        }
                        else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel4)
                        {
                            iNextLevel = 4;
                            nextSpell = offensiveSpells[3];
                        }
                        else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel3)
                        {
                            iNextLevel = 3;
                            nextSpell = offensiveSpells[2];
                        }
                        else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel2)
                        {
                            iNextLevel = 2;
                            nextSpell = offensiveSpells[1];
                        }
                        else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel1)
                        {
                            iNextLevel = 1;
                            nextSpell = offensiveSpells[0];
                        }
                        else //auto
                        {
                            if (currentMP >= 20 && usedAutoSpellMin <= 5 && usedAutoSpellMax >= 5 && cei.CanCast(offensiveSpells[4]))
                            {
                                iNextLevel = 5;
                                nextSpell = offensiveSpells[4];
                            }
                            if (currentMP >= 15 && usedAutoSpellMin <= 4 && usedAutoSpellMax >= 4 && cei.CanCast(offensiveSpells[3]))
                            {
                                iNextLevel = 4;
                                nextSpell = offensiveSpells[3];
                            }
                            else if (currentMP >= 10 && usedAutoSpellMin <= 3 && usedAutoSpellMax >= 3 && cei.CanCast(offensiveSpells[2]))
                            {
                                iNextLevel = 3;
                                nextSpell = offensiveSpells[2];
                            }
                            else if (currentMP >= 7 && usedAutoSpellMin <= 2 && usedAutoSpellMax >= 2 && cei.CanCast(offensiveSpells[1]))
                            {
                                iNextLevel = 2;
                                nextSpell = offensiveSpells[1];
                            }
                            else if (currentMP >= 3 && usedAutoSpellMin <= 1 && usedAutoSpellMax >= 1 && cei.CanCast(offensiveSpells[0]))
                            {
                                iNextLevel = 1;
                                nextSpell = offensiveSpells[0];
                            }
                        }
                        if (isAuto)
                        {
                            if (iNextLevel.HasValue && (!iLevel.HasValue || iNextLevel.Value > iLevel.Value))
                            {
                                iLevel = iNextLevel.Value;
                                realmTemp = nextRealm;
                                spellToRun = nextSpell.Value;
                            }
                        }
                        else //specific level offensive spell
                        {
                            if (nextSpell.HasValue && cei.CanCast(nextSpell.Value))
                            {
                                realmTemp = nextRealm;
                                spellToRun = nextSpell.Value;
                                break;
                            }
                        }
                    }
                }
                if (spellToRun.HasValue)
                {
                    SpellsEnum eSpell = spellToRun.Value;
                    SpellInformationAttribute sia = SpellsStatic.SpellsByEnum[eSpell];
                    manaDrain = sia.Mana;
                    command = "cast " + sia.SpellName + " " + mobTarget;
                    bct = BackgroundCommandType.OffensiveSpell;
                    realmToUse = realmTemp;
                }
                else
                {
                    manaDrain = 0;
                    bct = null;
                    ret = MagicCommandChoiceResult.OutOfMana;
                    realmToUse = null;
                }
            }
            if (manaDrain > 0 && manaDrain > currentMP)
            {
                manaDrain = 0;
                bct = null;
                ret = MagicCommandChoiceResult.OutOfMana;
                realmToUse = null;
            }
            return ret;
        }

        private bool SelectMobAfterKillMonster(AfterKillMonsterAction onMonsterKilledAction, BackgroundWorkerParameters bwp, out Strategy resetStrategy)
        {
            resetStrategy = null;
            if (_monsterKilled && (onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoom || onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoomOfSameType))
            {
                bwp.MonsterKilled = true;
                bwp.MonsterKilledType = _monsterKilledType;
                lock (_currentEntityInfo.EntityLock)
                {
                    MobTypeEnum monsterType = MobTypeEnum.LittleMouse;
                    if (onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoom)
                    {
                        MobTypeEnum? firstAttackableMob = _currentEntityInfo.GetFirstAttackableMob();
                        if (firstAttackableMob.HasValue)
                        {
                            monsterType = firstAttackableMob.Value;
                            if (_settingsData.DynamicMobData.TryGetValue(monsterType, out DynamicMobData dmd))
                            {
                                if (dmd.HasData() && (bwp.Strategy != null || dmd.Strategy != null))
                                {
                                    resetStrategy = new Strategy(bwp.Strategy, dmd);
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MobTypeEnum? monsterKilledType = _monsterKilledType;
                        if (!monsterKilledType.HasValue) return false;
                        monsterType = monsterKilledType.Value;
                        int index = _currentEntityInfo.CurrentRoomMobs.IndexOf(monsterKilledType.Value);
                        if (index < 0) return false;
                    }
                    bwp.MobText = string.Empty;
                    bwp.MobTextCounter = 0;
                    bwp.MobType = monsterType;
                    bwp.MobTypeCounter = 1;
                    string sMobText = monsterType.ToString();
                    _currentlyFightingMobText = string.Empty;
                    _currentlyFightingMobType = monsterType;
                    _currentlyFightingMobCounter = 1;
                    _mob = sMobText;
                    _monsterKilled = false;
                    _monsterKilledType = null;
                    _currentMonsterStatus = MonsterStatus.None;
                    _monsterStunnedSince = null;
                }
            }
            return true;
        }

        private CommandResultObject RemoveWeaponBeforeFlee(BackgroundWorkerParameters pms)
        {
            foreach (EquipmentSlot nextWeaponSlot in new EquipmentSlot[] { EquipmentSlot.Weapon1, EquipmentSlot.Weapon2})
            {
                string sWeaponText = string.Empty;
                ItemTypeEnum weaponValue = ItemTypeEnum.GoldCoins;
                lock (_currentEntityInfo.EntityLock)
                {
                    int iIndex = (int)nextWeaponSlot;
                    ItemEntity weaponEntity = _currentEntityInfo.Equipment[iIndex];
                    if (weaponEntity != null && weaponEntity.ItemType.HasValue)
                    {
                        weaponValue = weaponEntity.ItemType.Value;
                        sWeaponText = _currentEntityInfo.PickItemTextFromActualIndex(ItemLocationType.Equipment, weaponValue, iIndex, false);
                        if (string.IsNullOrEmpty(sWeaponText))
                        {
                            continue; //can't remove the weapon, but as we are fleeing don't fail for this. Should never happen but don't fail just in case.
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sWeaponText))
                {
                    CommandResultObject backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, weaponValue, sWeaponText, pms, AbortIfHazying);
                    if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
                    {
                        return backgroundCommandResultObject;
                    }
                    //continue in case of either success or failure. Even on a failure continue with the logic, since we still want to
                    //flee even if the weapon has to be dropped.
                }
            }
            return new CommandResultObject(CommandResult.CommandSuccessful);
        }

        /// <summary>
        /// equips an item
        /// </summary>
        /// <param name="item">item to equip</param>
        /// <param name="slot">equipment slot</param>
        /// <param name="commandType">command type</param>
        /// <param name="pms">background worker parameters</param>
        /// <param name="isBeforeCombat">true if before combat, false if during combat</param>
        /// <param name="failedToEquip">set to true if failed to equip after combat started</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject EquipSingleItem(ItemTypeEnum? item, EquipmentSlot slot, BackgroundCommandType commandType, BackgroundWorkerParameters pms, bool isBeforeCombat, ref bool failedToEquip)
        {
            CommandResultObject backgroundCommandResultObject;
            if (item.HasValue)
            {
                List<string> itemTexts = new List<string>();
                ItemTypeEnum itemValue = item.Value;
                lock (_currentEntityInfo.EntityLock)
                {
                    if (_currentEntityInfo.Equipment[(int)slot] == null)
                    {
                        if (_currentEntityInfo.InventoryContainsItemType(itemValue))
                        {
                            int iCounter = 0;
                            for (int i = 0; i < _currentEntityInfo.InventoryItems.Count; i++)
                            {
                                ItemEntity ie = _currentEntityInfo.InventoryItems[i];
                                if (ie.ItemType == itemValue)
                                {
                                    iCounter++;
                                    string itemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, itemValue, iCounter, false, false);
                                    if (!string.IsNullOrEmpty(itemText))
                                    {
                                        itemTexts.Add(itemText);
                                    }
                                }
                            }
                        }
                    }
                    else //already equipping something in that slot
                    {
                        //we could remove the existing item and equip the correct item (if different), but for the moment leave things as is
                        //and let the combat continue.
                        return new CommandResultObject(CommandResult.CommandSuccessful);
                    }
                }
                foreach (string sNextWeaponText in itemTexts)
                {
                    backgroundCommandResultObject = TryCommandAddingOrRemovingFromInventory(commandType, itemValue, sNextWeaponText, pms, AbortIfFleeingOrHazying);
                    if (backgroundCommandResultObject.Result != CommandResult.CommandUnsuccessfulAlways)
                    {
                        return backgroundCommandResultObject;
                    }
                }
                if (isBeforeCombat)
                {
                    //if combat hasn't started yet return failure and let the user take the next action
                    backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandUnsuccessfulAlways);
                }
                else
                {
                    failedToEquip = true;
                    AddConsoleMessage($"Unable to equip {itemValue}, continuing combat.");

                    //if combat has started we don't want to fail and stop combat. Current behavior is to display a message to the user and
                    //make them responsible for handling it appropriately
                    backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandSuccessful);
                }
            }
            else //nothing to wield
            {
                backgroundCommandResultObject = new CommandResultObject(CommandResult.CommandSuccessful);
            }
            return backgroundCommandResultObject;
        }

        private bool FleeExitDiscriminator(Exit exit)
        {
            return !exit.Hidden && (exit.Source == null || !exit.Source.NoFlee);
        }

        private bool BreakOutOfBackgroundCombat(AfterKillMonsterAction afterKillMonsterAction)
        {
            bool ret;
            if (afterKillMonsterAction == AfterKillMonsterAction.StopCombat && _monsterKilled)
                ret = true;
            else if (_fleeing)
                ret = true;
            else if (_hazying)
                ret = true;
            else if (_bwBackgroundProcess.CancellationPending)
                ret = true;
            else
            {
                Room r = _currentEntityInfo.CurrentRoom;
                if (r != null && r.Intangible)
                    ret = true;
                else
                    ret = false;
            }
            return ret;
        }

        private CommandResultObject RunBackgroundMeleeStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<MeleeStrategyStep> meleeSteps, ref bool meleeStepsFinished, ref MeleeStrategyStep? nextMeleeStep, ref DateTime? dtNextMeleeCommand, ref bool didDamage)
        {
            CommandResultObject backgroundCommandResultObject = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
            {
                //do nothing
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
            {
                meleeStepsFinished = true;
                _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                nextMeleeStep = null;
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandMustWait)
            {
                int waitMS = GetWaitMilliseconds(_waitSeconds);
                if (waitMS > 0)
                {
                    dtNextMeleeCommand = DateTime.UtcNow.AddMilliseconds(waitMS);
                }
                else
                {
                    dtNextMeleeCommand = null;
                }
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful) //attack was carried out (hit or miss)
            {
                _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                if (_lastCommandDamage != 0)
                {
                    didDamage = true;
                }
                if (meleeStepsFinished)
                {
                    nextMeleeStep = null;
                }
                else if (meleeSteps != null && meleeSteps.MoveNext())
                {
                    nextMeleeStep = meleeSteps.Current;
                    dtNextMeleeCommand = null;
                }
                else //no more steps
                {
                    nextMeleeStep = null;
                    meleeStepsFinished = true;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            return backgroundCommandResultObject;
        }

        private CommandResultObject RunBackgroundPotionsStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<PotionsStrategyStep> potionsSteps, ref bool potionsStepsFinished, ref PotionsStrategyStep? nextPotionsStep, ref DateTime? dtNextPotionsCommand)
        {
            CommandResultObject backgroundCommandResultObject = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped)
            {
                //do nothing
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
            {
                potionsStepsFinished = true;
                _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                nextPotionsStep = null;
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandMustWait)
            {
                int waitMS = GetWaitMilliseconds(_waitSeconds);
                if (waitMS > 0)
                {
                    dtNextPotionsCommand = DateTime.UtcNow.AddMilliseconds(waitMS);
                }
                else
                {
                    dtNextPotionsCommand = null;
                }
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful)
            {
                _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                if (potionsStepsFinished)
                {
                    nextPotionsStep = null;
                }
                else if (potionsSteps != null && potionsSteps.MoveNext())
                {
                    nextPotionsStep = potionsSteps.Current;
                    dtNextPotionsCommand = null;
                }
                else //no more steps
                {
                    nextPotionsStep = null;
                    potionsStepsFinished = true;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            return backgroundCommandResultObject;
        }

        private CommandResultObject RunBackgroundMagicStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, bool useManaPool, int manaDrain, IEnumerator<MagicStrategyStep> magicSteps, ref bool magicStepsFinished, ref MagicStrategyStep? nextMagicStep, ref DateTime? dtNextMagicCommand, ref bool didDamage)
        {
            CommandResultObject backgroundCommandResultObject = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResultObject.Result == CommandResult.CommandAborted || backgroundCommandResultObject.Result == CommandResult.CommandTimeout || backgroundCommandResultObject.Result == CommandResult.CommandEscaped || backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulThisTime)
            {
                //do nothing
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandUnsuccessfulAlways)
            {
                magicStepsFinished = true;
                _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                nextMagicStep = null;
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandMustWait)
            {
                int waitMS = GetWaitMilliseconds(_waitSeconds);
                if (waitMS > 0)
                {
                    dtNextMagicCommand = DateTime.UtcNow.AddMilliseconds(waitMS);
                }
                else
                {
                    dtNextMagicCommand = null;
                }
            }
            else if (backgroundCommandResultObject.Result == CommandResult.CommandSuccessful) //spell was cast
            {
                _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                if (nextMagicStep.Value == MagicStrategyStep.OffensiveSpellAuto ||
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel1 ||
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel2 ||
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel3 ||
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel4 ||
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel5)
                {
                    didDamage = true;
                }
                if (useManaPool)
                {
                    _currentMana -= manaDrain;
                }
                if (magicStepsFinished)
                {
                    nextMagicStep = null;
                }
                else if (magicSteps != null && magicSteps.MoveNext())
                {
                    nextMagicStep = magicSteps.Current;
                    dtNextMagicCommand = null;
                }
                else //no more steps
                {
                    nextMagicStep = null;
                    magicStepsFinished = true;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            return backgroundCommandResultObject;
        }

        private void CheckForQueuedMagicStep(BackgroundWorkerParameters pms, ref MagicStrategyStep? nextMagicStep)
        {
            MagicStrategyStep? queuedMagicStep;
            if (!nextMagicStep.HasValue)
            {
                lock (_queuedCommandLock)
                {
                    queuedMagicStep = pms.QueuedMagicStep;
                    if (queuedMagicStep.HasValue)
                    {
                        nextMagicStep = queuedMagicStep;
                        pms.QueuedMagicStep = null;
                    }
                }
            }
        }

        private void CheckForQueuedPotionsStep(BackgroundWorkerParameters pms, ref PotionsStrategyStep? nextPotionsStep)
        {
            PotionsStrategyStep? queuedPotionsStep;
            if (!nextPotionsStep.HasValue)
            {
                lock (_queuedCommandLock)
                {
                    queuedPotionsStep = pms.QueuedPotionsStep;
                    if (queuedPotionsStep.HasValue)
                    {
                        nextPotionsStep = queuedPotionsStep;
                        pms.QueuedPotionsStep = null;
                    }
                }
            }
        }

        private string GetExitWord(Exit exit, out bool useGo)
        {
            useGo = false;
            string target = exit.ExitText.ToLower().Trim();
            string ret = null;
            if (!exit.ForceGo)
            {
                switch (target)
                {
                    case "north":
                    case "northeast":
                    case "northwest":
                    case "west":
                    case "east":
                    case "south":
                    case "southeast":
                    case "southwest":
                    case "up":
                    case "down":
                    case "out":
                        ret = target;
                        break;
                    case "n":
                        ret = "north";
                        break;
                    case "ne":
                        ret = "northeast";
                        break;
                    case "nw":
                        ret = "northwest";
                        break;
                    case "w":
                        ret = "west";
                        break;
                    case "e":
                        ret = "east";
                        break;
                    case "sw":
                        ret = "southwest";
                        break;
                    case "s":
                        ret = "south";
                        break;
                    case "se":
                        ret = "southeast";
                        break;
                    case "u":
                        ret = "up";
                        break;
                    case "d":
                        ret = "down";
                        break;
                }
            }
            var enumerator = StringProcessing.PickWords(target).GetEnumerator();
            enumerator.MoveNext();
            string sWord = enumerator.Current;
            if (ret == null)
            {
                Room oSource = exit.Source;
                if (oSource != null)
                {
                    int iCounter = 0;
                    bool foundExactMatch = false;
                    foreach (Exit nextExit in oSource.Exits)
                    {
                        string sNextExitText = nextExit.ExitText;
                        if (sNextExitText.StartsWith(sWord))
                        {
                            iCounter++;
                            if (sNextExitText == sWord)
                            {
                                foundExactMatch = true;
                                break;
                            }
                        }
                    }
                    if (foundExactMatch && iCounter > 1)
                    {
                        useGo = true;
                        ret = sWord + " " + iCounter;
                    }
                }
            }
            if (ret == null)
            {
                useGo = true;
                ret = sWord;
            }
            return ret;
        }

        private CommandResultObject PreOpenDoorExit(Exit exit, string exitWord, BackgroundWorkerParameters pms)
        {
            CommandResultObject ret;
            if (exit.MustOpen)
            {
                ret = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitWord, pms, AbortIfFleeingOrHazying, false);
            }
            else //not a door exit
            {
                ret = new CommandResultObject(CommandResult.CommandSuccessful);
            }
            return ret;
        }

        private CommandResultObject CastSpellOnSelf(SpellsEnum spell, BackgroundWorkerParameters bwp, Func<bool> abortLogic)
        {
            BackgroundCommandType bct;
            switch (spell)
            {
                case SpellsEnum.vigor:
                    bct = BackgroundCommandType.Vigor;
                    break;
                case SpellsEnum.mend:
                    bct = BackgroundCommandType.MendWounds;
                    break;
                case SpellsEnum.curepoison:
                    bct = BackgroundCommandType.CurePoison;
                    break;
                case SpellsEnum.bless:
                    bct = BackgroundCommandType.Bless;
                    break;
                case SpellsEnum.protection:
                    bct = BackgroundCommandType.Protection;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            string spellName = SpellsStatic.SpellsByEnum[spell].SpellName;
            return RunSingleCommand(bct, "cast " + spellName, bwp, abortLogic, false);
        }

        private void WaitUntilNextCommandTry(int remainingMS, BackgroundCommandType commandType)
        {
            DateTime dtWaitUntilUTC = DateTime.UtcNow.AddMilliseconds(remainingMS);
            bool hazying = commandType == BackgroundCommandType.DrinkHazy;
            bool fleeing = commandType == BackgroundCommandType.Flee;
            DateTime dtUTCCurrent = DateTime.UtcNow;
            while (dtUTCCurrent < dtWaitUntilUTC)
            {
                TimeSpan tsRemaining = dtWaitUntilUTC - dtUTCCurrent;
                int nextWaitMS = Math.Min(Convert.ToInt32(tsRemaining.TotalMilliseconds), 100);

                //check if the wait should be aborted
                if (hazying)
                {
                    if (!_hazying) break;
                }
                else if (fleeing)
                {
                    if (!_fleeing || _hazying) break;
                }
                else
                {
                    if (_fleeing || _hazying) break;
                }
                if (_bwBackgroundProcess.CancellationPending) break;

                Thread.Sleep(nextWaitMS);

                //check if the wait should be aborted
                if (hazying)
                {
                    if (!_hazying) break;
                }
                else if (fleeing)
                {
                    if (!_fleeing || _hazying) break;
                }
                else
                {
                    if (_fleeing || _hazying) break;
                }
                if (_bwBackgroundProcess.CancellationPending) break;

                remainingMS -= nextWaitMS;
                RunQueuedCommandWhenBackgroundProcessRunning(_currentBackgroundParameters);

                //check if the wait should be aborted
                if (hazying)
                {
                    if (!_hazying) break;
                }
                else if (fleeing)
                {
                    if (!_fleeing || _hazying) break;
                }
                else
                {
                    if (_fleeing || _hazying) break;
                }
                if (_bwBackgroundProcess.CancellationPending) break;
                dtUTCCurrent = DateTime.UtcNow;
            }
        }

        private void RunQueuedCommandWhenBackgroundProcessRunning(BackgroundWorkerParameters pms)
        {
            List<string> queuedCommands = null;
            lock (_queuedCommandLock)
            {
                if (pms.QueuedCommands.Count > 0)
                {
                    queuedCommands = new List<string>(pms.QueuedCommands);
                }
                pms.QueuedCommands.Clear();
            }
            if (queuedCommands != null)
            {
                foreach (string next in queuedCommands)
                {
                    SendCommand(next, InputEchoType.On);
                }
            }
        }

        private bool AbortIfFleeingOrHazying()
        {
            return _fleeing || _hazying;
        }

        private bool AbortIfHazying()
        {
            return _hazying;
        }

        private CommandResultObject RunSingleCommandForCommandResult(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden)
        {
            _backgroundCommandType = commandType;
            try
            {
                bool allowAbort = commandType != BackgroundCommandType.Movement;
                return RunSingleCommandForCommandResultBase(commandType, command, pms, abortLogic, hidden);
            }
            finally
            {
                _backgroundCommandType = null;
            }
        }

        private CommandResultObject RunSingleCommandForCommandResultBase(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden)
        {
            //quitting and commands that could cause room transition can't be aborted if sent to the server
            bool allowAbort = commandType != BackgroundCommandType.Look && commandType != BackgroundCommandType.Movement && commandType != BackgroundCommandType.Flee && commandType != BackgroundCommandType.DrinkHazy && commandType != BackgroundCommandType.Quit;
            DateTime utcTimeoutPoint = DateTime.UtcNow.AddSeconds(_settingsData.CommandTimeoutSeconds);
            _commandResult = null;
            _commandSpecificResult = 0;
            _lastCommand = null;
            _lastCommandDamage = 0;
            _runningHiddenCommand = hidden;
            try
            {
                _lastCommand = command;
                SendCommand(command, hidden || _settingsData.ConsoleVerbosity == ConsoleOutputVerbosity.Minimum ? InputEchoType.Off : InputEchoType.On);
                CommandResult? currentResult = null;
                int specificResult = 0;
                while (!currentResult.HasValue)
                {
                    RunQueuedCommandWhenBackgroundProcessRunning(pms);
                    bool doSleep = true;
                    if (allowAbort)
                    {
                        if (_bwBackgroundProcess.CancellationPending)
                        {
                            currentResult = CommandResult.CommandAborted;
                            specificResult = 0;
                            doSleep = false;
                        }
                        else if (abortLogic != null && abortLogic())
                        {
                            currentResult = CommandResult.CommandEscaped;
                            specificResult = 0;
                            doSleep = false;
                        }
                        else if (DateTime.UtcNow >= utcTimeoutPoint)
                        {
                            AddConsoleMessage("Command timeout occurred for " + command);
                            currentResult = CommandResult.CommandTimeout;
                            specificResult = 0;
                            doSleep = false;
                        }
                    }
                    if (doSleep)
                    {
                        Thread.Sleep(50);
                        currentResult = _commandResult;
                        specificResult = _commandSpecificResult;
                    }
                }
                return new CommandResultObject(currentResult.Value, specificResult);
            }
            finally
            {
                _commandResult = null;
                _commandSpecificResult = 0;
                _commandInventoryItem = null;
                _lastCommand = null;
                _runningHiddenCommand = false;
            }
        }

        private void AddConsoleMessage(string Message)
        {
            AddConsoleMessages(new List<string>() { Message });
        }

        private void AddConsoleMessages(List<string> Messages)
        {
            lock (_newConsoleText)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in Messages)
                {
                    sb.Append(s);
                    sb.Append(Environment.NewLine);
                }
                sb.Append(": ");
                _newConsoleText.Add(sb.ToString());
            }
        }

        /// <summary>
        /// runs a single command, repeating the attempt if waiting is needed or the command is unreliable
        /// </summary>
        /// <param name="commandType">command type</param>
        /// <param name="command">command to run</param>
        /// <param name="pms">background worker parameters</param>
        /// <param name="abortLogic">abort logic</param>
        /// <param name="hidden">whether the output of the command should be hidden</param>
        /// <returns>result of the operation</returns>
        private CommandResultObject RunSingleCommand(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden)
        {
            int currentAttempts = 0;
            _backgroundCommandType = commandType;
            try
            {
                CommandResultObject resultObject = null;
                while (currentAttempts < MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND)
                {
                    if (_bwBackgroundProcess.CancellationPending) return new CommandResultObject(CommandResult.CommandAborted);
                    if (abortLogic != null && abortLogic()) return new CommandResultObject(CommandResult.CommandEscaped);
                    currentAttempts++;
                    resultObject = RunSingleCommandForCommandResultBase(commandType, command, pms, abortLogic, hidden);
                    CommandResult resultValue = resultObject.Result;
                    if (resultValue == CommandResult.CommandSuccessful || resultValue == CommandResult.CommandUnsuccessfulAlways || resultValue == CommandResult.CommandAborted || resultValue == CommandResult.CommandTimeout || resultValue == CommandResult.CommandEscaped)
                    {
                        return resultObject;
                    }
                    else if (resultValue == CommandResult.CommandMustWait)
                    {
                        int waitMS = GetWaitMilliseconds(_waitSeconds);
                        if (waitMS > 0)
                        {
                            WaitUntilNextCommandTry(waitMS, commandType);
                        }
                    }
                    else if (resultValue != CommandResult.CommandUnsuccessfulThisTime)
                    {
                        throw new InvalidOperationException();
                    }
                }
                //in theory we should never get here for command must wait since once the time period elapses
                //the command should run. we can get here for an unsuccessful this time command failing up to the
                //max attempt count. So change the command result to unsuccessful always and return that.
                resultObject.Result = CommandResult.CommandUnsuccessfulAlways;
                return resultObject;
            }
            finally
            {
                _backgroundCommandType = null;
            }
        }

        private static int GetWaitMilliseconds(int secondsRemaining)
        {
            int ret;
            if (secondsRemaining == 1)
                ret = 0;
            else
                ret = 400 + (1000 * (secondsRemaining - 2));
            return ret;
        }

        private void ToggleBackgroundProcessUI(BackgroundWorkerParameters bwp, bool inBackground)
        {
            bool enabled = !inBackground;
            foreach (Control ctl in GetControlsToDisableForBackgroundProcess())
            {
                ctl.Enabled = enabled;
            }
            foreach (ToolStripButton tsb in GetToolStripButtonsToDisableForBackgroundProcess())
            {
                tsb.Enabled = enabled;
            }
            foreach (ToolStripMenuItem tsmi in GetToolStripMenuItemsToDisableForBackgroundProcess())
            {
                tsmi.Enabled = enabled;
            }
            foreach (ToolStripDropDownButton tsdd in GetToolStripDropDownsToDisableForBackgroundProcess())
            {
                tsdd.Enabled = enabled;
            }
            btnAbort.Enabled = inBackground;
            EnableDisableActionButtons(bwp);
            RefreshCurrentAreaButtons(inBackground);
        }

        private IEnumerable<ToolStripDropDownButton> GetToolStripDropDownsToDisableForBackgroundProcess()
        {
            yield return tsddActions;
        }

        private IEnumerable<ToolStripButton> GetToolStripButtonsToDisableForBackgroundProcess()
        {
            yield return tsbReloadMap;
            yield return tsbQuit;
            yield return tsbLogout;
        }

        private IEnumerable<ToolStripMenuItem> GetToolStripMenuItemsToDisableForBackgroundProcess()
        {
            yield return tsmiEditSettings;
            yield return tsmiImportXML;
            yield return tsmiSaveSettings;
            yield return tsmiImportFromPlayer;
            yield return tsmiQuitWithoutSaving;
            yield return tsmiRestoreDefaults;
        }

        private IEnumerable<Control> GetControlsToDisableForBackgroundProcess()
        {
            yield return txtMob;
            yield return txtWand;
            yield return cboArea;
            yield return btnNorthwest;
            yield return btnNorth;
            yield return btnNortheast;
            yield return btnWest;
            yield return btnEast;
            yield return btnSouthwest;
            yield return btnSouth;
            yield return btnSoutheast;
            yield return btnUp;
            yield return btnDn;
            yield return btnOut;
            yield return btnOtherSingleMove;
            yield return btnSet;
            yield return btnLook;
            yield return btnNonCombatPermRun;
            yield return btnAdHocPermRun;
            yield return btnFightOne;
            yield return btnFightAll;
        }

        private void EnableDisableActionButtons(BackgroundWorkerParameters bwp)
        {
            BackgroundProcessPhase npp = _backgroundProcessPhase;
            bool inForeground = bwp == null || npp == BackgroundProcessPhase.None;
            CommandType eRunningCombatCommandTypes = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            if (!inForeground)
            {
                Strategy strategy = _currentBackgroundParameters.Strategy;
                if (strategy != null) eRunningCombatCommandTypes = strategy.CombatCommandTypes;
            }
            ToolStripButton tsb = null;
            Button btn = null;
            bool enabled;
            bool hasMobTarget;
            if (_currentlyFightingMobType.HasValue || !string.IsNullOrEmpty(_currentlyFightingMobText))
                hasMobTarget = true;
            else
                hasMobTarget = !string.IsNullOrEmpty(_mob);
            IsengardSettingData settingsData = _settingsData;
            bool haveSettings = settingsData != null;
            List<SpellsEnum> knownSpells = new List<SpellsEnum>();
            SpellsEnum? level1Spell = null;
            SpellsEnum? level2Spell = null;
            SpellsEnum? level3Spell = null;
            lock (_currentEntityInfo.EntityLock)
            {
                knownSpells.AddRange(_currentEntityInfo.SpellsKnown);
                if (haveSettings)
                {
                    RealmTypeFlags currentRealms = settingsData.Realms;
                    foreach (RealmTypeFlags nextRealm in Enum.GetValues(typeof(RealmTypeFlags)))
                    {
                        if (nextRealm != RealmTypeFlags.None && nextRealm != RealmTypeFlags.All && ((currentRealms & nextRealm) != RealmTypeFlags.None))
                        {
                            SpellsEnum nextSpell;
                            List<SpellsEnum> realmSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(nextRealm);
                            if (!level1Spell.HasValue)
                            {
                                nextSpell = realmSpells[0];
                                if (_currentEntityInfo.CanCast(nextSpell)) level1Spell = nextSpell;
                            }
                            if (!level2Spell.HasValue)
                            {
                                nextSpell = realmSpells[1];
                                if (_currentEntityInfo.CanCast(nextSpell)) level2Spell = nextSpell;
                            }
                            if (!level3Spell.HasValue)
                            {
                                nextSpell = realmSpells[2];
                                if (_currentEntityInfo.CanCast(nextSpell)) level3Spell = nextSpell;
                            }
                        }
                    }
                }
            }
            foreach (CommandButtonTag oTag in GetButtonsForEnablingDisabling())
            {
                object oControl = oTag.Control;
                if ((oTag.ObjectType & DependentObjectType.Mob) != DependentObjectType.None && !hasMobTarget)
                    enabled = false;
                else if ((oTag.ObjectType & DependentObjectType.Wand) != DependentObjectType.None && string.IsNullOrEmpty(_wand))
                    enabled = false;
                else if (inForeground)
                    enabled = true;
                else if (oTag.CommandType == CommandType.None) //these buttons can be clicked even if a background process is running
                    enabled = true;
                else if (npp != BackgroundProcessPhase.Combat) //combat buttons are only enabled in combat
                    enabled = false;
                else //combat buttons are only enabled if the strategy isn't doing that kind of combat
                    enabled = (oTag.CommandType & eRunningCombatCommandTypes) == CommandType.None;

                if (enabled)
                {
                    SpellsEnum? eSpell = null;
                    if (oControl == btnLevel1OffensiveSpell)
                        enabled = level1Spell.HasValue;
                    else if (oControl == btnLevel2OffensiveSpell)
                        enabled = level2Spell.HasValue;
                    else if (oControl == btnLevel3OffensiveSpell)
                        enabled = level3Spell.HasValue;
                    else if (oControl == btnStunMob)
                        eSpell = SpellsEnum.stun;
                    else if (oControl == btnCastVigor)
                        eSpell = SpellsEnum.vigor;
                    else if (oControl == btnCastMend)
                        eSpell = SpellsEnum.mend;
                    else if (oControl == btnCastCurePoison)
                        eSpell = SpellsEnum.curepoison;
                    if (eSpell.HasValue)
                    {
                        lock (_currentEntityInfo.EntityLock)
                        {
                            enabled = _currentEntityInfo.CanCast(eSpell.Value);
                        }
                    }
                }

                bool isToolStripButton = oTag.IsToolStripButton;
                bool isEnabled;
                if (isToolStripButton)
                {
                    tsb = (ToolStripButton)oControl;
                    isEnabled = tsb.Enabled;
                }
                else
                {
                    btn = (Button)oControl;
                    isEnabled = btn.Enabled;
                }
                if (enabled != isEnabled)
                {
                    if (isToolStripButton)
                        tsb.Enabled = enabled;
                    else
                        btn.Enabled = enabled;
                }
            }

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Flee || npp == BackgroundProcessPhase.Hazy || npp == BackgroundProcessPhase.Score)
                enabled = false;
            else
                enabled = true;
            if (enabled != btnFlee.Enabled)
                btnFlee.Enabled = enabled;

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Hazy || npp == BackgroundProcessPhase.Score)
                enabled = false;
            else
                enabled = true;
            if (enabled != btnDrinkHazy.Enabled)
                btnDrinkHazy.Enabled = enabled;

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Score)
                enabled = false;
            else
                enabled = true;
            if (enabled != tsbScore.Enabled)
                tsbScore.Enabled = enabled;

            PermRun currentPermRun = _currentPermRun;
            PermRun nextPermRun = _nextPermRun;
            
            if (inForeground)
                enabled = currentPermRun != null;
            else
                enabled = false;
            btnRemoveCurrentPermRun.Enabled = enabled;
            btnCompleteCurrentPermRun.Enabled = enabled;
            btnResumeCurrentPermRun.Enabled = enabled;

            btnRemoveNextPermRun.Enabled = nextPermRun != null;
        }

        private IEnumerable<CommandButtonTag> GetButtonsForEnablingDisabling()
        {
            yield return (CommandButtonTag)btnLevel1OffensiveSpell.Tag;
            yield return (CommandButtonTag)btnLevel2OffensiveSpell.Tag;
            yield return (CommandButtonTag)btnLevel3OffensiveSpell.Tag;
            yield return (CommandButtonTag)btnLookAtMob.Tag;
            yield return (CommandButtonTag)btnCastVigor.Tag;
            yield return (CommandButtonTag)btnCastCurePoison.Tag;
            yield return (CommandButtonTag)btnAttackMob.Tag;
            yield return (CommandButtonTag)btnDrinkVigor.Tag;
            yield return (CommandButtonTag)btnDrinkCurepoison.Tag;
            yield return (CommandButtonTag)btnUseWandOnMob.Tag;
            yield return (CommandButtonTag)btnPowerAttackMob.Tag;
            yield return (CommandButtonTag)btnCastMend.Tag;
            yield return (CommandButtonTag)btnDrinkMend.Tag;
            yield return (CommandButtonTag)btnStunMob.Tag;
            yield return (CommandButtonTag)tsbTime.Tag;
            yield return (CommandButtonTag)tsbInformation.Tag;
            yield return (CommandButtonTag)tsbInventoryAndEquipment.Tag;
            yield return (CommandButtonTag)tsbRemoveAll.Tag;
            yield return (CommandButtonTag)tsbWearAll.Tag;
            yield return (CommandButtonTag)tsbWho.Tag;
            yield return (CommandButtonTag)tsbUptime.Tag;
        }

        private void SendCommand(string command, InputEchoType echoType)
        {
            List<int> keys = new List<int>();
            foreach (char c in command)
            {
                if (_asciiMapping.TryGetValue(c, out int i))
                {
                    keys.Add(i);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            List<byte> bytesToWrite = new List<byte>();
            for (int i = 0; i < keys.Count; i++)
            {
                bytesToWrite.Add((byte)keys[i]);
                Thread.Sleep(1);
            }
            bytesToWrite.Add(13);
            bytesToWrite.Add(10);
            lock (_writeToNetworkStreamLock)
            {
                _tcpClientNetworkStream.Write(bytesToWrite.ToArray(), 0, bytesToWrite.Count);
            }
            bool displayToConsole = !string.IsNullOrEmpty(command) && echoType != InputEchoType.Off;
            bool displayToFullLog = !string.IsNullOrEmpty(command) && _fullLogLock != null;
            if (displayToConsole || displayToFullLog)
            {
                string sToConsole;
                if (echoType == InputEchoType.OnPassword)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < command.Length; i++)
                    {
                        sb.Append("*");
                    }
                    sToConsole = sb.ToString();
                }
                else
                {
                    sToConsole = command;
                }
                string sText = sToConsole + Environment.NewLine;
                if (displayToConsole)
                {
                    lock (_consoleTextLock)
                    {
                        _newConsoleText.Add(sText);
                    }
                }
                if (displayToFullLog)
                {
                    lock (_fullLogLock)
                    {
                        _fullLogLines.Add(sText);
                    }
                }
            }
        }

        private void btnOtherSingleMove_Click(object sender, EventArgs e)
        {
            string move = Interaction.InputBox("Move:", "Enter Move", string.Empty);
            if (!string.IsNullOrEmpty(move))
            {
                DoSingleMove(move.ToLower(), true);
            }
        }

        private void btnDoSingleMove_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).Tag.ToString();
            DoSingleMove(direction, false);
        }

        private void DoSingleMove(string direction, bool forceGo)
        {
            if (direction == "nw")
                direction = "northwest";
            else if (direction == "n")
                direction = "north";
            else if (direction == "ne")
                direction = "northeast";
            else if (direction == "w")
                direction = "west";
            else if (direction == "e")
                direction = "east";
            else if (direction == "sw")
                direction = "southwest";
            else if (direction == "s")
                direction = "south";
            else if (direction == "se")
                direction = "southeast";
            else if (direction == "u")
                direction = "up";
            else if (direction == "d")
                direction = "down";
            Exit navigateExit = null;
            bool ambiguous = false;
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            if (currentRoom != null)
            {
                Func<Exit, bool> discriminator = (e) =>
                {
                    return string.Equals(e.ExitText, direction, StringComparison.OrdinalIgnoreCase);
                };
                foreach (Exit foundExit in IsengardMap.GetRoomExits(currentRoom, discriminator))
                {
                    if (navigateExit == null)
                    {
                        navigateExit = foundExit;
                    }
                    else
                    {
                        ambiguous = true;
                        break;
                    }
                }
            }
            if (ambiguous || navigateExit == null)
            {
                navigateExit = new Exit(null, null, direction);
                navigateExit.ForceGo = forceGo;
            }
            NavigateSingleExitInBackground(navigateExit);
        }

        private void ctxConsole_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == tsmiClearConsole)
            {
                ClearConsole();
            }
        }

        private void ClearConsole()
        {
            lock (_consoleTextLock)
            {
                _newConsoleText.Clear();
            }
            rtbConsole.Clear();
        }

        private void btnLevel1OffensiveSpell_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.OffensiveSpellLevel1);
        }

        private void btnLevel2OffensiveSpell_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.OffensiveSpellLevel2);
        }

        private void btnLevel3OffensiveSpell_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.OffensiveSpellLevel3);
        }

        private void btnStun_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.Stun);
        }

        private void btnVigor_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.Vigor);
        }

        private void btnMendWounds_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicStrategyStep.MendWounds);
        }

        private void btnCurePoison_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                bwp.CureIfPoisoned = true;
                RunBackgroundProcess(bwp);
            }
            else
            {
                lock (_queuedCommandLock)
                {
                    bwp.QueuedMagicStep = MagicStrategyStep.CurePoison;
                }
            }
        }

        private void btnVigorPotion_Click(object sender, EventArgs e)
        {
            RunOrQueuePotionsStep(sender, PotionsStrategyStep.Vigor);
        }

        private void btnMendPotion_Click(object sender, EventArgs e)
        {
            RunOrQueuePotionsStep(sender, PotionsStrategyStep.MendWounds);
        }

        private void btnCurePoisonPotion_Click(object sender, EventArgs e)
        {
            RunOrQueuePotionsStep(sender, PotionsStrategyStep.CurePoison);
        }

        private void btnAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeStrategyStep.RegularAttack);
        }

        public void btnPowerAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeStrategyStep.PowerAttack);
        }

        private void RunOrQueuePotionsStep(object sender, PotionsStrategyStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                Strategy s = new Strategy();
                s.PotionsSteps = new List<PotionsStrategyStep>() { step };
                s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
                bwp.Strategy = s;
                RunBackgroundProcess(bwp);
            }
            else
            {
                lock (_queuedCommandLock)
                {
                    bwp.QueuedPotionsStep = step;
                }
            }
        }

        private void RunOrQueueMagicStep(object sender, MagicStrategyStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                Strategy s = new Strategy();
                s.MagicSteps = new List<MagicStrategyStep> { step };
                s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
                RunStandaloneStrategy(s);
            }
            else
            {
                lock (_queuedCommandLock)
                {
                    bwp.QueuedMagicStep = step;
                }
            }
        }

        private void RunOrQueueMeleeStep(object sender, MeleeStrategyStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                Strategy s = new Strategy();
                s.MeleeSteps = new List<MeleeStrategyStep>() { step };
                s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
                RunStandaloneStrategy(s);
            }
            else
            {
                AddConsoleMessage($"Unable to run {step} since a background process is running.");
            }
        }

        private void RunStandaloneStrategy(Strategy s)
        {
            MobTypeEnum? eMobType = null;
            string sMobText = string.Empty;
            int iMobCounter = 0;
            bool isCombat = s.IsCombatStrategy(CommandType.All, s.TypesWithStepsEnabled);
            string sMobTextInput = txtMob.Text;
            if (isCombat && !MobEntity.GetMobInfo(sMobTextInput, out sMobText, out eMobType, out iMobCounter))
            {
                MessageBox.Show("Invalid mob text: " + sMobTextInput);
                return;
            }
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            if (eMobType.HasValue)
            {
                bwp.MobType = eMobType;
                bwp.MobTypeCounter = iMobCounter;
            }
            else if (!string.IsNullOrEmpty(sMobText))
            {
                bwp.MobText = sMobText;
                bwp.MobTextCounter = iMobCounter;
            }
            else if (iMobCounter >= 1) //monster in room with specified count
            {
                bwp.MobTypeCounter = bwp.MobTextCounter = 1;
            }
            bwp.Strategy = s;
            RunBackgroundProcess(bwp);
        }

        private void btnLookAtMob_Click(object sender, EventArgs e)
        {
            RunCommand("look " + GetMobTarget(true));
        }

        private void btnUseWandOnMob_Click(object sender, EventArgs e)
        {
            RunCommand("zap " + _wand + " " + GetMobTarget(false));
        }

        private void btnDoAction_Click(object sender, EventArgs e)
        {
            object oButtonTag;
            if (sender is ToolStripButton)
                oButtonTag = ((ToolStripButton)sender).Tag;
            else
                oButtonTag = ((Button)sender).Tag;
            CommandButtonTag cmdButtonTag = oButtonTag as CommandButtonTag;
            string command;
            if (cmdButtonTag != null)
                command = cmdButtonTag.Command;
            else
                command = oButtonTag.ToString();
            RunCommand(command);
        }

        private void tsbInventoryAndEquipment_Click(object sender, EventArgs e)
        {
            RunCommand("equipment");
            RunCommand("inventory");
        }

        private void RunCommand(string command)
        {
            if (_currentBackgroundParameters != null) //queue to background process
            {
                lock (_queuedCommandLock)
                {
                    _currentBackgroundParameters.QueuedCommands.Add(command);
                }
            }
            else
            {
                SendCommand(command, InputEchoType.On);
            }
        }

        private void RunBackgroundProcess(BackgroundWorkerParameters backgroundParameters)
        {
            _currentBackgroundParameters = backgroundParameters;
            _backgroundProcessPhase = BackgroundProcessPhase.Initialization;
            ToggleBackgroundProcessUI(backgroundParameters, true);
            _bwBackgroundProcess.RunWorkerAsync(backgroundParameters);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _currentBackgroundParameters.Cancelled = true;
            _bwBackgroundProcess.CancelAsync();
            btnAbort.Enabled = false;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            string command;
            bool isSet = chkSetOn.Checked;
            if (isSet)
                command = "set";
            else
                command = "clear";
            command += " ";
            command += cboSetOption.SelectedItem.ToString();
            if (isSet)
            {
                string setValue = txtSetValue.Text;
                if (!string.IsNullOrEmpty(setValue))
                {
                    command += " ";
                    command += setValue;
                }
            }
            SendCommand(command, InputEchoType.On);
        }

        private void cboSetOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSetValue.Text = string.Empty;
            txtSetValue.Enabled = chkSetOn.Checked && cboSetOption.SelectedItem.ToString() == "wimpy";
        }

        private void chkSetOn_CheckedChanged(object sender, EventArgs e)
        {
            cboSetOption_SelectedIndexChanged(null, null);
        }

        private void btnNonCombatPermRun_Click(object sender, EventArgs e)
        {
            Strategy s = new Strategy();
            s.FinalMeleeAction = FinalStepAction.FinishCombat;
            s.FinalMagicAction = FinalStepAction.FinishCombat;
            s.FinalPotionsAction = FinalStepAction.FinishCombat;
            s.AfterKillMonsterAction = AfterKillMonsterAction.StopCombat;
            s.TypesWithStepsEnabled = CommandType.None;
            RunAdHocStrategy(s, PermRunEditFlow.AdHocNonCombat);
        }

        private void btnAdHocPermRun_Click(object sender, EventArgs e)
        {
            RunAdHocStrategy(null, PermRunEditFlow.AdHocCombat);
        }

        /// <summary>
        /// runs an ad hoc strategy
        /// </summary>
        /// <param name="strategy">strategy to run</param>
        /// <param name="permRunEditFlow">perm run edit flow</param>
        private void RunAdHocStrategy(Strategy strategy, PermRunEditFlow permRunEditFlow)
        {
            WorkflowSpells workflowSpellsPotions = WorkflowSpells.None;
            ItemsToProcessType inventoryFlow;
            DateTime utcNow = DateTime.UtcNow;

            PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(false);
            SupportedKeysFlags keys = _currentEntityInfo.GetAvailableKeys(false);
            WorkflowSpells workflowSpellsCast = _currentEntityInfo.GetAvailableWorkflowSpells(AvailableSpellTypes.Castable);
            lock (_currentEntityInfo.EntityLock)
            {
                foreach (ItemTypeEnum next in _currentEntityInfo.EnumerateInventoryAndEquipmentItems())
                {
                    StaticItemData sid = ItemEntity.StaticItemData[next];
                    if (sid.ItemClass == ItemClass.Potion && SpellsStatic.SpellsByEnum.TryGetValue(sid.Spell.Value, out SpellInformationAttribute sia) && sia.WorkflowSpellType.HasValue)
                    {
                        workflowSpellsPotions |= sia.WorkflowSpellType.Value;
                    }
                }
            }

            Room currentRoom = _currentEntityInfo.CurrentRoom;

            Area initArea = cboArea.SelectedItem as Area;

            FullType beforeFull;
            FullType afterFull;
            if (strategy == null || strategy.TypesWithStepsEnabled != CommandType.None)
            {
                beforeFull = FullType.Total;
                afterFull = FullType.Almost;
                inventoryFlow = ItemsToProcessType.ProcessMonsterDrops;
            }
            else
            {
                if (currentRoom == null || !currentRoom.HealingRoom.HasValue)
                {
                    beforeFull = FullType.None;
                    afterFull = FullType.None;
                    inventoryFlow = ItemsToProcessType.ProcessAllItemsInRoom;
                }
                else
                {
                    beforeFull = FullType.Almost;
                    afterFull = FullType.None;
                    inventoryFlow = ItemsToProcessType.NoProcessing;
                }
            }
            PermRun p;
            using (frmPermRun frm = new frmPermRun(_gameMap, _settingsData, skills, keys, _currentEntityInfo.CurrentRoom, txtMob.Text, GetGraphInputs, strategy, inventoryFlow, _currentEntityInfo, beforeFull, afterFull, workflowSpellsCast, workflowSpellsPotions, initArea, permRunEditFlow))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                p = new PermRun();
                if (permRunEditFlow == PermRunEditFlow.AdHocCombat)
                    p.Flow = PermRunFlow.AdHocCombat;
                else if (permRunEditFlow == PermRunEditFlow.AdHocNonCombat)
                    p.Flow = PermRunFlow.AdHocNonCombat;
                frm.SaveFormDataToPermRun(p);
            }
            _currentPermRun = null;
            DoPermRun(p, false);
        }

        internal class CommandButtonTag
        {
            public CommandButtonTag(object ctl, string Command, CommandType CommandType, DependentObjectType ObjectType)
            {
                this.Control = ctl;
                this.IsToolStripButton = ctl is ToolStripButton;
                this.Command = Command;
                this.CommandType = CommandType;
                this.ObjectType = ObjectType;
            }
            public object Control { get; set; }
            public string Command { get; set; }
            public CommandType CommandType { get; set; }
            public DependentObjectType ObjectType { get; set; }
            public bool IsToolStripButton { get; set; }
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            try
            {
                _programmaticUI = true;
                HandleTimerTick();
            }
            finally
            {
                _programmaticUI = false;
            }

        }

        public static bool IsDay(int hour)
        {
            return hour >= SUNRISE_GAME_HOUR && hour < SUNSET_GAME_HOUR;
        }

        private void HandleTimerTick()
        {
            DateTime dtUTCNow = DateTime.UtcNow;
            InitializationStep initStep = _initializationSteps;
            int autohpforthistick = _autohp;
            int autompforthistick = _automp;

            List<string> textToAdd = new List<string>();
            lock (_consoleTextLock)
            {
                if (_newConsoleText.Count > 0)
                {
                    foreach (string s in _newConsoleText)
                    {
                        textToAdd.Add(s);
                    }
                    _newConsoleText.Clear();
                }
            }
            foreach (string s in textToAdd)
            {
                rtbConsole.AppendText(s);
            }

            if (!_enteredUserName && _promptedUserName)
            {
                SendCommand(_username, InputEchoType.On);
                _enteredUserName = true;
            }
            else if (_enteredUserName && !_enteredPassword && _promptedPassword)
            {
                SendCommand(_password, InputEchoType.OnPassword);
                _enteredPassword = true;
            }

            bool haveSettings = _settingsData != null;
            if (haveSettings && ((initStep & InitializationStep.Score) != InitializationStep.None))
            {
                Color fullColor = _settingsData.FullColor;
                Color emptyColor = _settingsData.EmptyColor;
                Color backColor;
                int iCurrentMana = _currentMana;
                if (autompforthistick != HP_OR_MP_UNKNOWN)
                {
                    int iTotalMP = _totalmp;
                    string sText = autompforthistick.ToString() + "/" + iTotalMP;
                    if (iCurrentMana != HP_OR_MP_UNKNOWN)
                    {
                        sText += " (" + iCurrentMana + ")";
                        _mpColorR = 100;
                        _mpColorG = 100;
                        _mpColorB = 100;
                    }
                    else
                    {
                        ComputeColor(iCurrentMana, iTotalMP, out byte r, out byte g, out byte b, fullColor, emptyColor);
                        _mpColorR = r;
                        _mpColorG = g;
                        _mpColorB = b;
                    }
                    lblManaValue.Text = sText;
                }
                if (autohpforthistick != HP_OR_MP_UNKNOWN)
                {
                    lblHitpointsValue.Text = autohpforthistick.ToString() + "/" + _totalhp;
                    ComputeColor(autohpforthistick, _totalhp, out byte r, out byte g, out byte b, fullColor, emptyColor);
                    _hpColorR = r;
                    _hpColorG = g;
                    _hpColorB = b;
                }
                byte newHPR = _hpColorR;
                byte newHPG = _hpColorG;
                byte newHPB = _hpColorB;
                if (newHPR != _hpColorRUI || newHPG != _hpColorGUI || newHPB != _hpColorBUI)
                {
                    UIShared.GetForegroundColor(newHPR, newHPG, newHPB, out byte forer, out byte foreg, out byte foreb);
                    lblHitpointsValue.BackColor = Color.FromArgb(newHPR, newHPG, newHPB);
                    lblHitpointsValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
                    _hpColorRUI = newHPR;
                    _hpColorGUI = newHPG;
                    _hpColorBUI = newHPB;
                }
                byte newMPR = _mpColorR;
                byte newMPG = _mpColorG;
                byte newMPB = _mpColorB;
                if (newMPR != _mpColorRUI || newMPG != _mpColorGUI || newMPB != _mpColorBUI)
                {
                    UIShared.GetForegroundColor(newMPR, newMPG, newMPB, out byte forer, out byte foreg, out byte foreb);
                    lblManaValue.BackColor = Color.FromArgb(newMPR, newMPG, newMPB);
                    lblManaValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
                    _mpColorRUI = newMPR;
                    _mpColorGUI = newMPG;
                    _mpColorBUI = newMPB;
                }

                //refresh cooldowns (active and timers)
                List<SkillCooldown> cooldowns = new List<SkillCooldown>();
                lock (_currentEntityInfo.EntityLock)
                {
                    cooldowns.AddRange(_currentEntityInfo.SkillsCooldowns);
                }
                if (cooldowns.Count > 0)
                {
                    if (cooldowns[0].CooldownLabel == null) //recreate labels
                    {
                        grpSkillCooldowns.Controls.Clear();
                        int iLabelY = 17;
                        foreach (SkillCooldown next in cooldowns)
                        {
                            Label lblSkill = new Label();
                            lblSkill.AutoSize = true;
                            lblSkill.Location = new Point(5, iLabelY);
                            lblSkill.Size = new Size(64, 13);
                            lblSkill.Text = next.SkillName;
                            lblSkill.TextAlign = ContentAlignment.MiddleLeft;

                            Label lblValue = new Label();
                            lblValue.AutoSize = false;
                            lblValue.Location = new Point(84, iLabelY);
                            lblValue.Size = new Size(76, 15);
                            lblValue.TextAlign = ContentAlignment.MiddleCenter;
                            next.CooldownLabel = lblValue;

                            iLabelY += 19;

                            grpSkillCooldowns.Controls.Add(lblSkill);
                            grpSkillCooldowns.Controls.Add(lblValue);
                        }
                    }
                }
                foreach (SkillCooldown nextCooldown in cooldowns)
                {
                    SkillCooldownStatus status = nextCooldown.Status;
                    DateTime nextAvailable = nextCooldown.NextAvailable;
                    string sText;
                    if (status == SkillCooldownStatus.Active)
                    {
                        sText = "ACTIVE";
                        backColor = fullColor;
                    }
                    else if (status == SkillCooldownStatus.Available)
                    {
                        sText = "0:00";
                        backColor = fullColor;
                    }
                    else if (status == SkillCooldownStatus.Inactive)
                    {
                        sText = "?";
                        backColor = emptyColor;
                    }
                    else if (status == SkillCooldownStatus.Waiting)
                    {
                        DateTime dtDateValue = nextCooldown.NextAvailable;
                        if (dtUTCNow >= dtDateValue) //available now
                        {
                            sText = "0:00";
                        }
                        else //still waiting
                        {
                            TimeSpan ts = dtDateValue - dtUTCNow;
                            sText = ts.Minutes + ":" + ts.Seconds.ToString().PadLeft(2, '0');
                        }
                        backColor = sText == "0:00" ? fullColor : emptyColor;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    Label lbl = nextCooldown.CooldownLabel;
                    if (!string.Equals(sText, nextCooldown.RemainingTextUI))
                    {
                        lbl.Text = sText;
                        nextCooldown.RemainingTextUI = sText;
                    }
                    if (backColor != nextCooldown.RemainingColorUI)
                    {
                        UIShared.GetForegroundColor(backColor.R, backColor.G, backColor.B, out byte forer, out byte foreg, out byte foreb);
                        lbl.BackColor = backColor;
                        lbl.ForeColor = Color.FromArgb(forer, foreg, foreb);
                        nextCooldown.RemainingColorUI = backColor;
                    }
                }

                if (_refreshSpellsCast)
                {
                    List<string> spellNames = new List<string>();
                    List<SpellsEnum> spells = new List<SpellsEnum>();
                    lock (_currentEntityInfo.EntityLock)
                    {
                        spells.AddRange(_spellsCast);
                        _refreshSpellsCast = false;
                    }
                    if (spells.Count == 0)
                    {
                        spellNames.Add("None");
                    }
                    else
                    {
                        foreach (SpellsEnum nextSpell in spells)
                        {
                            spellNames.Add(SpellsStatic.SpellsByEnum[nextSpell].SpellName);
                        }
                    }
                    spellNames.Sort();
                    flpSpells.Controls.Clear();
                    foreach (string next in spellNames)
                    {
                        Label l = new Label();
                        l.AutoSize = true;
                        l.Text = next;
                        flpSpells.Controls.Add(l);
                    }
                }
            }
            
            double dGameSecondsFromResetPoint = (DateTime.UtcNow - _serverStartTime).TotalSeconds % 3600;
            double dGameExtraSeconds = dGameSecondsFromResetPoint % SECONDS_PER_GAME_HOUR;
            int iTime = (Convert.ToInt32(dGameSecondsFromResetPoint - dGameExtraSeconds) / SECONDS_PER_GAME_HOUR) + 1;
            if (iTime == 24) iTime = 0;
            if (iTime != _timeUI)
            {
                _timeUI = iTime;
                Color backColor, foreColor;
                if (IsDay(iTime))
                {
                    backColor = Color.Yellow;
                    foreColor = Color.Black;
                }
                else //night
                {
                    backColor = Color.Black;
                    foreColor = Color.White;
                }
                lblTime.BackColor = backColor;
                lblTime.ForeColor = foreColor;
                lblTime.Text = iTime.ToString().PadLeft(2, '0') + "00";
            }
            int? iTotalWeight;
            if ((initStep & InitializationStep.Inventory) != InitializationStep.None)
            {
                iTotalWeight = _currentEntityInfo.TotalInventoryWeight;
                if (iTotalWeight != _currentEntityInfo.TotalInventoryWeightUI)
                {
                    grpInventory.Text = "Inventory (#" + (iTotalWeight.HasValue ? iTotalWeight.Value.ToString() : "?") + ")";
                    _currentEntityInfo.TotalInventoryWeightUI = iTotalWeight;
                }
            }
            if ((initStep & InitializationStep.Equipment) != InitializationStep.None)
            {
                decimal dArmorClassScore = _currentEntityInfo.ArmorClassScore;
                bool bArmorClassScoreIsExact = _currentEntityInfo.ArmorClassScoreExact;
                decimal dArmorClassCalculated = _currentEntityInfo.ArmorClassCalculated;
                lock (_currentEntityInfo.EntityLock)
                {
                    dArmorClassScore = _currentEntityInfo.ArmorClassScore;
                    bArmorClassScoreIsExact = _currentEntityInfo.ArmorClassScoreExact;
                    dArmorClassCalculated = _currentEntityInfo.ArmorClassCalculated;
                    iTotalWeight = _currentEntityInfo.TotalEquipmentWeight;
                }
                if (iTotalWeight != _currentEntityInfo.TotalEquipmentWeightUI || dArmorClassScore != _currentEntityInfo.ArmorClassScoreUI || bArmorClassScoreIsExact != _currentEntityInfo.ArmorClassScoreExactUI || dArmorClassCalculated != _currentEntityInfo.ArmorClassCalculatedUI)
                {
                    string sEquipmentText = "Equipment (#" + (iTotalWeight.HasValue ? iTotalWeight.Value.ToString() : "?") + " ";
                    if (dArmorClassCalculated >= 0)
                    {
                        sEquipmentText += dArmorClassCalculated.ToString("N1") + "ac)";
                    }
                    else if (dArmorClassScore >= 0)
                    {
                        if (bArmorClassScoreIsExact)
                        {
                            sEquipmentText += dArmorClassScore.ToString("N1") + "ac)";
                        }
                        else
                        {
                            sEquipmentText += dArmorClassScore.ToString("N0") + "ac)";
                        }
                    }

                    grpEquipment.Text = sEquipmentText;

                    _currentEntityInfo.TotalEquipmentWeightUI = iTotalWeight;
                    _currentEntityInfo.ArmorClassScoreUI = dArmorClassScore;
                    _currentEntityInfo.ArmorClassScoreExactUI = bArmorClassScoreIsExact;
                    _currentEntityInfo.ArmorClassCalculatedUI = dArmorClassCalculated;
                }
            }

            int iMonsterDamage = _monsterDamage;
            MonsterStatus monsterStatus = _currentMonsterStatus;

            string sCurrentMonsterText = _currentlyFightingMobText;
            MobTypeEnum? eCurrentMonsterType = _currentlyFightingMobType;
            int iCurrentMonsterCounter = _currentlyFightingMobCounter;
            if (!string.Equals(sCurrentMonsterText, _currentlyFightingMobTextUI) ||
                eCurrentMonsterType != _currentlyFightingMobTypeUI ||
                iCurrentMonsterCounter != _currentlyFightingMobCounterUI)
            {
                string sMobText;
                if (eCurrentMonsterType.HasValue)
                {
                    sMobText = eCurrentMonsterType.Value.ToString();
                    if (iCurrentMonsterCounter > 1) sMobText += " " + iCurrentMonsterCounter;
                }
                else if (string.IsNullOrEmpty(sCurrentMonsterText))
                {
                    sMobText = sCurrentMonsterText;
                    if (iCurrentMonsterCounter > 1) sMobText += " " + iCurrentMonsterCounter;
                }
                else
                {
                    sMobText = "Mob";
                }
                grpMob.Text = sMobText;
                _currentlyFightingMobTextUI = sCurrentMonsterText;
                _currentlyFightingMobTypeUI = eCurrentMonsterType;
                _currentlyFightingMobCounterUI = iCurrentMonsterCounter;
            }
            if (iMonsterDamage != _monsterDamageUI)
            {
                txtMobDamage.Text = _monsterDamage <= 0 ? string.Empty : _monsterDamage.ToString();
                _monsterDamageUI = iMonsterDamage;
            }
            if (monsterStatus != _currentMonsterStatusUI)
            {
                txtMobStatus.Text = GetMonsterStatusText(monsterStatus);
                _currentMonsterStatusUI = monsterStatus;
            }

            string sCurrentPlayerHeader = _currentPlayerHeader;
            if (!string.Equals(sCurrentPlayerHeader, _currentPlayerHeaderUI))
            {
                grpCurrentPlayer.Text = sCurrentPlayerHeader;
                _currentPlayerHeaderUI = sCurrentPlayerHeader;
            }

            int iGold = _gold;
            if (iGold != _goldUI)
            {
                lblGold.Text = "Gold: " + iGold.ToString();
                _goldUI = iGold;
            }
            int iTNL = _tnl;
            int iExperience = _experience;
            if (iTNL != _tnlUI || iExperience != _experienceUI)
            {
                string tnlText;
                if (_tnl > 0)
                    tnlText = "TNL: " + iTNL.ToString();
                else
                    tnlText = "XP: " + _experience + " TNL: 0";
                lblToNextLevelValue.Text = tnlText;
                _tnlUI = iTNL;
                _experienceUI = iExperience;
            }

            PermRun pr = _currentPermRun;
            if (pr != _currentPermRunUI)
            {
                txtCurrentPermRun.Text = pr == null ? string.Empty : pr.ToString();
                _currentPermRunUI = pr;
            }
            pr = _nextPermRun;
            if (pr != _nextPermRunUI)
            {
                txtNextPermRun.Text = pr == null ? string.Empty : pr.ToString();
                _nextPermRunUI = pr;
            }

            if (haveSettings)
            {
                RefreshAutoEscapeUI(!_processedUIWithSettings);
                _processedUIWithSettings = true;
            }

            lock (_broadcastMessagesLock)
            {
                if (_broadcastMessages.Count > 0)
                {
                    foreach (string nextMessage in _broadcastMessages)
                    {
                        lstMessages.Items.Add(StringProcessing.GetDateTimeForDisplay(DateTime.Now, true, false) + " " + nextMessage);
                    }
                    lstMessages.TopIndex = lstMessages.Items.Count - 1;
                    _broadcastMessages.Clear();
                }
            }

            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            bool inBackgroundProcess = bwp != null;

            lock (_currentEntityInfo.EntityLock)
            {
                Room oCurrentRoom = _currentEntityInfo.CurrentRoom;
                if (oCurrentRoom != _currentEntityInfo.CurrentRoomUI)
                {
                    if ((_setCurrentArea || inBackgroundProcess) && oCurrentRoom != null)
                    {
                        foreach (Area a in _settingsData.EnumerateAreas())
                        {
                            if ((oCurrentRoom.HealingRoom.HasValue && a.TickRoom == oCurrentRoom.HealingRoom.Value) ||
                                (oCurrentRoom.PawnShoppe.HasValue && a.PawnShop == oCurrentRoom.PawnShoppe.Value) ||
                                a.InventorySinkRoomObject == oCurrentRoom)
                            {
                                _setCurrentArea = false;
                                cboArea.SelectedItem = a;
                                break;
                            }
                        }
                    }
                    string sCurrentRoom;
                    if (oCurrentRoom != null)
                    {
                        sCurrentRoom = oCurrentRoom.DisplayName;
                    }
                    else
                    {
                        sCurrentRoom = "No Current Room";
                    }
                    grpCurrentRoom.Text = sCurrentRoom;
                    _currentEntityInfo.CurrentRoomUI = oCurrentRoom;
                    RefreshCurrentAreaButtons(_currentBackgroundParameters != null);
                }
                if (_currentEntityInfo.CurrentEntityChanges.Count > 0)
                {
                    List<EntityChange> changes = new List<EntityChange>(_currentEntityInfo.CurrentEntityChanges);
                    _currentEntityInfo.CurrentEntityChanges.Clear();

                    TreeNode tnObviousMobs = _currentEntityInfo.tnObviousMobs;
                    TreeNode tnObviousItems = _currentEntityInfo.tnObviousItems;
                    TreeNode tnObviousExits = _currentEntityInfo.tnObviousExits;
                    TreeNode tnOtherExits = _currentEntityInfo.tnOtherExits;
                    TreeNode tnPermanentMobs = _currentEntityInfo.tnPermanentMobs;
                    TreeNode tnUnknownEntities = _currentEntityInfo.tnUnknownEntities;
                    foreach (EntityChange nextEntityChange in changes)
                    {
                        EntityChangeType rcType = nextEntityChange.ChangeType;
                        if (rcType == EntityChangeType.RefreshRoom)
                        {
                            bool firstTimeThrough = !_currentEntityInfo.UIProcessed;
                            treeCurrentRoom.Nodes.Clear();
                            tnObviousMobs.Nodes.Clear();
                            tnObviousItems.Nodes.Clear();
                            tnObviousExits.Nodes.Clear();
                            tnOtherExits.Nodes.Clear();
                            tnPermanentMobs.Nodes.Clear();
                            tnUnknownEntities.Nodes.Clear();
                            foreach (var nextEntry in nextEntityChange.Changes)
                            {
                                if (nextEntry.MobType.HasValue)
                                {
                                    tnObviousMobs.Nodes.Add(GetMobsNode(nextEntry.MobType.Value));
                                }
                                else if (nextEntry.Item != null)
                                {
                                    tnObviousItems.Nodes.Add(GetItemsNode(nextEntry.Item));
                                }
                                else if (nextEntry.UnknownTypeEntity != null)
                                {
                                    tnUnknownEntities.Nodes.Add(GetUnknownEntitiesNode(nextEntry.UnknownTypeEntity));
                                }
                            }
                            if (tnObviousMobs.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnObviousMobs);
                                if (firstTimeThrough || _currentEntityInfo.ObviousMobsTNExpanded)
                                {
                                    tnObviousMobs.Expand();
                                }
                            }
                            if (tnObviousItems.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnObviousItems);
                                if (firstTimeThrough || _currentEntityInfo.ObviousItemsTNExpanded)
                                {
                                    tnObviousItems.Expand();
                                }
                            }
                            if (tnUnknownEntities.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnUnknownEntities);
                                if (firstTimeThrough || _currentEntityInfo.UnknownEntitiesExpanded)
                                {
                                    tnUnknownEntities.Expand();
                                }
                            }
                            foreach (string s in nextEntityChange.Exits)
                            {
                                tnObviousExits.Nodes.Add(GetObviousExitNode(nextEntityChange, s));
                            }
                            if (tnObviousExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnObviousExits);
                                if (firstTimeThrough || _currentEntityInfo.ObviousExitsTNExpanded)
                                {
                                    tnObviousExits.Expand();
                                }
                            }
                            foreach (Exit nextExit in nextEntityChange.OtherExits)
                            {
                                tnOtherExits.Nodes.Add(GetOtherExitsNode(nextExit));
                            }
                            if (tnOtherExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnOtherExits);
                                if (firstTimeThrough || _currentEntityInfo.OtherExitsTNExpanded)
                                {
                                    tnOtherExits.Expand();
                                }
                            }
                            if (oCurrentRoom?.PermanentMobs != null)
                            {
                                foreach (MobTypeEnum nextPerm in oCurrentRoom.PermanentMobs)
                                {
                                    tnPermanentMobs.Nodes.Add(GetMobsNode(nextPerm));
                                }
                                if (tnPermanentMobs.Nodes.Count > 0)
                                {
                                    treeCurrentRoom.Nodes.Add(tnPermanentMobs);
                                    if (firstTimeThrough || _currentEntityInfo.PermMobsTNExpanded)
                                    {
                                        tnPermanentMobs.Expand();
                                    }
                                }
                            }
                            _currentEntityInfo.UIProcessed = true;
                        }
                        else if (rcType == EntityChangeType.AddExit)
                        {
                            string exit = nextEntityChange.Exits[0];
                            bool hasNodes = tnObviousExits.Nodes.Count > 0;
                            tnObviousExits.Nodes.Add(GetObviousExitNode(nextEntityChange, exit));
                            TreeNode removeNode = null;
                            foreach (TreeNode tn in tnOtherExits.Nodes)
                            {
                                if (tn.Tag == nextEntityChange.MappedExits[exit])
                                {
                                    removeNode = tn;
                                    break;
                                }
                            }
                            if (!hasNodes)
                            {
                                InsertTopLevelTreeNode(tnObviousExits);
                            }
                            tnOtherExits.Nodes.Remove(removeNode);
                            if (tnOtherExits.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(tnOtherExits);
                            }
                        }
                        else if (rcType == EntityChangeType.RemoveExit)
                        {
                            string exit = nextEntityChange.Exits[0];
                            TreeNode removeNode = null;
                            Exit foundExit = null;
                            foreach (TreeNode tn in tnObviousExits.Nodes)
                            {
                                foundExit = tn.Tag as Exit;
                                if (foundExit != null && foundExit.ExitText == exit)
                                {
                                    removeNode = tn;
                                    break;
                                }
                            }
                            if (removeNode != null)
                            {
                                tnObviousExits.Nodes.Remove(removeNode);
                                if (tnObviousExits.Nodes.Count == 0)
                                {
                                    treeCurrentRoom.Nodes.Remove(tnObviousExits);
                                }
                                tnOtherExits.Nodes.Add(GetOtherExitsNode(foundExit));
                            }
                        }
                        else if (rcType == EntityChangeType.AddMob)
                        {
                            bool hasNodes = tnObviousMobs.Nodes.Count > 0;
                            bool isFirst = true;
                            MobTypeEnum? firstMob = null;
                            TreeNode firstInserted = null;
                            foreach (var nextChange in nextEntityChange.Changes)
                            {
                                if (nextChange.MobType.HasValue)
                                {
                                    MobTypeEnum nextMobType = nextChange.MobType.Value;
                                    TreeNode inserted = GetMobsNode(nextMobType);
                                    if (nextChange.RoomMobIndex == -1)
                                    {
                                        tnObviousMobs.Nodes.Add(inserted);
                                    }
                                    else
                                    {
                                        tnObviousMobs.Nodes.Insert(nextChange.RoomMobIndex, inserted);
                                    }
                                    if (isFirst)
                                    {
                                        firstMob = nextMobType;
                                        isFirst = false;
                                        firstInserted = inserted;
                                    }
                                }
                            }
                            if (!hasNodes)
                            {
                                InsertTopLevelTreeNode(tnObviousMobs);
                                if (!inBackgroundProcess)
                                {
                                    string sNewMobText = string.Empty;
                                    MobTypeEnum? firstAttackableMob = _currentEntityInfo.GetFirstAttackableMob();
                                    if (firstAttackableMob.HasValue)
                                    {
                                        sNewMobText = firstAttackableMob.Value.ToString();
                                    }
                                    txtMob.Text = sNewMobText;
                                }
                            }
                        }
                        else if (rcType == EntityChangeType.RemoveMob)
                        {
                            foreach (var nextChange in nextEntityChange.Changes)
                            {
                                if (nextChange.MobType.HasValue)
                                {
                                    tnObviousMobs.Nodes.RemoveAt(nextChange.RoomMobIndex);
                                }
                            }
                            if (tnObviousMobs.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(tnObviousMobs);
                            }
                        }
                        else if (rcType == EntityChangeType.DropItem || rcType == EntityChangeType.CreateRoomItems)
                        {
                            bool hasItemNodes = tnObviousItems.Nodes.Count > 0;
                            bool hasUnknownEntityNodes = tnUnknownEntities.Nodes.Count > 0;
                            bool addedItem = false;
                            bool addedUnknownEntity = false;
                            foreach (var nextChange in nextEntityChange.Changes)
                            {
                                TreeNode inserted, parent;
                                int iIndex;
                                if (nextChange.Item != null)
                                {
                                    inserted = GetItemsNode(nextChange.Item);
                                    iIndex = nextChange.RoomItemIndex;
                                    addedItem = true;
                                    parent = tnObviousItems;
                                }
                                else if (nextChange.UnknownTypeEntity != null)
                                {
                                    inserted = GetUnknownEntitiesNode(nextChange.UnknownTypeEntity);
                                    iIndex = nextChange.RoomUnknownEntityIndex;
                                    addedUnknownEntity = true;
                                    parent = tnUnknownEntities;
                                }
                                else
                                {
                                    throw new InvalidOperationException();
                                }
                                if (iIndex == -1)
                                    parent.Nodes.Add(inserted);
                                else
                                    parent.Nodes.Insert(iIndex, inserted);
                            }
                            if (!hasItemNodes && addedItem)
                            {
                                InsertTopLevelTreeNode(tnObviousItems);
                            }
                            if (!hasUnknownEntityNodes && addedUnknownEntity)
                            {
                                InsertTopLevelTreeNode(tnUnknownEntities);
                            }
                        }
                        else if (rcType == EntityChangeType.PickUpItem || rcType == EntityChangeType.RemoveRoomItems)
                        {
                            bool removedItem = false;
                            bool removedUnknownEntity = false;
                            foreach (var nextChange in nextEntityChange.Changes)
                            {
                                TreeNode parent = null;
                                int iIndex = -1;
                                bool effectChange = true;
                                if (nextChange.RoomItemIndex >= 0 && nextChange.RoomItemAction.HasValue && !nextChange.RoomItemAction.Value)
                                {
                                    removedItem = true;
                                    parent = tnObviousItems;
                                    iIndex = nextChange.RoomItemIndex;
                                }
                                else if (nextChange.RoomUnknownEntityIndex >= 0 && nextChange.RoomUnknownEntityAction.HasValue && !nextChange.RoomUnknownEntityAction.Value)
                                {
                                    removedUnknownEntity = true;
                                    parent = tnUnknownEntities;
                                    iIndex = nextChange.RoomUnknownEntityIndex;
                                }
                                else
                                {
                                    effectChange = false;
                                }
                                if (effectChange)
                                {
                                    parent.Nodes.RemoveAt(iIndex);
                                }
                            }
                            if (removedItem && tnObviousItems.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(tnObviousItems);
                            }
                            if (removedUnknownEntity && tnUnknownEntities.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(tnUnknownEntities);
                            }
                        }

                        if (rcType == EntityChangeType.RefreshInventory)
                        {
                            lstInventory.Items.Clear();
                            foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                            {
                                lstInventory.Items.Add(new ItemInInventoryOrEquipmentList(nextEntry.Item, true));
                            }
                        }
                        else if (rcType == EntityChangeType.RefreshEquipment)
                        {
                            lstEquipment.Items.Clear();
                            foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                            {
                                lstEquipment.Items.Add(new ItemInInventoryOrEquipmentList(nextEntry.Item, false));
                            }
                        }
                        else
                        {
                            HashSet<EntityChangeType> invAdds = new HashSet<EntityChangeType>()
                            {
                                EntityChangeType.PickUpItem,
                                EntityChangeType.UnequipItem,
                                EntityChangeType.MagicallySentItem,
                                EntityChangeType.Trade,
                            };
                            HashSet<EntityChangeType> invRemoves = new HashSet<EntityChangeType>()
                            {
                                EntityChangeType.DropItem,
                                EntityChangeType.ConsumeItem,
                                EntityChangeType.EquipItem,
                                EntityChangeType.Trade
                            };
                            HashSet<EntityChangeType> eqAdds = new HashSet<EntityChangeType>()
                            {
                                EntityChangeType.EquipItem,
                            };
                            HashSet<EntityChangeType> eqRemoves = new HashSet<EntityChangeType>()
                            {
                                EntityChangeType.UnequipItem,
                                EntityChangeType.DestroyEquipment,
                            };
                            foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                            {
                                ItemEntity ie = nextEntry.Item;
                                ProcessInventoryOrEquipmentChange(true, lstInventory, rcType, nextEntry.InventoryAction, nextEntry.InventoryIndex, ie, invAdds, invRemoves);
                                ProcessInventoryOrEquipmentChange(false, lstEquipment, rcType, nextEntry.EquipmentAction, nextEntry.EquipmentIndex, ie, eqAdds, eqRemoves);
                            }
                        }
                    }
                }
            }

            EnableDisableActionButtons(bwp);
            if (!inBackgroundProcess)
            {
                RunPollTickIfNecessary(autohpforthistick, autompforthistick, dtUTCNow);

                bool hazying, fleeing;
                lock (_escapeLock)
                {
                    hazying = _hazying;
                    fleeing = _fleeing;
                }
                if (hazying || fleeing)
                {
                    StartEscapeBackgroundProcess(hazying, fleeing);
                }
            }
        }

        private void ProcessInventoryOrEquipmentChange(bool isInventory, ListBox lst, EntityChangeType changeType, bool? action, int index, ItemEntity item, HashSet<EntityChangeType> addTypes, HashSet<EntityChangeType> removeTypes)
        {
            bool isAdd;
            bool isOK;
            if (action.HasValue)
            {
                isAdd = action.Value;
                if (isAdd)
                    isOK = addTypes.Contains(changeType);
                else
                    isOK = removeTypes.Contains(changeType);
                if (isOK)
                {
                    if (isAdd)
                    {
                        ItemInInventoryOrEquipmentList it = new ItemInInventoryOrEquipmentList(item, isInventory);
                        if (index == -1)
                            lst.Items.Add(it);
                        else
                            lst.Items.Insert(index, it);
                    }
                    else
                    {
                        lst.Items.RemoveAt(index);
                    }
                }
            }
        }

        /// <summary>
        /// runs a poll tick if necessary if the first status update has completed and not at full HP+MP
        /// and the last status update happened at least 5 seconds ago
        /// and the last poll tick happened at least 5 seconds ago
        /// </summary>
        private void RunPollTickIfNecessary(int autohp, int automp, DateTime dtUTCNow)
        {
            //check for poll tick if the first status update has completed and not at full HP+MP
            if (_currentStatusLastComputed.HasValue && (autohp == HP_OR_MP_UNKNOWN || autohp < _totalhp || automp == HP_OR_MP_UNKNOWN || automp < _totalmp))
            {
                bool runPollTick = (dtUTCNow - _currentStatusLastComputed.Value).TotalSeconds >= 5;
                if (runPollTick && _lastPollTick.HasValue)
                {
                    runPollTick = (dtUTCNow - _lastPollTick.Value).TotalSeconds >= 5;
                }
                if (runPollTick)
                {
                    _lastPollTick = dtUTCNow;
                    SendCommand(string.Empty, InputEchoType.On);
                }
            }
        }

        private void InsertTopLevelTreeNode(TreeNode topLevelTreeNode)
        {
            if (!treeCurrentRoom.Nodes.Contains(topLevelTreeNode))
            {
                int iIndex = _currentEntityInfo.GetTopLevelTreeNodeLogicalIndex(topLevelTreeNode);
                bool found = false;
                for (int i = 0; i < treeCurrentRoom.Nodes.Count; i++)
                {
                    TreeNode next = treeCurrentRoom.Nodes[i];
                    int nextIndex = _currentEntityInfo.GetTopLevelTreeNodeLogicalIndex(next);
                    if (iIndex < nextIndex)
                    {
                        treeCurrentRoom.Nodes.Insert(i, topLevelTreeNode);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    treeCurrentRoom.Nodes.Add(topLevelTreeNode);
                }
                if (_currentEntityInfo.GetTopLevelTreeNodeExpanded(topLevelTreeNode))
                {
                    topLevelTreeNode.Expand();
                }
            }
        }

        private TreeNode GetOtherExitsNode(Exit nextExit)
        {
            string sNodeText = nextExit.ExitText + " (" + nextExit.Target.DisplayName + ")";
            TreeNode nextTreeNode = new TreeNode(sNodeText);
            nextTreeNode.Tag = nextExit;
            return nextTreeNode;
        }

        private TreeNode GetMobsNode(MobTypeEnum mob)
        {
            StaticMobData smd = MobEntity.StaticMobData[mob];
            string sName = smd.SingularName;
            if (smd.Experience > 0)
            {
                sName += " " + smd.Experience.ToString();
            }
            if (smd.Alignment.HasValue)
            {
                sName += " " + StaticMobData.GetAlignmentString(smd.Alignment.Value);
            }
            TreeNode ret = new TreeNode(sName);
            ret.Tag = mob;
            return ret;
        }

        private TreeNode GetItemsNode(ItemEntity item)
        {
            TreeNode ret = new TreeNode(item.GetItemString());
            ret.Tag = item;
            return ret;
        }

        private TreeNode GetUnknownEntitiesNode(UnknownTypeEntity ute)
        {
            TreeNode ret = new TreeNode(ute.Name);
            ret.Tag = ute;
            return ret;
        }

        private TreeNode GetObviousExitNode(EntityChange nextRoomChange, string s)
        {
            string sNodeText = s;
            Exit foundExit;

            //use the first word for the mapping, since subsequent words aren't used anyway
            string sMappedWord = s.Trim();
            int iSpaceIndex = sMappedWord.IndexOf(' ');
            if (iSpaceIndex > 0)
            {
                sMappedWord = sMappedWord.Substring(0, iSpaceIndex);
            }

            if (nextRoomChange.MappedExits.TryGetValue(sMappedWord, out foundExit))
            {
                if (foundExit == null)
                {
                    sNodeText += " (Ambiguous)";
                }
                else
                {
                    sNodeText += " (" + foundExit.Target.DisplayName + ")";
                }
            }
            TreeNode nextTreeNode = new TreeNode(sNodeText);
            if (foundExit != null)
            {
                nextTreeNode.Tag = foundExit;
            }
            else
            {
                nextTreeNode.Tag = s;
            }
            return nextTreeNode;
        }

        private static void ComputeColor(int current, int max, out byte r, out byte g, out byte b, Color fullColor, Color emptyColor)
        {
            byte iFullR = fullColor.R;
            byte iFullG = fullColor.G;
            byte iFullB = fullColor.B;
            byte iEmptyR = emptyColor.R;
            byte iEmptyG = emptyColor.G;
            byte iEmptyB = emptyColor.B;
            double multiplier = ((double)current) / max;

            r = (byte)(iEmptyR + Math.Round(multiplier * (iFullR - iEmptyR), 0));
            g = (byte)(iEmptyG + Math.Round(multiplier * (iFullG - iEmptyG), 0));
            b = (byte)(iEmptyB + Math.Round(multiplier * (iFullB - iEmptyB), 0));
        }

        private string GetMonsterStatusText(MonsterStatus? status)
        {
            string ret = string.Empty;
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case MonsterStatus.ExcellentCondition:
                        ret = "90-100%";
                        break;
                    case MonsterStatus.FewSmallScratches:
                        ret = "81-89%";
                        break;
                    case MonsterStatus.WincingInPain:
                        ret = "71-79%";
                        break;
                    case MonsterStatus.SlightlyBruisedAndBattered:
                        ret = "61-69%";
                        break;
                    case MonsterStatus.SomeMinorWounds:
                        ret = "51-59%";
                        break;
                    case MonsterStatus.BleedingProfusely:
                        ret = "41-49%";
                        break;
                    case MonsterStatus.NastyAndGapingWound:
                        ret = "31-39%";
                        break;
                    case MonsterStatus.ManyGreviousWounds:
                        ret = "21-29%";
                        break;
                    case MonsterStatus.MortallyWounded:
                        ret = "11-19%";
                        break;
                    case MonsterStatus.BarelyClingingToLife:
                        ret = "1-9%";
                        break;
                }
            }
            return ret;
        }

        private void txtMob_TextChanged(object sender, EventArgs e)
        {
            _mob = txtMob.Text;
            EnableDisableActionButtons(_currentBackgroundParameters);
        }

        private void txtWand_TextChanged(object sender, EventArgs e)
        {
            _wand = txtWand.Text;
            EnableDisableActionButtons(_currentBackgroundParameters);
            btnIncrementWand.Enabled = !string.IsNullOrEmpty(txtWand.Text);
        }

        private void btnIncrementWand_Click(object sender, EventArgs e)
        {
            string text = (txtWand.Text ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(text))
            {
                string baseWand = text;
                int currentIndex = 1;
                int iSpaceIndex = text.LastIndexOf(' ');
                if (iSpaceIndex > 0)
                {
                    string lastWord = text.Substring(iSpaceIndex + 1);
                    if (int.TryParse(lastWord, out int iExistingIndex) && iExistingIndex > 0)
                    {
                        currentIndex = iExistingIndex;
                        baseWand = text.Substring(0, iSpaceIndex);
                    }
                }
                currentIndex++;
                txtWand.Text = baseWand + " " + currentIndex;
            }
        }

        private void txtOneOffCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string sCommand = txtOneOffCommand.Text;
                string[] words = sCommand.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0)
                {
                    SendCommand(string.Empty, InputEchoType.On);
                }
                else
                {
                    string sFirstWord = words[0];
                    string sMovementCommand = GetOneWordExitCommand(sFirstWord);
                    bool isMovement = !string.IsNullOrEmpty(sMovementCommand) || (sFirstWord == "go" && words.Length > 1);
                    bool isLook = sFirstWord == "look" && words.Length == 1;
                    if (isMovement || isLook)
                    {
                        if (_currentBackgroundParameters == null)
                        {
                            if (isLook)
                            {
                                RunSingleBackgroundCommand(BackgroundCommandType.Look);
                            }
                            else if (isMovement)
                            {
                                string sCommandLower = sMovementCommand;
                                if (string.IsNullOrEmpty(sCommandLower))
                                {
                                    sCommandLower = words[1];
                                }
                                DoSingleMove(sCommandLower, sFirstWord == "go");
                            }
                        }
                        else if (isLook)
                        {
                            MessageBox.Show("Cannot run look command manually when background process running.");
                            return;
                        }
                        else if (isMovement)
                        {
                            MessageBox.Show("Cannot run movement command manually when background process running.");
                            return;
                        }
                    }
                    else if (sFirstWord == "quit")
                    {
                        TryQuit(_settingsData != null && _settingsData.SaveSettingsOnQuit, false);
                    }
                    else
                    {
                        SendCommand(txtOneOffCommand.Text, InputEchoType.On);
                    }
                    txtOneOffCommand.SelectAll();
                }
            }
        }

        private static string GetOneWordExitCommand(string input)
        {
            string ret = null;
            switch (input)
            {
                case "nw":
                case "northw":
                case "northwe":
                case "northwes":
                case "northwest":
                    ret = "northwest";
                    break;
                case "n":
                case "north":
                    ret = "north";
                    break;
                case "ne":
                case "northe":
                case "northea":
                case "northeas":
                case "northeast":
                    ret = "northeast";
                    break;
                case "w":
                case "wes":
                case "west":
                    ret = "west";
                    break;
                case "e":
                case "eas":
                case "east":
                    ret = "east";
                    break;
                case "sw":
                case "southw":
                case "southwe":
                case "southwes":
                case "southwest":
                    ret = "southwest";
                    break;
                case "s":
                case "south":
                    ret = "south";
                    break;
                case "se":
                case "southe":
                case "southea":
                case "southeas":
                case "southeast":
                    ret = "southeast";
                    break;
                case "u":
                case "up":
                    ret = "up";
                    break;
                case "d":
                case "dow":
                case "down":
                    ret = "down";
                    break;
                case "ou":
                case "out":
                    ret = "out";
                    break;
            }
            return ret;
        }

        private void TryQuit(bool SaveSettingsOnQuit, bool LogoutOnQuit)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp != null)
            {
                MessageBox.Show("Cannot quit when background process running.");
            }
            else
            {
                bwp = new BackgroundWorkerParameters();
                bwp.SingleCommandType = BackgroundCommandType.Quit;
                bwp.SaveSettingsOnQuit = SaveSettingsOnQuit;
                bwp.LogoutOnQuit = LogoutOnQuit;
                RunBackgroundProcess(bwp);
            }
        }

        private bool SaveSettings()
        {
            List<string> errorMessages = new List<string>();
            try
            {
                using (SQLiteConnection conn = IsengardSettingData.GetSqliteConnection(GetDatabasePath()))
                {
                    conn.Open();
                    int userID = IsengardSettingData.GetUserID(conn, _username, true);
                    _settingsData.SaveSettings(conn, userID);
                }
            }
            finally
            {
                if (errorMessages.Count > 0)
                {
                    MessageBox.Show(string.Join(Environment.NewLine, errorMessages.ToArray()));
                }
            }
            return errorMessages.Count == 0;
        }

        private void txtEmoteText_TextChanged(object sender, EventArgs e)
        {
            btnEmote.Enabled = !string.IsNullOrEmpty(txtCommandText.Text);
        }

        private void btnEmote_Click(object sender, EventArgs e)
        {
            SendCommand("emote " + txtCommandText.Text, InputEchoType.On);
            txtCommandText.Focus();
            txtCommandText.SelectAll();
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            SendCommand("say " + txtCommandText.Text, InputEchoType.On);
            txtCommandText.Focus();
            txtCommandText.SelectAll();
        }

        private void txtEmoteTarget_TextChanged(object sender, EventArgs e)
        {
            RefreshEmoteButtons();
        }

        private void chkShowEmotesWithoutTarget_CheckedChanged(object sender, EventArgs e)
        {
            RefreshEmoteButtons();
        }

        private void RefreshEmoteButtons()
        {
            bool ShowHasTarget = !string.IsNullOrEmpty(txtEmoteTarget.Text);
            bool ShowNoTarget = chkShowEmotesWithoutTarget.Checked;
            if (_showingWithTarget != ShowHasTarget || _showingWithoutTarget != ShowNoTarget)
            {
                flpEmotes.Controls.Clear();
                foreach (EmoteButton eb in _emoteButtons)
                {
                    if (eb.HasTarget ? ShowHasTarget : ShowNoTarget)
                    {
                        flpEmotes.Controls.Add(eb);
                    }
                }
                _showingWithTarget = ShowHasTarget;
                _showingWithoutTarget = ShowNoTarget;
            }
        }

        private void tsmiMob_Click(object sender, EventArgs e)
        {
            txtMob.Text = ((ToolStripMenuItem)sender).Text;
        }

        private void btnFlee_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                StartEscapeBackgroundProcess(false, true);
            }
            else
            {
                _fleeing = true;
            }
        }

        private void btnDrinkHazy_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                StartEscapeBackgroundProcess(true, false);
            }
            else
            {
                _hazying = true;
            }
        }

        private void StartEscapeBackgroundProcess(bool hazy, bool flee)
        {
            if (hazy) _hazying = true;
            if (flee) _fleeing = true;
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.Hazy = hazy;
            bwp.Flee = flee;
            RunBackgroundProcess(bwp);
        }

        private void btnScore_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                bwp.DoScore = true;
                RunBackgroundProcess(bwp);
            }
            else
            {
                SendCommand("score", InputEchoType.On);
            }
        }

        private void RefreshEnabledForSingleMoveButtons()
        {
            Room r = _currentEntityInfo.CurrentRoom;
            bool haveCurrentRoom = r != null;

            bool n, ne, nw, w, e, s, sw, se, u, d, o;
            n = ne = nw = w = e = s = sw = se = u = d = o = false;
            if (haveCurrentRoom)
            {
                foreach (Exit nextExit in r.Exits)
                {
                    switch (nextExit.ExitText.ToLower())
                    {
                        case "north":
                            n = true;
                            break;
                        case "northeast":
                            ne = true;
                            break;
                        case "northwest":
                            nw = true;
                            break;
                        case "west":
                            w = true;
                            break;
                        case "east":
                            e = true;
                            break;
                        case "south":
                            s = true;
                            break;
                        case "southeast":
                            se = true;
                            break;
                        case "southwest":
                            sw = true;
                            break;
                        case "up":
                            u = true;
                            break;
                        case "down":
                            d = true;
                            break;
                        case "out":
                            o = true;
                            break;
                    }
                }
            }
            else
            {
                n = ne = nw = w = e = s = sw = se = u = d = o = true;
            }
            btnNorth.Enabled = n;
            btnNorthwest.Enabled = nw;
            btnNortheast.Enabled = ne;
            btnWest.Enabled = w;
            btnEast.Enabled = e;
            btnSouth.Enabled = s;
            btnSouthwest.Enabled = sw;
            btnSoutheast.Enabled = se;
            btnUp.Enabled = u;
            btnDn.Enabled = d;
            btnOut.Enabled = o;
            btnOtherSingleMove.Enabled = true;
        }

        private void tcMain_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabEmotes)
            {
                if (!_initiatedEmotesTab)
                {
                    InitializeEmotes();
                    _initiatedEmotesTab = true;
                }
            }
            else if (e.TabPage == tabHelp)
            {
                if (!_initiatedHelpTab)
                {
                    InitializeHelp();
                    _initiatedHelpTab = true;
                }
            }
        }

        private GraphInputs GetGraphInputs()
        {
            bool flying, levitating;
            SupportedKeysFlags keys;
            lock (_currentEntityInfo.EntityLock)
            {
                flying = _spellsCast.Contains(SpellsEnum.fly);
                levitating = _spellsCast.Contains(SpellsEnum.levitate);
                keys = _currentEntityInfo.GetAvailableKeys(false) | SupportedKeysFlags.RustyKey;
            }
            return new GraphInputs(_class, _level, IsDay(_timeUI), flying, levitating, keys);
        }

        /// <summary>
        /// returns the current perm run and whether a background process is currently running
        /// </summary>
        /// <returns>current perm run and whether background process is currently running</returns>
        private KeyValuePair<PermRun, bool> GetCurrentPermRun()
        {
            return new KeyValuePair<PermRun, bool>(_currentPermRun, _currentBackgroundParameters != null);
        }

        private List<Exit> CalculateRouteExits(Room fromRoom, Room targetRoom)
        {
            return MapComputation.ComputeLowestCostPath(fromRoom, targetRoom, GetGraphInputs());
        }

        private void NavigateSingleExitInBackground(Exit exit)
        {
            NavigateExitsInBackground(new List<Exit>() { exit });
        }

        private void NavigateExitsInBackground(List<Exit> exits)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.Exits = exits;
            RunBackgroundProcess(bwp);
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            Room originalCurrentRoom = _currentEntityInfo.CurrentRoom;
            bool readOnly = _currentBackgroundParameters != null;
            frmGraph frm = new frmGraph(_gameMap, originalCurrentRoom, false, GetGraphInputs, VertexSelectionRequirement.ValidPathFromCurrentLocation, readOnly);
            if (frm.ShowDialog().GetValueOrDefault(false))
            {
                Room newCurrentRoom = _currentEntityInfo.CurrentRoom;
                if (newCurrentRoom == originalCurrentRoom)
                {
                    Room selectedRoom = frm.CurrentRoom;
                    List<Exit> selectedPath = frm.SelectedPath;
                    if (selectedPath != null)
                    {
                        NavigateExitsInBackground(frm.SelectedPath);
                    }
                    else
                    {
                        lock (_currentEntityInfo.EntityLock)
                        {
                            _currentEntityInfo.CurrentRoom = selectedRoom;
                        }
                    }
                }
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            Room originalCurrentRoom = _currentEntityInfo.CurrentRoom;
            bool readOnly = _currentBackgroundParameters != null;
            frmLocations frm = new frmLocations(_gameMap, _settingsData, originalCurrentRoom, false, GetGraphInputs, readOnly);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Room newCurrentRoom = _currentEntityInfo.CurrentRoom;
                if (newCurrentRoom == originalCurrentRoom)
                {
                    Room selectedRoom = frm.CurrentRoom;
                    List<Exit> selectedPath = frm.SelectedPath;
                    if (selectedPath != null)
                    {
                        NavigateExitsInBackground(frm.SelectedPath);
                    }
                    else
                    {
                        lock (_currentEntityInfo.EntityLock)
                        {
                            _currentEntityInfo.CurrentRoom = selectedRoom;
                        }
                    }
                }
            }
        }

        private void tsmiSetAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            int autoEscapeThreshold = _settingsData.AutoEscapeThreshold;
            string sDefault = autoEscapeThreshold > 0 ? autoEscapeThreshold.ToString() : string.Empty;
            string sNewAutoEscapeThreshold = Interaction.InputBox("New auto escape threshold:", "Auto Escape Threshold", sDefault);
            if (int.TryParse(sNewAutoEscapeThreshold, out int iNewAutoEscapeThreshold) && iNewAutoEscapeThreshold > 0 && iNewAutoEscapeThreshold < _totalhp)
            {
                _settingsData.AutoEscapeThreshold = iNewAutoEscapeThreshold;
                RefreshAutoEscapeUI(true);
            }
            else
            {
                MessageBox.Show("Invalid auto escape threshold: " + sNewAutoEscapeThreshold);
            }
        }

        private void tsmiClearAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _settingsData.AutoEscapeActive = false;
            _settingsData.AutoEscapeThreshold = 0;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiToggleAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _settingsData.AutoEscapeActive = !_autoEscapeActiveSaved;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _settingsData.AutoEscapeType = AutoEscapeType.Flee;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _settingsData.AutoEscapeType = AutoEscapeType.Hazy;
            RefreshAutoEscapeUI(true);
        }

        /// <summary>
        /// updates the auto-escape UI. This must run on the UI thread.
        /// </summary>
        private void RefreshAutoEscapeUI(bool forceSet)
        {
            bool autoEscapeActive = _settingsData.AutoEscapeActive;
            int autoEscapeThreshold = _settingsData.AutoEscapeThreshold;
            AutoEscapeType autoEscapeType = _settingsData.AutoEscapeType;
            bool fleeing = _fleeing;
            bool hazying = _hazying;
            if (forceSet || 
                autoEscapeActive != _autoEscapeActiveUI ||
                autoEscapeThreshold != _autoEscapeThresholdUI ||
                autoEscapeType != _autoEscapeTypeUI ||
                fleeing != _fleeingUI ||
                hazying != _hazyingUI)
            {
                Color autoEscapeBackColor;
                string autoEscapeText;
                if (hazying)
                {
                    autoEscapeBackColor = Color.Red;
                    autoEscapeText = "Trying to Hazy";
                }
                else if (fleeing)
                {
                    autoEscapeBackColor = Color.Red;
                    autoEscapeText = "Trying to Flee";
                }
                else
                {
                    string sAutoEscapeType = autoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
                    if (autoEscapeThreshold > 0)
                    {
                        autoEscapeText = sAutoEscapeType + " @ " + autoEscapeThreshold.ToString();
                    }
                    else
                    {
                        autoEscapeText = sAutoEscapeType;
                    }
                    if (autoEscapeActive)
                    {
                        if (autoEscapeType == AutoEscapeType.Hazy)
                        {
                            autoEscapeBackColor = Color.DarkBlue;
                        }
                        else //Flee
                        {
                            autoEscapeBackColor = Color.DarkRed;
                        }
                    }
                    else if (autoEscapeThreshold > 0)
                    {
                        autoEscapeText += " (Off)";
                        autoEscapeBackColor = Color.LightGray;
                    }
                    else
                    {
                        autoEscapeText += " (Off)";
                        autoEscapeBackColor = Color.Black;
                    }
                }

                UIShared.GetForegroundColor(autoEscapeBackColor.R, autoEscapeBackColor.G, autoEscapeBackColor.G, out byte forer, out byte foreg, out byte foreb);
                lblAutoEscapeValue.BackColor = autoEscapeBackColor;
                lblAutoEscapeValue.ForeColor = Color.FromArgb(forer, foreg, foreb);
                lblAutoEscapeValue.Text = autoEscapeText;

                _autoEscapeActiveUI = autoEscapeActive;
                _autoEscapeThresholdUI = autoEscapeThreshold;
                _autoEscapeTypeUI = autoEscapeType;
                _fleeingUI = fleeing;
                _hazyingUI = hazying;
            }
        }

        private void ctxAutoEscape_Opening(object sender, CancelEventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp != null)
            {
                e.Cancel = true;
            }
            else
            {
                _autoEscapeActiveSaved = _settingsData.AutoEscapeActive;
                tsmiAutoEscapeFlee.Checked = _settingsData.AutoEscapeType == AutoEscapeType.Flee;
                tsmiAutoEscapeHazy.Checked = _settingsData.AutoEscapeType == AutoEscapeType.Hazy;
                tsmiAutoEscapeIsActive.Checked = _settingsData.AutoEscapeActive;
                bool hasThreshold = _settingsData.AutoEscapeThreshold > 0;
                tsmiAutoEscapeIsActive.Enabled = hasThreshold;
                tsmiClearAutoEscapeThreshold.Enabled = hasThreshold;
            }
        }

        private void cboArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentAreaButtons(_currentBackgroundParameters != null);
        }

        private void RefreshCurrentAreaButtons(bool InBackground)
        {
            Area a = cboArea.SelectedItem as Area;
            Room rCurrentRoom = _currentEntityInfo.CurrentRoom;
            bool canEnable = a != null && !InBackground && rCurrentRoom != null;
            btnGoToHealingRoom.Enabled = canEnable && a.TickRoom.HasValue && _gameMap.HealingRooms[a.TickRoom.Value] != rCurrentRoom;
            btnGoToPawnShop.Enabled = canEnable && a.PawnShop.HasValue && _gameMap.PawnShoppes[a.PawnShop.Value] != rCurrentRoom;
            btnGoToInventorySink.Enabled = canEnable && a.InventorySinkRoomObject != null && a.InventorySinkRoomObject != rCurrentRoom;
        }

        private void btnGoToHealingRoom_Click(object sender, EventArgs e)
        {
            GoToAreaRoom(AreaRoomType.Tick);
        }

        private void btnGoToPawnShop_Click(object sender, EventArgs e)
        {
            GoToAreaRoom(AreaRoomType.Pawn);
        }

        private void btnGoToInventorySink_Click(object sender, EventArgs e)
        {
            GoToAreaRoom(AreaRoomType.InventorySink);
        }

        private void GoToAreaRoom(AreaRoomType roomType)
        {
            Area a = cboArea.SelectedItem as Area;
            if (a == null)
            {
                MessageBox.Show("No area selected.");
                return;
            }
            Room targetRoom = null;
            switch (roomType)
            {
                case AreaRoomType.Tick:
                    if (a.TickRoom.HasValue) targetRoom = _gameMap.HealingRooms[a.TickRoom.Value];
                    break;
                case AreaRoomType.Pawn:
                    if (a.PawnShop.HasValue) targetRoom = _gameMap.PawnShoppes[a.PawnShop.Value];
                    break;
                case AreaRoomType.InventorySink:
                    targetRoom = a.InventorySinkRoomObject;
                    break;
            }
            if (targetRoom == null)
            {
                MessageBox.Show($"No area {roomType} found.");
                return;
            }
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            if (currentRoom == null)
            {
                MessageBox.Show("No current room, unable to travel to specified room.");
                return;
            }
            if (currentRoom == targetRoom)
            {
                MessageBox.Show("Already at specified room.");
                return;
            }
            List<Exit> exits = CalculateRouteExits(currentRoom, targetRoom);
            if (exits == null)
            {
                MessageBox.Show("No path to target room found.");
                return;
            }
            NavigateExitsInBackground(exits);
        }

        private void treeCurrentRoom_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeCurrentRoom.SelectedNode = e.Node;
            }
        }

        private void treeCurrentRoom_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (!_programmaticUI)
            {
                _currentEntityInfo.SetExpandFlag(e.Node, true);
            }
        }

        private void treeCurrentRoom_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (!_programmaticUI)
            {
                _currentEntityInfo.SetExpandFlag(e.Node, false);
            }
        }

        private void treeCurrentRoom_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            TreeNode parentNode = selectedNode.Parent;
            bool isObviousMobs = parentNode == _currentEntityInfo.tnObviousMobs;
            bool isPermanentMobs = parentNode == _currentEntityInfo.tnPermanentMobs;
            if (isObviousMobs || isPermanentMobs)
            {
                MobTypeEnum selectedMob = (MobTypeEnum)selectedNode.Tag;
                //find the counter for the mob type of the selected mob
                int iMobCount = 0;
                foreach (TreeNode nextTreeNode in parentNode.Nodes)
                {
                    MobTypeEnum nextMobType = (MobTypeEnum)nextTreeNode.Tag;
                    if (nextMobType == selectedMob)
                    {
                        iMobCount++;
                    }
                    if (nextTreeNode == selectedNode)
                    {
                        break;
                    }
                }
                string sMobText = selectedMob.ToString();
                if (iMobCount > 1) sMobText += " " + iMobCount;
                txtMob.Text = sMobText;
            }
        }

        private void RunCommandOnRoomItemTreeNode(string command, TreeNode selectedNode)
        {
            int counter = FindItemOrMobCounterInRoomUI(selectedNode, true);
            if (counter == 0)
            {
                MessageBox.Show($"Cannot {command} unknown item.");
                return;
            }
            ItemEntity ie = (ItemEntity)selectedNode.Tag;
            lock (_currentEntityInfo.EntityLock)
            {
                bool validateAgainstOtherSources = command == "look";
                string sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, ie.ItemType.Value, counter, false, validateAgainstOtherSources);
                if (string.IsNullOrEmpty(sItemText))
                    MessageBox.Show($"Unable to construct {command} command for item.");
                else
                    SendCommand($"{command} {sItemText}", InputEchoType.On);
            }
        }

        private void treeCurrentRoom_DoubleClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeCurrentRoom.SelectedNode;
            if (selectedNode != null)
            {
                TreeNode parentNode = selectedNode.Parent;
                object oTag = selectedNode.Tag;
                if (parentNode == _currentEntityInfo.tnObviousItems) //pick up item
                {
                    RunCommandOnRoomItemTreeNode("look", selectedNode);
                }
                else if (parentNode == _currentEntityInfo.tnObviousExits || parentNode == _currentEntityInfo.tnOtherExits)
                {
                    if (oTag is Exit)
                    {
                        NavigateSingleExitInBackground((Exit)oTag);
                    }
                    else //string
                    {
                        string sTarget = oTag.ToString();
                        DoSingleMove(sTarget, sTarget.Contains(" "));
                    }
                }
                else if (parentNode == _currentEntityInfo.tnObviousMobs || parentNode == _currentEntityInfo.tnPermanentMobs) //look at mob
                {
                    int counter = FindItemOrMobCounterInRoomUI(selectedNode, false);
                    if (counter == 0)
                    {
                        MessageBox.Show("Cannot look at unknown mob.");
                        return;
                    }
                    MobTypeEnum mt = (MobTypeEnum)selectedNode.Tag;
                    MobLocationType mtLocType = parentNode == _currentEntityInfo.tnObviousMobs ? MobLocationType.CurrentRoomMobs : MobLocationType.PickFromList;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        Room currentRoom = _currentEntityInfo.CurrentRoom;
                        string sMobText = _currentEntityInfo.PickMobTextFromMobCounter(currentRoom.PermanentMobs, mtLocType, mt, counter, false, true);
                        if (string.IsNullOrEmpty(sMobText))
                        {
                            MessageBox.Show("Unable to look at mob.");
                        }
                        else
                        {
                            SendCommand("look " + sMobText, InputEchoType.On);
                        }
                    }
                }
            }
        }

        private int FindItemOrMobCounterInRoomUI(TreeNode node, bool isItem)
        {
            int counter = 0;
            ItemEntity ie;
            ItemTypeEnum ieType = ItemTypeEnum.GoldCoins;
            MobTypeEnum mtType = MobTypeEnum.LittleMouse;
            if (isItem)
            {
                ie = (ItemEntity)node.Tag;
                if (!ie.ItemType.HasValue) return 0;
                ieType = ie.ItemType.Value;
            }
            else
            {
                mtType = (MobTypeEnum)node.Tag;
            }
            foreach (TreeNode nextNode in node.Parent.Nodes)
            {
                bool matches;
                if (isItem)
                {
                    ItemEntity next = (ItemEntity)nextNode.Tag;
                    matches = ieType == next.ItemType.Value;
                }
                else
                {
                    MobTypeEnum nextMT = (MobTypeEnum)nextNode.Tag;
                    matches = nextMT == mtType;
                }
                if (matches)
                {
                    counter++;
                    if (node == nextNode)
                    {
                        break;
                    }
                }
            }
            return counter;
        }

        private void btnLook_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                RunSingleBackgroundCommand(BackgroundCommandType.Look);
            }
        }

        private void RunSingleBackgroundCommand(BackgroundCommandType commandType)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.SingleCommandType = commandType;
            RunBackgroundProcess(bwp);
        }

        private void ctxInventoryOrEquipmentItem_Opening(object sender, CancelEventArgs e)
        {
            bool inBackground = _currentBackgroundParameters != null;
            ctxInventoryOrEquipmentItem.Items.Clear();
            ListBox lst = (ListBox)ctxInventoryOrEquipmentItem.SourceControl;
            bool isInventory = lst == lstInventory;
            StaticItemData sid = null;
            ItemTypeEnum itemType = ItemTypeEnum.GoldCoins;
            Room r = _currentEntityInfo.CurrentRoom;
            List<SelectedInventoryOrEquipmentItem> sioeiList = new List<SelectedInventoryOrEquipmentItem>();
            foreach (ItemInInventoryOrEquipmentList oObj in lst.SelectedItems)
            {
                ItemEntity ie = oObj.Item;
                if (ie.ItemType.HasValue)
                {
                    itemType = ie.ItemType.Value;
                    sid = ItemEntity.StaticItemData[itemType];
                    if (!isInventory && sid.EquipmentType == EquipmentType.Unknown) //unknown equipment type cannot be acted on since selection text cannot be constructed
                    {
                        e.Cancel = true;
                    }
                }
                else //unknown item cannot be acted on since selection text cannot be constructed
                {
                    e.Cancel = true;
                    return;
                }
                int iCounter = 0;
                if (isInventory)
                {
                    foreach (ItemInInventoryOrEquipmentList nextEntry in lstInventory.Items)
                    {
                        if (nextEntry.Item.ItemType.HasValue && nextEntry.Item.ItemType == itemType)
                        {
                            iCounter++;
                        }
                        if (nextEntry == oObj)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ItemInInventoryOrEquipmentList nextEntry in lstEquipment.Items)
                    {
                        if (nextEntry.Item.ItemType.Value == itemType)
                        {
                            iCounter++;
                        }
                        if (nextEntry == oObj)
                        {
                            break;
                        }
                    }
                }
                if (iCounter == 0)
                {
                    e.Cancel = true;
                    return;
                }
                sioeiList.Add(new SelectedInventoryOrEquipmentItem(ie, itemType, iCounter, isInventory ? ItemLocationType.Inventory : ItemLocationType.Equipment));
            }
            if (sioeiList.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            bool hasMultiple = sioeiList.Count > 0;

            ctxInventoryOrEquipmentItem.Tag = sioeiList;

            string sInventoryEquipmentAction;
            ItemClass iclass = sid.ItemClass;
            ToolStripMenuItem tsmi;

            if (isInventory && r != null && r.PawnShoppe.HasValue)
            {
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "sell";
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            if (iclass == ItemClass.Potion)
            {
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "drink";
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            tsmi = new ToolStripMenuItem();
            tsmi.Text = "look";
            ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            if (isInventory)
            {
                if (!inBackground && r != null && _gameMap.Trades.ContainsKey(itemType))
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = "trade";
                    ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                }
                if (iclass == ItemClass.Scroll)
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = "read";
                    ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                }
                switch (iclass)
                {
                    case ItemClass.Equipment:
                        sInventoryEquipmentAction = "wear";
                        break;
                    case ItemClass.Weapon:
                        sInventoryEquipmentAction = "wield";
                        break;
                    case ItemClass.Usable: //can't hold, can only use
                        sInventoryEquipmentAction = "use";
                        break;
                    default:
                        sInventoryEquipmentAction = "hold";
                        break;
                }
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "drop";
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            else
            {
                sInventoryEquipmentAction = "remove";
            }
            if (sInventoryEquipmentAction != null)
            {
                tsmi = new ToolStripMenuItem();
                tsmi.Text = sInventoryEquipmentAction;
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            if (!hasMultiple)
            {
                if (iclass == ItemClass.Weapon)
                {
                    if (_settingsData.Weapon != itemType)
                    {
                        tsmi = new ToolStripMenuItem();
                        tsmi.Text = "Set Weapon";
                        ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                    }
                }
                else if (sid.EquipmentType == EquipmentType.Holding)
                {
                    if (_settingsData.HeldItem != itemType)
                    {
                        tsmi = new ToolStripMenuItem();
                        tsmi.Text = "Set Held Item";
                        ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                    }
                }
            }
        }

        private void ctxInventoryOrEquipmentItem_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            List<SelectedInventoryOrEquipmentItem> sioeis = (List<SelectedInventoryOrEquipmentItem>)ctxInventoryOrEquipmentItem.Tag;
            MobTypeEnum? foundTradeMob = null;
            Room tradeRoom = null;
            for (int i = sioeis.Count - 1; i >= 0; i--)
            {
                SelectedInventoryOrEquipmentItem sioei = sioeis[i];
                ItemTypeEnum eItemType = sioei.ItemType.Value;
                ToolStripMenuItem tsmi = (ToolStripMenuItem)e.ClickedItem;
                string menuText = tsmi.Text;
                if (menuText == "Set Weapon")
                {
                    _settingsData.Weapon = eItemType;
                    MessageBox.Show($"Weapon set to {eItemType}");
                }
                else if (menuText == "Set Held Item")
                {
                    _settingsData.HeldItem = eItemType;
                    MessageBox.Show($"Held item set to {eItemType}");
                }
                else if (menuText == "trade")
                {
                    if (!_gameMap.Trades.TryGetValue(eItemType, out MobTypeEnum tradeMob))
                    {
                        MessageBox.Show($"{eItemType} is not tradeable.");
                        return;
                    }
                    else if (foundTradeMob.HasValue && tradeMob != foundTradeMob.Value)
                    {
                        MessageBox.Show("Cannot trade with multiple mobs at once.");
                        return;
                    }
                    else if (tradeRoom == null && !_gameMap.MobRooms.TryGetValue(tradeMob, out tradeRoom))
                    {
                        MessageBox.Show($"No room found for {tradeMob}.");
                        return;
                    }
                    else if (tradeRoom == null)
                    {
                        MessageBox.Show($"Ambiguous room found for {tradeMob}.");
                        return;
                    }
                    else
                    {
                        foundTradeMob = tradeMob;
                    }
                }
                else
                {
                    ItemLocationType ilt = sioei.LocationType;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        bool validateAgainstOtherSources;
                        if (ilt == ItemLocationType.Equipment)
                            validateAgainstOtherSources = menuText != "remove";
                        else
                            validateAgainstOtherSources = false;
                        string sText = _currentEntityInfo.PickItemTextFromItemCounter(ilt, eItemType, sioei.Counter, false, validateAgainstOtherSources);
                        if (string.IsNullOrEmpty(sText))
                            MessageBox.Show("Unable to construct selection text for " + ilt + " " + eItemType + " " + sioei.Counter);
                        else
                            SendCommand(menuText + " " + sText, InputEchoType.On);
                    }
                }
            }

            if (tradeRoom != null)
            {
                Room currentRoom = _currentEntityInfo.CurrentRoom;
                if (currentRoom == null)
                {
                    MessageBox.Show("No current room, cannot trade.");
                    return;
                }
                List<Exit> foundPath = null;
                if (currentRoom != tradeRoom)
                {
                    foundPath = MapComputation.ComputeLowestCostPath(currentRoom, tradeRoom, GetGraphInputs());
                    if (foundPath == null)
                    {
                        MessageBox.Show("No path found to " + foundTradeMob.Value);
                        return;
                    }
                }
                if ((MessageBox.Show($"Are you sure you want to trade with {foundTradeMob.Value} at {tradeRoom.DisplayName}?", "Trade", MessageBoxButtons.OKCancel) == DialogResult.OK))
                {
                    BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
                    bwp.Exits = foundPath;
                    bwp.MobType = foundTradeMob.Value;
                    bwp.MobTypeCounter = 1;
                    bwp.InventoryItems = sioeis;
                    RunBackgroundProcess(bwp);
                }
            }
        }

        private void tsbReloadMap_Click(object sender, EventArgs e)
        {
            ReloadMap(true);
            MessageBox.Show("Reloaded!");
        }

        private void ctxMessages_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Clipboard.SetText(string.Join(Environment.NewLine, lstMessages.SelectedItems.OfType<string>()));
        }

        private void ctxMessages_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = lstMessages.SelectedItems.Count == 0;
        }

        private void tsmiEditSettings_Click(object sender, EventArgs e)
        {
            bool autoEscapeActive;
            int autoEscapeThreshold;
            AutoEscapeType autoEscapeType;
            lock (_escapeLock)
            {
                autoEscapeActive = _settingsData.AutoEscapeActive;
                autoEscapeThreshold = _settingsData.AutoEscapeThreshold;
                autoEscapeType = _settingsData.AutoEscapeType;
            }
            IsengardSettingData clone = new IsengardSettingData(_settingsData);
            frmConfiguration frm = new frmConfiguration(clone, autoEscapeThreshold, autoEscapeType, autoEscapeActive, _gameMap, GetGraphInputs, _currentEntityInfo);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                bool newAutoEscapeActive = frm.CurrentAutoEscapeActive;
                int newAutoEscapeThreshold = frm.CurrentAutoEscapeThreshold;
                AutoEscapeType newAutoEscapeType = frm.CurrentAutoEscapeType;
                lock (_escapeLock)
                {
                    if (autoEscapeActive != newAutoEscapeActive)
                    {
                        clone.AutoEscapeActive = newAutoEscapeActive;
                    }
                    clone.AutoEscapeThreshold = newAutoEscapeThreshold;
                    clone.AutoEscapeType = newAutoEscapeType;
                    _settingsData = clone;
                }
                AfterLoadSettings();
            }
        }

        private void tsmiExportXML_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML File|*.xml";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = " ";
                    using (XmlWriter xmlWriter = XmlWriter.Create(sfd.FileName, settings))
                    {
                        _settingsData.SaveToXmlWriter(xmlWriter);
                    }
                    MessageBox.Show(this, "Saved!");
                }
            }
        }

        private void tsmiImportXML_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "XML File|*.xml";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    List<string> errorMessages = new List<string>();
                    bool success = false;
                    try
                    {
                        _settingsData = new IsengardSettingData(ofd.FileName, errorMessages, true, _gameMap);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        AddConsoleMessage("Import failed: " + ex.ToString());
                    }
                    AddBroadcastMessages(errorMessages);
                    if (success)
                    {
                        AfterLoadSettings();
                        MessageBox.Show("Imported!");
                    }
                }
            }
        }

        private void AddBroadcastMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.Add(message);
                }
            }
        }

        private void AddBroadcastMessages(List<string> messages)
        {
            if (messages != null && messages.Count > 0)
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.AddRange(messages);
                }
            }
        }

        private void tsmiSaveSettings_Click(object sender, EventArgs e)
        {
            if (_settingsData == null)
            {
                MessageBox.Show("Settings were never loaded and thus cannot be saved.");
            }
            else
            {
                if (SaveSettings())
                {
                    MessageBox.Show("Settings Saved!");
                }
            }
        }

        private void tsmiImportFromPlayer_Click(object sender, EventArgs e)
        {
            string player = Interaction.InputBox("Player name:", "Enter Player Name", string.Empty);
            if (!string.IsNullOrEmpty(player))
            {
                string localDatabase = GetDatabasePath();
                using (SQLiteConnection conn = IsengardSettingData.GetSqliteConnection(localDatabase))
                {
                    conn.Open();
                    int userID = IsengardSettingData.GetUserID(conn, player, false);
                    if (userID == 0)
                    {
                        MessageBox.Show("That user does not have settings.");
                        return;
                    }
                    IsengardSettingData settingsData = LoadSettingsForUser(conn, userID);
                    settingsData.ScrubIDs();
                    _settingsData = settingsData;
                    AfterLoadSettings();
                    MessageBox.Show("Settings loaded!");
                }
            }
        }

        private IsengardSettingData LoadSettingsForUser(SQLiteConnection conn, int UserID)
        {
            List<string> errorMessages = new List<string>();
            IsengardSettingData ret = new IsengardSettingData(conn, UserID, errorMessages, _gameMap);
            AddBroadcastMessages(errorMessages);
            return ret;
        }

        private void tsmiQuitWithoutSaving_Click(object sender, EventArgs e)
        {
            TryQuit(false, false);
        }

        private void tsmiRestoreDefaults_Click(object sender, EventArgs e)
        {
            _settingsData = IsengardSettingData.GetDefaultSettings();
            AfterLoadSettings();
            MessageBox.Show("Defaults restored!");
        }

        private void tsbQuit_Click(object sender, EventArgs e)
        {
            TryQuit(_settingsData != null && _settingsData.SaveSettingsOnQuit, false);
        }

        private void tsbLogout_Click(object sender, EventArgs e)
        {
            TryQuit(_settingsData != null && _settingsData.SaveSettingsOnQuit, true);
        }

        private void btnPermRuns_Click(object sender, EventArgs e)
        {
            bool haveBackgroundProcess = _currentBackgroundParameters != null;
            Area currentArea = cboArea.SelectedItem as Area;
            using (frmPermRuns frm = new frmPermRuns(_settingsData, _gameMap, _currentEntityInfo, GetGraphInputs, haveBackgroundProcess, currentArea, GetCurrentPermRun))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    PermRun prToRun = frm.PermRunToRun;
                    if (prToRun != null)
                    {
                        if (haveBackgroundProcess)
                        {
                            _nextPermRun = prToRun;
                        }
                        else
                        {
                            _currentPermRun = prToRun;
                            DoPermRun(frm.PermRunToRun, false);
                        }
                    }
                    else
                    {
                        NavigateExitsInBackground(frm.NavigateToRoom);
                    }
                }
            }
        }

        private void DoPermRun(PermRun p, bool resume)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.CurrentArea = cboArea.SelectedItem as Area;
            bwp.SetPermRun(p, _gameMap, GetGraphInputs());
            bwp.SelectedItemsWithTargets = null;
            bwp.Resume = resume;
            if (!resume)
            {
                p.BeforeGold = _gold;
                p.BeforeExperience = _experience;
            }
            RunBackgroundProcess(bwp);
        }

        private void ctxCurrentRoom_Opening(object sender, CancelEventArgs e)
        {
            ctxCurrentRoom.Items.Clear();
            TreeNode selectedNode = treeCurrentRoom.SelectedNode;
            if (selectedNode == null || selectedNode.Parent == null)
            {
                e.Cancel = true;
            }
            else if (selectedNode.Parent == _currentEntityInfo.tnObviousItems)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Text = "get";
                ctxCurrentRoom.Items.Add(tsmi);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ctxCurrentRoom_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            RunCommandOnRoomItemTreeNode(e.ClickedItem.Text, treeCurrentRoom.SelectedNode);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_fullLogLock != null)
            {
                _bwFullLog.CancelAsync();
                while (!_fullLogFinished)
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void tsmiOpenLogFolder_Click(object sender, EventArgs e)
        {
            string sLogFolder = Path.Combine(Path.GetTempPath(), "Isengard");
            Process.Start("explorer.exe", sLogFolder);
        }

        private void tsmiSearch_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                RunSingleBackgroundCommand(BackgroundCommandType.Search);
            }
        }

        private void tsmiHide_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                RunSingleBackgroundCommand(BackgroundCommandType.Hide);
            }
        }

        private void tsmiItemManagement_Click(object sender, EventArgs e)
        {
            if (_currentEntityInfo.CurrentRoom != null)
            {
                BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
                using (frmItemManagement frm = new frmItemManagement(_currentEntityInfo, _gameMap, GetGraphInputs, _settingsData, cboArea.SelectedItem as Area))
                {
                    if (frm.ShowDialog(this) != DialogResult.OK) return;
                    bwp.InventoryProcessInputType = ItemsToProcessType.ProcessAllItemsInRoom;
                    bwp.SelectedItemsWithTargets = frm.ItemsToChange;
                    bwp.TargetRoom = _currentEntityInfo.CurrentRoom;
                    bwp.InventorySinkRoom = frm.SinkRoom;
                    bwp.HealingRoom = frm.TickRoom;
                    bwp.PawnShop = frm.PawnShop;
                }
                RunBackgroundProcess(bwp);
            }
            else
            {
                MessageBox.Show("No current room.");
            }
        }

        private void btnRemoveCurrentPermRun_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                _currentPermRun = null;
            }
        }

        private void btnRemoveNextPermRun_Click(object sender, EventArgs e)
        {
            _nextPermRun = null;
        }

        private void btnCompleteCurrentPermRun_Click(object sender, EventArgs e)
        {
            PermRun pr = _currentPermRun;
            if (pr != null)
            {
                CompletePermRun(pr);
            }
        }

        private void btnResumeCurrentPermRun_Click(object sender, EventArgs e)
        {
            PermRun pr = _currentPermRun;
            if (pr != null)
            {
                DoPermRun(pr, true);
            }
        }

        private void tsmiShipInfo_Click(object sender, EventArgs e)
        {
            DateTime dtCycle = _serverStartTime;
            StringBuilder sb = new StringBuilder();
            double minutesIntoCycle;
            double minutesIntoCurrentCycle;
            int cycleNumber;
            string secondsRemaining;
            if (dtCycle != DateTime.MinValue)
            {
                double dGameSecondsFromResetPoint = (DateTime.UtcNow - _serverStartTime).TotalSeconds % 3600;
                double dGameExtraSeconds = dGameSecondsFromResetPoint % SECONDS_PER_GAME_HOUR;
                int iTime = (Convert.ToInt32(dGameSecondsFromResetPoint - dGameExtraSeconds) / SECONDS_PER_GAME_HOUR) + 1;
                string sTime = iTime.ToString().PadLeft(2, '0') + "00";
                sb.AppendLine($"Current time " + sTime + " for " + (SECONDS_PER_GAME_HOUR - dGameExtraSeconds).ToString("N1") + " seconds.");

                minutesIntoCycle = (DateTime.UtcNow - dtCycle).TotalMinutes % 4;
                minutesIntoCurrentCycle = minutesIntoCycle % 1;
                cycleNumber = Convert.ToInt32(minutesIntoCycle - minutesIntoCurrentCycle);
                secondsRemaining = (60 - (60 * (minutesIntoCycle - cycleNumber))).ToString("N1");
                switch (cycleNumber)
                {
                    case 0:
                        sb.AppendLine($"Celduin Express sailing to Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Harbringer sailing to Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Omani Princess sailing to Umbar for {secondsRemaining} seconds.");
                        sb.AppendLine($"Bullroarer sailing to Mithlond (glitched) for {secondsRemaining} seconds.");
                        break;
                    case 1:
                        sb.AppendLine($"Celduin Express in Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Harbringer in Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Omani Princess in Umbar for {secondsRemaining} seconds.");
                        sb.AppendLine($"Bullroarer in Mithlond (but cannot board) for {secondsRemaining} seconds.");
                        break;
                    case 2:
                        sb.AppendLine($"Celduin Express sailing to Bree for {secondsRemaining} seconds.");
                        sb.AppendLine($"Harbringer sailing to Tharbad for {secondsRemaining} seconds.");
                        sb.AppendLine($"Omani Princess sailing to Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Bullroarer sailing to Nindamos (glitched) for {secondsRemaining} seconds.");
                        break;
                    case 3:
                        sb.AppendLine($"Celduin Express in Bree for {secondsRemaining} seconds.");
                        sb.AppendLine($"Harbringer in Tharbad for {secondsRemaining} seconds.");
                        sb.AppendLine($"Omani Princess in Mithlond for {secondsRemaining} seconds.");
                        sb.AppendLine($"Bullroarer in Nindamos, also can board in Mithlond for {secondsRemaining} seconds.");
                        break;
                }
            }
            string message = sb.ToString();
            if (string.IsNullOrEmpty(message))
            {
                message = "No time information available.";
            }
            MessageBox.Show(message);
        }

        private double GetTimeRemainingSecondsForExit(BoatExitType boatExit)
        {
            double dRet = 0;
            if (boatExit != BoatExitType.None)
            {
                List<int> targetCycles = new List<int>();
                switch (boatExit)
                {
                    case BoatExitType.MithlondEnterCelduinExpress:
                    case BoatExitType.MithlondExitCelduinExpress:
                    case BoatExitType.MithlondEnterHarbringer:
                    case BoatExitType.MithlondExitHarbringer:
                    case BoatExitType.UmbarEnterOmaniPrincess:
                    case BoatExitType.UmbarExitOmaniPrincess:
                        targetCycles.Add(1);
                        break;
                    case BoatExitType.BreeEnterCelduinExpress:
                    case BoatExitType.BreeExitCelduinExpress:
                    case BoatExitType.TharbadEnterHarbringer:
                    case BoatExitType.MithlondEnterOmaniPrincess:
                    case BoatExitType.MithlondExitOmaniPrincess:
                        targetCycles.Add(3);
                        break;
                    case BoatExitType.NindamosEnterBullroarer:
                        targetCycles.Add(3);
                        break;
                    case BoatExitType.NindamosExitBullroarer:
                        targetCycles.Add(3);
                        break;
                    case BoatExitType.MithlondEnterBullroarer:
                        targetCycles.Add(3);
                        break;
                    case BoatExitType.MithlondExitBullroarer:
                        targetCycles.Add(1);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                DateTime dtCycle = _serverStartTime;
                double minutesIntoCycle = (DateTime.UtcNow - dtCycle).TotalMinutes % 4;
                double minutesIntoCurrentCycle = minutesIntoCycle % 1;
                int cycleNumber = Convert.ToInt32(minutesIntoCycle - minutesIntoCurrentCycle);
                if (!targetCycles.Contains(cycleNumber))
                {
                    dRet = 60 - (60 * (minutesIntoCycle - cycleNumber));
                    while (true)
                    {
                        cycleNumber = (cycleNumber + 1) % 4;
                        if (targetCycles.Contains(cycleNumber))
                            break;
                        else
                            dRet += 60;
                    }
                }
            }
            return dRet;
        }

        private void btnFightOne_Click(object sender, EventArgs e)
        {
            DoFight(AfterKillMonsterAction.StopCombat);
        }

        private void btnFightAll_Click(object sender, EventArgs e)
        {
            DoFight(AfterKillMonsterAction.SelectFirstMonsterInRoom);
        }

        private void DoFight(AfterKillMonsterAction afterKillMonsterAction)
        {
            MobTypeEnum? firstAttackableMob;
            lock (_currentEntityInfo.EntityLock)
            {
                firstAttackableMob = _currentEntityInfo.GetFirstAttackableMob();
            }
            if (!firstAttackableMob.HasValue)
            {
                MessageBox.Show("No attackable mobs in room.");
                return;
            }
            MobTypeEnum mobValue = firstAttackableMob.Value;
            Strategy defaultStrategy = null;
            foreach (Strategy s in _settingsData.Strategies)
            {
                if (s.IsDefault)
                {
                    defaultStrategy = s;
                    break;
                }
            }
            _settingsData.DynamicMobData.TryGetValue(mobValue, out DynamicMobData dmd);
            if (defaultStrategy == null && dmd?.Strategy == null)
            {
                MessageBox.Show("No strategy found.");
                return;
            }
            Strategy effectiveDefaultStrategy = defaultStrategy ?? dmd?.Strategy;
            effectiveDefaultStrategy.AfterKillMonsterAction = afterKillMonsterAction;
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.MobType = mobValue;
            bwp.MobTypeCounter = 1;
            bwp.Strategy = effectiveDefaultStrategy;
            RunBackgroundProcess(bwp);
        }
    }
}
