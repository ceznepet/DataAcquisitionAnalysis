using System;
using System.Collections.Generic;
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
        private readonly KunbusIOData _changeCycle = new KunbusIOData(28, "Change");
        private readonly Thread _samplerThread;
        private readonly uint ChangeDetectionStatus = 0;
        private Measurement MeasuredVariables { get; }
        private List<Measurement> BatchMeasurement { get; }        
        private MongoSaver Saver { get; }
        private bool DeviceActive { get; }

        public KunbusIOModule(bool endian, string pathToConfiguration,
            string databaseLocation, string database, string document)
        {
            MeasuredVariables = JsonConvert.DeserializeObject<Measurement>(File.ReadAllText(pathToConfiguration));
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            _config = new ProfinetIOConfig {Period = 4, BigEndian = endian};
            BatchMeasurement = new List<Measurement>();
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

                if (DataChange(_changeCycle))
                {
                    ReadVariablesFromInputs();
                }

                if (BatchMeasurement.Count != 100)
                {
                    continue;
                }

                var saveThread = new Thread(() => Saver.SaveBatchIoData((BatchMeasurement.AsEnumerable())));
                saveThread.IsBackground = true;
                saveThread.Start();
                BatchMeasurement.Clear();
            }
        }

        private bool DataChange(KunbusIOData kunbusIo)
        {
            var result = ReadKunbusInputs(kunbusIo, false);
            if (result == ChangeDetectionStatus)
            {
                return true;
            }
            return false;
        }

        private void ReadVariablesFromInputs()
        {
            var time = GetDataRobotTime().ToDataTime().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            MeasuredVariables.Time = time;
            foreach (var variable in MeasuredVariables.Variables)
            {
                ReadVariableFromInputs(variable, false);
            }
            BatchMeasurement.Add(MeasuredVariables);
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

        private void ReadVariableFromInputs(MeasurementVariable variable, bool time)
        {
            foreach (var joint in variable.Joints)
            {
                joint.Value = ReadKunbusInputs(joint, time);
            }
        }


        private float ReadKunbusInputs(KunbusIOData kunbusIo, bool time)
        {
            var readData = new byte[kunbusIo._length];
            var readBytes = KunbusRevolutionPiWrapper.piControlRead(kunbusIo.BytOffset,
                                                                    kunbusIo._length,
                                                                    readData);

            if (_config.BigEndian ^ BitConverter.IsLittleEndian)
            {
                Array.Reverse(readData);
            }

            if (readBytes == kunbusIo._length)
            {
                return time ? readData.OutputConversion(new uint()) : BitConverter.ToSingle(readData, 0);
            }

            Logger.Warn("Hups... Somethink went wrong! No data were read.");
            throw new IOException();
        }
    }
}