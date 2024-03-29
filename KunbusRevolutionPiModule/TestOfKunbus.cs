﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Robot;
using KunbusRevolutionPiModule.Wrapper;
using Newtonsoft.Json;
using NLog;

namespace KunbusRevolutionPiModule
{
    public class KunbusIOModule
    {
        private readonly ProfinetIOConfig _config;
        private readonly Thread _samplerThread;        
        private Measurement MeasuredVariables { get; set; }
        private int NumberOfOutputs { get; set; }
        private int NumberOfBytes { get; set; }
        private bool DeviceActive { get; set; }
        private MongoSaver Saver { get; }
        private static readonly Logger _logger = LogManager.GetLogger("Kunbus Thread");

        public KunbusIOModule(int numberOfBytes, bool endian, string pathToConfiguration,
                            string databaseLocation, string database, string document)
        {
            NumberOfBytes = numberOfBytes;
            MeasuredVariables = JsonConvert.DeserializeObject<Measurement>(File.ReadAllText(pathToConfiguration));
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            NumberOfOutputs = MeasuredVariables.Variables.Count *
                              MeasuredVariables.Variables.First().Joints.Count * NumberOfBytes; // variables count * axis * byteField
            _config = new ProfinetIOConfig { Period = 4, BigEndian = endian };


            try { 
                KunbusRevolutionPiWrapper.piControlOpen();
                DeviceActive = true;
                _samplerThread = new Thread(GatherData);
                _samplerThread.Start();
            } catch(BadImageFormatException exception)
            {
                _logger.Error("It seems like the application is not running on Kunbus Device... {0}", exception);
            }
            _logger.Trace("End of I/O read.");
        }

        ~KunbusIOModule()
        {
            if (DeviceActive)
            {
                KunbusRevolutionPiWrapper.piControlClose();
                _samplerThread.Interrupt();
            }
            else
            {
                _logger.Warn("Application is not runnig on Kunbus Device...");
            }
        }

        private void GatherData()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                if (!DeviceActive) continue;
                var time = GetDataRobotTime().ToDataTime();
                MeasuredVariables.Time = time;
                foreach (var variable in MeasuredVariables.Variables)
                {                    
                    GetOneVariable(variable);
                }
                var toSave = MeasuredVariables;
                Saver.SaveIOData(toSave);
            }
        }

        private void GetOneVariable(MeasurementVariable variable)
        {
            foreach (var joint in variable.Joints)
            {
                joint.Value = ReadKunbusData(joint);
            }
        }

        private RobotTime GetDataRobotTime()
        {
            var roboTime = new RobotTime();
            foreach(var commponent in roboTime.CurrentTime)
            {
                commponent.Value = ReadKunbusData(commponent);

            }
            return roboTime;
        }

        private void ReadOutput()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                var readData = new byte[NumberOfOutputs];
                uint start = 0;
                var readBytes = KunbusRevolutionPiWrapper.piControlRead(start, (uint)NumberOfOutputs, readData);

                if (_config.BigEndian ^ BitConverter.IsLittleEndian)
                {
                    Array.Reverse(readData);
                }

                if (readBytes == NumberOfOutputs)
                {
                    _logger.Info("Data has been read! All of them: {0}", NumberOfOutputs);
                }
                else
                {
                    _logger.Warn("Hups... Somethink went wrong! No data were read.");
                }
            }
        }

        private float ReadKunbusData(KunbusIOData kunbusIO)
        {
            var readData = new byte[kunbusIO._length];
            var readBytes = KunbusRevolutionPiWrapper.piControlRead(kunbusIO.BytOffset,
                                                                    kunbusIO._length, 
                                                                    readData);

            if (_config.BigEndian ^ BitConverter.IsLittleEndian)
            {
                Array.Reverse(readData);
            }

            if (readBytes == kunbusIO._length)
            {
                return BitConverter.ToSingle(readData, 0);
            }
            else
            {
                _logger.Warn("Hups... Somethink went wrong! No data were read.");
                throw new IOException();
            }
        }

        private bool DataChange()
        {

        }

    }

}