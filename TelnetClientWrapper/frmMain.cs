using QuickGraph;
using QuickGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace IsengardClient
{
    public partial class frmMain : Form
    {
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int VK_RETURN = 0x0D;

        private int _level;
        private Dictionary<char, int> _keyMapping;
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
        private Room _breeEastGateInside = null;
        private Room _breeEastGateOutside = null;
        private Room _imladrisWestGateInside = null;
        private Room _imladrisWestGateOutside = null;
        private object _queuedCommandLock = new object();
        private Area _aBreePerms;
        private Area _aImladrisTharbadPerms;
        private Area _aMisc;
        private Area _aInaccessible;

        private const string AREA_BREE_PERMS = "Bree Perms";
        private const string AREA_IMLADRIS_THARBAD_PERMS = "Imladris/Tharbad Perms";
        private const string AREA_BREE = "Bree";
        private const string AREA_BREE_TO_HOBBITON = "Bree to Hobbiton";
        private const string AREA_BREE_TO_IMLADRIS = "Bree to Imladris";
        private const string AREA_IMLADRIS = "Imladris";
        private const string AREA_IMLADRIS_TO_THARBAD = "Imladris to Tharbad";
        private const string AREA_MISC = "Misc";
        private const string AREA_INTANGIBLE = "Intangible";
        private const string AREA_INACCESSIBLE = "Inaccessible";

        private const string VARIABLE_MOVEGAPMS = "movegapms";
        private const string VARIABLE_LEVEL1CASTROUNDS = "level1castrounds";
        private const string VARIABLE_LEVEL2CASTROUNDS = "level2castrounds";
        private const string VARIABLE_LEVEL3CASTROUNDS = "level3castrounds";
        private const string VARIABLE_HITANDRUNDIRECTION = "hitandrundirection";
        private const string VARIABLE_HITANDRUNPRECOMMAND = "hitandrunprecommand";
        private const string VARIABLE_STUNCASTROUNDS = "stuncastrounds";

        private const int PRIORITY_HEALING = 1;
        private const int PRIORITY_PERMS_MAIN = 2;
        private const int PRIORITY_PERMS_BIG = 3;
        private const int PRIORITY_PERMS_LESS = 4;

        public frmMain()
        {
            InitializeComponent();

            _areas = new List<Area>();
            _areasByName = new Dictionary<string, Area>();
            _aBreePerms = AddArea(AREA_BREE_PERMS);
            _aImladrisTharbadPerms = AddArea(AREA_IMLADRIS_THARBAD_PERMS);
            AddArea(AREA_BREE);
            AddArea(AREA_BREE_TO_HOBBITON);
            AddArea(AREA_BREE_TO_IMLADRIS);
            AddArea(AREA_IMLADRIS);
            AddArea(AREA_IMLADRIS_TO_THARBAD);
            _aMisc = AddArea(AREA_MISC);
            AddArea(AREA_INTANGIBLE);
            _aInaccessible = AddArea(AREA_INACCESSIBLE);

            _bw = new BackgroundWorker();
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            //virtual key code mapping
            _keyMapping = new Dictionary<char, int>();
            _keyMapping[' '] = 0x20;
            _keyMapping['0'] = 0x30;
            _keyMapping['1'] = 0x31;
            _keyMapping['2'] = 0x32;
            _keyMapping['3'] = 0x33;
            _keyMapping['4'] = 0x34;
            _keyMapping['5'] = 0x35;
            _keyMapping['6'] = 0x36;
            _keyMapping['7'] = 0x37;
            _keyMapping['8'] = 0x38;
            _keyMapping['9'] = 0x39;
            _keyMapping['A'] = 0x41;
            _keyMapping['a'] = 0x41;
            _keyMapping['B'] = 0x42;
            _keyMapping['b'] = 0x42;
            _keyMapping['C'] = 0x43;
            _keyMapping['c'] = 0x43;
            _keyMapping['D'] = 0x44;
            _keyMapping['d'] = 0x44;
            _keyMapping['E'] = 0x45;
            _keyMapping['e'] = 0x45;
            _keyMapping['F'] = 0x46;
            _keyMapping['f'] = 0x46;
            _keyMapping['G'] = 0x47;
            _keyMapping['g'] = 0x47;
            _keyMapping['H'] = 0x48;
            _keyMapping['h'] = 0x48;
            _keyMapping['I'] = 0x49;
            _keyMapping['i'] = 0x49;
            _keyMapping['J'] = 0x4A;
            _keyMapping['j'] = 0x4A;
            _keyMapping['K'] = 0x4B;
            _keyMapping['k'] = 0x4B;
            _keyMapping['L'] = 0x4C;
            _keyMapping['l'] = 0x4C;
            _keyMapping['M'] = 0x4D;
            _keyMapping['m'] = 0x4D;
            _keyMapping['N'] = 0x4E;
            _keyMapping['n'] = 0x4E;
            _keyMapping['O'] = 0x4F;
            _keyMapping['o'] = 0x4F;
            _keyMapping['P'] = 0x50;
            _keyMapping['p'] = 0x50;
            _keyMapping['Q'] = 0x51;
            _keyMapping['q'] = 0x51;
            _keyMapping['R'] = 0x52;
            _keyMapping['r'] = 0x52;
            _keyMapping['S'] = 0x53;
            _keyMapping['s'] = 0x53;
            _keyMapping['T'] = 0x54;
            _keyMapping['t'] = 0x54;
            _keyMapping['U'] = 0x55;
            _keyMapping['u'] = 0x55;
            _keyMapping['V'] = 0x56;
            _keyMapping['v'] = 0x56;
            _keyMapping['W'] = 0x57;
            _keyMapping['w'] = 0x57;
            _keyMapping['X'] = 0x58;
            _keyMapping['x'] = 0x58;
            _keyMapping['Y'] = 0x59;
            _keyMapping['y'] = 0x59;
            _keyMapping['Z'] = 0x5A;
            _keyMapping['z'] = 0x5A;
            _keyMapping['-'] = 0x6D;

            SetButtonTags();
            LoadConfiguration();
            InitializeMap();
            SetNightEdges(false);
            PopulateTree();

            cboSetOption.SelectedIndex = 0;
        }

        private void SetButtonTags()
        {
            btnLevel1OffensiveSpell.Tag = new CommandButtonTag("cast {realm1spell} {mob}", CommandType.Magic);
            btnLevel2OffensiveSpell.Tag = new CommandButtonTag("cast {realm2spell} {mob}", CommandType.Magic);
            btnLevel3OffensiveSpell.Tag = new CommandButtonTag("cast {realm3spell} {mob}", CommandType.Magic);
            btnFlee.Tag = new CommandButtonTag("flee", CommandType.Magic | CommandType.Melee | CommandType.Potions);
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
                    TreeNode tRoom = new TreeNode(r.Name);
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
                int ret = x.Priority.CompareTo(y.Priority);
                if (ret == 0) ret = x.Name.CompareTo(y.Name);
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

        private void LoadConfiguration()
        {
            cboMacros.Items.Add(string.Empty);
            _variables = new List<Variable>();
            _variablesByName = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);

            string configurationFile = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "Configuration.xml");
            FileInfo fi = new FileInfo(configurationFile);
            if (!fi.Exists)
            {
                MessageBox.Show("Configuration.xml does not exist!");
                return;
            }
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configurationFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Configuration.xml: " + ex.ToString());
                return;
            }

            List<string> errorMessages = new List<string>();

            string sDefaultRealm = doc.DocumentElement.GetAttribute("defaultrealm");
            if (!string.IsNullOrEmpty(sDefaultRealm))
            {
                switch (sDefaultRealm.ToLower())
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
                    default:
                        MessageBox.Show("Invalid default realm: " + sDefaultRealm);
                        break;
                }
            }

            string sLevel = doc.DocumentElement.GetAttribute("level");
            if (string.IsNullOrEmpty(sLevel))
            {
                MessageBox.Show("No level specified");
                _level = 1;
            }
            else if (!int.TryParse(sLevel, out _level))
            {
                MessageBox.Show("Invalid level specified: " + sLevel);
                _level = 1;
            }
            txtLevel.Text = _level.ToString();

            bool dupMacros = false;
            bool dupVariables = false;
            XmlElement macrosElement = null;
            XmlElement variablesElement = null;
            foreach (XmlNode nextNode in doc.DocumentElement.ChildNodes)
            {
                XmlElement elem = nextNode as XmlElement;
                if (elem == null) continue;
                string sName = elem.Name.ToLower();
                switch (sName)
                {
                    case "macros":
                        if (macrosElement == null)
                        {
                            macrosElement = elem;
                        }
                        else //duplicate
                        {
                            errorMessages.Add("Found duplicate macros nodes.");
                            dupMacros = true;
                        }
                        break;
                    case "variables":
                        if (variablesElement == null)
                        {
                            variablesElement = elem;
                        }
                        else
                        {
                            errorMessages.Add("Found duplicate variables nodes.");
                            dupVariables = true;
                        }
                        break;
                }
            }

            if (!dupVariables)
            {
                foreach (XmlNode nextNode in variablesElement.GetElementsByTagName("Variable"))
                {
                    XmlElement elemVariable = nextNode as XmlElement;
                    if (elemVariable == null)
                    {
                        errorMessages.Add("Found non-element variable node.");
                        continue;
                    }
                    string sName = elemVariable.GetAttribute("name");
                    if (string.IsNullOrEmpty(sName))
                    {
                        errorMessages.Add("Found variable with blank name.");
                        continue;
                    }
                    if (!IsValidMacroName(sName))
                    {
                        errorMessages.Add("Invalid variable name: " + sName);
                        continue;
                    }
                    if (_variablesByName.ContainsKey(sName))
                    {
                        errorMessages.Add("Duplicate variable name: " + sName);
                        continue;
                    }
                    string sType = elemVariable.GetAttribute("type");
                    if (string.IsNullOrEmpty(sType))
                    {
                        errorMessages.Add("Found variable with blank type.");
                        continue;
                    }
                    VariableType? foundVT = null;
                    foreach (VariableType vt in Enum.GetValues(typeof(VariableType)))
                    {
                        if (sType.ToLower().Equals(vt.ToString().ToLower()))
                        {
                            foundVT = vt;
                            break;
                        }
                    }
                    if (!foundVT.HasValue)
                    {
                        errorMessages.Add("Unable to find variable type: " + sType);
                        continue;
                    }

                    IntegerVariable vInt = null;
                    Variable v;
                    VariableType theVT = foundVT.Value;
                    switch (theVT)
                    {
                        case VariableType.Bool:
                            v = new BooleanVariable();
                            break;
                        case VariableType.Int:
                            vInt = new IntegerVariable();
                            v = vInt;
                            break;
                        case VariableType.String:
                            v = new StringVariable();
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    if (theVT == VariableType.Int)
                    {
                        string sMin = elemVariable.GetAttribute("min");
                        if (!string.IsNullOrEmpty(sMin))
                        {
                            if (int.TryParse(sMin, out int iMin))
                            {
                                vInt.Min = iMin;
                            }
                            else
                            {
                                errorMessages.Add("Invalid min variable value: " + sMin);
                                continue;
                            }
                        }
                        string sMax = elemVariable.GetAttribute("max");
                        if (!string.IsNullOrEmpty(sMax))
                        {
                            if (int.TryParse(sMax, out int iMax))
                            {
                                vInt.Max = iMax;
                            }
                            else
                            {
                                errorMessages.Add("Invalid max variable value: " + sMax);
                                continue;
                            }
                        }
                        if (vInt.Min.HasValue && vInt.Max.HasValue && vInt.Min.Value > vInt.Max.Value)
                        {
                            errorMessages.Add("Variable min greater than max");
                            continue;
                        }
                    }

                    string sValue = elemVariable.GetAttribute("value");
                    if (sValue == null)
                    {
                        if (theVT == VariableType.Bool)
                        {
                            ((BooleanVariable)v).Value = false;
                        }
                        else if (theVT == VariableType.Int)
                        {
                            if (vInt.Min.HasValue)
                                vInt.Value = vInt.Min.Value;
                            else
                                vInt.Value = 0;
                        }
                        else if (theVT == VariableType.String)
                        {
                            ((StringVariable)v).Value = string.Empty;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        if (theVT == VariableType.Bool)
                        {
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                if (!bool.TryParse(sValue, out bool bValue))
                                {
                                    errorMessages.Add("Invalid boolean variable value: " + sValue);
                                    continue;
                                }
                                ((BooleanVariable)v).Value = bValue;
                            }
                        }
                        else if (theVT == VariableType.Int)
                        {
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                if (!int.TryParse(sValue, out int iValue))
                                {
                                    errorMessages.Add("Invalid integer variable value: " + sValue);
                                    continue;
                                }
                                if (vInt.Min.HasValue && vInt.Min.Value > iValue)
                                {
                                    errorMessages.Add("Variable value less than min value");
                                    continue;
                                }
                                if (vInt.Max.HasValue && vInt.Max.Value < iValue)
                                {
                                    errorMessages.Add("Variable value greater than max value");
                                    continue;
                                }
                                vInt.Value = iValue;
                            }
                        }
                        else if (theVT == VariableType.String)
                        {
                            ((StringVariable)v).Value = sValue.ToLower();
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    v.Name = sName;
                    v.Type = theVT;
                    _variables.Add(v);
                    _variablesByName[sName] = v;
                }
            }

            if (!dupMacros)
            {
                int iOneClickTabIndex = 0;
                Dictionary<string, Macro> foundMacros = new Dictionary<string, Macro>(StringComparer.OrdinalIgnoreCase);
                foreach (XmlNode nextNode in macrosElement.GetElementsByTagName("Macro"))
                {
                    XmlElement elemMacro = nextNode as XmlElement;
                    if (elemMacro == null)
                    {
                        errorMessages.Add("Found non-element macro node.");
                        continue;
                    }
                    string macroName = elemMacro.GetAttribute("name");
                    if (string.IsNullOrEmpty(macroName))
                    {
                        errorMessages.Add("Found macro with blank name.");
                        continue;
                    }
                    if (foundMacros.ContainsKey(macroName))
                    {
                        errorMessages.Add("Found duplicate macro name: " + macroName);
                        continue;
                    }

                    string sSetParentLocation = elemMacro.GetAttribute("setparentlocation");
                    bool bSetParentLocation = false;
                    if (!string.IsNullOrEmpty(sSetParentLocation))
                    {
                        if (!bool.TryParse(sSetParentLocation, out bSetParentLocation))
                        {
                            errorMessages.Add("Invalid set parent location for " + macroName + " " + sSetParentLocation);
                            continue;
                        }
                    }

                    string sCombatCommandTypes = elemMacro.GetAttribute("combatcommandtypes");
                    CommandType eCombatCommandTypes = CommandType.None;
                    if (!string.IsNullOrEmpty(sCombatCommandTypes))
                    {
                        if (!Enum.TryParse(sCombatCommandTypes, out eCombatCommandTypes))
                        {
                            errorMessages.Add("Invalid combat command types for " + macroName + " " + sCombatCommandTypes);
                            continue;
                        }
                    }

                    string sFinalCommand = elemMacro.GetAttribute("finalcommand");

                    bool macroIsValid = true;
                    Macro oMacro = new Macro(macroName);
                    oMacro.SetParentLocation = bSetParentLocation;
                    oMacro.CombatCommandTypes = eCombatCommandTypes;
                    oMacro.FinalCommand = sFinalCommand;
                    List<MacroStepBase> foundSteps = ProcessStepsParentElement(elemMacro, macroName, errorMessages);
                    if (foundSteps == null)
                    {
                        macroIsValid = false;
                    }
                    else if (foundSteps.Count == 0)
                    {
                        errorMessages.Add("Macro has no steps: " + macroName);
                        macroIsValid = false;
                    }
                    oMacro.Steps = foundSteps;

                    string sOneClick = elemMacro.GetAttribute("oneclick");
                    bool isOneClick = false;
                    if (!string.IsNullOrEmpty(sOneClick))
                    {
                        if (!bool.TryParse(sOneClick, out isOneClick))
                        {
                            errorMessages.Add("Invalid one click flag for macro " + macroName);
                            macroIsValid = false;
                        }
                    }

                    if (macroIsValid)
                    {
                        foundMacros[macroName] = oMacro;
                        if (isOneClick)
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
                }
            }

            if (errorMessages.Count > 0)
            {
                MessageBox.Show("Errors loading configuration file" + Environment.NewLine + string.Join(Environment.NewLine, errorMessages));
            }
        }

        private List<MacroStepBase> ProcessStepsParentElement(XmlElement parentElement, string errorSource, List<string> errorMessages)
        {
            List<MacroStepBase> ret = new List<MacroStepBase>();
            XmlElement oStepsElem = null;
            foreach (XmlNode nextMacroNode in parentElement.ChildNodes)
            {
                XmlElement oElement = nextMacroNode as XmlElement;
                if (oElement == null) continue;
                string elemName = oElement.Name.ToLower();
                switch (elemName)
                {
                    case "steps":
                        if (oStepsElem == null)
                        {
                            oStepsElem = oElement;
                        }
                        else
                        {
                            oStepsElem = null;
                            errorMessages.Add("Found duplicate steps node: " + errorSource);
                            break;
                        }
                        break;
                }
            }
            if (oStepsElem == null)
            {
                errorMessages.Add("Failed to find steps node: " + errorSource);
                return null;
            }
            bool isValid = true;
            foreach (XmlNode nextStepNode in oStepsElem.ChildNodes)
            {
                XmlElement elemStep = nextStepNode as XmlElement;
                if (elemStep == null) continue;
                string stepType = elemStep.Name.ToLower();
                MacroStepBase step = null;
                switch (stepType)
                {
                    case "sequence":
                        List<MacroStepBase> loopSteps = ProcessStepsParentElement(elemStep, errorSource + " " + stepType, errorMessages);
                        MacroStepSequence seq = null;
                        if (loopSteps == null)
                        {
                            isValid = false;
                        }
                        else
                        {
                            seq = new MacroStepSequence();
                            seq.SubCommands = loopSteps;
                            ret.Add(seq);
                            step = seq;
                        }
                        break;
                    case "command":
                        string cmd = elemStep.GetAttribute("text");
                        if (cmd == null)
                        {
                            isValid = false;
                            errorMessages.Add("Macro step command missing text: " + errorSource);
                        }
                        else
                        {
                            MacroCommand oCommand = new MacroCommand(cmd);
                            ret.Add(oCommand);
                            step = oCommand;
                        }
                        break;
                    case "setnextcommandms":
                        MacroStepSetNextCommandWaitMS oWaitMSCommand = new MacroStepSetNextCommandWaitMS();
                        ret.Add(oWaitMSCommand);
                        step = oWaitMSCommand;
                        break;
                    default:
                        isValid = false;
                        errorMessages.Add("Invalid macro step type: " + errorSource + " " + stepType);
                        break;
                }
                string sWait = elemStep.GetAttribute("waitms");
                if (!string.IsNullOrEmpty(sWait))
                {
                    if (int.TryParse(sWait, out int iWaitMS))
                    {
                        if (step != null) step.WaitMS = iWaitMS;
                    }
                    else if (_variablesByName.TryGetValue(sWait, out Variable v))
                    {
                        if (v.Type != VariableType.Int)
                        {
                            isValid = false;
                            errorMessages.Add("WaitMS variable must be an integer: " + errorSource + " " + stepType);
                        }
                        else
                        {
                            if (step != null) step.WaitMSVariable = (IntegerVariable)v;
                        }
                    }
                    else
                    {
                        isValid = false;
                        errorMessages.Add("Invalid wait ms: " + errorSource + " " + stepType);
                    }
                }
                if (step != null && step is MacroStepSetNextCommandWaitMS && !step.WaitMS.HasValue)
                {
                    isValid = false;
                    errorMessages.Add("setnextcommandms step must have wait ms");
                }
                string sLoop = elemStep.GetAttribute("loop");
                if (!string.IsNullOrEmpty(sLoop))
                {
                    if (bool.TryParse(sLoop, out bool bLoop))
                    {
                        if (step != null) step.Loop = bLoop;
                    }
                    else if (int.TryParse(sLoop, out int iLoop))
                    {
                        if (iLoop < 0)
                        {
                            isValid = false;
                            errorMessages.Add("Invalid negative loop count: " + errorSource + " " + stepType);
                        }
                        else
                        {
                            if (step != null) step.LoopCount = iLoop;
                        }
                    }
                    else if (_variablesByName.TryGetValue(sLoop, out Variable v))
                    {
                        if (v.Type != VariableType.Int && v.Type != VariableType.Bool)
                        {
                            isValid = false;
                            errorMessages.Add("Loop variable must be an integer or boolean: " + errorSource + " " + stepType);
                        }
                        else
                        {
                            if (step != null) step.LoopVariable = v;
                        }
                    }
                    else
                    {
                        isValid = false;
                        errorMessages.Add("Invalid loop: " + errorSource + " " + stepType + " " + sLoop);
                    }
                }

                string sCondition = elemStep.GetAttribute("condition");
                if (!string.IsNullOrEmpty(sCondition))
                {
                    if (_variablesByName.TryGetValue(sCondition, out Variable v))
                    {
                        if (v.Type != VariableType.String)
                        {
                            isValid = false;
                            errorMessages.Add("Condition variable must be a string: " + errorSource + " " + stepType);
                        }
                        else
                        {
                            if (step != null) step.ConditionVariable = v;
                        }
                    }
                }

                string sSkipRounds = elemStep.GetAttribute("skiprounds");
                if (!string.IsNullOrEmpty(sSkipRounds))
                {
                    if (int.TryParse(sSkipRounds, out int iSkipRounds))
                    {
                        if (step != null) step.SkipRounds = iSkipRounds;
                    }
                    else
                    {
                        isValid = false;
                        errorMessages.Add("Invalid skip rounds: " + errorSource + " " + stepType + " " + sSkipRounds);
                    }
                }
            }
            return isValid ? ret : null;
        }

        private void btnOneClick_Click(object sender, EventArgs e)
        {
            RunMacro((Macro)((Button)sender).Tag);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentBackgroundParameters.FinalCommand))
            {
                if (SendCommand(_currentBackgroundParameters.FinalCommand, false))
                {
                    _currentBackgroundParameters.CommandsRun++;
                }
            }
            if ((_currentBackgroundParameters.SetTargetRoomIfCancelled || !_currentBackgroundParameters.Cancelled) && _currentBackgroundParameters.CommandsRun > 0)
            {
                Room targetRoom = _currentBackgroundParameters.TargetRoom;
                if (targetRoom != null)
                {
                    SetCurrentRoom(targetRoom);
                }
            }
            ToggleBackgroundProcess(false);
            _currentBackgroundParameters = null;
        }

        private void SetCurrentRoom(Room r)
        {
            if (!string.IsNullOrEmpty(r.Mob))
            {
                txtMob.Text = r.Mob;
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
                RunQueuedCommand();

                //wait for an appropriate amount of time for commands after the first
                if (oPreviousCommand != null)
                {
                    int remainingMS = overrideWaitMS.GetValueOrDefault(pms.WaitMS);
                    while (remainingMS > 0)
                    {
                        int nextWaitMS = Math.Min(remainingMS, 100);
                        if (_bw.CancellationPending) break;
                        Thread.Sleep(nextWaitMS);
                        remainingMS -= nextWaitMS;
                        RunQueuedCommand();
                    }
                }

                if (_bw.CancellationPending) break;
                string sCommand = nextCommand.Command;
                if (SendCommand(sCommand, false))
                {
                    pms.CommandsRun++;
                }
                else
                {
                    break;
                }
                RunQueuedCommand();
            }
        }

        private void RunQueuedCommand()
        {
            lock (_queuedCommandLock)
            {
                if (_currentBackgroundParameters.QueuedCommand != null)
                {
                    if (SendCommand(_currentBackgroundParameters.QueuedCommand, false))
                    {
                        _currentBackgroundParameters.CommandsRun++;
                    }
                    _currentBackgroundParameters.QueuedCommand = null;
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
                        MacroCommand nextCommand = (MacroCommand)nextStep;
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
                    if (ctl == btnAbort)
                    {
                        ctl.Enabled = running;
                    }
                    else if (ctl is Button)
                    {
                        if (ctl.Tag is CommandButtonTag)
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
                    else if (!(ctl is TextBox) && ctl != grpLocations)
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
            AddBreeCity(aBree, out Room oIxell, out Room oBreeTownSquare, out Room oBreeWestGateInside, out Room oSewerPipeExit);
            AddMayorMillwoodMansion(oIxell, out List<Room> mansionRooms, out Room oChancellorOfProtection, out Room oMayorMillwood);
            AddLocation(_aBreePerms, oChancellorOfProtection);
            AddLocation(_aBreePerms, oMayorMillwood);
            foreach (Room r in mansionRooms)
            {
                AddLocation(aBree, r);
            }

            AddBreeToHobbiton(oBreeWestGateInside);
            AddBreeToImladris(oSewerPipeExit);
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
            bardicGuildhall.Priority = PRIORITY_HEALING;
            AddBidirectionalExits(bardicGuildhall, nightingale3, BidirectionalExitType.WestEast);

            Room sabre3 = AddRoom("Tharbad Sabre");
            AddBidirectionalExits(sabre3, sabreNightingale, BidirectionalExitType.WestEast);

            Room illusion1 = AddRoom("Tharbad Illusion");
            AddBidirectionalExits(illusion1, sabreIllusion, BidirectionalExitType.NorthSouth);

            Room marketDistrictClothiers = AddRoom("Tharbad Market District Clothiers");
            AddBidirectionalExits(marketDistrictClothiers, illusion1, BidirectionalExitType.NorthSouth);

            Room oMasterJeweler = AddRoom("Master Jeweler 170 Rd");
            oMasterJeweler.Mob = "Jeweler";
            oMasterJeweler.Priority = PRIORITY_PERMS_BIG;
            AddBidirectionalExits(marketDistrictClothiers, oMasterJeweler, BidirectionalExitType.WestEast);
            SetVariablesForIndefiniteCasts(oMasterJeweler, true, 3);

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

            Room oKingBrundenThreshold = AddRoom("King Brunden's Wagon");
            AddExit(oGypsyRow4, oKingBrundenThreshold, "wagon");
            AddExit(oKingBrundenThreshold, oGypsyRow4, "out");

            Room oKingBrunden = AddRoom("King Brunden 300");
            oKingBrunden.Mob = oKingBrundenThreshold.Mob = "king";
            oKingBrunden.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oKingBrundenThreshold, oKingBrunden, "back");
            AddExit(oKingBrunden, oKingBrundenThreshold, "out");
            SetVariablesForIndefiniteCasts(oKingBrunden, true, 3);

            Room oGypsyBlademaster = AddRoom("Gypsy Blademaster 160 Bl");
            oGypsyBlademaster.Mob = "Blademaster";
            oGypsyBlademaster.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oGypsyRow3, oGypsyBlademaster, "tent");
            AddExit(oGypsyBlademaster, oGypsyRow3, "out");
            SetVariablesForIndefiniteCasts(oGypsyBlademaster, true, 3);

            Room oKingsMoneychanger = AddRoom("King's Moneychanger 150 Rd");
            oKingsMoneychanger.Mob = "Moneychanger";
            oKingsMoneychanger.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oGypsyRow2, oKingsMoneychanger, "tent");
            AddExit(oKingsMoneychanger, oGypsyRow2, "out");
            SetVariablesForIndefiniteCasts(oKingsMoneychanger, true, 3);

            Room oMadameNicolov = AddRoom("Madame Nicolov 180 Bl");
            oMadameNicolov.Mob = "Madame";
            oMadameNicolov.Priority = PRIORITY_PERMS_BIG;
            AddExit(oGypsyRow1, oMadameNicolov, "wagon");
            AddExit(oMadameNicolov, oGypsyRow1, "out");
            SetVariablesForIndefiniteCasts(oMadameNicolov, true, 3);

            Room gildedAppleZathrielThreshold = AddRoom("Zathriel Threshold");
            AddBidirectionalSameNameExit(sabre3, gildedAppleZathrielThreshold, "door", null);

            Room zathriel = AddRoom("Zathriel the Minstrel 220 Bl");
            zathriel.Mob = gildedAppleZathrielThreshold.Mob = "Minstrel";
            zathriel.Priority = PRIORITY_PERMS_BIG;
            AddExit(gildedAppleZathrielThreshold, zathriel, "stage");
            AddExit(zathriel, gildedAppleZathrielThreshold, "down");
            SetVariablesForIndefiniteCasts(zathriel, true, 3);

            Room oOliphauntsTattoos = AddRoom("Oliphaunt's Tattoos");
            AddBidirectionalExits(balle2, oOliphauntsTattoos, BidirectionalExitType.NorthSouth);
            
            Room oOliphant = AddRoom("Oliphaunt 310");
            oOliphant.Mob = oOliphauntsTattoos.Mob = "Oliphaunt";
            oOliphant.Priority = PRIORITY_PERMS_BIG;
            AddBidirectionalSameNameExit(oOliphauntsTattoos, oOliphant, "curtain", null);
            SetVariablesForIndefiniteCasts(oOliphant, true, 3);

            AddLocation(_aImladrisTharbadPerms, bardicGuildhall);
            AddLocation(_aImladrisTharbadPerms, zathriel);
            AddLocation(_aImladrisTharbadPerms, oOliphant);
            AddLocation(_aImladrisTharbadPerms, oMasterJeweler);
            AddLocation(_aImladrisTharbadPerms, oMadameNicolov);
            AddLocation(_aImladrisTharbadPerms, oKingsMoneychanger);
            AddLocation(_aImladrisTharbadPerms, oGypsyBlademaster);
            AddLocation(_aImladrisTharbadPerms, oKingBrunden);
        }

        private void AddBreeCity(Area aBree, out Room oIxell, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSewerPipeExit)
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
            Room oBreeDocks = breeStreets[9, 0] = AddRoom("Bree Docks"); //10x1
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
            Room oToBarracks = breeStreets[7, 2] = AddRoom("Sergeant Grimdall Threshold"); //Bree Main 8x3
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
            breeSewers[7, 3] = AddRoom("Bree Sewers Periwinkle/Main 8x4");
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
            Room oToBigPapa = breeStreets[7, 5] = AddRoom("Big Papa Threshold"); //Main 8x6
            Room oBigPapa = breeStreets[8, 5] = AddRoom("Big Papa 350 Bl"); //9x6
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
            oOrderOfLove.Mob = "Doctor";
            oOrderOfLove.Priority = PRIORITY_HEALING;
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
            Room oToCasino = breeStreets[4, 10] = AddRoom("Guido Threshold"); //Bree Ormenel 5x11
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

            Room oValveChamber = AddRoom("Valve Chamber");
            AddExit(breeSewers[7, 3], oValveChamber, "valve");
            AddExit(oValveChamber, breeSewers[7, 3], "south");

            Room oSewerPassage = AddRoom("Sewer Passage");
            AddBidirectionalExits(oSewerPassage, oValveChamber, BidirectionalExitType.NorthSouth);

            Room oSewerDemonThreshold = AddRoom("Sewer Demon Threshold");
            oSewerDemonThreshold.Mob = "demon";
            oSewerDemonThreshold.Priority = PRIORITY_PERMS_LESS;
            AddBidirectionalExits(oSewerDemonThreshold, oSewerPassage, BidirectionalExitType.SoutheastNorthwest);

            Room oPoorAlley1 = AddRoom("Bree Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");

            Room oCampusFreeClinic = AddRoom("Bree Campus Free Clinic");
            oCampusFreeClinic.Mob = "Student";
            oCampusFreeClinic.Priority = PRIORITY_HEALING;
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");

            Room oBreeRealEstateOffice = AddRoom("Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);

            oIxell = AddRoom("Ixell 70 Bl");
            oIxell.Mob = "Ixell";
            AddExit(oBreeRealEstateOffice, oIxell, "door");
            AddExit(oIxell, oBreeRealEstateOffice, "out");
            SetVariablesForIndefiniteCasts(oIxell, false, 3);

            Room oKistaHillsHousing = AddRoom("Kista Hills Housing");
            AddBidirectionalExits(oStreetToFallon, oKistaHillsHousing, BidirectionalExitType.NorthSouth);

            Room oChurchsEnglishGardenFallonThreshold = AddRoom("Fallon Threshold");
            AddBidirectionalSameNameExit(oKistaHillsHousing, oChurchsEnglishGardenFallonThreshold, "gate", null);
            Room oFallon = AddRoom("Fallon 350");
            oFallon.Priority = PRIORITY_PERMS_BIG;
            AddExit(oChurchsEnglishGardenFallonThreshold, oFallon, "door");
            AddExit(oFallon, oChurchsEnglishGardenFallonThreshold, "out");
            SetVariablesForPermWithThreshold(oFallon, oChurchsEnglishGardenFallonThreshold, "door", null, 2);
            oFallon.Mob = oChurchsEnglishGardenFallonThreshold.Mob = "Fallon";

            Room oGrantsStables = AddRoom("Grant's stables");
            if (_level < 11)
            {
                AddExit(oToGrantsStables, oGrantsStables, "stable");
            }
            AddExit(oGrantsStables, oToGrantsStables, "south");

            Room oGrant = AddRoom("Grant 170");
            oGrant.Mob = "Grant";
            oGrant.Priority = PRIORITY_PERMS_MAIN;
            Exit oToGrant = AddExit(oGrantsStables, oGrant, "gate");
            oToGrant.PreCommand = "open gate";
            AddExit(oGrant, oGrantsStables, "out");
            SetVariablesForIndefiniteCasts(oGrant, false, 2);

            Room oPansy = AddRoom("Pansy Smallburrows 95 Rd");
            oPansy.Mob = "Pansy";
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);
            SetVariablesForIndefiniteCasts(oPansy, true, 3);

            Room oDroolie = AddRoom("Droolie 100 Rd");
            oDroolie.Mob = "Droolie";
            oDroolie.Priority = PRIORITY_PERMS_LESS;
            AddExit(oNorthBridge, oDroolie, "rope");
            AddExit(oDroolie, oNorthBridge, "up");
            SetVariablesForIndefiniteCasts(oDroolie, false, 2);

            Room oIgor = AddRoom("Igor 130");
            oIgor.Mob = "Igor";
            oIgor.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            SetVariablesForIndefiniteCasts(oIgor, true, 3);

            Room oSnarlingMutt = AddRoom("Snarling Mutt 50 Rd");
            oSnarlingMutt.Mob = "Mutt";
            AddExit(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            AddExit(oSnarlingMutt, oToSnarSlystoneShoppe, "out");
            SetVariablesForIndefiniteCasts(oSnarlingMutt, false, 3);

            Room oGuido = AddRoom("Guido 350 Rd");
            oGuido.Priority = PRIORITY_PERMS_BIG;
            oToCasino.Mob = oGuido.Mob = "Guido";
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");
            SetVariablesForPermWithThreshold(oGuido, oToCasino, "casino", null, 2);

            Room oSergeantGrimdall = AddRoom("Sergeant Grimdall 350 Bl");
            oSergeantGrimdall.Priority = PRIORITY_PERMS_BIG;
            oToBarracks.Mob = oSergeantGrimdall.Mob = "Sergeant";
            AddExit(oToBarracks, oSergeantGrimdall, "barracks");
            AddExit(oSergeantGrimdall, oToBarracks, "east");
            SetVariablesForPermWithThreshold(oSergeantGrimdall, oToBarracks, "barracks", null, 2);

            oToBigPapa.Mob = oBigPapa.Mob = "papa";
            oBigPapa.Priority = PRIORITY_PERMS_BIG;
            SetVariablesForPermWithThreshold(oBigPapa, oToBigPapa, "east", null, 2);

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

            Room oScranlin = AddRoom("Scranlin 500");
            oScranlin.Priority = PRIORITY_PERMS_BIG;
            oScranlin.Mob = oScranlinThreshold.Mob = "Scranlin";
            AddExit(oScranlinThreshold, oScranlin, "outhouse");
            AddExit(oScranlin, oScranlinThreshold, "out");
            SetVariablesForPermWithThreshold(oScranlin, oScranlinThreshold, "outhouse", null, 2);

            AddLocation(_aBreePerms, oOrderOfLove);
            AddLocation(_aBreePerms, oCampusFreeClinic);
            AddLocation(_aInaccessible, oGrant);
            AddLocation(_aBreePerms, oIgor);
            AddLocation(_aBreePerms, oGuido);
            AddSubLocation(oGuido, oToCasino);
            AddLocation(_aBreePerms, oFallon);
            AddSubLocation(oFallon, oChurchsEnglishGardenFallonThreshold);
            AddLocation(_aBreePerms, oSergeantGrimdall);
            AddSubLocation(oSergeantGrimdall, oToBarracks);
            AddLocation(_aBreePerms, oBigPapa);
            AddSubLocation(oBigPapa, oToBigPapa);
            AddLocation(_aBreePerms, oScranlin);
            AddSubLocation(oScranlin, oScranlinThreshold);
            AddLocation(_aBreePerms, oDroolie);
            AddLocation(_aBreePerms, oSewerDemonThreshold);
            AddLocation(aBree, oPansy);
            AddLocation(aBree, oIxell);
            AddLocation(aBree, oSnarlingMutt);
            AddLocation(aBree, oBreeDocks);
            AddLocation(aBree, oBreeTownSquare);
            AddLocation(_aMisc, oBreePawnShopWest);
            AddLocation(_aMisc, oBreePawnShopEast);
            AddLocation(_aMisc, oLeonardosSwords);
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

        private void SetVariablesForIndefiniteCasts(Room perm, bool includeStun, int level)
        {
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNDIRECTION, string.Empty);
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNPRECOMMAND, string.Empty);
            string sIndefiniteNumber = "100";
            AddRoomVariableValue(perm, VARIABLE_LEVEL1CASTROUNDS, level == 1 ? sIndefiniteNumber : "0");
            AddRoomVariableValue(perm, VARIABLE_LEVEL2CASTROUNDS, level == 2 ? sIndefiniteNumber : "0");
            AddRoomVariableValue(perm, VARIABLE_LEVEL3CASTROUNDS, level == 3 ? sIndefiniteNumber : "0");
            AddRoomVariableValue(perm, VARIABLE_STUNCASTROUNDS, includeStun ? "1" : "0");
        }

        private void SetVariablesForPermWithThreshold(Room perm, Room threshold, string hitAndRunDirection, string hitAndRunPrecommand, int numRounds)
        {
            string sCastsPerStun, sStunRounds;
            if (numRounds == 1)
            {
                sCastsPerStun = "1";
                sStunRounds = "3";
            }
            else if (numRounds == 2)
            {
                sCastsPerStun = "2";
                sStunRounds = "2";
            }
            else
            {
                throw new InvalidOperationException();
            }
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNDIRECTION, string.Empty);
            AddRoomVariableValue(perm, VARIABLE_HITANDRUNPRECOMMAND, string.Empty);
            AddRoomVariableValue(perm, VARIABLE_LEVEL1CASTROUNDS, "0");
            AddRoomVariableValue(perm, VARIABLE_LEVEL2CASTROUNDS, "0");
            AddRoomVariableValue(perm, VARIABLE_LEVEL3CASTROUNDS, sCastsPerStun);
            AddRoomVariableValue(perm, VARIABLE_STUNCASTROUNDS, sStunRounds);
            AddRoomVariableValue(threshold, VARIABLE_HITANDRUNDIRECTION, hitAndRunDirection);
            AddRoomVariableValue(threshold, VARIABLE_HITANDRUNPRECOMMAND, hitAndRunPrecommand);
            AddRoomVariableValue(threshold, VARIABLE_LEVEL1CASTROUNDS, "0");
            AddRoomVariableValue(threshold, VARIABLE_LEVEL2CASTROUNDS, "0");
            AddRoomVariableValue(threshold, VARIABLE_LEVEL3CASTROUNDS, sCastsPerStun);
            AddRoomVariableValue(threshold, VARIABLE_STUNCASTROUNDS, sStunRounds);
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

            Room oPathToMansion4WarriorBardsx2 = AddRoom("Warrior Bards (Path) 100 Rd");
            oPathToMansion4WarriorBardsx2.Mob = sWarriorBard;
            AddExit(oPathToMansion3, oPathToMansion4WarriorBardsx2, "stone");
            AddExit(oPathToMansion4WarriorBardsx2, oPathToMansion3, "north");
            SetVariablesForIndefiniteCasts(oPathToMansion4WarriorBardsx2, false, 3);

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

            Room oGrandPorch = AddRoom("Warrior Bard (Porch) 100 Rd");
            oGrandPorch.Mob = sWarriorBard;
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");
            SetVariablesForIndefiniteCasts(oGrandPorch, false, 3);

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

            Room oWarriorBardMansionNorth = AddRoom("Warrior Bard Mansion N 100 Rd");
            oWarriorBardMansionNorth.Mob = sWarriorBard;
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);
            SetVariablesForIndefiniteCasts(oWarriorBardMansionNorth, false, 3);

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

            Room oWarriorBardMansionSouth = AddRoom("Warrior Bard Mansion S 100 Rd");
            oWarriorBardMansionSouth.Mob = sWarriorBard;
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);
            SetVariablesForIndefiniteCasts(oWarriorBardMansionSouth, false, 3);

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

            Room oWarriorBardMansionEast = AddRoom("Warrior Bard Mansion E 100 Rd");
            oWarriorBardMansionEast.Mob = sWarriorBard;
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);
            SetVariablesForIndefiniteCasts(oWarriorBardMansionEast, false, 3);

            Room oGrandStaircaseUpstairs = AddRoom("Grand Staircase");
            AddBidirectionalExits(oGrandStaircaseUpstairs, oWarriorBardMansionEast, BidirectionalExitType.UpDown);

            Room oRoyalHallwayUpstairs = AddRoom("Royal Hallway");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oGrandStaircaseUpstairs, BidirectionalExitType.WestEast);

            Room oMayorThreshold = AddRoom("Mayor Threshold");
            AddBidirectionalExits(oRoyalHallwayUpstairs, oMayorThreshold, BidirectionalExitType.NorthSouth);

            Room oChancellorThreshold = AddRoom("Chancellor Threshold");
            AddBidirectionalExits(oChancellorThreshold, oRoyalHallwayUpstairs, BidirectionalExitType.NorthSouth);

            //mayor is immune to stun
            oMayorMillwood = AddRoom("Mayor Millwood 220 Gy");
            oMayorMillwood.Priority = PRIORITY_PERMS_MAIN;
            Exit e = AddExit(oMayorThreshold, oMayorMillwood, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oMayorMillwood, oMayorThreshold, "out");
            oMayorMillwood.Mob = oMayorThreshold.Mob = "mayor";
            SetVariablesForIndefiniteCasts(oMayorMillwood, false, 3);

            oChancellorOfProtection = AddRoom("Chancellor of Protection 200 Bl");
            oChancellorOfProtection.Priority = PRIORITY_PERMS_MAIN;
            e = AddExit(oChancellorThreshold, oChancellorOfProtection, "chamber");
            e.PreCommand = "open chamber";
            AddExit(oChancellorOfProtection, oChancellorThreshold, "out");
            oChancellorOfProtection.Mob = oChancellorThreshold.Mob = "chancellor";
            SetVariablesForIndefiniteCasts(oChancellorOfProtection, true, 3);

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

        private void AddBreeToImladris(Room oSewerPipeExit)
        {
            Area oBreeToImladris = _areasByName[AREA_BREE_TO_IMLADRIS];

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

            Room oGreatEastRoadGoblinAmbushGobLrgLrg = AddRoom("Gob Ambush Gob/Lrg/Lrg 145");
            oGreatEastRoadGoblinAmbushGobLrgLrg.Mob = "goblin";
            oGreatEastRoadGoblinAmbushGobLrgLrg.Priority = PRIORITY_PERMS_MAIN;
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

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog 150");
            oRoadToFarm7HoundDog.Priority = PRIORITY_PERMS_MAIN;
            oRoadToFarm7HoundDog.Mob = "dog";
            AddExit(oRoadToFarm7HoundDog, oRoadToFarm6, "out");
            AddExit(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");
            SetVariablesForIndefiniteCasts(oRoadToFarm7HoundDog, true, 2);

            Room oFarmParlorManagerMulloyThreshold = AddRoom("Manager Mulloy Threshold");
            oFarmParlorManagerMulloyThreshold.Mob = "manager";
            AddBidirectionalSameNameExit(oFarmParlorManagerMulloyThreshold, oRoadToFarm7HoundDog, "door", "open door");
            Room oManagerMulloy = AddRoom("Manager Mulloy Bl");
            oManagerMulloy.Mob = "manager";
            AddExit(oFarmParlorManagerMulloyThreshold, oManagerMulloy, "study");
            AddExit(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "out");
            SetVariablesForPermWithThreshold(oManagerMulloy, oFarmParlorManagerMulloyThreshold, "study", null, 1);

            Room oCrabbe = AddRoom("Crabbe 250");
            oCrabbe.Mob = "Crabbe";
            oCrabbe.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oHallway, oCrabbe, "detention");
            AddExit(oCrabbe, oHallway, "out");
            SetVariablesForIndefiniteCasts(oCrabbe, true, 3);

            Room oMrWartnose = AddRoom("Mr. Wartnose 235");
            oMrWartnose.Mob = "Wartnose";
            oMrWartnose.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oUglyKidClassroomK7, oMrWartnose, "office");
            AddExit(oMrWartnose, oUglyKidClassroomK7, "out");
            SetVariablesForIndefiniteCasts(oMrWartnose, true, 3);

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

            Room oSewerTunnel2 = AddRoom("Sewer Tunnel");
            AddBidirectionalExits(oSewerTunnel2, oSewerTConnection, BidirectionalExitType.WestEast);

            Room oSewerPipe = AddRoom("Sewer Pipe");
            AddExit(oSewerTunnel2, oSewerPipe, "pipe");
            AddExit(oSewerPipe, oSewerTunnel2, "down");
            AddExit(oSewerPipe, oSewerPipeExit, "up");

            Room oSalamander = AddRoom("Salamander 100 Rd");
            oSalamander.Mob = "Salamander";
            oSalamander.Priority = PRIORITY_PERMS_LESS;
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            SetVariablesForIndefiniteCasts(oSalamander, true, 2);

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

            Room oNorthBrethilForest4GobAmbushThreshold = AddRoom("Gob Ambush War/Lrg/Lrg Threshold");
            oNorthBrethilForest4GobAmbushThreshold.Mob = "goblin";
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest3, BidirectionalExitType.NorthSouth);

            Room oNorthBrethilForest5GobAmbush = AddRoom("Gob Ambush War/Lrg/Lrg 170");
            oNorthBrethilForest5GobAmbush.Mob = "goblin";
            oNorthBrethilForest5GobAmbush.Priority = PRIORITY_PERMS_MAIN;
            AddBidirectionalExits(oNorthBrethilForest4GobAmbushThreshold, oNorthBrethilForest5GobAmbush, BidirectionalExitType.WestEast);

            //South Brethil Forest
            Room oDeepForest = AddRoom("Deep Forest");
            AddBidirectionalExits(oGreatEastRoad9, oDeepForest, BidirectionalExitType.NorthSouth);

            Room oBrethilForest = AddRoom("Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);

            Room oSpriteGuards = AddRoom("Sprite Guards 120 Bl");
            oSpriteGuards.Mob = "guard";
            oSpriteGuards.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oBrethilForest, oSpriteGuards, "brush");
            AddExit(oSpriteGuards, oBrethilForest, "east");
            SetVariablesForIndefiniteCasts(oSpriteGuards, true, 3);

            AddLocation(_aBreePerms, oRoadToFarm7HoundDog);
            AddLocation(oBreeToImladris, oManagerMulloy);
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

            Room oRearAlley = AddRoom("Rear Alley");
            AddBidirectionalExits(oImladrisCircle10, oRearAlley, BidirectionalExitType.WestEast);
            AddBidirectionalExits(oRearAlley, oImladrisAlley5, BidirectionalExitType.WestEast);

            oImladrisSouthGateInside = AddRoom("Southern Gate of Imladris");
            AddBidirectionalExits(oImladrisCircle8, oImladrisSouthGateInside, BidirectionalExitType.NorthSouth);

            Room oImladrisCityDump = AddRoom("Imladris City Dump");
            Exit e = AddExit(oImladrisCircle8, oImladrisCityDump, "north");
            e.OmitGo = true;
            AddExit(oImladrisCityDump, oImladrisCircle8, "south");
            AddExit(oImladrisCityDump, oRearAlley, "north");

            Room oImladrisHealingHand = AddRoom("Imladris Healing Hand");
            oImladrisHealingHand.Priority = PRIORITY_HEALING;
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

            Room oIorlas = AddRoom("Iorlas 200");
            oIorlas.Mob = "Iorlas";
            oIorlas.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oMountainTrail2, oIorlas, "shack");
            AddExit(oIorlas, oMountainTrail2, "door");
            SetVariablesForIndefiniteCasts(oIorlas, true, 3);

            AddLocation(_aImladrisTharbadPerms, oImladrisHealingHand);
            AddLocation(_aImladrisTharbadPerms, oIorlas);
            AddLocation(_aMisc, oImladrisPawnShop);
            AddLocation(_aMisc, oTyriesPriestSupplies);
        }

        private void AddBreeToHobbiton(Room oBreeWestGateInside)
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

            Room oBilboBaggins = AddRoom("Bilbo Baggins 260 Bl");
            oBilboBaggins.Priority = PRIORITY_PERMS_MAIN;
            oBilboBaggins.Mob = "Bilbo";
            AddBidirectionalSameNameExit(oSouthwingHallway, oBilboBaggins, "oakdoor", "open oakdoor");
            SetVariablesForIndefiniteCasts(oBilboBaggins, true, 3);

            Room oFrodoBaggins = AddRoom("Frodo Baggins 260 Bl");
            oFrodoBaggins.Priority = PRIORITY_PERMS_MAIN;
            oFrodoBaggins.Mob = "Frodo";
            AddBidirectionalSameNameExit(oSouthwingHallway, oFrodoBaggins, "curtain", null);
            SetVariablesForIndefiniteCasts(oFrodoBaggins, true, 3);

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes");
            AddExit(oGreatHallOfHeroes, oLeviathanNorthForkWestern, "out");
            AddExit(oLeviathanNorthForkWestern, oGreatHallOfHeroes, "hall");

            //something is hasted
            Room oSomething = AddRoom("Something 140");
            oSomething.Mob = "Something";
            if (_level < 11)
            {
                AddExit(oGreatHallOfHeroes, oSomething, "curtain");
            }
            AddExit(oSomething, oGreatHallOfHeroes, "curtain");
            SetVariablesForIndefiniteCasts(oSomething, true, 3);
            
            Room oShepherd = AddRoom("Shepherd 60 Bl");
            oShepherd.Mob = "Shepherd";
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");
            SetVariablesForIndefiniteCasts(oShepherd, false, 3);

            AddLocation(_aInaccessible, oSomething);
            AddLocation(_aBreePerms, oBilboBaggins);
            AddLocation(_aBreePerms, oFrodoBaggins);
            AddLocation(oBreeToHobbiton, oShepherd);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside, out Room oTharbadGateOutside)
        {
            Area oImladrisToTharbad = _areasByName[AREA_IMLADRIS_TO_THARBAD];

            Room oMistyTrail1 = AddRoom("Misty Trail");
            AddBidirectionalSameNameExit(oImladrisSouthGateInside, oMistyTrail1, "gate", null);

            Room oMistyTrail2 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail1, oMistyTrail2, BidirectionalExitType.NorthSouth);

            Room oMistyTrail3 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail2, oMistyTrail3, BidirectionalExitType.NorthSouth);

            Room oMistyTrail4 = AddRoom("Misty Trail");
            AddBidirectionalExits(oMistyTrail3, oMistyTrail4, BidirectionalExitType.SouthwestNortheast);

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

            Room oShantyTown2GraddyThreshold = AddRoom("Graddy Threshold");
            AddBidirectionalExits(oShantyTown1, oShantyTown2GraddyThreshold, BidirectionalExitType.NorthSouth);

            Room oPrinceBrunden = AddRoom("Prince Brunden 150 Bl");
            oPrinceBrunden.Mob = "Prince";
            oPrinceBrunden.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oGypsyCamp, oPrinceBrunden, "wagon");
            AddExit(oPrinceBrunden, oGypsyCamp, "out");
            SetVariablesForIndefiniteCasts(oPrinceBrunden, true, 3);

            Room oNaugrim = AddRoom("Naugrim 205");
            oNaugrim.Mob = "Naugrim";
            oNaugrim.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oNorthShantyTown, oNaugrim, "cask");
            AddExit(oNaugrim, oNorthShantyTown, "out");
            SetVariablesForIndefiniteCasts(oNaugrim, true, 3);

            Room oHogoth = AddRoom("Hogoth 260");
            oHogoth.Mob = "Hogoth";
            oHogoth.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oShantyTownWest, oHogoth, "shack");
            AddExit(oHogoth, oShantyTownWest, "out");
            SetVariablesForIndefiniteCasts(oHogoth, true, 3);

            Room oFaornil = AddRoom("Faornil 250 Rd");
            oFaornil.Mob = "Faornil";
            oFaornil.Priority = PRIORITY_PERMS_MAIN;
            AddExit(oShantyTown1, oFaornil, "tent");
            AddExit(oFaornil, oShantyTown1, "out");
            SetVariablesForIndefiniteCasts(oFaornil, true, 3);

            Room oGraddy = AddRoom("Graddy 350");
            oGraddy.Mob = oShantyTown2GraddyThreshold.Mob = "Graddy";
            oGraddy.Priority = PRIORITY_PERMS_BIG;
            AddExit(oShantyTown2GraddyThreshold, oGraddy, "wagon");
            AddExit(oGraddy, oShantyTown2GraddyThreshold, "out");
            SetVariablesForPermWithThreshold(oGraddy, oShantyTown2GraddyThreshold, "wagon", null, 2);

            AddLocation(_aImladrisTharbadPerms, oPrinceBrunden);
            AddLocation(_aImladrisTharbadPerms, oNaugrim);
            AddLocation(_aImladrisTharbadPerms, oHogoth);
            AddLocation(_aImladrisTharbadPerms, oFaornil);
            AddLocation(_aImladrisTharbadPerms, oGraddy);
            AddSubLocation(oGraddy, oShantyTown2GraddyThreshold);
        }

        private void AddIntangible(Room oBreeTownSquare)
        {
            Area oIntangible = _areasByName[AREA_INTANGIBLE];

            Room oTreeOfLife = AddRoom("Tree of Life");
            AddExit(oTreeOfLife, oBreeTownSquare, "down");

            Room oLimbo = AddRoom("Limbo");
            Exit e = AddExit(oLimbo, oTreeOfLife, "green");
            e.PreCommand = "open green";

            AddLocation(oIntangible, oTreeOfLife);
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
                return Name;
            }

            public Room(string name)
            {
                this.Name = name;
            }
            public string Name { get; set; }
            public string Mob { get; set; }
            public Dictionary<Variable, string> VariableValues { get; set; }
            public List<Room> SubLocations { get; set; }
            public Room ParentRoom { get; set; }
            public int Priority { get; set; }
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

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        private bool SendCommand(string command, bool showUI)
        {
            bool ret = true;
            IntPtr WindowToFind = FindWindow(null, txtWindow.Text);

            if (WindowToFind == IntPtr.Zero)
            {
                ret = false;
                if (showUI)
                {
                    MessageBox.Show("Failed to find window.");
                }
            }
            else
            {
                List<int> keys = new List<int>();
                foreach (char c in command)
                {
                    if (_keyMapping.TryGetValue(c, out int i))
                    {
                        keys.Add(i);
                    }
                    else
                    {
                        if (showUI)
                        {
                            MessageBox.Show("Failed to find key: " + c + ".");
                        }
                        ret = false;
                        break;
                    }
                }
                if (ret)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        int key = keys[i];
                        PostMessage((int)WindowToFind, WM_SYSKEYDOWN, key, 0);
                        Thread.Sleep(1);
                    }
                    PostMessage((int)WindowToFind, WM_SYSKEYDOWN, VK_RETURN, 0);
                }
            }
            return ret;
        }

        private enum ObjectType
        {
            Mob,
            Weapon,
            Wand,
            Potion,
            Realm1Spell,
            Realm2Spell,
            Realm3Spell,
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

        private bool IsValidMacroName(string name)
        {
            foreach (ObjectType ot in Enum.GetValues(typeof(ObjectType)))
            {
                if (ot.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            if (bool.TryParse(name, out _))
            {
                return false;
            }
            if (int.TryParse(name, out _))
            {
                return false;
            }
            return true;
        }

        private string TranslateCommand(string input, out string errorMessage)
        {
            input = input ?? string.Empty;
            string specifiedValue;
            errorMessage = string.Empty;
            input = TranslateVariables(input);
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
            input = TranslateVariables(input);
            return input;
        }

        private string TranslateVariables(string input)
        {
            foreach (Variable v in _variables)
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


        private void btnDoAction_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string command;
            if (btn.Tag is CommandButtonTag)
            {
                command = ((CommandButtonTag)btn.Tag).Command;
            }
            else
            {
                command = btn.Tag.ToString();
            }
            command = TranslateCommand(command, out string errorMessage);
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
                    if (SendCommand(command, true))
                    {
                        if (btn == btnFlee && m_oCurrentRoom != null && m_oCurrentRoom.SubLocations != null && m_oCurrentRoom.SubLocations.Count == 1)
                        {
                            SetCurrentRoom(m_oCurrentRoom.SubLocations[0]);
                        }
                    }
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
                    commands.Add(new MacroCommand(exit.PreCommand));
                }
                string nextCommand = exit.ExitText;
                if (!exit.OmitGo) nextCommand = "go " + nextCommand;
                commands.Add(new MacroCommand(nextCommand));
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
            public bool SetTargetRoomIfCancelled { get; set; }
            public int CommandsRun { get; set; }
            public Macro Macro { get; set; }
            public string QueuedCommand { get; set; }
            public string FinalCommand { get; set; }
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

        private void btnClearOneOff_Click(object sender, EventArgs e)
        {
            txtOneOffCommand.Text = string.Empty;
            txtOneOffCommand.Focus();
        }

        private void btnOneOffExecute_Click(object sender, EventArgs e)
        {
            SendCommand(txtOneOffCommand.Text, true);
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
            SendCommand(command, true);
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
            string sFinalCommand = string.Empty;
            if (!string.IsNullOrEmpty(m.FinalCommand))
            {
                sFinalCommand = TranslateCommand(m.FinalCommand, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }
            }
            _currentBackgroundParameters = new BackgroundWorkerParameters();
            _currentBackgroundParameters.Macro = m;
            _currentBackgroundParameters.FinalCommand = sFinalCommand;
            if (m.SetParentLocation && m_oCurrentRoom != null && m_oCurrentRoom.ParentRoom != null)
            {
                _currentBackgroundParameters.TargetRoom = m_oCurrentRoom.ParentRoom;
                _currentBackgroundParameters.SetTargetRoomIfCancelled = true;
            }
            RunCommands(stepsToRun, _currentBackgroundParameters);
        }

        private MacroStepBase TranslateStep(MacroStepBase input, out string errorMessage)
        {
            MacroStepBase ret = null;
            errorMessage = string.Empty;
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
                string translatedCommand = TranslateCommand(rawCommand, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage)) return null;
                ret = new MacroCommand(translatedCommand);
            }
            else if (input is MacroStepSetNextCommandWaitMS)
            {
                ret = input;
            }
            ret.WaitMS = input.WaitMS;
            ret.WaitMSVariable = input.WaitMSVariable;
            ret.Loop = input.Loop;
            ret.LoopCount = input.LoopCount;
            ret.LoopVariable = input.LoopVariable;
            ret.SkipRounds = input.SkipRounds;
            ret.ConditionVariable = input.ConditionVariable;
            return ret;
        }

        private class Macro
        {
            public Macro(string Name)
            {
                this.Name = Name;
                this.Steps = new List<MacroStepBase>();
            }
            public override string ToString()
            {
                return this.Name;
            }

            public string Name { get; set; }
            public List<MacroStepBase> Steps { get; set; }
            public bool SetParentLocation { get; set; }
            public CommandType CombatCommandTypes { get; set; }
            public string FinalCommand { get; set; }
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
        }

        private class MacroStepSequence : MacroStepBase
        {
            public List<MacroStepBase> SubCommands { get; set; }
            public MacroStepSequence()
            {
                this.SubCommands = new List<MacroStepBase>();
            }
        }

        private class MacroCommand : MacroStepBase
        {
            public string Command { get; set; }
            public MacroCommand(string Command)
            {
                this.Command = Command;
            }
        }

        private class MacroStepSetNextCommandWaitMS : MacroStepBase
        {
            public MacroStepSetNextCommandWaitMS()
            {
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
    }
}
