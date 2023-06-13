﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        private string _realm1Spell;
        private string _realm2Spell;
        private string _realm3Spell;
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

        private object _spellsLock = new object();
        private List<string> _spellsCast = new List<string>();
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
        private bool _setTickRoom = false;
        private BackgroundWorker _bw;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private BackgroundProcessPhase _backgroundProcessPhase;
        private PleaseWaitSequence _pleaseWaitSequence;
        private InitialLoginSequence _initializationLoginSequence;
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
        private List<string> _currentObviousExits;
        private List<string> _foundSearchedExits;
        
        private string _currentlyFightingMob;
        private string _currentlyFightingMobUI;
        private MonsterStatus _currentMonsterStatus;
        private MonsterStatus _currentMonsterStatusUI;
        private int _monsterDamage;
        private int _monsterDamageUI;
        private bool _monsterStunned;
        private bool _monsterKilled;

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
            _initializationLoginSequence = new InitialLoginSequence(OnInitialLogin);

            SetButtonTags();

            _asciiMapping = AsciiMapping.GetAsciiMapping();

            IsengardSettings sets = IsengardSettings.Default;
            _verboseMode = sets.VerboseMode;
            _queryMonsterStatus = sets.QueryMonsterStatus;
            _fullColor = sets.FullColor;
            _emptyColor = sets.EmptyColor;

            InitializeRealm(true);
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
            _initializationLoginSequence.Active = true;
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
            lock (_spellsLock)
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
            lock (_spellsLock)
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
                currentStep |= InitializationStep.Score;
                _initializationSteps = currentStep;
                if (currentStep == InitializationStep.BeforeFinalization)
                {
                    ProcessInitialLogin(flParams);
                }
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

        private void OnWho(FeedLineParameters flParams, HashSet<string> playerNames)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Who) == InitializationStep.None;

            _players = playerNames;

            if (forInit)
            {
                currentStep |= InitializationStep.Who;
                _initializationSteps = currentStep;
                if (currentStep == InitializationStep.BeforeFinalization)
                {
                    ProcessInitialLogin(flParams);
                }
                flParams.SuppressEcho = true;
            }
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
                currentStep |= InitializationStep.Time;
                _initializationSteps = currentStep;
                if (currentStep == InitializationStep.BeforeFinalization)
                {
                    ProcessInitialLogin(flParams);
                }
                flParams.SuppressEcho = true;
            }
        }

        private void OnRemoveEquipment(FeedLineParameters flParams)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.RemoveAll) == InitializationStep.None;

            //currently there is no processing of removing equipment

            if (forInit)
            {
                currentStep |= InitializationStep.RemoveAll;
                _initializationSteps = currentStep;
                if (currentStep == InitializationStep.BeforeFinalization)
                {
                    ProcessInitialLogin(flParams);
                }
                flParams.SuppressEcho = true;
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
            _initializationLoginSequence.Active = false;
            SendCommand("score", InputEchoType.Off);
            SendCommand("who", InputEchoType.Off);
            SendCommand("remove all", InputEchoType.Off);
            SendCommand("time", InputEchoType.Off);
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
                Room r = SetCurrentRoomIfUnambiguous(sRoomName);
                _setTickRoom = r != null && r.HealingRoom.HasValue;
            }
            else
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.Add("Initial login failed!");
                }
            }
        }

        private Room SetCurrentRoomIfUnambiguous(string sRoomName)
        {
            Room ret = null;
            if (_gameMap.UnambiguousRooms.TryGetValue(sRoomName, out ret))
            {
                m_oCurrentRoom = ret;
            }
            return ret;
        }

        private void OnRoomTransition(RoomTransitionInfo roomTransitionInfo, int damage, TrapType trapType)
        {
            RoomTransitionType rtType = roomTransitionInfo.TransitionType;
            string sRoomName = roomTransitionInfo.RoomName;
            List<string> obviousExits = roomTransitionInfo.ObviousExits;
            FeedLineParameters flParams = roomTransitionInfo.FeedLineParameters;

            BackgroundCommandType? bct = null;
            if (flParams != null)
            {
                bct = flParams.BackgroundCommandType;
            }
            bool fromBackgroundFlee = false;
            bool fromBackgroundMove = false;
            bool fromBackgroundHazy = false;
            bool fromAnyBackgroundCommand = bct.HasValue;
            if (fromAnyBackgroundCommand)
            {
                BackgroundCommandType bctValue = bct.Value;
                fromBackgroundFlee = bctValue == BackgroundCommandType.Flee;
                fromBackgroundMove = bctValue == BackgroundCommandType.Look || bctValue == BackgroundCommandType.Movement;
                fromBackgroundHazy = bctValue == BackgroundCommandType.DrinkHazy;
            }
            
            _currentObviousExits = obviousExits;
            if (rtType == RoomTransitionType.Flee)
            {
                _fleeing = false;
                if (fromBackgroundFlee)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                    _waitSeconds = 0;
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
                SetCurrentRoomIfUnambiguous(sRoomName);
            }
            else if (rtType == RoomTransitionType.Initial)
            {
                //Nothing to do here
            }
            else if (fromBackgroundMove)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
                _waitSeconds = 0;
            }
            if (fromBackgroundFlee || fromBackgroundMove) //not sure if you can flee to a trap room
            {
                _lastCommandDamage = damage;
                _lastCommandTrapType = trapType;
                _lastCommandMovementResult = MovementResult.Success;
            }

            List<string> errorMessages = roomTransitionInfo.ErrorMessages;
            if (errorMessages.Count > 0)
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.AddRange(errorMessages);
                }
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
        
        private void OnAttack(bool fumbled, int damage, bool killedMonster, int experience, bool powerAttacked, FeedLineParameters flParams)
        {
            if (powerAttacked)
            {
                ChangeSkillActive(SkillWithCooldownType.PowerAttack, false);
            }
            _tnl = Math.Max(0, _tnl - experience);
            if (!string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                if (fumbled) _fumbled = true;
                else if (killedMonster) _monsterKilled = true;
                _monsterDamage += damage;
            }
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
            {
                _lastCommandDamage = damage;
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void OnCastOffensiveSpell(int damage, bool killedMonster, int experience, FeedLineParameters flParams)
        {
            _tnl = Math.Max(0, _tnl - experience);
            if (!string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                _monsterDamage += damage;
                if (killedMonster) _monsterKilled = true;
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

        private void OnInformationalMessages(List<InformationalMessages> ims, List<string> broadcasts, List<string> addedPlayers, List<string> removedPlayers)
        {
            if (ims != null)
            {
                List<string> spellsOff = null;
                foreach (InformationalMessages nextMessage in ims)
                {
                    switch (nextMessage)
                    {
                        case InformationalMessages.DayStart:
                            _time = SUNRISE_GAME_HOUR;
                            _timeLastUpdatedUTC = DateTime.UtcNow;
                            break;
                        case InformationalMessages.NightStart:
                            _time = SUNSET_GAME_HOUR;
                            _timeLastUpdatedUTC = DateTime.UtcNow;
                            break;
                        case InformationalMessages.BlessOver:
                            if (spellsOff == null) spellsOff = new List<string>();
                            spellsOff.Add("bless");
                            break;
                        case InformationalMessages.ProtectionOver:
                            if (spellsOff == null) spellsOff = new List<string>();
                            spellsOff.Add("protection");
                            break;
                        case InformationalMessages.FlyOver:
                            if (spellsOff == null) spellsOff = new List<string>();
                            spellsOff.Add("fly");
                            break;
                        case InformationalMessages.ManashieldOff:
                            ChangeSkillActive(SkillWithCooldownType.Manashield, false);
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
                                _refreshSpellsCast = true;
                            }
                        }
                    }
                }
                Exit currentBackgroundExit = _currentBackgroundExit;
                if (currentBackgroundExit != null)
                {
                    InformationalMessages? waitForExit = currentBackgroundExit.WaitForMessage;
                    if (waitForExit.HasValue && ims.Contains(waitForExit.Value))
                    {
                        _currentBackgroundExitMessageReceived = true;
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
                                if (nextProcessingSequence.IsActive())
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
                _initializationLoginSequence,
                new InformationalMessagesSequence(OnInformationalMessages),
                new ScoreOutputSequence(_username, OnScore),
                new WhoOutputSequence(OnWho),
                new RemoveEquipmentSequence(OnRemoveEquipment),
                new MobStatusSequence(OnMobStatusSequence),
                new TimeOutputSequence(OnTime),
                _pleaseWaitSequence,
                new SuccessfulSearchSequence(SuccessfulSearch),
                new RoomTransitionSequence(OnRoomTransition, _username),
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

                //the search find failed output has a blank line before the message so use the second line.
                new ConstantOutputSequence("You didn't find anything.", FailSearch, ConstantSequenceMatchType.ExactMatch, 1, BackgroundCommandType.Search)
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
            btnLook.Tag = new CommandButtonTag(btnLook, "look", CommandType.None, DependentObjectType.None);
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
                Room wentToRoom = bwp.NavigatedToRoom;
                if (wentToRoom != null)
                {
                    SetCurrentRoom(wentToRoom);
                }
                if (bwp.ReachedTargetRoom && !string.IsNullOrEmpty(bwp.TargetRoomMob))
                {
                    txtMob.Text = bwp.TargetRoomMob;
                }
                _backgroundProcessPhase = BackgroundProcessPhase.None;
                ToggleBackgroundProcessUI(bwp, false);
                _currentBackgroundParameters = null;
                RefreshEnabledForSingleMoveButtons();
            }
        }

        private void SetCurrentRoom(Room r)
        {
            string defaultMob = r.GetDefaultMob();
            if (!string.IsNullOrEmpty(defaultMob))
            {
                txtMob.Text = defaultMob;
            }
            m_oCurrentRoom = r;
            RefreshEnabledForSingleMoveButtons();
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            try
            {
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
                    else
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

                bool atDestination = false;
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
                                        _currentObviousExits = null;
                                        backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, AbortIfFleeingOrHazying);
                                        if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                        {
                                            if (_currentObviousExits.Contains(exitText))
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
                            RunPreExitLogic(nextExit);
                            string nextCommand = GetExitCommand(exitText);
                            bool targetIsDamageRoom = nextExitTarget.DamageType.HasValue;

                            bool keepTryingMovement = true;
                            while (keepTryingMovement)
                            {
                                backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, AbortIfFleeingOrHazying);
                                if (backgroundCommandSuccess) //successfully traversed the exit to the new room
                                {
                                    exitList.RemoveAt(0);
                                    keepTryingMovement = false;
                                    if (nextExitTarget != null)
                                    {
                                        pms.NavigatedToRoom = nextExitTarget;
                                    }
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

                    //if we got here that means all exits were traversed successfully
                    pms.ReachedTargetRoom = true;
                    atDestination = true;
                }
                else if (!string.IsNullOrEmpty(pms.TargetRoomMob))
                {
                    atDestination = true;
                }

                if (atDestination)
                {
                    string sTargetRoomMob = pms.TargetRoomMob;
                    if (!string.IsNullOrEmpty(sTargetRoomMob))
                    {
                        _mob = sTargetRoomMob;
                    }
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
                        stratCurrent = new StrategyInstance(strategy, _autoSpellLevelMin, _autoSpellLevelMax, _currentlyFightingMob, _realm1Spell, _realm2Spell, _realm3Spell);

                        _monsterDamage = 0;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
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
                                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + _mob, pms, AbortIfFleeingOrHazying);
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
                            Exit singleFleeableExit = null;
                            Room r = pms.NavigatedToRoom;
                            if (r == null) r = m_oCurrentRoom;
                            if (r != null && _gameMap.MapGraph.TryGetOutEdges(r, out IEnumerable<Exit> exits))
                            {
                                List<Exit> fleeableExits = new List<Exit>();
                                foreach (Exit nextExit in exits)
                                {
                                    if (!nextExit.Hidden && !nextExit.NoFlee)
                                    {
                                        fleeableExits.Add(nextExit);
                                    }
                                }
                                if (fleeableExits.Count == 1) //run preexit logic if the flee is unambiguous
                                {
                                    singleFleeableExit = fleeableExits[0];
                                    RunPreExitLogic(singleFleeableExit);
                                }
                            }

                            backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Flee, "flee", pms, AbortIfHazying);
                            if (backgroundCommandSuccess)
                            {
                                pms.Fled = true;
                                if (singleFleeableExit != null)
                                {
                                    pms.NavigatedToRoom = singleFleeableExit.Target;
                                }
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
                        _pleaseWaitSequence.ClearLastMagicWaitSeconds();
                        _pleaseWaitSequence.ClearLastMeleeWaitSeconds();
                        _currentlyFightingMob = null;
                        _currentMonsterStatus = MonsterStatus.None;
                        _monsterStunned = false;
                        _monsterKilled = false;
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
                lock (_spellsLock)
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
                lock (_spellsLock)
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
                    nextMagicStep.Value == MagicStrategyStep.OffensiveSpellLevel3)
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

        private string GetExitCommand(string target)
        {
            string ret;
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
                    ret = target;
                    break;
                default:
                    ret = "go " + target;
                    break;
            }
            return ret;
        }

        private void RunPreExitLogic(Exit exit)
        {
            if (exit.MustOpen)
            {
                SendCommand("open " + exit.ExitText, InputEchoType.On);
            }
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
            yield return chkExecuteMove;
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
            yield return btnExitSingleMove;
            yield return btnOtherSingleMove;
            yield return btnHeal;
            yield return btnSkills;
            yield return btnSearch;
            yield return btnHide;
            yield return btnSet;
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
            foreach (CommandButtonTag oTag in GetButtonsForEnablingDisabling())
            {
                if ((oTag.ObjectType & DependentObjectType.Mob) != DependentObjectType.None && string.IsNullOrEmpty(_mob))
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
                object oControl = oTag.Control;
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
            yield return (CommandButtonTag)btnLook.Tag;
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
                DoSingleMove(chkExecuteMove.Checked, move);
            }
        }

        private void btnDoSingleMove_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).Tag.ToString();
            DoSingleMove(chkExecuteMove.Checked, direction);
        }

        private void DoSingleMove(bool move, Exit exit)
        {
            if (move)
            {
                NavigateExitsInBackground(new List<Exit>() { exit });
            }
            else
            {
                SetCurrentRoom(exit.Target);
            }
        }

        private void DoSingleMove(bool move, string direction)
        {
            if (m_oCurrentRoom != null)
            {
                Exit foundExit = null;
                if (_gameMap.MapGraph.TryGetOutEdges(m_oCurrentRoom, out IEnumerable<Exit> edges))
                {
                    foreach (Exit nextExit in edges)
                    {
                        if (string.Equals(nextExit.ExitText, direction, StringComparison.OrdinalIgnoreCase))
                        {
                            foundExit = nextExit;
                            break;
                        }
                    }
                }
                if (foundExit != null)
                {
                    DoSingleMove(move, foundExit);
                    return;
                }
                else
                {
                    DoClearCurrentLocation();
                }
            }
            if (move)
            {
                NavigateExitsInBackground(new List<Exit>() { new Exit(null, null, direction) });
            }
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
            return command.Replace("{realm1spell}", _realm1Spell)
                          .Replace("{realm2spell}", _realm2Spell)
                          .Replace("{realm3spell}", _realm3Spell)
                          .Replace("{mob}", _mob)
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
            public Room NavigatedToRoom { get; set; }

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
            public bool ReachedTargetRoom { get; set; }
            public bool Foreground { get; set; }
            public bool HealHitpoints { get; set; }
            public bool CureIfPoisoned { get; set; }
            public bool EnsureBlessed { get; set; }
            public bool EnsureProtected { get; set; }
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
                    lock (_spellsLock)
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
                RefreshEnabledForSingleMoveButtons();
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
                SendCommand(txtOneOffCommand.Text, InputEchoType.On);
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

        private void ctxMob_Opening(object sender, CancelEventArgs e)
        {
            Room r = m_oCurrentRoom;
            if (r == null || string.IsNullOrEmpty(r.Mob1))
            {
                e.Cancel = true;
            }
            else
            {
                tsmiMob1.Text = r.Mob1;
                tsmiMob2.Text = r.Mob2;
                tsmiMob2.Visible = !string.IsNullOrEmpty(r.Mob2);
                tsmiMob3.Text = r.Mob3;
                tsmiMob3.Visible = !string.IsNullOrEmpty(r.Mob3);
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
            if (r == null || !_gameMap.MapGraph.TryGetOutEdges(r, out IEnumerable<Exit> edges))
            {
                e.Cancel = true;
            }
            else
            {
                foreach (Exit nextEdge in edges)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Text = nextEdge.ExitText + ": " + nextEdge.Target.ToString();
                    tsmi.Tag = nextEdge;
                    ctxRoomExits.Items.Add(tsmi);
                }
                ToolStripMenuItem tsmiGraph = new ToolStripMenuItem();
                tsmiGraph.Text = "Graph";
                ctxRoomExits.Items.Add(tsmiGraph);
            }
        }

        private void ctxRoomExits_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip ctx = (ContextMenuStrip)sender;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)e.ClickedItem;
            Exit exit = (Exit)clickedItem.Tag;
            Button sourceButton = (Button)ctx.SourceControl;
            if (sourceButton == btnExitSingleMove)
            {
                DoSingleMove(chkExecuteMove.Checked, exit);
            }
            else //one click strategy button
            {
                List<Exit> exits;
                if (clickedItem.Text == "Graph")
                {
                    GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
                    frmGraph graphForm = new frmGraph(_gameMap, m_oCurrentRoom, true, flying, levitating, isDay, level);
                    bool? result = graphForm.ShowDialog();
                    exits = graphForm.SelectedPath;
                    if (exits == null) return;
                }
                else
                {
                    exits = new List<Exit>() { exit };
                }
                RunStrategy((Strategy)sourceButton.Tag, exits);
            }
        }

        private void chkExecuteMove_CheckedChanged(object sender, EventArgs e)
        {
            RefreshEnabledForSingleMoveButtons();
        }

        private void RefreshEnabledForSingleMoveButtons()
        {
            Room r = m_oCurrentRoom;
            bool haveCurrentRoom = r != null;
            bool enable = chkExecuteMove.Checked || haveCurrentRoom;

            bool n, ne, nw, w, e, s, sw, se, u, d;
            n = ne = nw = w = e = s = sw = se = u = d = false;
            if (haveCurrentRoom)
            {
                if (_gameMap.MapGraph.TryGetOutEdges(r, out IEnumerable<Exit> exits))
                {
                    foreach (Exit nextExit in exits)
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
                        }
                    }
                }
            }
            else
            {
                n = ne = nw = w = e = s = sw = se = u = d = enable;
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
            btnOtherSingleMove.Enabled = enable;
            btnExitSingleMove.Enabled = haveCurrentRoom;
        }

        private void btnExitSingleMove_Click(object sender, EventArgs e)
        {
            ctxRoomExits.Show(btnExitSingleMove, new Point(0, 0));
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
            lock (_spellsLock)
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
            List <Exit> pathExits = MapComputation.ComputeLowestCostPath(fromRoom, targetRoom, _gameMap.MapGraph, flying, levitating, isDay, level);
            if (pathExits == null)
            {
                MessageBox.Show("No path to target room found.");
            }
            return pathExits;
        }

        private void GoToRoom(Room targetRoom)
        {
            List<Exit> exits = CalculateRouteExits(m_oCurrentRoom, targetRoom);
            if (exits != null)
            {
                NavigateExitsInBackground(exits);
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
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmGraph frm = new frmGraph(_gameMap, m_oCurrentRoom, false, flying, levitating, isDay, level);

            frm.ShowDialog(); //do not check that form accepted because current room could be set

            if (m_oCurrentRoom != frm.CurrentRoom)
            {
                SetCurrentRoom(frm.CurrentRoom);
            }
            List<Exit> selectedPath = frm.SelectedPath;
            if (selectedPath != null)
            {
                NavigateExitsInBackground(selectedPath);
            }
        }

        private void btnLocations_Click(object sender, EventArgs e)
        {
            GetGraphInputs(out bool flying, out bool levitating, out bool isDay, out int level);
            frmLocations frm = new frmLocations(_gameMap, m_oCurrentRoom, flying, levitating, isDay, level);

            frm.ShowDialog(); //do not check that form accepted because current room could be set

            if (m_oCurrentRoom != frm.CurrentRoom)
            {
                SetCurrentRoom(frm.CurrentRoom);
            }
            List<Exit> selectedPath = frm.SelectedPath;
            if (selectedPath != null)
            {
                NavigateExitsInBackground(selectedPath);
            }
        }

        private void InitializeRealm(bool doDefault)
        {
            if (doDefault)
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
            List<string> spellList;
            if (_currentRealm == RealmType.Earth)
                spellList = CastOffensiveSpellSequence.EARTH_OFFENSIVE_SPELLS;
            else if (_currentRealm == RealmType.Fire)
                spellList = CastOffensiveSpellSequence.FIRE_OFFENSIVE_SPELLS;
            else if (_currentRealm == RealmType.Water)
                spellList = CastOffensiveSpellSequence.WATER_OFFENSIVE_SPELLS;
            else //wind
                spellList = CastOffensiveSpellSequence.WIND_OFFENSIVE_SPELLS;
            _realm1Spell = spellList[0];
            _realm2Spell = spellList[1];
            _realm3Spell = spellList[2];
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
                _gameMap.SetAlignment(frm.PreferredAlignment);
                _fullColor = sets.FullColor;
                _emptyColor = sets.EmptyColor;

                _currentRealm = frm.CurrentRealm;
                _autoSpellLevelMin = frm.CurrentAutoSpellLevelMin;
                _autoSpellLevelMax = frm.CurrentAutoSpellLevelMax;
                txtWeapon.Text = frm.CurrentWeapon;

                InitializeRealm(false);

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
    }
}
