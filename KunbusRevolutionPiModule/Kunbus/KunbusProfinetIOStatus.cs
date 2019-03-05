namespace KunbusRevolutionPiModule.Kunbus
{
    public enum KunbusProfinetIOStatus
    {
        OK = 1,
        HWCONFIG_ERR = -1,
        ADAPTER_NOT_FOUND = -2,
        CANT_STARTUP = -3,
        CANT_OPEN = -4,
        CANT_REGISTER_CBK = -5,
        CANT_SET_OPERATE = -6,
        CANT_SET_OFFLINE = -7,
        CANT_CLOSE = -8,
        CANT_SHUTDOWN = -9,
        CANT_WRITE = -10,
        CANT_READ = -11,
        CANT_INIT = -12
    }
}
