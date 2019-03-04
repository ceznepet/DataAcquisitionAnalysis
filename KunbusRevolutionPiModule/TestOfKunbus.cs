﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Robot;
using KunbusRevolutionPiModule.Wrapper;
using Newtonsoft.Json;

namespace KunbusRevolutionPiModule
{
    public class TestOfKunbus
    {
        private readonly ProfinetIOConfig _config;
        private readonly bool deviceActive = true;
        private readonly Thread _samplerThread;        
        private Measurement MeasuredVariables { get; set; }
        private int NumberOfOutputs { get; set; }
        private int NumberOfBytes { get; set; }
        private MongoSaver Saver { get; }

        public TestOfKunbus(int numberOfBytes, bool endian, string pathToConfiguration,
                            string databaseLocation, string database, string document)
        {
            NumberOfBytes = numberOfBytes;
            MeasuredVariables = JsonConvert.DeserializeObject<Measurement>(File.ReadAllText(pathToConfiguration));
            Saver = MongoDbCall.GetSaverToMongoDb(databaseLocation, database, document);
            NumberOfOutputs = MeasuredVariables.Variables.Count *
                              MeasuredVariables.Variables.First().Joints.Count * NumberOfBytes; // variables count * axis * byteField
            _config = new ProfinetIOConfig {Period = 12, BigEndian = endian};            

            

            KunbusRevolutionPiWrapper.piControlOpen();
            _samplerThread = new Thread(GatherData);
            _samplerThread.Start();
        }

        ~TestOfKunbus()
        {
            KunbusRevolutionPiWrapper.piControlClose();
            _samplerThread.Interrupt();
        }

        private void GatherData()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                if (!deviceActive) continue;
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
                    Console.WriteLine("Data has been read! All of them: {0}", NumberOfOutputs);
                }
                else
                {
                    Console.WriteLine("Hups... Somethink went wrong! No data were read.");
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
                Console.WriteLine("Hups... Somethink went wrong! No data were read.");
                throw new IOException();
            }
        }

    }

}