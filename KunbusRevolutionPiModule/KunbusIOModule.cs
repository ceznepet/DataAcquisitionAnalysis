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
using MarkovModule;
using Newtonsoft.Json;
using NLog;

namespace KunbusRevolutionPiModule
{
    public class KunbusIOModule
    {
        private KunbusIoVariables MeasuredVariables { get; }
        private MongoSaver Saver { get; }
        private bool DeviceActive { get; }
        private int[] Features { get; set; }
        private List<double[]> MeasurmentBatch { get; set; }
        private MarkovModel Markov { get; set; }
        private float EdgeDetection { get; set; }
        private static readonly Logger Logger = LogManager.GetLogger("Kunbus Thread");
        private readonly ProfinetIOConfig _config;
        private readonly VariableComponent _changeCycle;
        private readonly Thread _samplerThread;
        private Thread SaveThread;
        private Thread MarkovThread;
        private MeasuredVariables ToSaveMeasurement;        
        private List<VariableComponent> Time;
        private readonly uint ChangeDetectionStatus = 0;

        public KunbusIOModule(bool endian, string pathToConfiguration,
            string databaseLocation, string database, string document,
            int period)
        {       
            MeasuredVariables = JsonConvert.DeserializeObject<KunbusIoVariables>(File.ReadAllText(pathToConfiguration));
            Time = MeasuredVariables.Time;
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            _config = new ProfinetIOConfig {Period = period, BigEndian = endian};
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
        
        public KunbusIOModule(string pathToConfiguration, string pathToModels, bool endian, int period, int[] features)
        {
            MeasuredVariables = JsonConvert.DeserializeObject<KunbusIoVariables>(File.ReadAllText(pathToConfiguration));
            Markov = new MarkovModel(pathToModels);
            EdgeDetection = 0;
            Time = MeasuredVariables.Time;
            _config = new ProfinetIOConfig { Period = period, BigEndian = endian };
            _changeCycle = MeasuredVariables.ProfinetProperty[1];
            MeasurmentBatch = new List<double[]>();
            Features = features;
            try
            {
                KunbusRevolutionPiWrapper.piControlOpen();
                DeviceActive = true;
                _samplerThread = new Thread(LiveDataAcquisition);
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

        private void LiveDataAcquisition()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                if (!DeviceActive) continue;

                try
                {
                    ReadVariablesFromInputs();
                }
                catch (OutOfMemoryException exception)
                {
                    Logger.Error("Out of memory!");
                }
            }
        }

        private void DataAcquisition()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                if (!DeviceActive) continue;

                try
                {
                    ReadVariablesFromInputs();
                }
                catch (OutOfMemoryException exception)
                {
                    Logger.Error("Out of memory!");
                }
                
            }
        }

        private void ReadVariablesToBatch()
        {
            if(MeasuredVariables.ProfinetProperty[4].Value == EdgeDetection)
            {
                ToSaveMeasurement = null;
                ToSaveMeasurement = new MeasuredVariables();

                foreach (var variable in MeasuredVariables.Variables)
                {
                    ReadVariableFromInputs(variable, false);
                }

                var measuerement = MeasuredVariables.GetMeasuredValues().ToList();
                var tempArray = new List<double>();
                foreach (var position in Features)
                {
                    tempArray.Add(measuerement[position]);
                }
                MeasurmentBatch.Add(tempArray.ToArray());
            }
            else
            {
                MarkovThread = new Thread(() => Markov.DiscreteModel.OnlineDecide(MeasurmentBatch.ToArray()));
                MarkovThread.Start();
                MeasurmentBatch.Clear();
            }       
        }

        private void ReadVariablesFromInputs()
        {

            ToSaveMeasurement = null;
            ToSaveMeasurement = new MeasuredVariables();
            GetDataRobotTime();
            var time = Time.ToDateTime().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            var programNum = ReadKunbusInputs(MeasuredVariables.ProfinetProperty[0]);

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
            {
                if (component.Length > 1)
                {
                    component.Value = 0;
                    component.Value = (int) ReadKunbusInputs(component, true);
                }
                else
                {
                    component.Value = 0;
                    var value = ReadKunbusInputs(component);
                    component.Value = value;
                }
            }
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
                return time ? readData.OutputConversion(new ushort()) : BitConverter.ToSingle(readData, 0);
            }

            Logger.Warn("Hups... Somethink went wrong! No data were read.");
            throw new IOException();
        }
    }
}