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
        private IsengardMap _gameMap;
        private Room m_oCurrentRoom;
        private BackgroundWorker _bw;
        private BackgroundWorkerParameters _currentBackgroundParameters;
        private List<Variable> _variables;
        private Dictionary<string, Variable> _variablesByName;

        private object _queuedCommandLock = new object();
        private object _consoleTextLock = new object();
        private object _writeToNetworkStreamLock = new object();
        private ConsoleOutput _previousConsoleOutput = null;
        private List<ConsoleOutput> _newConsoleText = new List<ConsoleOutput>();
        private Dictionary<char, int> _asciiMapping;

        private List<EmoteButton> _emoteButtons = new List<EmoteButton>();
        private bool _showingWithTarget = false;
        private bool _showingWithoutTarget = false;
        private bool _fleeing;
        private bool? _manashieldResult;
        private int _waitSeconds = 0;
        private bool _fumbled;
        private bool _initiatedEmotesTab;
        private bool _initiatedHelpTab;
        private CommandResult? _commandResult;
        private Exit _currentBackgroundExit;
        private bool _currentBackgroundExitMessageReceived;
        private List<string> _currentObviousExits;

        internal frmMain(List<Variable> variables, Dictionary<string, Variable> variablesByName, string defaultRealm, int level, int totalhp, int totalmp, int healtickmp, AlignmentType preferredAlignment, string userName, string password, List<Macro> allMacros, List<string> startupCommands, string defaultWeapon, int autoHazyThreshold, bool autoHazyDefault)
        {
            InitializeComponent();

            string sFullSoundPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "En-us-full.ogg");
            _vwr = new VorbisWaveReader(sFullSoundPath);
            _woe = new WaveOutEvent();
            _woe.Init(_vwr);

            _asciiMapping = AsciiMapping.GetAsciiMapping();

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

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            SetButtonTags();
            _gameMap = new IsengardMap(preferredAlignment, level);
            _gameMap.SetNightEdges(false);
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

        private void OnFailFlee()
        {
            _commandResult = CommandResult.CommandUnsuccessfulThisTime;
            _waitSeconds = 0;
        }

        private void OnRoomTransition(RoomTransitionType rtType, string roomName, List<string> obviousExits)
        {
            _commandResult = CommandResult.CommandSuccessful;
            _waitSeconds = 0;
            _currentObviousExits = obviousExits;
            if (rtType == RoomTransitionType.Flee)
            {
                _fleeing = false;
            }
            else if (rtType == RoomTransitionType.Hazy)
            {
                _autoHazied = true;
                _fleeing = false;
            }
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
            _commandResult = CommandResult.CommandNotAttempted;
            _waitSeconds = waitSeconds;
        }

        private void OnFumbled()
        {
            _fumbled = true;
        }

        private void OnLifeSpellCast()
        {
            _commandResult = CommandResult.CommandSuccessful;
        }

        private void OnCantMoveThatWay()
        {
            _commandResult = CommandResult.CommandUnsuccessfulAlways;
        }

        private void OnBlocksYourExit()
        {
            _commandResult = CommandResult.CommandUnsuccessfulAlways;
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
                new RoomTransitionSequence(OnRoomTransition),
                new SkillCooldownSequence(SkillWithCooldownType.PowerAttack, OnGetSkillCooldown),
                new SkillCooldownSequence(SkillWithCooldownType.Manashield, OnGetSkillCooldown),
                new ConstantOutputSequence("You creative a protective manashield.", OnSuccessfulManashield, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("Your attempt to manashield failed.", OnFailManashield, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("Your manashield dissipates.", DoScore, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("The sun disappears over the horizon.", OnNight, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("The sun rises.", OnDay, ConstantSequenceMatchType.Contains),
                new SpellsCastSequence(OnSpellsCastChange),
                new ConstantOutputSequence("You feel less protected.", DoScore, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("You feel less holy.", DoScore, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("Bless spell cast.", OnLifeSpellCast, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("Protection spell cast.", OnLifeSpellCast, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("You failed to escape!", OnFailFlee, ConstantSequenceMatchType.Contains), //could be prefixed by "Scared of going X"*
                new PleaseWaitXSecondsSequence(OnWaitXSeconds),
                new ConstantOutputSequence("You FUMBLED your weapon.", OnFumbled, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("Vigor spell cast.", OnLifeSpellCast, ConstantSequenceMatchType.Contains),
                new ConstantOutputSequence("You can't go that way.", OnCantMoveThatWay, ConstantSequenceMatchType.FirstLineExactMatch),
                new ConstantOutputSequence(" blocks your exit.", OnBlocksYourExit, ConstantSequenceMatchType.FirstLineContains),
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
                        string sNewLineRaw = sNewLine;
                        if (oist == OutputItemSequenceType.HPMPStatus) //strip the HP/MP since it was already processed
                        {
                            int lastParenthesisLocation = sNewLine.LastIndexOf('(');
                            if (lastParenthesisLocation == 0)
                                sNewLine = string.Empty;
                            else
                                sNewLine = sNewLine.Substring(0, lastParenthesisLocation);
                        }

                        if (!string.IsNullOrEmpty(sNewLine))
                        {
                            string[] sNewLines = sNewLine.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                            Exit exit = _currentBackgroundExit;
                            if (exit != null && !string.IsNullOrEmpty(exit.WaitForMessage))
                            {
                                foreach (string s in sNewLines)
                                {
                                    if (s.StartsWith(exit.WaitForMessage))
                                    {
                                        _currentBackgroundExitMessageReceived = true;
                                        break;
                                    }
                                }
                            }

                            foreach (IOutputProcessingSequence nextProcessingSequence in outputProcessingSequences)
                            {
                                nextProcessingSequence.FeedLine(sNewLines);
                            }
                        }

                        lock (_consoleTextLock)
                        {
                            _newConsoleText.Add(new ConsoleOutput(sNewLineRaw, sNewLine, false));
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
                    case 170:
                        isUnknown = true;
                        break;
                    case 173:
                        isUnknown = true;
                        break;
                    case 235:
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
            CommandResult? currentResult;

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
                automp = _autoMana;
                if (automp.HasValue && automp.Value >= 8 && spellsCast != null && !spellsCast.Contains("bless"))
                {
                    CastLifeSpell("bless", maxAttempts, ref dtLastCombatCycle, combatCycleInterval, pms);
                }
                //cast protection if has enough mana and not curently protected
                automp = _autoMana;
                if (automp.HasValue && automp.Value >= 8 && spellsCast != null && !spellsCast.Contains("protection"))
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

            if (pms.Exits != null && pms.Exits.Count > 0)
            {
                WaitUntilNextCombatCycle(dtLastCombatCycle, combatCycleInterval);
                foreach (Exit nextExit in pms.Exits)
                {
                    _currentBackgroundExit = nextExit;
                    _currentBackgroundExitMessageReceived = false;

                    try
                    {
                        //for periodic exits, verify the exit actually exists
                        if (nextExit.Periodic)
                        {
                            bool foundExit = false;
                            do
                            {
                                _currentObviousExits = null;
                                if (_fleeing) break;
                                if (_bw.CancellationPending) break;
                                _commandResult = null;
                                SendCommand("look", false);
                                pms.CommandsRun++;
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
                                    if (currentResult.Value == CommandResult.CommandSuccessful)
                                    {
                                        if (_currentObviousExits.Contains(nextExit.ExitText))
                                        {
                                            foundExit = true;
                                        }
                                        else
                                        {
                                            WaitUntilNextCommand(5000, false, false);
                                        }
                                    }
                                    else //look is not supposed to fail
                                    {
                                        return;
                                    }
                                }
                            }
                            while (!foundExit);
                        }

                        if (!string.IsNullOrEmpty(nextExit.WaitForMessage))
                        {
                            while (!_currentBackgroundExitMessageReceived)
                            {
                                if (_fleeing) break;
                                if (_bw.CancellationPending) break;
                                Thread.Sleep(50);
                                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                            }
                        }

                        if (_fleeing) break;
                        if (_bw.CancellationPending) break;
                        RunPreExitLogic(pms, nextExit.PreCommand, nextExit.Target);
                        string nextCommand = GetExitCommand(nextExit.ExitText);

                        currentAttempts = 0;
                        while (currentAttempts < maxAttempts)
                        {
                            if (_fleeing) break;
                            if (_bw.CancellationPending) break;
                            _commandResult = null;
                            SendCommand(nextCommand, false);
                            pms.CommandsRun++;
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
                                if (currentResult.Value == CommandResult.CommandSuccessful)
                                {
                                    if (nextExit.Target != null)
                                    {
                                        pms.TargetRoom = nextExit.Target;
                                        pms.SetTargetRoomIfCancelled = true;
                                        if (!string.IsNullOrEmpty(nextExit.Target.PostMoveCommand))
                                        {
                                            SendCommand(nextExit.Target.PostMoveCommand, false);
                                            pms.CommandsRun++;
                                        }
                                    }
                                    break;
                                }
                                else if (currentResult.Value == CommandResult.CommandUnsuccessfulAlways)
                                {
                                    return;
                                }
                                else if (_waitSeconds > 1)
                                {
                                    int waitMS = 500 + (1000 * (_waitSeconds - 2));
                                    WaitUntilNextCommand(waitMS, false, false);
                                }
                            }
                        }
                    }
                    finally
                    {
                        _currentBackgroundExit = null;
                    }
                }
            }

            if (pms.Flee)
            {
                _fleeing = true;
            }

            foreach (MacroCommand nextCommand in IterateStepCommands(commands, pms, 0))
            {
                if (_bw.CancellationPending) break;
                oPreviousCommand = oCurrentCommand;
                oCurrentCommand = nextCommand;
                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters, false);
                if (_fleeing) break;
                if (_bw.CancellationPending) break;

                //use the combat cycle to determine how long to wait
                if (nextCommand.CombatCycle != null && dtLastCombatCycle.HasValue)
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
                    currentResult = _commandResult;
                    int waitSeconds = _waitSeconds;
                    if (currentResult.HasValue)
                    {
                        if (currentResult.Value == CommandResult.CommandSuccessful)
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
                    currentResult = _commandResult;
                    int waitSeconds = _waitSeconds;
                    if (currentResult.HasValue)
                    {
                        if (currentResult.Value == CommandResult.CommandSuccessful)
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
            if (targetRoom != null && targetRoom.IsTrapRoom)
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
                CommandResult? currentResult;
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
                    if (currentResult.Value == CommandResult.CommandSuccessful)
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
        private IEnumerable<MacroCommand> IterateStepCommands(List<MacroStepBase> Steps, BackgroundWorkerParameters parameters, int loopsPerformed)
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

                    if (nextStep is MacroStepSetVariable)
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
                        foreach (MacroCommand nextStepCommand in IterateStepCommands(seq.SubCommands, parameters, loopCount))
                        {
                            yield return nextStepCommand;
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
                        yield return nextCommand;
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
                    string sText = sToConsole + Environment.NewLine;
                    _newConsoleText.Add(new ConsoleOutput(sText, sText, true));
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
                NavigateExitsInBackground(exit.Target, new List<Exit>() { exit });
            }
            else
            {
                SetCurrentRoom(exit.Target);
            }
        }

        private void DoSingleMove(bool move, string direction, string command)
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
                NavigateExitsInBackground(null, new List<Exit>() { new Exit(null, null, command) });
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
            public List<Exit> Exits { get; set; }
            public Dictionary<string, Variable> Variables { get; set; }
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

        private void chkIsNight_CheckedChanged(object sender, EventArgs e)
        {
            _gameMap.SetNightEdges(chkIsNight.Checked);
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

        private void RunMacro(Macro m, List<Exit> preExits)
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
            _currentBackgroundParameters.Exits = preExits;
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
            else if (input is MacroStepSetVariable)
            {
                ret = input;
                isSameStep = true;
            }
            if (!isSameStep)
            {
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
                    foreach (ConsoleOutput nextConsoleOutput in _newConsoleText)
                    {
                        bool add = true;
                        if (!nextConsoleOutput.IsInput)
                        {
                            if (_previousConsoleOutput == null || !string.IsNullOrWhiteSpace(nextConsoleOutput.Content) || !string.Equals(_previousConsoleOutput.RawText, nextConsoleOutput.RawText))
                            {
                                _previousConsoleOutput = nextConsoleOutput;
                            }
                            else
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
                    bool previousFly = _currentSpellsCast != null && _currentSpellsCast.Contains("fly");
                    bool newFly = _newSpellsCast != null && _newSpellsCast.Contains("fly");
                    if (previousFly != newFly)
                    {
                        _gameMap.SetFlyEdges(newFly);
                    }
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
            if (m_oCurrentRoom != _gameMap.TreeOfLifeRoom && !_autoHazied && AutoHazyActive && autoHitpoints.HasValue && autoHitpoints.Value < _autoHazyThreshold && (!_lastTriedToAutoHazy.HasValue || ((dtUtcNow - _lastTriedToAutoHazy.Value) > new TimeSpan(0, 0, 2))))
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
            MapComputation mc = new MapComputation(m_oCurrentRoom, targetRoom, _gameMap.MapGraph);
            if (!mc.PathMapping.ContainsKey(targetRoom))
            {
                MessageBox.Show("No path to target room found.");
                return null;
            }
            List<Exit> exits = mc.GetExits();
            exits.Reverse();
            return exits;
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
            _currentBackgroundParameters = GenerateNewBackgroundParameters();
            _currentBackgroundParameters.TargetRoom = targetRoom;
            _currentBackgroundParameters.Exits = exits;
            RunCommands(new List<MacroStepBase>(), _currentBackgroundParameters);
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
            public string RawText { get; set; }
            public string Content { get; set; }
        }
    }

    [Flags]
    internal enum PromptedSkills
    {
        None = 0,
        PowerAttack = 1,
        Manashield = 2
    }

    internal enum CommandResult
    {
        CommandSuccessful,
        CommandUnsuccessfulThisTime,
        CommandUnsuccessfulAlways,
        CommandNotAttempted,
    }
}
