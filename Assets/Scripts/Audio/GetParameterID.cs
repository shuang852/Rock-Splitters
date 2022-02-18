using FMOD.Studio;

namespace Audio
{
    public class GetParameterID
    {
        public static PARAMETER_ID GetParameterIDByName(EventInstance instance, string paramName)
        {
            instance.getDescription(out var eventDescription);
            eventDescription.getParameterDescriptionByName(paramName, out var paramDescription);
            return paramDescription.id;
        }
    }
}