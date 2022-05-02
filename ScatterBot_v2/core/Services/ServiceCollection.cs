using ScatterBot_v2.Serialization;

namespace ScatterBot_v2.core.Services
{
    /// <summary>
    /// Collection of services for easy access.
    /// </summary>
    public class ServiceCollection
    {
        public NewUserHelperService NewUserHelperService;
        public MuteHelperService MuteHelperService;
        public PinHelperService PinHelperService;
        public SaveSystem SaveSystem;
        public ApplicationService ApplicationService;
        public MemberModerationService MemberModerationService;
    }
}