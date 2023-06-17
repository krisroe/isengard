using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmMain : Form
    {
        private TcpClient _tcpClient;
        private NetworkStream _tcpClientNetworkStream;

        private RealmType _currentRealm;
        private int _autoSpellLevelMin = -1;
        private int _autoSpellLevelMax = -1;

        private string _mob;
        private string _weapon;
        private string _wand;

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
        private int _level = 0;
        private string _currentPlayerHeader = null;
        private string _currentPlayerHeaderUI = null;

        private InitializationStep _initializationSteps;
        private InitialLoginInfo _loginInfo;

        private static DateTime? _currentStatusLastComputed;
        private static DateTime? _lastPollTick;
        private bool _verboseMode;
        private bool _queryMonsterStatus;
        private bool _finishedQuit;

        private object _spellsCastLock = new object();
        private List<string> _spellsCast = new List<string>();
        private object _spellsKnownLock = new object();
        private List<string> _spellsKnown = new List<string>();
        private bool _refreshSpellsCast = false;

        /// <summary>
        /// current list of players. This list is not necessarily accurate since invisible players
        /// are hidden from the who output when you cannot detect them.
        /// </summary>
        private HashSet<string> _players = null;

        private object _skillsLock = new object();
        private List<SkillCooldown> _cooldowns = new List<SkillCooldown>();

        private string _username;
        private string _password;
        private bool _promptedUserName;
        private bool _promptedPassword;
        private bool _enteredUserName;
        private bool _enteredPassword;
        private int _totalhp = 0;
        private int _totalmp = 0;
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

        private int _autoEscapeThreshold;
        private int _autoEscapeThresholdUI;
        private AutoEscapeType _autoEscapeType;
        private AutoEscapeType _autoEscapeTypeUI;
        private bool _autoEscapeActive;
        private bool _autoEscapeActiveUI;
        private bool _autoEscapeActiveSaved;
        private bool _fleeing;
        private bool _fleeingUI;
        private bool _hazying;
        private bool _hazyingUI;
        private object _escapeLock = new object();

        private IsengardMap _gameMap;

        private Room m_oCurrentRoom;
        private Room m_oCurrentRoomUI;
        private List<RoomChange> _currentRoomChanges = new List<RoomChange>();
        private object _roomChangeLock = new object();
        private int _roomChangeCounter = -1;
        private int _roomChangeCounterUI = -1;
        private List<string> _currentObviousExits;
        private List<string> _foundSearchedExits;
        private List<MobTypeEnum> _currentRoomMobs;
        private TreeNode _tnObviousMobs;
        private bool _obviousMobsTNExpanded = true;
        private TreeNode _tnObviousExits;
        private bool _obviousExitsTNExpanded = true;
        private TreeNode _tnOtherExits;
        private bool _otherExitsTNExpanded = true;
        private TreeNode _tnPermanentMobs;
        private bool _permMobsTNExpanded = true;
        private bool _programmaticUI = false;

        private bool _setTickRoom = false;
        private BackgroundWorker _bw;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private BackgroundProcessPhase _backgroundProcessPhase;
        private PleaseWaitSequence _pleaseWaitSequence;
        private Color _fullColor;
        private Color _emptyColor;

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
        private bool _fumbled;
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
        
        private string _currentlyFightingMob;
        private string _currentlyFightingMobUI;
        private MonsterStatus _currentMonsterStatus;
        private MonsterStatus _currentMonsterStatusUI;
        private int _monsterDamage;
        private int _monsterDamageUI;
        private bool _monsterStunned;
        private bool _monsterKilled;
        private MobTypeEnum? _monsterKilledType;

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

        private List<Strategy> _strategies;

        internal frmMain(string userName, string password)
        {
            InitializeComponent();

            this.MinimumSize = this.Size;
            _tnObviousMobs = treeCurrentRoom.Nodes["tnObviousMobs"];
            _tnObviousExits = treeCurrentRoom.Nodes["tnObviousExits"];
            _tnOtherExits = treeCurrentRoom.Nodes["tnOtherExits"];
            _tnPermanentMobs = treeCurrentRoom.Nodes["tnPermanentMobs"];

            cboTickRoom.Items.Add(string.Empty);
            foreach (var nextHealingRoom in Enum.GetValues(typeof(HealingRoom)))
            {
                cboTickRoom.Items.Add(nextHealingRoom);
            }

            _strategies = Strategy.GetDefaultStrategies();

            _pleaseWaitSequence = new PleaseWaitSequence(OnWaitXSeconds);

            SetButtonTags();

            _asciiMapping = AsciiMapping.GetAsciiMapping();

            IsengardSettings sets = IsengardSettings.Default;
            _verboseMode = sets.VerboseMode;
            _queryMonsterStatus = sets.QueryMonsterStatus;
            _fullColor = sets.FullColor;
            _emptyColor = sets.EmptyColor;

            InitializeRealm();
            InitializeAutoSpellLevels();

            AlignmentType ePreferredAlignment;
            if (!Enum.TryParse(sets.PreferredAlignment, out ePreferredAlignment))
            {
                ePreferredAlignment = AlignmentType.Blue;
            }

            SetAutoEscapeThresholdFromDefaults();

            txtWeapon.Text = sets.DefaultWeapon ?? string.Empty;

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

            RefreshStrategyButtons();

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            _gameMap = new IsengardMap(ePreferredAlignment);

            cboSetOption.SelectedIndex = 0;

            DoConnect();
        }

        private void SetAutoEscapeThresholdFromDefaults()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _autoEscapeThreshold = sets.DefaultAutoEscapeThreshold;
            _autoEscapeType = (AutoEscapeType)sets.DefaultAutoEscapeType;
            _autoEscapeActive = sets.DefaultAutoEscapeOnByDefault;
            if (_autoEscapeType != AutoEscapeType.Flee && _autoEscapeType != AutoEscapeType.Hazy)
            {
                _autoEscapeType = AutoEscapeType.Flee;
                sets.DefaultAutoEscapeType = Convert.ToInt32(_autoEscapeType);
            }
            if (_autoEscapeThreshold < 0)
            {
                _autoEscapeThreshold = 0;
                sets.DefaultAutoEscapeThreshold = _autoEscapeThreshold;
            }
            if (_autoEscapeThreshold == 0)
            {
                _autoEscapeActive = false;
                sets.DefaultAutoEscapeOnByDefault = _autoEscapeActive;
            }
            RefreshAutoEscapeUI(true);
        }

        private void RefreshStrategyButtons()
        {
            flpOneClickStrategies.Controls.Clear();
            int iOneClickTabIndex = 0;
            foreach (Strategy oStrategy in _strategies)
            {
                Button btnOneClick = new Button();
                btnOneClick.AutoSize = true;
                btnOneClick.TabIndex = iOneClickTabIndex++;
                btnOneClick.Tag = oStrategy;
                btnOneClick.Text = oStrategy.ToString();
                btnOneClick.UseVisualStyleBackColor = true;
                btnOneClick.Click += btnOneClick_Click;
                btnOneClick.ContextMenuStrip = ctxRoomExits;
                flpOneClickStrategies.Controls.Add(btnOneClick);
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
            _tnl = -1;
            _tnlUI = -1;
            lock (_skillsLock)
            {
                _cooldowns.Clear();
            }
            lock (_spellsCastLock)
            {
                _spellsCast.Clear();
                _refreshSpellsCast = true;
            }
            ClearConsole();
            _finishedQuit = false;
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
            if (!_finishedQuit)
            {
                if (MessageBox.Show("Disconnected. Reconnect?", "Disconnected", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DoConnect();
                }
                else
                {
                    _finishedQuit = true;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// handler for the output of score
        /// </summary>
        private void OnScore(FeedLineParameters flParams, int level, int maxHP, int maxMP, int tnl, List<SkillCooldown> cooldowns, List<string> spells, bool poisoned)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Score) == InitializationStep.None;

            bool suppressEcho = forInit;

            lock (_skillsLock)
            {
                bool clear = false;
                if (cooldowns.Count != _cooldowns.Count)
                {
                    clear = true;
                }
                else
                {
                    for (int i = 0; i < cooldowns.Count; i++)
                    {
                        if (cooldowns[i].SkillType != _cooldowns[i].SkillType)
                        {
                            clear = true;
                            break;
                        }
                    }
                }
                if (clear)
                {
                    _cooldowns.Clear();
                    _cooldowns.AddRange(cooldowns);
                }
                else //copy into the existing structures
                {
                    for (int i = 0; i < cooldowns.Count; i++)
                    {
                        SkillCooldown oExisting = _cooldowns[i];
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

            _level = level;
            _totalhp = maxHP;
            _totalmp = maxMP;
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

            lock (_spellsKnownLock)
            {
                _spellsKnown = SpellsList;
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

        private void OnRemoveEquipment(FeedLineParameters flParams)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.RemoveAll) == InitializationStep.None;

            //currently there is no processing of removing equipment

            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.RemoveAll, flParams);
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

        private void OnInitialLogin(InitialLoginInfo initialLoginInfo)
        {
            SendCommand("score", InputEchoType.Off);
            SendCommand("who", InputEchoType.Off);
            SendCommand("remove all", InputEchoType.Off);
            SendCommand("time", InputEchoType.Off);
            SendCommand("spells", InputEchoType.Off);
            _initializationSteps |= InitializationStep.Initialization;
            _loginInfo = initialLoginInfo;
        }

        private void ProcessInitialLogin(FeedLineParameters flp)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.UserName = _username;
            sets.Save();

            InitialLoginInfo info = _loginInfo;
            string sRoomName = info.RoomName;
            if (RoomTransitionSequence.ProcessRoom(sRoomName, info.ObviousExits, info.List1, info.List2, info.List3, OnRoomTransition, flp, RoomTransitionType.Initial, 0, TrapType.None))
            {
                _initializationSteps |= InitializationStep.Finalization;
                Room r = m_oCurrentRoom;
                _setTickRoom = r != null && r.HealingRoom.HasValue;
            }
            else
            {
                flp.ErrorMessages.Add("Initial login failed!");
            }
        }

        private Room GetCurrentRoomIfUnambiguous(string sRoomName)
        {
            Room ret;
            _gameMap.UnambiguousRooms.TryGetValue(sRoomName, out ret);
            return ret;
        }

        private void OnRoomTransition(FeedLineParameters flParams, RoomTransitionInfo roomTransitionInfo, int damage, TrapType trapType)
        {
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
            
            Room previousRoom = m_oCurrentRoom;

            //first try is always to look for an unambiguous name across the whole map
            Room newRoom = GetCurrentRoomIfUnambiguous(sRoomName);
            List<Room> disambiguationRooms = null;
            bool lookForRoomsByRoomName = false;
            if (rtType == RoomTransitionType.Flee)
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
            else if (rtType == RoomTransitionType.Hazy || rtType == RoomTransitionType.Death)
            {
                _hazying = false;
                _fleeing = false;
                if (fromAnyBackgroundCommand) //abort whatever background command is currently running
                {
                    if (fromBackgroundHazy && rtType == RoomTransitionType.Hazy)
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
                if (newRoom == null && _currentBackgroundExit.Target != null)
                {
                    Room targetRoom = _currentBackgroundExit.Target;
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
                if (_gameMap.AmbiguousRooms.TryGetValue(sRoomName, out List<Room> possibleRooms))
                {
                    disambiguationRooms.AddRange(possibleRooms);
                }
            }

            if (disambiguationRooms != null) //disambiguate based on obvious exits
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
                if (foundRoom != null)
                {
                    newRoom = foundRoom;
                }
            }
            if (newRoom == null && fromBackgroundLook && previousRoom != null)
            {
                newRoom = previousRoom;
            }

            lock (_roomChangeLock) //update the room change list with the next room
            {
                _currentObviousExits = obviousExits;

                _currentRoomChanges.Clear();
                RoomChange rc = new RoomChange();
                rc.Room = newRoom;
                rc.ChangeType = RoomChangeType.NewRoom;
                rc.Exits = new List<string>(obviousExits);
                _roomChangeCounter++;
                rc.GlobalCounter = _roomChangeCounter;
                if (newRoom != null)
                {
                    rc.MappedExits = new Dictionary<string, Exit>();
                    Dictionary<string, List<Exit>> periodicExits = new Dictionary<string, List<Exit>>();
                    foreach (Exit nextExit in IsengardMap.GetAllRoomExits(newRoom))
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
                        else if (nextExit.PresenceType != ExitPresenceType.RequiresSearch && obviousExits.Contains(nextExitText)) //requires search exits behave like hidden ones
                        {
                            rc.MappedExits[nextExitText] = nextExit;
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
                }

                List<MobTypeEnum> mobEnums = new List<MobTypeEnum>();
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
                                mobEnums.Add(mobTypeValue);
                            }
                        }
                    }
                }
                rc.Mobs = mobEnums;
                _currentRoomMobs = new List<MobTypeEnum>(mobEnums);

                _currentRoomChanges.Add(rc);
                m_oCurrentRoom = newRoom;
            }
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

        private void ChangeSkillActive(SkillWithCooldownType skill, bool active)
        {
            lock (_skillsLock)
            {
                foreach (SkillCooldown nextCooldown in _cooldowns)
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

        private void OnLifeSpellCast(FeedLineParameters flParams, BackgroundCommandType lifeSpellCommandType)
        {
            string activeSpell = null;
            if (lifeSpellCommandType == BackgroundCommandType.Bless)
                activeSpell = "bless";
            else if (lifeSpellCommandType == BackgroundCommandType.Protection)
                activeSpell = "protection";
            if (!string.IsNullOrEmpty(activeSpell))
            {
                AddActiveSpell(activeSpell);
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == lifeSpellCommandType)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void AddActiveSpell(string spellName)
        {
            lock (_spellsCast)
            {
                if (!_spellsCast.Contains(spellName))
                {
                    if (_spellsCast.Contains("None"))
                    {
                        _spellsCast.Remove("None");
                    }
                    _spellsCast.Add(spellName);
                    _refreshSpellsCast = true;
                }
            }
        }

        private void OnFly(FeedLineParameters flParams)
        {
            AddActiveSpell("fly");
        }

        private void FailMovement(FeedLineParameters flParams, MovementResult movementResult, int damage)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Movement)
            {
                _lastCommandDamage = damage;
                _lastCommandMovementResult = movementResult;

                //even though some of these results are fixable (e.g. trap rooms), return full failure to allow the caller
                //to decide to heal.
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

        private static void OpenDoorSuccess(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.OpenDoor)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OpenDoorFailure(FeedLineParameters flParams)
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

        private void FailDrinkHazy(FeedLineParameters flParams)
        {
            _hazying = false;
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.DrinkHazy)
            {
                flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
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

        private void OnEntityAttacksYou(FeedLineParameters flParams)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp != null && !string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                _monsterStunned = false;
            }
        }

        private void OnStun(FeedLineParameters flParams)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp != null && !string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                _monsterStunned = true;
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Stun)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
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
        
        private void OnAttack(bool fumbled, int damage, bool killedMonster, MobTypeEnum? eMobType, int experience, bool powerAttacked, FeedLineParameters flParams)
        {
            if (powerAttacked)
            {
                ChangeSkillActive(SkillWithCooldownType.PowerAttack, false);
            }
            _tnl = Math.Max(0, _tnl - experience);
            if (!string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                if (fumbled)
                {
                    _fumbled = true;
                }
                else if (killedMonster)
                {
                    _monsterKilled = true;
                    _monsterKilledType = eMobType;
                }
                _monsterDamage += damage;
            }
            if (eMobType.HasValue)
            {
                RemoveMobs(eMobType.Value, 1);
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
            {
                _lastCommandDamage = damage;
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void OnCastOffensiveSpell(int damage, bool killedMonster, MobTypeEnum? mobType, int experience, FeedLineParameters flParams)
        {
            _tnl = Math.Max(0, _tnl - experience);
            if (!string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                _monsterDamage += damage;
                if (killedMonster)
                {
                    _monsterKilled = true;
                    _monsterKilledType = mobType;
                }
            }
            if (mobType.HasValue)
            {
                RemoveMobs(mobType.Value, 1);
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
            RoomChange rc;
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
                    case InformationalMessageType.ManashieldOff:
                        ChangeSkillActive(SkillWithCooldownType.Manashield, false);
                        break;
                    case InformationalMessageType.FleeFailed:
                        finishedProcessing = true;
                        break;
                    case InformationalMessageType.BullroarerInMithlond:
                        lock (_roomChangeLock)
                        {
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInMithlond, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerInNindamos:
                        lock (_roomChangeLock)
                        {
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInNindamos, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerReadyForBoarding:
                        lock (_roomChangeLock)
                        {
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressInBree:
                        lock (_roomChangeLock)
                        {
                            bool removeMessage = true;
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "dock"));
                                        removeMessage = false;
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "steamboat"));
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
                        lock (_roomChangeLock)
                        {
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "dock"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "steamboat"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressLeftMithlond:
                        lock (_roomChangeLock)
                        {
                            Room currentRoom = m_oCurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "pier"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressMithlond:
                                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "gangway"));
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
                        rc = new RoomChange();
                        rc.ChangeType = RoomChangeType.AddMob;
                        rc.Mobs = new List<MobTypeEnum>();
                        for (int i = 0; i < next.MobCount; i++)
                        {
                            rc.Mobs.Add(next.Mob);
                        }
                        rc.GlobalCounter = _roomChangeCounter;
                        lock (_roomChangeLock)
                        {
                            _roomChangeCounter++;
                            rc.GlobalCounter = _roomChangeCounter;
                            _currentRoomChanges.Add(rc);
                        }
                        break;
                    case InformationalMessageType.MobWanderedAway:
                        RemoveMobs(next.Mob, next.MobCount);
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
                    if (_players.Contains(s))
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

        private void RemoveMobs(MobTypeEnum mobType, int Count)
        {
            RoomChange rc = new RoomChange();
            rc.ChangeType = RoomChangeType.RemoveMob;
            rc.Mobs = new List<MobTypeEnum>();
            for (int i = 0; i < Count; i++)
            {
                rc.Mobs.Add(mobType);
            }
            rc.GlobalCounter = _roomChangeCounter;
            lock (_roomChangeLock)
            {
                _roomChangeCounter++;
                rc.GlobalCounter = _roomChangeCounter;
                _currentRoomChanges.Add(rc);
            }
        }

        private void HandleHarbringerStatusChange(bool inPort)
        {
            lock (_roomChangeLock)
            {
                Room currentRoom = m_oCurrentRoom;
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
                        _currentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, inPort, sExit));
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
        private RoomChange GetAddExitRoomChangeForPeriodicExit(Room currentRoom, bool add, string exitText)
        {
            RoomChange rc = new RoomChange();
            rc.ChangeType = add ? RoomChangeType.AddExit : RoomChangeType.RemoveExit;
            _roomChangeCounter++;
            rc.GlobalCounter = _roomChangeCounter;
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
        private RoomChange GetAddExitRoomChangeForWaitForMessageExit(Room currentRoom, InformationalMessageType messageType, bool add)
        {
            RoomChange rc = new RoomChange();
            rc.ChangeType = add ? RoomChangeType.AddExit : RoomChangeType.RemoveExit;
            _roomChangeCounter++;
            rc.GlobalCounter = _roomChangeCounter;
            Exit e = IsengardMap.GetRoomExits(currentRoom, (exit) => { return exit.WaitForMessage.HasValue && exit.WaitForMessage.Value == messageType; }).First();
            rc.Exits.Add(e.ExitText);
            if (add)
            {
                rc.MappedExits[e.ExitText] = e;
            }
            return rc;
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
                            _finishedQuit = true;
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
                            flParams.CurrentlyFightingMob = _currentlyFightingMob;
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
                                if (flParams.SuppressEcho && !_verboseMode)
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
                                if (!_verboseMode && previousCommandResultCounter == newCommandResultCounter && !previousCommandResult.HasValue && flParams.CommandResult.HasValue)
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
                if (_autoEscapeActive && _autoEscapeThreshold > 0 && newHP < _autoEscapeThreshold)
                {
                    if (_autoEscapeType == AutoEscapeType.Flee)
                        _fleeing = true;
                    else if (_autoEscapeType == AutoEscapeType.Hazy)
                        _hazying = true;
                    _autoEscapeActive = false;
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
                new WhoOutputSequence(OnWho),
                new SpellsSequence(OnSpells),
                new RemoveEquipmentSequence(OnRemoveEquipment),
                new MobStatusSequence(OnMobStatusSequence),
                new TimeOutputSequence(OnTime),
                _pleaseWaitSequence,
                new RoomTransitionSequence(OnRoomTransition),
                new FailMovementSequence(FailMovement),
                new EntityAttacksYouSequence(OnEntityAttacksYou),
                new ConstantOutputSequence("You creative a protective manashield.", OnManashieldOn, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains, null), //could be prefixed by "Scared of going X"*
                new LifeSpellCastSequence(OnLifeSpellCast),
                new ConstantOutputSequence("Stun cast on ", OnStun, ConstantSequenceMatchType.StartsWith, 0, BackgroundCommandType.Stun),
                new ConstantOutputSequence("Your spell fails.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. alignment out of whack
                new ConstantOutputSequence("You don't know that spell.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells),
                new ConstantOutputSequence("Nothing happens.", OnSpellFails, ConstantSequenceMatchType.ExactMatch, 0, _backgroundSpells), //e.g. casting a spell from the tree of life
                new AttackSequence(OnAttack),
                new CastOffensiveSpellSequence(OnCastOffensiveSpell),
                new ConstantOutputSequence("You don't see that here.", OnYouDontSeeThatHere, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.Attack, BackgroundCommandType.LookAtMob }),
                new ConstantOutputSequence("That is not here.", OnThatIsNotHere, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Attack), //triggered by power attack
                new ConstantOutputSequence("That's not here.", OnCastOffensiveSpellMobNotPresent, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.OffensiveSpell, BackgroundCommandType.Stun }),
                new ConstantOutputSequence("It's not locked.", SuccessfulKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You successfully open the lock.", SuccessfulKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You failed.", FailKnock, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Knock),
                new ConstantOutputSequence("You don't have that.", FailDrinkHazy, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.DrinkHazy),
                new ConstantOutputSequence(" starts to evaporates before you drink it.", FailDrinkHazy, ConstantSequenceMatchType.EndsWith, 0, BackgroundCommandType.DrinkHazy),
                new ConstantOutputSequence("You prepare yourself for traps.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("You've already prepared.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("You can fly!", OnFly, ConstantSequenceMatchType.ExactMatch, 0, (BackgroundCommandType?)null),
                new ConstantOutputSequence("I don't see that exit.", OpenDoorFailure, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("You open the ", OpenDoorSuccess, ConstantSequenceMatchType.StartsWith, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("It's already open.", OpenDoorSuccess, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("It's locked.", OpenDoorFailure, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
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
            btnLevel1OffensiveSpell.Tag = new CommandButtonTag(btnLevel1OffensiveSpell, "cast {realm1spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnLevel2OffensiveSpell.Tag = new CommandButtonTag(btnLevel2OffensiveSpell, "cast {realm2spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnLevel3OffensiveSpell.Tag = new CommandButtonTag(btnLevel3OffensiveSpell, "cast {realm3spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnLookAtMob.Tag = new CommandButtonTag(btnLookAtMob, "look {mob}", CommandType.None, DependentObjectType.Mob);
            btnCastVigor.Tag = new CommandButtonTag(btnCastVigor, null, CommandType.Magic, DependentObjectType.None);
            btnCastCurePoison.Tag = new CommandButtonTag(btnCastCurePoison, null, CommandType.Magic, DependentObjectType.None);
            btnAttackMob.Tag = new CommandButtonTag(btnAttackMob, "kill {mob}", CommandType.Melee, DependentObjectType.Mob);
            btnDrinkYellow.Tag = new CommandButtonTag(btnDrinkYellow, "drink yellow", CommandType.Potions, DependentObjectType.None);
            btnDrinkGreen.Tag = new CommandButtonTag(btnDrinkGreen, "drink green", CommandType.Potions, DependentObjectType.None);
            btnWieldWeapon.Tag = new CommandButtonTag(btnWieldWeapon, "wield {weapon}", CommandType.None, DependentObjectType.Weapon);
            btnUseWandOnMob.Tag = new CommandButtonTag(btnUseWandOnMob, "zap {wand} {mob}", CommandType.Magic, DependentObjectType.Wand | DependentObjectType.Mob);
            btnPowerAttackMob.Tag = new CommandButtonTag(btnPowerAttackMob, "power {mob}", CommandType.Melee, DependentObjectType.Mob);
            btnRemoveWeapon.Tag = new CommandButtonTag(btnRemoveWeapon, "remove {weapon}", CommandType.None, DependentObjectType.Weapon);
            btnRemoveAll.Tag = new CommandButtonTag(btnRemoveAll, "remove all", CommandType.None, DependentObjectType.None);
            btnCastMend.Tag = new CommandButtonTag(btnCastMend, null, CommandType.Magic, DependentObjectType.None);
            btnReddishOrange.Tag = new CommandButtonTag(btnReddishOrange, "drink reddish-orange", CommandType.Potions, DependentObjectType.None);
            btnStunMob.Tag = new CommandButtonTag(btnStunMob, "cast stun {mob}", CommandType.Magic, DependentObjectType.Mob);
            tsbTime.Tag = new CommandButtonTag(tsbTime, "time", CommandType.None, DependentObjectType.None);
            tsbInformation.Tag = new CommandButtonTag(tsbInformation, "information", CommandType.None, DependentObjectType.None);
            tsbInventory.Tag = new CommandButtonTag(tsbInventory, "inventory", CommandType.None, DependentObjectType.None);
            tsbWho.Tag = new CommandButtonTag(tsbWho, "who", CommandType.None, DependentObjectType.None);
            tsbUptime.Tag = new CommandButtonTag(tsbUptime, "uptime", CommandType.None, DependentObjectType.None);
            tsbEquipment.Tag = new CommandButtonTag(tsbEquipment, "equipment", CommandType.None, DependentObjectType.None);
            tsbSpells.Tag = new CommandButtonTag(tsbSpells, "spells", CommandType.None, DependentObjectType.None);
        }

        private void btnOneClick_Click(object sender, EventArgs e)
        {
            RunStrategy((Strategy)((Button)sender).Tag, null);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_finishedQuit)
            {
                this.Close();
            }
            else
            {
                BackgroundWorkerParameters bwp = _currentBackgroundParameters;
                _commandResult = null;
                _lastCommand = null;
                _lastCommandDamage = 0;
                _lastCommandTrapType = TrapType.None;
                _lastCommandMovementResult = null;
                _backgroundProcessPhase = BackgroundProcessPhase.None;
                bool setMobToFirstAvailable = true;
                if (bwp.AtDestination && !string.IsNullOrEmpty(bwp.TargetRoomMob))
                {
                    if (bwp.MonsterKilled)
                    {
                        if (bwp.MonsterKilledType.HasValue)
                        {
                            MobTypeEnum eMobValue = bwp.MonsterKilledType.Value;
                            setMobToFirstAvailable = true;

                            //choose first monster of the same type
                            foreach (TreeNode nextNode in _tnObviousMobs.Nodes)
                            {
                                if (eMobValue == (MobTypeEnum)nextNode.Tag)
                                {
                                    txtMob.Text = GetTextForMob(_tnObviousMobs, nextNode);
                                    setMobToFirstAvailable = false;
                                }
                            }
                        }
                    }
                    else //presumably the mob is still there so leave it selected
                    {
                        txtMob.Text = bwp.TargetRoomMob;
                    }
                }
                if (setMobToFirstAvailable)
                {
                    string sText = null;
                    lock (_roomChangeLock)
                    {
                        if (_currentRoomMobs.Count > 0)
                        {
                            sText = MobEntity.PickMobTextWithinList(_currentRoomMobs[0], MobEntity.IterateThroughMobs(_currentRoomMobs, 1));
                        }
                    }
                    if (!string.IsNullOrEmpty(sText))
                    {
                        txtMob.Text = sText;
                    }
                }

                if (setMobToFirstAvailable && _tnObviousMobs.Nodes.Count > 0)
                {
                    treeCurrentRoom.SelectedNode = _tnObviousMobs.Nodes[0];
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
                    if (pms.SingleCommandType.Value == BackgroundCommandType.Look)
                    {
                        RunSingleCommandForCommandResult(pms.SingleCommandType.Value, "look", pms, null);
                    }
                    return;
                }

                Strategy strategy = pms.Strategy;
                bool backgroundCommandSuccess;
                CommandResult backgroundCommandResult;

                bool healHP = pms.HealHitpoints;
                bool ensureBlessed = pms.EnsureBlessed;
                bool ensureProtected = pms.EnsureProtected;
                bool cureIfPoisoned = pms.CureIfPoisoned;
                bool healPoison = false;
                if (cureIfPoisoned)
                {
                    _poisoned = false;
                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null);
                    if (backgroundCommandResult != CommandResult.CommandSuccessful)
                    {
                        return;
                    }
                    if (_poisoned)
                    {
                        healPoison = true;
                    }
                    else if (!healHP && !ensureBlessed && !ensureProtected) //display a message if a standalone curepoison was attempted
                    {
                        AddConsoleMessage("Not poisoned, thus cure-poison not cast.");
                    }
                }
                if (healHP || healPoison || ensureBlessed || ensureProtected)
                {
                    _backgroundProcessPhase = BackgroundProcessPhase.Heal;
                    if (!DoBackgroundHeal(healHP, ensureBlessed, ensureProtected, healPoison, pms))
                    {
                        return;
                    }
                }

                //Activate skills
                if ((pms.UsedSkills & PromptedSkills.Manashield) == PromptedSkills.Manashield)
                {
                    _backgroundProcessPhase = BackgroundProcessPhase.ActivateSkills;
                    backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Manashield, "manashield", pms, AbortIfFleeingOrHazying);
                    if (!backgroundCommandSuccess) return;
                }

                Exit previousExit = null;
                if (pms.Exits != null && pms.Exits.Count > 0)
                {
                    _backgroundProcessPhase = BackgroundProcessPhase.Movement;
                    List<Exit> exitList = new List<Exit>(pms.Exits);
                    Room oTarget = exitList[exitList.Count - 1].Target;
                    bool needHeal = false;
                    bool needCurepoison = false;
                    while (exitList.Count > 0)
                    {
                        Exit nextExit = exitList[0];
                        if (previousExit != null && previousExit == nextExit)
                        {
                            AddConsoleMessage("Movement recalculation produced the same path.");
                            return;
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
                                        lock (_currentRoomChanges)
                                        {
                                            _currentObviousExits = null;
                                        }
                                        backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, AbortIfFleeingOrHazying);
                                        if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                        {
                                            List<string> currentObviousExits;
                                            lock (_currentRoomChanges)
                                            {
                                                currentObviousExits = _currentObviousExits;
                                            }
                                            if (currentObviousExits.Contains(exitText))
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
                                            return;
                                        }
                                    }
                                    else if (presenceType == ExitPresenceType.RequiresSearch)
                                    {
                                        _foundSearchedExits = null;
                                        backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Search, "search", pms, AbortIfFleeingOrHazying);
                                        if (backgroundCommandSuccess)
                                        {
                                            if (_foundSearchedExits.Contains(exitText))
                                            {
                                                foundExit = true;
                                            }
                                        }
                                        else
                                        {
                                            return;
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
                                    if (_fleeing) break;
                                    if (_hazying) break;
                                    if (_bw.CancellationPending) break;
                                    Thread.Sleep(50);
                                    RunQueuedCommandWhenBackgroundProcessRunning(pms);
                                }
                            }

                            if (_fleeing) break;
                            if (_hazying) break;
                            if (_bw.CancellationPending) break;

                            if (nextExit.KeyType != KeyType.None)
                            {
                                if (!nextExit.RequiresKey())
                                {
                                    backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Knock, "knock " + exitText, pms, AbortIfFleeingOrHazying);
                                    if (!backgroundCommandSuccess) return;
                                }
                            }

                            if (nextExit.IsTrapExit || (nextExitTarget != null && nextExitTarget.IsTrapRoom))
                            {
                                backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Prepare, "prepare", pms, AbortIfFleeingOrHazying);
                                if (!backgroundCommandSuccess) return;
                            }
                            if (!PreOpenDoorExit(nextExit, pms))
                            {
                                return;
                            }
                            string nextCommand = GetExitCommand(nextExit);
                            bool targetIsDamageRoom = nextExitTarget != null && nextExitTarget.DamageType.HasValue;

                            bool keepTryingMovement = true;
                            while (keepTryingMovement)
                            {
                                backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, AbortIfFleeingOrHazying);
                                if (backgroundCommandSuccess) //successfully traversed the exit to the new room
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
                                    List<Exit> newRoute = CalculateRouteExits(nextExit.Source, oTarget);
                                    if (newRoute != null && newRoute.Count > 0)
                                    {
                                        exitList.Clear();
                                        exitList.AddRange(newRoute);
                                    }
                                    else //couldn't recalculate a new route
                                    {
                                        return;
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
                                    bool openSuccess = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitText, pms, AbortIfFleeingOrHazying);
                                    if (openSuccess)
                                    {
                                        keepTryingMovement = true;
                                    }
                                    else //couldn't open the door
                                    {
                                        return;
                                    }
                                }
                                else if (_lastCommandMovementResult == MovementResult.FallFailure)
                                {
                                    if (!DoBackgroundHeal(true, false, false, needCurepoison, pms)) return;
                                    SendCommand("stand", InputEchoType.On);
                                    keepTryingMovement = true;
                                }
                                else //total failure, abort the background process
                                {
                                    keepTryingMovement = false;
                                    return;
                                }
                            }
                            if (!targetIsDamageRoom && (needHeal || needCurepoison))
                            {
                                if (!DoBackgroundHeal(needHeal, false, false, needCurepoison, pms)) return;
                                needHeal = false;
                                needCurepoison = false;
                            }
                        }
                        finally
                        {
                            _currentBackgroundExit = null;
                            _foundSearchedExits = null;
                        }
                    }
                    pms.AtDestination = true; //all exits traversed successfully
                }
                else
                {
                    pms.AtDestination = true;
                }

               if (!string.IsNullOrEmpty(pms.TargetRoomMob) && pms.AtDestination)
               {
                    _mob = pms.TargetRoomMob;
                }

                bool useManaPool = pms.ManaPool > 0;
                bool stopIfMonsterKilled = false;
                bool hasInitialQueuedMagicStep;
                bool hasInitialQueuedMeleeStep;
                lock (_queuedCommandLock)
                {
                    hasInitialQueuedMagicStep = pms.QueuedMagicStep.HasValue;
                    hasInitialQueuedMeleeStep = pms.QueuedMeleeStep.HasValue;
                }
                if (_hazying || _fleeing || strategy != null || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep)
                {
                    try
                    {
                        _currentlyFightingMob = pms.TargetRoomMob;

                        StrategyInstance stratCurrent = null;
                        bool haveMeleeStrategySteps = false;
                        bool haveMagicStrategySteps = false;
                        if (strategy != null)
                        {
                            haveMagicStrategySteps = strategy.HasAnyMagicSteps();
                            haveMeleeStrategySteps = strategy.HasAnyMeleeSteps();
                            stopIfMonsterKilled = strategy.StopWhenKillMonster;
                        }
                        List<string> offensiveSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_currentRealm);
                        List<string> knownSpells;
                        lock (_spellsKnownLock)
                        {
                            knownSpells = _spellsKnown;
                        }
                        int? calculatedMinLevel, calculatedMaxLevel;
                        Strategy.GetMinMaxOffensiveSpellLevels(strategy, _autoSpellLevelMin, _autoSpellLevelMax, knownSpells, offensiveSpells, out calculatedMinLevel, out calculatedMaxLevel);

                        stratCurrent = new StrategyInstance(strategy, calculatedMinLevel, calculatedMaxLevel, _currentlyFightingMob, offensiveSpells, knownSpells);

                        _monsterDamage = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        if (useManaPool)
                        {
                            _currentMana = pms.ManaPool;
                        }

                        string sWeapon = _weapon;

                        if (haveMagicStrategySteps || haveMeleeStrategySteps || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Combat;
                            bool doPowerAttack = false;
                            if (haveMeleeStrategySteps || hasInitialQueuedMeleeStep)
                            {
                                if (!string.IsNullOrEmpty(sWeapon))
                                {
                                    SendCommand("wield " + sWeapon, InputEchoType.On);
                                }
                                doPowerAttack = (pms.UsedSkills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
                            }
                            IEnumerator<MagicStrategyStep> magicSteps = strategy?.GetMagicSteps().GetEnumerator();
                            IEnumerator<MeleeStrategyStep> meleeSteps = strategy?.GetMeleeSteps(doPowerAttack).GetEnumerator();
                            MagicStrategyStep? nextMagicStep = null;
                            MeleeStrategyStep? nextMeleeStep = null;
                            bool magicStepsFinished = magicSteps == null || !magicSteps.MoveNext();
                            bool meleeStepsFinished = meleeSteps == null || !meleeSteps.MoveNext();
                            bool magicOnlyWhenStunned = strategy != null && ((strategy.TypesToRunOnlyWhenMonsterStunned & CommandType.Magic) != CommandType.None);
                            bool meleeOnlyWhenStunned = strategy != null && ((strategy.TypesToRunOnlyWhenMonsterStunned & CommandType.Melee) != CommandType.None);
                            if (!magicStepsFinished) nextMagicStep = magicSteps.Current;
                            if (!meleeStepsFinished) nextMeleeStep = meleeSteps.Current;
                            DateTime? dtNextMagicCommand = null;
                            DateTime? dtNextMeleeCommand = null;
                            while (true) //combat cycle
                            {
                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);

                                bool didDamage = false;
                                string command;
                                if (nextMagicStep.HasValue && 
                                    (_monsterStunned || !magicOnlyWhenStunned) && 
                                    (!dtNextMagicCommand.HasValue || DateTime.UtcNow > dtNextMagicCommand.Value))
                                {
                                    int currentMana = useManaPool ? _currentMana : _automp;
                                    int currentHP = _autohp;
                                    int manaDrain;
                                    BackgroundCommandType? bct;
                                    MagicCommandChoiceResult result = stratCurrent.GetMagicCommand(nextMagicStep.Value, currentHP, _totalhp, currentMana, out manaDrain, out bct, out command);
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
                                        if (!RunBackgroundMagicStep(bct.Value, command, pms, useManaPool, manaDrain, magicSteps, ref magicStepsFinished, ref nextMagicStep, ref dtNextMagicCommand, ref didDamage)) 
                                            return;
                                    }
                                }

                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);

                                //flee or stop combat once steps complete
                                if (!nextMagicStep.HasValue && strategy != null)
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

                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);

                                if (nextMeleeStep.HasValue &&
                                    (_monsterStunned || !meleeOnlyWhenStunned) && 
                                    (!dtNextMeleeCommand.HasValue || DateTime.UtcNow > dtNextMeleeCommand.Value))
                                {
                                    stratCurrent.GetMeleeCommand(nextMeleeStep.Value, out command);

                                    if (_fumbled && !string.IsNullOrEmpty(_weapon))
                                    {
                                        SendCommand("wield " + _weapon, InputEchoType.On);
                                        _fumbled = false;
                                    }

                                    if (!RunBackgroundMeleeStep(BackgroundCommandType.Attack, command, pms, meleeSteps, ref meleeStepsFinished, ref nextMeleeStep, ref dtNextMeleeCommand, ref didDamage))
                                        return;
                                }

                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);

                                //flee or stop combat once steps complete
                                if (!nextMeleeStep.HasValue && strategy != null)
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

                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;

                                if (didDamage && _queryMonsterStatus)
                                {
                                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + _currentlyFightingMob, pms, AbortIfFleeingOrHazying);
                                    if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
                                    {
                                        return;
                                    }
                                    else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                                    {
                                        magicStepsFinished = true;
                                        meleeStepsFinished = true;
                                        nextMagicStep = null;
                                        nextMeleeStep = null;
                                        _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                                        _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                                    }
                                    else if (backgroundCommandResult != CommandResult.CommandSuccessful)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                }

                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);

                                //stop combat if all combat types are finished
                                if (!nextMagicStep.HasValue && !nextMeleeStep.HasValue) break;

                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;
                                RunQueuedCommandWhenBackgroundProcessRunning(pms);
                                if (BreakOutOfBackgroundCombat(stopIfMonsterKilled)) break;

                                Thread.Sleep(50);
                            }
                        }

                        //perform flee logic
                        if (_fleeing && (!stopIfMonsterKilled || !_monsterKilled) && !_hazying)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Flee;
                            if (!string.IsNullOrEmpty(sWeapon))
                            {
                                SendCommand("remove " + sWeapon, InputEchoType.On);
                                if (!_fleeing) goto BeforeHazy;
                                if (_hazying) goto BeforeHazy;
                                if (_bw.CancellationPending) return;
                            }

                            //determine the flee exit if there is only one place to flee to
                            Room r = m_oCurrentRoom;

                            //run the preexit logic for all target exits, since it won't be known beforehand
                            //which exit will be used.
                            List<Exit> availableExits = new List<Exit>();
                            foreach (Exit nextExit in IsengardMap.GetRoomExits(r, FleeExitDiscriminator))
                            {
                                if (PreOpenDoorExit(nextExit, pms))
                                {
                                    availableExits.Add(nextExit);
                                }
                            }
                            Exit singleFleeableExit = null;
                            if (availableExits.Count == 1) singleFleeableExit = availableExits[0];

                            backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Flee, "flee", pms, AbortIfHazying);
                            if (backgroundCommandSuccess)
                            {
                                pms.Fled = true;
                            }
                            else
                            {
                                if (!_hazying)
                                {
                                    return;
                                }
                            }
                        }

BeforeHazy:

                        //perform hazy logic
                        if (_hazying && (!stopIfMonsterKilled || !_monsterKilled))
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Hazy;

                            backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.DrinkHazy, "drink hazy", pms, null);
                            if (backgroundCommandSuccess)
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
                        _currentlyFightingMob = null;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        _currentMana = HP_OR_MP_UNKNOWN;
                    }
                }

                if (!pms.Fled && !pms.Hazied)
                {
                    bool runScore = pms.DoScore;
                    if (!runScore)
                    {
                        lock (_skillsLock)
                        {
                            foreach (SkillCooldown next in _cooldowns)
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
                        backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null);
                        if (backgroundCommandResult != CommandResult.CommandSuccessful)
                        {
                            return;
                        }
                        pms.DoScore = false;
                    }
                }

                if (pms.Quit)
                {
                    _backgroundProcessPhase = BackgroundProcessPhase.Quit;
                    RunSingleCommand(BackgroundCommandType.Quit, "quit", pms, null);
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

        private bool FleeExitDiscriminator(Exit exit)
        {
            return !exit.Hidden && !exit.NoFlee;
        }

        private bool BreakOutOfBackgroundCombat(bool stopIfMonsterKilled)
        {
            bool ret;
            if (stopIfMonsterKilled && _monsterKilled)
                ret = true;
            else if (_fleeing)
                ret = true;
            else if (_hazying)
                ret = true;
            else if (_bw.CancellationPending)
                ret = true;
            else
            {
                Room r = m_oCurrentRoom;
                if (r != null && r.Intangible)
                    ret = true;
                else
                    ret = false;
            }
            return ret;
        }

        private bool DoBackgroundHeal(bool healHP, bool doBless, bool doProtection, bool doCurePoison, BackgroundWorkerParameters pms)
        {
            int autohp;
            int automp = _automp;
            if (doCurePoison && (automp < 4 || !CastLifeSpell("cure-poison", pms)))
            {
                return false;
            }
            if (healHP)
            {
                while (true)
                {
                    autohp = _autohp;
                    automp = _automp;
                    if (autohp >= _totalhp) break;
                    if (automp < 2) break; //out of mana for vigor cast
                    if (!CastLifeSpell("vigor", pms))
                    {
                        return false;
                    }
                    if (_fleeing) break;
                    if (_hazying) break;
                    if (_bw.CancellationPending) break;
                    autohp = _autohp;
                    if (autohp >= _totalhp) break;
                }
                //stop background processing if failed to get to max hitpoints
                autohp = _autohp;
                if (autohp < _totalhp) return false;
            }

            //cast bless if has enough mana and not currently blessed
            if (doBless)
            {
                bool hasBless;
                lock (_spellsCastLock)
                {
                    hasBless = _spellsCast != null && _spellsCast.Contains("bless");
                }
                automp = _automp;
                if (automp >= 8 && !hasBless)
                {
                    if (!CastLifeSpell("bless", pms))
                    {
                        return false;
                    }
                }
            }

            //cast protection if has enough mana and not curently protected
            if (doProtection)
            {
                bool hasProtection;
                lock (_spellsCastLock)
                {
                    hasProtection = _spellsCast != null && _spellsCast.Contains("protection");
                }
                automp = _automp;
                if (automp >= 8 && !hasProtection)
                {
                    if (!CastLifeSpell("protection", pms))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool RunBackgroundMeleeStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<MeleeStrategyStep> meleeSteps, ref bool meleeStepsFinished, ref MeleeStrategyStep? nextMeleeStep, ref DateTime? dtNextMeleeCommand, ref bool didDamage)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
            {
                return false;
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
            return true;
        }

        private bool RunBackgroundMagicStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, bool useManaPool, int manaDrain, IEnumerator<MagicStrategyStep> magicSteps, ref bool magicStepsFinished, ref MagicStrategyStep? nextMagicStep, ref DateTime? dtNextMagicCommand, ref bool didDamage)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
            {
                return false;
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
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel4)
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
            return true;
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

        private string GetExitCommand(Exit exit)
        {
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
            string sWord = MobEntity.PickWord(target);
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
                        ret = "go " + sWord + " " + iCounter;
                    }
                }
            }
            if (ret == null)
            {
                ret = "go " + sWord;
            }
            return ret;
        }

        private bool PreOpenDoorExit(Exit exit, BackgroundWorkerParameters pms)
        {
            bool ret;
            if (exit.MustOpen)
            {
                ret = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exit.ExitText, pms, AbortIfFleeingOrHazying);
            }
            else //not a door exit
            {
                ret = true;
            }
            return ret;
        }

        private bool CastLifeSpell(string spellName, BackgroundWorkerParameters bwp)
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
            return RunSingleCommand(bct, "cast " + spellName, bwp, AbortIfFleeingOrHazying);
        }

        private void WaitUntilNextCommandTry(int remainingMS, BackgroundCommandType commandType)
        {
            bool quitting = commandType == BackgroundCommandType.Quit;
            bool hazying = commandType == BackgroundCommandType.DrinkHazy;
            bool fleeing = commandType == BackgroundCommandType.Flee;
            while (remainingMS > 0)
            {
                int nextWaitMS = Math.Min(remainingMS, 100);

                //check if the wait should be aborted
                if (!quitting)
                {
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
                }
                if (_bw.CancellationPending) break;

                Thread.Sleep(nextWaitMS);

                //check if the wait should be aborted
                if (!quitting)
                {
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
                }
                if (_bw.CancellationPending) break;

                remainingMS -= nextWaitMS;
                RunQueuedCommandWhenBackgroundProcessRunning(_currentBackgroundParameters);

                //check if the wait should be aborted
                if (!quitting)
                {
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
                }
                if (_bw.CancellationPending) break;
            }
        }

        private void RunQueuedCommandWhenBackgroundProcessRunning(BackgroundWorkerParameters pms)
        {
            string sQueuedCommand;
            lock (_queuedCommandLock)
            {
                sQueuedCommand = pms.QueuedCommand;
                if (sQueuedCommand != null)
                {
                    pms.QueuedCommand = null;
                }
            }
            if (!string.IsNullOrEmpty(sQueuedCommand))
            {
                SendCommand(sQueuedCommand, InputEchoType.On);
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

        private CommandResult RunSingleCommandForCommandResult(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            _backgroundCommandType = commandType;
            try
            {
                return RunSingleCommandForCommandResult(command, pms, abortLogic);
            }
            finally
            {
                _backgroundCommandType = null;
            }
        }

        private InputEchoType GetHiddenMessageEchoType()
        {
            return _verboseMode ? InputEchoType.On : InputEchoType.Off;
        }

        private CommandResult RunSingleCommandForCommandResult(string command, BackgroundWorkerParameters pms, Func<bool> abortLogic)
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
                SendCommand(command, GetHiddenMessageEchoType());
                CommandResult? currentResult = null;
                while (!currentResult.HasValue)
                {
                    RunQueuedCommandWhenBackgroundProcessRunning(pms);
                    if (_bw.CancellationPending || (abortLogic != null && abortLogic()))
                    {
                        currentResult = CommandResult.CommandAborted;
                    }
                    else if (DateTime.UtcNow >= utcTimeoutPoint)
                    {
                        AddConsoleMessage("Command timeout occurred for " + command);
                        currentResult = CommandResult.CommandTimeout;
                    }
                    else
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

        private bool RunSingleCommand(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic)
        {
            int currentAttempts = 0;
            bool commandSucceeded = false;
            _backgroundCommandType = commandType;
            try
            {
                CommandResult? result = null;
                while (currentAttempts < MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND && !commandSucceeded)
                {
                    if (_bw.CancellationPending) break;
                    if (abortLogic != null && abortLogic()) break;
                    result = RunSingleCommandForCommandResult(command, pms, abortLogic);
                    if (result.HasValue)
                    {
                        CommandResult resultValue = result.Value;
                        if (resultValue == CommandResult.CommandSuccessful || resultValue == CommandResult.CommandUnsuccessfulAlways || resultValue == CommandResult.CommandAborted || resultValue == CommandResult.CommandTimeout)
                        {
                            break;
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
                return result.GetValueOrDefault(CommandResult.CommandUnsuccessfulAlways) == CommandResult.CommandSuccessful;
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
            bool quitting = bwp.Quit;
            bool enabled;
            if (running && quitting)
                enabled = false;
            else
                enabled = !running;
            foreach (Control ctl in GetControlsToDisableForBackgroundProcess())
            {
                ctl.Enabled = enabled;
            }
            btnAbort.Enabled = running;
            EnableDisableActionButtons(bwp);
        }

        private IEnumerable<Control> GetControlsToDisableForBackgroundProcess()
        {
            yield return txtMob;
            yield return txtWeapon;
            yield return txtWand;
            yield return txtPotion;
            yield return cboTickRoom;
            yield return btnGoToHealingRoom;
            foreach (Button btn in flpOneClickStrategies.Controls)
            {
                yield return btn;
            }
            yield return btnGraph;
            yield return btnLocations;
            yield return btnClearCurrentLocation;
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
            yield return btnHeal;
            yield return btnSkills;
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
            string sMob = _currentlyFightingMob;
            if (string.IsNullOrEmpty(sMob))
            {
                sMob = _mob;
            }
            List<string> knownSpells;
            List<string> realmSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_currentRealm);
            lock (_spellsKnownLock)
            {
                knownSpells = _spellsKnown;
            }
            foreach (CommandButtonTag oTag in GetButtonsForEnablingDisabling())
            {
                object oControl = oTag.Control;
                if ((oTag.ObjectType & DependentObjectType.Mob) != DependentObjectType.None && string.IsNullOrEmpty(sMob))
                    enabled = false;
                else if ((oTag.ObjectType & DependentObjectType.Weapon) != DependentObjectType.None && string.IsNullOrEmpty(_weapon))
                    enabled = false;
                else if ((oTag.ObjectType & DependentObjectType.Wand) != DependentObjectType.None && string.IsNullOrEmpty(_wand))
                    enabled = false;
                else if (npp == BackgroundProcessPhase.Quit)
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
                        sSpell = realmSpells[0];
                    else if (oControl == btnLevel2OffensiveSpell)
                        sSpell = realmSpells[1];
                    else if (oControl == btnLevel3OffensiveSpell)
                        sSpell = realmSpells[2];
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
            else if (npp == BackgroundProcessPhase.Flee || npp == BackgroundProcessPhase.Hazy || npp == BackgroundProcessPhase.Score || npp == BackgroundProcessPhase.Quit)
                enabled = false;
            else
                enabled = true;
            if (enabled != btnFlee.Enabled)
                btnFlee.Enabled = enabled;

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Hazy || npp == BackgroundProcessPhase.Score || npp == BackgroundProcessPhase.Quit)
                enabled = false;
            else
                enabled = true;
            if (enabled != btnDrinkHazy.Enabled)
                btnDrinkHazy.Enabled = enabled;

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Score || npp == BackgroundProcessPhase.Quit)
                enabled = false;
            else
                enabled = true;
            if (enabled != tsbScore.Enabled)
                tsbScore.Enabled = enabled;

            if (inForeground)
                enabled = true;
            else if (npp == BackgroundProcessPhase.Quit)
                enabled = false;
            else
                enabled = true;
            if (enabled != tsbQuit.Enabled)
                tsbQuit.Enabled = enabled;
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
            yield return (CommandButtonTag)btnDrinkYellow.Tag;
            yield return (CommandButtonTag)btnDrinkGreen.Tag;
            yield return (CommandButtonTag)btnWieldWeapon.Tag;
            yield return (CommandButtonTag)btnUseWandOnMob.Tag;
            yield return (CommandButtonTag)btnPowerAttackMob.Tag;
            yield return (CommandButtonTag)btnRemoveWeapon.Tag;
            yield return (CommandButtonTag)btnRemoveAll.Tag;
            yield return (CommandButtonTag)btnCastMend.Tag;
            yield return (CommandButtonTag)btnReddishOrange.Tag;
            yield return (CommandButtonTag)btnStunMob.Tag;
            yield return (CommandButtonTag)tsbTime.Tag;
            yield return (CommandButtonTag)tsbInformation.Tag;
            yield return (CommandButtonTag)tsbInventory.Tag;
            yield return (CommandButtonTag)tsbWho.Tag;
            yield return (CommandButtonTag)tsbUptime.Tag;
            yield return (CommandButtonTag)tsbEquipment.Tag;
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
            Room currentRoom = m_oCurrentRoom;
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
            NavigateExitsInBackground(new List<Exit>() { navigateExit });
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

        private void btnAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeStrategyStep.RegularAttack);
        }

        public void btnPowerAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeStrategyStep.PowerAttack);
        }

        private void RunOrQueueMagicStep(object sender, MagicStrategyStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                Strategy s = new Strategy();
                s.LastMagicStep = step;
                bwp.TargetRoomMob = txtMob.Text;
                bwp.Strategy = s;
                RunBackgroundProcess(bwp);
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
                bwp = new BackgroundWorkerParameters();
                Strategy s = new Strategy();
                s.LastMeleeStep = step;
                bwp.Strategy = s;
                bwp.TargetRoomMob = txtMob.Text;
                RunBackgroundProcess(bwp);
            }
            else
            {
                lock (_queuedCommandLock)
                {
                    bwp.QueuedMeleeStep = step;
                }
            }
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
            RunCommand(TranslateCommand(command));
        }

        private string TranslateCommand(string command)
        {
            string sMob = _currentlyFightingMob;
            if (string.IsNullOrEmpty(sMob))
            {
                sMob = _mob;
            }
            if (command.Contains("{realm1spell}") || command.Contains("{realm2spell}") || command.Contains("{realm3spell}") || command.Contains("{realm4spell}"))
            {
                List<string> offensiveSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_currentRealm);
                command = command.Replace("{realm1spell}", offensiveSpells[0])
                                 .Replace("{realm2spell}", offensiveSpells[1])
                                 .Replace("{realm3spell}", offensiveSpells[2])
                                 .Replace("{realm4spell}", offensiveSpells[3]);
            }
            return command.Replace("{mob}", sMob)
                          .Replace("{weapon}", _weapon)
                          .Replace("{wand}", _wand);
        }

        private void RunCommand(string command)
        {
            if (_currentBackgroundParameters != null) //queue to background process
            {
                lock (_queuedCommandLock)
                {
                    _currentBackgroundParameters.QueuedCommand = command;
                }
            }
            else
            {
                SendCommand(command, InputEchoType.On);
            }
        }

        private void btnClearCurrentLocation_Click(object sender, EventArgs e)
        {
            DoClearCurrentLocation();
        }

        private void DoClearCurrentLocation()
        {
            m_oCurrentRoom = null;
            RefreshEnabledForSingleMoveButtons();
        }

        private void RunBackgroundProcess(BackgroundWorkerParameters backgroundParameters)
        {
            _currentBackgroundParameters = backgroundParameters;
            _backgroundProcessPhase = BackgroundProcessPhase.Initialization;
            ToggleBackgroundProcessUI(backgroundParameters, true);
            _bw.RunWorkerAsync(backgroundParameters);
        }

        private class BackgroundWorkerParameters
        {
            public List<Exit> Exits { get; set; }
            public bool Cancelled { get; set; }
            public Strategy Strategy { get; set; }
            public string QueuedCommand { get; set; }
            public MagicStrategyStep? QueuedMagicStep { get; set; }
            public MeleeStrategyStep? QueuedMeleeStep { get; set; }
            public int ManaPool { get; set; }
            public PromptedSkills UsedSkills { get; set; }
            public bool Hazy { get; set; }
            public bool Hazied { get; set; }
            public bool Flee { get; set; }
            public bool Fled { get; set; }
            public bool Quit { get; set; }
            public bool DoScore { get; set; }
            public string TargetRoomMob { get; set; }
            public bool Foreground { get; set; }
            public bool HealHitpoints { get; set; }
            public bool CureIfPoisoned { get; set; }
            public bool EnsureBlessed { get; set; }
            public bool EnsureProtected { get; set; }
            public BackgroundCommandType? SingleCommandType { get; set; }
            public bool MonsterKilled { get; set; }
            public MobTypeEnum? MonsterKilledType { get; set; }
            public bool AtDestination { get; set; }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _currentBackgroundParameters.Cancelled = true;
            _bw.CancelAsync();
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

        private void RunStrategy(Strategy strategy, List<Exit> preExits)
        {
            CommandType eStrategyCombatCommandType = strategy.CombatCommandTypes;
            bool isMagicStrategy = (eStrategyCombatCommandType & CommandType.Magic) == CommandType.Magic;
            bool isMeleeStrategy = (eStrategyCombatCommandType & CommandType.Melee) == CommandType.Melee;
            bool isCombatStrategy = isMagicStrategy || isMeleeStrategy;
            bool hasWeapon = !string.IsNullOrEmpty(txtWeapon.Text);
            if (isMeleeStrategy && !hasWeapon)
            {
                MessageBox.Show("No weapon specified.");
                return;
            }

            PromptedSkills activatedSkills;
            string targetRoomMob;
            if (!PromptForSkills(false, isMeleeStrategy, isCombatStrategy, preExits, out activatedSkills, out targetRoomMob))
            {
                return;
            }

            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.Strategy = strategy;
            if (strategy.ManaPool > 0) bwp.ManaPool = strategy.ManaPool;
            bwp.Exits = preExits;
            bwp.UsedSkills = activatedSkills;
            bwp.TargetRoomMob = targetRoomMob;
            RunBackgroundProcess(bwp);
        }

        private bool PromptForSkills(bool staticSkillsOnly, bool forMeleeCombat, bool isCombatStrategy, List<Exit> preExits, out PromptedSkills activatedSkills, out string mob)
        {
            activatedSkills = PromptedSkills.None;
            mob = string.Empty;

            PromptedSkills skills = PromptedSkills.None;
            DateTime utcNow = DateTime.UtcNow;

            lock (_skillsLock)
            {
                foreach (SkillCooldown nextCooldown in _cooldowns)
                {
                    SkillWithCooldownType sct = nextCooldown.SkillType;
                    SkillCooldownStatus status = nextCooldown.Status;
                    bool isAvailable = status == SkillCooldownStatus.Available || (status == SkillCooldownStatus.Waiting && utcNow >= nextCooldown.NextAvailable);
                    if (isAvailable)
                    {
                        if (sct == SkillWithCooldownType.PowerAttack)
                        {
                            if (!staticSkillsOnly && forMeleeCombat)
                            {
                                skills |= PromptedSkills.PowerAttack;
                            }
                        }
                        else if (sct == SkillWithCooldownType.Manashield)
                        {
                            skills |= PromptedSkills.Manashield;
                        }
                    }
                }
            }

            if (staticSkillsOnly && skills == PromptedSkills.None)
            {
                MessageBox.Show(this, "No skills available.");
                return false;
            }

            Room targetRoom;
            if (preExits == null)
            {
                targetRoom = m_oCurrentRoom;
            }
            else
            {
                targetRoom = preExits[preExits.Count - 1].Target;
            }

            using (frmPreBackgroundProcessPrompt frmSkills = new frmPreBackgroundProcessPrompt(skills, targetRoom, txtMob.Text, isCombatStrategy))
            {
                if (frmSkills.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }
                activatedSkills = frmSkills.SelectedSkills;
                mob = frmSkills.Mob;
            }
            return true;
        }

        public class CommandButtonTag
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
            if (_finishedQuit)
            {
                this.Close();
                return;
            }

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

            if ((initStep & InitializationStep.Score) != InitializationStep.None)
            {
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
                        ComputeColor(iCurrentMana, iTotalMP, out byte r, out byte g, out byte b);
                        _mpColorR = r;
                        _mpColorG = g;
                        _mpColorB = b;
                    }
                    lblManaValue.Text = sText;
                }
                if (autohpforthistick != HP_OR_MP_UNKNOWN)
                {
                    lblHitpointsValue.Text = autohpforthistick.ToString() + "/" + _totalhp;
                    ComputeColor(autohpforthistick, _totalhp, out byte r, out byte g, out byte b);
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
                lock (_skillsLock)
                {
                    cooldowns.AddRange(_cooldowns);
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
                        backColor = _fullColor;
                    }
                    else if (status == SkillCooldownStatus.Available)
                    {
                        sText = "0:00";
                        backColor = _fullColor;
                    }
                    else if (status == SkillCooldownStatus.Inactive)
                    {
                        sText = "?";
                        backColor = _emptyColor;
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
                        backColor = sText == "0:00" ? _fullColor : _emptyColor;
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

            string sMonster = _currentlyFightingMob;
            int iMonsterDamage = _monsterDamage;
            MonsterStatus monsterStatus = _currentMonsterStatus;

            if (!string.Equals(sMonster, _currentlyFightingMobUI))
            {
                grpMob.Text = sMonster ?? "Mob";
                _currentlyFightingMobUI = sMonster;
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

            int iTNL = _tnl;
            if (iTNL != _tnlUI)
            {
                lblToNextLevelValue.Text = iTNL.ToString();
                _tnlUI = iTNL;
            }

            RefreshAutoEscapeUI(false);

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

            lock (_roomChangeLock)
            {
                List<RoomChange> changes = null;
                int iNewCounter = -1;
                Room oCurrentRoom = m_oCurrentRoom;
                if (oCurrentRoom != m_oCurrentRoomUI)
                {
                    if (_setTickRoom)
                    {
                        HealingRoom? healFlag = oCurrentRoom.HealingRoom;
                        if (healFlag.HasValue)
                        {
                            cboTickRoom.SelectedItem = healFlag.Value;
                        }
                        _setTickRoom = false;
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
                    m_oCurrentRoomUI = oCurrentRoom;
                    if (_currentBackgroundParameters == null)
                    {
                        btnGoToHealingRoom.Enabled = oCurrentRoom != null;
                        RefreshEnabledForSingleMoveButtons();
                    }
                }
                if (_roomChangeCounter != _roomChangeCounterUI)
                {
                    for (int i = 0; i < _currentRoomChanges.Count; i++)
                    {
                        RoomChange nextRoomChange = _currentRoomChanges[i];
                        int iCounter = nextRoomChange.GlobalCounter;
                        if (iCounter > _roomChangeCounterUI)
                        {
                            if (changes == null) changes = new List<RoomChange>();
                            changes.Add(nextRoomChange);
                        }
                    }
                }
                if (changes != null)
                {
                    foreach (RoomChange nextRoomChange in changes)
                    {
                        RoomChangeType rcType = nextRoomChange.ChangeType;
                        if (rcType == RoomChangeType.NewRoom)
                        {
                            bool firstTimeThrough = _roomChangeCounterUI == -1;
                            treeCurrentRoom.Nodes.Clear();
                            _tnObviousMobs.Nodes.Clear();
                            _tnObviousExits.Nodes.Clear();
                            _tnOtherExits.Nodes.Clear();
                            _tnPermanentMobs.Nodes.Clear();
                            foreach (MobTypeEnum nextMob in nextRoomChange.Mobs)
                            {
                                _tnObviousMobs.Nodes.Add(GetMobsNode(nextMob));
                            }
                            if (_tnObviousMobs.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(_tnObviousMobs);
                                if (firstTimeThrough || _obviousMobsTNExpanded)
                                {
                                    _tnObviousMobs.Expand();
                                }
                            }
                            foreach (string s in nextRoomChange.Exits)
                            {
                                _tnObviousExits.Nodes.Add(GetObviousExitNode(nextRoomChange, s));
                            }
                            if (_tnObviousExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(_tnObviousExits);
                                if (firstTimeThrough || _obviousExitsTNExpanded)
                                {
                                    _tnObviousExits.Expand();
                                }
                            }
                            foreach (Exit nextExit in nextRoomChange.OtherExits)
                            {
                                _tnOtherExits.Nodes.Add(GetOtherExitsNode(nextExit));
                            }
                            if (_tnOtherExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(_tnOtherExits);
                                if (firstTimeThrough || _otherExitsTNExpanded)
                                {
                                    _tnOtherExits.Expand();
                                }
                            }
                            if (oCurrentRoom?.PermanentMobs != null)
                            {
                                foreach (MobTypeEnum nextPerm in oCurrentRoom.PermanentMobs)
                                {
                                    _tnPermanentMobs.Nodes.Add(GetMobsNode(nextPerm));
                                }
                                if (_tnPermanentMobs.Nodes.Count > 0)
                                {
                                    treeCurrentRoom.Nodes.Add(_tnPermanentMobs);
                                    if (firstTimeThrough || _permMobsTNExpanded)
                                    {
                                        _tnPermanentMobs.Expand();
                                    }
                                }
                            }
                        }
                        else if (rcType == RoomChangeType.AddExit)
                        {
                            string exit = nextRoomChange.Exits[0];
                            bool hasNodes = _tnObviousExits.Nodes.Count > 0;
                            _tnObviousExits.Nodes.Add(GetObviousExitNode(nextRoomChange, exit));
                            TreeNode removeNode = null;
                            foreach (TreeNode tn in _tnOtherExits.Nodes)
                            {
                                if (tn.Tag == nextRoomChange.MappedExits[exit])
                                {
                                    removeNode = tn;
                                    break;
                                }
                            }
                            if (!hasNodes)
                            {
                                InsertTopLevelTreeNode(_tnObviousExits);
                            }
                            _tnOtherExits.Nodes.Remove(removeNode);
                            if (_tnOtherExits.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(_tnOtherExits);
                            }
                        }
                        else if (rcType == RoomChangeType.RemoveExit)
                        {
                            string exit = nextRoomChange.Exits[0];
                            TreeNode removeNode = null;
                            Exit foundExit = null;
                            foreach (TreeNode tn in _tnObviousExits.Nodes)
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
                                _tnObviousExits.Nodes.Remove(removeNode);
                                if (_tnObviousExits.Nodes.Count == 0)
                                {
                                    treeCurrentRoom.Nodes.Remove(_tnObviousExits);
                                }
                                _tnOtherExits.Nodes.Add(GetOtherExitsNode(foundExit));
                            }
                        }
                        else if (rcType == RoomChangeType.AddMob)
                        {
                            bool hasNodes = _tnObviousMobs.Nodes.Count > 0;
                            bool isFirst = true;
                            MobTypeEnum? firstMob = null;
                            TreeNode firstInserted = null;
                            foreach (MobTypeEnum nextMobType in nextRoomChange.Mobs)
                            {
                                int iInsertionPoint = FindNewMobInsertionPoint(nextMobType);
                                TreeNode inserted = GetMobsNode(nextMobType);
                                if (iInsertionPoint == -1)
                                {
                                    _tnObviousMobs.Nodes.Add(inserted);
                                }
                                else
                                {
                                    _tnObviousMobs.Nodes.Insert(iInsertionPoint, inserted);
                                }
                                if (isFirst)
                                {
                                    firstMob = nextMobType;
                                    isFirst = false;
                                    firstInserted = inserted;
                                }
                            }
                            if (!hasNodes)
                            {
                                InsertTopLevelTreeNode(_tnObviousMobs);
                                if (bwp == null)
                                {
                                    txtMob.Text = GetTextForMob(_tnObviousMobs, firstInserted);
                                }
                            }
                        }
                        else if (rcType == RoomChangeType.RemoveMob)
                        {
                            bool somethingRemoved = false;
                            foreach (MobTypeEnum nextMobType in nextRoomChange.Mobs)
                            {
                                int removalIndex = FindMobIndex(nextMobType);
                                if (removalIndex != -1)
                                {
                                    somethingRemoved = true;
                                    _tnObviousMobs.Nodes.RemoveAt(removalIndex);
                                }
                            }
                            if (somethingRemoved)
                            {
                                if (_tnObviousMobs.Nodes.Count == 0)
                                {
                                    treeCurrentRoom.Nodes.Remove(_tnObviousMobs);
                                }
                            }
                        }
                        
                        iNewCounter = nextRoomChange.GlobalCounter;
                    }
                    _roomChangeCounterUI = iNewCounter;
                }
            }

            EnableDisableActionButtons(bwp);
            if (bwp == null) //processing that only happens when a background process is not running
            {
                //check for poll tick if the first status update has completed and not at full HP+MP
                if (_currentStatusLastComputed.HasValue && (autohpforthistick == HP_OR_MP_UNKNOWN || autohpforthistick < _totalhp || autompforthistick == HP_OR_MP_UNKNOWN || autompforthistick < _totalmp))
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

        private int FindMobIndex(MobTypeEnum mobType)
        {
            int i = 0;
            int iFoundIndex = -1;
            foreach (TreeNode tn in _tnObviousMobs.Nodes)
            {
                MobTypeEnum nextMob = (MobTypeEnum)tn.Tag;
                if (nextMob == mobType)
                {
                    iFoundIndex = i;
                }
                i++;
            }
            return iFoundIndex;
        }

        private int FindNewMobInsertionPoint(MobTypeEnum newMob)
        {
            string sSingular = MobEntity.MobToSingularMapping[newMob];
            int i = 0;
            int iFoundIndex = -1;
            foreach (TreeNode tn in _tnObviousMobs.Nodes)
            {
                MobTypeEnum nextMob = (MobTypeEnum)tn.Tag;
                string sNextSingular = MobEntity.MobToSingularMapping[nextMob];
                if (sSingular.CompareTo(sNextSingular) < 0)
                {
                    iFoundIndex = i;
                    break;
                }
                i++;
            }
            return iFoundIndex;
        }

        private void InsertTopLevelTreeNode(TreeNode topLevelTreeNode)
        {
            if (!treeCurrentRoom.Nodes.Contains(topLevelTreeNode))
            {
                int iIndex = GetTopLevelTreeNodeLogicalIndex(topLevelTreeNode);
                bool found = false;
                for (int i = 0; i < treeCurrentRoom.Nodes.Count; i++)
                {
                    TreeNode next = treeCurrentRoom.Nodes[i];
                    int nextIndex = GetTopLevelTreeNodeLogicalIndex(next);
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
                if (GetTopLevelTreeNodeExpanded(topLevelTreeNode))
                {
                    topLevelTreeNode.Expand();
                }
            }
        }

        private bool GetTopLevelTreeNodeExpanded(TreeNode topLevelTreeNode)
        {
            bool ret = false;
            if (topLevelTreeNode == _tnObviousMobs)
            {
                ret = _obviousMobsTNExpanded;
            }
            else if (topLevelTreeNode == _tnObviousExits)
            {
                ret = _obviousExitsTNExpanded;
            }
            else if (topLevelTreeNode == _tnOtherExits)
            {
                ret = _otherExitsTNExpanded;
            }
            else if (topLevelTreeNode == _tnPermanentMobs)
            {
                ret = _permMobsTNExpanded;
            }
            return ret;
        }

        private int GetTopLevelTreeNodeLogicalIndex(TreeNode topLevelTreeNode)
        {
            int i = 0;
            if (topLevelTreeNode == _tnObviousMobs)
            {
                i = 1;
            }
            else if (topLevelTreeNode == _tnObviousExits)
            {
                i = 2;
            }
            else if (topLevelTreeNode == _tnOtherExits)
            {
                i = 3;
            }
            else if (topLevelTreeNode == _tnPermanentMobs)
            {
                i = 4;
            }
            return i;
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
            TreeNode ret = new TreeNode(MobEntity.MobToSingularMapping[mob]);
            ret.Tag = mob;
            return ret;
        }

        private TreeNode GetObviousExitNode(RoomChange nextRoomChange, string s)
        {
            string sNodeText = s;
            Exit foundExit;
            if (nextRoomChange.MappedExits.TryGetValue(s, out foundExit))
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

        private void ComputeColor(int current, int max, out byte r, out byte g, out byte b)
        {
            byte iFullR = _fullColor.R;
            byte iFullG = _fullColor.G;
            byte iFullB = _fullColor.B;
            byte iEmptyR = _emptyColor.R;
            byte iEmptyG = _emptyColor.G;
            byte iEmptyB = _emptyColor.B;
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

        private void txtWeapon_TextChanged(object sender, EventArgs e)
        {
            _weapon = txtWeapon.Text;
            EnableDisableActionButtons(_currentBackgroundParameters);
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

                string sCommandLower = sCommand.ToLower().Trim();
                bool isMovement = sCommandLower == "nw" ||
                                  sCommandLower == "northwest" ||
                                  sCommandLower == "n" ||
                                  sCommandLower == "north" ||
                                  sCommandLower == "ne" ||
                                  sCommandLower == "northeast" ||
                                  sCommandLower == "w" ||
                                  sCommandLower == "west" ||
                                  sCommandLower == "e" ||
                                  sCommandLower == "east" ||
                                  sCommandLower == "sw" ||
                                  sCommandLower == "southwest" ||
                                  sCommandLower == "s" ||
                                  sCommandLower == "south" ||
                                  sCommandLower == "se" ||
                                  sCommandLower == "southeast" ||
                                  sCommandLower == "u" ||
                                  sCommandLower == "up" ||
                                  sCommandLower == "d" ||
                                  sCommandLower == "down" ||
                                  sCommandLower == "out" ||
                                  sCommandLower.StartsWith("go ");
                bool isLook = sCommandLower == "look";
                if (isMovement || isLook)
                {
                    if (_currentBackgroundParameters == null)
                    {
                        if (isLook)
                        {
                            RunLookBackgroundCommand();
                        }
                        else
                        {
                            if (sCommandLower.StartsWith("go "))
                            {
                                sCommandLower = sCommandLower.Substring("go ".Length).Trim();
                                if (string.IsNullOrEmpty(sCommandLower))
                                {
                                    MessageBox.Show("Invalid go command.");
                                    return;
                                }
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
                else
                {
                    SendCommand(txtOneOffCommand.Text, InputEchoType.On);
                }
                txtOneOffCommand.SelectAll();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_finishedQuit)
            {
                if (MessageBox.Show(this, "Are you sure you want to quit?", "Isengard", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    btnQuit_Click(null, null);
                }
                e.Cancel = true;
            }
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

        private void btnQuit_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                bwp.Quit = true;
                RunBackgroundProcess(bwp);
            }
            else
            {
                bwp.Quit = true;
            }
        }

        private void ctxRoomExits_Opening(object sender, CancelEventArgs e)
        {
            Room r = m_oCurrentRoom;
            ctxRoomExits.Items.Clear();
            if (r == null)
            {
                e.Cancel = true;
            }
            else
            {
                bool foundExit = false;
                foreach (Exit nextEdge in IsengardMap.GetAllRoomExits(r))
                {
                    foundExit = true;
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Text = nextEdge.ExitText + ": " + nextEdge.Target.ToString();
                    tsmi.Tag = nextEdge;
                    ctxRoomExits.Items.Add(tsmi);
                }
                if (foundExit)
                {
                    ToolStripMenuItem tsmiGraph = new ToolStripMenuItem();
                    tsmiGraph.Text = "Graph";
                    ctxRoomExits.Items.Add(tsmiGraph);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void ctxRoomExits_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ctx = (ContextMenuStrip)sender;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
            Exit exit = (Exit)clickedItem.Tag;
            Button sourceButton = (Button)ctx.SourceControl;
            List<Exit> exits;
            if (clickedItem.Text == "Graph")
            {
                GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
                frmGraph graphForm = new frmGraph(_gameMap, m_oCurrentRoom, true, flying, levitating, isDay, level);
                graphForm.ShowDialog();
                exits = graphForm.SelectedPath;
                if (exits == null) return;
            }
            else
            {
                exits = new List<Exit>() { exit };
            }
            RunStrategy((Strategy)sourceButton.Tag, exits);
        }

        private void RefreshEnabledForSingleMoveButtons()
        {
            Room r = m_oCurrentRoom;
            bool haveCurrentRoom = r != null;

            bool n, ne, nw, w, e, s, sw, se, u, d, o;
            n = ne = nw = w = e = s = sw = se = u = d = o = false;
            if (haveCurrentRoom)
            {
                foreach (Exit nextExit in IsengardMap.GetAllRoomExits(r))
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

        private void GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level)
        {
            lock (_spellsCastLock)
            {
                if (_spellsCast == null)
                {
                    flying = false;
                    levitating = false;
                }
                else
                {
                    flying = _spellsCast.Contains("fly");
                    levitating = _spellsCast.Contains("levitation");
                }
            }
            isDay = TimeOutputSequence.IsDay(_time);
            level = _level;
        }

        private List<Exit> CalculateRouteExits(Room fromRoom, Room targetRoom)
        {
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            List <Exit> pathExits = MapComputation.ComputeLowestCostPath(fromRoom, targetRoom, flying, levitating, isDay, level);
            if (pathExits == null)
            {
                MessageBox.Show("No path to target room found.");
            }
            return pathExits;
        }

        private void GoToRoom(Room targetRoom)
        {
            Room currentRoom = m_oCurrentRoom;
            if (currentRoom != null)
            {
                List<Exit> exits = CalculateRouteExits(currentRoom, targetRoom);
                if (exits != null)
                {
                    NavigateExitsInBackground(exits);
                }
            }
        }

        private void NavigateExitsInBackground(List<Exit> exits)
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.Exits = exits;
            RunBackgroundProcess(bwp);
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            Room originalCurrentRoom = m_oCurrentRoom;
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmGraph frm = new frmGraph(_gameMap, m_oCurrentRoom, false, flying, levitating, isDay, level);

            if (frm.ShowDialog().GetValueOrDefault(false))
            {
                Room newCurrentRoom = m_oCurrentRoom;
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
                        lock (_roomChangeLock)
                        {
                            m_oCurrentRoom = selectedRoom;
                            _roomChangeCounterUI = -1;
                        }
                    }
                }
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            Room originalCurrentRoom = m_oCurrentRoom;
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmLocations frm = new frmLocations(_gameMap, originalCurrentRoom, flying, levitating, isDay, level);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Room newCurrentRoom = m_oCurrentRoom;
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
                        lock (_roomChangeLock)
                        {
                            m_oCurrentRoom = selectedRoom;
                            _roomChangeCounterUI = -1;
                        }
                    }
                }
            }
        }

        private void InitializeRealm()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _currentRealm = (RealmType)sets.DefaultRealm;
            if (_currentRealm != RealmType.Earth &&
                _currentRealm != RealmType.Fire &&
                _currentRealm != RealmType.Water &&
                _currentRealm != RealmType.Wind)
            {
                _currentRealm = RealmType.Earth;
                sets.DefaultRealm = Convert.ToInt32(_currentRealm);
            }
        }

        private void InitializeAutoSpellLevels()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _autoSpellLevelMin = sets.DefaultAutoSpellLevelMin;
            _autoSpellLevelMax = sets.DefaultAutoSpellLevelMax;
            if (_autoSpellLevelMin > _autoSpellLevelMax || _autoSpellLevelMax < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMax > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM || _autoSpellLevelMin < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMin > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _autoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                _autoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
                sets.DefaultAutoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                sets.DefaultAutoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
            }
        }

        private void tsbConfiguration_Click(object sender, EventArgs e)
        {
            bool autoEscapeActive;
            int autoEscapeThreshold;
            AutoEscapeType autoEscapeType;
            lock (_escapeLock)
            {
                autoEscapeActive = _autoEscapeActive;
                autoEscapeThreshold = _autoEscapeThreshold;
                autoEscapeType = _autoEscapeType;
            }
            frmConfiguration frm = new frmConfiguration(_currentRealm, _autoSpellLevelMin, _autoSpellLevelMax, txtWeapon.Text ?? string.Empty, autoEscapeThreshold, autoEscapeType, autoEscapeActive, _strategies);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                IsengardSettings sets = IsengardSettings.Default;
                _queryMonsterStatus = sets.QueryMonsterStatus;
                _verboseMode = sets.VerboseMode;
                _fullColor = sets.FullColor;
                _emptyColor = sets.EmptyColor;

                _currentRealm = frm.CurrentRealm;
                _autoSpellLevelMin = frm.CurrentAutoSpellLevelMin;
                _autoSpellLevelMax = frm.CurrentAutoSpellLevelMax;
                txtWeapon.Text = frm.CurrentWeapon;

                bool newAutoEscapeActive = frm.CurrentAutoEscapeActive;
                int newAutoEscapeThreshold = frm.CurrentAutoEscapeThreshold;
                AutoEscapeType newAutoEscapeType = frm.CurrentAutoEscapeType;
                lock (_escapeLock)
                {
                    if (autoEscapeActive != newAutoEscapeActive)
                    {
                        _autoEscapeActive = newAutoEscapeActive;
                    }
                    _autoEscapeThreshold = newAutoEscapeThreshold;
                    _autoEscapeType = newAutoEscapeType;
                }

                if (frm.ChangedStrategies)
                {
                    _strategies = frm.Strategies;
                    RefreshStrategyButtons();
                }
            }
        }

        private void btnSkills_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null && PromptForSkills(true, false, false, null, out PromptedSkills activatedSkills, out _))
            {
                bwp = new BackgroundWorkerParameters();
                bwp.UsedSkills = activatedSkills;
                RunBackgroundProcess(bwp);
            }
        }

        private void btnHeal_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = new BackgroundWorkerParameters();
                bwp.HealHitpoints = true;
                bwp.EnsureBlessed = true;
                bwp.EnsureProtected = true;
                bwp.CureIfPoisoned = true;
                RunBackgroundProcess(bwp);
            }
        }

        private void tsmiSetAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            string sDefault = _autoEscapeThreshold > 0 ? _autoEscapeThreshold.ToString() : string.Empty;
            string sNewAutoEscapeThreshold = Interaction.InputBox("New auto escape threshold:", "Auto Escape Threshold", sDefault);
            if (int.TryParse(sNewAutoEscapeThreshold, out int iNewAutoEscapeThreshold) && iNewAutoEscapeThreshold > 0 && iNewAutoEscapeThreshold < _totalhp)
            {
                _autoEscapeThreshold = iNewAutoEscapeThreshold;
                RefreshAutoEscapeUI(true);
            }
            else
            {
                MessageBox.Show("Invalid auto escape threshold: " + sNewAutoEscapeThreshold);
            }
        }

        private void tsmiClearAutoEscapeThreshold_Click(object sender, EventArgs e)
        {
            _autoEscapeActive = false;
            _autoEscapeThreshold = 0;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiToggleAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _autoEscapeActive = !_autoEscapeActiveSaved;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiSetDefaultAutoEscape_Click(object sender, EventArgs e)
        {
            IsengardSettings sets = IsengardSettings.Default;
            sets.DefaultAutoEscapeOnByDefault = _autoEscapeActive;
            sets.DefaultAutoEscapeThreshold = _autoEscapeThreshold;
            sets.DefaultAutoEscapeType = Convert.ToInt32(_autoEscapeType);
            sets.Save();
        }

        /// <summary>
        /// updates the auto-escape UI. This must run on the UI thread.
        /// </summary>
        private void RefreshAutoEscapeUI(bool forceSet)
        {
            bool autoEscapeActive = _autoEscapeActive;
            int autoEscapeThreshold = _autoEscapeThreshold;
            AutoEscapeType autoEscapeType = _autoEscapeTypeUI;
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
                    string sAutoEscapeType = _autoEscapeType == AutoEscapeType.Hazy ? "Hazy" : "Flee";
                    if (_autoEscapeThreshold > 0)
                    {
                        autoEscapeText = sAutoEscapeType + " @ " + _autoEscapeThreshold.ToString();
                    }
                    else
                    {
                        autoEscapeText = sAutoEscapeType;
                    }
                    if (_autoEscapeActive)
                    {
                        if (_autoEscapeType == AutoEscapeType.Hazy)
                        {
                            autoEscapeBackColor = Color.DarkBlue;
                        }
                        else //Flee
                        {
                            autoEscapeBackColor = Color.DarkRed;
                        }
                    }
                    else if (_autoEscapeThreshold > 0)
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

                tsmiAutoEscapeFlee.Checked = _autoEscapeType == AutoEscapeType.Flee;
                tsmiAutoEscapeHazy.Checked = _autoEscapeType == AutoEscapeType.Hazy;

                tsmiAutoEscapeIsActive.Checked = _autoEscapeActive;

                _autoEscapeActiveUI = autoEscapeActive;
                _autoEscapeThresholdUI = autoEscapeThreshold;
                _autoEscapeTypeUI = autoEscapeType;
                _fleeingUI = fleeing;
                _hazyingUI = hazying;
            }
        }

        private void tsmiRestoreDefaultAutoEscape_Click(object sender, EventArgs e)
        {
            SetAutoEscapeThresholdFromDefaults();
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
                _autoEscapeActiveSaved = _autoEscapeActive;
                IsengardSettings sets = IsengardSettings.Default;
                bool hasThreshold = _autoEscapeThreshold > 0;
                tsmiAutoEscapeIsActive.Enabled = hasThreshold;
                tsmiClearAutoEscapeThreshold.Enabled = hasThreshold;
                bool differentFromDefault = _autoEscapeActive != sets.DefaultAutoEscapeOnByDefault || _autoEscapeThreshold != sets.DefaultAutoEscapeThreshold || Convert.ToInt32(_autoEscapeType) != sets.DefaultAutoEscapeType;
                tsmiSetDefaultAutoEscape.Enabled = tsmiRestoreDefaultAutoEscape.Enabled = differentFromDefault;
            }
        }

        private void cboTickRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGoToHealingRoom.Enabled = cboTickRoom.SelectedIndex > 0;
        }

        private void btnGoToHealingRoom_Click(object sender, EventArgs e)
        {
            GoToRoom(_gameMap.HealingRooms[(HealingRoom)cboTickRoom.SelectedItem]);
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
                SetExpandFlag(e.Node, true);
            }
        }

        private void treeCurrentRoom_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (!_programmaticUI)
            {
                SetExpandFlag(e.Node, false);
            }
        }

        private void treeCurrentRoom_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            TreeNode parentNode = selectedNode.Parent;
            if (parentNode == _tnObviousMobs || parentNode == _tnPermanentMobs)
            {
                txtMob.Text = GetTextForMob(parentNode, selectedNode);
            }
        }

        private string GetTextForMob(TreeNode parentNode, TreeNode selectedNode)
        {
            MobTypeEnum eMT = (MobTypeEnum)selectedNode.Tag;
            string word = MobEntity.PickWordForMob(eMT);
            int iCounter = 0;
            foreach (TreeNode nextTreeNode in parentNode.Nodes)
            {
                MobTypeEnum eNextMT = (MobTypeEnum)nextTreeNode.Tag;
                string sSingular = MobEntity.MobToSingularMapping[eNextMT];
                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                {
                    if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                    {
                        iCounter++;
                        break;
                    }
                }
                if (nextTreeNode == selectedNode)
                {
                    break;
                }
            }
            return word + " " + iCounter;
        }

        private void SetExpandFlag(TreeNode node, bool expanded)
        {
            if (node == _tnObviousMobs)
            {
                _obviousMobsTNExpanded = expanded;
            }
            else if (node == _tnObviousExits)
            {
                _obviousExitsTNExpanded = expanded;
            }
            else if (node == _tnOtherExits)
            {
                _otherExitsTNExpanded = expanded;
            }
            else if (node == _tnPermanentMobs)
            {
                _permMobsTNExpanded = expanded;
            }
        }

        private void tsmiGoToRoom_Click(object sender, EventArgs e)
        {
            TreeNode node = treeCurrentRoom.SelectedNode;
            Exit exit = node.Tag as Exit;
            if (exit == null)
            {
                string s = node.Tag as String;
                if (s != null)
                {
                    exit = new Exit(null, null, s);
                }
            }
            if (exit != null)
            {
                NavigateExitsInBackground(new List<Exit>() { exit });
            }
        }

        private void ctxCurrentRoom_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = treeCurrentRoom.SelectedNode;
            bool isObviousExit = false;
            bool isOtherExit = false;
            TreeNode parent = node?.Parent;
            if (parent == _tnObviousExits)
            {
                isObviousExit = true;
            }
            else if (parent == _tnOtherExits)
            {
                isOtherExit = true;
            }
            if (!isOtherExit && !isObviousExit)
            {
                e.Cancel = true;
            }
        }

        private void btnLook_Click(object sender, EventArgs e)
        {
            if (_currentBackgroundParameters == null)
            {
                RunLookBackgroundCommand();
            }
        }

        private void RunLookBackgroundCommand()
        {
            BackgroundWorkerParameters bwp = new BackgroundWorkerParameters();
            bwp.SingleCommandType = BackgroundCommandType.Look;
            RunBackgroundProcess(bwp);
        }
    }
}
