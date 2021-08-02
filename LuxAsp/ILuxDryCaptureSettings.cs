using Microsoft.Extensions.Configuration;

namespace LuxAsp
{
    public interface ILuxDryCaptureSettings
    {
        /// <summary>
        /// Capture Settings on the Configs object.
        /// </summary>
        /// <param name="Configs"></param>
        void CaptureSettings(IConfigurationBuilder Configs);
    }
}
