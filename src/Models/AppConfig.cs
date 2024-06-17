namespace Microsoft.Azure.SpaceFx.HostServices.Position;
public static class Models {
    public class APP_CONFIG : Core.APP_CONFIG {
        [Flags]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PluginPermissions {
            NONE = 0
        }

        public class PLUG_IN : Core.Models.PLUG_IN {
            [JsonConverter(typeof(JsonStringEnumConverter))]

            public PluginPermissions CALCULATED_PLUGIN_PERMISSIONS {
                get {
                    PluginPermissions result;
                    System.Enum.TryParse(PLUGIN_PERMISSIONS, out result);
                    return result;
                }
            }

            public PLUG_IN() {
                PLUGIN_PERMISSIONS = "";
                PROCESSING_ORDER = 100;
            }
        }
        public APP_CONFIG() : base() {
        }
    }
}