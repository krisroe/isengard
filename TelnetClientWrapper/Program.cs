using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static IsengardClient.frmMain;

using System.Xml;

namespace IsengardClient
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadConfiguration(out List<Variable> variables, out Dictionary<string, Variable> variablesByName, out string defaultRealm, out int level, out int totalhp, out int totalmp, out int healtickmp, out AlignmentType preferredAlignment, out string userName, out List<Macro> allMacros, out List<string> startupCommands, out string defaultWeapon, out int autoHazyThreshold, out bool autoHazyDefault);

            string password;
            using (frmLogin loginForm = new frmLogin(userName))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                userName = loginForm.UserName;
                password = loginForm.Password;
            }

            Application.Run(new frmMain(variables, variablesByName, defaultRealm, level, totalhp, totalmp, healtickmp, preferredAlignment, userName, password, allMacros, startupCommands, defaultWeapon, autoHazyThreshold, autoHazyDefault));
        }

        private static void LoadConfiguration(out List<Variable> variables, out Dictionary<string, Variable> variablesByName, out string defaultRealm, out int level, out int totalhp, out int totalmp, out int healtickmp, out AlignmentType preferredAlignment, out string userName, out List<Macro> allMacros, out List<string> startupCommands, out string defaultWeapon, out int autoHazyThreshold, out bool autoHazyDefault)
        {
            variables = new List<Variable>();
            variablesByName = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            defaultRealm = string.Empty;
            level = 0;
            totalhp = 0;
            totalmp = 0;
            healtickmp = 0;
            preferredAlignment = AlignmentType.Grey;
            userName = string.Empty;
            allMacros = new List<Macro>();
            startupCommands = new List<string>();
            defaultWeapon = string.Empty;
            autoHazyThreshold = 0;
            autoHazyDefault = false;

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

            XmlElement docElement = doc.DocumentElement;

            defaultRealm = docElement.GetAttribute("defaultrealm");
            if (!string.IsNullOrEmpty(defaultRealm))
            {
                defaultRealm = defaultRealm.ToLower();
                if (defaultRealm != "earth" &&
                    defaultRealm != "fire" &&
                    defaultRealm != "water" &&
                    defaultRealm != "wind")
                {
                    MessageBox.Show("Invalid default realm: " + defaultRealm);
                    defaultRealm = string.Empty;
                }
            }

            defaultWeapon = docElement.GetAttribute("defaultweapon");

            string sLevel = docElement.GetAttribute("level");
            if (string.IsNullOrEmpty(sLevel))
            {
                MessageBox.Show("No level specified");
                level = 1;
            }
            else if (!int.TryParse(sLevel, out level))
            {
                MessageBox.Show("Invalid level specified: " + sLevel);
                level = 1;
            }

            string sTotalHP = docElement.GetAttribute("totalhp");
            if (string.IsNullOrEmpty(sTotalHP))
            {
                MessageBox.Show("No total HP specified.");
                totalhp = 0;
            }
            else if (!int.TryParse(sTotalHP, out totalhp))
            {
                MessageBox.Show("Invalid total HP specified: " + sTotalHP);
                totalhp = 0;
            }

            string sTotalMP = docElement.GetAttribute("totalmp");
            if (string.IsNullOrEmpty(sTotalMP))
            {
                MessageBox.Show("No total MP specified.");
                totalmp = 0;
            }
            else if (!int.TryParse(sTotalMP, out totalmp))
            {
                MessageBox.Show("Invalid total MP specified: " + sTotalMP);
                totalmp = 0;
            }

            string sHealTickMP = docElement.GetAttribute("healtickmp");
            if (string.IsNullOrEmpty(sHealTickMP))
            {
                MessageBox.Show("No heal tick MP specified.");
                healtickmp = 0;
            }
            else if (!int.TryParse(sHealTickMP, out healtickmp))
            {
                MessageBox.Show("Invalid heal tick MP specified: " + sHealTickMP);
                healtickmp = 0;
            }

            string sAutoHazyThreshold = docElement.GetAttribute("autohazythreshold");
            if (!string.IsNullOrEmpty(sAutoHazyThreshold))
            {
                if (!int.TryParse(sAutoHazyThreshold, out autoHazyThreshold))
                {
                    MessageBox.Show("Invalid auto hazy threshold: " + sAutoHazyThreshold);
                    autoHazyThreshold = 0;
                }
            }

            string sAutoHazyDefault = docElement.GetAttribute("autohazydefault");
            if (!string.IsNullOrEmpty(sAutoHazyDefault))
            {
                if (!bool.TryParse(sAutoHazyDefault, out autoHazyDefault))
                {
                    MessageBox.Show("Invalid auto hazy default: " + sAutoHazyDefault);
                    autoHazyDefault = false;
                }
            }

            string sPreferredAlignment = docElement.GetAttribute("preferredalignment");
            if (Enum.TryParse(sPreferredAlignment, out AlignmentType ePreferredAlignment))
            {
                preferredAlignment = ePreferredAlignment;
            }
            else
            {
                MessageBox.Show("Invalid preferred alignment type");
                preferredAlignment = AlignmentType.Grey;
            }

            string sStartupCommands = docElement.GetAttribute("startupcommands");
            if (!string.IsNullOrEmpty(sStartupCommands))
            {
                startupCommands.AddRange(sStartupCommands.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }

            userName = docElement.GetAttribute("username");

            bool dupMacros = false;
            bool dupVariables = false;
            XmlElement macrosElement = null;
            XmlElement variablesElement = null;
            foreach (XmlNode nextNode in docElement.ChildNodes)
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
                    if (!Macro.IsValidMacroName(sName))
                    {
                        errorMessages.Add("Invalid variable name: " + sName);
                        continue;
                    }
                    if (variablesByName.ContainsKey(sName))
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
                    variables.Add(v);
                    variablesByName[sName] = v;
                }
            }

            if (!dupMacros)
            {
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

                    string sFlee = elemMacro.GetAttribute("flee");
                    bool bFlee = false;
                    if (!string.IsNullOrEmpty(sFlee))
                    {
                        if (!bool.TryParse(sFlee, out bFlee))
                        {
                            errorMessages.Add("Invalid flee for " + macroName + " " + sFlee);
                            continue;
                        }
                    }

                    string sFinalCommand = elemMacro.GetAttribute("finalcommand");
                    string sFinalCommand2 = elemMacro.GetAttribute("finalcommand2");

                    bool macroIsValid = true;
                    Macro oMacro = new Macro(macroName);
                    oMacro.CombatCommandTypes = eCombatCommandTypes;
                    oMacro.FinalCommand = sFinalCommand;
                    oMacro.FinalCommand2 = sFinalCommand2;
                    oMacro.Flee = bFlee;
                    List<MacroStepBase> foundSteps = ProcessStepsParentElement(elemMacro, macroName, errorMessages, variablesByName);
                    if (foundSteps == null)
                    {
                        macroIsValid = false;
                    }
                    oMacro.Steps = foundSteps;

                    string sFinalCommandCondition = elemMacro.GetAttribute("finalcommandcondition");
                    if (!string.IsNullOrEmpty(sFinalCommandCondition))
                    {
                        if (variablesByName.TryGetValue(sFinalCommandCondition, out Variable v))
                        {
                            if (v.Type != VariableType.String)
                            {
                                macroIsValid = false;
                                errorMessages.Add("Final command Condition variable must be a string: " + macroName);
                            }
                            else
                            {
                                oMacro.FinalCommandConditionVariable = v;
                            }
                        }
                    }

                    string sFinalCommand2Condition = elemMacro.GetAttribute("finalcommand2condition");
                    if (!string.IsNullOrEmpty(sFinalCommand2Condition))
                    {
                        if (variablesByName.TryGetValue(sFinalCommand2Condition, out Variable v))
                        {
                            if (v.Type != VariableType.String)
                            {
                                macroIsValid = false;
                                errorMessages.Add("Final command 2 Condition variable must be a string: " + macroName);
                            }
                            else
                            {
                                oMacro.FinalCommand2ConditionVariable = v;
                            }
                        }
                    }

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
                    oMacro.OneClick = isOneClick;

                    if (macroIsValid)
                    {
                        foundMacros[macroName] = oMacro;
                        allMacros.Add(oMacro);
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                MessageBox.Show("Errors loading configuration file" + Environment.NewLine + string.Join(Environment.NewLine, errorMessages));
            }
        }

        private static List<MacroStepBase> ProcessStepsParentElement(XmlElement parentElement, string errorSource, List<string> errorMessages, Dictionary<string, Variable> variablesByName)
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
            bool isValid = true;
            if (oStepsElem != null)
            {
                foreach (XmlNode nextStepNode in oStepsElem.ChildNodes)
                {
                    XmlElement elemStep = nextStepNode as XmlElement;
                    if (elemStep == null) continue;
                    string stepType = elemStep.Name.ToLower();
                    MacroStepBase step = null;
                    switch (stepType)
                    {
                        case "sequence":
                            List<MacroStepBase> loopSteps = ProcessStepsParentElement(elemStep, errorSource + " " + stepType, errorMessages, variablesByName);
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
                                MacroCommand oCommand = new MacroCommand(cmd, cmd);
                                ret.Add(oCommand);
                                step = oCommand;
                            }
                            break;
                        case "setnextcommandms":
                            MacroStepSetNextCommandWaitMS oWaitMSCommand = new MacroStepSetNextCommandWaitMS();
                            ret.Add(oWaitMSCommand);
                            step = oWaitMSCommand;
                            break;
                        case "setvariable":
                            MacroStepSetVariable oSetVariableCommand = new MacroStepSetVariable();
                            ret.Add(oSetVariableCommand);
                            step = oSetVariableCommand;
                            break;
                        case "manastun":
                            MacroManaSpellStun oStunCommand = new MacroManaSpellStun();
                            ret.Add(oStunCommand);
                            step = oStunCommand;
                            break;
                        case "manaoffensive":
                            MacroManaSpellOffensive oOffensiveCommand = new MacroManaSpellOffensive();
                            ret.Add(oOffensiveCommand);
                            step = oOffensiveCommand;
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
                        else if (variablesByName.TryGetValue(sWait, out Variable v))
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
                        else if (variablesByName.TryGetValue(sLoop, out Variable v))
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

                    string sVariable = elemStep.GetAttribute("variable");
                    if (!string.IsNullOrEmpty(sVariable))
                    {
                        if (step is MacroStepSetVariable)
                        {
                            if (!variablesByName.TryGetValue(sVariable, out Variable v))
                            {
                                isValid = false;
                                errorMessages.Add("Invalid variable: " + errorSource + " " + stepType + " " + sVariable);
                            }
                            else
                            {
                                string sValue = elemStep.GetAttribute("value");
                                int iTemp = 0;
                                bool bTemp = false;
                                if (sValue == null)
                                {
                                    errorMessages.Add("No variable value specified: " + errorSource + " " + stepType + " " + sVariable);
                                }
                                else if (v.Type == VariableType.Bool && !bool.TryParse(sValue, out bTemp))
                                {
                                    errorMessages.Add("Invalid boolean variable value specified: " + errorSource + " " + stepType + " " + sVariable);
                                }
                                else if (v.Type == VariableType.Int && !int.TryParse(sValue, out iTemp))
                                {
                                    errorMessages.Add("Invalid integer variable value specified: " + errorSource + " " + stepType + " " + sVariable);
                                }
                                else
                                {
                                    Variable copyVar = Variable.CopyVariable(v);
                                    ((MacroStepSetVariable)step).Variable = copyVar;
                                    switch (v.Type)
                                    {
                                        case VariableType.Bool:
                                            ((BooleanVariable)copyVar).Value = bTemp;
                                            break;
                                        case VariableType.Int:
                                            ((IntegerVariable)copyVar).Value = iTemp;
                                            break;
                                        case VariableType.String:
                                            ((StringVariable)copyVar).Value = sValue;
                                            break;
                                    }
                                    ((MacroStepSetVariable)step).Variable = copyVar;
                                }
                            }
                        }
                        else
                        {
                            isValid = false;
                            errorMessages.Add("Variable found but not for set variable step: " + errorSource + " " + stepType + " " + sVariable);
                        }
                    }

                    string sCondition = elemStep.GetAttribute("condition");
                    if (!string.IsNullOrEmpty(sCondition))
                    {
                        if (variablesByName.TryGetValue(sCondition, out Variable v))
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
            }
            return isValid ? ret : null;
        }
    }
}
