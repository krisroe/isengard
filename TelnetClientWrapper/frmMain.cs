using Microsoft.VisualBasic;
using NAudio.Vorbis;
using NAudio.Wave;
using QuickGraph;
using QuickGraph.Algorithms.Search;
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

        /// <summary>
        /// preferred alignment type for the player. Choose mobs that will push the player in this direction.
        /// </summary>
        private AlignmentType _preferredAlignment;

        private static Color BACK_COLOR_GO = Color.LightGreen;
        private static Color BACK_COLOR_CAUTION = Color.Yellow;
        private static Color BACK_COLOR_STOP = Color.LightSalmon;
        private static Color BACK_COLOR_NEUTRAL = Color.LightGray;
        private static DateTime? _currentStatusLastComputed;
        private static DateTime? _lastPollTick;
        private bool? _setDay;
        private bool _finishedQuit;
        private List<string> _currentSpellsCast;
        private List<string> _newSpellsCast;
        private bool _doScore;
        private static Dictionary<SkillWithCooldownType, SkillCooldownStatus> _skillCooldowns;
        private string _username;
        private string _password;
        private bool _promptedUserName;
        private bool _promptedPassword;
        private bool _enteredUserName;
        private bool _enteredPassword;
        private int _level;
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
        private List<string> _startupCommands;
        private bool _ranStartupCommands;
        private AdjacencyGraph<Room, Exit> _map;
        private Room m_oCurrentRoom;
        private BreadthFirstSearchAlgorithm<Room, Exit> _currentSearch;
        private Dictionary<Room, Exit> _pathMapping;
        private BackgroundWorker _bw;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private List<Area> _areas;
        private Dictionary<string, Area> _areasByName;
        private List<Variable> _variables;
        private Dictionary<string, Variable> _variablesByName;

        private List<Exit> _nightEdges = new List<Exit>();
        private List<Exit> _celduinExpressEdges = new List<Exit>();

        private Room _breeEastGateInside = null;
        private Room _breeEastGateOutside = null;
        private Room _imladrisWestGateInside = null;
        private Room _imladrisWestGateOutside = null;
        private Room _breeDocks = null;
        private Room _boatswain = null;
        private Room _treeOfLife = null;
        private object _queuedCommandLock = new object();
        private object _consoleTextLock = new object();
        private object _writeToNetworkStreamLock = new object();
        private List<string> _newConsoleText = new List<string>();
        private Area _aBreePerms;
        private Area _aImladrisTharbadPerms;
        private Area _aShips;
        private Area _aMisc;
        private Area _aInaccessible;
        private Dictionary<char, int> _asciiMapping;
        private List<RoomGraph> _graphs;
        private RoomGraph _breeStreetsGraph;

        private const string AREA_BREE_PERMS = "Bree Perms";
        private const string AREA_IMLADRIS_THARBAD_PERMS = "Imladris/Tharbad Perms";
        private const string AREA_BREE = "Bree";
        private const string AREA_MISC = "Misc";
        private const string AREA_SHIPS = "Ships";
        private const string AREA_INTANGIBLE = "Intangible";
        private const string AREA_INACCESSIBLE = "Inaccessible";

        private const string VARIABLE_MOVEGAPMS = "movegapms";

        private List<EmoteButton> _emoteButtons = new List<EmoteButton>();
        private bool _showingWithTarget = false;
        private bool _showingWithoutTarget = false;
        private bool _fleeing;
        private bool? _manashieldResult;
        private int _waitSeconds = 0;
        private bool _fumbled;
        private bool _initiatedEmotesTab;
        private bool _initiatedHelpTab;

        /// <summary>
        /// result for the flee or quit commands
        /// </summary>
        private bool? _commandResult;

        internal frmMain(List<Variable> variables, Dictionary<string, Variable> variablesByName, string defaultRealm, int level, int totalhp, int totalmp, int healtickmp, AlignmentType preferredAlignment, string userName, string password, List<Macro> allMacros, List<string> startupCommands, string defaultWeapon, int autoHazyThreshold, bool autoHazyDefault)
        {
            InitializeComponent();

            string sFullSoundPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "En-us-full.ogg");
            _vwr = new VorbisWaveReader(sFullSoundPath);
            _woe = new WaveOutEvent();
            _woe.Init(_vwr);

            _asciiMapping = AsciiMapping.GetAsciiMapping();
            _graphs = new List<RoomGraph>();

            _variables = variables;
            _variablesByName = variablesByName;
            _startupCommands = startupCommands;

            _skillCooldowns = new Dictionary<SkillWithCooldownType, SkillCooldownStatus>();
            _skillCooldowns[SkillWithCooldownType.PowerAttack] = new SkillCooldownStatus();
            _skillCooldowns[SkillWithCooldownType.Manashield] = new SkillCooldownStatus();

            if (!string.IsNullOrEmpty(defaultRealm))
            {
                switch (defaultRealm)
                {
                    case "earth":
                        radEarth.Checked = true;
                        break;
                    case "fire":
                        radFire.Checked = true;
                        break;
                    case "water":
                        radWater.Checked = true;
                        break;
                    case "wind":
                        radWind.Checked = true;
                        break;
                }
            }

            _level = level;
            txtLevel.Text = _level.ToString();
            _totalhp = totalhp;
            _totalmp = totalmp;
            _healtickmp = healtickmp;
            _autoHazyThreshold = autoHazyThreshold;
            txtAutoHazyThreshold.Text = _autoHazyThreshold.ToString();
            chkAutoHazy.Checked = autoHazyDefault;

            _preferredAlignment = preferredAlignment;
            txtPreferredAlignment.Text = _preferredAlignment.ToString();
            txtWeapon.Text = defaultWeapon;

            _username = userName;
            _password = password;

            cboMacros.Items.Add(string.Empty);
            int iOneClickTabIndex = 0;
            foreach (Macro oMacro in allMacros)
            {
                if (oMacro.OneClick)
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
                else
                {
                    cboMacros.Items.Add(oMacro);
                }
            }

            _areas = new List<Area>();
            _areasByName = new Dictionary<string, Area>();
            _aBreePerms = AddArea(AREA_BREE_PERMS);
            _aImladrisTharbadPerms = AddArea(AREA_IMLADRIS_THARBAD_PERMS);
            AddArea(AREA_BREE);
            _aMisc = AddArea(AREA_MISC);
            _aShips = AddArea(AREA_SHIPS);
            AddArea(AREA_INTANGIBLE);
            _aInaccessible = AddArea(AREA_INACCESSIBLE);

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            SetButtonTags();
            InitializeMap();
            SetNightEdges(false);
            PopulateTree();

            cboSetOption.SelectedIndex = 0;
            cboCelduinExpress.SelectedIndex = 0;
            cboMaxOffLevel.SelectedIndex = 0;

            foreach (RoomGraph g in _graphs)
            {
                var oldRooms = g.Rooms;
                g.Rooms = new Dictionary<Room, System.Windows.Point>();
                foreach (KeyValuePair<Room, System.Windows.Point> next in oldRooms)
                {
                    g.Rooms[next.Key] = new System.Windows.Point(next.Value.X * g.ScalingFactor, next.Value.Y * g.ScalingFactor);
                }
            }

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
            SendCommand("help " + btn.Text, false);
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
            SendCommand(command, false);
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
            ClearConsole();
            _finishedQuit = false;
            _currentStatusLastComputed = null;
            _ranStartupCommands = false;
            _promptedUserName = false;
            _promptedPassword = false;
            _enteredUserName = false;
            _enteredPassword = false;

            _tcpClient = new TcpClient("isengard.nazgul.com", 4040);
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

        private class SkillCooldownStatus
        {
            public bool IsActive { get; set; }
            public DateTime? NextAvailableDate { get; set; }
        }

        private void DoScore()
        {
            _doScore = true;
        }

        private void OnGetSkillCooldown(SkillWithCooldownType skillWithCooldownType, bool isActive, DateTime? nextAvailableDate)
        {
            SkillCooldownStatus oStatus = _skillCooldowns[skillWithCooldownType];
            oStatus.IsActive = isActive;
            oStatus.NextAvailableDate = nextAvailableDate;
        }

        private void OnNight()
        {
            _setDay = false;
        }

        private void OnDay()
        {
            _setDay = true;
        }

        private void OnSpellsCastChange(List<string> spells)
        {
            _newSpellsCast = spells;
        }

        private void OnHazy()
        {
            _autoHazied = true;
            _fleeing = false;
        }

        private void OnFailFlee()
        {
            _commandResult = false;
            _waitSeconds = 0;
        }

        private void OnSuccessfulFlee()
        {
            _fleeing = false;
            _commandResult = true;
        }

        private void OnFailManashield()
        {
            _manashieldResult = false;
        }

        private void OnSuccessfulManashield()
        {
            _manashieldResult = true;
        }

        private void OnWaitXSeconds(int waitSeconds)
        {
            _commandResult = false;
            _waitSeconds = waitSeconds;
        }

        private void OnFumbled()
        {
            _fumbled = true;
        }

        private void OnLifeSpellCast()
        {
            _commandResult = true;
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
            List<IOutputProcessingSequence> outputProcessingSequences = new List<IOutputProcessingSequence>()
            {
                new SkillCooldownSequence(SkillWithCooldownType.PowerAttack, OnGetSkillCooldown),
                new SkillCooldownSequence(SkillWithCooldownType.Manashield, OnGetSkillCooldown),
                new ConstantOutputSequence("You creative a protective manashield.", OnSuccessfulManashield),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield),
                new ConstantOutputSequence("Your manashield dissipates.", DoScore),
                new ConstantOutputSequence("The sun disappears over the horizon.", OnNight),
                new ConstantOutputSequence("The sun rises.", OnDay),
                new SpellsCastSequence(OnSpellsCastChange),
                new ConstantOutputSequence("You feel less protected.", DoScore),
                new ConstantOutputSequence("You feel less holy.", DoScore),
                new ConstantOutputSequence("Bless spell cast.", OnLifeSpellCast),
                new ConstantOutputSequence("Protection spell cast.", OnLifeSpellCast),
                new ConstantOutputSequence("You phase in and out of existence.", OnHazy),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee),
                new ConstantOutputSequence("You run like a chicken.", OnSuccessfulFlee),
                new PleaseWaitXSecondsSequence(OnWaitXSeconds),
                new ConstantOutputSequence("You FUMBLED your weapon.", OnFumbled),
                new ConstantOutputSequence("Vigor spell cast.", OnLifeSpellCast),
            };

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
                        foreach (IOutputProcessingSequence nextProcessingSequence in outputProcessingSequences)
                        {
                            nextProcessingSequence.FeedLine(sNewLine);
                        }

                        lock (_consoleTextLock)
                        {
                            _newConsoleText.Add(sNewLine);
                        }

                        currentOutputItemData.Clear();
                        nextCharacterQueue.Clear();
                    }
                }
            }
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
                    case 170:
                        isUnknown = true;
                        break;
                    case 173:
                        isUnknown = true;
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
            btnLevel1OffensiveSpell.Tag = new CommandButtonTag("cast {realm1spell} {mob}", CommandType.Magic);
            btnLevel2OffensiveSpell.Tag = new CommandButtonTag("cast {realm2spell} {mob}", CommandType.Magic);
            btnLevel3OffensiveSpell.Tag = new CommandButtonTag("cast {realm3spell} {mob}", CommandType.Magic);
            btnDrinkHazy.Tag = new CommandButtonTag("drink hazy", CommandType.Potions);
            btnLookAtMob.Tag = new CommandButtonTag("look {mob}", CommandType.None);
            btnLook.Tag = new CommandButtonTag("look", CommandType.None);
            btnCastVigor.Tag = new CommandButtonTag("cast vigor", CommandType.Magic);
            btnManashield.Tag = new CommandButtonTag("manashield", CommandType.Magic);
            btnCastCurePoison.Tag = new CommandButtonTag("cast cure-poison", CommandType.Magic);
            btnTime.Tag = new CommandButtonTag("time", CommandType.None);
            btnScore.Tag = new CommandButtonTag("score", CommandType.None);
            btnInformation.Tag = new CommandButtonTag("information", CommandType.None);
            btnCastProtection.Tag = new CommandButtonTag("cast protection", CommandType.Magic);
            btnInventory.Tag = new CommandButtonTag("inventory", CommandType.None);
            btnAttackMob.Tag = new CommandButtonTag("kill {mob}", CommandType.Melee);
            btnDrinkYellow.Tag = new CommandButtonTag("drink yellow", CommandType.Potions);
            btnDrinkGreen.Tag = new CommandButtonTag("drink green", CommandType.Potions);
            btnWieldWeapon.Tag = new CommandButtonTag("wield {weapon}", CommandType.None);
            btnCastBless.Tag = new CommandButtonTag("cast bless", CommandType.Magic);
            btnUseWandOnMob.Tag = new CommandButtonTag("zap {wand} {mob}", CommandType.Magic);
            btnWho.Tag = new CommandButtonTag("who", CommandType.None);
            btnUptime.Tag = new CommandButtonTag("uptime", CommandType.None);
            btnEquipment.Tag = new CommandButtonTag("equipment", CommandType.None);
            btnPowerAttackMob.Tag = new CommandButtonTag("power {mob}", CommandType.Melee);
            btnRemoveWeapon.Tag = new CommandButtonTag("remove {weapon}", CommandType.None);
            btnRemoveAll.Tag = new CommandButtonTag("remove all", CommandType.None);
            btnFumbleMob.Tag = new CommandButtonTag("cast fumble {mob}", CommandType.Magic);
            btnCastMend.Tag = new CommandButtonTag("cast mend-wounds", CommandType.Magic);
            btnReddishOrange.Tag = new CommandButtonTag("drink reddish-orange", CommandType.Potions);
            btnStunMob.Tag = new CommandButtonTag("cast stun {mob}", CommandType.Magic);
        }

        private Area AddArea(string areaName)
        {
            Area a = new Area(areaName);
            _areas.Add(a);
            _areasByName[a.Name] = a;
            return a;
        }

        private void PopulateTree()
        {
            foreach (Area a in _areas)
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
                    PopulateSubLocations(tRoom, r);
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

        private void PopulateSubLocations(TreeNode t, Room r)
        {
            if (r.SubLocations != null)
            {
                foreach (var nextSubLocation in r.SubLocations)
                {
                    TreeNode tSub = new TreeNode(nextSubLocation.Name);
                    tSub.Tag = nextSubLocation;
                    t.Nodes.Add(tSub);
                    PopulateSubLocations(tSub, nextSubLocation);
                }
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
                if (!string.IsNullOrEmpty(_currentBackgroundParameters.FinalCommand))
                {
                    SendCommand(_currentBackgroundParameters.FinalCommand, false);
                    _currentBackgroundParameters.CommandsRun++;
                }
                if (!string.IsNullOrEmpty(_currentBackgroundParameters.FinalCommand2))
                {
                    SendCommand(_currentBackgroundParameters.FinalCommand2, false);
                    _currentBackgroundParameters.CommandsRun++;
                }
                if ((_currentBackgroundParameters.SetTargetRoomIfCancelled || !_currentBackgroundParameters.Cancelled) && _currentBackgroundParameters.CommandsRun > 0)
                {
                    Room targetRoom = _currentBackgroundParameters.TargetRoom;
                    if (targetRoom != null)
                    {
                        SetCurrentRoom(targetRoom);
                    }
                }
                if (_currentBackgroundParameters.DoScore && _currentBackgroundParameters.CommandsRun > 0)
                {
                    _doScore = true;
                }
                ToggleBackgroundProcess(false);
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
            if (r.VariableValues != null)
            {
                foreach (KeyValuePair<Variable, string> next in r.VariableValues)
                {
                    switch (next.Key.Type)
                    {
                        case VariableType.Bool:
                            ((BooleanVariable)next.Key).Value = bool.Parse(next.Value);
                            break;
                        case VariableType.Int:
                            ((IntegerVariable)next.Key).Value = int.Parse(next.Value);
                            break;
                        case VariableType.String:
                            ((StringVariable)next.Key).Value = next.Value;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
            RefreshEnabledForSingleMoveButtons();
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            Macro m = pms.Macro;
            List<MacroStepBase> commands = pms.Commands;
            MacroCommand oPreviousCommand;
            MacroCommand oCurrentCommand = null;

            int maxAttempts = 20;
            int currentAttempts;
            DateTime? dtLastCombatCycle = null;
            int combatCycleInterval = ((IntegerVariable)pms.Variables["combatcycleinterval"]).Value;

            //Heal
            if (m != null && m.Heal)
            {
                int? autohp, automp;
                while (true)
                {
                    autohp = _autoHitpoints;
                    automp = _autoMana;
                    if (!autohp.HasValue) break;
                    if (autohp.Value >= _totalhp) break;
                    if (!automp.HasValue) break;
                    if (automp.Value < 2) break; //out of mana for vigor cast
                    CastLifeSpell("vigor", maxAttempts, ref dtLastCombatCycle, combatCycleInterval, pms);
                    if (_fleeing) break;
                    if (_bw.CancellationPending) break;
                    autohp = _autoHitpoints;
                    if (autohp.Value >= _totalhp) break;
                }
                //stop background processing if failed to get to max hitpoints
                autohp = _autoHitpoints;
                if (autohp.Value < _totalhp) return;

                //cast bless if has enough mana and not currently blessed
                List<string> spellsCast = _currentSpellsCast;
                if (_totalmp > 8 && spellsCast != null && !spellsCast.Contains("bless"))
                {
                    CastLifeSpell("bless", maxAttempts, ref dtLastCombatCycle, combatCycleInterval, pms);
                }
                //cast protection if has enough mana and not curently protected
                if (_totalmp > 8 && spellsCast != null && !spellsCast.Contains("protection"))
                {
                    CastLifeSpell("protection", maxAttempts, ref dtLastCombatCycle, combatCycleInterval, pms);
                }
            }

            //Activate skills
            if ((pms.UsedSkills & PromptedSkills.Manashield) == PromptedSkills.Manashield)
            {
                currentAttempts = 0;
                WaitUntilNextCombatCycle(dtLastCombatCycle, combatCycleInterval);
                while (currentAttempts < maxAttempts)
                {
                    if (_fleeing) break;
                    if (_bw.CancellationPending) break;
                    _manashieldResult = null;
                    SendCommand("manashield", false);
                    _currentBackgroundParameters.CommandsRun++;
                    currentAttempts++;
                    while (!_manashieldResult.HasValue)
                    {
                        Thread.Sleep(50);
                        RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                        if (_fleeing) break;
                        if (_bw.CancellationPending) break;
                    }
                    if (_fleeing) break;
                    if (_bw.CancellationPending) break;

                    if (_manashieldResult.HasValue)
                    {
                        dtLastCombatCycle = DateTime.UtcNow;
                        if (_manashieldResult.Value) //stop if successful
                        {
                            pms.DoScore = true;
                            break;
                        }
                    }
                }
            }

            //Run the pre-exit if present
            Exit preExit = pms.PreExit;
            if (preExit != null)
            {
                WaitUntilNextCombatCycle(dtLastCombatCycle, combatCycleInterval);
                RunPreExitLogic(pms, preExit.PreCommand, preExit.Target);
                string nextCommand = preExit.ExitText;
                if (!preExit.OmitGo) nextCommand = "go " + nextCommand;
                SendCommand(nextCommand, false);
                pms.CommandsRun++;
                pms.TargetRoom = preExit.Target;
                pms.SetTargetRoomIfCancelled = true;
            }

            if (pms.Flee)
            {
                _fleeing = true;
            }

            foreach (var nextCommandInfo in IterateStepCommands(commands, pms, 0))
            {
                MacroCommand nextCommand = nextCommandInfo.Key;
                int? overrideWaitMS = nextCommandInfo.Value;

                if (_bw.CancellationPending) break;
                oPreviousCommand = oCurrentCommand;
                oCurrentCommand = nextCommand;
                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                if (_fleeing) break;
                if (_bw.CancellationPending) break;

                //wait for an appropriate amount of time for the next command
                if (nextCommand.CombatCycle == null) //use the wait ms for commands after the first
                {
                    if (oPreviousCommand != null)
                    {
                        int remainingMS = overrideWaitMS.GetValueOrDefault(pms.WaitMS);
                        WaitUntilNextCommand(remainingMS, false, false);
                    }
                }
                else if (dtLastCombatCycle.HasValue) //just use the combat cycle to determine the timing
                {
                    WaitUntilNextCombatCycle(dtLastCombatCycle, combatCycleInterval);
                }

                if (_bw.CancellationPending) break;
                if (_fleeing) break;

                bool stop;
                ProcessCommand(nextCommand, pms, out stop);
                if (nextCommand.CombatCycle != null)
                {
                    dtLastCombatCycle = DateTime.UtcNow;
                }
                if (stop) break;
                if (_fleeing) break;
                if (_bw.CancellationPending) break;

                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                if (_fleeing) break;
                if (_bw.CancellationPending) break;
            }
            if (_fleeing)
            {
                string sWeapon = ((StringVariable)_currentBackgroundParameters.Variables["weapon"]).Value;
                if (!string.IsNullOrEmpty(sWeapon))
                {
                    SendCommand("remove " + sWeapon, false);
                    _currentBackgroundParameters.CommandsRun++;
                    if (!_fleeing) return;
                    if (_bw.CancellationPending) return;
                }

                //determine the flee exit if there is only one place to flee to
                Exit singleFleeableExit = null;
                Room r = m_oCurrentRoom;
                if (r != null && _map.TryGetOutEdges(r, out IEnumerable<Exit> exits))
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
                        RunPreExitLogic(pms, singleFleeableExit.PreCommand, singleFleeableExit.Target);
                    }
                }

                currentAttempts = 0;
                while (_fleeing && currentAttempts < maxAttempts)
                {
                    _commandResult = null;
                    SendCommand("flee", false);
                    _currentBackgroundParameters.CommandsRun++;
                    currentAttempts++;
                    while (!_commandResult.HasValue) //wait for the result
                    {
                        Thread.Sleep(50);
                        if (!_fleeing) break;
                        if (_bw.CancellationPending) break;
                    }
                    bool? currentResult = _commandResult;
                    int waitSeconds = _waitSeconds;
                    if (currentResult.HasValue)
                    {
                        if (currentResult.Value)
                        {
                            if (singleFleeableExit != null)
                            {
                                _currentBackgroundParameters.TargetRoom = singleFleeableExit.Target;
                            }
                            _fleeing = false;
                        }
                        else if (waitSeconds > 1)
                        {
                            WaitUntilNextCommand(500 + (1000 * (waitSeconds - 2)), true, false);
                            if (!_fleeing) break;
                            if (_bw.CancellationPending) break;
                        }
                    }
                }
            }

            if (pms.Quit)
            {
                currentAttempts = 0;
                while (currentAttempts < maxAttempts)
                {
                    _commandResult = null;
                    SendCommand("quit", false);
                    _currentBackgroundParameters.CommandsRun++;
                    currentAttempts++;
                    while (!_commandResult.HasValue) //wait for the result
                    {
                        Thread.Sleep(50);
                        if (_bw.CancellationPending) break;
                    }
                    bool? currentResult = _commandResult;
                    int waitSeconds = _waitSeconds;
                    if (currentResult.HasValue)
                    {
                        if (currentResult.Value)
                        {
                            break;
                        }
                        else if (waitSeconds > 1)
                        {
                            WaitUntilNextCommand(500 + (1000 * (waitSeconds - 2)), false, true);
                            if (_bw.CancellationPending) break;
                        }
                    }
                }
            }
        }

        private void RunPreExitLogic(BackgroundWorkerParameters pms, string preCommand, Room targetRoom)
        {
            if (!string.IsNullOrEmpty(preCommand))
            {
                SendCommand(preCommand, false);
                if (pms != null)
                {
                    pms.CommandsRun++;
                }
            }
            if (targetRoom.IsTrapRoom)
            {
                SendCommand("prepare", false);
                if (pms != null)
                {
                    pms.CommandsRun++;
                }
            }
        }

        private void CastLifeSpell(string spellName, int maxAttempts, ref DateTime? dtLastCombatCycle, int combatCycleInterval, BackgroundWorkerParameters bwp)
        {
            int currentAttempts = 0;
            WaitUntilNextCombatCycle(dtLastCombatCycle, combatCycleInterval);
            while (currentAttempts < maxAttempts)
            {
                if (_fleeing) break;
                if (_bw.CancellationPending) break;
                _commandResult = null;
                SendCommand("cast " + spellName, false);
                _currentBackgroundParameters.CommandsRun++;
                currentAttempts++;
                bool? currentResult;
                while (true)
                {
                    currentResult = _commandResult;
                    if (currentResult.HasValue) break;
                    Thread.Sleep(50);
                    RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                    if (_fleeing) break;
                    if (_bw.CancellationPending) break;
                }
                if (currentResult.HasValue)
                {
                    if (currentResult.Value)
                    {
                        dtLastCombatCycle = DateTime.UtcNow;
                        if (spellName == "bless" || spellName == "protection")
                        {
                            bwp.DoScore = true;
                        }
                    }
                    else if (_waitSeconds > 1)
                    {
                        int waitMS = 500 + (1000 * (_waitSeconds - 2));
                        WaitUntilNextCommand(waitMS, false, false);
                    }
                    break;
                }
                if (_fleeing) break;
                if (_bw.CancellationPending) break;
            }
        }

        /// <summary>
        /// spins until the next combat cycle
        /// </summary>
        /// <param name="dtLastCombatCycle">timestamp for the last combat cycle, if there was one</param>
        /// <param name="combatCycleInterval">combat cycle interval milliseconds</param>
        private void WaitUntilNextCombatCycle(DateTime? dtLastCombatCycle, int combatCycleInterval)
        {
            if (dtLastCombatCycle.HasValue) //spin until getting to the next combat cycle
            {
                int remainingMS = (int)(dtLastCombatCycle.Value.AddMilliseconds(combatCycleInterval) - DateTime.UtcNow).TotalMilliseconds;
                WaitUntilNextCommand(remainingMS, false, false);
            }
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
                string sWeapon = ((StringVariable)_currentBackgroundParameters.Variables["weapon"]).Value;
                if (!string.IsNullOrEmpty(sWeapon))
                {
                    SendCommand("wield " + sWeapon, false);
                    _currentBackgroundParameters.CommandsRun++;
                }
                _fumbled = false;
            }

            lock (_queuedCommandLock)
            {
                if (pms.QueuedCommand != null)
                {
                    SendCommand(pms.QueuedCommand, false);
                    pms.CommandsRun++;
                    pms.QueuedCommand = null;
                }
            }
        }

        private void ProcessCommand(MacroCommand nextCommand, BackgroundWorkerParameters pms, out bool stop)
        {
            MacroStepCombatCycle oCombatCycle = nextCommand.CombatCycle;
            string rawCommand;
            int? manaDrain = null;
            stop = false;
            if (oCombatCycle == null || oCombatCycle.Magic == MagicCombatCycleType.None)
            {
                rawCommand = nextCommand.RawCommand;
            }
            else if (oCombatCycle.Magic == MagicCombatCycleType.Stun)
            {
                rawCommand = "cast stun {mob}";
                manaDrain = 10;
            }
            else if (oCombatCycle.Magic == MagicCombatCycleType.OffensiveSpell)
            {
                int iMaxOffLevel = Convert.ToInt32(pms.MaxOffensiveLevel);
                if (_currentMana >= 10 && iMaxOffLevel >= 3)
                {
                    manaDrain = 10;
                    rawCommand = "cast {realm3spell} {mob}";
                }
                else if (_currentMana >= 7 && iMaxOffLevel >= 2)
                {
                    manaDrain = 7;
                    rawCommand = "cast {realm2spell} {mob}";
                }
                else if (_currentMana >= 3)
                {
                    manaDrain = 3;
                    rawCommand = "cast {realm1spell} {mob}";
                }
                else //don't have enough mana to cast an offensive spell
                {
                    stop = true;
                    return;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            //stop processing if out of mana
            if (oCombatCycle != null && oCombatCycle.Magic != MagicCombatCycleType.None && _currentMana < manaDrain.Value)
            {
                stop = true;
                return;
            }

            //re-translate the command in case variables have changed
            string actualCommand = TranslateCommand(rawCommand, _currentBackgroundParameters.GetVariables(), out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                stop = true; //not much we can do here except stop
                return;
            }

            SendCommand(actualCommand, false);
            pms.CommandsRun++;
            if (manaDrain.HasValue)
            {
                if (!pms.AutoMana)
                {
                    _currentMana -= manaDrain.Value;
                }
                if (_currentMana < 3) //no mana left for casting any more offensive spells
                {
                    stop = true;
                }
            }

            if (oCombatCycle != null && oCombatCycle.Attack)
            {
                IEnumerable<Variable> vars = _currentBackgroundParameters.GetVariables();
                foreach (var nextVar in vars)
                {
                    if (nextVar.Name == "attacktype" && ((StringVariable)nextVar).Value == "power")
                    {
                        pms.DoScore = true;
                    }
                }
                string attackCommand = TranslateCommand("{attacktype} {mob}", vars, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    stop = true; //not much we can do here except stop
                    return;
                }
                SendCommand(attackCommand, false);
                pms.CommandsRun++;
            }
        }

        /// <summary>
        /// iterates through step commands
        /// </summary>
        /// <param name="Steps">step commands</param>
        /// <param name="parameters">parameters to the background worker</param>
        /// <param name="loopsPerformed">number of loops already performed</param>
        /// <returns>commands to run</returns>
        private IEnumerable<KeyValuePair<MacroCommand, int?>> IterateStepCommands(List<MacroStepBase> Steps, BackgroundWorkerParameters parameters, int loopsPerformed)
        {
            Dictionary<string, Variable> variables = parameters.Variables;
            foreach (MacroStepBase nextStep in Steps)
            {
                Variable conditionV = nextStep.ConditionVariable;
                if (conditionV != null)
                {
                    if (conditionV.Type == VariableType.String)
                    {
                        if (string.IsNullOrEmpty(((StringVariable)conditionV).Value))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                if (nextStep.SkipRounds > 0 && loopsPerformed < nextStep.SkipRounds)
                {
                    continue;
                }

                int? loopCountMax = null;
                if (nextStep.LoopCount.HasValue)
                {
                    loopCountMax = nextStep.LoopCount.Value;
                }
                else if (nextStep.LoopVariable != null && nextStep.LoopVariable.Type == VariableType.Int)
                {
                    loopCountMax = ((IntegerVariable)variables[nextStep.LoopVariable.Name]).Value;
                }
                bool doInfiniteLoop = false;
                if (nextStep.Loop.HasValue)
                {
                    doInfiniteLoop = nextStep.Loop.Value;
                }
                else if (nextStep.LoopVariable != null && nextStep.LoopVariable.Type == VariableType.Bool)
                {
                    doInfiniteLoop = ((BooleanVariable)variables[nextStep.LoopVariable.Name]).Value;
                }

                int loopCount = 0;
                while (true)
                {
                    //do nothing if the loop count is zero
                    if (loopCountMax.HasValue && loopCountMax.Value == 0)
                    {
                        break;
                    }

                    bool overrideWaitMS;
                    if (nextStep is MacroStepSetNextCommandWaitMS)
                    {
                        parameters.NextCommandWaitMS = ((MacroStepSetNextCommandWaitMS)nextStep).WaitMS.Value;
                    }
                    else if (nextStep is MacroStepSetVariable)
                    {
                        MacroStepSetVariable mssv = (MacroStepSetVariable)nextStep;
                        if (mssv.Variable.Type == VariableType.Bool)
                        {
                            ((BooleanVariable)parameters.Variables[mssv.Variable.Name]).Value = ((BooleanVariable)mssv.Variable).Value;
                        }
                        else if (mssv.Variable.Type == VariableType.Int)
                        {
                            ((IntegerVariable)parameters.Variables[mssv.Variable.Name]).Value = ((IntegerVariable)mssv.Variable).Value;
                        }
                        else if (mssv.Variable.Type == VariableType.String)
                        {
                            ((StringVariable)parameters.Variables[mssv.Variable.Name]).Value = ((StringVariable)mssv.Variable).Value;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else if (nextStep is MacroStepSequence)
                    {
                        MacroStepSequence seq = (MacroStepSequence)nextStep;
                        overrideWaitMS = parameters.NextCommandWaitMS.HasValue;
                        int overrideWaitMSValue = overrideWaitMS ? parameters.NextCommandWaitMS.Value : 0;
                        int previousWaitMS = parameters.WaitMS;
                        bool pastFirst = false;
                        foreach (var nextStepCommand in IterateStepCommands(seq.SubCommands, parameters, loopCount))
                        {
                            MacroCommand nextSubCommand = nextStepCommand.Key;
                            if (pastFirst)
                            {
                                yield return nextStepCommand;
                            }
                            else
                            {
                                if (overrideWaitMS)
                                {
                                    yield return new KeyValuePair<MacroCommand, int?>(nextSubCommand, overrideWaitMSValue);
                                    parameters.NextCommandWaitMS = null;
                                    SetWaitMS(seq, parameters, variables);
                                }
                                else
                                {
                                    SetWaitMS(seq, parameters, variables);
                                    yield return nextStepCommand;
                                }
                                pastFirst = true;
                            }
                        }
                    }
                    else
                    {
                        MacroCommand nextCommand;
                        if (nextStep is MacroStepCombatCycle)
                        {
                            nextCommand = new MacroCommand(string.Empty, string.Empty);
                            nextCommand.CopyFrom(nextStep);
                            nextCommand.CombatCycle = (MacroStepCombatCycle)nextStep;
                        }
                        else
                        {
                            nextCommand = (MacroCommand)nextStep;
                        }
                        overrideWaitMS = parameters.NextCommandWaitMS.HasValue;
                        if (overrideWaitMS)
                        {
                            int overrideWaitMSValue = parameters.NextCommandWaitMS.Value;
                            yield return new KeyValuePair<MacroCommand, int?>(nextCommand, overrideWaitMSValue);
                        }
                        else
                        {
                            SetWaitMS(nextCommand, parameters, variables);
                            yield return new KeyValuePair<MacroCommand, int?>(nextCommand, null);
                        }
                    }
                    loopCount++;

                    if (loopCountMax.HasValue)
                    {
                        //stop if reached the number of times to loop
                        if (loopCount >= loopCountMax.Value)
                        {
                            break;
                        }
                    }
                    else if (!doInfiniteLoop)
                    {
                        break; //stop if not in a loop
                    }
                }
            }
        }

        private void SetWaitMS(MacroStepBase nextStep, BackgroundWorkerParameters parameters, Dictionary<string, Variable> variables)
        {
            if (nextStep.WaitMS.HasValue)
            {
                parameters.WaitMS = nextStep.WaitMS.Value;
            }
            else if (nextStep.WaitMSVariable != null)
            {
                parameters.WaitMS = ((IntegerVariable)variables[nextStep.WaitMSVariable.Name]).Value;
            }
        }

        private void ToggleBackgroundProcess(bool running)
        {
            Macro m = _currentBackgroundParameters.Macro;
            bool quitting = _currentBackgroundParameters.Quit;
            CommandType eRunningCombatCommandTypes = CommandType.Magic | CommandType.Melee | CommandType.Potions;
            if (m != null) eRunningCombatCommandTypes = m.CombatCommandTypes;
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
                    bool regularLogicEnabled = !running;
                    if (ctl is Button)
                    {
                        if (ctl == btnFlee)
                        {
                            regularLogic = true;
                            regularLogicEnabled = true;
                        }
                        else if (ctl == btnAbort)
                        {
                            ctl.Enabled = running;
                        }
                        else if (ctl.Tag is CommandButtonTag)
                        {
                            CommandButtonTag cbt = (CommandButtonTag)ctl.Tag;
                            regularLogic = true;
                            regularLogicEnabled = !running || ((eRunningCombatCommandTypes & cbt.CommandType) == CommandType.None);
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
                        if (ctl == txtOneOffCommand)
                        {
                            regularLogic = true;
                        }
                    }
                    else if (ctl != grpLocations && ctl != grpConsole)
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
                            ctl.Enabled = regularLogicEnabled;
                        }
                    }
                }
            }
        }


        private void InitializeMap()
        {
            _map = new AdjacencyGraph<Room, Exit>();

            foreach (Area a in _areas)
            {
                a.Locations.Clear();
            }

            Area aBree = _areasByName[AREA_BREE];
            AddBreeCity(aBree, out Room oIxell, out Room oBreeTownSquare, out Room oBreeWestGateInside, out Room oSewerPipeExit, out Room aqueduct, out Room sewerTunnelToTConnection);
            AddMayorMillwoodMansion(oIxell, out List<Room> mansionRooms, out Room oChancellorOfProtection, out Room oMayorMillwood);
            AddLocation(_aBreePerms, oChancellorOfProtection);
            AddLocation(_aBreePerms, oMayorMillwood);
            foreach (Room r in mansionRooms)
            {
                AddLocation(aBree, r);
            }

            AddBreeToHobbiton(oBreeWestGateInside, aqueduct);
            AddBreeToImladris(oSewerPipeExit, sewerTunnelToTConnection);
            AddImladrisCity(out Room oImladrisSouthGateInside);
            AddImladrisToTharbad(oImladrisSouthGateInside, out Room oTharbadGateOutside);
            AddTharbadCity(oTharbadGateOutside);
            AddIntangible(oBreeTownSquare);
        }

        private void AddTharbadCity(Room oTharbadGateOutside)
        {
            Room balleNightingale = AddRoom("Tharbad Balle/Nightingale");
            Room balle1 = AddRoom("Tharbad Balle");
            AddBidirectionalExits(balleNightingale, balle1, BidirectionalExitType.WestEast);
            AddBidirectionalSameNameExit(oTharbadGateOutside, balleNightingale, "gate");

            Room balleIllusion = AddRoom("Tharbad Balle/Illusion");
            AddBidirectionalExits(balle1, balleIllusion, BidirectionalExitType.WestEast);

            Room balle2 = AddRoom("Tharbad Balle");
            AddBidirectionalExits(balleIllusion, balle2, BidirectionalExitType.WestEast);

            Room balleEvard = AddRoom("Tharbad Balle/Evard");
            AddBidirectionalExits(balle2, balleEvard, BidirectionalExitType.WestEast);

            Room evard1 = AddRoom("Tharbad Evard");
            AddBidirectionalExits(balleEvard, evard1, BidirectionalExitType.NorthSouth);

            Room evard2 = AddRoom("Tharbad Evard");
            AddBidirectionalExits(evard1, evard2, BidirectionalExitType.NorthSouth);

            Room sabreEvard = AddRoom("Tharbad Sabre/Evard");
            AddBidirectionalExits(evard2, sabreEvard, BidirectionalExitType.NorthSouth);

            Room sabre1 = AddRoom("Tharbad Sabre");
            AddBidirectionalExits(sabre1, sabreEvard, BidirectionalExitType.WestEast);

            Room sabreIllusion = AddRoom("Tharbad Sabre/Illusion");
            AddBidirectionalExits(sabreIllusion, sabre1, BidirectionalExitType.WestEast);

            Room sabre2 = AddRoom("Tharbad Sabre");
            AddBidirectionalExits(sabre2, sabreIllusion, BidirectionalExitType.WestEast);

            Room sabreNightingale = AddRoom("Tharbad Sabre/Nightingale");
            AddBidirectionalExits(sabreNightingale, sabre2, BidirectionalExitType.WestEast);

            Room nightingale1 = AddRoom("Tharbad Nightingale");
            AddBidirectionalExits(nightingale1, sabreNightingale, BidirectionalExitType.NorthSouth);

            Room nightingale2 = AddRoom("Tharbad Nightingale");
            AddBidirectionalExits(nightingale2, nightingale1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(balleNightingale, nightingale2, BidirectionalExitType.NorthSouth);

            Room nightingale3 = AddRoom("Tharbad Nightingale");
            AddBidirectionalExits(sabreNightingale, nightingale3, BidirectionalExitType.NorthSouth);

            Room bardicGuildhall = AddRoom("Tharbad Bardic Guildhall");
            bardicGuildhall.IsHealingRoom = true;
            AddBidirectionalExits(bardicGuildhall, nightingale3, BidirectionalExitType.WestEast);

            Room oGuildmasterAnsette = AddRoom("Guildmaster Ansette");
            oGuildmasterAnsette.Mob1 = "Ansette";
            oGuildmasterAnsette.Experience1 = 1200;
            Exit e = AddExit(bardicGuildhall, oGuildmasterAnsette, "door");
            e.Hidden = true;
            AddExit(oGuildmasterAnsette, bardicGuildhall, "out");

            Room sabre3 = AddRoom("Tharbad Sabre");
            AddBidirectionalExits(sabre3, sabreNightingale, BidirectionalExitType.WestEast);

            Room illusion1 = AddRoom("Tharbad Illusion");
            AddBidirectionalExits(illusion1, sabreIllusion, BidirectionalExitType.NorthSouth);

            Room marketDistrictClothiers = AddRoom("Tharbad Market District Clothiers");
            AddBidirectionalExits(marketDistrictClothiers, illusion1, BidirectionalExitType.NorthSouth);

            Room oMasterJeweler = AddRoom("Master Jeweler");
            oMasterJeweler.Mob1 = "Jeweler";
            oMasterJeweler.Experience1 = 170;
            oMasterJeweler.Alignment = AlignmentType.Red;
            AddBidirectionalExits(marketDistrictClothiers, oMasterJeweler, BidirectionalExitType.WestEast);

            Room oEntranceToGypsyEncampment = AddRoom("Entrance to Gypsy Encampment");
            AddExit(oMasterJeweler, oEntranceToGypsyEncampment, "row");
            AddExit(oEntranceToGypsyEncampment, oMasterJeweler, "market");

            Room oGypsyRow1 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oEntranceToGypsyEncampment, oGypsyRow1, BidirectionalExitType.WestEast);

            Room oGypsyRow2 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow1, oGypsyRow2, BidirectionalExitType.WestEast);

            Room oGypsyRow3 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow3, oGypsyRow2, BidirectionalExitType.NorthSouth);

            Room oGypsyRow4 = AddRoom("Gypsy Row");
            AddBidirectionalExits(oGypsyRow4, oGypsyRow3, BidirectionalExitType.WestEast);

            Room oKingBrundensWagon = AddRoom("King Brunden's Wagon");
            AddExit(oGypsyRow4, oKingBrundensWagon, "wagon");
            AddExit(oKingBrundensWagon, oGypsyRow4, "out");

            Room oKingBrunden = AddRoom("King Brunden");
            oKingBrunden.Mob1 = "king";
            oKingBrunden.Experience1 = 300;
            AddExit(oKingBrundensWagon, oKingBrunden, "back");
            AddExit(oKingBrunden, oKingBrundensWagon, "out");

            Room oGypsyBlademaster = AddRoom("Gypsy Blademaster");
            oGypsyBlademaster.Mob1 = "Blademaster";
            oGypsyBlademaster.Experience1 = 160;
            oGypsyBlademaster.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow3, oGypsyBlademaster, "tent");
            AddExit(oGypsyBlademaster, oGypsyRow3, "out");

            Room oKingsMoneychanger = AddRoom("King's Moneychanger");
            oKingsMoneychanger.Mob1 = "Moneychanger";
            oKingsMoneychanger.Experience1 = 150;
            oKingsMoneychanger.Alignment = AlignmentType.Red;
            AddExit(oGypsyRow2, oKingsMoneychanger, "tent");
            AddExit(oKingsMoneychanger, oGypsyRow2, "out");

            Room oMadameNicolov = AddRoom("Madame Nicolov");
            oMadameNicolov.Mob1 = "Madame";
            oMadameNicolov.Experience1 = 180;
            oMadameNicolov.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow1, oMadameNicolov, "wagon");
            AddExit(oMadameNicolov, oGypsyRow1, "out");

            Room gildedApple = AddRoom("Gilded Applie");
            AddBidirectionalSameNameExit(sabre3, gildedApple, "door");

            Room zathriel = AddRoom("Zathriel the Minstrel");
            zathriel.Mob1 = "Minstrel";
            zathriel.Experience1 = 220;
            zathriel.Alignment = AlignmentType.Blue;
            e = AddExit(gildedApple, zathriel, "stage");
            e.Hidden = true;
            AddExit(zathriel, gildedApple, "down");

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);

            Room oOliphaunt = AddRoom("Oliphaunt");
            oOliphaunt.Mob1 = "Oliphaunt";
            oOliphaunt.Experience1 = 310;
            oOliphaunt.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphaunt, "curtain");

            AddLocation(_aImladrisTharbadPerms, bardicGuildhall);
            AddLocation(_aImladrisTharbadPerms, oGuildmasterAnsette);
            AddLocation(_aImladrisTharbadPerms, zathriel);
            AddLocation(_aImladrisTharbadPerms, oOliphaunt);
            AddLocation(_aImladrisTharbadPerms, oMasterJeweler);
            AddLocation(_aImladrisTharbadPerms, oMadameNicolov);
            AddLocation(_aImladrisTharbadPerms, oKingsMoneychanger);
            AddLocation(_aImladrisTharbadPerms, oGypsyBlademaster);
            AddLocation(_aImladrisTharbadPerms, oKingBrunden);
        }

        private void AddBreeCity(Area aBree, out Room oIxell, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSewerPipeExit, out Room aqueduct, out Room sewerTunnelToTConnection)
        {
            //Bree's road structure is a 15x11 grid
            Room[,] breeStreets = new Room[16, 11];
            Room[,] breeSewers = new Room[16, 11];
            breeStreets[0, 0] = AddRoom("Thalion/Wain"); //1x1
            breeStreets[1, 0] = AddRoom("Thalion"); //2x1
            breeStreets[2, 0] = AddRoom("Thalion"); //3x1
            breeStreets[3, 0] = AddRoom("Thalion/High"); //4x1
            breeStreets[4, 0] = AddRoom("Thalion"); //5x1
            breeStreets[5, 0] = AddRoom("Thalion"); //6x1
            breeStreets[6, 0] = AddRoom("Thalion"); //7x1
            breeStreets[7, 0] = AddRoom("Thalion/Main"); //8x1
            _breeDocks = breeStreets[9, 0] = AddRoom("Docks"); //10x1
            oSewerPipeExit = breeStreets[10, 0] = AddRoom("Thalion/Crissaegrim"); //11x1
            breeStreets[11, 0] = AddRoom("Thalion"); //12x1
            breeStreets[12, 0] = AddRoom("Thalion"); //13x1
            breeStreets[13, 0] = AddRoom("Thalion"); //14x1
            breeStreets[14, 0] = AddRoom("Thalion/Brownhaven"); //15x1
            breeStreets[0, 1] = AddRoom("Wain"); //1x2
            Room oToCampusFreeClinic = breeStreets[3, 1] = AddRoom("High"); //4x2
            breeStreets[7, 1] = AddRoom("Main"); //8x2
            breeStreets[10, 1] = AddRoom("Crissaegrim"); //11x2
            breeStreets[14, 1] = AddRoom("Brownhaven"); //15x2
            breeStreets[0, 2] = AddRoom("Wain"); //1x3
            Room oToPawnShopWest = breeStreets[3, 2] = AddRoom("High"); //4x3
            Room oToBarracks = breeStreets[7, 2] = AddRoom("Main"); //8x3
            breeStreets[10, 2] = AddRoom("Crissaegrim"); //11x3
            breeStreets[14, 2] = AddRoom("Brownhaven"); //15x3
            breeStreets[0, 3] = AddRoom("Periwinkle/Wain"); //1x4
            breeSewers[0, 3] = AddRoom("Sewers Periwinkle/Wain"); //1x4
            AddExit(breeSewers[0, 3], breeStreets[0, 3], "up");
            breeStreets[1, 3] = AddRoom("Periwinkle"); //2x4
            breeSewers[1, 3] = AddRoom("Sewers Periwinkle"); //2x4
            breeStreets[2, 3] = AddRoom("Periwinkle"); //3x4
            breeSewers[2, 3] = AddRoom("Sewers Periwinkle"); //3x4
            breeStreets[3, 3] = AddRoom("Periwinkle/High"); //4x4
            breeSewers[3, 3] = AddRoom("Sewers Periwinkle/High"); //4x4
            AddExit(breeSewers[3, 3], breeStreets[3, 3], "up");
            breeStreets[4, 3] = AddRoom("Periwinkle"); //5x4
            breeSewers[4, 3] = AddRoom("Sewers Periwinkle"); //5x4
            breeStreets[5, 3] = AddRoom("Periwinkle"); //6x4
            breeSewers[5, 3] = AddRoom("Sewers Periwinkle"); //6x4
            breeStreets[6, 3] = AddRoom("Periwinkle"); //7x4
            breeSewers[6, 3] = AddRoom("Sewers Periwinkle"); //7x4
            breeStreets[7, 3] = AddRoom("Periwinkle/Main"); //8x4
            Room oShirriff = breeSewers[7, 3] = AddRoom("Shirriffs"); //Bree Sewers Periwinkle/Main 8x4
            AddExit(breeSewers[7, 3], breeStreets[7, 3], "up");
            breeStreets[8, 3] = AddRoom("Periwinkle"); //9x4
            breeStreets[9, 3] = AddRoom("South Bridge"); //10x4
            breeStreets[10, 3] = AddRoom("Periwinkle/Crissaegrim"); //11x4
            breeStreets[11, 3] = AddRoom("Periwinkle"); //12x4
            Room oPeriwinklePoorAlley = breeStreets[12, 3] = AddRoom("Periwinkle/PoorAlley"); //13x4
            breeStreets[13, 3] = AddRoom("Periwinkle"); //14x4
            breeStreets[14, 3] = AddRoom("Periwinkle/Brownhaven"); //15x4
            breeStreets[0, 4] = AddRoom("Wain"); //1x5
            breeSewers[0, 4] = AddRoom("Sewers Wain"); //1x5
            Room oToBlindPigPubAndUniversity = breeStreets[3, 4] = AddRoom("High"); //4x5
            breeStreets[7, 4] = AddRoom("Main"); //8x5
            Room oToSnarSlystoneShoppe = breeStreets[10, 4] = AddRoom("Crissaegrim"); //11x5
            breeStreets[14, 4] = AddRoom("Brownhaven"); //15x5
            breeStreets[0, 5] = AddRoom("Wain"); //1x6
            breeSewers[0, 5] = AddRoom("Sewers Wain"); //1x6
            breeStreets[3, 5] = AddRoom("High"); //4x6
            breeStreets[7, 5] = AddRoom("Main"); //8x6
            Room oBigPapa = breeStreets[8, 5] = AddRoom("Big Papa"); //9x6
            breeStreets[10, 5] = AddRoom("Crissaegrim"); //11x6
            breeStreets[14, 5] = AddRoom("Brownhaven"); //15x6
            breeStreets[0, 6] = AddRoom("Wain"); //1x7
            breeSewers[0, 6] = AddRoom("Sewers Wain"); //1x7
            breeStreets[3, 6] = AddRoom("High"); //4x7
            breeStreets[7, 6] = AddRoom("Main"); //8x7
            breeStreets[10, 6] = AddRoom("Crissaegrim"); //11x7
            breeStreets[14, 6] = AddRoom("Brownhaven"); //15x7
            oWestGateInside = breeStreets[0, 7] = AddRoom("West Gate"); //1x8
            breeSewers[0, 7] = AddRoom("Sewers West Gate"); //1x8
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Leviathan"); //2x8
            breeStreets[2, 7] = AddRoom("Leviathan"); //3x8
            breeStreets[3, 7] = AddRoom("Leviathan/High"); //4x8
            breeStreets[4, 7] = AddRoom("Leviathan"); //5x8
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Town Square"); //6x8
            breeStreets[6, 7] = AddRoom("Leviathan"); //7x8
            breeStreets[7, 7] = AddRoom("Leviathan/Main"); //8x8
            breeStreets[8, 7] = AddRoom("Leviathan"); //9x8
            Room oNorthBridge = breeStreets[9, 7] = AddRoom("North Bridge"); //10x8
            breeStreets[10, 7] = AddRoom("Leviathan/Crissaegrim"); //11x8
            breeStreets[11, 7] = AddRoom("Leviathan"); //12x8
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Leviathan"); //13x8
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Leviathan"); //14x8
            _breeEastGateInside = breeStreets[14, 7] = AddRoom("East Gate"); //15x8
            breeStreets[0, 8] = AddRoom("Wain"); //1x9
            breeSewers[0, 8] = AddRoom("Sewers Wain"); //1x9
            breeStreets[3, 8] = AddRoom("High"); //4x9
            breeStreets[7, 8] = AddRoom("Main"); //8x9
            breeStreets[10, 8] = AddRoom("Crissaegrim"); //11x9
            breeStreets[14, 8] = AddRoom("Brownhaven"); //15x9
            Room oOrderOfLove = breeStreets[15, 8] = AddRoom("Order of Love"); //16x9
            oOrderOfLove.IsHealingRoom = true;
            switch (_preferredAlignment)
            {
                case AlignmentType.Blue:
                    oOrderOfLove.Mob1 = "Drunk";
                    break;
                case AlignmentType.Red:
                    oOrderOfLove.Mob1 = "Doctor";
                    break;
            }
            breeStreets[0, 9] = AddRoom("Wain"); //1x10
            breeSewers[0, 9] = AddRoom("Sewers Wain"); //1x10
            breeStreets[3, 9] = AddRoom("High"); //4x10
            breeStreets[7, 9] = AddRoom("Main"); //8x10
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Crissaegrim"); //11x10
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Brownhaven"); //15x10
            breeStreets[0, 10] = AddRoom("Ormenel/Wain"); //1x11
            breeSewers[0, 10] = AddRoom("Sewers Ormenel/Wain"); //1x11
            Exit e = AddExit(breeStreets[0, 10], breeSewers[0, 10], "sewer");
            e.PreCommand = "open sewer";
            breeStreets[1, 10] = AddRoom("Ormenel"); //2x11
            Room oToZoo = breeStreets[2, 10] = AddRoom("Ormenel"); //3x11
            breeStreets[3, 10] = AddRoom("Ormenel/High"); //4x11
            Room oToCasino = breeStreets[4, 10] = AddRoom("Ormenel"); //5x11
            breeStreets[5, 10] = AddRoom("Ormenel"); //6x11
            breeStreets[6, 10] = AddRoom("Ormenel"); //7x11
            breeStreets[7, 10] = AddRoom("Ormenel/Main"); //8x11
            breeStreets[10, 10] = AddRoom("Ormenel"); //11x11
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Ormenel"); //12x11
            breeStreets[12, 10] = AddRoom("Ormenel"); //13x11
            Room oStreetToFallon = breeStreets[13, 10] = AddRoom("Ormenel"); //14x11
            breeStreets[14, 10] = AddRoom("Brownhaven/Ormenel"); //15x11

            _breeStreetsGraph = new RoomGraph("Bree Streets");
            _breeStreetsGraph.ScalingFactor = 100;
            _graphs.Add(_breeStreetsGraph);

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    AddGridBidirectionalExits(breeStreets, x, y);
                }
            }

            //add exits for the sewers. due to screwiness on periwinkle this can't be done automatically.
            AddBidirectionalExits(breeSewers[0, 10], breeSewers[0, 9], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 9], breeSewers[0, 8], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 8], breeSewers[0, 7], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 7], breeSewers[0, 6], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 6], breeSewers[0, 5], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 5], breeSewers[0, 4], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 4], breeSewers[0, 3], BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(breeSewers[0, 3], breeSewers[1, 3], BidirectionalExitType.WestEast);
            AddExit(breeSewers[1, 3], breeSewers[3, 3], "east");
            AddExit(breeSewers[3, 3], breeSewers[2, 3], "west");
            AddExit(breeSewers[2, 3], breeSewers[1, 3], "west");
            AddBidirectionalExits(breeSewers[3, 3], breeSewers[4, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[4, 3], breeSewers[5, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[5, 3], breeSewers[6, 3], BidirectionalExitType.WestEast);
            AddBidirectionalExits(breeSewers[6, 3], breeSewers[7, 3], BidirectionalExitType.WestEast);

            oShirriff.Mob1 = "shirriff";
            oShirriff.Experience1 = 325;
            oShirriff.Experience2 = 325;

            Room oValveChamber = AddRoom("Valve Chamber");
            e = AddExit(breeSewers[7, 3], oValveChamber, "valve");
            e.Hidden = true;
            AddExit(oValveChamber, breeSewers[7, 3], "south");

            Room oSewerPassageFromValveChamber = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerPassageFromValveChamber, oValveChamber, BidirectionalExitType.NorthSouth);

            Room oSewerDemonThreshold = AddRoom("Sewer Demon Threshold");
            oSewerDemonThreshold.Mob1 = "demon";
            AddBidirectionalExits(oSewerDemonThreshold, oSewerPassageFromValveChamber, BidirectionalExitType.SoutheastNorthwest);

            Room oPoorAlley1 = AddRoom("Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");
            _breeStreetsGraph.Rooms[oPoorAlley1] = new System.Windows.Point(12, 4);

            Room oPoorAlley2 = AddRoom("Poor Alley");
            AddBidirectionalExits(oPoorAlley1, oPoorAlley2, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oPoorAlley2] = new System.Windows.Point(12, 5);

            Room oPoorAlley3 = AddRoom("Poor Alley");
            AddBidirectionalExits(oPoorAlley2, oPoorAlley3, BidirectionalExitType.NorthSouth);
            AddExit(oPeriwinklePoorAlley, oPoorAlley3, "alley");
            AddExit(oPoorAlley3, oPeriwinklePoorAlley, "south");
            _breeStreetsGraph.Rooms[oPoorAlley3] = new System.Windows.Point(12, 6);

            Room oCampusFreeClinic = AddRoom("Bree Campus Free Clinic");
            oCampusFreeClinic.Mob1 = "Student";
            oCampusFreeClinic.IsHealingRoom = true;
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");
            _breeStreetsGraph.Rooms[oCampusFreeClinic] = new System.Windows.Point(4, 9);

            Room oBreeRealEstateOffice = AddRoom("Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oBreeRealEstateOffice] = new System.Windows.Point(11, -0.5);

            oIxell = AddRoom("Ixell 70 Bl");
            oIxell.Mob1 = "Ixell";
            AddExit(oBreeRealEstateOffice, oIxell, "door");
            AddExit(oIxell, oBreeRealEstateOffice, "out");
            _breeStreetsGraph.Rooms[oIxell] = new System.Windows.Point(11, -1);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oKistaHillsHousing] = new System.Windows.Point(13, -0.5);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate");
            Room oFallon = AddRoom("Fallon");
            oFallon.Experience1 = 350;
            oFallon.Alignment = AlignmentType.Blue;
            AddExit(oChurchsEnglishGarden, oFallon, "door");
            AddExit(oFallon, oChurchsEnglishGarden, "out");
            oChurchsEnglishGarden.Mob1 = oFallon.Mob1 = "Fallon";
            _breeStreetsGraph.Rooms[oChurchsEnglishGarden] = new System.Windows.Point(13, -1);
            _breeStreetsGraph.Rooms[oFallon] = new System.Windows.Point(13, -1.5);

            Room oGrantsStables = AddRoom("Grant's stables");
            if (_level < 11)
            {
                AddExit(oToGrantsStables, oGrantsStables, "stable");
            }
            AddExit(oGrantsStables, oToGrantsStables, "south");

            Room oGrant = AddRoom("Grant");
            oGrant.Mob1 = "Grant";
            oGrant.Experience1 = 170;
            Exit oToGrant = AddExit(oGrantsStables, oGrant, "gate");
            oToGrant.PreCommand = "open gate";
            AddExit(oGrant, oGrantsStables, "out");

            Room oPansy = AddRoom("Pansy Smallburrows");
            oPansy.Mob1 = "Pansy";
            oPansy.Experience1 = 95;
            oPansy.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oPansy] = new System.Windows.Point(13, 1);

            Room oDroolie = AddRoom("Droolie");
            oDroolie.Mob1 = "Droolie";
            oDroolie.Experience1 = 100;
            oDroolie.Alignment = AlignmentType.Red;
            e = AddExit(oNorthBridge, oDroolie, "rope");
            e.Hidden = true;
            AddExit(oDroolie, oNorthBridge, "up");
            _breeStreetsGraph.Rooms[oDroolie] = new System.Windows.Point(9, 3.5);

            Room oBrandywineRiver1 = AddRoom("Brandywine River");
            AddExit(oDroolie, oBrandywineRiver1, "down");
            //AddExit(oBrandywineRiver1, oDroolie, "rope"); //requires fly

            Room oBrandywineRiver2 = AddRoom("Brandywine River");
            AddBidirectionalExits(oBrandywineRiver1, oBrandywineRiver2, BidirectionalExitType.WestEast);

            Room oOohlgrist = AddRoom("Oohlgrist");
            oOohlgrist.Mob1 = "Oohlgrist";
            oOohlgrist.Experience1 = 110;
            AddExit(oBrandywineRiver2, oOohlgrist, "boat");
            AddExit(oOohlgrist, oBrandywineRiver2, "river");

            Room oBrandywineRiverBoathouse = AddRoom("Brandywine River Boathouse");
            AddExit(oOohlgrist, oBrandywineRiverBoathouse, "shore");
            AddExit(oBrandywineRiverBoathouse, oOohlgrist, "boat");

            Room oRockyBeach1 = AddRoom("Rocky Beach");
            AddBidirectionalExits(oBrandywineRiverBoathouse, oRockyBeach1, BidirectionalExitType.WestEast);

            Room oRockyBeach2 = AddRoom("Rocky Beach");
            AddBidirectionalExits(oRockyBeach1, oRockyBeach2, BidirectionalExitType.WestEast);

            Room oHermitsCave = AddRoom("Hermit Fisher");
            oHermitsCave.Mob1 = "Fisher";
            oHermitsCave.Experience1 = 60;
            AddExit(oRockyBeach2, oHermitsCave, "cave");
            AddExit(oHermitsCave, oRockyBeach2, "out");

            Room oRockyAlcove = AddRoom("Rocky Alcove");
            AddExit(oRockyBeach1, oRockyAlcove, "alcove");
            AddExit(oRockyAlcove, oRockyBeach1, "north");

            Room oSewerDrain = AddRoom("Sewer Drain");
            AddBidirectionalSameNameExit(oRockyAlcove, oSewerDrain, "grate");

            Room oDrainTunnel1 = AddRoom("Drain Tunnel");
            AddExit(oSewerDrain, oDrainTunnel1, "south");

            Room oDrainTunnel2 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel1, oDrainTunnel2, "north");

            Room oDrainTunnel3 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel2, oDrainTunnel3, "south");

            Room oDrainTunnel4 = AddRoom("Drain Tunnel");
            AddExit(oDrainTunnel3, oDrainTunnel4, "south");

            sewerTunnelToTConnection = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oDrainTunnel4, sewerTunnelToTConnection, BidirectionalExitType.NorthSouth);

            Room oBoardedSewerTunnel = AddRoom("Boarded Sewer Tunnel");
            AddBidirectionalExits(sewerTunnelToTConnection, oBoardedSewerTunnel, BidirectionalExitType.WestEast);

            Room oSewerOrcChamber = AddRoom("Sewer Orc Guards");
            oSewerOrcChamber.Mob1 = "Guard";
            oSewerOrcChamber.Experience1 = 70;
            oSewerOrcChamber.Experience2 = 70;
            e = AddExit(oBoardedSewerTunnel, oSewerOrcChamber, "busted");
            e.Hidden = true;
            e = AddExit(oSewerOrcChamber, oBoardedSewerTunnel, "busted");
            e.Hidden = true;

            Room oSewerOrcLair = AddRoom("Sewer Orc Lair");
            AddBidirectionalExits(oSewerOrcLair, oSewerOrcChamber, BidirectionalExitType.NorthSouth);

            Room oSewerPassage = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerOrcLair, oSewerPassage, BidirectionalExitType.WestEast);

            Room oSewerOrcStorageRoom = AddRoom("Sewer Orc Storage Room");
            AddBidirectionalExits(oSewerPassage, oSewerOrcStorageRoom, BidirectionalExitType.WestEast);

            Room oSlopingSewerPassage = AddRoom("Sloping Sewer Passage");
            AddBidirectionalExits(oSewerOrcStorageRoom, oSlopingSewerPassage, BidirectionalExitType.NorthSouth);

            Room oSewerPassageInFrontOfGate = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSlopingSewerPassage, oSewerPassageInFrontOfGate, BidirectionalExitType.NorthSouth);

            Room oIgor = AddRoom("Igor");
            oIgor.Mob1 = "Igor";
            oIgor.Experience1 = 130;
            oIgor.Alignment = AlignmentType.Grey;
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            _breeStreetsGraph.Rooms[oIgor] = new System.Windows.Point(2, 6);

            Room oSnarlingMutt = AddRoom("Snarling Mutt");
            oSnarlingMutt.Mob1 = "Mutt";
            oSnarlingMutt.Experience1 = 50;
            oSnarlingMutt.Alignment = AlignmentType.Red;
            AddExit(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            AddExit(oSnarlingMutt, oToSnarSlystoneShoppe, "out");
            _breeStreetsGraph.Rooms[oSnarlingMutt] = new System.Windows.Point(9, 6);

            Room oGuido = AddRoom("Guido");
            oGuido.Mob1 = "Guido";
            oGuido.Experience1 = 350;
            oGuido.Alignment = AlignmentType.Red;
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            _breeStreetsGraph.Rooms[oGuido] = new System.Windows.Point(4, -0.5);

            Room oGodfather = AddRoom("Godfather");
            oGodfather.Mob1 = "Godfather";
            oGodfather.Experience1 = 1200;
            e = AddExit(oGuido, oGodfather, "door");
            e.Hidden = true;
            e.PreCommand = "open door";
            e = AddExit(oGodfather, oGuido, "door");
            e.PreCommand = "open door";
            _breeStreetsGraph.Rooms[oGodfather] = new System.Windows.Point(4, -1);

            Room oSergeantGrimdall = AddRoom("Sergeant Grimdall");
            oSergeantGrimdall.Mob1 = "Sergeant";
            oSergeantGrimdall.Experience1 = 350;
            oSergeantGrimdall.Alignment = AlignmentType.Blue;
            AddExit(oToBarracks, oSergeantGrimdall, "barracks");
            AddExit(oSergeantGrimdall, oToBarracks, "east");
            _breeStreetsGraph.Rooms[oSergeantGrimdall] = new System.Windows.Point(6, 8);

            Room oGuardsRecRoom = AddRoom("Guard's Rec Room");
            AddBidirectionalExits(oSergeantGrimdall, oGuardsRecRoom, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oGuardsRecRoom] = new System.Windows.Point(6, 8.5);

            oBigPapa.Mob1 = "papa";
            oBigPapa.Experience1 = 350;
            oBigPapa.Alignment = AlignmentType.Blue;

            Room oBreePawnShopWest = AddRoom("Ixell's Antique Shop");
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopWest] = new System.Windows.Point(2, 8);

            Room oBreePawnShopEast = AddRoom("Pawn Shop");
            AddBidirectionalExits(oPoorAlley1, oBreePawnShopEast, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBreePawnShopEast] = new System.Windows.Point(13, 4);

            Room oLeonardosFoundry = AddRoom("Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");
            _breeStreetsGraph.Rooms[oLeonardosFoundry] = new System.Windows.Point(9, 1);

            Room oLeonardosSwords = AddRoom("Leonardo's Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oLeonardosSwords] = new System.Windows.Point(9, 0.5);

            Room oZooEntrance = AddRoom("Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");
            _breeStreetsGraph.Rooms[oZooEntrance] = new System.Windows.Point(2, -0.5);

            Room oPathThroughScranlinsZoo = AddRoom("Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);
            _breeStreetsGraph.Rooms[oZooEntrance] = new System.Windows.Point(2, -1);

            Room oScranlinsPettingZoo = AddRoom("Scranlin's Petting Zoo");
            e = AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            e.OmitGo = true;
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");
            _breeStreetsGraph.Rooms[oScranlinsPettingZoo] = new System.Windows.Point(2, -1.5);

            Room oScranlinThreshold = AddRoom("Scranlin Threshold");
            e = AddExit(oScranlinsPettingZoo, oScranlinThreshold, "clearing");
            e.Hidden = true;
            AddExit(oScranlinThreshold, oScranlinsPettingZoo, "gate");
            _breeStreetsGraph.Rooms[oScranlinThreshold] = new System.Windows.Point(2, -2);

            Room oScranlin = AddRoom("Scranlin");
            oScranlin.Mob1 = oScranlinThreshold.Mob1 = "Scranlin";
            oScranlin.Experience1 = 500;
            oScranlin.Alignment = AlignmentType.Red;
            e = AddExit(oScranlinThreshold, oScranlin, "outhouse");
            e.Hidden = true;
            AddExit(oScranlin, oScranlinThreshold, "out");
            _breeStreetsGraph.Rooms[oScranlin] = new System.Windows.Point(2, -2.5);

            Room oTunnel = AddRoom("Tunnel");
            e = AddExit(breeSewers[0, 10], oTunnel, "tunnel");
            e.Hidden = true;
            AddExit(oTunnel, breeSewers[0, 10], "tunnel");

            Room oLatrine = AddRoom("Latrine");
            AddExit(oTunnel, oLatrine, "south");
            e = AddExit(oLatrine, oTunnel, "north");
            e.OmitGo = true;
            e.Hidden = true;

            Room oEugenesDungeon = AddRoom("Eugene's Dungeon");
            AddBidirectionalExits(oEugenesDungeon, oLatrine, BidirectionalExitType.SouthwestNortheast);

            Room oShadowOfIncendius = AddRoom("Shadow of Incendius");
            AddBidirectionalExits(oShadowOfIncendius, oEugenesDungeon, BidirectionalExitType.WestEast);

            Room oEugeneTheExecutioner = AddRoom("Eugene the Executioner");
            oEugeneTheExecutioner.IsTrapRoom = true;
            AddExit(oEugenesDungeon, oEugeneTheExecutioner, "up");

            Room oBurnedRemainsOfNimrodel = AddRoom("Nimrodel");
            oBurnedRemainsOfNimrodel.Mob1 = "Nimrodel";
            oBurnedRemainsOfNimrodel.Experience1 = 300;
            AddExit(oEugeneTheExecutioner, oBurnedRemainsOfNimrodel, "out");
            AddExit(oBurnedRemainsOfNimrodel, oEugeneTheExecutioner, "door");

            aqueduct = AddRoom("Aqueduct");
            AddExit(oBurnedRemainsOfNimrodel, aqueduct, "pipe");
            AddExit(aqueduct, oBurnedRemainsOfNimrodel, "out");

            _boatswain = AddRoom("Boatswain");
            _boatswain.Mob1 = "Boatswain";
            _boatswain.Experience1 = 350;
            AddLocation(_aShips, _boatswain);

            Room oPearlAlley = AddRoom("Pearl Alley");
            AddExit(oBreeTownSquare, oPearlAlley, "alley");
            AddExit(oPearlAlley, oBreeTownSquare, "north");
            _breeStreetsGraph.Rooms[oPearlAlley] = new System.Windows.Point(5, 3.5);

            Room oBartenderWaitress = AddRoom("Prancing Pony Bar/Wait");
            oBartenderWaitress.Mob1 = "Bartender";
            oBartenderWaitress.Mob2 = "Waitress";
            oBartenderWaitress.Experience1 = 15;
            oBartenderWaitress.Experience2 = 7;
            AddBidirectionalExits(oPearlAlley, oBartenderWaitress, BidirectionalExitType.WestEast);
            _breeStreetsGraph.Rooms[oBartenderWaitress] = new System.Windows.Point(6, 3.5);

            AddLocation(_aBreePerms, oOrderOfLove);
            AddLocation(_aBreePerms, oCampusFreeClinic);
            AddLocation(_aInaccessible, oGrant);
            AddLocation(_aBreePerms, oIgor);
            AddLocation(_aBreePerms, oGuido);
            AddLocation(_aBreePerms, oGodfather);
            AddLocation(_aBreePerms, oFallon);
            AddLocation(_aBreePerms, oSergeantGrimdall);
            AddLocation(_aBreePerms, oBigPapa);
            AddLocation(_aBreePerms, oScranlin);
            AddSubLocation(oScranlin, oScranlinThreshold);
            AddLocation(_aBreePerms, oDroolie);
            AddLocation(aBree, oSewerDemonThreshold);
            AddLocation(aBree, oPansy);
            AddLocation(aBree, oIxell);
            AddLocation(aBree, oSnarlingMutt);
            AddLocation(aBree, _breeDocks);
            AddLocation(aBree, oBreeTownSquare);
            AddLocation(aBree, oShadowOfIncendius);
            AddLocation(_aBreePerms, oBurnedRemainsOfNimrodel);
            AddLocation(_aBreePerms, oShirriff);
            AddLocation(aBree, oOohlgrist);
            AddLocation(aBree, oHermitsCave);
            AddLocation(aBree, oSewerOrcChamber);
            AddLocation(aBree, oBartenderWaitress);
        }

        private void AddGridBidirectionalExits(Room[,] grid, int x, int y)
        {
            Room r = grid[x, y];
            if (r != null)
            {
                _breeStreetsGraph.Rooms[r] = new System.Windows.Point(x, (10 - y));

                //look for a square to the west and add the east/west exits
                if (x > 0)
                {
                    Room roomToWest = grid[x - 1, y];
                    if (roomToWest != null)
                    {
                        AddBidirectionalExits(roomToWest, r, BidirectionalExitType.WestEast);
                    }
                }
                //look for a square to the south and add the north/south exits
                if (y > 0)
                {
                    Room roomToSouth = grid[x, y - 1];
                    if (roomToSouth != null)
                    {
                        AddBidirectionalExits(r, roomToSouth, BidirectionalExitType.NorthSouth);
                    }
                }
            }
        }

        /// <summary>
        /// adds rooms for mayor millwood's mansion
        /// </summary>
        /// <param name="oIxell">Ixell (entrance to mansion)</param>
        /// <param name="MansionLocations">mansion locations</param>
        private void AddMayorMillwoodMansion(Room oIxell, out List<Room> MansionLocations, out Room oChancellorOfProtection, out Room oMayorMillwood)
        {
            string sWarriorBard = "Warrior bard";

            Room oPathToMansion1 = AddRoom("Construction Site");
            AddExit(oIxell, oPathToMansion1, "back");
            AddExit(oPathToMansion1, oIxell, "hoist");

            Room oPathToMansion2 = AddRoom("Southern View");
            AddBidirectionalExits(oPathToMansion1, oPathToMansion2, BidirectionalExitType.NorthSouth);

            Room oPathToMansion3 = AddRoom("The South Wall");
            AddBidirectionalExits(oPathToMansion2, oPathToMansion3, BidirectionalExitType.NorthSouth);

            Room oPathToMansion4WarriorBardsx2 = AddRoom("Warrior Bards (Path)");
            oPathToMansion4WarriorBardsx2.Mob1 = sWarriorBard;
            oPathToMansion4WarriorBardsx2.Experience1 = 100;
            oPathToMansion4WarriorBardsx2.Alignment = AlignmentType.Red;
            AddExit(oPathToMansion3, oPathToMansion4WarriorBardsx2, "stone");
            AddExit(oPathToMansion4WarriorBardsx2, oPathToMansion3, "north");

            Room oPathToMansion5 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion4WarriorBardsx2, oPathToMansion5, BidirectionalExitType.SouthwestNortheast);

            Room oPathToMansion6 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion5, oPathToMansion6, BidirectionalExitType.NorthSouth);

            Room oPathToMansion7 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion6, oPathToMansion7, BidirectionalExitType.SoutheastNorthwest);

            Room oPathToMansion8 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion7, oPathToMansion8, BidirectionalExitType.NorthSouth);

            Room oPathToMansion9 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion8, oPathToMansion9, BidirectionalExitType.SoutheastNorthwest);

            Room oPathToMansion10 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion9, oPathToMansion10, BidirectionalExitType.SouthwestNortheast);

            Room oPathToMansion11 = AddRoom("Red Oak Tree");
            AddBidirectionalExits(oPathToMansion10, oPathToMansion11, BidirectionalExitType.NorthSouth);

            Room oPathToMansion12 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion11, oPathToMansion12, BidirectionalExitType.WestEast);

            Room oGrandPorch = AddRoom("Warrior Bard (Porch)");
            oGrandPorch.Mob1 = sWarriorBard;
            oGrandPorch.Experience1 = 100;
            oGrandPorch.Alignment = AlignmentType.Red;
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");

            Room oMansionInside1 = AddRoom("Mansion Inside");
            AddBidirectionalSameNameExit(oGrandPorch, oMansionInside1, "door", "open door");

            Room oMansionInside2 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionInside1, oMansionInside2, BidirectionalExitType.WestEast);

            Room oMansionFirstFloorToNorthStairwell1 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell1, oMansionInside2, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToNorthStairwell2 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell2, oMansionFirstFloorToNorthStairwell1, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToNorthStairwell3 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell3, oMansionFirstFloorToNorthStairwell2, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToNorthStairwell4 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell3, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToNorthStairwell5 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToNorthStairwell4, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.WestEast);

            Room oWarriorBardMansionNorth = AddRoom("Warrior Bard Mansion N");
            oWarriorBardMansionNorth.Mob1 = sWarriorBard;
            oWarriorBardMansionNorth.Experience1 = 100;
            oWarriorBardMansionNorth.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToSouthStairwell1 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToSouthStairwell1, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToSouthStairwell2 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell1, oMansionFirstFloorToSouthStairwell2, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToSouthStairwell3 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell2, oMansionFirstFloorToSouthStairwell3, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToSouthStairwell4 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell3, oMansionFirstFloorToSouthStairwell4, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToSouthStairwell5 = AddRoom("Long Hallway");
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell4, oMansionFirstFloorToSouthStairwell5, BidirectionalExitType.WestEast);

            Room oWarriorBardMansionSouth = AddRoom("Warrior Bard Mansion S");
            oWarriorBardMansionSouth.Mob1 = sWarriorBard;
            oWarriorBardMansionSouth.Experience1 = 100;
            oWarriorBardMansionSouth.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);

            Room oMansionFirstFloorToEastStairwell1 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionInside2, oMansionFirstFloorToEastStairwell1, BidirectionalExitType.WestEast);

            //note: this room has north and south exits to dungeons not currently included
            Room oMansionFirstFloorToEastStairwell2 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell1, oMansionFirstFloorToEastStairwell2, BidirectionalExitType.WestEast);

            Room oMansionFirstFloorToEastStairwell3 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell2, oMansionFirstFloorToEastStairwell3, BidirectionalExitType.WestEast);

            Room oMansionFirstFloorToEastStairwell4 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell3, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.WestEast);

            Room oMansionFirstFloorToEastStairwell5 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell4, BidirectionalExitType.SouthwestNortheast);

            Room oMansionFirstFloorToEastStairwell6 = AddRoom("Main Hallway");
            AddBidirectionalExits(oMansionFirstFloorToEastStairwell5, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.SoutheastNorthwest);

            Room oWarriorBardMansionEast = AddRoom("Warrior Bard Mansion E");
            oWarriorBardMansionEast.Mob1 = sWarriorBard;
            oWarriorBardMansionEast.Experience1 = 100;
            oWarriorBardMansionEast.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, oWarriorBardMansionEast, BidirectionalExitType.UpDown);

            Room oRoyalHallwayUpstairs = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oGrandStaircaseUpstairs, BidirectionalExitType.WestEast);

            Room oRoyalHallwayToMayor = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oRoyalHallwayToMayor, BidirectionalExitType.NorthSouth);

            Room oRoyalHallwayToChancellor = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayToChancellor, oRoyalHallwayUpstairs, BidirectionalExitType.NorthSouth);

            //mayor is immune to stun
            oMayorMillwood = AddRoom("Mayor Millwood");
            oMayorMillwood.Experience1 = 220;
            oMayorMillwood.Alignment = AlignmentType.Grey;
            Exit e = AddExit(oRoyalHallwayToMayor, oMayorMillwood, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oMayorMillwood, oRoyalHallwayToMayor, "out");
            oMayorMillwood.Mob1 = oRoyalHallwayToMayor.Mob1 = "mayor";

            oChancellorOfProtection = AddRoom("Chancellor of Protection");
            oChancellorOfProtection.Experience1 = 200;
            oChancellorOfProtection.Alignment = AlignmentType.Blue;
            e = AddExit(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oChancellorOfProtection, oRoyalHallwayToChancellor, "out");
            oChancellorOfProtection.Mob1 = oRoyalHallwayToChancellor.Mob1 = "chancellor";

            MansionLocations = new List<Room>
            {
                oPathToMansion4WarriorBardsx2,
                oGrandPorch,
                oWarriorBardMansionNorth,
                oWarriorBardMansionSouth,
                oWarriorBardMansionEast
            };
        }

        private void SetNightEdges(bool isNight)
        {
            foreach (Exit e in _nightEdges)
            {
                _map.RemoveEdge(e);
            }
            if (!isNight)
            {
                _nightEdges.Add(AddExit(_breeEastGateOutside, _breeEastGateInside, "gate"));
                _nightEdges.Add(AddExit(_imladrisWestGateOutside, _imladrisWestGateInside, "gate"));
            }
        }

        private void SetCelduinExpressEdges()
        {
            foreach (Exit e in _celduinExpressEdges)
            {
                _map.RemoveEdge(e);
            }
            if (cboCelduinExpress.SelectedItem.ToString() == "Bree")
            {
                _celduinExpressEdges.Add(AddExit(_breeDocks, _boatswain, "steamboat"));
                _celduinExpressEdges.Add(AddExit(_boatswain, _breeDocks, "dock"));
            }
        }

        private void AddBreeToImladris(Room oSewerPipeExit, Room sewerTunnelToTConnection)
        {
            _breeEastGateOutside = AddRoom("East Gate of Bree");
            AddExit(_breeEastGateInside, _breeEastGateOutside, "gate");

            Room oGreatEastRoad1 = AddRoom("Great East Road");
            AddBidirectionalExits(_breeEastGateOutside, oGreatEastRoad1, BidirectionalExitType.WestEast);
            AddToFarmHouseAndUglies(oGreatEastRoad1, out Room oOuthouse);

            Room oGreatEastRoad2 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad1, oGreatEastRoad2, BidirectionalExitType.WestEast);
            AddGalbasiDowns(oGreatEastRoad2);

            Room oGreatEastRoad3 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad2, oGreatEastRoad3, BidirectionalExitType.WestEast);

            Room oGreatEastRoad4 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad3, oGreatEastRoad4, BidirectionalExitType.WestEast);

            Room oGreatEastRoad5 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad4, oGreatEastRoad5, BidirectionalExitType.WestEast);

            Room oGreatEastRoad6 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad5, oGreatEastRoad6, BidirectionalExitType.WestEast);

            Room oGreatEastRoadGoblinAmbushGobLrgLrg = AddRoom("Gob Ambush #1");
            oGreatEastRoadGoblinAmbushGobLrgLrg.Mob1 = "goblin";
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience1 = 50;
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience2 = 50;
            oGreatEastRoadGoblinAmbushGobLrgLrg.Experience3 = 45;
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad6, BidirectionalExitType.SouthwestNortheast);

            Room oGreatEastRoad8 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoadGoblinAmbushGobLrgLrg, oGreatEastRoad8, BidirectionalExitType.SoutheastNorthwest);

            Room oGreatEastRoad9 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad8, oGreatEastRoad9, BidirectionalExitType.WestEast);

            Room oGreatEastRoad10 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad9, oGreatEastRoad10, BidirectionalExitType.WestEast);

            Room oGreatEastRoad11 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad10, oGreatEastRoad11, BidirectionalExitType.WestEast);

            Room oGreatEastRoad12 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad11, oGreatEastRoad12, BidirectionalExitType.WestEast);

            Room oGreatEastRoad13 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad12, oGreatEastRoad13, BidirectionalExitType.WestEast);

            Room oGreatEastRoad14 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad13, oGreatEastRoad14, BidirectionalExitType.WestEast);

            _imladrisWestGateOutside = _imladrisWestGateOutside = AddRoom("West Gate of Imladris");
            AddBidirectionalExits(oGreatEastRoad14, _imladrisWestGateOutside, BidirectionalExitType.WestEast);

            Room oCatchBasin = AddRoom("Catch Basin");
            AddExit(oOuthouse, oCatchBasin, "hole");
            AddExit(oCatchBasin, oOuthouse, "out");

            Room oSepticTank = AddRoom("Septic Tank");
            AddBidirectionalSameNameExit(oCatchBasin, oSepticTank, "grate");

            Room oDrainPipe1 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oSepticTank, oDrainPipe1, "pipe");

            Room oDrainPipe2 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oDrainPipe1, oDrainPipe2, "culvert");

            Room oBrandywineRiverShore = AddRoom("Southeastern Short of Brandywine River");
            AddExit(oDrainPipe2, oBrandywineRiverShore, "out");
            AddExit(oBrandywineRiverShore, oDrainPipe2, "grate");

            Room oSewerDitch = AddRoom("Sewer Ditch");
            AddExit(oBrandywineRiverShore, oSewerDitch, "ditch");
            AddExit(oSewerDitch, oBrandywineRiverShore, "out");

            Room oSewerTunnel1 = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel1, oSewerDitch, BidirectionalExitType.NorthSouth);

            Room oSewerTConnection = AddRoom("Sewer T-Connection");
            AddBidirectionalExits(oSewerTConnection, oSewerTunnel1, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(sewerTunnelToTConnection, oSewerTConnection, BidirectionalExitType.NorthSouth);

            Room oSewerTunnel2 = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);

            Room oSewerPipe = AddRoom("Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");

            Room oSalamander = AddRoom("Salamander");
            oSalamander.Mob1 = "Salamander";
            oSalamander.Experience1 = 100;
            oSalamander.Alignment = AlignmentType.Red;
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");

            Room oNorthBrethilForest1 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oGreatEastRoadGoblinAmbushGobLrgLrg, BidirectionalExitType.NorthSouth);

            Room oNorthBrethilForest2 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest1, oNorthBrethilForest2, BidirectionalExitType.WestEast);

            Room oDarkFootpath = AddRoom("Dark Footpath");
            AddBidirectionalExits(oDarkFootpath, oGreatEastRoad10, BidirectionalExitType.NorthSouth);
            AddBidirectionalExits(oNorthBrethilForest2, oDarkFootpath, BidirectionalExitType.WestEast);

            //North Brethil Forest
            Room oNorthBrethilForest3 = AddRoom("North Brethil Forest");
            AddBidirectionalExits(oNorthBrethilForest3, oDarkFootpath, BidirectionalExitType.NorthSouth);

            Room oNorthBrethilForest4GobAmbushThreshold = AddRoom("Gob Ambush #2 Threshold");
            oNorthBrethilForest4GobAmbushThreshold.Mob1 = "goblin";
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush #2");
            oNorthBrethilForest5GobAmbush.Mob1 = "goblin";
            oNorthBrethilForest5GobAmbush.Experience1 = 70;
            oNorthBrethilForest5GobAmbush.Experience2 = 50;
            oNorthBrethilForest5GobAmbush.Experience3 = 50;
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);

            //South Brethil Forest
            Room oDeepForest = AddRoom("Deep Forest");
            AddBidirectionalExits(oGreatEastRoad9, oDeepForest, BidirectionalExitType.NorthSouth);

            Room oBrethilForest = AddRoom("Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);

            Room oSpriteGuards = AddRoom("Sprite Guards");
            oSpriteGuards.Mob1 = "guard";
            oSpriteGuards.Experience1 = 120;
            oSpriteGuards.Experience2 = 120;
            oSpriteGuards.Alignment = AlignmentType.Blue;
            AddExit(oBrethilForest, oSpriteGuards, "brush");
            AddExit(oSpriteGuards, oBrethilForest, "east");

            AddLocation(_aBreePerms, oSalamander);
            AddLocation(_aBreePerms, oGreatEastRoadGoblinAmbushGobLrgLrg);
            AddLocation(_aBreePerms, oNorthBrethilForest5GobAmbush);
            AddSubLocation(oNorthBrethilForest5GobAmbush, oNorthBrethilForest4GobAmbushThreshold);
            AddLocation(_aBreePerms, oSpriteGuards);
            AddLocation(_aMisc, _breeEastGateOutside);
        }

        private void AddToFarmHouseAndUglies(Room oGreatEastRoad1, out Room oOuthouse)
        {
            Room oRoadToFarm1 = AddRoom("Farmland");
            AddBidirectionalExits(oGreatEastRoad1, oRoadToFarm1, BidirectionalExitType.NorthSouth);

            Room oRoadToFarm2 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm1, oRoadToFarm2, BidirectionalExitType.NorthSouth);

            Room oRoadToFarm3 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm2, oRoadToFarm3, BidirectionalExitType.NorthSouth);

            Room oRoadToFarm4 = AddRoom("Farmland");
            AddBidirectionalExits(oRoadToFarm3, oRoadToFarm4, BidirectionalExitType.NorthSouth);

            Room oRoadToFarm5 = AddRoom("Path to Ranch House");
            AddBidirectionalExits(oRoadToFarm5, oRoadToFarm4, BidirectionalExitType.WestEast);

            Room oRoadToFarm6 = AddRoom("Ranch House Front Steps");
            AddBidirectionalExits(oRoadToFarm6, oRoadToFarm5, BidirectionalExitType.WestEast);

            oOuthouse = AddRoom("Outhouse");
            AddBidirectionalExits(oRoadToFarm4, oOuthouse, BidirectionalExitType.WestEast);

            Room oSwimmingPond = AddRoom("Swimming Pond");
            AddExit(oOuthouse, oSwimmingPond, "pond");
            AddExit(oSwimmingPond, oOuthouse, "west");

            Room oMuddyPath = AddRoom("Muddy Path");
            AddExit(oSwimmingPond, oMuddyPath, "path");
            AddExit(oMuddyPath, oSwimmingPond, "pond");

            Room oSmallPlayground = AddRoom("Small Playground");
            AddBidirectionalExits(oSmallPlayground, oMuddyPath, BidirectionalExitType.SouthwestNortheast);

            Room oUglyKidSchoolEntrance = AddRoom("Ugly Kid School Entrance");
            AddBidirectionalSameNameExit(oSmallPlayground, oUglyKidSchoolEntrance, "gate");

            Room oMuddyFoyer = AddRoom("Muddy Foyer");
            if (_level < 11)
            {
                AddExit(oUglyKidSchoolEntrance, oMuddyFoyer, "front");
            }
            AddExit(oMuddyFoyer, oUglyKidSchoolEntrance, "out");

            Room oUglyKidClassroomK7 = AddRoom("Ugly Kid Classroom K-7");
            AddExit(oMuddyFoyer, oUglyKidClassroomK7, "classroom");
            AddExit(oUglyKidClassroomK7, oMuddyFoyer, "foyer");

            Room oHallway = AddRoom("Hallway");
            AddExit(oUglyKidClassroomK7, oHallway, "hallway");
            AddExit(oHallway, oUglyKidClassroomK7, "classroom");

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog");
            oRoadToFarm7HoundDog.Mob1 = "dog";
            oRoadToFarm7HoundDog.Experience1 = 150;
            oRoadToFarm7HoundDog.Alignment = AlignmentType.Blue;
            AddExit(oRoadToFarm7HoundDog, oRoadToFarm6, "out");
            AddExit(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Manager Mulloy Threshold");
            oFarmParlorManagerMulloyThreshold.Mob1 = "manager";
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", "open door");
            Room oManagerMulloy = AddRoom("Manager Mulloy");
            oManagerMulloy.Mob1 = "manager";
            oManagerMulloy.Experience1 = 600;
            oManagerMulloy.Alignment = AlignmentType.Blue;
            AddExit(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            AddExit(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "out");

            Room oFarmKitchen = AddRoom("Kitchen");
            AddExit(oFarmParlorManagerMulloyThreshold, oFarmKitchen, "kitchen");
            AddExit(oFarmKitchen, oFarmParlorManagerMulloyThreshold, "parlor");

            Room oFarmBackPorch = AddRoom("Back Porch");
            AddExit(oFarmKitchen, oFarmBackPorch, "backdoor");
            AddExit(oFarmBackPorch, oFarmKitchen, "kitchen");

            Room oFarmCat = AddRoom("Farm Cat");
            oFarmCat.Mob1 = "cat";
            oFarmCat.Experience1 = 550;
            AddExit(oFarmBackPorch, oFarmCat, "woodshed");
            Exit e = AddExit(oFarmCat, oFarmBackPorch, "out");
            e.NoFlee = true;

            Room oCrabbe = AddRoom("Crabbe");
            oCrabbe.Mob1 = "Crabbe";
            oCrabbe.Experience1 = 250;
            AddExit(oHallway, oCrabbe, "detention");
            AddExit(oCrabbe, oHallway, "out");

            Room oMrWartnose = AddRoom("Mr. Wartnose");
            oMrWartnose.Mob1 = "Wartnose";
            oMrWartnose.Experience1 = 235;
            AddExit(oUglyKidClassroomK7, oMrWartnose, "office");
            AddExit(oMrWartnose, oUglyKidClassroomK7, "out");

            AddLocation(_aBreePerms, oRoadToFarm7HoundDog);
            AddLocation(_aBreePerms, oManagerMulloy);
            AddLocation(_aBreePerms, oFarmCat);
            AddSubLocation(oManagerMulloy, oFarmParlorManagerMulloyThreshold);
            AddLocation(_aInaccessible, oMrWartnose);
            AddLocation(_aInaccessible, oCrabbe);
        }

        private void AddGalbasiDowns(Room oGreatEastRoad2)
        {
            Room oGalbasiDownsEntrance = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsEntrance, oGreatEastRoad2, BidirectionalExitType.NorthSouth);

            Room oGalbasiDownsNorth = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsEntrance, BidirectionalExitType.NorthSouth);

            Room oGalbasiDownsNortheast = AddRoom("Galbasi Downs");
            AddBidirectionalExits(oGalbasiDownsNorth, oGalbasiDownsNortheast, BidirectionalExitType.WestEast);

            Room oGalbasiDownsFurthestNorth = AddRoom("Galbasi Downs End");
            AddExit(oGalbasiDownsFurthestNorth, oGalbasiDownsNortheast, "southeast");
            Exit e = AddExit(oGalbasiDownsNortheast, oGalbasiDownsFurthestNorth, "northwest");
            e.Hidden = true;

            AddLocation(_aMisc, oGalbasiDownsFurthestNorth);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside)
        {
            _imladrisWestGateInside = AddRoom("West Gate of Imladris");
            AddExit(_imladrisWestGateInside, _imladrisWestGateOutside, "gate");

            Room oImladrisCircle1 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle1, _imladrisWestGateInside, BidirectionalExitType.SouthwestNortheast);

            Room oImladrisCircle2 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle2, oImladrisCircle1, BidirectionalExitType.SouthwestNortheast);

            Room oImladrisCircle3 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle2, BidirectionalExitType.SouthwestNortheast);

            Room oImladrisCircle4 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle3, oImladrisCircle4, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisCircle5 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle4, oImladrisCircle5, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisMainStreet1 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(_imladrisWestGateInside, oImladrisMainStreet1, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet2 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet1, oImladrisMainStreet2, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet3 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet2, oImladrisMainStreet3, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet4 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet3, oImladrisMainStreet4, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet5 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisMainStreet4, oImladrisMainStreet5, BidirectionalExitType.WestEast);

            Room oImladrisAlley3 = AddRoom("Side Alley North");
            AddBidirectionalExits(oImladrisMainStreet5, oImladrisAlley3, BidirectionalExitType.WestEast);

            Room oImladrisAlley4 = AddRoom("Side Alley North");
            AddBidirectionalExits(oImladrisAlley3, oImladrisAlley4, BidirectionalExitType.NorthSouth);

            Room oImladrisAlley5 = AddRoom("Side Alley South");
            AddBidirectionalExits(oImladrisAlley4, oImladrisAlley5, BidirectionalExitType.NorthSouth);

            Room oImladrisSmallAlley1 = AddRoom("Small Alley");
            AddExit(oImladrisAlley3, oImladrisSmallAlley1, "alley");
            AddExit(oImladrisSmallAlley1, oImladrisAlley3, "south");

            Room oImladrisSmallAlley2 = AddRoom("Small Alley");
            AddBidirectionalExits(oImladrisSmallAlley2, oImladrisSmallAlley1, BidirectionalExitType.NorthSouth);

            Room oImladrisPawnShop = AddRoom("Sharkey's Pawn Shop");
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door");

            Room oImladrisTownCircle = AddRoom("Imladris Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet6 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);

            Room oEastGateOfImladrisInside = AddRoom("East Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle5, oEastGateOfImladrisInside, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);

            Room oEastGateOfImladrisOutside = AddRoom("Gates of Imladris");
            AddBidirectionalSameNameExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate");

            Room oImladrisCircle6 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oEastGateOfImladrisInside, oImladrisCircle6, BidirectionalExitType.SouthwestNortheast);

            Room oImladrisCircle7 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle6, oImladrisCircle7, BidirectionalExitType.SouthwestNortheast);

            Room oImladrisCircle10 = AddRoom("Imladris Circle");
            AddBidirectionalExits(_imladrisWestGateInside, oImladrisCircle10, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisCircle9 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle10, oImladrisCircle9, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisCircle8 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle9, oImladrisCircle8, BidirectionalExitType.SoutheastNorthwest);
            AddBidirectionalExits(oImladrisCircle7, oImladrisCircle8, BidirectionalExitType.SouthwestNortheast);
            AddExit(oImladrisAlley5, oImladrisCircle8, "south");

            Room oRearAlley = AddRoom("Master Assassin Threshold"); //Rear Alley
            AddBidirectionalExits(oImladrisCircle10, oRearAlley, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);

            Room oPoisonedDagger = AddRoom("Master Assassins");
            oPoisonedDagger.Mob1 = oRearAlley.Mob1 = "assassin";
            oPoisonedDagger.Experience1 = 600;
            oPoisonedDagger.Experience2 = 600;
            AddBidirectionalSameNameExit(oRearAlley, oPoisonedDagger, "door");

            oImladrisSouthGateInside = AddRoom("Southern Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);

            Room oImladrisCityDump = AddRoom("Imladris City Dump");
            Exit e = AddExit(oImladrisCircle8, oImladrisCityDump, "north");
            e.OmitGo = true;
            AddExit(oImladrisCityDump, oImladrisCircle8, "south");
            e = AddExit(oImladrisCityDump, oRearAlley, "north");
            e.Hidden = true;

            Room oImladrisHealingHand = AddRoom("Imladris Healing Hand");
            oImladrisHealingHand.IsHealingRoom = true;
            AddBidirectionalExits(oImladrisHealingHand, oImladrisMainStreet5, BidirectionalExitType.NorthSouth);

            Room oTyriesPriestSupplies = AddRoom("Tyrie's Priest Supplies");
            AddBidirectionalExits(oImladrisMainStreet5, oTyriesPriestSupplies, BidirectionalExitType.NorthSouth);

            Room oMountainPath1 = AddRoom("Mountain Path");
            AddBidirectionalExits(oEastGateOfImladrisOutside, oMountainPath1, BidirectionalExitType.WestEast);

            Room oMountainPath2 = AddRoom("Mountain Path");
            AddBidirectionalExits(oMountainPath2, oMountainPath1, BidirectionalExitType.SouthwestNortheast);

            Room oMountainTrail1 = AddRoom("Mountain Trail");
            AddBidirectionalExits(oMountainTrail1, oMountainPath2, BidirectionalExitType.NorthSouth);

            Room oMountainTrail2 = AddRoom("Mountain Trail");
            AddBidirectionalExits(oMountainTrail2, oMountainTrail1, BidirectionalExitType.SouthwestNortheast);

            Room oIorlas = AddRoom("Iorlas");
            oIorlas.Mob1 = "Iorlas";
            oIorlas.Experience1 = 200;
            oIorlas.Alignment = AlignmentType.Grey;
            AddExit(oMountainTrail2, oIorlas, "shack");
            AddExit(oIorlas, oMountainTrail2, "door");

            AddLocation(_aImladrisTharbadPerms, oImladrisHealingHand);
            AddLocation(_aImladrisTharbadPerms, oIorlas);
            AddLocation(_aImladrisTharbadPerms, oPoisonedDagger);
            AddSubLocation(oPoisonedDagger, oRearAlley);
            AddLocation(_aMisc, oImladrisPawnShop);
            AddLocation(_aMisc, oTyriesPriestSupplies);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room aqueduct)
        {
            Room oBreeWestGateOutside = AddRoom("West Gate of Bree");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate");

            Room oLeviathanNorthForkWestern = AddRoom("The Grand Intersection - Leviathan Way/North Fork Road/Western Road");
            AddBidirectionalExits(oLeviathanNorthForkWestern, oBreeWestGateOutside, BidirectionalExitType.WestEast);

            Room oNorthFork1 = AddRoom("North Fork Road");
            AddBidirectionalExits(oNorthFork1, oLeviathanNorthForkWestern, BidirectionalExitType.SoutheastNorthwest);

            Room oWesternRoad1 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad1, oLeviathanNorthForkWestern, BidirectionalExitType.WestEast);

            Room oWesternRoad2 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad2, oWesternRoad1, BidirectionalExitType.WestEast);

            Room oWesternRoad3 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad3, oWesternRoad2, BidirectionalExitType.WestEast);

            Room oWesternRoad4 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad4, oWesternRoad3, BidirectionalExitType.WestEast);

            Room oWesternRoad5 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad5, oWesternRoad4, BidirectionalExitType.WestEast);

            Room oWesternRoad6 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad6, oWesternRoad5, BidirectionalExitType.WestEast);

            Room oWesternRoad7 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad7, oWesternRoad6, BidirectionalExitType.WestEast);

            Room oWesternRoad8 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad8, oWesternRoad7, BidirectionalExitType.WestEast);

            Room oWesternRoad9 = AddRoom("Western Road");
            AddBidirectionalExits(oWesternRoad9, oWesternRoad8, BidirectionalExitType.WestEast);

            Room oVillageOfHobbiton1 = AddRoom("Village of Hobbiton");
            AddBidirectionalExits(oVillageOfHobbiton1, oWesternRoad9, BidirectionalExitType.WestEast);

            Room oMainSquareOfHobbiton = AddRoom("Main Square of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton1, BidirectionalExitType.WestEast);

            Room oVillageOfHobbiton2 = AddRoom("Village of Hobbiton");
            AddBidirectionalExits(oMainSquareOfHobbiton, oVillageOfHobbiton2, BidirectionalExitType.NorthSouth);

            Room oValleyRoad = AddRoom("Valley Road");
            AddBidirectionalExits(oVillageOfHobbiton2, oValleyRoad, BidirectionalExitType.NorthSouth);

            Room oHillAtBagEnd = AddRoom("Hill at Bag End");
            AddBidirectionalExits(oValleyRoad, oHillAtBagEnd, BidirectionalExitType.SoutheastNorthwest);

            Room oBilboFrodoHobbitHoleCondo = AddRoom("Bilbo/Frodo Hobbit Hole Condo");
            AddExit(oHillAtBagEnd, oBilboFrodoHobbitHoleCondo, "down");
            AddExit(oBilboFrodoHobbitHoleCondo, oHillAtBagEnd, "out");

            Room oBilboFrodoCommonArea = AddRoom("Common Area");
            AddBidirectionalSameNameExit(oBilboFrodoHobbitHoleCondo, oBilboFrodoCommonArea, "curtain");

            Room oEastwingHallway = AddRoom("Eastwing Hallway");
            AddExit(oBilboFrodoCommonArea, oEastwingHallway, "eastwing");
            AddExit(oEastwingHallway, oBilboFrodoCommonArea, "common");

            Room oSouthwingHallway = AddRoom("Southwing Hallway");
            AddExit(oEastwingHallway, oSouthwingHallway, "southwing");
            AddExit(oSouthwingHallway, oEastwingHallway, "eastwing");

            Room oBilboBaggins = AddRoom("Bilbo Baggins");
            oBilboBaggins.Mob1 = "Bilbo";
            oBilboBaggins.Experience1 = 260;
            oBilboBaggins.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oSouthwingHallway, oBilboBaggins, "oakdoor", "open oakdoor");

            Room oFrodoBaggins = AddRoom("Frodo Baggins");
            oFrodoBaggins.Mob1 = "Frodo";
            oFrodoBaggins.Experience1 = 260;
            oFrodoBaggins.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain");

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes");
            AddExit(oGreatHallOfHeroes, oLeviathanNorthForkWestern, "out");
            AddExit(oLeviathanNorthForkWestern, oGreatHallOfHeroes, "hall");

            //something is hasted
            Room oSomething = AddRoom("Something");
            oSomething.Mob1 = "Something";
            oSomething.Experience1 = 140;
            if (_level < 11)
            {
                Exit e = AddExit(oGreatHallOfHeroes, oSomething, "curtain");
                e.Hidden = true;
            }
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");

            Room oShepherd = AddRoom("Shepherd");
            oShepherd.Mob1 = "Shepherd";
            oShepherd.Experience1 = 60;
            oShepherd.Alignment = AlignmentType.Blue;
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");

            Room oSmoulderingVillage = AddRoom("Smoldering Village");
            //Gate is locked (and knocking doesn't work) so not treating as an exit. This is only accessible from the other way around.
            //AddExit(oShepherd, oSmoulderingVillage, "gate");
            AddExit(oSmoulderingVillage, oShepherd, "gate");

            Room oWell = AddRoom("Well");
            AddExit(oSmoulderingVillage, oWell, "well");
            AddExit(oWell, oSmoulderingVillage, "ladder");

            Room oKasnarTheGuard = AddRoom("Kasnar");
            oKasnarTheGuard.Mob1 = "Kasnar";
            oKasnarTheGuard.Experience1 = 535;
            AddExit(oWell, oKasnarTheGuard, "pipe");
            AddExit(oKasnarTheGuard, oWell, "north");

            AddExit(aqueduct, oKasnarTheGuard, "north");
            //AddExit(oKasnarTheGuard, aqueduct, "south") //Exit is locked and knockable but not treating as an exit for the mapping

            AddLocation(_aInaccessible, oSomething);
            AddLocation(_aBreePerms, oBilboBaggins);
            AddLocation(_aBreePerms, oFrodoBaggins);
            AddLocation(_aBreePerms, oShepherd);
            AddLocation(_aBreePerms, oKasnarTheGuard);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            Room oMistyTrail1 = AddRoom("Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate");

            Room oBrunskidTradersGuild1 = AddRoom("Brunskid Trader's Guild Store Front");
            AddBidirectionalExits(oBrunskidTradersGuild1, oMistyTrail1, BidirectionalExitType.WestEast);

            Room oCutthroatAssassinThreshold = AddRoom("Hiester Assassin Cutthroat Threshold");
            AddBidirectionalExits(oCutthroatAssassinThreshold, oBrunskidTradersGuild1, BidirectionalExitType.WestEast);

            Room oCutthroatAssassin = AddRoom("Hiester Assassin Cutthroat");
            AddBidirectionalExits(oCutthroatAssassin, oCutthroatAssassinThreshold, BidirectionalExitType.WestEast);
            oCutthroatAssassin.Mob1 = "hiester";
            oCutthroatAssassin.Mob2 = "assassin";
            oCutthroatAssassin.Mob3 = "cutthroat";
            oCutthroatAssassin.Experience1 = 1200;
            oCutthroatAssassin.Experience2 = 600;
            oCutthroatAssassin.Experience3 = 500;

            Room oMistyTrail2 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);

            Room oMistyTrail3 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);

            Room oMistyTrail4 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);

            Room oPotionFactoryReception = AddRoom("Potion Factory Guard");
            AddBidirectionalExits(oPotionFactoryReception, oMistyTrail4, BidirectionalExitType.WestEast);
            oPotionFactoryReception.Mob1 = "Guard";
            oPotionFactoryReception.Experience1 = 110;

            Room oPotionFactoryAdministrativeOffices = AddRoom("Potion Factory Administrative Offices");
            AddBidirectionalExits(oPotionFactoryReception, oPotionFactoryAdministrativeOffices, BidirectionalExitType.NorthSouth);

            Room oMarkFrey = AddRoom("Mark Frey");
            oMarkFrey.Mob1 = "Frey";
            oMarkFrey.Experience1 = 450;
            AddExit(oPotionFactoryAdministrativeOffices, oMarkFrey, "door");
            AddExit(oMarkFrey, oPotionFactoryAdministrativeOffices, "out");

            Room oMistyTrail5 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail4, oMistyTrail5, BidirectionalExitType.NorthSouth);

            Room oMistyTrail6 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail5, oMistyTrail6, BidirectionalExitType.NorthSouth);

            Room oMistyTrail7 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail6, oMistyTrail7, BidirectionalExitType.NorthSouth);

            Room oMistyTrail8 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail7, oMistyTrail8, BidirectionalExitType.NorthSouth);

            Room oMistyTrail9 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail8, oMistyTrail9, BidirectionalExitType.NorthSouth);

            Room oMistyTrail10 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail9, oMistyTrail10, BidirectionalExitType.SouthwestNortheast);

            Room oMistyTrail11 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail10, oMistyTrail11, BidirectionalExitType.SouthwestNortheast);

            Room oMistyTrail12 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail11, oMistyTrail12, BidirectionalExitType.NorthSouth);

            Room oMistyTrail13 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail12, oMistyTrail13, BidirectionalExitType.SouthwestNortheast);

            Room oMistyTrail14 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail13, oMistyTrail14, BidirectionalExitType.SouthwestNortheast);

            oTharbadGateOutside = AddRoom("North Gate of Tharbad");
            AddBidirectionalExits(oMistyTrail14, oTharbadGateOutside, BidirectionalExitType.NorthSouth);

            //Shanty Town
            Room oRuttedDirtRoad = AddRoom("Rutted Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);

            Room oNorthEdgeOfShantyTown = AddRoom("North Edge of Shanty Town");
            AddBidirectionalExits(oRuttedDirtRoad, oNorthEdgeOfShantyTown, BidirectionalExitType.NorthSouth);

            Room oRedFoxLane = AddRoom("Red Fox Lane");
            AddBidirectionalExits(oRedFoxLane, oNorthEdgeOfShantyTown, BidirectionalExitType.WestEast);

            Room oGypsyCamp = AddRoom("Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp, oRedFoxLane, BidirectionalExitType.SoutheastNorthwest);

            Room oNorthShantyTown = AddRoom("North Shanty Town");
            AddBidirectionalExits(oRedFoxLane, oNorthShantyTown, BidirectionalExitType.SouthwestNortheast);

            Room oShantyTownDump = AddRoom("Shanty Town Dump");
            AddBidirectionalExits(oNorthShantyTown, oShantyTownDump, BidirectionalExitType.SouthwestNortheast);

            Room oShantyTownWest = AddRoom("Shanty Town West");
            AddBidirectionalExits(oShantyTownDump, oShantyTownWest, BidirectionalExitType.NorthSouth);

            Room oShantyTown1 = AddRoom("Shanty Town");
            AddBidirectionalExits(oNorthEdgeOfShantyTown, oShantyTown1, BidirectionalExitType.NorthSouth);

            Room oShantyTown2 = AddRoom("Shanty Town");
            AddBidirectionalExits(oShantyTown1, oShantyTown2, BidirectionalExitType.NorthSouth);

            Room oPrinceBrunden = AddRoom("Prince Brunden");
            oPrinceBrunden.Mob1 = "Prince";
            oPrinceBrunden.Experience1 = 150;
            oPrinceBrunden.Alignment = AlignmentType.Blue;
            AddExit(oGypsyCamp, oPrinceBrunden, "wagon");
            AddExit(oPrinceBrunden, oGypsyCamp, "out");

            Room oNaugrim = AddRoom("Naugrim");
            oNaugrim.Mob1 = "Naugrim";
            oNaugrim.Experience1 = 205;
            oNaugrim.Alignment = AlignmentType.Red;
            AddExit(oNorthShantyTown, oNaugrim, "cask");
            AddExit(oNaugrim, oNorthShantyTown, "out");

            Room oHogoth = AddRoom("Hogoth");
            oHogoth.Mob1 = "Hogoth";
            oHogoth.Experience1 = 260;
            oHogoth.Alignment = AlignmentType.Blue;
            AddExit(oShantyTownWest, oHogoth, "shack");
            AddExit(oHogoth, oShantyTownWest, "out");

            Room oFaornil = AddRoom("Faornil");
            oFaornil.Mob1 = "Faornil";
            oFaornil.Experience1 = 250;
            oFaornil.Alignment = AlignmentType.Red;
            AddExit(oShantyTown1, oFaornil, "tent");
            AddExit(oFaornil, oShantyTown1, "out");

            Room oGraddy = AddRoom("Graddy");
            oGraddy.Mob1 = "Graddy";
            oGraddy.Experience1 = 350;
            AddExit(oShantyTown2, oGraddy, "wagon");
            AddExit(oGraddy, oShantyTown2, "out");

            Room oGraddyOgre = AddRoom("Graddy Ogre");
            oGraddyOgre.Mob1 = "Ogre";
            oGraddyOgre.Experience1 = 150;
            Exit e = AddExit(oGraddy, oGraddyOgre, "gate");
            e.PreCommand = "open gate";
            e = AddExit(oGraddyOgre, oGraddy, "gate");
            e.PreCommand = "open gate";

            AddLocation(_aImladrisTharbadPerms, oCutthroatAssassin);
            AddSubLocation(oCutthroatAssassin, oCutthroatAssassinThreshold);
            AddLocation(_aImladrisTharbadPerms, oPrinceBrunden);
            AddLocation(_aImladrisTharbadPerms, oNaugrim);
            AddLocation(_aImladrisTharbadPerms, oHogoth);
            AddLocation(_aImladrisTharbadPerms, oFaornil);
            AddLocation(_aImladrisTharbadPerms, oGraddy);
            AddLocation(_aImladrisTharbadPerms, oGraddyOgre);
            AddLocation(_aImladrisTharbadPerms, oMarkFrey);
            AddLocation(_aImladrisTharbadPerms, oPotionFactoryReception);
        }

        private void AddIntangible(Room oBreeTownSquare)
        {
            Area oIntangible = _areasByName[AREA_INTANGIBLE];

            _treeOfLife = AddRoom("Tree of Life");
            AddExit(_treeOfLife, oBreeTownSquare, "down");

            Room oLimbo = AddRoom("Limbo");
            Exit e = AddExit(oLimbo, _treeOfLife, "green");
            e.PreCommand = "open green";

            AddLocation(oIntangible, _treeOfLife);
            AddLocation(oIntangible, oLimbo);
        }

        private Room AddRoom(string roomName)
        {
            Room r = new Room(roomName);
            _map.AddVertex(r);
            return r;
        }

        private void AddRoomVariableValue(Room r, string variableName, string variableValue)
        {
            if (!_variablesByName.TryGetValue(variableName, out Variable v))
            {
                MessageBox.Show("AddRoomVariableValue: failed to find variable " + variableName);
                return;
            }
            switch (v.Type)
            {
                case VariableType.Bool:
                    if (!bool.TryParse(variableValue, out bool bValue))
                    {
                        MessageBox.Show("AddRoomVariableValue: failed to parse boolean for " + variableValue);
                        return;
                    }
                    break;
                case VariableType.Int:
                    if (!int.TryParse(variableValue, out int iValue))
                    {
                        MessageBox.Show("AddRoomVariableValue: failed to parse int for " + variableValue);
                        return;
                    }
                    IntegerVariable iv = (IntegerVariable)v;
                    if (iv.Min.HasValue && iValue < iv.Min.Value)
                    {
                        MessageBox.Show("AddRoomVariableValue: integer value less than min value " + variableValue);
                        return;
                    }
                    if (iv.Max.HasValue && iValue > iv.Max.Value)
                    {
                        MessageBox.Show("AddRoomVariableValue: integer value greater than max value: " + variableValue);
                        return;
                    }
                    break;
                case VariableType.String:
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (r.VariableValues == null) r.VariableValues = new Dictionary<Variable, string>();
            r.VariableValues[v] = variableValue;
        }

        private Exit AddExit(Room a, Room b, string exitText)
        {
            Exit e = new Exit(a, b, exitText);
            _map.AddEdge(e);
            return e;
        }

        private void AddLocation(Area area, Room locRoom)
        {
            area.Locations.Add(locRoom);
        }

        private void AddSubLocation(Room locRoom, Room subRoom)
        {
            if (locRoom.SubLocations == null) locRoom.SubLocations = new List<Room>();
            locRoom.SubLocations.Add(subRoom);
            subRoom.ParentRoom = locRoom;
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText)
        {
            AddBidirectionalSameNameExit(aRoom, bRoom, exitText, null);
        }

        private void AddBidirectionalSameNameExit(Room aRoom, Room bRoom, string exitText, string preCommand)
        {
            Exit e = new Exit(aRoom, bRoom, exitText);
            e.PreCommand = preCommand;
            _map.AddEdge(e);
            e = new Exit(bRoom, aRoom, exitText);
            e.PreCommand = preCommand;
            _map.AddEdge(e);
        }

        private void AddBidirectionalExits(Room aRoom, Room bRoom, BidirectionalExitType exitType)
        {
            string exitAtoB = string.Empty;
            string exitBtoA = string.Empty;
            switch (exitType)
            {
                case BidirectionalExitType.WestEast:
                    exitAtoB = "east";
                    exitBtoA = "west";
                    break;
                case BidirectionalExitType.NorthSouth:
                    exitAtoB = "south";
                    exitBtoA = "north";
                    break;
                case BidirectionalExitType.SoutheastNorthwest:
                    exitAtoB = "southeast";
                    exitBtoA = "northwest";
                    break;
                case BidirectionalExitType.SouthwestNortheast:
                    exitAtoB = "southwest";
                    exitBtoA = "northeast";
                    break;
                case BidirectionalExitType.UpDown:
                    exitAtoB = "down";
                    exitBtoA = "up";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            _map.AddEdge(new Exit(aRoom, bRoom, exitAtoB));
            _map.AddEdge(new Exit(bRoom, aRoom, exitBtoA));
        }

        private enum BidirectionalExitType
        {
            WestEast,
            NorthSouth,
            SoutheastNorthwest,
            SouthwestNortheast,
            UpDown,
        }

        internal class Area
        {
            public Area(string name)
            {
                this.Name = name;
                this.Locations = new List<Room>();
            }

            public override string ToString()
            {
                return this.Name;
            }

            public string Name { get; set; }
            public List<Room> Locations { get; set; }
        }

        public enum AlignmentType
        {
            Blue,
            Grey,
            Red,
        }

        private void SendCommand(string command, bool IsPassword)
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
            if (!string.IsNullOrEmpty(command))
            {
                string sToConsole;
                if (IsPassword)
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
                    _newConsoleText.Add(sToConsole);
                    _newConsoleText.Add(Environment.NewLine);
                }
            }
        }

        private string ValidateSpecifiedObject(ObjectType objType, out string errorMessage)
        {
            errorMessage = string.Empty;
            TextBox txt = null;
            string value = string.Empty;
            switch (objType)
            {
                case ObjectType.Mob:
                    txt = txtMob;
                    break;
                case ObjectType.Weapon:
                    txt = txtWeapon;
                    break;
                case ObjectType.Wand:
                    txt = txtWand;
                    break;
                case ObjectType.Potion:
                    txt = txtPotion;
                    break;
                case ObjectType.Realm1Spell:
                    if (radEarth.Checked)
                        value = "rumble";
                    else if (radWind.Checked)
                        value = "hurt";
                    else if (radFire.Checked)
                        value = "burn";
                    else if (radWater.Checked)
                        value = "blister";
                    else
                        throw new InvalidOperationException();
                    break;
                case ObjectType.Realm2Spell:
                    if (radEarth.Checked)
                        value = "crush";
                    else if (radWind.Checked)
                        value = "dustgust";
                    else if (radFire.Checked)
                        value = "fireball";
                    else if (radWater.Checked)
                        value = "waterbolt";
                    else
                        throw new InvalidOperationException();
                    break;
                case ObjectType.Realm3Spell:
                    if (radEarth.Checked)
                        value = "shatterstone";
                    else if (radWind.Checked)
                        value = "shockbolt";
                    else if (radFire.Checked)
                        value = "burstflame";
                    else if (radWater.Checked)
                        value = "steamblast";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (txt != null)
            {
                value = txt.Text;
                if (string.IsNullOrEmpty(value))
                {
                    errorMessage = "No " + objType + " specified.";
                    value = null;
                }
            }
            return value;
        }

        private string TranslateCommand(string input, IEnumerable<Variable> variables, out string errorMessage)
        {
            input = input ?? string.Empty;
            string specifiedValue;
            errorMessage = string.Empty;
            input = TranslateVariables(input, variables);
            foreach (ObjectType ot in Enum.GetValues(typeof(ObjectType)))
            {
                string lowerOT = ot.ToString().ToLower();
                string replacement = "{" + lowerOT + "}";
                if (input.Contains(replacement))
                {
                    specifiedValue = ValidateSpecifiedObject(ot, out errorMessage);
                    if (string.IsNullOrEmpty(specifiedValue))
                    {
                        return null;
                    }
                    else
                    {
                        input = input.Replace(replacement, specifiedValue);
                    }
                }
            }
            input = TranslateVariables(input, variables);
            return input;
        }

        private string TranslateVariables(string input, IEnumerable<Variable> variables)
        {
            foreach (Variable v in variables)
            {
                string sValue;
                if (v.Type == VariableType.Bool)
                {
                    sValue = ((BooleanVariable)v).Value.ToString();
                }
                else if (v.Type == VariableType.Int)
                {
                    sValue = ((IntegerVariable)v).Value.ToString();
                }
                else if (v.Type == VariableType.String)
                {
                    sValue = ((StringVariable)v).Value;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                input = input.Replace("{" + v.Name.ToLower() + "}", sValue);
            }
            return input;
        }

        private void btnOtherSingleMove_Click(object sender, EventArgs e)
        {
            string move = Interaction.InputBox("Move:", "Enter Move", string.Empty);
            if (!string.IsNullOrEmpty(move))
            {
                DoSingleMove(chkExecuteMove.Checked, move, "go " + move);
            }
        }

        private void btnDoSingleMove_Click(object sender, EventArgs e)
        {
            string direction = ((Button)sender).Tag.ToString();
            string cmd;
            if (direction == "north" || direction == "south")
            {
                cmd = direction;
            }
            else
            {
                cmd = "go " + direction;
            }
            DoSingleMove(chkExecuteMove.Checked, direction, cmd);
        }

        private void DoSingleMove(bool move, Exit exit)
        {
            if (move)
            {
                RunPreExitLogic(null, exit.PreCommand, exit.Target);
                string nextCommand = exit.ExitText;
                if (!exit.OmitGo) nextCommand = "go " + nextCommand;
                SendCommand(nextCommand, false);
            }
            SetCurrentRoom(exit.Target);
        }

        private void DoSingleMove(bool move, string direction, string command)
        {
            if (m_oCurrentRoom != null)
            {
                Exit foundExit = null;
                if (_map.TryGetOutEdges(m_oCurrentRoom, out IEnumerable<Exit> edges))
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
                SendCommand(command, false);
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

        private void btnQuit_Click(object sender, EventArgs e)
        {
            _currentBackgroundParameters = GenerateNewBackgroundParameters();
            _currentBackgroundParameters.Quit = true;
            RunCommands(new List<MacroStepBase>(), _currentBackgroundParameters);
        }

        private void btnDoAction_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string command;
            CommandButtonTag cmdButtonTag = btn.Tag as CommandButtonTag;
            if (cmdButtonTag != null)
            {
                command = cmdButtonTag.Command;
            }
            else
            {
                command = btn.Tag.ToString();
            }
            command = TranslateCommand(command, _variables, out string errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (_currentBackgroundParameters != null) //queue to background process
                {
                    lock (_queuedCommandLock)
                    {
                        _currentBackgroundParameters.QueuedCommand = command;
                    }
                }
                else //send the command to the telnet window
                {
                    SendCommand(command, false);
                }
            }
            else
                MessageBox.Show(errorMessage);
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

        private void RunCommands(List<MacroStepBase> commands, BackgroundWorkerParameters backgroundParameters)
        {
            Dictionary<string, Variable> copyVariables = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            foreach (Variable v in _variables)
            {
                copyVariables[v.Name] = Variable.CopyVariable(v);
            }
            backgroundParameters.Commands = commands;
            backgroundParameters.Variables = copyVariables;
            _bw.RunWorkerAsync(backgroundParameters);
            ToggleBackgroundProcess(true);
        }

        private class BackgroundWorkerParameters
        {
            public Room TargetRoom { get; set; }
            public List<MacroStepBase> Commands { get; set; }
            public Exit PreExit { get; set; }
            public Dictionary<string, Variable> Variables { get; set; }
            public int? NextCommandWaitMS { get; set; }
            public int WaitMS { get; set; }
            public bool Cancelled { get; set; }
            public bool SetTargetRoomIfCancelled { get; set; }
            public int CommandsRun { get; set; }
            public Macro Macro { get; set; }
            public string QueuedCommand { get; set; }
            public string FinalCommand { get; set; }
            public string FinalCommand2 { get; set; }
            public int MaxOffensiveLevel { get; set; }
            public bool AutoMana { get; set; }
            public PromptedSkills UsedSkills { get; set; }
            public bool AutoHazy { get; set; }
            public bool Flee { get; set; }
            public bool Quit { get; set; }
            public bool DoScore { get; set; }

            public IEnumerable<Variable> GetVariables()
            {
                foreach (Variable v in Variables.Values)
                {
                    yield return v;
                }
            }
        }

        private void Alg_TreeEdge(Exit e)
        {
            _pathMapping[e.Target] = e;
            if (e.Target == _currentBackgroundParameters.TargetRoom)
            {
                _currentSearch.Abort();
            }
        }

        private void chkIsNight_CheckedChanged(object sender, EventArgs e)
        {
            SetNightEdges(chkIsNight.Checked);
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
            SendCommand(command, false);
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

        private void btnRunMacro_Click(object sender, EventArgs e)
        {
            RunMacro((Macro)cboMacros.SelectedItem, null);
        }

        private void RunMacro(Macro m, Exit preExit)
        {
            bool isMeleeMacro = ((m.CombatCommandTypes & CommandType.Melee) == CommandType.Melee);
            bool hasWeapon = !string.IsNullOrEmpty(txtWeapon.Text);
            if (isMeleeMacro && !hasWeapon)
            {
                MessageBox.Show("No weapon specified.");
                return;
            }

            PromptedSkills activatedSkills = PromptedSkills.None;

            if (m.DoSkills)
            {
                bool promptPowerAttack = isMeleeMacro && txtPowerAttackTime.Text == "0:00";
                bool promptManashield = txtManashieldTime.Text == "0:00";

                PromptedSkills skills = PromptedSkills.None;
                if (promptPowerAttack) skills |= PromptedSkills.PowerAttack;
                if (promptManashield) skills |= PromptedSkills.Manashield;

                if (skills != PromptedSkills.None)
                {
                    using (frmPromptSkills frmSkills = new frmPromptSkills(skills))
                    {
                        if (frmSkills.ShowDialog(this) != DialogResult.OK)
                        {
                            return;
                        }
                        activatedSkills = frmSkills.SelectedSkills;
                    }
                }
            }

            bool powerAttack = (activatedSkills & PromptedSkills.PowerAttack) == PromptedSkills.PowerAttack;
            ((StringVariable)_variablesByName["attacktype"]).Value = powerAttack ? "power" : "attack";

            List<MacroStepBase> stepsToRun = new List<MacroStepBase>();
            string errorMessage;
            foreach (MacroStepBase nextMacroStep in m.Steps)
            {
                stepsToRun.Add(TranslateStep(nextMacroStep, out errorMessage));
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }
            }
            bool stop;
            string sFinalCommand = GetFinalCommand(m.FinalCommand, m.FinalCommandConditionVariable, out stop);
            if (stop) return;
            string sFinalCommand2 = GetFinalCommand(m.FinalCommand2, m.FinalCommand2ConditionVariable, out stop);
            if (stop) return;
            _currentBackgroundParameters = GenerateNewBackgroundParameters();
            _currentBackgroundParameters.Macro = m;
            _currentBackgroundParameters.PreExit = preExit;
            _currentBackgroundParameters.FinalCommand = sFinalCommand;
            _currentBackgroundParameters.FinalCommand2 = sFinalCommand2;
            _currentBackgroundParameters.UsedSkills = activatedSkills;
            _currentBackgroundParameters.Flee = m.Flee;
            RunCommands(stepsToRun, _currentBackgroundParameters);
        }

        private string GetFinalCommand(string FinalCommand, Variable FinalCommandConditionVariable, out bool stop)
        {
            stop = false;
            string sFinalCommand = string.Empty;
            if (!string.IsNullOrEmpty(FinalCommand))
            {
                bool runFinalCommand = true;
                if (FinalCommandConditionVariable != null)
                {
                    StringVariable cond = (StringVariable)_variablesByName[FinalCommandConditionVariable.Name];
                    runFinalCommand = !string.IsNullOrEmpty(cond.Value);
                }
                if (runFinalCommand)
                {
                    string errorMessage;
                    sFinalCommand = TranslateCommand(FinalCommand, _variables, out errorMessage);
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        MessageBox.Show(errorMessage);
                        stop = true;
                        return null;
                    }
                }
            }
            return sFinalCommand;
        }

        private MacroStepBase TranslateStep(MacroStepBase input, out string errorMessage)
        {
            MacroStepBase ret = null;
            errorMessage = string.Empty;
            bool isSameStep = false;
            string translatedCommand;
            if (input is MacroStepSequence)
            {
                MacroStepSequence sourceSequence = (MacroStepSequence)input;
                MacroStepSequence translatedSequence = new MacroStepSequence();
                foreach (MacroStepBase nextStep in sourceSequence.SubCommands)
                {
                    translatedSequence.SubCommands.Add(TranslateStep(nextStep, out errorMessage));
                    if (!string.IsNullOrEmpty(errorMessage)) return null;
                }
                ret = translatedSequence;
            }
            else if (input is MacroCommand)
            {
                string rawCommand = ((MacroCommand)input).Command;
                translatedCommand = TranslateCommand(rawCommand, _variables, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage)) return null;
                ret = new MacroCommand(rawCommand, translatedCommand);
            }
            else if (input is MacroStepCombatCycle)
            {
                //the command here doesn't matter as long as it requires a mob, since it
                //could contain an attack, stun, or offensive spell
                TranslateCommand("attack {mob}", _variables, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage)) return null;
                ret = input;
                isSameStep = true;
            }
            else if (input is MacroStepSetNextCommandWaitMS || input is MacroStepSetVariable)
            {
                ret = input;
                isSameStep = true;
            }
            if (!isSameStep)
            {
                ret.WaitMS = input.WaitMS;
                ret.WaitMSVariable = input.WaitMSVariable;
                ret.Loop = input.Loop;
                ret.LoopCount = input.LoopCount;
                ret.LoopVariable = input.LoopVariable;
                ret.SkipRounds = input.SkipRounds;
                ret.ConditionVariable = input.ConditionVariable;
            }
            return ret;
        }

        public class MacroStepBase
        {
            /// <summary>
            /// persistent wait milliseconds
            /// </summary>
            public int? WaitMS { get; set; }
            /// <summary>
            /// persistent wait milliseconds from a variable
            /// </summary>
            public IntegerVariable WaitMSVariable { get; set; }
            public bool? Loop { get; set; }
            public int? LoopCount { get; set; }
            public Variable LoopVariable { get; set; }
            /// <summary>
            /// number of times through a loop to skip
            /// </summary>
            public int SkipRounds { get; set; }
            /// <summary>
            /// variable controlling whether the step executes
            /// </summary>
            public Variable ConditionVariable { get; set; }

            public void CopyFrom(MacroStepBase source)
            {
                this.WaitMS = source.WaitMS;
                this.WaitMSVariable = source.WaitMSVariable;
                this.Loop = source.Loop;
                this.LoopCount = source.LoopCount;
                this.LoopVariable = source.LoopVariable;
                this.SkipRounds = source.SkipRounds;
                this.ConditionVariable = source.ConditionVariable;
            }
        }

        private void cboMacros_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRunMacro.Enabled = cboMacros.SelectedIndex > 0;
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            new frmVariables(_variables).ShowDialog(this);
        }

        public class CommandButtonTag
        {
            public CommandButtonTag(string Command, CommandType CommandType)
            {
                this.Command = Command;
                this.CommandType = CommandType;
            }
            public string Command { get; set; }
            public CommandType CommandType { get; set; }
        }

        [Flags]
        public enum CommandType
        {
            None = 0,
            Melee = 1,
            Magic = 2,
            Potions = 4,
        }

        private void cboCelduinExpress_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCelduinExpressEdges();
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
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
                    textToAdd.AddRange(_newConsoleText);
                    _newConsoleText.Clear();
                }
            }
            foreach (string s in textToAdd)
            {
                rtbConsole.AppendText(s);
            }

            if (!_enteredUserName && _promptedUserName)
            {
                SendCommand(_username, false);
                _enteredUserName = true;
            }
            else if (_enteredUserName && !_enteredPassword && _promptedPassword)
            {
                SendCommand(_password, true);
                _enteredPassword = true;
            }

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
            foreach (KeyValuePair<SkillWithCooldownType, SkillCooldownStatus> nextCoolDown in _skillCooldowns)
            {
                SkillWithCooldownType eType = nextCoolDown.Key;
                SkillCooldownStatus oStatus = nextCoolDown.Value;
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
                if (oStatus.IsActive)
                {
                    sText = "ACTIVE";
                    backColor = BACK_COLOR_GO;
                }
                else if (oStatus.NextAvailableDate.HasValue)
                {
                    DateTime dtDateValue = oStatus.NextAvailableDate.Value;
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
                else
                {
                    sText = string.Empty;
                    backColor = BACK_COLOR_NEUTRAL;
                }
                txt.Text = sText;
                txt.BackColor = backColor;
            }
            if (autohpforthistick.HasValue && autompforthistick.HasValue && autohpforthistick.Value == _totalhp && autompforthistick.Value == _totalmp &&
                ((_previoustickautohp.HasValue && _previoustickautohp.Value != autohpforthistick.Value) ||
                (_previoustickautomp.HasValue && _previoustickautomp.Value != autompforthistick.Value)))
            {
                _woe.Play();
            }
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
                    SetCurrentRoom(_treeOfLife);
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
                        SendCommand(string.Empty, false);
                    }
                }
                if (_doScore)
                {
                    _doScore = false;
                    SendCommand("score", false);
                }
                if (_currentStatusLastComputed.HasValue && !_ranStartupCommands)
                {
                    _ranStartupCommands = true;
                    foreach (string nextCommand in _startupCommands)
                    {
                        SendCommand(nextCommand, false);
                    }
                }
                if (_setDay.HasValue)
                {
                    bool setDayValue = _setDay.Value;
                    _setDay = null;
                    chkIsNight.Checked = !setDayValue;
                }
                if (_newSpellsCast != null)
                {
                    List<string> newSpellsCast = _newSpellsCast;
                    _currentSpellsCast = _newSpellsCast;
                    _newSpellsCast = null;
                    flpSpells.Controls.Clear();
                    foreach (string next in newSpellsCast)
                    {
                        Label l = new Label();
                        l.AutoSize = true;
                        l.Text = next;
                        flpSpells.Controls.Add(l);
                    }
                }
            }
            _previoustickautohp = autohpforthistick;
            _previoustickautomp = autompforthistick;
        }

        private void CheckAutoHazy(bool AutoHazyActive, DateTime dtUtcNow, int? autoHitpoints)
        {
            if (m_oCurrentRoom != _treeOfLife && !_autoHazied && AutoHazyActive && autoHitpoints.HasValue && autoHitpoints.Value < _autoHazyThreshold && (!_lastTriedToAutoHazy.HasValue || ((dtUtcNow - _lastTriedToAutoHazy.Value) > new TimeSpan(0, 0, 2))))
            {
                _lastTriedToAutoHazy = dtUtcNow;
                SendCommand("drink hazy", false);
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
            ((StringVariable)_variablesByName["weapon"]).Value = txtWeapon.Text;
        }

        private void txtOneOffCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                SendCommand(txtOneOffCommand.Text, false);
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
            SendCommand("emote " + txtCommandText.Text, false);
            txtCommandText.Focus();
            txtCommandText.SelectAll();
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            SendCommand("say " + txtCommandText.Text, false);
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
            if (_currentBackgroundParameters == null)
            {
                _currentBackgroundParameters = new BackgroundWorkerParameters();
                _currentBackgroundParameters.MaxOffensiveLevel = Convert.ToInt32(cboMaxOffLevel.SelectedItem.ToString());
                _currentBackgroundParameters.AutoMana = chkAutoMana.Checked;
                _currentBackgroundParameters.AutoHazy = chkAutoHazy.Checked;
                _currentBackgroundParameters.Flee = true;
                RunCommands(new List<MacroStepBase>(), _currentBackgroundParameters);
            }
            else
            {
                _fleeing = true;
                btnFlee.Enabled = false;
            }
        }

        private void ctxRoomExits_Opening(object sender, CancelEventArgs e)
        {
            Room r = m_oCurrentRoom;
            ctxRoomExits.Items.Clear();
            if (r == null || !_map.TryGetOutEdges(r, out IEnumerable<Exit> edges))
            {
                e.Cancel = true;
            }
            else
            {
                bool hasEdges = false;
                foreach (Exit nextEdge in edges)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Text = nextEdge.ExitText + ": " + nextEdge.Target.ToString();
                    tsmi.Tag = nextEdge;
                    ctxRoomExits.Items.Add(tsmi);
                    hasEdges = true;
                }
                e.Cancel = !hasEdges;
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
                Macro m = (Macro)sourceButton.Tag;
                RunMacro(m, exit);
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
                if (_map.TryGetOutEdges(r, out IEnumerable<Exit> exits))
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
                    Room r = node.Tag as Room;
                    if (r == null)
                    {
                        cancel = true;
                    }
                    else
                    {
                        tsmiGoToLocation.Visible = m_oCurrentRoom != null;
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

        private void GoToRoom(Room targetRoom)
        {
            _currentBackgroundParameters = GenerateNewBackgroundParameters();
            int moveGapMS;
            if (_variablesByName.TryGetValue(VARIABLE_MOVEGAPMS, out Variable movegapmsvar) && movegapmsvar is IntegerVariable)
                moveGapMS = ((IntegerVariable)movegapmsvar).Value;
            else
                moveGapMS = 260;
            _currentBackgroundParameters.WaitMS = moveGapMS;
            _currentBackgroundParameters.TargetRoom = targetRoom;
            _pathMapping = new Dictionary<Room, Exit>();
            _currentSearch = new BreadthFirstSearchAlgorithm<Room, Exit>(_map);
            _currentSearch.TreeEdge += Alg_TreeEdge;
            _currentSearch.Compute(m_oCurrentRoom);

            if (!_pathMapping.ContainsKey(targetRoom))
            {
                MessageBox.Show("No path to target room found.");
                return;
            }

            Room currentRoom = targetRoom;
            List<Exit> exits = new List<Exit>();
            while (currentRoom != m_oCurrentRoom)
            {
                Exit nextExit = _pathMapping[currentRoom];
                exits.Add(nextExit);
                currentRoom = nextExit.Source;
            }

            List<MacroStepBase> commands = new List<MacroStepBase>();
            for (int i = exits.Count - 1; i >= 0; i--)
            {
                Exit exit = exits[i];
                if (!string.IsNullOrEmpty(exit.PreCommand))
                {
                    commands.Add(new MacroCommand(exit.PreCommand, exit.PreCommand));
                }
                if (exit.Target.IsTrapRoom)
                {
                    commands.Add(new MacroCommand("prepare", "prepare"));
                }
                string nextCommand = exit.ExitText;
                if (!exit.OmitGo) nextCommand = "go " + nextCommand;
                commands.Add(new MacroCommand(nextCommand, nextCommand));
            }

            RunCommands(commands, _currentBackgroundParameters);
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            frmGraph frm = new frmGraph(_map, _graphs, m_oCurrentRoom);
            frm.ShowDialog();
            m_oCurrentRoom = frm.CurrentRoom;
            txtCurrentRoom.Text = m_oCurrentRoom == null ? string.Empty : m_oCurrentRoom.ToString();
            if (frm.GoToRoom != null)
            {
                GoToRoom(frm.GoToRoom);
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
}
