﻿using Microsoft.VisualBasic;
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
        private int _gold = -1;
        private int _goldUI = -1;
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

        private object _entityLock = new object();
        private CurrentRoomInfo _currentRoomInfo = new CurrentRoomInfo();
        private List<string> _foundSearchedExits;
        private InventoryEquipment _inventoryEquipment = new InventoryEquipment();
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
            InitializeAutoEscapeThreshold();

            AlignmentType ePreferredAlignment;
            if (!Enum.TryParse(sets.PreferredAlignment, out ePreferredAlignment))
            {
                ePreferredAlignment = AlignmentType.Blue;
            }

            txtWeapon.Text = sets.Weapon ?? string.Empty;

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

        private void InitializeAutoEscapeThreshold()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _autoEscapeThreshold = sets.AutoEscapeThreshold;
            _autoEscapeType = (AutoEscapeType)sets.AutoEscapeType;
            _autoEscapeActive = sets.AutoEscapeActive;
            if (_autoEscapeType != AutoEscapeType.Flee && _autoEscapeType != AutoEscapeType.Hazy)
            {
                _autoEscapeType = AutoEscapeType.Flee;
                sets.AutoEscapeType = Convert.ToInt32(_autoEscapeType);
            }
            if (_autoEscapeThreshold < 0)
            {
                _autoEscapeThreshold = 0;
                sets.AutoEscapeThreshold = _autoEscapeThreshold;
            }
            if (_autoEscapeThreshold == 0)
            {
                _autoEscapeActive = false;
                sets.AutoEscapeActive = _autoEscapeActive;
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
                btnOneClick.ContextMenuStrip = ctxStrategy;
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
            IsengardSettings sets = IsengardSettings.Default;
            if (sets.RemoveAllOnStartup)
            {
                _initializationSteps = InitializationStep.None;
            }
            else
            {
                _initializationSteps = InitializationStep.RemoveAll;
            }
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
        private void OnScore(FeedLineParameters flParams, int level, int maxHP, int maxMP, int gold, int tnl, List<SkillCooldown> cooldowns, List<string> spells, bool poisoned)
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

        private void OnEquipment(FeedLineParameters flParams, List<KeyValuePair<string, string>> equipment)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Equipment) == InitializationStep.None;

            lock (_entityLock)
            {
                InventoryEquipmentChange changes = new InventoryEquipmentChange();
                changes.ChangeType = InventoryEquipmentChangeType.RefreshEquipment;
                changes.GlobalCounter = ++_inventoryEquipment.InventoryEquipmentCounter;
                for (int i = 0; i < _inventoryEquipment.Equipment.Length; i++)
                {
                    _inventoryEquipment.Equipment[i] = null;
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
                            foreach (EquipmentSlot nextSlot in InventoryEquipment.GetSlotsForEquipmentType(eqType, false))
                            {
                                int iNextSlot = (int)nextSlot;
                                if (_inventoryEquipment.Equipment[iNextSlot] == null)
                                {
                                    _inventoryEquipment.Equipment[iNextSlot] = itemType;
                                    foundSlot = true;
                                    break;
                                }
                            }
                            if (foundSlot)
                            {
                                InventoryEquipmentEntry entry = new InventoryEquipmentEntry();
                                entry.ItemType = itemType;
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
                    _inventoryEquipment.InventoryEquipmentChanges.Add(changes);
                }
            }
            if (forInit)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.Equipment, flParams);
            }
        }

        private void OnInventory(FeedLineParameters flParams, List<ItemEntity> items)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Inventory) == InitializationStep.None;

            lock (_entityLock)
            {
                InventoryEquipmentChange changes = new InventoryEquipmentChange();
                changes.ChangeType = InventoryEquipmentChangeType.RefreshInventory;
                changes.GlobalCounter = ++_inventoryEquipment.InventoryEquipmentCounter;
                _inventoryEquipment.InventoryItems.Clear();
                foreach (ItemEntity nextItemEntity in items)
                {
                    if (nextItemEntity.ItemType.HasValue)
                    {
                        ItemTypeEnum nextItemValue = nextItemEntity.ItemType.Value;
                        for (int i = 0; i < nextItemEntity.Count; i++)
                        {
                            _inventoryEquipment.InventoryItems.Add(nextItemValue);
                            InventoryEquipmentEntry entry = new InventoryEquipmentEntry();
                            entry.ItemType = nextItemValue;
                            entry.InventoryAction = true;
                            changes.Changes.Add(entry);
                        }
                    }
                }
                _inventoryEquipment.InventoryEquipmentChanges.Add(changes);
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

        private void OnInitialLogin(InitialLoginInfo initialLoginInfo)
        {
            SendCommand("score", InputEchoType.Off);
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
                Room r = _currentRoomInfo.CurrentRoom;
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
            
            Room previousRoom = _currentRoomInfo.CurrentRoom;

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
            else if (rtType == RoomTransitionType.WordOfRecall || rtType == RoomTransitionType.Death)
            {
                _hazying = false;
                _fleeing = false;
                if (roomTransitionInfo.DrankHazy)
                {
                    AddOrRemoveItemsFromInventoryOrEquipment(flParams, new List<ItemTypeEnum>() { ItemTypeEnum.HazyPotion }, false, false);
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

            lock (_entityLock) //update the room change list with the next room
            {
                _currentRoomInfo.CurrentObviousExits.Clear();
                _currentRoomInfo.CurrentObviousExits.AddRange(obviousExits);

                _currentRoomInfo.CurrentRoomChanges.Clear();
                RoomChange rc = new RoomChange();
                rc.Room = newRoom;
                rc.ChangeType = RoomChangeType.NewRoom;
                rc.Exits = new List<string>(obviousExits);
                rc.GlobalCounter = ++_currentRoomInfo.RoomChangeCounter;
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
                        else if (nextExit.PresenceType != ExitPresenceType.RequiresSearch && ExitExistsInList(obviousExits, nextExitText)) //requires search exits behave like hidden ones
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
                    List<MobTypeEnum> currentRoomMobs = _currentRoomInfo.CurrentRoomMobs;
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
                                }
                            }
                        }
                    }
                    rc.Mobs = new List<MobTypeEnum>(currentRoomMobs);
                    _currentRoomInfo.CurrentRoomChanges.Add(rc);
                    _currentRoomInfo.CurrentRoom = newRoom;
                }
            }
        }

        private bool ExitExistsInList(List<string> displayedObviousExits, string mapExitText)
        {
            bool ret = false;
            foreach (string sNextExit in displayedObviousExits)
            {
                //use the first word for the mapping since subsequent words aren't used anyway
                string sCheck = sNextExit.Trim();
                int iFirstSpace = sCheck.IndexOf(' ');
                if (iFirstSpace > 0)
                {
                    sCheck = sCheck.Substring(0, iFirstSpace);
                }
                if (sCheck == mapExitText)
                {
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

        private void OnSelfSpellCast(FeedLineParameters flParams, BackgroundCommandType? commandType, string activeSpell)
        {
            if (!string.IsNullOrEmpty(activeSpell))
            {
                AddActiveSpell(activeSpell);
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

        private void FailDrinkPotion(FeedLineParameters flParams)
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
            if (fumbled)
            {
                lock (_entityLock)
                {
                    if (_inventoryEquipment.Equipment[(int)EquipmentSlot.Weapon1].HasValue)
                    {
                        var weapon = new List<ItemTypeEnum>() { _inventoryEquipment.Equipment[(int)EquipmentSlot.Weapon1].Value };
                        AddOrRemoveItemsFromInventoryOrEquipment(flParams, weapon, false, true);
                    }
                }
            }
            if (!string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                if (!fumbled && killedMonster)
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
                    case InformationalMessageType.ManashieldOff:
                        ChangeSkillActive(SkillWithCooldownType.Manashield, false);
                        break;
                    case InformationalMessageType.FleeFailed:
                        finishedProcessing = true;
                        break;
                    case InformationalMessageType.BullroarerInMithlond:
                        lock (_entityLock)
                        {
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInMithlond, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerInNindamos:
                        lock (_entityLock)
                        {
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.Bullroarer:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForWaitForMessageExit(currentRoom, InformationalMessageType.BullroarerInNindamos, true));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.BullroarerReadyForBoarding:
                        lock (_entityLock)
                        {
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.BullroarerMithlond:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                    case BoatEmbarkOrDisembark.BullroarerNindamos:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "gangway"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressInBree:
                        lock (_entityLock)
                        {
                            bool removeMessage = true;
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "dock"));
                                        removeMessage = false;
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, true, "steamboat"));
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
                        lock (_entityLock)
                        {
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "dock"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressBree:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "steamboat"));
                                        break;
                                }
                            }
                        }
                        break;
                    case InformationalMessageType.CelduinExpressLeftMithlond:
                        lock (_entityLock)
                        {
                            Room currentRoom = _currentRoomInfo.CurrentRoom;
                            if (currentRoom != null && currentRoom.BoatLocationType.HasValue)
                            {
                                switch (currentRoom.BoatLocationType.Value)
                                {
                                    case BoatEmbarkOrDisembark.CelduinExpress:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "pier"));
                                        break;
                                    case BoatEmbarkOrDisembark.CelduinExpressMithlond:
                                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, false, "gangway"));
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
                        rc.GlobalCounter = _currentRoomInfo.RoomChangeCounter;
                        MobTypeEnum nextMob = next.Mob;
                        List<MobTypeEnum> currentRoomMobs = _currentRoomInfo.CurrentRoomMobs;
                        lock (_entityLock)
                        {
                            int index = currentRoomMobs.LastIndexOf(nextMob);
                            int iInsertionPoint;
                            if (index >= 0)
                            {
                                iInsertionPoint = index + 1;
                            }
                            else
                            {
                                iInsertionPoint = _currentRoomInfo.FindNewMobInsertionPoint(nextMob);
                            }
                            bool insertAtEnd = iInsertionPoint == -1;
                            rc.Index = insertAtEnd ? -1 : iInsertionPoint;
                            for (int i = 0; i < next.MobCount; i++)
                            {
                                rc.Mobs.Add(nextMob);
                                if (insertAtEnd)
                                    currentRoomMobs.Add(nextMob);
                                else
                                    currentRoomMobs.Insert(iInsertionPoint, nextMob);
                            }
                            rc.GlobalCounter = ++_currentRoomInfo.RoomChangeCounter;
                            _currentRoomInfo.CurrentRoomChanges.Add(rc);
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

        private void RemoveMobs(MobTypeEnum mobType, int Count)
        {
            RoomChange rc = new RoomChange();
            rc.ChangeType = RoomChangeType.RemoveMob;
            rc.Mobs = new List<MobTypeEnum>();
            List<MobTypeEnum> currentRoomMobs = _currentRoomInfo.CurrentRoomMobs;
            lock (_entityLock)
            {
                int index = currentRoomMobs.LastIndexOf(mobType);
                if (index >= 0)
                {
                    int iLastIndexCount = Count - 1;
                    for (int i = 0; i < Count; i++)
                    {
                        if (currentRoomMobs[index] == mobType)
                        {
                            rc.Mobs.Add(mobType);
                            currentRoomMobs.RemoveAt(index);
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
                    rc.GlobalCounter = ++_currentRoomInfo.RoomChangeCounter;
                    rc.Index = index;
                    _currentRoomInfo.CurrentRoomChanges.Add(rc);
                }
            }
        }

        private void HandleHarbringerStatusChange(bool inPort)
        {
            lock (_entityLock)
            {
                Room currentRoom = _currentRoomInfo.CurrentRoom;
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
                        _currentRoomInfo.CurrentRoomChanges.Add(GetAddExitRoomChangeForPeriodicExit(currentRoom, inPort, sExit));
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
            _currentRoomInfo.RoomChangeCounter++;
            rc.GlobalCounter = _currentRoomInfo.RoomChangeCounter;
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
            rc.GlobalCounter = ++_currentRoomInfo.RoomChangeCounter;
            Exit e = IsengardMap.GetRoomExits(currentRoom, (exit) => { return exit.WaitForMessage.HasValue && exit.WaitForMessage.Value == messageType; }).First();
            rc.Exits.Add(e.ExitText);
            if (add)
            {
                rc.MappedExits[e.ExitText] = e;
            }
            return rc;
        }

        private void OnInventoryManagement(FeedLineParameters flParams, List<ItemTypeEnum> items, bool isAdd, bool isEquipment, int? gold, int sellGold, List<string> activeSpells, bool potionConsumed)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.RemoveAll) == InitializationStep.None;
            bool hasItems = items != null && items.Count > 0;
            bool hasSpells = activeSpells != null && activeSpells.Count > 0;
            bool hasGold = gold.HasValue || sellGold > 0;
            bool couldBeRemoveAll = !hasSpells && !hasGold && !isAdd;
            if (hasItems)
            {
                AddOrRemoveItemsFromInventoryOrEquipment(flParams, items, isAdd, isEquipment);
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
            if (forInit && couldBeRemoveAll)
            {
                AfterProcessInitializationStep(currentStep, InitializationStep.RemoveAll, flParams);
            }
        }

        private void AddOrRemoveItemsFromInventoryOrEquipment(FeedLineParameters flParams, List<ItemTypeEnum> items, bool isAdd, bool isEquipment)
        {
            InventoryEquipmentChangeType changeType;
            if (isEquipment)
            {
                if (isAdd)
                {
                    changeType = InventoryEquipmentChangeType.EquipItem;
                }
                else
                {
                    changeType = InventoryEquipmentChangeType.UnequipItem;
                }
            }
            else
            {
                if (isAdd)
                {
                    changeType = InventoryEquipmentChangeType.AddItemToInventory;
                }
                else
                {
                    changeType = InventoryEquipmentChangeType.RemoveItemFromInventory;
                }
            }
            InventoryEquipmentChange iec = new InventoryEquipmentChange();
            iec.ChangeType = changeType;
            iec.GlobalCounter = ++_inventoryEquipment.InventoryEquipmentCounter;
            lock (_entityLock)
            {
                foreach (ItemTypeEnum nextItem in items)
                {
                    bool addChange = false;
                    InventoryEquipmentEntry changeEntry = new InventoryEquipmentEntry();
                    changeEntry.ItemType = nextItem;
                    if (isEquipment)
                    {
                        StaticItemData sid = ItemEntity.StaticItemData[nextItem];
                        if (sid.EquipmentType != EquipmentType.Unknown)
                        {
                            foreach (EquipmentSlot nextSlot in InventoryEquipment.GetSlotsForEquipmentType(sid.EquipmentType, !isAdd))
                            {
                                int iSlotIndex = (int)nextSlot;
                                if (isAdd)
                                {
                                    if (_inventoryEquipment.Equipment[iSlotIndex] == null)
                                    {
                                        changeEntry.EquipmentIndex = _inventoryEquipment.FindNewEquipmentInsertionPoint(nextSlot);
                                        changeEntry.EquipmentAction = true;
                                        _inventoryEquipment.Equipment[iSlotIndex] = sid.ItemType;
                                        iec.AddOrRemoveInventoryItem(_inventoryEquipment, nextItem, false, changeEntry);
                                        addChange = true;
                                        break;
                                    }
                                }
                                else //remove
                                {
                                    if (_inventoryEquipment.Equipment[iSlotIndex] == nextItem)
                                    {
                                        changeEntry.EquipmentIndex = _inventoryEquipment.FindEquipmentRemovalPoint(nextSlot);
                                        changeEntry.EquipmentAction = false;
                                        _inventoryEquipment.Equipment[iSlotIndex] = null;
                                        iec.AddOrRemoveInventoryItem(_inventoryEquipment, nextItem, true, changeEntry);
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
                    else
                    {
                        addChange = iec.AddOrRemoveInventoryItem(_inventoryEquipment, nextItem, isAdd, changeEntry);
                    }
                    if (addChange)
                    {
                        iec.Changes.Add(changeEntry);
                    }
                }
                if (iec.Changes.Count > 0)
                {
                    _inventoryEquipment.InventoryEquipmentChanges.Add(iec);
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
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains, null), //could be prefixed by "Scared of going X"*
                new SelfSpellCastSequence(OnSelfSpellCast),
                new ConstantOutputSequence("Stun cast on ", OnStun, ConstantSequenceMatchType.StartsWith, 0, BackgroundCommandType.Stun),
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
                new ConstantOutputSequence("You don't have that.", FailDrinkPotion, ConstantSequenceMatchType.ExactMatch, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion }),
                new ConstantOutputSequence(" starts to evaporates before you drink it.", FailDrinkPotion, ConstantSequenceMatchType.EndsWith, 0, new List<BackgroundCommandType>() { BackgroundCommandType.DrinkHazy, BackgroundCommandType.DrinkNonHazyPotion }),
                new ConstantOutputSequence("You prepare yourself for traps.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("You've already prepared.", OnSuccessfulPrepare, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Prepare),
                new ConstantOutputSequence("I don't see that exit.", CantSeeExit, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("You open the ", OpenDoorSuccess, ConstantSequenceMatchType.StartsWith, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("It's already open.", OpenDoorSuccess, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.OpenDoor),
                new ConstantOutputSequence("You see nothing special about it.", OnSeeNothingSpecial, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.LookAtMob),
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
            btnLookAtMob.Tag = new CommandButtonTag(btnLookAtMob, "look {mob}", CommandType.None, DependentObjectType.Mob);
            btnCastVigor.Tag = new CommandButtonTag(btnCastVigor, null, CommandType.Magic, DependentObjectType.None);
            btnCastCurePoison.Tag = new CommandButtonTag(btnCastCurePoison, null, CommandType.Magic, DependentObjectType.None);
            btnAttackMob.Tag = new CommandButtonTag(btnAttackMob, null, CommandType.Melee, DependentObjectType.Mob);
            btnDrinkVigor.Tag = new CommandButtonTag(btnDrinkVigor, null, CommandType.Potions, DependentObjectType.None);
            btnDrinkCurepoison.Tag = new CommandButtonTag(btnDrinkCurepoison, null, CommandType.Potions, DependentObjectType.None);
            btnDrinkMend.Tag = new CommandButtonTag(btnDrinkMend, null, CommandType.Potions, DependentObjectType.None);
            btnUseWandOnMob.Tag = new CommandButtonTag(btnUseWandOnMob, "zap {wand} {mob}", CommandType.Magic, DependentObjectType.Wand | DependentObjectType.Mob);
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
                            //choose first monster of the same type
                            MobTypeEnum eMobValue = bwp.MonsterKilledType.Value;
                            List<MobTypeEnum> currentRoomMobs = _currentRoomInfo.CurrentRoomMobs;
                            lock (_entityLock)
                            {
                                int iIndexOfMonster = currentRoomMobs.IndexOf(eMobValue);
                                if (iIndexOfMonster >= 0)
                                {
                                    txtMob.Text = PickMobText(iIndexOfMonster);
                                    setMobToFirstAvailable = false;
                                }
                            }
                        }
                    }
                    else //presumably the mob is still there so leave it selected
                    {
                        txtMob.Text = bwp.TargetRoomMob;
                        setMobToFirstAvailable = false;
                    }
                }
                if (setMobToFirstAvailable)
                {
                    string sText = null;
                    List<MobTypeEnum> currentRoomMobs = _currentRoomInfo.CurrentRoomMobs;
                    lock (_entityLock)
                    {
                        if (currentRoomMobs.Count > 0)
                        {
                            sText = PickMobText(0);
                        }
                    }
                    if (!string.IsNullOrEmpty(sText))
                    {
                        txtMob.Text = sText;
                    }
                }

                if (setMobToFirstAvailable && _currentRoomInfo.tnObviousMobs.Nodes.Count > 0)
                {
                    treeCurrentRoom.SelectedNode = _currentRoomInfo.tnObviousMobs.Nodes[0];
                }
                ToggleBackgroundProcessUI(bwp, false);
                _currentBackgroundParameters = null;
                RefreshEnabledForSingleMoveButtons();
            }
        }

        /// <summary>
        /// pick selection text for an entity, assumes the entity lock is present
        /// </summary>
        /// <param name="index">index of the mob in the room mob list</param>
        /// <param name="isMob">true for a mob, false for an inventory item</param>
        /// <returns>selection text for the entity</returns>
        private string PickMobText(int mobIndex)
        {
            string ret = null;
            List<MobTypeEnum> mobs = _currentRoomInfo.CurrentRoomMobs;
            if (mobs.Count > mobIndex)
            {
                MobTypeEnum eMobType = mobs[mobIndex];
                foreach (string word in MobEntity.GetMobWords(eMobType))
                {
                    string sSingular;

                    //find word index within the list of mobs
                    int iCounter = 0;
                    for (int i = 0; i < mobIndex; i++)
                    {
                        MobTypeEnum eMob = _currentRoomInfo.CurrentRoomMobs[i];
                        bool matches = false;
                        if (eMob == eMobType)
                        {
                            matches = true;
                        }
                        else
                        {
                            sSingular = MobEntity.StaticMobData[eMob].SingularName;
                            foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                            {
                                if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                                {
                                    matches = true;
                                    break;
                                }
                            }
                        }
                        if (matches)
                        {
                            iCounter++;
                        }
                    }
                    iCounter++;

                    bool isDuplicate = false;

                    int iInventoryCounter = 0;
                    foreach (ItemTypeEnum nextItem in _inventoryEquipment.InventoryItems)
                    {
                        sSingular = ItemEntity.StaticItemData[nextItem].SingularName;
                        foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                        {
                            if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                            {
                                iInventoryCounter++;
                                if (iInventoryCounter == iCounter)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    isDuplicate = iInventoryCounter == iCounter;

                    if (!isDuplicate)
                    {
                        int iEquipmentCounter = 0;
                        foreach (ItemTypeEnum? nextItem in _inventoryEquipment.Equipment)
                        {
                            if (nextItem.HasValue)
                            {
                                ItemTypeEnum eItemValue = nextItem.Value;
                                sSingular = ItemEntity.StaticItemData[eItemValue].SingularName;
                                foreach (string nextWord in sSingular.Split(new char[] { ' ' }))
                                {
                                    if (nextWord.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                                    {
                                        iEquipmentCounter++;
                                        if (iEquipmentCounter == iCounter)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        isDuplicate = iEquipmentCounter == iCounter;
                    }

                    if (!isDuplicate)
                    {
                        if (iCounter == 1)
                        {
                            ret = word;
                        }
                        else
                        {
                            ret = word + " " + iCounter;
                        }
                        break;
                    }
                }
            }
            return ret;
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            try
            {
                if (pms.SingleCommandType.HasValue)
                {
                    BackgroundCommandType cmdType = pms.SingleCommandType.Value;
                    if (cmdType == BackgroundCommandType.Look)
                    {
                        RunSingleCommandForCommandResult(pms.SingleCommandType.Value, "look", pms, null);
                    }
                    else if (cmdType == BackgroundCommandType.Search)
                    {
                        RunSingleCommand(BackgroundCommandType.Search, "search", pms, null);
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
                                        backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, AbortIfFleeingOrHazying);
                                        if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                        {
                                            if (_currentRoomInfo.CurrentObviousExits.Contains(exitText))
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
                            bool useGo;
                            string sExitWord = GetExitWord(nextExit, out useGo);
                            if (!PreOpenDoorExit(nextExit, sExitWord, pms))
                            {
                                return;
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
                            
                            if (needHeal || needCurepoison)
                            {
                                bool doHealingLogic = !targetIsDamageRoom;
                                if (doHealingLogic)
                                {
                                    lock (_entityLock)
                                    {
                                        foreach (var nextMob in _currentRoomInfo.CurrentRoomMobs)
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
                                    if (!DoBackgroundHeal(needHeal, false, false, needCurepoison, pms)) return;
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
                AfterKillMonsterAction onMonsterKilledAction = AfterKillMonsterAction.ContinueCombat;
                bool hasInitialQueuedMagicStep;
                bool hasInitialQueuedMeleeStep;
                bool hasInitialQueuedPotionsStep;
                lock (_queuedCommandLock)
                {
                    hasInitialQueuedMagicStep = pms.QueuedMagicStep.HasValue;
                    hasInitialQueuedMeleeStep = pms.QueuedMeleeStep.HasValue;
                    hasInitialQueuedPotionsStep = pms.QueuedPotionsStep.HasValue;
                }
                if (_hazying || _fleeing || strategy != null || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep)
                {
                    int savedAutoSpellMin = _autoSpellLevelMin;
                    int savedAutoSpellMax = _autoSpellLevelMax;
                    try
                    {
                        _currentlyFightingMob = pms.TargetRoomMob;

                        bool haveMeleeStrategySteps = false;
                        bool haveMagicStrategySteps = false;
                        bool havePotionsStrategySteps = false;
                        if (strategy != null)
                        {
                            haveMagicStrategySteps = strategy.HasAnyMagicSteps();
                            haveMeleeStrategySteps = strategy.HasAnyMeleeSteps();
                            havePotionsStrategySteps = strategy.HasAnyPotionsSteps();
                            onMonsterKilledAction = strategy.AfterKillMonsterAction;
                            if (strategy.AutoSpellLevelMin != -1 && strategy.AutoSpellLevelMax != -1)
                            {
                                _autoSpellLevelMin = strategy.AutoSpellLevelMin;
                                _autoSpellLevelMax = strategy.AutoSpellLevelMax;
                            }
                        }
                        List<string> offensiveSpells = CastOffensiveSpellSequence.GetOffensiveSpellsForRealm(_currentRealm);
                        List<string> knownSpells;
                        lock (_spellsKnownLock)
                        {
                            knownSpells = _spellsKnown;
                        }
                        int? calculatedMinLevel, calculatedMaxLevel;
                        Strategy.GetMinMaxOffensiveSpellLevels(strategy, _autoSpellLevelMin, _autoSpellLevelMax, knownSpells, offensiveSpells, out calculatedMinLevel, out calculatedMaxLevel);

                        _monsterDamage = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        if (useManaPool)
                        {
                            _currentMana = pms.ManaPool;
                        }

                        //validate weapon
                        ItemTypeEnum? weaponItem = null;
                        bool useMelee = haveMeleeStrategySteps || hasInitialQueuedMeleeStep;
                        if (useMelee && !string.IsNullOrEmpty(_weapon))
                        {
                            string errorMessage = null;
                            if (Enum.TryParse(_weapon, out ItemTypeEnum weaponItemValue))
                            {
                                StaticItemData weaponData = ItemEntity.StaticItemData[weaponItemValue];
                                if (weaponData.WeaponType.HasValue)
                                {
                                    weaponItem = weaponItemValue;
                                }
                                else
                                {
                                    errorMessage = "Weapon is not actually a weapon: " + _weapon;
                                }
                            }
                            else
                            {
                                errorMessage = "Invalid weapon: " + _weapon;
                            }
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                lock (_broadcastMessagesLock)
                                {
                                    _broadcastMessages.Add(errorMessage);
                                }
                            }
                        }

                        if (haveMagicStrategySteps || haveMeleeStrategySteps || havePotionsStrategySteps || hasInitialQueuedMagicStep || hasInitialQueuedMeleeStep || hasInitialQueuedPotionsStep)
                        {
                            _backgroundProcessPhase = BackgroundProcessPhase.Combat;
                            bool doPowerAttack = false;
                            if (useMelee)
                            {
                                WieldWeapon(weaponItem);
                                doPowerAttack = (pms.UsedSkills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
                            }
                            IEnumerator<MagicStrategyStep> magicSteps = strategy?.GetMagicSteps().GetEnumerator();
                            IEnumerator<MeleeStrategyStep> meleeSteps = strategy?.GetMeleeSteps(doPowerAttack).GetEnumerator();
                            IEnumerator<PotionsStrategyStep> potionsSteps = strategy?.GetPotionsSteps().GetEnumerator();
                            MagicStrategyStep? nextMagicStep = null;
                            MeleeStrategyStep? nextMeleeStep = null;
                            PotionsStrategyStep? nextPotionsStep = null;
                            bool magicStepsFinished = magicSteps == null || !magicSteps.MoveNext();
                            bool meleeStepsFinished = meleeSteps == null || !meleeSteps.MoveNext();
                            bool potionsStepsFinished = potionsSteps == null || !potionsSteps.MoveNext();
                            bool magicOnlyWhenStunned = strategy != null && ((strategy.TypesToRunOnlyWhenMonsterStunned & CommandType.Magic) != CommandType.None);
                            bool meleeOnlyWhenStunned = strategy != null && ((strategy.TypesToRunOnlyWhenMonsterStunned & CommandType.Melee) != CommandType.None);
                            bool potionsOnlyWhenStunned = strategy != null && ((strategy.TypesToRunOnlyWhenMonsterStunned & CommandType.Potions) != CommandType.None);
                            if (!magicStepsFinished) nextMagicStep = magicSteps.Current;
                            if (!meleeStepsFinished) nextMeleeStep = meleeSteps.Current;
                            if (!potionsStepsFinished) nextPotionsStep = potionsSteps.Current;
                            DateTime? dtNextMagicCommand = null;
                            DateTime? dtNextMeleeCommand = null;
                            DateTime? dtNextPotionsCommand = null;
                            while (true) //combat cycle
                            {
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (magicStepsFinished) CheckForQueuedMagicStep(pms, ref nextMagicStep);
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

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
                                    MagicCommandChoiceResult result = GetMagicCommand(strategy, nextMagicStep.Value, currentHP, _totalhp, currentMana, out manaDrain, out bct, out command, offensiveSpells, knownSpells);
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

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
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

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (meleeStepsFinished) CheckForQueuedMeleeStep(pms, ref nextMeleeStep);
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

                                if (nextMeleeStep.HasValue &&
                                    (_monsterStunned || !meleeOnlyWhenStunned) && 
                                    (!dtNextMeleeCommand.HasValue || DateTime.UtcNow > dtNextMeleeCommand.Value))
                                {
                                    GetMeleeCommand(nextMeleeStep.Value, out command);
                                    WieldWeapon(weaponItem); //wield the weapon in case it was fumbled
                                    if (!RunBackgroundMeleeStep(BackgroundCommandType.Attack, command, pms, meleeSteps, ref meleeStepsFinished, ref nextMeleeStep, ref dtNextMeleeCommand, ref didDamage))
                                        return;
                                }

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
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

                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);
                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;

                                if (nextPotionsStep.HasValue &&
                                    (_monsterStunned || !potionsOnlyWhenStunned) &&
                                    (!dtNextPotionsCommand.HasValue || DateTime.UtcNow > dtNextPotionsCommand.Value))
                                {
                                    PotionsCommandChoiceResult potionChoice = GetPotionsCommand(strategy, nextPotionsStep.Value, out command, _inventoryEquipment, _entityLock, _autohp, _totalhp);
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
                                        if (!RunBackgroundPotionsStep(BackgroundCommandType.DrinkNonHazyPotion, command, pms, potionsSteps, ref potionsStepsFinished, ref nextPotionsStep, ref dtNextPotionsCommand))
                                            return;
                                    }
                                }

                                if (!SelectMobAfterKillMonster(onMonsterKilledAction, pms)) break;
                                if (BreakOutOfBackgroundCombat(onMonsterKilledAction)) break;
                                if (potionsStepsFinished) CheckForQueuedPotionsStep(pms, ref nextPotionsStep);

                                //flee or stop combat once steps complete
                                if (!nextPotionsStep.HasValue && strategy != null)
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
                            Room r = _currentRoomInfo.CurrentRoom;

                            //run the preexit logic for all target exits, since it won't be known beforehand
                            //which exit will be used.
                            List<Exit> availableExits = new List<Exit>();
                            foreach (Exit nextExit in IsengardMap.GetRoomExits(r, FleeExitDiscriminator))
                            {
                                string sExitWord = GetExitWord(nextExit, out _);
                                if (PreOpenDoorExit(nextExit, sExitWord, pms))
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
                        if (_hazying && (onMonsterKilledAction != AfterKillMonsterAction.StopCombat || !_monsterKilled))
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
                        _pleaseWaitSequence.ClearLastPotionsWaitSeconds();
                        _currentlyFightingMob = null;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
                        _monsterKilledType = null;
                        _currentMana = HP_OR_MP_UNKNOWN;
                        _autoSpellLevelMin = savedAutoSpellMin;
                        _autoSpellLevelMax = savedAutoSpellMax;
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

        public PotionsCommandChoiceResult GetPotionsCommand(Strategy Strategy, PotionsStrategyStep nextPotionsStep, out string command, InventoryEquipment inventoryEquipment, object entityLockObject, int currentHP, int totalHP)
        {
            command = null;
            lock (entityLockObject)
            {
                bool supportsMend = Strategy != null && Strategy.PotionsMendOnlyWhenDownXHP > 0;
                bool supportsVigor = Strategy != null && Strategy.PotionsVigorOnlyWhenDownXHP > 0;
                if (nextPotionsStep == PotionsStrategyStep.Vigor && !supportsVigor) return PotionsCommandChoiceResult.Fail;
                if (nextPotionsStep == PotionsStrategyStep.MendWounds && !supportsMend) return PotionsCommandChoiceResult.Fail;
                if (nextPotionsStep == PotionsStrategyStep.GenericHeal && !supportsVigor && !supportsMend) return PotionsCommandChoiceResult.Fail;
                bool canMend = supportsMend && currentHP + Strategy.PotionsMendOnlyWhenDownXHP <= totalHP;
                bool canVigor = supportsVigor && currentHP + Strategy.PotionsVigorOnlyWhenDownXHP <= totalHP;
                if (nextPotionsStep == PotionsStrategyStep.Vigor && !canVigor) return PotionsCommandChoiceResult.Skip;
                if (nextPotionsStep == PotionsStrategyStep.MendWounds && !canMend) return PotionsCommandChoiceResult.Skip;
                if (nextPotionsStep == PotionsStrategyStep.GenericHeal && !canVigor && !canMend) return PotionsCommandChoiceResult.Skip;

                //check inventory for potions
                foreach (int inventoryIndex in GetValidPotionsIndices(nextPotionsStep, inventoryEquipment, canVigor, canMend))
                {
                    ItemTypeEnum itemType = inventoryEquipment.InventoryItems[inventoryIndex];
                    string sText = inventoryEquipment.PickItemTextFromActualIndex(true, itemType, inventoryIndex);
                    if (!string.IsNullOrEmpty(sText))
                    {
                        command = "drink " + sText;
                        break;
                    }
                }

                //check held equipment slot for a potion
                if (!string.IsNullOrEmpty(command))
                {
                    int iHeldSlot = (int)EquipmentSlot.Held;
                    ItemTypeEnum? heldItem = inventoryEquipment.Equipment[iHeldSlot];
                    if (heldItem.HasValue)
                    {
                        ItemTypeEnum eHeldItem = heldItem.Value;
                        StaticItemData sid = ItemEntity.StaticItemData[eHeldItem];
                        ValidPotionType potionValidity = GetPotionValidity(sid, nextPotionsStep, canMend, canVigor);
                        if (potionValidity == ValidPotionType.Primary || potionValidity == ValidPotionType.Secondary)
                        {
                            string sText = inventoryEquipment.PickItemTextFromActualIndex(false, eHeldItem, iHeldSlot);
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

        private IEnumerable<int> GetValidPotionsIndices(PotionsStrategyStep nextPotionsStep, InventoryEquipment inventoryEquipment, bool canVigor, bool canMend)
        {
            int iIndex = 0;
            List<int> savedIndexes = new List<int>();
            foreach (ItemTypeEnum nextItem in inventoryEquipment.InventoryItems)
            {
                StaticItemData sid = ItemEntity.StaticItemData[nextItem];
                ValidPotionType potionValidity = GetPotionValidity(sid, nextPotionsStep, canMend, canVigor);
                if (potionValidity == ValidPotionType.Primary)
                {
                    yield return iIndex;
                }
                else if (potionValidity == ValidPotionType.Secondary)
                {
                    savedIndexes.Add(iIndex);
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

        public void GetMeleeCommand(MeleeStrategyStep nextMeleeStep, out string command)
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
            command = sAttackType + " " + _currentlyFightingMob;
        }

        public MagicCommandChoiceResult GetMagicCommand(Strategy Strategy, MagicStrategyStep nextMagicStep, int currentHP, int totalHP, int currentMP, out int manaDrain, out BackgroundCommandType? bct, out string command, List<string> offensiveSpells, List<string> knownSpells)
        {
            MagicCommandChoiceResult ret = MagicCommandChoiceResult.Cast;
            bool doCast;
            command = null;
            manaDrain = 0;
            bct = null;
            if (nextMagicStep == MagicStrategyStep.Stun)
            {
                command = "cast stun " + _currentlyFightingMob;
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
                    if (Strategy != null && Strategy.MagicMendOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.MagicMendOnlyWhenDownXHP <= totalHP;
                    else
                        doCast = currentHP < totalHP;
                    if (doCast)
                    {
                        nextMagicStep = MagicStrategyStep.MendWounds;
                    }
                }
                if (nextMagicStep == MagicStrategyStep.GenericHeal || nextMagicStep == MagicStrategyStep.MendWounds)
                {
                    if (Strategy != null && Strategy.MagicVigorOnlyWhenDownXHP > 0)
                        doCast = currentHP + Strategy.MagicVigorOnlyWhenDownXHP <= totalHP;
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
                    if (currentMP >= 15 && _autoSpellLevelMin <= 4 && _autoSpellLevelMax >= 4 && knownSpells.Contains(offensiveSpells[3]))
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel4;
                    }
                    else if (currentMP >= 10 && _autoSpellLevelMin <= 3 && _autoSpellLevelMax >= 3 && knownSpells.Contains(offensiveSpells[2]))
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel3;
                    }
                    else if (currentMP >= 7 && _autoSpellLevelMin <= 2 && _autoSpellLevelMax >= 2 && knownSpells.Contains(offensiveSpells[1]))
                    {
                        nextMagicStep = MagicStrategyStep.OffensiveSpellLevel2;
                    }
                    else if (currentMP >= 3 && _autoSpellLevelMin <= 1 && _autoSpellLevelMax >= 1 && knownSpells.Contains(offensiveSpells[0]))
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
                    if (nextMagicStep == MagicStrategyStep.OffensiveSpellLevel4)
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
                        command = "cast " + spell + " " + _currentlyFightingMob;
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
                lock (_entityLock)
                {
                    int index;
                    if (onMonsterKilledAction == AfterKillMonsterAction.SelectFirstMonsterInRoom)
                    {
                        if (_currentRoomInfo.CurrentRoomMobs.Count == 0) return false;
                        index = 0;
                    }
                    else
                    {
                        if (!_monsterKilledType.HasValue) return false;
                        index = _currentRoomInfo.CurrentRoomMobs.IndexOf(_monsterKilledType.Value);
                        if (index < 0) return false;
                    }
                    string sMobText = PickMobText(index);
                    if (string.IsNullOrEmpty(sMobText)) return false;
                    _currentlyFightingMob = sMobText;
                    _mob = sMobText;
                    bwp.TargetRoomMob = sMobText;
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
                lock (_entityLock)
                {
                    if (_inventoryEquipment.Equipment[(int)EquipmentSlot.Weapon1] == weaponItemValue)
                    {
                        string sWeaponText = _inventoryEquipment.PickItemTextFromItemCounter(false, weaponItemValue, 1);
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
                lock (_entityLock)
                {
                    if (_inventoryEquipment.Equipment[(int)EquipmentSlot.Weapon1] == null && _inventoryEquipment.InventoryItems.Contains(weaponItemValue))
                    {
                        string sWeaponText = _inventoryEquipment.PickItemTextFromItemCounter(true, weaponItemValue, 1);
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
            return !exit.Hidden && !exit.NoFlee;
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
                Room r = _currentRoomInfo.CurrentRoom;
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

        private bool RunBackgroundPotionsStep(BackgroundCommandType bct, string command, BackgroundWorkerParameters pms, IEnumerator<PotionsStrategyStep> potionsSteps, ref bool potionsStepsFinished, ref PotionsStrategyStep? nextPotionsStep, ref DateTime? dtNextPotionsCommand)
        {
            CommandResult backgroundCommandResult = RunSingleCommandForCommandResult(bct, command, pms, AbortIfFleeingOrHazying);
            if (backgroundCommandResult == CommandResult.CommandAborted || backgroundCommandResult == CommandResult.CommandTimeout)
            {
                return false;
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

        private bool PreOpenDoorExit(Exit exit, string exitWord, BackgroundWorkerParameters pms)
        {
            bool ret;
            if (exit.MustOpen)
            {
                ret = RunSingleCommand(BackgroundCommandType.OpenDoor, "open " + exitWord, pms, AbortIfFleeingOrHazying);
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
            Room currentRoom = _currentRoomInfo.CurrentRoom;
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
                s.LastPotionsStep = step;
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

        private void tsbInventoryAndEquipment_Click(object sender, EventArgs e)
        {
            RunCommand("equipment");
            RunCommand("inventory");
        }

        private string TranslateCommand(string command)
        {
            string sMob = _currentlyFightingMob;
            if (string.IsNullOrEmpty(sMob))
            {
                sMob = _mob;
            }
            return command.Replace("{mob}", sMob)
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
            public PotionsStrategyStep? QueuedPotionsStep { get; set; }
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
                if (MessageBox.Show("No weapon specified. Continue?", "Strategy", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
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
                targetRoom = _currentRoomInfo.CurrentRoom;
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

            int iGold = _gold;
            if (iGold != _goldUI)
            {
                lblGold.Text = "Gold: " + iGold.ToString();
                _goldUI = iGold;
            }
            int iTNL = _tnl;
            if (iTNL != _tnlUI)
            {
                lblToNextLevelValue.Text = "TNL: " + iTNL.ToString();
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

            lock (_entityLock)
            {
                List<RoomChange> changes = null;
                int iNewCounter = -1;
                Room oCurrentRoom = _currentRoomInfo.CurrentRoom;
                if (oCurrentRoom != _currentRoomInfo.CurrentRoomUI)
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
                    _currentRoomInfo.CurrentRoomUI = oCurrentRoom;
                    if (_currentBackgroundParameters == null)
                    {
                        btnGoToHealingRoom.Enabled = oCurrentRoom != null;
                        RefreshEnabledForSingleMoveButtons();
                    }
                }
                if (_currentRoomInfo.RoomChangeCounter != _currentRoomInfo.RoomChangeCounterUI)
                {
                    List<RoomChange> currentRoomChanges = _currentRoomInfo.CurrentRoomChanges;
                    for (int i = 0; i < currentRoomChanges.Count; i++)
                    {
                        RoomChange nextRoomChange = currentRoomChanges[i];
                        int iCounter = nextRoomChange.GlobalCounter;
                        if (iCounter > _currentRoomInfo.RoomChangeCounterUI)
                        {
                            if (changes == null) changes = new List<RoomChange>();
                            changes.Add(nextRoomChange);
                        }
                    }
                }
                if (changes != null)
                {
                    TreeNode tnObviousMobs = _currentRoomInfo.tnObviousMobs;
                    TreeNode tnObviousExits = _currentRoomInfo.tnObviousExits;
                    TreeNode tnOtherExits = _currentRoomInfo.tnOtherExits;
                    TreeNode tnPermanentMobs = _currentRoomInfo.tnPermanentMobs;
                    foreach (RoomChange nextRoomChange in changes)
                    {
                        RoomChangeType rcType = nextRoomChange.ChangeType;
                        if (rcType == RoomChangeType.NewRoom)
                        {
                            bool firstTimeThrough = _currentRoomInfo.RoomChangeCounterUI == -1;
                            treeCurrentRoom.Nodes.Clear();
                            tnObviousMobs.Nodes.Clear();
                            tnObviousExits.Nodes.Clear();
                            tnOtherExits.Nodes.Clear();
                            tnPermanentMobs.Nodes.Clear();
                            foreach (MobTypeEnum nextMob in nextRoomChange.Mobs)
                            {
                                tnObviousMobs.Nodes.Add(GetMobsNode(nextMob));
                            }
                            if (tnObviousMobs.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnObviousMobs);
                                if (firstTimeThrough || _currentRoomInfo.ObviousMobsTNExpanded)
                                {
                                    tnObviousMobs.Expand();
                                }
                            }
                            foreach (string s in nextRoomChange.Exits)
                            {
                                tnObviousExits.Nodes.Add(GetObviousExitNode(nextRoomChange, s));
                            }
                            if (tnObviousExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnObviousExits);
                                if (firstTimeThrough || _currentRoomInfo.ObviousExitsTNExpanded)
                                {
                                    tnObviousExits.Expand();
                                }
                            }
                            foreach (Exit nextExit in nextRoomChange.OtherExits)
                            {
                                tnOtherExits.Nodes.Add(GetOtherExitsNode(nextExit));
                            }
                            if (tnOtherExits.Nodes.Count > 0)
                            {
                                treeCurrentRoom.Nodes.Add(tnOtherExits);
                                if (firstTimeThrough || _currentRoomInfo.OtherExitsTNExpanded)
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
                                    if (firstTimeThrough || _currentRoomInfo.PermMobsTNExpanded)
                                    {
                                        tnPermanentMobs.Expand();
                                    }
                                }
                            }
                        }
                        else if (rcType == RoomChangeType.AddExit)
                        {
                            string exit = nextRoomChange.Exits[0];
                            bool hasNodes = tnObviousExits.Nodes.Count > 0;
                            tnObviousExits.Nodes.Add(GetObviousExitNode(nextRoomChange, exit));
                            TreeNode removeNode = null;
                            foreach (TreeNode tn in tnOtherExits.Nodes)
                            {
                                if (tn.Tag == nextRoomChange.MappedExits[exit])
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
                        else if (rcType == RoomChangeType.RemoveExit)
                        {
                            string exit = nextRoomChange.Exits[0];
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
                        else if (rcType == RoomChangeType.AddMob)
                        {
                            bool hasNodes = tnObviousMobs.Nodes.Count > 0;
                            bool isFirst = true;
                            MobTypeEnum? firstMob = null;
                            TreeNode firstInserted = null;
                            int iAddIndex = nextRoomChange.Index;
                            bool insertAtEnd = iAddIndex == -1;
                            foreach (MobTypeEnum nextMobType in nextRoomChange.Mobs)
                            {
                                TreeNode inserted = GetMobsNode(nextMobType);
                                if (insertAtEnd)
                                {
                                    tnObviousMobs.Nodes.Add(inserted);
                                }
                                else
                                {
                                    tnObviousMobs.Nodes.Insert(iAddIndex, inserted);
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
                                InsertTopLevelTreeNode(tnObviousMobs);
                                if (bwp == null)
                                {
                                    string sNewMobText = string.Empty;
                                    lock (_entityLock)
                                    {
                                        if (_currentRoomInfo.CurrentRoomMobs.Count > 0)
                                        {
                                            sNewMobText = PickMobText(0);
                                        }
                                    }
                                    txtMob.Text = sNewMobText;
                                }
                            }
                        }
                        else if (rcType == RoomChangeType.RemoveMob)
                        {
                            int iRemoveIndex = nextRoomChange.Index;
                            for (int i = 0; i < nextRoomChange.Mobs.Count; i++)
                            {
                                tnObviousMobs.Nodes.RemoveAt(iRemoveIndex);
                            }
                            if (tnObviousMobs.Nodes.Count == 0)
                            {
                                treeCurrentRoom.Nodes.Remove(tnObviousMobs);
                            }
                        }
                        
                        iNewCounter = nextRoomChange.GlobalCounter;
                    }
                    _currentRoomInfo.RoomChangeCounterUI = iNewCounter;
                }
            }

            lock (_entityLock)
            {
                List<InventoryEquipmentChange> changes = null;
                int iNewCounter = -1;
                if (_inventoryEquipment.InventoryEquipmentCounter != _inventoryEquipment.InventoryEquipmentCounterUI)
                {
                    List<InventoryEquipmentChange> currentInvEqChanges = _inventoryEquipment.InventoryEquipmentChanges;
                    for (int i = 0; i < currentInvEqChanges.Count; i++)
                    {
                        InventoryEquipmentChange nextInvEqChange = currentInvEqChanges[i];
                        int iCounter = nextInvEqChange.GlobalCounter;
                        if (iCounter > _inventoryEquipment.InventoryEquipmentCounterUI)
                        {
                            if (changes == null) changes = new List<InventoryEquipmentChange>();
                            changes.Add(nextInvEqChange);
                        }
                    }
                    if (changes != null)
                    {
                        foreach (InventoryEquipmentChange nextInvEqChange in changes)
                        {
                            InventoryEquipmentChangeType iect = nextInvEqChange.ChangeType;
                            if (iect == InventoryEquipmentChangeType.RefreshInventory)
                            {
                                lstInventory.Items.Clear();
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    lstInventory.Items.Add(new ItemInInventoryList(nextEntry.ItemType));
                                }
                            }
                            else if (iect == InventoryEquipmentChangeType.RefreshEquipment)
                            {
                                lstEquipment.Items.Clear();
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    lstEquipment.Items.Add(new ItemInEquipmentList(nextEntry.ItemType));
                                }
                            }
                            if (iect == InventoryEquipmentChangeType.RemoveItemFromInventory || iect == InventoryEquipmentChangeType.EquipItem)
                            {
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    if (nextEntry.InventoryAction.HasValue && !nextEntry.InventoryAction.Value)
                                    {
                                        lstInventory.Items.RemoveAt(nextEntry.InventoryIndex);
                                    }
                                }
                            }
                            if (iect == InventoryEquipmentChangeType.AddItemToInventory || iect == InventoryEquipmentChangeType.UnequipItem)
                            {
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    if (nextEntry.InventoryAction.GetValueOrDefault(true))
                                    {
                                        ItemInInventoryList it = new ItemInInventoryList(nextEntry.ItemType);
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
                            if (iect == InventoryEquipmentChangeType.EquipItem)
                            {
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    if (nextEntry.EquipmentAction.GetValueOrDefault(true))
                                    {
                                        ItemInEquipmentList it = new ItemInEquipmentList(nextEntry.ItemType);
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
                            else if (iect == InventoryEquipmentChangeType.UnequipItem)
                            {
                                foreach (InventoryEquipmentEntry nextEntry in nextInvEqChange.Changes)
                                {
                                    if (nextEntry.EquipmentAction.HasValue && !nextEntry.EquipmentAction.Value)
                                    {
                                        lstEquipment.Items.RemoveAt(nextEntry.EquipmentIndex);
                                    }
                                }
                            }
                            iNewCounter = nextInvEqChange.GlobalCounter;
                        }
                        _inventoryEquipment.InventoryEquipmentCounterUI = iNewCounter;
                    }
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

        private void InsertTopLevelTreeNode(TreeNode topLevelTreeNode)
        {
            if (!treeCurrentRoom.Nodes.Contains(topLevelTreeNode))
            {
                int iIndex = _currentRoomInfo.GetTopLevelTreeNodeLogicalIndex(topLevelTreeNode);
                bool found = false;
                for (int i = 0; i < treeCurrentRoom.Nodes.Count; i++)
                {
                    TreeNode next = treeCurrentRoom.Nodes[i];
                    int nextIndex = _currentRoomInfo.GetTopLevelTreeNodeLogicalIndex(next);
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
                if (_currentRoomInfo.GetTopLevelTreeNodeExpanded(topLevelTreeNode))
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
            TreeNode ret = new TreeNode(MobEntity.StaticMobData[mob].SingularName);
            ret.Tag = mob;
            return ret;
        }

        private TreeNode GetObviousExitNode(RoomChange nextRoomChange, string s)
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
                            RunSingleBackgroundCommand(BackgroundCommandType.Look);
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
            Room r = _currentRoomInfo.CurrentRoom;
            ctxStrategy.Items.Clear();
            ToolStripMenuItem tsmi;
            if (r != null)
            {
                foreach (Exit nextEdge in IsengardMap.GetAllRoomExits(r))
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = nextEdge.ExitText + ": " + nextEdge.Target.ToString();
                    tsmi.Tag = nextEdge;
                    ctxStrategy.Items.Add(tsmi);
                }
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "Graph";
                ctxStrategy.Items.Add(tsmi);
                tsmi = new ToolStripMenuItem();
                tsmi.Text = "Location";
                ctxStrategy.Items.Add(tsmi);
            }
            tsmi = new ToolStripMenuItem();
            tsmi.Text = "Edit";
            ctxStrategy.Items.Add(tsmi);
        }

        private void ctxStrategy_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ctx = (ContextMenuStrip)sender;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
            Exit exit = (Exit)clickedItem.Tag;
            Button sourceButton = (Button)ctx.SourceControl;
            List<Exit> exits;
            string sItemText = clickedItem.Text;
            Room currentRoom = _currentRoomInfo.CurrentRoom;
            if (sItemText == "Graph")
            {
                GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
                frmGraph graphForm = new frmGraph(_gameMap, currentRoom, true, flying, levitating, isDay, level);
                graphForm.ShowDialog();
                exits = graphForm.SelectedPath;
                if (exits == null) return;
            }
            else if (sItemText == "Location")
            {
                GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
                frmLocations locationsForm = new frmLocations(_gameMap, currentRoom, true, flying, levitating, isDay, level);
                locationsForm.ShowDialog();
                exits = locationsForm.SelectedPath;
                if (exits == null) return;
            }
            else if (sItemText == "Edit")
            {
                Strategy s = (Strategy)sourceButton.Tag;
                frmStrategy frm = new frmStrategy(new Strategy(s));
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    s = frm.NewStrategy;
                    sourceButton.Tag = s;
                    sourceButton.Text = s.ToString();
                }
                return;
            }
            else
            {
                exits = new List<Exit>() { exit };
            }
            RunStrategy((Strategy)sourceButton.Tag, exits);
        }

        private void RefreshEnabledForSingleMoveButtons()
        {
            Room r = _currentRoomInfo.CurrentRoom;
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
            Room currentRoom = _currentRoomInfo.CurrentRoom;
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
            Room originalCurrentRoom = _currentRoomInfo.CurrentRoom;
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmGraph frm = new frmGraph(_gameMap, originalCurrentRoom, false, flying, levitating, isDay, level);

            if (frm.ShowDialog().GetValueOrDefault(false))
            {
                Room newCurrentRoom = _currentRoomInfo.CurrentRoom;
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
                        lock (_entityLock)
                        {
                            _currentRoomInfo.CurrentRoom = selectedRoom;
                            _currentRoomInfo.RoomChangeCounterUI = -1;
                        }
                    }
                }
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            Room originalCurrentRoom = _currentRoomInfo.CurrentRoom;
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmLocations frm = new frmLocations(_gameMap, originalCurrentRoom, false, flying, levitating, isDay, level);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                Room newCurrentRoom = _currentRoomInfo.CurrentRoom;
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
                        lock (_entityLock)
                        {
                            _currentRoomInfo.CurrentRoom = selectedRoom;
                            _currentRoomInfo.RoomChangeCounterUI = -1;
                        }
                    }
                }
            }
        }

        private void InitializeRealm()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _currentRealm = (RealmType)sets.Realm;
            if (_currentRealm != RealmType.Earth &&
                _currentRealm != RealmType.Fire &&
                _currentRealm != RealmType.Water &&
                _currentRealm != RealmType.Wind)
            {
                _currentRealm = RealmType.Earth;
                sets.Realm = Convert.ToInt32(_currentRealm);
            }
        }

        private void InitializeAutoSpellLevels()
        {
            IsengardSettings sets = IsengardSettings.Default;
            _autoSpellLevelMin = sets.AutoSpellLevelMin;
            _autoSpellLevelMax = sets.AutoSpellLevelMax;
            if (_autoSpellLevelMin > _autoSpellLevelMax || _autoSpellLevelMax < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMax > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM || _autoSpellLevelMin < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || _autoSpellLevelMin > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                _autoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                _autoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
                sets.AutoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                sets.AutoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
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
                IsengardSettings sets = IsengardSettings.Default;
                sets.AutoEscapeThreshold = _autoEscapeThreshold;
                sets.Save();
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
            IsengardSettings sets = IsengardSettings.Default;
            sets.AutoEscapeActive = false;
            sets.AutoEscapeThreshold = 0;
            sets.Save();
            RefreshAutoEscapeUI(true);
        }

        private void tsmiToggleAutoEscapeActive_Click(object sender, EventArgs e)
        {
            _autoEscapeActive = !_autoEscapeActiveSaved;
            IsengardSettings sets = IsengardSettings.Default;
            sets.AutoEscapeActive = _autoEscapeActive;
            sets.Save();
            RefreshAutoEscapeUI(true);
        }

        private void tsmiAutoEscapeFlee_Click(object sender, EventArgs e)
        {
            _autoEscapeType = AutoEscapeType.Flee;
            IsengardSettings sets = IsengardSettings.Default;
            sets.AutoEscapeType = (int)_autoEscapeType;
            sets.Save();
            tsmiAutoEscapeFlee.Checked = true;
            tsmiAutoEscapeHazy.Checked = false;
            RefreshAutoEscapeUI(true);
        }

        private void tsmiAutoEscapeHazy_Click(object sender, EventArgs e)
        {
            _autoEscapeType = AutoEscapeType.Hazy;
            IsengardSettings sets = IsengardSettings.Default;
            sets.AutoEscapeType = (int)_autoEscapeType;
            sets.Save();
            tsmiAutoEscapeFlee.Checked = false;
            tsmiAutoEscapeHazy.Checked = true;
            RefreshAutoEscapeUI(true);
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
                bool hasThreshold = _autoEscapeThreshold > 0;
                tsmiAutoEscapeIsActive.Enabled = hasThreshold;
                tsmiClearAutoEscapeThreshold.Enabled = hasThreshold;
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
                _currentRoomInfo.SetExpandFlag(e.Node, true);
            }
        }

        private void treeCurrentRoom_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (!_programmaticUI)
            {
                _currentRoomInfo.SetExpandFlag(e.Node, false);
            }
        }

        private void treeCurrentRoom_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            TreeNode parentNode = selectedNode.Parent;
            bool isObviousMobs = parentNode == _currentRoomInfo.tnObviousMobs;
            bool isPermanentMobs = parentNode == _currentRoomInfo.tnPermanentMobs;
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
                int iFoundMobCount = 0;
                string mobText = null;
                lock (_entityLock)
                {
                    //find the equivalently numbered mob in the current room list
                    int iCurrentMobIndex = -1;
                    int iIteratorMobIndex = 0;
                    foreach (MobTypeEnum nextMobType in _currentRoomInfo.CurrentRoomMobs)
                    {
                        if (nextMobType == selectedMob)
                        {
                            iCurrentMobIndex = iIteratorMobIndex;
                            iFoundMobCount++;
                            if (iFoundMobCount == iMobCount)
                            {
                                break;
                            }
                        }
                        iIteratorMobIndex++;
                    }
                    if (iCurrentMobIndex >= 0)
                    {
                        mobText = PickMobText(iCurrentMobIndex);
                    }
                }

                //Fall back on using the index in the permanent mobs list. This
                //is particularly useful for hidden permanent mobs.
                if (isPermanentMobs && iFoundMobCount == 0 && string.IsNullOrEmpty(mobText))
                {
                    var enumerator = MobEntity.GetMobWords(selectedMob).GetEnumerator();
                    enumerator.MoveNext();
                    mobText = enumerator.Current;
                    if (iMobCount > 1)
                    {
                        mobText += " " + iMobCount;
                    }
                }

                txtMob.Text = mobText;
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
            if (parent == _currentRoomInfo.tnObviousExits)
            {
                isObviousExit = true;
            }
            else if (parent == _currentRoomInfo.tnOtherExits)
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
            StaticItemData sid;
            int iCounter = 0;
            ItemTypeEnum itemType;
            lock (_entityLock)
            {
                object oObj = lst.SelectedItem;
                if (oObj == null)
                {
                    e.Cancel = true;
                    return;
                }
                if (isInventory)
                {
                    itemType = ((ItemInInventoryList)oObj).ItemType;
                }
                else
                {
                    itemType = ((ItemInEquipmentList)oObj).ItemType;
                }
                sid = ItemEntity.StaticItemData[itemType];
                if (isInventory)
                {
                    foreach (ItemInInventoryList nextEntry in lstInventory.Items)
                    {
                        if (nextEntry.ItemType == itemType)
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
                        if (nextEntry.ItemType == itemType)
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
            }

            SelectedInventoryOrEquipmentItem sioei = new SelectedInventoryOrEquipmentItem();
            sioei.ItemType = itemType;
            sioei.Counter = iCounter;
            sioei.IsInventory = isInventory;
            ctxInventoryOrEquipmentItem.Tag = sioei;

            string sActionTransferBetweenInventoryAndEquipment;
            ItemClass iclass = sid.ItemClass;
            ToolStripMenuItem tsmi;
            if (isInventory)
            {
                if (iclass == ItemClass.Potion)
                {
                    tsmi = new ToolStripMenuItem();
                    tsmi.Text = "drink";
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
        }

        private void ctxInventoryOrEquipmentItem_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SelectedInventoryOrEquipmentItem sioei = (SelectedInventoryOrEquipmentItem)ctxInventoryOrEquipmentItem.Tag;
            lock (_entityLock)
            {
                string sText = _inventoryEquipment.PickItemTextFromItemCounter(sioei.IsInventory, sioei.ItemType, sioei.Counter);
                if (!string.IsNullOrEmpty(sText))
                {
                    SendCommand(e.ClickedItem.Text + " " + sText, InputEchoType.On);
                }
            }
        }

        private class SelectedInventoryOrEquipmentItem
        {
            public ItemTypeEnum ItemType;
            public int Counter;
            public bool IsInventory;
        }
    }
}
