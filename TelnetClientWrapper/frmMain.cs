using Microsoft.VisualBasic;
using QuickGraph;
using QuickGraph.Algorithms.Search;
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
        private bool _quitting;
        private bool _finishedQuit;
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
        private List<int> _queue = new List<int>();
        private Dictionary<char, int> _asciiMapping;
        private Dictionary<int, char> _reverseAsciiMapping;

        private const string AREA_BREE_PERMS = "Bree Perms";
        private const string AREA_IMLADRIS_THARBAD_PERMS = "Imladris/Tharbad Perms";
        private const string AREA_BREE = "Bree";
        private const string AREA_BREE_TO_HOBBITON = "Bree to Hobbiton";
        private const string AREA_IMLADRIS = "Imladris";
        private const string AREA_IMLADRIS_TO_THARBAD = "Imladris to Tharbad";
        private const string AREA_MISC = "Misc";
        private const string AREA_SHIPS = "Ships";
        private const string AREA_INTANGIBLE = "Intangible";
        private const string AREA_INACCESSIBLE = "Inaccessible";

        private const string VARIABLE_MOVEGAPMS = "movegapms";
        private const string VARIABLE_HITANDRUNDIRECTION = "hitandrundirection";
        private const string VARIABLE_HITANDRUNPRECOMMAND = "hitandrunprecommand";

        private List<EmoteButton> _emoteButtons = new List<EmoteButton>();
        private bool _showingWithTarget = false;
        private bool _showingWithoutTarget = false;
        private bool _fleeing;
        private bool? _fleeResult;

        internal frmMain(List<Variable> variables, Dictionary<string, Variable> variablesByName, string defaultRealm, int level, int totalhp, int totalmp, int healtickmp, AlignmentType preferredAlignment, string userName, string password, List<Macro> allMacros, List<string> startupCommands, string defaultWeapon, int autoHazyThreshold, bool autoHazyDefault)
        {
            InitializeComponent();

            InitializeEmotes();

            _asciiMapping = AsciiMapping.GetAsciiMapping(out _reverseAsciiMapping);

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
            AddArea(AREA_BREE_TO_HOBBITON);
            AddArea(AREA_IMLADRIS);
            AddArea(AREA_IMLADRIS_TO_THARBAD);
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

        private void btnEmoteButton_Click(object sender, EventArgs e)
        {
            EmoteButton source = (EmoteButton)sender;
            string command = source.Emote.Command;
            if (source.HasTarget)
            {
                command += " " + txtEmoteTarget.Text;
            }
            SendCommand(command, false, false);
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
            _quitting = false;
            _finishedQuit = false;
            _currentStatusLastComputed = null;
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
            if (_quitting && _finishedQuit)
            {
                this.Close();
            }
            else
            {
                if (MessageBox.Show("Disconnected. Reconnect?", "Disconnected", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DoConnect();
                }
                else
                {
                    _quitting = true;
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

        private void OnNamePrompt()
        {
            _promptedUserName = true;
        }

        private void OnPasswordPrompt()
        {
            _promptedPassword = true;
        }

        private void DoScore()
        {
            _doScore = true;
        }

        private void OnGetHPMP(int hp, int mp)
        {
            _autoHitpoints = hp;
            _autoMana = mp;
            _currentStatusLastComputed = DateTime.UtcNow;
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

        private void FinishedQuit()
        {
            if (_quitting)
            {
                _finishedQuit = true;
            }
        }

        private void OnHazy()
        {
            _autoHazied = true;
            _fleeing = false;
        }

        private void OnFailFlee()
        {
            _fleeResult = false;
        }

        private void OnSuccessfulFlee()
        {
            _fleeing = false;
            _fleeResult = true;
        }

        private void _bwNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
            List<ISequence> sequences = new List<ISequence>()
            {
                new ConstantSequence("Please enter name: ", OnNamePrompt, _asciiMapping),
                new ConstantSequence("Please enter password: ", OnPasswordPrompt, _asciiMapping),
                new HPMPSequence(_asciiMapping, OnGetHPMP),
                new SkillCooldownSequence(SkillWithCooldownType.PowerAttack, _asciiMapping, OnGetSkillCooldown),
                new SkillCooldownSequence(SkillWithCooldownType.Manashield, _asciiMapping, OnGetSkillCooldown),
                new ConstantSequence("You creative a protective manashield.", DoScore, _asciiMapping),
                new ConstantSequence("Your manashield dissipates.", DoScore, _asciiMapping),
                new ConstantSequence("The sun disappears over the horizon.", OnNight, _asciiMapping),
                new ConstantSequence("The sun rises.", OnDay, _asciiMapping),
                new SpellsCastSequence(_asciiMapping, _reverseAsciiMapping, OnSpellsCastChange),
                new ConstantSequence("You feel less protected.", DoScore, _asciiMapping),
                new ConstantSequence("You feel less holy.", DoScore, _asciiMapping),
                new ConstantSequence("You feel watched.", DoScore, _asciiMapping),
                new ConstantSequence("You feel holy.", DoScore, _asciiMapping),
                new ConstantSequence("Goodbye!", FinishedQuit, _asciiMapping),
                new ConstantSequence("You phase in and out of existence.", OnHazy, _asciiMapping),
                new ConstantSequence("You failed to escape!", OnFailFlee, _asciiMapping),
                new ConstantSequence("You run like a chicken.", OnSuccessfulFlee, _asciiMapping),
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
                    foreach (ISequence nextSequence in sequences)
                    {
                        nextSequence.FeedByte(nextByte);
                    }
                    ProcessInputCharacter(nextByte);

                    if (!_enteredUserName && _promptedUserName)
                    {
                        SendCommand(_username, false, false);
                        _enteredUserName = true;
                    }
                    if (_enteredUserName && !_enteredPassword && _promptedPassword)
                    {
                        SendCommand(_password, true, false);
                        _enteredPassword = true;
                    }
                }
            }
        }

        private void ProcessInputCharacter(int nextByte)
        {
            if (nextByte == 13) //carriage return
            {
                if (_queue.Count == 1 && _queue[0] == 10)
                {
                    lock (_consoleTextLock)
                    {
                        _newConsoleText.Add(Environment.NewLine);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else if (nextByte == 10) //line feed
            {
                _queue.Clear();
                _queue.Add(10);
            }
            else if (nextByte == 27) //escape
            {
                _queue.Clear();
                _queue.Add(27);
            }
            else if (_queue.Count > 0 && _queue[0] == 27)
            {
                if (nextByte == 109) //ends the escape sequence
                {
                    ConsoleColor? cc;
                    if (_queue[1] == 91 && _queue[2] == 51)
                    {
                        switch (_queue[3])
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
                    _queue.Clear();
                }
                else
                {
                    _queue.Add(nextByte);
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
                    case 32:
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
                    case 37:
                        c = '%';
                        break;
                    case 39:
                        c = '\'';
                        break;
                    case 40:
                        c = '(';
                        break;
                    case 41:
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
                    case 48:
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
                    case 57:
                        c = '9';
                        break;
                    case 58:
                        c = ':';
                        break;
                    case 59:
                        c = ';';
                        break;
                    case 63:
                        c = '?';
                        break;
                    case 64:
                        c = '@';
                        break;
                    case 65:
                        c = 'A';
                        break;
                    case 66:
                        c = 'B';
                        break;
                    case 67:
                        c = 'C';
                        break;
                    case 68:
                        c = 'D';
                        break;
                    case 69:
                        c = 'E';
                        break;
                    case 70:
                        c = 'F';
                        break;
                    case 71:
                        c = 'G';
                        break;
                    case 72:
                        c = 'H';
                        break;
                    case 73:
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
                    case 77:
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
                    case 84:
                        c = 'T';
                        break;
                    case 85:
                        c = 'U';
                        break;
                    case 86:
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
                    case 91:
                        c = '[';
                        break;
                    case 93:
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
                    case 124:
                        c = '|';
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
                lock (_consoleTextLock)
                {
                    _newConsoleText.Add(sNewString);
                }
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
            btnQuit.Tag = new CommandButtonTag("quit", CommandType.None);
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
            RunMacro((Macro)((Button)sender).Tag);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _fleeing = false;
            _fleeResult = null;
            if (!string.IsNullOrEmpty(_currentBackgroundParameters.FinalCommand))
            {
                SendCommand(_currentBackgroundParameters.FinalCommand, false, false);
                _currentBackgroundParameters.CommandsRun++;
            }
            if (!string.IsNullOrEmpty(_currentBackgroundParameters.FinalCommand2))
            {
                SendCommand(_currentBackgroundParameters.FinalCommand2, false, false);
                _currentBackgroundParameters.CommandsRun++;
            }
            if (!_currentBackgroundParameters.Cancelled && _currentBackgroundParameters.CommandsRun > 0)
            {
                Room targetRoom = _currentBackgroundParameters.TargetRoom;
                if (targetRoom != null)
                {
                    SetCurrentRoom(targetRoom);
                }
            }
            if (_currentBackgroundParameters.PowerAttack && _currentBackgroundParameters.CommandsRun > 0)
            {
                _doScore = true;
                chkPowerAttack.Checked = false;
            }
            else if (!_currentBackgroundParameters.WasPowerAttackAvailableAtStart && IsPowerAttackAvailable())
            {
                chkPowerAttack.Checked = true;
            }
            ToggleBackgroundProcess(false);
            _currentBackgroundParameters = null;
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
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            List<MacroStepBase> commands = pms.Commands;
            MacroCommand oPreviousCommand;
            MacroCommand oCurrentCommand = null;
            foreach (var nextCommandInfo in IterateStepCommands(commands, pms, 0))
            {
                MacroCommand nextCommand = nextCommandInfo.Key;
                int? overrideWaitMS = nextCommandInfo.Value;

                if (_bw.CancellationPending) break;
                oPreviousCommand = oCurrentCommand;
                oCurrentCommand = nextCommand;
                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters);
                if (_fleeing) break;
                if (_bw.CancellationPending) break;

                //wait for an appropriate amount of time for commands after the first
                if (oPreviousCommand != null)
                {
                    int remainingMS = overrideWaitMS.GetValueOrDefault(pms.WaitMS);
                    while (remainingMS > 0)
                    {
                        int nextWaitMS = Math.Min(remainingMS, 100);
                        if (_fleeing) break;
                        if (_bw.CancellationPending) break;
                        Thread.Sleep(nextWaitMS);
                        remainingMS -= nextWaitMS;
                        RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters);
                        if (_fleeing) break;
                        if (_bw.CancellationPending) break;
                    }
                }

                if (_bw.CancellationPending) break;
                if (_fleeing) break;

                ManaDrainType mdType = nextCommand.ManaDrain;
                bool stop;
                ProcessCommand(nextCommand, pms, out stop);
                if (stop) break;
                if (_fleeing) break;
                if (_bw.CancellationPending) break;

                RunAutoCommandsWhenMacroRunning(_currentBackgroundParameters);
                if (_fleeing) break;
                if (_bw.CancellationPending) break;
            }
            if (_fleeing)
            {
                string sWeapon = ((StringVariable)_currentBackgroundParameters.Variables["weapon"]).Value;
                if (!string.IsNullOrEmpty(sWeapon))
                {
                    SendCommand("remove " + sWeapon, false, false);
                    _currentBackgroundParameters.CommandsRun++;
                    if (!_fleeing) return;
                    if (_bw.CancellationPending) return;
                }
                int maxAttempts = 20;
                int currentAttempts = 0;
                while (_fleeing && currentAttempts < maxAttempts)
                {
                    _fleeResult = null;
                    SendCommand("flee", false, false);
                    _currentBackgroundParameters.CommandsRun++;
                    currentAttempts++;
                    while (!_fleeResult.HasValue)
                    {
                        Thread.Sleep(50);
                        if (!_fleeing) break;
                        if (_bw.CancellationPending) break;
                    }
                    if (_fleeResult.Value)
                    {
                        Room r = m_oCurrentRoom;
                        if (r != null)
                        {
                            if (_map.TryGetOutEdges(r, out IEnumerable<Exit> edges))
                            {
                                List<Exit> exits = new List<Exit>();
                                foreach (Exit nextExit in edges)
                                {
                                    exits.Add(nextExit);
                                }
                                if (exits.Count == 1)
                                {
                                    _currentBackgroundParameters.TargetRoom = exits[0].Target;
                                }
                            }
                        }
                        _fleeing = false;
                    }
                }
            }
        }

        private void RunAutoCommandsWhenMacroRunning(BackgroundWorkerParameters pms)
        {
            CheckAutoHazy(pms.AutoHazy, DateTime.UtcNow);
            
            lock (_queuedCommandLock)
            {
                if (pms.QueuedCommand != null)
                {
                    SendCommand(pms.QueuedCommand, false, false);
                    pms.CommandsRun++;
                    pms.QueuedCommand = null;
                }
            }
        }

        private void ProcessCommand(MacroCommand nextCommand, BackgroundWorkerParameters pms, out bool stop)
        {
            string rawCommand;
            int? manaDrain = null;
            ManaDrainType mdType = nextCommand.ManaDrain;
            stop = false;
            if (mdType == ManaDrainType.Stun)
            {
                rawCommand = "cast stun {mob}";
                manaDrain = 10;
            }
            else if (mdType == ManaDrainType.Offensive)
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
                rawCommand = nextCommand.RawCommand;
            }

            //stop processing if out of mana
            if (mdType != ManaDrainType.None && _currentMana < manaDrain.Value)
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

            SendCommand(actualCommand, false, false);
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
                        if (nextStep is MacroManaSpellStun)
                        {
                            nextCommand = new MacroCommand(string.Empty, string.Empty);
                            nextCommand.CopyFrom(nextStep);
                            nextCommand.ManaDrain = ManaDrainType.Stun;
                        }
                        else if (nextStep is MacroManaSpellOffensive)
                        {
                            nextCommand = new MacroCommand(string.Empty, string.Empty);
                            nextCommand.CopyFrom(nextStep);
                            nextCommand.ManaDrain = ManaDrainType.Offensive;
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
                    if (ctl is Button)
                    {
                        if (ctl != btnFlee)
                        {
                            if (ctl == btnAbort)
                            {
                                ctl.Enabled = running;
                            }
                            else if (ctl.Tag is CommandButtonTag)
                            {
                                CommandButtonTag cbt = (CommandButtonTag)ctl.Tag;
                                ctl.Enabled = !running || ((eRunningCombatCommandTypes & cbt.CommandType) == CommandType.None);
                            }
                            else if (ctl.Tag is string)
                            {
                                ctl.Enabled = !running;
                            }
                            else
                            {
                                ctl.Enabled = !running;
                            }
                        }
                    }
                    else if (ctl is TextBox)
                    {
                        if (ctl == txtOneOffCommand)
                        {
                            ctl.Enabled = !running;
                        }
                    }
                    else if (ctl != grpLocations && ctl != grpConsole)
                    {
                        ctl.Enabled = !running;
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
            AddBidirectionalSameNameExit(oTharbadGateOutside, balleNightingale, "gate", null);

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
            SetVariablesForIndefiniteCasts(oMasterJeweler);

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
            SetVariablesForIndefiniteCasts(oKingBrunden);

            Room oGypsyBlademaster = AddRoom("Gypsy Blademaster");
            oGypsyBlademaster.Mob1 = "Blademaster";
            oGypsyBlademaster.Experience1 = 160;
            oGypsyBlademaster.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow3, oGypsyBlademaster, "tent");
            AddExit(oGypsyBlademaster, oGypsyRow3, "out");
            SetVariablesForIndefiniteCasts(oGypsyBlademaster);

            Room oKingsMoneychanger = AddRoom("King's Moneychanger");
            oKingsMoneychanger.Mob1 = "Moneychanger";
            oKingsMoneychanger.Experience1 = 150;
            oKingsMoneychanger.Alignment = AlignmentType.Red;
            AddExit(oGypsyRow2, oKingsMoneychanger, "tent");
            AddExit(oKingsMoneychanger, oGypsyRow2, "out");
            SetVariablesForIndefiniteCasts(oKingsMoneychanger);

            Room oMadameNicolov = AddRoom("Madame Nicolov");
            oMadameNicolov.Mob1 = "Madame";
            oMadameNicolov.Experience1 = 180;
            oMadameNicolov.Alignment = AlignmentType.Blue;
            AddExit(oGypsyRow1, oMadameNicolov, "wagon");
            AddExit(oMadameNicolov, oGypsyRow1, "out");
            SetVariablesForIndefiniteCasts(oMadameNicolov);

            Room gildedApple = AddRoom("Gilded Applie");
            AddBidirectionalSameNameExit(sabre3, gildedApple, "door", null);

            Room zathriel = AddRoom("Zathriel the Minstrel");
            zathriel.Mob1 = "Minstrel";
            zathriel.Experience1 = 220;
            zathriel.Alignment = AlignmentType.Blue;
            AddExit(gildedApple, zathriel, "stage");
            AddExit(zathriel, gildedApple, "down");
            SetVariablesForIndefiniteCasts(zathriel);

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);

            Room oOliphaunt = AddRoom("Oliphaunt");
            oOliphaunt.Mob1 = "Oliphaunt";
            oOliphaunt.Experience1 = 310;
            oOliphaunt.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphaunt, "curtain", null);
            SetVariablesForIndefiniteCasts(oOliphaunt);

            AddLocation(_aImladrisTharbadPerms, bardicGuildhall);
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
            breeStreets[0, 0] = AddRoom("Bree Thalion/Wain 1x1");
            breeStreets[1, 0] = AddRoom("Bree Thalion 2x1");
            breeStreets[2, 0] = AddRoom("Bree Thalion 3x1");
            breeStreets[3, 0] = AddRoom("Bree Thalion/High 4x1");
            breeStreets[4, 0] = AddRoom("Bree Thalion 5x1");
            breeStreets[5, 0] = AddRoom("Bree Thalion 6x1");
            breeStreets[6, 0] = AddRoom("Bree Thalion 7x1");
            breeStreets[7, 0] = AddRoom("Bree Thalion/Main 8x1");
            _breeDocks = breeStreets[9, 0] = AddRoom("Bree Docks"); //10x1
            oSewerPipeExit = breeStreets[10, 0] = AddRoom("Bree Thalion/Crissaegrim 11x1");
            breeStreets[11, 0] = AddRoom("Bree Thalion 12x1");
            breeStreets[12, 0] = AddRoom("Bree Thalion 13x1");
            breeStreets[13, 0] = AddRoom("Bree Thalion 14x1");
            breeStreets[14, 0] = AddRoom("Bree Thalion/Brownhaven 15x1");
            breeStreets[0, 1] = AddRoom("Bree Wain 1x2");
            Room oToCampusFreeClinic = breeStreets[3, 1] = AddRoom("Bree High 4x2");
            breeStreets[7, 1] = AddRoom("Bree Main 8x2");
            breeStreets[10, 1] = AddRoom("Bree Crissaegrim 11x2");
            breeStreets[14, 1] = AddRoom("Bree Brownhaven 15x2");
            breeStreets[0, 2] = AddRoom("Bree Wain 1x3");
            Room oToPawnShopWest = breeStreets[3, 2] = AddRoom("Bree High 4x3");
            Room oToBarracks = breeStreets[7, 2] = AddRoom("Bree Main 8x3");
            breeStreets[10, 2] = AddRoom("Bree Crissaegrim 11x3");
            breeStreets[14, 2] = AddRoom("Bree Brownhaven 15x3");
            breeStreets[0, 3] = AddRoom("Bree Periwinkle/Wain 1x4");
            breeSewers[0, 3] = AddRoom("Bree Sewers Periwinkle/Wain 1x4");
            AddExit(breeSewers[0, 3], breeStreets[0, 3], "up");
            breeStreets[1, 3] = AddRoom("Bree Periwinkle 2x4");
            breeSewers[1, 3] = AddRoom("Bree Sewers Periwinkle 2x4");
            breeStreets[2, 3] = AddRoom("Bree Periwinkle 3x4");
            breeSewers[2, 3] = AddRoom("Bree Sewers Periwinkle 3x4");
            breeStreets[3, 3] = AddRoom("Bree Periwinkle/High 4x4");
            breeSewers[3, 3] = AddRoom("Bree Sewers Periwinkle/high 4x4");
            AddExit(breeSewers[3, 3], breeStreets[3, 3], "up");
            breeStreets[4, 3] = AddRoom("Bree Periwinkle 5x4");
            breeSewers[4, 3] = AddRoom("Bree Sewers Periwinkle 5x4");
            breeStreets[5, 3] = AddRoom("Bree Periwinkle 6x4");
            breeSewers[5, 3] = AddRoom("Bree Sewers Periwinkle 6x4");
            breeStreets[6, 3] = AddRoom("Bree Periwinkle 7x4");
            breeSewers[6, 3] = AddRoom("Bree Sewers Periwinkle 7x4");
            breeStreets[7, 3] = AddRoom("Bree Periwinkle/Main 8x4");
            Room oShirriff = breeSewers[7, 3] = AddRoom("Shirriffs"); //Bree Sewers Periwinkle/Main 8x4
            AddExit(breeSewers[7, 3], breeStreets[7, 3], "up");
            breeStreets[8, 3] = AddRoom("Bree Periwinkle 9x4");
            breeStreets[9, 3] = AddRoom("Bree South Bridge 10x4");
            breeStreets[10, 3] = AddRoom("Bree Periwinkle/Crissaegrim 11x4");
            breeStreets[11, 3] = AddRoom("Bree Periwinkle 12x4");
            breeStreets[12, 3] = AddRoom("Bree Periwinkle/PoorAlley 13x4");
            breeStreets[13, 3] = AddRoom("Bree Periwinkle 14x4");
            breeStreets[14, 3] = AddRoom("Bree Periwinkle/Brownhaven 15x4");
            breeStreets[0, 4] = AddRoom("Bree Wain 1x5");
            breeSewers[0, 4] = AddRoom("Bree Sewers Wain 1x5");
            Room oToBlindPigPubAndUniversity = breeStreets[3, 4] = AddRoom("Bree High 4x5");
            breeStreets[7, 4] = AddRoom("Bree Main 8x5");
            Room oToSnarSlystoneShoppe = breeStreets[10, 4] = AddRoom("Bree Crissaegrim 11x5");
            breeStreets[14, 4] = AddRoom("Bree Brownhaven 15x5");
            breeStreets[0, 5] = AddRoom("Bree Wain 1x6");
            breeSewers[0, 5] = AddRoom("Bree Sewers Wain 1x6");
            breeStreets[3, 5] = AddRoom("Bree High 4x6");
            breeStreets[7, 5] = AddRoom("Bree Main 8x6");
            Room oBigPapa = breeStreets[8, 5] = AddRoom("Big Papa"); //9x6
            breeStreets[10, 5] = AddRoom("Bree Crissaegrim 11x6");
            breeStreets[14, 5] = AddRoom("Bree Brownhaven 15x6");
            breeStreets[0, 6] = AddRoom("Bree Wain 1x7");
            breeSewers[0, 6] = AddRoom("Bree Sewers Wain 1x7");
            breeStreets[3, 6] = AddRoom("Bree High 4x7");
            breeStreets[7, 6] = AddRoom("Bree Main 8x7");
            breeStreets[10, 6] = AddRoom("Bree Crissaegrim 11x7");
            breeStreets[14, 6] = AddRoom("Bree Brownhaven 15x7");
            oWestGateInside = breeStreets[0, 7] = AddRoom("Bree West Gate 1x8");
            breeSewers[0, 7] = AddRoom("Bree Sewers West Gate 1x8");
            AddExit(breeSewers[0, 7], oWestGateInside, "up");
            breeStreets[1, 7] = AddRoom("Bree Leviathan 2x8");
            breeStreets[2, 7] = AddRoom("Bree Leviathan 3x8");
            breeStreets[3, 7] = AddRoom("Bree Leviathan/High 4x8");
            breeStreets[4, 7] = AddRoom("Bree Leviathan 5x8");
            oBreeTownSquare = breeStreets[5, 7] = AddRoom("Bree Town Square 6x8");
            breeStreets[6, 7] = AddRoom("Bree Leviathan 7x8");
            breeStreets[7, 7] = AddRoom("Bree Leviathan/Main 8x8");
            breeStreets[8, 7] = AddRoom("Bree Leviathan 9x8");
            Room oNorthBridge = breeStreets[9, 7] = AddRoom("Bree North Bridge 10x8");
            breeStreets[10, 7] = AddRoom("Bree Leviathan/Crissaegrim 11x8");
            breeStreets[11, 7] = AddRoom("Bree Leviathan 12x8");
            Room oLeviathanPoorAlley = breeStreets[12, 7] = AddRoom("Bree Leviathan 13x8");
            Room oToGrantsStables = breeStreets[13, 7] = AddRoom("Bree Leviathan 14x8");
            _breeEastGateInside = breeStreets[14, 7] = AddRoom("Bree East Gate 15x8");
            breeStreets[0, 8] = AddRoom("Bree Wain 1x9");
            breeSewers[0, 8] = AddRoom("Bree Sewers Wain 1x9");
            breeStreets[3, 8] = AddRoom("Bree High 4x9");
            breeStreets[7, 8] = AddRoom("Bree Main 8x9");
            breeStreets[10, 8] = AddRoom("Bree Crissaegrim 11x9");
            breeStreets[14, 8] = AddRoom("Bree Brownhaven 15x9");
            Room oOrderOfLove = breeStreets[15, 8] = AddRoom("Bree Order of Love"); //16x9
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
            breeStreets[0, 9] = AddRoom("Bree Wain 1x10");
            breeSewers[0, 9] = AddRoom("Bree Sewers Wain 1x10");
            breeStreets[3, 9] = AddRoom("Bree High 4x10");
            breeStreets[7, 9] = AddRoom("Bree Main 8x10");
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Bree Crissaegrim 11x10");
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Bree Brownhaven 15x10");
            breeStreets[0, 10] = AddRoom("Bree Ormenel/Wain 1x11");
            breeSewers[0, 10] = AddRoom("Bree Sewers Ormenel/Wain 1x11");
            Exit e = AddExit(breeStreets[0, 10], breeSewers[0, 10], "sewer");
            e.PreCommand = "open sewer";
            breeStreets[1, 10] = AddRoom("Bree Ormenel 2x11");
            Room oToZoo = breeStreets[2, 10] = AddRoom("Bree Ormenel 3x11");
            breeStreets[3, 10] = AddRoom("Bree Ormenel/High 4x11");
            Room oToCasino = breeStreets[4, 10] = AddRoom("Bree Ormenel 5x11");
            breeStreets[5, 10] = AddRoom("Bree Ormenel 6x11");
            breeStreets[6, 10] = AddRoom("Bree Ormenel 7x11");
            breeStreets[7, 10] = AddRoom("Bree Ormenel/Main 8x11");
            breeStreets[10, 10] = AddRoom("Bree Ormenel 11x11");
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Bree Ormenel 12x11");
            breeStreets[12, 10] = AddRoom("Bree Ormenel 13x11");
            Room oStreetToFallon = breeStreets[13, 10] = AddRoom("Bree Ormenel 14x11");
            breeStreets[14, 10] = AddRoom("Bree Brownhaven/Ormenel 15x11");

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
            SetVariablesForIndefiniteCasts(oShirriff);

            Room oValveChamber = AddRoom("Valve Chamber");
            AddExit(breeSewers[7, 3], oValveChamber, "valve");
            AddExit(oValveChamber, breeSewers[7, 3], "south");

            Room oSewerPassageFromValveChamber = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerPassageFromValveChamber, oValveChamber, BidirectionalExitType.NorthSouth);

            Room oSewerDemonThreshold = AddRoom("Sewer Demon Threshold");
            oSewerDemonThreshold.Mob1 = "demon";
            AddBidirectionalExits(oSewerDemonThreshold, oSewerPassageFromValveChamber, BidirectionalExitType.SoutheastNorthwest);

            Room oPoorAlley1 = AddRoom("Bree Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");

            Room oCampusFreeClinic = AddRoom("Bree Campus Free Clinic");
            oCampusFreeClinic.Mob1 = "Student";
            oCampusFreeClinic.IsHealingRoom = true;
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");

            Room oBreeRealEstateOffice = AddRoom("Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);

            oIxell = AddRoom("Ixell 70 Bl");
            oIxell.Mob1 = "Ixell";
            AddExit(oBreeRealEstateOffice, oIxell, "door");
            AddExit(oIxell, oBreeRealEstateOffice, "out");
            SetVariablesForIndefiniteCasts(oIxell);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);

            Room oChurchsEnglishGarden = AddRoom("Chuch's English Garden");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGarden, "gate", null);
            Room oFallon = AddRoom("Fallon");
            oFallon.Experience1 = 350;
            oFallon.Alignment = AlignmentType.Blue;
            AddExit(oChurchsEnglishGarden, oFallon, "door");
            AddExit(oFallon, oChurchsEnglishGarden, "out");
            SetVariablesForIndefiniteCasts(oFallon);
            oFallon.Mob1 = "Fallon";

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
            SetVariablesForIndefiniteCasts(oGrant);

            Room oPansy = AddRoom("Pansy Smallburrows");
            oPansy.Mob1 = "Pansy";
            oPansy.Experience1 = 95;
            oPansy.Alignment = AlignmentType.Red;
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            SetVariablesForIndefiniteCasts(oPansy);

            Room oDroolie = AddRoom("Droolie");
            oDroolie.Mob1 = "Droolie";
            oDroolie.Experience1 = 100;
            oDroolie.Alignment = AlignmentType.Red;
            AddExit(oNorthBridge, oDroolie, "rope");
            AddExit(oDroolie, oNorthBridge, "up");
            SetVariablesForIndefiniteCasts(oDroolie);

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
            AddBidirectionalSameNameExit(oRockyAlcove, oSewerDrain, "grate", null);

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
            AddBidirectionalSameNameExit(oBoardedSewerTunnel, oSewerOrcChamber, "busted", null);

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
            SetVariablesForIndefiniteCasts(oIgor);

            Room oSnarlingMutt = AddRoom("Snarling Mutt");
            oSnarlingMutt.Mob1 = "Mutt";
            oSnarlingMutt.Experience1 = 50;
            oSnarlingMutt.Alignment = AlignmentType.Red;
            AddExit(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            AddExit(oSnarlingMutt, oToSnarSlystoneShoppe, "out");
            SetVariablesForIndefiniteCasts(oSnarlingMutt);

            Room oGuido = AddRoom("Guido");
            oGuido.Mob1 = "Guido";
            oGuido.Experience1 = 350;
            oGuido.Alignment = AlignmentType.Red;
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            SetVariablesForIndefiniteCasts(oGuido);

            Room oSergeantGrimdall = AddRoom("Sergeant Grimdall");
            oSergeantGrimdall.Mob1 = "Sergeant";
            oSergeantGrimdall.Experience1 = 350;
            oSergeantGrimdall.Alignment = AlignmentType.Blue;
            AddExit(oToBarracks, oSergeantGrimdall, "barracks");
            AddExit(oSergeantGrimdall, oToBarracks, "east");
            SetVariablesForIndefiniteCasts(oSergeantGrimdall);

            oBigPapa.Mob1 = "papa";
            oBigPapa.Experience1 = 350;
            oBigPapa.Alignment = AlignmentType.Blue;
            SetVariablesForIndefiniteCasts(oBigPapa);

            Room oBreePawnShopWest = AddRoom("Bree Pawn Shop West (Ixell's Antique Shop)");
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);

            Room oBreePawnShopEast = AddRoom("Bree Pawn Shop East");
            AddBidirectionalExits(oPoorAlley1, oBreePawnShopEast, BidirectionalExitType.WestEast);

            Room oLeonardosFoundry = AddRoom("Bree Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");

            Room oLeonardosSwords = AddRoom("Bree Leonardo's Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);

            Room oZooEntrance = AddRoom("Scranlin's Zoological Wonders");
            AddExit(oToZoo, oZooEntrance, "zoo");
            AddExit(oZooEntrance, oToZoo, "exit");

            Room oPathThroughScranlinsZoo = AddRoom("Path through Scranlin's Zoo");
            AddBidirectionalExits(oPathThroughScranlinsZoo, oZooEntrance, BidirectionalExitType.NorthSouth);

            Room oScranlinsPettingZoo = AddRoom("Scranlin's Petting Zoo");
            e = AddExit(oPathThroughScranlinsZoo, oScranlinsPettingZoo, "north");
            e.OmitGo = true;
            AddExit(oScranlinsPettingZoo, oPathThroughScranlinsZoo, "south");

            Room oScranlinThreshold = AddRoom("Scranlin Threshold");
            AddExit(oScranlinsPettingZoo, oScranlinThreshold, "clearing");
            AddExit(oScranlinThreshold, oScranlinsPettingZoo, "gate");

            Room oScranlin = AddRoom("Scranlin");
            oScranlin.Mob1 = oScranlinThreshold.Mob1 = "Scranlin";
            oScranlin.Experience1 = 500;
            oScranlin.Alignment = AlignmentType.Red;
            AddExit(oScranlinThreshold, oScranlin, "outhouse");
            AddExit(oScranlin, oScranlinThreshold, "out");
            SetVariablesForPermWithThreshold(oScranlin, oScranlinThreshold, "outhouse", null);

            Room oTunnel = AddRoom("Tunnel");
            AddBidirectionalSameNameExit(breeSewers[0, 10], oTunnel, "tunnel", null);

            Room oLatrine = AddRoom("Latrine");
            AddExit(oTunnel, oLatrine, "south");
            e = AddExit(oLatrine, oTunnel, "north");
            e.OmitGo = true;

            Room oEugenesDungeon = AddRoom("Eugene's Dungeon");
            AddBidirectionalExits(oEugenesDungeon, oLatrine, BidirectionalExitType.SouthwestNortheast);

            Room oShadowOfIncendius = AddRoom("Shadow of Incendius");
            AddBidirectionalExits(oShadowOfIncendius, oEugenesDungeon, BidirectionalExitType.WestEast);

            Room oEugeneTheExecutioner = AddRoom("Eugene the Executioner");
            AddExit(oEugenesDungeon, oEugeneTheExecutioner, "up");

            Room oBurnedRemainsOfNimrodel = AddRoom("Nimrodel");
            oBurnedRemainsOfNimrodel.Mob1 = "Nimrodel";
            oBurnedRemainsOfNimrodel.Experience1 = 300;
            AddExit(oEugeneTheExecutioner, oBurnedRemainsOfNimrodel, "out");
            AddExit(oBurnedRemainsOfNimrodel, oEugeneTheExecutioner, "door");
            SetVariablesForIndefiniteCasts(oBurnedRemainsOfNimrodel);

            aqueduct = AddRoom("Aqueduct");
            AddExit(oBurnedRemainsOfNimrodel, aqueduct, "pipe");
            AddExit(aqueduct, oBurnedRemainsOfNimrodel, "out");

            _boatswain = AddRoom("Boatswain");
            _boatswain.Mob1 = "Boatswain";
            _boatswain.Experience1 = 350;
            AddLocation(_aShips, _boatswain);

            AddLocation(_aBreePerms, oOrderOfLove);
            AddLocation(_aBreePerms, oCampusFreeClinic);
            AddLocation(_aInaccessible, oGrant);
            AddLocation(_aBreePerms, oIgor);
            AddLocation(_aBreePerms, oGuido);
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
            AddLocation(_aMisc, oBreePawnShopWest);
            AddLocation(_aMisc, oBreePawnShopEast);
            AddLocation(_aMisc, oLeonardosSwords);
            AddLocation(_aBreePerms, oShirriff);
            AddLocation(aBree, oOohlgrist);
            AddLocation(aBree, oHermitsCave);
            AddLocation(aBree, oSewerOrcChamber);
        }

        private void AddGridBidirectionalExits(Room[,] grid, int x, int y)
        {
            Room r = grid[x, y];
            if (r != null)
            {
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

        private void SetVariablesForIndefiniteCasts(Room perm)
        {
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNDIRECTION, string.Empty);
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNPRECOMMAND, string.Empty);
        }

        private void SetVariablesForPermWithThreshold(Room perm, Room threshold, string hitAndRunDirection, string hitAndRunPrecommand)
        {
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNDIRECTION, string.Empty);
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNPRECOMMAND, string.Empty);
            AddRoomVariableValue(threshold, VARIABLE_HITANDRUNDIRECTION, hitAndRunDirection);
            AddRoomVariableValue(threshold, VARIABLE_HITANDRUNPRECOMMAND, hitAndRunPrecommand);
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
            SetVariablesForIndefiniteCasts(oPathToMansion4WarriorBardsx2);

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
            SetVariablesForIndefiniteCasts(oGrandPorch);

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
            SetVariablesForIndefiniteCasts(oWarriorBardMansionNorth);

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
            SetVariablesForIndefiniteCasts(oWarriorBardMansionSouth);

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
            SetVariablesForIndefiniteCasts(oWarriorBardMansionEast);

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
            SetVariablesForIndefiniteCasts(oMayorMillwood);

            oChancellorOfProtection = AddRoom("Chancellor of Protection");
            oChancellorOfProtection.Experience1 = 200;
            oChancellorOfProtection.Alignment = AlignmentType.Blue;
            e = AddExit(oRoyalHallwayToChancellor, oChancellorOfProtection, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oChancellorOfProtection, oRoyalHallwayToChancellor, "out");
            oChancellorOfProtection.Mob1 = oRoyalHallwayToChancellor.Mob1 = "chancellor";
            SetVariablesForIndefiniteCasts(oChancellorOfProtection);

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

            Room oGreatEastRoad2 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoad1, oGreatEastRoad2, BidirectionalExitType.WestEast);

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

            Room oOuthouse = AddRoom("Outhouse");
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
            AddBidirectionalSameNameExit(oSmallPlayground, oUglyKidSchoolEntrance, "gate", null);

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
            SetVariablesForIndefiniteCasts(oRoadToFarm7HoundDog);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Manager Mulloy Threshold");
            oFarmParlorManagerMulloyThreshold.Mob1 = "manager";
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", "open door");
            Room oManagerMulloy = AddRoom("Manager Mulloy");
            oManagerMulloy.Mob1 = "manager";
            oManagerMulloy.Experience1 = 600;
            oManagerMulloy.Alignment = AlignmentType.Blue;
            AddExit(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            AddExit(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "out");
            SetVariablesForPermWithThreshold(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "study", null);

            Room oCrabbe = AddRoom("Crabbe");
            oCrabbe.Mob1 = "Crabbe";
            oCrabbe.Experience1 = 250;
            AddExit(oHallway, oCrabbe, "detention");
            AddExit(oCrabbe, oHallway, "out");
            SetVariablesForIndefiniteCasts(oCrabbe);

            Room oMrWartnose = AddRoom("Mr. Wartnose");
            oMrWartnose.Mob1 = "Wartnose";
            oMrWartnose.Experience1 = 235;
            AddExit(oUglyKidClassroomK7, oMrWartnose, "office");
            AddExit(oMrWartnose, oUglyKidClassroomK7, "out");
            SetVariablesForIndefiniteCasts(oMrWartnose);

            Room oCatchBasin = AddRoom("Catch Basin");
            AddExit(oOuthouse, oCatchBasin, "hole");
            AddExit(oCatchBasin, oOuthouse, "out");

            Room oSepticTank = AddRoom("Septic Tank");
            AddBidirectionalSameNameExit(oCatchBasin, oSepticTank, "grate", null);

            Room oDrainPipe1 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oSepticTank, oDrainPipe1, "pipe", null);

            Room oDrainPipe2 = AddRoom("Drain Pipe");
            AddBidirectionalSameNameExit(oDrainPipe1, oDrainPipe2, "culvert", null);

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
            SetVariablesForIndefiniteCasts(oSalamander);

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
            SetVariablesForIndefiniteCasts(oSpriteGuards);

            AddLocation(_aBreePerms, oRoadToFarm7HoundDog);
            AddLocation(_aBreePerms, oManagerMulloy);
            AddSubLocation(oManagerMulloy, oFarmParlorManagerMulloyThreshold);
            AddLocation(_aBreePerms, oSalamander);
            AddLocation(_aInaccessible, oMrWartnose);
            AddLocation(_aInaccessible, oCrabbe);
            AddLocation(_aBreePerms, oGreatEastRoadGoblinAmbushGobLrgLrg);
            AddLocation(_aBreePerms, oNorthBrethilForest5GobAmbush);
            AddSubLocation(oNorthBrethilForest5GobAmbush, oNorthBrethilForest4GobAmbushThreshold);
            AddLocation(_aBreePerms, oSpriteGuards);
            AddLocation(_aMisc, _breeEastGateOutside);
        }

        private void AddImladrisCity(out Room oImladrisSouthGateInside)
        {
            Area oImladris = _areasByName[AREA_IMLADRIS];

            _imladrisWestGateInside = AddRoom("West Gate of Imladris");
            AddExit(_imladrisWestGateInside, _imladrisWestGateOutside, "gate");

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
            AddBidirectionalSameNameExit(oImladrisPawnShop, oImladrisSmallAlley2, "door", null);

            Room oImladrisTownCircle = AddRoom("Imladris Town Circle");
            AddBidirectionalExits(oImladrisAlley3, oImladrisTownCircle, BidirectionalExitType.WestEast);

            Room oImladrisMainStreet6 = AddRoom("Imladris Main Street");
            AddBidirectionalExits(oImladrisTownCircle, oImladrisMainStreet6, BidirectionalExitType.WestEast);

            Room oEastGateOfImladrisInside = AddRoom("East Gate of Imladris");
            AddBidirectionalExits(oImladrisMainStreet6, oEastGateOfImladrisInside, BidirectionalExitType.WestEast);

            Room oEastGateOfImladrisOutside = AddRoom("Gates of Imladris");
            AddBidirectionalSameNameExit(oEastGateOfImladrisInside, oEastGateOfImladrisOutside, "gate", null);

            Room oImladrisCircle10 = AddRoom("Imladris Circle");
            AddBidirectionalExits(_imladrisWestGateInside, oImladrisCircle10, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisCircle9 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle10, oImladrisCircle9, BidirectionalExitType.SoutheastNorthwest);

            Room oImladrisCircle8 = AddRoom("Imladris Circle");
            AddBidirectionalExits(oImladrisCircle9, oImladrisCircle8, BidirectionalExitType.SoutheastNorthwest);
            AddExit(oImladrisAlley5, oImladrisCircle8, "south");

            Room oRearAlley = AddRoom("Master Assassin Threshold"); //Rear Alley
            AddBidirectionalExits(oImladrisCircle10, oRearAlley, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);

            Room oPoisonedDagger = AddRoom("Master Assassins");
            oPoisonedDagger.Mob1 = oRearAlley.Mob1 = "assassin";
            oPoisonedDagger.Experience1 = 600;
            oPoisonedDagger.Experience2 = 600;
            AddBidirectionalSameNameExit(oRearAlley, oPoisonedDagger, "door", null);
            SetVariablesForPermWithThreshold(oPoisonedDagger, oRearAlley, "door", null);

            oImladrisSouthGateInside = AddRoom("Southern Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);

            Room oImladrisCityDump = AddRoom("Imladris City Dump");
            Exit e = AddExit(oImladrisCircle8, oImladrisCityDump, "north");
            e.OmitGo = true;
            AddExit(oImladrisCityDump, oImladrisCircle8, "south");
            AddExit(oImladrisCityDump, oRearAlley, "north");

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
            SetVariablesForIndefiniteCasts(oIorlas);

            AddLocation(_aImladrisTharbadPerms, oImladrisHealingHand);
            AddLocation(_aImladrisTharbadPerms, oIorlas);
            AddLocation(_aImladrisTharbadPerms, oPoisonedDagger);
            AddSubLocation(oPoisonedDagger, oRearAlley);
            AddLocation(_aMisc, oImladrisPawnShop);
            AddLocation(_aMisc, oTyriesPriestSupplies);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside, Room aqueduct)
        {
            Area oBreeToHobbiton = _areasByName[AREA_BREE_TO_HOBBITON];

            Room oBreeWestGateOutside = AddRoom("West Gate of Bree");
            AddBidirectionalSameNameExit(oBreeWestGateInside, oBreeWestGateOutside, "gate", null);

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
            AddBidirectionalSameNameExit(oBilboFrodoHobbitHoleCondo, oBilboFrodoCommonArea, "curtain", null);

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
            SetVariablesForIndefiniteCasts(oBilboBaggins);

            Room oFrodoBaggins = AddRoom("Frodo Baggins");
            oFrodoBaggins.Mob1 = "Frodo";
            oFrodoBaggins.Experience1 = 260;
            oFrodoBaggins.Alignment = AlignmentType.Blue;
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain", null);
            SetVariablesForIndefiniteCasts(oFrodoBaggins);

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes");
            AddExit(oGreatHallOfHeroes, oLeviathanNorthForkWestern, "out");
            AddExit(oLeviathanNorthForkWestern, oGreatHallOfHeroes, "hall");

            //something is hasted
            Room oSomething = AddRoom("Something");
            oSomething.Mob1 = "Something";
            oSomething.Experience1 = 140;
            if (_level < 11)
            {
                AddExit(oGreatHallOfHeroes, oSomething, "curtain");
            }
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");
            SetVariablesForIndefiniteCasts(oSomething);

            Room oShepherd = AddRoom("Shepherd");
            oShepherd.Mob1 = "Shepherd";
            oShepherd.Experience1 = 60;
            oShepherd.Alignment = AlignmentType.Blue;
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");
            SetVariablesForIndefiniteCasts(oShepherd);

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
            SetVariablesForIndefiniteCasts(oKasnarTheGuard);

            AddExit(aqueduct, oKasnarTheGuard, "north");
            //AddExit(oKasnarTheGuard, aqueduct, "south") //Exit is locked and knockable but not treating as an exit for the mapping

            AddLocation(_aInaccessible, oSomething);
            AddLocation(_aBreePerms, oBilboBaggins);
            AddLocation(_aBreePerms, oFrodoBaggins);
            AddLocation(oBreeToHobbiton, oShepherd);
            AddLocation(_aBreePerms, oKasnarTheGuard);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            Area oImladrisToTharbad = _areasByName[AREA_IMLADRIS_TO_THARBAD];

            Room oMistyTrail1 = AddRoom("Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate", null);

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
            SetVariablesForPermWithThreshold(oCutthroatAssassin, oCutthroatAssassinThreshold, "west", null);

            Room oMistyTrail2 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);

            Room oMistyTrail3 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);

            Room oMistyTrail4 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);

            Room oPotionFactoryReception = AddRoom("Reception Area of Potion Factory");
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
            SetVariablesForIndefiniteCasts(oMarkFrey);

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
            SetVariablesForIndefiniteCasts(oPrinceBrunden);

            Room oNaugrim = AddRoom("Naugrim");
            oNaugrim.Mob1 = "Naugrim";
            oNaugrim.Experience1 = 205;
            oNaugrim.Alignment = AlignmentType.Red;
            AddExit(oNorthShantyTown, oNaugrim, "cask");
            AddExit(oNaugrim, oNorthShantyTown, "out");
            SetVariablesForIndefiniteCasts(oNaugrim);

            Room oHogoth = AddRoom("Hogoth");
            oHogoth.Mob1 = "Hogoth";
            oHogoth.Experience1 = 260;
            oHogoth.Alignment = AlignmentType.Blue;
            AddExit(oShantyTownWest, oHogoth, "shack");
            AddExit(oHogoth, oShantyTownWest, "out");
            SetVariablesForIndefiniteCasts(oHogoth);

            Room oFaornil = AddRoom("Faornil");
            oFaornil.Mob1 = "Faornil";
            oFaornil.Experience1 = 250;
            oFaornil.Alignment = AlignmentType.Red;
            AddExit(oShantyTown1, oFaornil, "tent");
            AddExit(oFaornil, oShantyTown1, "out");
            SetVariablesForIndefiniteCasts(oFaornil);

            Room oGraddy = AddRoom("Graddy");
            oGraddy.Mob1 = "Graddy";
            oGraddy.Experience1 = 350;
            AddExit(oShantyTown2, oGraddy, "wagon");
            AddExit(oGraddy, oShantyTown2, "out");
            SetVariablesForIndefiniteCasts(oGraddy);

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

        internal class Room
        {
            public override string ToString()
            {
                string ret = Name;
                if (Experience1.HasValue)
                {
                    if (Experience2.HasValue)
                    {
                        if (Experience3.HasValue)
                        {
                            ret += (" " + Experience1.Value + "/" + Experience2.Value + "/" + Experience3.Value);
                        }
                        else
                        {
                            ret += (" " + Experience1.Value + "/" + Experience2.Value);
                        }
                    }
                    else
                    {
                        ret += (" " + Experience1.Value);
                    }
                }
                if (Alignment.HasValue)
                {
                    ret += " ";
                    string sAlign = string.Empty;
                    switch (Alignment.Value)
                    {
                        case AlignmentType.Blue:
                            sAlign = "Bl";
                            break;
                        case AlignmentType.Grey:
                            sAlign = "Gy";
                            break;
                        case AlignmentType.Red:
                            sAlign = "Rd";
                            break;
                    }
                    ret += sAlign;
                }
                return ret;
            }

            public Room(string name)
            {
                this.Name = name;
            }
            public string Name { get; set; }
            public string Mob1 { get; set; }
            public string Mob2 { get; set; }
            public string Mob3 { get; set; }
            public Dictionary<Variable, string> VariableValues { get; set; }
            public List<Room> SubLocations { get; set; }
            public Room ParentRoom { get; set; }
            public AlignmentType? Alignment { get; set; }
            public int? Experience1 { get; set; }
            public int? Experience2 { get; set; }
            public int? Experience3 { get; set; }
            public bool IsHealingRoom { get; set; }

            public string GetDefaultMob()
            {
                string defaultMob = this.Mob1;
                if (!string.IsNullOrEmpty(this.Mob1))
                {
                    if (!string.IsNullOrEmpty(this.Mob2))
                    {
                        if (!string.Equals(defaultMob, this.Mob2, StringComparison.OrdinalIgnoreCase))
                        {
                            defaultMob = string.Empty;
                        }
                        else if (!string.Equals(defaultMob, this.Mob3, StringComparison.OrdinalIgnoreCase))
                        {
                            defaultMob = string.Empty;
                        }
                    }
                }
                return defaultMob;
            }
        }

        public enum AlignmentType
        {
            Blue,
            Grey,
            Red,
        }

        internal class Exit : Edge<Room>
        {
            public override string ToString()
            {
                return Source.ToString() + "--" + ExitText + " -->" + Target.ToString();
            }
            /// <summary>
            /// text for the exit
            /// </summary>
            public string ExitText { get; set; }
            /// <summary>
            /// whether to omit go for the exit
            /// </summary>
            public bool OmitGo { get; set; }
            /// <summary>
            /// command to run before using the exit
            /// </summary>
            public string PreCommand { get; set; }
            public Exit(Room source, Room target, string exitText) : base(source, target)
            {
                this.ExitText = exitText;
            }
        }

        private void SendCommand(string command, bool IsPassword, bool runIfQuitting)
        {
            if (!_quitting || runIfQuitting)
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
                DoSingleMove(move, "go " + move);
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
            DoSingleMove(direction, cmd);
        }

        private void DoSingleMove(string direction, string command)
        {
            SendCommand(command, false, false);
            if (m_oCurrentRoom != null)
            {
                Room newRoom = null;
                if (_map.TryGetOutEdges(m_oCurrentRoom, out IEnumerable<Exit> edges))
                {
                    foreach (Exit nextExit in edges)
                    {
                        if (string.Equals(nextExit.ExitText, direction, StringComparison.OrdinalIgnoreCase))
                        {
                            newRoom = nextExit.Target;
                        }
                    }
                }
                if (newRoom != null)
                {
                    SetCurrentRoom(newRoom);
                }
                else
                {
                    m_oCurrentRoom = null;
                    txtCurrentRoom.Text = string.Empty;
                }
            }
        }

        private void ctxGoSingleDirection_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Button btn = (Button)ctxGoSingleDirection.SourceControl;
            string direction = btn.Tag.ToString();
            Room newRoom = null;
            if (_map.TryGetOutEdges(m_oCurrentRoom, out IEnumerable<Exit> edges))
            {
                foreach (Exit nextExit in edges)
                {
                    if (string.Equals(nextExit.ExitText, direction, StringComparison.OrdinalIgnoreCase))
                    {
                        newRoom = nextExit.Target;
                    }
                }
            }
            if (newRoom != null)
            {
                SetCurrentRoom(newRoom);
            }
            else
            {
                MessageBox.Show("Cannot go that direction.");
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
            _quitting = true;
            SendCommand("quit", false, true);
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
                    SendCommand(command, false, false);
                }
            }
            else
                MessageBox.Show(errorMessage);
        }

        private void btnSetCurrentLocation_Click(object sender, EventArgs e)
        {
            TreeNode oSelectedTreeNode = treeLocations.SelectedNode as TreeNode;
            if (oSelectedTreeNode != null)
            {
                Room r = oSelectedTreeNode.Tag as Room;
                if (r != null)
                {
                    SetCurrentRoom(r);
                }
            }
        }

        private void btnGoToLocation_Click(object sender, EventArgs e)
        {
            if (m_oCurrentRoom == null)
            {
                MessageBox.Show("No current room.");
                return;
            }
            TreeNode oSelectedTreeNode = treeLocations.SelectedNode;
            if (oSelectedTreeNode == null)
            {
                MessageBox.Show("Nothing selected in tree.");
                return;

            }

            Room targetRoom = oSelectedTreeNode.Tag as Room;
            if (targetRoom == null)
            {
                MessageBox.Show("Selected tree node is not a room.");
                return;
            }

            _currentBackgroundParameters = new BackgroundWorkerParameters();
            int moveGapMS;
            if (_variablesByName.TryGetValue(VARIABLE_MOVEGAPMS, out Variable movegapmsvar) && movegapmsvar is IntegerVariable)
                moveGapMS = ((IntegerVariable)movegapmsvar).Value;
            else
                moveGapMS = 260;
            _currentBackgroundParameters.WaitMS = moveGapMS;
            _currentBackgroundParameters.TargetRoom = targetRoom;
            _currentBackgroundParameters.AutoHazy = chkAutoHazy.Checked;
            _currentBackgroundParameters.WasPowerAttackAvailableAtStart = IsPowerAttackAvailable();
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
                string nextCommand = exit.ExitText;
                if (!exit.OmitGo) nextCommand = "go " + nextCommand;
                commands.Add(new MacroCommand(nextCommand, nextCommand));
            }

            RunCommands(commands, _currentBackgroundParameters);
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
            public Dictionary<string, Variable> Variables { get; set; }
            public int? NextCommandWaitMS { get; set; }
            public int WaitMS { get; set; }
            public bool Cancelled { get; set; }
            public int CommandsRun { get; set; }
            public Macro Macro { get; set; }
            public string QueuedCommand { get; set; }
            public string FinalCommand { get; set; }
            public string FinalCommand2 { get; set; }
            public int MaxOffensiveLevel { get; set; }
            public bool AutoMana { get; set; }
            public bool PowerAttack { get; set; }
            public bool AutoHazy { get; set; }
            public bool WasPowerAttackAvailableAtStart { get; set; }

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
            SendCommand(command, false, false);
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
            RunMacro((Macro)cboMacros.SelectedItem);
        }

        private void RunMacro(Macro m)
        {
            bool powerAttack = ((m.CombatCommandTypes & CommandType.Melee) == CommandType.Melee) && chkPowerAttack.Checked;
            if (powerAttack)
            {
                if (string.IsNullOrEmpty(txtWeapon.Text))
                {
                    MessageBox.Show("No weapon specified.");
                    return;
                }
            }
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
            _currentBackgroundParameters = new BackgroundWorkerParameters();
            _currentBackgroundParameters.Macro = m;
            _currentBackgroundParameters.FinalCommand = sFinalCommand;
            _currentBackgroundParameters.FinalCommand2 = sFinalCommand2;
            _currentBackgroundParameters.MaxOffensiveLevel = Convert.ToInt32(cboMaxOffLevel.SelectedItem.ToString());
            _currentBackgroundParameters.AutoMana = chkAutoMana.Checked;
            _currentBackgroundParameters.AutoHazy = chkAutoHazy.Checked;
            _currentBackgroundParameters.WasPowerAttackAvailableAtStart = IsPowerAttackAvailable();
            _currentBackgroundParameters.PowerAttack = powerAttack;
            RunCommands(stepsToRun, _currentBackgroundParameters);
        }

        private bool IsPowerAttackAvailable()
        {
            SkillCooldownStatus oStatus = _skillCooldowns[SkillWithCooldownType.PowerAttack];
            return !oStatus.IsActive && oStatus.NextAvailableDate.HasValue && oStatus.NextAvailableDate.Value <= DateTime.UtcNow;
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
            else if (input is MacroManaSpellStun)
            {
                TranslateCommand("cast stun {mob}", _variables, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage)) return null;
                ret = input;
                isSameStep = true;
            }
            else if (input is MacroManaSpellOffensive)
            {
                //the spell isn't known at this point, but it doesn't matter since all we need to 
                //do here is verify the mob variable is present.
                TranslateCommand("cast rumble {mob}", _variables, out errorMessage);
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
            bool autoMana = chkAutoMana.Checked;
            if (autoMana)
            {
                _currentMana = _autoMana;
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
            if (_autoHitpoints.HasValue)
            {
                txtHitpoints.Text = _autoHitpoints.Value.ToString() + "/" + _totalhp;
                Color backColor;
                if (_autoHitpoints.Value == _totalhp)
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
                string sPreviousText = txt.Text;
                if (eType == SkillWithCooldownType.PowerAttack && sText == "0:00" && !string.Equals(sPreviousText, sText) && !string.IsNullOrEmpty(txtWeapon.Text) && !btnAbort.Enabled)
                {
                    chkPowerAttack.Checked = true;
                }
                txt.Text = sText;
                txt.BackColor = backColor;
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
                    CheckAutoHazy(chkAutoHazy.Checked, dtUtcNow);
                }

                //check for poll tick if a macro is not running and the first status update has completed and not at full HP+MP
                if (_currentStatusLastComputed.HasValue && (!_autoHitpoints.HasValue || _autoHitpoints.Value < _totalhp || !_autoMana.HasValue || _autoMana.Value < _totalmp))
                {
                    bool runPollTick = (dtUtcNow - _currentStatusLastComputed.Value).TotalSeconds >= 5;
                    if (runPollTick && _lastPollTick.HasValue)
                    {
                        runPollTick = (dtUtcNow - _lastPollTick.Value).TotalSeconds >= 5;
                    }
                    if (runPollTick)
                    {
                        _lastPollTick = dtUtcNow;
                        SendCommand(string.Empty, false, false);
                    }
                }
                if (_doScore)
                {
                    _doScore = false;
                    SendCommand("score", false, false);
                }
                if (_currentStatusLastComputed.HasValue && !_ranStartupCommands)
                {
                    _ranStartupCommands = true;
                    foreach (string nextCommand in _startupCommands)
                    {
                        SendCommand(nextCommand, false, false);
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
        }

        private void CheckAutoHazy(bool AutoHazyActive, DateTime dtUtcNow)
        {
            if (m_oCurrentRoom != _treeOfLife && !_autoHazied && AutoHazyActive && _autoHitpoints.HasValue && _autoHitpoints.Value < _autoHazyThreshold && (!_lastTriedToAutoHazy.HasValue || ((dtUtcNow - _lastTriedToAutoHazy.Value) > new TimeSpan(0, 0, 2))))
            {
                _lastTriedToAutoHazy = dtUtcNow;
                SendCommand("drink hazy", false, false);
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
                SendCommand(txtOneOffCommand.Text, false, false);
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
            btnEmote.Enabled = !string.IsNullOrEmpty(txtEmoteText.Text);
        }

        private void btnEmote_Click(object sender, EventArgs e)
        {
            SendCommand("emote " + txtEmoteText.Text, false, false);
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
            _fleeing = true;
            if (_currentBackgroundParameters == null)
            {
                _currentBackgroundParameters = new BackgroundWorkerParameters();
                _currentBackgroundParameters.MaxOffensiveLevel = Convert.ToInt32(cboMaxOffLevel.SelectedItem.ToString());
                _currentBackgroundParameters.AutoMana = chkAutoMana.Checked;
                _currentBackgroundParameters.AutoHazy = chkAutoHazy.Checked;
                _currentBackgroundParameters.WasPowerAttackAvailableAtStart = IsPowerAttackAvailable();
                _currentBackgroundParameters.PowerAttack = false;
                RunCommands(new List<MacroStepBase>(), _currentBackgroundParameters);
            }
        }
    }
}
