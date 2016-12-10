using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public interface ISettingsService
    {
        Settings Settings { get; }

        List<ProfileViewModel> ProfileViewModels { get; }

        List<OutputViewModel> OutputViewModels { get; }

        void SaveSettings();

        void SaveProfile(ProfileViewModel profile);
    }
}