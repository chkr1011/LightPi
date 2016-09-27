namespace LightPi.OrchestratorFirmware.Devices
{
    internal interface IOutputDevice
    {
        void Initialize();
        void WriteState(byte[] buffer);
    }
}
