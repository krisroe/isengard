using Microsoft.VisualBasic;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmMain : Form
    {
        private TcpClient _tcpClient;
        private NetworkStream _tcpClientNetworkStream;
        private VorbisWaveReader _vwr;
        private WaveOutEvent _woe;

        private string _realm1Spell;
        private string _realm2Spell;
        private string _realm3Spell;
        private string _mob;
        private string _weapon;
        private string _wand;

        private bool _isNight;

        private InitializationStep _initializationSteps;
        private InitialLoginInfo _loginInfo;

        private static Color BACK_COLOR_GO = Color.LightGreen;
        private static Color BACK_COLOR_CAUTION = Color.Yellow;
        private static Color BACK_COLOR_STOP = Color.LightSalmon;
        private static Color BACK_COLOR_NEUTRAL = Color.LightGray;

        private static DateTime? _currentStatusLastComputed;
        private static DateTime? _lastPollTick;
        private bool _verboseMode;
        private bool _queryMonsterStatus;
        private bool _finishedQuit;
        private bool _doScore;

        private object _spellsLock = new object();
        private List<string> _spellsCast = new List<string>();
        private bool _refreshSpellsCast = false;
        private HashSet<string> _players = null;

        private object _skillsLock = new object();
        private List<SkillCooldown> _cooldowns = new List<SkillCooldown>();

        private string _username;
        private string _password;
        private bool _promptedUserName;
        private bool _promptedPassword;
        private bool _enteredUserName;
        private bool _enteredPassword;
        private int _totalhp;
        private int _totalmp;
        private int? _previoustickautohp;
        private int? _previoustickautomp;
        private int _healtickmp;
        private int _autoHazyThreshold;
        private DateTime? _lastTriedToAutoHazy;
        private bool _autoHazied;
        private static int? _autoMana;
        private static int? _autoHitpoints;
        private int? _currentMana;
        private IsengardMap _gameMap;
        private Room m_oCurrentRoom;
        private BackgroundWorker _bw;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private BackgroundProcessPhase _backgroundProcessPhase;
        private PleaseWaitSequence _pleaseWaitSequence;

        private object _queuedCommandLock = new object();
        private object _consoleTextLock = new object();
        private object _writeToNetworkStreamLock = new object();
        private object _broadcastMessagesLock = new object();
        private ConsoleOutput _previousConsoleOutput = null;
        private List<ConsoleOutput> _newConsoleText = new List<ConsoleOutput>();
        private Dictionary<char, int> _asciiMapping;
        private List<string> _broadcastMessages = new List<string>();

        private List<EmoteButton> _emoteButtons = new List<EmoteButton>();
        private bool _showingWithTarget = false;
        private bool _showingWithoutTarget = false;
        private bool _fleeing;
        private int _waitSeconds = 0;
        private bool _fumbled;
        private bool _initiatedEmotesTab;
        private bool _initiatedHelpTab;
        private CommandResult? _commandResult;
        private int _commandResultCounter = 0;
        private int _lastCommandDamage;
        private string _lastCommand;
        private BackgroundCommandType? _backgroundCommandType;
        private Exit _currentBackgroundExit;
        private bool _currentBackgroundExitMessageReceived;
        private List<string> _currentObviousExits;
        private List<string> _foundSearchedExits;
        private string _currentlyFightingMob;
        private MonsterStatus? _currentMonsterStatus;
        private int _monsterDamage;
        private const int MAX_ATTEMPTS_FOR_BACKGROUND_COMMAND = 20;
        private List<BackgroundCommandType> _backgroundSpells = new List<BackgroundCommandType>()
        {
            BackgroundCommandType.Vigor,
            BackgroundCommandType.MendWounds,
            BackgroundCommandType.Protection,
            BackgroundCommandType.Bless,
            BackgroundCommandType.Stun,
            BackgroundCommandType.OffensiveSpell
        };

        internal frmMain(string defaultRealm, int level, int totalhp, int totalmp, int healtickmp, AlignmentType preferredAlignment, string userName, string password, List<Macro> allMacros, string defaultWeapon, int autoHazyThreshold, bool autoHazyDefault, bool verboseMode, bool queryMonsterStatus)
        {
            InitializeComponent();

            _pleaseWaitSequence = new PleaseWaitSequence(OnWaitXSeconds);

            SetButtonTags();

            string sFullSoundPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "En-us-full.ogg");
            _vwr = new VorbisWaveReader(sFullSoundPath);
            _woe = new WaveOutEvent();
            _woe.Init(_vwr);

            _asciiMapping = AsciiMapping.GetAsciiMapping();
            _verboseMode = verboseMode;
            _queryMonsterStatus = queryMonsterStatus;

            if (!string.IsNullOrEmpty(defaultRealm))
            {
                RadioButton defaultRealmButton;
                switch (defaultRealm)
                {
                    case "earth":
                        defaultRealmButton = radEarth;
                        break;
                    case "fire":
                        defaultRealmButton = radFire;
                        break;
                    case "water":
                        defaultRealmButton = radWater;
                        break;
                    case "wind":
                        defaultRealmButton = radWind;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                defaultRealmButton.Checked = true;
                SetCurrentRealmButton(defaultRealmButton);
            }

            txtLevel.Text = level.ToString();
            _totalhp = totalhp;
            _totalmp = totalmp;
            _healtickmp = healtickmp;
            _autoHazyThreshold = autoHazyThreshold;
            txtAutoHazyThreshold.Text = _autoHazyThreshold.ToString();
            chkAutoHazy.Checked = autoHazyDefault;

            txtPreferredAlignment.Text = preferredAlignment.ToString();
            txtWeapon.Text = defaultWeapon;

            _username = userName;
            _password = password;

            int iOneClickTabIndex = 0;
            foreach (Macro oMacro in allMacros)
            {
                Button btnOneClick = new Button();
                btnOneClick.AutoSize = true;
                btnOneClick.TabIndex = iOneClickTabIndex++;
                btnOneClick.Tag = oMacro;
                btnOneClick.Text = oMacro.Name;
                btnOneClick.UseVisualStyleBackColor = true;
                btnOneClick.Click += btnOneClick_Click;
                btnOneClick.ContextMenuStrip = ctxRoomExits;
                flpOneClickMacros.Controls.Add(btnOneClick);
            }

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            _gameMap = new IsengardMap(preferredAlignment, level);
            PopulateTree();

            cboSetOption.SelectedIndex = 0;
            cboMaxOffLevel.SelectedIndex = 0;

            DoConnect();
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
                new Emote("stand", "stand up", null), //does not seem to work
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
        /// force a score after a cooldown skill goes off
        /// </summary>
        private void DoScore(FeedLineParameters flParams)
        {
            _doScore = true;
        }

        /// <summary>
        /// handler for the output of score
        /// </summary>
        private void OnScore(FeedLineParameters flParams, List<SkillCooldown> cooldowns, List<string> spells)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Score) == InitializationStep.None;

            bool suppressEcho = forInit;

            lock (_skillsLock)
            {
                _cooldowns.Clear();
                _cooldowns.AddRange(cooldowns);
            }
            lock (_spellsLock)
            {
                _spellsCast.Clear();
                _spellsCast.AddRange(spells);
                _refreshSpellsCast = true;
            }

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

        private void OnTime(FeedLineParameters flParams, bool isNight)
        {
            InitializationStep currentStep = _initializationSteps;
            bool forInit = (currentStep & InitializationStep.Time) == InitializationStep.None;

            _isNight = isNight;

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
            SendCommand("score", InputEchoType.Off);
            SendCommand("who", InputEchoType.Off);
            SendCommand("remove all", InputEchoType.Off);
            SendCommand("time", InputEchoType.Off);
            _initializationSteps |= InitializationStep.Initialization;
            _loginInfo = initialLoginInfo;
        }

        private void ProcessInitialLogin(FeedLineParameters flp)
        {
            InitialLoginInfo info = _loginInfo;
            if (RoomTransitionSequence.ProcessRoom(info.RoomName, info.ObviousExits, info.List1, info.List2, info.List3, OnRoomTransition, flp, RoomTransitionType.Initial))
            {
                _initializationSteps |= InitializationStep.Finalization;
            }
            else
            {
                lock (_broadcastMessagesLock)
                {
                    _broadcastMessages.Add("Initial login failed!");
                }
            }
        }

        private void OnRoomTransition(RoomTransitionInfo roomTransitionInfo)
        {
            RoomTransitionType rtType = roomTransitionInfo.TransitionType;
            string roomName = roomTransitionInfo.RoomName;
            List<string> obviousExits = roomTransitionInfo.ObviousExits;
            FeedLineParameters flParams = roomTransitionInfo.FeedLineParameters;

            BackgroundCommandType? bct = null;
            if (flParams != null)
            {
                bct = flParams.BackgroundCommandType;
            }
            _currentObviousExits = obviousExits;
            if (rtType == RoomTransitionType.Flee)
            {
                if (bct.HasValue && bct.Value == BackgroundCommandType.Flee)
                {
                    flParams.CommandResult = CommandResult.CommandSuccessful;
                    _waitSeconds = 0;
                    _fleeing = false;
                }
            }
            else if (rtType == RoomTransitionType.Hazy)
            {
                _autoHazied = true;
                _fleeing = false;
                if (bct.HasValue) //hazy aborts whatever background command is currently running
                {
                    flParams.CommandResult = CommandResult.CommandUnsuccessfulAlways;
                }
            }
            else if (rtType == RoomTransitionType.Initial)
            {
                //Nothing to do here
            }
            else
            {
                if (bct.HasValue)
                {
                    BackgroundCommandType bctValue = bct.Value;
                    if (bctValue == BackgroundCommandType.Look || bctValue == BackgroundCommandType.Movement)
                    {
                        flParams.CommandResult = CommandResult.CommandSuccessful;
                        _waitSeconds = 0;
                    }
                }
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

        private static void OnSuccessfulManashield(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Manashield)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
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

        private static void OnVigorSpellCast(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Vigor)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OnMendWoundsSpellCast(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.MendWounds)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OnBlessSpellCast(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Bless)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void OnProtectionSpellCast(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Protection)
            {
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private static void FailMovement(FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Movement)
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

        private static void OnStun(FeedLineParameters flParams)
        {
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
        
        private void OnAttack(bool fumbled, int damage, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.Attack)
            {
                if (fumbled)
                {
                    _fumbled = true;
                }
                _lastCommandDamage = damage;
                _monsterDamage += damage;
                flParams.CommandResult = CommandResult.CommandSuccessful;
            }
        }

        private void OnCastOffensiveSpell(int damage, FeedLineParameters flParams)
        {
            BackgroundCommandType? bct = flParams.BackgroundCommandType;
            if (bct.HasValue && bct.Value == BackgroundCommandType.OffensiveSpell)
            {
                _lastCommandDamage = damage;
                _monsterDamage += damage;
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
                            _isNight = false;
                            break;
                        case InformationalMessages.NightStart:
                            _isNight = true;
                            break;
                        case InformationalMessages.BlessOver:
                            if (spellsOff == null) spellsOff = new List<string>();
                            spellsOff.Add("bless");
                            break;
                        case InformationalMessages.ProtectionOver:
                            if (spellsOff == null) spellsOff = new List<string>();
                            spellsOff.Add("protection");
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
            if (addedPlayers != null)
            {
                foreach (string s in addedPlayers)
                {
                    if (_players.Contains(s))
                    {
                        lock (_broadcastMessagesLock)
                        {
                            _broadcastMessages.Add("Player unexpectedly present: " + s);
                        }
                    }
                    else
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
                    else
                    {
                        lock (_broadcastMessagesLock)
                        {
                            _broadcastMessages.Add("Player unexpectedly missing: " + s);
                        }
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
            List<IOutputProcessingSequence> seqs = GetProcessingSequences();
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
                            _autoHitpoints = oii.HP;
                            _autoMana = oii.MP;
                            _currentStatusLastComputed = DateTime.UtcNow;
                        }

                        StringBuilder sb = new StringBuilder();
                        foreach (int nextOutputItemByte in currentOutputItemData)
                        {
                            ProcessInputCharacter(nextOutputItemByte, nextCharacterQueue, sb);
                        }

                        string sNewLine = sb.ToString();
                        string sNewLineRaw = sNewLine;
                        string hpmpStatus = string.Empty;
                        if (oist == OutputItemSequenceType.HPMPStatus) //strip the HP/MP since it was already processed
                        {
                            int lastParenthesisLocation = sNewLine.LastIndexOf('(');
                            if (lastParenthesisLocation == 0)
                            {
                                sNewLine = string.Empty;
                            }
                            else
                            {
                                hpmpStatus = sNewLine.Substring(lastParenthesisLocation);
                                sNewLine = sNewLine.Substring(0, lastParenthesisLocation);
                            }
                        }

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
                            try
                            {
                                foreach (IOutputProcessingSequence nextProcessingSequence in seqs)
                                {
                                    nextProcessingSequence.FeedLine(flParams);
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
                            catch (Exception ex)
                            {
                                lock (_broadcastMessagesLock)
                                {
                                    _broadcastMessages.Add("SPE: " + ex.ToString());
                                }
                            }
                            //recompute the lines if they were changed by sequence logic
                            bool linesChanged = sNewLinesList.Count != initialCount;
                            if (linesChanged)
                            {
                                sNewLine = string.Join(Environment.NewLine, sNewLinesList);
                            }
                            if (!string.IsNullOrEmpty(sNewLine))
                            {
                                if (linesChanged)
                                {
                                    sNewLineRaw = sNewLine + hpmpStatus;
                                }
                                int newCommandResultCounter = _commandResultCounter;
                                if (!_verboseMode && previousCommandResultCounter == newCommandResultCounter && !previousCommandResult.HasValue && flParams.CommandResult.HasValue)
                                {
                                    sNewLineRaw = _lastCommand + Environment.NewLine + sNewLineRaw;
                                }
                                if (flParams.CommandResult.HasValue)
                                {
                                    _commandResult = flParams.CommandResult.Value;
                                }
                            }
                        }

                        if (echoType == InputEchoType.On)
                        {
                            lock (_consoleTextLock)
                            {
                                _newConsoleText.Add(new ConsoleOutput(sNewLineRaw, sNewLine, false));
                            }
                        }

                        currentOutputItemData.Clear();
                        nextCharacterQueue.Clear();
                    }
                }
            }
        }

        private List<IOutputProcessingSequence> GetProcessingSequences()
        {
            List<IOutputProcessingSequence> seqs = new List<IOutputProcessingSequence>
            {
                new InitialLoginSequence(OnInitialLogin),
                new InformationalMessagesSequence(OnInformationalMessages),
                new ScoreOutputSequence(_username, OnScore),
                new WhoOutputSequence(OnWho),
                new RemoveEquipmentSequence(OnRemoveEquipment),
                new MobStatusSequence(OnMobStatusSequence),
                new TimeOutputSequence(OnTime),
                _pleaseWaitSequence,
                new SuccessfulSearchSequence(SuccessfulSearch),
                new RoomTransitionSequence(OnRoomTransition),
                new ConstantOutputSequence("You creative a protective manashield.", OnSuccessfulManashield, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Manashield),
                new ConstantOutputSequence("Your manashield dissipates.", DoScore, ConstantSequenceMatchType.ExactMatch, 0),
                new ConstantOutputSequence("Bless spell cast.", OnBlessSpellCast, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Bless),
                new ConstantOutputSequence("Protection spell cast.", OnProtectionSpellCast, ConstantSequenceMatchType.Contains, 0, BackgroundCommandType.Protection),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains, null), //could be prefixed by "Scared of going X"*
                new ConstantOutputSequence("Vigor spell cast.", OnVigorSpellCast, ConstantSequenceMatchType.Contains, 0),
                new ConstantOutputSequence("Mend-wounds spell cast.", OnMendWoundsSpellCast, ConstantSequenceMatchType.Contains, 0),
                new ConstantOutputSequence("You can't go that way.", FailMovement, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Movement),
                new ConstantOutputSequence("That exit is closed for the night.", FailMovement, ConstantSequenceMatchType.ExactMatch, 0, BackgroundCommandType.Movement),
                new ConstantOutputSequence(" blocks your exit.", FailMovement, ConstantSequenceMatchType.Contains, 0, BackgroundCommandType.Movement),
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

        private const string CAST_VIGOR_SPELL = "cast vigor";
        private const string CAST_MENDWOUNDS_SPELL = "cast mend-wounds";

        private void SetButtonTags()
        {
            btnLevel1OffensiveSpell.Tag = new CommandButtonTag(btnLevel1OffensiveSpell, "cast {realm1spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnLevel2OffensiveSpell.Tag = new CommandButtonTag(btnLevel2OffensiveSpell, "cast {realm2spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnLevel3OffensiveSpell.Tag = new CommandButtonTag(btnLevel3OffensiveSpell, "cast {realm3spell} {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnDrinkHazy.Tag = new CommandButtonTag(btnDrinkHazy, "drink hazy", CommandType.Potions, DependentObjectType.None);
            btnLookAtMob.Tag = new CommandButtonTag(btnLookAtMob, "look {mob}", CommandType.None, DependentObjectType.Mob);
            btnLook.Tag = new CommandButtonTag(btnLook, "look", CommandType.None, DependentObjectType.None);
            btnCastVigor.Tag = new CommandButtonTag(btnCastVigor, CAST_VIGOR_SPELL, CommandType.Magic, DependentObjectType.None);
            btnCastCurePoison.Tag = new CommandButtonTag(btnCastCurePoison, "cast cure-poison", CommandType.Magic, DependentObjectType.None);
            btnAttackMob.Tag = new CommandButtonTag(btnAttackMob, "kill {mob}", CommandType.Melee, DependentObjectType.Mob);
            btnDrinkYellow.Tag = new CommandButtonTag(btnDrinkYellow, "drink yellow", CommandType.Potions, DependentObjectType.None);
            btnDrinkGreen.Tag = new CommandButtonTag(btnDrinkGreen, "drink green", CommandType.Potions, DependentObjectType.None);
            btnWieldWeapon.Tag = new CommandButtonTag(btnWieldWeapon, "wield {weapon}", CommandType.None, DependentObjectType.Weapon);
            btnUseWandOnMob.Tag = new CommandButtonTag(btnUseWandOnMob, "zap {wand} {mob}", CommandType.Magic, DependentObjectType.Wand | DependentObjectType.Mob);
            btnPowerAttackMob.Tag = new CommandButtonTag(btnPowerAttackMob, "power {mob}", CommandType.Melee, DependentObjectType.Mob);
            btnRemoveWeapon.Tag = new CommandButtonTag(btnRemoveWeapon, "remove {weapon}", CommandType.None, DependentObjectType.Weapon);
            btnRemoveAll.Tag = new CommandButtonTag(btnRemoveAll, "remove all", CommandType.None, DependentObjectType.None);
            btnFumbleMob.Tag = new CommandButtonTag(btnFumbleMob, "cast fumble {mob}", CommandType.Magic, DependentObjectType.Mob);
            btnCastMend.Tag = new CommandButtonTag(btnCastMend, CAST_MENDWOUNDS_SPELL, CommandType.Magic, DependentObjectType.None);
            btnReddishOrange.Tag = new CommandButtonTag(btnReddishOrange, "drink reddish-orange", CommandType.Potions, DependentObjectType.None);
            btnStunMob.Tag = new CommandButtonTag(btnStunMob, "cast stun {mob}", CommandType.Magic, DependentObjectType.Mob);
            tsbTime.Tag = new CommandButtonTag(tsbTime, "time", CommandType.None, DependentObjectType.None);
            tsbInformation.Tag = new CommandButtonTag(tsbInformation, "information", CommandType.None, DependentObjectType.None);
            tsbInventory.Tag = new CommandButtonTag(tsbInventory, "inventory", CommandType.None, DependentObjectType.None);
            tsbWho.Tag = new CommandButtonTag(tsbWho, "who", CommandType.None, DependentObjectType.None);
            tsbUptime.Tag = new CommandButtonTag(tsbUptime, "uptime", CommandType.None, DependentObjectType.None);
            tsbEquipment.Tag = new CommandButtonTag(tsbEquipment, "equipment", CommandType.None, DependentObjectType.None);
        }

        private void PopulateTree()
        {
            foreach (Area a in _gameMap.Areas)
            {
                TreeNode tArea = new TreeNode(a.Name);
                tArea.Tag = a;
                treeLocations.Nodes.Add(tArea);
                tArea.Expand();
                a.Locations.Sort(new RoomComparer());
                foreach (Room r in a.Locations)
                {
                    TreeNode tRoom = new TreeNode(r.ToString());
                    tRoom.ContextMenuStrip = ctxLocations;
                    tRoom.Tag = r;
                    tArea.Nodes.Add(tRoom);
                }
            }
        }

        private class RoomComparer : IComparer<Room>
        {
            public int Compare(Room x, Room y)
            {
                int ret;
                if (x.Experience1.HasValue != y.Experience1.HasValue)
                {
                    ret = x.Experience1.HasValue ? 1 : -1;
                }
                else if (!x.Experience1.HasValue && !y.Experience1.HasValue)
                {
                    ret = 0;
                }
                else
                {
                    ret = x.Experience1.Value.CompareTo(y.Experience1.Value);
                }
                return ret;
            }
        }

        private void btnOneClick_Click(object sender, EventArgs e)
        {
            RunMacro((Macro)((Button)sender).Tag, null);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_finishedQuit)
            {
                this.Close();
            }
            else
            {
                _fleeing = false;
                _commandResult = null;
                _lastCommand = null;
                _lastCommandDamage = 0;
                Room wentToRoom = _currentBackgroundParameters.NavigatedToRoom;
                if (wentToRoom != null)
                {
                    SetCurrentRoom(wentToRoom);
                }
                //trigger a foreground asynchronous score (not suppressed from output).
                //This can happen if the background proces was aborted.
                if (_currentBackgroundParameters.DoScore)
                {
                    _doScore = true;
                }
                if (_currentBackgroundParameters.ReachedTargetRoom && !string.IsNullOrEmpty(_currentBackgroundParameters.TargetRoomMob))
                {
                    txtMob.Text = _currentBackgroundParameters.TargetRoomMob;
                }
                _backgroundProcessPhase = BackgroundProcessPhase.None;
                ToggleBackgroundProcess(_currentBackgroundParameters, false);
                _currentBackgroundParameters = null;
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
            txtCurrentRoom.Text = m_oCurrentRoom.Name;
            RefreshEnabledForSingleMoveButtons();
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            Macro m = pms.Macro;
            bool backgroundCommandSuccess;
            CommandResult backgroundCommandResult;

            //Heal
            if (m != null && m.Heal)
            {
                _backgroundProcessPhase = BackgroundProcessPhase.Heal;
                int? autohp, automp;
                while (true)
                {
                    autohp = _autoHitpoints;
                    automp = _autoMana;
                    if (!autohp.HasValue) break;
                    if (autohp.Value >= _totalhp) break;
                    if (!automp.HasValue) break;
                    if (automp.Value < 2) break; //out of mana for vigor cast
                    if (!CastLifeSpell("vigor", pms))
                    {
                        return;
                    }
                    if (_fleeing) break;
                    if (_bw.CancellationPending) break;
                    autohp = _autoHitpoints;
                    if (autohp.Value >= _totalhp) break;
                }
                //stop background processing if failed to get to max hitpoints
                autohp = _autoHitpoints;
                if (autohp.Value < _totalhp) return;

                //cast bless if has enough mana and not currently blessed
                bool hasBless;
                lock (_spellsLock)
                {
                    hasBless = _spellsCast != null && _spellsCast.Contains("bless");
                }
                automp = _autoMana;
                if (automp.HasValue && automp.Value >= 8 && !hasBless)
                {
                    if (!CastLifeSpell("bless", pms))
                    {
                        return;
                    }
                }

                //cast protection if has enough mana and not curently protected
                bool hasProtection;
                lock (_spellsLock)
                {
                    hasProtection = _spellsCast != null && _spellsCast.Contains("protection");
                }
                automp = _autoMana;
                if (automp.HasValue && automp.Value >= 8 && !hasProtection)
                {
                    if (!CastLifeSpell("protection", pms))
                    {
                        return;
                    }
                }
            }

            //Activate skills
            if ((pms.UsedSkills & PromptedSkills.Manashield) == PromptedSkills.Manashield)
            {
                _backgroundProcessPhase = BackgroundProcessPhase.ActivateSkills;
                backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Manashield, "manashield", pms, BeforeFleeCommandAbortLogic, false, false);
                if (backgroundCommandSuccess)
                {
                    pms.DoScore = true;
                }
                else
                {
                    return;
                }
            }

            bool atDestination = false;
            if (pms.Exits != null && pms.Exits.Count > 0)
            {
                _backgroundProcessPhase = BackgroundProcessPhase.Movement;
                foreach (Exit nextExit in pms.Exits)
                {
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
                                    backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Look, "look", pms, BeforeFleeCommandAbortLogic, false);
                                    if (backgroundCommandResult == CommandResult.CommandSuccessful)
                                    {
                                        if (_currentObviousExits.Contains(exitText))
                                        {
                                            foundExit = true;
                                        }
                                        else
                                        {
                                            WaitUntilNextCommand(5000, false, false);
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
                                    backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Search, "search", pms, BeforeFleeCommandAbortLogic, false, false);
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
                                if (_bw.CancellationPending) break;
                                Thread.Sleep(50);
                                RunAutoCommandsWhenMacroRunning(pms, false);
                            }
                        }

                        if (_fleeing) break;
                        if (_bw.CancellationPending) break;

                        if (nextExit.KeyType != KeyType.None)
                        {
                            if (!nextExit.RequiresKey())
                            {
                                backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Knock, "knock " + exitText, pms, BeforeFleeCommandAbortLogic, false, false);
                                if (!backgroundCommandSuccess) return;
                            }
                        }

                        //run preexit logic
                        RunPreExitLogic(nextExit.PreCommand, nextExit.Target);

                        //determine the exit command
                        string nextCommand = GetExitCommand(exitText);

                        backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Movement, nextCommand, pms, BeforeFleeCommandAbortLogic, false, false);
                        if (backgroundCommandSuccess)
                        {
                            if (nextExit.Target != null)
                            {
                                pms.NavigatedToRoom = nextExit.Target;
                                if (!string.IsNullOrEmpty(nextExit.Target.PostMoveCommand))
                                {
                                    SendCommand(nextExit.Target.PostMoveCommand, InputEchoType.On);
                                }
                            }
                        }
                        else
                        {
                            return;
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

            if (pms.Flee)
            {
                _fleeing = true;
            }

            bool haveMeleeSteps = false;
            bool haveMagicSteps = false;
            bool didFlee = false;
            if (m != null)
            {
                haveMagicSteps = m.MagicCombatSteps != null && m.MagicCombatSteps.Count > 0;
                haveMeleeSteps = m.MeleeCombatSteps != null && m.MeleeCombatSteps.Count > 0;
            }
            if (_fleeing || haveMagicSteps || haveMeleeSteps)
            {
                try
                {
                    _currentlyFightingMob = pms.TargetRoomMob;
                    _monsterDamage = 0;
                    _currentMonsterStatus = null;

                    string sWeapon = _weapon;

                    if (haveMagicSteps || haveMeleeSteps)
                    {
                        _backgroundProcessPhase = BackgroundProcessPhase.Combat;
                        bool doPowerAttack = false;
                        if (haveMeleeSteps)
                        {
                            if (!string.IsNullOrEmpty(sWeapon))
                            {
                                SendCommand("wield " + sWeapon, InputEchoType.On);
                            }
                            doPowerAttack = (pms.UsedSkills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
                        }

                        IEnumerator<MagicCombatStep?> magicSteps = m.GetMagicSteps().GetEnumerator();
                        IEnumerator<MeleeCombatStep?> meleeSteps = m.GetMeleeSteps(doPowerAttack).GetEnumerator();
                        MagicCombatStep? nextMagicStep = null;
                        MeleeCombatStep? nextMeleeStep = null;
                        bool magicStepsFinished = !magicSteps.MoveNext();
                        bool meleeStepsFinished = !meleeSteps.MoveNext();
                        bool magicTotallyFinished = false;
                        bool meleeTotallyFinished = false;
                        if (!magicStepsFinished) nextMagicStep = magicSteps.Current;
                        if (!meleeStepsFinished) nextMeleeStep = meleeSteps.Current;
                        DateTime? dtNextMagicCommand = null;
                        DateTime? dtNextMeleeCommand = null;
                        while (true) //combat cycle
                        {
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            bool didDamage = false;
                            string command = null;
                            bool skipMagicStep = false;
                            if (nextMagicStep.HasValue && (!dtNextMagicCommand.HasValue || DateTime.UtcNow > dtNextMagicCommand.Value))
                            {
                                int? currentMana = _currentMana;
                                int? currentHP = _autoHitpoints;
                                int manaDrain = 0;
                                BackgroundCommandType? bct = null;
                                if (nextMagicStep == MagicCombatStep.Stun)
                                {
                                    command = "cast stun " + _currentlyFightingMob;
                                    manaDrain = 10;
                                    bct = BackgroundCommandType.Stun;
                                }
                                else if (nextMagicStep == MagicCombatStep.Vigor)
                                {
                                    if (currentHP.HasValue && currentHP.Value >= _totalhp)
                                    {
                                        skipMagicStep = true;
                                    }
                                    else
                                    {
                                        command = CAST_VIGOR_SPELL;
                                        manaDrain = 2;
                                        bct = BackgroundCommandType.Vigor;
                                    }
                                }
                                else if (nextMagicStep == MagicCombatStep.MendWounds)
                                {
                                    if (currentHP.HasValue && currentHP.Value >= _totalhp)
                                    {
                                        skipMagicStep = true;
                                    }
                                    else
                                    {
                                        command = CAST_MENDWOUNDS_SPELL;
                                        manaDrain = 6;
                                        bct = BackgroundCommandType.MendWounds;
                                    }
                                }
                                else
                                {
                                    if (nextMagicStep == MagicCombatStep.OffensiveSpellAuto)
                                    {
                                        int iMaxOffLevel = pms.MaxOffensiveLevel;
                                        if (currentMana >= 10 && iMaxOffLevel >= 3)
                                        {
                                            nextMagicStep = MagicCombatStep.OffensiveSpellLevel3;
                                        }
                                        else if (currentMana >= 7 && iMaxOffLevel >= 2)
                                        {
                                            nextMagicStep = MagicCombatStep.OffensiveSpellLevel2;
                                        }
                                        else if (currentMana >= 3)
                                        {
                                            nextMagicStep = MagicCombatStep.OffensiveSpellLevel1;
                                        }
                                        else //out of mana
                                        {
                                            nextMagicStep = null;
                                        }
                                    }
                                    if (nextMagicStep.HasValue)
                                    {
                                        if (nextMagicStep == MagicCombatStep.OffensiveSpellLevel3)
                                        {
                                            command = "cast " + _realm3Spell + " " + _currentlyFightingMob;
                                            manaDrain = 10;
                                            bct = BackgroundCommandType.OffensiveSpell;
                                        }
                                        else if (nextMagicStep == MagicCombatStep.OffensiveSpellLevel2)
                                        {
                                            command = "cast " + _realm2Spell + " " + _currentlyFightingMob;
                                            manaDrain = 7;
                                            bct = BackgroundCommandType.OffensiveSpell;
                                        }
                                        else if (nextMagicStep == MagicCombatStep.OffensiveSpellLevel1)
                                        {
                                            command = "cast " + _realm1Spell + " " + _currentlyFightingMob;
                                            manaDrain = 3;
                                            bct = BackgroundCommandType.OffensiveSpell;
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException();
                                        }
                                    }
                                }

                                if (skipMagicStep)
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
                                else
                                {
                                    if (manaDrain > currentMana) //out of mana
                                    {
                                        nextMagicStep = null;
                                    }
                                    if (nextMagicStep.HasValue)
                                    {
                                        backgroundCommandResult = RunSingleCommandForCommandResult(bct.Value, command, pms, BeforeFleeCommandAbortLogic, false);
                                        if (backgroundCommandResult == CommandResult.CommandAborted)
                                        {
                                            return;
                                        }
                                        else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                                        {
                                            magicTotallyFinished = true;
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
                                            if (nextMagicStep.Value != MagicCombatStep.Stun)
                                            {
                                                didDamage = true;
                                            }
                                            if (!pms.AutoMana)
                                            {
                                                _currentMana -= manaDrain;
                                            }
                                            if (_currentMana < 3) //no mana left for casting any more offensive spells
                                            {
                                                nextMagicStep = null;
                                            }
                                            else if (magicStepsFinished)
                                            {
                                                nextMagicStep = null;
                                            }
                                            else if (magicSteps.MoveNext())
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
                                    }
                                }
                            }
                            if (!nextMagicStep.HasValue && m.MagicEnd == CombatStepEnd.Flee)
                            {
                                _fleeing = true;
                            }
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            if (nextMeleeStep.HasValue && (!dtNextMeleeCommand.HasValue || DateTime.UtcNow > dtNextMeleeCommand.Value))
                            {
                                bool isPowerAttack = false;
                                string sAttackType;
                                if (nextMeleeStep == MeleeCombatStep.PowerAttack)
                                {
                                    sAttackType = "power";
                                    isPowerAttack = true;
                                }
                                else if (nextMeleeStep == MeleeCombatStep.RegularAttack)
                                {
                                    sAttackType = "attack";
                                }
                                else
                                {
                                    throw new InvalidOperationException();
                                }
                                command = sAttackType + " " + _mob;
                                backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Attack, command, pms, BeforeFleeCommandAbortLogic, false);
                                if (backgroundCommandResult == CommandResult.CommandAborted)
                                {
                                    return;
                                }
                                else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                                {
                                    meleeTotallyFinished = true;
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
                                        if (isPowerAttack) //refresh power attack cooldown when macro finishes
                                        {
                                            pms.DoScore = true;
                                        }
                                        didDamage = true;
                                    }
                                    if (meleeStepsFinished)
                                    {
                                        nextMeleeStep = null;
                                    }
                                    else if (meleeSteps.MoveNext())
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
                            }
                            if (!nextMeleeStep.HasValue && m.MeleeEnd == CombatStepEnd.Flee)
                            {
                                _fleeing = true;
                            }
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            if (didDamage && _queryMonsterStatus)
                            {
                                backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.LookAtMob, "look " + _mob, pms, BeforeFleeCommandAbortLogic, false);
                                if (backgroundCommandResult == CommandResult.CommandAborted)
                                {
                                    return;
                                }
                                else if (backgroundCommandResult == CommandResult.CommandUnsuccessfulAlways)
                                {
                                    magicTotallyFinished = true;
                                    meleeTotallyFinished = true;
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
                            if (!magicTotallyFinished && !nextMagicStep.HasValue)
                            {
                                MagicCombatStep? queuedMagicStep;
                                lock (_queuedCommandLock)
                                {
                                    queuedMagicStep = pms.QueuedMagicStep;
                                    if (queuedMagicStep.HasValue) pms.QueuedMagicStep = null;
                                }
                                if (queuedMagicStep.HasValue)
                                {
                                    nextMagicStep = queuedMagicStep.Value;
                                }
                            }
                            if (!meleeTotallyFinished && !nextMeleeStep.HasValue)
                            {
                                MeleeCombatStep? queuedMeleeStep;
                                lock (_queuedCommandLock)
                                {
                                    queuedMeleeStep = pms.QueuedMeleeStep;
                                    if (queuedMeleeStep.HasValue) pms.QueuedMeleeStep = null;
                                }
                                if (queuedMeleeStep.HasValue)
                                {
                                    nextMeleeStep = queuedMeleeStep.Value;
                                }
                            }
                            if (!nextMagicStep.HasValue && !nextMeleeStep.HasValue)
                            {
                                break;
                            }
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            RunAutoCommandsWhenMacroRunning(pms, false);
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            Thread.Sleep(50);
                        }
                    }

                    if (_fleeing)
                    {
                        _backgroundProcessPhase = BackgroundProcessPhase.Flee;
                        if (!string.IsNullOrEmpty(sWeapon))
                        {
                            SendCommand("remove " + sWeapon, InputEchoType.On);
                            if (!_fleeing) return;
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
                                RunPreExitLogic(singleFleeableExit.PreCommand, singleFleeableExit.Target);
                            }
                        }

                        backgroundCommandSuccess = RunSingleCommand(BackgroundCommandType.Flee, "flee", pms, null, true, false);
                        if (backgroundCommandSuccess)
                        {
                            didFlee = true;
                            if (singleFleeableExit != null)
                            {
                                pms.NavigatedToRoom = singleFleeableExit.Target;
                            }
                            _fleeing = false;
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
                    _currentMonsterStatus = null;
                }
            }

            if (pms.DoScore && !didFlee)
            {
                _backgroundProcessPhase = BackgroundProcessPhase.Score;
                backgroundCommandResult = RunSingleCommandForCommandResult(BackgroundCommandType.Score, "score", pms, null, false);
                if (backgroundCommandResult != CommandResult.CommandSuccessful)
                {
                    return;
                }
                pms.DoScore = false;
            }

            if (pms.Quit)
            {
                _backgroundProcessPhase = BackgroundProcessPhase.Quit;
                RunSingleCommand(BackgroundCommandType.Quit, "quit", pms, null, false, true);
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

        private void RunPreExitLogic(string preCommand, Room targetRoom)
        {
            if (!string.IsNullOrEmpty(preCommand))
            {
                SendCommand(preCommand, InputEchoType.On);
            }
            if (targetRoom != null && targetRoom.IsTrapRoom)
            {
                SendCommand("prepare", InputEchoType.On);
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
                case "bless":
                    bct = BackgroundCommandType.Bless;
                    break;
                case "protection":
                    bct = BackgroundCommandType.Protection;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            bool successfullyCast = RunSingleCommand(bct, "cast " + spellName, bwp, BeforeFleeCommandAbortLogic, false, false);
            if (successfullyCast)
            {
                if (bct == BackgroundCommandType.Bless || bct == BackgroundCommandType.Protection)
                {
                    bwp.DoScore = true;
                }
            }
            return successfullyCast;
        }

        private void WaitUntilNextCommand(int remainingMS, bool fleeing, bool quitting)
        {
            while (remainingMS > 0)
            {
                int nextWaitMS = Math.Min(remainingMS, 100);
                if (!quitting && fleeing != _fleeing) break;
                if (_bw.CancellationPending) break;
                Thread.Sleep(nextWaitMS);
                remainingMS -= nextWaitMS;
                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, fleeing);
                if (!quitting && fleeing != _fleeing) break;
                if (_bw.CancellationPending) break;
            }
        }

        private void RunAutoCommandsWhenMacroRunning(BackgroundWorkerParameters pms, bool fleeing)
        {
            CheckAutoHazy(pms.AutoHazy, DateTime.UtcNow, _autoHitpoints);

            if (!fleeing && _fumbled)
            {
                string sWeapon = _weapon;
                if (!string.IsNullOrEmpty(sWeapon))
                {
                    SendCommand("wield " + sWeapon, InputEchoType.On);
                }
                _fumbled = false;
            }

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

        private bool BeforeFleeCommandAbortLogic()
        {
            return _fleeing;
        }

        private CommandResult RunSingleCommandForCommandResult(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool fleeing)
        {
            _backgroundCommandType = commandType;
            try
            {
                return RunSingleCommandForCommandResult(command, pms, abortLogic, fleeing);
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

        private CommandResult RunSingleCommandForCommandResult(string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool fleeing)
        {
            _commandResult = null;
            _commandResultCounter++;
            _lastCommand = null;
            _lastCommandDamage = 0;
            try
            {
                _lastCommand = command;
                SendCommand(command, GetHiddenMessageEchoType());
                CommandResult? currentResult;
                while (true)
                {
                    currentResult = _commandResult;
                    if (currentResult.HasValue) break;
                    Thread.Sleep(50);
                    RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, fleeing);
                    if (_bw.CancellationPending) break;
                    if (abortLogic != null && abortLogic()) break;
                }
                return currentResult.GetValueOrDefault(CommandResult.CommandAborted);
            }
            finally
            {
                _commandResult = null;
                _lastCommand = null;
            }
        }

        private bool RunSingleCommand(BackgroundCommandType commandType, string command, BackgroundWorkerParameters pms, Func<bool> abortLogic, bool fleeing, bool quitting)
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
                    result = RunSingleCommandForCommandResult(command, pms, abortLogic, fleeing);
                    if (result.HasValue)
                    {
                        CommandResult resultValue = result.Value;
                        if (resultValue == CommandResult.CommandSuccessful || resultValue == CommandResult.CommandUnsuccessfulAlways || resultValue == CommandResult.CommandAborted)
                        {
                            break;
                        }
                        else if (resultValue == CommandResult.CommandMustWait)
                        {
                            int waitMS = GetWaitMilliseconds(_waitSeconds);
                            if (waitMS > 0)
                            {
                                WaitUntilNextCommand(waitMS, fleeing, quitting);
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

        private void ToggleBackgroundProcess(BackgroundWorkerParameters bwp, bool running)
        {
            bool quitting = bwp.Quit;
            List<Panel> topLevelPanels = new List<Panel>
            {
                pnlMain,
                pnlAncillary
            };
            foreach (Panel p in topLevelPanels)
            {
                foreach (Control ctl in p.Controls)
                {
                    bool regularLogic = false;
                    if (ctl is Button)
                    {
                        if (ctl == btnFlee)
                        {
                            regularLogic = false;
                        }
                        else if (ctl == btnAbort)
                        {
                            ctl.Enabled = running;
                        }
                        else if (ctl.Tag is CommandButtonTag)
                        {
                            regularLogic = false;
                        }
                        else if (ctl.Tag is string)
                        {
                            regularLogic = true;
                        }
                        else
                        {
                            regularLogic = true;
                        }
                    }
                    else if (ctl is TextBox)
                    {
                        regularLogic = true;
                    }
                    else if (ctl != grpLocations && ctl != grpConsole && ctl != chkIsNight)
                    {
                        regularLogic = true;
                    }
                    if (regularLogic)
                    {
                        if (running && quitting)
                        {
                            ctl.Enabled = false;
                        }
                        else
                        {
                            ctl.Enabled = !running;
                        }
                    }
                }
            }
            EnableDisableActionButtons(bwp);
        }

        private void EnableDisableActionButtons(BackgroundWorkerParameters bwp)
        {
            BackgroundProcessPhase npp = _backgroundProcessPhase;
            bool inForeground = bwp == null || npp == BackgroundProcessPhase.None;
            CommandType eRunningCombatCommandTypes = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            if (!inForeground)
            {
                Macro m = _currentBackgroundParameters.Macro;
                if (m != null) eRunningCombatCommandTypes = m.CombatCommandTypes;
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
                else if (oTag.CommandType == CommandType.None)
                    enabled = true;
                else if (npp != BackgroundProcessPhase.Combat) //combat buttons are only enabled in combat
                    enabled = false;
                else //combat buttons are only enabled if the macro isn't doing that kind of combat
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
            else if (npp == BackgroundProcessPhase.Flee || npp == BackgroundProcessPhase.Score || npp == BackgroundProcessPhase.Quit)
                enabled = false;
            else
                enabled = true;
            if (enabled != btnFlee.Enabled)
                btnFlee.Enabled = enabled;

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
            yield return (CommandButtonTag)btnDrinkHazy.Tag;
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
            yield return (CommandButtonTag)btnFumbleMob.Tag;
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
                    _newConsoleText.Add(new ConsoleOutput(sText, sText, true));
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
                NavigateExitsInBackground(exit.Target, new List<Exit>() { exit });
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
                NavigateExitsInBackground(null, new List<Exit>() { new Exit(null, null, direction) });
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
            RunOrQueueMagicStep(sender, MagicCombatStep.OffensiveSpellLevel1);
        }

        private void btnLevel2OffensiveSpell_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicCombatStep.OffensiveSpellLevel2);
        }

        private void btnLevel3OffensiveSpell_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicCombatStep.OffensiveSpellLevel3);
        }

        private void btnStun_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicCombatStep.Stun);
        }

        private void btnVigor_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicCombatStep.Vigor);
        }

        private void btnMendWounds_Click(object sender, EventArgs e)
        {
            RunOrQueueMagicStep(sender, MagicCombatStep.MendWounds);
        }

        private void btnAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeCombatStep.RegularAttack);
        }

        public void btnPowerAttackMob_Click(object sender, EventArgs e)
        {
            RunOrQueueMeleeStep(sender, MeleeCombatStep.PowerAttack);
        }

        private void RunOrQueueMagicStep(object sender, MagicCombatStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                CommandButtonTag cbt = (CommandButtonTag)((Button)sender).Tag;
                RunCommand(TranslateCommand(cbt.Command));
            }
            else
            {
                lock (_queuedCommandLock)
                {
                    bwp.QueuedMagicStep = step;
                }
            }
        }

        private void RunOrQueueMeleeStep(object sender, MeleeCombatStep step)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                CommandButtonTag cbt = (CommandButtonTag)((Button)sender).Tag;
                RunCommand(TranslateCommand(cbt.Command));
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
            txtCurrentRoom.Text = string.Empty;
            RefreshEnabledForSingleMoveButtons();
        }

        private BackgroundWorkerParameters GenerateNewBackgroundParameters()
        {
            BackgroundWorkerParameters ret = new BackgroundWorkerParameters();
            ret.AutoHazy = chkAutoHazy.Checked;
            ret.MaxOffensiveLevel = Convert.ToInt32(cboMaxOffLevel.SelectedItem.ToString());
            ret.AutoMana = chkAutoMana.Checked;
            return ret;
        }

        private void RunCommands(BackgroundWorkerParameters backgroundParameters)
        {
            _currentBackgroundParameters = backgroundParameters;
            _backgroundProcessPhase = BackgroundProcessPhase.Initialization;
            _bw.RunWorkerAsync(backgroundParameters);
            ToggleBackgroundProcess(backgroundParameters, true);
        }

        private class BackgroundWorkerParameters
        {
            public Room NavigatedToRoom { get; set; }

            public List<Exit> Exits { get; set; }
            public bool Cancelled { get; set; }
            public Macro Macro { get; set; }
            public string QueuedCommand { get; set; }
            public MagicCombatStep? QueuedMagicStep { get; set; }
            public MeleeCombatStep? QueuedMeleeStep { get; set; }
            public int MaxOffensiveLevel { get; set; }
            public bool AutoMana { get; set; }
            public PromptedSkills UsedSkills { get; set; }
            public bool AutoHazy { get; set; }
            public bool Flee { get; set; }
            public bool Quit { get; set; }
            public bool DoScore { get; set; }
            public string TargetRoomMob { get; set; }
            public bool ReachedTargetRoom { get; set; }
            public bool Foreground { get; set; }
        }

        private void chkIsNight_CheckedChanged(object sender, EventArgs e)
        {
            _isNight = chkIsNight.Checked;
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

        private void RunMacro(Macro m, List<Exit> preExits)
        {
            CommandType eMacroCombatCommandType = m.CombatCommandTypes;
            bool isMagicMacro = (eMacroCombatCommandType & CommandType.Magic) == CommandType.Magic;
            bool isMeleeMacro = (eMacroCombatCommandType & CommandType.Melee) == CommandType.Melee;
            bool isCombatMacro = isMagicMacro || isMeleeMacro;
            bool hasWeapon = !string.IsNullOrEmpty(txtWeapon.Text);
            if (isMeleeMacro && !hasWeapon)
            {
                MessageBox.Show("No weapon specified.");
                return;
            }

            PromptedSkills activatedSkills = PromptedSkills.None;
            string targetRoomMob = txtMob.Text;

            if (m.ShowPreForm)
            {
                bool promptPowerAttack = isMeleeMacro && txtPowerAttackTime.Text == "0:00";
                bool promptManashield = txtManashieldTime.Text == "0:00";

                PromptedSkills skills = PromptedSkills.None;
                if (promptPowerAttack) skills |= PromptedSkills.PowerAttack;
                if (promptManashield) skills |= PromptedSkills.Manashield;

                Room targetRoom;
                if (preExits == null)
                {
                    targetRoom = m_oCurrentRoom;
                }
                else
                {
                    targetRoom = preExits[preExits.Count - 1].Target;
                }

                using (frmPreMacroPrompt frmSkills = new frmPreMacroPrompt(skills, targetRoom, txtMob.Text, isCombatMacro))
                {
                    if (frmSkills.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                    activatedSkills = frmSkills.SelectedSkills;
                    targetRoomMob = frmSkills.Mob;
                }
            }

            BackgroundWorkerParameters bwp = GenerateNewBackgroundParameters();
            bwp.Macro = m;
            bwp.Exits = preExits;
            bwp.UsedSkills = activatedSkills;
            bwp.TargetRoomMob = targetRoomMob;
            RunCommands(bwp);
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

        [Flags]
        public enum DependentObjectType
        {
            None = 0,
            Mob = 1,
            Weapon = 2,
            Wand = 4,
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            InitializationStep initStep = _initializationSteps;
            int? autohpforthistick = _autoHitpoints;
            int? autompforthistick = _autoMana;
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
                    foreach (ConsoleOutput nextConsoleOutput in _newConsoleText)
                    {
                        bool add = true;
                        if (!nextConsoleOutput.IsInput)
                        {
                            if (_previousConsoleOutput == null || !string.IsNullOrWhiteSpace(nextConsoleOutput.Content) || !string.Equals(_previousConsoleOutput.RawText, nextConsoleOutput.RawText))
                            {
                                _previousConsoleOutput = nextConsoleOutput;
                            }
                            else //suppresses echo for the current output since it was the same as the previous output (e.g. poll ticks)
                            {
                                add = false;
                            }
                        }
                        if (add)
                        {
                            textToAdd.Add(nextConsoleOutput.RawText);
                        }
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
                bool autoMana = chkAutoMana.Checked;
                if (autoMana)
                {
                    _currentMana = autompforthistick;
                }
                else
                {
                    txtMana.BackColor = BACK_COLOR_NEUTRAL;
                }
                if (_currentMana.HasValue)
                {
                    string sText = _currentMana.Value.ToString();
                    if (autoMana)
                    {
                        sText += "/" + _totalmp;
                    }
                    txtMana.Text = sText;
                    if (autoMana)
                    {
                        Color backColor;
                        if (_currentMana == _totalmp)
                            backColor = BACK_COLOR_GO;
                        else if (_currentMana + _healtickmp > _totalmp)
                            backColor = BACK_COLOR_CAUTION;
                        else
                            backColor = BACK_COLOR_STOP;
                        txtMana.BackColor = backColor;
                    }
                }
                if (autohpforthistick.HasValue)
                {
                    int autohpvalue = autohpforthistick.Value;
                    txtHitpoints.Text = autohpvalue.ToString() + "/" + _totalhp;
                    Color backColor;
                    if (autohpvalue == _totalhp)
                        backColor = BACK_COLOR_GO;
                    else
                        backColor = BACK_COLOR_STOP;
                    txtHitpoints.BackColor = backColor;
                }

                //refresh cooldowns (active and timers)
                List<SkillCooldown> cooldowns = new List<SkillCooldown>();
                lock (_skillsLock)
                {
                    cooldowns.AddRange(_cooldowns);
                }
                foreach (SkillCooldown nextCooldown in cooldowns)
                {
                    SkillWithCooldownType eType = nextCooldown.SkillType;
                    DateTime dtUTCNow = DateTime.UtcNow;
                    TextBox txt;
                    switch (eType)
                    {
                        case SkillWithCooldownType.PowerAttack:
                            txt = txtPowerAttackTime;
                            break;
                        case SkillWithCooldownType.Manashield:
                            txt = txtManashieldTime;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    string sText;
                    Color backColor;
                    if (nextCooldown.Active)
                    {
                        sText = "ACTIVE";
                        backColor = BACK_COLOR_GO;
                    }
                    else //not currently active
                    {
                        DateTime? dtNextAvailable = nextCooldown.NextAvailable;
                        if (dtNextAvailable.HasValue)
                        {
                            DateTime dtDateValue = nextCooldown.NextAvailable.Value;
                            if (dtUTCNow >= dtDateValue)
                            {
                                sText = "0:00";
                            }
                            else
                            {
                                TimeSpan ts = dtDateValue - dtUTCNow;
                                sText = ts.Minutes + ":" + ts.Seconds.ToString().PadLeft(2, '0');
                            }
                            backColor = sText == "0:00" ? BACK_COLOR_GO : BACK_COLOR_STOP;
                        }
                        else //available now
                        {
                            sText = "0:00";
                            backColor = BACK_COLOR_GO;
                        }
                    }
                    if (!string.Equals(sText, txt.Text))
                    {
                        txt.Text = sText;
                    }
                    if (backColor != txt.BackColor)
                    {
                        txt.BackColor = backColor;
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
                if (chkIsNight.Checked != _isNight)
                {
                    chkIsNight.Checked = _isNight;
                }
            }
            if (autohpforthistick.HasValue && autompforthistick.HasValue && autohpforthistick.Value == _totalhp && autompforthistick.Value == _totalmp &&
                ((_previoustickautohp.HasValue && _previoustickautohp.Value != autohpforthistick.Value) ||
                (_previoustickautomp.HasValue && _previoustickautomp.Value != autompforthistick.Value)))
            {
                _woe.Play();
            }
            string sMonster = _currentlyFightingMob;
            int iMonsterDamage = _monsterDamage;
            MonsterStatus? monsterStatus = _currentMonsterStatus;
            string sCurrentMobGroupText = grpMob.Text;
            string sCurrentMobStatusText = txtMobStatus.Text;
            string sCurrentMobDamageText = txtMobDamage.Text;
            string sNewGroupMobText;
            string sNewMobStatusText;
            string sNewMobDamageText = sCurrentMobDamageText;
            if (string.IsNullOrEmpty(sMonster))
            {
                sNewGroupMobText = "Mob";
                sNewMobStatusText = string.Empty;
            }
            else
            {
                sNewGroupMobText = sMonster;
                sNewMobStatusText = GetMonsterStatusText(monsterStatus);
                sNewMobDamageText = iMonsterDamage.ToString();
            }
            if (!string.Equals(sCurrentMobGroupText, sNewGroupMobText))
            {
                grpMob.Text = sNewGroupMobText;
            }
            if (!string.Equals(sCurrentMobStatusText, sNewMobStatusText))
            {
                txtMobStatus.Text = sNewMobStatusText;
            }
            if (!string.Equals(sCurrentMobDamageText, sNewMobDamageText))
            {
                txtMobDamage.Text = sNewMobDamageText;
            }
            string sMob = _mob ?? string.Empty;
            if (!string.Equals(sMob, txtMob.Text))
            {
                txtMob.Text = sMob;
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
            EnableDisableActionButtons(_currentBackgroundParameters);
            if (!btnAbort.Enabled)
            {
                DateTime dtUtcNow = DateTime.UtcNow;

                if (_autoHazied)
                {
                    chkAutoHazy.Checked = false;
                    _lastTriedToAutoHazy = null;
                    _autoHazied = false;
                    if (_currentBackgroundParameters != null)
                    {
                        _bw.CancelAsync();
                        _currentBackgroundParameters.AutoHazy = false;
                    }
                    SetCurrentRoom(_gameMap.TreeOfLifeRoom);
                }
                else
                {
                    CheckAutoHazy(chkAutoHazy.Checked, dtUtcNow, autohpforthistick);
                }

                //check for poll tick if a macro is not running and the first status update has completed and not at full HP+MP
                if (_currentStatusLastComputed.HasValue && (!autohpforthistick.HasValue || autohpforthistick.Value < _totalhp || !autompforthistick.HasValue || autompforthistick.Value < _totalmp))
                {
                    bool runPollTick = (dtUtcNow - _currentStatusLastComputed.Value).TotalSeconds >= 5;
                    if (runPollTick && _lastPollTick.HasValue)
                    {
                        runPollTick = (dtUtcNow - _lastPollTick.Value).TotalSeconds >= 5;
                    }
                    if (runPollTick)
                    {
                        _lastPollTick = dtUtcNow;
                        SendCommand(string.Empty, InputEchoType.On);
                    }
                }
                if (_doScore)
                {
                    _doScore = false;
                    SendCommand("score", InputEchoType.On);
                }
            }
            _previoustickautohp = autohpforthistick;
            _previoustickautomp = autompforthistick;
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

        private void CheckAutoHazy(bool AutoHazyActive, DateTime dtUtcNow, int? autoHitpoints)
        {
            if (m_oCurrentRoom != _gameMap.TreeOfLifeRoom && !_autoHazied && AutoHazyActive && autoHitpoints.HasValue && autoHitpoints.Value < _autoHazyThreshold && (!_lastTriedToAutoHazy.HasValue || ((dtUtcNow - _lastTriedToAutoHazy.Value) > new TimeSpan(0, 0, 2))))
            {
                _lastTriedToAutoHazy = dtUtcNow;
                SendCommand("drink hazy", InputEchoType.On);
            }
        }

        private void btnManaSet_Click(object sender, EventArgs e)
        {
            string sNewMana = Interaction.InputBox("New mana:", "Mana", txtMana.Text);
            if (int.TryParse(sNewMana, out int iNewMana))
            {
                chkAutoMana.Checked = false;
                _currentMana = iNewMana;
            }
        }

        private void btnSetAutoHazyThreshold_Click(object sender, EventArgs e)
        {
            string sNewAutoHazyThreshold = Interaction.InputBox("New auto hazy threshold:", "Auto Hazy Threshold", txtAutoHazyThreshold.Text);
            if (int.TryParse(sNewAutoHazyThreshold, out int iNewAutoHazyThreshold) && iNewAutoHazyThreshold > 0 && iNewAutoHazyThreshold <= _totalmp)
            {
                _autoHazyThreshold = iNewAutoHazyThreshold;
                txtAutoHazyThreshold.Text = _autoHazyThreshold.ToString();
            }
            else
            {
                MessageBox.Show("Invalid auto hazy threshold: " + sNewAutoHazyThreshold);
            }
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
                bwp = GenerateNewBackgroundParameters();
                bwp.Flee = true;
                RunCommands(bwp);
            }
            else
            {
                _fleeing = true;
            }
        }

        private void btnScore_Click(object sender, EventArgs e)
        {
            BackgroundWorkerParameters bwp = _currentBackgroundParameters;
            if (bwp == null)
            {
                bwp = GenerateNewBackgroundParameters();
                bwp.DoScore = true;
                bwp.Foreground = true;
                RunCommands(bwp);
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
                bwp = GenerateNewBackgroundParameters();
                bwp.Quit = true;
                RunCommands(bwp);
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
            else //one click macro button
            {
                List<Exit> exits;
                if (clickedItem.Text == "Graph")
                {
                    frmGraph graphForm = new frmGraph(_gameMap, m_oCurrentRoom, true);
                    bool? result = graphForm.ShowDialog();
                    if (!result.GetValueOrDefault(false))
                    {
                        return;
                    }
                    exits = CalculateRouteExits(graphForm.GoToOrSelectRoom);
                    if (exits == null) return;
                }
                else
                {
                    exits = new List<Exit>() { exit };
                }
                Macro m = (Macro)sourceButton.Tag;
                RunMacro(m, exits);
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

        private void ctxLocations_Opening(object sender, CancelEventArgs e)
        {
            bool cancel = false;
            if (btnAbort.Enabled)
            {
                cancel = true;
            }
            else
            {
                TreeNode node = treeLocations.SelectedNode;
                if (node == null)
                {
                    cancel = true;
                }
                else
                {
                    Room currentRoom = m_oCurrentRoom;
                    Room r = node.Tag as Room;
                    if (r == null || currentRoom == r)
                    {
                        cancel = true;
                    }
                    else
                    {
                        tsmiGoToLocation.Visible = currentRoom != null;
                    }
                }
            }
            if (cancel)
            {
                e.Cancel = true;
            }
        }

        private void treeLocations_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeLocations.SelectedNode = e.Node;
            }
        }

        private void ctxLocations_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsi = e.ClickedItem;
            Room targetRoom = (Room)treeLocations.SelectedNode.Tag;
            if (tsi == tsmiSetLocation)
            {
                SetCurrentRoom(targetRoom);
            }
            else if (tsi == tsmiGoToLocation)
            {
                GoToRoom(targetRoom);
            }
        }

        private List<Exit> CalculateRouteExits(Room targetRoom)
        {
            bool flying = false;
            lock (_spellsLock)
            {
                flying = _spellsCast != null && _spellsCast.Contains("fly");
            }
            List <Exit> pathExits = MapComputation.ComputeLowestCostPath(m_oCurrentRoom, targetRoom, _gameMap.MapGraph, flying, !_isNight);
            if (pathExits == null)
            {
                MessageBox.Show("No path to target room found.");
            }
            return pathExits;
        }

        private void GoToRoom(Room targetRoom)
        {
            List<Exit> exits = CalculateRouteExits(targetRoom);
            if (exits != null)
            {
                NavigateExitsInBackground(targetRoom, exits);
            }
        }

        private void NavigateExitsInBackground(Room targetRoom, List<Exit> exits)
        {
            BackgroundWorkerParameters bwp = GenerateNewBackgroundParameters();
            bwp.Exits = exits;
            RunCommands(bwp);
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            frmGraph frm = new frmGraph(_gameMap, m_oCurrentRoom, false);
            frm.ShowDialog();
            if (m_oCurrentRoom != frm.CurrentRoom)
            {
                SetCurrentRoom(frm.CurrentRoom);
            }
            if (frm.GoToOrSelectRoom != null)
            {
                GoToRoom(frm.GoToOrSelectRoom);
            }
        }

        internal class ConsoleOutput
        {
            public ConsoleOutput(string RawText, string Content, bool IsInput)
            {
                this.RawText = RawText;
                this.IsInput = IsInput;
                this.Content = Content;
            }
            public bool IsInput { get; set; }
            /// <summary>
            /// raw content of the output, which is what is displayed in the console
            /// </summary>
            public string RawText { get; set; }
            /// <summary>
            /// content of the output (not including HP/MP status)
            /// </summary>
            public string Content { get; set; }
        }

        private void radRealm_CheckedChanged(object sender, System.EventArgs e)
        {
            RadioButton radRealm = (RadioButton)sender;
            if (radRealm.Checked)
            {
                SetCurrentRealmButton(radRealm);
            }
        }

        private void SetCurrentRealmButton(RadioButton radRealm)
        {
            if (radRealm == radEarth)
            {
                _realm1Spell = "rumble";
                _realm2Spell = "crush";
                _realm3Spell = "shatterstone";
            }
            else if (radRealm == radFire)
            {
                _realm1Spell = "burn";
                _realm2Spell = "fireball";
                _realm3Spell = "burstflame";
            }
            else if (radRealm == radWater)
            {
                _realm1Spell = "blister";
                _realm2Spell = "waterbolt";
                _realm3Spell = "steamblast";
            }
            else if (radRealm == radWind)
            {
                _realm1Spell = "hurt";
                _realm2Spell = "dustgust";
                _realm3Spell = "shockbolt";
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }

    [Flags]
    internal enum PromptedSkills
    {
        None = 0,
        PowerAttack = 1,
        Manashield = 2
    }

    public enum CommandResult
    {
        CommandSuccessful,
        CommandUnsuccessfulThisTime,
        CommandUnsuccessfulAlways,
        CommandMustWait,
        CommandAborted,
    }

    public enum BackgroundCommandType
    {
        Movement,
        Look,
        LookAtMob,
        Search,
        Knock,
        Vigor,
        MendWounds,
        Bless,
        Protection,
        Manashield,
        Stun,
        OffensiveSpell,
        Attack,
        Flee,
        Score,
        Quit,
    }

    internal enum MonsterStatus
    {
        ExcellentCondition,
        FewSmallScratches,
        WincingInPain,
        SlightlyBruisedAndBattered,
        SomeMinorWounds,
        BleedingProfusely,
        NastyAndGapingWound,
        ManyGreviousWounds,
        MortallyWounded,
        BarelyClingingToLife,
    }

    internal enum InputEchoType
    {
        On,
        OnPassword,
        Off,
    }

    internal enum BackgroundProcessPhase
    {
        None,
        Initialization,
        Heal,
        ActivateSkills,
        Movement,
        Combat,
        Flee,
        Score,
        Quit,
    }

    [Flags]
    public enum CommandType
    {
        None = 0,
        Melee = 1,
        Magic = 2,
        Potions = 4,
    }

    [Flags]
    internal enum InitializationStep
    {
        None = 0,
        Initialization = 1,
        RemoveAll = 2,
        Score = 4,
        Time = 8,
        Who = 16,
        BeforeFinalization = 31,
        Finalization = 32,
        All = 63,
    }
}
