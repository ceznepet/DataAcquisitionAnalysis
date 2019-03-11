using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Extensions;
using Common.Models;
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
        private KunbusIoVariables MeasuredVariables { get; }
        private MongoSaver Saver { get; }
        private bool DeviceActive { get; }
        private static readonly Logger Logger = LogManager.GetLogger("Kunbus Thread");
        private readonly ProfinetIOConfig _config;
        private readonly VariableComponent _changeCycle;
        private readonly Thread _samplerThread;
        private Thread SaveThread;
        private MeasuredVaribles ToSaveMeasurement;
        private List<VariableComponent> Time;
        private readonly uint ChangeDetectionStatus = 0;

        public KunbusIOModule(bool endian, string pathToConfiguration,
            string databaseLocation, string database, string document)
        {       
            MeasuredVariables = JsonConvert.DeserializeObject<KunbusIoVariables>(File.ReadAllText(pathToConfiguration));
            Time = MeasuredVariables.Time;
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            _config = new ProfinetIOConfig {Period = 4, BigEndian = endian};
            _changeCycle = MeasuredVariables.ProfinetProperty[1];
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

        private bool DataChange(VariableComponent kunbusIo)
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
            ToSaveMeasurement = new MeasuredVaribles();
            var time = Time.ToDateTime().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            var programNum = GetIntIo(MeasuredVariables.ProfinetProperty[0]);

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

        private void GetDataRobotTime()
        {
            foreach (var component in Time)
            foreach (var component in robotTime.CurrentTime)
            {
                component.Value = ReadKunbusInputs(component, true);
            }
            return robotTime;
        }

        private int GetIntIo(VariableComponent kunbusIo)
        {
            return ReadKunbusInputs(kunbusIo);
        }

        private void ReadVariableFromInputs(Variable variable, bool time)
        {
            foreach (var joint in variable.Joints)
            {
                joint.Value = ReadKunbusInputs(joint, time);
            }
        }

        private int ReadKunbusInputs(VariableComponent kunbusIo)
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
                return readData[0];
            }

            Logger.Warn("Hups... Somethink went wrong! No data were read.");
            throw new IOException();
        }


        private float ReadKunbusInputs(VariableComponent kunbusIo, bool time)
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