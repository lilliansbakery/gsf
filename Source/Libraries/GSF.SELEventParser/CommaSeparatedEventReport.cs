﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GSF.SELEventParser
{
    public class CommaSeparatedEventReport
    {
        #region [ Members ]

        // Fields
        private string m_command;
        private Header m_header;
        private Firmware m_firmware;
        private int m_eventNumber;
        private AnalogSection m_analogSection;
        private double m_averageFrequency;
        private double m_samplesPerCycleAnalog;
        private double m_samplesPerCycleDigital;
        private double m_numberOfCycles;
        private string m_event;
        private int m_triggerIndex;
        private int m_initialReadingIndex;
        private Dictionary<string, string> m_groupSettings;
        private Dictionary<string, string> m_controlEquations;
        private Dictionary<string, string> m_globalSettings;

        #endregion

        #region [Constructor]
        public CommaSeparatedEventReport()
        {
            m_groupSettings = new Dictionary<string, string>();
            m_controlEquations = new Dictionary<string, string>();
            m_globalSettings = new Dictionary<string, string>();
        }
        #endregion

        #region [ Properties ]

        public string Command
        {
            get
            {
                return m_command;
            }
            set
            {
                m_command = value;
            }
        }

        public Header Header
        {
            get
            {
                return m_header;
            }
            set
            {
                m_header = value;
            }
        }

        public Firmware Firmware
        {
            get
            {
                return m_firmware;
            }
            set
            {
                m_firmware = value;
            }
        }

        public int EventNumber
        {
            get
            {
                return m_eventNumber;
            }
            set
            {
                m_eventNumber = value;
            }
        }

        public AnalogSection AnalogSection
        {
            get
            {
                return m_analogSection;
            }
            set
            {
                m_analogSection = value;
            }
        }

        public double AverageFrequency
        {
            get
            {
                return m_averageFrequency;
            }
            set
            {
                m_averageFrequency = value;
            }
        }

        public double SamplesPerCycleAnalog
        {
            get
            {
                return m_samplesPerCycleAnalog;
            }
            set
            {
                m_samplesPerCycleAnalog = value;
            }
        }

        public double SamplesPerCycleDigital
        {
            get
            {
                return m_samplesPerCycleDigital;
            }
            set
            {
                m_samplesPerCycleDigital = value;
            }
        }

        public double NumberOfCycles
        {
            get
            {
                return m_numberOfCycles;
            }
            set
            {
                m_numberOfCycles = value;
            }
        }

        public string Event
        {
            get
            {
                return m_event;
            }
            set
            {
                m_event = value;
            }
        }

        public int TriggerIndex
        {
            get
            {
                return m_triggerIndex;
            }
            set
            {
                m_triggerIndex = value;
            }
        }

        public int InitialReadingIndex
        {
            get
            {
                return m_initialReadingIndex;
            }
            set
            {
                m_initialReadingIndex = value;
            }
        }

        public void SetGroupSetting(string key, string value)
        {
            if (m_groupSettings.ContainsKey(key))
            {
                m_groupSettings[key] = value;
            }
            else
            {
                m_groupSettings.Add(key, value);
            }
        }

        public string GetGroupSettings(string key)
        {
            string result = null;

            if (m_groupSettings.ContainsKey(key))
            {
                result = m_groupSettings[key];
            }

            return result;
        }

        public Dictionary<string, string> GroupSetting
        {
            set
            {
                m_groupSettings = value;
            }
        } 

        public void SetControlEquations(string key, string value)
        {
            if (m_controlEquations.ContainsKey(key))
            {
                m_controlEquations[key] = value;
            }
            else
            {
                m_controlEquations.Add(key, value);
            }
        }

        public string GetControlEquations(string key)
        {
            string result = null;

            if (m_controlEquations.ContainsKey(key))
            {
                result = m_controlEquations[key];
            }

            return result;
        }

        public Dictionary<string, string> ControlEquations
        {
            set
            {
                m_controlEquations = value;
            }
        }


        public void SetGlobalSetting(string key, string value)
        {
            if (m_globalSettings.ContainsKey(key))
            {
                m_globalSettings[key] = value;
            }
            else
            {
                m_globalSettings.Add(key, value);
            }
        }

        public string GetGlobalSettings(string key)
        {
            string result = null;

            if (m_globalSettings.ContainsKey(key))
            {
                result = m_globalSettings[key];
            }

            return result;
        }

        public Dictionary<string, string> GlobalSetting
        {
            set
            {
                m_globalSettings = value;
            }
        }


        #endregion

        #region [ Static ]

        // Static Methods

        public static CommaSeparatedEventReport Parse(string[] lines, ref int index)
        {
            return Parse(60.0D, lines, ref index);
        }

        public static CommaSeparatedEventReport Parse(double systemFrequency, string[] lines, ref int index)
        {
            CommaSeparatedEventReport commaSeparatedEventReport = new CommaSeparatedEventReport();
            commaSeparatedEventReport.Firmware = new Firmware();
            commaSeparatedEventReport.Header = new Header();

            int triggerIndex = 0;

            foreach (string line in lines)
            {
                if (line.Split(',').Length > 11 && line.Split(',')[11].Contains(">"))
                    commaSeparatedEventReport.TriggerIndex = triggerIndex;

                ++triggerIndex;    
            }

            while (!lines[index].ToUpper().Contains("SEL"))
                ++index;

            // Parse the report firmware id and checksum
            commaSeparatedEventReport.Firmware.ID = lines[index].Split(',')[0].Split('=')[1].Split('"')[0];
            //commaSeparatedEventReport.Firmware.Checksum = Convert.ToInt32(lines[index].Split(',')[1].Replace("\"", ""), 16);

            while (!lines[index].ToUpper().Contains("MONTH"))
                ++index;

            // Parse the date
            commaSeparatedEventReport.Header.EventTime = new DateTime(Convert.ToInt32(lines[++index].Split(',')[2]), Convert.ToInt32(lines[index].Split(',')[0]), Convert.ToInt32(lines[index].Split(',')[1]), Convert.ToInt32(lines[index].Split(',')[3]), Convert.ToInt32(lines[index].Split(',')[4]), Convert.ToInt32(lines[index].Split(',')[5]));

            // Set rest of header
            commaSeparatedEventReport.Header.SerialNumber = 0;
            commaSeparatedEventReport.Header.RelayID = "";
            commaSeparatedEventReport.Header.StationID = "";

            while (!lines[index].ToUpper().Contains("FREQ"))
                ++index;

            List<string> sampleStatHeader = lines[index].Split(',').ToList();
            List<string> sampleStats = lines[++index].Split(',').ToList();

            commaSeparatedEventReport.AverageFrequency = Convert.ToDouble(sampleStats[sampleStatHeader.FindIndex(x => x.ToUpper().Contains("FREQ"))]);
            commaSeparatedEventReport.SamplesPerCycleAnalog = Convert.ToDouble(sampleStats[sampleStatHeader.FindIndex(x => x.ToUpper().Contains("SAM/CYC_A"))]);
            commaSeparatedEventReport.SamplesPerCycleDigital = Convert.ToDouble(sampleStats[sampleStatHeader.FindIndex(x => x.ToUpper().Contains("SAM/CYC_D"))]);
            commaSeparatedEventReport.NumberOfCycles = Convert.ToDouble(sampleStats[sampleStatHeader.FindIndex(x => x.ToUpper().Contains("NUM_OF_CYC"))]);
            commaSeparatedEventReport.Event = sampleStats[sampleStatHeader.FindIndex(x => x.ToUpper().Contains("EVENT"))].Replace("/","");

            while (!lines[index].ToUpper().Contains("IA"))
                ++index;

            commaSeparatedEventReport.AnalogSection = new AnalogSection();

            string[] fields = lines[index].Split(',');

            int fieldIndex = 0;

            while(fields[fieldIndex].Trim('"').ToUpper() != "TRIG")
            {
                commaSeparatedEventReport.AnalogSection.AnalogChannels.Add(new Channel<double>());
                commaSeparatedEventReport.AnalogSection.AnalogChannels[commaSeparatedEventReport.AnalogSection.AnalogChannels.Count - 1].Name = fields[fieldIndex++].Split('(')[0].Trim('"');
            }

            foreach (string channel in fields[++fieldIndex].Split(' '))
            {
                if (channel != "\"")
                {
                    commaSeparatedEventReport.AnalogSection.DigitalChannels.Add(new Channel<bool?>());
                    commaSeparatedEventReport.AnalogSection.DigitalChannels[commaSeparatedEventReport.AnalogSection.DigitalChannels.Count - 1].Name = channel.Trim('"');
                }
            }

            commaSeparatedEventReport.InitialReadingIndex = ++index;
            int timeStepTicks = Convert.ToInt32(Math.Round(10000000.0 / systemFrequency / commaSeparatedEventReport.SamplesPerCycleAnalog));

            while (!lines[index].ToUpper().Contains("SETTINGS") && lines[index].ToUpper() != string.Empty)
            {
                int diff = commaSeparatedEventReport.TriggerIndex - index - commaSeparatedEventReport.InitialReadingIndex;
                commaSeparatedEventReport.AnalogSection.TimeChannel.Samples.Add(commaSeparatedEventReport.Header.EventTime.AddTicks(-1 * timeStepTicks * diff));

                int channelIndex = 0;
                foreach (var analogChannel in commaSeparatedEventReport.AnalogSection.AnalogChannels)
                {
                    analogChannel.Samples.Add(Convert.ToDouble(lines[index].Split(',')[channelIndex]) * (fields[channelIndex++].ToUpper().Contains("KV") ? 1000 : 1));
                }

                string digitals = lines[index].Split(',')[++channelIndex].Replace("\"", "");

                int forEachIndex = 0;
                foreach (Channel<bool?> channel in commaSeparatedEventReport.AnalogSection.DigitalChannels)
                {

                    if(digitals == "" || !Regex.IsMatch(digitals, @"\A\b[0-9a-fA-F]+\b\Z"))
                        channel.Samples.Add(null);
                    else
                        channel.Samples.Add(Convert.ToString(Convert.ToInt32(digitals[forEachIndex / 4].ToString(), 16), 2).PadLeft(4, '0')[forEachIndex % 4] == '1');
                    ++forEachIndex;
                }
                ++index;
            }

            try
            {
                //skip "SETTINGS" AND " LINES
                if (lines[index].ToUpper() == "SETTINGS")
                    ++index;
                if (lines[index] == "\"")
                    ++index;

                EventFile.SkipBlanks(lines, ref index);
                // SKIP GROUP 1 AND GROUP SETTINGS lINES
                if (lines[index].ToUpper() == "GROUP 1")
                    ++index;
                if (lines[index].ToUpper() == "GROUP SETTINGS:")
                    ++index;

                commaSeparatedEventReport.GroupSetting = Settings.Parse(lines, ref index);
                EventFile.SkipBlanks(lines, ref index);
                ++index;
                commaSeparatedEventReport.ControlEquations = ControlEquation.Parse(lines, ref index);
                EventFile.SkipBlanks(lines, ref index);
                commaSeparatedEventReport.GlobalSetting = Settings.Parse(lines, ref index);
            }
            catch(IndexOutOfRangeException)
            {
                if(index < lines.Length)
                {
                    throw;
                }
            }

            return commaSeparatedEventReport;
        }

        #endregion
    }
}
