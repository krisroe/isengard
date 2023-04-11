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

        private const string AREA_BREE = "Bree";
        private const string AREA_BREE_TO_HOBBITON = "Bree to Hobbiton";
        private const string AREA_BREE_TO_IMLADRIS = "Bree to Imladris";
        private const string AREA_IMLADRIS = "Imladris";
        private const string AREA_IMLADRIS_TO_THARBAD = "Imladris to Tharbad";
        private const string AREA_INTANGIBLE = "Intangible";

        private const string VARIABLE_MOVEGAPMS = "movegapms";
        private const string VARIABLE_LEVEL1CASTROUNDS = "level1castrounds";
        private const string VARIABLE_LEVEL2CASTROUNDS = "level2castrounds";

        public frmMain()
        {
            InitializeComponent();

            _areas = new List<Area>();
            _areasByName = new Dictionary<string, Area>();
            AddArea(AREA_BREE);
            AddArea(AREA_BREE_TO_HOBBITON);
            AddArea(AREA_BREE_TO_IMLADRIS);
            AddArea(AREA_IMLADRIS);
            AddArea(AREA_IMLADRIS_TO_THARBAD);
            AddArea(AREA_INTANGIBLE);

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

            LoadConfiguration();
            InitializeMap();
            SetNightEdges(false);
            PopulateTree();

            cboSetOption.SelectedIndex = 0;
        }

        private void AddArea(string areaName)
        {
            Area a = new Area(areaName);
            _areas.Add(a);
            _areasByName[a.Name] = a;
        }

        private void PopulateTree()
        {
            foreach (Area a in _areas)
            {
                TreeNode tArea = new TreeNode(a.Name);
                tArea.Tag = a;
                treeLocations.Nodes.Add(tArea);
                tArea.Expand();
                foreach (Room r in a.Locations)
                {
                    TreeNode tRoom = new TreeNode(r.Name);
                    tRoom.Tag = r;
                    tArea.Nodes.Add(tRoom);
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

                    bool macroIsValid = true;
                    Macro oMacro = new Macro(macroName);
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
            if (!e.Cancelled)
            {
                Room targetRoom = _currentBackgroundParameters.TargetRoom;
                if (targetRoom != null)
                {
                    if (!string.IsNullOrEmpty(targetRoom.Mob))
                    {
                        txtMob.Text = targetRoom.Mob;
                    }
                    m_oCurrentRoom = targetRoom;
                    txtCurrentRoom.Text = m_oCurrentRoom.Name;
                    if (targetRoom.VariableValues != null)
                    {
                        foreach (KeyValuePair<Variable, string> next in targetRoom.VariableValues)
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
            }
            ToggleBackgroundProcess(false);
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerParameters pms = (BackgroundWorkerParameters)e.Argument;
            List<MacroStepBase> commands = pms.Commands;
            MacroCommand oPreviousCommand;
            MacroCommand oCurrentCommand = null;
            foreach (MacroCommand nextCommand in IterateStepCommands(commands, pms, 0))
            {
                if (_bw.CancellationPending) break;
                oPreviousCommand = oCurrentCommand;
                oCurrentCommand = nextCommand;

                //wait for an appropriate amount of time for commands after the first
                if (oPreviousCommand != null)
                {
                    int remainingMS = pms.WaitMS;
                    while (remainingMS > 0)
                    {
                        int nextWaitMS = Math.Min(remainingMS, 100);
                        if (_bw.CancellationPending) break;
                        Thread.Sleep(nextWaitMS);
                        remainingMS -= nextWaitMS;
                    }
                }

                if (_bw.CancellationPending) break;
                string sCommand = nextCommand.Command;
                if (!SendCommand(sCommand, false))
                {
                    break;
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
        private IEnumerable<MacroCommand> IterateStepCommands(List<MacroStepBase> Steps, BackgroundWorkerParameters parameters, int loopsPerformed)
        {
            Dictionary<string, Variable> variables = parameters.Variables;
            foreach (MacroStepBase nextStep in Steps)
            {
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

                    if (nextStep is MacroStepSequence)
                    {
                        MacroStepSequence seq = (MacroStepSequence)nextStep;
                        bool pastFirst = false;
                        foreach (MacroCommand nextSubCommand in IterateStepCommands(seq.SubCommands, parameters, loopCount))
                        {
                            if (!pastFirst)
                            {
                                SetWaitMS(seq, parameters, variables);
                                pastFirst = true;
                            }
                            yield return nextSubCommand;
                        }
                    }
                    else
                    {
                        MacroCommand nextCommand = (MacroCommand)nextStep;
                        SetWaitMS(nextCommand, parameters, variables);
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
            foreach (Control ctl in this.Controls)
            {
                if (ctl != txtCurrentRoom) //skip always-disabled controls
                {
                    bool enable;
                    if (running)
                    {
                        enable = ctl == btnAbort;
                    }
                    else
                    {
                        enable = ctl != btnAbort;
                    }
                    ctl.Enabled = enable;
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
            AddMayorMillwoodMansion(oIxell, out Room oWarriorBardsOnPath, out Room oWarriorBardMansionGrandPorch, out Room oWarriorBardMansionNorth, out Room oWarriorBardMansionSouth, out Room oWarriorBardMansionEast);
            AddLocation(aBree, oWarriorBardsOnPath);
            AddLocation(aBree, oWarriorBardMansionGrandPorch);
            AddLocation(aBree, oWarriorBardMansionNorth);
            AddLocation(aBree, oWarriorBardMansionSouth);
            AddLocation(aBree, oWarriorBardMansionEast);

            AddBreeToHobbiton(oBreeWestGateInside);
            AddBreeToImladris(oSewerPipeExit);
            AddImladrisCity(out Room oImladrisSouthGateInside);
            AddImladrisToTharbad(oImladrisSouthGateInside);
            AddIntangible(oBreeTownSquare);
        }

        private void AddBreeCity(Area aBree, out Room oIxell, out Room oBreeTownSquare, out Room oWestGateInside, out Room oSewerPipeExit)
        {
            //Bree's road structure is a 15x11 grid
            Room[,] breeStreets = new Room[16, 11];
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
            breeStreets[7, 2] = AddRoom("Bree Main 8x3");
            breeStreets[10, 2] = AddRoom("Bree Crissaegrim 11x3");
            breeStreets[14, 2] = AddRoom("Bree Brownhaven 15x3");
            breeStreets[0, 3] = AddRoom("Bree Periwinkle/Wain 1x4");
            breeStreets[1, 3] = AddRoom("Bree Periwinkle 2x4");
            breeStreets[2, 3] = AddRoom("Bree Periwinkle 3x4");
            breeStreets[3, 3] = AddRoom("Bree Periwinkle/High 4x4");
            breeStreets[4, 3] = AddRoom("Bree Periwinkle 5x4");
            breeStreets[5, 3] = AddRoom("Bree Periwinkle 6x4");
            breeStreets[6, 3] = AddRoom("Bree Periwinkle 7x4");
            breeStreets[7, 3] = AddRoom("Bree Periwinkle/Main 8x4");
            breeStreets[8, 3] = AddRoom("Bree Periwinkle 9x4");
            breeStreets[9, 3] = AddRoom("Bree South Bridge 10x4");
            breeStreets[10, 3] = AddRoom("Bree Periwinkle/Crissaegrim 11x4");
            breeStreets[11, 3] = AddRoom("Bree Periwinkle 12x4");
            breeStreets[12, 3] = AddRoom("Bree Periwinkle/PoorAlley 13x4");
            breeStreets[13, 3] = AddRoom("Bree Periwinkle 14x4");
            breeStreets[14, 3] = AddRoom("Bree Periwinkle/Brownhaven 15x4");
            breeStreets[0, 4] = AddRoom("Bree Wain 1x5");
            Room oToBlindPigPubAndUniversity = breeStreets[3, 4] = AddRoom("Bree High 4x5");
            breeStreets[7, 4] = AddRoom("Bree Main 8x5");
            Room oToSnarSlystoneShoppe = breeStreets[10, 4] = AddRoom("Bree Crissaegrim 11x5");
            breeStreets[14, 4] = AddRoom("Bree Brownhaven 15x5");
            breeStreets[0, 5] = AddRoom("Bree Wain 1x6");
            breeStreets[3, 5] = AddRoom("Bree High 4x6");
            breeStreets[7, 5] = AddRoom("Bree Main 8x6");
            breeStreets[8, 5] = AddRoom("Bree Papa Joe's 9x6");
            breeStreets[10, 5] = AddRoom("Bree Crissaegrim 11x6");
            breeStreets[14, 5] = AddRoom("Bree Brownhaven 15x6");
            breeStreets[0, 6] = AddRoom("Bree Wain 1x7");
            breeStreets[3, 6] = AddRoom("Bree High 4x7");
            breeStreets[7, 6] = AddRoom("Bree Main 8x7");
            breeStreets[10, 6] = AddRoom("Bree Crissaegrim 11x7");
            breeStreets[14, 6] = AddRoom("Bree Brownhaven 15x7");
            oWestGateInside = breeStreets[0, 7] = AddRoom("Bree West Gate 1x8");
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
            breeStreets[3, 8] = AddRoom("Bree High 4x9");
            breeStreets[7, 8] = AddRoom("Bree Main 8x9");
            breeStreets[10, 8] = AddRoom("Bree Crissaegrim 11x9");
            breeStreets[14, 8] = AddRoom("Bree Brownhaven 15x9");
            Room oOrderOfLove = breeStreets[15, 8] = AddRoom("Bree Order of Love 16x9");
            oOrderOfLove.Mob = "Doctor";
            breeStreets[0, 9] = AddRoom("Bree Wain 1x10");
            breeStreets[3, 9] = AddRoom("Bree High 4x10");
            breeStreets[7, 9] = AddRoom("Bree Main 8x10");
            Room oToLeonardosFoundry = breeStreets[10, 9] = AddRoom("Bree Crissaegrim 11x10");
            Room oToGamblingPit = breeStreets[14, 9] = AddRoom("Bree Brownhaven 15x10");
            breeStreets[0, 10] = AddRoom("Bree Ormenel/Wain 1x11");
            breeStreets[1, 10] = AddRoom("Bree Ormenel 2x11");
            breeStreets[2, 10] = AddRoom("Bree Ormenel 3x11");
            breeStreets[3, 10] = AddRoom("Bree Ormenel/High 4x11");
            Room oToCasino = breeStreets[4, 10] = AddRoom("Bree Ormenel 5x11");
            breeStreets[5, 10] = AddRoom("Bree Ormenel 6x11");
            breeStreets[6, 10] = AddRoom("Bree Ormenel 7x11");
            breeStreets[7, 10] = AddRoom("Bree Ormenel/Main 8x11");
            breeStreets[10, 10] = AddRoom("Bree Ormenel 11x11");
            Room oToRealEstateOffice = breeStreets[11, 10] = AddRoom("Bree Ormenel 12x11");
            breeStreets[12, 10] = AddRoom("Bree Ormenel 13x11");
            breeStreets[13, 10] = AddRoom("Bree Ormenel 14x11");
            breeStreets[14, 10] = AddRoom("Bree Brownhaven/Ormenel 15x11");

            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 11; y++)
                {
                    Room r = breeStreets[x, y];
                    if (r != null)
                    {
                        //look for a square to the west and add the east/west exits
                        if (x > 0)
                        {
                            Room roomToWest = breeStreets[x - 1, y];
                            if (roomToWest != null)
                            {
                                AddBidirectionalExits(roomToWest, r, BidirectionalExitType.WestEast);
                            }
                        }
                        //look for a square to the south and add the north/south exits
                        if (y > 0)
                        {
                            Room roomToSouth = breeStreets[x, y - 1];
                            if (roomToSouth != null)
                            {
                                AddBidirectionalExits(r, roomToSouth, BidirectionalExitType.NorthSouth);
                            }
                        }
                    }
                }

            Room oPoorAlley1 = AddRoom("Bree Poor Alley");
            AddExit(oLeviathanPoorAlley, oPoorAlley1, "alley");
            AddExit(oPoorAlley1, oLeviathanPoorAlley, "north");

            Room oCampusFreeClinic = AddRoom("Bree Campus Free Clinic");
            oCampusFreeClinic.Mob = "Student";
            AddExit(oToCampusFreeClinic, oCampusFreeClinic, "clinic");
            AddExit(oCampusFreeClinic, oToCampusFreeClinic, "west");

            Room oBreeRealEstateOffice = AddRoom("Bree Real Estate Office");
            AddBidirectionalExits(oToRealEstateOffice, oBreeRealEstateOffice, BidirectionalExitType.NorthSouth);

            oIxell = AddRoom("Ixell");
            oIxell.Mob = "Ixell";
            AddExit(oBreeRealEstateOffice, oIxell, "door");
            AddExit(oIxell, oBreeRealEstateOffice, "out");
            AddRoomVariableValue(oIxell, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oIxell, VARIABLE_LEVEL1CASTROUNDS, "0");

            Room oGrantsStables = AddRoom("Grant's stables");
            AddExit(oToGrantsStables, oGrantsStables, "stable");
            AddExit(oGrantsStables, oToGrantsStables, "south");

            Room oGrant = AddRoom("Grant's office");
            oGrant.Mob = "Grant";
            Exit oToGrant = AddExit(oGrantsStables, oGrant, "gate");
            oToGrant.PreCommand = "open gate";
            AddExit(oGrant, oGrantsStables, "out");
            AddRoomVariableValue(oGrant, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oGrant, VARIABLE_LEVEL1CASTROUNDS, "1");

            Room oPansy = AddRoom("Pansy Smallburrows");
            oPansy.Mob = "Pansy";
            AddBidirectionalExits(oPansy, oToGamblingPit, BidirectionalExitType.WestEast);

            Room oDroolie = AddRoom("Droolie");
            oDroolie.Mob = "Droolie";
            AddExit(oNorthBridge, oDroolie, "rope");
            AddExit(oDroolie, oNorthBridge, "up");
            AddRoomVariableValue(oDroolie, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oDroolie, VARIABLE_LEVEL1CASTROUNDS, "1");

            Room oIgor = AddRoom("Igor");
            oIgor.Mob = "Igor";
            AddExit(oIgor, oToBlindPigPubAndUniversity, "east");
            AddExit(oToBlindPigPubAndUniversity, oIgor, "pub");
            AddRoomVariableValue(oIgor, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oIgor, VARIABLE_LEVEL1CASTROUNDS, "1");

            Room oSnarlingMutt = AddRoom("Snarling Mutt");
            oSnarlingMutt.Mob = "Mutt";
            AddExit(oToSnarSlystoneShoppe, oSnarlingMutt, "shoppe");
            AddExit(oSnarlingMutt, oToSnarSlystoneShoppe, "out");
            AddRoomVariableValue(oSnarlingMutt, VARIABLE_LEVEL2CASTROUNDS, "2");
            AddRoomVariableValue(oSnarlingMutt, VARIABLE_LEVEL1CASTROUNDS, "1");

            Room oGuido = AddRoom("Guido");
            oGuido.Mob = "Guido";
            AddExit(oToCasino, oGuido, "casino");
            AddExit(oGuido, oToCasino, "north");

            Room oBreePawnShopWest = AddRoom("Bree Pawn Shop West (Ixell's Antique Shop)");
            AddBidirectionalExits(oBreePawnShopWest, oToPawnShopWest, BidirectionalExitType.WestEast);

            Room oBreePawnShopEast = AddRoom("Bree Pawn Shop East");
            AddBidirectionalExits(oPoorAlley1, oBreePawnShopEast, BidirectionalExitType.WestEast);

            Room oLeonardosFoundry = AddRoom("Bree Leonardo's Foundry");
            AddExit(oToLeonardosFoundry, oLeonardosFoundry, "foundry");
            AddExit(oLeonardosFoundry, oToLeonardosFoundry, "east");

            Room oLeonardosSwords = AddRoom("Bree Leonardo's Swords");
            AddBidirectionalExits(oLeonardosSwords, oLeonardosFoundry, BidirectionalExitType.NorthSouth);

            AddLocation(aBree, oOrderOfLove);
            AddLocation(aBree, oCampusFreeClinic);
            AddLocation(aBree, oGrant);
            AddLocation(aBree, oIxell);
            AddLocation(aBree, oPansy);
            AddLocation(aBree, oDroolie);
            AddLocation(aBree, oIgor);
            AddLocation(aBree, oSnarlingMutt);
            AddLocation(aBree, oBreeDocks);
            AddLocation(aBree, oGuido);
            AddLocation(aBree, oBreePawnShopWest);
            AddLocation(aBree, oBreePawnShopEast);
            AddLocation(aBree, oLeonardosSwords);
        }

        /// <summary>
        /// adds rooms for mayor millwood's mansion
        /// </summary>
        /// <param name="oIxell">Ixell (entrance to mansion)</param>
        /// <param name="oPathToMansion4">Warrior bards on mansion path</param>
        /// <param name="oGrandPorch">Warrior bard on grand porch mansion entrance</param>
        /// <param name="oWarriorBardMansionNorth">warrior bard in north mansion stairwell</param>
        /// <param name="oWarriorBardMansionSouth">warrior bard in south mansion stairwell</param>
        /// <param name="oWarriorBardMansionEast">warrior bard in east mansion stairwell</param>
        private void AddMayorMillwoodMansion(Room oIxell, out Room oPathToMansion4, out Room oGrandPorch, out Room oWarriorBardMansionNorth, out Room oWarriorBardMansionSouth, out Room oWarriorBardMansionEast)
        {
            string sWarriorBard = "Warrior bard";

            Room oPathToMansion1 = AddRoom("Construction Site");
            AddExit(oIxell, oPathToMansion1, "back");
            AddExit(oPathToMansion1, oIxell, "hoist");

            Room oPathToMansion2 = AddRoom("Southern View");
            AddBidirectionalExits(oPathToMansion1, oPathToMansion2, BidirectionalExitType.NorthSouth);

            Room oPathToMansion3 = AddRoom("The South Wall");
            AddBidirectionalExits(oPathToMansion2, oPathToMansion3, BidirectionalExitType.NorthSouth);

            oPathToMansion4 = AddRoom("Warrior Bards (Path to Mansion)");
            oPathToMansion4.Mob = sWarriorBard;
            AddExit(oPathToMansion3, oPathToMansion4, "stone");
            AddExit(oPathToMansion4, oPathToMansion3, "north");
            AddRoomVariableValue(oPathToMansion4, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oPathToMansion4, VARIABLE_LEVEL1CASTROUNDS, "0");

            Room oPathToMansion5 = AddRoom("Stone Path");
            AddBidirectionalExits(oPathToMansion4, oPathToMansion5, BidirectionalExitType.SouthwestNortheast);

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

            oGrandPorch = AddRoom("Warrior Bard (Grand Porch)");
            oGrandPorch.Mob = sWarriorBard;
            AddExit(oPathToMansion12, oGrandPorch, "porch");
            AddExit(oGrandPorch, oPathToMansion12, "path");
            AddRoomVariableValue(oGrandPorch, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oGrandPorch, VARIABLE_LEVEL1CASTROUNDS, "0");

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

            oWarriorBardMansionNorth = AddRoom("Warrior Bard Mansion North");
            oWarriorBardMansionNorth.Mob = sWarriorBard;
            AddBidirectionalExits(oWarriorBardMansionNorth, oMansionFirstFloorToNorthStairwell5, BidirectionalExitType.NorthSouth);
            AddRoomVariableValue(oWarriorBardMansionNorth, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oWarriorBardMansionNorth, VARIABLE_LEVEL1CASTROUNDS, "0");

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

            oWarriorBardMansionSouth = AddRoom("Warrior Bard Mansion South");
            oWarriorBardMansionSouth.Mob = sWarriorBard;
            AddBidirectionalExits(oMansionFirstFloorToSouthStairwell5, oWarriorBardMansionSouth, BidirectionalExitType.NorthSouth);
            AddRoomVariableValue(oWarriorBardMansionSouth, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oWarriorBardMansionSouth, VARIABLE_LEVEL1CASTROUNDS, "0");

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

            oWarriorBardMansionEast = AddRoom("Warrior Bard Mansion East");
            oWarriorBardMansionEast.Mob = sWarriorBard;
            AddBidirectionalExits(oWarriorBardMansionEast, oMansionFirstFloorToEastStairwell6, BidirectionalExitType.WestEast);
            AddRoomVariableValue(oWarriorBardMansionEast, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oWarriorBardMansionEast, VARIABLE_LEVEL1CASTROUNDS, "0");
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

            Room oGreatEastRoadGoblinAmbush = AddRoom("Goblin Ambush");
            oGreatEastRoadGoblinAmbush.Mob = "goblin";
            AddBidirectionalExits(oGreatEastRoadGoblinAmbush, oGreatEastRoad6, BidirectionalExitType.SouthwestNortheast);

            Room oGreatEastRoad8 = AddRoom("Great East Road");
            AddBidirectionalExits(oGreatEastRoadGoblinAmbush, oGreatEastRoad8, BidirectionalExitType.SoutheastNorthwest);

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

            Room oRoadToFarm7HoundDog = AddRoom("Hound Dog");
            oRoadToFarm7HoundDog.Mob = "dog";
            AddExit(oRoadToFarm7HoundDog, oRoadToFarm6, "out");
            AddExit(oRoadToFarm6, oRoadToFarm7HoundDog, "porch");
            AddRoomVariableValue(oRoadToFarm7HoundDog, VARIABLE_LEVEL2CASTROUNDS, "4");
            AddRoomVariableValue(oRoadToFarm7HoundDog, VARIABLE_LEVEL1CASTROUNDS, "0");

            Room oOuthouse = AddRoom("Outhouse");
            AddBidirectionalExits(oRoadToFarm4, oOuthouse, BidirectionalExitType.WestEast);

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

            Room oSalamander = AddRoom("Salamander");
            oSalamander.Mob = "Salamander";
            AddExit(oBrandywineRiverShore, oSalamander, "reeds");
            AddExit(oSalamander, oBrandywineRiverShore, "shore");
            AddRoomVariableValue(oSalamander, VARIABLE_LEVEL2CASTROUNDS, "4");
            AddRoomVariableValue(oSalamander, VARIABLE_LEVEL1CASTROUNDS, "0");

            Room oDeepForest = AddRoom("Deep Forest");
            AddBidirectionalExits(oGreatEastRoad9, oDeepForest, BidirectionalExitType.NorthSouth);

            Room oBrethilForest = AddRoom("Brethil Forest");
            AddBidirectionalExits(oDeepForest, oBrethilForest, BidirectionalExitType.NorthSouth);

            Room oSpriteGuards = AddRoom("Sprite Guards");
            oSpriteGuards.Mob = "guard";
            AddExit(oBrethilForest, oSpriteGuards, "brush");
            AddExit(oSpriteGuards, oBrethilForest, "east");
            AddRoomVariableValue(oSpriteGuards, VARIABLE_LEVEL2CASTROUNDS, "4");
            AddRoomVariableValue(oSpriteGuards, VARIABLE_LEVEL1CASTROUNDS, "0");

            AddLocation(oBreeToImladris, oRoadToFarm7HoundDog);
            AddLocation(oBreeToImladris, oSalamander);
            AddLocation(oBreeToImladris, oGreatEastRoadGoblinAmbush);
            AddLocation(oBreeToImladris, oSpriteGuards);
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
            AddBidirectionalExits(oImladrisHealingHand, oImladrisMainStreet5, BidirectionalExitType.NorthSouth);

            Room oTyriesPriestSupplies = AddRoom("Tyrie's Priest Supplies");
            AddBidirectionalExits(oImladrisMainStreet5, oTyriesPriestSupplies, BidirectionalExitType.NorthSouth);

            AddLocation(oImladris, oImladrisHealingHand);
            AddLocation(oImladris, oTyriesPriestSupplies);
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

            Room oGreatHallOfHeroes = AddRoom("Great Hall of Heroes");
            AddExit(oGreatHallOfHeroes, oLeviathanNorthForkWestern, "out");
            AddExit(oLeviathanNorthForkWestern, oGreatHallOfHeroes, "hall");

            Room oSomething = AddRoom("Something");
            oSomething.Mob = "Something";
            AddBidirectionalSameNameExit(oGreatHallOfHeroes, oSomething, "curtain", null);
            AddRoomVariableValue(oSomething, VARIABLE_LEVEL2CASTROUNDS, "4");
            AddRoomVariableValue(oSomething, VARIABLE_LEVEL1CASTROUNDS, "0");
            
            Room oShepherd = AddRoom("Shepherd");
            oShepherd.Mob = "Shepherd";
            AddExit(oNorthFork1, oShepherd, "pasture");
            AddExit(oShepherd, oNorthFork1, "south");
            AddRoomVariableValue(oShepherd, VARIABLE_LEVEL2CASTROUNDS, "3");
            AddRoomVariableValue(oShepherd, VARIABLE_LEVEL1CASTROUNDS, "0");

            AddLocation(oBreeToHobbiton, oSomething);
            AddLocation(oBreeToHobbiton, oShepherd);
        }

        private void AddImladrisToTharbad(Room oImladrisSouthGateInside)
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

            //Shanty Town
            Room oRuttedDirtRoad = AddRoom("Rutted Dirt Road");
            AddBidirectionalExits(oRuttedDirtRoad, oMistyTrail8, BidirectionalExitType.WestEast);

            Room oNorthEdgeOfShantyTown = AddRoom("North Edge of Shanty Town");
            AddBidirectionalExits(oRuttedDirtRoad, oNorthEdgeOfShantyTown, BidirectionalExitType.NorthSouth);

            Room oRedFoxLane = AddRoom("Red Fox Lane");
            AddBidirectionalExits(oRedFoxLane, oNorthEdgeOfShantyTown, BidirectionalExitType.WestEast);

            Room oGypsyCamp = AddRoom("Gypsy Camp");
            AddBidirectionalExits(oGypsyCamp, oRedFoxLane, BidirectionalExitType.SoutheastNorthwest);

            Room oPrinceBrunden = AddRoom("Prince Brunden");
            oPrinceBrunden.Mob = "Prince";
            AddExit(oGypsyCamp, oPrinceBrunden, "wagon");
            AddExit(oPrinceBrunden, oGypsyCamp, "out");
            AddRoomVariableValue(oPrinceBrunden, VARIABLE_LEVEL2CASTROUNDS, "4");
            AddRoomVariableValue(oPrinceBrunden, VARIABLE_LEVEL1CASTROUNDS, "0");

            AddLocation(oImladrisToTharbad, oPrinceBrunden);
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
            string command = TranslateCommand(btn.Tag.ToString(), out string errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                SendCommand(command, true);
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
                    m_oCurrentRoom = r;
                    if (!string.IsNullOrEmpty(m_oCurrentRoom.Mob))
                    {
                        txtMob.Text = m_oCurrentRoom.Mob;
                    }
                    txtCurrentRoom.Text = m_oCurrentRoom.Name;
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
            public int WaitMS { get; set; }
        }

        private void Alg_TreeEdge(Exit e)
        {
            _pathMapping[e.Target] = e;
            if (e.Target == _currentBackgroundParameters.TargetRoom)
            {
                _currentSearch.Abort();
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            SendCommand(string.Empty, true);
        }

        private void chkIsNight_CheckedChanged(object sender, EventArgs e)
        {
            SetNightEdges(chkIsNight.Checked);
        }

        private void txtOneOffCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
          if (e.KeyChar == (char)Keys.Return)
            {
                SendCommand(txtOneOffCommand.Text, true);
            }
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
            foreach (MacroStepBase nextMacroStep in m.Steps)
            {
                string errorMessage;
                stepsToRun.Add(TranslateStep(nextMacroStep, out errorMessage));
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    MessageBox.Show(errorMessage);
                    return;
                }
            }
            _currentBackgroundParameters = new BackgroundWorkerParameters();
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
            ret.WaitMS = input.WaitMS;
            ret.WaitMSVariable = input.WaitMSVariable;
            ret.Loop = input.Loop;
            ret.LoopCount = input.LoopCount;
            ret.LoopVariable = input.LoopVariable;
            ret.SkipRounds = input.SkipRounds;
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
        }

        public class MacroStepBase
        {
            public int? WaitMS { get; set; }
            public IntegerVariable WaitMSVariable { get; set; }
            public bool? Loop { get; set; }
            public int? LoopCount { get; set; }
            public Variable LoopVariable { get; set; }
            /// <summary>
            /// number of times through a loop to skip
            /// </summary>
            public int SkipRounds { get; set; }
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

        private void cboMacros_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRunMacro.Enabled = cboMacros.SelectedIndex > 0;
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            new frmVariables(_variables).ShowDialog(this);
        }
    }
}
