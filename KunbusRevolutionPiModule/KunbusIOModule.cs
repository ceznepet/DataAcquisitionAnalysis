﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule.Conversion;
using KunbusRevolutionPiModule.Kunbus;
using KunbusRevolutionPiModule.Robot;
using KunbusRevolutionPiModule.Wrapper;
using Newtonsoft.Json;
using NLog;

namespace KunbusRevolutionPiModule
{
    public class KunbusIOModule
    {
        private static readonly Logger Logger = LogManager.GetLogger("Kunbus Thread");
        private readonly ProfinetIOConfig _config;
        private readonly KunbusIOData _changeCycle;
        private readonly Thread _samplerThread;
        private Thread SaveThread;
        private MongoVariables ToSaveMeasurement;
        private readonly uint ChangeDetectionStatus = 0;

        public KunbusIOModule(bool endian, string pathToConfiguration,
            string databaseLocation, string database, string document)
        {       
            MeasuredVariables = JsonConvert.DeserializeObject<Measurement>(File.ReadAllText(pathToConfiguration));
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            _config = new ProfinetIOConfig {Period = 4, BigEndian = endian};
            _changeCycle = MeasuredVariables.ProfinetProperty[1].IoData;
            try
            {
                KunbusRevolutionPiWrapper.piControlOpen();
                DeviceActive = true;
                _samplerThread = new Thread(DataAcquisition);
                _samplerThread.Start();

            }
            catch (BadImageFormatException exception)
            {
                Logger.Error("It seems like the application is not running on Kunbus Device... {0}", exception);
            }

            Logger.Trace("End of I/O read.");
        }

        private Measurement MeasuredVariables { get; }
        private MongoSaver Saver { get; }
        private bool DeviceActive { get; }

        ~KunbusIOModule()   
        {
            if (DeviceActive)
            {
                KunbusRevolutionPiWrapper.piControlClose();
                _samplerThread.Interrupt();
            }
            else
            {
                Logger.Warn("Application is not runnig on Kunbus Device...");
            }
        }

        private void DataAcquisition()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);
                
                if (!DeviceActive) continue;
                if (DataChange(_changeCycle))
                {
                    ReadVariablesFromInputs();
                }
            }
        }

        private bool DataChange(KunbusIOData kunbusIo)
        {
            var result = GetIntIo(kunbusIo);
            if (result == ChangeDetectionStatus)
            {
                return true;
            }
            return false;
        }

        private void ReadVariablesFromInputs()
        {

            ToSaveMeasurement = null;
            ToSaveMeasurement = new MongoVariables();
            var time = GetDataRobotTime().ToDataTime().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            var programNum = GetIntIo(MeasuredVariables.ProfinetProperty[0].IoData);

            ToSaveMeasurement.RobotTime = time;
            ToSaveMeasurement.SaveTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            ToSaveMeasurement.ProgramNumber = programNum;

            foreach (var variable in MeasuredVariables.Variables)
            {
                ReadVariableFromInputs(variable, false);
            }
            
            ToSaveMeasurement.Variables = MeasuredVariables.Variables;
            SaveThread = new Thread(() => Saver.SaveIOData(ToSaveMeasurement));
            SaveThread.Start();
        }

        private RobotTime GetDataRobotTime()
        {
            var robotTime = new RobotTime();
            foreach (var component in robotTime.CurrentTime)
            {
                component.Value = ReadKunbusInputs(component, true);
            }
            return robotTime;
        }

        private int GetIntIo(KunbusIOData kunbusIo)
        {
            return ReadKunbusInputs(kunbusIo);
        }

        private void ReadVariableFromInputs(MeasurementVariable variable, bool time)
        {
            foreach (var joint in variable.Joints)
            {
                joint.Value = ReadKunbusInputs(joint, time);
            }
        }

        private int ReadKunbusInputs(KunbusIOData kunbusIo)
        {
            var readData = new byte[kunbusIo.Length];
            var readBytes = KunbusRevolutionPiWrapper.piControlRead(kunbusIo.BytOffset,
                kunbusIo.Length,
                readData);

            if (_config.BigEndian ^ BitConverter.IsLittleEndian)
            {
                Array.Reverse(readData);
            }

            if (readBytes == kunbusIo.Length)
            {
                return (int) readData[0];
            }

            Logger.Warn("Hups... Somethink went wrong! No data were read.");
            throw new IOException();
        }


        private float ReadKunbusInputs(KunbusIOData kunbusIo, bool time)
        {
            var readData = new byte[kunbusIo.Length];
            var readBytes = KunbusRevolutionPiWrapper.piControlRead(kunbusIo.BytOffset,
                                                                    kunbusIo.Length,
                                                                    readData);

            if (_config.BigEndian ^ BitConverter.IsLittleEndian)
            {
                Array.Reverse(readData);
            }

            if (readBytes == kunbusIo.Length)
            {
                return time ? readData.OutputConversion(new uint()) : BitConverter.ToSingle(readData, 0);
            }

            Logger.Warn("Hups... Somethink went wrong! No data were read.");
            throw new IOException();
        }
    }
}