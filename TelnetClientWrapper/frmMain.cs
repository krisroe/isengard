using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
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
        private TcpClient _tcpClient;
        private NetworkStream _tcpClientNetworkStream;

        private IsengardSettingData _settingsData;
        private bool _processedUIWithSettings;

        private string _mob;
        private string _wand;

        private string _weapon;
        private string _weaponUI;

        private const int SECONDS_PER_GAME_HOUR = 150;
        private const int SUNRISE_GAME_HOUR = 6;
        private const int SUNSET_GAME_HOUR = 20;
        private int _time = -1; //time from 0 (midnight) to 23 (11 PM)
        private DateTime _timeLastUpdatedUTC = DateTime.MinValue;
        private int _timeUI = -1;
        private object _timeLock = new object();

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
        private ClassType _class;
        private int _level = 0;
        private string _currentPlayerHeader = null;
        private string _currentPlayerHeaderUI = null;

        private InitializationStep _initializationSteps;
        private InitialLoginInfo _loginInfo;

        private static DateTime? _currentStatusLastComputed;
        private static DateTime? _lastPollTick;

        private object _spellsCastLock = new object();
        private List<string> _spellsCast = new List<string>();
        private bool _refreshSpellsCast = false;

        /// <summary>
        /// current list of players. This list is not necessarily accurate since invisible players
        /// are hidden from the who output when you cannot detect them.
        /// </summary>
        private HashSet<string> _players = null;

        private string _username;
        private string _password;
        private bool _promptedUserName;
        private bool _promptedPassword;
        private bool _enteredUserName;
        private bool _enteredPassword;
        private int _totalhp = 0;
        private int _totalmp = 0;
        private double _armorClass = 0;
        private double _armorClassUI = 0;
        private string _armorClassText = string.Empty;
        private string _armorClassTextUI = string.Empty;
        private double _armorClassCalculated = 0;
        private double _armorClassCalculatedUI = 0;
        private int _gold = -1;
        private int _goldUI = -1;
        private int _experience = -1;
        private int _experienceUI = -1;
        private int _tnl = -1;
        private int _tnlUI = -1;
        private bool _poisoned;

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

        private bool _setTickOrPawnRoom = false;
        private BackgroundWorker _bw;
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
        private int _commandResultCounter = 0;
        private int _lastCommandDamage;
        private TrapType _lastCommandTrapType;
        private MovementResult? _lastCommandMovementResult;
        private string _lastCommand;
        private BackgroundCommandType? _backgroundCommandType;
        private Exit _currentBackgroundExit;
        private bool _currentBackgroundExitMessageReceived;

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

        /// <summary>
        /// number of times to try to attempt a background command before giving up
        /// </summary>
        private const int MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND = 20;

        /// <summary>
        /// time to wait before a single command times out
        /// </summary>
        private const int SINGLE_COMMAND_TIMEOUT_SECONDS = 5;

        private List<BackgroundCommandType> _backgroundSpells = new List<BackgroundCommandType>()
        {
            BackgroundCommandType.Vigor,
            BackgroundCommandType.MendWounds,
            BackgroundCommandType.Protection,
            BackgroundCommandType.CurePoison,
            BackgroundCommandType.Bless,
            BackgroundCommandType.Stun,
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

        internal frmMain(string userName, string password)
        {
            InitializeComponent();
            this.MinimumSize = this.Size;

            cboTickRoom.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }
            cboTickRoom.SelectedIndex = 0;
            cboPawnShop.Items.Add(string.Empty);
            foreach (var nextPawnShop in Enum.GetValues(typeof(PawnShoppe)))
            {
                cboPawnShop.Items.Add(nextPawnShop);
            }
            cboPawnShop.SelectedIndex = 0;

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

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

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
                for (int i = _settingsData.PermRuns.Count - 1; i >= 0; i--)
                {
                    bool isValid = true;
                    PermRun pr = _settingsData.PermRuns[i];
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
                            errorMessages.Add("Target room not found for perm run after reload.");
                        }
                    }
                    if (!isValid)
                    {
                        _settingsData.PermRuns.RemoveAt(i);
                    }
                }
                ProcessLocationRoomsAfterReload(_settingsData.Locations, newMap, errorMessages);
            }
            if (errorMessages.Count > 0)
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.AddRange(errorMessages);
                }
            }
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
                "flee",
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
            lock (_timeLock)
            {
                _time = -1;
                _timeLastUpdatedUTC = DateTime.MinValue;
            }
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
            lock (_currentEntityInfo.SkillsLock)
            {
                _currentEntityInfo.SkillsCooldowns.Clear();
            }
            lock (_spellsCastLock)
            {
                _spellsCast.Clear();
                _refreshSpellsCast = true;
            }
            ClearConsole();
            _currentStatusLastComputed = null;
            _promptedUserName = false;
            _promptedPassword = false;
            _enteredUserName = false;
            _enteredPassword = false;

            _tcpClient = new TcpClient(Program.HOST_NAME, Program.PORT);
            _tcpClientNetworkStream = _tcpClient.GetStream();
            BackgroundWorker _bwNetwork = new BackgroundWorker();
            _bwNetwork.DoWork += _bwNetwork_DoWork;
            _bwNetwork.RunWorkerCompleted += _bwNetwork_RunWorkerCompleted;
            _bwNetwork.RunWorkerAsync();
        }

        private void _bwNetwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!_backgroundCommandType.HasValue || _backgroundCommandType != BackgroundCommandType.Quit)
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

        private void OnInformation(FeedLineParameters flParams, int earth, int wind, int fire, int water, int divination, int arcana, int life, int sorcery, int experience, int tnl)
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

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Information, flParams);
            }
        }

        /// <summary>
        /// handler for the output of score
        /// </summary>
        private void OnScore(FeedLineParameters flParams, ClassType playerClass, int level, int maxHP, int maxMP, double armorClass, string armorClassText, int gold, int tnl, List<SkillCooldown> cooldowns, List<string> spells, bool poisoned)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Score) == InitializationStep.None;

            bool suppressEcho = forInit;

            lock (_currentEntityInfo.SkillsLock)
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
            }
            lock (_spellsCastLock)
            {
                _spellsCast.Clear();
                _spellsCast.AddRange(spells);
                _refreshSpellsCast = true;
            }

            _class = playerClass;
            _level = level;
            _totalhp = maxHP;
            _totalmp = maxMP;
            _armorClass = armorClass;
            _armorClassText = armorClassText;

            bool hasProtection;
            lock (_spellsCastLock)
            {
                hasProtection = _spellsCast.Contains("protection");
            }
            lock (_currentEntityInfo.EntityLock)
            {
                bool calculatedArmorClassSuccessful = true;
                double calculatedArmorClass = hasProtection ? 1 : 0;
                for (int i = 0; i < (int)EquipmentSlot.Count; i++)
                {
                    if (i != (int)EquipmentSlot.Held && i != (int)EquipmentSlot.Weapon1 && i != (int)EquipmentSlot.Weapon2)
                    {
                        ItemTypeEnum? nextEquipment = _currentEntityInfo.Equipment[i];
                        if (nextEquipment.HasValue)
                        {
                            StaticItemData sid = ItemEntity.StaticItemData[nextEquipment.Value];
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
                _armorClassCalculated = calculatedArmorClassSuccessful ? calculatedArmorClass : -1;
            }

            _gold = gold;
            _tnl = tnl;
            _currentPlayerHeader = _username + " (lvl " + level + ")";
            _poisoned = poisoned;

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Score, flParams);
            }

            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Score)
            {
                BackgroundWorkerParameters bwp = _currentBackgroundParameters;
                flParams.CommandResult = CommandResult.CommandSuccessful;
                suppressEcho = forInit || (bwp != null && !bwp.Foreground); //suppress output if not initiated explicitly by the user
            }

            flParams.SuppressEcho = suppressEcho;
        }

        private void OnSpells(FeedLineParameters flParams, List<string> SpellsList)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Spells) == InitializationStep.None;

            lock (_currentEntityInfo.SpellsKnownLock)
            {
                _currentEntityInfo.SpellsKnown = SpellsList;
            }

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Spells, flParams);
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
                ProcessInitialLogin(flp);
            }
            flp.SuppressEcho = true;
        }

        private void OnTime(FeedLineParameters flParams, int hour)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Time) == InitializationStep.None;

            lock (_timeLock)
            {
                if (_time != hour)
                {
                    _time = hour;
                    _timeLastUpdatedUTC = DateTime.UtcNow;
                }
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Time, flParams);
            }
        }

        private void OnEquipment(FeedLineParameters flParams, List<KeyValuePair<string, string>> equipment)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Equipment) == InitializationStep.None;

            lock (_currentEntityInfo.EntityLock)
            {
                EntityChange changes = new EntityChange();
                changes.ChangeType = EntityChangeType.RefreshEquipment;
                for (int i = 0; i < _currentEntityInfo.Equipment.Length; i++)
                {
                    _currentEntityInfo.Equipment[i] = null;
                }
                foreach (var nextEntry in equipment)
                {
                    var itemInfo = Entity.GetEntity(nextEntry.Key, EntityTypeFlags.Item, flParams.ErrorMessages, null, false) as ItemEntity;
                    if (itemInfo is UnknownItemEntity)
                    {
                        flParams.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)itemInfo).Name);
                    }
                    else if (itemInfo.Count != 1)
                    {
                        flParams.ErrorMessages.Add("Unexpected item count for worn equipment " + itemInfo.ItemType.Value.ToString() + ": " + itemInfo.Count);
                    }
                    else
                    {
                        ItemTypeEnum itemType = itemInfo.ItemType.Value;
                        if (Enum.TryParse(nextEntry.Value, out EquipmentType eqType))
                        {
                            bool foundSlot = false;
                            foreach (EquipmentSlot nextSlot in CurrentEntityInfo.GetSlotsForEquipmentType(eqType, false))
                            {
                                int iNextSlot = (int)nextSlot;
                                if (_currentEntityInfo.Equipment[iNextSlot] == null)
                                {
                                    _currentEntityInfo.Equipment[iNextSlot] = itemType;
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
                                flParams.ErrorMessages.Add("Unable to find equipment slot for: " + nextEntry.Value);
                            }
                        }
                        else
                        {
                            flParams.ErrorMessages.Add("Invalid equipment slot: " + nextEntry.Value);
                        }
                    }
                }
                if (changes.Changes.Count > 0)
                {
                    _currentEntityInfo.CurrentEntityChanges.Add(changes);
                }
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Equipment, flParams);
            }
        }

        private void OnInventory(FeedLineParameters flParams, List<ItemEntity> items, int TotalWeight)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Inventory) == InitializationStep.None;

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
            if (bct.HasValue && bct == BackgroundCommandType.Inventory)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                flParams.SuppressEcho = true;
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Inventory, flParams);
            }
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
            SendCommand("score", InputEchoType.Off);
            SendCommand("information", InputEchoType.Off);
            SendCommand("who", InputEchoType.Off);
            SendCommand("inventory", InputEchoType.Off);
            SendCommand("equipment", InputEchoType.Off);
            SendCommand("time", InputEchoType.Off);
            SendCommand("spells", InputEchoType.Off);
            if ((_initializationSteps & InitializationStep.RemoveAll) == InitializationStep.None)
            {
                SendCommand("remove all", InputEchoType.Off);
            }
            _initializationSteps |= InitializationStep.Initialization;
            _loginInfo = initialLoginInfo;
        }

        private void AfterLoadSettings()
        {
            _weapon = _settingsData.Weapon.HasValue ? _settingsData.Weapon.Value.ToString() : string.Empty;
            tsmiQuitWithoutSaving.Visible = _settingsData.SaveSettingsOnQuit;

            flpOneClickStrategies.Controls.Clear();
            int iOneClickTabIndex = 0;
            foreach (Strategy oStrategy in _settingsData.Strategies)
            {
                Button btnOneClick = new Button();
                btnOneClick.AutoSize = true;
                btnOneClick.TabIndex = iOneClickTabIndex++;
                btnOneClick.Tag = oStrategy;
                btnOneClick.Text = oStrategy.ToString();
                btnOneClick.UseVisualStyleBackColor = true;
                btnOneClick.Click += btnOneClick_Click;
                btnOneClick.ContextMenuStrip = ctxStrategy;
                flpOneClickStrategies.Controls.Add(btnOneClick);
            }
        }

        private void ProcessInitialLogin(FeedLineParameters flp)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.UserName = _username;
            sets.Save();

            InitialLoginInfo info = _loginInfo;
            string sRoomName = info.RoomName;
            if (RoomTransitionSequence.ProcessRoom(sRoomName, info.ObviousExits, info.List1, info.List2, info.List3, OnRoomTransition, flp, RoomTransitionType.Initial, 0, TrapType.None, false))
            {
                _initializationSteps |= InitializationStep.Finalization;
                Room r = _currentEntityInfo.CurrentRoom;
                _setTickOrPawnRoom = r != null && (r.HealingRoom.HasValue || r.PawnShoppe.HasValue);
            }
            else
            {
                flp.ErrorMessages.Add("Initial login failed!");
            }
        }

        private Room GetCurrentRoomIfUnambiguous(string sRoomName)
        {
            Room ret;
            _gameMap.UnambiguousRoomsByBackendName.TryGetValue(sRoomName, out ret);
            return ret;
        }

        private void OnRoomTransition(FeedLineParameters flParams, RoomTransitionInfo roomTransitionInfo, int damage, TrapType trapType)
        {
            Exit currentBackgroundExit = _currentBackgroundExit;
            RoomTransitionType rtType = roomTransitionInfo.TransitionType;
            string sRoomName = roomTransitionInfo.RoomName;
            List<string> obviousExits = roomTransitionInfo.ObviousExits;

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
                    AddOrRemoveItemsFromInventoryOrEquipment(flParams, new List<ItemEntity>() { new ItemEntity(ItemTypeEnum.HazyPotion, 1, 1) }, ItemManagementAction.ConsumeItem);
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
                _lastCommandTrapType = trapType;
                _lastCommandMovementResult = MovementResult.Success;
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
                        if (nextExit.PresenceType == ExitPresenceType.Periodic || nextExit.WaitForMessage.HasValue)
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
                            ItemTypeEnum? itemType = nextItem.ItemType;
                            if (itemType.HasValue)
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
                    if (errorMessages.Count > 0)
                    {
                        lock (_broadcastMessagesLock)
                        {
                            _broadcastMessages.AddRange(errorMessages);
                        }
                    }
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
                else if (e.PresenceType != ExitPresenceType.Periodic)
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
            ChangeSkillActive(SkillWithCooldownType.Manashield, true);
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Manashield)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
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

        private void ChangeSkillActive(SkillWithCooldownType skill, bool active)
        {
            lock (_currentEntityInfo.SkillsLock)
            {
                foreach (SkillCooldown nextCooldown in _currentEntityInfo.SkillsCooldowns)
                {
                    if (nextCooldown.SkillType == skill)
                    {
                        nextCooldown.Status = active ? SkillCooldownStatus.Active : SkillCooldownStatus.Inactive;
                        nextCooldown.NextAvailable = DateTime.MinValue;
                        break;
                    }
                }
            }
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

        private void OnSelfSpellCast(FeedLineParameters flParams, BackgroundCommandType? commandType, string activeSpell, List<ItemEntity> consumedItems)
        {
            if (!string.IsNullOrEmpty(activeSpell))
            {
                AddActiveSpell(activeSpell);
            }
            if (consumedItems != null && consumedItems.Count > 0)
            {
                AddOrRemoveItemsFromInventoryOrEquipment(flParams, consumedItems, ItemManagementAction.ConsumeItem);
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && commandType.HasValue && bct.Value == commandType.Value)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void AddActiveSpell(string spellName)
        {
            AddActiveSpells(new List<string>() { spellName });
        }

        private void AddActiveSpells(List<string> spellNames)
        {
            bool changed = false;
            if (spellNames != null && spellNames.Count > 0)
            {
                lock (_spellsCast)
                {
                    foreach (string nextSpell in spellNames)
                    {
                        if (!_spellsCast.Contains(nextSpell))
                        {
                            if (_spellsCast.Contains("None"))
                            {
                                _spellsCast.Remove("None");
                            }
                            _spellsCast.Add(nextSpell);
                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        _refreshSpellsCast = true;
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
                    _lastCommandMovementResult = movementResult;

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

        private static void FailSearch(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Search)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
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

        private void SuccessfulSearch(List<string> exits, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Search)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                _foundSearchedExits = exits;
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

        private static void FailKnock(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Knock)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
            }
        }

        private void FailItemAction(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.DrinkHazy || bctValue == BackgroundCommandType.DrinkNonHazyPotion || bctValue == BackgroundCommandType.SellItem || bctValue == BackgroundCommandType.DropItem)
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
        private static void OnSeeNothingSpecial(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.LookAtMob)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OnCannotPickUpItem(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
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

        private static void OnCannotCarryAnymore(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.GetItem)
            {
                //fixable if the user drops stuff
                flParams.CommandResult = CommandResult.CommandUnsuccessfulThisTime;
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
            if (powerAttacked)
            {
                ChangeSkillActive(SkillWithCooldownType.PowerAttack, false);
            }
            _experience += experience;
            _tnl = Math.Max(0, _tnl - experience);
            if (fumbled)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    ItemTypeEnum? weaponIT = _currentEntityInfo.Equipment[(int)EquipmentSlot.Weapon1];
                    if (weaponIT.HasValue)
                    {
                        AddOrRemoveItemsFromInventoryOrEquipment(flParams, new List<ItemEntity>() { new ItemEntity(weaponIT.Value, 1, 1) }, ItemManagementAction.Unequip);
                    }
                }
            }
            lock (_currentEntityInfo.EntityLock)
            {
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
        private static void OnYouDontSeeThatHere(FeedLineParameters flParams)
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
                        flParams.SuppressEcho = true;
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
            if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
            }
        }

        private static void OnCastOffensiveSpellMobNotPresent(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if (bctValue == BackgroundCommandType.OffensiveSpell || bctValue == BackgroundCommandType.Stun)
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
                if (bctValue == BackgroundCommandType.OffensiveSpell || bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.Attack)
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
                    _currentMonsterStatus = status;
                }
            }
        }

        private void OnInformationalMessages(FeedLineParameters flp, List<string> broadcasts, List<string> addedPlayers, List<string> removedPlayers)
        {
            List<InformationalMessages> infoMsgs = flp.InfoMessages;
            List<string> spellsOff = null;
            bool finishedProcessing = false;
            Exit currentBackgroundExit = _currentBackgroundExit;
            EntityChange rc;
            BackgroundCommandType? bct = flp.BackgroundCommandType;
            bool isFightingMob = flp.IsFightingMob;
            foreach (InformationalMessages next in infoMsgs)
            {
                InformationalMessageType nextMessage = next.MessageType;

                if (currentBackgroundExit != null && currentBackgroundExit.WaitForMessage.HasValue && currentBackgroundExit.WaitForMessage.Value == nextMessage)
                {
                    _currentBackgroundExitMessageReceived = true;
                }

                switch (nextMessage)
                {
                    case InformationalMessageType.DayStart:
                        _time = SUNRISE_GAME_HOUR;
                        _timeLastUpdatedUTC = DateTime.UtcNow;
                        break;
                    case InformationalMessageType.NightStart:
                        _time = SUNSET_GAME_HOUR;
                        _timeLastUpdatedUTC = DateTime.UtcNow;
                        break;
                    case InformationalMessageType.BlessOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("bless");
                        break;
                    case InformationalMessageType.ProtectionOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("protection");
                        break;
                    case InformationalMessageType.FlyOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("fly");
                        break;
                    case InformationalMessageType.LevitationOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("levitation");
                        break;
                    case InformationalMessageType.InvisibilityOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("invisibility");
                        break;
                    case InformationalMessageType.DetectInvisibleOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("detect-invisible");
                        break;
                    case InformationalMessageType.KnowAuraOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("know-aura");
                        break;
                    case InformationalMessageType.EndureFireOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("endure-fire");
                        break;
                    case InformationalMessageType.EndureColdOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("endure-cold");
                        break;
                    case InformationalMessageType.EndureEarthOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("endure-earth");
                        break;
                    case InformationalMessageType.EndureWaterOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("endure-water");
                        break;
                    case InformationalMessageType.DetectMagicOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("detect-magic");
                        break;
                    case InformationalMessageType.LightOver:
                        if (spellsOff == null) spellsOff = new List<string>();
                        spellsOff.Add("light");
                        break;
                    case InformationalMessageType.ManashieldOff:
                        ChangeSkillActive(SkillWithCooldownType.Manashield, false);
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
                        if (bct.HasValue && bct.Value == BackgroundCommandType.Stun)
                        {
                            flp.CommandResult = CommandResult.CommandSuccessful;
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInMithlond, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInNindamos, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "dock"));
                                        removeMessage = false;
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "steamboat"));
                                        removeMessage = false;
                                        break;
                                }
                            }
                            if (removeMessage)
                            {
                                for (int i = 0; i < flp.Lines.Count; i++)
                                {
                                    if (flp.Lines[i] == InformationalMessagesSequence.CELDUIN_EXPRESS_IN_BREE_MESSAGE)
                                    {
                                        flp.Lines.RemoveAt(i);
                                        break;
                                    }
                                }
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "dock"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "steamboat"));
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
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "pier"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressMithlond:
                                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "gangway"));
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
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.DestroyEquipment);
                        break;
                    case InformationalMessageType.EquipmentFellApart:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.Unequip);
                        break;
                    case InformationalMessageType.WeaponIsBroken:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.Unequip);
                        if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
                        {
                            _lastCommandDamage = 0;
                            flp.CommandResult = CommandResult.CommandSuccessful;
                        }
                        break;
                    case InformationalMessageType.ItemMagicallySentToYou:
                        AddOrRemoveItemsFromInventoryOrEquipment(flp, new List<ItemEntity>() { next.Item }, ItemManagementAction.MagicallySentItem);
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
                }
            }
            if (spellsOff != null)
            {
                lock (_spellsCast)
                {
                    foreach (string nextSpell in spellsOff)
                    {
                        if (_spellsCast.Contains(nextSpell))
                        {
                            _spellsCast.Remove(nextSpell);
                            if (_spellsCast.Count == 0)
                            {
                                _spellsCast.Add("None");
                            }
                            _refreshSpellsCast = true;
                        }
                    }
                }
            }

            if (broadcasts != null)
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.AddRange(broadcasts);
                }
            }

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

            if (finishedProcessing)
            {
                flp.FinishedProcessing = true;
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
                    string sExit = null;
                    switch (currentRoom.BoatLocationType.Value)
                    {
                        case BoatEmbarkOrDisembark.Harbringer:
                            sExit = "ship";
                            break;
                        case BoatEmbarkOrDisembark.HarbringerMithlond:
                            sExit = "gangplank";
                            break;
                        case BoatEmbarkOrDisembark.HarbringerTharbad:
                            sExit = "gangway";
                            break;
                    }
                    if (sExit != null)
                    {
                        _currentEntityInfo.CurrentEntityChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, inPort, sExit));
                    }
                }
            }
        }

        /// <summary>
        /// gets a room change object for a periodic exit
        /// </summary>
        /// <param name="currentRoom">current room</param>
        /// <param name="add">true to add the exit, false to remove the exit</param>
        /// <param name="exitText">exit text</param>
        /// <returns>room change object</returns>
        private EntityChange GetAddExitRoomChangeForPeriodicExit(Room currentRoom, bool add, string exitText)
        {
            EntityChange rc = new EntityChange();
            rc.ChangeType = add ? EntityChangeType.AddExit : EntityChangeType.RemoveExit;
            Exit e = IsengardMap.GetRoomExits(currentRoom, (exit) => { return exit.PresenceType == ExitPresenceType.Periodic && exit.ExitText == exitText; }).First();
            rc.Exits.Add(e.ExitText);
            if (add)
            {
                rc.MappedExits[e.ExitText] = e;
            }
            return rc;
        }

        /// <summary>
        /// gets a room change object for a wait for message exit
        /// </summary>
        /// <param name="currentRoom">current room</param>
        /// <param name="messageType">message type</param>
        /// <param name="add">true to add the exit, false to remove the exit</param>
        /// <returns>room change object</returns>
        private EntityChange GetAddExitRoomChangeForWaitForMessageExit(Room currentRoom, InformationalMessageType messageType, bool add)
        {
            EntityChange rc = new EntityChange();
            rc.ChangeType = add ? EntityChangeType.AddExit : EntityChangeType.RemoveExit;
            Exit e = IsengardMap.GetRoomExits(currentRoom, (exit) => { return exit.WaitForMessage.HasValue && exit.WaitForMessage.Value == messageType; }).First();
            rc.Exits.Add(e.ExitText);
            if (add)
            {
                rc.MappedExits[e.ExitText] = e;
            }
            return rc;
        }

        private void OnInventoryManagement(FeedLineParameters flParams, List<ItemEntity> items, ItemManagementAction action, int? gold, int sellGold, List<string> activeSpells, bool potionConsumed)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.RemoveAll) == InitializationStep.None;
            bool hasItems = items != null && items.Count > 0;
            bool hasSpells = activeSpells != null && activeSpells.Count > 0;
            bool hasGold = gold.HasValue || sellGold > 0;
            bool couldBeRemoveAll = !hasSpells && !hasGold && action == ItemManagementAction.Unequip;
            if (hasItems)
            {
                AddOrRemoveItemsFromInventoryOrEquipment(flParams, items, action);
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
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue)
            {
                BackgroundCommandType bctValue = bct.Value;
                if ((action == ItemManagementAction.PickUpItem && bctValue == BackgroundCommandType.GetItem) ||
                    (action == ItemManagementAction.SellItem && bctValue == BackgroundCommandType.SellItem) ||
                    (action == ItemManagementAction.DropItem && bctValue == BackgroundCommandType.DropItem) ||
                    (action == ItemManagementAction.Unequip && bctValue == BackgroundCommandType.RemoveEquipment))
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                }
            }
            if (forInit && couldBeRemoveAll)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.RemoveAll, flParams);
            }
        }

        private void AddOrRemoveItemsFromInventoryOrEquipment(FeedLineParameters flParams, List<ItemEntity> items, ItemManagementAction action)
        {
            EntityChangeType changeType;
            List<ItemEntity> pickupItemsMoney = null;
            if (action == ItemManagementAction.Equip)
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
                pickupItemsMoney = new List<ItemEntity>();
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
            EntityChange iec = new EntityChange();
            iec.ChangeType = changeType;
            lock (_currentEntityInfo.EntityLock)
            {
                foreach (ItemEntity nextItemEntity in items)
                {
                    bool addChange = false;
                    EntityChangeEntry changeEntry = new EntityChangeEntry();
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
                                        _currentEntityInfo.Equipment[iSlotIndex] = sid.ItemType;
                                        iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, false, changeEntry);
                                        addChange = true;
                                        break;
                                    }
                                }
                                else //remove
                                {
                                    if (_currentEntityInfo.Equipment[iSlotIndex] == nextItem)
                                    {
                                        changeEntry.EquipmentIndex = _currentEntityInfo.FindEquipmentRemovalPoint(nextSlot);
                                        changeEntry.EquipmentAction = false;
                                        _currentEntityInfo.Equipment[iSlotIndex] = null;
                                        if (changeType == EntityChangeType.UnequipItem)
                                        {
                                            iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, true, changeEntry);
                                        }
                                        addChange = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            flParams.ErrorMessages.Add("Equipment type not found for: " + nextItem);
                        }
                    }
                    else //equipment not involved
                    {
                        if (sid == null || (sid.ItemClass != ItemClass.Money && sid.ItemClass != ItemClass.Coins))
                        {
                            //add/remove from inventory
                            bool isAddToInventory = changeType == EntityChangeType.PickUpItem || changeType == EntityChangeType.MagicallySentItem;
                            addChange |= iec.AddOrRemoveEntityItemFromInventory(_currentEntityInfo, nextItemEntity, isAddToInventory, changeEntry);
                            if (changeType == EntityChangeType.PickUpItem) //remove from room items
                            {
                                addChange |= iec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, nextItemEntity, false, changeEntry);
                            }
                        }
                        else if (changeType == EntityChangeType.PickUpItem)
                        {
                            pickupItemsMoney.Add(nextItemEntity); //picking up money handled below since it doesn't go into inventory
                        }
                        if (changeType == EntityChangeType.DropItem)
                        {
                            addChange |= iec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, nextItemEntity, true, changeEntry);
                        }
                    }
                    if (addChange)
                    {
                        iec.Changes.Add(changeEntry);
                    }
                }
                if (iec.Changes.Count > 0)
                {
                    _currentEntityInfo.CurrentEntityChanges.Add(iec);
                }
                if (pickupItemsMoney != null && pickupItemsMoney.Count > 0)
                {
                    iec = new EntityChange();
                    iec.ChangeType = EntityChangeType.RemoveRoomItems;
                    foreach (ItemEntity ie in pickupItemsMoney)
                    {
                        EntityChangeEntry changeEntry = new EntityChangeEntry();
                        changeEntry.Item = ie;
                        if (iec.AddOrRemoveEntityItemFromRoomItems(_currentEntityInfo, ie, false, changeEntry))
                        {
                            iec.Changes.Add(changeEntry);
                        }
                    }
                    if (iec.Changes.Count > 0)
                    {
                        _currentEntityInfo.CurrentEntityChanges.Add(iec);
                    }
                }
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
                            flParams.PlayerNames = _players;
                            int previousCommandResultCounter = _commandResultCounter;
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
                                if (flParams.SuppressEcho && !GetVerboseMode())
                                {
                                    echoType = InputEchoType.Off;
                                }
                                if (flParams.FinishedProcessing)
                                {
                                    break;
                                }
                            }

                            if (flParams.ErrorMessages.Count > 0)
                            {
                                lock (_broadcastMessagesLock)
                                {
                                    foreach (string s in flParams.ErrorMessages)
                                    {
                                        _broadcastMessages.Add(s);
                                    }
                                }
                            }

                            //recompute the lines if they were changed by sequence logic
                            bool linesChanged = sNewLinesList.Count != initialCount;
                            if (linesChanged)
                            {
                                sNewLine = string.Join(Environment.NewLine, sNewLinesList);
                            }
                            haveContent = !string.IsNullOrWhiteSpace(sNewLine);
                            if (haveContent)
                            {
                                int newCommandResultCounter = _commandResultCounter;
                                if (!GetVerboseMode() && previousCommandResultCounter == newCommandResultCounter && !previousCommandResult.HasValue && flParams.CommandResult.HasValue)
                                {
                                    sNewLine = _lastCommand + Environment.NewLine + sNewLine;
                                }
                                if (flParams.CommandResult.HasValue)
                                {
                                    _commandResult = flParams.CommandResult.Value;
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
                    AddConsoleMessage(messages);
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
                new InformationalMessagesSequence(_username, OnInformationalMessages),
                new InitialLoginSequence(OnInitialLogin),
                new ScoreOutputSequence(_username, OnScore),
                new InformationOutputSequence(OnInformation),
                new WhoOutputSequence(OnWho),
                new SpellsSequence(OnSpells),
                new InventorySequence(OnInventory),
                new EquipmentSequence(OnEquipment),
                new MobStatusSequence(OnMobStatusSequence),
                new TimeOutputSequence(OnTime),
                _pleaseWaitSequence,
                new RoomTransitionSequence(OnRoomTransition),
                new FailMovementSequence(FailMovement),
                new EntityAttacksYouSequence(OnEntityAttacksYou),
                new InventoryEquipmentManagementSequence(OnInventoryManagement),
                new ConstantOutputSequence("You creative a protective manashield.", OnManashieldOn, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("You already have a manashield!", OnManashieldOn, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("You create a protective fireshield.", OnFireshieldOn, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Fireshield),
                new ConstantOutputSequence("You already have a fireshield active!", OnFireshieldOn, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Fireshield),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains, null), //could be prefixed by "Scared of going X"*
                new SelfSpellCastSequence(OnSelfSpellCast),
                new ConstantOutputSequence("Your spell fails.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. alignment out of whack
                new ConstantOutputSequence("You don't know that spell.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells),
                new ConstantOutputSequence("Nothing happens.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. casting a spell from the tree of life
                new AttackSequence(OnAttack),
                new CastOffensiveSpellSequence(OnCastOffensiveSpell),
                new ConstantOutputSequence("You don't see that here.", OnYouDontSeeThatHere, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Attack, BackgroundCommandType.LookAtMob }),
                new ConstantOutputSequence("That is not here.", OnThatIsNotHere, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Attack), //triggered by power attack
                new ConstantOutputSequence("That's not here.", OnCastOffensiveSpellMobNotPresent, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun }),
                new ConstantOutputSequence("You cannot harm him.", OnTryAttackUnharmableMob, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun, BackgroundCommandType.Attack }),
                new ConstantOutputSequence("You cannot harm her.", OnTryAttackUnharmableMob, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun, BackgroundCommandType.Attack }),
                new ConstantOutputSequence("It's not locked.", SuccessfulKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You successfully open the lock.", SuccessfulKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You failed.", FailKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You don't have that.", FailItemAction, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion, BackgroundCommandType.SellItem, BackgroundCommandType.DropItem }),
                new ConstantOutputSequence(" starts to evaporates before you drink it.", FailItemAction, ConstantSequenceMatchType.EndsWith, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion }),
                new ConstantOutputSequence("You prepare yourself for traps.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("You've already prepared.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("I don't see that exit.", CantSeeExit, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("You open the ", OpenDoorSuccess, ConstantSequenceMatchType.StartsWith, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("It's already open.", OpenDoorSuccess, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("You see nothing special about it.", OnSeeNothingSpecial, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.LookAtMob),
                new ConstantOutputSequence("That isn't here.", OnCannotPickUpItem, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.GetItem),
                new ConstantOutputSequence("You may not take that. You have already fulfilled that quest.", OnCannotPickUpItem, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.GetItem),
                new ConstantOutputSequence("You can't carry anymore.", OnCannotCarryAnymore, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.GetItem),
                new ConstantOutputSequence("You can't take that!", OnCannotPickUpItem, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.GetItem),
                new ConstantOutputSequence(" won't let you take anything.", OnCannotPickUpItem, ConstantSequenceMatchType.EndsWith, 0, BackgroundCommandType.GetItem),
                new ConstantOutputSequence("The shopkeep says, \"I won't buy that crap from you.\"", OnCannotSellItem, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.SellItem),
                new ConstantOutputSequence("The shopkeep won't buy that from you.", OnCannotSellItem, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.SellItem),
                new ConstantOutputSequence("You aren't using that.", OnCannotRemoveEquipment, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.RemoveEquipment),
                new ConstantOutputSequence("You can't.  It's cursed!", OnCannotRemoveEquipment, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.RemoveEquipment),
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

        private void btnOneClick_Click(object sender, EventArgs e)
        {
            RunStrategy((Strategy)((Button)sender).Tag);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp.SingleCommandType.HasValue && bwp.SingleCommandType.Value == BackgroundCommandType.Quit && bwp.SingleCommandResult == CommandResult.CommandSuccessful)
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
                TimeSpan permRunTime;
                if (bwp.Success && bwp.PermRunStart != DateTime.MinValue)
                {
                    permRunTime = DateTime.UtcNow - bwp.PermRunStart;
                    int goldDiff = _gold - bwp.BeforeGold;
                    int xpDiff = _experience - bwp.BeforeExperience;
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
                    AddConsoleMessage(messages);
                }
                _commandResult = null;
                _lastCommand = null;
                _lastCommandDamage = 0;
                _lastCommandTrapType = TrapType.None;
                _lastCommandMovementResult = null;
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
                    List<MobTypeEnum> currentRoomMobs = _currentEntityInfo.CurrentRoomMobs;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        if (currentRoomMobs.Count > 0)
                        {
                            sText = _currentEntityInfo.PickMobTextFromMobCounter(null, MobLocationType.CurrentRoomMobs, currentRoomMobs[0], 1, false, true);
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

                if (bwp.Success && bwp.PermRun != null)
                {
                    if (bwp.PermRun.TickRoom.HasValue)
                    {
                        cboTickRoom.SelectedItem = bwp.PermRun.TickRoom.Value;
                    }
                    if (bwp.PermRun.PawnShop.HasValue && bwp.UsedPawnShoppe)
                    {
                        cboPawnShop.SelectedItem = bwp.PermRun.PawnShop.Value;
                    }
                }

                ToggleBackgroundProcessUI(bwp, false);
                _currentBackgroundParameters = null;
                RefreshEnabledForSingleMoveButtons();
            }
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            try
            {
                if (pms.SingleCommandType.HasValue)
                {
                    BackgroundCommandType cmdType = pms.SingleCommandType.Value;
                    CommandResult commandResult;
                    if (cmdType == BackgroundCommandType.Look)
                    {
                        commandResult = RunSingleCommandForCommandResult(pms.SingleCommandType.Value, "look", pms, null, false);
                    }
                    else
                    {
                        bool success;
                        if (cmdType == BackgroundCommandType.Search)
                        {
                            success = RunSingleCommand(BackgroundCommandType.Search, "search", pms, null, false) == CommandResult.CommandSuccessful;
                        }
                        else if (cmdType == BackgroundCommandType.Quit)
                        {
                            success = RunSingleCommand(BackgroundCommandType.Quit, "quit", pms, null, false) == CommandResult.CommandSuccessful;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        commandResult = success ? CommandResult.CommandSuccessful : CommandResult.CommandUnsuccessfulThisTime;
                    }
                    pms.SingleCommandResult = commandResult;
                    return;
                }

                Strategy strategy = pms.Strategy;
                CommandResult backgroundCommandResult = CommandResult.CommandSuccessful;

                if ((pms.PermRun != null && pms.PermRun.FullBeforeStarting) || pms.CureIfPoisoned)
                {
                    //cure-poison if needed
                    _poisoned = false;
                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null, true);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful) return;
                    bool healPoison = _poisoned;
                    if (pms.CureIfPoisoned && !healPoison) //display a message if a standalone curepoison was attempted
                    {
                        AddConsoleMessage("Not poisoned, thus cure-poison not cast.");
                        return;
                    }
                    if (!_fleeing && !_hazying && healPoison)
                    {
                        SpellInformationAttribute sia = SpellsStatic.SpellsByEnum[SpellsEnum.curepoison];
                        if (_automp < sia.Mana)
                        {
                            return;
                        }
                        else
                        {
                            backgroundCommandResult = CastSpellOnSelf("cure-poison", pms);
                            if (backgroundCommandResult != CommandResult.CommandSuccessful && backgroundCommandResult != CommandResult.CommandEscaped)
                            {
                                return;
                            }
                        }
                    }
                    if (pms.CureIfPoisoned) return; //for standalone cure-poison that's all we need to do

                    if (!_fleeing && !_hazying && pms.PermRun != null && pms.PermRun.FullBeforeStarting && !IsFull(pms.PermRun.SpellsToCast))
                    {
                        if (!pms.PermRun.TickRoom.HasValue)
                        {
                            return;
                        }
                        backgroundCommandResult = NavigateToTickRoom(pms, true);
                        if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                        {
                            return;
                        }
                        if (!_hazying && !_fleeing)
                        {
                            backgroundCommandResult = GetFullInBackground(pms, true);
                            if (backgroundCommandResult != CommandResult.CommandSuccessful && backgroundCommandResult != CommandResult.CommandEscaped)
                            {
                                return;
                            }
                        }
                    }
                }
                if (!_fleeing && !_hazying && pms.PermRun != null)
                {
                    pms.PermRunStart = DateTime.UtcNow;
                }

                bool haveThreshold = pms.PermRun != null && pms.PermRun.ThresholdRoomObject != null;
                if (!_fleeing && !_hazying)
                {
                    bool moved = true;
                    if (haveThreshold)
                        backgroundCommandResult = NavigateToSpecificRoom(pms.PermRun.ThresholdRoomObject, pms, true);
                    else if (pms.TargetRoom != null)
                        backgroundCommandResult = NavigateToSpecificRoom(pms.TargetRoom, pms, true);
                    else if (pms.Exits != null && pms.Exits.Count > 0)
                        backgroundCommandResult = TraverseExitsAlreadyInBackground(pms.Exits, pms, true);
                    else
                        moved = false;
                    if (moved)
                    {
                        if (backgroundCommandResult == CommandResult.CommandSuccessful)
                        {
                            pms.AtDestination = !haveThreshold;
                        }
                        else if (backgroundCommandResult != CommandResult.CommandEscaped)
                        {
                            return;
                        }
                    }
                }

                PromptedSkills skillsToRun = pms.PermRun == null ? PromptedSkills.None : pms.PermRun.SkillsToRun;

                //activate potions/skills at the threshold if there is a threshold
                if (!_fleeing && !_hazying && haveThreshold)
                {
                    backgroundCommandResult = ActivatePotionsAndSkills(pms, skillsToRun);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful && backgroundCommandResult != CommandResult.CommandEscaped)
                    {
                        return;
                    }
                }

                if (!_fleeing && !_hazying && haveThreshold)
                {
                    backgroundCommandResult = NavigateToSpecificRoom(pms.TargetRoom, pms, true);
                    if (backgroundCommandResult == CommandResult.CommandSuccessful)
                    {
                        pms.AtDestination = true;
                    }
                    else if (backgroundCommandResult != CommandResult.CommandEscaped)
                    {
                        return;
                    }
                }

                //verify the mob is present and attackable before activating skills
                if (_bw.CancellationPending) return;
                if (!_hazying && !_fleeing && pms.ExpectsMob() && !AttackIsGoodToGo(pms))
                {
                    AddConsoleMessage("Target mob not present.");
                    return;
                }

                //activate potions/skills at the target if there is no threshold
                if (!_fleeing && !_hazying && !haveThreshold)
                {
                    backgroundCommandResult = ActivatePotionsAndSkills(pms, skillsToRun);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful && backgroundCommandResult != CommandResult.CommandEscaped)
                    {
                        return;
                    }
                }

                bool hasInitialQueuedMagicStep;
                bool hasInitialQueuedMeleeStep;
                bool hasInitialQueuedPotionsStep;
                lock (_queuedCommandLock)
                {
                    hasInitialQueuedMagicStep = pms.QueuedMagicStep.HasValue;
                    hasInitialQueuedMeleeStep = pms.QueuedMeleeStep.HasValue;
                    hasInitialQueuedPotionsStep = pms.QueuedPotionsStep.HasValue;
                }
                bool hasCombat = pms.HasTargetMob();
                string sMobText;
                if (hasCombat)
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
                if (_hazying || _fleeing || strategy != null || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep)
                {
                    try
                    {
                        _mob = sMobText;
                        _currentlyFightingMobText = pms.MobText;
                        _currentlyFightingMobType = pms.MobType;
                        _currentlyFightingMobCounter = pms.MobType.HasValue ? pms.MobTypeCounter : pms.MobTextCounter;
                        bool useManaPool = false;
                        AfterKillMonsterAction onMonsterKilledAction = AfterKillMonsterAction.ContinueCombat;
                        int usedAutoSpellMin = _settingsData.AutoSpellLevelMin;
                        int usedAutoSpellMax = _settingsData.AutoSpellLevelMax;

                        bool haveMeleeStrategySteps = false;
                        bool haveMagicStrategySteps = false;
                        bool havePotionsStrategySteps = false;
                        if (strategy != null)
                        {
                            useManaPool = strategy.ManaPool > 0;
                            haveMagicStrategySteps = strategy.HasAnyMagicSteps();
                            haveMeleeStrategySteps = strategy.HasAnyMeleeSteps();
                            havePotionsStrategySteps = strategy.HasAnyPotionsSteps();
                            onMonsterKilledAction = strategy.AfterKillMonsterAction;
                            if (strategy.AutoSpellLevelMin != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET && strategy.AutoSpellLevelMax != IsengardSettingData.AUTO_SPELL_LEVEL_NOT_SET)
                            {
                                usedAutoSpellMin = strategy.AutoSpellLevelMin;
                                usedAutoSpellMax = strategy.AutoSpellLevelMax;
                            }
                        }
                        List<string> offensiveSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_settingsData.Realm);
                        int realmProficiency = 0;
                        switch (_settingsData.Realm)
                        {
                            case RealmType.Earth:
                                realmProficiency = _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Earth];
                                break;
                            case RealmType.Wind:
                                realmProficiency = _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Wind];
                                break;
                            case RealmType.Fire:
                                realmProficiency = _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Fire];
                                break;
                            case RealmType.Water:
                                realmProficiency = _currentEntityInfo.UserSpellProficiencies[SpellProficiency.Water];
                                break;
                        }
                        List<string> knownSpells;
                        lock (_currentEntityInfo.SpellsKnownLock)
                        {
                            knownSpells = _currentEntityInfo.SpellsKnown;
                        }
                        int? calculatedMinLevel, calculatedMaxLevel;
                        Strategy.GetMinMaxOffensiveSpellLevels(strategy, usedAutoSpellMin, usedAutoSpellMax, knownSpells, offensiveSpells, out calculatedMinLevel, out calculatedMaxLevel);

                        _monsterDamage = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunnedSince = null;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        _monsterKilledItems.Clear();
                        if (useManaPool)
                        {
                            _currentMana = strategy.ManaPool;
                        }
                        ItemTypeEnum? weaponItem = _settingsData.Weapon;
                        bool useMelee = haveMeleeStrategySteps || hasInitialQueuedMeleeStep;
                        if (haveMagicStrategySteps || haveMeleeStrategySteps || havePotionsStrategySteps || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep || hasInitialQueuedPotionsStep)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Combat;
                            bool doPowerAttack = false;
                            if (useMelee)
                            {
                                WieldWeapon(weaponItem);
                                doPowerAttack = (skillsToRun & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
                            }
                            IEnumerator<MagicStrategyStep> magicSteps = strategy?.GetMagicSteps().GetEnumerator();
                            IEnumerator<MeleeStrategyStep> meleeSteps = strategy?.GetMeleeSteps(doPowerAttack).GetEnumerator();
                            IEnumerator<PotionsStrategyStep> potionsSteps = strategy?.GetPotionsSteps().GetEnumerator();
                            MagicStrategyStep? nextMagicStep = null;
                            MeleeStrategyStep? nextMeleeStep = null;
                            PotionsStrategyStep? nextPotionsStep = null;
                            bool magicStepsEnabled = strategy != null && ((strategy.TypesWithStepsEnabled & CommandType.Magic) != CommandType.None);
                            bool meleeStepsEnabled = strategy != null && ((strategy.TypesWithStepsEnabled & CommandType.Melee) != CommandType.None);
                            bool potionsStepsEnabled = strategy != null && ((strategy.TypesWithStepsEnabled & CommandType.Potions) != CommandType.None);
                            bool magicStepsFinished = magicSteps == null || !magicSteps.MoveNext();
                            bool meleeStepsFinished = meleeSteps == null || !meleeSteps.MoveNext();
                            bool potionsStepsFinished = potionsSteps == null || !potionsSteps.MoveNext();
                            int? magicOnlyWhenStunnedAfterXMS = strategy?.MagicOnlyWhenStunnedForXMS;
                            int? meleeOnlyWhenStunnedAfterXMS = strategy?.MeleeOnlyWhenStunnedForXMS;
                            int? potionsOnlyWhenStunnedAfterXMS = strategy?.PotionsOnlyWhenStunnedForXMS;
                            if (!magicStepsFinished) nextMagicStep = magicSteps.Current;
                            if (!meleeStepsFinished) nextMeleeStep = meleeSteps.Current;
                            if (!potionsStepsFinished) nextPotionsStep = potionsSteps.Current;
                            DateTime? dtNextMagicCommand = null;
                            DateTime? dtNextMeleeCommand = null;
                            DateTime? dtNextPotionsCommand = null;
                            DateTime dtUtcNow;
                            bool allowBasedOnStun;
                            string command;
                            while (true) //combat cycle
                            {
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

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
                                    string sMobTarget = GetMobTarget(false);
                                    if (string.IsNullOrEmpty(sMobTarget))
                                    {
                                        AddConsoleMessage("Target mob not found.");
                                        return;
                                    }

                                    int currentMana = useManaPool ? _currentMana : _automp;
                                    int currentHP = _autohp;
                                    int manaDrain;
                                    BackgroundCommandType? bct;
                                    MagicCommandChoiceResult result = GetMagicCommand(strategy, nextMagicStep.Value, currentHP, _totalhp, currentMana, out manaDrain, out bct, out command, offensiveSpells, knownSpells, usedAutoSpellMin, usedAutoSpellMax, realmProficiency, sMobTarget, _settingsData);
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
                                        backgroundCommandResult = RunBackgroundMagicStep(bct.Value, command, pms, useManaPool, manaDrain, magicSteps, ref magicStepsFinished, ref nextMagicStep, ref dtNextMagicCommand, ref didDamage);
                                        if (backgroundCommandResult == CommandResult.CommandEscaped)
                                        {
                                            break; //break out of combat
                                        }
                                        else if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                        {
                                            return;
                                        }
                                    }
                                }

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);

                                //flee or stop combat once steps complete
                                if (!nextMagicStep.HasValue && magicStepsFinished && magicStepsEnabled)
                                {
                                    FinalStepAction finalAction = strategy.FinalMagicAction;
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
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

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
                                    WieldWeapon(weaponItem); //wield the weapon in case it was fumbled
                                    backgroundCommandResult = RunBackgroundMeleeStep(BackgroundCommandType.Attack, command, pms, meleeSteps, ref meleeStepsFinished, ref nextMeleeStep, ref dtNextMeleeCommand, ref didDamage);
                                    if (backgroundCommandResult == CommandResult.CommandEscaped)
                                    {
                                        break; //break out of combat
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                }

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);

                                //flee or stop combat once steps complete
                                if (!nextMeleeStep.HasValue && meleeStepsFinished && meleeStepsEnabled)
                                {
                                    FinalStepAction finalAction = strategy.FinalMeleeAction;
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
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

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
                                    PotionsCommandChoiceResult potionChoice = GetPotionsCommand(strategy, nextPotionsStep.Value, out command, _autohp, _totalhp, _settingsData);
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
                                        backgroundCommandResult = RunBackgroundPotionsStep(BackgroundCommandType.DrinkNonHazyPotion, command, pms, potionsSteps, ref potionsStepsFinished, ref nextPotionsStep, ref dtNextPotionsCommand);
                                        if (backgroundCommandResult == CommandResult.CommandEscaped)
                                        {
                                            break; //break out of combat
                                        }
                                        else if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                        {
                                            return;
                                        }
                                    }
                                }

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);

                                //flee or stop combat once steps complete
                                if (!nextPotionsStep.HasValue && potionsStepsFinished && potionsStepsEnabled)
                                {
                                    FinalStepAction finalAction = strategy.FinalPotionsAction;
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
                                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + sMobTextForLook, pms, AbortIfFleeingOrHazying, true);
                                    if (backgroundCommandResult == CommandResult.CommandEscaped)
                                    {
                                        break; //break out of combat
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
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
                                    else if (backgroundCommandResult != CommandResult.CommandSuccessful)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }

                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);
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
                            if (RemoveWeapon(weaponItem))
                            {
                                if (!_fleeing) goto BeforeHazy;
                                if (_hazying) goto BeforeHazy;
                                if (_bw.CancellationPending) return;
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
                                backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Flee, "flee", pms, AbortIfHazying, false);
                                if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                {
                                    return;
                                }
                                else if (backgroundCommandResult == CommandResult.CommandSuccessful)
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
                                    backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, AbortIfHazying, false);
                                    if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandSuccessful)
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
                            if (TryDrinkHazy(pms) == CommandResult.CommandSuccessful)
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
                    }
                }

                if (!pms.Fled && !pms.Hazied)
                {
                    backgroundCommandResult = DoInventoryManagement(pms);
                    if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                    {
                        return;
                    }
                    if (!_hazying && pms.PermRun != null && pms.PermRun.FullAfterFinishing && pms.PermRun.TickRoom.HasValue && !IsFull(pms.PermRun.SpellsToCast))
                    {
                        backgroundCommandResult = NavigateToTickRoom(pms, false);
                        if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                        {
                            return;
                        }
                        if (!_hazying)
                        {
                            backgroundCommandResult = GetFullInBackground(pms, false);
                            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                            {
                                return;
                            }
                        }
                    }

                    if (_hazying)
                    {
                        if (TryDrinkHazy(pms) == CommandResult.CommandSuccessful)
                        {
                            pms.Hazied = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    bool runScore = pms.DoScore;
                    if (!runScore)
                    {
                        lock (_currentEntityInfo.SkillsLock)
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
                        backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null, true);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return;
                        }
                        pms.DoScore = false;
                    }
                }

                //determine success. if the user indicated success using the complete button don't question that
                if (!pms.Success)
                {
                    pms.Success = !pms.Fled && !pms.Hazied && (!hasCombat || pms.MonsterKilled);
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

        private CommandResult ActivatePotionsAndSkills(BackgroundWorkerParameters pms, PromptedSkills skillsToRun)
        {
            CommandResult backgroundCommandResult;
            WorkflowSpells spellsToPot = pms.PermRun == null ? WorkflowSpells.None : pms.PermRun.SpellsToPotion;
            spellsToPot &= ~WorkflowSpells.CurePoison;
            if (spellsToPot != WorkflowSpells.None)
            {
                List<string> spellsCast = new List<string>();
                lock (_spellsCastLock)
                {
                    spellsCast.AddRange(_spellsCast);
                }
                lock (_currentEntityInfo.EntityLock) //validate spells are either active or have a potion for them
                {
                    foreach (WorkflowSpells nextPotSpell in Enum.GetValues(typeof(WorkflowSpells)))
                    {
                        if (nextPotSpell != WorkflowSpells.None && ((nextPotSpell & spellsToPot) != WorkflowSpells.None))
                        {
                            SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextPotSpell];
                            if (spellsCast.Contains(sia.SpellName))
                            {
                                spellsToPot &= ~nextPotSpell;
                            }
                            else
                            {
                                if (!_currentEntityInfo.HasPotionForSpell(sia.SpellType, out _, out _)) return CommandResult.CommandUnsuccessfulAlways;
                            }
                        }
                    }
                }
                foreach (WorkflowSpells nextPotSpell in Enum.GetValues(typeof(WorkflowSpells)))
                {
                    if (_hazying || _fleeing) return CommandResult.CommandEscaped;
                    if (_bw.CancellationPending) return CommandResult.CommandAborted;
                    if (nextPotSpell != WorkflowSpells.None && ((nextPotSpell & spellsToPot) != WorkflowSpells.None))
                    {
                        backgroundCommandResult = DrinkPotionForSpell(SpellsStatic.WorkflowSpellsByEnum[nextPotSpell], pms);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResult;
                        }
                    }
                }
            }
            if (!_hazying && !_fleeing && ((skillsToRun & PromptedSkills.Manashield) == PromptedSkills.Manashield))
            {
                _backgroundProcessPhase = BackgroundProcessPhase.ActivateSkills;
                backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Manashield, "manashield", pms, AbortIfFleeingOrHazying, false);
                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResult;
                }
            }
            if (!_hazying && !_fleeing && ((skillsToRun & PromptedSkills.Fireshield) == PromptedSkills.Fireshield))
            {
                _backgroundProcessPhase = BackgroundProcessPhase.ActivateSkills;
                backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Fireshield, "fireshield", pms, AbortIfFleeingOrHazying, false);
                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResult;
                }
            }
            if (_hazying) backgroundCommandResult = CommandResult.CommandEscaped;
            else if (_bw.CancellationPending) backgroundCommandResult = CommandResult.CommandAborted;
            else backgroundCommandResult = CommandResult.CommandSuccessful;
            return backgroundCommandResult;
        }

        private CommandResult TryDrinkHazy(BackgroundWorkerParameters pms)
        {
            _backgroundProcessPhase = BackgroundProcessPhase.Hazy;
            return RunSingleCommand(BackgroundCommandType.DrinkHazy, "drink hazy", pms, null, false);
        }

        private CommandResult DoInventoryManagement(BackgroundWorkerParameters pms)
        {
            if (pms.PermRun == null) return CommandResult.CommandSuccessful;
            ItemsToProcessType eInvProcessInputs = pms.PermRun.ItemsToProcessType;
            InventoryManagementWorkflow eInventoryWorkflow = pms.InventoryManagementFlow;
            CommandResult backgroundCommandResult;
            if (eInvProcessInputs != ItemsToProcessType.NoProcessing && (eInvProcessInputs == ItemsToProcessType.ProcessAllItemsInRoom || (pms.MonsterKilled && eInvProcessInputs == ItemsToProcessType.ProcessMonsterDrops)))
            {
                List<ItemEntity> itemsToProcess = new List<ItemEntity>();
                lock (_currentEntityInfo.EntityLock)
                {
                    if (eInvProcessInputs == ItemsToProcessType.ProcessAllItemsInRoom)
                    {
                        itemsToProcess.AddRange(_currentEntityInfo.CurrentRoomItems);
                    }
                    else if (eInvProcessInputs == ItemsToProcessType.ProcessMonsterDrops)
                    {
                        itemsToProcess.AddRange(_monsterKilledItems);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            NextItemCycle:
                bool somethingDone = false;
                bool anythingCouldNotBePickedUpFromSourceRoom = false;
                bool anythingFailedForSourceRoom = false;
                List<ItemEntity> itemsToRemoveFromProcessing = new List<ItemEntity>();
                List<ItemEntity> itemsPickedUp = new List<ItemEntity>();
                int weightFailed = int.MaxValue;
                foreach (ItemEntity nextItem in itemsToProcess)
                {
                    if (nextItem.ItemType.HasValue)
                    {
                        ItemTypeEnum eItemType = nextItem.ItemType.Value;
                        StaticItemData sid = ItemEntity.StaticItemData[eItemType];
                        DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, eItemType);

                        bool processItem = false;
                        if (sid.ItemClass == ItemClass.Fixed)
                            processItem = false;
                        else if (eInventoryWorkflow == InventoryManagementWorkflow.Ferry)
                            processItem = true;
                        else if (didWithInherit.OverflowAction == ItemInventoryOverflowAction.Ignore)
                            processItem = false;
                        else if (didWithInherit.KeepCount > 0 || didWithInherit.TickCount > 0)
                            processItem = true;
                        else if (didWithInherit.OverflowAction == ItemInventoryOverflowAction.SellOrJunk)
                            processItem = true;
                        else
                        {
                            anythingFailedForSourceRoom = true;
                            continue;
                        }
                        if (processItem)
                        {
                            if (sid.Weight == 0 || sid.Weight < weightFailed) //if heavier than something that already couldn't be picked up skip
                            {
                                string sItemText;
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, eItemType, 1, true, false);
                                }
                                if (string.IsNullOrEmpty(sItemText))
                                {
                                    anythingFailedForSourceRoom = true;
                                }
                                else
                                {
                                    backgroundCommandResult = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.GetItem, eItemType, sItemText, pms);
                                    if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                    {
                                        itemsPickedUp.Add(nextItem);
                                        somethingDone = true;
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulThisTime)
                                    {
                                        if (sid.Weight > 0 && sid.Weight < weightFailed)
                                        {
                                            weightFailed = sid.Weight;
                                        }
                                        anythingCouldNotBePickedUpFromSourceRoom = true;
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandEscaped)
                                    {
                                        return backgroundCommandResult;
                                    }
                                    else
                                    {
                                        anythingFailedForSourceRoom = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            itemsToRemoveFromProcessing.Add(nextItem);
                            continue;
                        }
                    }
                    else
                    {
                        anythingFailedForSourceRoom = true;
                    }
                }
                foreach (ItemEntity nextItem in itemsToRemoveFromProcessing) itemsToProcess.Remove(nextItem);
                foreach (ItemEntity nextItem in itemsPickedUp) itemsToProcess.Remove(nextItem);
                if (anythingFailedForSourceRoom) return CommandResult.CommandUnsuccessfulAlways;

                List<ItemEntity> itemsToSendToInventorySink = new List<ItemEntity>();
                List<ItemEntity> itemsToSellOrJunk = new List<ItemEntity>();
                foreach (ItemEntity nextItemPickedUp in itemsPickedUp)
                {
                    ItemTypeEnum itemType = nextItemPickedUp.ItemType.Value;
                    StaticItemData sid = ItemEntity.StaticItemData[itemType];
                    if (sid.ItemClass != ItemClass.Coins && sid.ItemClass != ItemClass.Money)
                    {
                        if (eInventoryWorkflow == InventoryManagementWorkflow.Ferry)
                        {
                            itemsToSendToInventorySink.Add(nextItemPickedUp);
                        }
                        else //manage items
                        {
                            DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, itemType);
                            int inventoryCount;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                inventoryCount = _currentEntityInfo.GetTotalInventoryCount(itemType);
                            }
                            if (didWithInherit.KeepCount > 0 && inventoryCount <= didWithInherit.KeepCount)
                                itemsToProcess.Remove(nextItemPickedUp);
                            else if (didWithInherit.TickCount > 0)
                                itemsToSendToInventorySink.Add(nextItemPickedUp);
                            else if (didWithInherit.OverflowAction == ItemInventoryOverflowAction.SellOrJunk)
                                itemsToSellOrJunk.Add(nextItemPickedUp);
                            else //don't know what to do with the item
                            {
                                AddConsoleMessage("Don't know what to do with " + itemType);
                                return CommandResult.CommandUnsuccessfulAlways;
                            }
                        }
                    }
                }

                //sell/junk anything that was picked up and should immediately be sold or junked
                backgroundCommandResult = SellOrJunkItems(itemsToSellOrJunk, pms, ref somethingDone);
                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResult;
                }
                itemsToSellOrJunk.Clear();

                if (itemsToSendToInventorySink.Count > 0)
                {
                    if (pms.InventorySinkRoom == null)
                    {
                        AddConsoleMessage("No inventory sink room specified.");
                        return CommandResult.CommandUnsuccessfulAlways;
                    }

                StartInventorySinkRoomProcessing:

                    backgroundCommandResult = NavigateToInventorySinkRoom(pms, false);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResult;
                    }

                    bool anythingCouldNotBePickedUpFromTickRoom = false;
                    bool anythingFailedForTickRoom = false;

                    //process inventory for items to drop in the inventory sink room or sell/junk
                    HashSet<ItemTypeEnum> tickItemsProcessedForDropping = new HashSet<ItemTypeEnum>();
                    foreach (ItemEntity ie in itemsToSendToInventorySink)
                    {
                        ItemTypeEnum itemType = ie.ItemType.Value;
                        int iCountToDisposeOf = 0;
                        int iCountToDrop = 0;
                        if (eInventoryWorkflow == InventoryManagementWorkflow.Ferry)
                        {
                            iCountToDrop = 1;
                        }
                        else
                        {
                            if (!tickItemsProcessedForDropping.Contains(itemType))
                            {
                                DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, itemType);
                                int iCountToGetRidOfFromInventory = 0;
                                int iTickRoomCount;
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    int iInventoryCount = _currentEntityInfo.GetTotalInventoryCount(itemType);
                                    if (didWithInherit.KeepCount == -1)
                                        iCountToGetRidOfFromInventory = iInventoryCount;
                                    else if (iInventoryCount > didWithInherit.KeepCount)
                                        iCountToGetRidOfFromInventory = iInventoryCount - didWithInherit.KeepCount;
                                    iTickRoomCount = _currentEntityInfo.GetTotalRoomItemsCount(itemType);
                                }
                                if (didWithInherit.TickCount == -1)
                                {
                                    iCountToDisposeOf = iCountToGetRidOfFromInventory;
                                }
                                else if (didWithInherit.TickCount == int.MaxValue)
                                {
                                    iCountToDrop = iCountToGetRidOfFromInventory;
                                }
                                else if (iTickRoomCount >= didWithInherit.TickCount)
                                {
                                    iCountToDisposeOf = iCountToGetRidOfFromInventory;
                                }
                                else
                                {
                                    int iTickRoomFree = didWithInherit.TickCount - iTickRoomCount;
                                    if (iTickRoomFree >= iCountToGetRidOfFromInventory)
                                    {
                                        iCountToDrop = iCountToGetRidOfFromInventory;
                                    }
                                    else
                                    {
                                        iCountToDrop = iTickRoomFree;
                                        iCountToDisposeOf = iCountToGetRidOfFromInventory - iCountToDrop;
                                    }
                                }
                                tickItemsProcessedForDropping.Add(itemType);
                            }
                        }
                        for (int i = 0; i < iCountToDisposeOf; i++)
                        {
                            itemsToSellOrJunk.Add(ie);
                        }
                        for (int i = 0; i < iCountToDrop; i++)
                        {
                            string sItemText;
                            bool remove = false;
                            lock (_currentEntityInfo.EntityLock)
                            {
                                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, itemType, 1, true, false);
                                if (string.IsNullOrEmpty(sItemText))
                                {
                                    remove = true;
                                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Equipment, itemType, 1, true, false);
                                }
                            }
                            if (remove)
                            {
                                backgroundCommandResult = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, itemType, sItemText, pms);
                                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                                {
                                    return backgroundCommandResult;
                                }
                                lock (_currentEntityInfo.EntityLock)
                                {
                                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, itemType, 1, true, false);
                                    if (string.IsNullOrEmpty(sItemText)) return CommandResult.CommandUnsuccessfulAlways;
                                }
                            }
                            backgroundCommandResult = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, itemType, sItemText, pms);
                            if (backgroundCommandResult != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResult;
                            }
                            somethingDone = true;
                        }
                    }

                    //sell/junk anything from the inventory that should be sold or junked
                    if (itemsToSellOrJunk.Count > 0)
                    {
                        backgroundCommandResult = SellOrJunkItems(itemsToSellOrJunk, pms, ref somethingDone);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResult;
                        }
                        itemsToSellOrJunk.Clear();
                        goto StartInventorySinkRoomProcessing;
                    }

                    weightFailed = int.MaxValue;
                    if (eInventoryWorkflow == InventoryManagementWorkflow.ManageSourceItems)
                    {
                        HashSet<ItemTypeEnum> inventorySinkItemsProcessed = new HashSet<ItemTypeEnum>();
                        foreach (ItemEntity ie in itemsToSendToInventorySink)
                        {
                            ItemTypeEnum itemType = ie.ItemType.Value;
                            if (!inventorySinkItemsProcessed.Contains(itemType))
                            {
                                StaticItemData sid = ItemEntity.StaticItemData[itemType];
                                DynamicItemDataWithInheritance didWithInherit = new DynamicItemDataWithInheritance(_settingsData, itemType);

                                //dispose of overflow items from tick room
                                int iTickRoomCount = 0;
                                int iOverflow = 0;
                                lock (_currentEntityInfo)
                                {
                                    iTickRoomCount = _currentEntityInfo.GetTotalRoomItemsCount(itemType);
                                    if (didWithInherit.TickCount == -1)
                                        iOverflow = iTickRoomCount;
                                    else if (iTickRoomCount > didWithInherit.TickCount)
                                        iOverflow = iTickRoomCount - didWithInherit.TickCount;
                                }
                                for (int i = 0; i < iOverflow; i++)
                                {
                                    if (sid.Weight == 0 || sid.Weight < weightFailed) //if heavier than something that already couldn't be picked up skip
                                    {
                                        string sItemText;
                                        lock (_currentEntityInfo.EntityLock)
                                        {
                                            sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, itemType, 1, true, false);
                                        }
                                        backgroundCommandResult = TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.GetItem, itemType, sItemText, pms);
                                        if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                        {
                                            itemsToSellOrJunk.Add(ie);
                                        }
                                        else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulThisTime)
                                        {
                                            if (sid.Weight > 0 && sid.Weight < weightFailed)
                                            {
                                                weightFailed = sid.Weight;
                                            }
                                            anythingCouldNotBePickedUpFromTickRoom = true;
                                        }
                                        else
                                        {
                                            anythingFailedForTickRoom = true;
                                        }
                                    }
                                    else //can't pick up this item anymore
                                    {
                                        break;
                                    }
                                }
                                inventorySinkItemsProcessed.Add(itemType);
                            }
                        }
                    }
                    if (anythingFailedForTickRoom) return CommandResult.CommandUnsuccessfulAlways;

                    if (itemsToSellOrJunk.Count > 0)
                    {
                        backgroundCommandResult = SellOrJunkItems(itemsToSellOrJunk, pms, ref somethingDone);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResult;
                        }
                        itemsToSellOrJunk.Clear();
                        if (anythingCouldNotBePickedUpFromTickRoom)
                        {
                            goto StartInventorySinkRoomProcessing;
                        }
                    }
                }

                if (anythingCouldNotBePickedUpFromSourceRoom)
                {
                    backgroundCommandResult = NavigateToSpecificRoom(pms.TargetRoom, pms, false);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResult;
                    }
                    pms.AtDestination = true;
                    if (somethingDone)
                    {
                        goto NextItemCycle;
                    }
                    else
                    {
                        return CommandResult.CommandUnsuccessfulAlways;
                    }
                }
            }
            CommandResult ret;
            if (_bw.CancellationPending)
                ret = CommandResult.CommandAborted;
            if (_hazying)
                ret = CommandResult.CommandEscaped;
            else
                ret = CommandResult.CommandSuccessful;
            return ret;
        }

        private CommandResult DrinkPotionForSpell(SpellInformationAttribute spellInfo, BackgroundWorkerParameters pms)
        {
            string sItemText;
            bool removeHeldPotion = false;
            ItemTypeEnum? potItem;
            lock (_currentEntityInfo.EntityLock)
            {
                if (!_currentEntityInfo.HasPotionForSpell(spellInfo.SpellType, out potItem, out bool? inInventory)) return CommandResult.CommandUnsuccessfulAlways;
                ItemLocationType ilt = inInventory.Value ? ItemLocationType.Inventory : ItemLocationType.Equipment;
                sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ilt, potItem.Value, 1, false, ilt == ItemLocationType.Equipment);
                if (string.IsNullOrEmpty(sItemText) && ilt == ItemLocationType.Equipment)
                {
                    removeHeldPotion = true;
                }
            }
            if (removeHeldPotion)
            {
                lock (_currentEntityInfo.EntityLock)
                {
                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Equipment, potItem.Value, 1, false, false);
                    if (string.IsNullOrEmpty(sItemText)) return CommandResult.CommandUnsuccessfulAlways;
                }
                if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.RemoveEquipment, potItem.Value, sItemText, pms) != CommandResult.CommandSuccessful) return CommandResult.CommandUnsuccessfulAlways;
                lock (_currentEntityInfo.EntityLock)
                {
                    sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, potItem.Value, 1, false, false);
                }
                if (string.IsNullOrEmpty(sItemText)) return CommandResult.CommandUnsuccessfulAlways;
            }
            return RunSingleCommand(BackgroundCommandType.DrinkNonHazyPotion, "drink " + sItemText, pms, AbortIfFleeingOrHazying, false);
        }

        /// <summary>
        /// navigates to a tick room. assumed to be in the context of a perm run
        /// </summary>
        /// <param name="pms">background parameter</param>
        /// <param name="allowFlee">whether flee is allowed</param>
        /// <returns>result of the operation</returns>
        private CommandResult NavigateToTickRoom(BackgroundWorkerParameters pms, bool allowFlee)
        {
            return NavigateToSpecificRoom(_gameMap.HealingRooms[pms.PermRun.TickRoom.Value], pms, allowFlee);
        }

        private CommandResult NavigateToInventorySinkRoom(BackgroundWorkerParameters pms, bool allowFlee)
        {
            return NavigateToSpecificRoom(pms.InventorySinkRoom, pms, allowFlee);
        }

        private CommandResult NavigateToSpecificRoom(Room r, BackgroundWorkerParameters pms, bool allowFlee)
        {
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            CommandResult backgroundCommandResult;
            if (currentRoom != r)
            {
                var nextRoute = CalculateRouteExits(currentRoom, r, true);
                if (nextRoute == null) return CommandResult.CommandUnsuccessfulAlways;
                backgroundCommandResult = TraverseExitsAlreadyInBackground(nextRoute, pms, allowFlee);
            }
            else
            {
                backgroundCommandResult = CommandResult.CommandSuccessful;
            }
            pms.AtDestination = r == pms.TargetRoom;
            return backgroundCommandResult;
        }

        private bool IsFull(WorkflowSpells castWorkflowSpells)
        {
            if (_autohp < _totalhp) return false;
            if (_automp < _totalmp) return false;
            castWorkflowSpells &= ~WorkflowSpells.CurePoison;
            bool ret = true;
            if (castWorkflowSpells != WorkflowSpells.None)
            {
                lock (_spellsCastLock)
                {
                    foreach (WorkflowSpells nextWorkflowSpell in Enum.GetValues(typeof(WorkflowSpells)))
                    {
                        if ((nextWorkflowSpell & castWorkflowSpells) != WorkflowSpells.None)
                        {
                            SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextWorkflowSpell];
                            if (!_spellsCast.Contains(sia.SpellName))
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        private CommandResult SellOrJunkItems(List<ItemEntity> items, BackgroundWorkerParameters pms, ref bool somethingDone)
        {
            CommandResult backgroundCommandResult;
            bool success = true;
            if (items.Count > 0)
            {
                if (!pms.PermRun.PawnShop.HasValue)
                {
                    AddConsoleMessage("No pawn shop available.");
                    return CommandResult.CommandUnsuccessfulAlways;
                }
                Room currentRoom = _currentEntityInfo.CurrentRoom;
                Room targetRoom = _gameMap.PawnShoppes[pms.PermRun.PawnShop.Value];
                if (currentRoom != targetRoom)
                {
                    List<Exit> nextRoute = CalculateRouteExits(currentRoom, targetRoom, true);
                    if (nextRoute == null) return CommandResult.CommandUnsuccessfulAlways;
                    backgroundCommandResult = TraverseExitsAlreadyInBackground(nextRoute, pms, false);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResult;
                    }
                }
                foreach (ItemEntity nextItem in items)
                {
                    ItemTypeEnum itemType = nextItem.ItemType.Value;
                    string sItemText;
                    lock (_currentEntityInfo.EntityLock)
                    {
                        sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, itemType, 1, true, false);
                    }
                    if (string.IsNullOrEmpty(sItemText))
                    {
                        AddConsoleMessage("Unable to sell/junk" + itemType);
                        success = false;
                        continue;
                    }

                    StaticItemData sid = ItemEntity.StaticItemData[itemType];
                    bool trySell = sid.SellGold > 0 || sid.Sellable == SellableEnum.Unknown;
                    if (trySell && TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.SellItem, itemType, sItemText, pms) == CommandResult.CommandSuccessful)
                    {
                        somethingDone = true;
                        pms.UsedPawnShoppe = true;
                        continue;
                    }

                    if (sid.Sellable == SellableEnum.Junk)
                    {
                        if (TryCommandAddingOrRemovingFromInventory(BackgroundCommandType.DropItem, itemType, sItemText, pms) == CommandResult.CommandSuccessful)
                        {
                            somethingDone = true;
                            pms.UsedPawnShoppe = true;
                            continue;
                        }
                        else
                        {
                            AddConsoleMessage("Failed to junk " + itemType);
                            success = true;
                        }
                    }
                    else
                    {
                        AddConsoleMessage("Failed to sell non-junk item: " + itemType);
                        success = false;
                    }
                }
            }
            CommandResult ret;
            if (_bw.CancellationPending)
                ret = CommandResult.CommandAborted;
            else if (_hazying)
                ret = CommandResult.CommandEscaped;
            else if (!success)
                ret = CommandResult.CommandUnsuccessfulAlways;
            else
                ret = CommandResult.CommandSuccessful;
            return ret;
        }

        private bool AttackIsGoodToGo(BackgroundWorkerParameters pms)
        {
            bool ret;
            if (pms.MobType.HasValue)
            {
                ret = !string.IsNullOrEmpty(GetMobTargetFromMobType(pms.MobType.Value, pms.MobTypeCounter, false));
            }
            else if (!string.IsNullOrEmpty(pms.MobText))
            {
                ret = true; //there's no way of verifying from mob text, so just say ok
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
        private CommandResult TryCommandAddingOrRemovingFromInventory(BackgroundCommandType commandType, ItemTypeEnum itemType, string itemText, BackgroundWorkerParameters pms)
        {
            CommandResult backgroundCommandResult;
            StaticItemData sid = ItemEntity.StaticItemData[itemType];
            bool checkWeight = sid.Weight == 0;
            int beforeWeight = 0;
            int afterWeight;
            int beforeGold = _gold;
            if (checkWeight)
            {
                backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Inventory, "inventory", pms, null, true);
                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                {
                    return backgroundCommandResult;
                }
                lock (_currentEntityInfo.EntityLock)
                {
                    beforeWeight = _currentEntityInfo.TotalInventoryWeight.Value;
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
                default:
                    throw new InvalidOperationException();
            }
            CommandResult ret = RunSingleCommandForCommandResult(commandType, command + " " + itemText, pms, null, false);
            if (ret == CommandResult.CommandSuccessful)
            {
                List<string> broadcastMessages = null;
                if (commandType == BackgroundCommandType.SellItem)
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
                    if (RunSingleCommandForCommandResult(BackgroundCommandType.Inventory, "inventory", pms, null, true) != CommandResult.CommandSuccessful)
                    {
                        return CommandResult.CommandUnsuccessfulAlways;
                    }
                    lock (_currentEntityInfo.EntityLock)
                    {
                        afterWeight = _currentEntityInfo.TotalInventoryWeight.Value;
                    }
                    int weightDifference = afterWeight - beforeWeight;
                    if (commandType == BackgroundCommandType.DropItem || commandType == BackgroundCommandType.SellItem)
                    {
                        weightDifference = -weightDifference;
                    }
                    if (weightDifference > 0)
                    {
                        if (broadcastMessages == null) broadcastMessages = new List<string>();
                        broadcastMessages.Add("Found weight for " + sid.ItemType.ToString() + ": " + weightDifference);
                        sid.Weight = weightDifference;
                    }
                }
                if (broadcastMessages != null)
                {
                    lock (_broadcastMessagesLock)
                    {
                        _broadcastMessages.AddRange(broadcastMessages);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// gets full hitpoints in a background process, optionally with cure-poison
        /// This is used when traversing exits after a trap room is encountered.
        /// </summary>
        /// <param name="pms">background worker parameters</param>
        /// <param name="needCurePoison">whether cure-poison is needed</param>
        /// <returns>true if cure-poison was successfully cast if needed and hitpoints got to maximum</returns>
        private CommandResult GetFullHitpoints(BackgroundWorkerParameters pms, bool needCurePoison)
        {
            CommandResult nextCommandResult;
            WorkflowSpells spellsToCast = pms.PermRun == null ? WorkflowSpells.None : pms.PermRun.SpellsToCast;
            WorkflowSpells spellsToPotion = pms.PermRun == null ? WorkflowSpells.None : pms.PermRun.SpellsToPotion;
            if (needCurePoison)
            {
                SpellInformationAttribute siaCurePoison = SpellsStatic.SpellsByEnum[SpellsEnum.curepoison];
                if ((spellsToPotion & WorkflowSpells.CurePoison) != WorkflowSpells.None)
                {
                    nextCommandResult = DrinkPotionForSpell(siaCurePoison, pms);
                    if (nextCommandResult == CommandResult.CommandSuccessful)
                    {
                        needCurePoison = false;
                    }
                    else if (nextCommandResult == CommandResult.CommandAborted || nextCommandResult == CommandResult.CommandTimeout || nextCommandResult == CommandResult.CommandEscaped)
                    {
                        return nextCommandResult;
                    }
                }
                if (needCurePoison && ((spellsToCast & WorkflowSpells.CurePoison) != WorkflowSpells.None))
                {
                    if (_automp >= siaCurePoison.Mana)
                    {
                        nextCommandResult = CastSpellOnSelf("cure-poison", pms);
                        if (nextCommandResult == CommandResult.CommandSuccessful)
                        {
                            needCurePoison = false;
                        }
                        else if (nextCommandResult == CommandResult.CommandAborted || nextCommandResult == CommandResult.CommandTimeout || nextCommandResult == CommandResult.CommandEscaped)
                        {
                            return nextCommandResult;
                        }
                    }
                }
                if (needCurePoison)
                {
                    AddConsoleMessage("Unable to cure poison.");
                    return CommandResult.CommandUnsuccessfulAlways;
                }
            }
            SpellInformationAttribute siaVigor = SpellsStatic.SpellsByEnum[SpellsEnum.vigor];
            while (_autohp < _totalhp && _automp >= siaVigor.Mana)
            {
                nextCommandResult = CastSpellOnSelf("vigor", pms);
                if (nextCommandResult != CommandResult.CommandSuccessful)
                {
                    return nextCommandResult;
                }
            }
            return CommandResult.CommandSuccessful;
        }

        /// <summary>
        /// gets full in a background process
        /// </summary>
        /// <param name="pms">background parameters</param>
        /// <param name="forStart">true for before starting the perm run, false for after finishing the perm run</param>
        /// <returns>true if successfully got to full, false otherwise</returns>
        private CommandResult GetFullInBackground(BackgroundWorkerParameters pms, bool forStart)
        {
            int iTickHP = GetHealingRoomTickHP();
            int iTickMP = GetHealingRoomTickMP();
            WorkflowSpells spellsToCast = pms.PermRun.SpellsToCast;
            spellsToCast &= ~WorkflowSpells.CurePoison;
            CommandResult backgroundCommandResult;
            while (!IsFull(spellsToCast))
            {
                int automp = _automp;
                int autohp = _autohp;
                int numTicksForFullMP = (_totalmp - automp - 1) / iTickMP + 1;
                int numTicksForFullHP = (_totalhp - autohp - 1) / iTickHP + 1;

                bool castSomething = false;
                if (numTicksForFullHP > numTicksForFullMP)
                {
                    backgroundCommandResult = CastSpellOnSelf("vigor", pms);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResult;
                    }
                    castSomething = true;
                }
                else
                {
                    if (automp + iTickMP > _totalmp) //wait until almost full mp before casting spells
                    {
                        if (!forStart && autohp >= _totalhp)
                        {
                            //after the workflow, at full hp and less than one tick of mp left. Even though not technically full, the user can
                            //make a decision whether to do something at slightly less than full mp, so stop the fulling
                            //at this point.
                            return CommandResult.CommandSuccessful;
                        }
                        else if (autohp + iTickHP > _totalhp)
                        {
                            foreach (WorkflowSpells nextSpell in Enum.GetValues(typeof(WorkflowSpells)))
                            {
                                if ((nextSpell & spellsToCast) != WorkflowSpells.None)
                                {
                                    SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[nextSpell];
                                    string spellName = sia.SpellName;
                                    bool hasSpellActive;
                                    lock (_spellsCastLock)
                                    {
                                        hasSpellActive = _spellsCast.Contains(spellName);
                                    }
                                    if (!hasSpellActive && _automp >= sia.Mana)
                                    {
                                        backgroundCommandResult = CastSpellOnSelf(spellName, pms);
                                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                                        {
                                            return backgroundCommandResult;
                                        }
                                        castSomething = true;
                                        spellsToCast &= ~nextSpell;
                                        break;
                                    }
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
                        if (_fleeing || _hazying) return CommandResult.CommandEscaped;
                        if (_bw.CancellationPending) return CommandResult.CommandAborted;
                        RunQueuedCommandWhenBackgroundProcessRunning(pms);
                    }
                }
                if (_fleeing || _hazying) return CommandResult.CommandEscaped;
                if (_bw.CancellationPending) return CommandResult.CommandAborted;
                RunQueuedCommandWhenBackgroundProcessRunning(pms);
            }
            return CommandResult.CommandSuccessful;
        }

        public PotionsCommandChoiceResult GetPotionsCommand(Strategy Strategy, PotionsStrategyStep nextPotionsStep, out string command, int currentHP, int totalHP, IsengardSettingData settings)
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
                    ItemTypeEnum? heldItem = _currentEntityInfo.Equipment[iHeldSlot];
                    if (heldItem.HasValue)
                    {
                        ItemTypeEnum eHeldItem = heldItem.Value;
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

        private CommandResult TraverseExitsAlreadyInBackground(List<Exit> exits, BackgroundWorkerParameters pms, bool allowFlee)
        {
            Func<bool> abortLogic;
            if (allowFlee)
                abortLogic = AbortIfFleeingOrHazying;
            else
                abortLogic = AbortIfHazying;
            Exit previousExit = null;
            _backgroundProcessPhase = BackgroundProcessPhase.Movement;
            List<Exit> exitList = new List<Exit>(exits);
            Room oTarget = exitList[exitList.Count - 1].Target;
            bool needHeal = false;
            bool needCurepoison = false;
            CommandResult backgroundCommandResult;
            while (exitList.Count > 0)
            {
                Exit nextExit = exitList[0];
                if (previousExit != null && previousExit == nextExit)
                {
                    AddConsoleMessage("Movement recalculation produced the same path.");
                    return CommandResult.CommandUnsuccessfulAlways;
                }
                Room nextExitTarget = nextExit.Target;
                string exitText = nextExit.ExitText;
                _currentBackgroundExit = nextExit;
                _currentBackgroundExitMessageReceived = false;
                try
                {
                    //for exits that aren't always present, ensure the exit exists
                    ExitPresenceType presenceType = nextExit.PresenceType;
                    if (presenceType != ExitPresenceType.Always)
                    {
                        bool foundExit = false;
                        do
                        {
                            if (presenceType == ExitPresenceType.Periodic)
                            {
                                backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, abortLogic, false);
                                if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                {
                                    if (_currentEntityInfo.CurrentObviousExits.Contains(exitText))
                                    {
                                        foundExit = true;
                                    }
                                    else
                                    {
                                        WaitUntilNextCommandTry(5000, BackgroundCommandType.Look);
                                    }
                                }
                                else //look is not supposed to fail (but could be aborted)
                                {
                                    return backgroundCommandResult;
                                }
                            }
                            else if (presenceType == ExitPresenceType.RequiresSearch)
                            {
                                _foundSearchedExits = null;
                                backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Search, "search", pms, abortLogic, false);
                                if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                {
                                    if (_foundSearchedExits.Contains(exitText))
                                    {
                                        foundExit = true;
                                    }
                                }
                                else
                                {
                                    return backgroundCommandResult;
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        while (!foundExit);
                    }

                    //for wait for message exits, wait until the message is received
                    if (nextExit.WaitForMessage.HasValue)
                    {
                        while (!_currentBackgroundExitMessageReceived)
                        {
                            if (allowFlee && _fleeing) return CommandResult.CommandEscaped;
                            if (_hazying) return CommandResult.CommandEscaped;
                            if (_bw.CancellationPending) return CommandResult.CommandAborted;
                            Thread.Sleep(50);
                            RunQueuedCommandWhenBackgroundProcessRunning(pms);
                        }
                    }

                    if (allowFlee && _fleeing) return CommandResult.CommandEscaped;
                    if (_hazying) return CommandResult.CommandEscaped;
                    if (_bw.CancellationPending) return CommandResult.CommandAborted;

                    if ((nextExit.KeyType.HasValue || nextExit.IsUnknownKnockableKeyType) && !nextExit.RequiresKey())
                    {
                        backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Knock, "knock " + exitText, pms, abortLogic, false);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResult;
                        }
                    }

                    if (nextExit.IsTrapExit || (nextExitTarget != null && nextExitTarget.IsTrapRoom))
                    {
                        backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Prepare, "prepare", pms, AbortIfFleeingOrHazying, false);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return backgroundCommandResult;
                        }
                    }
                    bool useGo;
                    string sExitWord = GetExitWord(nextExit, out useGo);
                    backgroundCommandResult = PreOpenDoorExit(nextExit, sExitWord, pms);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return backgroundCommandResult;
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
                        backgroundCommandResult = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, null, false);
                        if (backgroundCommandResult == CommandResult.CommandSuccessful) //successfully traversed the exit to the new room
                        {
                            exitList.RemoveAt(0);
                            keepTryingMovement = false;
                            if ((_lastCommandTrapType & TrapType.PoisonDart) != TrapType.None)
                            {
                                needCurepoison = true;
                            }
                            if (_lastCommandDamage != 0) //trap room
                            {
                                needHeal = true;
                            }
                            if ((_lastCommandTrapType & TrapType.Fall) != TrapType.None)
                            {
                                SendCommand("stand", InputEchoType.On);
                            }
                        }
                        else if (_lastCommandMovementResult == MovementResult.MapFailure)
                        {
                            List<Exit> newRoute = CalculateRouteExits(nextExit.Source, oTarget, true);
                            if (newRoute != null && newRoute.Count > 0)
                            {
                                exitList.Clear();
                                exitList.AddRange(newRoute);
                            }
                            else //couldn't recalculate a new route
                            {
                                return CommandResult.CommandUnsuccessfulAlways;
                            }
                            keepTryingMovement = false;
                        }
                        else if (_lastCommandMovementResult == MovementResult.StandFailure)
                        {
                            SendCommand("stand", InputEchoType.On);
                            keepTryingMovement = true;
                        }
                        else if (_lastCommandMovementResult == MovementResult.ClosedDoorFailure)
                        {
                            backgroundCommandResult = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitText, pms, AbortIfFleeingOrHazying, false);
                            if (backgroundCommandResult == CommandResult.CommandSuccessful)
                            {
                                keepTryingMovement = true;
                            }
                            else
                            {
                                return backgroundCommandResult;
                            }
                        }
                        else if (_lastCommandMovementResult == MovementResult.FallFailure)
                        {
                            backgroundCommandResult = GetFullHitpoints(pms, needCurepoison);
                            if (backgroundCommandResult != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResult;
                            }
                            SendCommand("stand", InputEchoType.On);
                            keepTryingMovement = true;
                        }
                        else //total failure, abort the background process
                        {
                            keepTryingMovement = false;
                            return CommandResult.CommandUnsuccessfulAlways;
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
                            backgroundCommandResult = GetFullHitpoints(pms, needCurepoison);
                            if (backgroundCommandResult != CommandResult.CommandSuccessful)
                            {
                                return backgroundCommandResult;
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
            CommandResult ret;
            if (_bw.CancellationPending)
                ret = CommandResult.CommandAborted;
            else if (_hazying || _fleeing)
                ret = CommandResult.CommandEscaped;
            else
                ret = CommandResult.CommandSuccessful;
            return ret;
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

        public MagicCommandChoiceResult GetMagicCommand(Strategy Strategy, MagicStrategyStep nextMagicStep, int currentHP, int totalHP, int currentMP, out int manaDrain, out BackgroundCommandType? bct, out string command, List<string> offensiveSpells, List<string> knownSpells, int usedAutoSpellMin, int usedAutoSpellMax, int realmProficiency, string mobTarget, IsengardSettingData settingsData)
        {
            MagicCommandChoiceResult ret = MagicCommandChoiceResult.Cast;
            bool doCast;
            command = null;
            manaDrain = 0;
            bct = null;
            if (nextMagicStep == MagicStrategyStep.Stun)
            {
                command = "cast stun " + mobTarget;
                manaDrain = 10;
                bct = BackgroundCommandType.Stun;
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
                if (nextMagicStep == MagicStrategyStep.OffensiveSpellAuto)
                {
                    if (currentMP >= 20 && usedAutoSpellMin <= 5 && usedAutoSpellMax >= 5 && knownSpells.Contains(offensiveSpells[4]) && realmProficiency >= 70)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel5;
                    }
                    if (currentMP >= 15 && usedAutoSpellMin <= 4 && usedAutoSpellMax >= 4 && knownSpells.Contains(offensiveSpells[3]) && realmProficiency >= 50)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel4;
                    }
                    else if (currentMP >= 10 && usedAutoSpellMin <= 3 && usedAutoSpellMax >= 3 && knownSpells.Contains(offensiveSpells[2]) && realmProficiency >= 35)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel3;
                    }
                    else if (currentMP >= 7 && usedAutoSpellMin <= 2 && usedAutoSpellMax >= 2 && knownSpells.Contains(offensiveSpells[1]) && realmProficiency >= 15)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel2;
                    }
                    else if (currentMP >= 3 && usedAutoSpellMin <= 1 && usedAutoSpellMax >= 1 && knownSpells.Contains(offensiveSpells[0]) && realmProficiency >= 5)
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel1;
                    }
                    else //out of mana
                    {
                        ret = MagicCommandChoiceResult.OutOfMana;
                    }
                }
                if (ret == MagicCommandChoiceResult.Cast)
                {
                    string spell;
                    if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel5)
                    {
                        spell = offensiveSpells[4];
                        manaDrain = 20;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel4)
                    {
                        spell = offensiveSpells[3];
                        manaDrain = 15;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel3)
                    {
                        spell = offensiveSpells[2];
                        manaDrain = 10;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel2)
                    {
                        spell = offensiveSpells[1];
                        manaDrain = 7;
                    }
                    else if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel1)
                    {
                        spell = offensiveSpells[0];
                        manaDrain = 3;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    if (knownSpells.Contains(spell))
                    {
                        command = "cast " + spell + " " + mobTarget;
                        bct = BackgroundCommandType.OffensiveSpell;
                    }
                    else
                    {
                        manaDrain = 0;
                        ret = MagicCommandChoiceResult.OutOfMana;
                    }
                }
            }
            if (manaDrain > 0 && manaDrain > currentMP)
            {
                manaDrain = 0;
                bct = null;
                ret = MagicCommandChoiceResult.OutOfMana;
            }
            return ret;
        }

        private bool SelectMobAfterKillMonster(AfterKillMonsterAction onMonsterKilledAction, BackgroundWorkerParameters bwp)
        {
            if (_monsterKilled && (onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoom || onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoomOfSameType))
            {
                bwp.MonsterKilled = true;
                bwp.MonsterKilledType = _monsterKilledType;
                lock (_currentEntityInfo.EntityLock)
                {
                    MobTypeEnum monsterType = MobTypeEnum.LittleMouse;
                    int index;
                    if (onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoom)
                    {
                        if (_currentEntityInfo.CurrentRoomMobs.Count == 0) return false;
                        index = 0;
                        monsterType = _currentEntityInfo.CurrentRoomMobs[0];
                    }
                    else
                    {
                        if (!_monsterKilledType.HasValue) return false;
                        monsterType = _monsterKilledType.Value;
                        index = _currentEntityInfo.CurrentRoomMobs.IndexOf(_monsterKilledType.Value);
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
                }
            }
            return true;
        }

        private bool RemoveWeapon(ItemTypeEnum? weaponItem)
        {
            bool ret = false;
            if (weaponItem.HasValue)
            {
                ItemTypeEnum weaponItemValue = weaponItem.Value;
                string sWieldCommand = null;
                lock (_currentEntityInfo.EntityLock)
                {
                    if (_currentEntityInfo.Equipment[(int)EquipmentSlot.Weapon1] == weaponItemValue)
                    {
                        string sWeaponText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Equipment, weaponItemValue, 1, false, false);
                        if (!string.IsNullOrEmpty(sWeaponText))
                        {
                            sWieldCommand = "remove " + sWeaponText;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sWieldCommand))
                {
                    ret = true;
                    SendCommand(sWieldCommand, InputEchoType.On);
                }
            }
            return ret;
        }

        private void WieldWeapon(ItemTypeEnum? weaponItem)
        {
            if (weaponItem.HasValue)
            {
                ItemTypeEnum weaponItemValue = weaponItem.Value;
                string sWieldCommand = null;
                lock (_currentEntityInfo.EntityLock)
                {
                    if (_currentEntityInfo.Equipment[(int)EquipmentSlot.Weapon1] == null && _currentEntityInfo.InventoryContainsItemType(weaponItemValue))
                    {
                        string sWeaponText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Inventory, weaponItemValue, 1, false, false);
                        if (!string.IsNullOrEmpty(sWeaponText))
                        {
                            sWieldCommand = "wield " + sWeaponText;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sWieldCommand))
                {
                    SendCommand(sWieldCommand, InputEchoType.On);
                }
            }
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
            else if (_bw.CancellationPending)
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

        private CommandResult RunBackgroundMeleeStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<MeleeStrategyStep> meleeSteps, ref bool meleeStepsFinished, ref MeleeStrategyStep? nextMeleeStep, ref DateTime? dtNextMeleeCommand, ref bool didDamage)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandEscaped)
            {
                //do nothing
            }
            else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
            {
                meleeStepsFinished = true;
                _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                nextMeleeStep = null;
            }
            else if (backgroundCommandResult == CommandResult.CommandMustWait)
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
            else if (backgroundCommandResult == CommandResult.CommandSuccessful) //attack was carried out (hit or miss)
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
            return backgroundCommandResult;
        }

        private CommandResult RunBackgroundPotionsStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<PotionsStrategyStep> potionsSteps, ref bool potionsStepsFinished, ref PotionsStrategyStep? nextPotionsStep, ref DateTime? dtNextPotionsCommand)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandEscaped)
            {
                //do nothing
            }
            else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
            {
                potionsStepsFinished = true;
                _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                nextPotionsStep = null;
            }
            else if (backgroundCommandResult == CommandResult.CommandMustWait)
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
            else if (backgroundCommandResult == CommandResult.CommandSuccessful)
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
            return backgroundCommandResult;
        }

        private CommandResult RunBackgroundMagicStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, bool useManaPool, int manaDrain, IEnumerator<MagicStrategyStep> magicSteps, ref bool magicStepsFinished, ref MagicStrategyStep? nextMagicStep, ref DateTime? dtNextMagicCommand, ref bool didDamage)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying, false);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout || backgroundCommandResult == CommandResult.CommandEscaped)
            {
                //do nothing
            }
            else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
            {
                magicStepsFinished = true;
                _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                nextMagicStep = null;
            }
            else if (backgroundCommandResult == CommandResult.CommandMustWait)
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
            else if (backgroundCommandResult == CommandResult.CommandSuccessful) //spell was cast
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
            return backgroundCommandResult;
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

        private void CheckForQueuedMeleeStep(BackgroundWorkerParameters pms, ref MeleeStrategyStep? nextMeleeStep)
        {
            MeleeStrategyStep? queuedMeleeStep;
            if (!nextMeleeStep.HasValue)
            {
                lock (_queuedCommandLock)
                {
                    queuedMeleeStep = pms.QueuedMeleeStep;
                    if (queuedMeleeStep.HasValue)
                    {
                        nextMeleeStep = queuedMeleeStep;
                        pms.QueuedMeleeStep = null;
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

        private CommandResult PreOpenDoorExit(Exit exit, string exitWord, BackgroundWorkerParameters pms)
        {
            CommandResult ret;
            if (exit.MustOpen)
            {
                ret = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitWord, pms, AbortIfFleeingOrHazying, false);
            }
            else //not a door exit
            {
                ret = CommandResult.CommandSuccessful;
            }
            return ret;
        }

        private CommandResult CastSpellOnSelf(string spellName, BackgroundWorkerParameters bwp)
        {
            BackgroundCommandType bct;
            switch (spellName)
            {
                case "vigor":
                    bct = BackgroundCommandType.Vigor;
                    break;
                case "mend-wounds":
                    bct = BackgroundCommandType.MendWounds;
                    break;
                case "cure-poison":
                    bct = BackgroundCommandType.CurePoison;
                    break;
                case "bless":
                    bct = BackgroundCommandType.Bless;
                    break;
                case "protection":
                    bct = BackgroundCommandType.Protection;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return RunSingleCommand(bct, "cast " + spellName, bwp, AbortIfFleeingOrHazying, false);
        }

        private void WaitUntilNextCommandTry(int remainingMS, BackgroundCommandType commandType)
        {
            bool hazying = commandType == BackgroundCommandType.DrinkHazy;
            bool fleeing = commandType == BackgroundCommandType.Flee;
            while (remainingMS > 0)
            {
                int nextWaitMS = Math.Min(remainingMS, 100);

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
                if (_bw.CancellationPending) break;

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
                if (_bw.CancellationPending) break;

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
                if (_bw.CancellationPending) break;
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

        private CommandResult RunSingleCommandForCommandResult(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden)
        {
            _backgroundCommandType = commandType;
            try
            {
                bool allowAbort = commandType != BackgroundCommandType.Movement;
                return RunSingleCommandForCommandResult(command, pms, abortLogic, hidden, allowAbort);
            }
            finally
            {
                _backgroundCommandType = null;
            }
        }

        private InputEchoType GetHiddenMessageEchoType()
        {
            return GetVerboseMode() ? InputEchoType.On : InputEchoType.Off;
        }

        private CommandResult RunSingleCommandForCommandResult(string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden, bool allowAbort)
        {
            DateTime utcTimeoutPoint = DateTime.UtcNow.AddSeconds(SINGLE_COMMAND_TIMEOUT_SECONDS);
            _commandResult = null;
            _commandResultCounter++;
            _lastCommand = null;
            _lastCommandDamage = 0;
            _lastCommandTrapType = TrapType.None;
            _lastCommandMovementResult = null;
            try
            {
                _lastCommand = command;
                InputEchoType echoType;
                if (hidden)
                    echoType = InputEchoType.Off;
                else
                    echoType = GetHiddenMessageEchoType();
                SendCommand(command, echoType);
                CommandResult? currentResult = null;
                while (!currentResult.HasValue)
                {
                    RunQueuedCommandWhenBackgroundProcessRunning(pms);
                    bool doSleep = true;
                    if (allowAbort)
                    {
                        if (_bw.CancellationPending)
                        {
                            currentResult = CommandResult.CommandAborted;
                            doSleep = false;
                        }
                        else if (abortLogic != null && abortLogic())
                        {
                            currentResult = CommandResult.CommandEscaped;
                            doSleep = false;
                        }
                        else if (DateTime.UtcNow >= utcTimeoutPoint)
                        {
                            AddConsoleMessage("Command timeout occurred for " + command);
                            currentResult = CommandResult.CommandTimeout;
                            doSleep = false;
                        }
                    }
                    if (doSleep)
                    {
                        Thread.Sleep(50);
                        currentResult = _commandResult;
                    }
                }
                return currentResult.Value;
            }
            finally
            {
                _commandResult = null;
                _lastCommand = null;
            }
        }

        private void AddConsoleMessage(string Message)
        {
            AddConsoleMessage(new List<string>() { Message });
        }

        private void AddConsoleMessage(List<string> Messages)
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
        private CommandResult RunSingleCommand(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool hidden)
        {
            int currentAttempts = 0;
            _backgroundCommandType = commandType;
            try
            {
                CommandResult? result = null;
                bool allowAbort = commandType != BackgroundCommandType.Movement;
                while (currentAttempts < MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND)
                {
                    if (_bw.CancellationPending) return CommandResult.CommandAborted;
                    if (abortLogic != null && abortLogic()) return CommandResult.CommandEscaped;
                    currentAttempts++;
                    result = RunSingleCommandForCommandResult(command, pms, abortLogic, hidden, allowAbort);
                    if (result.HasValue)
                    {
                        CommandResult resultValue = result.Value;
                        if (resultValue == CommandResult.CommandSuccessful || resultValue == CommandResult.CommandUnsuccessfulAlways || resultValue == CommandResult.CommandAborted || resultValue == CommandResult.CommandTimeout || resultValue == CommandResult.CommandEscaped)
                        {
                            return resultValue;
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
                }
                //if we got here we failed for all attempts, so it's reasonable to say it always fails here
                return CommandResult.CommandUnsuccessfulAlways;
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

        private void ToggleBackgroundProcessUI(BackgroundWorkerParameters bwp, bool running)
        {
            bool enabled;
            if (running)
                enabled = false;
            else
                enabled = !running;
            foreach (Control ctl in GetControlsToDisableForBackgroundProcess())
            {
                ctl.Enabled = enabled;
            }
            foreach (ToolStripButton tsb in GetToolStripButtonsToDisableForBackgroundProcess())
            {
                tsb.Enabled = enabled;
            }
            foreach (ToolStripDropDownButton tsddb in GetToolStripDropdownsToDisableForBackgroundProcess())
            {
                tsddb.Enabled = enabled;
            }
            btnAbort.Enabled = running;
            btnComplete.Enabled = running && bwp.PermRun != null;
            EnableDisableActionButtons(bwp);
        }

        private IEnumerable<ToolStripButton> GetToolStripButtonsToDisableForBackgroundProcess()
        {
            yield return tsbReloadMap;
            yield return tsbQuit;
            yield return tsbLogout;
        }

        private IEnumerable<ToolStripDropDownButton> GetToolStripDropdownsToDisableForBackgroundProcess()
        {
            yield return tsddbSettings;
        }

        private IEnumerable<Control> GetControlsToDisableForBackgroundProcess()
        {
            yield return txtMob;
            yield return txtWand;
            yield return txtPotion;
            yield return cboTickRoom;
            yield return btnGoToHealingRoom;
            yield return cboPawnShop;
            yield return btnGoToPawnShop;
            foreach (Button btn in flpOneClickStrategies.Controls)
            {
                yield return btn;
            }
            yield return btnFerry;
            yield return btnPermRuns;
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
            yield return btnSearch;
            yield return btnHide;
            yield return btnSet;
            yield return btnLook;
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
            bool haveSettings = _settingsData != null;
            List<string> knownSpells;
            List<string> realmSpells = haveSettings ? CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_settingsData.Realm) : null;
            lock (_currentEntityInfo.SpellsKnownLock)
            {
                knownSpells = _currentEntityInfo.SpellsKnown;
            }
            foreach (CommandButtonTag oTag in GetButtonsForEnablingDisabling())
            {
                object oControl = oTag.Control;
                if ((oTag.ObjectType & DependentObjectType.Mob) != DependentObjectType.None && !hasMobTarget)
                    enabled = false;
                else if ((oTag.ObjectType & DependentObjectType.Weapon) != DependentObjectType.None && (!haveSettings || !_settingsData.Weapon.HasValue))
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
                    string sSpell = null;
                    if (oControl == btnLevel1OffensiveSpell)
                        sSpell = realmSpells == null ? "unknown" : realmSpells[0];
                    else if (oControl == btnLevel2OffensiveSpell)
                        sSpell = realmSpells == null ? "unknown" : realmSpells[1];
                    else if (oControl == btnLevel3OffensiveSpell)
                        sSpell = realmSpells == null ? "unknown" : realmSpells[2];
                    else if (oControl == btnStunMob)
                        sSpell = "stun";
                    else if (oControl == btnCastVigor)
                        sSpell = "vigor";
                    else if (oControl == btnCastMend)
                        sSpell = "mend-wounds";
                    else if (oControl == btnCastCurePoison)
                        sSpell = "cure-poison";
                    if (!string.IsNullOrEmpty(sSpell))
                    {
                        enabled = knownSpells.Contains(sSpell);
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
            if (!string.IsNullOrEmpty(command) && echoType != InputEchoType.Off)
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
                lock (_consoleTextLock)
                {
                    string sText = sToConsole + Environment.NewLine;
                    _newConsoleText.Add(sText);
                }
            }
        }

        private void btnOtherSingleMove_Click(object sender, EventArgs e)
        {
            string move = Interaction.InputBox("Move:", "Enter Move", string.Empty);
            if (!string.IsNullOrEmpty(move))
            {
                DoSingleMove(move.ToLower());
            }
        }

        private void btnDoSingleMove_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).Tag.ToString();
            DoSingleMove(direction);
        }

        private void DoSingleMove(string direction)
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
                lock (_queuedCommandLock)
                {
                    bwp.QueuedMeleeStep = step;
                }
            }
        }

        private void RunStandaloneStrategy(Strategy s)
        {
            if (MobEntity.GetMobInfo(txtMob.Text, out string sMobText, out MobTypeEnum? eMobType, out int iMobCounter))
            {
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
            else
            {
                MessageBox.Show("Invalid mob text: " + txtMob.Text);
            }
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
            _bw.RunWorkerAsync(backgroundParameters);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            DoAbort();
        }

        /// <summary>
        /// handles manually completing a perm run. The intent of the button is for when the fulling finishes, but the logic doesn't
        /// pick up on it for a few seconds, so the player can finish it a few seconds early.
        /// </summary>
        private void btnComplete_Click(object sender, EventArgs e)
        {
            _currentBackgroundParameters.Success = true;
            DoAbort();
        }

        private void DoAbort()
        {
            _currentBackgroundParameters.Cancelled = true;
            _bw.CancelAsync();
            btnAbort.Enabled = false;
            btnComplete.Enabled = false;
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

        private void btnFerry_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            using (frmFerry frm = new frmFerry(_currentEntityInfo, _gameMap, GetGraphInputs, _settingsData))
            {
                if (frm.ShowDialog(this) != DialogResult.OK) return;
                bwp.InventoryProcessInputType = ItemsToProcessType.ProcessAllItemsInRoom;
                bwp.InventoryManagementFlow = InventoryManagementWorkflow.Ferry;
                bwp.TargetRoom = frm.SourceRoom;
                bwp.InventorySinkRoom = frm.SinkRoom;
            }
            RunBackgroundProcess(bwp);
        }

        private void RunStrategy(Strategy strategy)
        {
            CommandType eStrategyCombatCommandType = strategy.CombatCommandTypes;
            bool isMeleeStrategy = (eStrategyCombatCommandType & CommandType.Melee) == CommandType.Melee;
            bool hasWeapon = _settingsData.Weapon.HasValue;
            if (isMeleeStrategy && !hasWeapon)
            {
                if (MessageBox.Show("No weapon specified. Continue?", "Strategy", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
            }
            WorkflowSpells workflowSpellsPotions = WorkflowSpells.None;
            ItemsToProcessType inventoryFlow;
            DateTime utcNow = DateTime.UtcNow;

            PromptedSkills skills = _currentEntityInfo.GetAvailableSkills(false);
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

            HealingRoom? initHealingRoom = cboTickRoom.SelectedIndex == 0 ? (HealingRoom?)null : (HealingRoom)cboTickRoom.SelectedItem;
            PawnShoppe? initPawnShoppe = cboPawnShop.SelectedIndex == 0 ? (PawnShoppe?)null : (PawnShoppe)cboPawnShop.SelectedItem;

            bool defaultFullAfterFinishing;
            bool defaultFullBeforeStarting;
            if (strategy.TypesWithStepsEnabled == CommandType.None)
            {
                defaultFullBeforeStarting = false;
                defaultFullAfterFinishing = false;
                inventoryFlow = ItemsToProcessType.ProcessAllItemsInRoom;
            }
            else
            {
                defaultFullAfterFinishing = true;
                inventoryFlow = ItemsToProcessType.ProcessMonsterDrops;
                defaultFullBeforeStarting = _autohp < _totalhp || _automp + GetHealingRoomTickMP() <= _totalmp;
                if (!defaultFullBeforeStarting)
                {
                    WorkflowSpells alwaysOnSpells = _settingsData.AlwaysOnSpells;
                    foreach (WorkflowSpells next in Enum.GetValues(typeof(WorkflowSpells)))
                    {
                        if ((next & alwaysOnSpells) != WorkflowSpells.None)
                        {
                            SpellInformationAttribute sia = SpellsStatic.WorkflowSpellsByEnum[next];
                            lock (_spellsCastLock)
                            {
                                if (!_spellsCast.Contains(sia.SpellName))
                                {
                                    defaultFullBeforeStarting = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            PermRun p;
            using (frmPermRun frm = new frmPermRun(_gameMap, _settingsData, skills, _currentEntityInfo.CurrentRoom, txtMob.Text, GetGraphInputs, strategy, initHealingRoom, initPawnShoppe, inventoryFlow, _currentEntityInfo, defaultFullBeforeStarting, defaultFullAfterFinishing, workflowSpellsCast, workflowSpellsPotions))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                p = new PermRun();
                frm.SaveFormDataToPermRun(p);
            }
            DoPermRun(p);
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
                lock (_currentEntityInfo.SkillsLock)
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
                    List<string> spells = new List<string>();
                    lock (_spellsCastLock)
                    {
                        spells.AddRange(_spellsCast);
                        _refreshSpellsCast = false;
                    }
                    flpSpells.Controls.Clear();
                    foreach (string next in spells)
                    {
                        Label l = new Label();
                        l.AutoSize = true;
                        l.Text = next;
                        flpSpells.Controls.Add(l);
                    }
                }
            }
            if ((initStep & InitializationStep.Time) != InitializationStep.None)
            {
                int iTime = _time;
                lock (_timeLock) //auto-advance the hour if an hour's worth of game time has elapsed
                {
                    DateTime dtTimeLastUpdatedUTC = _timeLastUpdatedUTC;
                    if ((dtUTCNow - dtTimeLastUpdatedUTC).TotalSeconds >= SECONDS_PER_GAME_HOUR)
                    {
                        if (iTime == 23)
                            iTime = 0;
                        else
                            iTime++;
                        _time = iTime;
                        _timeLastUpdatedUTC = dtTimeLastUpdatedUTC.AddSeconds(SECONDS_PER_GAME_HOUR);
                    }
                }
                if (iTime != _timeUI)
                {
                    _timeUI = iTime;
                    Color backColor, foreColor;
                    if (_time >= SUNRISE_GAME_HOUR && _time < SUNSET_GAME_HOUR) //day
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

            double dArmorClass = _armorClass;
            string sArmorClassText = _armorClassText;
            double dArmorClassCalculated = _armorClassCalculated;
            if (dArmorClass != _armorClassUI || dArmorClassCalculated != _armorClassCalculatedUI && sArmorClassText != _armorClassTextUI)
            {
                bool isExact = sArmorClassText.Contains(".");
                if (isExact)
                {
                    lblArmorClassValue.Text = "AC: " + sArmorClassText;
                }
                else if (dArmorClassCalculated != -1)
                {
                    lblArmorClassValue.Text = "AC: " + sArmorClassText + " (" + dArmorClass.ToString("N1") + ")";
                }
                else
                {
                    lblArmorClassValue.Text = "AC: " + sArmorClassText;
                }
                _armorClassUI = dArmorClass;
                _armorClassCalculatedUI = dArmorClassCalculated;
                _armorClassTextUI = sArmorClassText;
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

            if (haveSettings)
            {
                RefreshAutoEscapeUI(!_processedUIWithSettings);
                string sWeapon = _weapon;
                if (_weaponUI != sWeapon)
                {
                    txtWeapon.Text = sWeapon;
                    _weaponUI = sWeapon;
                }
                _processedUIWithSettings = true;
            }

            lock (_broadcastMessagesLock)
            {
                if (_broadcastMessages.Count > 0)
                {
                    foreach (string nextMessage in _broadcastMessages)
                    {
                        DateTime dtNow = DateTime.Now;
                        lstMessages.Items.Add(dtNow.Day.ToString().PadLeft(2, '0') + "/" + dtNow.Month.ToString().PadLeft(2, '0') + "/" + dtNow.Year.ToString().Substring(2, 2) + " " + dtNow.Hour.ToString().PadLeft(2, '0') + ":" + dtNow.Minute.ToString().PadLeft(2, '0') + ":" + dtNow.Second.ToString().PadLeft(2, '0') + " " + nextMessage);
                    }
                    lstMessages.TopIndex = lstMessages.Items.Count - 1;
                    _broadcastMessages.Clear();
                }
            }

            BackgroundWorkerParameters bwp = _currentBackgroundParameters;

            lock (_currentEntityInfo.EntityLock)
            {
                Room oCurrentRoom = _currentEntityInfo.CurrentRoom;
                if (oCurrentRoom != _currentEntityInfo.CurrentRoomUI)
                {
                    if (_setTickOrPawnRoom && oCurrentRoom != null)
                    {
                        HealingRoom? healFlag = oCurrentRoom.HealingRoom;
                        if (healFlag.HasValue)
                        {
                            cboTickRoom.SelectedItem = healFlag.Value;
                        }
                        else
                        {
                            PawnShoppe? pawnFlag = oCurrentRoom.PawnShoppe;
                            if (pawnFlag.HasValue)
                            {
                                cboPawnShop.SelectedItem = pawnFlag.Value;
                            }
                        }
                        _setTickOrPawnRoom = false;
                    }
                    string sCurrentRoom;
                    if (oCurrentRoom != null)
                    {
                        sCurrentRoom = oCurrentRoom.Name;
                    }
                    else
                    {
                        sCurrentRoom = "No Current Room";
                    }
                    grpCurrentRoom.Text = sCurrentRoom;
                    _currentEntityInfo.CurrentRoomUI = oCurrentRoom;
                    if (_currentBackgroundParameters == null)
                    {
                        btnGoToHealingRoom.Enabled = oCurrentRoom != null;
                        RefreshEnabledForSingleMoveButtons();
                    }
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
                                if (bwp == null)
                                {
                                    string sNewMobText = string.Empty;
                                    if (_currentEntityInfo.CurrentRoomMobs.Count > 0)
                                    {
                                        sNewMobText = _currentEntityInfo.CurrentRoomMobs[0].ToString();
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
                                lstInventory.Items.Add(new ItemInInventoryList(nextEntry.Item));
                            }
                        }
                        else if (rcType == EntityChangeType.RefreshEquipment)
                        {
                            lstEquipment.Items.Clear();
                            foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                            {
                                lstEquipment.Items.Add(new ItemInEquipmentList(nextEntry.Item));
                            }
                        }
                        else
                        {
                            if (rcType == EntityChangeType.DropItem || rcType == EntityChangeType.ConsumeItem || rcType == EntityChangeType.EquipItem)
                            {
                                foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                                {
                                    if (nextEntry.InventoryAction.HasValue && !nextEntry.InventoryAction.Value)
                                    {
                                        lstInventory.Items.RemoveAt(nextEntry.InventoryIndex);
                                    }
                                }
                            }
                            if (rcType == EntityChangeType.PickUpItem || rcType == EntityChangeType.UnequipItem || rcType == EntityChangeType.MagicallySentItem)
                            {
                                foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                                {
                                    if (nextEntry.InventoryAction.GetValueOrDefault(true))
                                    {
                                        ItemInInventoryList it = new ItemInInventoryList(nextEntry.Item);
                                        if (nextEntry.InventoryIndex == -1)
                                        {
                                            lstInventory.Items.Add(it);
                                        }
                                        else
                                        {
                                            lstInventory.Items.Insert(nextEntry.InventoryIndex, it);
                                        }
                                    }
                                }
                            }
                            if (rcType == EntityChangeType.EquipItem)
                            {
                                foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                                {
                                    if (nextEntry.EquipmentAction.GetValueOrDefault(true))
                                    {
                                        ItemInEquipmentList it = new ItemInEquipmentList(nextEntry.Item);
                                        if (nextEntry.EquipmentIndex == -1)
                                        {
                                            lstEquipment.Items.Add(it);
                                        }
                                        else
                                        {
                                            lstEquipment.Items.Insert(nextEntry.EquipmentIndex, it);
                                        }
                                    }
                                }
                            }
                            else if (rcType == EntityChangeType.UnequipItem || rcType == EntityChangeType.DestroyEquipment)
                            {
                                foreach (EntityChangeEntry nextEntry in nextEntityChange.Changes)
                                {
                                    if (nextEntry.EquipmentAction.HasValue && !nextEntry.EquipmentAction.Value)
                                    {
                                        lstEquipment.Items.RemoveAt(nextEntry.EquipmentIndex);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EnableDisableActionButtons(bwp);
            if (bwp == null) //processing that only happens when a background process is not running
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
            string sNodeText = nextExit.ExitText + " (" + nextExit.Target.Name + ")";
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
                    sNodeText += " (" + foundExit.Target.Name + ")";
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
                                DoSingleMove(sCommandLower);
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
                bwp.Foreground = true;
                RunBackgroundProcess(bwp);
            }
            else
            {
                bwp.DoScore = true;
            }
        }

        private void ctxStrategy_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ctx = (ContextMenuStrip)sender;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
            Button sourceButton = (Button)ctx.SourceControl;
            if (clickedItem.Text == "Edit")
            {
                Strategy s = (Strategy)sourceButton.Tag;
                frmStrategy frm = new frmStrategy(s, _settingsData);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    sourceButton.Tag = s;
                    sourceButton.Text = s.ToString();
                }
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
            lock (_spellsCastLock)
            {
                flying = _spellsCast.Contains("fly");
                levitating = _spellsCast.Contains("levitation");
            }
            return new GraphInputs(_class, _level, TimeOutputSequence.IsDay(_time), flying, levitating);
        }

        private List<Exit> CalculateRouteExits(Room fromRoom, Room targetRoom, bool silent)
        {
            GraphInputs gi = GetGraphInputs();
            List <Exit> pathExits = MapComputation.ComputeLowestCostPath(fromRoom, targetRoom, gi);
            if (pathExits == null && !silent)
            {
                MessageBox.Show("No path to target room found.");
            }
            return pathExits;
        }

        private void GoToRoom(Room targetRoom)
        {
            Room currentRoom = _currentEntityInfo.CurrentRoom;
            if (currentRoom != null)
            {
                if (currentRoom == targetRoom)
                {
                    MessageBox.Show("Already at specified room.");
                }
                else
                {
                    List<Exit> exits = CalculateRouteExits(currentRoom, targetRoom, false);
                    if (exits != null)
                    {
                        NavigateExitsInBackground(exits);
                    }
                }
            }
            else
            {
                MessageBox.Show("No current room, unable to travel to specified room.");
            }
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

        private bool GetVerboseMode()
        {
            return _settingsData == null ? true : _settingsData.VerboseMode;
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

        private void cboTickRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasTickRoom = cboTickRoom.SelectedIndex > 0;
            btnGoToHealingRoom.Enabled = hasTickRoom;
            if (hasTickRoom)
            {
                HealingRoom eHealingRoom = (HealingRoom)cboTickRoom.SelectedItem;
                if (Enum.TryParse(eHealingRoom.ToString(), out PawnShoppe pawnShoppe))
                {
                    cboPawnShop.SelectedItem = pawnShoppe;
                }
            }
        }

        private void btnGoToHealingRoom_Click(object sender, EventArgs e)
        {
            GoToRoom(_gameMap.HealingRooms[(HealingRoom)cboTickRoom.SelectedItem]);
        }

        private void cboPawnShop_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasPawnRoom = cboPawnShop.SelectedIndex > 0;
            btnGoToPawnShop.Enabled = hasPawnRoom;
            if (hasPawnRoom)
            {
                PawnShoppe ePawnShoppe = (PawnShoppe)cboPawnShop.SelectedItem;
                if (Enum.TryParse(ePawnShoppe.ToString(), out HealingRoom healingRoom))
                {
                    cboTickRoom.SelectedItem = healingRoom;
                }
            }
        }

        private void btnGoToPawnShop_Click(object sender, EventArgs e)
        {
            GoToRoom(_gameMap.PawnShoppes[(PawnShoppe)cboPawnShop.SelectedItem]);
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

        private void treeCurrentRoom_DoubleClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeCurrentRoom.SelectedNode;
            TreeNode parentNode = selectedNode.Parent;
            object oTag = selectedNode.Tag;
            if (parentNode == _currentEntityInfo.tnObviousItems) //pick up item
            {
                int counter = FindItemOrMobCounterInRoomUI(selectedNode, true);
                ItemEntity ie = (ItemEntity)selectedNode.Tag;
                lock (_currentEntityInfo.EntityLock)
                {
                    string sItemText = _currentEntityInfo.PickItemTextFromItemCounter(ItemLocationType.Room, ie.ItemType.Value, counter, false, false);
                    if (string.IsNullOrEmpty(sItemText))
                    {
                        MessageBox.Show("Unable to pick up item.");
                    }
                    else
                    {
                        SendCommand("get " + sItemText, InputEchoType.On);
                    }
                }
            }
            else if (parentNode == _currentEntityInfo.tnObviousExits || parentNode == _currentEntityInfo.tnOtherExits)
            {
                if (oTag is Exit)
                {
                    NavigateSingleExitInBackground((Exit)oTag);
                }
                else //string
                {
                    DoSingleMove(oTag.ToString());
                }
            }
            else if (parentNode == _currentEntityInfo.tnObviousMobs || parentNode == _currentEntityInfo.tnPermanentMobs) //look at mob
            {
                int counter = FindItemOrMobCounterInRoomUI(selectedNode, false);
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

        private int FindItemOrMobCounterInRoomUI(TreeNode node, bool isItem)
        {
            int counter = 0;
            ItemEntity ie;
            ItemTypeEnum ieType = ItemTypeEnum.GoldCoins;
            MobTypeEnum mtType = MobTypeEnum.LittleMouse;
            if (isItem)
            {
                ie = (ItemEntity)node.Tag;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                RunSingleBackgroundCommand(BackgroundCommandType.Search);
            }
        }

        private void ctxInventoryOrEquipmentItem_Opening(object sender, CancelEventArgs e)
        {
            ctxInventoryOrEquipmentItem.Items.Clear();
            ListBox lst = (ListBox)ctxInventoryOrEquipmentItem.SourceControl;
            bool isInventory = lst == lstInventory;
            StaticItemData sid = null;
            int iCounter = 0;
            ItemTypeEnum itemType = ItemTypeEnum.GoldCoins;
            Room r;
            lock (_currentEntityInfo.EntityLock)
            {
                object oObj = lst.SelectedItem;
                if (oObj == null)
                {
                    e.Cancel = true;
                    return;
                }
                ItemEntity ie = isInventory ? ((ItemInInventoryList)oObj).Item : ((ItemInEquipmentList)oObj).Item;
                if (ie.ItemType.HasValue)
                {
                    itemType = ie.ItemType.Value;
                    sid = ItemEntity.StaticItemData[itemType];
                }
                else //unknown item cannot be acted on since selection text cannot be constructed
                {
                    e.Cancel = true;
                    return;
                }
                if (isInventory)
                {
                    foreach (ItemInInventoryList nextEntry in lstInventory.Items)
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
                    foreach (ItemInEquipmentList nextEntry in lstEquipment.Items)
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
                r = _currentEntityInfo.CurrentRoom;
            }

            SelectedInventoryOrEquipmentItem sioei = new SelectedInventoryOrEquipmentItem();
            sioei.ItemType = itemType;
            sioei.Counter = iCounter;
            sioei.IsInventory = isInventory;
            ctxInventoryOrEquipmentItem.Tag = sioei;

            string sActionTransferBetweenInventoryAndEquipment;
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
                if (iclass == ItemClass.Scroll)
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = "read";
                    ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                }
                else if (iclass == ItemClass.Usable)
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = "use";
                    ctxInventoryOrEquipmentItem.Items.Add(tsmi);
                }
                switch (iclass)
                {
                    case ItemClass.Equipment:
                        sActionTransferBetweenInventoryAndEquipment = "wear";
                        break;
                    case ItemClass.Weapon:
                        sActionTransferBetweenInventoryAndEquipment = "wield";
                        break;
                    default:
                        sActionTransferBetweenInventoryAndEquipment = "hold";
                        break;
                }
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "drop";
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            else
            {
                sActionTransferBetweenInventoryAndEquipment = "remove";
            }
            if (sActionTransferBetweenInventoryAndEquipment != null)
            {
                tsmi = new ToolStripMenuItem();
                tsmi.Text = sActionTransferBetweenInventoryAndEquipment;
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
            if (iclass == ItemClass.Weapon && _settingsData.Weapon != itemType)
            {
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "Set Weapon";
                ctxInventoryOrEquipmentItem.Items.Add(tsmi);
            }
        }

        private void ctxInventoryOrEquipmentItem_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SelectedInventoryOrEquipmentItem sioei = (SelectedInventoryOrEquipmentItem)ctxInventoryOrEquipmentItem.Tag;
            ItemTypeEnum eItemType = sioei.ItemType;
            ToolStripMenuItem tsmi = (ToolStripMenuItem)e.ClickedItem;
            string command = tsmi.Text;
            if (tsmi.Text == "Set Weapon")
            {
                _settingsData.Weapon = eItemType;
                txtWeapon.Text = eItemType.ToString();
            }
            else
            {
                ItemLocationType ilt = sioei.IsInventory ? ItemLocationType.Inventory : ItemLocationType.Equipment;
                lock (_currentEntityInfo.EntityLock)
                {
                    bool validateAgainstOtherSources;
                    if (ilt == ItemLocationType.Equipment)
                        validateAgainstOtherSources = command != "remove";
                    else
                        validateAgainstOtherSources = false;
                    string sText = _currentEntityInfo.PickItemTextFromItemCounter(ilt, eItemType, sioei.Counter, false, validateAgainstOtherSources);
                    if (!string.IsNullOrEmpty(sText))
                    {
                        SendCommand(command + " " + sText, InputEchoType.On);
                    }
                }
            }
        }

        private class SelectedInventoryOrEquipmentItem
        {
            public ItemTypeEnum ItemType;
            public int Counter;
            public bool IsInventory;
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
            frmConfiguration frm = new frmConfiguration(clone, autoEscapeThreshold, autoEscapeType, autoEscapeActive);
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
                    if (errorMessages.Count > 0)
                    {
                        lock (_broadcastMessagesLock)
                        {
                            _broadcastMessages.AddRange(errorMessages);
                        }
                    }
                    if (success)
                    {
                        AfterLoadSettings();
                        MessageBox.Show("Imported!");
                    }
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
            if (errorMessages.Count > 0)
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.AddRange(errorMessages);
                }
            }
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
            using (frmPermRuns frm = new frmPermRuns(_settingsData, _gameMap, _currentEntityInfo, GetGraphInputs))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    DoPermRun(frm.PermRunToRun);
                }
            }
        }

        private void DoPermRun(PermRun p)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.SetPermRun(p, _gameMap);
            bwp.InventoryManagementFlow = InventoryManagementWorkflow.ManageSourceItems;
            bwp.BeforeGold = _gold;
            bwp.BeforeExperience = _experience;
            RunBackgroundProcess(bwp);
        }
    }
}
