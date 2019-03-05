using System;
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
        private readonly ProfinetIOConfig _config;
        private readonly Thread _samplerThread;
        private Measurement MeasuredVariables { get; set; }
        private MongoSaver Saver { get; set; }
        private int NumberOfOutputs { get; set; }
        private int NumberOfBytes { get; set; }
        private bool DeviceActive { get; set; }
        private readonly uint ChangeDetectionStatus = 0;
        private readonly KunbusIOData _changeCycle = new KunbusIOData(28, "Change");

        private static readonly Logger Logger = LogManager.GetLogger("Kunbus Thread");

        public KunbusIOModule(int numberOfBytes, bool endian, string pathToConfiguration,
                            string databaseLocation, string database, string document)
        {
            NumberOfBytes = numberOfBytes;
            MeasuredVariables = JsonConvert.DeserializeObject<Measurement>(File.ReadAllText(pathToConfiguration));
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            NumberOfOutputs = MeasuredVariables.Variables.Count *
                              MeasuredVariables.Variables.First().Joints.Count * NumberOfBytes; // variables count * axis * byteField
            _config = new ProfinetIOConfig { Period = 4, BigEndian = endian };


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
            var time = GetDataRobotTime().ToDataTime();
            MeasuredVariables.Time = time;
            foreach (var variable in MeasuredVariables.Variables)
            {
                ReadVariableFromInputs(variable, false);
            }
            var toSave = MeasuredVariables;
            Saver.SaveIOData(toSave);
        }

        private RobotTime GetDataRobotTime()
        {
            var roboTime = new RobotTime();
            foreach (var commponent in roboTime.CurrentTime)
            {
                var valeu = ReadKunbusInputs(commponent, true);
                commponent.Value = valeu;

            }
            return roboTime;
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
            else
            {
                Logger.Warn("Hups... Somethink went wrong! No data were read.");
                throw new IOException();
            }
        }

    }
}